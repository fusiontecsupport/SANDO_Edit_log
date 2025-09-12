
update stuffingmaster set  STFBCHASTATEID = 0
where STFBCHASTATEID is null

ALTER TABLE [dbo].[stuffingmaster] DROP CONSTRAINT DF__STUFFINGM__STFBC__5165E3D6
GO


alter table stuffingmaster 
alter column STFBCHASTATEID int not  null 


ALTER TABLE [dbo].[stuffingmaster] ADD  CONSTRAINT DF__STUFFINGM__STFBC__5165E3D6  DEFAULT ((0)) FOR STFBCHASTATEID
GO
