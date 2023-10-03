using AutoMapper;
using System;

namespace DevBasics.CarManagement.Dependencies
{
    public class CarRegistrationModel : IHaveCustomMappings
    {
        public int RegisteredCarId { get; set; }
        public string RegistrationId { get; set; }
        public bool IsExistingVehicleInAzureDB { get; set; }
        public string VehicleIdentificationNumber { get; set; }
        public string CompanyId { get; set; }
        public string CustomerId { get; set; }
        public string Source { get; set; }
        public string CustomerRegistrationReference { get; set; }
        public string CarPool { get; set; }
        public string EmailAddresses { get; set; }
        public string TransactionType { get; set; }
        public string TransactionState { get; set; }
        public DateTime? ErrorNotificationSent { get; set; }
        public string CarPoolNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string ErpDeliveryNumber { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string ErpRegistrationNumber { get; internal set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CarRegistrationModel, ErpRegistermentRegistration>();

            configuration.CreateMap<CarRegistrationModel, CarRegistrationDto>()
                .ForMember(dest => dest.CarIdentificationNumber, opt => opt.MapFrom(x => x.VehicleIdentificationNumber));

            configuration.CreateMap<CarRegistrationDto, CarRegistrationModel>()
                .ForMember(dest => dest.CarPool, opt => opt.MapFrom(x => x.CarPoolNumber))
                .ForMember(dest => dest.VehicleIdentificationNumber, opt => opt.MapFrom(x => x.CarIdentificationNumber));
        }
    }
}
