using UI;
using UnityEngine;
using YG;

[RequireComponent(typeof(UIWindow))]
public class ObjectEnabler : MonoBehaviour
{
    [SerializeField] private bool _isTargetDeviceDesktop;

    private void Awake()
    {
        UIWindow window = GetComponent<UIWindow>();
        window.Initialize();

        switch (_isTargetDeviceDesktop)
        {
            case true when YandexGame.EnvironmentData.isDesktop == true:
                window.Open();
                break;

            case false when YandexGame.EnvironmentData.isDesktop == false:
                window.Open();
                break;
        }
    }
}