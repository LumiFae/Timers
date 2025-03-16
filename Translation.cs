using Exiled.API.Interfaces;

namespace Timers
{
    public class Translation : ITranslation
    {
        public string Timer { get; set; } = "<b>{minutes}:{seconds}</b>";
        public string ServerSpecificSettingHeading { get; set; } = "Ajustes de temporizador de reaparición";
        public string OverlaySettingText { get; set; } = "Mostrar temporizadores de reaparición";
        public string Enable { get; set; } = "Habilitar";
        public string Disable { get; set; } = "Deshabilitar";
        public string OverlaySettingHint { get; set; } = "Habilitar o deshabilitar los temporizadores debajo de las barras de reaparición.";
    }
}