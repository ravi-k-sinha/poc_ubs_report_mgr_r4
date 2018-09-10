namespace UBS.ReportManager.Service
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abstractions.Model.Domain;
    using Abstractions.Repository;
    using Abstractions.Service;
    using LendFoundry.Foundation.Logging;

    public class ReportService : IReportService
    {
        private ILogger Logger { get; }
        private IReportRepository ReportRepository { get; }

        public ReportService(IReportRepository reportRepository, ILogger logger)
        {
            ReportRepository = reportRepository ?? throw new ArgumentException(nameof(reportRepository));
            Logger = logger ?? throw new ArgumentException(nameof(logger));
        }


        public async Task<IReport> GetReport(string id)
        {
            return await ReportRepository.GetReport(id);
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

        public Task<bool> DeleteReport(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> GenerateReport(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}