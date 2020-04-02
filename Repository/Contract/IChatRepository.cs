using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;

namespace ChatServer.Repository.Contract
{
    public interface IChatRepository
    {
        List<Chat> Get(string appId);
        Task<List<Chat>> GetAsync(string appId);
        Chat Get(string appId, string chatId);
        Task<Chat> GetAsync(string appId, string chatId);
        Task<List<Chat>> GetByUserIdAsync(string appId, string userId);
        List<ChatConversation> GetChatConversations(string app_Id, string chatId, int pageIndex, int pageSize);
        Task<List<ChatConversation>> GetChatConversationsAsync(string appId, string chatId, int pageIndex, int pageSize);
        Task<List<ChatConversation>> GetUnReadChatConversationsAsync(string appId, string chatId, string userId);
        Task<List<ChatMember>> GetChatMembersAsync(string appId, string chatId);
        Chat Create(Chat chat);
        Task<Chat> CreateAsync(Chat chat);
        void Update(string id, Chat chatIn);
        Task UpdateAsync(string id, Chat chatIn);
        void AddMessageToChat(string chatId, ChatConversation conversation);
        Task AddMessageToChatAsync(string chatId, ChatConversation conversation);
        Task AddReaderToConversationAsync(string chatId, string conversationId, ChatConversationReader conversationReader);
        void Remove(Chat chatIn);
        Task RemoveAsync(Chat chatIn);
        void Remove(string id);
        Task RemoveAsync(string id);
    }
}