using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PropertyManagementSystemVer2.Web.Pages.Shared
{
    [Authorize]
    public class ChatModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
