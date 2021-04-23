using eConnectApi.Models;
using Microsoft.Dynamics.GP.eConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using eConnectApi.Extensions;
using System.Data.SqlClient;
using System.Web.Http;
using System.Configuration;

namespace eConnectApi.Controllers
{
    [RoutePrefix("api/gl")]
    public class GLController: ApiController
    {
        private string _cnnString;

        public GLController()
        {
            var cnn = ConfigurationManager.ConnectionStrings["eConnect"];
            if (cnn != null)
                _cnnString = cnn.ConnectionString;
        }

        /// <summary>
        /// Just a test endpoint to make sure routing is working
        /// </summary>
        /// <returns></returns>
        [Route("time")]
        [HttpGet]
        public string GetTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }

        /// <summary>
        /// Get the next journal entry number by using the GetNextDocNumbers method provided by eConnect assemblies
        /// </summary>
        /// <returns></returns>
        [Route("nextjournal")]
        [HttpGet]
        public eConnectTransactionResult GetNextJournalNumber()
        {
            GetNextDocNumbers docNumber = new GetNextDocNumbers();
            try
            {
                if (string.IsNullOrEmpty(_cnnString))
                    throw new Exception("Missing configuration for eConnect");

                string journalNumber = docNumber.GetNextGLJournalEntryNumber(IncrementDecrement.Increment, _cnnString);
                return new eConnectTransactionResult
                {
                    Succeed = true,
                    Data = journalNumber,
                    ErrorMessage = string.Empty
                };
            }
            catch (Exception ex)
            {
                return new eConnectTransactionResult
                {
                    Succeed = false,
                    ErrorMessage = ex.Message,
                    Data = ex
                };
            }
        }

        /// <summary>
        /// Creates a Journal entry by calling eConnect using eConnect assemblies and the eConnect service
        /// </summary>
        /// <param name="gl"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("transaction")]
        public eConnectTransactionResult CreateJournal([FromBody]GLHeader gl)
        {
            eConnectMethods eConnect = new eConnectMethods();
            string eConnectXML = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(_cnnString))
                    throw new Exception("Missing configuration for eConnect");

