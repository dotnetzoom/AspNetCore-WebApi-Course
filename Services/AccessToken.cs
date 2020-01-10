namespace Services
{
    public class AccessToken
    {
        public string Access_Token { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }

        public AccessToken(string accessToken, string refreshToken,int expires)
        {
            Access_Token = accessToken;
            RefreshToken = refreshToken;
            TokenType = "Bearer";
            ExpiresIn = expires;
        }
    }
}