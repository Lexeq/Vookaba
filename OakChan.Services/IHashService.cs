using System.IO;

namespace OakChan.Services
{
    public interface IHashService
    {
        public byte[] ComputeHash(Stream stream);

        public byte[] ComputeHash(byte[] data);
    }
}