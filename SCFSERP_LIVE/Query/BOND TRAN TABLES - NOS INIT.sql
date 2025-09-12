select * from SPBW..transactionmaster order by tranmid desc
select * from SPBW..transactiondetail order by trandid desc
select * from SPBW..TRANSACTIONMASTERFACTOR order by TRANMFID desc
select * from SPBW..bondmaster order by bndid desc
select * from SPBW..exbondmaster order by ebndid desc
/*
truncate table bondgateindetail
DBCC CHECKIDENT ('bondgateindetail', RESEED, 1);
truncate table bondgateoutdetail
DBCC CHECKIDENT ('bondgateoutdetail', RESEED, 14700);
truncate table bondvehicleticketdetail
DBCC CHECKIDENT ('bondvehicleticketdetail', RESEED, 14700);
truncate table bondmaster --5724
 DBCC CHECKIDENT ('bondmaster', RESEED, 5750);
truncate table exbondmaster --13922
DBCC CHECKIDENT ('exbondmaster', RESEED, 14000);
truncate table bondtransactionmasterfactor --14612
DBCC CHECKIDENT ('bondtransactionmasterfactor', RESEED, 24000);
truncate table bondtransactiondetail --14612
DBCC CHECKIDENT ('bondtransactiondetail', RESEED, 38600);
--delete bondtransactionmaster
truncate table bondtransactionmaster --14612
DBCC CHECKIDENT ('bondtransactionmaster', RESEED, 14700);
*/

select * From bondvehicleticketdetail