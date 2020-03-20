using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace League
{
    public partial class RunesForm : Form
    {
        public RunesForm()
        {
            InitializeComponent();
        }
    }
}

    /*
     * 
     * [
     *  {
     *   -TREE- {id, key, icon, name}
     *    {
     *     "slots"
     *      "runes"
     *       {id, key, icon, name, descriptions}
     *      "runes"
     *      "runes"
     *      "runes"
     *    }
     *    -TREE- {id, key, icon, name}
     *    -TREE- {id, key, icon, name}
     *    -TREE- {id, key, icon, name}
     *    -TREE- {id, key, icon, name}
     *   }
     *  ]
     *  
     */

/*
 * Keystones and minor runes radio buttons should all
 * be generated on runtime to deal with potential changes
 * to runes in the future
 */

