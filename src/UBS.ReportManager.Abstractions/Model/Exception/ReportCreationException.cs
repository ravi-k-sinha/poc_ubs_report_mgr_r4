namespace UBS.ReportManager.Abstractions.Model.Exception
{
    using System;

    public class ReportCreationException : Exception
    {
        public ReportCreationException() : base("There was a problem inserting/updating report(s)")
        {
        }

        public ReportCreationException(string message) : base(message)
        {
        }
    }
}