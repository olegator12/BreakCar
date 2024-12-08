using System;
using UI;

public static class LevelManager
{
    private static Action<int, GameWindows> _loadCallback;
    private static Action _backCallback;
    private static int _maxLevel;

    public static int CurrentLevel { get; set; } = -1;

    public static void Register(Action<int, GameWindows> loadCallback, Action backCallback, int maxLevel)
    {
        _loadCallback = loadCallback;
        _backCallback = backCallback;
        _maxLevel = maxLevel;
    }

    public static void LoadLevel(int id)
    {
        if (id < 0 || id >= _maxLevel)
            return;

        if (CurrentLevel >= 0)
            _backCallback?.Invoke();

        _loadCallback?.Invoke(id, GameWindows.Game);
        CurrentLevel = id;
    }

    public static void LoadNextLevel()
    {
        if (CurrentLevel == _maxLevel - 1)
            return;

        CurrentLevel++;
        LoadLevel(CurrentLevel);
    }

    public static void LoadPreviousLevel()
    {
        if (CurrentLevel == 0)
            return;

        CurrentLevel--;
        LoadLevel(CurrentLevel);
    }
}