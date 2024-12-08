using System;
using System.Collections.Generic;
using UnityEngine;

namespace ButchersGames
{
    namespace CheatMenu
    {
        public class FPSReport : MonoBehaviour
        {
            public static FPSReport Instance = null;

            const string AvgFPS_PREF = "AvgFPSLvl";
            const string MinFPS_PREF = "MinFPSLvl";
            const int FPSGroupSize = 5;

            // Замени на свои параметры
            static int CurrentLvl => LevelManager.CurrentLevel;
            static int MaxLvl => 120;

            int oldLvl;
            int avgFPS;
            int frameCount;
            float sumFPS, tFPS;
            List<int> fpsList;
            Dictionary<int, FPSGroup[]> lvlFPSGroups;
            Dictionary<int, List<string>> actionsGroups;
            event Action<int> NextFrameAction;

            void Awake()
            {
                if (Instance)
                {
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    Instance = this;
                    DontDestroyOnLoad(gameObject);
                }
            }

            void Start()
            {
                lvlFPSGroups = new();
                oldLvl = CurrentLvl;
                Reset();
            }

            void Update()
            {
                if (Time.timeSinceLevelLoad < 0.1f) return;

                int curLvl = CurrentLvl;
                if (curLvl >= MaxLvl) return;

                if (oldLvl != curLvl)
                {
                    Calculate();
                    Reset();
                    oldLvl = curLvl;
                }

                float unscaledDeltaTime = Time.unscaledDeltaTime;
                frameCount++;
                tFPS += unscaledDeltaTime;

                if (tFPS >= 1)
                {
                    int fps = (int)(frameCount / tFPS);
                    sumFPS += fps;
                    fpsList.Add(fps);

                    NextFrameAction?.Invoke(fps);
                    NextFrameAction = null;

                    frameCount = 0;
                    tFPS = 0;
                }
            }

            void Calculate()
            {
                if (fpsList.Count == 0) return;

                avgFPS = (int)(sumFPS / fpsList.Count);
                PlayerPrefs.SetInt(AvgFPS_PREF + oldLvl, avgFPS);

                int diffSum = 0;
                for (int i = 0; i < fpsList.Count; i++)
                {
                    int diff = fpsList[i] - avgFPS;
                    diffSum += diff * diff;
                }
                int minFPS = Mathf.RoundToInt(avgFPS - Mathf.Sqrt((float)diffSum / fpsList.Count));
                PlayerPrefs.SetInt(MinFPS_PREF + oldLvl, minFPS);

                FPSGroup[] groups = new FPSGroup[minFPS / FPSGroupSize + 1];
                for (int i = 0; i < fpsList.Count; i++)
                {
                    if (fpsList[i] <= minFPS)
                    {
                        int group = fpsList[i] / 5;
                        groups[group].sec++;
                        groups[group].actions = actionsGroups.ContainsKey(group) ? new(actionsGroups[group]) : new();
                    }
                }
                lvlFPSGroups[oldLvl] = groups;
            }

            void Reset()
            {
                fpsList = new();
                actionsGroups = new();
                sumFPS = 0;
                tFPS = 0;
                frameCount = 0;
            }

            /// <summary>
            /// Соотносит событие с FPS текущей и следующей секунды
            /// </summary>
            /// <param name="actionName">Название события</param>
            public static void Log(string actionName) { if (Instance) Instance.LogAction(actionName); }
            void LogAction(string actionName)
            {
                NextFrameAction += (x) => AddAction(x, actionName);
                if (fpsList != null && fpsList.Count > 0)
                {
                    AddAction(fpsList[^1], actionName);
                }
            }

            void AddAction(int fps, string actionName)
            {
                int group = fps / 5;
                if (!actionsGroups.ContainsKey(group)) actionsGroups[group] = new();
                if (!actionsGroups[group].Contains(actionName)) actionsGroups[group].Add(actionName);
            }

            /// <summary>
            /// Выводит отчёт по FPS в консоль и сохраняет в файл (WebGL)
            /// </summary>
            public void Print()
            {
                int fullGameAvgFPS = 0, lvlCounted = 0;
                string message = "";

                for (int i = 0; i < MaxLvl; i++)
                {
                    if (PlayerPrefs.HasKey(AvgFPS_PREF + i))
                    {
                        int avg = PlayerPrefs.GetInt(AvgFPS_PREF + i);
                        int min = PlayerPrefs.GetInt(MinFPS_PREF + i);
                        message += $"Lvl {i}: min {min}, avg {avg}" + "\n";

                        lvlCounted++;
                        fullGameAvgFPS += avg;

                        if (lvlFPSGroups.ContainsKey(i))
                        {
                            int displayedGroups = 0;
                            for (int j = 0; j < lvlFPSGroups[i].Length; j++)
                            {
                                if (lvlFPSGroups[i][j].sec > 0)
                                {
                                    message += $"{j * 5}-{Mathf.Min((j + 1) * 5, min)}: ";
                                    for (int action = 0; action < lvlFPSGroups[i][j].actions.Count; action++)
                                    {
                                        message += lvlFPSGroups[i][j].actions[action] + ", ";
                                    }
                                    message += $"({lvlFPSGroups[i][j].sec}s); ";
                                    displayedGroups++;
                                }
                            }
                            if (displayedGroups > 0) message += "\n";
                        }
                        message += "\n";
                    }
                }
                if (lvlCounted > 0) message = message.Insert(0, $"Average FPS for the entire game: {Math.Round((float)fullGameAvgFPS / lvlCounted, 1)}" + "\n\n");

                Debug.Log(message);
                WebGLFileSaver.SaveFile(message, "FPS Report.txt");
            }

            struct FPSGroup
            {
                public int sec;
                public List<string> actions;
            }
        }
    }
}