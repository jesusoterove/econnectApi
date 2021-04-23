using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eConnectApi.Models
{
    public class eConnectCustomException : Exception
    {
        public eConnectError Error { get; protected set; }
        public eConnectCustomException(string message, eConnectError error) : base(message)
        {
            Error = error;
        }
    }
}