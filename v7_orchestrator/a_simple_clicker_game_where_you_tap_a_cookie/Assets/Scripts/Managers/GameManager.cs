using UnityEngine;
using System;
using System.Collections.Generic;

namespace V7Games.ZenFarmManager
{
    /// <summary>
    /// Manages the overall game state, core mechanics, and progression for The Ancestral Dough Guardian.
    /// This central hub connects various game systems, ensuring a harmonious and serene player experience.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // --- Singleton Instance ---
        /// <summary>
        /// The static instance of the GameManager, ensuring only one exists throughout the game.
        /// </summary>
        public static GameManager Instance { get; private set; }

        // --- Manager References ---
        [Tooltip("Reference to the UIManager for handling all UI updates.")]
        [SerializeField] private UIManager uiManager;
        [Tooltip("Reference to the AudioManager for playing sound effects and music.")]
        [SerializeField] private AudioManager audioManager;
        [Tooltip("Reference to the SaveLoadManager for persisting game data.")]
        [SerializeField] private SaveLoadManager saveLoadManager;
        [Tooltip("Reference to the MotherDoughController for visual and interactive dough management.")]
        [SerializeField] private MotherDoughController motherDoughController;
        [Tooltip("Reference to the WisdomCookieController for spawning and managing Wisdom Cookies.")]
        [SerializeField] private WisdomCookieController wisdomCookieController;

        // --- Core Game Data Attributes ---
        [Header("Dough Attributes")]
        [Tooltip("The current spiritual harmony level of the Mother Dough. Increases with meditative kneads.")]
        [SerializeField] private double spiritualHarmony = 0;
        [Tooltip("The current amount of Dough Essence, used as currency for dojo enhancements.")]
        [SerializeField] private double doughEssence = 0;
        [Tooltip("The index of the current visual form of the Mother Dough.")]
        [SerializeField] private int currentDoughFormIndex = 0;
        [Tooltip("The total number of Wisdom Cookies collected throughout the game.")]
        [SerializeField] private int wisdomCookiesCollected = 0;
        [Tooltip("Counts the number of taps since the last Wisdom Cookie was generated.")]
        [SerializeField] private int tapsSinceLastWisdomCookie = 0;

        // --- Game Configuration Parameters ---
        [Header("Game Configuration")]
        [Tooltip("Passive Spiritual Harmony generated per second, even when idle.")]
        [SerializeField] private float passiveHarmonyPerSecond = 0.1f;
        [Tooltip("Passive Dough Essence generated per second, even when idle.")]
        [SerializeField] private float passiveEssencePerSecond = 0.05f;
        [Tooltip("The amount of Spiritual Harmony gained with each meditative tap.")]
        [SerializeField] private float tapHarmonyGain = 1.0f;
        [Tooltip("The amount of Dough Essence gained with each meditative tap.")]
        [SerializeField] private float tapEssenceGain = 0.5f;
        [Tooltip("The number of taps required to potentially trigger the manifestation of a Wisdom Cookie.")]
        [SerializeField] private int tapsForWisdomCookie = 100;

        // --- Scriptable Object References for Configuration Data ---
        [Header("Configuration Data")]
        [Tooltip("A list of DoughFormConfig ScriptableObjects, defining each stage of the Mother Dough's evolution.")]
        [SerializeField] private List<DoughFormConfig> doughFormConfigs;
        [Tooltip("A list of WisdomCookieConfig ScriptableObjects, defining each unique Wisdom Cookie.")]
        [SerializeField] private List<WisdomCookieConfig> wisdomCookieConfigs;
        [Tooltip("A list of DojoItemConfig ScriptableObjects, defining available enhancements for the dojo.")]
        [SerializeField] private List<DojoItemConfig> dojoItemConfigs;

