namespace UBS.ReportManager.Abstractions.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LendFoundry.Foundation.Persistence;
    using Model.Domain;

    public interface IReportRepository : IRepository<IReport>
    {
        Task<IReport> GetReport(string id);
        Task<List<IReport>> GetAllReports();
        
        Task<List<IReport>> AddReports(List<IReport> newReports);
  
        Task<List<IReport>> UpdateReports(List<IReport> newReports);
     
        Task<List<IReport>> DeleteReport(string id);
    }
}