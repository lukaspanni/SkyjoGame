using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Model;
using Xunit;

namespace GameLogicTests
{
    public class PlayerSet_Tests
    {
        private readonly PlayerCardSet cardSet;
        private PlayingCard[,] cards;
        private int xdim = 4;
        private int ydim = 4;
        private short card_value = 1;

        public PlayerSet_Tests()
        {
            cards = new PlayingCard[xdim, ydim];
            for (int i = 0; i < cards.GetLength(0); i++)
            {
                for (int j = 0; j < cards.GetLength(1); j++)
                {
                    cards[i, j] = new PlayingCard(card_value);
                }
            }
            cardSet = new PlayerCardSet(cards);
        }

        [Fact]
        public void CardSet_ExposeAll_Sum()
        {
            cardSet.ExposeAll();
            Assert.Equal(4 * 4 * card_value, cardSet.ExposedValueSum);
        }

        [Fact]
        public void CardSet_RefreshSet_RemoveSum0()
        {
            cardSet.ExposeAll();
            cardSet.RefreshSet();   //should remove all because all values are equal
            foreach (var item in cardSet.Cards)
            {
                Assert.Null(item);
            }
            Assert.Equal(0, cardSet.ExposedValueSum);
        }

        [Fact]
        public void CardSet_Replace_CorrectCard()
        {
            short val = (short)(card_value - 1);
            PlayingCard card = new PlayingCard(val);
            cardSet.ExposeAll();
            cardSet.Replace(card, 1, 1);
            Assert.Same(card, cardSet.Cards[1, 1]);
        }

    }
}
