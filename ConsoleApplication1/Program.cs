using System;
using System.IO;
using PS.Http;

namespace ConsoleApplication1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var jsonHttp = new JsonHttp(new HttpClient(), new MessageSerializer());

            var playerId = GetPlayerId(jsonHttp);

            var newGameResponse = jsonHttp.Post<NewGameResponse>($"http://138.68.21.118/api/player/{playerId}/new-game",
                new object());
            Console.Out.WriteLine(
                $"Game started. Secret is {newGameResponse.SecretLength} digits and you have {newGameResponse.RemainingGuesses} guesses.");

            NewGuessResponse guessResponse;

            do
            {
                Console.Out.Write("What is your guess? ");
                var guess = Console.In.ReadLine();
                guessResponse = jsonHttp.Post<NewGuessResponse>($"http://138.68.21.118/api/player/{playerId}/guess", new GuessRequest(guess));
                if (guessResponse.WasCorrect)
                {
                    Console.Out.WriteLine($"{guessResponse.Guess} was correct!");
                }
                else
                {
                    Console.Out.Write($"{guessResponse.Guess} was incorrect. {guessResponse.CorrectDigits} digits were correct and {guessResponse.MisplacedDigits} were misplaced. {guessResponse.RemainingGuesses} guesses remain. ");
                }
            } while (guessResponse.RemainingGuesses > 0 && !guessResponse.WasCorrect);

            if (guessResponse.WasCorrect)
            {
            }
            else
            {
                Console.Out.WriteLine($"You lost.");
            }
            Console.Out.WriteLine("Hit [Enter] to exit.");
            Console.In.ReadLine();
        }

        private static string GetPlayerId(JsonHttp jsonHttp)
        {
            if (File.Exists("player.id"))
            {
                var lines = File.ReadAllLines("player.id");
                Console.Out.WriteLine($"Welcome back, {lines[0]}");
                return lines[1];
            }

            Console.Out.Write("Enter your name: ");
            var playerName = Console.In.ReadLine();
            var playerRegistrationResponse = jsonHttp.Post<PlayerRegistrationResponse>($"http://138.68.21.118/api/player", new
            {
                name = playerName
            });
            File.WriteAllText("player.id", playerName + Environment.NewLine);
            File.AppendAllText("player.id", playerRegistrationResponse.PlayerId + Environment.NewLine);
            return playerRegistrationResponse.PlayerId;
        }
    }

    internal class GuessRequest
    {
        public GuessRequest(string guess)
        {
            Guess = guess;
        }

        public string Guess { get; set; }
    }

    internal class NewGuessResponse
    {
        public string Guess { get; set; }
        public bool WasCorrect { get; set; }
        public int CorrectDigits { get; set; }
        public int MisplacedDigits { get; set; }
        public int RemainingGuesses { get; set; }
    }

    internal class NewGameResponse
    {
        public string SecretLength { get; set; }
        public string RemainingGuesses { get; set; }
    }

    internal class PlayerRegistrationResponse
    {
        public string PlayerId { get; set; }
    }
}