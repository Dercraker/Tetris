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
        public string RiverseTime { get; set; }


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

        public int reverseTimes { get; set; }
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
        public string inputSD;
        public string inputHD;
        public string inputR;
        public string inputL;
        public string inputRR;
        public string inputRL;
        public string inputP;
        public string inputH;
        public bool usedSD = false;
        public bool usedHD = false;
        public bool usedR = false;
        public bool usedL = false;
        public bool usedRR = false;
        public bool usedRL = false;
        public bool usedP = false;
        public bool usedH = false;

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
            Demo demo = new Demo();
            demo.DemoStart(demoImgControls, this);
        }
        private async void DemoGame_Loaded2(object sender, RoutedEventArgs e)
        {
            Demo demo = new Demo();
            demo.DemoStart(demoImgControls2, this);
        }
        private async void InputSoftD(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                usedSD = true;
                inputSD = "Enter";
                SoftDrops.Text = inputSD;
            }else if(e.Key == Key.LeftShift)
            {
                usedSD = true;
                inputSD = "LeftShift";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.RightShift)
            {
                usedSD = true;
                inputSD = "RightShift";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.Space)
            {
                usedSD = true;
                inputSD = "Space";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.LeftAlt)
            {
                usedSD = true;
                inputSD = "LeftAlt";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.RightAlt)
            {
                usedSD = true;
                inputSD = "RightAlt";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                usedSD = true;
                inputSD = "LeftCtrl";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.RightCtrl)
            {
                usedSD = true;
                inputSD = "RightCtrl";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.OemMinus)
            {
                usedSD = true;
                inputSD = "OemMinus";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.OemComma)
            {
                usedSD = true;
                inputSD = "OemComma";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.OemQuestion)
            {
                usedSD = true;
                inputSD = "OemQuestion";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.OemSemicolon)
            {
                usedSD = true;
                inputSD = "OemSemicolon";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.OemPeriod)
            {
                usedSD = true;
                inputSD = "OemPeriod";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.OemCloseBrackets)
            {
                usedSD = true;
                inputSD = "OemCloseBrackets";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.OemTilde)
            {
                usedSD = true;
                inputSD = "OemTilde";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.Oem8)
            {
                usedSD = true;
                inputSD = "OemTilde";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.Down)
            {
                usedSD = true;
                inputSD = "Down";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.Up)
            {
                usedSD = true;
                inputSD = "Up";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.Left)
            {
                usedSD = true;
                inputSD = "Down";
                SoftDrops.Text = inputSD;
            }
            else if (e.Key == Key.Right)
            {
                usedSD = true;
                inputSD = "Right";
                SoftDrops.Text = inputSD;
            }
            for (int i = 0; i <= 9; i++)
            {
                KeyConverter x = new KeyConverter();
                string number = i.ToString();
                number = "NumPad" + number;
                Key num = (Key)x.ConvertFromString(number);
                if (e.Key == num)
                    {
                    usedSD = true;
                    inputSD = number;
                    SoftDrops.Text = inputSD;
                }
            }           
        }
        private async void InputHardD(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                usedHD = true;
                inputHD = "Enter";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.LeftShift)
            {
                usedHD = true;
                inputHD = "LeftShift";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.RightShift)
            {
                usedHD = true;
                inputHD = "RightShift";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.Space)
            {
                usedHD = true;
                inputHD = "Space";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.LeftAlt)
            {
                usedHD = true;
                inputHD = "LeftAlt";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.RightAlt)
            {
                usedHD = true;
                inputHD = "RightAlt";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                usedHD = true;
                inputHD = "LeftCtrl";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.RightCtrl)
            {
                usedHD = true;
                inputHD = "RightCtrl";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.OemMinus)
            {
                usedHD = true;
                inputHD = "OemMinus";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.OemComma)
            {
                usedHD = true;
                inputHD = "OemComma";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.OemQuestion)
            {
                usedHD = true;
                inputHD = "OemQuestion";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.OemSemicolon)
            {
                usedHD = true;
                inputHD = "OemSemicolon";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.OemPeriod)
            {
                usedHD = true;
                inputHD = "OemPeriod";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.OemCloseBrackets)
            {
                usedHD = true;
                inputHD = "OemCloseBrackets";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.OemTilde)
            {
                usedHD = true;
                inputHD = "OemTilde";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.Oem8)
            {
                usedHD = true;
                inputHD = "OemTilde";
                HardDrops.Text = "exclamation";
            }
            else if (e.Key == Key.Down)
            {
                usedHD = true;
                inputHD = "Down";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.Up)
            {
                usedHD = true;
                inputHD = "Up";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.Left)
            {
                usedHD = true;
                inputHD = "Down";
                HardDrops.Text = inputHD;
            }
            else if (e.Key == Key.Right)
            {
                usedHD = true;
                inputHD = "Right";
                HardDrops.Text = inputHD;
            }
            for (int i = 0; i <= 9; i++)
            {
                KeyConverter x = new KeyConverter();
                string number = i.ToString();
                number = "NumPad" + number;
                Key num = (Key)x.ConvertFromString(number);
                if (e.Key == num)
                {
                    usedHD = true;
                    inputHD = number;
                    HardDrops.Text = inputHD;
                }
            }
        }
        private async void InputRight(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                usedR = true;
                inputR = "Enter";
                Right.Text = inputR;
            }
            else if (e.Key == Key.LeftShift)
            {
                usedR = true;
                inputR = "LeftShift";
                Right.Text = inputR;
            }
            else if (e.Key == Key.RightShift)
            {
                usedR = true;
                inputR = "RightShift";
                Right.Text = inputR;
            }
            else if (e.Key == Key.Space)
            {
                usedR = true;
                inputR = "Space";
                Right.Text = inputR;
            }
            else if (e.Key == Key.LeftAlt)
            {
                usedR = true;
                inputR = "LeftAlt";
                Right.Text = inputR;
            }
            else if (e.Key == Key.RightAlt)
            {
                usedR = true;
                inputR = "RightAlt";
                Right.Text = inputR;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                usedR = true;
                inputR = "LeftCtrl";
                Right.Text = inputR;
            }
            else if (e.Key == Key.RightCtrl)
            {
                usedR = true;
                inputR = "RightCtrl";
                Right.Text = inputR;
            }
            else if (e.Key == Key.OemMinus)
            {
                usedR = true;
                inputR = "OemMinus";
                Right.Text = inputR;
            }
            else if (e.Key == Key.OemComma)
            {
                usedR = true;
                inputR = "OemComma";
                Right.Text = inputR;
            }
            else if (e.Key == Key.OemQuestion)
            {
                usedR = true;
                inputR = "OemQuestion";
                Right.Text = inputR;
            }
            else if (e.Key == Key.OemSemicolon)
            {
                usedR = true;
                inputR = "OemSemicolon";
                Right.Text = inputR;
            }
            else if (e.Key == Key.OemPeriod)
            {
                usedR = true;
                inputR = "OemPeriod";
                Right.Text = inputR;
            }
            else if (e.Key == Key.OemCloseBrackets)
            {
                usedR = true;
                inputR = "OemCloseBrackets";
                Right.Text = inputR;
            }
            else if (e.Key == Key.OemTilde)
            {
                usedR = true;
                inputR = "OemTilde";
                Right.Text = inputR;
            }
            else if (e.Key == Key.Oem8)
            {
                usedR = true;
                inputR = "OemTilde";
                Right.Text = "exclamation";
            }
            else if (e.Key == Key.Down)
            {
                usedR = true;
                inputR = "Down";
                Right.Text = inputR;
            }
            else if (e.Key == Key.Up)
            {
                usedR = true;
                inputR = "Up";
                Right.Text = inputR;
            }
            else if (e.Key == Key.Left)
            {
                usedR = true;
                inputR = "Down";
                Right.Text = inputR;
            }
            else if (e.Key == Key.Right)
            {
                usedR = true;
                inputR = "Right";
                Right.Text = inputR;
            }
            for (int i = 0; i <= 9; i++)
            {
                KeyConverter x = new KeyConverter();
                string number = i.ToString();
                number = "NumPad" + number;
                Key num = (Key)x.ConvertFromString(number);
                if (e.Key == num)
                {
                    usedR = true;
                    inputR = number;
                    Right.Text = inputR;
                }
            }
        }
        private async void InputLeft(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                usedL = true;
                inputL = "Enter";
                Left.Text = inputL;
            }
            else if (e.Key == Key.LeftShift)
            {
                usedL = true;
                inputL = "LeftShift";
                Left.Text = inputL;
            }
            else if (e.Key == Key.RightShift)
            {
                usedL = true;
                inputL = "RightShift";
                Left.Text = inputL;
            }
            else if (e.Key == Key.Space)
            {
                usedL = true;
                inputL = "Space";
                Left.Text = inputL;
            }
            else if (e.Key == Key.LeftAlt)
            {
                usedL = true;
                inputL = "LeftAlt";
                Left.Text = inputL;
            }
            else if (e.Key == Key.RightAlt)
            {
                usedL = true;
                inputL = "RightAlt";
                Left.Text = inputL;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                usedL = true;
                inputL = "LeftCtrl";
                Left.Text = inputL;
            }
            else if (e.Key == Key.RightCtrl)
            {
                usedL = true;
                inputL = "RightCtrl";
                Left.Text = inputL;
            }
            else if (e.Key == Key.OemMinus)
            {
                usedL = true;
                inputL = "OemMinus";
                Left.Text = inputL;
            }
            else if (e.Key == Key.OemComma)
            {
                usedL = true;
                inputL = "OemComma";
                Left.Text = inputL;
            }
            else if (e.Key == Key.OemQuestion)
            {
                usedL = true;
                inputL = "OemQuestion";
                Left.Text = inputL;
            }
            else if (e.Key == Key.OemSemicolon)
            {
                usedL = true;
                inputL = "OemSemicolon";
                Left.Text = inputL;
            }
            else if (e.Key == Key.OemPeriod)
            {
                usedL = true;
                inputL = "OemPeriod";
                Left.Text = inputL;
            }
            else if (e.Key == Key.OemCloseBrackets)
            {
                usedL = true;
                inputL = "OemCloseBrackets";
                Left.Text = inputL;
            }
            else if (e.Key == Key.OemTilde)
            {
                usedL = true;
                inputL = "OemTilde";
                Left.Text = inputL;
            }
            else if (e.Key == Key.Oem8)
            {
                usedL = true;
                inputL = "OemTilde";
                Left.Text = "exclamation";
            }
            else if (e.Key == Key.Down)
            {
                usedL = true;
                inputL = "Down";
                Left.Text = inputL;
            }
            else if (e.Key == Key.Up)
            {
                usedL = true;
                inputL = "Up";
                Left.Text = inputL;
            }
            else if (e.Key == Key.Left)
            {
                usedL = true;
                inputL = "Down";
                Left.Text = inputL;
            }
            else if (e.Key == Key.Right)
            {
                usedL = true;
                inputL = "Right";
                Left.Text = inputL;
            }
            for (int i = 0; i <= 9; i++)
            {
                KeyConverter x = new KeyConverter();
                string number = i.ToString();
                number = "NumPad" + number;
                Key num = (Key)x.ConvertFromString(number);
                if (e.Key == num)
                {
                    usedL = true;
                    inputL = number;
                    Left.Text = inputL;
                }
            }
        }
        private async void InputRotRight(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                usedRR = true;
                inputRR = "Enter";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.LeftShift)
            {
                usedRR = true;
                inputRR = "LeftShift";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.RightShift)
            {
                usedRR = true;
                inputRR = "RightShift";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.Space)
            {
                usedRR = true;
                inputRR = "Space";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.LeftAlt)
            {
                usedRR = true;
                inputRR = "LeftAlt";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.RightAlt)
            {
                usedRR = true;
                inputRR = "RightAlt";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                usedRR = true;
                inputRR = "LeftCtrl";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.RightCtrl)
            {
                usedRR = true;
                inputRR = "RightCtrl";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.OemMinus)
            {
                usedRR = true;
                inputRR = "OemMinus";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.OemComma)
            {
                usedRR = true;
                inputRR = "OemComma";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.OemQuestion)
            {
                usedRR = true;
                inputRR = "OemQuestion";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.OemSemicolon)
            {
                usedRR = true;
                inputRR = "OemSemicolon";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.OemPeriod)
            {
                usedRR = true;
                inputRR = "OemPeriod";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.OemCloseBrackets)
            {
                usedRR = true;
                inputRR = "OemCloseBrackets";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.OemTilde)
            {
                usedRR = true;
                inputRR = "OemTilde";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.Oem8)
            {
                usedRR = true;
                inputRR = "OemTilde";
                RotateRight.Text = "exclamation";
            }
            else if (e.Key == Key.Down)
            {
                usedRR = true;
                inputRR = "Down";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.Up)
            {
                usedRR = true;
                inputRR = "Up";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.Left)
            {
                usedRR = true;
                inputRR = "Down";
                RotateRight.Text = inputRR;
            }
            else if (e.Key == Key.Right)
            {
                usedRR = true;
                inputRR = "Right";
                RotateRight.Text = inputRR;
            }
            for (int i = 0; i <= 9; i++)
            {
                KeyConverter x = new KeyConverter();
                string number = i.ToString();
                number = "NumPad" + number;
                Key num = (Key)x.ConvertFromString(number);
                if (e.Key == num)
                {
                    usedRR = true;
                    inputRR = number;
                    RotateRight.Text = inputRR;
                }
            }
        }
        private async void InputRotLeft(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                usedRL = true;
                inputRL = "Enter";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.LeftShift)
            {
                usedRL = true;
                inputRL = "LeftShift";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.RightShift)
            {
                usedRL = true;
                inputRL = "RightShift";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.Space)
            {
                usedRL = true;
                inputRL = "Space";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.LeftAlt)
            {
                usedRL = true;
                inputRL = "LeftAlt";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.RightAlt)
            {
                usedRL = true;
                inputRL = "RightAlt";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                usedRL = true;
                inputRL = "LeftCtrl";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.RightCtrl)
            {
                usedRL = true;
                inputRL = "RightCtrl";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.OemMinus)
            {
                usedRL = true;
                inputRL = "OemMinus";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.OemComma)
            {
                usedRL = true;
                inputRL = "OemComma";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.OemQuestion)
            {
                usedRL = true;
                inputRL = "OemQuestion";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.OemSemicolon)
            {
                usedRL = true;
                inputRL = "OemSemicolon";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.OemPeriod)
            {
                usedRL = true;
                inputRL = "OemPeriod";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.OemCloseBrackets)
            {
                usedRL = true;
                inputRL = "OemCloseBrackets";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.OemTilde)
            {
                usedRL = true;
                inputRL = "OemTilde";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.Oem8)
            {
                usedRL = true;
                inputRL = "OemTilde";
                RotateLeft.Text = "exclamation";
            }
            else if (e.Key == Key.Down)
            {
                usedRL = true;
                inputRL = "Down";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.Up)
            {
                usedRL = true;
                inputRL = "Up";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.Left)
            {
                usedRL = true;
                inputRL = "Down";
                RotateLeft.Text = inputRL;
            }
            else if (e.Key == Key.Right)
            {
                usedRL = true;
                inputRL = "Right";
                RotateLeft.Text = inputRL;
            }
            for (int i = 0; i <= 9; i++)
            {
                KeyConverter x = new KeyConverter();
                string number = i.ToString();
                number = "NumPad" + number;
                Key num = (Key)x.ConvertFromString(number);
                if (e.Key == num)
                {
                    usedRL = true;
                    inputRL = number;
                    RotateLeft.Text = inputRL;
                }
            }
        }
        private async void Pauses(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                usedP = true;
                inputP = "Enter";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.LeftShift)
            {
                usedP = true;
                inputP = "LeftShift";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.RightShift)
            {
                usedP = true;
                inputP = "RightShift";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.Space)
            {
                usedP = true;
                inputP = "Space";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.LeftAlt)
            {
                usedP = true;
                inputP = "LeftAlt";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.RightAlt)
            {
                usedP = true;
                inputP = "RightAlt";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                usedP = true;
                inputP = "LeftCtrl";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.RightCtrl)
            {
                usedP = true;
                inputP = "RightCtrl";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.OemMinus)
            {
                usedP = true;
                inputP = "OemMinus";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.OemComma)
            {
                usedP = true;
                inputP = "OemComma";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.OemQuestion)
            {
                usedP = true;
                inputP = "OemQuestion";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.OemSemicolon)
            {
                usedP = true;
                inputP = "OemSemicolon";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.OemPeriod)
            {
                usedP = true;
                inputP = "OemPeriod";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.OemCloseBrackets)
            {
                usedP = true;
                inputP = "OemCloseBrackets";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.OemTilde)
            {
                usedP = true;
                inputP = "OemTilde";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.Oem8)
            {
                usedP = true;
                inputP = "OemTilde";
                Pause.Text = "exclamation";
            }
            else if (e.Key == Key.Down)
            {
                usedP = true;
                inputP = "Down";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.Up)
            {
                usedP = true;
                inputP = "Up";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.Left)
            {
                usedP = true;
                inputP = "Down";
                Pause.Text = inputP;
            }
            else if (e.Key == Key.Right)
            {
                usedP = true;
                inputP = "Right";
                Pause.Text = inputP;
            }
            for (int i = 0; i <= 9; i++)
            {
                KeyConverter x = new KeyConverter();
                string number = i.ToString();
                number = "NumPad" + number;
                Key num = (Key)x.ConvertFromString(number);
                if (e.Key == num)
                {
                    usedP = true;
                    inputP = number;
                    Pause.Text = inputP;
                }
            }
        }
        private async void Holds(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                usedH = true;
                inputH = "Enter";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.LeftShift)
            {
                usedH = true;
                inputH = "LeftShift";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.RightShift)
            {
                usedH = true;
                inputH = "RightShift";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.Space)
            {
                usedH = true;
                inputH = "Space";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.LeftAlt)
            {
                usedH = true;
                inputH = "LeftAlt";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.RightAlt)
            {
                usedH = true;
                inputH = "RightAlt";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                usedH = true;
                inputH = "LeftCtrl";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.RightCtrl)
            {
                usedH = true;
                inputH = "RightCtrl";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.OemMinus)
            {
                usedH = true;
                inputH = "OemMinus";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.OemComma)
            {
                usedH = true;
                inputH = "OemComma";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.OemQuestion)
            {
                usedH = true;
                inputH = "OemQuestion";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.OemSemicolon)
            {
                usedH = true;
                inputH = "OemSemicolon";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.OemPeriod)
            {
                usedH = true;
                inputH = "OemPeriod";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.OemCloseBrackets)
            {
                usedH = true;
                inputH = "OemCloseBrackets";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.OemTilde)
            {
                usedH = true;
                inputH = "OemTilde";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.Oem8)
            {
                usedH = true;
                inputH = "OemTilde";
                Hold.Text = "exclamation";
            }
            else if (e.Key == Key.Down)
            {
                usedH = true;
                inputH = "Down";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.Up)
            {
                usedH = true;
                inputH = "Up";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.Left)
            {
                usedH = true;
                inputH = "Down";
                Hold.Text = inputH;
            }
            else if (e.Key == Key.Right)
            {
                usedH = true;
                inputH = "Right";
                Hold.Text = inputH;
            }
            for (int i = 0; i <= 9; i++)
            {
                KeyConverter x = new KeyConverter();
                string number = i.ToString();
                number = "NumPad" + number;
                Key num = (Key)x.ConvertFromString(number);
                if (e.Key == num)
                {
                    usedH = true;
                    inputH = number;
                    Hold.Text = inputH;
                }
            }
        }
        private async void InputTabSD(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                usedSD = true;
                inputSD = "Tab";
                SoftDrops.Text = inputSD;
            }
        }
        private async void InputTabHD(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                usedHD = true;
                inputHD = "Tab";
                HardDrops.Text = inputHD;
            }
        }
        private async void InputTabRight(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                usedR = true;
                inputR = "Tab";
                Right.Text = inputR;
            }
        }
        private async void InputTabLeft(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                usedL = true;
                inputL = "Tab";
                Left.Text = inputL;
            }
        }
        private async void InputTabRRight(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                usedRR = true;
                inputRR = "Tab";
                RotateRight.Text = inputRR;
            }
        }
        private async void InputTabRLeft(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                usedRL = true;
                inputRL = "Tab";
                RotateLeft.Text = inputRL;
            }
        }
        private async void InputTabPause(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                usedP = true;
                inputP = "Tab";
                Pause.Text = inputP;
            }
        }
        private async void InputTabHold(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                usedH = true;
                inputH = "Tab";
                Hold.Text = inputH;
            }
        }
        private void OutOption(object sender, RoutedEventArgs e)
        {
            OptionPage.Visibility = Visibility.Hidden;
            int.TryParse(ReverseTime.Text, out reverseTime);
            if (reverseTime == 0) reverseTimes = 60;
            else reverseTimes = reverseTime;
            int.TryParse(DemoSpeeds.Text, out demoSpeed);
            int.TryParse(MinSpeed.Text, out minSpeed);
            int.TryParse(MaxSpeed.Text, out maxSpeed);
            int.TryParse(ClearBonus.Text, out clearBonus);
            
            StringToKey();
        }
        private void StringToKey()
        {
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
            KeyConverter x = new KeyConverter();
            if (usedSD)
            {
                sD = (Key)x.ConvertFromString(inputSD);
                usedSD = false;
            }
            else
            {
                sD = (Key)x.ConvertFromString(softDrop);
            }

            if (usedHD)
            {
                hD = (Key)x.ConvertFromString(inputHD);
                usedHD = false;
            }
            else
            {
                hD = (Key)x.ConvertFromString(hardDrop);
            }

            if (usedR)
            {
                rights = (Key)x.ConvertFromString(inputR);
                usedR = false;
            }
            else
            {
                rights = (Key)x.ConvertFromString(right);
            }

            if (usedL)
            {
                lefts = (Key)x.ConvertFromString(inputL);
                usedL = false;
            }
            else
            {
                lefts = (Key)x.ConvertFromString(left);
            }

            if (usedRR)
            {
                rR = (Key)x.ConvertFromString(inputRR);
                usedRR = false;
            }
            else
            {
                rR = (Key)x.ConvertFromString(rotateRight);
            }

            if (usedRL)
            {
                rL = (Key)x.ConvertFromString(inputRL);
                usedRL = false;
            }
            else
            {
                rL = (Key)x.ConvertFromString(rotateLeft);
            }

            if (usedP)
            {
                pose = (Key)x.ConvertFromString(inputP);
                usedRR = false;
            }
            else
            {
                pose = (Key)x.ConvertFromString(pause);
            }

            if (usedH)
            {
                holds = (Key)x.ConvertFromString(inputH);
                usedH = false;
            }
            else
            {
                holds = (Key)x.ConvertFromString(hold);
            }
        }

        private void Deletesave(object sender, RoutedEventArgs e)
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

                DeleteGamesList.Items.Add(item);
            }

            DeleteSave.Visibility = Visibility.Collapsed;
            DeleteGamesList.Visibility = Visibility.Visible;
        }
        private void DeleteGame_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string itemIndex = sender.ToString().Trim().Split(":")[1];
            if (itemIndex != "1" && DeleteGamesList.SelectedValue.ToString().Split(": ")[1] != "Select SaveGame")
            {
                string filePath = ((ComboBoxItem)DeleteGamesList.SelectedItem).Tag.ToString();


                DeleteSave.Visibility = Visibility.Visible;
                DeleteGamesList.Visibility = Visibility.Collapsed;

                DeleteGamesList.Items.RemoveAt(1);
                DeleteGamesList.SelectedIndex = 0;

                File.Delete(filePath);
            }
        }
        private void DeleteAllSave_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("./SaveGames"))
            {
                string[] allfiles = Directory.GetFiles("./SaveGames", "*.*", SearchOption.AllDirectories);
                foreach(string file in allfiles)
                {
                    File.Delete(file);
                }
            } 
        }
        private async void KeyInput(object sender, KeyEventArgs e)
        {
            StringToKey();
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
            else if (e.Key == holds) gameStatus.HoldTetramino();
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
            DeleteAllSave.IsEnabled = false;

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
    }
}
