namespace UBS.ReportManager.Api
{
    using System.Globalization;

    public class Settings
    {
        public static string ServiceName { get; } = "report-manager";

        private static string Prefix { get; } = ServiceName.ToUpper(CultureInfo.CurrentCulture);
    }
}