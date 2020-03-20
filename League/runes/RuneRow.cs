using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace League
{
    class RuneRow
    {
        // TODO: accessor methods
        public List<Rune> runes = new List<Rune>();

        public RuneRow(List<Rune> runes)
        {
            this.runes = runes;
        }
    }
}
