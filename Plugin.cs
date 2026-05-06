using Exiled.API.Features;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using UserSettings.ServerSpecific;

namespace Timers
{
    public class Plugin : Plugin<Config, Translation>
    {
        private Events events = null!;

        public override string Name => "Timers";

        public override string Author => "LumiFae";

        public override string Prefix => "Timers";

        public override Version Version => new(1, 5, 1);

        public static Plugin Instance { private set; get; } = null!;

        internal SSTwoButtonsSetting Setting { get; private set; } = null!;

        public override void OnEnabled()
        {
            Logger.Debug("Enabling plugin...", Config.Debug);
            Instance = this;

            events = new();
            CustomHandlersManager.RegisterEventsHandler(events);

            ServerSpecificSettingsSync.ServerOnSettingValueReceived += Events.OnSettingUpdated;

            SSGroupHeader header = new(Translation.ServerSpecificSettingHeading);
            Setting = new SSTwoButtonsSetting(Config.ServerSpecificSettingId, Translation.OverlaySettingText, Translation.Enable, Translation.Disable, hint:Translation.OverlaySettingHint);

            ServerSpecificSettingsSync.DefinedSettings = [..ServerSpecificSettingsSync.DefinedSettings ?? [], header, Setting];
                
            ServerSpecificSettingsSync.SendToAll();
        }

        public override void OnDisabled()
        {
            CustomHandlersManager.UnregisterEventsHandler(events);   
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= Events.OnSettingUpdated;
            events = null!;
        }
    }
}