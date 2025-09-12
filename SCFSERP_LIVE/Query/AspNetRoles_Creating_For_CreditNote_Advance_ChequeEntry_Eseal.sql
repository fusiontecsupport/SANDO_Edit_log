select *from AspNetRoles where Name Like '%CreditNote%'

--  Name Like '%CreditNoteDelete%'
--  Name Like '%CreditNoteCreate%'
--  Name Like '%CreditNotePrint%'
--  Name Like '%CreditNoteEdit%'
--  Name Like '%CreditNoteIndex%'

update AspNetRoles set RMenuType='Credit Note',RControllerName='CreditNote',
RMenuGroupId=24,RMenuGroupOrder=1,RMenuIndex='Index' where Name Like '%CreditNote%'  


select *from AspNetRoles where Name Like '%FinanceAdvance%'

--  Name Like '%FinanceAdvanceIndex%'
--  Name Like '%FinanceAdvanceCreate%'
--  Name Like '%FinanceAdvanceDelete%'
--  Name Like '%FinanceAdvanceEdit%'
--  Name Like '%FinanceAdvancePrint%'


update AspNetRoles set RMenuType='Advance',RControllerName='FinanceAdvance',
RMenuGroupId=24,RMenuGroupOrder=2,RMenuIndex='Index' where Name Like '%FinanceAdvance%'  

select *from AspNetRoles where Name Like '%esealinv%'

--  Name Like '%ESealInvoiceIndex%'
--  Name Like '%ESealInvoiceCreate%'
--  Name Like '%ESealInvoiceEdit%'
--  Name Like '%ESealInvoicePrint%'
--  Name Like '%ESealInvoiceDelete%'


update AspNetRoles set RMenuType='Invoice',RControllerName='ESealInvoice',
RMenuGroupId=23,RMenuGroupOrder=4,RMenuIndex='Index' where Name Like '%esealinv%'

select *from AspNetRoles where name like '%ChequeEntry%'

--  Name Like '%ChequeEntryIndex%'
--  Name Like '%ChequeEntryCreate%'
--  Name Like '%ChequeEntryEdit%'
--  Name Like '%ChequeEntryPrint%'
--  Name Like '%ChequeEntryDelete%'


update AspNetRoles set RMenuType='Cheque Entry',RControllerName='ChequeEntry',
RMenuGroupId=24,RMenuGroupOrder=3,RMenuIndex='Index' where Name Like '%ChequeEntry%'  