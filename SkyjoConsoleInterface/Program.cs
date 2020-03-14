using System;
using System.Collections.Generic;
using GameLogic.Model;

namespace SkyjoConsoleInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Abort with CTRL-C");
            List<Player> players = new List<Player>();
            if (args.Length == 0)
            {
                int player_count = 1;
                Console.Write("Player-{0} Name: ", player_count++);
                string input = Console.ReadLine();
                while (!string.IsNullOrWhiteSpace(input))
                {
                    players.Add(new UserPlayer(input));
                    Console.Write("Player-{0} Name: ", player_count++);
                    input = Console.ReadLine();
                }

                if (players.Count < 2)
                {
                    Console.WriteLine("At least 2 players required");
                    Environment.Exit(0);
                }
                Game game = new Game(players);
                //Start game - First Turn Action (expose 2 cards per player)
                //Then: for each player do Action
                //Until: one player finished => RoundFinishedException
            }
        }
    }
}
