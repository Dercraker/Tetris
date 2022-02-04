using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;


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
        public BitmapImage ToImage { get; set; }
        public GameGird gameGrid { get; }
        public int Score { get; private set; }
        public WaitingLine waitingLine { get; }
        public bool gameOver { get; private set; }
        public int GameSpeed { get; set; }
        public int SpeedLevel { get; set; }
        public int time { get; private set; }
        public int lastScoreTime { get; set; }
        public int combos { get; set; }


        private DispatcherTimer Timer;

        public GameStatus()
        {
            Score = 0;
            GameSpeed = 400;
            SpeedLevel = 1;
            lastScoreTime = 0;
            combos = 1;
            gameGrid = new GameGird(22, 10);
            waitingLine = new WaitingLine();
            currentTetramino = waitingLine.UpdateTetramino();
        }

        public void SetTimer()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }
        public void SetReverseTimer()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 1);
            time = 45;
            Timer.Tick -= Timer_Tick;
            Timer.Start();
        }

        public void StopTimer()
        {
            Timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            time++;
        }

        public void DisplayLine(int r)
        {
            String str = String.Format("Line {0}",r);
            for (int c = 0; c < gameGrid.colums; c++)
            {
                str = String.Concat(str, ", ", gameGrid[r, c]);
            }
            //MessageBox.Show(String.Format("{0}", str));
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
            return false;
        }
        private void PlaceItem()
        {
            foreach (Position p in currentTetramino.positionsOfRotation())
            {
                gameGrid[p.row, p.column] = currentTetramino.tetraminoId;
            }
            Score += bonusScore(gameGrid.ClearGrid());
            NewGameSpeed(Score);
            if (IsGameOver())
            {
                gameOver = true;
            }
            else
            {
                //MessageBox.Show(String.Format("1 OffSet row :{0} , OffSet column :{1}", currentTetramino.offSet.row,currentTetramino.offSet.column));
                currentTetramino = waitingLine.UpdateTetramino();
                //MessageBox.Show(String.Format("2 OffSet row :{0} , OffSet column :{1}", currentTetramino.offSet.row,currentTetramino.offSet.column));
            }
        }
        public void NewGameSpeed(int score)
        {
            while (GameSpeed > 150 && score > SpeedLevel * 5)
            {
                SpeedLevel++;
                GameSpeed -= 50;
            }
        }
        public int bonusScore(int line)
        {
            int result = 0;
            result = multipleLineBonus(line);
            result = result == 0 ? 0 : combosTimeBonus(result);
            return result;
        }
        public int multipleLineBonus(int line)
        {
            int result = 0;
            for(int i = 0; i <= line; i++)
            {
                result += i;
            }
            return result;
        }
        public int combosTimeBonus(int result)
        {
            int currentTime = time;
            int diff = currentTime - lastScoreTime;
            if (diff <= 15 && time > 15)
            {
                if (combos < 10)
                {
                    combos++;
                }
            } else
            {
                combos = 1;
            }
            lastScoreTime = currentTime;
            return result * combos;
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
