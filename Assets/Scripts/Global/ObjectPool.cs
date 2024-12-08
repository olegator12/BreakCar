using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ObjectPool<T> : IPushable
    where T : SpawnableObject
{
    private readonly Queue<SpawnableObject> _spawnQueue = new ();
    private readonly SpawnableObject _spawnableObject;

    private Transform _holder;
    private Action<T> _onCreate;

    public ObjectPool(SpawnableObject spawnableObject)
    {
        _spawnableObject = spawnableObject;
    }

    public int Count => _spawnQueue.Count;

    public void RegisterHolder(Transform holder)
    {
        _holder = holder;
    }

    public void RegisterOnCreate(Action<T> callback)
    {
        _onCreate = callback;
    }

    public void Push(SpawnableObject spawnableObject)
    {
        spawnableObject.Transform.SetParent(_holder);
        PushOnInitialize(spawnableObject);
    }

    public T Pull(Vector3 position, Action<T> initializeCallback = null)
    {
        if (_spawnQueue.Count == 0)
        {
            SpawnableObject spawnableObject =
                Object.Instantiate(_spawnableObject, position, Quaternion.identity, _holder).Initialize(this);

            initializeCallback?.Invoke(spawnableObject as T);
            _onCreate?.Invoke(spawnableObject as T);
            PushOnInitialize(spawnableObject);
        }

        return _spawnQueue.Dequeue().Pull<T>(position);
    }

    private void PushOnInitialize(SpawnableObject spawnableObject)
    {
        spawnableObject.SetActive(false);
        _spawnQueue.Enqueue(spawnableObject);
    }
}