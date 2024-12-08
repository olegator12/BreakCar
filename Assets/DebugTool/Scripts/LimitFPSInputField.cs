using TMPro;
using UnityEngine;

namespace ButchersGames
{
    namespace CheatMenu
    {
        public class LimitFPSInputField : MonoBehaviour
        {
            void Awake() => GetComponent<TMP_InputField>().onEndEdit.AddListener(LimitFPS);
            void LimitFPS(string fps) { if (int.TryParse(fps, out int _fps)) Application.targetFrameRate = _fps; }
        }
    }
}