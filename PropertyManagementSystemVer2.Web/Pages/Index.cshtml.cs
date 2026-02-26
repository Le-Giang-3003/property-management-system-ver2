using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PropertyManagementSystemVer2.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToPage("/Account/Login");
            }

            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (role == "Admin") return RedirectToPage("/Admin/Dashboard");
            if (role == "Landlord") return RedirectToPage("/Landlord/Dashboard");
            
            return RedirectToPage("/Tenant/Dashboard");
        }
    }
}
