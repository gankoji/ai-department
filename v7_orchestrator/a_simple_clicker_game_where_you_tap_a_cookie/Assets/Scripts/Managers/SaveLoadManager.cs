using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; // Potentially for binary, but JSON is often preferred for readability/debug
using System.Collections.Generic; // Required for List<T> if used in GameData

namespace V7Games.ZenFarmManager
{
    /// <summary>
    /// Manages saving and loading of game data.
    /// Ensures data persistence across play sessions, contributing to a calm and continuous player experience.
    /// </summary>
    public class SaveLoadManager : MonoBehaviour
    {
        public static SaveLoadManager Instance { get; private set; }

        [SerializeField]
        private GameManager gameManager; // Reference to the GameManager to access/update game state

        private string saveFilePath;
        private const string SaveFileName = "zenfarm_dough_guardian_save.json";

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Initializes the singleton instance and sets up the save file path.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes

            saveFilePath = Path.Combine(Application.persistentDataPath, SaveFileName);
            Debug.Log($"[SaveLoadManager] Save path: {saveFilePath}");
        }

        /// <summary>
        /// Attempts to save the current game data to a JSON file.
        /// This ensures the player's progress is preserved, offering peace of mind.
        /// </summary>
        public void SaveGame()
        {
            if (gameManager == null)
            {
                Debug.LogError("[SaveLoadManager] GameManager reference is missing. Cannot save game.");
                return;
            }

            try
            {
                // Ensure the GameData class is Serializable
                GameData dataToSave = gameManager.GetGameData();
                string json = JsonUtility.ToJson(dataToSave, true); // true for pretty print

                File.WriteAllText(saveFilePath, json);
                Debug.Log("[SaveLoadManager] Game data saved successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveLoadManager] Failed to save game data: {e.Message}");
            }
        }

        /// <summary>
        /// Attempts to load game data from a JSON file.
        /// If no save file exists, the game will start with default data, ensuring a smooth beginning.
        /// </summary>
        /// <returns>The loaded GameData object, or null if loading fails or no save file exists.</returns>
        public GameData LoadGame()
        {
            if (!File.Exists(saveFilePath))
            {
                Debug.Log("[SaveLoadManager] No save file found. Starting new game.");
                return null; // Indicates no save file, GameManager should initialize default data
            }

            try
            {
                string json = File.ReadAllText(saveFilePath);
                GameData loadedData = JsonUtility.FromJson<GameData>(json);

                Debug.Log("[SaveLoadManager] Game data loaded successfully.");
                return loadedData;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveLoadManager] Failed to load game data: {e.Message}");
                // Consider backing up corrupt files or attempting a simpler load strategy if this is critical
                return null; // Return null to indicate failure, GameManager should handle this
            }
        }

        /// <summary>
        /// Deletes the current save file.
        /// Useful for testing or providing a "reset progress" option, but should be used with care.
        /// </summary>
        public void DeleteSaveGame()
        {
            if (File.Exists(saveFilePath))
            {
                try
                {
                    File.Delete(saveFilePath);
                    Debug.Log("[SaveLoadManager] Save file deleted successfully.");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[SaveLoadManager] Failed to delete save file: {e.Message}");
                }
            }
            else
            {
                Debug.Log("[SaveLoadManager] No save file to delete.");
            }
        }

        /// <summary>
        /// Ensures the GameManager reference is set.
        /// This method can be called by GameManager itself during its Awake/Start if needed.
        /// </summary>
        /// <param name="manager">The GameManager instance.</param>
        public void SetGameManager(GameManager manager)
        {
            if (gameManager == null)
            {
                gameManager = manager;
                Debug.Log("[SaveLoadManager] GameManager reference set.");
            }
            else if (gameManager != manager)
            {
                Debug.LogWarning("[SaveLoadManager] GameManager reference is being reassigned. This might indicate an issue.");
                gameManager = manager;
            }
        }
    }
}