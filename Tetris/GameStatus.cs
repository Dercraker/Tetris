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
        private Tetramino holdingTetramino;
        private Tetramino tempoTetramino;
        public int AddScore { get; private set; }

        public Tetramino HoldingTetramino
        {
            get => holdingTetramino;
            private set
            {
                holdingTetramino = value;
                holdingTetramino.reset();
            }
        }

        public Tetramino TempoTetramino
        {
            get => tempoTetramino;
            private set
            {
                tempoTetramino = value;
                tempoTetramino.reset();
            }
        }
        public BitmapImage ToImage { get; set; }


        public GameGird gameGrid { get; }
        public WaitingLine waitingLine { get; }
        public bool gameOver { get; set; }
        public int GameSpeed { get; set; }
        public string GameMode { get; set; }
        public bool Pause { get; set; }
        public int SpeedLevel { get; set; }
        public Scores scores { get; set; }

        public Timer timer { get; set; }

        private DispatcherTimer Timer = null!;
        private DispatcherTimer ReverseTotalTimer = null!;

        public int pressHoldTetramino = 0;

        public GameStatus()
        {
            GameSpeed = 400;
            SpeedLevel = 1;
            AddScore = 0;
            Pause = false;
            gameGrid = new GameGird(22, 10);
            waitingLine = new WaitingLine();
            currentTetramino = waitingLine.UpdateTetramino();
            scores = new Scores();
            holdingTetramino = null!;
        }

        public void DisplayLine(int r)
        {
            String str = String.Format("Line {0}",r);
            for (int c = 0; c < gameGrid.colums; c++)
            {
                str = String.Concat(str, ", ", gameGrid[r, c]);
            }
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
            AddScore = scores.bonusScore(gameGrid.ClearGrid());
            scores.score += AddScore;
            scores.GameScoreBonus(AddScore, gameGrid);
            if (GameMode == "Reverse-Tetris")
            {
                timer.time += AddScore * 15;
                if (IsGameOver())
                {
                    timer.time -= gameGrid.ReverseClearGrid() * 10;
                }
                else
                {
                    currentTetramino = waitingLine.UpdateTetramino();
                }
            } 
            else if (GameMode == "Tetris")
            {
                if (IsGameOver())
                {
                    gameOver = true;
                }
                else
                {
                    currentTetramino = waitingLine.UpdateTetramino();
                    pressHoldTetramino = 0;
                }
                }
            
            NewGameSpeed(scores.score);
            
        }
        public void NewGameSpeed(int score)
        {
            while (GameSpeed > 150 && score > SpeedLevel * 5)
            {
                SpeedLevel++;
                GameSpeed -= 50;
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
        public void RandomMove()
        {
            Random rand = new Random();
            int number = rand.Next(1, 101);

            if (number <= 20)
            {
                MoveDownTetramino();
            } else if (number > 20 && number <= 40)
            {
                MoveLeftTetramino();
            }else if (number > 40 && number <= 60)
            {
                MoveRightTetramino();
            }else if (number > 60 && number <= 80)
            {
                RotateNextTetramino();
            }else if (number > 80 && number <= 100)
            {
                RotatePrevTetramino();
            }
        }
        public int GetHarDropDistance(Position p)
        {
            int dropDistance = 0;

            while(gameGrid.IsEmptyBox(p.row + dropDistance + 1, p.column))
            {
                dropDistance++;
            }

            return dropDistance;
        }
        public int HardDropTetramino()
        {
            int dropDistance = gameGrid.rows;

            foreach(Position p in CurrentTetramino.positionsOfRotation())
            {
                dropDistance = Math.Min(dropDistance, GetHarDropDistance(p));
            }

            return dropDistance;
        }
        public void HardDrop()
        {
            CurrentTetramino.MoveTetramino(HardDropTetramino(), 0);
            PlaceItem();
        }
        public async Task GamePause(Grid Page, TextBlock MainTime, TextBlock TotalTime, TextBlock CurrentScore, TextBlock BreakLine, TextBlock BestCombos)
        {
            Timer.Stop();
            if (GameMode == "Reverse-Tetris") ReverseTotalTimer.Stop();
            MainTime.Text = String.Format("Time : {0}'{1}''", timer.time / 60, timer.time % 60);
            TotalTime.Text = String.Format("Total Time : {0}'{1}''", scores.reverseTotalTime / 60, scores.reverseTotalTime % 60);
            CurrentScore.Text = String.Format("Score : {0}pts", scores.score);
            BreakLine.Text = String.Format("Nb Line : {0}", scores.nbLine);
            BestCombos.Text = String.Format("Combos : *{0}", scores.bestCombos);

            if (GameMode == "Reverse-Tetris") TotalTime.Visibility = Visibility.Visible;
            if (GameMode == "Tetris") TotalTime.Visibility = Visibility.Hidden;

            Page.Visibility = Visibility.Visible;

            while (Pause)
            {
                await Task.Delay(1);
            }

            Page.Visibility = Visibility.Hidden;
            await Task.Delay(3000);
            Timer.Start();
            if (GameMode == "Reverse-Tetris") ReverseTotalTimer.Start();
        }
        public void HoldTetramino()
        {
            if (pressHoldTetramino == 0)
            {
                pressHoldTetramino++;
                if (holdingTetramino == null)
                {
                    currentTetramino.offSet = new Position(currentTetramino.SpawnPoint.row, currentTetramino.SpawnPoint.column);
                    currentTetramino.rotate = 0;
                    holdingTetramino = currentTetramino;
                    currentTetramino = waitingLine.UpdateTetramino();
                }
                else
                {
                    currentTetramino.offSet = new Position(currentTetramino.SpawnPoint.row, currentTetramino.SpawnPoint.column);
                    currentTetramino.rotate = 0;
                    tempoTetramino = holdingTetramino;
                    holdingTetramino = currentTetramino;
                    currentTetramino = tempoTetramino;
                }
            }
        }
    }
}
