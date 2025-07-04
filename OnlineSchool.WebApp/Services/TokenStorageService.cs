namespace OnlineSchool.WebApp.Services
{
    public class TokenStorageService
    {
        public string Token { get; private set; }

        public void SetToken(string token)
        {
            Token = token;
        }
    }
}