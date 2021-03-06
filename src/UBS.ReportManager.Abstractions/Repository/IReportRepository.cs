namespace UBS.ReportManager.Abstractions.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LendFoundry.Foundation.Persistence;
    using Model.Domain;

    public interface IReportRepository : IRepository<IReport>
    {
        Task<IReport> GetReport(string idOrCode, bool includeDeleted = false);
        Task<List<IReport>> GetAllReports(bool includeDeleted = false);
        
        Task<List<IReport>> AddReports(List<IReport> newReports);
    }
}