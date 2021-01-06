using Member.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Member.ViewModels
{
    public class VM
    {
        public List<Memb> Memb { get; set; }
        public List<IndustryD> IndustryD { get; set; }
        public List<Industry> Industry { get; set; }
        public List<Collection> Collection { get; set; }
        public List<MesR> MesR { get; set; }
        public List<MesS> MesS { get; set; }
        public List<Mes> Mes { get; set; }

    }
    public class MesRpt
    {
        public IEnumerable<MesR> MesR { get; set; }
        public IEnumerable<Mes> Mes { get; set; }

    }
}