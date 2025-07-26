from pydantic import BaseModel, Field
from typing import List, Optional
from datetime import datetime

# --- Core Data Models ---

class PlayerState(BaseModel):
    """Represents the complete state of a player's game.
    This model is the source of truth for player progression and resources.
    """
    player_id: str = Field(..., description="Unique identifier for the player.")
    spiritual_harmony: float = Field(..., ge=0.0, description="Current spiritual harmony of the Mother Dough.")
    dough_essence: float = Field(..., ge=0.0, description="Current dough essence available to the player.")
    current_dough_form_id: str = Field(..., description="The ID of the current visual form of the Mother Dough.")
    unlocked_wisdom_cookies_ids: List[str] = Field(default_factory=list, description="List of IDs of unlocked Wisdom Cookies.")
    unlocked_dojo_items_ids: List[str] = Field(default_factory=list, description="List of IDs of unlocked Dojo enhancement items.")
    last_activity_at: datetime = Field(..., description="Timestamp of the player's last recorded activity (UTC).")

# --- Request Models (from Client to Server) ---

class SavePlayerStateRequest(BaseModel):
    """Request model for a client to periodically save its current game state to the server.
    The server will reconcile this with its own state, applying idle progression.
    """
    player_state: PlayerState = Field(..., description="The complete player state from the client to save.")

class TapRequest(BaseModel):
    """Request model for a player's 'Meditative Knead' action."""
    player_id: str = Field(..., description="Unique identifier for the player performing the tap.")
    # Timestamp is crucial for server-side validation and idle calculations
    timestamp: datetime = Field(..., description="Timestamp when the tap occurred on the client (UTC).")

class UnlockDojoItemRequest(BaseModel):
    """Request model for a player to unlock a Dojo enhancement item."""
    player_id: str = Field(..., description="Unique identifier for the player attempting the unlock.")
    item_id: str = Field(..., description="The ID of the Dojo item the player wishes to unlock.")

class UnlockWisdomCookieRequest(BaseModel):
    """Request model for a player to unlock a Wisdom Cookie."""
    player_id: str = Field(..., description="Unique identifier for the player attempting the unlock.")
    cookie_id: str = Field(..., description="The ID of the Wisdom Cookie the player wishes to unlock.")

# --- Response Models (from Server to Client) ---

class PlayerStateResponse(BaseModel):
    """Response model for returning updated player game state after an action or save."""
    player_state: PlayerState = Field(..., description="The updated complete player state from the server.")

class ErrorResponse(BaseModel):
    """Generic error response model for API failures."""
    detail: str = Field(..., description="A description of the error.")

# --- Configuration Schemas (Internal or for potential static data endpoints) ---
# These models define the structure for game content data, which might be loaded from
# configuration files or a separate content management system, rather than being
# part of the player's mutable state.

class DoughFormConfig(BaseModel):
    """Configuration for a specific Mother Dough form."""
    id: str = Field(..., description="Unique ID for the dough form (e.g., 'ancient_earth_dough').")
    name: str = Field(..., description="Display name of the dough form.")
    harmony_threshold: float = Field(..., ge=0.0, description="Spiritual Harmony required to transition to this form.")
    visual_asset_id: str = Field(..., description="Identifier for the Unity visual asset for this form.")
    ambient_sound_id: str = Field(..., description="Identifier for the Unity ambient sound for this form.")

class WisdomCookieConfig(BaseModel):
    """Configuration for a specific Wisdom Cookie."""
    id: str = Field(..., description="Unique ID for the wisdom cookie (e.g., 'proverb_of_stillness_1').")
    proverb: Optional[str] = Field(None, description="The insightful proverb revealed by this cookie.")
    visual_pattern_id: Optional[str] = Field(None, description="Identifier for the Unity visual pattern asset.")
    meditation_prompt: Optional[str] = Field(None, description="A prompt for a calming meditation.")
    harmony_threshold_to_yield: float = Field(..., ge=0.0, description="Spiritual Harmony required for the Mother Dough to yield this cookie.")

class DojoItemConfig(BaseModel):
    """Configuration for a specific Dojo enhancement item."""
    id: str = Field(..., description="Unique ID for the dojo item (e.g., 'singing_bowl_of_serenity').")
    name: str = Field(..., description="Display name of the item.")
    description: str = Field(..., description="Description of the item's benefit or aesthetic.")
    cost_essence: float = Field(..., ge=0.0, description="Dough Essence required to unlock this item.")
    harmony_bonus_per_hour: float = Field(default=0.0, ge=0.0, description="Passive Spiritual Harmony generation bonus per hour from this item.")
    essence_bonus_per_hour: float = Field(default=0.0, ge=0.0, description="Passive Dough Essence generation bonus per hour from this item.")
    visual_asset_id: str = Field(..., description="Identifier for the Unity visual asset for this item.")

# --- Authentication/Player Identification Schemas (Basic) ---

class PlayerLogin(BaseModel):
    """Basic model for player identification/login.
    In a real system, this would involve more robust authentication (e.g., email/password, OAuth tokens).
    For simplicity, we'll assume player_id is sufficient for initial identification.
    """
    player_id: str = Field(..., description="Unique identifier for the player to log in.")

class AuthTokenResponse(BaseModel):
    """Response model for a successful authentication, providing an access token."""
    access_token: str = Field(..., description="JWT or similar token for API authentication.")
    token_type: str = "bearer"