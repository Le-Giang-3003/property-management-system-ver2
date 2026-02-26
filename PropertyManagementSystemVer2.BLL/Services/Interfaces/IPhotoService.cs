using System.IO;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IPhotoService
    {
        Task<string?> AddPhotoAsync(Stream fileStream, string fileName);
    }
}
