using UnityEngine;

/// <summary>
/// Configuration for a specific Wisdom Cookie, containing its unique content.
/// Each Wisdom Cookie reveals profound insights, proverbs, or meditative prompts.
/// </summary>
[CreateAssetMenu(fileName = "NewWisdomCookie", menuName = "V7 Games/Ancestral Dough Guardian/Wisdom Cookie Config")]
public class WisdomCookieConfig : ScriptableObject
{
    [Tooltip("A unique identifier for this Wisdom Cookie.")]
    public string id;

    [Tooltip("The witty and refined name for this Wisdom Cookie.")]
    public string cookieName;

    [TextArea(3, 6)]
    [Tooltip("The insightful proverb or core wisdom revealed by this cookie.")]
    public string proverbText;

    [TextArea(3, 6)]
    [Tooltip("An optional short, calming meditation prompt associated with this cookie.")]
    public string meditationPrompt;

    [Tooltip("Optional: A visual pattern (e.g., a Sprite for a mandala or serene image) that accompanies this wisdom.")]
    public Sprite visualPatternSprite; 
}