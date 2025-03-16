using Exiled.API.Features.Waves;
using Exiled.Events.EventArgs.Player;
#if HSM
using HintServiceMeow.Core.Utilities;
using PlayerRoles;
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
            if (TimedWave.TryGetTimedWave<NtfSpawnWave>(out var ntfWave))
            {
                Log.Debug("NtfWave found");
                Plugin.Instance.NtfWave = ntfWave;
            }
            
            if (TimedWave.TryGetTimedWave<NtfMiniWave>(out var ntfMiniWave))
            {
                Log.Debug("NtfMiniWave found");
                Plugin.Instance.NtfMiniWave = ntfMiniWave;
            }
            
            if (TimedWave.TryGetTimedWave<ChaosSpawnWave>(out var chaosWave))
            {
                Log.Debug("ChaosWave found");
                Plugin.Instance.ChaosWave = chaosWave;
            }
            
            if (TimedWave.TryGetTimedWave<ChaosMiniWave>(out var chaosMiniWave))
            {
                Log.Debug("ChaosMiniWave found");
                Plugin.Instance.ChaosMiniWave = chaosMiniWave;
            }
        }

        public void OnPlayerVerified(VerifiedEventArgs ev)
        {
            Log.Debug("Player joined, sending settings.");
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
        }
        
#if HSM
        private bool CanSendHint(RoleTypeId id)
        {
            return id is RoleTypeId.Spectator or RoleTypeId.Overwatch;
        }
        
        public void OnPlayerChangingRole(ChangingRoleEventArgs ev)
        {
            var willSendHint = CanSendHint(ev.NewRole);
            var currentlySendingHint = CanSendHint(ev.Player.Role.Type);

            if (willSendHint == currentlySendingHint) return;
            var display = PlayerDisplay.Get(ev.Player);
            if (willSendHint && display.GetHint(Plugin.Instance.RespawnTimerDisplay.Guid) == null)
                display.AddHint(Plugin.Instance.RespawnTimerDisplay);
            else
                display.RemoveHint(Plugin.Instance.RespawnTimerDisplay);
        }
#endif
    }
}