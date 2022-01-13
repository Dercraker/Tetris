using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.tetraminoList
{
    public class JTetramino : Tetramino
    {
        private Position[][] boxRotation = new Position[][]
        {
            new Position[] { new Position(0,0), new Position(1,0), new Position(1,1), new Position(1,2) },
            new Position[] { new Position(0,1), new Position(0,2), new Position(1,1), new Position(2,1) },
            new Position[] { new Position(1,0), new Position(1,1), new Position(1,2), new Position(2,2) },
            new Position[] { new Position(0,1), new Position(1,1), new Position(2,0), new Position(2,1) },
        };
        public override int tetraminoId => 2;
        protected override Position SpawnPoint => new Position(0, 3);
        protected override Position[][] BoxRotation => boxRotation;
    }
}
