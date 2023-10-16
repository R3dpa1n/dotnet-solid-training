using AutoMapper;
using DevBasics.CarManagement.Dependencies;
using DevBasics.CarManagement.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Transactions;
using static DevBasics.CarManagement.Dependencies.RegistrationApiResponseBase;

namespace DevBasics.CarManagement
{
	public class RegisterCarService : BaseService, IRegisterCarService
	{
		private readonly ICarDataHelper carDataHelper;
		private readonly IRegistrationHelper registrationHelper;
		private readonly ITransactionService transactionService;

		public RegisterCarService(
			CarManagementSettings settings,
			HttpHeaderSettings httpHeader,
			IBulkRegistrationService bulkRegisterService,
			ILeasingRegistrationRepository registrationRepository,
			ICarRegistrationRepository carRegistrationRepository,
			ICarDataHelper carDataHelper,
			IRegistrationHelper registrationHelper,
			ITransactionService transactionService)
				: base(
					  settings,
					  httpHeader,
					  bulkRegisterService,
					  registrationRepository,
					  carRegistrationRepository)
		{
			Console.WriteLine($"Initializing service {nameof(RegisterCarService)}");

			this.carDataHelper = carDataHelper;
			this.registrationHelper = registrationHelper;
			this.transactionService = transactionService;
		}

