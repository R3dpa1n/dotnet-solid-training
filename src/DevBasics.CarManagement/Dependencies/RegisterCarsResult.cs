using System.Collections.Generic;

namespace DevBasics.CarManagement.Dependencies
{
    public class RegisterCarsResult
    {
        public string RegistrationId { get; set; }
        public bool AlreadyRegistered { get; set; } = false;

        public List<string> RegisteredCars { get; set; } = new List<string>();
    }
}
