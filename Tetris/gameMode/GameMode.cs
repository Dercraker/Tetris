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
    public abstract class GameMode
    {
        public GameStatus gameStatus { get; set; }
        public MainWindow mw { get; set; }
        public SoundPlayer TetrisGM_Sound = new SoundPlayer(Resource1.Tetris_99_Main_Theme);


        public GameMode(MainWindow mainWindow)
        {
            mw = mainWindow;
        }
        public abstract void init(GameStatus gm = null);
        public abstract Task Run();
        public abstract void Stop();
    }
}
