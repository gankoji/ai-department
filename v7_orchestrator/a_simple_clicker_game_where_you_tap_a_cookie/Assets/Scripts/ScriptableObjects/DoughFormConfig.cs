using UnityEngine;

namespace V7Games.ZenFarmManager.Gameplay
{
    /// <summary>
    /// Configuration for a specific Dough Form, defining its visual, audio, and progression properties.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDoughFormConfig", menuName = "V7 Games/Dough Forms/Dough Form Config", order = 1)]
    public class DoughFormConfig : ScriptableObject
    {
        [Tooltip("The unique identifier for this Dough Form.")]
        [SerializeField] private string doughFormId;
        /// <summary>
        /// Gets the unique identifier for this Dough Form.
        /// </summary>
        public string DoughFormId => doughFormId;

        [Tooltip("The display name for this Dough Form (e.g., 'Ancient Earth Dough').")]
        [SerializeField] private string formName;
        /// <summary>
        /// Gets the display name for this Dough Form.
        /// </summary>
        public string FormName => formName;

        [Tooltip("A refined, witty, and joyfully calm description of this Dough Form.")]
        [TextArea(3, 6)]
        [SerializeField] private string description;
        /// <summary>
        /// Gets the description of this Dough Form.
        /// </summary>
        public string Description => description;

        [Tooltip("The minimum Spiritual Harmony required to unlock and transition to this Dough Form.")]
        [SerializeField] private long requiredSpiritualHarmony;
        /// <summary>
        /// Gets the minimum Spiritual Harmony required to unlock this Dough Form.
        /// </summary>
        public long RequiredSpiritualHarmony => requiredSpiritualHarmony;

        [Tooltip("The material used to render the Mother Dough when it is in this form.")]
        [SerializeField] private Material doughMaterial;
        /// <summary>
        /// Gets the material associated with this Dough Form.
        /// </summary>
        public Material DoughMaterial => doughMaterial;

        [Tooltip("The ambient audio clip that plays when this Dough Form is active.")]
        [SerializeField] private AudioClip formAmbientSound;
        /// <summary>
        /// Gets the ambient audio clip for this Dough Form.
        /// </summary>
        public AudioClip FormAmbientSound => formAmbientSound;

        [Tooltip("Optional: A visual effect prefab associated with this Dough Form (e.g., subtle particles).")]
        [SerializeField] private GameObject formVFXPrefab;
        /// <summary>
        /// Gets the visual effect prefab for this Dough Form.
        /// </summary>
        public GameObject FormVFXPrefab => formVFXPrefab;

        /// <summary>
        /// Validates the configuration when it's loaded or changed in the editor.
        /// </summary>
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(doughFormId))
            {
                Debug.LogWarning($"DoughFormConfig '{name}' has an empty DoughFormId. Please assign a unique ID.", this);
            }
            if (string.IsNullOrEmpty(formName))
            {
                Debug.LogWarning($"DoughFormConfig '{name}' has an empty FormName. Please assign a display name.", this);
            }
            if (requiredSpiritualHarmony < 0)
            {
                Debug.LogWarning($"DoughFormConfig '{name}' has a negative RequiredSpiritualHarmony. It should be 0 or positive.", this);
            }
        }
    }
}