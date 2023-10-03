using System.Collections.Generic;

namespace DevBasics.CarManagement.Dependencies
{
    public class RegisterCarsModel
    {
        public string VendorId { get; set; }
        public string CompanyId { get; set; }
        public string CustomerId { get; set; }
        public bool DeactivateAutoRegistrationProcessing { get; set; }

        public IList<CarRegistrationModel> Cars { get; set; } = new List<CarRegistrationModel>();
    }
}