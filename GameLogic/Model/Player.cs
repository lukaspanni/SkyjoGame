using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Model
{
    public abstract class Player
    {
        public PlayingCard TemporaryCard { get; set; }
        public PlayerCardSet CurrentCardSet { get; set; }
        public Game Game { get; private set; }
        public string Id { get; protected set; }

        public Player() : this(null, null) { }
        public Player(PlayerCardSet initialCardSet, Game game)
        {
            CurrentCardSet = initialCardSet;
            Game = game;
            Id = CurrentCardSet.GetHashCode().ToString();
        }


        public void DrawCovered()
        {
            TemporaryCard = Game.CoveredStackTop;
        }

        public void DrawExposed()
        {
            TemporaryCard = Game.ExposedCard;
        }


        public void CardAction((byte,byte)coordinates, bool replace)
        {
            if (TemporaryCard == null) throw new InvalidOperationException("A card has to be drawn before this method is called");
            if (replace)
            {
                PlayingCard replaced = CurrentCardSet.Replace(TemporaryCard, coordinates);
                Game.ExposedCard = replaced;
            }
            else
            {
                CurrentCardSet.Expose(coordinates);
                Game.ExposedCard = TemporaryCard;
            }
            TemporaryCard = null;
            try
            {
                CurrentCardSet.RefreshSet();
            }catch(RoundFinishedException rfe)
            {
                rfe.PlayerSource = this;
                Game.Notify(rfe);
            }
        }

        public void FirstTurnAction((byte, byte) c1, (byte,byte) c2)
        {
            if (c1.Item1 != c2.Item1 || c1.Item2 != c2.Item2)
            {
                CurrentCardSet.Expose(c1);
                CurrentCardSet.Expose(c2);
            }
        }

    }
}
