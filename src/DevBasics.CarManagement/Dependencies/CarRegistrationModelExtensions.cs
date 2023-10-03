using System.Collections.Generic;
using System.Linq;

namespace DevBasics.CarManagement.Dependencies
{
    public static class CarRegistrationModelExtensions
    {
        public static IList<CarRegistrationModel> RemoveDuplicates(this IList<CarRegistrationModel> items)
        {
            List<CarRegistrationModel> validatedItems = new List<CarRegistrationModel>();

            IEnumerable<CarRegistrationModel> itemsWithDin = items.Where(x => string.IsNullOrWhiteSpace(x.VehicleIdentificationNumber) == false);
            IEnumerable<CarRegistrationModel> distinctItemsWithDin = itemsWithDin.GroupBy(x => x.VehicleIdentificationNumber).Select(x => x.FirstOrDefault());
            IEnumerable<CarRegistrationModel> itemsWithoutDin = items.Where(x => string.IsNullOrWhiteSpace(x.VehicleIdentificationNumber));

            if (distinctItemsWithDin != null
                && distinctItemsWithDin.Any())
            {
                validatedItems.AddRange(distinctItemsWithDin);
            }

            if (itemsWithoutDin != null
                && itemsWithoutDin.Any())
            {
                validatedItems.AddRange(itemsWithoutDin);
            }

            return validatedItems;
        }
    }
}