using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tetris.tetraminoList;

namespace Tetris
{
    public class WaitingLine
    {
        //Initialisation des différent tetramino && des variables
        private Tetramino[] tetraminosTab = new Tetramino[]
        {
            new ITetramino(),
            new JTetramino(),
            new LTetramino(),
            new OTetramino(),
            new STetramino(),
            new TTetramino(),
            new ZTetramino(),
        };
        private Random random = new Random();
        public Tetramino NextTetramino { get; set; }


        
        //Avoir une copie d'un Tetramino random dans la liste de tetraminos disponible
        private Tetramino RandomTetramino()
        {
            Tetramino newTetramino = tetraminosTab[random.Next(tetraminosTab.Length)];
            Tetramino newInstance = newTetramino.DeepCopy();
            newInstance.reset();
            return newInstance;
        }



        //Avoir un tetramino en fonction de son id
        public Tetramino GetTetramino(int id)
        {
            Tetramino newTetramino = tetraminosTab[id];
            Tetramino newInstance = newTetramino.DeepCopy();
            newInstance.reset();
            return newInstance;
        }



        //Permet d'obtenir de définir le prochain tetramino
        public WaitingLine(string NextTeraminoId = null)
        {
            if (NextTeraminoId == null)
            {
                NextTetramino = RandomTetramino();
            } else
            {
                NextTetramino = GetTetramino(int.Parse(NextTeraminoId));
            }
            
        }



        //Update le prochain tetramino au current et ajou un rndom tetramino au next
        public Tetramino UpdateTetramino()
        {
            Tetramino tetramino = NextTetramino;

            do
            {
                NextTetramino = RandomTetramino();
            }
            while(tetramino.tetraminoId == NextTetramino.tetraminoId);
            return tetramino;
        }
    }
}
