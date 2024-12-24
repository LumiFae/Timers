using System.ComponentModel;
using System.Drawing;
using Exiled.API.Interfaces;

namespace Timers
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("The color of the NTF timer when NTF are spawning.")]
        public Color NtfSpawnColor = Color.Blue;
        [Description("The color of the Chaos timer when Chaos are spawning.")]
        public Color ChaosSpawnColor = Color.Green;
    }
}