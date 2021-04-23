using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eConnectApi.Models
{
    public class GLHeader
    {
        public string BACHNUMB { get; set; }
        public int JRNENTRY { get; set; }
        public string REFRENCE { get; set; }
        public DateTime TRXDATE { get; set; }
        public DateTime RVRSNGDT { get; set; }
        public int TRXTYPE { get; set; }
        public int SQNCLINE { get; set; }
        public int SERIES { get; set; }
        public string CURNCYID { get; set; }
        public decimal XCHGRATE { get; set; }
        public string RATETPID { get; set; }
        public DateTime EXPNDATE { get; set; }
        public DateTime EXCHDATE { get; set; }
        public string EXGTBDSC { get; set; }
        public string EXTBLSRC { get; set; }
        public int RATEEXPR { get; set; }
        public int DYSTINCR { get; set; }
        public decimal RATEVARC { get; set; }
        public int TRXDTDEF { get; set; }
        public int RTCLCMTD { get; set; }
        public int PRVDSLMT { get; set; }
        public int DATELMTS { get; set; }
        public DateTime TIME1 { get; set; }
        public int RequesterTrx { get; set; }
        public string SOURCDOC { get; set; }
        public int Ledger_ID { get; set; }
        public string USERID { get; set; }
        public bool Adjustment_Transaction { get; set; }
        public string NOTETEXT { get; set; }
        public string USRDEFND1 { get; set; }
        public string USRDEFND2 { get; set; }
        public string USRDEFND3 { get; set; }
        public string USRDEFND4 { get; set; }
        public string USRDEFND5 { get; set; }

        public IEnumerable<GLLine> Lines { get; set; }

        public GLHeader()
        {
            SERIES = 2;
            CURNCYID = 
                RATETPID =
                EXGTBDSC =
                EXTBLSRC =
                SOURCDOC =
                USERID =
                NOTETEXT =
                USRDEFND1 =
                USRDEFND2 =
                USRDEFND3 =
                USRDEFND4 =
                USRDEFND5 =
                string.Empty;
            RATEEXPR = -1;
            DYSTINCR = -1;
            TRXDTDEF = -1;
            RTCLCMTD = -1;
            Ledger_ID = 1;


            Lines = new List<GLLine>();
        }
    }
}
