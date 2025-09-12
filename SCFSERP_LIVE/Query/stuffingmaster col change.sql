
alter table STUFFINGMASTER drop constraint DF__STUFFINGM__STFBC__4D9552F2

alter table STUFFINGMASTER
alter column STFBCHAADDR1 varchar(100) null 

alter table STUFFINGMASTER drop constraint DF__STUFFINGM__STFBC__4E89772B
alter table STUFFINGMASTER
alter column STFBCHAADDR2 varchar(100) null


select * from STUFFINGMASTER where STFBCHAADDR1 is null
select * from STUFFINGMASTER where STFBCHAADDR2 is null


update STUFFINGMASTER
set STFBCHAGSTNO = ''
where STFBCHAGSTNO is null

update STUFFINGMASTER
set STFBCHAADDR1 = ''
where STFBCHAADDR1 is null


update STUFFINGMASTER
set STFBCHAADDR2 = ''
where STFBCHAADDR2 is null


update STUFFINGMASTER
set STFBCHAADDR3 = ''
where STFBCHAADDR3 is null


update STUFFINGMASTER
set STFBCHAADDR4 = ''
where STFBCHAADDR4 is null