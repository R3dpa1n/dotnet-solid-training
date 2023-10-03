using DevBasics.CarManagement.Dependencies;
using System.Threading.Tasks;

namespace DevBasics.CarManagement
{
    public interface ILeasingRegistrationRepository
    {
        Task<AppSettingDto> GetAppSettingAsync(string salesOrgIdentifier, CarBrand requestOrigin);
        Task<int> InsertHistoryAsync(CarRegistrationDto dbCar, string userName, string transactionStateName = null, string transactionTypeName = null);
        Task<bool> UpdateCarAsync(CarRegistrationDto dbCar);
    }
}
