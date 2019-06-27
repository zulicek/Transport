using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transport.Models;
using System.ComponentModel.DataAnnotations;

namespace Transport.ViewModels
{
    public class ZahtjeviLokacijeViewModel
    {
        public List<Zahtjev> Zahtjevi { get; set; }
        public List<ZahtjevLokacija> ZahtjeviLokacije { get; set; }
    }
}
