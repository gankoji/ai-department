from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.orm import Session
from datetime import datetime
from typing import Optional

from .. import crud, models, schemas
from ..dependencies import get_db

router = APIRouter(
    prefix="/save_load",
    tags=["Save/Load"],
    summary="Endpoints for game data persistence.",
)

@router.post(
    "/save",
    response_model=schemas.SaveResponse,
    status_code=status.HTTP_200_OK,
    summary="Save player game data",
    description="Persists the current game state for a given player ID. Creates a new save or updates an existing one.",
)
async def save_game_data(
    game_data: schemas.GameDataSchema,
    db: Session = Depends(get_db)
) -> schemas.SaveResponse:
    """
    Saves the provided game state data for a specific player.
    If a save for the player already exists, it will be updated; otherwise, a new one will be created.
    """
    try:
        updated_save = crud.update_player_save_data(db, game_data)
        return schemas.SaveResponse(
            message=f"Game data for player '{updated_save.player_id}' saved successfully.",
            success=True,
            timestamp=updated_save.last_updated.isoformat()
        )
    except Exception as e:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail=f"Failed to save game data: {str(e)}"
        )

@router.get(
    "/load/{player_id}",
    response_model=schemas.LoadResponse,
    status_code=status.HTTP_200_OK,
    summary="Load player game data",
    description="Retrieves the latest saved game state for a specified player ID.",
)
async def load_game_data(
    player_id: str,
    db: Session = Depends(get_db)
) -> schemas.LoadResponse:
    """
    Loads the game state data for a given player ID.
    Returns the game state if found, otherwise returns an empty state.
    """
    db_save_data: Optional[models.PlayerSaveData] = crud.get_player_save_data(db, player_id)

    if db_save_data:
        return schemas.LoadResponse(
            player_id=player_id,
            game_state=db_save_data.game_state
        )
    else:
        # Return a response indicating no save data found, but still a successful operation
        return schemas.LoadResponse(
            player_id=player_id,
            game_state=None # Or an initial empty game state if the client expects it
        )