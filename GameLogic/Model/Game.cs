using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Model
{
    class Game
    {
        private Queue<PlayingCard> _coveredStack;

        public byte RoundCounter { get; set; } = 0;
        public List<Player> Players { get; private set; }
        public byte CurrentPlayer { get; private set; } = 0;
        public PlayingCard CoveredStackTop { get => _coveredStack.Dequeue(); }
        public PlayingCard ExposedCard { get; private set; } = null;


        public Game(IEnumerable<Player> players)
        {
            Players = new List<Player>(players);
            List<PlayingCard> cards = CreateGameCards();
            DistributeCards(cards);
            _coveredStack = new Queue<PlayingCard>(cards);
        }

        private List<PlayingCard> CreateGameCards()
        {
            List<PlayingCard> cards = new List<PlayingCard>();
            for (short i = PlayingCard.MinValue; i < PlayingCard.MaxValue; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    cards.Add(new PlayingCard(i));
                }
            }
            cards.Shuffle();
            return cards;
        }

        private void DistributeCards(List<PlayingCard> cards)
        {
            foreach (Player player in Players)
            {
                PlayingCard[,] playerCards = new PlayingCard[4, 4];
                try
                {
                    for (int i = 0; i < playerCards.GetLength(0); i++)
                    {
                        for (int j = 0; j < playerCards.GetLength(1); j++)
                        {
                            playerCards[i, j] = cards[0];
                            cards.RemoveAt(0);
                        }
                    }
                }
                catch (Exception e)
                {
                    //TODO: handle Exception if no more Cards available
                }
            }
        }

    }

    public static class ListHelper
    {
        private static readonly Random random = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
