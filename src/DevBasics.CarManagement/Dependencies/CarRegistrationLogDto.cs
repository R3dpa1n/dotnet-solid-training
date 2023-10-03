namespace DevBasics.CarManagement.Dependencies
{
    using System;

    public partial class CarRegistrationLogDto
    {
        public string RegistrationId { get; set; }
        public DateTime RowCreationDate { get; set; }
        public string UserName { get; set; }
        public string TransactionType { get; set; }
        public string TransactionState { get; set; }
    }
}
