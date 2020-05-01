namespace ChatServer.Model.ViewModels {
    public class BaseKeysetFilterModel{
        public int limit{get;set;}

        public BaseKeysetFilterModel(){
            this.limit=50;
        }
    }
}