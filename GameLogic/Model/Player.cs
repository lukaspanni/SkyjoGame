using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace GameLogic.Model
{
    public abstract class Player
    {
        private PlayerCardSet _currentCardSet;
        public PlayingCard TemporaryCard { get; private set; }

        public PlayerCardSet CurrentCardSet
        {
            get => _currentCardSet;
            set
            {
                _currentCardSet = value;
                if(Id == null && value != null) Id = _currentCardSet.GetHashCode().ToString();
            }
        }

        public Game Game { get; set; }
        public string Id { get; protected set; }
        

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
