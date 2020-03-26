using System.Collections.Generic;

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
