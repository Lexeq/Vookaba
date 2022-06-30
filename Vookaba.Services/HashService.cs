using System.IO;
using System.Security.Cryptography;

namespace Vookaba.Services
{
    public  class HashService : IHashService
    {
        private const string AlgorithmName = "SHA1";

        public byte[] ComputeHash(Stream stream)
        {
            using var hasher = SHA1.Create(AlgorithmName);
            return hasher.ComputeHash(stream);
        }

        public byte[] ComputeHash(byte[] data)
        {
            using var hasher = SHA1.Create(AlgorithmName);
            return hasher.ComputeHash(data);
        }
    }

}