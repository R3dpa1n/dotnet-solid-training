using DevBasics.CarManagement.Dependencies;
using System;

namespace DevBasics.CarManagement
{
    public static class CarPoolNumberHelper
    {
        public static void Generate(CarBrand requestOrigin, string endCustomerRegistrationReference, out string registrationRegistrationId, out string registrationNumber)
        {
            registrationRegistrationId = GenerateRegistrationRegistrationId();
            registrationNumber = string.Empty;

            switch (requestOrigin)
            {
                case CarBrand.Ford:
                    registrationNumber = GenerateFordRegistrationNumber(endCustomerRegistrationReference, registrationRegistrationId);
                    break;

                case CarBrand.Toyota:
                    registrationNumber = GenerateToyotaRegistrationNumber(endCustomerRegistrationReference, registrationRegistrationId);
                    break;

                case CarBrand.Undefined:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(requestOrigin), requestOrigin, null);
            }
        }

        public static string GenerateRegistrationRegistrationId()
        {
            return DateTime.Now.Ticks.ToString();
        }

        private static string GenerateFordRegistrationNumber(string endCustomerRegistrationReference, string registrationRegistrationId)
        {
            return string.IsNullOrWhiteSpace(endCustomerRegistrationReference) ? registrationRegistrationId : endCustomerRegistrationReference;
        }

        private static string GenerateToyotaRegistrationNumber(string endCustomerRegistrationReference, string registrationRegistrationId)
        {
            if (string.IsNullOrWhiteSpace(endCustomerRegistrationReference))
            {
                return registrationRegistrationId;
            }

            const int maxLength = 32;

            string endCustomerRegistrationReferenceShort = endCustomerRegistrationReference.Length > 23
                ? endCustomerRegistrationReference.Substring(0, 23)
                : endCustomerRegistrationReference;

            Guid uniqueValue = Guid.NewGuid();
            string uniqueValueBase64 = Convert.ToBase64String(uniqueValue.ToByteArray());

            // Remove unnecessary characters from base64 string.
            uniqueValueBase64 = uniqueValueBase64.Replace("=", string.Empty);
            uniqueValueBase64 = uniqueValueBase64.Replace("+", string.Empty);
            uniqueValueBase64 = uniqueValueBase64.Replace("/", string.Empty);
            string uniqueValueBase64Short = uniqueValueBase64.Substring(0, 8);

            string depRegistrationNumber = $"{endCustomerRegistrationReferenceShort}-{uniqueValueBase64Short}";

            return depRegistrationNumber.Length > maxLength ? depRegistrationNumber.Substring(0, maxLength) : depRegistrationNumber;
        }
    }
}