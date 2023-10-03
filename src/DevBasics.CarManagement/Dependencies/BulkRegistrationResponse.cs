using System.Collections.Generic;

namespace DevBasics.CarManagement.Dependencies
{
    public class BulkRegistrationResponse
    {
        public string RegistrationId { get; set; }
        public string Response { get; set; }
        public IList<string> Errors { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string TransactionId { get; set; }
    }
}
