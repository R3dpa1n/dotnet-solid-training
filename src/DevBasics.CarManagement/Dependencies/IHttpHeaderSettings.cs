namespace DevBasics.CarManagement.Dependencies
{
	public interface IHttpHeaderSettings
	{
		string SalesOrgIdentifier { get; set; }
		CarBrand WebAppType { get; set; }
	}
}