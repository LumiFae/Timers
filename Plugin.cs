using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using UserSettings.ServerSpecific;
using LabApi.Loader;

namespace Timers
{
    public class Plugin : LabApi.Loader.Features.Plugins.Plugin
    {

        private Events _events = null!;
        public override string Name => "Timers";
        public override string Author => "LumiFae";
        public override Version Version => new(1, 5, 0);
        
        public override string Description { get; } = "Adds countdown timers to the respawn UI";
        public override Version RequiredApiVersion { get; } = new (1, 0, 2);

        public static Plugin Instance { private set; get; } = null!;

        public Translation Translation { get; private set; } = null!;
        public Config Config { get; private set; } = null!;

        internal SSTwoButtonsSetting Setting { get; private set; } = null!;

        public override void Enable()
        {
            Logger.Debug("Enabling plugin...", Config.Debug);
            Instance = this;

            _events = new();
            CustomHandlersManager.RegisterEventsHandler(_events);

            ServerSpecificSettingsSync.ServerOnSettingValueReceived += Events.OnSettingUpdated;

            SSGroupHeader header = new(Translation.ServerSpecificSettingHeading);
            Setting = new SSTwoButtonsSetting(Config.ServerSpecificSettingId, Translation.OverlaySettingText, Translation.Enable, Translation.Disable, hint:Translation.OverlaySettingHint);

            ServerSpecificSettingsSync.DefinedSettings = [..ServerSpecificSettingsSync.DefinedSettings ?? [], header, Setting];
                
            ServerSpecificSettingsSync.SendToAll();
        }

        public override void Disable()
        {
            CustomHandlersManager.UnregisterEventsHandler(_events);   
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= Events.OnSettingUpdated;
            _events = null!;
        }
        
        public override void LoadConfigs()
        {
            this.TryLoadConfig("config.yml", out Config? config);
            Config = config ?? new Config();
            this.TryLoadConfig("translation.yml", out Translation? translation);
            Translation = translation ?? new Translation();
        }
    }
}