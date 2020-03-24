using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic.Model
{
    public class ScoreBoard
    {
        public event EventHandler PointsThresholdReached;

        public int PointsThreshold { get; set; } = 100;
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

            if (Scores.Values.Max() >= PointsThreshold)
            {
                OnMaxPointsReached(EventArgs.Empty);
            }
        }

        protected virtual void OnMaxPointsReached(EventArgs e)
        {
            EventHandler handler = PointsThresholdReached;
            handler?.Invoke(this, e);
        }

    }
}
