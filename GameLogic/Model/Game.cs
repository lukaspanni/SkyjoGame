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
        public bool LastAction { get; private set; } = false;
        public bool GameFinished { get; private set; } = false;
        public List<Player> Players { get; private set; }
        public Player RoundFinishingPlayer { get; private set; }
        public PlayingCard CoveredStackTop { get => _coveredStack.Dequeue(); }
        public PlayingCard ExposedCard { get; set; } = null;
        public ScoreBoard ScoreBoard { get; set; }


        public Game(IEnumerable<Player> players)
        {
            Players = new List<Player>(players);
            Players.ForEach(p => p.Game = this);
            ScoreBoard = new ScoreBoard(Players);
            ScoreBoard.PointsThresholdReached += ScoreBoardOnPointsThresholdReached;
            StartRound();
        }

        public void StartRound()
        {
            if (GameFinished) return;
            LastAction = false;
            //TODO: Cache Cards, dont create every round
            List<PlayingCard> cards = CreateGameCards();
            DistributeCards(cards);
            ExposedCard = cards[0];
            cards.RemoveAt(0);
            _coveredStack = new Queue<PlayingCard>(cards);
        }

        public void FinishRound()
        {
            foreach (Player player in Players)
            {
                if(player == RoundFinishingPlayer) continue;
                player.CurrentCardSet.ExposeAll();
            }
            if (RoundFinishingPlayer?.CurrentCardSet.ExposedValueSum > Players.Min(p => p.CurrentCardSet.ExposedValueSum))
            {
                RoundFinishingPlayer.CurrentCardSet.DoubleSum();
            }
            ScoreBoard.UpdateScores(Players);
            RoundFinishingPlayer = null;
            RoundCounter++;
        }

        private void ScoreBoardOnPointsThresholdReached(object sender, EventArgs e)
        {
            GameFinished = true;
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
                PlayingCard[,] playerCards = new PlayingCard[PlayerCardSet.rowCount, PlayerCardSet.columnCount];
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
                    player.CurrentCardSet = new PlayerCardSet(playerCards);
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
            if (!LastAction)
            {
                LastAction = true;
                RoundFinishingPlayer = rfe.PlayerSource;
            }
        }

    }
}
