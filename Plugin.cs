using System.Drawing;
using System.Text;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.API.Features.Waves;
using HintServiceMeow.Core.Models.HintContent.HintContent;
using PlayerRoles;
using Respawning;
#if RUEI
using RueI;
using RueI.Displays;
using RueI.Elements;
using RueI.Extensions.HintBuilding;
using RueI.Parsing.Enums;
#endif
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
        
#if RUEI
        internal AutoElement RespawnTimerDisplay { private set; get; }
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
            RespawnTimerDisplay = new(Roles.Spectator, new DynamicElement(GetTimers, 910))
            {
                UpdateEvery = new (TimeSpan.FromSeconds(1))
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
        
        public static string ConvertToHex(Color color)
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
        internal string GetTimers(ReferenceHub player)
#endif
        {
#if HSM
            if(!Round.InProgress) 
                return "";
            if(Player.TryGet(player, out Player p) && p.Role.Type != RoleTypeId.Spectator)
            {
                return "";
            }
#endif
            
            TimeSpan ntfTime = NtfRespawnTime() + TimeSpan.FromSeconds(18);
            TimeSpan chaosTime = ChaosRespawnTime() + TimeSpan.FromSeconds(13);
            SSTwoButtonsSetting setting = ServerSpecificSettingsSync.GetSettingOfUser<SSTwoButtonsSetting>(
#if RUEI
                core.Hub,
#else
                player,
#endif
                Config.ServerSpecificSettingId
            );
            
            
            if(setting.SyncIsB) return "";

            StringBuilder builder = new StringBuilder()
#if RUEI
                .SetAlignment(HintBuilding.AlignStyle.Center);
#else
                .Append("<align=center>");
#endif

            if (WaveManager._nextWave != null && WaveManager._nextWave.TargetFaction == Faction.FoundationStaff)
            {
#if RUEI
                builder.SetColor(Config.NtfSpawnColor).Append(TimerText(ntfTime)).CloseColor();
#else
                builder.Append($"<color={ConvertToHex(Config.NtfSpawnColor)}>").Append(TimerText(ntfTime)).Append("</color>");
#endif
            }
            else
            {
                builder.Append(TimerText(ntfTime));
            }
            
            builder
#if RUEI
                .AddSpace(Config.SpaceBetweenTimers, MeasurementUnit.Ems);
#else
                .Append($"<space={Config.SpaceBetweenTimers}ems>");
#endif
            

            if (WaveManager._nextWave != null && WaveManager._nextWave.TargetFaction == Faction.FoundationEnemy)
            {
#if RUEI
                builder.SetColor(Config.ChaosSpawnColor).Append(TimerText(chaosTime)).CloseColor();
#else
                builder.Append($"<color={ConvertToHex(Config.ChaosSpawnColor)}>").Append(TimerText(chaosTime)).Append("</color>");
#endif
            }
            else
            {
                builder.Append(TimerText(chaosTime));
            }
            
#if HSM
            builder.Append("</align>");
#endif
            
            return builder.ToString();
        }
    }
}