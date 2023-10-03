using System.Collections.Generic;

namespace DevBasics.CarManagement.Dependencies
{
    public class RegistrationRequest : RegistrationBase
    {
        public IList<DeliveryRequest> Deliveries { get; set; }
    }
}
