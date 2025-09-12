USE [SCFS_ERP]
GO

/****** Object:  Table [dbo].[EXPORTSHIPPINGBILLMASTER]    Script Date: 26-07-2021 18:04:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[EXPORTSHIPPINGBILLMASTER](
	[ESBMID] [int] IDENTITY(1,1) NOT NULL,
	[COMPYID] [int] NOT NULL,
	[ESBMDNO] [varchar](50) NULL,
	[ESBMDATE] [smalldatetime] NULL,
	[EXPRTID] [int] NULL,
	[EXPRTNAME] [varchar](100) NULL,
	[CHAID] [int] NULL,
	[CHANAME] [varchar](100) NULL,
	[ESBMREFNO] [varchar](25) NULL,
	[ESBMREFDATE] [smalldatetime] NULL,
	[ESBMREFAMT] [numeric](18, 2) NULL,
	[ESBMFOBAMT] [numeric](18, 2) NULL,
	[ESBMDPNAME] [varchar](50) NULL,
	[PRDTGID] [int] NULL,
	[PRDTDESC] [varchar](100) NULL,
	[ESBMNOP] [numeric](18, 2) NULL,
	[ESBMQTY] [numeric](18, 4) NULL,
	[ESBMRMKS] [varchar](250) NULL,
	[ESBMITYPE] [smallint] NULL,
	[ESBMIDATE] [smalldatetime] NULL,
	[CUSRID] [varchar](100) NULL,
	[LMUSRID] [int] NULL,
	[PRCSDATE] [datetime] NULL,
	[DISPSTATUS] [smallint] NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


