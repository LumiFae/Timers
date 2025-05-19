#if HSM
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Models.Hints;
#else
using RueI;
using RueI.Displays;
using RueI.Elements;
#endif
using System.Drawing;
using System.Text;
using Exiled.API.Features.Waves;
using PlayerRoles;
using Respawning;
using UserSettings.ServerSpecific;

namespace Timers
{
    public class HintManager
    {
        public static HintManager Instance { get; set; }

        private static Config Config => Plugin.Instance.Config;
        private static Translation Translation => Plugin.Instance.Translation;

        public void Initialise()
        {
            Instance = this;

#if RUEI
            RueIMain.EnsureInit();

            RespawnTimerDisplay = new(Roles.Spectator | Roles.Overwatch, new DynamicElement(core => GetTimers(core.Hub), 910))
            {
                UpdateEvery = new(TimeSpan.FromSeconds(1))
            };
#else
            RespawnTimerDisplay = new()
            {
                AutoText = arg => GetTimers(arg.PlayerDisplay.ReferenceHub),
                TargetY = 105,
                FontSize = 35,
                SyncSpeed = HintSyncSpeed.Fast
            };
#endif
        }

        internal TimedWave NtfWave;
        internal TimedWave NtfMiniWave;
        internal TimedWave ChaosWave;
        internal TimedWave ChaosMiniWave;

#if RUEI
        internal AutoElement RespawnTimerDisplay { private set; get; }
#else
        internal DynamicHint RespawnTimerDisplay { private set; get; }
#endif

        private TimeSpan NtfRespawnTime()
        {
            if (NtfMiniWave != null && !NtfMiniWave.Timer.IsPaused) return NtfMiniWave.Timer.TimeLeft;

            return NtfWave != null ? NtfWave.Timer.TimeLeft : TimeSpan.Zero;
        }

        private TimeSpan ChaosRespawnTime()
        {
            if (ChaosMiniWave != null && !ChaosMiniWave.Timer.IsPaused) return ChaosMiniWave.Timer.TimeLeft;

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

        private string GetTimers(ReferenceHub hub)
        {
            TimeSpan ntfTime = NtfRespawnTime() + TimeSpan.FromSeconds(18);
            if (ntfTime < TimeSpan.Zero) ntfTime = TimeSpan.Zero;
            TimeSpan chaosTime = ChaosRespawnTime() + TimeSpan.FromSeconds(13);
            if (chaosTime < TimeSpan.Zero) chaosTime = TimeSpan.Zero;

            SSTwoButtonsSetting setting;
            try
            {
                setting = ServerSpecificSettingsSync.GetSettingOfUser<SSTwoButtonsSetting>(hub, Config.ServerSpecificSettingId);
            }
            catch (NullReferenceException)
            {
                return string.Empty;
            }

            if (setting.SyncIsB) return string.Empty;

            StringBuilder builder = new StringBuilder()
                .Append("<align=center>");

            if (WaveManager._nextWave != null
                && WaveManager._nextWave.TargetFaction == Faction.FoundationStaff
                && ntfTime.TotalSeconds <= 18)
                builder.Append($"<color={ConvertToHex(Config.NtfSpawnColor)}>").Append(TimerText(ntfTime)).Append("</color>");
            else
                builder.Append(TimerText(ntfTime));

            builder
                .Append($"<space={Config.SpaceBetweenTimers}ems>");

            if (WaveManager._nextWave != null
                && WaveManager._nextWave.TargetFaction == Faction.FoundationEnemy
                && chaosTime.TotalSeconds <= 13)
                builder.Append($"<color={ConvertToHex(Config.ChaosSpawnColor)}>").Append(TimerText(chaosTime)).Append("</color>");
            else
                builder.Append(TimerText(chaosTime));

            builder.Append("</align>");

            return builder.ToString();
        }
    }
}