namespace ChatServer.Helper
{
    public class AppSettings : IAppSettings
    {
        public string secret { get; set; }
    }

    public interface IAppSettings
    {
        string secret { get; set; }
    }
}