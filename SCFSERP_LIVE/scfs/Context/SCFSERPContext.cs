//using scfs.Data;
using scfs.Data;
using scfs.Models;
using scfs_erp.Models;
using System;
using System.Data.Entity;
using scfs.Models;

namespace scfs_erp.Context
{
    public class SCFSERPContext:DbContext
    {
        public DbSet<TMPRPT_IDS> TMPRPT_IDS { get; set; }
        public DbSet<TMP_CARTING_ORDER_TRUCK_DUPCHK> TMP_CARTING_ORDER_TRUCK_DUPCHK { get; set; }
        
        public DbSet<NEW_TMPRPT_IDS> NEW_TMPRPT_IDS { get; set; }

        public DbSet<VW_ACCOUNTING_YEAR_DETAIL_ASSGN> VW_ACCOUNTING_YEAR_DETAIL_ASSGN { get; set; }
        public DbSet<VW_EXPORT_GATEOUT_TRUCKNO_CBX_ASSGN> VW_EXPORT_GATEOUT_TRUCKNO_CBX_ASSGN { get; set; }
        public DbSet<VW_IMPORT_GATEOUT_TRUCKNO_CBX_ASSGN> VW_IMPORT_GATEOUT_TRUCKNO_CBX_ASSGN { get; set; }
        public DbSet<VW_EXPORT_GATEOUT_CONTAINER_CBX_ASSGN> VW_EXPORT_GATEOUT_CONTAINER_CBX_ASSGN { get; set; }
        
        public DbSet<VW_EXPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN> VW_EXPORT_GATEOUT_CONTAINER_CBX_CHNG_ASSGN { get; set; }
        public DbSet<VW_EXPORT_SHIPPING_BILL_WISE_STUFFNG_DETAIL_ASSGN> VW_EXPORT_SHIPPING_BILL_WISE_STUFFNG_DETAIL_ASSGN { get; set; }
        public DbSet<VW_EXPORT_SHIPPINGBILL_WISE_CARTINGORDER_DETAIL_ASSGN> VW_EXPORT_SHIPPINGBILL_WISE_CARTINGORDER_DETAIL_ASSGN { get; set; }

        public DbSet<VW_EXPORT_CONT_STUFFING_DETAIL_ASSGN> VW_EXPORT_CONT_STUFFING_DETAIL_ASSGN { get; set; }
        public DbSet<VW_EXPORT_CONTAINER_DETAIL_QUERY_ASSGN> VW_EXPORT_CONTAINER_DETAIL_QUERY_ASSGN { get; set; }
        public DbSet<VW_EXPORT_SHIPPING_BILL_INVOICE_DETAIL> VW_EXPORT_SHIPPING_BILL_INVOICE_DETAIL { get; set; }

        public DbSet<VW_CARGO_OUT_CONTAINER_CBX_ASSGN> VW_CARGO_OUT_CONTAINER_CBX_ASSGNS { get; set; }

        //public DbSet<VW_NEW_CATEGORY_STATETYPE_ASSGN> VW_NEW_CATEGORY_STATETYPE_ASSGNs { get; set; }
        public DbSet<AccountGroupMaster> accountgroupmasters { get; set; }

        public DbSet<AccountHeadMaster> accountheadmasters { get; set; }

        public DbSet<CategoryTypeMaster> categorytypemasters { get; set; }

        public DbSet<CategoryMaster> categorymasters { get; set; }

        public DbSet<Category_Address_Details> categoryaddressdetails { get; set; }

        public DbSet<CompanyMaster> companymasters { get; set; }

        public DbSet<TransactionMaster> transactionmaster { get; set; }

        public DbSet<TransactionDetail> transactiondetail { get; set; }

        public DbSet<TransactionModeMaster> transactionmodemaster { get; set; }

        public DbSet<Performa_TransactionMaster> performatransactionmaster { get; set; }

        public DbSet<Performa_TransactionDetail> performatransactiondetail { get; set; }

        public DbSet<Performa_TransactionMasterFactor> performatransactionmasterfactor { get; set; }
        
        public DbSet<TransactionMasterFactor> transactionmasterfactor { get; set; }

        public DbSet<ETransactionMaster> etransactionmasters { get; set; }

        public DbSet<CreditNote_TransactionMaster> creditnotetransactionmaster { get; set; }

        public DbSet<CreditNote_TransactionDetail> creditnotetransactiondetail { get; set; }

        public DbSet<CreditNote_TypeMaster> CreditNoteTypeMaster { get; set; }

        public DbSet<DebitNote_TransactionMaster> debitnotetransactionmaster { get; set; }

        public DbSet<DebitNote_TransactionDetail> debitnotetransactiondetail { get; set; }

        public DbSet<DebitNote_TypeMaster> debitnotetypemaster { get; set; }


        public DbSet<BookedMaster> bookedmasters { get; set; }

        public DbSet<DesignationMaster> designationmasters { get; set; }

        public DbSet<DesignationTypeMaster> designationtypemasters { get; set; }

