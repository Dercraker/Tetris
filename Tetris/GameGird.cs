using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class GameGird{
        private int[,] grid;
        public int colums { get; }
        public int rows { get; }
        public int this[int r, int c]
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }

        public bool IsInsideGrid(int r, int c)
        {
            return  r >= 0 && r < rows && c >= 0 && c < colums;
        }
        public bool IsEmptyBox(int r, int c)
        {
            return IsInsideGrid(r, c) && grid[r, c] == 0;
        }
        public bool IsEmptyRow(int r)
        {
            for ( int i = 0; i< colums; i++)
            {
                if (grid[r, i] != 0) return false;
            }
            return true;
        }

        public bool IsFullRow(int r)
        {
            return !IsEmptyRow(r);
        }

        public void CleanRow(int r)
        {
            for (int i = 0; i < colums; i++)
            {
                grid[r, i] = 0;
            }
        }
        public void DownRow(int r, int nbRow)
        {
            for (int c = 0; c < colums; c++)
            {
                grid[r + nbRow, c] = grid[r,c];
                grid[r,c] = 0;
            }
        }

        public int ClearGrid()
        {
            int nbRowClear = 0;

            for (int i = rows-1; i >= 0; i--)
            {
                if (IsFullRow(i))
                {
                    nbRowClear++;
                    CleanRow(i);
                } else if (nbRowClear > 0 ) {
                    DownRow(i, nbRowClear);
                }
            }
            return nbRowClear;
        }

        public GameGird(int rows, int colums)
        {
            this.rows = rows;
            this.colums = colums;
            this.grid = new int[rows, colums];
        }
    }
}
