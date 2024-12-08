using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace ButchersGames
{
    namespace CheatMenu
    {
        public class DebugCanvas : MonoBehaviour
        {
            public static DebugCanvas Instance = null;

            [SerializeField, Tooltip("Ускорять игру на зажатие ПКМ?")] bool rbmSpeedUp = false;
            [SerializeField] float speedUpTimeScale = 3;

            void Awake()
            {
                if (Instance)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Instance = this;
                    DontDestroyOnLoad(gameObject);
                }
            }

            void Update()
            {
                if (!rbmSpeedUp) return;
                if (Input.GetMouseButton(1)) Time.timeScale = Mathf.Lerp(Time.timeScale, speedUpTimeScale, Time.deltaTime);
                else if (Input.GetMouseButtonUp(1)) Time.timeScale = 1;
            }

            public void ResetSaves()
            {
                YandexGame.ResetSaveProgress();
                YandexGame.SaveProgress();
                SceneManager.LoadScene(0);
            }

            // Замени на свои функции
            public void LoadPrevLvl() => LevelManager.LoadPreviousLevel();
            public void LoadNextLvl() => LevelManager.LoadNextLevel();
        }
    }
}