using UnityEngine;

namespace GameplayScripts.Runtime
{
    public class Obstacle : MonoBehaviour
    {
        public static bool IsObstacle(Collider2D collider2D) =>
            collider2D != null && collider2D.GetComponent<Obstacle>();
    }
}