# econnectApi
Sample of using dynamics GP eConnect  as a custom web service to integrate and read data from Microsoft Dynamics GP.

This is just a proof of concept and should not be used as it is for a production environment. At least Api Security must be added before using in production.
This sample is using eConnect Api for Microsoft Dynamics GP 2018.

When deploying make sure the Application Pool configured with the Api application has `Enable 32-Bit Application` = True on the Advanced Settings.

* Getting new Journal Entry Number
```
GET /eConnectApi/api/gl/nextjournal HTTP/1.1
Host: [YOUR_SERVER_NAME_OR_IP_ADDRESS]
Content-Type: application/json
Content-Length: 0
```

Output
Data will point to the new Journal Entry Number available on Microsoft Dynamics GP
```
{
    "Succeed": true,
    "ErrorMessage": "",
    "Data": "349401"
}
```

* Creating a new Journal Entry Number

Notice we are sending a value to USRDEFND1, in this case to indicate the Facility ID in a System which has Binary Stream Multi-Entity Management enabled

This endpoint will use eConnect assemblies for the integration
```
POST /eConnectApi/api/gl/transaction HTTP/1.1
Host: [YOUR_SERVER_NAME_OR_IP_ADDRESS]
Content-Type: application/json
Content-Length: 686

{
    "BACHNUMB": "JOGLAPI001",
    "JRNENTRY": 349401,
    "REFRENCE": "GL API ENTRY",
    "TRXDATE": "2021-04-21",
    "RVRSNGDT": "1900-01-01",
    "TRXTYPE": 0,
    "SQNCLINE": 0,
    "USRDEFND1": "200",
    "Lines": [
        {
            "SQNCLINE": 16384,
            "CRDTAMNT": 0,
            "DEBITAMT": 170,
            "ACTNUMST": "509-00-1200-000",
            "DSCRIPTN": "Line 1",
            "CURNCYID": "USD"
        },
        {
            "SQNCLINE": 32768,
            "CRDTAMNT": 170,
            "DEBITAMT": 0,
            "ACTNUMST": "509-00-1054-000",
            "DSCRIPTN": "Line 2",
            "CURNCYID": "USD"
        }
    ]
}
```

Succees Output
```
{
    "Succeed": true,
    "ErrorMessage": "",
    "Data": 349401
}
```

