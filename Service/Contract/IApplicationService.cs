using System.Threading.Tasks;
using ChatServer.Model.ViewModels;

namespace ChatServer.Service.Contract {
    public interface IApplicationService {
        Task SeedData ();
    }
}