using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Save
    {
        public Tetramino currentTetramino { get; set; }
        public Tetramino holdingTetramino { get; set; }
        public GameGrid gameGrid { get; set; }
        public WaitingLine waitingLine { get; }
        public int GameSpeed { get; set; }
        public string GameMode { get; set; }
        public int SpeedLevel { get; set; }
        public Scores scores { get; set; }

        public GameStatus LoadSave()
        {
            return new GameStatus()
            {
                gameGrid = this.gameGrid,
                GameSpeed = this.GameSpeed,
                GameMode = this.GameMode,
                SpeedLevel = this.SpeedLevel,
                scores = this.scores,

                Pause = true
            };
        }
    }

    public static class SaveFactory
    {
        public static Save CreateSave(this GameStatus gm)
        {
            return new Save
            {
                gameGrid = gm.gameGrid,
                GameSpeed = gm.GameSpeed,
                GameMode = gm.GameMode,
                SpeedLevel = gm.SpeedLevel,
                scores = gm.scores,
            };
        }
    }
}
