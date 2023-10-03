using System.Collections.Generic;

namespace DevBasics.CarManagement.Dependencies
{
    public class ServiceResult
    {
        public ServiceResult()
        {
        }

        public List<int> RegisteredCarIds { get; set; } = new List<int>();
        public string TransactionId { get; set; } = string.Empty;
        public string TransactionState { get; set; } = "Rejected";
        public string Message { get; set; }
        public string RegistrationId { get; set; }
    }
}
