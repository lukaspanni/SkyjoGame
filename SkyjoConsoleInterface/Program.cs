using System;
using System.Collections.Generic;
using GameLogic.Model;

namespace SkyjoConsoleInterface
{
    class Program
    {
        //TODO: Split into different Methods
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
            }
            else
            {
                foreach (string name in args)
                {
                    players.Add(new UserPlayer(name));
                }
            }

            Game game = new Game(players);
            while (!game.GameFinished)
            {
                PlayRound(game);
            }
            //TODO: display finish message

        }

        private static void PlayRound(Game game)
        {
            //TODO: show card values to players
            // First Turn Action
            foreach (Player player in game.Players)
            {
                Console.WriteLine("{0} Expose 2 cards:", player.Id);
                (byte, byte)? coor1;
                (byte, byte)? coor2;
                do
                {
                    Console.Write("First card coordinates (x,y): ");
                    string input = Console.ReadLine();
                    coor1 = ParseCoordinateInput(input);
                } while (coor1 == null);
                do
                {
                    Console.Write("Second card coordinates (x,y): ");
                    string input = Console.ReadLine();
                    coor2 = ParseCoordinateInput(input);
                } while (coor2 == null);
                player.FirstTurnAction(coor1.Value, coor2.Value);
            }

            while (!game.RoundFinished)
            {
                foreach (Player player in game.Players)
                {
                    RoundAction(player);
                    if (game.RoundFinished) break;
                }
            }
            // Round Finished, every player has one last Action
            foreach (Player player in game.Players)
            {
                if (player == game.RoundFinishingPlayer) break;
                RoundAction(player);
            }
            game.FinishRound();
            //TODO: display round stats
        }

        private static void RoundAction(Player player)
        {
            Console.WriteLine("{0} your turn: ", player.Id);
            bool? draw_choice;
            do
            {
                Console.Write("Draw exposed card (0) or draw covered card (1): ");
                draw_choice = ParseBooleanInput(Console.ReadLine());
            } while (draw_choice == null);

            if (draw_choice.Value) player.DrawCovered();
            else player.DrawExposed();

            Console.WriteLine("The value of the drawn card is {0}.\n", player.TemporaryCard);
            bool? replace_choice;
            do
            {
                Console.Write("Expose a card (0) or replace a card (1)");
                replace_choice = ParseBooleanInput(Console.ReadLine());
            } while (replace_choice == null);

            (byte, byte)? coordinates;
            do
            {
                Console.Write("{0} card at coordinates (x,y):", replace_choice.Value ? "Replace" : "Expose");
                coordinates = ParseCoordinateInput(Console.ReadLine());
            } while (coordinates == null);

            player.CardAction(coordinates.Value, replace_choice.Value);
        }

        private static (byte, byte)? ParseCoordinateInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length < 3) return null;
            input = input.Trim();
            char[] splitChars = { ',', ';', '-', ' ' };
            if (input.IndexOfAny(splitChars) != -1)
            {
                byte[] coor = new byte[2];
                int index = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    if (char.IsDigit(input[i]))
                    {
                        coor[index++] = (byte)char.GetNumericValue(input[i]);
                    }
                }
                return (coor[0], coor[1]);
            }
            return null;
        }

        /// <summary>
        /// Parse 0/1 Input, check if string Contains 0 or 1 (0 preferred)
        /// </summary>
        /// <param name="input"></param>
        /// <returns>false if 0, true if 1, null if neither</returns>
        private static bool? ParseBooleanInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.IndexOfAny(new[] { '0', '1' }) == -1) return null;
            input = input.Trim();
            if (input.Contains('0')) return false;
            if (input.Contains('1')) return true;
            return null;
        }
    }
}
