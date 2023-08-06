namespace Vookaba.Services.Abstractions
{
    public interface IHashService
    {
        public byte[] ComputeHash(Stream stream);

        public byte[] ComputeHash(byte[] data);
    }
}