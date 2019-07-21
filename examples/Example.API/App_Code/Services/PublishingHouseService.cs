using System;

namespace Example.API.Services
{
    public interface IPublishingHouseService
    {
        int PublishBooks();
    }

    public class PublishingHouseService : IPublishingHouseService
    {
        public int PublishBooks() => new Random((int) DateTime.Now.Ticks).Next(0, 10);
    }
}
