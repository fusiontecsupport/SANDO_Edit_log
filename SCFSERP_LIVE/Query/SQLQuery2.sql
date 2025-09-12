USE [cfs]
GO

DECLARE	@return_value Int

EXEC	@return_value = [dbo].[SP_STUFFING_CONTAINER_NO_CBX_ASSGN]
		@PCHAID = 3,
		@PSTFMID = 0

SELECT	@return_value as 'Return Value'

GO
