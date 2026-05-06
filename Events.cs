using HintServiceMeow.Core.Utilities;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;
using UserSettings.ServerSpecific;

namespace Timers
{
    public class Events : CustomEventsHandler
    {
        public override void OnPlayerJoined(PlayerJoinedEventArgs ev) => ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);

        public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
        {
            PlayerDisplay display = PlayerDisplay.Get(ev.Player);

            bool hasHint = display.HasHint(HintManager.TimerHint.Guid);

            if (ev.NewRole.Team != Team.Dead)
            {
                if (hasHint)
                    display.RemoveHint(HintManager.TimerHint);

                return;
            }

            if (!hasHint)
                ev.Player.AddHint();
        }

        public static void OnSettingUpdated(ReferenceHub hub, ServerSpecificSettingBase setting)
        {
            if (setting is not SSTwoButtonsSetting buttons || buttons.SettingId != Plugin.Instance.Setting.SettingId)
                return;

            PlayerDisplay display = PlayerDisplay.Get(hub);

            bool hasHint = display.HasHint(HintManager.TimerHint.Guid);

            if (hasHint && buttons.SyncIsB)
                display.RemoveHint(HintManager.TimerHint);
            else if (!hasHint && hub.roleManager.CurrentRole.Team == Team.Dead && Round.IsRoundStarted)
                display.AddHint(HintManager.TimerHint);

            Logger.Debug($"Player settings updated, round is {(Round.IsRoundStarted ? "started" : "not started")}", Plugin.Instance.Config.Debug);
        }
    }
}