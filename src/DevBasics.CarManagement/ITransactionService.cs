using DevBasics.CarManagement.Dependencies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBasics.CarManagement
{
	public interface ITransactionService
	{
		Task<string> BeginTransactionAsync(ICarRegistrationRepository CarLeasingRepository, ILeasingRegistrationRepository LeasingRegistrationRepository, IList<string> cars, string customerId, string companyId, RegistrationType registrationType, string identity, string transactionId = null, string registrationNumber = null);
		Task<IList<int>> FinishTransactionAsync(ICarRegistrationRepository CarLeasingRepository, RegistrationType registrationType, BulkRegistrationResponse apiResponse, IList<string> carIdentifier, string companyId, string identity, string transactionStateBackup = null, BulkRegistrationRequest requestModel = null);
	}
}