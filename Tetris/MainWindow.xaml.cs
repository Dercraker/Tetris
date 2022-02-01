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
<<<<<<< HEAD


=======
>>>>>>> MUSIC-AmauryF

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ImageSource[] tetraminoImages = new ImageSource[]
        {
            new BitmapImage(new Uri("../assets/tetramino/cleanTetramino.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/tetramino/IDiamound.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/tetramino/JLapilazuli.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/tetramino/LRedStone.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/tetramino/OGold.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/tetramino/SCharbon.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/tetramino/TEmerald.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/tetramino/ZIron.png", System.UriKind.Relative))
        };

        public ImageSource[] boxImages = new ImageSource[]
        {
            new BitmapImage(new Uri("../assets/fullbox/CleanBox.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/fullbox/diamond_block.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/fullbox/lapis_block.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/fullbox/redstone_block.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/fullbox/gold_block.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/fullbox/coal_block.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/fullbox/emerald_block.png", System.UriKind.Relative)),
            new BitmapImage(new Uri("../assets/fullbox/iron_block.png", System.UriKind.Relative))
        };


        public Image[,] imgControls;
        public GameStatus gameStatus = new GameStatus();

        public MainWindow()
        {
            InitializeComponent();
            imgControls = SetUpGameGridCanvas(gameStatus.gameGrid);
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
            ScoreText.Text = String.Format("Score : {0}", gameStatus.Score);
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
<<<<<<< HEAD
<<<<<<< Dev

            while (!gameStatus.gameOver)
            {

                await Task.Delay(gameStatus.GameSpeed);

=======
=======
>>>>>>> MUSIC-AmauryF
            SoundPlayer sp = new SoundPlayer();
            sp.SoundLocation = "C:/Users/FAILLER/Documents/Cours/B2/C#/Tetris/Tetris/Sound/Tetris 99MainTheme.wav";
            sp.PlayLooping();
            while (!gameStatus.gameOver)
            {
                //MessageBox.Show(String.Format("Current pos {0},{1}", gameStatus.CurrentTetramino.offSet.row, gameStatus.CurrentTetramino.offSet.column));
                if (gameStatus.Score%5 == 1 && gameStatus.Score != 1)
                {
                    if (gameStatus.Score/5 == speedLevel + 1)
                    {
                        speed -= 50;
                        speedLevel += 1;
                        //MessageBox.Show(String.Format("ze spid : {0}, speed2 : {1} ", speed, speedLevel));
                        if (speed < 50)
                        {
                            speed = 50;
                            //MessageBox.Show(String.Format("ze spid : {0} ", speed));
                        }
                    }
                    
                }
                await Task.Delay(speed);
>>>>>>> Background music et timer stop
                gameStatus.MoveDownTetramino();
                Draw(gameStatus);
            }
            sp.Stop();
            gameStatus.StopTimer();
            GameOverResult.Text = String.Format("Score : {0}", gameStatus.Score);
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
            MainMenu.Visibility = Visibility.Hidden;
            await Game();

        }
        private void Options(object sender, RoutedEventArgs e)
        {
            OptionPage.Visibility = Visibility.Visible;
        }
    }
}
