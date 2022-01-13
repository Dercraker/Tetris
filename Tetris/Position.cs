using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Position
    {
        public int row { get; set; }
        public int column { get; set; }


        public Position(int r, int c)
        {
            this.row = r;
            this.column = c;
        }
    }
}
