using Exiled.API.Interfaces;

namespace Timers
{
    public class Translation : ITranslation
    {
        public string Timer { get; set; } = "{minutes}<size=22>M</size> {seconds}<size=22>S</size>";
    }
}