using AutoMapper;
using DevBasics.CarManagement.Dependencies;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DevBasics.CarManagement.Dependencies.RegistrationApiResponseBase;

namespace DevBasics.CarManagement.Helper
{
	public class RegistrationHelper : IRegistrationHelper
	{
		private readonly IMapper _mapper;

		public RegistrationHelper(
			IMapper mapper)
		{
			_mapper = mapper;
		}

		public async Task<ServiceResult> ForceBulkRegistration(ICarRegistrationRepository CarLeasingRepository, IList<CarRegistrationModel> forceItems, string identity)
		{
			List<RegisterCarRequest> subsequentRegistrationRequestModels = new List<RegisterCarRequest>();
			RegistrationApiResponse subsequentRegistrationResponse = new RegistrationApiResponse();
			ServiceResult forceResponse = new ServiceResult();
			Dictionary<int, DateTime?> latestHistoryRowCreationDate = new Dictionary<int, DateTime?>();

			try
			{
				Console.WriteLine(
							$"The registration with registration ids {string.Join(", ", forceItems.Select(x => x.RegistrationId))} has already been processed but forceRegisterment is true, " +
							$"so the registration registration items will be registrationed again.");

				IList<CarRegistrationModel> currentDbCars = await CarLeasingRepository.GetApiRegisteredCarsAsync(forceItems.Select(x => x.VehicleIdentificationNumber).ToList());

				foreach (CarRegistrationModel forceRegisterCar in forceItems)
				{
					CarRegistrationModel currentDbCar = currentDbCars
						.FirstOrDefault(y => y.VehicleIdentificationNumber == forceRegisterCar.VehicleIdentificationNumber);

					latestHistoryRowCreationDate.Add(
						currentDbCar.RegisteredCarId,
						(await CarLeasingRepository.GetLatestCarHistoryEntryAsync(forceRegisterCar.VehicleIdentificationNumber)).RowCreationDate
					 );

					AssignCarValuesForUpdate(CarLeasingRepository, currentDbCar, forceRegisterCar, identity, source: "Force Registerment");

					// Map the car to the needed request model for a subsequent registration transaction.
					RegisterCarRequest item = new RegisterCarRequest()
					{
						RegistrationNumber = currentDbCar.RegistrationId,
						Car = forceRegisterCar.VehicleIdentificationNumber,
						ErpRegistrationNumber = string.Empty,
						CompanyId = forceRegisterCar.CompanyId,
						CustomerId = forceRegisterCar.CustomerId,
					};
					subsequentRegistrationRequestModels.Add(item);
				}

				forceResponse.TransactionId = subsequentRegistrationResponse.ActionResult.FirstOrDefault().TransactionId;
				if (subsequentRegistrationResponse.Status != ApiResult.ERROR.ToString())
				{
					forceResponse.Message = subsequentRegistrationResponse.Status;
				}
				else
				{
					// Revert all force cars to data status of latest history item.
					IEnumerable<ServiceResult> failedTransactions = subsequentRegistrationResponse.ActionResult.Where(x => x.Message == ApiResult.ERROR.ToString());
					foreach (ServiceResult item in failedTransactions)
					{
						await HandleDataRevertAsync(CarLeasingRepository, item.RegisteredCarIds, identity);
					}

					throw new ForceRegistermentException("Subsequent registration transaction returned an error");
				}

				Console.WriteLine(
					$"Forcing registration of an existing registration has been procecces. Return data (serialized as JSON): {JsonConvert.SerializeObject(forceResponse)}");
			}
			catch (ForceRegistermentException feEx)
			{
				Console.WriteLine(
					$"Forced registration of an already existing registration registration failed. Values have been restored from car history." +
					$"Data of forced registration (serialized as JSON): {JsonConvert.SerializeObject(forceItems)}: {feEx}");

				forceResponse.Message = "FORCE_ERROR";
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Forced registration of an already existing registration failed due to unexpected reason." +
					$"Data of forced registration (serialized as JSON): {JsonConvert.SerializeObject(forceItems)}: {ex}");

				forceResponse.Message = "FORCE_ERROR";
			}

			return forceResponse;
		}

