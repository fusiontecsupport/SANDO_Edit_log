using System;

namespace scfs_erp.Models
{
    public class GateInDetailEditLogRow
    {
        public int GIDNO { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedOn { get; set; }
        public string Version { get; set; }
        public string Modules { get; set; }
    }
}
