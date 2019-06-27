using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class Vozilo
    {
        public int Id { get; set; }
        public string Tip { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite marku vozila")]
        public string Marka { get; set; }
        public string Boja { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite registarsku oznaku vozila")]
        public string RegistarskaOznaka { get; set; }
        public bool? Ekolosko { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite prijevoznika")]
        public int IdPrijevoznik { get; set; }

        public Prijevoznik IdPrijevoznikNavigation { get; set; }
    }
}
