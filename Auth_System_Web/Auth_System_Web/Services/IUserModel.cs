using Auth_System_Web.Entities;

namespace Auth_System_Web.Services
{
    public interface IUserModel
    {
        Answer? RegisterUser(User entity);

        Task<UserAnswer> GetUserInformation(string firebaseToken);

        Task<UserCompareAnswer> CompareFaces(string webcam);
    }
}
