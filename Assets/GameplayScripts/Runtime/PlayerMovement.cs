using UnityEngine;

namespace GameplayScripts.Runtime
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [System.Serializable]
        private struct PhysicsSettings
        {
            public float maxDistance;
            public LayerMask layerMask;
        }

        [System.Serializable]
        private struct MovementSettings
        {
            public float angleVelocity;
            public float jumpVelocity;
            public float moveVelocity;
        }


        [SerializeField] private PhysicsSettings physics;
        [SerializeField] private MovementSettings movement;

        private Rigidbody2D _rigidbody;
        private Vector2 _startPosition;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _startPosition = _rigidbody.position;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                Respawn();

            var velocity = GetVelocity();

            if (Input.GetKey(KeyCode.Space) && CanJump())
                velocity += movement.jumpVelocity * Vector2.up;

            ApplyAngularVelocity();
            ApplyMovementVelocity(velocity);
        }


        private void FixedUpdate()
        {
            var position = _rigidbody.position;
            RaycastHit2D hit =
                Physics2D.Raycast(
                    position,
                    Vector2.down,
                    physics.maxDistance,
                    physics.layerMask.value);

            IsGrounded = hit.collider != null;
            Debug.DrawRay(position, Vector3.down * physics.maxDistance, IsGrounded ? Color.green : Color.red);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.contactCount == 0) return;

            if (Obstacle.IsObstacle(other.collider))
                Respawn();

            Vector2 direction = other.GetContact(0).normal;
            float sqr = Vector2.SqrMagnitude(direction - Vector2.up);
            if (sqr < 0.225f) return;

            Respawn();
        }


        private Vector2 GetVelocity() => new(movement.moveVelocity, _rigidbody.velocity.y);

        private bool CanJump() => IsGrounded && _rigidbody.velocity.y < movement.jumpVelocity;

        private void ApplyAngularVelocity()
        {
            if (!IsGrounded)
                _rigidbody.angularVelocity = movement.angleVelocity;
            else
                _rigidbody.angularVelocity = _rigidbody.rotation = 0;
        }

        private void ApplyMovementVelocity(Vector2 velocity) => _rigidbody.velocity = velocity;

        private void Respawn()
        {
            EventManager.InvokePlayerDied();
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;
            _rigidbody.position = _startPosition;
        }

        private void OnGUI()
        {
            var rect = GUIUtility.GUIToScreenRect(new Rect(8, 8, 160, 160));
            GUI.Label(rect, $"IsGrounded: {IsGrounded}\n" +
                            $"R2D_Velocity: {_rigidbody.velocity}\n" +
                            $"R2D_Angular: {_rigidbody.angularVelocity}\n");
        }
    }
}