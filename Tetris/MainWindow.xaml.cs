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
        public gameMode.GameMode game { get; set; }
        public GameStatus gameStatus = new GameStatus();
        public SoundPlayer SoundMenu = new SoundPlayer(Resource1.MainMenuSound);

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

        public void DisplayHoldedTetra()
        {
           Tetramino holdedTetramino = gameStatus.HoldingTetramino;
            if (holdedTetramino == null)
            {
                HoldedImage.Source = boxImages[0];
            }
            else
            {
                HoldedImage.Source = tetraminoImages[holdedTetramino.tetraminoId];
            }
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
            DisplayHoldedTetra();
        }
        private async void DemoGame_Loaded(object sender, RoutedEventArgs e)
        {
            DemoStart(demoImgControls);
        }
        private async void DemoGame_Loaded2(object sender, RoutedEventArgs e)
        {
            DemoStart(demoImgControls2);
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
                case Key.LeftShift:
                    gameStatus.HoldTetramino();
                    break;
                case Key.Space:
                    gameStatus.HardDrop();
                    break;
                case Key.Z:
                    gameStatus.Pause = gameStatus.Pause ? false: true;
                    break;
                default:
                    return;
            }
            Draw(gameStatus);
        }
        private async void RestartGame(object sender, RoutedEventArgs e)
        {
            
            MenuGameOver.Visibility = Visibility.Hidden;

            switch (gameStatus.GameMode)
            {
                case "Tetris":
                    {
                        game = new gameMode.Tetris(this);
                        game.init();
                        gameStatus = game.gameStatus;
                        await game.Run();
                        break;
                    }
                case "Reverse-Tetris":
                    {
                        game = new gameMode.RevrseTetris(this);
                        game.init();
                        gameStatus = game.gameStatus;
                        await game.Run();
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
                        gameMode.GameMode game = new gameMode.Tetris(this);
                        game.init();
                        gameStatus = game.gameStatus;
                        await game.Run();
                        break;
                    }
                case "Reverse-Tetris":
                    {
                        gameMode.GameMode game = new gameMode.RevrseTetris(this);
                        game.init();
                        gameStatus = game.gameStatus;
                        await game.Run();
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
        private async void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists("./SaveGames")) Directory.CreateDirectory("./SaveGames");

            Save save = new Save()
            {
                Status = gameStatus
            };

            string fileName = String.Format("./SaveGames/{0}.json",gameStatus.GameMode + "_" + DateTime.Now.ToString("dd-MM-yy_H-mm-ss"));
            using FileStream createStream = File.Create(fileName);

            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };

            await JsonSerializer.SerializeAsync(createStream, save, options);
            await createStream.DisposeAsync();

            
            ReturnMainMenu(sender,e);
        }
        private void GameLoading(object sender, RoutedEventArgs e)
        {
            string[] allfiles = Directory.GetFiles("./SaveGames", "*.*", SearchOption.AllDirectories);
            foreach (string file in allfiles)
            {
                string fileName = file.Split(".json")[0].Substring(12);
                string gmName = fileName.Split("_")[0];
                string[] date = fileName.Split("_")[1].Split("-");
                string[] heur = fileName.Split("_")[2].Split("-");
                fileName = String.Format("{0} : {1},{2},{3} {4}h{5}m{6}s", gmName, date[0], date[1], date[2], heur[0], heur[1], heur[2]);

                ComboBoxItem item = new ComboBoxItem
                {
                    Tag = file.ToString(),
                    Content = fileName
                };

                SaveGamesList.Items.Add(item);
            }
            
            GameLoad.Visibility = Visibility.Collapsed;
            SaveGamesList.Visibility = Visibility.Visible;
        }
        private async void SaveGamesList_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string itemIndex = sender.ToString().Trim().Split(":")[1];
            if (itemIndex != "1" && SaveGamesList.SelectedValue.ToString().Split(": ")[1] != "Select SaveGame")
            {
                string filePath = ((ComboBoxItem)SaveGamesList.SelectedItem).Tag.ToString();
                using FileStream openStream = File.OpenRead(filePath);
                Save save = await JsonSerializer.DeserializeAsync<Save>(openStream);

                GameLoad.Visibility = Visibility.Visible;
                SaveGamesList.Visibility = Visibility.Collapsed;

                SoundMenu.Stop();

                PausePage.Visibility = Visibility.Visible;
                MainMenu.Visibility = Visibility.Hidden;

                SaveGamesList.Items.RemoveAt(1);
                SaveGamesList.SelectedIndex= 0;

                GameStatus gm = save.Status;
                switch (gm.GameMode)
                {
                    case "Tetris":
                        {
                            gameMode.GameMode game = new gameMode.Tetris(this);
                            game.init(gm);
                            await game.Run();
                            break;
                        }
                    case "Reverse-Tetris":
                        {
                            gameMode.GameMode game = new gameMode.RevrseTetris(this);
                            game.init(gm);
                            await game.Run();
                            break;
                        }
                }

            }
        }

        ////////////
        /// DEMO ///
        ////////////

        public async void DemoStart(Image[,] imgctrl)
        {
            GameStatus g = new GameStatus();
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
