using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Media;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace Tetris.gameMode
{
    internal class RevrseTetris : GameMode
    {
        public RevrseTetris(MainWindow mainWindow) : base(mainWindow)
        {
        }

        public override void init(GameStatus gm = null)
        {
            gameStatus = gm == null ? new GameStatus() : gm;
            gameStatus.GameMode = "Reverse-Tetris";
            gameStatus.timer.time = 60;
            mw.Draw(gameStatus);
            TetrisGM_Sound.PlayLooping();
        }

        public override async Task Run()
        {
            gameStatus.timer.SetReverseTimer();
            gameStatus.timer.SetTotalTimer();

            while (!gameStatus.gameOver)
            {
                if (gameStatus.timer.time <= 0)
                {
                    gameStatus.timer.time = 0;
                    gameStatus.gameOver = true;
                    break;
                }
                await Task.Delay(gameStatus.GameSpeed);
                gameStatus.MoveDownTetramino();
                mw.Draw(gameStatus);
                if (gameStatus.Pause) await gameStatus.GamePause(mw.PausePage, mw.MainTime, mw.TotalTime, mw.CurrentScore, mw.BreakLine, mw.BestCombos);
            }

            Stop();
        }

        public override void Stop()
        {
            TetrisGM_Sound.Stop();
            gameStatus.timer.StopTimer();
            gameStatus.timer.StopTotalTimer();
            mw.GameOverResult.Text = String.Format("Score : {0}", gameStatus.scores.score);
            mw.GameOverTimer.Text = String.Format("Time : {0}'{1}''", gameStatus.timer.time / 60, gameStatus.timer.time % 60);
            mw.MenuGameOver.Visibility = Visibility.Visible;
        }

    }
}