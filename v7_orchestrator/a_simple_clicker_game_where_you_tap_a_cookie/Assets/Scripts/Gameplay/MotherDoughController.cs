using UnityEngine;
using System.Collections.Generic; // Not strictly needed for this version, but good for general awareness

namespace V7Games.AncestralDoughGuardian
{
    /// <summary>
    /// Manages the core logic and state of the Mother Dough, including its spiritual harmony, essence generation,
    /// dough form evolution, and interactions. This script acts as the central hub for the Mother Dough's lifecycle.
    /// </summary>
    public class MotherDoughController : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Reference to the UIManager for updating UI elements.")]
        [SerializeField] private UIManager uiManager;
        [Tooltip("Reference to the AudioManager for playing sound effects and ambient music.")]
        [SerializeField] private AudioManager audioManager;
        [Tooltip("Reference to the SaveLoadManager for persistent data storage.")]
        [SerializeField] private SaveLoadManager saveLoadManager;
        [Tooltip("Reference to the WisdomCookieController to trigger the yielding of Wisdom Cookies.")]
        [SerializeField] private WisdomCookieController wisdomCookieController;
        [Tooltip("Reference to the Renderer component of the Mother Dough (e.g., MeshRenderer).")]
        [SerializeField] private Renderer doughRenderer;
        [Tooltip("Prefab for the particle effect to play when Chi is infused into the dough.")]
        [SerializeField] private ParticleSystem chiInfusionEffectPrefab; 

        [Header("Dough Configuration")]
        [Tooltip("Ordered list of Dough Form ScriptableObjects, defining the progression and visuals.")]
        [SerializeField] private DoughFormConfig[] doughFormConfigs;
        [Tooltip("The amount of Spiritual Harmony gained with each active tap interaction.")]
        [SerializeField] private float tapHarmonyGain = 1.0f;
        [Tooltip("The amount of Dough Essence gained with each active tap interaction.")]
        [SerializeField] private float tapEssenceGain = 1.0f;
        [Tooltip("The rate at which Spiritual Harmony passively increases per second (idle progression).")]
        [SerializeField] private float passiveHarmonyPerSecond = 0.1f;
        [Tooltip("The rate at which Dough Essence passively increases per second (idle progression).")]
        [SerializeField] private float passiveEssencePerSecond = 0.1f;
        [Tooltip("The cumulative Spiritual Harmony required to yield a Wisdom Cookie.")]
        [SerializeField] private float wisdomCookieYieldThreshold = 100.0f; 

        // Internal State Variables
        private float _currentSpiritualHarmony;
        private float _currentDoughEssence;
        private int _currentDoughFormIndex;
        private float _harmonySinceLastWisdomCookie; // Tracks harmony specifically for Wisdom Cookie yields

        /// <summary>
        /// Gets the current spiritual harmony of the Mother Dough.
        /// </summary>
        public float CurrentSpiritualHarmony => _currentSpiritualHarmony;

        /// <summary>
        /// Gets the current dough essence of the Mother Dough.
        /// </summary>
        public float CurrentDoughEssence => _currentDoughEssence;

        /// <summary>
        /// Gets the configuration of the Mother Dough's current visual form.
        /// </summary>
        public DoughFormConfig CurrentDoughForm => doughFormConfigs[_currentDoughFormIndex];

        /// <summary>
        /// Called when the script instance is being loaded.
        /// This is where dependencies are validated and initial game state is loaded.
        /// </summary>
        private void Awake()
        {
            // Essential dependency checks to prevent runtime errors
            ValidateDependencies();

            // Load saved game data to restore the dough's state
            GameData loadedData = saveLoadManager.LoadGameData();
            LoadData(loadedData);
        }

        /// <summary>
        /// Called on the first frame a script is enabled.
        /// Applies the current dough form's visuals and updates the UI.
        /// </summary>
        private void Start()
        {
            ApplyDoughForm(_currentDoughFormIndex); // Ensure the dough's appearance matches its loaded state
            UpdateUI(); // Refresh all UI elements to reflect the current state
        }

        /// <summary>
        /// Called every frame. Used for continuous, passive resource generation.
        /// </summary>
        private void Update()
        {
            // Increment harmony and essence passively over time
            IncreaseSpiritualHarmony(passiveHarmonyPerSecond * Time.deltaTime);
            IncreaseDoughEssence(passiveEssencePerSecond * Time.deltaTime);
        }

