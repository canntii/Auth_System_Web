using Auth_System_Web.Entities;

namespace Auth_System_Web.Services
{
    public interface IScheduleModel
    {

        Answer? RegisterSchedule(string location);
    }
}
