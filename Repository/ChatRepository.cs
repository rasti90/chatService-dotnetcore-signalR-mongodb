using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Model;
using ChatServer.Model.Enum;
using ChatServer.Model.ViewModels;
using ChatServer.Repository.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ChatServer.Repository {
    public class ChatRepository : IChatRepository {
        private readonly IMongoCollection<Chat> _chats;
        private readonly IMongoCollection<User> _users;
        private readonly ILogger _logger;
        public ChatRepository (IDatabaseSettings settings, ILogger<ChatRepository> logger) {
            var client = new MongoClient (settings.ConnectionString);
            var database = client.GetDatabase (settings.DatabaseName);
            this._logger = logger;
            _chats = database.GetCollection<Chat> (settings.ChatsCollectionName);
            _users = database.GetCollection<User> (settings.UsersCollectionName);
        }

        public List<Chat> Get (string appId) {
            try {
                return _chats.Find<Chat> (chat => chat.AppId == appId).Project<Chat> (Builders<Chat>.Projection
                    .Exclude ("ChatConversations")).ToList ();
            } catch (Exception ex) {
                _logger.LogError (ex, "Get ChatRepository Exception");
                return null;
            }
        }

        public async Task<List<Chat>> GetAsync (string appId) {
            try {
                return await _chats.Find (chat => chat.AppId == appId).Project<Chat> (Builders<Chat>.Projection
                    .Exclude ("ChatConversations")).ToListAsync ();
            } catch (Exception ex) {
                _logger.LogError (ex, "GetAsync ChatRepository Exception");
                return null;
            }
        }

        public Chat Get (string appId, string chatId) {
            try {
                return _chats.Find<Chat> (chat => chat.AppId == appId && chat.Id == chatId).Project<Chat> (Builders<Chat>.Projection
                    .Slice ("ChatConversations", 0, 50)).FirstOrDefault ();
            } catch (Exception ex) {
                _logger.LogError (ex, "Get ChatRepository Exception");
                return null;
            }
        }

        public async Task<Chat> GetAsync (string appId, string chatId) {
            try {
                // await _chats.Find<Chat>(chat => chat.Id == id).Project<Chat>(Builders<Chat>.Projection
                //     .Exclude("chatConversations")).FirstOrDefaultAsync();
                return await _chats.Find (chat => chat.AppId == appId && chat.Id == chatId).Project<Chat> (Builders<Chat>.Projection
                    .Slice ("ChatConversations", 0, 50)).FirstOrDefaultAsync ();
            } catch (Exception ex) {
                _logger.LogError (ex, "GetAsync ChatRepository Exception");
                return null;
            }
        }

        public async Task<List<Chat>> GetByUserIdAsync (string appId, string userId) {
            try {
                List<Chat> result = new List<Chat> ();
                await _chats.Find<Chat> (chat => chat.AppId == appId && chat.ChatMembers.Any (member => member.UserId == userId))
                    //.Project<Chat>(Builders<Chat>.Projection.Slice("chatConversations",0,50)).ToListAsync();
                    .ForEachAsync (userchat => result.Add (new Chat () {
                        Id = userchat.Id,
                            AppId = userchat.AppId,
                            Name = userchat.Name,
                            ChatMembers = userchat.ChatMembers,
                            ChatType = userchat.ChatType,
                            ChatConversations = userchat.ChatConversations.Where (conversation => conversation.UserId != userId && !conversation.ChatConversationReaders.Any (u => u.UserId == userId)).ToList ()
                    }));
                return result;
            } catch (Exception ex) {
                _logger.LogError (ex, "GetByUserIdAsync ChatRepository Exception");
                return null;
            }
        }

        public List<ChatConversation> GetChatConversations (string app_Id, string chatId, int pageIndex, int pageSize) {
            try {
                var chat = _chats.Find<Chat> (c => c.AppId == app_Id && c.Id == chatId).Project<Chat> (Builders<Chat>.Projection
                    .Slice ("ChatConversations", (pageIndex * pageSize), pageSize)).FirstOrDefault ();
                var query = from conversation in chat.ChatConversations.AsQueryable ()
                join user in _users.AsQueryable () on
                conversation.UserId equals user.Id into ConversationUser
                orderby conversation.Date descending
                select new ChatConversation () {
                    Id = conversation.Id,
                    UserId = conversation.UserId,
                    Text = conversation.Text,
                    FileId = conversation.FileId,
                    Date = conversation.Date,
                    ParentConversationId = conversation.ParentConversationId,
                    ChatConversationReaders = conversation.ChatConversationReaders,
                    User = ConversationUser.FirstOrDefault ()
                };
                return query.Reverse ().ToList ();
            } catch (Exception ex) {
                _logger.LogError (ex, "GetChatConversations ChatRepository Exception");
                return null;
            }
        }

        public async Task<List<ChatConversation>> GetChatConversationsAsync (string app_Id, string chatId, int pageIndex, int pageSize) {
            try {
                var chat = await _chats.Find<Chat> (c => c.AppId == app_Id && c.Id == chatId).Project<Chat> (Builders<Chat>.Projection
                    .Slice ("ChatConversations", (pageIndex * pageSize), pageSize)).FirstOrDefaultAsync ();
                var query = from conversation in chat.ChatConversations.AsQueryable ()
                join user in _users.AsQueryable () on
                conversation.UserId equals user.Id into ConversationUser
                orderby conversation.Date descending
                select new ChatConversation () {
                    Id = conversation.Id,
                    UserId = conversation.UserId,
                    Text = conversation.Text,
                    FileId = conversation.FileId,
                    Date = conversation.Date,
                    ParentConversationId = conversation.ParentConversationId,
                    ChatConversationReaders = conversation.ChatConversationReaders,
                    User = ConversationUser.Select (conversationUser => new User () { Id = conversationUser.Id, FullName = conversationUser.FullName }).FirstOrDefault ()
                };
                return await Task.FromResult (query.Reverse ().ToList ());
                //return await Task.FromResult(query.Skip(pageIndex * pageSize).Take(pageSize).ToList());
            } catch (Exception ex) {
                _logger.LogError (ex, "GetChatConversationsAsync ChatRepository Exception");
                return null;
            }
        }

        public async Task<List<ChatConversation>> GetChatConversationsAsync (ChatHistoryFilterModel filter) {
            try {
                /* TO DO 
                    optimize the query with aggregation framework*/
                var chat = await _chats.Find (chat => chat.Id == filter.ChatId).FirstOrDefaultAsync ();
                if (filter.DirectionType == KeysetFilterModelType.Next)
                    chat.ChatConversations = chat.ChatConversations
                    .Where (conversation => conversation.Date.CompareTo (filter.EdgeDateTime) > 0)
                    .OrderBy (o => o.Date).Take (filter.Limit).ToList ();
                else
                    chat.ChatConversations = chat.ChatConversations
                    .Where (conversation => conversation.Date.CompareTo (filter.EdgeDateTime) < 0)
                    .OrderByDescending (o => o.Date).Take (filter.Limit).Reverse ().ToList ();
                var query = from conversation in chat.ChatConversations.AsQueryable ()
                join user in _users.AsQueryable () on
                conversation.UserId equals user.Id into ConversationUser
                select new ChatConversation () {
                    Id = conversation.Id,
                    UserId = conversation.UserId,
                    Text = conversation.Text,
                    FileId = conversation.FileId,
                    Date = conversation.Date,
                    ParentConversationId = conversation.ParentConversationId,
                    ChatConversationReaders = conversation.ChatConversationReaders,
                    User = ConversationUser.Select (conversationUser => new User () { Id = conversationUser.Id, FullName = conversationUser.FullName }).FirstOrDefault ()
                };
                return await Task.FromResult (query.ToList ());
                //return await Task.FromResult(query.Skip(pageIndex * pageSize).Take(pageSize).ToList());
            } catch (Exception ex) {
                _logger.LogError (ex, "GetChatConversationsAsync ChatRepository Exception");
                return null;
            }
        }

        public async Task<List<ChatConversation>> GetUnReadChatConversationsAsync (string app_Id, string chatId, string userId) {
            try {
                var chat = await _chats.Find<Chat> (c => c.AppId == app_Id && c.Id == chatId)
                    //.Project<Chat>(Builders<Chat>.Projection
                    //.Slice(c => c.ChatConversations.Find(conv => conv.UserId!=userId && !conv.ChatConversationReaders.Any(reader => reader.UserId == userId)),0)).FirstOrDefaultAsync();
                    .FirstOrDefaultAsync ();
                var query = from conversation in chat.ChatConversations.AsQueryable ()
                where conversation.UserId != userId && !conversation.ChatConversationReaders.Any (reader => reader.UserId == userId)
                join user in _users.AsQueryable () on
                conversation.UserId equals user.Id into ConversationUser
                orderby conversation.Date descending
                select new ChatConversation () {
                    Id = conversation.Id,
                    UserId = conversation.UserId,
                    Text = conversation.Text,
                    FileId = conversation.FileId,
                    Date = conversation.Date,
                    ParentConversationId = conversation.ParentConversationId,
                    ChatConversationReaders = conversation.ChatConversationReaders,
                    User = ConversationUser.Select (conversationUser => new User () { Id = conversationUser.Id, FullName = conversationUser.FullName }).FirstOrDefault ()
                };
                return await Task.FromResult (query.Reverse ().ToList ());
            } catch (Exception ex) {
                _logger.LogError (ex, "GetUnReadChatConversationsAsync ChatRepository Exception");
                return null;
            }
        }

        public async Task<List<ChatMember>> GetChatMembersAsync (string appId, string chatId) {
            try {
                var chat = await _chats.Find<Chat> (c => c.AppId == appId && c.Id == chatId).FirstOrDefaultAsync ();
                var query = from member in chat.ChatMembers.AsQueryable ()
                join user in _users.AsQueryable () on
                member.UserId equals user.Id into MemberUser
                select new ChatMember () {
                    UserId = member.UserId,
                    User = MemberUser.Select (memberUser => new User () { Id = memberUser.Id, FullName = memberUser.FullName }).FirstOrDefault ()
                };
                return await Task.FromResult (query.ToList ());
            } catch (Exception ex) {
                _logger.LogError (ex, "GetChatMembersAsync ChatRepository Exception");
                return null;
            }
        }

        public Chat Create (Chat chat) {
            try {
                _chats.InsertOne (chat);
                return chat;
            } catch (Exception ex) {
                _logger.LogError (ex, "Create ChatRepository Exception");
                return null;
            }
        }

        public async Task<Chat> CreateAsync (Chat chat) {
            try {
                await _chats.InsertOneAsync (chat);
                return chat;
            } catch (Exception ex) {
                _logger.LogError (ex, "CreateAsync ChatRepository Exception");
                return null;
            }
        }

        public void Update (string id, Chat chatIn) {
            try {
                _chats.ReplaceOne (chat => chat.Id == id, chatIn);
            } catch (Exception ex) {
                _logger.LogError (ex, "Update ChatRepository Exception");
            }
        }

        public async Task UpdateAsync (string id, Chat chatIn) {
            try {
                await _chats.ReplaceOneAsync (chat => chat.Id == id, chatIn);
            } catch (Exception ex) {
                _logger.LogError (ex, "UpdateAsync ChatRepository Exception");
            }
        }

        public void AddMessageToChat (string chatId, ChatConversation conversation) {
            try {
                _chats.FindOneAndUpdate (Builders<Chat>.Filter.Eq ("Id", chatId), Builders<Chat>.Update.Push ("ChatConversations", conversation));
            } catch (Exception ex) {
                _logger.LogError (ex, "AddMessageToChat ChatRepository Exception");
            }
        }

        public async Task AddMessageToChatAsync (string chatId, ChatConversation conversation) {
            try {
                await _chats.FindOneAndUpdateAsync (Builders<Chat>.Filter.Eq ("Id", chatId), Builders<Chat>.Update.Push ("ChatConversations", conversation));
            } catch (Exception ex) {
                _logger.LogError (ex, "AddMessageToChatAsync ChatRepository Exception");
            }
        }

        public async Task AddReaderToConversationAsync (string chatId, string conversationId, ChatConversationReader conversationReader) {
            try {
                var filter = Builders<Chat>.Filter.And (
                    Builders<Chat>.Filter.Where (chat => chat.Id == chatId),
                    Builders<Chat>.Filter.Eq ("ChatConversations.Id", conversationId));
                var update = Builders<Chat>.Update.Push ("ChatConversations.$.ChatConversationReaders", conversationReader);
                await _chats.FindOneAndUpdateAsync (filter, update);
            } catch (Exception ex) {
                _logger.LogError (ex, "AddReaderToConversationAsync ChatRepository Exception");
            }
        }

        public void Remove (Chat chatIn) {
            try {
                _chats.DeleteOne (chat => chat.Id == chatIn.Id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove ChatRepository Exception");
            }
        }

        public async Task RemoveAsync (Chat chatIn) {
            try {
                await _chats.DeleteOneAsync (chat => chat.Id == chatIn.Id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove ChatRepository Exception");
            }
        }

        public void Remove (string id) {
            try {
                _chats.DeleteOne (chat => chat.Id == id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove ChatRepository Exception");
            }
        }

        public async Task RemoveAsync (string id) {
            try {
                await _chats.DeleteOneAsync (chat => chat.Id == id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove ChatRepository Exception");
            }
        }

    }
}