        public DbSet<DepartmentMaster> departmentmasters { get; set; }

        public DbSet<ConditionMaster> conditionmasters { get; set; }

        public DbSet<ContainerSizeMaster> containersizemasters { get; set; }

        public DbSet<SoftDepartmentMaster> softdepartmentmasters { get; set; }

        public DbSet<ChargeMaster> chargemasters { get; set; }

        public DbSet<HSNCodeMaster> HSNCodeMasters { get; set; }

        public DbSet<UnitMaster> unitmasters { get; set; }

        public DbSet<StateMaster> statemasters { get; set; }

        public DbSet<RowMaster> rowmasters { get; set; }

        public DbSet<SlotMaster> slotmasters { get; set; }        

        public DbSet<VehicleMaster> vehiclemasters { get; set; }

        public DbSet<LocationMaster> locationmasters { get; set; }

        public DbSet<GradeMaster> grademasters { get; set; }

        public DbSet<SlabNarrationMaster> slabNarrations { get; set; }

        public DbSet<PlaceMaster> placemasters { get; set; }

        public DbSet<ProductGroupMaster> productgroupmasters { get; set; }

        public DbSet<TaxTypeMaster> taxtypemaster { get; set; }

        public DbSet<DisplayOrderMaster> displayordermasters { get; set; }

        public DbSet<CostFactorMaster> costfactormasters { get; set; }

        public DbSet<BankMaster> bankmasters { get; set; }

        public DbSet<SlabTypeMaster> slabtypemasters { get; set; } 

        public DbSet<TariffGroupMaster> tariffgroupmasters { get; set; }

        public DbSet<TariffMaster> tariffmasters { get; set; }

        public DbSet<Import_Slab_Handling_Type_Master> import_slab_handling_type_masters { get; set; }

        public DbSet<Import_Slab_Yard_Type_Master> import_slab_yard_type_masters { get; set; }

        public DbSet<Import_Slab_Wages_Type_Master> import_slab_wages_type_masters { get; set; }

        public DbSet<SlabMaster> slabmasters { get; set; }

        //public DbSet<Import_Seal_Master> import_seal_masters { get; set; }

        public DbSet<EmptyComponentCodeMaster> emptycomponentcodemasters { get; set; }  

        public DbSet<EmptyDamageCodeMaster> emptydamagecodemasters { get; set; }

        public DbSet<EmptyLocationCodeMaster> emptylocationcodemasters { get; set; }

        public DbSet<EmptyRepairCodeMaster> emptyrepaircodemasters { get; set; }

        public DbSet<EmptySlabtypeMaster> emptyslabtypemasters { get; set; }

        public DbSet<ContainerThruMaster> containerthrumasters { get; set; }

        public DbSet<StagMaster> stagmasters { get; set; }

        public DbSet<GodownMaster> godownmasters { get; set; }

        public DbSet<Export_OperationTypeMaster> Export_OperationTypeMaster { get; set; }

        public DbSet<ExportTariffOperationMaster> ExportTariffOperationMaster { get; set; }

        public DbSet<ExportTariffGroupMaster> ExportTariffGroupMasters { get; set; }

        public DbSet<ExportTariffTypeMaster> exporttarifftypemasters { get; set; }

        public DbSet<ExportTariffMaster> exporttariffmaster { get; set; }

        public DbSet<ExportSlabTypeMaster> exportslabtypemaster { get; set; }

        public DbSet<ExportSlabMaster> exportslabmasters { get; set; }
                
        public DbSet<Export_Invoice_Register> Export_Invoice_Register { get; set; }
        public DbSet<StuffingMaster> stuffingmasters { get; set; }
        public DbSet<StuffingDetail> stuffingdetails { get; set; }
        public DbSet<ContainerTypeMaster> containertypemasters { get; set; }

        public DbSet<ContainerStatusMaster> containerstatusmasters { get; set; }

        public DbSet<EndorsementChargeTypeMaster> EndorsementChargeTypeMasters { get; set; }

        public DbSet<EndorsementChargeMaster> EndorsementChargeMasters { get; set; }

        public DbSet<Export_Endorsement_Rate_Master> Export_Endorsement_Rate_Master { get; set; }

        public DbSet<Export_Slab_Yard_Type> exportslabyardtype { get; set; }

        public DbSet<Export_Slab_wages_type> exportslabwagestype { get; set; }

        public DbSet<Export_Slab_Charge_Type> exportslabchargetype { get; set; }

        public DbSet<VesselMaster> vesselmasters { get; set; }

        public DbSet<YardMaster> yardmasters { get; set; }

        public DbSet<PortTypeMaster> porttypemaster { get; set; }

        public DbSet<GPModeMaster> gpmodemasters { get; set; }

        public DbSet<GateInDetail> gateindetails { get; set; }

        public DbSet<GateOutDetail> gateoutdetail { get; set; }

        public DbSet<RemoteGateIn> remotegateindetails { get; set; }

        public DbSet<ImportDestuffSlipOperation> ImportDestuffSlipOperation { get; set; }

