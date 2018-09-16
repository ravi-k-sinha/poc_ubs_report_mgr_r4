namespace UBS.ReportManager.Abstractions.Model.Communication
{
    using System.Collections.Generic;
    using Domain;

    public class ReportCreationResponse
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public static List<ReportCreationResponse> FromCreatedReports(List<Report> createdReports)
        {
            var response = new List<ReportCreationResponse>();
            createdReports.ForEach(newReport => response.Add(new ReportCreationResponse
            {
                Id = newReport.Id,
                Code = newReport.Code,
                Name = newReport.Name
            }));
            return response;
        }
    }
}