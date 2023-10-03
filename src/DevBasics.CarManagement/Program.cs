using AutoMapper;
using DevBasics.CarManagement.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevBasics.CarManagement
{
    internal sealed class Program
    {
        internal static async Task Main()
        {
            var service = new ServiceCollection();

            service
                .AddTransient<CarManagementSettings>()
                .AddTransient<HttpHeaderSettings>()
                .AddTransient<IKowoLeasingApiClient, KowoLeasingApiClientMock>()
                .AddTransient<ITransactionStateService, TransactionStateServiceMock>()
                .AddTransient<IBulkRegistrationService, BulkRegistrationServiceMock>()
                .AddTransient<ILeasingRegistrationRepository, LeasingRegistrationRepository>()
                .AddTransient<IRegistrationDetailService, RegistrationDetailServiceMock>()
                .AddTransient<ICarRegistrationRepository, CarRegistrationRepository>()
                .AddTransient<MapperConfiguration>(_ => new MapperConfiguration(configuration => new CarRegistrationModel().CreateMappings(configuration)))
                .AddTransient<IMapper>(provider => provider.GetRequiredService<MapperConfiguration>().CreateMapper())
                .AddTransient<ICarManagementService, CarManagementService>();

            IServiceProvider provider = service.BuildServiceProvider();

            ICarManagementService carManagementServiceFactory = provider.GetRequiredService<ICarManagementService>();

            await carManagementServiceFactory.RegisterCarsAsync(
                new RegisterCarsModel
                {
                    CompanyId = "Company",
                    CustomerId = "Customer",
                    VendorId = "Vendor",
                    Cars = new List<CarRegistrationModel>
                    {
                        new CarRegistrationModel
                        {
                            CompanyId = "Company",
                            CustomerId = "Customer",
                            VehicleIdentificationNumber = Guid.NewGuid().ToString(),
                            DeliveryDate = DateTime.Now.AddDays(14).Date,
                            ErpDeliveryNumber = Guid.NewGuid().ToString()
                        }
                    }
                },
                false,
                new Claims());
        }
    }
}
