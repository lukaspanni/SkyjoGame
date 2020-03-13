using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GameLogic.Model
{
    public class PlayerCardSet
    {
        private PlayingCard[,] _cards;

        public PlayingCard[,] Cards { get => _cards; private set { _cards = value; RefreshSet(); } }
        public int ExposedValueSum { get; private set; }

        public PlayerCardSet(PlayingCard[,] cards)
        {
            foreach (PlayingCard card in cards)
            {
                card.Exposed = false;
            }
            Cards = cards;
        }

        /// <summary>
        /// Refresh Current Card Set and Calculate Sum
        /// </summary>
        public void RefreshSet()
        {
            int sum = 0;
            int exposedCount = 0;
            for (int i = 0; i < Cards.GetLength(0); i++)
            {
                for (int j = 0; j < Cards.GetLength(1); j++)
                {
                    if (i == 0)
                    {
                        if (Cards[i, j] != null)
                        {
                            if (Cards[i, j].Exposed && Cards[i + 1, j].Exposed && Cards[i + 2, j].Exposed)
                            {
                                if (Cards[i, j].Value == Cards[i + 1, j].Value && Cards[i, j].Value == Cards[i + 2, j].Value)
                                {
                                    Cards[i, j] = null;
                                    Cards[i + 1, j] = null;
                                    Cards[i + 2, j] = null;
                                }
                            }
                        }
                    }
                    if (Cards[i, j] != null && Cards[i, j].Exposed)
                    {
                        exposedCount += 1;
                        sum += Cards[i, j].Value;
                    }
                }
            }
            ExposedValueSum = sum;
            if (exposedCount == Cards.Length)
            {
                throw new RoundFinishedException();
            }
        }

        private bool CheckDimensions(int x, int y)
        {
            if (x <= Cards.GetLength(0) && y <= Cards.GetLength(1))
            {
                return true;
            }
            return false;
        }

        public void Expose(int x, int y)
        {
            if (CheckDimensions(x, y) && !Cards[x, y].Exposed)
            {
                Cards[x, y].Exposed = true;
                ExposedValueSum += Cards[x, y].Value;
            }
        }

        public PlayingCard Replace(PlayingCard replacement, int x, int y)
        {
            if (CheckDimensions(x, y))
            {
                replacement.Exposed = true;
                PlayingCard card = Cards[x, y];
                Cards[x, y] = replacement;
                if (card.Exposed)
                {
                    ExposedValueSum += (replacement.Value - card.Value);
                }
                else
                {
                    ExposedValueSum += replacement.Value;
                }
                return card;
            }
            return null;
        }

        public void ExposeAll()
        {
            int sum = 0;
            foreach (PlayingCard card in Cards)
            {
                card.Exposed = true;
                sum += card.Value;
            }
            ExposedValueSum = sum;
        }

        public void DoubleSum()
        {
            if (ExposedValueSum > 0)
            {
                ExposedValueSum *= 2;

            }
        }
    }
}
