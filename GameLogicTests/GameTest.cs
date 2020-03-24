using System;
using System.Collections.Generic;
using System.Text;
using GameLogic;
using GameLogic.Model;
using Xunit;

namespace GameLogicTests
{
    public class GameTest
    {
        private Game game;

        public GameTest()
        {
            List<Player> players = new List<Player>();
            players.Add(new UserPlayer("Player1"));
            players.Add(new UserPlayer("Player2"));
            players.Add(new UserPlayer("Player3"));
            game = new Game(players);
        }

        [Fact]
        public void Create_Game_Different()
        {
            Game game2 = new Game(new List<Player>());
            Assert.NotSame(game, game2);
            Assert.NotSame(game.CoveredStackTop, game2.CoveredStackTop);
        }

        [Fact]
        public void Create_Game_CardsShuffled()
        {
            PlayingCard[] cards = new PlayingCard[15];
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = game.CoveredStackTop;
            }

            Assert.Contains(cards, x => x != cards[0]);
        }

        [Fact]
        public void Start_Round_CardsDistributed()
        {
            game.StartRound();
            foreach (Player player in game.Players)
            {
                Assert.NotNull(player.CurrentCardSet);
            }
        }

        [Fact]
        public void Finish_Round()
        {
            Player p = game.Players[0];
            int oldCounter = game.RoundCounter;
            p.CurrentCardSet.ExposeAll();
            Assert.Throws<RoundFinishedException>(() => p.CurrentCardSet.RefreshSet());
            p.DrawCovered();
            p.CardAction((0,0), true);
            Assert.Equal(p,game.RoundFinishingPlayer);
            game.FinishRound();
            Assert.Equal(oldCounter+1, game.RoundCounter);
            Assert.Null(game.RoundFinishingPlayer);
        }

        [Fact]
        public void Player_CardAction_UpdateExposed()
        {
            Player p = game.Players[0];
            p.DrawCovered();
            PlayingCard newExposed = p.TemporaryCard;
            p.CardAction((0,0), false);
            Assert.Same(newExposed, game.ExposedCard);
            p.DrawExposed();
            Assert.Same(newExposed, p.TemporaryCard);
            newExposed = p.CurrentCardSet.Cards[0, 0];
            p.CardAction((0,0), true);
            Assert.Same(newExposed, game.ExposedCard);
        }
    }
}
