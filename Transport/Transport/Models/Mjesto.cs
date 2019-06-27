using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class Mjesto
    {
        public Mjesto()
        {
            Lokacija = new HashSet<Lokacija>();
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite naziv mjesta")]
        public string Naziv { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite poštanski broj mjesta")]
        public int PostanskiBroj { get; set; }

        public ICollection<Lokacija> Lokacija { get; set; }
    }
}
