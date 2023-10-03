using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBasics.CarManagement.Dependencies
{
    public interface IRegistrationService
    {
        Task<RegisterCarsResult> SaveRegistrations(RegisterCarsModel registerCarsModel, Claims claims, string registrationId, string identity, bool isForcedRegisterment, CarBrand brand);
    }

    internal sealed class RegistrationService : IRegistrationService
    {
        public IList<Tuple<DateTime, RegisterCarsModel, string, string, CarBrand>> Registrations { get; } = new List<Tuple<DateTime, RegisterCarsModel, string, string, CarBrand>>();

        public Task<RegisterCarsResult> SaveRegistrations(RegisterCarsModel registerCarsModel, Claims claims, string registrationId, string identity, bool isForcedRegisterment, CarBrand brand)
        {
            var result = new RegisterCarsResult
            {
                RegistrationId = registrationId,
                RegisteredCars = new List<string>(registerCarsModel.Cars.Select(cr => cr.VehicleIdentificationNumber))
            };

            Registrations.Add(Tuple.Create(DateTime.Now, registerCarsModel, identity, registrationId, brand));

            return Task.FromResult(result);
        }
    }
}