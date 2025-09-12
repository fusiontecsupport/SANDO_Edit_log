ALTER proc pr_Fetch_CHAIMP_Dtl
@Type int,
@TERM VARCHAR(100)=''
as
begin
	SET @TERM = '%' + ISNULL(@TERM,'')+ '%'
	select CATENAME, CATEID 
	from categorymaster (NOLOCK)
	where catetid = 4
	AND CATENAME LIKE @TERM
end