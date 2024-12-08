using UnityEngine;

public class SpawnableObject : MonoBehaviour
{
    private GameObject _gameObject;
    private IPushable _spawner;

    public Transform Transform { get; private set; }

    public SpawnableObject Initialize(IPushable spawner)
    {
        _spawner = spawner;
        _gameObject = gameObject;
        Transform = transform;

        SetActive(false);
        return this;
    }

    public T Pull<T>(Vector3 position)
        where T : SpawnableObject
    {
        if (Transform is RectTransform rectTransform)
        {
            rectTransform.anchoredPosition = position;
            return Pull<T>();
        }

        Transform.position = position;
        return Pull<T>();
    }

    public void Push()
    {
        _spawner.Push(this);
        OnPush();

        if (Transform is RectTransform rectTransform)
        {
            rectTransform.anchoredPosition = Vector3.zero;
            return;
        }

        Transform.position = Vector3.zero;
    }

    public void SetActive(bool value)
    {
        _gameObject.SetActive(value);
    }

    private T Pull<T>()
        where T : SpawnableObject
    {
        SetActive(true);
        return this as T;
    }

    public virtual void OnPush()
    {
    }
}