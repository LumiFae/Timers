using System.Drawing;
using System.Text;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using PlayerRoles;
using Respawning;
using RueI.API.Elements;

namespace Timers
{
    public static class HintManager
    {
        private static Config Config => Plugin.Instance.Config;
        private static Translation Translation => Plugin.Instance.Translation;

        private static MtfWave? NtfWave => RespawnWaves.PrimaryMtfWave;
        private static MiniMtfWave? NtfMiniWave => RespawnWaves.MiniMtfWave;
        private static ChaosWave? ChaosWave => RespawnWaves.PrimaryChaosWave;
        private static MiniChaosWave? ChaosMiniWave => RespawnWaves.MiniChaosWave;

        public static CachedElement Element { get; } = new(910, TimeSpan.FromSeconds(1), GetTimers)
        {
            UpdateInterval = TimeSpan.FromSeconds(1)
        };
        
        public static Tag Tag { get; } = new("Timers");

        private static TimeSpan NtfRespawnTime()
        {
            if (NtfMiniWave != null && !NtfMiniWave.Base.Timer.IsPaused) return TimeSpan.FromSeconds(NtfMiniWave.TimeLeft);

            return NtfWave != null ? TimeSpan.FromSeconds(NtfWave.TimeLeft) : TimeSpan.Zero;
        }

        private static TimeSpan ChaosRespawnTime()
        {
            if (ChaosMiniWave != null && !ChaosMiniWave.Base.Timer.IsPaused) return TimeSpan.FromSeconds(ChaosMiniWave.TimeLeft);

            return ChaosWave != null ? TimeSpan.FromSeconds(ChaosWave.TimeLeft) : TimeSpan.Zero;
        }

        private static string TimerText(TimeSpan timer)
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

        private static string GetTimers()
        {
            if (!Round.IsRoundInProgress)
                return string.Empty;
            
            Logger.Debug("Getting timers...", Config.Debug);
            TimeSpan ntfTime = NtfRespawnTime() + TimeSpan.FromSeconds(18);
            if (ntfTime < TimeSpan.Zero) ntfTime = TimeSpan.Zero;
            TimeSpan chaosTime = ChaosRespawnTime() + TimeSpan.FromSeconds(13);
            if (chaosTime < TimeSpan.Zero) chaosTime = TimeSpan.Zero;

            StringBuilder builder = StringBuilderPool.Shared.Rent()
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
            
            return StringBuilderPool.Shared.ToStringReturn(builder);
        }
    }
}