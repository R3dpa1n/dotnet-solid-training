namespace DevBasics.CarManagement.Dependencies
{
    public class CarRequest : CarBase
    {
        private string _deviceId;

        public string VehicleIdentificationNumber
        {
            get => _deviceId;
            set => _deviceId = value.ToUpper();
        }
    }
}
