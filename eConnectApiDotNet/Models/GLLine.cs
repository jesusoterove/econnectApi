using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eConnectApi.Models
{
    public class GLLine
    {
        public int SQNCLINE { get; set; }
        public int ACTINDX { get; set; }
        public decimal CRDTAMNT { get; set; }
        public decimal DEBITAMT { get; set; }
        public string ACTNUMST { get; set; }
        public string DSCRIPTN { get; set; }
        public string ORCTRNUM { get; set; }
        public string ORDOCNUM { get; set; }
        public string ORMSTRID { get; set; }
        public string ORMSTRNM { get; set; }
        public int ORTRXTYP { get; set; }
        public int OrigSeqNum { get; set; }
        public string ORTRXDESC { get; set; }
        public string TAXDTLID { get; set; }
        public decimal TAXAMNT { get; set; }
        public string TAXACTNUMST { get; set; }
        public string CURNCYID { get; set; }
        public string XCHGRATE { get; set; }
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
        public string PRVDSLMT { get; set; }
        public string DATELMTS { get; set; }
        public DateTime TIME1 { get; set; }
        public int RequesterTrx { get; set; }
        public string USRDEFND1 { get; set; }
        public string USRDEFND2 { get; set; }
        public string USRDEFND3 { get; set; }
        public string USRDEFND4 { get; set; }
        public string USRDEFND5 { get; set; }

        public GLLine()
        {
            ACTNUMST =
                DSCRIPTN =
                ORCTRNUM =
                ORDOCNUM =
                ORMSTRID =
                ORMSTRNM =
                ORTRXDESC =
                TAXDTLID =
                TAXACTNUMST =
                CURNCYID =
                RATETPID =
                EXGTBDSC =
                EXTBLSRC =
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
        }
    }
}
