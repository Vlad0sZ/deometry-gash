using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameplayScripts.Runtime
{
    public class GameWatcher : MonoBehaviour
    {
        [SerializeField] private PlayerMovement player;
        [SerializeField] private float defaultRespawnTime;
        [SerializeField] private Button reloadLevelButton;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip winClip;

        private GameObject _playerObject;
        private Coroutine _restartCoroutine;

        private void OnEnable()
        {
            EventManager.OnPlayerDied += OnPlayerDied;
            EventManager.OnPlayerFinished += OnPlayerFinished;
            reloadLevelButton.onClick.AddListener(ReloadLevel);
        }

        private void Start()
        {
            _playerObject = player.gameObject;
            ReloadLevel();
        }

        private void OnDisable()
        {
            EventManager.OnPlayerDied -= OnPlayerDied;
            EventManager.OnPlayerFinished -= OnPlayerFinished;
            reloadLevelButton.onClick.RemoveListener(ReloadLevel);
        }

        private void OnPlayerFinished(Vector2 _)
        {
            player.CanMove = false;
            musicSource.Stop();
            musicSource.PlayOneShot(winClip);
            reloadLevelButton.gameObject.SetActive(true);
        }

        private void OnPlayerDied(Vector2 diePosition)
        {
            if (_restartCoroutine != null)
                StopCoroutine(_restartCoroutine);

            _restartCoroutine = StartCoroutine(RespawnAfter(defaultRespawnTime));
        }


        private IEnumerator RespawnAfter(float seconds)
        {
            _playerObject.SetActive(false);
            musicSource.Stop();

            yield return new WaitForSeconds(seconds);
            _playerObject.SetActive(true);
            player.Respawn();
            musicSource.Play();
            _restartCoroutine = null;
        }


        private void ReloadLevel()
        {
            reloadLevelButton.gameObject.SetActive(false);
            player.Respawn();
            musicSource.Play();
        }
    }
}