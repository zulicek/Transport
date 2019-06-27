using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class StatusPonude
    {
        public StatusPonude()
        {
            PonudaPrijevoza = new HashSet<PonudaPrijevoza>();
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite status ponude")]
        public string Status { get; set; }

        public ICollection<PonudaPrijevoza> PonudaPrijevoza { get; set; }
    }
}
