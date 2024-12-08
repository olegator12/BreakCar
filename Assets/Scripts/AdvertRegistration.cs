using System;
using UnityEngine;

public abstract class AdvertRegistration : MonoBehaviour
{
    public abstract void RegisterCallback(AdvertName name, Action callback);
}