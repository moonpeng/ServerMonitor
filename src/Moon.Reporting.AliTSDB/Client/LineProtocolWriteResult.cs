using System;
using System.Collections.Generic;
using System.Text;

namespace Moon.Reporting.AliTSDB.Client
{
    public struct LineProtocolWriteResult
    {
        public LineProtocolWriteResult(bool success)
        {
            Success = success;
            ErrorMessage = null;
        }

        public LineProtocolWriteResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }

        public bool Success { get; }
    }
}
