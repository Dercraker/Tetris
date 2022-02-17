using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Position
    {
        //Initialisation des variables
        public int row { get; set; }
        public int column { get; set; }

        //Constructor
        public Position(int r, int c)
        {
            this.row = r;
            this.column = c;
        }
    }
}
