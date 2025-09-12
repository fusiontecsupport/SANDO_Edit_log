using scfs_erp.Controllers.Import;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace scfs_erp.Models
{
    [JsonObject(Title = "user")]
    public class Export_SCMTR_Stuffing
    {
        public class SCMTR_Header
        {
            public string indicator { get; set; }//"P",
            public string messageID { get; set; } // "CUCHE01"
            public int sequenceOrControlNumber { get; set; } // 1004
            public string date { get; set; } // "20210709"
            public string time { get; set; } // "T11:21"
            public string reportingEvent { get; set; } // "SF"
            public string senderID { get; set; } // "INMAA1STL1"
            public string receiverID { get; set; } // "INMAA1"
            public string versionNo { get; set; } // "1.0"
        }

        public class SCMTR_MASTERS
        {
            //public List<SCMTR_Cargo_Details> cargoDetails { get; set; }
            public SCMTR_Declaration declaration { get; set; }
            public SCMTR_Location location { get; set; }
            public List<SCMTR_Cargo_Container> cargoContainer { get; set; }
            //public SCMTR_Cargo_Container cargoContainer { get; set; }

        }

        public class SCMTR_Declaration
        {
            public string messageType { get; set; }//"F",
            public string portOfReporting { get; set; } // "INMAA1"
            public int jobNo { get; set; } // 1004
            public string jobDate { get; set; } // "20210709"
            public string reportingEvent { get; set; } // "SF"

        }

        public class SCMTR_Location
        {
            public string reportingPartyType { get; set; }//"F",
            public string reportingPartyCode { get; set; } // "INMAA1"
            public string reportingLocationCode { get; set; } // 1004
            public string reportingLocationName { get; set; } // "20210709"
            public string authorisedPersonPAN { get; set; } // "SF"

        }

        public class SCMTR_Cargo_Container
        {
            public string messageType { get; set; }//"F",
            public int equipmentSequenceNo { get; set; } // 1
            public string equipmentID { get; set; } // "WHLU2472835"
            public string equipmentType { get; set; } // "20210709"
            public string equipmentSize { get; set; } // "20"
            public string equipmentLoadStatus { get; set; } // "20"
            public string finalDestinationLocation { get; set; } // "20"
            public string eventDate { get; set; } // "20"
            public string equipmentSealType { get; set; } // "20"
            public string equipmentSealNumber { get; set; } // "20"
            public string equipmentStatus { get; set; } // "20"
            public string equipmentPkg { get; set; } // "20"
            public int equipmentQuantity { get; set; } // "20"
            public string equipmentQUC { get; set; } // "20"
            public List<SCMTR_Cargo_Details> cargoDetails { get; set; }

        }

        public class SCMTR_Cargo_Details
        {
            public string messageType { get; set; }//"F",
            public int cargoSequenceNo { get; set; }//"F",
            public string documentType { get; set; }//"F",
            public string documentSite { get; set; }//"F",
            public int documentNumber { get; set; }//"F",
            public string documentDate { get; set; }//"F",
            public string shipmentLoadStatus { get; set; }//"F",
            public string packageType { get; set; }//"F",
            public int quantity { get; set; }//"F",
            public int packetsFrom { get; set; }//"F",
            public int packetsTo { get; set; }//"F",
            public string packUQC { get; set; }//"F",
            public string mcinPcin { get; set; }//"F",

        }

        public class Response
        {
            //public string Version { get; set; }
            public SCMTR_Header headerField { get; set; }
            public SCMTR_MASTERS master { get; set; }
            //public string Version { get; set; }
            //public DocDtls DocDtls { get; set; }
            //public SellerDtls SellerDtls { get; set; }
            //public BuyerDtls BuyerDtls { get; set; }
            //public ValDtls ValDtls { get; set; }
            //public RefDtls RefDtls { get; set; }
            //
        }
    }
}