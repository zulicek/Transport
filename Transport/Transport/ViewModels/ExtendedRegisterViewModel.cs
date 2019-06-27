using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Transport.ViewModels
{
    public class ExtendedRegisterViewModel
    {

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite OIB korisnika")]
        public string Oib { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite ime korisnika")]
        public string Ime { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite prezime korisnika")]
        public string Prezime { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite telefonski broj korisnika")]
        public string TelBroj { get; set; }

        [Required(ErrorMessage = "Molim izaberite ulogu")]
        public string Uloga { get; set; }

    }
}
