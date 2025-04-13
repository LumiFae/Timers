#if HSM
using HintServiceMeow.Core.Utilities;
using PlayerRoles;
#endif
using Exiled.API.Features.Waves;
using Exiled.Events.EventArgs.Player;
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
                HintManager.Instance.NtfWave = ntfWave;
            }

            if (TimedWave.TryGetTimedWave<NtfMiniWave>(out TimedWave ntfMiniWave))
            {
                Log.Debug("NtfMiniWave found");
                HintManager.Instance.NtfMiniWave = ntfMiniWave;
            }

            if (TimedWave.TryGetTimedWave<ChaosSpawnWave>(out TimedWave chaosWave))
            {
                Log.Debug("ChaosWave found");
                HintManager.Instance.ChaosWave = chaosWave;
            }

            if (TimedWave.TryGetTimedWave<ChaosMiniWave>(out TimedWave chaosMiniWave))
            {
                Log.Debug("ChaosMiniWave found");
                HintManager.Instance.ChaosMiniWave = chaosMiniWave;
            }
        }

        public void OnPlayerVerified(VerifiedEventArgs ev)
        {
            Log.Debug("Player joined, sending settings.");
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
        }

#if HSM
        public void OnLeft(LeftEventArgs ev)
        {
            PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Player);
            if (playerDisplay?.GetHint(HintManager.Instance.RespawnTimerDisplay.Guid) == null) return;
            playerDisplay.RemoveHint(HintManager.Instance.RespawnTimerDisplay);
        }
        
        private bool CanSendHint(RoleTypeId id)
        {
            return id is RoleTypeId.Spectator or RoleTypeId.Overwatch;
        }
        
        public void OnPlayerChangingRole(ChangingRoleEventArgs ev)
        {
            bool willSendHint = CanSendHint(ev.NewRole);
            bool currentlySendingHint = CanSendHint(ev.Player.Role.Type);

            if (willSendHint == currentlySendingHint) return;
            PlayerDisplay display = PlayerDisplay.Get(ev.Player);
            if (display == null) return;
            if (willSendHint && display.GetHint(HintManager.Instance.RespawnTimerDisplay.Guid) == null && ev.Player.ReferenceHub != null && ev.Player.ReferenceHub.gameObject != null)
                display.AddHint(HintManager.Instance.RespawnTimerDisplay);
            else
                display.RemoveHint(HintManager.Instance.RespawnTimerDisplay);
        }
#endif
    }
}