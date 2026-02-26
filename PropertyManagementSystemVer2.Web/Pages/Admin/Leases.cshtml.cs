using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PropertyManagementSystemVer2.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class LeasesModel : PageModel
    {
        public void OnGet()
        {
            // Future implementation: Fetch all leases paginated for admin
        }
    }
}
