namespace UBS.ReportManager.Abstractions.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model.Domain;
    using Model.Operational;

    public interface IReportService
    {
        Task<IReport> GetReport(string idOrCode, bool includeDeleted = false);
        Task<List<IReport>> GetAllReports(bool includeDeleted = false);
        Task<bool> AddReports(List<Report> newReports);
        Task<bool> UpdateReports(List<Report> updatedReports);
        Task<bool> DeleteReport(string id);
        Task<GeneratedJsReportData> GenerateReport(string id, string reportParams);
    }
}