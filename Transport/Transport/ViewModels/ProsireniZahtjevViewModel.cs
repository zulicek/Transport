using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transport.Models;
using System.ComponentModel.DataAnnotations;

namespace Transport.ViewModels
{


    public class ProsireniZahtjevViewModel
    {
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
        public StatusZahtjeva IdStatusZahtjevaNavigation { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite snaručitelja")]
        public int IdNarucitelj { get; set; }
        public Narucitelj IdNaruciteljNavigation { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite ulicu početne lokacije")]
        public string PocetnaLokacijaUlica { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite kućni broj početne lokacije")]
        public int PocetnaLokacijaKucniBroj { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite naziv početnog mjesta")]
        public string PocetnoMjesto { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite poštanski broj početnog mjesta")]
        public int PocetnoMjestoPbr { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite ulicu odredišne lokacije")]
        public string OdredisnaLokacijaUlica { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite kućni broj odredišne lokacije")]
        public int OdredisnaLokacijaKucniBroj { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite naziv odredišnog mjesta")]
        public string OdredisnoMjesto { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite poštanski broj odredišnog mjesta")]
        public int OdredisnoMjestoPbr { get; set; }
    }
}