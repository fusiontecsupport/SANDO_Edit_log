


ALTER TABLE [dbo].TransactionMaster DROP CONSTRAINT DF_TRANSACTIONMASTER_LMUSRID
GO

alter table TransactionMaster 
alter column LMUSRID varchar(100)  null 

ALTER TABLE [dbo].TransactionMaster ADD  CONSTRAINT DF_TRANSACTIONMASTER_LMUSRID  DEFAULT (('')) FOR [LMUSRID]
GO
