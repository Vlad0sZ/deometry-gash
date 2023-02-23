using UnityEngine;

namespace GameplayScripts.Runtime
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [System.Serializable]
        private struct PhysicsSettings
        {
            [Tooltip("Distance for ground BoxCast")]
            public float maxDistance;

            [Tooltip("Size for ground BoxCast")] public Vector2 castSize;

            [Tooltip("LayerMask for ground BoxCost")]
            public LayerMask layerMask;

            [Tooltip("Min angle collision for walls")]
            public float minAngleDirection;
        }

        [System.Serializable]
        private struct MovementSettings
        {
            [Tooltip("Rotate velocity, when player in air")]
            public float angleVelocity;

            [Tooltip("static jump force")] public float jumpVelocity;

            [Tooltip("static move velocity")] public float moveVelocity;
        }


        [SerializeField] private PhysicsSettings physics;
        [SerializeField] private MovementSettings movement;

        private Rigidbody2D _rigidbody;
        private Vector2 _startPosition;
        private Coroutine _restartRoutine;

        private bool IsGrounded { get; set; }
        public bool CanMove { get; set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _startPosition = _rigidbody.position;
        }

        private void Update()
        {
            if (!CanMove)
                return;

            var velocity = GetVelocity();

            if (Input.GetKey(KeyCode.Space) && CanJump(velocity))
                velocity += movement.jumpVelocity * Vector2.up;

            ApplyAngularVelocity();
            ApplyMovementVelocity(velocity);
        }


        private void FixedUpdate()
        {
            var position = _rigidbody.position;
            RaycastHit2D hit =
                Physics2D.BoxCast(
                    position,
                    physics.castSize,
                    0,
                    Vector2.down,
                    physics.maxDistance,
                    physics.layerMask.value);

            IsGrounded = hit.collider != null;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.contactCount == 0) return;

            if (Obstacle.IsObstacle(other.collider))
            {
                PlayerDied();
                return;
            }

            // detect walls
            Vector2 direction = other.GetContact(0).normal;
            float angle = Vector2.Angle(direction, Vector2.up);
            if (angle > physics.minAngleDirection) PlayerDied();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            // Finish is obstacle & trigger
            if (Obstacle.IsObstacle(col))
                EventManager.InvokePlayerFinished(_rigidbody.position);
        }


        private Vector2 GetVelocity() => new(movement.moveVelocity, _rigidbody.velocity.y);

        private bool CanJump(Vector2 velocity) => IsGrounded && velocity.y < movement.jumpVelocity;

        private void ApplyAngularVelocity()
        {
            if (!IsGrounded)
                _rigidbody.angularVelocity = movement.angleVelocity;
            else
                _rigidbody.angularVelocity = 0;
        }

        private void ApplyMovementVelocity(Vector2 velocity) => _rigidbody.velocity = velocity;

        private void PlayerDied() => EventManager.InvokePlayerDied(_rigidbody.position);

        public void Respawn()
        {
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;
            _rigidbody.rotation = 0;
            _rigidbody.position = _startPosition;
            CanMove = true;
            EventManager.InvokePlayerRespawn();
        }
    }
}