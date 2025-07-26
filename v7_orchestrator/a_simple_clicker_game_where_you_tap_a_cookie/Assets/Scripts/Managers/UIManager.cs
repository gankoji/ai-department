using UnityEngine;
using System.Collections.Generic;

namespace V7Games.ZenDoughGuardian
{
    /// <summary>
    /// Manages the visibility and state of various UI panels within the game,
    /// ensuring a serene and intuitive player experience.
    /// Follows the Singleton pattern for easy access from other game systems.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of the UIManager.
        /// </summary>
        public static UIManager Instance { get; private set; }

        [Header("UI Canvases")]
        [Tooltip("The main game UI canvas, showing the Mother Dough and core interactions.")]
        [SerializeField] private GameObject _mainGameCanvas;
        [Tooltip("The canvas for the Scroll of Enlightenment, displaying collected wisdom.")]
        [SerializeField] private GameObject _scrollOfEnlightenmentCanvas;
        [Tooltip("The canvas for the Dojo Enhancement shop, allowing players to beautify their sanctuary.")]
        [SerializeField] private GameObject _dojoEnhancementCanvas;
        [Tooltip("The popup canvas for displaying Wisdom Cookies and their profound insights.")]
        [SerializeField] private GameObject _wisdomCookiePopupCanvas;

        [Header("UI Components (for direct interaction if needed)")]
        [Tooltip("Reference to the MainGameUI script component.")]
        [SerializeField] private MainGameUI _mainGameUI;
        [Tooltip("Reference to the ScrollOfEnlightenmentUI script component.")]
        [SerializeField] private ScrollOfEnlightenmentUI _scrollOfEnlightenmentUI;
        [Tooltip("Reference to the DojoEnhancementUI script component.")]
        [SerializeField] private DojoEnhancementUI _dojoEnhancementUI;
        [Tooltip("Reference to the WisdomCookiePopup script component.")]
        [SerializeField] private WisdomCookiePopup _wisdomCookiePopup;


        /// <summary>
        /// Initializes the UIManager singleton and ensures all UI canvases are initially hidden.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                // If another instance already exists, destroy this one to maintain singleton integrity.
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes if needed for a loading screen scenario.

            // Ensure all UI canvases are initially deactivated to prevent overlap.
            HideAllCanvases();
        }

        /// <summary>
        /// Displays the main game UI and hides all other canvases.
        /// </summary>
        public void ShowMainGameUI()
        {
            HideAllCanvases();
            _mainGameCanvas.SetActive(true);
            // Optionally, refresh or initialize the main game UI here.
        }

        /// <summary>
        /// Displays the Scroll of Enlightenment UI and hides all other canvases.
        /// </summary>
        public void ShowScrollOfEnlightenmentUI()
        {
            HideAllCanvases();
            _scrollOfEnlightenmentCanvas.SetActive(true);
            _scrollOfEnlightenmentUI?.RefreshScrollContent(); // Ensure scroll content is up-to-date.
        }

        /// <summary>
        /// Displays the Dojo Enhancement UI and hides all other canvases.
        /// </summary>
        public void ShowDojoEnhancementUI()
        {
            HideAllCanvases();
            _dojoEnhancementCanvas.SetActive(true);
            _dojoEnhancementUI?.RefreshDojoItems(); // Ensure dojo items are properly displayed.
        }

        /// <summary>
        /// Displays the Wisdom Cookie popup with the given proverb and optional image.
        /// If the popup is already active, it will update its content.
        /// </summary>
        /// <param name="proverb">The insightful proverb or meditation text to display.</param>
        /// <param name="image">An optional image to accompany the proverb (e.g., a visual pattern or icon).</param>
        public void ShowWisdomCookiePopup(string proverb, Sprite image = null)
        {
            // The popup might overlay other UI, so we don't necessarily hide everything.
            // However, ensure it's brought to the front if using a canvas group or similar.
            _wisdomCookiePopupCanvas.SetActive(true);
            _wisdomCookiePopup?.DisplayWisdom(proverb, image);
        }

        /// <summary>
        /// Hides the Wisdom Cookie popup.
        /// </summary>
        public void HideWisdomCookiePopup()
        {
            _wisdomCookiePopupCanvas.SetActive(false);
        }

        /// <summary>
        /// Deactivates all registered UI canvases. This is a helper method
        /// to ensure only one primary UI view is active at a time, promoting clarity.
        /// </summary>
        private void HideAllCanvases()
        {
            _mainGameCanvas.SetActive(false);
            _scrollOfEnlightenmentCanvas.SetActive(false);
            _dojoEnhancementCanvas.SetActive(false);
            // The wisdom cookie popup might be handled separately depending on design,
            // but for a clean slate, include it here too.
            _wisdomCookiePopupCanvas.SetActive(false);
        }

        /// <summary>
        /// Called when the GameObject is destroyed. Cleans up the singleton instance.
        /// </summary>
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}