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

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ImageSource[] tetraminoImages = new ImageSource[]
        {
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/tetramino/cleanTetramino.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/tetramino/IDiamound.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/tetramino/JLapilazuli.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/tetramino/LRedStone.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/tetramino/OGold.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/tetramino/SCharbon.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/tetramino/TEmerald.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/tetramino/ZIron.png"))
        };

        public ImageSource[] boxImages = new ImageSource[]
        {
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/fullbox/CleanBox.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/fullbox/diamond_block.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/fullbox/lapis_block.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/fullbox/redstone_block.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/fullbox/gold_block.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/fullbox/coal_block.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/fullbox/emerald_block.png")),
            new BitmapImage(new Uri("C:/Users/Dercraker/OneDrive - Ynov/Cours/Tetris/Tetris/Tetris/assets/fullbox/iron_block.png"))
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
            GetNextBlock(gameStatus.waitingLine);
        }
        private async void GameGridCanvas_Load(object sender, RoutedEventArgs e)
        {
            await Game();
            
            //Draw(gameStatus);
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
            while (!gameStatus.gameOver)
            {
                //MessageBox.Show(String.Format("Current pos {0},{1}", gameStatus.CurrentTetramino.offSet.row, gameStatus.CurrentTetramino.offSet.column));
                await Task.Delay(500);
                gameStatus.MoveDownTetramino();
                Draw(gameStatus);
            }
            GameOverResult.Text = String.Format("Score : {0}", gameStatus.Score);
            MenuGameOver.Visibility = Visibility.Visible;

        }
        private async void RestartGame(object sender, RoutedEventArgs e)
        {
            gameStatus = new GameStatus();
            MenuGameOver.Visibility= Visibility.Hidden;
            await Game();
        }
    }
}
