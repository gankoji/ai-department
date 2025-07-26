from sqlalchemy import Column, Integer, String, Float, DateTime, ForeignKey
from sqlalchemy.orm import relationship
from sqlalchemy.sql import func
from .database import Base

class Player(Base):
    """
    SQLAlchemy model for player data.
    Stores core player progression and resources.
    """
    __tablename__ = "players"

    id = Column(Integer, primary_key=True, index=True)
    user_id = Column(String, unique=True, index=True, nullable=False) # Unique identifier for the player
    spiritual_harmony = Column(Float, default=0.0, nullable=False)
    dough_essence = Column(Float, default=0.0, nullable=False)
    current_dough_form = Column(String, default="Ancient Earth Dough", nullable=False) # Corresponds to DoughFormConfig ID
    last_login_at = Column(DateTime(timezone=True), default=func.now(), nullable=False)
    created_at = Column(DateTime(timezone=True), default=func.now(), nullable=False)
    updated_at = Column(DateTime(timezone=True), default=func.now(), onupdate=func.now(), nullable=False)

    # Relationships
    wisdom_cookies = relationship("PlayerWisdomCookie", back_populates="player", cascade="all, delete-orphan")
    dojo_items = relationship("PlayerDojoItem", back_populates="player", cascade="all, delete-orphan")

    def __repr__(self):
        return f"<Player(id={self.id}, user_id='{self.user_id}', harmony={self.spiritual_harmony})>"

class PlayerWisdomCookie(Base):
    """
    SQLAlchemy model for tracking which Wisdom Cookies a player has collected.
    """
    __tablename__ = "player_wisdom_cookies"

    id = Column(Integer, primary_key=True, index=True)
    player_id = Column(Integer, ForeignKey("players.id"), nullable=False)
    wisdom_cookie_id = Column(String, nullable=False) # Corresponds to WisdomCookieConfig ID
    collected_at = Column(DateTime(timezone=True), default=func.now(), nullable=False)

    # Relationship
    player = relationship("Player", back_populates="wisdom_cookies")

    def __repr__(self):
        return f"<PlayerWisdomCookie(id={self.id}, player_id={self.player_id}, cookie_id='{self.wisdom_cookie_id}')>"

class PlayerDojoItem(Base):
    """
    SQLAlchemy model for tracking which Dojo Enhancement items a player has purchased.
    """
    __tablename__ = "player_dojo_items"

    id = Column(Integer, primary_key=True, index=True)
    player_id = Column(Integer, ForeignKey("players.id"), nullable=False)
    dojo_item_id = Column(String, nullable=False) # Corresponds to DojoItemConfig ID
    purchased_at = Column(DateTime(timezone=True), default=func.now(), nullable=False)

    # Relationship
    player = relationship("Player", back_populates="dojo_items")

    def __repr__(self):
        return f"<PlayerDojoItem(id={self.id}, player_id={self.player_id}, item_id='{self.dojo_item_id}')>"