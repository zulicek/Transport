using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class Naplata
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite razlog naplate")]
        public int IdRazlog { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim uizaberite vrstu naplate")]
        public int IdVrstaNaplate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite prijevoz")]
        public int IdPrijevoz { get; set; }

        public DateTime? RokIzvrsenjaNaplate { get; set; }

        public Prijevoz IdPrijevozNavigation { get; set; }
        public RazlogNaplate IdRazlogNavigation { get; set; }
        public VrstaNaplate IdVrstaNaplateNavigation { get; set; }
    }
}
