using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eConnectApi.Models
{
    public class eConnectTransactionResult
    {
        public bool Succeed { get; set; }
        public string ErrorMessage { get; set; }
        public object Data { get; set; }
    }
}