        // --- Events for UI and System Updates ---
        /// <summary>
        /// Event fired when Spiritual Harmony changes. Provides the new harmony value.
        /// Systems like UI can subscribe to this for updates.
        /// </summary>
        public static event Action<double> OnSpiritualHarmonyChanged;
        /// <summary>
        /// Event fired when Dough Essence changes. Provides the new essence value.
        /// Systems like UI can subscribe to this for updates.
        /// </summary>
        public static event Action<double> OnDoughEssenceChanged;
        /// <summary>
        /// Event fired when the Mother Dough's form changes. Provides the new DoughFormConfig.
        /// Systems like the MotherDoughController can subscribe to this for visual updates.
        /// </summary>
        public static event Action<DoughFormConfig> OnDoughFormChanged;
        /// <summary>
        /// Event fired when a Wisdom Cookie is generated. Provides the WisdomCookieConfig that manifested.
        /// Systems like the WisdomCookieController or UI can subscribe for pop-ups/animations.
        /// </summary>
        public static event Action<WisdomCookieConfig> OnWisdomCookieGenerated;
        /// <summary>
        /// Event fired when a Dojo item is successfully purchased. Provides the purchased DojoItemConfig.
        /// Systems like UI or visualizers can subscribe to this for updates.
        /// </summary>
        public static event Action<DojoItemConfig> OnDojoItemPurchased;


        /// <summary>
        /// Called when the script instance is being loaded. Initializes the singleton and checks for essential references.
        /// </summary>
        private void Awake()
        {
            // Enforce Singleton pattern: ensure only one GameManager instance exists.
            if (Instance == null)
            {
                Instance = this;
                // Keep GameManager alive across scene loads as it manages global game state.
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // If another instance already exists, destroy this one to prevent duplicates.
                Destroy(gameObject);
                return;
            }

            // Runtime checks to ensure all essential manager references are set in the Inspector.
            // This helps prevent NullReferenceExceptions during gameplay.
            if (uiManager == null) Debug.LogError("UIManager reference not set in GameManager!", this);
            if (audioManager == null) Debug.LogError("AudioManager reference not set in GameManager!", this);
            if (saveLoadManager == null) Debug.LogError("SaveLoadManager reference not set in GameManager!", this);
            if (motherDoughController == null) Debug.LogError("MotherDoughController reference not set in GameManager!", this);
            if (wisdomCookieController == null) Debug.LogError("WisdomCookieController reference not set in GameManager!", this);

            // Optional: Initialize other managers if they have their own setup methods.
            // uiManager?.Initialize();
            // audioManager?.Initialize();
            // saveLoadManager?.Initialize();
        }

        /// <summary>
        /// Called on the frame when a script is enabled just before any Update methods are called the first time.
        /// Attempts to load previous game data and initializes the game state based on it.
        /// </summary>
        private void Start()
        {
            LoadGame(); // Attempt to load any previously saved game data.

            // Ensure UI and visual elements are updated to reflect the loaded or default game state.
            OnSpiritualHarmonyChanged?.Invoke(SpiritualHarmony);
            OnDoughEssenceChanged?.Invoke(DoughEssence);
            UpdateDoughFormVisual(); // Set the Mother Dough's visual form.
        }

        /// <summary>
        /// Update is called once per frame. Handles passive progression of game attributes.
        /// </summary>
        private void Update()
        {
            // Increment Spiritual Harmony and Dough Essence passively over time, reflecting the dough's inherent vitality.
            AddSpiritualHarmony(passiveHarmonyPerSecond * Time.deltaTime);
            AddDoughEssence(passiveEssencePerSecond * Time.deltaTime);
        }

        /// <summary>
        /// Adds a specified amount of Spiritual Harmony to the Mother Dough.
        /// Triggers an event and checks for dough form progression.
        /// </summary>
        /// <param name="amount">The quantity of harmony to add. Must be non-negative.</param>
        public void AddSpiritualHarmony(double amount)
        {
            if (amount < 0) return; // Prevent unintended negative additions.
            spiritualHarmony += amount;
            OnSpiritualHarmonyChanged?.Invoke(spiritualHarmony); // Notify listeners of the change.
            CheckForDoughFormProgression(); // Check if the dough is ready to evolve.
        }

