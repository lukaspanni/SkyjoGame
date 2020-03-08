using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Model
{
    abstract class Player
    {

        public PlayingCard TemporaryCard { get; set; }
        public PlayerCardSet CurrentCardSet { get; set; }
        public Game Game { get; private set; }

        public Player() : this(null, null) { }
        public Player(PlayerCardSet initialCardSet, Game game)
        {
            CurrentCardSet = initialCardSet;
            Game = game;
        }

        public void DrawCovered()
        {
            TemporaryCard = Game.CoveredStackTop;
        }

        public void DrawExposed()
        {
            TemporaryCard = Game.ExposedCard;
        }

        public void Action(int x, int y)
        {
            if(TemporaryCard == null)
            {
                CurrentCardSet.Expose(x, y);
            }
            else
            {
                PlayingCard replaced = CurrentCardSet.Replace(TemporaryCard, x, y);
                TemporaryCard = null;
                Game.ExposedCard = replaced;
            }
            try
            {
                CurrentCardSet.RefreshSet();
            }catch(RoundFinishedException rfe)
            {
                rfe.PlayerSource = this;
                Game.Notify(rfe);
            }
        }

    }
}
