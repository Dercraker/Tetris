using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    
    public class Save
    {
        public GameStatus Status { get; set; }
        public Save()
        {
            Status = new GameStatus();
        }
    }
}
