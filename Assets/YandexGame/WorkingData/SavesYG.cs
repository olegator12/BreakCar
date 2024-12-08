using System.Collections.Generic;

namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";

        public bool promptDone;
        // ----------------------------------

        public bool didCompleteTutorial;
        public List<int> openedWeapons;
        public int priorityWeapon;
        public int weaponLevelProgress;
        public int weaponLevelPointer;
        public int moneyCount;
        public int beforeEndMoneyCount;
        public bool isSoundActive;
        public bool isMusicActive;
        public float painterTurnState;
        public List<int> completedLevels;
        public int levelCompleteCount;

        public SavesYG()
        {
            didCompleteTutorial = false;
            moneyCount = 100;
            beforeEndMoneyCount = 100;
            isSoundActive = true;
            isMusicActive = true;
            painterTurnState = 1f;
            priorityWeapon = -1;
            weaponLevelPointer = 0;
            weaponLevelProgress = 0;

            openedWeapons = new List<int>()
            {
                0,
                1,
                2,
            };

            completedLevels = new List<int>()
            {
                0,
            };
        }
    }
}