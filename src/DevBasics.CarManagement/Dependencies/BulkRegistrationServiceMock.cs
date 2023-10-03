using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBasics.CarManagement.Dependencies
{
    public interface IBulkRegistrationService
    {
        Task<BulkRegistrationResponse> ExecuteRegistrationAsync(BulkRegistrationRequest requestPayload);
    }

    internal sealed class BulkRegistrationServiceMock : IBulkRegistrationService
    {
        public IList<Tuple<DateTime, BulkRegistrationRequest>> Requests = new List<Tuple<DateTime, BulkRegistrationRequest>>();

        public Task<BulkRegistrationResponse> ExecuteRegistrationAsync(BulkRegistrationRequest requestPayload)
        {
            Requests.Add(Tuple.Create(DateTime.Now, requestPayload));

            var result = new BulkRegistrationResponse
            {
                ErrorCode = "",
                ErrorMessage = "",
                Errors = new List<string>(),
                RegistrationId = requestPayload.Registrations.FirstOrDefault()?.RegistrationNumber,
                TransactionId = requestPayload.TransactionId,
                Response = ""
            };

            return Task.FromResult(result);
        }
    }
}