        /// <summary>
        /// Adds a specified amount of Dough Essence to the player's currency.
        /// Triggers an event to update UI elements.
        /// </summary>
        /// <param name="amount">The quantity of essence to add. Must be non-negative.</param>
        public void AddDoughEssence(double amount)
        {
            if (amount < 0) return; // Prevent unintended negative additions.
            doughEssence += amount;
            OnDoughEssenceChanged?.Invoke(doughEssence); // Notify listeners of the change.
        }

        /// <summary>
        /// Handles the player's meditative tap interaction with the Mother Dough.
        /// This is the core active gameplay loop.
        /// </summary>
        public void OnDoughTapped()
        {
            // Play a gentle sound effect to accompany the tap, enhancing the tactile feedback.
            audioManager?.PlaySFX(audioManager.tapSFX);

            // Apply the immediate gains from a meditative tap.
            AddSpiritualHarmony(tapHarmonyGain);
            AddDoughEssence(tapEssenceGain);

            // Increment the counter for Wisdom Cookie generation and check if one is due.
            tapsSinceLastWisdomCookie++;
            CheckForWisdomCookieGeneration();

            // Notify the MotherDoughController to provide visual feedback for the tap (e.g., subtle pulsation).
            motherDoughController?.OnDoughTappedFeedback();
        }

        /// <summary>
        /// Checks if the Mother Dough has accumulated enough Spiritual Harmony to evolve to its next form.
        /// If so, it updates the dough's form and triggers relevant visual/audio feedback.
        /// </summary>
        private void CheckForDoughFormProgression()
        {
            // Ensure there is a next form available in the configuration list.
            if (currentDoughFormIndex + 1 < doughFormConfigs.Count)
            {
                DoughFormConfig nextForm = doughFormConfigs[currentDoughFormIndex + 1];
                // If current harmony meets or exceeds the requirement for the next form.
                if (spiritualHarmony >= nextForm.requiredHarmony)
                {
                    currentDoughFormIndex++; // Advance to the next form.
                    UpdateDoughFormVisual(); // Apply the new visual state.
                    Debug.Log($"Mother Dough gracefully evolved to Form: {nextForm.formName}! A new stage of enlightenment.");
                }
            }
        }

        /// <summary>
        /// Updates the Mother Dough's visual representation and ambient soundscape based on its current form index.
        /// </summary>
        private void UpdateDoughFormVisual()
        {
            // Ensure the current index is within the bounds of available dough forms.
            if (currentDoughFormIndex >= 0 && currentDoughFormIndex < doughFormConfigs.Count)
            {
                DoughFormConfig currentForm = doughFormConfigs[currentDoughFormIndex];
                // Instruct the MotherDoughController to apply the new visual and perhaps ambient sounds.
                motherDoughController?.SetDoughForm(currentForm);
                OnDoughFormChanged?.Invoke(currentForm); // Notify listeners of the form change.
                audioManager?.PlaySFX(audioManager.gongChimeSFX); // Play a resonant chime for this significant event.
            }
            else
            {
                Debug.LogWarning("Attempted to set Mother Dough form using an invalid index. Check DoughFormConfigs.", this);
            }
        }

        /// <summary>
        /// Checks if the conditions are met to generate a new Wisdom Cookie.
        /// Currently based on tap count, but can be expanded with harmony thresholds or other criteria.
        /// </summary>
        private void CheckForWisdomCookieGeneration()
        {
            // If enough taps have occurred since the last cookie, or if specific harmony thresholds are met.
            if (tapsSinceLastWisdomCookie >= tapsForWisdomCookie)
            {
                GenerateWisdomCookie();
                tapsSinceLastWisdomCookie = 0; // Reset the tap counter for the next cookie.
            }
        }

