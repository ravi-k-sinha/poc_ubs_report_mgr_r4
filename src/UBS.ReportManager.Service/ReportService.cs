namespace UBS.ReportManager.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abstractions.Model.Domain;
    using Abstractions.Model.Exception;
    using Abstractions.Repository;
    using Abstractions.Service;
    using LendFoundry.Foundation.Date;
    using LendFoundry.Foundation.Logging;

    public class ReportService : IReportService
    {
        private ILogger Logger { get; }
        private IReportRepository ReportRepository { get; }
        private ITenantTime TenantTime { get; }

        public ReportService(IReportRepository reportRepository, ITenantTime tenantTime, ILogger logger)
        {
            ReportRepository = reportRepository ?? throw new ArgumentException(nameof(reportRepository));
            TenantTime = tenantTime ?? throw new ArgumentException(nameof(tenantTime));
            Logger = logger ?? throw new ArgumentException(nameof(logger));
        }


        public async Task<IReport> GetReport(string id)
        {
            return await ReportRepository.GetReport(id);
        }

        public async Task<List<IReport>> GetAllReports(bool includeDeleted = false)
        {
            return await ReportRepository.GetAllReports(includeDeleted);
        }

        public async Task<bool> AddReports(List<Report> newReports)
        {
            var interfaceTyped = new List<IReport>();
            newReports.ForEach(r => interfaceTyped.Add(r));
            
            await ReportRepository.AddReports(interfaceTyped);
            return true;
        }

        public Task<bool> UpdateReports(List<Report> updatedReports)
        {
            throw new System.NotImplementedException();
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

        public Task<bool> GenerateReport(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}