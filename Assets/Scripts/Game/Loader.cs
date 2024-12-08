using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class Loader : MonoBehaviour
    {
        [SerializeField] private string _sceneName = "Game";

        private void Awake()
        {
            SceneManager.LoadScene(_sceneName);
        }
    }
}