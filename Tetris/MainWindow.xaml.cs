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
        System.IO.Stream mainMenu = Resource1.MainMenuSound;
        //public Scores scores = new Scores();

        public MainWindow()
        {
            InitializeComponent();
            imgControls = SetUpGameGridCanvas(gameStatus.gameGrid);
            SoundPlayer menu = new SoundPlayer(mainMenu);
            menu.PlayLooping();
        }

        public Image[,] SetUpGameGridCanvas(GameGird g)
        {
            Image[,] imgControls = new Image[g.rows,g.colums];
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
                    Canvas.SetTop(imgControl, (r - 2) * boxSize+10);
                    GameGridCanvas.Children.Add(imgControl);
                    imgControls[r,c] = imgControl;
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
            for (int r = 0;r < g.rows; r++)
            {
                for (int c = 0; c < g.colums; c++)
                {
                    int boxId = g[r,c];
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
            ScoreText.Text = String.Format("Score : {0}", gameStatus.scores.score);
            if (gameStatus.combos > 1)
            {
                CombosText.Visibility = Visibility.Visible;
                CombosText.Text = String.Format("Combos : {0}", gameStatus.combos);
            } else
            {
                CombosText.Visibility = Visibility.Hidden;
            }
            TimerCount.Text = String.Format("Timer : {0}'{1}''", gameStatus.time/60, gameStatus.time%60);
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
                    break ;
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
        public async Task Game()
        {
            Draw(gameStatus);
            gameStatus.SetTimer();

            System.IO.Stream mainThemeTetris = Resource1.Tetris_99_Main_Theme;
            SoundPlayer player = new SoundPlayer(mainThemeTetris);

            player.PlayLooping();

            while (!gameStatus.gameOver)
            {
                await Task.Delay(gameStatus.GameSpeed);
                gameStatus.MoveDownTetramino();
                Draw(gameStatus);
            }

            player.Stop();
            gameStatus.StopTimer();
            GameOverResult.Text = String.Format("Score : {0}", gameStatus.scores.score);
            GameOverTimer.Text = String.Format("Time : {0}'{1}''", gameStatus.time/60, gameStatus.time%60);
            MenuGameOver.Visibility = Visibility.Visible;

        }
        private async void RestartGame(object sender, RoutedEventArgs e)
        {
            gameStatus = new GameStatus();
            MenuGameOver.Visibility= Visibility.Hidden;
            await Game();
        }
        private void ReturnMainMenu(object sender, RoutedEventArgs e)
        {
            gameStatus = new GameStatus();
            MainMenu.Visibility= Visibility.Visible;
            SoundPlayer menu = new SoundPlayer(mainMenu);
            menu.Stream.Position = 0;
            menu.PlayLooping();
            MenuGameOver.Visibility = Visibility.Hidden;
        }
        private void OutOption(object sender, RoutedEventArgs e)
        {
            OptionPage.Visibility= Visibility.Hidden;
        }
        private void KillProgram(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private async void LaunchGame(object sender, RoutedEventArgs e)
        {
            SoundPlayer menu = new SoundPlayer(mainMenu);
            menu.Stop();
            MainMenu.Visibility = Visibility.Hidden;
            await Game();

        }
        private void Options(object sender, RoutedEventArgs e)
        {
            OptionPage.Visibility = Visibility.Visible;
        }
    }
}
