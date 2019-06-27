using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transport.Models;
using Transport.ViewModels;

namespace Transport.ViewModels
{
    public class ProsireniPrijevozViewModel
    {
        public int Id { get; set; }
        public ProsireniZahtjevViewModel ProsireniZahtjev { get; set; }
        public Prijevoz Prijevoz { get; set; }
        public Naplata NaplataNaručitelju { get; set; }
        public Naplata NaplataPrijevozniku { get; set; }
    }
}
