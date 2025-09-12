alter table SlabMaster
add SDPTTYPEID INT not null default 0 -- 1 for IMPORT / 2 for NON PNR

UPDATE SlabMaster
SET SDPTTYPEID = 1 WHERE SDPTTYPEID = 0