namespace DevBasics.CarManagement.Dependencies
{
	public class HttpHeaderSettings : IHttpHeaderSettings
	{
		public string SalesOrgIdentifier { get; set; }
		public CarBrand WebAppType { get; set; }
	}
}
