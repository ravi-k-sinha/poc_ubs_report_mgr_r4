namespace UBS.ReportManager.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Abstractions.Model.Domain;
    using Abstractions.Model.Exception;
    using Abstractions.Model.Operational;
    using Abstractions.Repository;
    using Abstractions.Service;
    using jsreport.Client;
    using jsreport.Shared;
    using LendFoundry.Foundation.Date;
    using LendFoundry.Foundation.Logging;
    using LendFoundry.Foundation.Services;
    using LendFoundry.Security.Tokens;

    public class ReportService : IReportService
    {
        private ILogger Logger { get; }
        private IReportRepository ReportRepository { get; }
        private IServiceProvider ServiceProvider { get; }
        private ITenantTime TenantTime { get; }
        private ITokenReader TokenReader { get; }
        private IReportingService ReportingService { get; }
        private IRenderService RenderService { get; }

        public ReportService(IServiceProvider serviceProvider, IReportRepository reportRepository, ITenantTime tenantTime,
            ITokenReader tokenReader, IReportingService reportingService, IRenderService renderService, ILogger logger)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentException(nameof(tenantTime));
            TenantTime = tenantTime ?? throw new ArgumentException(nameof(tenantTime));
            TokenReader = tokenReader ?? throw new ArgumentException(nameof(tokenReader));
            // Read the token so that we are sure that Authorization Bearer token is present & is valid
            // Else the invocation should result in 401 being returned
            tokenReader.Read();
            
            ReportRepository = reportRepository ?? throw new ArgumentException(nameof(reportRepository));
            ReportingService = reportingService ?? throw new ArgumentException(nameof(reportingService));
            RenderService = renderService ?? throw new ArgumentException(nameof(renderService));
            Logger = logger ?? throw new ArgumentException(nameof(logger));
        }

        public async Task<IReport> GetReport(string idOrCode, bool includeDeleted = false)
        {
            return await ReportRepository.GetReport(idOrCode, includeDeleted) ??
                   throw new NotFoundException($"A report was not found with id/code={idOrCode}");
        }

        public async Task<List<IReport>> GetAllReports(bool includeDeleted = false)
        {
            return await ReportRepository.GetAllReports(includeDeleted);
        }

        public async Task<bool> AddReports(List<Report> newReports)
        {
            var interfaceTyped = new List<IReport>();
            newReports.ForEach(r => interfaceTyped.Add(r));
            try
            {
                await ReportRepository.AddReports(interfaceTyped);
            }
            catch (ReportStorageException rse)
            {
                throw new InvalidArgumentException(rse.Message);
            }

            return true;
        }

        public Task<bool> UpdateReports(List<Report> updatedReports)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteReport(string id)
        {
            var report = await ReportRepository.GetReport(id);

            if (report == null)
            {
                throw new ReportNotFoundException($"A report was not found with id={id}");
            }

            report.DeletedOn = TenantTime.Now;
            ReportRepository.Update(report);
            return true;
        }

        public async Task<GeneratedJsReportData> GenerateReport(string id, string reportParams)
        {
            var report = await GetReport(id);
            
            // TODO Check reportParams is not null, if input was expected
            // TODO Check if reportParams is as per schema

            var dsUrl = report.DatasourceUrl;

            // TODO If dsUri is not defined, then jsreport can be directly invoked
            // TODO Check either both dsUrl and dataSchema are present or absent
            
            Uri dsUri;

            try
            {
                dsUri = new Uri(dsUrl);
            }
            catch (Exception)
            {
                throw new InvalidArgumentException($"Data-source url=[{dsUrl}] associated with the report is not valid");
            }

            var reportJsonData = await ReportUtil.FetchJsonData(ServiceProvider, TokenReader, dsUri, reportParams);

            // TODO Check whether reportJsonData is as per schema
            
            // TODO Handle exception when Reporting service is not running
            // TODO Handle exception when referred template is not available
            // TODO Handle any other exception when generating the report
            var jsReport = await ReportingService.RenderByNameAsync(report.JsReportTemplate.Name, reportJsonData);
            
            Logger.Info("Reached current end of implementation");

            // TODO file name should be part of the model, maybe with a timestamp
            return new GeneratedJsReportData(jsReport.Content, report.GeneratedFileName, report.GeneratedFileExtension);
        }
    }
}