		public async Task<ServiceResult> RegisterCarsAsync(RegisterCarsModel registerCarsModel, bool isForcedRegistration, Claims claims, string identity = "Unknown")
		{
			ServiceResult serviceResult = new ServiceResult();

			try
			{
				// See Feature 307.
				registerCarsModel.Cars.ToList().ForEach(x =>
				{
					if (!string.IsNullOrWhiteSpace(x.VehicleIdentificationNumber))
					{
						x.VehicleIdentificationNumber = x.VehicleIdentificationNumber.ToUpper();
					}
				});

				registerCarsModel.Cars = registerCarsModel.Cars.RemoveDuplicates();

				Console.WriteLine($"Trying to invoke initial bulk registration for {registerCarsModel.Cars.Count} cars. " +
					$"Cars: {string.Join(", ", registerCarsModel.Cars.Select(x => x.VehicleIdentificationNumber))}, " +
					$"Is forced registration: {isForcedRegistration}");

				if (isForcedRegistration && !registerCarsModel.DeactivateAutoRegistrationProcessing)
				{
					List<CarRegistrationModel> existingItems = registerCarsModel.Cars.Where(x => x.IsExistingVehicleInAzureDB).ToList();
					List<CarRegistrationModel> notExistingItems = registerCarsModel.Cars.Where(x => !x.IsExistingVehicleInAzureDB).ToList();

					ServiceResult forceResponse = await registrationHelper.ForceBulkRegistration(CarLeasingRepository, existingItems, "Force Registerment User");

					if (forceResponse.Message.Contains("ERROR") || notExistingItems.Count == 0)
					{
						return forceResponse;
					}
					else
					{
						registerCarsModel.Cars = notExistingItems;
					}
				}

				CarPoolNumberHelper.Generate(
					CarBrand.Toyota,
					registerCarsModel.Cars.FirstOrDefault()!.CarPool,
					out string registrationId,
					out string carPoolNumber);

				Console.WriteLine($"Created unique car pool number {carPoolNumber} and registration id {registrationId}");

				foreach (CarRegistrationModel car in registerCarsModel.Cars)
				{
					car.CarPoolNumber = carPoolNumber;
					car.RegistrationId = registrationId;

					// See Bug 281.
					if (string.IsNullOrWhiteSpace(car.ErpRegistrationNumber))
					{
						AddDeliveryDate(car);

						AddErpDeliveryNumber(car, registrationId);
					}

					bool hasMissingData = carDataHelper.HasMissingData(car);
					if (hasMissingData)
					{
						Console.WriteLine($"Car {car.VehicleIdentificationNumber} has missing data. " +
							$"Set to transaction status {TransactionResult.MissingData.ToString()}");

						car.TransactionState = TransactionResult.MissingData.ToString("D");
					}
				}

				registerCarsModel.VendorId = registerCarsModel.Cars.Select(x => x.CompanyId).FirstOrDefault();
				registerCarsModel.CompanyId = registerCarsModel.VendorId;
				registerCarsModel.CustomerId = registerCarsModel.Cars.FirstOrDefault()!.CustomerId;

				RegisterCarsResult registrationResult = await new RegistrationService().SaveRegistrations(
					registerCarsModel, claims, registrationId, identity, isForcedRegistration, CarBrand.Toyota);

				if (registerCarsModel.Cars.Any(x => x.TransactionState == TransactionResult.MissingData.ToString("D"))
						&& !registerCarsModel.Cars.All(x => x.TransactionState == TransactionResult.MissingData.ToString("D")))
				{
					registerCarsModel.Cars = registerCarsModel
						.Cars
						.Where(x => x.TransactionState != TransactionResult.MissingData.ToString("D"))
						.ToList();
				}

				if (registrationResult.AlreadyRegistered)
				{
					serviceResult.Message = TransactionHelper.ALREADY_ENROLLED;
					return serviceResult;
				}

				if (registrationResult.RegisteredCars != null && registrationResult.RegisteredCars.Count > 0)
				{
					Console.WriteLine(
						$"Registering {registrationResult.RegisteredCars.Count} cars for registration with id {registrationResult.RegistrationId}. " +
						$"(RegistrationId = {registrationId})");

					bool hasMissingData = carDataHelper.HasMissingData(registerCarsModel.Cars.FirstOrDefault());

					string transactionId = await BeginTransactionGenerateId(
											registerCarsModel.Cars.Select(x => x.VehicleIdentificationNumber).ToList(),
											registerCarsModel.CustomerId,
											registerCarsModel.CompanyId,
											RegistrationType.Register,
											identity);

					if (!hasMissingData)
					{
						serviceResult = ExecuteBulkRegistration(registerCarsModel, transactionId, registrationResult, registrationId, identity, serviceResult).Result;
					}
					else
					{
						Console.WriteLine($"Car has missing data. Trying to set transaction status to {TransactionResult.MissingData}");

						await UpdateRegisteredCarsWithUpdatedData(registerCarsModel, identity);

						serviceResult.RegistrationId = registrationResult.RegistrationId;
						serviceResult.Message = TransactionResult.MissingData.ToString();

						Console.WriteLine($"Processing of bulk registration ended. Return data (serialized as JSON): {JsonConvert.SerializeObject(serviceResult)}");

						return serviceResult;
					}
				}
				else
				{
					Console.WriteLine(
						$"Nothing to do, the list of cars to register is empty! Returning empty result with HTTP 200. " +
						$"(RegistrationId = {registrationResult.RegistrationId})");

					// ANNAHME: Laut Message darüber ist nichts zu tun, => der nachfolgende Code kann gelöscht werden!

					////IEnumerable<IGrouping<string, CarRegistrationModel>> group = registerCarsModel.Cars.GroupBy(x => x.RegistrationId);

					////foreach (IGrouping<string, CarRegistrationModel> grp in group)
					////{
					////	IList<CarRegistrationModel> dbApiCars = await CarLeasingRepository.GetApiRegisteredCarsAsync(grp.Key);

					////	foreach (CarRegistrationModel dbApiCar in dbApiCars)
					////	{
					////		CarRegistrationDto dbCar = new CarRegistrationDto
					////		{
					////			RegistrationId = dbApiCar.RegistrationId
					////		};

					////		bool hasMissingData = carDataHelper.HasMissingData(dbApiCar);
					////		if (registerCarsModel.DeactivateAutoRegistrationProcessing && !hasMissingData)
					////		{
					////			Console.WriteLine(
					////				$"Automatic registration is deactivated (value = {registerCarsModel.DeactivateAutoRegistrationProcessing})" +
					////				$"and contains all relevant data (HasMissingData = {hasMissingData}). " +
					////				$"Set the transaction status of car {dbApiCar.VehicleIdentificationNumber} to {TransactionResult.ActionRequired.ToString()}" +
					////				$"Car (serialized as JSON): {dbCar}");

					////			dbCar.TransactionState = (int?)TransactionResult.ActionRequired;
					////			uiResponseStatusMsg = ApiResult.WARNING.ToString();
					////		}
					////		else
					////		{
					////			Console.WriteLine(
					////				$"Automatic registration is activated (value = {registerCarsModel.DeactivateAutoRegistrationProcessing}) " +
					////				$"or car doesn't contain all relevant data (HasMissingData = {hasMissingData}) or both. " +
					////				$"Set the transaction status of car {dbApiCar.VehicleIdentificationNumber} to {TransactionResult.MissingData.ToString()}. " +
					////				$"Car (serialized as JSON): {dbCar}");

					////			dbCar.TransactionState = (int?)TransactionResult.MissingData;
					////			uiResponseStatusMsg = TransactionResult.MissingData.ToString();
					////		}

					////		await new CarRegistrationRepository(LeasingRegistrationRepository, BulkRegistrationService, _mapper).UpdateRegisteredCarAsync(dbCar, identity);
					////	}
					////}

					serviceResult.RegistrationId = registrationId;
				}

				return serviceResult;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error saving registration for {CarBrand.Toyota} application: {ex}");
				throw;
			}
		}

