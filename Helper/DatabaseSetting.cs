namespace ChatServer.Helper {
    public class DatabaseSettings : IDatabaseSettings {
        public string ApplicationsCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
        public string ChatsCollectionName { get; set; }
        public string FilesCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IDatabaseSettings {
        string ApplicationsCollectionName { get; set; }
        string UsersCollectionName { get; set; }
        string ChatsCollectionName { get; set; }
        string FilesCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}