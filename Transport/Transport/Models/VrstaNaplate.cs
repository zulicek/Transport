using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class VrstaNaplate
    {
        public VrstaNaplate()
        {
            Naplata = new HashSet<Naplata>();
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite  vrstu naplate")]
        public string Vrsta { get; set; }

        public ICollection<Naplata> Naplata { get; set; }
    }
}
