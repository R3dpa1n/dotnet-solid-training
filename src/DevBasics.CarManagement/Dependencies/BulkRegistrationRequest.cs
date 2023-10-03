using System.Collections.Generic;

namespace DevBasics.CarManagement.Dependencies
{
    public class BulkRegistrationRequest
    {
        public RequestContext RequestContext { get; set; }
        public string CompanyId { get; set; }
        public string TransactionId { get; set; }
        public IList<RegistrationRequest> Registrations { get; set; }
    }
}
