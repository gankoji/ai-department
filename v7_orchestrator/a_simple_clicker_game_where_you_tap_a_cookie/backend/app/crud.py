# backend/app/crud.py

from sqlalchemy.orm import Session
from . import models, schemas
from datetime import datetime


def get_player(db: Session, player_id: int) -> models.Player | None:
    """
    Retrieves a player by their unique ID.
    """
    return db.query(models.Player).filter(models.Player.id == player_id).first()


def get_player_by_username(db: Session, username: str) -> models.Player | None:
    """
    Retrieves a player by their username.
    Useful for login and registration checks to ensure uniqueness.
    """
    return db.query(models.Player).filter(models.Player.username == username).first()


def create_player(db: Session, player_create: schemas.PlayerCreate) -> models.Player:
    """
    Creates a new player record in the database.
    Initializes core game progress and metadata for a new guardian.
    """
    db_player = models.Player(
        username=player_create.username,
        # In a production system, 'hashed_password' would be a securely
        # hashed version of the password provided by player_create.password.
        # For this example, it's a placeholder. Authentication logic
        # would typically handle proper hashing and verification.
        hashed_password="placeholder_hashed_password",
        spiritual_harmony=0,
        dough_essence=0,
        current_dough_form_id=1,  # Assuming a starting Dough Form ID of 1
        last_login_at=datetime.utcnow()
    )
    db.add(db_player)
    db.commit()
    db.refresh(db_player)
    return db_player


def update_player_data(db: Session, player_id: int, player_update: schemas.PlayerUpdate) -> models.Player | None:
    """
    Updates a player's core game data such as spiritual harmony, dough essence,
    current dough form, and last login timestamp.
    """
    db_player = get_player(db, player_id)
    if db_player:
        # Update fields dynamically from the Pydantic model, excluding unset fields
        update_data = player_update.dict(exclude_unset=True)
        for key, value in update_data.items():
            setattr(db_player, key, value)

        db.add(db_player)
        db.commit()
        db.refresh(db_player)
    return db_player


def get_player_wisdom_cookies(db: Session, player_id: int) -> list[models.PlayerWisdomCookie]:
    """
    Retrieves all wisdom cookies that a specific player has unlocked.
    """
    return db.query(models.PlayerWisdomCookie).filter(
        models.PlayerWisdomCookie.player_id == player_id
    ).all()


def add_wisdom_cookie_to_player(db: Session, player_id: int, wisdom_cookie_id: int) -> models.PlayerWisdomCookie:
    """
    Adds a new wisdom cookie to a player's collection.
    Ensures that duplicate cookies are not added.
    """
    existing_cookie = db.query(models.PlayerWisdomCookie).filter(
        models.PlayerWisdomCookie.player_id == player_id,
        models.PlayerWisdomCookie.wisdom_cookie_id == wisdom_cookie_id
    ).first()

    if not existing_cookie:
        db_player_wisdom_cookie = models.PlayerWisdomCookie(
            player_id=player_id,
            wisdom_cookie_id=wisdom_cookie_id
        )
        db.add(db_player_wisdom_cookie)
        db.commit()
        db.refresh(db_player_wisdom_cookie)
        return db_player_wisdom_cookie
    return existing_cookie  # Return the existing record if already present


def get_player_dojo_items(db: Session, player_id: int) -> list[models.PlayerDojoItem]:
    """
    Retrieves all dojo items that a specific player has purchased.
    """
    return db.query(models.PlayerDojoItem).filter(
        models.PlayerDojoItem.player_id == player_id
    ).all()


def add_dojo_item_to_player(db: Session, player_id: int, dojo_item_id: int) -> models.PlayerDojoItem:
    """
    Adds a new dojo item to a player's collection.
    Ensures that duplicate items are not added.
    """
    existing_item = db.query(models.PlayerDojoItem).filter(
        models.PlayerDojoItem.player_id == player_id,
        models.PlayerDojoItem.dojo_item_id == dojo_item_id
    ).first()

    if not existing_item:
        db_player_dojo_item = models.PlayerDojoItem(
            player_id=player_id,
            dojo_item_id=dojo_item_id
        )
        db.add(db_player_dojo_item)
        db.commit()
        db.refresh(db_player_dojo_item)
        return db_player_dojo_item
    return existing_item  # Return the existing record if already present


# CRUD operations for static game data (WisdomCookieData, DojoItemData, DoughFormConfig)
# These are typically read-only from the backend's perspective, representing game content.

def get_wisdom_cookie_data(db: Session, wisdom_cookie_id: int) -> models.WisdomCookieData | None:
    """
    Retrieves static data for a specific wisdom cookie by its ID.
    """
    return db.query(models.WisdomCookieData).filter(
        models.WisdomCookieData.id == wisdom_cookie_id
    ).first()


def get_all_wisdom_cookie_data(db: Session) -> list[models.WisdomCookieData]:
    """
    Retrieves all static data for wisdom cookies.
    """
    return db.query(models.WisdomCookieData).all()


def get_dojo_item_data(db: Session, dojo_item_id: int) -> models.DojoItemData | None:
    """
    Retrieves static data for a specific dojo item by its ID.
    """
    return db.query(models.DojoItemData).filter(
        models.DojoItemData.id == dojo_item_id
    ).first()


def get_all_dojo_item_data(db: Session) -> list[models.DojoItemData]:
    """
    Retrieves all static data for dojo items.
    """
    return db.query(models.DojoItemData).all()


def get_dough_form_config(db: Session, dough_form_id: int) -> models.DoughFormConfig | None:
    """
    Retrieves configuration data for a specific dough form by its ID.
    """
    return db.query(models.DoughFormConfig).filter(
        models.DoughFormConfig.id == dough_form_id
    ).first()


def get_all_dough_form_configs(db: Session) -> list[models.DoughFormConfig]:
    """
    Retrieves all configuration data for dough forms.
    """
    return db.query(models.DoughFormConfig).all()