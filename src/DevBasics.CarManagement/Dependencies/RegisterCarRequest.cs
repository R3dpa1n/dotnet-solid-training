namespace DevBasics.CarManagement.Dependencies
{
    public class RegisterCarRequest
    {
        public string CustomerId { get; set; }
        public string CompanyId { get; set; }
        public string ErpRegistrationNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public string Car { get; set; }
    }
}