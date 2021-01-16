using System;
using System.IO;

namespace Hangman
{
    class Program
    {
        const string hungMan = @"
 _______
 │/    |
 │     o
 │    -O-
 │    / \
 │
 │__";
        const string freeMan = @"
 _______
 │/    
 │     
 │    
 │        \o/
 │         O
 │__      / \
";
        const string wordsFileName = "words.txt";

        static void Main()
        {
            bool newGame = true;

            while (newGame)
            {
                ClearConsole();
                Console.WriteLine("Welcome to Hangman!");
                WriteNewLines(1);

                string hangmanWord = GetHangmanWord();

                WriteNewLines(1);
                Console.Write("Let's play! Press a key to continue... ");
                Console.ReadLine();
                WriteNewLines(1);

                bool isGuessed = IsGuessed(hangmanWord);

                ClearConsole();
                WriteNewLines(3);

                if (isGuessed)
                {
                    Console.WriteLine(freeMan);
                    WriteNewLines(1);
                    Console.WriteLine($"You've guessed the word! It was {hangmanWord}.");
                }
                else
                {
                    Console.WriteLine(hungMan);
                    WriteNewLines(1);
                    Console.WriteLine($"You couldn't escape the gallows. The word was {hangmanWord}.");
                }

                WriteNewLines(1);
                Console.Write("Do you want to start a new game? (Y/N) ");
                newGame = UserSaysYes();
            }
        }

        static string GetHangmanWord()
        {
            string hangmanWord;

            Console.Write("Do you want to input your own word? (Y/N) ");

            bool wantsUserInput = UserSaysYes();

            if (wantsUserInput)
            {
                hangmanWord = GetInputHangmanWord();
                
                WriteNewLines(1);

                AddHangmanWordToWordsFile(hangmanWord);
            }
            else
            {
                string[] pregeneratedWords = File.ReadAllLines(wordsFileName);

                hangmanWord = pregeneratedWords[new Random().Next(0, pregeneratedWords.Length - 1)];
            }

            return hangmanWord;
        }

