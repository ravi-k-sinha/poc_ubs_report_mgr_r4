namespace UBS.ReportManager.Service
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Security;
    using System.Text;
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
    using Microsoft.AspNetCore.JsonPatch;
    using Newtonsoft.Json;

    public class ReportService : IReportService
    {
        private ILogger Logger { get; }
        private IReportRepository ReportRepository { get; }
        private IServiceProvider ServiceProvider { get; }
        private ITenantTime TenantTime { get; }
        private ITokenReader TokenReader { get; }
        private IReportingService ReportingService { get; }
        private IRenderService RenderService { get; }

        public ReportService(IServiceProvider serviceProvider, IReportRepository reportRepository,
            ITenantTime tenantTime, ITokenReader tokenReader, IReportingService reportingService, 
            IRenderService renderService, ILogger logger)
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
                   throw new NotFoundException(ReportUtil.GetNotFoundExMsgForIdOrCode(idOrCode));
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
            catch (ReportCreationException rse)
            {
                throw new InvalidArgumentException(rse.Message);
            }

            return true;
        }

        public Task<bool> UpdateReports(List<Report> updatedReports)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateReport(string idOrCode, JsonPatchDocument<Report> reportPatch)
        {
            var report = await ReportRepository.GetReport(idOrCode) ??
                         throw new ReportNotFoundException(ReportUtil.GetNotFoundExMsgForIdOrCode(idOrCode));

            var copiedReport = (Report) JsonConvert.DeserializeObject(JsonConvert.SerializeObject(report), typeof(Report));
            
            reportPatch.ApplyTo(copiedReport);

            var valid = ReportUtil.ValidatePatchedReport((Report)report, copiedReport, out var errors);

            if (!valid)
            {
                var sb = new StringBuilder();
                errors.ForEach(e => sb.Append(e + ", "));
                throw new InvalidArgumentException($"Errors in the patch submitted : {sb}");
            }

            copiedReport.TenantId = report.TenantId; // TenantId is marked with [JsonIgnore], so need to copy it
            copiedReport.UpdatedOn = TenantTime.Now;
            ReportRepository.Update(copiedReport);

            return true;
        }

        public async Task<bool> SetReportActiveStatus(string idOrCode, bool activeStatus)
        {
            var report = await ReportRepository.GetReport(idOrCode) ??
                throw new ReportNotFoundException(ReportUtil.GetNotFoundExMsgForIdOrCode(idOrCode));

            report.Active = activeStatus;
            report.UpdatedOn = TenantTime.Now;
            ReportRepository.Update(report);
            return true;
        }

        public async Task<bool> DeleteReport(string idOrCode)
        {
            var report = await ReportRepository.GetReport(idOrCode) ??
                throw new ReportNotFoundException(ReportUtil.GetNotFoundExMsgForIdOrCode(idOrCode));

            report.DeletedOn = TenantTime.Now;
            report.UpdatedOn = TenantTime.Now;
            ReportRepository.Update(report);
            return true;
        }

        public async Task<GeneratedJsReportData> GenerateReport(string id, string reportParams)
        {
            var report = await GetReport(id);

            if (!string.IsNullOrWhiteSpace(report.InputJsonSchema)
                && !ReportUtil.ValidAsPerSchema(reportParams, report.InputJsonSchema, out var errors))
            {
                var sb = new StringBuilder();
                foreach (var error in errors)
                {
                    sb.Append(error); 
                    sb.Append(',');
                }
                
                throw new InvalidArgumentException($"Input params are missing or invalid. Errors=[{sb}]");
            }

            var dsUrl = report.DatasourceUrl;

            Uri dsUri;

            try
            {
                dsUri = new Uri(dsUrl);
            }
            catch (Exception)
            {
                throw new InvalidArgumentException(
                    $"Data-source url=[{dsUrl}] associated with the report is not valid");
            }

            string reportJsonData;
            
            try
            {
                reportJsonData = await ReportUtil.FetchJsonData(ServiceProvider, TokenReader, dsUri, reportParams);
            }
            catch (Exception e)
            {
                // TODO Change this to throw 500, when the framework can support sending specific messages with 500 code.
                // Currently the default 'We couldn't process your request' is sent
                throw new InvalidArgumentException($"Could not fetch report data from data-source=[{dsUrl}]. " +
                                                   $"Data-source exception message is [{e.Message}]", e);
            }

            if (!string.IsNullOrWhiteSpace(report.DataJsonSchema)
                && !ReportUtil.ValidAsPerSchema(reportJsonData, report.DataJsonSchema, out var errors2))
            {
                var sb = new StringBuilder();
                foreach (var error in errors2)
                {
                    sb.Append(error); 
                    sb.Append(',');
                }

                throw new InvalidArgumentException($"Report data was not received or invalid. Errors=[{sb}]");
            }

            jsreport.Types.Report jsReport;
            try
            {
                jsReport = await ReportingService.RenderAsync(report.JsReportTemplate.Code, reportJsonData);
            }
            catch (HttpRequestException hre)
            {
                throw new InvalidArgumentException($"Cannot connect to JSReport server. Error=[{hre.Message}]", hre);
            }
            catch (JsReportException jre)
            {
                throw new InvalidArgumentException($"JsReport server could not generate the report. Error=[{jre.Message}]", jre);
            }

            return new GeneratedJsReportData(jsReport.Content, report.GeneratedFileName, report.GeneratedFileExtension);
        }
    }
}