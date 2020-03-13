using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Model;
using Xunit;

namespace GameLogicTests
{
    public class GameTest
    {
        private Game game;

        public GameTest()
        {
            game = new Game(new List<Player>());
        }

        [Fact]
        public void Create_Game_Different()
        {
            Game game2 = new Game(new List<Player>());
            Assert.NotSame(game, game2);
            Assert.NotEqual(game.CoveredStackTop, game2.CoveredStackTop);
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

    }
}
