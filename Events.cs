#if HSM
using HintServiceMeow.Core.Utilities;
using PlayerRoles;
#endif
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using Respawning;
using Respawning.Waves;
using RueI.API;
using UserSettings.ServerSpecific;

namespace Timers
{
    public class Events : CustomEventsHandler
    {
        public override void OnPlayerJoined(PlayerJoinedEventArgs ev) =>
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);

        public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
        {
            if (ev.NewRole.Team != Team.Dead)
            {
                RueDisplay.Get(ev.Player).Remove(HintManager.Tag);
                return;
            }
            
            ev.Player.AddHint();
        }

        public override void OnServerRoundStarted() =>
            Timing.RunCoroutine(HintManager.GenerateElements());

        public static void OnSettingUpdated(ReferenceHub hub, ServerSpecificSettingBase setting)
        {
            if (setting is not SSTwoButtonsSetting buttons || buttons.SettingId != Plugin.Instance.Setting.SettingId)
                return;

            RueDisplay display = RueDisplay.Get(hub);

            if (buttons.SyncIsB)
                display.Remove(HintManager.Tag);
            else if (hub.roleManager.CurrentRole.Team == Team.Dead && Round.IsRoundStarted) 
            {
                display.Show(HintManager.Tag, HintManager.CurrentElement);
            }
            
            Logger.Debug($"Player settings updated, round is {(Round.IsRoundStarted ? "started" : "not started")}", Plugin.Instance.Config.Debug);
        }
    }
}