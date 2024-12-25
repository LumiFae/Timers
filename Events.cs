using Exiled.API.Features.Waves;
using Exiled.Events.EventArgs.Player;
#if HSM
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Models.HintContent.HintContent;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
#endif
using Respawning.Waves;
using UserSettings.ServerSpecific;
using Log = Exiled.API.Features.Log;

namespace Timers
{
    public class Events
    {
        public void OnRoundStart()
        {
            if (TimedWave.TryGetTimedWave<NtfSpawnWave>(out TimedWave ntfWave))
            {
                Log.Debug("NtfWave found");
                Plugin.Instance.NtfWave = ntfWave;
            }
            if (TimedWave.TryGetTimedWave<NtfMiniWave>(out TimedWave ntfMiniWave))
            {
                Log.Debug("NtfMiniWave found");
                Plugin.Instance.NtfMiniWave = ntfMiniWave;
            }
            if (TimedWave.TryGetTimedWave<ChaosSpawnWave>(out TimedWave chaosWave))
            {
                Log.Debug("ChaosWave found");
                Plugin.Instance.ChaosWave = chaosWave;
            }
            if (TimedWave.TryGetTimedWave<ChaosMiniWave>(out TimedWave chaosMiniWave))
            {
                Log.Debug("ChaosMiniWave found");
                Plugin.Instance.ChaosMiniWave = chaosMiniWave;
            }
        }

        public void OnPlayerVerified(VerifiedEventArgs ev)
        {
            Log.Debug("Player joined, sending settings.");
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
#if HSM
            PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Player);

            DynamicHint hint = new()
            {
                AutoText = update => Plugin.Instance.GetTimers(update.Player),
                TargetY = 105,
                FontSize = 35,
                SyncSpeed = HintSyncSpeed.Fast
            };
            
            playerDisplay.AddHint(hint);
#endif
        }
    }
}