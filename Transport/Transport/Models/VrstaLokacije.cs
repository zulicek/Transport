using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class VrstaLokacije
    {
        public VrstaLokacije()
        {
            Lokacija = new HashSet<Lokacija>();
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite vrstu lokacije")]
        public string Vrsta { get; set; }

        public ICollection<Lokacija> Lokacija { get; set; }
    }
}
