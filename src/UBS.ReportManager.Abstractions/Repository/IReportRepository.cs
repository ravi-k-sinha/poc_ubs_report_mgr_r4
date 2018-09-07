namespace UBS.ReportManager.Abstractions.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model.Domain;

    public interface IReportRepository
    {
        Task<IReport> GetReport(string id);
        Task<List<IReport>> GetAllReports(string id);
        
        Task<List<IReport>> AddReports(List<IReport> newReports);
  
        Task<List<IReport>> UpdateReports(List<IReport> newReports);
     
        Task<List<IReport>> DeleteReport(string id);
    }
}