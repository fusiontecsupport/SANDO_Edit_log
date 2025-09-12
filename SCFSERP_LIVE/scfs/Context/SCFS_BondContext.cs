using scfs_erp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using scfs_erp;

namespace scfs.Context
{
    public class SCFS_BondContext : DbContext
    {
        public DbSet<Bond_NEW_TMPRPT_IDS> BOND_NEW_TMPRPT_IDS { get; set; }
        public DbSet<Bond_TransactionMaster> Bond_TransactionMasters { get; set; }
        public DbSet<Bond_TransactionDetail> Bond_TransactionDetails { get; set; }
        public DbSet<ExBondMaster> ExBondMasters { get; set; }
        public DbSet<Z_TRANSACTION_DOSPRINT_ASSGN> Z_TRANSACTION_DOSPRINT_ASSGNs { get; set; }

    }
}