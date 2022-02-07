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

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow mainWindow;
        public ImageSource[] tetraminoImages = new ImageSource[]
        {
            new BitmapImage(new Uri("../Resources/cleanTetramino.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/IDiamound.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/JLapilazuli.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/LRedStone.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/OGold.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/SCharbon.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/TEmerald.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/ZIron.png", System.UriKind.Relative))
        };

        public ImageSource[] boxImages = new ImageSource[]
        {
            new BitmapImage(new Uri("../Resources/CleanBox.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/diamond_block.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/lapis_block.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/redstone_block.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/gold_block.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/coal_block.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/emerald_block.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../Resources/iron_block.png", System.UriKind.Relative))
        };


        public Image[,] imgControls;
        public GameStatus gameStatus = new GameStatus();

        public SoundPlayer SoundMenu = new SoundPlayer(Resource1.MainMenuSound);
        public SoundPlayer TetrisGM_Sound = new SoundPlayer(Resource1.Tetris_99_Main_Theme);

        public MainWindow()
        {
            InitializeComponent();
            imgControls = SetUpGameGridCanvas(gameStatus.gameGrid);

            SoundMenu.PlayLooping();
        }

        public Image[,] SetUpGameGridCanvas(GameGird g)
        {
            Image[,] imgControls = new Image[g.rows, g.colums];
            int boxSize = 25;
            for (int r = 0; r < g.rows; r++)
            {
                for (int c = 0; c < g.colums; c++)
                {
                    Image imgControl = new Image
                    {
                        Width = boxSize,
                        Height = Width
                    };
                    Canvas.SetLeft(imgControl, c * boxSize);
                    Canvas.SetTop(imgControl, (r - 2) * boxSize + 10);
                    GameGridCanvas.Children.Add(imgControl);
                    imgControls[r, c] = imgControl;
                }
            }
            return imgControls;
        }
        public void GetNextBlock(WaitingLine waitingLine)
        {
            Tetramino nextTetramino = waitingLine.NextTetramino;
            NextImage.Source = tetraminoImages[nextTetramino.tetraminoId];
        }
        public void DrawGrid(GameGird g)
        {
            for (int r = 0; r < g.rows; r++)
            {
                for (int c = 0; c < g.colums; c++)
                {
                    int boxId = g[r, c];
                    imgControls[r, c].Source = boxImages[boxId];
                }
            }
        }
        public void DrawBox(Tetramino t)
        {
            foreach (Position p in t.positionsOfRotation())
            {
                imgControls[p.row, p.column].Source = boxImages[t.tetraminoId];
            }
        }
        public void Draw(GameStatus g)
        {
            DrawGrid(g.gameGrid);
            DrawBox(g.CurrentTetramino);
            ScoreText.Text = String.Format("Score : {0}", gameStatus.Score);
            if (gameStatus.combos > 1)
            {
                CombosText.Visibility = Visibility.Visible;
                CombosText.Text = String.Format("Combos : {0}", gameStatus.combos);
            }
            else
            {
                CombosText.Visibility = Visibility.Hidden;
            }
            TimerCount.Text = String.Format("Timer : {0}'{1}''", gameStatus.time / 60, gameStatus.time % 60);
            GetNextBlock(gameStatus.waitingLine);
        }

        private async void KeyInput(object sender, KeyEventArgs e)
        {
            if (gameStatus.gameOver)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Right:
                    gameStatus.MoveRightTetramino();
                    break;
                case Key.Left:
                    gameStatus.MoveLeftTetramino();
                    break;
                case Key.Down:
                    gameStatus.MoveDownTetramino();
                    break;
                case Key.Q:
                    gameStatus.RotatePrevTetramino();
                    break;
                case Key.D:
                    gameStatus.RotateNextTetramino();
                    break;
                default:
                    return;
            }
            Draw(gameStatus);
        }
        private async void RestartGame(object sender, RoutedEventArgs e)
        {
            gameStatus = new GameStatus();
            MenuGameOver.Visibility = Visibility.Hidden;

            switch (gameStatus.GameMode)
            {
                case "Tetris":
                    {
                        await GameRun();
                        break;
                    }
                case "Reverse-Tetris":
                    {
                        ReverseTetrisInit();
                        break;
                    }

            }
        }
        private void ReturnMainMenu(object sender, RoutedEventArgs e)
        {
            gameStatus = new GameStatus();
            MainMenu.Visibility = Visibility.Visible;

            SoundMenu.Stream.Position = 0;
            SoundMenu.PlayLooping();

            MenuGameOver.Visibility = Visibility.Hidden;
        }
        private void OutOption(object sender, RoutedEventArgs e)
        {
            OptionPage.Visibility = Visibility.Hidden;
        }
        private void KillProgram(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private async void LaunchGame(object sender, RoutedEventArgs e)
        {

            SoundMenu.Stop();
            MainMenu.Visibility = Visibility.Hidden;
            gameStatus.GameMode = DropDownGameModes.SelectedValue.ToString().Split(" ")[1];

            switch (gameStatus.GameMode)
            {
                case "Tetris":
                    {
                        await GameRun();
                        break;
                    }
                case "Reverse-Tetris":
                    {
                        ReverseTetrisInit();
                        break;
                    }

            }
        }
        private void Options(object sender, RoutedEventArgs e)
        {
            OptionPage.Visibility = Visibility.Visible;
        }

        /////////////////
        /// TETRIS GM ///
        /////////////////

        public async Task GameRun()
        {
            gameStatus.GameMode = "Tetris";
            gameStatus.time = 0;
            Draw(gameStatus);
            gameStatus.SetTimer();
            TetrisGM_Sound.PlayLooping();

            while (!gameStatus.gameOver)
            {
                await Task.Delay(gameStatus.GameSpeed);
                gameStatus.MoveDownTetramino();
                Draw(gameStatus);
            }

            TetrisGM_Sound.Stop();
            gameStatus.StopTimer();
            GameOverResult.Text = String.Format("Score : {0}", gameStatus.Score);
            GameOverTimer.Text = String.Format("Time : {0}'{1}''", gameStatus.time / 60, gameStatus.time % 60);
            MenuGameOver.Visibility = Visibility.Visible;
        }

        /////////////////////////
        /// REVERSE-TETRIS GM ///
        /////////////////////////
        
        public async void ReverseTetrisInit()
        {
            gameStatus.GameMode = "Reverse-Tetris";
            gameStatus.time = 60;
            await GameRun2();
        }

        public async Task GameRun2()
        {
            Draw(gameStatus);
            gameStatus.SetReverseTimer();
            TetrisGM_Sound.PlayLooping();

            while (!gameStatus.gameOver)
            {
                if (gameStatus.time <= 0)
                {
                    gameStatus.gameOver = true;
                    break;
                }
                await Task.Delay(gameStatus.GameSpeed);
                gameStatus.MoveDownTetramino();
                Draw(gameStatus);
            }

            TetrisGM_Sound.Stop();
            gameStatus.StopTimer();
            GameOverResult.Text = String.Format("Score : {0}", gameStatus.Score);
            GameOverTimer.Text = String.Format("Time : {0}'{1}''", gameStatus.time / 60, gameStatus.time % 60);
            MenuGameOver.Visibility = Visibility.Visible;
        }
    }
}
