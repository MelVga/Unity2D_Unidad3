using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    private FirebaseFirestore db;

    public int coins = 0;
    public bool level1Completed = false;
    public bool level2Completed = false;

    private string playerId = "player1";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            DependencyStatus dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
                Debug.Log("Firebase conectado");
                LoadData();
            }
            else
            {
                Debug.LogError("Firebase no disponible: " + dependencyStatus);
                LoadLocalData();
            }
        });
    }

    public void AddCoin(int amount = 1)
    {
        coins += amount;
        SaveLocalData();
        SaveData();
    }

    public void CompleteLevel(int levelNumber)
    {
        if (levelNumber == 1) level1Completed = true;
        if (levelNumber == 2) level2Completed = true;

        SaveLocalData();
        SaveData();
    }

    public void SaveData()
    {
        if (db == null) return;

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "coins", coins },
            { "level1Completed", level1Completed },
            { "level2Completed", level2Completed }
        };

        db.Collection("players").Document(playerId).SetAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("Datos guardados en Firestore");
            }
            else
            {
                Debug.LogError("Error al guardar en Firestore: " + task.Exception);
            }
        });
    }

    public void LoadData()
    {
        if (db == null) return;

        db.Collection("players").Document(playerId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                var snapshot = task.Result;

                if (snapshot.Exists)
                {
                    coins = snapshot.GetValue<int>("coins");
                    level1Completed = snapshot.GetValue<bool>("level1Completed");
                    level2Completed = snapshot.GetValue<bool>("level2Completed");

                    SaveLocalData();
                    Debug.Log("Datos cargados desde Firestore");
                }
                else
                {
                    Debug.Log("No hay datos previos en Firestore");
                    LoadLocalData();
                }
            }
            else
            {
                Debug.LogError("Error al cargar Firestore: " + task.Exception);
                LoadLocalData();
            }
        });
    }

    public void SaveLocalData()
    {
        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.SetInt("level1Completed", level1Completed ? 1 : 0);
        PlayerPrefs.SetInt("level2Completed", level2Completed ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadLocalData()
    {
        coins = PlayerPrefs.GetInt("coins", 0);
        level1Completed = PlayerPrefs.GetInt("level1Completed", 0) == 1;
        level2Completed = PlayerPrefs.GetInt("level2Completed", 0) == 1;

        Debug.Log("Datos cargados desde PlayerPrefs");
    }
}