using System;

namespace DevBasics.CarManagement.Dependencies
{
    public class RegistrationApiResponseBase
    {
        public string Status => Enum.GetName(typeof(ApiResult), Result);
        public string ErrorMessage { get; set; }

        #region HELPER

        public ApiResult Result { get; set; } = ApiResult.ERROR;

        public enum ApiResult
        {
            SUCCESS,
            ERROR,
            WARNING
        }

        #endregion
    }
}