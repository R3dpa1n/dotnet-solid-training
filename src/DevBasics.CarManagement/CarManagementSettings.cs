using System.Collections.Generic;

namespace DevBasics.CarManagement
{
	public class CarManagementSettings : ICarManagementSettings
	{
		public IDictionary<int, string> ApiEndpoints { get; set; } = new Dictionary<int, string>();
		public IDictionary<string, string> HttpHeaders { get; set; } = new Dictionary<string, string>();
		public IDictionary<string, string> LanguageCodes { get; set; } = new Dictionary<string, string>();
		public IDictionary<string, int> TimeZones { get; set; } = new Dictionary<string, int>();

		public CarManagementSettings()
		{
			// Define all Leasing API Endpoints.
			ApiEndpoints.Add(1, "/bulk-registration-devices");
			ApiEndpoints.Add(2, "/check-transaction-status");
			ApiEndpoints.Add(3, "/show-registration-details");

			// Define headers for HTTP-Requests.
			HttpHeaders.Add("Content-Type", "application/json");

			// Define valid language codes (see Leasing API Spec).
			LanguageCodes.Add("Dutch", "nl");
			LanguageCodes.Add("English", "en");
			LanguageCodes.Add("French", "fr");
			LanguageCodes.Add("German", "de");
			LanguageCodes.Add("Spanish", "es");
			LanguageCodes.Add("Italian", "it");
			LanguageCodes.Add("Japanese", "jp");
			LanguageCodes.Add("Traditional Chinese", "zf");
			LanguageCodes.Add("Simple Chinese", "zh");
			LanguageCodes.Add("Swedish", "sv");
			LanguageCodes.Add("Finnish", "fi");
			LanguageCodes.Add("Danish", "dk");
			LanguageCodes.Add("Norwegian", "no");
			LanguageCodes.Add("Thailand", "th");
			LanguageCodes.Add("Brazilian Portugese", "br");
			LanguageCodes.Add("Czech", "cs");
			LanguageCodes.Add("Hungarian", "hu");
			LanguageCodes.Add("Polish", "pl");
			LanguageCodes.Add("Portuguese", "pt");
			LanguageCodes.Add("Korean", "ko");
			LanguageCodes.Add("Malay", "my");
			LanguageCodes.Add("Romanian", "ro");
			LanguageCodes.Add("Slovak", "sk");
			LanguageCodes.Add("Ukrainian", "uk");
			LanguageCodes.Add("Hindi", "hi");

			TimeZones.Add("Europe/London", 0);
			TimeZones.Add("Europe/Lisbon", 0);
			TimeZones.Add("America/Noronha", 120);
			TimeZones.Add("Atlantic/South_Georgia", 120);
			TimeZones.Add("America/Argentina/Buenos_Aires", 180);
			TimeZones.Add("America/Sao_Paulo", 180);
			TimeZones.Add("America/Godthab", 180);
			TimeZones.Add("America/St_Johns", 210);
			TimeZones.Add("America/Halifax", 240);
			TimeZones.Add("America/Aruba", 240);
			TimeZones.Add("America/New_York", 300);
			TimeZones.Add("EST", 300);
			TimeZones.Add("America/Chicago", 360);
			TimeZones.Add("America/Mexico_City", 360);
			TimeZones.Add("America/Phoenix", 420);
			TimeZones.Add("America/Santa_Isabel", 480);
			TimeZones.Add("America/Vancouver", 480);
			TimeZones.Add("America/Los_Angeles", 480);
			TimeZones.Add("America/Anchorage", 540);
			TimeZones.Add("America/Yakutat", 540);
			TimeZones.Add("Pacific/Honolulu", 600);
			TimeZones.Add("Europe/Berlin", -60);
			TimeZones.Add("Europe/Bratislava", -60);
			TimeZones.Add("Europe/Bucharest", -120);
			TimeZones.Add("Europe/Istanbul", -120);
			TimeZones.Add("Asia/Kuwait", -180);
			TimeZones.Add("Asia/Muscat", -240);
			TimeZones.Add("Asia/Oral", -300);
			TimeZones.Add("Asia/Yekaterinburg", -300);
			TimeZones.Add("Asia/Kolkata", -330);
			TimeZones.Add("Asia/Omsk", -360);
			TimeZones.Add("Indian/Cocos", -390);
			TimeZones.Add("Asia/Pontianak", -420);
			TimeZones.Add("Asia/Bangkok", -420);
			TimeZones.Add("Asia/Singapore", -480);
			TimeZones.Add("Asia/Kuala_Lumpur", -480);
			TimeZones.Add("Australia/Perth", -480);
			TimeZones.Add("Asia/Shanghai", -480);
			TimeZones.Add("Asia/Tokyo", -540);
			TimeZones.Add("Asia/Seoul", -540);
			TimeZones.Add("Australia/Adelaide", -570);
			TimeZones.Add("Australia/Melbourne", -600);
			TimeZones.Add("Australia/Lord_Howe", -630);
			TimeZones.Add("Etc/GMT-11", -660);
			TimeZones.Add("Pacific/Auckland", -720);
			TimeZones.Add("Etc/GMT-13", -780);
		}
	}
}
