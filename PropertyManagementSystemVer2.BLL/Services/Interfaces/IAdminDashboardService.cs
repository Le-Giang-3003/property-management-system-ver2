
using PropertyManagementSystemVer2.BLL.DTOs.Admin;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetDashboardAsync();
    }
}
