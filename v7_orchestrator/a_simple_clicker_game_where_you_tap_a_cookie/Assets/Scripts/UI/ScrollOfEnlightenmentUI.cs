using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using V7Games.Data; // Assuming WisdomCookieConfig is in this namespace
using V7Games.Managers; // Assuming GameManager is in this namespace

namespace V7Games.UI
{
    /// <summary>
    /// Manages the display and interaction of the Scroll of Enlightenment UI.
    /// This UI displays collected Wisdom Cookies and allows players to view their profound insights.
    /// </summary>
    public class ScrollOfEnlightenmentUI : MonoBehaviour
    {
        [Header("Scroll UI Elements")]
        [Tooltip("The root GameObject for the entire Scroll of Enlightenment panel.")]
        [SerializeField] private GameObject scrollPanel;
        [Tooltip("The Transform parent under which individual Wisdom Cookie entries will be instantiated.")]
        [SerializeField] private Transform scrollContentParent;
        [Tooltip("The prefab for a single entry within the scroll, which should contain a ScrollEntryUI component.")]
        [SerializeField] private GameObject scrollEntryPrefab;

        [Header("Selected Wisdom Cookie Details Panel")]
        [Tooltip("The root GameObject for the details panel that displays a selected Wisdom Cookie's information.")]
        [SerializeField] private GameObject detailsPanel;
        [Tooltip("TextMeshPro text component to display the proverb or meditation of the selected cookie.")]
        [SerializeField] private TMP_Text proverbText;
        [Tooltip("Image component to display the visual pattern or artwork of the selected cookie.")]
        [SerializeField] private Image visualPatternImage;
        [Tooltip("Button to close the details panel.")]
        [SerializeField] private Button closeDetailsButton;

        private List<ScrollEntryUI> currentEntries = new List<ScrollEntryUI>();
        private GameManager gameManager;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Initializes UI states and caches necessary manager references.
        /// </summary>
        void Awake()
        {
            scrollPanel.SetActive(false);
            detailsPanel.SetActive(false);

            // Subscribe to the close button click event
            closeDetailsButton.onClick.AddListener(CloseDetailsPanel);

            // Cache reference to the GameManager.
            // For larger projects, consider more robust dependency injection or a static instance pattern for managers.
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("ScrollOfEnlightenmentUI: GameManager not found in scene. Please ensure it is present.", this);
            }
        }

        /// <summary>
        /// Opens the Scroll of Enlightenment UI panel and populates it with all currently collected Wisdom Cookies.
        /// </summary>
        public void OpenScroll()
        {
            scrollPanel.SetActive(true);
            PopulateScroll();
            CloseDetailsPanel(); // Ensure the details panel is closed when the main scroll opens
            Debug.Log("Scroll of Enlightenment opened, ready for serene browsing.");
        }

        /// <summary>
        /// Closes the Scroll of Enlightenment UI panel and hides any open details.
        /// </summary>
        public void CloseScroll()
        {
            scrollPanel.SetActive(false);
            CloseDetailsPanel();
            Debug.Log("Scroll of Enlightenment closed, returning to quiet contemplation.");
        }

        /// <summary>
        /// Populates the scroll content area with entries for each collected Wisdom Cookie.
        /// Clears previous entries before repopulating.
        /// </summary>
        private void PopulateScroll()
        {
            // Cleanse the scroll of previous wisdom to make way for fresh insights
            foreach (var entry in currentEntries)
            {
                // Unsubscribe from event before destroying to prevent memory leaks
                if (entry != null)
                {
                    entry.OnSelected -= DisplayWisdomCookieDetails;
                }
                Destroy(entry.gameObject);
            }
            currentEntries.Clear();

            if (gameManager == null)
            {
                Debug.LogWarning("Cannot populate scroll: GameManager reference is null.");
                return;
            }

            // Retrieve the list of collected Wisdom Cookie configurations from the GameManager.
            List<WisdomCookieConfig> collectedCookies = gameManager.GetCollectedWisdomCookies(); 

            if (collectedCookies == null || collectedCookies.Count == 0)
            {
                Debug.Log("The Scroll of Enlightenment is currently blank. Continue your meditative kneads to unveil new wisdom.");
                return;
            }

            // Instantiate a UI entry for each collected wisdom cookie.
            foreach (var cookieConfig in collectedCookies)
            {
                GameObject entryGo = Instantiate(scrollEntryPrefab, scrollContentParent);
                ScrollEntryUI entryUI = entryGo.GetComponent<ScrollEntryUI>();

                if (entryUI != null)
                {
                    entryUI.SetData(cookieConfig);
                    // Subscribe to the OnSelected event of each entry to display its details when clicked.
                    entryUI.OnSelected += DisplayWisdomCookieDetails;
                    currentEntries.Add(entryUI);
                }
                else
                {
                    Debug.LogWarning($"ScrollOfEnlightenmentUI: ScrollEntryPrefab '{scrollEntryPrefab.name}' is missing the 'ScrollEntryUI' component. Please ensure it's attached.", entryGo);
                    Destroy(entryGo); // Clean up the improperly instantiated object
                }
            }
        }

        /// <summary>
        /// Displays the detailed information (proverb and visual pattern) of a selected Wisdom Cookie.
        /// </summary>
        /// <param name="cookieConfig">The <see cref="WisdomCookieConfig"/> data of the cookie to display.</param>
        public void DisplayWisdomCookieDetails(WisdomCookieConfig cookieConfig)
        {
            if (cookieConfig == null)
            {
                Debug.LogWarning("Attempted to display details for a null Wisdom Cookie config.");
                CloseDetailsPanel();
                return;
            }

            proverbText.text = cookieConfig.proverb;

            if (cookieConfig.visualPattern != null)
            {
                visualPatternImage.sprite = cookieConfig.visualPattern;
                visualPatternImage.gameObject.SetActive(true);
            }
            else
            {
                visualPatternImage.gameObject.SetActive(false);
                Debug.Log($"Wisdom Cookie '{cookieConfig.name}' has no visual pattern to display.");
            }

            detailsPanel.SetActive(true);
            Debug.Log($"Revealing insight from: {cookieConfig.name}");
        }

        /// <summary>
        /// Closes the details panel that displays a single Wisdom Cookie's information.
        /// </summary>
        public void CloseDetailsPanel()
        {
            detailsPanel.SetActive(false);
            Debug.Log("Wisdom Cookie details panel closed.");
        }

        /// <summary>
        /// Called when the MonoBehaviour will be destroyed.
        /// Cleans up event listeners to prevent potential memory leaks.
        /// </summary>
        void OnDestroy()
        {
            // Unsubscribe from the close button click event
            if (closeDetailsButton != null)
            {
                closeDetailsButton.onClick.RemoveListener(CloseDetailsPanel);
            }

            // Unsubscribe from all ScrollEntryUI events
            foreach (var entry in currentEntries)
            {
                if (entry != null)
                {
                    entry.OnSelected -= DisplayWisdomCookieDetails;
                }
            }
        }
    }
}