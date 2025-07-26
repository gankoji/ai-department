using UnityEngine;
using UnityEngine.UI;
using TMPro; // Assuming TextMeshPro for text display
using System; // For Action events

namespace V7Games.UI
{
    /// <summary>
    /// Manages the main game UI elements for "The Ancestral Dough Guardian."
    /// This script is responsible for displaying the current state of the Mother Dough's
    /// Spiritual Harmony and Dough Essence, handling the primary "Meditative Knead" interaction,
    /// and providing navigation to other core UI screens like the Scroll of Enlightenment and Dojo Enhancements.
    /// It ensures a responsive and intuitive interface for the player's serene journey.
    /// </summary>
    public class MainGameUI : MonoBehaviour
    {
        [Header("UI Element References")]
        [Tooltip("The TextMeshProUGUI component displaying the current Spiritual Harmony.")]
        [SerializeField] private TextMeshProUGUI _spiritualHarmonyText;

        [Tooltip("The TextMeshProUGUI component displaying the current Dough Essence.")]
        [SerializeField] private TextMeshProUGUI _doughEssenceText;

        [Tooltip("The button for performing the 'Meditative Knead' action on the Mother Dough.")]
        [SerializeField] private Button _meditativeKneadButton;

        [Tooltip("The Slider component visualizing the progress towards the next Dough Form or Wisdom Cookie.")]
        [SerializeField] private Slider _harmonyProgressSlider;

        [Tooltip("The button to navigate to the Scroll of Enlightenment UI.")]
        [SerializeField] private Button _scrollButton;

        [Tooltip("The button to navigate to the Dojo Enhancement UI.")]
        [SerializeField] private Button _dojoEnhancementButton;

        [Header("Controller References")]
        [Tooltip("Reference to the MotherDoughController to interact with core gameplay logic.")]
        [SerializeField] private V7Games.Gameplay.MotherDoughController _motherDoughController;

        [Tooltip("Reference to the UIManager for controlling UI screen visibility.")]
        [SerializeField] private V7Games.Managers.UIManager _uiManager;

