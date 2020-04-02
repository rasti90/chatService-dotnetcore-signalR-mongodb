using MongoDB.Driver;
using ChatServer.Model;
using ChatServer.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using ChatServer.Repository.Contract;

namespace ChatServer.Repository
{
    public class ChatRepository :IChatRepository
    {
        private readonly IMongoCollection<Chat> _chats;
        private readonly IMongoCollection<User> _users;
        public ChatRepository(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _chats = database.GetCollection<Chat>(settings.ChatsCollectionName);
            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public List<Chat> Get(string appId) =>
            _chats.Find<Chat>(chat => chat.AppId == appId).Project<Chat>(Builders<Chat>.Projection
                .Exclude("ChatConversations")).ToList();

        public async Task<List<Chat>> GetAsync(string appId) =>
            await _chats.Find(chat =>chat.AppId == appId).Project<Chat>(Builders<Chat>.Projection
                .Exclude("ChatConversations")).ToListAsync();

        public Chat Get(string appId, string chatId) =>
            _chats.Find<Chat>(chat => chat.AppId == appId && chat.Id == chatId).Project<Chat>(Builders<Chat>.Projection
                .Slice("ChatConversations",0,50)).FirstOrDefault();

        public async  Task<Chat> GetAsync(string appId, string chatId) =>
            // await _chats.Find<Chat>(chat => chat.Id == id).Project<Chat>(Builders<Chat>.Projection
            //     .Exclude("chatConversations")).FirstOrDefaultAsync();
            await _chats.Find(chat => chat.AppId == appId && chat.Id == chatId).Project<Chat>(Builders<Chat>.Projection
                .Slice("ChatConversations",0,50)).FirstOrDefaultAsync();

        public async Task<List<Chat>> GetByUserIdAsync(string appId, string userId)
        {
            try{
            List<Chat> result = new List<Chat>();
            await _chats.Find<Chat>(chat => chat.AppId == appId && chat.ChatMembers.Any(member => member.UserId == userId))
                //.Project<Chat>(Builders<Chat>.Projection.Slice("chatConversations",0,50)).ToListAsync();
                .ForEachAsync(userchat => result.Add(new Chat()
                    {
                        Id = userchat.Id,
                        AppId = userchat.AppId,
                        Name = userchat.Name,
                        ChatMembers = userchat.ChatMembers,
                        ChatType = userchat.ChatType,
                        ChatConversations = userchat.ChatConversations.Where(conversation => conversation.UserId != userId && !conversation.ChatConversationReaders.Any(u => u.UserId == userId)).ToList()
                    }
                ));
            return result;
            }catch(Exception ex){
                return null;
            }
        }

        public List<ChatConversation> GetChatConversations(string app_Id, string chatId, int pageIndex, int pageSize)
        {
            var chat = _chats.Find<Chat>(c => c.AppId==app_Id && c.Id == chatId).Project<Chat>(Builders<Chat>.Projection
                .Slice("ChatConversations",(pageIndex * pageSize),pageSize)).FirstOrDefault();
            var query = from conversation in chat.ChatConversations.AsQueryable()
                        join user in _users.AsQueryable() on
                            conversation.UserId equals user.Id into ConversationUser
                        orderby conversation.Date descending
                        select new ChatConversation()
                        {
                            Id = conversation.Id,
                            UserId = conversation.UserId,
                            Text = conversation.Text,
                            FileId = conversation.FileId,
                            Date = conversation.Date,
                            ParentConversationId = conversation.ParentConversationId,
                            ChatConversationReaders = conversation.ChatConversationReaders,
                            User = ConversationUser.FirstOrDefault()
                        };
            return query.Reverse().ToList();
        }

        public async Task<List<ChatConversation>> GetChatConversationsAsync(string app_Id, string chatId, int pageIndex, int pageSize)
        {
            try
            {
                var chat = await _chats.Find<Chat>(c => c.AppId==app_Id && c.Id == chatId).Project<Chat>(Builders<Chat>.Projection
                    .Slice("ChatConversations",(pageIndex * pageSize),pageSize)).FirstOrDefaultAsync();
                var query = from conversation in chat.ChatConversations.AsQueryable()
                            join user in _users.AsQueryable() on
                                conversation.UserId equals user.Id into ConversationUser
                            orderby conversation.Date descending
                            select new ChatConversation()
                            {
                                Id = conversation.Id,
                                UserId = conversation.UserId,
                                Text = conversation.Text,
                                FileId = conversation.FileId,
                                Date = conversation.Date,
                                ParentConversationId = conversation.ParentConversationId,
                                ChatConversationReaders = conversation.ChatConversationReaders,
                                User = ConversationUser.Select(conversationUser => new User() { Id = conversationUser.Id, FullName=conversationUser.FullName}).FirstOrDefault()
                            };
                return await Task.FromResult(query.Reverse().ToList());
                //return await Task.FromResult(query.Skip(pageIndex * pageSize).Take(pageSize).ToList());
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ChatConversation>> GetUnReadChatConversationsAsync(string app_Id, string chatId, string userId)
        {
            try{
            var chat = await _chats.Find<Chat>(c => c.AppId==app_Id && c.Id == chatId)
                //.Project<Chat>(Builders<Chat>.Projection
                //.Slice(c => c.ChatConversations.Find(conv => conv.UserId!=userId && !conv.ChatConversationReaders.Any(reader => reader.UserId == userId)),0)).FirstOrDefaultAsync();
                .FirstOrDefaultAsync();
            var query = from conversation in chat.ChatConversations.AsQueryable()
                        where conversation.UserId!=userId && !conversation.ChatConversationReaders.Any(reader => reader.UserId == userId)
                        join user in _users.AsQueryable() on
                            conversation.UserId equals user.Id into ConversationUser
                        orderby conversation.Date descending
                        select new ChatConversation()
                        {
                            Id = conversation.Id,
                            UserId = conversation.UserId,
                            Text = conversation.Text,
                            FileId = conversation.FileId,
                            Date = conversation.Date,
                            ParentConversationId = conversation.ParentConversationId,
                            ChatConversationReaders = conversation.ChatConversationReaders,
                            User = ConversationUser.Select(conversationUser => new User() { Id = conversationUser.Id, FullName=conversationUser.FullName }).FirstOrDefault()
                        };
            return await Task.FromResult(query.Reverse().ToList());
            }catch(Exception ex){
                return null;
            }
        }

        public async Task<List<ChatMember>> GetChatMembersAsync(string appId, string chatId)
        {
            var chat = await _chats.Find<Chat>(c => c.AppId==appId && c.Id == chatId).FirstOrDefaultAsync();
            var query = from member in chat.ChatMembers.AsQueryable()
                        join user in _users.AsQueryable() on
                            member.UserId equals user.Id into MemberUser
                        select new ChatMember()
                        {
                            UserId = member.UserId,
                            User = MemberUser.Select(memberUser => new User() { Id = memberUser.Id, FullName=memberUser.FullName}).FirstOrDefault()
                        };
            return await Task.FromResult(query.ToList());
        }

        public Chat Create(Chat chat)
        {
            _chats.InsertOne(chat);
            return chat;
        }

        public async Task<Chat> CreateAsync(Chat chat)
        {
            await _chats.InsertOneAsync(chat);
            return chat;
        }

        public void Update(string id, Chat chatIn) =>
            _chats.ReplaceOne(chat => chat.Id == id, chatIn);

        public async Task UpdateAsync(string id, Chat chatIn) =>
            await _chats.ReplaceOneAsync(chat => chat.Id == id, chatIn);

        public void AddMessageToChat(string chatId, ChatConversation conversation) =>
            _chats.FindOneAndUpdate(Builders<Chat>.Filter.Eq("Id", chatId), Builders<Chat>.Update.Push("ChatConversations", conversation));

        public async Task AddMessageToChatAsync(string chatId, ChatConversation conversation) =>
            await _chats.FindOneAndUpdateAsync(Builders<Chat>.Filter.Eq("Id", chatId), Builders<Chat>.Update.Push("ChatConversations", conversation));

        public async Task AddReaderToConversationAsync(string chatId, string conversationId, ChatConversationReader conversationReader)
        {
            var filter = Builders<Chat>.Filter.And(
                Builders<Chat>.Filter.Where(chat => chat.Id == chatId),
                Builders<Chat>.Filter.Eq("ChatConversations.Id", conversationId));
            var update = Builders<Chat>.Update.Push("ChatConversations.$.ChatConversationReaders", conversationReader);
            await _chats.FindOneAndUpdateAsync(filter, update);
        }

        public void Remove(Chat chatIn) =>
            _chats.DeleteOne(chat => chat.Id == chatIn.Id);

        public async Task RemoveAsync(Chat chatIn) =>
            await _chats.DeleteOneAsync(chat => chat.Id == chatIn.Id);

        public void Remove(string id) =>
            _chats.DeleteOne(chat => chat.Id == id);

        public async Task RemoveAsync(string id) =>
            await _chats.DeleteOneAsync(chat => chat.Id == id);

    }
}
