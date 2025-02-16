using System.Text;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.API.Features.Waves;
using PlayerRoles;
using Respawning;
#if RUEI
using RueI;
using RueI.Displays;
using RueI.Elements;
#else
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Enum;
#endif
using UserSettings.ServerSpecific;
using Color = System.Drawing.Color;

namespace Timers
{
    public class Plugin: Plugin<Config, Translation>
    {
        public override string Name => "Timers";
        public override string Author => "LumiFae";
        public override string Prefix => "Timers";
        public override Version Version => new (1, 3, 1);
        public override Version RequiredExiledVersion => new (9, 2, 1);
        public override PluginPriority Priority => PluginPriority.Default;
        
        public static Plugin Instance { private set; get; }

        private Events _events;
        
#if RUEI
        internal AutoElement RespawnTimerDisplay { private set; get; }
#else
        internal DynamicHint RespawnTimerDisplay { private set; get; }
#endif

        internal TimedWave NtfWave;
        internal TimedWave NtfMiniWave;
        internal TimedWave ChaosWave;
        internal TimedWave ChaosMiniWave;
        
        public override void OnEnabled()
        {
#if RUEI
            RueIMain.EnsureInit();
#endif
            Instance = this;

            _events = new();

            Exiled.Events.Handlers.Server.RoundStarted += _events.OnRoundStart;
            Exiled.Events.Handlers.Player.Verified += _events.OnPlayerVerified;
#if RUEI
            RespawnTimerDisplay = new (Roles.Spectator | Roles.Overwatch, new DynamicElement(GetTimers, 910))
            {
                UpdateEvery = new (TimeSpan.FromSeconds(1))
            };
#else
            Exiled.Events.Handlers.Player.ChangingRole += _events.OnPlayerChangingRole;
            RespawnTimerDisplay = new()
            {
                AutoText = GetTimers,
                TargetY = 105,
                FontSize = 35,
                SyncSpeed = HintSyncSpeed.Fast
            };
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
            Exiled.Events.Handlers.Server.RoundStarted -= _events.OnRoundStart;
            Exiled.Events.Handlers.Player.Verified -= _events.OnPlayerVerified;
#if HSM
            Exiled.Events.Handlers.Player.ChangingRole -= _events.OnPlayerChangingRole;
#endif
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
        
        private static string ConvertToHex(Color color)
        {
            string alphaInclude = color.A switch
            {
                255 => string.Empty,
                _ => color.A.ToString("X2")
            };

            return $"#{color.R:X2}{color.G:X2}{color.B:X2}{alphaInclude}";
        }
        
#if RUEI
        internal string GetTimers(DisplayCore core)
#else
        internal string GetTimers(AbstractHint.TextUpdateArg arg)
#endif
        {
            TimeSpan ntfTime = NtfRespawnTime() + TimeSpan.FromSeconds(18);
            if(ntfTime < TimeSpan.Zero) ntfTime = TimeSpan.Zero;
            TimeSpan chaosTime = ChaosRespawnTime() + TimeSpan.FromSeconds(13);
            if(chaosTime < TimeSpan.Zero) chaosTime = TimeSpan.Zero;
            SSTwoButtonsSetting setting = ServerSpecificSettingsSync.GetSettingOfUser<SSTwoButtonsSetting>(
#if RUEI
                core.Hub,
#else
                arg.Player,
#endif
                Config.ServerSpecificSettingId
            );
            
            
            if(setting.SyncIsB) return "";

            StringBuilder builder = new StringBuilder()
                .Append("<align=center>");

            if (WaveManager._nextWave != null && WaveManager._nextWave.TargetFaction == Faction.FoundationStaff)
            {
                builder.Append($"<color={ConvertToHex(Config.NtfSpawnColor)}>").Append(TimerText(ntfTime)).Append("</color>");
            }
            else
            {
                builder.Append(TimerText(ntfTime));
            }
            
            builder
                .Append($"<space={Config.SpaceBetweenTimers}ems>");
            
            if (WaveManager._nextWave != null && WaveManager._nextWave.TargetFaction == Faction.FoundationEnemy)
            {
                builder.Append($"<color={ConvertToHex(Config.ChaosSpawnColor)}>").Append(TimerText(chaosTime)).Append("</color>");
            }
            else
            {
                builder.Append(TimerText(chaosTime));
            }
            
            builder.Append("</align>");
            
            return builder.ToString();
        }
    }
}