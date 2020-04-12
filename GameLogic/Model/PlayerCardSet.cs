using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;

namespace GameLogic.Model
{
    public class PlayerCardSet
    {
        private PlayingCard[,] _cards;

        public PlayingCard[,] Cards { get => _cards; private set { _cards = value; RefreshSet(); } }
        public int ExposedValueSum { get; private set; }

        public static readonly int columnCount = 4;
        public static readonly int rowCount = 3;

        public PlayerCardSet(PlayingCard[,] cards)
        {
            if (cards.GetLength(0) == rowCount && cards.GetLength(1) == columnCount)
            {
                foreach (PlayingCard card in cards)
                {
                    card.Exposed = false;
                }

                Cards = cards;
            }
            else
            {
                Cards = new PlayingCard[rowCount,columnCount];
            }
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
                    if (Cards[i, j] == null)
                    {
                        exposedCount += 1;  // null-card counts as exposed
                    }
                    else if (Cards[i, j].Exposed)
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

        private bool CheckDimensions((byte,byte) coordinates)
        {
            if (coordinates.Item1 <= Cards.GetLength(0) && coordinates.Item2 <= Cards.GetLength(1))
            {
                return true;
            }
            return false;
        }

        public void Expose((byte,byte) coordinates)
        {
            if (CheckDimensions(coordinates) && Cards[coordinates.Item1, coordinates.Item2] != null && !Cards[coordinates.Item1, coordinates.Item2].Exposed)
            {
                Cards[coordinates.Item1, coordinates.Item2].Exposed = true;
                ExposedValueSum += Cards[coordinates.Item1, coordinates.Item2].Value;
            }
            else
            {
                throw new CouldNotExposeError();
            }
        }

        public PlayingCard Replace(PlayingCard replacement, (byte, byte) coordinates)
        {
            if (CheckDimensions(coordinates))
            {
                replacement.Exposed = true;
                PlayingCard card = Cards[coordinates.Item1, coordinates.Item2];
                Cards[coordinates.Item1, coordinates.Item2] = replacement;
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
                if (card != null) card.Exposed = true;
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
