using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;

namespace ChatServer.Repository.Contract {
    public interface IUserProfileRepository {
        List<UserProfile> Get ();
        Task<List<UserProfile>> GetAsync ();
        UserProfile Get (string id);
        Task<UserProfile> GetAsync (string id);
        UserProfile Find (System.Linq.Expressions.Expression<Func<UserProfile, bool>> expression);
        Task<UserProfile> FindAsync (System.Linq.Expressions.Expression<Func<UserProfile, bool>> expression);
        UserProfile Create (UserProfile user);
        Task<UserProfile> CreateAsync (UserProfile user);
        void Update (string id, UserProfile userIn);
        void Remove (UserProfile userIn);
        void Remove (string id);
    }
}