using UnityEngine;
using UnityEngine.UI;
using TMPro; // Assuming TextMeshPro is used for text elements

namespace V7Games.UI
{
    /// <summary>
    /// Manages the display and interaction of the Wisdom Cookie popup.
    /// This UI element presents the profound proverb or visual pattern revealed by a Wisdom Cookie.
    /// </summary>
    public class WisdomCookiePopup : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("The TextMeshProUGUI component to display the wisdom proverb.")]
        [SerializeField] private TextMeshProUGUI proverbText;

        [Tooltip("The Image component to display the associated visual pattern, if any.")]
        [SerializeField] private Image visualPatternImage;

        [Tooltip("The Button component to close the popup.")]
        [SerializeField] private Button closeButton;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Caches component references and sets up event listeners.
        /// </summary>
        private void Awake()
        {
            // Ensure UI elements are assigned in the Inspector
            if (proverbText == null) Debug.LogError("Proverb Text (TextMeshProUGUI) not assigned in WisdomCookiePopup.", this);
            if (visualPatternImage == null) Debug.LogError("Visual Pattern Image not assigned in WisdomCookiePopup.", this);
            if (closeButton == null) Debug.LogError("Close Button not assigned in WisdomCookiePopup.", this);

            // Add listener for the close button
            closeButton?.onClick.AddListener(OnCloseButtonClicked);
            
            // Initially hide the popup
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Displays the Wisdom Cookie popup with the provided data.
        /// </summary>
        /// <param name="proverb">The wise proverb text to display.</param>
        /// <param name="visualPattern">The visual pattern (Sprite) to display, can be null if only text.</param>
        public void Show(string proverb, Sprite visualPattern = null)
        {
            if (proverbText != null)
            {
                proverbText.text = proverb;
            }

            if (visualPatternImage != null)
            {
                visualPatternImage.sprite = visualPattern;
                visualPatternImage.gameObject.SetActive(visualPattern != null); // Only show image if sprite exists
            }

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the Wisdom Cookie popup.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Handles the click event for the close button.
        /// </summary>
        private void OnCloseButtonClicked()
        {
            Hide();
            // Potentially inform UIManager or GameManager that the popup is closed
            // UIManager.Instance.OnWisdomCookiePopupClosed(); // Example
        }

        /// <summary>
        /// Called when the MonoBehaviour will be destroyed.
        /// Removes event listeners to prevent memory leaks.
        /// </summary>
        private void OnDestroy()
        {
            closeButton?.onClick.RemoveListener(OnCloseButtonClicked);
        }
    }
}