		private async Task UpdateRegisteredCarsWithUpdatedData(RegisterCarsModel registerCarsModel, string identity)
		{
			IEnumerable<IGrouping<string, CarRegistrationModel>> group = registerCarsModel.Cars.GroupBy(x => x.RegistrationId);

			foreach (IGrouping<string, CarRegistrationModel> grp in group)
			{
				IList<CarRegistrationModel> dbApiCars = await CarLeasingRepository.GetApiRegisteredCarsAsync(grp.Key);
				foreach (CarRegistrationModel dbApiCar in dbApiCars)
				{
					CarRegistrationDto dbCar = new CarRegistrationDto
					{
						RegistrationId = dbApiCar.RegistrationId
					};

					dbCar.TransactionState = (int?)TransactionResult.MissingData;
					await CarLeasingRepository.UpdateRegisteredCarAsync(dbCar, identity);

					Console.WriteLine($"Updated car {dbApiCar.VehicleIdentificationNumber} to database. " +
						$"Car (serialized as JSON): {JsonConvert.SerializeObject(dbApiCar)}");
				}
			}
		}

		private async Task<ServiceResult> ExecuteBulkRegistration(RegisterCarsModel registerCarsModel, string transactionId, RegisterCarsResult registrationResult, string registrationId, string identity, ServiceResult serviceResult)
		{
			BulkRegistrationRequest requestPayload = null;
			BulkRegistrationResponse apiTransactionResult = null;
			try
			{
				requestPayload = await MapToModel(RegistrationType.Register, registerCarsModel, transactionId);
				apiTransactionResult = await BulkRegistrationService.ExecuteRegistrationAsync(requestPayload);
			}
			catch (Exception ex)
			{
				Console.WriteLine(
					$"Registering cars for registration with id {registrationResult.RegistrationId} (RegistrationId = {registrationId}) failed. " +
					$"Database transaction will be finished anyway: {ex}");
			}

			IList<int> identifier = await transactionService.FinishTransactionAsync(CarLeasingRepository, RegistrationType.Register,
				apiTransactionResult,
				registrationResult.RegisteredCars,
				registerCarsModel.CompanyId,
				identity);

			// Mapping to model that is excpected by the UI.
			return MapToModel(serviceResult,
				apiTransactionResult,
				requestPayload?.TransactionId,
				identifier,
				registrationId);
		}

		private async Task<BulkRegistrationRequest> MapToModel(RegistrationType registrationType, RegisterCarsModel cars, string transactionId)
		{
			BulkRegistrationRequest requestModel = new BulkRegistrationRequest();
			List<DeliveryRequest> deliveryModels = new List<DeliveryRequest>();

			try
			{
				requestModel.RequestContext = await base.InitializeRequestContextAsync();

				requestModel.TransactionId = transactionId;
				requestModel.CompanyId = cars.CompanyId;
				requestModel.Registrations = new List<RegistrationRequest>();

				IEnumerable<IGrouping<string, CarRegistrationModel>> groups = cars.Cars.GroupBy(x => x.CarPoolNumber);

				foreach (IGrouping<string, CarRegistrationModel> registration in groups)
				{
					DateTime registrationDate = registration.Min(item => item.RegistrationDate.GetValueOrDefault());
					string convertedRegistrationDate = $"{registrationDate.Year}-{registrationDate.Month}-{registrationDate.Day}T{registrationDate.Hour:00}:{registrationDate.Minute:00}:{registrationDate.Second:00}Z";

					if (registrationType != RegistrationType.Reset)
					{
						deliveryModels = GetDeliveryGroups(registration).ToList();
					}

					requestModel.Registrations.Add(new RegistrationRequest
					{
						RegistrationNumber = registration.Key,
						CustomerId = cars.CustomerId,
						RegistrationDate = convertedRegistrationDate,
						RegistrationType = registrationType.ToString(),
						Deliveries = deliveryModels
					});
				}

				Console.WriteLine($"Mapping from registration to request model successful. Data (serialized as JSON): {JsonConvert.SerializeObject(requestModel)}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Mapping registration to request model failed. Data (serialized as JSON): {JsonConvert.SerializeObject(cars)}: {ex}");

				throw;
			}

			return requestModel;
		}