        public DbSet<OpenSheetMaster> opensheetmasters { get; set; }

        public DbSet<OpenSheetDetail> opensheetdetails { get; set; }

        public DbSet<ManualOpenSheetMaster> manualopensheetmasters { get; set; }

        public DbSet<ManualOpenSheetDetail> manualopensheetdetails { get; set; }

        public DbSet<AuthorizationSlipMaster> authorizatioslipmaster { get; set; }

        public DbSet<AuthorizationSlipDetail> authorizationslipdetail { get; set; }

        public DbSet<VehicleTicketDetail> vehicleticketdetail { get; set; }

        public DbSet<ExportSealTypeMaster> exportsealtypemasters { get; set; }

        public DbSet<Export_Seal_Type_Master> Export_Seal_Type_Masters { get; set; }
        
        public DbSet<ProductTypeMaster> producttypemasters { get; set; }

        public DbSet<ExportVehicleTypeMaster> exportvehicletypemasters { get; set; }

        public DbSet<ExportVehicleGroupMaster> exportvehiclegroupmasters { get; set; }

        public DbSet<ExportShippingBillMaster> exportshippingbillmasters { get; set; }

        public DbSet<StuffingProductDetail> stuffingproductdetails { get; set; }

        public DbSet<ShippingBillMaster> shippingbillmasters { get; set; }

        public DbSet<ShippingBillDetail> shippingbilldetails { get; set; }
       
        public DbSet<Export_Gateout_Gootype> export_Gateout_Gootypes { get; set; }

        public DbSet<OpenSheetViaDetails> opensheetviadetails { get; set; }

        public DbSet<BillEntryMaster> billentrymasters { get; set; }

        public DbSet<BillEntryDetail> billentrydetails { get; set; }

        public DbSet<Vw_Opensheet_Cbx_Assgn_01> view_opensheet_cbx_assign_01 { get; set; }

        public DbSet<Vw_ManualOpensheet_Cbx_Assgn_01> view_manualopensheet_cbx_assign_01 { get; set; }
        
        public DbSet<VW_Gatein_Block_Contnrno_Assgn> VW_Gatein_Block_Contnrno_Assgn { get; set; }

        public DbSet<VW_ESEAL_GATEINDETAIL_CONTAINERNO> VW_ESeal_GateInDetail_Containerno { get; set; }

        public DbSet<OpenSheetSealDetails> opensheetsealdetails { get; set; }

        public DbSet<Import_Seal_Master> importsealmasters { get; set; }

        public DbSet<ExportShippingBillDetail> billdetail { get; set; }

        public DbSet<DeliveryOrderMaster> DeliveryOrderMasters { get; set; }

        public DbSet<DeliveryOrderDetail> DeliveryOrderDetails { get; set; }

        public DbSet<Export_SealTypeMaster> Export_SealTypeMasters { get; set; }

        public DbSet<VW_NEW_CATEGORY_STATETYPE_ASSGN> VW_NEW_CATEGORY_STATETYPE_ASSGNs { get; set; }

        public DbSet<GateInLorryHireDetail> GateInLorryHireDetail { get; set; }

        public DbSet<VW_OPENSHEET_BILL_ENTRY_MID_ASSGN> VW_OPENSHEET_BILL_ENTRY_MID_ASSGNs { get; set; }

        public DbSet<GateInBlockDetails> gateinblockdetails { get; set; }

        public DbSet<VW_IMPORT_CONTAINER_DETAIL_QUERY_ASSGN> VW_IMPORT_CONTAINER_DETAIL_QUERY_ASSGNs { get; set; }

        public DbSet<Stuffing_Json_Detail> stuffing_json_details { get; set; }

        public DbSet<Stuffing_Message_Type> stuffing_message_types { get; set; }

        public DbSet<PortofReporting> portofreportings { get; set; }

        public DbSet<ReportingEvent> reportingevents { get; set; }

        public DbSet<ReportingPartyType> reportingpartytypes { get; set; }

        public DbSet<Equipment_Type_Master> equipment_type_masters { get; set; }

        public DbSet<Equipment_Load_Status> equipment_load_statuss { get; set; }

        public DbSet<Equipment_Seal_Type> equipment_seal_types { get; set; }

        public DbSet<Equipment_Status> equipment_status { get; set; }

        public DbSet<Equipment_UQC> equipment_UQCs { get; set; }

        public DbSet<Final_Destination_Code> final_destination_codes { get; set; }

        public DbSet<ShutoutVTDetail> shutoutvtdtl { get; set; }
        public DbSet<ExportCargoBlockDetails> exportcargoblockdtl { get; set; }
        public DbSet<CustomerGroupMaster> customergroupmasters { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }
            modelBuilder.Entity<OpenSheetMaster>().Property(x => x.OSMWGHT).HasPrecision(18, 4);
            //modelBuilder.Entity<BillEntryMaster>().Property(x => x.WGHT).HasPrecision(18, 4);
        }
    }

}