		private async Task HandleDataRevertAsync(ICarRegistrationRepository CarLeasingRepository, List<int> dbCarsToRevert, string identity, bool onlyForceRegistermentItems = true)
		{
			Console.WriteLine($"Trying to execute data revert of cars. " +
				$"Cars (serialized as JSON): {JsonConvert.SerializeObject(dbCarsToRevert)}, " +
				$"Azure User Identity: {identity}," +
				$"Only Force Registerment Items: {onlyForceRegistermentItems}");

			try
			{
				if (dbCarsToRevert != null && dbCarsToRevert.Count > 0)
				{
					foreach (int id in dbCarsToRevert)
					{
						// Get all history items of car which should be reverted.
						IEnumerable<CarRegistrationLogDto> carHistory = await CarLeasingRepository.GetCarHistoryAsync(id.ToString());

						DateTime? rowCreationDate = null;
						if (!onlyForceRegistermentItems)
						{
							rowCreationDate = carHistory?
								.Where(x => string.IsNullOrWhiteSpace(x.TransactionType) || x.TransactionType != TransactionResult.Progress.ToString())
								.FirstOrDefault()
								.RowCreationDate;
						}
						else
						{
							Console.WriteLine($"Get latest row creation date of car with ID {id} of 'Force Registerment User'");

							// Extract the RowCreationDate of the item which contains the data to revert the car.
							rowCreationDate = carHistory?
								.Where(x => x.UserName != "Force Registerment User" && (string.IsNullOrWhiteSpace(x.TransactionType) || x.TransactionType != TransactionResult.Progress.ToString()))
								.FirstOrDefault()
								.RowCreationDate;
						}

						// Call method to revert the car.
						bool isReverted = await RevertCarDataAsync(CarLeasingRepository, id, rowCreationDate, identity);

						Console.WriteLine($"Revert completed with result: {isReverted}");
					}
				}
				else
				{
					Console.WriteLine($"List of cars to revert is empty. Revert not possible");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(
					$"Unexpected error while handling data revert of cars: Revert failed." +
					$"Cars (serialized as JSON): {JsonConvert.SerializeObject(dbCarsToRevert)}, " +
					$"Azure User Identity: {identity}," +
					$"Only Force Registerment Items: {onlyForceRegistermentItems}: {ex}");
			}
		}

		private async Task<bool> RevertCarDataAsync(ICarRegistrationRepository CarLeasingRepository, int carDatasetId, DateTime? rowCreationDate, string identity)
		{
			Console.WriteLine($"Trying to revert car data to values of latest history item of car with ID {carDatasetId}");

			try
			{
				if (rowCreationDate != null)
				{

					CarRegistrationModel currentCarData = await CarLeasingRepository.GetApiRegisteredCarAsync(carDatasetId);

					var car = (await CarLeasingRepository.GetCarHistoryAsync(carDatasetId.ToString()))
						.FirstOrDefault(x => x.RowCreationDate == rowCreationDate);

					CarRegistrationModel latestCarHistoryData = new CarRegistrationModel
					{
						RegistrationId = car.RegistrationId
					};

					Console.WriteLine(
						$"Current car data (serialized as JSON): {JsonConvert.SerializeObject(currentCarData)}\n" +
						$"Latest history data (serialized as JSON): {JsonConvert.SerializeObject(latestCarHistoryData)}");

					latestCarHistoryData.ErrorNotificationSent = null;
					AssignCarValuesForUpdate(CarLeasingRepository, currentCarData, latestCarHistoryData, identity, true);

					return true;
				}
				else
				{
					Console.WriteLine($"RowCreationDate is null, revert item cannot be identified. Aborting revert");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unexpected error while reading current data with id {carDatasetId} and revert item of date {rowCreationDate}: {ex}");
			}

			return false;
		}

		private void AssignCarValuesForUpdate(ICarRegistrationRepository CarLeasingRepository, CarRegistrationModel carToUpdate, CarRegistrationModel carUpdateValues, string identity, bool saveWithHistory = false, string source = null)
		{
			Console.WriteLine($"Trying to assign values from revert item." +
				$"Car (serialized as JSON): {carToUpdate}, " +
				$"Revert Data (serialized as JSON): {carUpdateValues}");

			try
			{
				carToUpdate.ErpRegistrationNumber = carUpdateValues.ErpRegistrationNumber;
				carToUpdate.CompanyId = carUpdateValues.CompanyId;
				carToUpdate.CustomerId = carUpdateValues.CustomerId;
				carToUpdate.EmailAddresses = carUpdateValues.EmailAddresses;
				carToUpdate.CustomerRegistrationReference = carUpdateValues.CustomerRegistrationReference;
				carToUpdate.CarPool = carUpdateValues.CarPool;
				carToUpdate.ErrorNotificationSent = carUpdateValues.ErrorNotificationSent;

				carToUpdate.Source = source ?? carUpdateValues.Source;

				if (!string.IsNullOrWhiteSpace(carUpdateValues.CarPoolNumber))
				{
					carToUpdate.CarPoolNumber = carUpdateValues.CarPoolNumber;
				}

				CarLeasingRepository.UpdateErpRegistrationItemAsync(_mapper.Map<ErpRegistermentRegistration>(carToUpdate));

				CarRegistrationDto dbCar = _mapper.Map<CarRegistrationDto>(carToUpdate);

				// Mapping ignores these two properties, so the values have to be set manually.
				Enum.TryParse(carToUpdate.TransactionType, out RegistrationType parsedTransactionType);
				Enum.TryParse(carToUpdate.TransactionState, out TransactionResult parsedTransactionStatus);
				dbCar.TransactionType = (int)parsedTransactionType;
				dbCar.TransactionState = (int)parsedTransactionStatus;

				CarLeasingRepository.UpdateRegisteredCarAsync(dbCar, identity, saveWithHistory);

				Console.WriteLine($"Reverted car data. Car (serialized as JSON): {JsonConvert.SerializeObject(dbCar)}");
			}
			catch (Exception ex)
			{
				Console.WriteLine(
					$"Unexpected error while assingning values from revert item. " +
					$"Car (serialized as JSON): {carToUpdate}, " +
					$"Revert Data (serialized as JSON): {carUpdateValues}: {ex}");
			}
		}
	}
}
