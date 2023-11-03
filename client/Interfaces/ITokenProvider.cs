namespace client.Interfaces
{
    public interface ITokenProvider
    {
        /// <summary>Sets a token value.</summary>
        /// <param name="token">The token value that you want to set.</param>
        void SetToken(string token);

        /// <summary>Returns a nullable string.</summary>
        string? GetToken();

        /// <summary>Clears a token.</summary>
        void ClearToken();
    }
}