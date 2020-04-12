using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;

namespace GameLogic.Model
{
    public class ComputerPlayer : Player
    {
        private static int comCount = 0;
        private int threshold;
        private Random random;

        public ComputerPlayer()
        {
            Id = "COM" + comCount++;
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

        public void AutoFirstTurnAction()
        {
            (byte, byte) coor1 = ((byte)random.Next(2), (byte)random.Next(3));
            (byte, byte) coor2 = ((byte)random.Next(2), (byte)random.Next(3));
            FirstTurnAction(coor1, coor2);
        }

        public void AutoPlayRound()
        {
            DecideCoveredExposed();
            DecideExposeReplace();
        }

        private void DecideCoveredExposed()
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
            if (TemporaryCard.Value > threshold)
            {
                DecisionExpose();
            }
            else if (TemporaryCard.Value < threshold)
            {
                DecisionReplace();
            }
            else
            {
                if (random.Next() % 2 == 0) DecisionExpose();
                else DecisionReplace();
            }
        }

        private void DecisionExpose()
        {
            List<(PlayingCard, (byte, byte))> covered = new List<(PlayingCard, (byte, byte))>();
            for (byte i = 0; i < CurrentCardSet.Cards.GetLength(0); i++)
            {
                for (byte j = 0; j < CurrentCardSet.Cards.GetLength(1); j++)
                {
                    if (!CurrentCardSet.Cards[i, j].Exposed)
                        covered.Add((CurrentCardSet.Cards[i, j], (i, j)));
                }
            }
            int expose_index = random.Next(covered.Count);
            CardAction(covered[expose_index].Item2, false);
        }

        private void DecisionReplace()
        {
            (byte, byte) largest_coor = (0, 0);
            for (byte i = 0; i < CurrentCardSet.Cards.GetLength(0); i++)
            {
                for (byte j = 0; j < CurrentCardSet.Cards.GetLength(1); j++)
                {
                    if (CurrentCardSet.Cards[i, j].Value > CurrentCardSet.Cards[largest_coor.Item1, largest_coor.Item2].Value) largest_coor = (i, j);
                }
            }

            if (CurrentCardSet.Cards[largest_coor.Item1, largest_coor.Item2].Value < TemporaryCard.Value) DecisionExpose();
            else CardAction(largest_coor, true);

        }
    }
}
