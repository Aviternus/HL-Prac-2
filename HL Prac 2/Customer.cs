//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HL_Prac_2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customer
    {
        public int id { get; set; }
        public string customer_name { get; set; }
        public Nullable<int> info_contact_id { get; set; }
        public Nullable<int> billing_contact_id { get; set; }
        public Nullable<int> billing_address_id { get; set; }
        public string terms { get; set; }
    
        public virtual Address Address { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual Contact Contact1 { get; set; }
    }
}
