from typing import List, Optional

from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.orm import Session

from app import crud, schemas
from app.database import get_db

router = APIRouter(
    prefix="/player",
    tags=["Player Data"],
    responses={404: {"description": "Not found"}},
)

@router.get("/{player_id}", response_model=schemas.PlayerRead)
def get_player_data(player_id: int, db: Session = Depends(get_db)) -> schemas.PlayerRead:
    """
    Retrieve the complete game data for a specific player.

    This endpoint provides a snapshot of the player's progress in 'The Ancestral Dough Guardian',
    including their spiritual harmony, dough essence, unlocked dough forms, and collected wisdom cookies.
    """
    db_player = crud.get_player_by_id(db, player_id=player_id)
    if db_player is None:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Player not found")
    return db_player

@router.put("/{player_id}", response_model=schemas.PlayerRead)
def update_player_data(
    player_id: int, player_update: schemas.PlayerUpdate, db: Session = Depends(get_db)
) -> schemas.PlayerRead:
    """
    Update specific game data fields for a player.

    This endpoint allows the game client to send updates to the player's game state,
    such as changes in spiritual harmony, dough essence, or newly unlocked items.
    The update is partial; only fields provided in the request body will be modified.
    """
    db_player = crud.get_player_by_id(db, player_id=player_id)
    if db_player is None:
        raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Player not found")

    updated_player = crud.update_player(db, db_player=db_player, player_update=player_update)
    return updated_player

# Additional granular endpoints could be added here if specific game mechanics
# require atomic updates for specific player data elements (e.g.,
# adding a single wisdom cookie, unlocking a single dojo item).
# For now, the comprehensive PUT endpoint should suffice for most client needs.