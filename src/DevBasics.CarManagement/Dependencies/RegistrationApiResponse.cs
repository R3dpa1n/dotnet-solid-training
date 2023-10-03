using System.Collections.Generic;

namespace DevBasics.CarManagement.Dependencies
{
    public class RegistrationApiResponse : RegistrationApiResponseBase
    {
        public List<ServiceResult> ActionResult { get; set; } = new List<ServiceResult>();
    }
}
