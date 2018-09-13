namespace UBS.ReportManager.Abstractions.Model.Domain
{
    public class JsReportTemplate
    {
        /// <summary>
        /// Unique code for the template that will generate a report. This template will exist in a jsreport server
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// Unique name of the template that will generate a report. This template will exist with a jsreport server
        /// </summary>
        public string Name { get; set; }
    }
}