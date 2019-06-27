using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class StatusZahtjeva
    {
        public StatusZahtjeva()
        {
            Zahtjev = new HashSet<Zahtjev>();
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite status zahtjeva")]
        public string Status { get; set; }

        public ICollection<Zahtjev> Zahtjev { get; set; }
    }
}
