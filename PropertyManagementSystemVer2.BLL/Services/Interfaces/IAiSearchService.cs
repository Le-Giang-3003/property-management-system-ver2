using PropertyManagementSystemVer2.BLL.DTOs;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IAiSearchService
    {
        /// <summary>
        /// Gọi Gemini AI để phân tích câu hỏi người dùng, trả về filter đã trích xuất
        /// </summary>
        Task<ServiceResultDto<AiParsedFilterDto>> ParseUserQueryAsync(string userQuery);

        /// <summary>
        /// Phân tích câu hỏi → trích xuất filter → query database → trả kết quả tổng hợp
        /// </summary>
        Task<ServiceResultDto<AiSearchResultDto>> SearchPropertiesAsync(string userQuery, int page = 1, int pageSize = 12);
    }
}
