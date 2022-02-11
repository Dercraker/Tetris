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
        public static MainWindow mainWindow { get; }
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
        public Image[,] demoImgControls;
        public Image[,] demoImgControls2;
        public GameStatus gameStatus = new GameStatus(22,10);
        public SoundPlayer SoundMenu = new SoundPlayer(Resource1.MainMenuSound);
        public SoundPlayer TetrisGM_Sound = new SoundPlayer(Resource1.Tetris_99_Main_Theme);

        public MainWindow()
        {
            InitializeComponent();
            imgControls = SetUpGameGridCanvas(gameStatus.gameGrid, GameGridCanvas);
            demoImgControls = SetUpGameGridCanvas(gameStatus.gameGrid, DemoGame);
            demoImgControls2 = SetUpGameGridCanvas(gameStatus.gameGrid, DemoGame2);
            SoundMenu.PlayLooping();
        }

        public Image[,] SetUpGameGridCanvas(GameGird g, Canvas canvas)
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
                    canvas.Children.Add(imgControl);
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
                    imgControls[r, c].Opacity = 1;
                    imgControls[r, c].Source = boxImages[boxId];
                }
            }
        }
        public void DrawHardBlockPosition(Tetramino block)
        {
            int dropDistance = gameStatus.HardDropTetramino();

            foreach(Position p in block.positionsOfRotation())
            {
                imgControls[p.row + dropDistance, p.column].Opacity = 0.33;
                imgControls[p.row + dropDistance, p.column].Source = boxImages[block.tetraminoId];
            }
        }
        public void DrawBox(Tetramino t)
        {
            foreach (Position p in t.positionsOfRotation())
            {
                imgControls[p.row, p.column].Opacity = 1;
                imgControls[p.row, p.column].Source = boxImages[t.tetraminoId];
            }
        }
        public void Draw(GameStatus g)
        {
            DrawGrid(g.gameGrid);
            DrawHardBlockPosition(g.CurrentTetramino);
            DrawBox(g.CurrentTetramino);
            ScoreText.Text = String.Format("Score : {0}", gameStatus.scores.score);
            if (gameStatus.scores.combos > 1)
            {
                CombosText.Visibility = Visibility.Visible;
                CombosText.Text = String.Format("Combos : {0}", gameStatus.scores.combos);
            }
            else
            {
                CombosText.Visibility = Visibility.Hidden;
            }
            TimerCount.Text = String.Format("Timer : {0}'{1}''", gameStatus.scores.time / 60, gameStatus.scores.time % 60);
            GetNextBlock(gameStatus.waitingLine);
        }
        private async void DemoGame_Loaded(object sender, RoutedEventArgs e)
        {
            DemoStart(demoImgControls);
        }
        private async void DemoGame_Loaded2(object sender, RoutedEventArgs e)
        {
            DemoStart(demoImgControls2);
        }

        public Key hD;
        public Key sD;
        public Key rights;
        public Key lefts;
        public Key rR;
        public Key rL;
        public Key pose;
        public Key holds;
        public string hardDrop;
        public int reverseTime;
        public int demoSpeed;
        public int minSpeed;
        public int maxSpeed;
        public int clearBonus;
        public string softDrop;
        public string right;
        public string left;
        public string rotateRight;
        public string rotateLeft;
        public string pause;
        public string hold;

        private void OutOption(object sender, RoutedEventArgs e)
        {
            OptionPage.Visibility = Visibility.Hidden;
            int.TryParse(ReverseTime.Text, out reverseTime);
            int.TryParse(DemoSpeeds.Text, out demoSpeed);
            int.TryParse(MinSpeed.Text, out minSpeed);
            int.TryParse(MaxSpeed.Text, out maxSpeed);
            int.TryParse(ClearBonus.Text, out clearBonus);
            hardDrop = HardDrops.Text;
            softDrop = SoftDrops.Text;
            right = Right.Text;
            left = Left.Text;
            rotateRight = RotateRight.Text;
            rotateLeft = RotateLeft.Text;
            pause = Pause.Text;
            hold = Hold.Text;
            if (softDrop != "") softDrop = softDrop.ToUpper();
            else softDrop = "Down";
            if (hardDrop != "") hardDrop = hardDrop.ToUpper();
            else hardDrop = "Space";
            if (right != "") right = right.ToUpper();
            else right = "Right";
            if (left != "") left = left.ToUpper();
            else left = "Left";
            if (rotateRight != "") rotateRight = rotateRight.ToUpper();
            else rotateRight = "D";
            if (rotateLeft != "") rotateLeft = rotateLeft.ToUpper();
            else rotateLeft = "Q";
            if (pause != "") pause = pause.ToUpper();
            else pause = "Z";
            if (hold != "") hold = hold.ToUpper();
            else hold = "Shift";
            StringToKey();
        }



        private void StringToKey()
        {
            KeyConverter x = new KeyConverter();
            sD = (Key)x.ConvertFromString(softDrop);
            hD = (Key)x.ConvertFromString(hardDrop);
            rights = (Key)x.ConvertFromString(right);
            lefts = (Key)x.ConvertFromString(left);
            rR = (Key)x.ConvertFromString(rotateRight);
            rL = (Key)x.ConvertFromString(rotateLeft);
            pose = (Key)x.ConvertFromString(pause);
            holds = (Key)x.ConvertFromString(hold);
        }

        private async void KeyInput(object sender, KeyEventArgs e)
        {
            if (gameStatus.gameOver)
            {
                return;
            }
            if (e.Key == rights) gameStatus.MoveRightTetramino();
            else if (e.Key == lefts) gameStatus.MoveLeftTetramino();
            else if (e.Key == sD) gameStatus.MoveDownTetramino();
            else if (e.Key == rR) gameStatus.RotatePrevTetramino();
            else if (e.Key == rL) gameStatus.RotateNextTetramino();
            else if (e.Key == hD) gameStatus.HardDrop();
            else if (e.Key == pose) gameStatus.Pause = gameStatus.Pause ? false : true;
            Draw(gameStatus);
        }
        private async void RestartGame(object sender, RoutedEventArgs e)
        {
            
            MenuGameOver.Visibility = Visibility.Hidden;

            switch (gameStatus.GameMode)
            {
                case "Tetris":
                    {
                        gameStatus = new GameStatus(22,10);
                        await GameRun();
                        break;
                    }
                case "Reverse-Tetris":
                    {
                        gameStatus = new GameStatus(22, 10);
                        await GameRun2();
                        break;
                    }

            }
        }
        private void ReturnMainMenu(object sender, RoutedEventArgs e)
        {
            gameStatus = new GameStatus(22, 10);
            MainMenu.Visibility = Visibility.Visible;

            SoundMenu.Stream.Position = 0;
            SoundMenu.PlayLooping();

            MenuGameOver.Visibility = Visibility.Hidden;
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
                        await GameRun2();
                        break;
                    }

            }
        }
        private void Options(object sender, RoutedEventArgs e)
        {
            OptionPage.Visibility = Visibility.Visible;
        }
        private void ResumeGame_Click(object sender, RoutedEventArgs e)
        {
            gameStatus.Pause = false;
        }

        /////////////////
        /// TETRIS GM ///
        /////////////////

        public async Task GameRun()
        {
            gameStatus = new GameStatus(22, 10);
            gameStatus.GameMode = "Tetris";


            Draw(gameStatus);
            gameStatus.SetTimer();
            TetrisGM_Sound.PlayLooping();

            while (!gameStatus.gameOver)
            {
                await Task.Delay(gameStatus.GameSpeed);
                gameStatus.MoveDownTetramino();
                Draw(gameStatus);
                if (gameStatus.Pause) await gameStatus.GamePause(PausePage,MainTime,TotalTime,CurrentScore, BreakLine, BestCombos);
            }

            TetrisGM_Sound.Stop();
            gameStatus.StopTimer();
            GameOverResult.Text = String.Format("Score : {0}", gameStatus.scores.score);
            GameOverTimer.Text = String.Format("Time : {0}'{1}''", gameStatus.scores.time / 60, gameStatus.scores.time % 60);
            MenuGameOver.Visibility = Visibility.Visible;
        }

        /////////////////////////
        /// REVERSE-TETRIS GM ///
        /////////////////////////

        public async Task GameRun2()
        {
            gameStatus = new GameStatus(22, 10);
            gameStatus.GameMode = "Reverse-Tetris";
            gameStatus.scores.time = 60;


            Draw(gameStatus);
            gameStatus.SetReverseTimer();
            gameStatus.SetTotalTimer();
            TetrisGM_Sound.PlayLooping();

            while (!gameStatus.gameOver)
            {
                if (gameStatus.scores.time <= 0)
                {
                    gameStatus.scores.time = 0;
                    gameStatus.gameOver = true;
                    break;
                }
                await Task.Delay(gameStatus.GameSpeed);
                gameStatus.MoveDownTetramino();
                Draw(gameStatus);
                if (gameStatus.Pause) await gameStatus.GamePause(PausePage,MainTime,TotalTime,CurrentScore, BreakLine, BestCombos);
            }

            TetrisGM_Sound.Stop();
            gameStatus.StopTimer();
            gameStatus.StopTotalTimer();
            GameOverResult.Text = String.Format("Score : {0}", gameStatus.scores.score);
            GameOverTimer.Text = String.Format("Time : {0}'{1}''", gameStatus.scores.time / 60, gameStatus.scores.time % 60);
            MenuGameOver.Visibility = Visibility.Visible;
        }



        ////////////
        /// DEMO ///
        ////////////

        public async void DemoStart(Image[,] imgctrl)
        {
            GameStatus g = new GameStatus(22, 10);
            g.GameMode = "Tetris";

            DemoDrawGrid(g.gameGrid,imgctrl);
            DemoDrawBox(g.CurrentTetramino, imgctrl);

            Tetramino nextTetramino = g.waitingLine.NextTetramino;
            NextImage.Source = tetraminoImages[nextTetramino.tetraminoId];

            while (!g.gameOver)
            {
                await Task.Delay(120);
                g.RandomMove();
                g.MoveDownTetramino();
                DemoDrawGrid(g.gameGrid,imgctrl);
                DemoDrawBox(g.CurrentTetramino, imgctrl);
            }
            DemoStart(imgctrl);
        }
        public void DemoDrawGrid(GameGird g, Image[,] imgctrl)
        {
            for (int r = 0; r < g.rows; r++)
            {
                for (int c = 0; c < g.colums; c++)
                {
                    int boxId = g[r, c];
                    imgctrl[r,c].Opacity = 1;
                    imgctrl[r,c].Source = boxImages[boxId];
                }
            }
        }
        public void DemoDrawBox(Tetramino t, Image[,] imgctrl)
        {
            foreach (Position p in t.positionsOfRotation())
            {
                imgctrl[p.row, p.column].Source = boxImages[t.tetraminoId];
            }
        }
    }
}
