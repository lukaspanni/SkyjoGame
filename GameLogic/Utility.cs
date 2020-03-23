using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using GameLogic.Model;

namespace GameLogic
{
    public class RoundFinishedException : Exception
    {
        public Player PlayerSource { get; set; }
        public RoundFinishedException()
        {
        }

        public RoundFinishedException(string message) : base(message)
        {
        }

        public RoundFinishedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RoundFinishedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public static class ListUtility
    {
        private static readonly Random random = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
