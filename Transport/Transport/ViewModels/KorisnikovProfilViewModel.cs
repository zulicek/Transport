using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Transport.Models;

namespace Transport.ViewModels
{
    public class KorisnikovProfilViewModel
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite OIB korisnika")]
        public string Oib { get; set; }

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

        public bool PrimaEmail { get; set; }

        public string ZahtijevaEko { get; set; }

        public string NazivTvrtke { get; set; }

        public float OcjenaNarucitelj { get; set; }

        public float OcjenaPrijevoznik { get; set; }

    }
}
