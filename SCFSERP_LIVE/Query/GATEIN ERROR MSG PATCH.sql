SELECT * FROM SOFT_TABLE_DELETE_DETAIL WHERE OPTNSTR = 'GATEINDETAIL'

UPDATE SOFT_TABLE_DELETE_DETAIL
SET DISPDESC =  'Selected Record Exists In Authorization Slip Detail..........................!'
WHERE OPTNSTR = 'GATEINDETAIL'
AND TABNAME = 'AUTHORIZATIONSLIPDETAIL'
AND DISPDESC =   'Selected Record Exists In Non PNR Authorization Slip Detail..........................!'