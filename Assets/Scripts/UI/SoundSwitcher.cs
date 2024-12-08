using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Random = UnityEngine.Random;

namespace UI
{
    public class SoundSwitcher : MonoBehaviour
    {
        [SerializeField] private Button _music;
        [SerializeField] private Button _sound;
        [SerializeField] private GameObject _soundIcon;
        [SerializeField] private GameObject _musicIcon;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] private List<AudioSource> _sounds;

        private void OnDestroy()
        {
            _music.onClick.RemoveListener(OnClickMusic);
            _sound.onClick.RemoveListener(OnClickSound);
            Application.focusChanged -= OnFocusChanged;
            YandexGame.onVisibilityWindowGame -= OnFocusChanged;
        }

        private void FixedUpdate()
        {
            if (_musicSource.isPlaying == false)
                PlayMusic();
        }

        public void Initialize()
        {
            UpdateSound();

            _music.onClick.AddListener(OnClickMusic);
            _sound.onClick.AddListener(OnClickSound);
            Application.focusChanged += OnFocusChanged;
            YandexGame.onVisibilityWindowGame += OnFocusChanged;

            PlayMusic();
        }

        private void OnFocusChanged(bool isInApp)
        {
            if (isInApp == false)
            {
                Time.timeScale = 0f;
                AudioListener.pause = true;
            }
            else
            {
                if (YandexGame.nowAdsShow == true || YandexGame.isVisibilityWindowGame == false ||
                    Application.isFocused == false)
                    return;

                Time.timeScale = 1f;
                AudioListener.pause = false;
            }
        }

        public void AddSound(AudioSource source)
        {
            if (source == null)
                return;

            source.mute = YandexGame.savesData.isSoundActive == false;
            _sounds.Add(source);
        }

        private void UpdateSound()
        {
            bool isMusicActive = YandexGame.savesData.isMusicActive == false;
            bool isSoundActive = YandexGame.savesData.isSoundActive == false;

            _musicSource.mute = isMusicActive;
            _musicIcon.SetActive(isMusicActive);

            _sounds.RemoveAll(item => item == null);

            foreach (AudioSource sound in _sounds)
                sound.mute = isSoundActive;

            _soundIcon.SetActive(isSoundActive);
        }

        private void OnClickSound()
        {
            YandexGame.savesData.isSoundActive = YandexGame.savesData.isSoundActive == false;
            YandexGame.SaveProgress();
            UpdateSound();
        }

        private void OnClickMusic()
        {
            YandexGame.savesData.isMusicActive = YandexGame.savesData.isMusicActive == false;
            YandexGame.SaveProgress();
            UpdateSound();
        }

        private void PlayMusic()
        {
            _musicSource.clip = _clips[Random.Range(0, _clips.Length)];
            _musicSource.Play();
        }
    }
}