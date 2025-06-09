using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using System.IO;

public class HighScoreManager : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    private ScoreList highScores;

    [System.Serializable]
    public class ScoreList
    {
        public List<ScoreData> scoreData;
    }
    [System.Serializable]
    public class ScoreData
    {
        public string name;
        public int score;

        public ScoreData(string newName, int newScore)
        {
            name = newName;
            score = newScore;
        }
    }
    public ScoreList HIGHSCORES => highScores;
    #region Singleton
    private static HighScoreManager _instance;
    public static HighScoreManager instance => _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (this != _instance)
        {
            Destroy(this.gameObject);
        }
        highScores.scoreData = new List<ScoreData>();
#if !UNITY_WEBGL
        LoadData();
#endif
    }
    #endregion


    #region SAVE AND LOAD
    public void SaveData()
    {
        string json = JsonUtility.ToJson(highScores);
#if UNITY_EDITOR_WIN
        File.WriteAllText(Application.dataPath + "/highscore.json", json);
#else
        File.WriteAllText(Application.persistentDataPath + "/highscore.json", json);
#endif
        Debug.Log("DATA SAVED");
    }

    public void LoadData()
    {

        if (File.Exists(GetCurrentPath() + "/highscore.json"))
        {
#if UNITY_EDITOR_WIN
            string json = File.ReadAllText(Application.dataPath + "/highscore.json");
#else
            string json = File.ReadAllText(Application.persistentDataPath + "/highscore.json");
#endif
            highScores = JsonUtility.FromJson<ScoreList>(json);
            Debug.Log("SAVED DATA LOADED");
            //SaveData();
        }
        else
        {
            Debug.Log("NOT SAVED DATA EXIST");
            highScores.scoreData = new List<ScoreData>();
            while (highScores.scoreData.Count < 10)
            {
                highScores.scoreData.Add(new ScoreData("AAA", 0));
            }
            SaveData();
        }
    }

    private string GetCurrentPath()
    {
#if UNITY_EDITOR_WIN
        string toReturn = Application.dataPath;
#else
        string toReturn = Application.persistentDataPath;
#endif
        return toReturn;
    }
#endregion

#region Highscore methods
    public bool NewHighScore(int score, string name)
    {
        for (int i = 0; i < highScores.scoreData.Count; i++)
        {
            if (score > highScores.scoreData[i].score)
            {
                highScores.scoreData.RemoveAt(highScores.scoreData.Count - 1);
                highScores.scoreData.Insert(i, new ScoreData(name, score));
                SaveData();
                return true;
            }
        }
        SaveData();
        return false;
    }

    public void ResetData()
    {
        highScores.scoreData = new List<ScoreData>();
        while (highScores.scoreData.Count < 10)
        {
            highScores.scoreData.Add(new ScoreData("AAA", 0));
        }
        SaveData();
    }
#endregion
}