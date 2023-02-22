using UnityEngine;
using UnityEngine.UI;

namespace GameplayScripts.Runtime
{
    public class ScoreBoard : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        private int _cachedScore;
        private float _milliseconds;

        private float Score
        {
            get => _milliseconds;
            set => UpdateText(value);
        }

        private void OnEnable()
        {
            EventManager.OnPlayerDied += ClearScore;
        }

        private void OnDisable()
        {
            EventManager.OnPlayerDied -= ClearScore;
        }

        private void Update() => Score += Time.deltaTime * 10;

        public void UpdateText(float value)
        {
            _milliseconds = value;
            if (Mathf.Abs(_cachedScore - _milliseconds) < 1f) return;
            _cachedScore = Mathf.RoundToInt(_milliseconds);
            scoreText.text = _cachedScore.ToString();
        }

        public void ClearScore() => Score = 0;
    }
}