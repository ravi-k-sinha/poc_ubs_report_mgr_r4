using System;

namespace UBS.ReportManager.Abstractions.Model.Domain
{
    public class Report : IReport
    {
        public string TenantId { get; set; }
        public string Id { get; set; }
        public string TemplateCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool StorageAllowed { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public DateTimeOffset DeletedOn { get; set; }
    }
}