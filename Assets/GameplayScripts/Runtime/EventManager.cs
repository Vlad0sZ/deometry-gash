using System;
using UnityEngine;

namespace GameplayScripts.Runtime
{
    public static class EventManager
    {
        public static event Action<Vector2> OnPlayerDied;
        public static event Action<Vector2> OnPlayerFinished;

        public static event Action OnPlayerRespawn;

        internal static void InvokePlayerDied(Vector2 position) => OnPlayerDied?.Invoke(position);

        internal static void InvokePlayerFinished(Vector2 position) => OnPlayerFinished?.Invoke(position);

        internal static void InvokePlayerRespawn() => OnPlayerRespawn?.Invoke();
    }
}