namespace UBS.ReportManager.Service
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Abstractions.Model.Domain;
    using Abstractions.Model.Exception;
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
        private ITenantTime TenantTime { get; }
        private ITokenReader TokenReader { get; }
        private IReportingService ReportingService { get; }
        private IRenderService RenderService { get; }

        public ReportService(IReportRepository reportRepository, ITenantTime tenantTime,
            ITokenReader tokenReader, IReportingService reportingService, IRenderService renderService, ILogger logger)
        {
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

        public async Task<IReport> GetReport(string id, bool includeDeleted = false)
        {
            try
            {
                return await ReportRepository.GetReport(id, includeDeleted) ??
                       throw new NotFoundException($"A report was not found with id={id}");
            }
            catch (FormatException)
            {
                throw new InvalidArgumentException($"The id={id} is not a valid identifier");
            }
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

        public async Task<Stream> GenerateReport(string id)
        {
            /*
             * 1. * Get the report
             * 2. * If error send 404, 400
             * 3. * Get the data-source endpoint
             * 4. * If endpoint is not valid, send 400
             * 5. Use utility to invoke endpoint and get json data
             * 6. Use jsreport client to conect to and invoke jsreport server with template code
             * 7. Include data received from the service (Maybe a check can be there that empty data was not received)
             * 7. Receive the PDF file from jsreport (or it probably can be any file format configured with the template)
             * 8. Resend the PDF file to the caller
             * 9. If storage is requested and allowed (to be done later, then store the report somewhere (S3, Mongo, etc.))
             */

            var report = await GetReport(id);
            var dsUrl = report.DatasourceUrl;

            Uri dsUri = null;

            try
            {
                dsUri = new Uri(dsUrl);
            }
            catch (Exception)
            {
                throw new InvalidArgumentException($"Data-source url=[{dsUrl}] associated with the report is not valid");
            }

            var jsReport = await ReportingService.RenderByNameAsync("Sample 2", new
            {
                data = new
                {
                    key1 = "X1",
                    key2 = "X2"
                }
            });
            
            Logger.Info("Reached current end of implementation");

            // TODO file name should be part of the model, maybe with a timestamp
            return jsReport.Content;
        }
    }
}