        [Tooltip("Reference to the AudioManager for playing UI-related sound effects.")]
        [SerializeField] private V7Games.Managers.AudioManager _audioManager;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// This is used to cache component references and attach listeners to UI buttons.
        /// Error checks are performed to ensure all required references are assigned in the Inspector.
        /// </summary>
        private void Awake()
        {
            // --- Component Reference Validation ---
            if (_spiritualHarmonyText == null) Debug.LogError("Spiritual Harmony Text (TextMeshProUGUI) not assigned in MainGameUI.", this);
            if (_doughEssenceText == null) Debug.LogError("Dough Essence Text (TextMeshProUGUI) not assigned in MainGameUI.", this);
            if (_meditativeKneadButton == null) Debug.LogError("Meditative Knead Button not assigned in MainGameUI.", this);
            if (_harmonyProgressSlider == null) Debug.LogError("Harmony Progress Slider not assigned in MainGameUI.", this);
            if (_scrollButton == null) Debug.LogError("Scroll Button not assigned in MainGameUI.", this);
            if (_dojoEnhancementButton == null) Debug.LogError("Dojo Enhancement Button not assigned in MainGameUI.", this);
            if (_motherDoughController == null) Debug.LogError("Mother Dough Controller not assigned in MainGameUI. Ensure it's in the scene and assigned.", this);
            if (_uiManager == null) Debug.LogError("UI Manager not assigned in MainGameUI. Ensure it's in the scene and assigned.", this);
            if (_audioManager == null) Debug.LogError("Audio Manager not assigned in MainGameUI. Ensure it's in the scene and assigned.", this);

            // --- Button Event Listeners ---
            _meditativeKneadButton.onClick.AddListener(OnMeditativeKnead);
            _scrollButton.onClick.AddListener(OnScrollButtonClicked);
            _dojoEnhancementButton.onClick.AddListener(OnDojoEnhancementButtonClicked);
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// This is the ideal place to subscribe to events from other systems, ensuring
        /// that UI updates are driven by data changes rather than constant polling in Update().
        /// An initial UI refresh is performed to display current values immediately.
        /// </summary>
        private void OnEnable()
        {
            if (_motherDoughController != null)
            {
                _motherDoughController.OnSpiritualHarmonyChanged += UpdateSpiritualHarmonyUI;
                _motherDoughController.OnDoughEssenceChanged += UpdateDoughEssenceUI;
                _motherDoughController.OnHarmonyProgressChanged += UpdateHarmonyProgressUI;
            }

            // Initial UI update to reflect current game state
            if (_motherDoughController != null)
            {
                UpdateSpiritualHarmonyUI(_motherDoughController.CurrentSpiritualHarmony);
                UpdateDoughEssenceUI(_motherDoughController.CurrentDoughEssence);
                UpdateHarmonyProgressUI(_motherDoughController.CurrentHarmonyProgressNormalized);
            }
        }

        /// <summary>
        /// Called when the behaviour becomes disabled or inactive.
        /// Crucially, this method unsubscribes from events to prevent potential memory leaks
        /// and ensure that this UI script does not attempt to update after it has been deactivated.
        /// </summary>
        private void OnDisable()
        {
            if (_motherDoughController != null)
            {
                _motherDoughController.OnSpiritualHarmonyChanged -= UpdateSpiritualHarmonyUI;
                _motherDoughController.OnDoughEssenceChanged -= UpdateDoughEssenceUI;
                _motherDoughController.OnHarmonyProgressChanged -= UpdateHarmonyProgressUI;
            }
        }

        /// <summary>
        /// Handles the click event for the "Meditative Knead" button.
        /// Invokes the core "Meditative Knead" action on the MotherDoughController
        /// and plays a subtle sound effect to provide audio feedback.
        /// </summary>
        private void OnMeditativeKnead()
        {
            _motherDoughController.PerformMeditativeKnead();
            _audioManager.PlaySFX("TapSFX"); // Play a subtle, calming tap sound
        }

        /// <summary>
        /// Updates the displayed Spiritual Harmony value in the UI.
        /// This method is typically called in response to an event from the MotherDoughController.
        /// </summary>
        /// <param name="harmonyValue">The new Spiritual Harmony value to display.</param>
        private void UpdateSpiritualHarmonyUI(long harmonyValue)
        {
            if (_spiritualHarmonyText != null)
            {
                _spiritualHarmonyText.text = $"Harmony: {harmonyValue}";
            }
        }

        /// <summary>
        /// Updates the displayed Dough Essence value in the UI.
        /// This method is typically called in response to an event from the MotherDoughController.
        /// </summary>
        /// <param name="essenceValue">The new Dough Essence value to display.</param>
        private void UpdateDoughEssenceUI(long essenceValue)
        {
            if (_doughEssenceText != null)
            {
                _doughEssenceText.text = $"Essence: {essenceValue}";
            }
        }

        /// <summary>
        /// Updates the value of the harmony progress slider.
        /// This visualizes the player's progress towards the next significant milestone (e.g., a new Dough Form or Wisdom Cookie).
        /// </summary>
        /// <param name="normalizedProgress">The progress value, clamped between 0 and 1, representing completion towards the next stage.</param>
        private void UpdateHarmonyProgressUI(float normalizedProgress)
        {
            if (_harmonyProgressSlider != null)
            {
                _harmonyProgressSlider.value = normalizedProgress;
            }
        }

        /// <summary>
        /// Handles the click event for the "Scroll" button.
        /// Instructs the UIManager to display the Scroll of Enlightenment UI and plays a navigation sound.
        /// </summary>
        private void OnScrollButtonClicked()
        {
            _uiManager.ShowUI(Managers.UIManager.UIType.ScrollOfEnlightenment);
            _audioManager.PlaySFX("UINavigateSFX"); // Assuming a generic UI navigation sound
        }

        /// <summary>
        /// Handles the click event for the "Dojo Enhancement" button.
        /// Instructs the UIManager to display the Dojo Enhancement UI and plays a navigation sound.
        /// </summary>
        private void OnDojoEnhancementButtonClicked()
        {
            _uiManager.ShowUI(Managers.UIManager.UIType.DojoEnhancement);
            _audioManager.PlaySFX("UINavigateSFX"); // Assuming a generic UI navigation sound
        }
    }
}