USE [SCFS_ERP]
GO

/****** Object:  Table [dbo].[SHIPPINGBILLMASTER]    Script Date: 28-07-2021 14:02:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SHIPPINGBILLMASTER](
	[SBMID] [int] IDENTITY(1,1) NOT NULL,
	[COMPYID] [int] NOT NULL,
	[SBMDATE] [smalldatetime] NOT NULL,
	[SBMTIME] [datetime] NOT NULL,
	[SBMNO] [int] NOT NULL,
	[SBMDNO] [varchar](50) NOT NULL,
	[EXPRTID] [int] NOT NULL,
	[EXPRTNAME] [varchar](100) NOT NULL,
	[CHAID] [int] NOT NULL,
	[CHANAME] [varchar](100) NOT NULL,
	[SBMRMKS] [varchar](250) NULL,
	[CUSRID] [varchar](100) NULL,
	[LMUSRID] [int] NULL,
	[DISPSTATUS] [smallint] NULL,
	[PRCSDATE] [datetime] NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


