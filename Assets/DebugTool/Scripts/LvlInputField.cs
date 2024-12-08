using TMPro;
using UnityEngine;

namespace ButchersGames
{
    namespace CheatMenu
    {
        public class LvlInputField : MonoBehaviour
        {
            // Замени на свою функцию
            static void LoadLvl(int lvl) => LevelManager.LoadLevel(lvl);

            void Awake() => GetComponent<TMP_InputField>().onEndEdit.AddListener(LoadLvl);
            void LoadLvl(string lvl) { if (int.TryParse(lvl, out int lvlID)) LoadLvl(lvlID); }
        }
    }
}