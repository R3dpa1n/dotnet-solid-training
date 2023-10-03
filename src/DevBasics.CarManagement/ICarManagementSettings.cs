using System.Collections.Generic;

namespace DevBasics.CarManagement
{
	public interface ICarManagementSettings
	{
		IDictionary<int, string> ApiEndpoints { get; set; }
		IDictionary<string, string> HttpHeaders { get; set; }
		IDictionary<string, string> LanguageCodes { get; set; }
		IDictionary<string, int> TimeZones { get; set; }
	}
}