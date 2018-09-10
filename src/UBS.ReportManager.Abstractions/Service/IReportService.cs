namespace UBS.ReportManager.Abstractions.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model.Domain;

    public interface IReportService
    {
        Task<IReport> GetReport(string id);
        Task<List<IReport>> GetAllReports();
        Task<bool> AddReports(List<Report> newReports);
        Task<bool> UpdateReports(List<Report> updatedReports);
        Task<bool> DeleteReport(string id);
        Task<bool> GenerateReport(string id);
    }
}