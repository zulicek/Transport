using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class Zahtjev
    {
        public Zahtjev()
        {
            PonudaPrijevoza = new HashSet<PonudaPrijevoza>();
            ZahtjevLokacija = new HashSet<ZahtjevLokacija>();
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite datum i vrijeme kad je najranije potrebno prevesti teret")]
        public DateTime VrijemePocetka { get; set; }
        public DateTime? VrijemeZavrsetka { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite cijenu neizvršenja prijevoza")]
        public double CijenaNeizvrsenja { get; set; }
        public double? Sirina { get; set; }
        public double? Visina { get; set; }
        public double? Duiljina { get; set; }
        public double? Masa { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite  opis zathjeva")]
        public string Opis { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite status zahtjeva")]
        public int IdStatusZahtjeva { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite snaručitelja")]
        public int IdNarucitelj { get; set; }

        public Narucitelj IdNaruciteljNavigation { get; set; }
        public StatusZahtjeva IdStatusZahtjevaNavigation { get; set; }
        public ICollection<PonudaPrijevoza> PonudaPrijevoza { get; set; }
        public ICollection<ZahtjevLokacija> ZahtjevLokacija { get; set; }
    }
}
