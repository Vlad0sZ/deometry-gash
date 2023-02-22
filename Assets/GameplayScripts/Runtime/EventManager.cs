using System;

namespace GameplayScripts.Runtime
{
    public static class EventManager
    {
        public static event Action OnPlayerDied;

        public static event Action OnPlayerFinished;

        internal static void InvokePlayerDied() => OnPlayerDied?.Invoke();

        internal static void InvokePlayerFinished() => OnPlayerFinished?.Invoke();
    }
}