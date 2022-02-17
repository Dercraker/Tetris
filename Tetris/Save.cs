using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Save
    {
        //Initialisation des variable 
        public Tetramino currentTetramino { get; set; }
        public Tetramino holdingTetramino { get; set; }
        public GameGrid gameGrid { get; set; }
        public WaitingLine waitingLine { get; set; }
        public int GameSpeed { get; set; }
        public string GameMode { get; set; }
        public int SpeedLevel { get; set; }
        public Scores scores { get; set; }



        //Permet de charger une instance de save
        public GameStatus LoadSave()
        {
            return new GameStatus()
            {
                waitingLine = this.waitingLine,
                CurrentTetramino = this.currentTetramino.NewTetramino(waitingLine),
                HoldingTetramino = this.holdingTetramino.NewTetramino(waitingLine),
                gameGrid = this.gameGrid,
                GameSpeed = this.GameSpeed,
                GameMode = this.GameMode,
                SpeedLevel = this.SpeedLevel,
                scores = this.scores,

                Pause = true
            };
        }
    }



    //Permet de crée des methode statique
    public static class SaveFactory
    {



        //Get tous les params requis et crée une instance de save
        public static Save CreateSave(this GameStatus gm)
        {
            return new Save
            {
                waitingLine = gm.waitingLine,
                currentTetramino = gm.CurrentTetramino,
                holdingTetramino = gm.HoldingTetramino,
                gameGrid = gm.gameGrid,
                GameSpeed = gm.GameSpeed,
                GameMode = gm.GameMode,
                SpeedLevel = gm.SpeedLevel,
                scores = gm.scores,
            };
        }



        //Permet d'obtenir un nouveau tetramino grace a un id
        public static Tetramino NewTetramino(this Tetramino t, WaitingLine w) => t.tetraminoId switch
        {
            1 => w.GetTetramino(t.tetraminoId - 1),
            2 => w.GetTetramino(t.tetraminoId - 1),
            3 => w.GetTetramino(t.tetraminoId - 1),
            4 => w.GetTetramino(t.tetraminoId - 1),
            5 => w.GetTetramino(t.tetraminoId - 1),
            6 => w.GetTetramino(t.tetraminoId - 1),
            7 => w.GetTetramino(t.tetraminoId - 1),
            _ => throw new ArgumentException()
        };
    }
}
