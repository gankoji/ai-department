using UnityEngine;
using V7Games.ScriptableObjects; // Namespace for ScriptableObjects
using V7Games.Managers; // Namespace for Managers
using V7Games.Enums; // Namespace for Enums (for WisdomPatternType)

namespace V7Games.Gameplay
{
    /// <summary>
    /// Controls the behavior and interaction of a Wisdom Cookie in the game world.
    /// </summary>
    /// <remarks>
    /// Wisdom Cookies are manifestations of the Mother Dough's accumulated insight,
    /// revealing proverbs or visual patterns when tapped.
    /// </remarks>
    public class WisdomCookieController : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("The ScriptableObject holding this cookie's data and content.")]
        [SerializeField]
        private WisdomCookieConfig _config;

        [Header("Visuals & Effects")]
        [Tooltip("The parent GameObject containing all visual elements of the cookie.")]
        [SerializeField]
        private GameObject _visualsParent;
        [Tooltip("Particle system to play when the cookie is collected.")]
        [SerializeField]
        private ParticleSystem _collectParticles;
        [Tooltip("Optional Animator component for cookie-specific animations (e.g., fade out, shrink).")]
        [SerializeField]
        private Animator _animator;

        // Cached references to essential managers
        private UIManager _uiManager;
        private AudioManager _audioManager;
        private GameManager _gameManager;

        /// <summary>
        /// Gets the configuration data for this Wisdom Cookie.
        /// </summary>
        public WisdomCookieConfig Config => _config;

        private void Awake()
        {
            // Cache manager references. Assumes these managers are Singletons accessible via an 'Instance' property.
            _uiManager = UIManager.Instance;
            _audioManager = AudioManager.Instance;
            _gameManager = GameManager.Instance;

            // Log an error and disable the component if critical dependencies are missing.
            if (_uiManager == null || _audioManager == null || _gameManager == null)
            {
                Debug.LogError($"WisdomCookieController on '{gameObject.name}': One or more required managers (UIManager, AudioManager, GameManager) are not found. Disabling component.", this);
                enabled = false;
            }
        }

        /// <summary>
        /// Initializes the Wisdom Cookie with its specific configuration.
        /// This method should be called immediately after instantiation.
        /// </summary>
        /// <param name="config">The WisdomCookieConfig to apply to this cookie.</param>
        public void Initialize(WisdomCookieConfig config)
        {
            if (config == null)
            {
                Debug.LogError($"WisdomCookieController on '{gameObject.name}': Initialization failed. Provided config is null.", this);
                return;
            }
            _config = config;

            // Optional: Update sprite or material based on config if the cookie's visual needs to change per config.
            // Example:
            // SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            // if (spriteRenderer != null && _config.cookieSprite != null)
            // {
            //     spriteRenderer.sprite = _config.cookieSprite;
            // }

            // Ensure the cookie visuals are active when initialized.
            if (_visualsParent != null)
            {
                _visualsParent.SetActive(true);
            }
        }

        /// <summary>
        /// Handles the player's tap interaction with the Wisdom Cookie.
        /// Requires a Collider2D or Collider component on the GameObject.
        /// </summary>
        private void OnMouseDown()
        {
            // Do not process tap if the cookie is not properly configured.
            if (_config == null)
            {
                Debug.LogWarning($"WisdomCookieController on '{gameObject.name}': Tapped an unconfigured cookie. Ignoring.", this);
                return;
            }

            CollectCookie();
        }

        /// <summary>
        /// Executes the actions associated with collecting the Wisdom Cookie.
        /// This includes playing effects, rewarding the player, and showing the wisdom content.
        /// </summary>
        private void CollectCookie()
        {
            // Play specific collection sound effect from config, or a generic one if not specified.
            if (_config.collectSFX != null)
            {
                _audioManager.PlaySFX(_config.collectSFX);
            }
            else
            {
                // Assuming AudioManager has a method to play a generic SFX by name/ID.
                _audioManager.PlaySFX("GenericCollectChime");
            }

            // Play visual particle effects.
            if (_collectParticles != null)
            {
                _collectParticles.Play();
            }

            // Trigger collection animation (e.g., a shrink or burst animation).
            if (_animator != null)
            {
                _animator.SetTrigger("Collect"); // Assumes an Animator parameter named "Collect" exists.
            }

            // Inform the GameManager to update player progress and add the wisdom to the Scroll of Enlightenment.
            _gameManager.AddWisdomCookieToScroll(_config);
            _gameManager.AddDoughEssence(_config.essenceReward);

            // Display the Wisdom Cookie's content to the player via the UI.
            _uiManager.ShowWisdomCookiePopup(_config.proverbText, _config.patternType);

            // Deactivate and eventually destroy the cookie GameObject after its effects have had time to play.
            // The duration is estimated based on particle system or a default if no particles.
            float destroyDelay = (_collectParticles != null && _collectParticles.isPlaying) ? _collectParticles.main.duration : 0.5f;
            Invoke(nameof(DeactivateAndDestroy), destroyDelay);
        }

        /// <summary>
        /// Hides the cookie's visuals and schedules its destruction.
        /// This allows visual effects to complete before the object is removed from the scene.
        /// </summary>
        private void DeactivateAndDestroy()
        {
            // Hide the cookie's visual representation immediately.
            if (_visualsParent != null)
            {
                _visualsParent.SetActive(false);
            }
            else
            {
                // If no specific visuals parent, hide the entire GameObject.
                gameObject.SetActive(false);
            }

            // Destroy the GameObject after a very short additional delay to ensure all references are cleared.
            Destroy(gameObject, 0.1f);
        }
    }
}