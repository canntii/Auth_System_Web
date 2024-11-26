using Auth_System_API.Services;
using Auth_System_Web.Entities;
using Google.Cloud.Storage.V1;
namespace Auth_System_API.Models
{
    public class Utilities(IConfiguration _configuration) : IUtilities
    {
        private string _bucketName = _configuration.GetSection("settings:FirebaseStorage").Value ?? string.Empty;


        public async Task<String>? UploadImage(string image64)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(image64);

                var fileName = $"{Guid.NewGuid()}_user_image.png";
                var storageClient = StorageClient.Create();

                using (var stream = new MemoryStream(imageBytes))
                {
                    // Sube el objeto a Firebase Storage
                    var storageObject = await storageClient.UploadObjectAsync(
                        _bucketName,    // Nombre del bucket
                        fileName,       // Nombre del archivo
                        "image/png",    // Tipo de contenido
                        stream,
                        new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead }
                    );
                }

                // Genera la URL pública de la imagen
                var fileUrl = $"https://storage.googleapis.com/{_bucketName}/{Uri.EscapeDataString(fileName)}";

                // Retorna la URL en la respuesta
                return fileUrl;
                

            }
            catch (Exception ex)
            {
                return null;

            }

        }

    }
}
