using System;
using System.Text;

namespace CodeSanook.Common
{
    public class RandomHelper
    {
        private static Random random = new Random();

        public static String GenerateRandomCharacters(int randomLength)
        {
            // Create an array of characters to user for password reset.
            // Exclude confusing or ambiguous characters such as 1 0 l o i
            var characters = new String[] {
                "2", "3", "4", "5", "6", "7", "8", "9",
                "a", "b", "c", "d", "e", "f", "g", "h", "j", "k", "m", "n",
                "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
            };

            var randomCharacters = new StringBuilder();
            for (int i = 0; i < randomLength; i++)
            {
                randomCharacters.Append(characters[random.Next(characters.Length)]);
            }
            return randomCharacters.ToString();
        }
    }
}