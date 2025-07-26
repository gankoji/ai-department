using System;
using System.Collections.Generic;

namespace V7Games.ZenFarmManager.Data
{
    /// <summary>
    /// Represents the complete game state data that needs to be saved and loaded.
    /// This class is designed to be serializable for persistence.
    /// </summary>
    [Serializable]
    public class GameData
    {
        // Core Resources
        public double DoughEssence;
        public double SpiritualHarmony;

        // Progression States
        public int CurrentDoughFormIndex; // Index corresponding to DoughFormConfig ScriptableObjects
        public List<string> UnlockedWisdomCookieIds; // IDs of Wisdom Cookies revealed
        public List<string> UnlockedDojoItemIds;     // IDs of Dojo Enhancement items purchased

        // Time-based data for idle progression
        public string LastSaveTime; // Stored as ISO 8601 string for cross-platform compatibility

        /// <summary>
        /// Initializes a new instance of the GameData class with default values.
        /// This constructor is used when starting a new game.
        /// </summary>
        public GameData()
        {
            DoughEssence = 0;
            SpiritualHarmony = 0;
            CurrentDoughFormIndex = 0; // Start with the first Dough Form
            UnlockedWisdomCookieIds = new List<string>();
            UnlockedDojoItemIds = new List<string>();
            LastSaveTime = DateTime.UtcNow.ToString("O"); // "O" for ISO 8601 format
        }

        /// <summary>
        /// Updates the last save time to the current UTC time.
        /// </summary>
        public void UpdateLastSaveTime()
        {
            LastSaveTime = DateTime.UtcNow.ToString("O");
        }

        /// <summary>
        /// Adds a Wisdom Cookie ID to the list of unlocked cookies if it's not already present.
        /// </summary>
        /// <param name="wisdomCookieId">The unique ID of the Wisdom Cookie.</param>
        public void AddUnlockedWisdomCookie(string wisdomCookieId)
        {
            if (!UnlockedWisdomCookieIds.Contains(wisdomCookieId))
            {
                UnlockedWisdomCookieIds.Add(wisdomCookieId);
            }
        }

        /// <summary>
        /// Adds a Dojo Item ID to the list of unlocked items if it's not already present.
        /// </summary>
        /// <param name="dojoItemId">The unique ID of the Dojo Item.</param>
        public void AddUnlockedDojoItem(string dojoItemId)
        {
            if (!UnlockedDojoItemIds.Contains(dojoItemId))
            {
                UnlockedDojoItemIds.Add(dojoItemId);
            }
        }
    }
}