namespace UBS.ReportManager.Abstractions.Model.Communication
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Domain;

    public class ReportCreationRequest
    {
        [Required]
        public string Code { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Purpose { get; set; }
        
        public string Description { get; set; }
        
        [Required]
        public JsReportTemplate JsReportTemplate { get; set; }
        
        public string InputJsonSchema { get; set; }
        public string DataJsonSchema { get; set; }
        public string DatasourceUrl { get; set; }
        public bool StorageAllowed { get; set; }
        
        [Required]
        public string GeneratedFileName { get; set; }
        
        [Required]
        public string GeneratedFileExtension { get; set; }
        
        public bool Active { get; set; }

        public Report ToReport()
        {
            var report = new Report();
            report.Code = Code;
            report.Name = Name;
            report.Purpose = Purpose;
            report.Description = Description;
            report.JsReportTemplate = JsReportTemplate;
            report.InputJsonSchema = InputJsonSchema;
            report.DataJsonSchema = DataJsonSchema;
            report.DatasourceUrl = DatasourceUrl;
            report.StorageAllowed = StorageAllowed;
            report.GeneratedFileName = GeneratedFileName;
            report.GeneratedFileExtension = GeneratedFileExtension;
            report.Active = Active;

            return report;
        }

        public static List<Report> ToReportList(List<ReportCreationRequest> requests)
        {
            var reports = new List<Report>();
            requests.ForEach(request => reports.Add(request.ToReport()));
            return reports;
        }
    }
}