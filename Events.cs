#if HSM
using HintServiceMeow.Core.Utilities;
using PlayerRoles;
#endif
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using Respawning;
using Respawning.Waves;
using UserSettings.ServerSpecific;

namespace Timers
{
    public class Events
    {
        public void OnRoundStart()
        {
            if (WaveManager.TryGet(out NtfSpawnWave ntfWave))
            {
                Logger.Debug("NtfWave found", Plugin.Instance.Config.Debug);
                HintManager.Instance.NtfWave = ntfWave;
            }

            if (WaveManager.TryGet(out NtfMiniWave ntfMiniWave))
            {
                Logger.Debug("NtfMiniWave found", Plugin.Instance.Config.Debug);
                HintManager.Instance.NtfMiniWave = ntfMiniWave;
            }

            if (WaveManager.TryGet(out ChaosSpawnWave chaosWave))
            {
                Logger.Debug("ChaosWave found", Plugin.Instance.Config.Debug);
                HintManager.Instance.ChaosWave = chaosWave;
            }

            if (WaveManager.TryGet(out ChaosMiniWave chaosMiniWave))
            {
                Logger.Debug("ChaosMiniWave found", Plugin.Instance.Config.Debug);
                HintManager.Instance.ChaosMiniWave = chaosMiniWave;
            }
        }

        public void OnPlayerVerified(PlayerJoinedEventArgs ev)
        {
            Logger.Debug("Player joined, sending settings.", Plugin.Instance.Config.Debug);
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
        }

#if HSM
        public void OnLeft(PlayerLeftEventArgs ev)
        {
            Logger.Debug("Player left, removing their hints, if any.", Plugin.Instance.Config.Debug);
            PlayerDisplay playerDisplay = PlayerDisplay.Get(ev.Player);
            if (playerDisplay?.GetHint(HintManager.Instance.RespawnTimerDisplay.Guid) == null) return;
            playerDisplay.RemoveHint(HintManager.Instance.RespawnTimerDisplay);
        }
        
        private bool CanSendHint(RoleTypeId id)
        {
            return id is RoleTypeId.Spectator or RoleTypeId.Overwatch;
        }
        
        public void OnPlayerChangingRole(PlayerChangingRoleEventArgs ev)
        {
            Logger.Debug("Player changing role, checking if valid roles for hints...", Plugin.Instance.Config.Debug);
            bool willSendHint = CanSendHint(ev.NewRole);
            bool currentlySendingHint = CanSendHint(ev.Player.Role);

            if (willSendHint == currentlySendingHint) return;
            Logger.Debug("Player roles are valid, managing hint...", Plugin.Instance.Config.Debug);
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