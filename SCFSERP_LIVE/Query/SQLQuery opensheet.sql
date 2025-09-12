USE [cfs]
GO

DECLARE	@return_value Int

EXEC	@return_value = [dbo].[Procedure]
		@IGMNO ='2091374',
		@GPLNO ='297'

SELECT	@return_value as 'Return Value'

GO


INSERT INTO VW_OPENSHEET_CONTAINER_CBX_ASSGN_01 values(1,'2091374','297','TGU2323232','20','12554','-','df','rtr','hjh')

EXEC SP_OPENSHEET_CONTAINER_FLX_ASSGN @PIGMNO='2091374',@PGPLNO='297'