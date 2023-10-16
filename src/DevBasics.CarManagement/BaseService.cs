using DevBasics.CarManagement.Dependencies;
using DevBasics.CarManagement.Exceptions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DevBasics.CarManagement
{
	public class BaseService
    {
        public CarManagementSettings Settings { get; set; }

        public HttpHeaderSettings HttpHeader { get; set; }

        public IBulkRegistrationService BulkRegistrationService { get; set; }

        public ILeasingRegistrationRepository LeasingRegistrationRepository { get; set; }

        public ICarRegistrationRepository CarLeasingRepository { get; set; }

        public BaseService(
            CarManagementSettings settings,
            HttpHeaderSettings httpHeader,
            IBulkRegistrationService bulkRegistrationService = null,
            ILeasingRegistrationRepository leasingRegistrationRepository = null,
            ICarRegistrationRepository carLeasingRepository = null)
        {
            Settings = settings;
            HttpHeader = httpHeader;
            BulkRegistrationService = bulkRegistrationService;
            LeasingRegistrationRepository = leasingRegistrationRepository;
            CarLeasingRepository = carLeasingRepository;
        }

        public async Task<RequestContext> InitializeRequestContextAsync()
        {
            Console.WriteLine("Trying to initialize request context...");

            try
            {
                AppSettingDto settingResult = await LeasingRegistrationRepository.GetAppSettingAsync(HttpHeader.SalesOrgIdentifier, HttpHeader.WebAppType);

                if (settingResult == null)
                {
					throw new AppSettingException("Error while retrieving settings from database");
                }

                RequestContext requestContext = new RequestContext()
                {
                    ShipTo = settingResult.SoldTo,
                    LanguageCode = Settings.LanguageCodes["English"],
                    TimeZone = "Europe/Berlin"
                };

                Console.WriteLine($"Initializing request context successful. Data (serialized as JSON): {JsonConvert.SerializeObject(requestContext)}");

                return requestContext;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initializing request context failed: {ex}");
                return null;
            }
        }

		// See Feature 182.
		public static CarRegistrationModel AddDeliveryDate(CarRegistrationModel car)
		{
			if (car.DeliveryDate == null)
			{
				DateTime today = DateTime.Now.Date;
				DateTime delivery = today.AddDays(-1);
				car.DeliveryDate = delivery;
			}

			return car;
		}

        public static CarRegistrationModel AddErpDeliveryNumber(CarRegistrationModel car, string registrationId)
        {
			// See Feature 182.
			if (string.IsNullOrWhiteSpace(car.ErpDeliveryNumber))
			{
				car.ErpDeliveryNumber = registrationId;

				Console.WriteLine($"Car {car.VehicleIdentificationNumber} has no value for Delivery Number: Setting default value to registration id {registrationId}");
			}

            return car;
		}
    }
}
