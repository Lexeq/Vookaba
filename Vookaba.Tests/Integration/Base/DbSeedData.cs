using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vookaba.DAL.Entities;
using Vookaba.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

namespace Vookaba.Tests.Integration.Base
{
    public class DbSeedData
    {
        private readonly List<AuthorToken> _tokens = new();
        private readonly List<Thread> _threads = new();
        private readonly List<Board> _boards = new();
        private readonly List<Image> _images = new();
        private readonly List<Post> _posts = new();
        private readonly List<ApplicationUser> _users = new();
        private readonly List<Ban> _bans = new();

        internal IEnumerable<AuthorToken> Tokens => _tokens;
        internal IEnumerable<Thread> Threads => _threads;
        internal IEnumerable<Board> Boards => _boards;
        internal IEnumerable<Image> Images => _images;
        internal IEnumerable<Post> Posts => _posts;
        internal IEnumerable<ApplicationUser> Users => _users;
        internal IEnumerable<Ban> Bans => _bans;

        internal AuthorToken DefaultToken { get; }
            = new AuthorToken
            {
                Token = new Guid("11111111-1111-1111-1111-111111111111"),
                Created = DateTime.SpecifyKind(DateTime.Parse("2000-01-01"), DateTimeKind.Utc)
            };

        internal Board DefaultBoard { get; }
            = new Board { Key = "b", BumpLimit = 100, Name = "b" };

        public DbSeedData AddDefaults()
        {
            AddTokens(DefaultToken);
            AddBoards(DefaultBoard);
            return this;
        }

        public DbSeedData FromResource(string resourceName)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Vookaba.Tests.Integration.SeedData.{resourceName.ToLowerInvariant()}.json");
            using var reader = new StreamReader(stream);
            return FromJson(reader.ReadToEnd());
        }

        public DbSeedData FromJson(string json)
        {
            JsonSerializerSettings settings = new();
            settings.Converters.Add(new IPAddressConverter());
            JsonSerializer serializer = JsonSerializer.Create(settings);
            var jObj = JObject.Parse(json);

            var tokens = jObj["tokens"]?.ToObject<AuthorToken[]>(serializer);
            if (tokens != null)
                AddTokens(tokens);

            var boards = jObj["boards"]?.ToObject<Board[]>();
            if (boards != null)
                AddBoards(boards);

            var threads = jObj["threads"]?.ToObject<Thread[]>();
            if (threads != null)
                AddThreads(threads);

            var images = jObj["images"]?.ToObject<Image[]>();
            if (images != null)
                AddImages(images);

            var posts = jObj["posts"]?.ToObject<Post[]>(serializer);
            if (posts != null)
                AddPosts(posts);

            var bans = jObj["bans"]?.ToObject<Ban[]>(serializer);
            if (bans != null)
                AddBans(bans);

            var users = jObj["users"]?.ToObject<ApplicationUser[]>(serializer);
            if (users != null)
                AddUsers(users);

            return this;
        }

        public DbSeedData AddTokens(params AuthorToken[] tokens)
        {
            _tokens.AddRange(tokens);
            return this;
        }

        public DbSeedData AddBoards(params Board[] boards)
        {
            _boards.AddRange(boards);
            return this;
        }

        public DbSeedData AddThreads(params Thread[] threads)
        {
            _threads.AddRange(threads);
            return this;
        }

        public DbSeedData AddPosts(params Post[] posts)
        {
            _posts.AddRange(posts);
            return this;
        }

        public DbSeedData AddImages(params Image[] images)
        {
            _images.AddRange(images);
            return this;
        }

        public DbSeedData AddBans(params Ban[] bans)
        {
            _bans.AddRange(bans);
            return this;
        }

        public DbSeedData AddUsers(params ApplicationUser[] users)
        {
            _users.AddRange(users);
            return this;
        }

        class IPAddressConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(IPAddress));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(value.ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return IPAddress.Parse((string)reader.Value);
            }
        }
    }
}