        /// <summary>
        /// Generates a new Wisdom Cookie based on the progression of collected cookies.
        /// Spawns it visually and notifies listeners.
        /// </summary>
        private void GenerateWisdomCookie()
        {
            // Ensure there are still unique Wisdom Cookies left to collect.
            if (wisdomCookiesCollected < wisdomCookieConfigs.Count)
            {
                WisdomCookieConfig newWisdomCookie = wisdomCookieConfigs[wisdomCookiesCollected];
                // Instruct the WisdomCookieController to visually spawn the cookie.
                wisdomCookieController?.SpawnWisdomCookie(newWisdomCookie);
                wisdomCookiesCollected++; // Increment the count of collected cookies.
                OnWisdomCookieGenerated?.Invoke(newWisdomCookie); // Notify listeners (e.g., UI for a pop-up).
                Debug.Log($"A new Wisdom Cookie has manifested: '{newWisdomCookie.cookieName}'! Its insights await.");
                audioManager?.PlaySFX(audioManager.gongChimeSFX); // Play a special SFX for cookie manifestation.
            }
            else
            {
                Debug.Log("All unique Wisdom Cookies collected! The Scroll of Enlightenment is complete.", this);
                // Future expansion: potentially loop through a set of "endless" wisdom, or provide a different endgame reward.
            }
        }

        /// <summary>
        /// Attempts to purchase a Dojo enhancement item using Dough Essence.
        /// Applies the item's benefits and triggers relevant events.
        /// </summary>
        /// <param name="itemConfig">The configuration of the item to purchase.</param>
        /// <returns>True if the purchase was successful, false otherwise (e.g., insufficient essence).</returns>
        public bool PurchaseDojoItem(DojoItemConfig itemConfig)
        {
            if (itemConfig == null)
            {
                Debug.LogError("Attempted to purchase a null DojoItemConfig. Please ensure the item configuration is valid.", this);
                return false;
            }

            // Check if the player has enough Dough Essence to afford the item.
            if (doughEssence >= itemConfig.cost)
            {
                doughEssence -= itemConfig.cost; // Deduct the cost.
                OnDoughEssenceChanged?.Invoke(doughEssence); // Update UI.

                // Apply the item's passive benefits to the game's core generation rates.
                passiveHarmonyPerSecond += itemConfig.passiveHarmonyBoost;
                passiveEssencePerSecond += itemConfig.passiveEssenceBoost;

                // Future expansion: Track purchased items (e.g., in a list of IDs) to reapply on load or for UI display.
                // purchasedDojoItemIds.Add(itemConfig.itemId);

                OnDojoItemPurchased?.Invoke(itemConfig); // Notify listeners of the successful purchase.
                Debug.Log($"Successfully purchased Dojo item: '{itemConfig.itemName}'. Remaining Essence: {doughEssence:F2}");
                audioManager?.PlaySFX(audioManager.gongChimeSFX); // Play a pleasant chime for a successful transaction.
                return true;
            }
            else
            {
                Debug.Log($"Insufficient Dough Essence to purchase '{itemConfig.itemName}'. Needed: {itemConfig.cost:F2}, Have: {doughEssence:F2}", this);
                uiManager?.ShowNotification("Not enough Essence! Meditate further to accumulate more.", UIManager.NotificationType.Warning); // Provide player feedback.
                return false;
            }
        }

        /// <summary>
        /// Saves the current game progress to persistent storage via the SaveLoadManager.
        /// </summary>
        public void SaveGame()
        {
            // Create a new GameData object with the current state of core game variables.
            GameData data = new GameData
            {
                spiritualHarmony = this.spiritualHarmony,
                doughEssence = this.doughEssence,
                currentDoughFormIndex = this.currentDoughFormIndex,
                wisdomCookiesCollected = this.wisdomCookiesCollected,
                tapsSinceLastWisdomCookie = this.tapsSinceLastWisdomCookie,
                // Add any other relevant game state variables here, e.g., a list of purchased dojo item IDs.
                // purchasedDojoItemIds = new List<string>(dojoItemsOwned)
            };
            saveLoadManager?.SaveGameData(data); // Delegate the actual saving to the SaveLoadManager.
            Debug.Log("Game progress saved successfully.");
            uiManager?.ShowNotification("Progress saved!", UIManager.NotificationType.Info);
        }

