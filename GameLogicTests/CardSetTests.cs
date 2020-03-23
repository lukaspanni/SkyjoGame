using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Model;
using Xunit;

namespace GameLogicTests
{
    public class CardSetTests
    {
        private readonly PlayerCardSet cardSet;
        private short card_value = 1;

        public CardSetTests()
        {
            PlayingCard[,] cards = new PlayingCard[PlayerCardSet.rowCount, PlayerCardSet.columnCount];
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
            Assert.Equal(PlayerCardSet.rowCount * PlayerCardSet.columnCount * card_value, cardSet.ExposedValueSum);
        }

        [Fact]
        public void CardSet_RefreshSet_RemoveSum0()
        {
            cardSet.ExposeAll();
            cardSet.RefreshSet();   //should remove all because all values are equal
            Assert.Null(cardSet.Cards[0, 0]);
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

        [Theory]
        [InlineData(1, 1, 1, false)]
        [InlineData(2, 1, 5, true)]
        [InlineData(0, 2, -1, false)]
        public void CardSet_Replace_Sum(int x, int y, short val, bool expose)
        {
            PlayingCard card = new PlayingCard(val);
            if (expose)
            {
                cardSet.Expose(x, y);
                Assert.Equal(cardSet.Cards[x,y].Value, cardSet.ExposedValueSum);
            }

            cardSet.Replace(card, x, y);
            Assert.Equal(val, cardSet.ExposedValueSum);
            
        }

        [Theory]
        [InlineData(13, 16, 255)]
        [InlineData(-3, -5, -9)]
        [InlineData(4096, 32767, -32768)]
        public void PlayingCard_Value_SetInvalid(short v1, short v2, short v3)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => cardSet.Cards[0, 0].Value = v1);
            Assert.Throws<ArgumentOutOfRangeException>(() => cardSet.Cards[0, 0].Value = v2);
            Assert.Throws<ArgumentOutOfRangeException>(() => cardSet.Cards[0, 0].Value = v3);
        }
    }
}
