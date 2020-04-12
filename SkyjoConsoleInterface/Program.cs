using System;
using System.Collections.Generic;
using System.Threading.Channels;
using GameLogic;
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
                    players.Add(new ComputerPlayer());
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
            // First Turn Action
            foreach (Player player in game.Players)
            {
                if (player is ComputerPlayer com)
                {
                    com.AutoFirstTurnAction();
                    Console.WriteLine("Card set of ComputerPlayer {0}:",com.Id);
                    DisplayCardSet(com.CurrentCardSet);
                    continue;
                }
                DisplayCardSet(player.CurrentCardSet);
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
                DisplayCardSet(player.CurrentCardSet);
            }

            while (!game.LastAction)
            {
                foreach (Player player in game.Players)
                {
                    RoundAction(player);
                    if (game.LastAction) break;
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
            Console.WriteLine(new string('=', 40) + "\n");
            Console.WriteLine("{0} your turn: ", player.Id);
            DisplayCardSet(player.CurrentCardSet);
            if (player is ComputerPlayer com)
            {
                com.AutoPlayRound();
                Console.WriteLine("ComputerPlayer {0} finished turn. Card Set:", com.Id);
                DisplayCardSet(com.CurrentCardSet);
                return;
            }

            Console.WriteLine("The exposed card value is: {0}", player.Game.ExposedCard.Value);
            bool? draw_choice;
            do
            {
                Console.Write("Draw exposed card (0) or draw covered card (1): ");
                draw_choice = ParseBooleanInput(Console.ReadLine());
            } while (draw_choice == null);

            bool? replace_choice;
            if (draw_choice.Value)
            {
                player.DrawCovered();
                Console.WriteLine("The value of the drawn card is {0}.", player.TemporaryCard.Value);
                do
                {
                    Console.Write("Expose a card (0) or replace a card (1): ");
                    replace_choice = ParseBooleanInput(Console.ReadLine());
                } while (replace_choice == null);
            }
            else
            {
                player.DrawExposed();
                replace_choice = true;
            }

            (byte, byte)? coordinates;
            do
            {
                Console.Write("{0} card at coordinates (x,y): ", replace_choice.Value ? "Replace" : "Expose");
                coordinates = ParseCoordinateInput(Console.ReadLine());
                if (coordinates != null)
                {
                    try
                    {
                        player.CardAction(coordinates.Value, replace_choice.Value);
                        break;
                    }
                    catch (CouldNotExposeError)
                    {
                        Console.WriteLine("Expose Failed, try again:");
                        coordinates = null;
                    }
                }
            } while (coordinates == null);

            DisplayCardSet(player.CurrentCardSet);
        }

        private static void DisplayCardSet(PlayerCardSet set)
        {
            Console.WriteLine();
            for (int i = -1; i < set.Cards.GetLength(0); i++)
            {
                if (i == -1)
                {
                    for (int j = 0; j < set.Cards.GetLength(1); j++)
                    {
                        if (j == 0) Console.Write("  ║");
                        Console.Write("  " + j);
                    }

                    Console.WriteLine("\n" + new string('=', (set.Cards.GetLength(1) + 1) * 3 + 1));
                    continue;
                }
                Console.Write(i + " ║");
                for (int j = 0; j < set.Cards.GetLength(1); j++)
                {
                    if (set.Cards[i, j] == null) Console.Write("{0,3}", " ");
                    else if (set.Cards[i, j].Exposed) Console.Write("{0,3}", set.Cards[i, j].Value);
                    else Console.Write("{0,3}", "X");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Parse X,Y Coordinate Input
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Coordinate-Tuple, null if cannot parse</returns>
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
