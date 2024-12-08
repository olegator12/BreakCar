using UnityEngine;
using UnityEngine.UI;
using YG;

namespace UI
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class GridConstraintsInstaller : MonoBehaviour
    {
        private const int DesktopRowCount = 1;
        private const int MobileRowCount = 2;

        private void Awake()
        {
            GetComponent<GridLayoutGroup>().constraintCount =
                YandexGame.EnvironmentData.isDesktop == true ? DesktopRowCount : MobileRowCount;
        }
    }
}