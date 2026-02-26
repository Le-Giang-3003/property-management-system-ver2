using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.BLL.Settings;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string?> AddPhotoAsync(Stream fileStream, string fileName)
        {
            if (fileStream == null || fileStream.Length == 0) return null;

            var uploadResult = new ImageUploadResult();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                Console.WriteLine($"Cloudinary Upload Error: {uploadResult.Error.Message}");
                return null;
            }

            return uploadResult.SecureUrl?.ToString();
        }
    }
}
