using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transport.Models;

namespace Transport.ViewModels
{
    public class KorisniciVozilaViewModel
    {
        public List<Korisnik> Korisnici { get; set; }
        public List<Narucitelj> Narucitelji { get; set; }
        public List<Prijevoznik> Prijevoznici { get; set; }
        public List<Vozilo> Vozila { get; set; }
    }
}
