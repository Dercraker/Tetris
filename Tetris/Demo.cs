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
        //Initialisation des varaibles
        private ImageSource[] tetraminoImages { get; set; }
        private Image NextImage { get; set; }
        private ImageSource[] boxImages { get; set; }



        //Initialisation et lancement d'une demo
        public async void DemoStart(Image[,] imgctrl, MainWindow mw)
        {
            GameStatus g = new GameStatus();
            tetraminoImages = mw.tetraminoImages;
            boxImages = mw.boxImages;
            g.GameMode = "Tetris";

            DemoDrawGrid(g.gameGrid, imgctrl);
            DemoDrawBox(g.CurrentTetramino, imgctrl);

            Tetramino nextTetramino = g.waitingLine.NextTetramino;

            while (!g.gameOver)
            {
                await Task.Delay(120);
                g.RandomMove();
                g.MoveDownTetramino();
                DemoDrawGrid(g.gameGrid, imgctrl);
                DemoDrawBox(g.CurrentTetramino, imgctrl);
            }
            DemoStart(imgctrl, mw);
        }



        //Display des demo sur le canvas 
        public void DemoDrawGrid(GameGrid g, Image[,] imgctrl)
        {
            for (int r = 0; r < g.rows; r++)
            {
                for (int c = 0; c < g.colums; c++)
                {
                    int boxId = g[r, c];
                    imgctrl[r, c].Opacity = 1;
                    imgctrl[r, c].Source = boxImages[boxId];
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