        /// <summary>
        /// Loads previous game progress from persistent storage via the SaveLoadManager.
        /// Resets the game state to the loaded values.
        /// </summary>
        public void LoadGame()
        {
            GameData data = saveLoadManager?.LoadGameData(); // Attempt to load game data.

            if (data != null)
            {
                // Assign loaded values to the GameManager's properties.
                this.spiritualHarmony = data.spiritualHarmony;
                this.doughEssence = data.doughEssence;
                this.currentDoughFormIndex = data.currentDoughFormIndex;
                this.wisdomCookiesCollected = data.wisdomCookiesCollected;
                this.tapsSinceLastWisdomCookie = data.tapsSinceLastWisdomCookie;

                // Future expansion: If tracking purchased items, reapply their passive boosts here.
                // foreach (string itemId in data.purchasedDojoItemIds)
                // {
                //     DojoItemConfig item = dojoItemConfigs.Find(i => i.itemId == itemId);
                //     if (item != null)
                //     {
                //         passiveHarmonyPerSecond += item.passiveHarmonyBoost;
                //         passiveEssencePerSecond += item.passiveEssenceBoost;
                //     }
                // }

                // Trigger events and update visuals to reflect the loaded state.
                OnSpiritualHarmonyChanged?.Invoke(spiritualHarmony);
                OnDoughEssenceChanged?.Invoke(doughEssence);
                UpdateDoughFormVisual();
                Debug.Log("Game loaded successfully. Welcome back to the dojo!");
                uiManager?.ShowNotification("Progress loaded!", UIManager.NotificationType.Info);
            }
            else
            {
                // If no save data is found or loading fails, initialize a new game.
                Debug.Log("No saved game found or failed to load. Starting a new journey with the Mother Dough.", this);
                // Ensure all core attributes are reset to their default starting values.
                spiritualHarmony = 0;
                doughEssence = 0;
                currentDoughFormIndex = 0;
                wisdomCookiesCollected = 0;
                tapsSinceLastWisdomCookie = 0;
                // Trigger events and update visuals for the new game state.
                OnSpiritualHarmonyChanged?.Invoke(spiritualHarmony);
                OnDoughEssenceChanged?.Invoke(doughEssence);
                UpdateDoughFormVisual();
            }
        }

        /// <summary>
        /// Gets the current Spiritual Harmony value of the Mother Dough.
        /// </summary>
        public double SpiritualHarmony
        {
            get => spiritualHarmony;
            private set => spiritualHarmony = value; // Private setter ensures modifications are controlled by GameManager methods.
        }

        /// <summary>
        /// Gets the current Dough Essence value.
        /// </summary>
        public double DoughEssence
        {
            get => doughEssence;
            private set => doughEssence = value; // Private setter ensures modifications are controlled by GameManager methods.
        }

        /// <summary>
        /// Gets the configuration data for the Mother Dough's current form.
        /// </summary>
        public DoughFormConfig CurrentDoughFormConfig
        {
            get
            {
                if (currentDoughFormIndex >= 0 && currentDoughFormIndex < doughFormConfigs.Count)
                {
                    return doughFormConfigs[currentDoughFormIndex];
                }
                Debug.LogWarning("Attempted to access DoughFormConfig with an invalid index. Returning null.", this);
                return null;
            }
        }

        /// <summary>
        /// Gets the total number of Wisdom Cookies that have been collected.
        /// </summary>
        public int WisdomCookiesCollected => wisdomCookiesCollected;
    }
}