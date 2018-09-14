namespace UBS.ReportManager.Abstractions.Model.Operational
{
    using System.IO;

    /// <summary>
    /// This value class is used for gathering important data about a generated JS report
    /// </summary>
    public class GeneratedJsReportData
    {
        public Stream Content { get;}
        public string Name { get; }
        public string Extension { get; }

        public GeneratedJsReportData(Stream content, string name, string extension)
        {
            Content = content;
            Name = name;
            Extension = extension;
        }
    }
}