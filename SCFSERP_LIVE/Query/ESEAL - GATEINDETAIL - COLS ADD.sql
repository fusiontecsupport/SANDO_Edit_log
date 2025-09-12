alter table [GATEINDETAIL]
ADD [EXPRTRID] [int] NOT NULL CONSTRAINT [DF_GATEINDETAIL_EXPRTRID]  DEFAULT ((0))

alter table [GATEINDETAIL]
ADD [EXPRTRNAME] [varchar](100) NOT NULL CONSTRAINT [DF_GATEINDETAIL_EXPRTRNAME]  DEFAULT ('')

alter table [GATEINDETAIL]
ADD [DRVMBLNO] [varchar](15) NULL CONSTRAINT [DF_GATEINDETAIL_DRVMBLNO]  DEFAULT ('')


alter table [GATEINDETAIL]
ADD [DRVLCNO] [varchar](50) NULL CONSTRAINT [DF_GATEINDETAIL_DRVLCNO]  DEFAULT ('')


alter table [GATEINDETAIL]
drop constraint  [DF_GATEINDETAIL_DRVMBLNO]

alter table [GATEINDETAIL]
alter column [DRVMBLNO] [varchar](15) NULL 

alter table [GATEINDETAIL]
add constraint  [DF_GATEINDETAIL_DRVMBLNO]  DEFAULT ('') for [DRVMBLNO]



alter table [GATEINDETAIL]
drop constraint  [DF_GATEINDETAIL_DRVLCNO]

alter table [GATEINDETAIL]
alter column [DRVLCNO] [varchar](50) NULL 

alter table [GATEINDETAIL]
add constraint  [DF_GATEINDETAIL_DRVLCNO]  DEFAULT ('') for [DRVLCNO]



alter table [GATEINDETAIL]
drop constraint  [DF_GATEINDETAIL_EXPRTRID]

alter table [GATEINDETAIL]
alter column [EXPRTRID] int NULL 

alter table [GATEINDETAIL]
add constraint  [DF_GATEINDETAIL_EXPRTRID]  DEFAULT (0) for [EXPRTRID]


alter table [GATEINDETAIL]
drop constraint  [DF_GATEINDETAIL_EXPRTRNAME]

alter table [GATEINDETAIL]
alter column [EXPRTRNAME] [varchar](100) NULL 

alter table [GATEINDETAIL]
add constraint  [DF_GATEINDETAIL_EXPRTRNAME]  DEFAULT ('') for [EXPRTRNAME]


update GATEINDETAIL
set drvmblno = ''
where DRVMBLNO is null

update GATEINDETAIL
set drvlcno = ''
where drvlcno is null