using System.Security.Cryptography;
using Vookaba.Services.Abstractions;

namespace Vookaba.Services
{
    public class HashService : IHashService
    {
        private HashAlgorithm CreateHasher()
        {
            return SHA1.Create();
        }
        public byte[] ComputeHash(Stream stream)
        {
            using var hasher = CreateHasher();
            return hasher.ComputeHash(stream);
        }

        public byte[] ComputeHash(byte[] data)
        {
            using var hasher = CreateHasher();
            return hasher.ComputeHash(data);
        }
    }

}