using System;
using System.Collections.Generic;

namespace Transport.Models
{
    public partial class Prijevoznik
    {
        public Prijevoznik()
        {
            PonudaPrijevoza = new HashSet<PonudaPrijevoza>();
            Vozilo = new HashSet<Vozilo>();
        }

        public int IdKorisnik { get; set; }
        public string NazivTvrtke { get; set; }

        public Korisnik IdKorisnikNavigation { get; set; }
        public ICollection<PonudaPrijevoza> PonudaPrijevoza { get; set; }
        public ICollection<Vozilo> Vozilo { get; set; }
    }
}
