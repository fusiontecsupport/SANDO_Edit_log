using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using scfs_erp;
using scfs.Models;

namespace scfs_erp.Context
{
    public class BondContext : DbContext
    {
        // Bond Objects 

        public DbSet<BondGodownMaster> bondgodownmasters { get; set; }
        public DbSet<BondGodownTypeMaster> bondgodowntypemasters { get; set; }

        public DbSet<BondMaster> bondinfodtls { get; set; }
        public DbSet<BondGateInDetail> bondgateindtls { get; set; }

        public DbSet<BondSlabClassificationMaster> bondslabclassificationmasters { get; set; }
        public DbSet<BondSlabTypeMaster> bondslabtypemasters { get; set; }
        public DbSet<BondSlabMaster> bondslabmasters { get; set; }
        public DbSet<BondTariffMaster> bondtariffmasters { get; set; }
        public DbSet<BondYardMaster> bondyardmasters { get; set; }

        public DbSet<Bond_NEW_TMPRPT_IDS> BOND_NEW_TMPRPT_IDS { get; set; }
        public DbSet<Bond_TransactionMaster> Bond_TransactionMasters { get; set; }
        public DbSet<Bond_TransactionDetail> Bond_TransactionDetails { get; set; }
        public DbSet<ExBondMaster> ExBondMasters { get; set; }
        public DbSet<Z_TRANSACTION_DOSPRINT_ASSGN> Z_TRANSACTION_DOSPRINT_ASSGNs { get; set; }

        public DbSet<CategoryMaster> categorymasters { get; set; }
        public DbSet<VesselMaster> vesselmasters { get; set; }

        public DbSet<BondProductGroupMaster> bondproductgroupmasters { get; set; }
        public DbSet<ProductTypeMaster> producttypemasters { get; set; }
        public DbSet<ContainerTypeMaster> containertypemasters { get; set; }
        public DbSet<ContainerSizeMaster> containersizemasters { get; set; }
        public DbSet<SlotMaster> slotmasters { get; set; }
        public DbSet<VehicleMaster> vehiclemasters { get; set; }
        public DbSet<ExportTariffMaster> bondtariffmaster { get; set; }
        public DbSet<BondTransactionMaster> bondtransactionmaster { get; set; }

        public DbSet<BondTransactionDetail> bondtransactiondetail { get; set; }
        public DbSet<BondTransactionMasterFactor> bondtransactionmasterfactor { get; set; }
        public DbSet<TransactionModeMaster> bondtransactionmodemaster { get; set; }
        
        public DbSet<BankMaster> bankmasters { get; set; }
        public DbSet<ExportTariffGroupMaster> BondTariffGroupMasters { get; set; }
        public DbSet<Export_Invoice_Register> Bond_Invoice_Register { get; set; }

        public DbSet<ChargeMaster> chargemasters { get; set; }
        public DbSet<Import_Slab_Wages_Type_Master> import_slab_wages_type_masters { get; set; }
        public DbSet<Import_Slab_Handling_Type_Master> import_slab_handling_type_masters { get; set; }
        public DbSet<SoftDepartmentMaster> softdepartmentmasters { get; set; }
        public DbSet<TariffGroupMaster> tariffgroupmasters { get; set; }
        public DbSet<Category_Address_Details> categoryaddressdetails { get; set; }
        public DbSet<HSNCodeMaster> HSNCodeMasters { get; set; }
        public DbSet<Ex_BondMaster> exbondinfodtls { get; set; }
        public DbSet<ExBondVehicleTicket> exbondvtdtls { get; set; }
        public DbSet<BondContainerOut> bondcontnroutdtls { get; set; }
        public DbSet<BondPerformaTransactionMaster> bondperformatransactionmaster { get; set; }

        public DbSet<BondPerformaTransactionDetail> bondperformatransactiondetail { get; set; }
        public DbSet<BondPerformaTransactionMasterFactor> bondperformatransactionmasterfactor { get; set; }


    }
}