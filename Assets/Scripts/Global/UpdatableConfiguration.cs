using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public abstract class UpdatableConfiguration<TK, TV> : ScriptableObject
    where TK : Enum
{
    [SerializeField] private SerializedDictionary<TK, TV> _content;

    private void OnValidate()
    {
        UpdateContent(_content);
    }

    public Dictionary<TK, TV> GetConfiguration()
    {
        return _content;
    }

    private void UpdateContent<T1, T2>(SerializedDictionary<T1, T2> content)
    {
        foreach (SerializedPair<T1, T2> pair in Create<T1, T2>())
        {
            if (content.ContainsKey(pair.Key) == true)
                continue;

            content.Add(pair.Key, pair.Value);
        }
    }

    private List<SerializedPair<T1, T2>> Create<T1, T2>()
    {
        List<SerializedPair<T1, T2>> result = new ();
        T2 value = default;

        foreach (T1 type in Enum.GetValues(typeof(T1)))
            result.Add(new SerializedPair<T1, T2>(type, value));

        return result;
    }
}