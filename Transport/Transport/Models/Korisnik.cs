using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class Korisnik
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite OIB korisnika")]
        public string Oib { get; set; }

        public bool PrimaEmail { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite ime korisnika")]
        public string Ime { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite prezime korisnika")]
        public string Prezime { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite lozinku korisnika")]
        public string Lozinka { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite telefonski broj korisnika")]
        public string TelBroj { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite e-mail adresu korisnika")]
        public string Email { get; set; }

        public Narucitelj Narucitelj { get; set; }
        public Prijevoznik Prijevoznik { get; set; }
    }
}
