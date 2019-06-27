using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transport.Models;
using Transport.ViewModels;

namespace Transport.ViewModels
{
    public class ZahtjevPonudeViewModel
    {
        public ProsireniZahtjevViewModel ProsireniZahtjev { get; set; }

        public List<PonudaPrijevoza> PonudePrijevoza { get; set; }

        public List<Vozilo> Vozila { get; set; }
    }
}
