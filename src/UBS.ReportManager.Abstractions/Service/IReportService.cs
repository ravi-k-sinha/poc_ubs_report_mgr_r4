namespace UBS.ReportManager.Abstractions.Service
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.JsonPatch;
    using Model.Domain;
    using Model.Operational;

    public interface IReportService
    {
        Task<IReport> GetReport(string idOrCode, bool includeDeleted = false);
        Task<List<IReport>> GetAllReports(bool includeDeleted = false);
        Task<bool> AddReports(List<Report> newReports);
        Task<bool> UpdateReports(List<Report> updatedReports);
        Task<bool> SetReportActiveStatus(string idOrCode, bool activeStatus);
        Task<bool> DeleteReport(string idOrCode);
        Task<GeneratedJsReportData> GenerateReport(string id, string reportParams);
        Task<bool> UpdateReport(string idOrCode, JsonPatchDocument<Report> reportPatch);
    }
}