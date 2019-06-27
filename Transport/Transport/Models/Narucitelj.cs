using System;
using System.Collections.Generic;

namespace Transport.Models
{
    public partial class Narucitelj
    {
        public Narucitelj()
        {
            Zahtjev = new HashSet<Zahtjev>();
        }

        public int IdKorisnik { get; set; }
        public string ZahtijevaEko { get; set; }

        public Korisnik IdKorisnikNavigation { get; set; }
        public ICollection<Zahtjev> Zahtjev { get; set; }
    }
}
