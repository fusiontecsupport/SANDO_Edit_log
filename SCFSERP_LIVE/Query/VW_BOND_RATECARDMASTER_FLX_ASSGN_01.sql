alter VIEW [dbo].[VW_BOND_RATECARDMASTER_FLX_ASSGN_01]
AS
SELECT     TOP (100) PERCENT dbo.BondSlabMaster.SLABTID, dbo.BondSlabMaster.TARIFFMID, dbo.BondSlabMaster.CHAID, dbo.BondSlabMaster.CHRGETYPE, 
                      dbo.BondSlabMaster.CONTNRSID, dbo.BondSlabMaster.SDTYPE, dbo.BondSlabMaster.YRDTYPE, dbo.BondSlabMaster.HTYPE, 
					  dbo.BondSlabMaster.WTYPE, dbo.BondSlabMaster.PERIODTID, dbo.BondSlabMaster.PRDTGID, dbo.BondSlabMaster.HANDTYPE,
                      MAX(dbo.BondSlabMaster.SLABMDATE) AS XTRANSDATE
FROM         dbo.BondTariffMaster (nolock) INNER JOIN
                      dbo.BondSlabMaster (nolock) ON dbo.BondTariffMaster.TARIFFMID = dbo.BondSlabMaster.TARIFFMID
WHERE     (dbo.BondSlabMaster.SLABMDATE <=
                          (SELECT     MAX(TRANNDATE) AS Expr1
                            FROM          dbo.TRANSACTION_DAYEND_MASTER (nolock)))
GROUP BY dbo.BondSlabMaster.TARIFFMID, dbo.BondSlabMaster.SLABTID, dbo.BondSlabMaster.CHAID, dbo.BondSlabMaster.CHRGETYPE, dbo.BondSlabMaster.CONTNRSID, 
                      dbo.BondSlabMaster.SDTYPE, dbo.BondSlabMaster.YRDTYPE, dbo.BondSlabMaster.HTYPE, dbo.BondSlabMaster.WTYPE, 
					  dbo.BondSlabMaster.PERIODTID, dbo.BondSlabMaster.PRDTGID, dbo.BondSlabMaster.HANDTYPE
ORDER BY dbo.BondSlabMaster.TARIFFMID
