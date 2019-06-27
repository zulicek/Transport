using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class Lokacija
    {
        public Lokacija()
        {
            ZahtjevLokacija = new HashSet<ZahtjevLokacija>();
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite mjesto lokacije")]
        public int IdMjesto { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite vrstu lokacije")]
        public int IdVrstaLokacije { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite ulicu")]
        public string Ulica { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite kućni broj")]
        public int KucniBroj { get; set; }

        public Mjesto IdMjestoNavigation { get; set; }
        public VrstaLokacije IdVrstaLokacijeNavigation { get; set; }
        public ICollection<ZahtjevLokacija> ZahtjevLokacija { get; set; }
    }
}
