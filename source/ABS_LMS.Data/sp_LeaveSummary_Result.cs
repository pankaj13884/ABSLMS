//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ABS_LMS.Data
{
    using System;
    
    public partial class sp_LeaveSummary_Result
    {
        public int LeaveTypeId { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Count { get; set; }
        public string Frequency { get; set; }
        public string EmployeeType { get; set; }
        public Nullable<decimal> Total { get; set; }
        public decimal LeaveTaken { get; set; }
        public decimal CarryForward { get; set; }
        public decimal Balance { get; set; }
    }
}
