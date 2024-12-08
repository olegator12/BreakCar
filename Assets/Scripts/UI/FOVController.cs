using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVController : MonoBehaviour
{
    Camera cam;
    Vector2 screen;
    [SerializeField] Vector2 screenVertical;
    [SerializeField] Vector2 screenHorizontal;
    [SerializeField] float fovVertical;
    [SerializeField] float fovHorizontal;

    private void Start()
    {
        cam = GetComponent<Camera>();
        screen = new Vector2(Screen.width, Screen.height);
        float coef = screen.x / screen.y;
        float coefVertical = screenVertical.x / screenVertical.y;
        float coefHorizontal = screenHorizontal.x / screenHorizontal.y;
        float offsetFov = Mathf.Abs(fovHorizontal - fovVertical);
        float offsetCoef = Mathf.Abs(coefHorizontal - coefVertical);
        float fovStep = offsetFov / offsetCoef;
        float fov = fovVertical + ((coefVertical - coef) * fovStep);
        cam.fieldOfView = fov;
    }
}
