//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Member.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MesR
    {
        public int MsRId { get; set; }
        public int MsId { get; set; }
        public int UsId { get; set; }
        public int StId { get; set; }
        public string MsRCont { get; set; }
        public System.DateTime MsRDate { get; set; }
        public bool MsCheck { get; set; }
    
        public virtual Memb Memb { get; set; }
        public virtual Mes Mes { get; set; }
        public virtual Stock Stock { get; set; }
    }
}