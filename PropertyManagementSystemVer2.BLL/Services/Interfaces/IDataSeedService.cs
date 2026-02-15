namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    /// <summary>
    /// Service seed dữ liệu ban đầu (admin, role,...). Chỉ seed khi chưa có dữ liệu (idempotent).
    /// </summary>
    public interface IDataSeedService
    {
        /// <summary>
        /// Đảm bảo dữ liệu seed đã có: nếu chưa có Admin thì tạo một tài khoản Admin mặc định.
        /// </summary>
        Task EnsureSeedDataAsync(CancellationToken ct = default);
    }
}
