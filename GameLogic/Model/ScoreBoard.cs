using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Model
{
    public class ScoreBoard
    {
        public Dictionary<Player, int> Scores { get; private set; }
        
        public ScoreBoard(List<Player> players)
        {
            Scores = new Dictionary<Player, int>(players.Count);
            foreach (Player player in players)
            {
                Scores.Add(player, 0);
            }
        }

        public void UpdateScores(List<Player> players)
        {
            foreach (Player player in players)
            {
                Scores[player] += player.CurrentCardSet.ExposedValueSum;
            }
        }

    }
}
