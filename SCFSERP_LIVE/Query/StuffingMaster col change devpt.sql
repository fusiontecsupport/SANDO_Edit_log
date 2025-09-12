
update stuffingmaster set  STFBCHASTATEID = 0
where STFBCHASTATEID is null

ALTER TABLE [dbo].[stuffingmaster] DROP CONSTRAINT DF__STUFFINGM__STFBC__4A19C7C9
GO


alter table stuffingmaster 
alter column STFBCHASTATEID int not  null 


ALTER TABLE [dbo].[stuffingmaster] ADD  CONSTRAINT DF__STUFFINGM__STFBC__4A19C7C9  DEFAULT ((0)) FOR STFBCHASTATEID
GO
