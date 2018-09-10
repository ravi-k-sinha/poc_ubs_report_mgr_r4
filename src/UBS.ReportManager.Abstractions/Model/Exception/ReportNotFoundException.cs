namespace UBS.ReportManager.Abstractions.Model.Exception
{
    using System;

    public class ReportNotFoundException : Exception
    {
        public ReportNotFoundException() : base("The report was not found")
        {
        }

        public ReportNotFoundException(string message) : base(message)
        {
        }
    }
}