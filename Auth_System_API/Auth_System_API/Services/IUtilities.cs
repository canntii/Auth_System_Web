namespace Auth_System_API.Services
{
    public interface IUtilities
    {

        Task<String>? UploadImage(string image64);
    }
}
