using System.Collections.Generic;

namespace DevBasics.CarManagement.Dependencies
{
    public class RegistermentRegistrationsResponse : RegistrationApiResponseBase
    {
        public List<CarRegistrationModel> RegistermentRegistrations { get; set; }
    }
}
