using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Model
{
    public class ComputerPlayer : Player
    {
        private int threshold;
        private Random random;

        public ComputerPlayer()
        {
            random = new Random();
            int sum = 0;
            int count = 0;
            foreach ((int, int[]) valueTuple in Game.CardDistribution)
            {
                foreach (int i in valueTuple.Item2)
                {
                    sum += valueTuple.Item1 * i;
                    count += 1;
                }
            }

            threshold = sum / count;
        }

        private void DecideCoverdExposed()
        {
            if (Game.ExposedCard.Value > threshold)
            {
                DrawCovered();
            }
            else if (Game.ExposedCard.Value < threshold)
            {
                DrawExposed();
            }
            else
            {
                if (random.Next() % 2 == 0) DrawCovered();
                else DrawExposed();
            }
        }

        private void DecideExposeReplace()
        {
            //TODO
        }
    }
}
