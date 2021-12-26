namespace Nexus.OAuth.Domain
{
    public class GeneralHelpers
    {
        /// <summary>
        /// Transform string password in string hash 
        /// </summary>
        /// <param name="password">String password</param>
        /// <returns>New hash by password</returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Valid password by password hash.
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="hash">Password hash.</param>
        /// <returns>Password is valid</returns>
        public static bool ValidPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        /// <summary>
        /// Generate Tokens with specific length
        /// </summary>
        /// <param name="size">Token Size</param>
        /// <param name="lower">Use lowercase characters.</param>
        /// <param name="upper">Use uppercase characters.</param>
        /// <returns>New token with size value.</returns>
        public static string GenerateToken(int size, bool upper = true, bool lower = true)
        {
            // ASCII characters rangers
            byte[] lowers = new byte[] { 97, 123 };
            // Upercase latters
            byte[] uppers = new byte[] { 65, 91 };
            // ASCII numbers
            byte[] numbers = new byte[] { 48, 58 };

            Random random = new();
            string result = string.Empty;

            for (int i = 0; i < size; i++)
            {
                int type = random.Next(0, lower ? 3 : 2);

                byte[] possibles = type switch
                {
                    1 => upper ? uppers : numbers,
                    2 => lowers,
                    _ => numbers
                };

                int selected = random.Next(possibles[0], possibles[1]);
                char character = (char)selected;

                result += character;
            }

            return result;
        }
    }
}