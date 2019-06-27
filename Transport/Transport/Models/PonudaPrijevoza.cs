using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class PonudaPrijevoza
    {
        public PonudaPrijevoza()
        {
            Prijevoz = new HashSet<Prijevoz>();
        }

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim unesite cijenu prijevoza")]
        public double Cijena { get; set; }
        public DateTime? RokIstekaPonude { get; set; }
        public DateTime? RokOtkazaPonude { get; set; }
        public double? CijenaOtkaza { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite zahtjev")]
        public int IdZahtjev { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite prijevoznika")]
        public int IdPrijevoznik { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite status ponude")]
        public int IdStatusPonude { get; set; }

        public Prijevoznik IdPrijevoznikNavigation { get; set; }
        public StatusPonude IdStatusPonudeNavigation { get; set; }
        public Zahtjev IdZahtjevNavigation { get; set; }
        public ICollection<Prijevoz> Prijevoz { get; set; }
    }
}
