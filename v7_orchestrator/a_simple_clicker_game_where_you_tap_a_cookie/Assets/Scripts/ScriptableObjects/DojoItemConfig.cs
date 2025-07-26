using UnityEngine;

namespace V7Games.AncestralDoughGuardian.ScriptableObjects
{
    /// <summary>
    /// Configuration for a single Dojo enhancement item.
    /// These items beautify the meditative sanctuary and provide subtle passive benefits.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDojoItemConfig", menuName = "V7 Games/Dojo Item Configuration", order = 5)]
    public class DojoItemConfig : ScriptableObject
    {
        [Header("Item Identification")]
        [Tooltip("The refined name of this Dojo enhancement, inspiring peace and contemplation.")]
        public string itemName = "New Dojo Item";

        [Tooltip("A poetic description of the item's purpose and its serene influence on the sanctuary.")]
        [TextArea(3, 6)]
        public string itemDescription = "An ancient artifact, carefully chosen to deepen the tranquility of your meditative space.";

        [Header("Acquisition & Benefits")]
        [Tooltip("The Dough Essence required to acquire this tranquil enhancement, reflecting its inherent value.")]
        public int doughEssenceCost = 100;

        [Tooltip("The subtle, passive increase in Spiritual Harmony generation this item bestows upon the Mother Dough.")]
        public float spiritualHarmonyBonus = 0.01f; // Represents a percentage increase, e.g., 0.01 for 1%

        [Tooltip("The gentle bonus to Chi infused per meditative knead, reflecting a deeper connection.")]
        public float chiBonusPerTap = 0f; // Can be used for specific meditative mini-games or general tap bonus

        [Header("Visual Representation")]
        [Tooltip("The GameObject prefab that represents this item within the Dojo scene, contributing to its serene aesthetic.")]
        public GameObject itemPrefab;

        /// <summary>
        /// Provides a concise, refined summary of the Dojo item.
        /// </summary>
        /// <returns>A string detailing the item's name, cost, and primary benefit.</returns>
        public string GetSummary()
        {
            return $"{itemName} (Cost: {doughEssenceCost} Essence, Harmony Infusion: {spiritualHarmonyBonus * 100:F0}%)";
        }
    }
}