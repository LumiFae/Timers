using System.Text;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.API.Features.Waves;
using PlayerRoles;
using Respawning;
using RueI;
using RueI.Displays;
using RueI.Elements;
using RueI.Extensions.HintBuilding;
using RueI.Parsing.Enums;
using UserSettings.ServerSpecific;

namespace Timers
{
    public class Plugin: Plugin<Config, Translation>
    {
        public override string Name => "Timers";
        public override string Author => "LumiFae";
        public override string Prefix => "Timers";
        public override Version Version => new (1, 1, 0);
        public override Version RequiredExiledVersion => new (9, 0, 0);
        public override PluginPriority Priority => PluginPriority.Default;
        
        public static Plugin Instance { private set; get; }

        private Events _events;

        internal AutoElement RespawnTimerDisplay { private set; get; }

        internal TimedWave NtfWave;
        internal TimedWave NtfMiniWave;
        internal TimedWave ChaosWave;
        internal TimedWave ChaosMiniWave;
        
        public override void OnEnabled()
        {
            RueIMain.EnsureInit();
            Instance = this;

            _events = new();

            Exiled.Events.Handlers.Server.RoundStarted += _events.OnRoundStart;
            Exiled.Events.Handlers.Player.Verified += _events.OnPlayerVerified;
            
            RespawnTimerDisplay = new(Roles.Spectator, new DynamicElement(GetTimers, 910))
            {
                UpdateEvery = new (TimeSpan.FromSeconds(1))
            };

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
            Exiled.Events.Handlers.Server.RoundStarted -= _events.OnRoundStart;
            Exiled.Events.Handlers.Player.Verified -= _events.OnPlayerVerified;
            _events = null;
            base.OnDisabled();
        }

        private TimeSpan NtfRespawnTime()
        {
            if (NtfMiniWave != null && !NtfMiniWave.Timer.IsPaused)
            {
                return NtfMiniWave.Timer.TimeLeft;
            }
            return NtfWave != null ? NtfWave.Timer.TimeLeft : TimeSpan.Zero;
        }
        
        private TimeSpan ChaosRespawnTime()
        {
            if (ChaosMiniWave != null && !ChaosMiniWave.Timer.IsPaused)
            {
                return ChaosMiniWave.Timer.TimeLeft;
            }
            return ChaosWave != null ? ChaosWave.Timer.TimeLeft : TimeSpan.Zero;
        }

        private string TimerText(TimeSpan timer)
        {
            return Translation.Timer.Replace("{minutes}", timer.Minutes.ToString("D1"))
                .Replace("{seconds}", timer.Seconds.ToString("D2"));
        }
        
        private string GetTimers(DisplayCore core)
        {
            TimeSpan ntfTime = NtfRespawnTime() + TimeSpan.FromSeconds(18);
            TimeSpan chaosTime = ChaosRespawnTime() + TimeSpan.FromSeconds(13);

            SSTwoButtonsSetting setting = ServerSpecificSettingsSync.GetSettingOfUser<SSTwoButtonsSetting>(core.Hub, Config.ServerSpecificSettingId);
            
            if(setting.SyncIsB) return "";

            StringBuilder builder = new StringBuilder()
                .SetAlignment(HintBuilding.AlignStyle.Center);

            if (WaveManager._nextWave != null && WaveManager._nextWave.TargetFaction == Faction.FoundationStaff)
            {
                builder.SetColor(Config.NtfSpawnColor).Append(TimerText(ntfTime)).CloseColor();
            }
            else
            {
                builder.Append(TimerText(ntfTime));
            }
            
            builder
                .AddSpace(Config.SpaceBetweenTimers, MeasurementUnit.Ems);

            if (WaveManager._nextWave != null && WaveManager._nextWave.TargetFaction == Faction.FoundationEnemy)
            {
                builder.SetColor(Config.ChaosSpawnColor).Append(TimerText(chaosTime)).CloseColor();
            }
            else
            {
                builder.Append(TimerText(chaosTime));
            }
            
            return builder.ToString();
        }
    }
}