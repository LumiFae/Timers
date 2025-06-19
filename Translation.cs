#if EXILED
using Exiled.API.Interfaces;
#endif

namespace Timers
{
    public class Translation
#if EXILED
        : ITranslation
#endif
    {
        public string Timer { get; set; } = "{minutes}<size=22>M</size> {seconds}<size=22>S</size>";
        public string ServerSpecificSettingHeading { get; set; } = "Respawn Timer Settings";
        public string OverlaySettingText { get; set; } = "Show Respawn Timers";
        public string Enable { get; set; } = "Enable";
        public string Disable { get; set; } = "Disable";
        public string OverlaySettingHint { get; set; } =
            "Enables or disables the timers below the respawn bars, can't use the in-game setting for this due to the way it's implemented.";
    }
}