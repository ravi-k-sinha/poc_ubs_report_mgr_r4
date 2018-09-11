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
        /// Unique code for the template that will generate this report. This template will exist in a jsreport
        /// installation
        /// </summary>
        string TemplateCode { get; set; }

        /// <summary>
        /// A unique name of this report
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A description of this report explaining the purpose
        /// </summary>
        string Description { get; set; }
        
        /// <summary>
        /// A URL which when invoked is supposed to return data in JSON format
        /// </summary>
        string DatasourceUrl { get; set; }

        /// <summary>
        /// Determines whether generated report files are allowed to be stored
        /// </summary>
        bool StorageAllowed { get; set; }

        DateTimeOffset CreatedOn { get; set; }
        DateTimeOffset UpdatedOn { get; set; }
        DateTimeOffset DeletedOn { get; set; }
    }
}