using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class RazlogNaplate
    {
        public RazlogNaplate()
        {
            Naplata = new HashSet<Naplata>();
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite razlog naplate")]
        public string Razlog { get; set; }

        public ICollection<Naplata> Naplata { get; set; }
    }
}
