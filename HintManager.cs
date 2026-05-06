using System.Text;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using PlayerRoles;
using Respawning;
using UserSettings.ServerSpecific;

namespace Timers
{
    public static class HintManager
    {
        private static Config Config => Plugin.Instance.Config;

        private static Translation Translation => Plugin.Instance.Translation;

        private static MtfWave? NtfWave => RespawnWaves.PrimaryMtfWave;

        private static MiniMtfWave? NtfMiniWave => RespawnWaves.MiniMtfWave;

        private static ChaosWave? ChaosWave => RespawnWaves.PrimaryChaosWave;

        private static MiniChaosWave? ChaosMiniWave => RespawnWaves.MiniChaosWave;

        public static Hint TimerHint { get; } = new()
        {
            AutoText = _ => GetTimers(),

            YCoordinate = 105,
            FontSize = 35,
            SyncSpeed = HintSyncSpeed.Fast,
        };

        public static void AddHint(this Player player)
        {
            SSTwoButtonsSetting setting;

            try
            {
                setting = ServerSpecificSettingsSync.GetSettingOfUser<SSTwoButtonsSetting>(player.ReferenceHub, Plugin.Instance.Setting.SettingId);
            }
            catch (NullReferenceException)
            {
                return;
            }

            if (setting == null)
                return;

            if (setting.SyncIsB)
                return;

            PlayerDisplay display = PlayerDisplay.Get(player);

            display.AddHint(TimerHint);
        }

        private static TimeSpan NtfRespawnTime()
        {
            if (NtfMiniWave != null && !NtfMiniWave.Base.Timer.IsPaused)
                return TimeSpan.FromSeconds(NtfMiniWave.TimeLeft);

            return NtfWave != null ? TimeSpan.FromSeconds(NtfWave.TimeLeft) : TimeSpan.Zero;
        }

        private static TimeSpan ChaosRespawnTime()
        {
            if (ChaosMiniWave != null && !ChaosMiniWave.Base.Timer.IsPaused)
                return TimeSpan.FromSeconds(ChaosMiniWave.TimeLeft);

            return ChaosWave != null ? TimeSpan.FromSeconds(ChaosWave.TimeLeft) : TimeSpan.Zero;
        }

        private static string TimerText(TimeSpan timer)
        {
            return Translation.Timer
                .Replace("{minutes}", timer.Minutes.ToString("D1"))
                .Replace("{seconds}", timer.Seconds.ToString("D2"));
        }

        private static string GetTimers()
        {
            TimeSpan ntfTime = NtfRespawnTime() + TimeSpan.FromSeconds(18);
            if (ntfTime < TimeSpan.Zero)
                ntfTime = TimeSpan.Zero;
            TimeSpan chaosTime = ChaosRespawnTime() + TimeSpan.FromSeconds(13);
            if (chaosTime < TimeSpan.Zero)
                chaosTime = TimeSpan.Zero;

            StringBuilder builder = StringBuilderPool.Shared.Rent()
                .Append("<align=center>");

            if (WaveManager._nextWave != null
                && WaveManager._nextWave.TargetFaction == Faction.FoundationStaff
                && ntfTime.TotalSeconds <= 18)
                builder.Append($"<color={Config.NtfSpawnColor.ToHex()}>").Append(TimerText(ntfTime)).Append("</color>");
            else
                builder.Append(TimerText(ntfTime));

            builder.Append($"<space={Config.SpaceBetweenTimers}ems>");

            if (WaveManager._nextWave != null
                && WaveManager._nextWave.TargetFaction == Faction.FoundationEnemy
                && chaosTime.TotalSeconds <= 13)
                builder.Append($"<color={Config.ChaosSpawnColor.ToHex()}>").Append(TimerText(chaosTime)).Append("</color>");
            else
                builder.Append(TimerText(chaosTime));

            builder.Append("</align>");
            
            return StringBuilderPool.Shared.ToStringReturn(builder);
        }
    }
}