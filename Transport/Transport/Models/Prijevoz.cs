using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Transport.Models
{
    public partial class Prijevoz
    {
        public Prijevoz()
        {
            Naplata = new HashSet<Naplata>();
        }

        public int Id { get; set; }
        public int? OcjenaPrijevoznika { get; set; }
        public int? OcjenaNarucitelja { get; set; }
        public string OpisUslugePrijevoznika { get; set; }
        public string OpisUslugeNarucitelja { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Molim izaberite ponudu prijevoza")]
        public int IdPonudaPrijevoza { get; set; }

        public PonudaPrijevoza IdPonudaPrijevozaNavigation { get; set; }
        public ICollection<Naplata> Naplata { get; set; }
    }
}