Failed Output
```
{
    "Succeed": false,
    "ErrorMessage": "eConnect call failed due: Sql procedure error codes returned: \n\rError Number = 7738  Stored Procedure= taGLTransactionLineInsert  Error Description = Input variable contains a duplicate Sequence Line Number (SQNCLINE)\r\nNode Identifier Parameters: taGLTransactionLineInsert\r\nBACHNUMB = JOGLAPI001\r\nJRNENTRY = 349399\r\nCRDTAMNT = 0\r\nDEBITAMT = 170\r\nRelated Error Code Parameters for Node : taGLTransactionLineInsert\r\nSQNCLINE = 16384\r\n\r\n\r\n<taGLTransactionLineInsert>\r\n  <BACHNUMB>JOGLAPI001</BACHNUMB>\r\n  <JRNENTRY>349399</JRNENTRY>\r\n  <SQNCLINE>16384</SQNCLINE>\r\n  <ACTINDX>0</ACTINDX>\r\n  <CRDTAMNT>0</CRDTAMNT>\r\n  <DEBITAMT>170</DEBITAMT>\r\n  <ACTNUMST>509-00-1200-000</ACTNUMST>\r\n  <DSCRIPTN>Line 1</DSCRIPTN>\r\n  <ORCTRNUM></ORCTRNUM>\r\n  <ORDOCNUM></ORDOCNUM>\r\n  <ORMSTRID></ORMSTRID>\r\n  <ORMSTRNM></ORMSTRNM>\r\n  <ORTRXTYP>0</ORTRXTYP>\r\n  <OrigSeqNum>0</OrigSeqNum>\r\n  <ORTRXDESC></ORTRXDESC>\r\n  <TAXDTLID></TAXDTLID>\r\n  <TAXAMNT>0</TAXAMNT>\r\n  <TAXACTNUMST></TAXACTNUMST>\r\n  <DOCDATE>4/21/2021</DOCDATE>\r\n  <CURNCYID>USD</CURNCYID>\r\n  <XCHGRATE />\r\n  <RATETPID></RATETPID>\r\n  <EXPNDATE>1/1/1900</EXPNDATE>\r\n  <EXCHDATE>1/1/1900</EXCHDATE>\r\n  <EXGTBDSC></EXGTBDSC>\r\n  <EXTBLSRC></EXTBLSRC>\r\n  <RATEEXPR>-1</RATEEXPR>\r\n  <DYSTINCR>-1</DYSTINCR>\r\n  <RATEVARC>0</RATEVARC>\r\n  <TRXDTDEF>-1</TRXDTDEF>\r\n  <RTCLCMTD>-1</RTCLCMTD>\r\n  <PRVDSLMT />\r\n  <DATELMTS />\r\n  <TIME1>1/1/1900</TIME1>\r\n  <RequesterTrx>0</RequesterTrx>\r\n  <USRDEFND1></USRDEFND1>\r\n  <USRDEFND2></USRDEFND2>\r\n  <USRDEFND3></USRDEFND3>\r\n  <USRDEFND4></USRDEFND4>\r\n  <USRDEFND5></USRDEFND5>\r\n</taGLTransactionLineInsert>\r\n",
    "Data": "<eConnect>\r\n  <GLTransactionType>\r\n    <taGLTransactionLineInsert_Items>\r\n      <taGLTransactionLineInsert>\r\n        <BACHNUMB>JOGLAPI001</BACHNUMB>\r\n        <JRNENTRY>349399</JRNENTRY>\r\n        <SQNCLINE>16384</SQNCLINE>\r\n        <ACTINDX>0</ACTINDX>\r\n        <CRDTAMNT>0</CRDTAMNT>\r\n        <DEBITAMT>170</DEBITAMT>\r\n        <ACTNUMST>509-00-1200-000</ACTNUMST>\r\n        <DSCRIPTN>Line 1</DSCRIPTN>\r\n        <ORCTRNUM></ORCTRNUM>\r\n        <ORDOCNUM></ORDOCNUM>\r\n        <ORMSTRID></ORMSTRID>\r\n        <ORMSTRNM></ORMSTRNM>\r\n        <ORTRXTYP>0</ORTRXTYP>\r\n        <OrigSeqNum>0</OrigSeqNum>\r\n        <ORTRXDESC></ORTRXDESC>\r\n        <TAXDTLID></TAXDTLID>\r\n        <TAXAMNT>0</TAXAMNT>\r\n        <TAXACTNUMST></TAXACTNUMST>\r\n        <DOCDATE>4/21/2021</DOCDATE>\r\n        <CURNCYID>USD</CURNCYID>\r\n        <XCHGRATE />\r\n        <RATETPID></RATETPID>\r\n        <EXPNDATE>1/1/1900</EXPNDATE>\r\n        <EXCHDATE>1/1/1900</EXCHDATE>\r\n        <EXGTBDSC></EXGTBDSC>\r\n        <EXTBLSRC></EXTBLSRC>\r\n        <RATEEXPR>-1</RATEEXPR>\r\n        <DYSTINCR>-1</DYSTINCR>\r\n        <RATEVARC>0</RATEVARC>\r\n        <TRXDTDEF>-1</TRXDTDEF>\r\n        <RTCLCMTD>-1</RTCLCMTD>\r\n        <PRVDSLMT />\r\n        <DATELMTS />\r\n        <TIME1>1/1/1900</TIME1>\r\n        <RequesterTrx>0</RequesterTrx>\r\n        <USRDEFND1></USRDEFND1>\r\n        <USRDEFND2></USRDEFND2>\r\n        <USRDEFND3></USRDEFND3>\r\n        <USRDEFND4></USRDEFND4>\r\n        <USRDEFND5></USRDEFND5>\r\n      </taGLTransactionLineInsert>\r\n      <taGLTransactionLineInsert>\r\n        <BACHNUMB>JOGLAPI001</BACHNUMB>\r\n        <JRNENTRY>349399</JRNENTRY>\r\n        <SQNCLINE>32768</SQNCLINE>\r\n        <ACTINDX>0</ACTINDX>\r\n        <CRDTAMNT>170</CRDTAMNT>\r\n        <DEBITAMT>0</DEBITAMT>\r\n        <ACTNUMST>509-00-1054-000</ACTNUMST>\r\n        <DSCRIPTN>Line 2</DSCRIPTN>\r\n        <ORCTRNUM></ORCTRNUM>\r\n        <ORDOCNUM></ORDOCNUM>\r\n        <ORMSTRID></ORMSTRID>\r\n        <ORMSTRNM></ORMSTRNM>\r\n        <ORTRXTYP>0</ORTRXTYP>\r\n        <OrigSeqNum>0</OrigSeqNum>\r\n        <ORTRXDESC></ORTRXDESC>\r\n        <TAXDTLID></TAXDTLID>\r\n        <TAXAMNT>0</TAXAMNT>\r\n        <TAXACTNUMST></TAXACTNUMST>\r\n        <DOCDATE>4/21/2021</DOCDATE>\r\n        <CURNCYID>USD</CURNCYID>\r\n        <XCHGRATE />\r\n        <RATETPID></RATETPID>\r\n        <EXPNDATE>1/1/1900</EXPNDATE>\r\n        <EXCHDATE>1/1/1900</EXCHDATE>\r\n        <EXGTBDSC></EXGTBDSC>\r\n        <EXTBLSRC></EXTBLSRC>\r\n        <RATEEXPR>-1</RATEEXPR>\r\n        <DYSTINCR>-1</DYSTINCR>\r\n        <RATEVARC>0</RATEVARC>\r\n        <TRXDTDEF>-1</TRXDTDEF>\r\n        <RTCLCMTD>-1</RTCLCMTD>\r\n        <PRVDSLMT />\r\n        <DATELMTS />\r\n        <TIME1>1/1/1900</TIME1>\r\n        <RequesterTrx>0</RequesterTrx>\r\n        <USRDEFND1></USRDEFND1>\r\n        <USRDEFND2></USRDEFND2>\r\n        <USRDEFND3></USRDEFND3>\r\n        <USRDEFND4></USRDEFND4>\r\n        <USRDEFND5></USRDEFND5>\r\n      </taGLTransactionLineInsert>\r\n    </taGLTransactionLineInsert_Items>\r\n    <taGLTransactionHeaderInsert>\r\n      <BACHNUMB>JOGLAPI001</BACHNUMB>\r\n      <JRNENTRY>349399</JRNENTRY>\r\n      <REFRENCE>GL API ENTRY</REFRENCE>\r\n      <TRXDATE>4/21/2021</TRXDATE>\r\n      <RVRSNGDT>1/1/1900</RVRSNGDT>\r\n      <TRXTYPE>0</TRXTYPE>\r\n      <SQNCLINE>0</SQNCLINE>\r\n      <SERIES>2</SERIES>\r\n      <CURNCYID></CURNCYID>\r\n      <XCHGRATE>0</XCHGRATE>\r\n      <RATETPID></RATETPID>\r\n      <EXPNDATE>1/1/1900</EXPNDATE>\r\n      <EXCHDATE>1/1/1900</EXCHDATE>\r\n      <EXGTBDSC></EXGTBDSC>\r\n      <EXTBLSRC></EXTBLSRC>\r\n      <RATEEXPR>-1</RATEEXPR>\r\n      <DYSTINCR>-1</DYSTINCR>\r\n      <RATEVARC>0</RATEVARC>\r\n      <TRXDTDEF>-1</TRXDTDEF>\r\n      <RTCLCMTD>-1</RTCLCMTD>\r\n      <PRVDSLMT>0</PRVDSLMT>\r\n      <DATELMTS>0</DATELMTS>\r\n      <TIME1>1/1/1900</TIME1>\r\n      <RequesterTrx>0</RequesterTrx>\r\n      <SOURCDOC></SOURCDOC>\r\n      <Ledger_ID>1</Ledger_ID>\r\n      <USERID></USERID>\r\n      <Adjustment_Transaction>0</Adjustment_Transaction>\r\n      <NOTETEXT></NOTETEXT>\r\n      <USRDEFND1>200</USRDEFND1>\r\n      <USRDEFND2></USRDEFND2>\r\n      <USRDEFND3></USRDEFND3>\r\n      <USRDEFND4></USRDEFND4>\r\n      <USRDEFND5></USRDEFND5>\r\n      <USRDEFND5></USRDEFND5>\r\n    </taGLTransactionHeaderInsert>\r\n  </GLTransactionType>\r\n</eConnect>"
}
```
* Creating a new Journal Entry Number (SQL)

