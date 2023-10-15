using DevBasics.CarManagement.Dependencies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBasics.CarManagement.Helper
{
	public interface IRegistrationHelper
	{
		Task<ServiceResult> ForceBulkRegistration(ICarRegistrationRepository CarLeasingRepository, IList<CarRegistrationModel> forceItems, string identity);
	}
}