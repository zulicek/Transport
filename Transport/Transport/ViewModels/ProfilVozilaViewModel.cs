using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transport.Models;

namespace Transport.ViewModels
{
    public class ProfilVozilaViewModel
    {
        public KorisnikovProfilViewModel KorisnikovProfil { get; set; }

        public List<Vozilo> Vozila { get; set; }
    }
}
