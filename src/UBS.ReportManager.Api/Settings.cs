namespace UBS.ReportManager.Api
{
    using System;
    using System.Globalization;

    public class Settings
    {
        public static string ServiceName { get; } = "report-manager";

        private static string Prefix { get; } = ServiceName.ToUpper(CultureInfo.CurrentCulture);
        
        public static string TenantUrl => 
            Environment.GetEnvironmentVariable($"{Prefix}_TENANT_URL") ?? "http://tenant:5000";
        
        public static string ConfigurationUrl => 
            Environment.GetEnvironmentVariable($"{Prefix}_CONFIGURATION_URL") ?? "http://configuration:5000";

        public static string JsReportUrl => 
            Environment.GetEnvironmentVariable($"{Prefix}_JSREPORT_URL") ?? "http://jsreport:5488";

        public static string MongoConnectionString =>
            Environment.GetEnvironmentVariable($"{Prefix}_MONGO_CONNECTION_STRING") ?? "mongodb://localhost:27017";
        
        public static string MongoDatabaseName => 
            Environment.GetEnvironmentVariable($"{Prefix}_MONGO_DB_NAME") ?? "reports";
    }
}