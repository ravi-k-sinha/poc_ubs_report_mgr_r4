namespace UBS.ReportManager.Abstractions.Model.Domain
{
    using System;
    using LendFoundry.Foundation.Persistence;

    /// <summary>
    /// Provides information for generating a report using a template from jsreport server. Multiple reports can exist
    /// for the same template code, since data-source may be different 
    /// </summary>
    public interface IReport : IAggregate
    {
        /// <summary>
        /// A unique code for this report
        /// </summary>
        string Code { get; set; }
        
        /// <summary>
        /// A unique name of this report
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// Denotes the purpose of this report. Few possible values are 'Report', 'Email Body', 'Email Attachment',
        /// 'Document', etc.
        /// </summary>
        string Purpose { get; set; }
        
        /// <summary>
        /// A description of this report explaining the context, purpose and usage of this report 
        /// </summary>
        string Description { get; set; }
        
        /// <summary>
        /// Information about the JSReport template that will be used to generate this report.
        /// </summary>
        JsReportTemplate JsReportTemplate { get; set; }
        
        /// <summary>
        /// A JSON schema that is used for validating the input received by the caller to create a report
        /// </summary>
        string InputJsonSchema { get; set; }
        
        /// <summary>
        /// A JSON schema that is used for validating the report data received after invoking the data-source
        /// </summary>
        string DataJsonSchema { get; set; }
        
        /// <summary>
        /// A URL which when invoked is supposed to return data in JSON format
        /// </summary>
        string DatasourceUrl { get; set; }

        /// <summary>
        /// Determines whether generated report files are allowed to be stored
        /// </summary>
        bool StorageAllowed { get; set; }
        
        /// <summary>
        /// The name that will be used for the generated report file
        /// </summary>
        string GeneratedFileName { get; set; }
        
        /// <summary>
        /// The extension that will be used for the generated file. Example extensions are PDF/pdf, HTML/html, xls, etc.
        /// </summary>
        string GeneratedFileExtension { get; set; }
        
        /// <summary>
        /// Determines if this report is currently active or not. This information may be used by interfaces in listing
        /// reports appropriately
        /// </summary>
        bool Active { get; set; }

        DateTimeOffset CreatedOn { get; set; }
        DateTimeOffset UpdatedOn { get; set; }
        DateTimeOffset DeletedOn { get; set; }
    }
}