        static string GetInputHangmanWord()
        {
            string hangmanWord = null;
            bool isWord = false;
            char[] invalidChars = new char[] { ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',', '.', ';', ':', '!', '?', '/', '\\', '\'', '"', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=' };
         
            while (!isWord)
            {
                WriteNewLines(1);
                Console.Write("Your word: ");
                hangmanWord = Console.ReadLine().ToLower();

                if (string.IsNullOrEmpty(hangmanWord) || hangmanWord.IndexOfAny(invalidChars) != -1)
                {
                    WriteRedText("You have to enter a word for hangman.");
                    PlaySound(SoundType.Error);
                }
                else
                {
                    isWord = true;
                }
            }

            return hangmanWord;
        }

        static void AddHangmanWordToWordsFile(string hangmanWord)
        {
            Console.Write("Do you want to add this word to the list of pregenerated words? (Y/N) ");

            if (UserSaysYes())
            {
                File.AppendAllText(wordsFileName, $"{Environment.NewLine}{hangmanWord.ToLower()}");
            }
        }

        static bool IsGuessed(string hangmanWord)
        {
            string[] hangman =
                {
@" _______
 │/    |
 │     o
 │    -O-
 │    / 
 │
 │__",
@" _______
 │/    |
 │     o
 │    -O-
 │    
 │
 │__",
@" _______
 │/    |
 │     o
 │    -O
 │    
 │
 │__",
@" _______
 │/    |
 │     o
 │     O
 │    
 │
 │__",
@" _______
 │/    |
 │     o
 │    
 │    
 │
 │__",
@" _______
 │/    |
 │     
 │    
 │    
 │
 │__",
@" _______
 │/    
 │     
 │    
 │    
 │
 │__",
            };

            bool isGuessed = false;
            bool isHung = false;

            string dots = new string('.', hangmanWord.Length);
            char[] guessedWord = dots.ToCharArray();

            int tries = hangman.Length;

            string triedLetters = null;

            while (!isGuessed && !isHung)
            {
                ClearConsole();
                Console.WriteLine("You have to guess the following word:");
                Console.WriteLine(guessedWord);
                WriteNewLines(1);
                Console.WriteLine("Look out for the gallows!");
                Console.WriteLine(hangman[tries - 1]);
                WriteNewLines(1);
                Console.WriteLine($"You have tried the following letters: {triedLetters}");

                char letter = GetLetter();

                if (!hangmanWord.Contains(letter))
                {
                    triedLetters = $"{triedLetters} {letter}";
                    tries--;

                    WriteNewLines(1);
                    WriteRedText($"The word doesn't contain your letter {letter}. You have {tries} tries left.");
                    PlaySound(SoundType.Error);
                    Console.Write("Press a key to continue... ");
                    Console.ReadLine();

                    if (tries == 0)
                    {
                        isHung = true;
                    }
                }
                else
                {
                    for (int i = 0; i < hangmanWord.Length; i++)
                    {
                        if (hangmanWord[i] == letter)
                        {
                            guessedWord[i] = letter;
                        }
                    }

                    WriteNewLines(1);
                    Console.WriteLine("The word contains your letter!");
                    PlaySound(SoundType.Confirmation);
                    Console.Write("Press a key to continue... ");
                    Console.ReadLine();

                    if (new string(guessedWord) == hangmanWord)
                    {
                        isGuessed = true;
                    }
                }
            }

            return isGuessed;
        }

        static char GetLetter()
        {
            bool isLetter = false;
            char letter = default;
            
            while (!isLetter)
            {
                WriteNewLines(1);
                Console.Write("Which letter do you want to guess? ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    ShowLetterErrorMessage();
                }
                else if (!char.IsLetter(input[0]) || input.Length > 1)
                {
                    ShowLetterErrorMessage();
                }
                else
                {
                    isLetter = true;
                    letter = char.ToLower(input[0]);
                }
            }

            return letter;
        }

        static void ShowLetterErrorMessage()
        {
            WriteNewLines(1);
            WriteRedText("You have to enter one letter.");
            PlaySound(SoundType.Error);
            WriteNewLines(1);
        }

        static bool UserSaysYes()
        {
            string yesOrNo = Console.ReadLine().Trim().ToUpper();

            string[] validAnswers = { "Y", "N", "YES", "NO" };

            bool isYes;

            while (Array.IndexOf(validAnswers, yesOrNo) == -1)
            {
                WriteRedText("This input is invalid.");
                PlaySound(SoundType.Error);
                WriteNewLines(1);
                Console.Write("Please enter Yes (Y) or No (N). ");
                yesOrNo = Console.ReadLine().Trim().ToUpper();
            }

            if (yesOrNo == "Y" || yesOrNo == "YES")
            {
                isYes = true;
            }
            else
            {
                isYes = false;
            }

            return isYes;
        }

        static void ClearConsole()
        {
            string banner = @"
  _   _                                         
 | | | | __ _ _ __   __ _ _ __ ___   __ _ _ __  
 | |_| |/ _` | '_ \ / _` | '_ ` _ \ / _` | '_ \ 
 |  _  | (_| | | | | (_| | | | | | | (_| | | | |
 |_| |_|\__,_|_| |_|\__, |_| |_| |_|\__,_|_| |_|
                    |___/                       
";

            Console.Clear();
            Console.WriteLine(banner);
        }

        static void WriteNewLines(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Console.WriteLine();
            }
        }

        static void WriteRedText(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        static void PlaySound(SoundType type)
        {
            if (type == SoundType.Error)
            {
                Console.Beep(400, 50);
                Console.Beep(300, 50);
                Console.Beep(250, 150);
            }
            else if (type == SoundType.Confirmation)
            {
                Console.Beep(400, 50);
                Console.Beep(800, 50);
                Console.Beep(1200, 150);
            }
        }
    }
}
