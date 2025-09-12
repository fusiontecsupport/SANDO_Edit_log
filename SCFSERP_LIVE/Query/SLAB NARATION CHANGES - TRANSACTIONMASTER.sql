alter table transactionmaster
add SLABNARN_HANDLDESC VARCHAR(500) DEFAULT ''

alter table transactionmaster
add SLABNARN_ADNLDESC VARCHAR(500) DEFAULT ''

alter table transactionmaster
add SLABNARN_STS INT DEFAULT 0

select *from transactionmaster