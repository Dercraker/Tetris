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
    public class Demo
    {
        private ImageSource[] tetraminoImages { get; set; }
        private Image[,] demoImgControls { get; set; }
        private MainWindow mainWindow { get; set; }
        private Image NextImage { get; set; }
        private ImageSource[] boxImages { get; set; }
        public Demo(MainWindow main, ImageSource[] imageSources, ImageSource[] imageSources1, Image image, Image[,] imgctrl)
        {
            mainWindow = main;
            tetraminoImages = imageSources;
            boxImages = imageSources1;
            NextImage = image;
            demoImgControls = imgctrl;
        }
        public async void Start()
        {
            GameStatus g = new GameStatus();
            g.GameMode = "Tetris";
            g.scores.time = 0;

            DemoDrawGrid(g.gameGrid);
            DemoDrawBox(g.CurrentTetramino);

            Tetramino nextTetramino = g.waitingLine.NextTetramino;
            NextImage.Source = tetraminoImages[nextTetramino.tetraminoId];

            while (!g.gameOver)
            {
                await Task.Delay(200);
                g.MoveDownTetramino();
                DemoDrawGrid(g.gameGrid);
                DemoDrawBox(g.CurrentTetramino);
            }
            Start();
        }
        public void DemoDrawGrid(GameGird g)
        {
            for (int r = 0; r < g.rows; r++)
            {
                for (int c = 0; c < g.colums; c++)
                {
                    int boxId = g[r, c];
                    demoImgControls[r, c].Opacity = 1;
                    demoImgControls[r, c].Source = boxImages[boxId];
                }
            }
        }
        public void DemoDrawBox(Tetramino t)
        {
            foreach (Position p in t.positionsOfRotation())
            {
                demoImgControls[p.row, p.column].Source = boxImages[t.tetraminoId];
            }
        }

    }
}
