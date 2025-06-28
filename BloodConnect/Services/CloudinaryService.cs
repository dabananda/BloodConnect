using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;

namespace BloodConnect.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "bloodconnect/profile_pics",
                    Transformation = new Transformation().Width(300).Height(300).Crop("fill").Gravity("face")
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }

            return null;
        }
    }
}
