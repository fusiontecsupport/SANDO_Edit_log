-- use SCFS_ERP_DEVPT
use SCFS_ERP
alter table gateoutdetail
ADD ESSBLNO VARCHAR(50) NULL DEFAULT ('')

alter table gateoutdetail
ADD ESSBLDT DATETIME NULL DEFAULT (NULL)

alter table gateoutdetail
ADD ESPORT INT NULL DEFAULT (0)

alter table gateoutdetail
ADD ESTOPLACE VARCHAR(100) NULL DEFAULT ('')

alter table gateoutdetail
ADD ESINVNO VARCHAR(50) NULL DEFAULT ('')

alter table gateoutdetail
ADD ESINVDT DATETIME NULL DEFAULT (NULL)
