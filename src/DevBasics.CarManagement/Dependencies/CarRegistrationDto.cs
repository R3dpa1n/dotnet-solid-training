namespace DevBasics.CarManagement.Dependencies
{
    using System;

    public partial class CarRegistrationDto
    {
        public int RegisteredCarId { get; set; }
        public string RegistrationId { get; set; }
        public string CarIdentificationNumber { get; set; }
        public string TransactionId { get; set; }
        public int? SalesOrganizationUnitId { get; set; }
        public DateTime RegistermentRegistrationCreationDate { get; set; }
        public int ErpRegistermentRegistrationId { get; set; }
        public bool? DisableAutomaticRegisterment { get; set; }
        public string CustomerId { get; set; }
        public string VendorId { get; set; }
        public string Source { get; set; }
        public int? TransactionType { get; set; }
        public int? TransactionState { get; set; }
        public DateTime? TransactionStartDate { get; set; }
        public DateTime? TransactionEndDate { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string CompanyId { get; set; }
        public DateTime? ConfirmationMailSent { get; set; }
        public DateTime? ErrorNotificationSent { get; set; }
        public bool Archived { get; set; }
        public string CarPoolNumber { get; set; }
        public string AccTransactionId { get; set; }
        public DateTime? RegistrationDate { get; set; }
    }
}
