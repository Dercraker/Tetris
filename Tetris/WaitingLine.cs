using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tetris.tetraminoList;

namespace Tetris
{
    public class WaitingLine
    {
        public Tetramino[] tetraminosTab = new Tetramino[]
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
            Tetramino newTetramino = tetraminosTab[random.Next(tetraminosTab.Length)];
            Tetramino newInstance = newTetramino.DeepCopy();
            newInstance.reset();
            return newInstance;
        }
        public Tetramino SetNextTetramino(int id)
        {
            Tetramino newTetramino = tetraminosTab[id];
            Tetramino newInstance = newTetramino.DeepCopy();
            newInstance.reset();
            return newInstance;
        }
        public WaitingLine(string NextTeraminoId = null)
        {
            if (NextTeraminoId == null)
            {
                NextTetramino = RandomTetramino();
            } else
            {
                NextTetramino = SetNextTetramino(int.Parse(NextTeraminoId));
            }
            
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