                eConnectXML = GLToXML(gl);
                eConnect.CreateTransactionEntity(_cnnString, eConnectXML);
                return new eConnectTransactionResult
                {
                    Succeed = true,
                    Data = gl.JRNENTRY,
                    ErrorMessage = string.Empty
                };
            }
            catch (eConnectException eEx)
            {
                return new eConnectTransactionResult
                {
                    Succeed = false,
                    ErrorMessage = String.Format("eConnect call failed due: {0}", eEx.Message),
                    Data = eConnectXML
                };
            }
            catch (Exception ex)
            {
                return new eConnectTransactionResult
                {
                    Succeed = false,
                    ErrorMessage = ex.Message,
                    Data = eConnectXML
                };
            }
        }

        /// <summary>
        /// Creates a Journal Entry by calling eConnect ta procedures manually
        /// </summary>
        /// <param name="gl"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("sqltransaction")]
        public eConnectTransactionResult CreateJournalSql([FromBody] GLHeader gl)
        {
            eConnectError error = null;
            SqlConnection cnn = null;
            SqlTransaction trx = null;
            SqlCommand command = null;
            try
            {
                cnn = new SqlConnection(_cnnString);
                cnn.Open();
                trx = cnn.BeginTransaction();
                command = cnn.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Transaction = trx;
                foreach (var line in gl.Lines)
                {
                    command.CommandText = "taGLTransactionLineInsert";
                    command.Parameters.Clear();
                    command.Parameters.AddRange(BuildGLLineParameters(gl, line));
                    command.ExecuteNonQuery();

                    error = HandleEConnectErrorState(command.Parameters);
                    if (error != null)
                    {
                        throw new eConnectCustomException("taGLTransactionLineInsert", error);
                    }
                }

                command.CommandText = "taGLTransactionHeaderInsert";
                command.Parameters.Clear();
                command.Parameters.AddRange(BuildGLHdrParameters(gl));
                command.ExecuteNonQuery();

                error = HandleEConnectErrorState(command.Parameters);
                if (error != null)
                {
                    throw new eConnectCustomException("taGLTransactionHeaderInsert", error);
                }
                //Complete transaction
                trx.Commit();
                return new eConnectTransactionResult
                {
                    Succeed = true,
                    Data = gl.JRNENTRY,
                    ErrorMessage = string.Empty
                };
            }
            catch (eConnectCustomException ex)
            {
                //Cancel transaction
                if (trx != null) trx.Rollback();

                return new eConnectTransactionResult
                {
                    Succeed = false,
                    ErrorMessage = ex.Message,
                    Data = ex.Error
                };
            }
            catch (Exception ex)
            {
                //Cancel transaction
                if (trx != null) trx.Rollback();

                return new eConnectTransactionResult
                {
                    Succeed = false,
                    ErrorMessage = ex.Message,
                    Data = ex
                };
            }
            finally
            {
                if (trx != null) trx.Dispose();
                if (command != null) command.Dispose();
                if (cnn != null)
                {
                    cnn.Close();
                    cnn.Dispose();
                }
                trx = null;
                command = null;
                cnn = null;
            }
        }

        private SqlParameter[] BuildGLLineParameters(GLHeader hdr, GLLine line)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@I_vBACHNUMB", hdr.BACHNUMB));
            parameters.Add(new SqlParameter("@I_vJRNENTRY", hdr.JRNENTRY));
            parameters.Add(new SqlParameter("@I_vSQNCLINE", line.SQNCLINE));
            parameters.Add(new SqlParameter("@I_vCRDTAMNT", line.CRDTAMNT));
            parameters.Add(new SqlParameter("@I_vDEBITAMT", line.DEBITAMT));
            parameters.Add(new SqlParameter("@I_vACTNUMST", line.ACTNUMST));
            parameters.Add(new SqlParameter("@I_vDSCRIPTN", line.DSCRIPTN));
            parameters.Add(new SqlParameter("@I_vDOCDATE", hdr.TRXDATE.ToGPDate()));
            parameters.Add(new SqlParameter("@I_vCURNCYID", line.CURNCYID));
            SqlParameter inOut = new SqlParameter("@O_iErrorState", System.Data.SqlDbType.Int, 4); //, System.Data.ParameterDirection.Output, true, 0, 0, "", System.Data.DataRowVersion.Current, null);
            inOut.Direction = System.Data.ParameterDirection.Output;
            parameters.Add(inOut);
            inOut = new SqlParameter("@oErrString", System.Data.SqlDbType.VarChar, 255); //, System.Data.ParameterDirection.Output, true, 0, 0, "", System.Data.DataRowVersion.Current, null);
            inOut.Direction = System.Data.ParameterDirection.Output;
            parameters.Add(inOut);
            return parameters.ToArray();
        }

        private SqlParameter[] BuildGLHdrParameters(GLHeader hdr)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@I_vBACHNUMB", hdr.BACHNUMB));
            parameters.Add(new SqlParameter("@I_vJRNENTRY", hdr.JRNENTRY));
            parameters.Add(new SqlParameter("@I_vREFRENCE", hdr.REFRENCE));
            parameters.Add(new SqlParameter("@I_vTRXDATE", hdr.TRXDATE.ToGPDate()));
            parameters.Add(new SqlParameter("@I_vRVRSNGDT", hdr.RVRSNGDT.ToGPDate()));
            parameters.Add(new SqlParameter("@I_vTRXTYPE", hdr.TRXTYPE));
            parameters.Add(new SqlParameter("@I_vSQNCLINE", hdr.SQNCLINE));
            parameters.Add(new SqlParameter("@I_vUSRDEFND1", hdr.USRDEFND1));

            SqlParameter inOut = new SqlParameter("@O_iErrorState", System.Data.SqlDbType.Int, 4); //, System.Data.ParameterDirection.Output, true, 0, 0, "", System.Data.DataRowVersion.Current, null);
            inOut.Direction = System.Data.ParameterDirection.Output;
            parameters.Add(inOut);
            inOut = new SqlParameter("@oErrString", System.Data.SqlDbType.VarChar, 255); //, System.Data.ParameterDirection.Output, true, 0, 0, "", System.Data.DataRowVersion.Current, null);
            inOut.Direction = System.Data.ParameterDirection.Output;
            parameters.Add(inOut);
            return parameters.ToArray();
        }

        private eConnectError HandleEConnectErrorState(SqlParameterCollection parameters)
        {
            var errorStateParam = parameters["@O_iErrorState"];
            var errorStringParam = parameters["@oErrString"];
            int errorState = Convert.ToInt32(errorStateParam.Value);
            if (errorState != 0)
            {
                return new eConnectError
                {
                    ErrorCode = errorState,
                    ErrorList = errorStringParam.Value as string,
                    Message = "eConnect procedure call failed" //This should be enhanced by getting the error description from DYNAMICS..taErrorCode
                };
            }
            return null;
        }

        private string GLToXML(GLHeader gl)
        {
            XElement lines = new XElement("taGLTransactionLineInsert_Items");
            foreach (var line in gl.Lines)
            {
                lines.Add(new XElement("taGLTransactionLineInsert",
                    new XElement("BACHNUMB", gl.BACHNUMB),
                    new XElement("JRNENTRY", gl.JRNENTRY),
                    new XElement("SQNCLINE", line.SQNCLINE),
                    new XElement("ACTINDX", line.ACTINDX),
                    new XElement("CRDTAMNT", line.CRDTAMNT),
                    new XElement("DEBITAMT", line.DEBITAMT),
                    new XElement("ACTNUMST", line.ACTNUMST),
                    new XElement("DSCRIPTN", line.DSCRIPTN),
                    new XElement("ORCTRNUM", line.ORCTRNUM),
                    new XElement("ORDOCNUM", line.ORDOCNUM),
                    new XElement("ORMSTRID", line.ORMSTRID),
                    new XElement("ORMSTRNM", line.ORMSTRNM),
                    new XElement("ORTRXTYP", line.ORTRXTYP),
                    new XElement("OrigSeqNum", line.OrigSeqNum),
                    new XElement("ORTRXDESC", line.ORTRXDESC),
                    new XElement("TAXDTLID", line.TAXDTLID),
                    new XElement("TAXAMNT", line.TAXAMNT),
                    new XElement("TAXACTNUMST", line.TAXACTNUMST),
                    new XElement("DOCDATE", gl.TRXDATE.ToGPDate()),
                    new XElement("CURNCYID", line.CURNCYID),
                    new XElement("XCHGRATE", line.XCHGRATE),
                    new XElement("RATETPID", line.RATETPID),
                    new XElement("EXPNDATE", line.EXPNDATE.ToGPDate()),
                    new XElement("EXCHDATE", line.EXCHDATE.ToGPDate()),
                    new XElement("EXGTBDSC", line.EXGTBDSC),
                    new XElement("EXTBLSRC", line.EXTBLSRC),
                    new XElement("RATEEXPR", line.RATEEXPR),
                    new XElement("DYSTINCR", line.DYSTINCR),
                    new XElement("RATEVARC", line.RATEVARC),
                    new XElement("TRXDTDEF", line.TRXDTDEF),
                    new XElement("RTCLCMTD", line.RTCLCMTD),
                    new XElement("PRVDSLMT", line.PRVDSLMT),
                    new XElement("DATELMTS", line.DATELMTS),
                    new XElement("TIME1", line.TIME1.ToGPDate()),
                    new XElement("RequesterTrx", line.RequesterTrx),
                    new XElement("USRDEFND1", line.USRDEFND1),
                    new XElement("USRDEFND2", line.USRDEFND2),
                    new XElement("USRDEFND3", line.USRDEFND3),
                    new XElement("USRDEFND4", line.USRDEFND4),
                    new XElement("USRDEFND5", line.USRDEFND5)
                ));
            }
            XElement glTrx = new XElement("taGLTransactionHeaderInsert",
                new XElement("BACHNUMB", gl.BACHNUMB),
                new XElement("JRNENTRY", gl.JRNENTRY),
                new XElement("REFRENCE", gl.REFRENCE),
                new XElement("TRXDATE", gl.TRXDATE.ToGPDate()),
                new XElement("RVRSNGDT", gl.RVRSNGDT.ToGPDate()),
                new XElement("TRXTYPE", gl.TRXTYPE),
                new XElement("SQNCLINE", gl.SQNCLINE),
                new XElement("SERIES", gl.SERIES),
                new XElement("CURNCYID", gl.CURNCYID),
                new XElement("XCHGRATE", gl.XCHGRATE),
                new XElement("RATETPID", gl.RATETPID),
                new XElement("EXPNDATE", gl.EXPNDATE.ToGPDate()),
                new XElement("EXCHDATE", gl.EXCHDATE.ToGPDate()),
                new XElement("EXGTBDSC", gl.EXGTBDSC),
                new XElement("EXTBLSRC", gl.EXTBLSRC),
                new XElement("RATEEXPR", gl.RATEEXPR),
                new XElement("DYSTINCR", gl.DYSTINCR),
                new XElement("RATEVARC", gl.RATEVARC),
                new XElement("TRXDTDEF", gl.TRXDTDEF),
                new XElement("RTCLCMTD", gl.RTCLCMTD),
                new XElement("PRVDSLMT", gl.PRVDSLMT),
                new XElement("DATELMTS", gl.DATELMTS),
                new XElement("TIME1", gl.TIME1.ToGPDate()),
                new XElement("RequesterTrx", gl.RequesterTrx),
                new XElement("SOURCDOC", gl.SOURCDOC),
                new XElement("Ledger_ID", gl.Ledger_ID),
                new XElement("USERID", gl.USERID),
                new XElement("Adjustment_Transaction", gl.Adjustment_Transaction ? 1 : 0),
                new XElement("NOTETEXT", gl.NOTETEXT),
                new XElement("USRDEFND1", gl.USRDEFND1),
                new XElement("USRDEFND2", gl.USRDEFND2),
                new XElement("USRDEFND3", gl.USRDEFND3),
                new XElement("USRDEFND4", gl.USRDEFND4),
                new XElement("USRDEFND5", gl.USRDEFND5),
                new XElement("USRDEFND5", gl.USRDEFND5)
            );


            XDocument doc = new XDocument(
                new XElement("eConnect",
                    new XElement("GLTransactionType",
                    lines,
                    glTrx)
                ));

            return doc.ToString();
        }
    }
}
