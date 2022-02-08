using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Scores
    {
        public int time { get; set; }
        public int lastScoreTime { get; set; }
        public int combos { get; set; }
        public int score { get; set; }
        public int bonusClear { get; set; }

        public Scores()
        {
            score = 0;
            lastScoreTime = 0;
            combos = 1;
        }

        public  int bonusScore(int line)
        {
            int result = 0;
            result = multipleLineBonus(line);
            result = result == 0 ? 0 : combosTimeBonus(result);
            return result;
        }
        public  int multipleLineBonus(int line)
        {
            int result = 0;
            for (int i = 0; i <= line; i++)
            {
                result += i;
            }
            return result;
        }
        public int combosTimeBonus(int result)
        {
            int currentTime = time;
            int diff = currentTime - lastScoreTime;
            if (diff <= 15 && time > 15)
            {
                if (combos < 10)
                {
                    combos++;
                }
            }
            else
            {
                combos = 1;
            }
            lastScoreTime = currentTime;
            return result * combos;
        }
        public void GameScoreBonus(int addResult, GameGird g)
        {
            bonusClear += addResult;
            if(bonusClear >= 75)
            {
                for (int r = 0; r < g.rows; r++)
                {
                    g.CleanRow(r);
                }
                bonusClear = 0;
            }
        }
    }
}
