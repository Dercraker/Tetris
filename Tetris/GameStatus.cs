using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class GameStatus
    {
        private Tetramino currentTetramino;
        public Tetramino CurrentTetramino { 
            get => currentTetramino; 
            private set
            {
                currentTetramino = value;
                currentTetramino.reset();
            }
        }
        public GameGird gameGrid { get; }
        public WaitingLine waitingLine { get; }
        public bool gameOver { get; private set; }

        public GameStatus()
        {
            gameGrid = new GameGird(22, 10);
            waitingLine = new WaitingLine();
            currentTetramino = waitingLine.UpdateTetramino();
        }

        private bool IsValidTetramino()
        {
            foreach (Position p in currentTetramino.positionsOfRotation())
            {
                if (!gameGrid.IsEmptyBox(p.row, p.column))
                {
                    return false;
                }
            }
            return true;
        }

        public void RotateNextTetramino()
        {
            currentTetramino.NextRotate();
            if (!IsValidTetramino())
            {
                currentTetramino.PrevRotate();
            }
        }

        public void RotatePrevTetramino()
        {
            currentTetramino.PrevRotate();
            if (!IsValidTetramino())
            {
                currentTetramino.NextRotate();
            }
        }

        public void MoveRightTetramino()
        {
            currentTetramino.MoveTetramino(0, 1);
            if (!IsValidTetramino())
            {
                currentTetramino.MoveTetramino(0, -1); 
            }
        }
        public void MoveLeftTetramino()
        {
            currentTetramino.MoveTetramino(0, -1);
            if (!IsValidTetramino())
            {
                currentTetramino.MoveTetramino(0, 1);
            }
        }

        public bool IsGameOver()
        {
            if (!gameGrid.IsEmptyRow(0) && !gameGrid.IsEmptyRow(1))
            {
                return true;
            }
            return true;
        }

        private void PlaceItem()
        {
            foreach (Position p in currentTetramino.positionsOfRotation())
            {
                gameGrid[p.row, p.column] = currentTetramino.tetraminoId;
            }

            gameGrid.ClearGrid();

            if (IsGameOver())
            {
                gameOver = true;
            }
            else
            {
                MessageBox.Show(String.Format("{0}", "PlaceItem"));
                currentTetramino = waitingLine.UpdateTetramino();
            }
        }

        public void MoveDownTetramino()
        {
            currentTetramino.MoveTetramino(1, 0);
            if (!IsValidTetramino())
            {
                currentTetramino.MoveTetramino(-1, 0);
                PlaceItem();
            }
        }
    }
}
