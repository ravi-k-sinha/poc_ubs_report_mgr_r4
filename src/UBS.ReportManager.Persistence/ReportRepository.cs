namespace UBS.ReportManager.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Abstractions.Model.Domain;
    using Abstractions.Repository;

    public class ReportRepository : IReportRepository
    {
        public Task<IReport> GetReport(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<IReport>> GetAllReports(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<IReport>> AddReports(List<IReport> newReports)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<IReport>> UpdateReports(List<IReport> newReports)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<IReport>> DeleteReport(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}