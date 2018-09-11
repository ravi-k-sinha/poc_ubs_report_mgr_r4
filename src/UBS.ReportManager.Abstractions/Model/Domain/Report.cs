namespace UBS.ReportManager.Abstractions.Model.Domain
{
    using System;
    using LendFoundry.Foundation.Persistence;

    public class Report : Aggregate, IReport
    {
        public string TemplateCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DatasourceUrl { get; set; }
        public bool StorageAllowed { get; set; }
        
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public DateTimeOffset DeletedOn { get; set; }
    }
}