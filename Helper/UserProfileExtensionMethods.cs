using System.Collections.Generic;
using System.Linq;
using ChatServer.Model;

namespace ChatServer.Helper {
    public static class UserProfileExtensionMethods {
        public static IEnumerable<UserProfile> WithoutPasswords (this IEnumerable<UserProfile> users) {
            return users.Select (u => u.WithoutPassword ());
        }

        public static UserProfile WithoutPassword (this UserProfile user) {
            user.passwordHash = null;
            return user;
        }
    }
}