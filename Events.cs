using Exiled.API.Features.Waves;
using Exiled.Events.EventArgs.Player;
#if HSM
using Exiled.API.Features;
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
        }
        
#if HSM
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
            if (willSendHint && display.GetHint(Plugin.Instance.RespawnTimerDisplay.Guid) == null)
                display.AddHint(Plugin.Instance.RespawnTimerDisplay);
            else
                display.RemoveHint(Plugin.Instance.RespawnTimerDisplay);
        }
#endif
    }
}