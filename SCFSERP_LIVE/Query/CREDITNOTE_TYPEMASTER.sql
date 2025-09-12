USE [SCFS_ERP]
GO

/****** Object:  Table [dbo].[PRODUCTTYPEMASTER]    Script Date: 17/09/2021 17:29:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CREDITNOTE_TYPEMASTER](
	[CNTID] [int] primary key identity(1,1) not null,
	[CNTDESC] [varchar](50) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


