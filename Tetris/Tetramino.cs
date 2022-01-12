using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public abstract class Tetramino
    {
        public Position[][] boxRotation { get; set; }
        public Position spawnPoint { get; set; }
        public int tetraminoId {  get; set; }
        public int rotate { get; set; }
        public Position offSet { get; set; }

        public Tetramino()
        {
            offSet = new Position(spawnPoint.row, spawnPoint.column);
        }

        public IEnumerable<Position> positionsOfRotation()
        {
            foreach (Position p in boxRotation[rotate])
            {
                yield return new Position(p.row + offSet.row, p.column + offSet.column);
            }
        }

        public void NextRotate()
        {
            rotate = (rotate + 1) % boxRotation.Length;
        }
        public void PrevRotate()
        {
            if (rotate == 0)
            {
                rotate = boxRotation.Length - 1;
            }
            else
            {
                rotate--;
            }
        }
        public void MoveTetramino(int r, int c)
        {
            offSet.row = r;
            offSet.column = c;
        }
        public void reset()
        {
            rotate = 0;
            offSet = new Position(spawnPoint.row, spawnPoint.column);
        }

    }
}