Notice we are sending a value to USRDEFND1, in this case to indicate the Facility ID in a System which has Binary Stream Multi-Entity Management enabled

This endpoint will use eConnect SQL Procedures (taGLTransactionLineInsert, taGLTransactionHeaderInsert) for the integration

```
POST /eConnectApi/api/gl/sqltransaction HTTP/1.1
Host: [YOUR_SERVER_NAME_OR_IP_ADDRESS]
Content-Type: application/json
Content-Length: 686

{
    "BACHNUMB": "JOGLAPI001",
    "JRNENTRY": 349402,
    "REFRENCE": "GL API ENTRY",
    "TRXDATE": "2021-04-21",
    "RVRSNGDT": "1900-01-01",
    "TRXTYPE": 0,
    "SQNCLINE": 0,
    "USRDEFND1": "200",
    "Lines": [
        {
            "SQNCLINE": 16384,
            "CRDTAMNT": 0,
            "DEBITAMT": 170,
            "ACTNUMST": "509-00-1200-000",
            "DSCRIPTN": "Line 1",
            "CURNCYID": "USD"
        },
        {
            "SQNCLINE": 32768,
            "CRDTAMNT": 170,
            "DEBITAMT": 0,
            "ACTNUMST": "509-00-1054-000",
            "DSCRIPTN": "Line 2",
            "CURNCYID": "USD"
        }
    ]
}
```

Output

```
{
    "Succeed": true,
    "ErrorMessage": "",
    "Data": 349402
}
```
