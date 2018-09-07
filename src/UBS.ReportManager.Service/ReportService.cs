namespace UBS.ReportManager.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abstractions.Model.Domain;
    using Abstractions.Service;

    public class ReportService : IReportService
    {
        public Task<IReport> GetReport(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> AddReports(List<Report> newReports)
        {
            throw new System.NotImplementedException();
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