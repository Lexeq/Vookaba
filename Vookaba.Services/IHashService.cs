using System.IO;

namespace Vookaba.Services
{
    public interface IHashService
    {
        public byte[] ComputeHash(Stream stream);

        public byte[] ComputeHash(byte[] data);
    }
}