        /// <summary>
        /// Validates that all essential serialized fields are assigned in the Inspector.
        /// Logs errors if any dependency is missing, aiding in setup.
        /// </summary>
        private void ValidateDependencies()
        {
            if (uiManager == null) Debug.LogError("UIManager is not assigned in MotherDoughController. Please assign it in the Inspector.", this);
            if (audioManager == null) Debug.LogError("AudioManager is not assigned in MotherDoughController. Please assign it in the Inspector.", this);
            if (saveLoadManager == null) Debug.LogError("SaveLoadManager is not assigned in MotherDoughController. Please assign it in the Inspector.", this);
            if (wisdomCookieController == null) Debug.LogError("WisdomCookieController is not assigned in MotherDoughController. Please assign it in the Inspector.", this);
            if (doughRenderer == null) Debug.LogError("Dough Renderer is not assigned in MotherDoughController. Please assign it in the Inspector.", this);
            if (doughFormConfigs == null || doughFormConfigs.Length == 0) Debug.LogError("Dough Form Configs are not assigned or empty in MotherDoughController. Please assign them in the Inspector.", this);
        }

        /// <summary>
        /// Public method to be invoked when the Mother Dough is actively tapped or interacted with.
        /// Triggers harmony/essence gain, plays SFX, and initiates visual feedback.
        /// </summary>
        public void OnDoughTap()
        {
            HandleDoughInteraction(tapHarmonyGain, tapEssenceGain); // Process the tap's effects
            audioManager.PlaySFX(audioManager.tapSFX); // Play the subtle tap sound effect

            // Instantiate and play the Chi Infusion visual effect
            if (chiInfusionEffectPrefab != null)
            {
                ParticleSystem vfxInstance = Instantiate(chiInfusionEffectPrefab, transform.position, Quaternion.identity);
                vfxInstance.Play();
                // Destroy the VFX GameObject after its duration to prevent scene clutter
                Destroy(vfxInstance.gameObject, vfxInstance.main.duration); 
            }
        }

        /// <summary>
        /// Centralized method to handle any type of interaction that increases dough attributes.
        /// </summary>
        /// <param name="harmonyAmount">The amount of spiritual harmony to add from this interaction.</param>
        /// <param name="essenceAmount">The amount of dough essence to add from this interaction.</param>
        private void HandleDoughInteraction(float harmonyAmount, float essenceAmount)
        {
            IncreaseSpiritualHarmony(harmonyAmount);
            IncreaseDoughEssence(essenceAmount);
            CheckForWisdomCookieYield(); // Determine if a Wisdom Cookie should be manifested
            UpdateUI(); // Ensure the UI reflects the latest state after interaction
        }

        /// <summary>
        /// Increases the spiritual harmony of the Mother Dough.
        /// This method also checks for potential dough form evolutions.
        /// </summary>
        /// <param name="amount">The amount of harmony to add.</param>
        private void IncreaseSpiritualHarmony(float amount)
        {
            _currentSpiritualHarmony += amount;
            _harmonySinceLastWisdomCookie += amount; // Accumulate harmony for Wisdom Cookie checks
            CheckForDoughFormChange(); // Check if the dough is ready to transform
            uiManager.UpdateSpiritualHarmonyUI(_currentSpiritualHarmony); // Update the harmony display in UI
        }

        /// <summary>
        /// Increases the dough essence. Made public for potential external calls (e.g., from Dojo Enhancements).
        /// </summary>
        /// <param name="amount">The amount of essence to add.</param>
        public void IncreaseDoughEssence(float amount)
        {
            _currentDoughEssence += amount;
            uiManager.UpdateDoughEssenceUI(_currentDoughEssence); // Update the essence display in UI
        }

        /// <summary>
        /// Checks if the Mother Dough's spiritual harmony has reached the threshold for the next form.
        /// If so, it triggers the evolution to the next dough form.
        /// </summary>
        private void CheckForDoughFormChange()
        {
            // Only proceed if there are more forms to unlock
            if (_currentDoughFormIndex + 1 < doughFormConfigs.Length)
            {
                DoughFormConfig nextForm = doughFormConfigs[_currentDoughFormIndex + 1];
                if (_currentSpiritualHarmony >= nextForm.requiredHarmony)
                {
                    _currentDoughFormIndex++; // Advance to the next form
                    ApplyDoughForm(_currentDoughFormIndex); // Apply the new form's visuals and audio
                    audioManager.PlaySFX(audioManager.gongChimeSFX); // Play a special chime for evolution
                    Debug.Log($"The Mother Dough has gracefully transformed into the {CurrentDoughForm.doughName}!");
                    uiManager.ShowNotification($"The Mother Dough resonates anew! It has transformed into the {CurrentDoughForm.doughName}!"); // Notify player
                }
            }
        }

