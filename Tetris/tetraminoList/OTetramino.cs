﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.tetraminoList
{
    public class OTetramino : Tetramino
    {
        private Position[][] boxRotation = new Position[][]
        {
            new Position[] { new Position(0,0), new Position(0,1), new Position(1,0), new Position(1,1) }
        };
        public override int tetraminoId => 4;
        public override Position SpawnPoint => new Position(0, 4);
        protected override Position[][] BoxRotation => boxRotation;
    }
}
