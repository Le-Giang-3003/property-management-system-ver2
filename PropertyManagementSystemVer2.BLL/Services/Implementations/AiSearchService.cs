using Microsoft.Extensions.Options;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.BLL.Settings;
using PropertyManagementSystemVer2.DAL.Enums;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class AiSearchService : IAiSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _settings;
        private readonly IPropertyService _propertyService;

        // System prompt yêu cầu Gemini trả về JSON thuần túy
        private const string SystemPrompt = """
            Bạn là AI hỗ trợ tìm kiếm bất động sản tại Việt Nam.
            Phân tích câu yêu cầu của người dùng và trả về JSON hợp lệ (KHÔNG kèm markdown, KHÔNG có ```json).
            Chỉ trả về object JSON với cấu trúc sau, điền null nếu thông tin không được đề cập:
            {
              "City": "tên thành phố bằng tiếng Việt hoặc null",
              "District": "tên quận/huyện hoặc null",
              "PropertyType": "Apartment hoặc House hoặc Room hoặc Villa hoặc Office hoặc null",
              "MinPrice": số_nguyên_VND_hoặc_null,
              "MaxPrice": số_nguyên_VND_hoặc_null,
              "MinBedrooms": số_nguyên_hoặc_null,
              "MinArea": số_nguyên_m2_hoặc_null,
              "AllowPets": true_hoặc_false_hoặc_null,
              "AllowSmoking": true_hoặc_false_hoặc_null,
              "SummaryVi": "tóm tắt nhu cầu bằng tiếng Việt, ngắn gọn 1 câu"
            }
            Lưu ý: Nếu người dùng nói "triệu" thì nhân với 1000000. Nếu nói "nghìn" thì nhân với 1000.
            """;

        public AiSearchService(HttpClient httpClient, IOptions<GeminiSettings> settings, IPropertyService propertyService)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _propertyService = propertyService;
        }

        public async Task<ServiceResultDto<AiParsedFilterDto>> ParseUserQueryAsync(string userQuery)
        {
            if (string.IsNullOrWhiteSpace(userQuery))
                return ServiceResultDto<AiParsedFilterDto>.Failure("Vui lòng nhập nhu cầu tìm kiếm.");

            if (string.IsNullOrWhiteSpace(_settings.ApiKey) || _settings.ApiKey == "YOUR_GEMINI_API_KEY_HERE")
                return ServiceResultDto<AiParsedFilterDto>.Failure("Gemini API key chưa được cấu hình.");

            try
            {
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.ModelName}:generateContent?key={_settings.ApiKey}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = SystemPrompt + "\n\nCâu yêu cầu của người dùng: \"" + userQuery + "\"" }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.1,
                        maxOutputTokens = 500
                    }
                };

                var response = await _httpClient.PostAsJsonAsync(url, requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return ServiceResultDto<AiParsedFilterDto>.Failure($"Lỗi kết nối Gemini API: {response.StatusCode}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var geminiResponse = JsonDocument.Parse(responseJson);

                // Trích xuất text từ Gemini response
                var text = geminiResponse
                    .RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "";

                // Làm sạch text (đôi khi Gemini trả về có ```json wrapper)
                text = text.Trim();
                if (text.StartsWith("```"))
                {
                    var lines = text.Split('\n');
                    text = string.Join('\n', lines.Skip(1).Take(lines.Length - 2));
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var parsed = JsonSerializer.Deserialize<AiParsedFilterDto>(text, options);
                if (parsed == null)
                    return ServiceResultDto<AiParsedFilterDto>.Failure("Không thể phân tích nhu cầu tìm kiếm.");

                return ServiceResultDto<AiParsedFilterDto>.Success(parsed);
            }
            catch (Exception ex)
            {
                return ServiceResultDto<AiParsedFilterDto>.Failure($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public async Task<ServiceResultDto<AiSearchResultDto>> SearchPropertiesAsync(string userQuery, int page = 1, int pageSize = 12)
        {
            // Bước 1: Phân tích nhu cầu bằng AI
            var parseResult = await ParseUserQueryAsync(userQuery);
            if (!parseResult.IsSuccess || parseResult.Data == null)
                return ServiceResultDto<AiSearchResultDto>.Failure(parseResult.Message);

            var filter = parseResult.Data;

            // Bước 2: Map AI filter → PropertySearchDto (DTO có sẵn)
            var searchDto = new PropertySearchDto
            {
                City = filter.City,
                District = filter.District,
                Status = PropertyStatus.Available, // Chỉ tìm bất động sản đang cho thuê
                MinPrice = filter.MinPrice,
                MaxPrice = filter.MaxPrice,
                MinBedrooms = filter.MinBedrooms,
                MinArea = filter.MinArea,
                PageNumber = page,
                PageSize = pageSize
            };

            // Map PropertyType từ string sang enum
            if (!string.IsNullOrWhiteSpace(filter.PropertyType) &&
                Enum.TryParse<PropertyType>(filter.PropertyType, ignoreCase: true, out var propertyType))
            {
                searchDto.PropertyType = propertyType;
            }

            // Bước 3: Query database
            var searchResult = await _propertyService.SearchPropertiesAsync(searchDto);
            if (!searchResult.IsSuccess || searchResult.Data == null)
                return ServiceResultDto<AiSearchResultDto>.Failure(searchResult.Message);

            // Bước 4: Lọc thêm AllowPets / AllowSmoking (nếu DB không hỗ trợ filter này)
            var properties = searchResult.Data.Items;
            if (filter.AllowPets == true)
                properties = properties.Where(p => p.AllowPets).ToList();
            if (filter.AllowSmoking == true)
                properties = properties.Where(p => p.AllowSmoking).ToList();

            var result = new AiSearchResultDto
            {
                ParsedFilter = filter,
                Properties = new PagedResultDto<PropertyListDto>
                {
                    Items = properties,
                    TotalCount = filter.AllowPets == true || filter.AllowSmoking == true
                        ? properties.Count
                        : searchResult.Data.TotalCount,
                    PageNumber = searchResult.Data.PageNumber,
                    PageSize = searchResult.Data.PageSize
                }
            };

            return ServiceResultDto<AiSearchResultDto>.Success(result);
        }
    }
}
