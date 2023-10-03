namespace DevBasics.CarManagement.Dependencies
{
    using System;

    public partial class ErpRegistermentRegistration
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
    }
}
