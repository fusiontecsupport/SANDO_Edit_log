/*
exec pr_Get_ExBond_VT_Info @compyid  = 31 ,  @VTDID  = 57
exec pr_Get_ExBond_VT_Info @compyid  = 31 ,  @VTDID  = 43
exec pr_Get_ExBond_VT_Info @compyid  = 31 ,  @VTDID  = 99
select * from exbondmaster where ebnddno ='3002490'
select * from bondvehicleticketdetail where
 SELECT * FROM BONDMASTER where bndid = 1003
 select * From [BONDTRANSACTIONMASTER]
 */
alter PROC pr_Get_ExBond_VT_Info
@compyid int,
@VTDID int
as
begin
	SET NOCOUNT ON
	declare @tbl table 
	(BNDID INT,
	 BNDDNO VARCHAR(50),
	 BNDDATE varchar(12),
	 EBNDID INT,
	 EBNDDNO VARCHAR(50),
	 EBNDDATE varchar(12),
	 VTDID INT,
	 VTDNO VARCHAR(50),
	 VTDATE varchar(12),
	 IMPRTID int,
	 CHAID int,
	 BNDBENO varchar(50),
	 BNDBLNO varchar(50),
	 BNDIGMNO varchar(50),
	 BNDLINENO varchar(50),
	 EXBONDNOP NUMERIC(18,2),
	 EXBONDSPC NUMERIC(18,2),
	 BNDGWGHT numeric(18,2),
	 PRDTGID int,
	 PRDTDESC varchar(100),
	 PRDTTID int,	 
	 BNDBEDATE varchar(12),
	 BNDBLDATE varchar(12),
	 BNDTYPE varchar(10),
	 VTNOP int,
	 VTSPC numeric(18,2),
	 EXBONDBALNOP NUMERIC(18,2),
	 EXBONDBALSPC NUMERIC(18,2),	 
	 CHANAME VARCHAR(100),
	 IMPRTRNAME VARCHAR(100),
	 BNDTYPEDESC VARCHAR(100),
	 EBNDASSAMT numeric(18,2),
	 EBNDDTYAMT numeric(18,2),
	 EBNDINSAMT numeric(18,2),
	 BNDSDate varchar(12),
	 BNDCDate varchar(12),
	 BNDISDate varchar(12),
	 BNDICDate varchar(12),
	 BBNDNOP numeric(18,2),
	 BBNDSPC numeric(18,2),
	 BEBNDASSAMT numeric(18,2),
	 BEBNDDTYAMT numeric(18,2),
	 BEBNDINSAMT numeric(18,2),
	 EBNDNOC numeric(18,2),
	 EBVTNOC numeric(18,2),
	 MAXINSDT DATETIME
	 )

	 declare @SelectFinalVT table
	 (
	 ebndid int,
	 ebndno varchar(100),
	 vtdid int,
	 ebndnop int,
	 vtspc numeric(18,2),
	 vtnop numeric(18,2)
	 )

	 declare @ebndid int , @totVTQty numeric(18,2), @totVTSpc numeric(18,2), 
	 @tmpVTQty numeric(18,2), @tmpVTSpc numeric(18,2), @bndid int

	 
	 select @ebndid = EBNDID, @tmpVTQty = VTQTY, @tmpVTSpc = VTAREA
	 from BONDVEHICLETICKETDETAIL (nolock)
	 where VTDID = @VTDID
	 
	select @bndid = BNDID
	from exbondmaster (nolock) 
	where ebndid = @ebndid

	 declare @balassval numeric(18,2), @baldtyamt numeric(18,2),  @balinsamt numeric(18,2),
	 @BNDFDATE DATETIME, @storagefromdt datetime,  @insfromdt datetime, @bndspc numeric(18,2)
	select @balassval = BNDCIFAMT, @baldtyamt= BNDDTYAMT, @balinsamt = BNDINSAMT, @bndspc = BNDSPC
	from BONDMASTER (nolock)
	where BNDID = @BNDID
	--select @balassval , @baldtyamt, @balinsamt
	select  @storagefromdt = max(TRANSEDATE)+1
	from BONDTRANSACTIONDETAIL a(nolock) left join EXBONDMASTER b (nolock) on a.EBNDID = b.EBNDID
	where b.bndid= @bndid
	and slabtid = 2

	select   @insfromdt = max(TRANIEDATE)+1
	from BONDTRANSACTIONDETAIL a(nolock) left join EXBONDMASTER b (nolock) on a.EBNDID = b.EBNDID
	where b.bndid= @bndid
	and slabtid = 4

	select @balassval = @balassval - isnull(sum(EBNDASSAMT),0), @baldtyamt= @baldtyamt - isnull(sum(EBNDDTYAMT),0), 
	@balinsamt = @balinsamt- isnull(sum(EBNDINSAMT),0), @bndspc = @bndspc - isnull(sum(EBNDSPC),0)
	from EXBONDMASTER b (nolock) 
	where b.bndid= @bndid
	and b.ebndid in (select ebndid from BONDTRANSACTIONDETAIL where bndid = @bndid)
	--select @balassval , @baldtyamt, @balinsamt

	--select max(TRANSEDATE)+1,   max(TRANIEDATE)+1
	--from BONDTRANSACTIONDETAIL a(nolock) left join EXBONDMASTER b (nolock) on a.EBNDID = b.EBNDID
	--where b.bndid= @bndid

	 insert into @SelectFinalVT
	 select  e.EBNDID, e.EBNDDNO,  max(b.VTDID), e.EBNDNOP, e.EBNDSPC, sum(b.vtqty)
	from BONDVEHICLETICKETDETAIL b(nolock)
			left JOIN BONDTRANSACTIONDETAIL d (NOLOCK) ON b.EBNDID = d.EBNDID and b.VTDID = d.VTDID
			left join BONDTRANSACTIONMASTER A(nolock) on d.TRANMID= a.TRANMID and a.TRANTID=3
			left JOIN EXBONDMASTER e (NOLOCK) ON b.EBNDID = e.EBNDID and d.TRANDEBNDNO = e.EBNDDNO
	where   b.EBNDID = @ebndid 	and 
	d.VTDID is null
	group by  e.EBNDID, e.EBNDDNO,  e.EBNDNOP, e.EBNDSPC
	having sum(b.vtqty) =  e.EBNDNOP

	SELECT @BNDFDATE = BNDFDATE fROM BONDMASTER WHERE BNDID = @bndid

	select @totVTQty = vtnop ,@totVTSpc = vtspc from @SelectFinalVT 

	 INSERT INTO @tbl
	 select A.BNDID ,  a.BNDDNO ,  convert(varchar,BNDDATE,103) ,EB.EBNDID ,  EB.EBNDDNO ,  convert(varchar,EB.EBNDDATE,103) , 
	 VT.VTDID ,  VT.VTDNO ,  convert(varchar,VT.VTDATE,103) , 
	 A.IMPRTID, A.CHAID, BNDBENO, BNDBLNO, BNDIGMNO,isnull(BNDLINENO,''), isnull(BND20,0),isnull(BND40,0),BNDGWGHT,
	 A.PRDTGID, A.PRDTDESC,PRDTTID,convert(varchar,BNDBEDATE,103) , convert(varchar,BNDBLDATE,103) ,BNDTYPE,
	 isnull(@tmpVTQty,@tmpVTQty), @bndspc,
	 --case when isnull(@totVTSpc,@tmpVTSpc) <= 5 then 5 else isnull(@totVTSpc,@tmpVTSpc) end,
	 eb.EBNDNOP,eb.EBNDSPC, B.CATENAME, C.CATENAME, 
	 CASE WHEN BNDTYPE = 1 THEN 'FCL' ELSE 'LCL' END,
	 --a.BNDCIFAMT/a.BNDNOP*isnull(@totVTQty,BEBNOP),a.BNDDTYAMT/a.BNDNOP*isnull(@totVTQty,BEBNOP), a.BNDINSAMT/a.BNDNOP*isnull(@totVTQty,BEBNOP), 
	 @balassval, @baldtyamt, @balinsamt,
	 convert(varchar,max(ISNULL(isnull(@storagefromdt,GIDATE),@BNDFDATE)),103) 'BNDSDATE', convert(varchar,max(VTDATE),103) 'BNDCDATE',
	 convert(varchar,max(ISNULL(isnull(@insfromdt,GIDATE),@BNDFDATE)),103) 'BNDISDATE', convert(varchar,max(VTDATE),103) 'BNDICDATE',
	 BEBNOP BBNDNOP, BEBSPC BBNDSPC , 0 BEBNDASSAMT , 0 BEBNDDTYAMT , 0 BEBNDINSAMT,EBNDNOC, EBVTNOC,max(VTDATE)
	 from BONDVEHICLETICKETDETAIL VT(NOLOCK) JOIN EXBONDMASTER EB(NOLOCK) ON VT.EBNDID = EB.EBNDID
		JOIN BONDMASTER a (nolock) ON EB.BNDID = A.BNDID
		left join [VW_VT_EXBOND_BALANCE_DETAIL_ASSGN] e (nolock) on VT.EBNDID = e.EBNDID 
		LEFT JOIN CATEGORYMASTER B(NOLOCK) ON A.CHAID = B.CATEID AND B.CATETID = 4
		LEFT JOIN CATEGORYMASTER C(NOLOCK) ON A.IMPRTID = C.CATEID AND C.CATETID = 1
		LEFT JOIN BONDGATEINDETAIL D(NOLOCK) ON A.BNDID = D.BNDID			
		
		--left join [BONDTRANSACTIONDETAIL] BID (nolock) on
		--(a.BNDID = BID.BNDID ) 		--AND eb.EBNDID = BID.EBNDID
		----or
		----(a.BNDDNO = BID.TRANDBNDNO AND eb.EBNDDNO = BID.TRANDEBNDNO)) 
		--left join [BONDTRANSACTIONMASTER] BIM (nolock) on   BID.tranmid = BIM.tranmid 	-- and BIM.TRANTID=2 EB.BNDID = BIM.TRANLMID AND A.BNDDNO = BIM.TRANLMNO AND	
		--left join [BONDTRANSACTIONDETAIL] EBID (nolock) on 
		--((EB.eBNDID = eBID.eBNDID ) 		-- AND eb.EBNDID = eBID.EBNDID
		--or
		--(a.BNDDNO = eBID.TRANDBNDNO )) 	-- AND eb.EBNDDNO = eBID.TRANDEBNDNO
		--left join [BONDTRANSACTIONMASTER] EBIM (nolock) on EBID.tranmid = EBIM.tranmid and eBIM.TRANTID=3	--vt.VTDID = EBIM.TRANLMID AND vt.VTDNO = EBIM.TRANLMNO AND	
		

	where -- a.COMPYID = @compyid 	and 
	vt.VTDID = @VTDID
	GROUP BY A.BNDID ,  a.BNDDNO ,  BNDDATE , A.IMPRTID, A.CHAID, BNDBENO, BNDBLNO, BNDIGMNO,BNDLINENO, BND20,BND40,BNDGWGHT,
	 A.PRDTGID, A.PRDTDESC,PRDTTID,BNDNOP,BNDSPC,BNDBEDATE, BNDBLDATE,BNDTYPE,B.CATENAME, C.CATENAME, 
	 CASE WHEN BNDTYPE = 1 THEN 'FCL' ELSE 'LCL' END,BNDCIFAMT,BNDDTYAMT,BNDINSAMT, BNDDATE, BNDTDATE,
	 BEBNOP, BEBSPC , EB.EBNDID ,  EB.EBNDDNO ,  EB.EBNDDATE, 	 VT.VTDID ,  VT.VTDNO ,VTDATE, 
	 EBNDNOP,EBNDSPC,EBNDNOC,EBVTNOC

	 update @tbl
	 set BNDSDate ='' 
	 where BNDSDate is null

	update @tbl
	 set BNDCDate ='' 
	 where BNDCDate is null

	 update @tbl
	 set BNDISDate ='' 
	 where BNDISDate is null

	 declare @dt1 datetime, @dt2 datetime, @dtwkd1 int, @dtwkd2 int, @dystoadd int
	select @dt1=@insfromdt-1
	select @dt2=max(MAXINSDT) from @tbl
	set @dystoadd =0
	--select @dt1, @dt2
	if @dt1 is not null and @dt2 is not null
	begin
		select @dtwkd1= datepart(WEEKDAY, @dt1)
		select @dtwkd2=datepart(WEEKDAY, @dt2)
	
		select @dystoadd = case when @dtwkd1 <@dtwkd2 then @dtwkd2 - @dtwkd1 else @dtwkd1-@dtwkd2 end
		select @dystoadd = @dystoadd-1 
	end
	
	-- select  BNDICDate , dateadd(d,@dystoadd,MAXINSDT) from @tbl
	-- where BNDICDate is not null
	if(@dystoadd>0)
	begin
	 update @tbl
	 set BNDICDate =  convert(varchar,(dateadd(d,@dystoadd,MAXINSDT) ),103) 
	 where BNDICDate is not null
	 end
	--update @tbl
	-- set BNDICDate ='' 
	-- where BNDICDate is null



	 SELECT * FROM @tbl
end
