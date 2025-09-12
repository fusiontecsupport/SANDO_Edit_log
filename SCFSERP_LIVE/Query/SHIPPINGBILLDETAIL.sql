USE [SCFS_ERP]
GO

/****** Object:  Table [dbo].[SHIPPINGBILLDETAIL]    Script Date: 27-07-2021 18:08:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SHIPPINGBILLDETAIL](
	[SBDID] [int] NOT NULL,
	[SBMID] [int] NOT NULL,
	[GIDID] [int] NOT NULL,
	[SBDDATE] [smalldatetime] NOT NULL,
	[SBDNO] [int] NOT NULL,
	[SBDDNO] [varchar](50) NOT NULL,
	[TRUCKNO] [varchar](20) NOT NULL,
	[PRDTTID] [int] NOT NULL,
	[PRDTGID] [int] NOT NULL,
	[PRDTDESC] [varchar](100) NOT NULL,
	[SBDNOP] [numeric](18, 2) NULL,
	[SBDQTY] [numeric](18, 4) NULL,
	[GDWNID] [int] NOT NULL,
	[STAGID] [int] NOT NULL,
	[CUSRID] [int] NULL,
	[LMUSRID] [int] NULL,
	[DISPSTATUS] [smallint] NULL,
	[PRCSDATE] [datetime] NULL,
	[SBTYPE] [smallint] NULL,
	[ESBMID] [int] NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


