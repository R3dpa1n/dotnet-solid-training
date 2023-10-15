using DevBasics.CarManagement.Dependencies;

namespace DevBasics.CarManagement.Helper
{
	public class CarDataHelper : ICarDataHelper
	{
		public bool HasMissingData(CarRegistrationModel car)
		{
			return string.IsNullOrWhiteSpace(car.CompanyId)
				|| string.IsNullOrWhiteSpace(car.VehicleIdentificationNumber)
				|| string.IsNullOrWhiteSpace(car.CustomerId)
				|| car.DeliveryDate == null
				|| string.IsNullOrWhiteSpace(car.ErpDeliveryNumber);
		}
	}
}
