select GIDID, COUNT(ASLMID)
 from  [SCFS].dbo.z_din_18092021 (nolock)
 GROUP BY GIDID 
 HAVING COUNT(ASLMID) > 1

 SELECT * from  [SCFS].dbo.z_din_18092021 (nolock)
 WHERE GIDID IN ( 1508588, 1638491)

 SELECT * FROM SCFS.DBO.AUTHORIZATIONSLIPMASTER
 WHERE ASLMID IN ( 166024, 514846, 182246, 538402)
 
 SELECT * FROM SCFS.DBO.AUTHORIZATIONSLIPDETAIL
 WHERE ASLDID IN ( 196405, 615073, 215898, 642559)

 select * From opensheetmaster where osmid = 383
 
 select * From opensheetdetail where osmid = 383

 select * from BILLENTRYdetail where BILLEDID = 1314
 
 select * From billentrymaster where billemid = 383