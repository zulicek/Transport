using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class ZahtjevLokacija
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite zahtjev")]
        public int IdZahtjev { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite lokaciju")]
        public int IdLokacija { get; set; }

        public Lokacija IdLokacijaNavigation { get; set; }
        public Zahtjev IdZahtjevNavigation { get; set; }
    }
}
