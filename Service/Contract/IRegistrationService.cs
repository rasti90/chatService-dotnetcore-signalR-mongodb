using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;
using ChatServer.Model.ViewModels;

namespace ChatServer.Service.Contract
{
    public interface IRegistrationService
    {
        Task<UserProfile> Register(RegisterVM model);
    }
}