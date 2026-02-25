using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int highScore;
}

[System.Serializable]
public class PlayerDatabase
{
    public List<PlayerData> players = new List<PlayerData>();
}

public class DataPersistent : MonoBehaviour
{
    public static DataPersistent Instance;
    private string savePath;
    public PlayerDatabase playerDatabase;
    public string bestPlayer;
    public int highScore;
    public string currentPlayerName;
    public int currentHighScore;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Application.persistentDataPath + "/playerDB.json";
        LoadDatabase();
    }

    public void LoadDatabase()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            playerDatabase = JsonUtility.FromJson<PlayerDatabase>(json);
        }
        else
        {
            playerDatabase = new PlayerDatabase();
        }
    }

    public void SaveDatabase()
    {
        string json = JsonUtility.ToJson(playerDatabase, true);
        File.WriteAllText(savePath, json);
    }

    public void SavePlayer(int newScore=0)
    {
        if (newScore > currentHighScore)
        {
            currentHighScore = newScore;
        }
        
        if (!string.IsNullOrEmpty(currentPlayerName))
        {
            PlayerData existingPlayer = GetPlayerByName(currentPlayerName);

            if (existingPlayer != null)
            {
                if (currentHighScore > existingPlayer.highScore)
                {
                    existingPlayer.highScore = currentHighScore;
                }
            }
            else
            {
                PlayerData newPlayer = new PlayerData
                {
                    playerName = currentPlayerName,
                    highScore = currentHighScore
                };
                playerDatabase.players.Add(newPlayer);
            }
            SaveDatabase();
            UpdateGlobalStats();
        }
    }


    public void LoadPlayer(string playerName)
    {
        currentPlayerName = playerName;
        if (File.Exists(savePath))
        {
            PlayerData currentPlayerData = GetPlayerByName(currentPlayerName);

            if (currentPlayerData != null)
            {
                currentHighScore = currentPlayerData.highScore;
            }
        }
    }

    public PlayerData GetPlayerByName(string playerName)
    {
        return playerDatabase.players.Find(p => p.playerName == playerName);
    }

    public PlayerData GetBestPlayer()
    {
        return playerDatabase.players.OrderByDescending(p => p.highScore).FirstOrDefault();
    }

    private void UpdateGlobalStats()
    {
        PlayerData bestPlayerData = GetBestPlayer();
        if (bestPlayer != null)
        {
            bestPlayer = bestPlayerData.playerName;
            highScore = bestPlayerData.highScore;
        }
    } 

}
