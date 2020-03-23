using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GameLogic.Model
{
    public class Game
    {
        private Queue<PlayingCard> _coveredStack;

        public byte RoundCounter { get; set; } = 0;
        public List<Player> Players { get; private set; }
        public byte CurrentPlayer { get; private set; } = 0;
        public PlayingCard CoveredStackTop { get => _coveredStack.Dequeue(); }
        public PlayingCard ExposedCard { get; set; } = null;
        public ScoreBoard ScoreBoard { get; set; }


        public Game(IEnumerable<Player> players)
        {
            Players = new List<Player>(players);
            List<PlayingCard> cards = CreateGameCards();
            DistributeCards(cards);
            ExposedCard = cards[0];
            cards.RemoveAt(0);
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
                PlayingCard[,] playerCards = new PlayingCard[PlayerCardSet.columnCount, PlayerCardSet.rowCount];
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

        internal void Notify(Exception exception)
        {
            if (!(exception is RoundFinishedException)) return;
            RoundFinishedException rfe = exception as RoundFinishedException;
            //TODO: Change to match game rules (every player has one last action)
            foreach (Player player in Players)
            {
                if(player == rfe.PlayerSource) continue;
                player.CurrentCardSet.ExposeAll();
            }
            if(rfe.PlayerSource.CurrentCardSet.ExposedValueSum >= Players.Min(p => p.CurrentCardSet.ExposedValueSum))
            {
                rfe.PlayerSource.CurrentCardSet.DoubleSum();
            }
            ScoreBoard.UpdateScores(Players);
        }

    }
}
