

select *from DeliveryOrderMaster



UPDATE t1
SET t1.DOREFDATE = t2.DODATE
FROM DeliveryOrderMaster t1
INNER JOIN DeliveryOrderMaster t2 ON t1.DOMID = t2.DOMID 
WHERE t1.DOREFDATE IS NULL