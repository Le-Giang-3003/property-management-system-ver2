using System;

namespace HashGen
{
    class Program
    {
        static void Main(string[] args)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("123456");
            Console.WriteLine("GENERATED_HASH=" + hash);
            
            var isValid = BCrypt.Net.BCrypt.Verify("123456", hash);
            Console.WriteLine("IS_VALID=" + isValid);
        }
    }
}