		private ServiceResult MapToModel(
			ServiceResult serviceResult, BulkRegistrationResponse registrationResponse,
			string internalTransactionId, IList<int> identifier, string registrationId = "n/a")
		{
			serviceResult.TransactionId = internalTransactionId;
			serviceResult.RegistrationId = registrationId;
			serviceResult.RegisteredCarIds = identifier.ToList();
			serviceResult.TransactionState = "n/a";

			if (registrationResponse != null)
			{
				if (registrationResponse.RegistrationId != null)
				{
					serviceResult.Message = "SUCCESS";
				}
				else if (registrationResponse.TransactionId != null || registrationResponse.Errors?.Count > 0)
				{
					serviceResult.Message = "ERROR";
				}
			}
			else
			{
				serviceResult.Message = "ERROR";
			}

			return serviceResult;
		}

		private IList<DeliveryRequest> GetDeliveryGroups(IEnumerable<CarRegistrationModel> cars)
		{
			List<DeliveryRequest> deliveryGroups = new List<DeliveryRequest>();

			List<List<IGrouping<string, CarRegistrationModel>>> groupedCars = cars
				.GroupBy(x => x.DeliveryDate)
				.Select(grp => grp.AsEnumerable())
				.AsEnumerable()
				.Select(y => y.AsEnumerable().GroupBy(z => z.ErpDeliveryNumber))
				.Select(grp => grp.ToList())
				.ToList();

			foreach (List<IGrouping<string, CarRegistrationModel>> group in groupedCars)
			{
				foreach (IGrouping<string, CarRegistrationModel> carList in group)
				{
					List<CarRequest> carsOfGroup = new List<CarRequest>();
					foreach (CarRegistrationModel car in carList)
					{
						carsOfGroup.Add(new CarRequest() { VehicleIdentificationNumber = car.VehicleIdentificationNumber, AssetTag = string.Empty });
					}

					DateTime deliveryDate = carList.FirstOrDefault()!.DeliveryDate.Value;
					string convertedDeliveryDate = string.Format("{0}-{1}-{2}T{3:00}:{4:00}:{5:00}Z",
						deliveryDate.Year, deliveryDate.Month, deliveryDate.Day, deliveryDate.Hour, deliveryDate.Minute, deliveryDate.Second);

					deliveryGroups.Add(new DeliveryRequest()
					{
						DeliveryNumber = carList.FirstOrDefault()!.ErpDeliveryNumber,
						DeliveryDate = convertedDeliveryDate,
						Cars = carsOfGroup
					});
				}
			}

			return deliveryGroups;
		}

		private async Task<string> BeginTransactionGenerateId(IList<string> cars,
			string customerId, string companyId, RegistrationType registrationType, string identity, string registrationNumber = null)
		{
			Console.WriteLine(
				$"Trying to generate internal database transaction and initialize the transaction. Cars: {string.Join(",  ", cars)} ");

			try
			{
				string transactionId = DateTime.Now.Ticks.ToString();
				if (transactionId.Length > 32)
				{
					transactionId = transactionId.Substring(0, 32);
				}

				return await transactionService.BeginTransactionAsync(CarLeasingRepository, LeasingRegistrationRepository, cars, customerId, companyId, registrationType, identity, transactionId, registrationNumber);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Generating internal Transaction ID and initializing transaction failed. Cars: {string.Join(", ", cars)}: {ex}");

				throw;
			}
		}
	}
}
