alter table stuffingmaster
add [TRANIMPADDR1] [varchar](100) NULL DEFAULT (NULL)
alter table stuffingmaster
add 	[TRANIMPADDR2] [varchar](100) NULL DEFAULT (NULL)
alter table stuffingmaster
add 	[TRANIMPADDR3] [varchar](100) NULL DEFAULT (NULL)
alter table stuffingmaster
add 	[TRANIMPADDR4] [varchar](100) NULL DEFAULT (NULL)
alter table stuffingmaster
add 	[STATEID] [int] NULL DEFAULT ((0))
alter table stuffingmaster
add 	[CATEAGSTNO] [varchar](50) NULL DEFAULT ('')
alter table stuffingmaster
add 	[STF_SBILL_RNO] [varchar](50) NULL DEFAULT (NULL)
alter table stuffingmaster
add 	[STF_FORM13_RNO] [varchar](50) NULL DEFAULT (NULL)

update stuffingmaster
set STATEID =0 where STATEID is null

update stuffingmaster
set [CATEAGSTNO] ='' where [CATEAGSTNO] is null

-- select * from stuffingmaster