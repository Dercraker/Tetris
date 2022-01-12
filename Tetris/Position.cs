using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Position
    {
        public int row { get => row; set => row = value;}
        public int column { get => column; set => row = value;}


        public Position(int r, int c)
        {
            this.row = r;
            this.column = c;
        }
    }
}