        /// <summary>
        /// Applies the visual (material) and ambient audio properties of the specified dough form.
        /// </summary>
        /// <param name="formIndex">The index of the dough form to apply from the `doughFormConfigs` array.</param>
        private void ApplyDoughForm(int formIndex)
        {
            if (formIndex >= 0 && formIndex < doughFormConfigs.Length)
            {
                DoughFormConfig selectedForm = doughFormConfigs[formIndex];

                // Update the dough's material
                if (doughRenderer != null && selectedForm.doughMaterial != null)
                {
                    doughRenderer.material = selectedForm.doughMaterial;
                }

                // Update the ambient music to match the dough form's theme
                if (audioManager != null)
                {
                    audioManager.SetAmbientMusic(selectedForm.ambientMusic); 
                }

                Debug.Log($"Applying Dough Form: {selectedForm.doughName}");
            }
            else
            {
                Debug.LogError($"Attempted to apply an invalid dough form index: {formIndex}. Please check the doughFormConfigs array.", this);
            }
        }

        /// <summary>
        /// Checks if enough harmony has been accumulated since the last Wisdom Cookie yield.
        /// If so, it instructs the WisdomCookieController to manifest a new cookie and resets the counter.
        /// </summary>
        private void CheckForWisdomCookieYield()
        {
            if (_harmonySinceLastWisdomCookie >= wisdomCookieYieldThreshold)
            {
                wisdomCookieController.YieldWisdomCookie(); // Trigger the Wisdom Cookie manifestation
                _harmonySinceLastWisdomCookie = 0; // Reset the counter for the next cookie
                Debug.Log("Mother Dough has gently yielded a Wisdom Cookie!");
                uiManager.ShowNotification("A new Wisdom Cookie has manifested! Tap it to unveil its insight."); // Notify player
            }
        }

        /// <summary>
        /// Updates all relevant UI elements to reflect the current state of the Mother Dough.
        /// </summary>
        private void UpdateUI()
        {
            uiManager.UpdateSpiritualHarmonyUI(_currentSpiritualHarmony);
            uiManager.UpdateDoughEssenceUI(_currentDoughEssence);
            uiManager.UpdateDoughFormNameUI(CurrentDoughForm.doughName);
            // Additional UI updates can be added here as needed (e.g., progress to next form)
        }

        /// <summary>
        /// Gathers the current state of the Mother Dough for saving game progress.
        /// </summary>
        /// <returns>A GameData object containing the current state of the Mother Dough.</returns>
        public GameData GetSaveData()
        {
            // Populate a GameData object with the current state values
            return new GameData
            {
                spiritualHarmony = _currentSpiritualHarmony,
                doughEssence = _currentDoughEssence,
                doughFormIndex = _currentDoughFormIndex,
                harmonySinceLastWisdomCookie = _harmonySinceLastWisdomCookie
                // Expand GameData with any other relevant state variables from this controller
            };
        }

        /// <summary>
        /// Loads the game state for the Mother Dough from a provided GameData object.
        /// This is typically called during game initialization or loading from a save file.
        /// </summary>
        /// <param name="data">The GameData object from which to load the state.</param>
        public void LoadData(GameData data)
        {
            _currentSpiritualHarmony = data.spiritualHarmony;
            _currentDoughEssence = data.doughEssence;
            _currentDoughFormIndex = data.doughFormIndex;
            _harmonySinceLastWisdomCookie = data.harmonySinceLastWisdomCookie;

            // Ensure the loaded dough form index is within valid bounds
            if (_currentDoughFormIndex >= doughFormConfigs.Length)
            {
                _currentDoughFormIndex = doughFormConfigs.Length - 1; // Cap at the highest available form
            }
            if (_currentDoughFormIndex < 0)
            {
                _currentDoughFormIndex = 0; // Default to the first form if somehow negative
            }

            ApplyDoughForm(_currentDoughFormIndex); // Immediately apply the loaded form's appearance
            UpdateUI(); // Refresh UI to reflect the loaded state
            Debug.Log("MotherDoughController loaded data successfully.");
        }
    }
}