using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.tetraminoList;

namespace Tetris
{
    public class WaitingLine
    {
        public Tetramino[] tetramino = new Tetramino[]
        {
            new ITetramino(),
            new JTetramino(),
            new LTetramino(),
            new OTetramino(),
            new STetramino(),
            new TTetramino(),
            new ZTetramino(),
        };
        public Random random = new Random();
        public Tetramino NextTetramino { get; set; }
        public Tetramino RandomTetramino()
        {
            return tetramino[random.Next(tetramino.Length)];
        }
        public WaitingLine()
        {
            NextTetramino = RandomTetramino();
        }
        public Tetramino UpdateTetramino()
        {
            Tetramino tetramino = NextTetramino;

            do
            {
                NextTetramino = RandomTetramino();
            }
            while(tetramino.tetraminoId == NextTetramino.tetraminoId);
            return tetramino;
        }
    }
}
