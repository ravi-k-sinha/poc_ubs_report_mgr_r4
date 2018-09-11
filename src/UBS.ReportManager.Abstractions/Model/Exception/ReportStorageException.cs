namespace UBS.ReportManager.Abstractions.Model.Exception
{
    using System;

    public class ReportStorageException : Exception
    {
        public ReportStorageException() : base("There was a problem inserting/updating report(s)")
        {
        }

        public ReportStorageException(string message) : base(message)
        {
        }
    }
}