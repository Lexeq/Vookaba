using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vookaba.Services.DTO;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Vookaba.Services.Abstractions;

namespace Vookaba.Utils
{
    public class TripcodePostProcessor : IPostProcessor
    {
        private const string tripcodeMark = "##";

        private readonly byte[] _salt;
        private readonly IHttpContextAccessor _contextAccessor;

        private static byte[] ComputeHash(string password, byte[] salt, int count)
        {
            return KeyDerivation.Pbkdf2(
                   password: password,
                   salt: salt,
                   prf: KeyDerivationPrf.HMACSHA1,
                   iterationCount: 100,
                   numBytesRequested: count);
        }

        public TripcodePostProcessor(IHttpContextAccessor contextAccessor, IOptions<DataProtectionOptions> protector, ILogger<TripcodePostProcessor> logger)
        {
            if (protector.Value?.ApplicationDiscriminator == null)
            {
                logger.LogCritical("ApplicationDiscriminator is null.");
                throw new ArgumentException(nameof(protector), "ApplicationDiscriminator is not set.");
            }
            _contextAccessor = contextAccessor;
            _salt = ComputeHash(protector.Value.ApplicationDiscriminator, Array.Empty<byte>(), 32);
        }

        public Task ProcessAsync(PostCreationDto post)
        {
            if (string.IsNullOrEmpty(post.AuthorName))
            {
                return Task.CompletedTask;
            }

            var tripSeparatorIndex = post.AuthorName?.IndexOf(tripcodeMark, StringComparison.Ordinal);
            if (!tripSeparatorIndex.HasValue || tripSeparatorIndex < 0)
            {
                return Task.CompletedTask;
            }
            string tripKey;

            //tripcode based on authorToken
            if (tripSeparatorIndex + tripcodeMark.Length == post.AuthorName.Length)
            {
                tripKey = _contextAccessor.HttpContext.User.FindFirstValue(Common.ApplicationConstants.ClaimTypes.AuthorToken);
            }
            //tripcode based on password
            else
            {
                tripKey = post.AuthorName[(tripSeparatorIndex.Value + tripcodeMark.Length)..];
            }
            post.AuthorName = post.AuthorName[0..tripSeparatorIndex.Value];

            string hashed = Convert.ToBase64String(ComputeHash(tripKey, _salt, 10))[..10];
            post.Tripcode = hashed;
            return Task.CompletedTask;
        }
    }
}
