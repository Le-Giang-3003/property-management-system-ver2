using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PropertyManagementSystemVer2.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class MaintenanceModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
