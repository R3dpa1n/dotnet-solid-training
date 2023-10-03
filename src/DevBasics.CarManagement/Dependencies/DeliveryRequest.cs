using System.Collections.Generic;

namespace DevBasics.CarManagement.Dependencies
{
    public class DeliveryRequest : DeliveryBase
    {
        public IList<CarRequest> Cars { get; set; }
    }
}
