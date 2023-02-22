using System;
using UnityEngine;

namespace GameplayScripts.Runtime
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;
        private Collider2D _collider;
        private Vector3 _startPosition;
        private Transform _transform;

        public float torqueVelocity;
        public Vector2 jumpVelocity;
        public Vector2 moveVelocity;

        private Vector2 _velocity;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            _transform = transform;
            _collider = GetComponent<Collider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _startPosition = _transform.position;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                Respawn();

            _velocity = new Vector2(moveVelocity.x, _rigidbody.velocity.y);
            if (Input.GetKey(KeyCode.Space) && IsGrounded && _velocity.y < jumpVelocity.y)
                _velocity += jumpVelocity;

            _rigidbody.velocity = _velocity;
        }

        private void FixedUpdate()
        {
            if (!IsGrounded)
                AddTorque();
            else _rigidbody.angularVelocity = 0;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.contactCount == 0) return;
            var direction = other.GetContact(0).normal;
            var sqr = Vector2.SqrMagnitude(direction - Vector2.up);

            if (sqr < 0.1f)
                IsGrounded = true;
            else
            {
                Debug.Log($"Direction was {direction}, sqr = {sqr}");
                Respawn();
            }
        }

        private void OnCollisionExit2D(Collision2D other) => IsGrounded = false;

        private void Respawn()
        {
            _rigidbody.Sleep();
            _transform.position = _startPosition;
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;
            _rigidbody.WakeUp();
        }

        private void AddTorque()
        {
            float impulse = (torqueVelocity * Mathf.Deg2Rad) * _rigidbody.inertia;
            _rigidbody.angularVelocity = torqueVelocity;
        }

        private void OnGUI()
        {
            var rect = GUIUtility.GUIToScreenRect(new Rect(8, 8, 160, 160));
            GUI.Label(rect, $"Position: {_transform.position}\n" +
                            $"IsGrounded: {IsGrounded}\n" +
                            $"R2D_Velocity: {_rigidbody.velocity}\n" +
                            $"R2D_Angular: {_rigidbody.angularVelocity}\n" +
                            $"velocity: {_velocity}");
        }
    }
}