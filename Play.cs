using System.Xml;
using System.Xml.Serialization;

namespace Program2
{
    public static class Play
    {
        static void toXML(Player player, List<Game> gamelist)
        {
            XmlSerializer pl = new XmlSerializer(typeof(Player));
            XmlSerializer g = new XmlSerializer(typeof(Game));
            //ensure that a new document is created, with users name and a random number
            string path = player.user + "s_data.xml";
            TextWriter writer = new StreamWriter(@path);
            pl.Serialize(writer, player);
            foreach(var game in gamelist)
            {
                g.Serialize(writer,game as Game);
            }
            writer.Close();
        }
        //code for promting user if they would like to continue the game
        public static bool promp(Player player)
        {
            bool cont;
            char input = Console.ReadLine()[0];
            if(input == 'y' || input == 'Y')
            {
                player.life = 6;
                cont = true;
            }
            else 
            {
                cont = false;
            }
            return cont;
        }
        public static void gameEngine(Player player)
        {
            //vars
            Boolean cont = true;
            List<Game> gameslist = new List<Game>();
            //Start of 'game engine' loop
            while(cont)
            {
                //Initialize game
                //generate random phrase
                var phrase = Puzzle.phraseGen();
                //split phrase into a char array 
                char[] letters = Puzzle.createphraseArr(phrase);
                //create an array for 'guesses'
                char[] guessed = Puzzle.createGuessArr(letters);
                //create an array for wrong entries, since players have 6 lives, they can only get 6 things wrong 
                char[] wrong = new char[6];
                //play, also set victory conditions, if playerlife reaches zero, or if the puzzle is solved
                while(player.life != 0 && Puzzle.victoryCheck(letters,guessed) == false)
                {
                    //display the empty guesses array, format for easier reading by user 
                    Console.WriteLine("-------------------------------");
                    Console.WriteLine("Guess this phrase: ");
                    Puzzle.PrintArr(guessed);
                    Console.WriteLine();
                    //spit out wrong inputs user has input thus far, display life 
                    Console.Write("Life left: " + player.life + "\n Wrong inputs so far: ");
                    Puzzle.PrintArr(wrong);
                    Console.WriteLine();
                    //ask for user input 
                    Console.Write("Enter letter to guess: ");
                    try
                    {
                        char guess = Console.ReadLine()[0];
                        //subtract a life if incorrect guess, display incorrect letter in wrong array 
                        if(Puzzle.check(guess, guessed, letters) == false)
                        {
                            player.life -= 1;
                            wrong[player.life] = guess;
                        }
                    }
                    catch(IndexOutOfRangeException)
                    {
                        Console.WriteLine("OUT OF BOUNDS, DO NOT ENTER NULL VALUES!!! ");
                    }

                }
                Console.WriteLine("-------------------------------");
                Console.WriteLine("The correct phrase was: ");
                Puzzle.PrintArr(letters);
                Console.WriteLine();
                Console.WriteLine("-------------------------------");
                //loose messasge, update loss score 
                if(player.life == 0)
                {
                    player.gameslost += 1;
                    //create a game object in gamelist, track phrase and date of attempt, win=false
                    gameslist.Add(new Game
                    (
                        DateTime.Today,
                        phrase,
                        false
                    ));
                    Console.WriteLine("Looks like you lost, care to play again? (Y/N): ");
                    cont = Play.promp(player);
                }
                //win message, update win score 
                else
                {
                    player.gameswon += 1;
                    //create a game object in gamelist, track phrase and date of attempt, win=true
                    gameslist.Add(new Game
                    (
                        DateTime.Today,
                        phrase,
                        true
                    ));
                    Console.WriteLine("Looks like you won! Care to play again? (Y/N): ");
                    cont = Play.promp(player);
                }
            }
            //out of game loop, export all games and player data to xml
            //'convert' ints into decimals for winrate calculation, calculate winpercent
            decimal top = player.gameswon;
            decimal bottom = player.gameslost+player.gameswon;
            player.calculateWin(top, bottom, player);
            Console.WriteLine(player.winpercent);
            Play.toXML(player, gameslist);
        }
    }
}