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

    }
}
