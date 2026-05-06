using System.ComponentModel;
using Exiled.API.Interfaces;
using UnityEngine;

namespace Timers
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("The color of the Chaos timer when Chaos are spawning.")]
        public Color ChaosSpawnColor { get; set; } = Color.green;

        [Description("The color of the NTF timer when NTF are spawning.")]
        public Color NtfSpawnColor { get; set; } = Color.blue;

        [Description("The ID of the server specific setting, only change this if it conflicts with another plugin.")]
        public int ServerSpecificSettingId { get; set; } = 333;
        
        [Description("The space between each timer, in Ems.")]
        public int SpaceBetweenTimers { get; set; } = 16;
    }
}