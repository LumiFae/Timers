using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
#if RUEI
#else
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Enum;
#endif

namespace Timers
{
    public class Plugin : Plugin<Config, Translation>
    {

        private Events _events;
        private HintManager _hintManager;
        public override string Name => "Timers";
        public override string Author => "LumiFae. Modified by InfernalBreach Team";
        public override string Prefix => "Timers";
        public override Version Version => new(1, 3, 3);
        public override Version RequiredExiledVersion => new(9, 2, 1);
        public override PluginPriority Priority => PluginPriority.Default;

        public static Plugin Instance { private set; get; }

        public override void OnEnabled()
        {
            Instance = this;

            _events = new();
            _hintManager = new();
            _hintManager.Initialise();

            Server.RoundStarted += _events.OnRoundStart;
            Player.Verified += _events.OnPlayerVerified;
#if HSM
            Exiled.Events.Handlers.Player.ChangingRole += _events.OnPlayerChangingRole;
            Exiled.Events.Handlers.Player.Left += _events.OnLeft;
#endif

            HeaderSetting header = new(Translation.ServerSpecificSettingHeading);
            IEnumerable<SettingBase> settings = new SettingBase[]
            {
                header,
                new TwoButtonsSetting(Config.ServerSpecificSettingId, Translation.OverlaySettingText,
                    Translation.Enable, Translation.Disable, hintDescription: Translation.OverlaySettingHint)
            };

            SettingBase.Register(settings);
            SettingBase.SendToAll();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Server.RoundStarted -= _events.OnRoundStart;
            Player.Verified -= _events.OnPlayerVerified;
#if HSM
            Exiled.Events.Handlers.Player.ChangingRole -= _events.OnPlayerChangingRole;
            Exiled.Events.Handlers.Player.Left -= _events.OnLeft;
#endif
            _events = null;
            _hintManager = null;
            base.OnDisabled();
        }
    }
}