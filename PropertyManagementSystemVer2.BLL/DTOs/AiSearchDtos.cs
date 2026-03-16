namespace PropertyManagementSystemVer2.BLL.DTOs
{
    /// <summary>
    /// Request DTO – nhu cầu người dùng nhập bằng ngôn ngữ tự nhiên
    /// </summary>
    public class AiSearchRequestDto
    {
        public string UserQuery { get; set; } = string.Empty;
    }

    /// <summary>
    /// Filter được AI trích xuất từ câu hỏi tự nhiên
    /// </summary>
    public class AiParsedFilterDto
    {
        public string? City { get; set; }
        public string? District { get; set; }

        /// <summary>Apartment | House | Room | Villa | Office | null</summary>
        public string? PropertyType { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinBedrooms { get; set; }
        public int? MinArea { get; set; }
        public bool? AllowPets { get; set; }
        public bool? AllowSmoking { get; set; }

        /// <summary>AI tóm tắt nhu cầu bằng tiếng Việt ngắn gọn (hiển thị cho user)</summary>
        public string? SummaryVi { get; set; }
    }

    /// <summary>
    /// Kết quả tổng hợp: filter AI trích xuất + danh sách bất động sản phù hợp
    /// </summary>
    public class AiSearchResultDto
    {
        public AiParsedFilterDto ParsedFilter { get; set; } = new();
        public PagedResultDto<PropertyListDto> Properties { get; set; } = new();
    }
}
