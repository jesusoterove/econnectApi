using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eConnectApi.Models
{
    public class eConnectError
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorList { get; set; }
    }
}
