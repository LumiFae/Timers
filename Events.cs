using Exiled.API.Features.Waves;
using GameCore;
using Respawning.Waves;
using Log = Exiled.API.Features.Log;

namespace Timers
{
    public class Events
    {
        public void OnRoundStart()
        {
            if (TimedWave.TryGetTimedWave<NtfSpawnWave>(out TimedWave ntfWave))
            {
                Log.Info("NtfWave found");
                Plugin.Instance.NtfWave = ntfWave;
            }
            if (TimedWave.TryGetTimedWave<NtfMiniWave>(out TimedWave ntfMiniWave))
            {
                Log.Info("NtfMiniWave found");
                Plugin.Instance.NtfMiniWave = ntfMiniWave;
            }
            if (TimedWave.TryGetTimedWave<ChaosSpawnWave>(out TimedWave chaosWave))
            {
                Log.Info("ChaosWave found");
                Plugin.Instance.ChaosWave = chaosWave;
            }
            if (TimedWave.TryGetTimedWave<ChaosMiniWave>(out TimedWave chaosMiniWave))
            {
                Log.Info("ChaosMiniWave found");
                Plugin.Instance.ChaosMiniWave = chaosMiniWave;
            }
        }
    }
}