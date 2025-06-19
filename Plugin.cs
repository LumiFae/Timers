using LabApi.Events.Handlers;
using LabApi.Features.Console;
using UserSettings.ServerSpecific;
#if EXILED
using Exiled.API.Features;
#elif LabAPI
using LabApi.Loader;
#endif

namespace Timers
{
    public class Plugin : 
#if LabAPI
        LabApi.Loader.Features.Plugins.Plugin
#elif EXILED
        Plugin<Config, Translation>
#endif
    {

        private Events _events;
        private HintManager _hintManager;
        public override string Name => "Timers";
        public override string Author => "LumiFae";
        public override Version Version => new(1, 3, 3);
        
#if EXILED
        public override Version RequiredExiledVersion => new(9, 2, 1);
        public override string Prefix => "Timers";
#elif LabAPI
        public override string Description { get; } = "Adds countdown timers to the respawn UI";
        public override Version RequiredApiVersion { get; } = new (1, 0, 2);
#endif

        public static Plugin Instance { private set; get; }
        
#if LabAPI
        public Translation Translation { get; private set; }
        public Config Config { get; private set; }
#endif

        #if EXILED
        public override void OnEnabled()
#elif LabAPI
        public override void Enable()
#endif
        {
#if LabAPI
            if (!Config.IsEnabled) return;
#endif
            Logger.Debug("Enabling plugin...", Config.Debug);
            Instance = this;

            _events = new();
            _hintManager = new();
            _hintManager.Initialise();

            ServerEvents.RoundStarted += _events.OnRoundStart;
            PlayerEvents.Joined += _events.OnPlayerVerified;
#if HSM
            PlayerEvents.ChangingRole += _events.OnPlayerChangingRole;
            PlayerEvents.Left += _events.OnLeft;
#endif
            
            Logger.Debug("Subscribed to events", Config.Debug);

            SSGroupHeader header = new(Translation.ServerSpecificSettingHeading);
            SSTwoButtonsSetting setting = new(Config.ServerSpecificSettingId, Translation.OverlaySettingText, Translation.Enable, Translation.Disable, hint:Translation.OverlaySettingHint);

            ServerSpecificSettingsSync.DefinedSettings = [..ServerSpecificSettingsSync.DefinedSettings, header, setting];
            
            Logger.Debug("Registered settings", Config.Debug);
                
            ServerSpecificSettingsSync.SendToAll();
            
            Logger.Debug("Sending to all players...", Config.Debug);

#if EXILED
            base.OnEnabled();
#endif
        }

#if LabAPI
        public override void Disable()
#elif EXILED
        public override void OnDisabled()
#endif
        {
            ServerEvents.RoundStarted -= _events.OnRoundStart;
            PlayerEvents.Joined -= _events.OnPlayerVerified;
#if HSM
            PlayerEvents.ChangingRole -= _events.OnPlayerChangingRole;
            PlayerEvents.Left -= _events.OnLeft;
#endif
            _events = null;
            _hintManager = null;
#if EXILED
            base.OnDisabled();
#endif
        }
        
#if LabAPI
        public override void LoadConfigs()
        {
            this.TryLoadConfig("config.yml", out Config config);
            Config = config ?? new Config();
            this.TryLoadConfig("translation.yml", out Translation translation);
            Translation = translation ?? new Translation();
        }
#endif
    }
}