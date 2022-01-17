using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public abstract class Tetramino
    {
        protected abstract Position[][] BoxRotation { get;}
        protected  abstract Position SpawnPoint { get; }
        public abstract int tetraminoId { get;}
        private int rotate;
        public Position offSet { get; set; }

        public Tetramino()
        {
            offSet = new Position(SpawnPoint.row, SpawnPoint.column);
        }
        public Tetramino DeepCopy()
        {
            return (Tetramino)this.MemberwiseClone();
        }

        public IEnumerable<Position> positionsOfRotation()
        {
            foreach (Position p in BoxRotation[rotate])
            {
                yield return new Position(p.row + offSet.row, p.column + offSet.column);
            }
        }

        public void NextRotate()
        {
            rotate = (rotate + 1) % BoxRotation.Length;
        }
        public void PrevRotate()
        {
            if (rotate == 0)
            {
                rotate = BoxRotation.Length - 1;
            }
            else
            {
                rotate--;
            }
        }
        public void MoveTetramino(int r, int c)
        {
            offSet.row += r;
            offSet.column += c;
        }
        public void reset()
        {
            rotate = 0;
            //MessageBox.Show(String.Format("{0}", offSet));
            offSet.row = SpawnPoint.row;
            offSet.column = SpawnPoint.column;
            //MessageBox.Show(String.Format("{0}", offSet));
        }

    }
}
