using Microsoft.AspNetCore.Http;

namespace SportMarket.Services
{
    public interface IDataService
    {
        public byte[] ImageToByte(IFormFile photo);
        public byte[] ImageToByte(string name);
    }
}