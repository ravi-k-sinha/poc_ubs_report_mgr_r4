namespace UBS.ReportManager.Abstractions.Service
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Model.Domain;

    public interface IReportService
    {
        Task<IReport> GetReport(string id, bool includeDeleted = false);
        Task<List<IReport>> GetAllReports(bool includeDeleted = false);
        Task<bool> AddReports(List<Report> newReports);
        Task<bool> UpdateReports(List<Report> updatedReports);
        Task<bool> DeleteReport(string id);
        Task<Stream> GenerateReport(string id);
    }
}