alter table StuffingMASTER
add  STFBCATEAID	int	not null default 0 

alter table StuffingMASTER 
add  STFBCHAGSTNO	varchar(50)	null default ''

alter table StuffingMASTER 
add  STFBCHAADDR1	int	null default 0 

alter table StuffingMASTER 
add  STFBCHAADDR2	varchar(50)	null default ''

alter table StuffingMASTER 
add  STFBCHAADDR3	varchar(100)	null default ''

alter table StuffingMASTER 
add  STFBCHAADDR4	varchar(100)	null default ''

alter table StuffingMASTER 
add  STFBCHASTATEID	int	null default 0 