using UnityEngine;
using UnityEngine.UI;
using TMPro; // Assuming TextMeshPro is used for UI text
using System; // For Action event

/// <summary>
/// Manages the UI for purchasing Dojo Enhancements.
/// Displays available items, their costs, and handles purchase interactions.
/// </summary>
public class DojoEnhancementUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI doughEssenceText;
    [SerializeField] private Transform enhancementItemsParent;
    [SerializeField] private GameObject enhancementItemUIPrefab; // Prefab for a single enhancement item UI

    [Header("Configuration")]
    [SerializeField] private DojoItemConfig[] allDojoItems; // Drag all your DojoItemConfig ScriptableObjects here

    private GameManager gameManager;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes references.
    /// </summary>
    private void Awake()
    {
        gameManager = GameManager.Instance; // Assuming GameManager is a Singleton
        if (gameManager == null)
        {
            Debug.LogError("DojoEnhancementUI: GameManager instance not found! Ensure GameManager is set up as a Singleton.");
        }
    }

    /// <summary>
    /// Called when the object becomes enabled and active.
    /// Subscribes to events and populates the UI.
    /// </summary>
    private void OnEnable()
    {
        if (gameManager != null)
        {
            gameManager.OnDoughEssenceChanged += UpdateDoughEssenceUI;
            gameManager.OnDojoItemPurchased += UpdateDojoItemState; // Event for when an item is purchased
            UpdateDoughEssenceUI(gameManager.GetDoughEssence()); // Initial update of essence display
            PopulateEnhancementsUI();
        }
    }

    /// <summary>
    /// Called when the behaviour becomes disabled or inactive.
    /// Unsubscribes from events.
    /// </summary>
    private void OnDisable()
    {
        if (gameManager != null)
        {
            gameManager.OnDoughEssenceChanged -= UpdateDoughEssenceUI;
            gameManager.OnDojoItemPurchased -= UpdateDojoItemState;
        }
    }

    /// <summary>
    /// Populates the UI with all available dojo enhancement items.
    /// Clears existing items before adding new ones.
    /// </summary>
    private void PopulateEnhancementsUI()
    {
        // Clear existing items to prevent duplicates on re-enable
        foreach (Transform child in enhancementItemsParent)
        {
            Destroy(child.gameObject);
        }

        if (allDojoItems == null || allDojoItems.Length == 0)
        {
            Debug.LogWarning("No DojoItemConfig ScriptableObjects assigned to DojoEnhancementUI.");
            return;
        }

        foreach (DojoItemConfig itemConfig in allDojoItems)
        {
            GameObject itemUI = Instantiate(enhancementItemUIPrefab, enhancementItemsParent);

            // Assuming the prefab has children named 'NameText', 'CostText', 'IconImage', 'PurchaseButton'
            // For a more robust solution, a dedicated 'DojoItemDisplay' script on the prefab would be ideal.
            TextMeshProUGUI nameText = itemUI.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI costText = itemUI.transform.Find("CostText")?.GetComponent<TextMeshProUGUI>();
            Image iconImage = itemUI.transform.Find("IconImage")?.GetComponent<Image>();
            Button purchaseButton = itemUI.transform.Find("PurchaseButton")?.GetComponent<Button>();

            if (nameText != null) nameText.text = itemConfig.itemName;
            if (iconImage != null && itemConfig.icon != null) iconImage.sprite = itemConfig.icon;

            if (purchaseButton != null)
            {
                purchaseButton.onClick.RemoveAllListeners(); // Ensure no duplicate listeners if repopulating
                purchaseButton.onClick.AddListener(() => OnPurchaseButtonClicked(itemConfig));
            }

            // Update the state of this specific item UI immediately after creation
            UpdateSingleDojoItemUI(itemUI, itemConfig);
        }
    }

    /// <summary>
    /// Handles the purchase button click for a specific dojo item.
    /// Attempts to purchase the item via the GameManager.
    /// </summary>
    /// <param name="itemConfig">The configuration of the item to purchase.</param>
    private void OnPurchaseButtonClicked(DojoItemConfig itemConfig)
    {
        if (gameManager == null) return;

        bool success = gameManager.TryPurchaseDojoItem(itemConfig);
        if (success)
        {
            Debug.Log($"Successfully purchased {itemConfig.itemName}!");
            // The UpdateDojoItemState event will now be triggered by GameManager and handle UI update.
        }
        else
        {
            Debug.Log($"Could not purchase {itemConfig.itemName}. Not enough Dough Essence or already purchased.");
            // Optionally, provide more direct visual feedback for failed purchase (e.g., shake, red text)
        }
    }

    /// <summary>
    /// Updates the displayed Dough Essence count in the UI.
    /// This method is subscribed to the GameManager's OnDoughEssenceChanged event.
    /// </summary>
    /// <param name="newEssenceAmount">The new amount of Dough Essence.</param>
    private void UpdateDoughEssenceUI(int newEssenceAmount)
    {
        if (doughEssenceText != null)
        {
            doughEssenceText.text = $"Essence: {newEssenceAmount}";
        }
        UpdateAllItemInteractability(); // Update button states whenever essence changes
    }

    /// <summary>
    /// Updates the UI state of a specific dojo item, typically when it's purchased.
    /// This method is subscribed to the GameManager's OnDojoItemPurchased event.
    /// </summary>
    /// <param name="purchasedItemConfig">The DojoItemConfig that was purchased.</param>
    private void UpdateDojoItemState(DojoItemConfig purchasedItemConfig)
    {
        // Find the specific UI element for the purchased item and update it.
        // This is a simple linear search. For many items, consider mapping itemConfig to its GameObject for performance.
        foreach (Transform child in enhancementItemsParent)
        {
            TextMeshProUGUI nameText = child.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null && nameText.text == purchasedItemConfig.itemName)
            {
                UpdateSingleDojoItemUI(child.gameObject, purchasedItemConfig);
                break; // Item found and updated, no need to continue searching
            }
        }
        UpdateAllItemInteractability(); // Re-evaluate all buttons, as essence might have changed due to purchase
    }

    /// <summary>
    /// Updates the visual state of a single enhancement item UI element.
    /// This includes its cost text, and purchase button interactability.
    /// </summary>
    /// <param name="itemUIObject">The GameObject of the item's UI.</param>
    /// <param name="itemConfig">The configuration for the item.</param>
    private void UpdateSingleDojoItemUI(GameObject itemUIObject, DojoItemConfig itemConfig)
    {
        Button purchaseButton = itemUIObject.transform.Find("PurchaseButton")?.GetComponent<Button>();
        TextMeshProUGUI costText = itemUIObject.transform.Find("CostText")?.GetComponent<TextMeshProUGUI>();

        if (gameManager == null) return;

        bool isPurchased = gameManager.IsDojoItemPurchased(itemConfig);

        if (costText != null)
        {
            costText.text = isPurchased ? "PURCHASED" : $"{itemConfig.cost} Essence";
            costText.color = isPurchased ? new Color(0.3f, 0.7f, 0.3f) : Color.white; // Subtle green for purchased
        }

        if (purchaseButton != null)
        {
            // Button is interactable only if not purchased AND player has enough essence
            purchaseButton.interactable = !isPurchased && gameManager.GetDoughEssence() >= itemConfig.cost;
        }
    }

    /// <summary>
    /// Updates the interactability of all purchase buttons based on current Dough Essence.
    /// This ensures buttons are enabled/disabled correctly as essence changes or items are purchased.
    /// </summary>
    private void UpdateAllItemInteractability()
    {
        foreach (Transform child in enhancementItemsParent)
        {
            TextMeshProUGUI nameText = child.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
            {
                // Find the corresponding DojoItemConfig from our array using its name
                DojoItemConfig itemConfig = Array.Find(allDojoItems, item => item.itemName == nameText.text);
                if (itemConfig != null)
                {
                    UpdateSingleDojoItemUI(child.gameObject, itemConfig);
                }
            }
        }
    }
}