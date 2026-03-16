using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using System.Security.Claims;

namespace PropertyManagementSystemVer2.Web.Pages.Tenant
{
    [Authorize]
    public class AiSearchModel : PageModel
    {
        private readonly IAiSearchService _aiSearchService;

        public AiSearchModel(IAiSearchService aiSearchService)
        {
            _aiSearchService = aiSearchService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Query { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public AiSearchResultDto? SearchResult { get; set; }
        public string? ErrorMessage { get; set; }
        public bool HasSearched { get; set; } = false;

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrWhiteSpace(Query))
            {
                HasSearched = true;
                var result = await _aiSearchService.SearchPropertiesAsync(Query, CurrentPage, 12);
                if (result.IsSuccess)
                    SearchResult = result.Data;
                else
                    ErrorMessage = result.Errors.FirstOrDefault() ?? "Có lỗi xảy ra khi tìm kiếm.";
            }
        }
    }
}
