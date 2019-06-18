namespace X.Monitor.Base
{
    public interface IProbe
    {
        string Name { get; }
        bool GetState();
    }

}