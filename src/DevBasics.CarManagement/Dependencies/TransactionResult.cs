namespace DevBasics.CarManagement.Dependencies
{
    public enum TransactionResult
    {
        None = 0,

        DataError = -1,
        RegistrationDataError = -10,
        DeliveryDataError = -11,
        CarDataError = -12,

        Progress = 1,
        Failed = 2,
        Registered = 3,
        ActionRequired = 4,
        NotRegistered = 5,
        MissingData = 6
    }
}
