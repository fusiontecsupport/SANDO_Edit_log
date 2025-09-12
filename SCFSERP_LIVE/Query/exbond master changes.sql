SELECT * FROM EXBONDMASTER

alter table EXBONDMASTER drop constraint DF_EXBONDMASTER_EBNDDCONTRS
alter table EXBONDMASTER
drop column EBNDNOC 

alter table EXBONDMASTER
alter column EBNDNOC numeric(15,2)