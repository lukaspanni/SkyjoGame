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

        public void FirstTurnAction(int x1, int y1, int x2, int y2)
        {
            if (x1 != x2 || y1 != y2)
            {
                CurrentCardSet.Expose(x1, y1);
                CurrentCardSet.Expose(x2, y2);
            }
        }

    }
}
