
using System;
using System.Linq;

namespace CodeSanook.Common.DataTypes
{
    public static class StringHelper
    {
        private static Random random = new Random();

        public static byte[] GetBytesFromAsciiString(this string input)
        {
            return System.Text.Encoding.ASCII.GetBytes(input);
        }

        public static string GetRandomAsciiString(this int length)
        {
            var byteArray = Enumerable.Range(1, length)
                .Select(_ => (byte)random.Next(33, 127)).ToArray();//random from unicode 33 to 126
            return System.Text.Encoding.ASCII.GetString(byteArray);
        }
    }
}