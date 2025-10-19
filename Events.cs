#if HSM
using HintServiceMeow.Core.Utilities;
using PlayerRoles;
#endif
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using PlayerRoles;
using Respawning;
using Respawning.Waves;
using RueI.API;
using UserSettings.ServerSpecific;

namespace Timers
{
    public class Events : CustomEventsHandler
    {
        public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
        {
            Logger.Debug("Player joined, sending settings.", Plugin.Instance.Config.Debug);
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
        }

        public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev) =>
            RueDisplay.Get(ev.Player).SetVisible(HintManager.Tag, !ev.Player.IsAlive);

        public static void OnSettingUpdated(ReferenceHub hub, ServerSpecificSettingBase setting)
        {
            if (setting is not SSTwoButtonsSetting buttons || buttons.SettingId != Plugin.Instance.Setting.SettingId)
                return;

            RueDisplay display = RueDisplay.Get(hub);
            
            if(buttons.SyncIsB)
                display.Remove(HintManager.Tag);
            else
                display.Show(HintManager.Tag, HintManager.Element);
            
            display.SetVisible(HintManager.Tag, hub.roleManager.CurrentRole.Team == Team.Dead);
        }
    }
}