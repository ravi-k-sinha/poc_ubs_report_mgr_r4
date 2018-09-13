namespace UBS.ReportManager.Abstractions.Model.Domain
{
    using System;
    using LendFoundry.Foundation.Persistence;

    public class Report : Aggregate, IReport
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Purpose { get; set; }
        public string Description { get; set; }
        public JsReportTemplate JsReportTemplate { get; set; }
        public string InputJsonSchema { get; set; }
        public string DataJsonSchema { get; set; }
        public string DatasourceUrl { get; set; }
        public bool StorageAllowed { get; set; }
        public string GeneratedFileName { get; set; }
        public string GeneratedFileExtension { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public DateTimeOffset DeletedOn { get; set; }
    }
}