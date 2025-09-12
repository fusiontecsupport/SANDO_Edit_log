USE [SCFS_ERP]
GO

ALTER TABLE [dbo].[GATEINDETAIL] DROP CONSTRAINT [DF_GATEINDETAIL_LMUSRID]
GO

alter table gateindetail 
alter column LMUSRID varchar(100)  null 

ALTER TABLE [dbo].[GATEINDETAIL] ADD  CONSTRAINT [DF_GATEINDETAIL_LMUSRID]  DEFAULT ((0)) FOR [LMUSRID]
GO


USE [SCFS_ERP]
GO

ALTER TABLE [dbo].[GATEINDETAIL] DROP CONSTRAINT [DF_GATEINDETAIL_LMUSRID]
GO

alter table gateindetail 
alter column LMUSRID varchar(100)  null 

ALTER TABLE [dbo].[GATEINDETAIL] ADD  CONSTRAINT [DF_GATEINDETAIL_LMUSRID]  DEFAULT ((0)) FOR [LMUSRID]
GO

alter table shippingbillmaster
alter column LMUSRID varchar(100) null 

alter table shippingbilldetail
alter column LMUSRID varchar(100) null 

alter table shippingbilldetail
alter column CUSRID varchar(100) null 


alter table shippingbilldetail 
DROP column SBDID 

alter table shippingbilldetail 
ADD SBDID INT IDENTITY (1,1) NOT NULL PRIMARY KEY



ALTER TABLE [dbo].EXPORTSHIPPINGBILLMASTER DROP CONSTRAINT DF_EXPORTSHIPPINGBILLMASTER_LMUSRID
alter table EXPORTSHIPPINGBILLMASTER
alter column LMUSRID varchar(100) null 

ALTER TABLE [dbo].EXPORTSHIPPINGBILLMASTER ADD  CONSTRAINT DF_EXPORTSHIPPINGBILLMASTER_LMUSRID  DEFAULT (('')) FOR [LMUSRID]
