namespace Nexus.OAuth.Libary.Base;

public abstract class BaseClient
{
    private const int defaultClientKeyLength = 96;
    public BaseClient()
    {
        ClientKey = GenerateToken(defaultClientKeyLength);
    }

    public BaseClient(string clientKey)
    {
        ClientKey = clientKey;
    }

    public string ClientKey
    {
        get
        {
            if (string.IsNullOrEmpty(_clientKey))
                _clientKey = GenerateToken(defaultClientKeyLength);

            return _clientKey;
        }
        set
        {
            if (!string.IsNullOrEmpty(_clientKey))
                _clientKey = value;
        }
    }

    private static string? _clientKey;

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