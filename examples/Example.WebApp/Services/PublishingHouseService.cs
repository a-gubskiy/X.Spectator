namespace Example.WebApp.Services;

public class PublishingHouseService
{
    /// <summary>
    /// Publish new books
    /// </summary>
    /// <returns></returns>
    public int PublishBooks() => new Random((int) DateTime.Now.Ticks).Next(0, 10);
}