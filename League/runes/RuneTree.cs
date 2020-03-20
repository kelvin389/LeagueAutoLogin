using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace League
{
    class RuneTree
    {
        // TODO: accessor methods
        public int id;
        public string name;
        public string iconPath;
        public List<RuneRow> runeRows = new List<RuneRow>();

        public RuneTree(List<RuneRow> rows, int id, string name, string path)
        {
            runeRows = rows;
            this.id = id;
            this.name = name;
            this.iconPath = path;
        }
    }
}
