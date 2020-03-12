using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Model
{
   public class PlayingCard
    {
        private static readonly short lowerBound = -2;
        private static readonly short upperBound = 12;
        private short _value;

        public static short MinValue { get => lowerBound; }
        public static short MaxValue { get => upperBound; }
        public short Value {
            get => _value; set {
                if (value >= lowerBound && value <= upperBound)
                {
                    _value = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Invalid value! Value must be between " + lowerBound + " and " + upperBound);
                }
            }
        }
        public bool Exposed { get; set; } 

        public PlayingCard(short value)
        {
            Value = value;
            Exposed = false;
        }

    }
}
