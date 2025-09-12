/*
EXEC pr_Get_ExBond_Info 31, 1
go
 SELECT * FROM BONDMASTER where bndid = 1003
 */
ALTER PROC pr_Get_ExBond_Info
@compyid int,
@exbondid int
as
begin
	declare @tbl table 
	(EBNDID INT,
	 EBNDDNO VARCHAR(50),
	 EBNDDATE varchar(12),
	 BNDID INT,
	 BNDDNO VARCHAR(50),
	 BNDDATE varchar(12),
	 IMPRTID int,
	 CHAID int,
	 EBNDBENO varchar(50),
	 PRDTGID int,
	 PRDTDESC varchar(100),	 
	 EBPRDTTID int,
	 EBPRDTDESC varchar(100),
	 BPRDTTID int,
	 BNDNOP int,
	 BNDSPC numeric(18,2),
	 EBNDBEDATE varchar(12),
	 BNDBEDATE varchar(12),
	 BNDBLDATE varchar(12),
	 EBNDTYPE varchar(10),
	 CHANAME VARCHAR(100),
	 IMPRTRNAME VARCHAR(100),
	 BNDTYPEDESC VARCHAR(100),
	 BNDCIFAMT numeric(18,2),
	 BNDDTYAMT numeric(18,2),
	 BNDINSAMT numeric(18,2),
	 CUMVTNOP numeric(18,2),
	 CUMVTSPC numeric(18,2),
	 BEBNOP numeric(18,2),
	 BEBSPC numeric(18,2)
	 )

	 INSERT INTO @tbl
	 select E.EBNDID ,  E.EBNDDNO ,  convert(varchar,E.EBNDDATE,103) , 
	 A.BNDID ,  a.BNDDNO ,  convert(varchar,BNDDATE,103) , E.IMPRTID, E.CHAID, E.EBNDBENO, 
	 A.PRDTGID, A.PRDTDESC,E.PRDTGID, BP.PRDTGDESC,	 PRDTTID,EBNDNOP,EBNDSPC,  
	 convert(varchar,E.EBNDBEDATE,103) , convert(varchar,BNDBEDATE,103) ,convert(varchar,BNDBLDATE,103) ,
	 E.EBNDCTYPE, B.CATENAME, C.CATENAME, CASE WHEN E.EBNDCTYPE = 1 THEN 'FCL' ELSE 'LCL' END,
	 BNDCIFAMT,BNDDTYAMT, BNDINSAMT, isnull(CUMVTNOP,0), isnull(CUMVTSPC ,0), isnull(BEBNOP,0), isnull(BEBSPC ,0)
	 from EXBONDMASTER E(NOLOCK)
		JOIN BONDMASTER a (nolock) ON E.BNDID = A.BNDID AND E.COMPYID = A.COMPYID
		LEFT JOIN BONDPRODUCTGROUPMASTER BP(NOLOCK) ON E.PRDTGID = BP.PRDTGID
		LEFT JOIN CATEGORYMASTER B(NOLOCK) ON E.CHAID = B.CATEID AND B.CATETID = 4
		LEFT JOIN CATEGORYMASTER C(NOLOCK) ON E.IMPRTID = C.CATEID AND C.CATETID = 1
		left join [VW_VT_EXBOND_BALANCE_DETAIL_ASSGN] F (nolock) on E.EBNDID = F.EBNDID
	where E.COMPYID = @compyid
	and E.EBNDID = @exbondid

	-- update @tbl
	-- set BNDSDate ='' 
	-- where BNDSDate is null

	--update @tbl
	-- set BNDCDate ='' 
	-- where BNDCDate is null

	 SELECT * FROM @tbl
end


