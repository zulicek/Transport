using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transport.Models;

namespace Transport.ViewModels
{
    public class PonudeVozilaViewModel
    {
        public List<PonudaPrijevoza> PonudePrijevoza { get; set; }
        public List<Vozilo> Vozila { get; set; }
        public List<Prijevoz> Prijevozi { get; set; }
    }
}
