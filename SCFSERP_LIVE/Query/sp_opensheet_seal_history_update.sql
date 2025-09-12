ALTER proc sp_opensheet_seal_history_update
@compyid int,
@OSMID int,
@OSDID int,
@CSEALNO varchar(50)
as
begin
	
	IF ISNUMERIC(@CSEALNO) = 1
	BEGIN

		if NOT exists(	select '*' from OPENSHEET_SEAL_DETAIL (NOLOCK)
						where COMPYID = @compyid 
						and OSMID = @OSMID 
						AND OSDID = @OSDID 
						AND OSSDESC = @CSEALNO)
		AND NOT exists(	select '*' from OPENSHEET_SEAL_DETAIL (NOLOCK)
						where COMPYID = @compyid 
						AND OSDID != @OSDID 
						AND OSSDESC = @CSEALNO)
		BEGIN
			INSERT INTO OPENSHEET_SEAL_DETAIL (OSSDATE, OSMID, SEALMID, OSSDESC, CUSRID, LMUSRID, DISPSTATUS, PRCSDATE, COMPYID)
			SELECT GIDATE, OSMID,GIDID,@CSEALNO,CUSRID,LMUSRID,DISPSTATUS,GETDATE(),@compyid
			FROM OPENSHEETDETAIL (NOLOCK)
			WHERE OSDID = @OSDID
		END
	END

end