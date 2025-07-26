import pytest
from fastapi.testclient import TestClient
from unittest.mock import MagicMock
from datetime import datetime

from app.main import app
from app.dependencies import get_db
from app.models import Player, GameState
from app.schemas import PlayerCreate, PlayerResponse, GameStateCreate, GameStateResponse


@pytest.fixture(name="mock_db_session")
def mock_db_session_fixture():
    """
    Fixture to provide a mock database session for tests.
    We'll mock the common methods used by our CRUD operations.
    """
    session = MagicMock()
    # Ensure query.filter.first returns None by default for non-existent items
    session.query.return_value.filter.return_value.first.return_value = None
    yield session
    session.close()


@pytest.fixture(name="client")
def client_fixture(mock_db_session: MagicMock):
    """
    Fixture to provide a TestClient instance with the mocked database dependency.
    """
    def override_get_db():
        yield mock_db_session

    app.dependency_overrides[get_db] = override_get_db
    with TestClient(app) as test_client:
        yield test_client
    app.dependency_overrides.clear()


# --- Player Data Router Tests (/players) ---

def test_create_player(client: TestClient, mock_db_session: MagicMock):
    """
    Test creating a new player.
    Endpoint: POST /players/
    """
    player_data = {"player_name": "ZenMaster"}
    
    mock_player_instance = Player(id=1, player_name="ZenMaster")
    
    mock_db_session.add.return_value = None
    mock_db_session.commit.return_value = None
    mock_db_session.refresh.return_value = mock_player_instance 

    response = client.post("/players/", json=player_data)

    assert response.status_code == 200
    assert response.json()["player_name"] == "ZenMaster"
    assert response.json()["id"] == 1
    mock_db_session.add.assert_called_once()
    mock_db_session.commit.assert_called_once()
    mock_db_session.refresh.assert_called_once()


def test_create_player_already_exists(client: TestClient, mock_db_session: MagicMock):
    """
    Test creating a player that already exists.
    """
    player_data = {"player_name": "ExistingPlayer"}
    existing_player = Player(id=1, player_name="ExistingPlayer")
    
    mock_db_session.query.return_value.filter.return_value.first.return_value = existing_player

    response = client.post("/players/", json=player_data)
    assert response.status_code == 400
    assert "Player with this name already exists" in response.json()["detail"]


def test_get_player_metadata(client: TestClient, mock_db_session: MagicMock):
    """
    Test retrieving player metadata.
    Endpoint: GET /players/{player_id}
    """
    player_id = 1
    mock_player_instance = Player(id=player_id, player_name="ZenMaster")
    
    mock_db_session.query.return_value.filter.return_value.first.return_value = mock_player_instance

    response = client.get(f"/players/{player_id}")

    assert response.status_code == 200
    assert response.json()["id"] == player_id
    assert response.json()["player_name"] == "ZenMaster"


def test_get_player_metadata_not_found(client: TestClient, mock_db_session: MagicMock):
    """
    Test retrieving player metadata for a non-existent player.
    """
    player_id = 999
    response = client.get(f"/players/{player_id}")
    assert response.status_code == 404
    assert "Player not found" in response.json()["detail"]


# --- Save/Load Router Tests (/save, /load) ---

def test_save_game_state_new(client: TestClient, mock_db_session: MagicMock):
    """
    Test saving a game state for a player for the first time.
    Endpoint: POST /save/
    """
    player_id = 1
    game_state_data = {
        "player_id": player_id,
        "spiritual_harmony": 200,
        "dough_essence": 100,
        "current_dough_form": "Celestial Cloud Dough",
        "wisdom_cookies_collected": ["Proverb of Calm"],
        "dojo_enhancements_owned": ["Singing Bowl of Serenity"],
        "last_played": datetime.now().isoformat()
    }

    mock_player_instance = Player(id=player_id, player_name="ZenMaster")
    
    mock_db_session.query.return_value.filter.return_value.first.side_effect = [
        mock_player_instance,
        None 
    ]
    
    mock_game_state_instance = GameState(**game_state_data)
    mock_game_state_instance.id = 1

    mock_db_session.add.return_value = None
    mock_db_session.commit.return_value = None
    mock_db_session.refresh.return_value = mock_game_state_instance

    response = client.post("/save/", json=game_state_data)

    assert response.status_code == 200
    assert response.json()["player_id"] == player_id
    assert response.json()["spiritual_harmony"] == 200
    assert "Proverb of Calm" in response.json()["wisdom_cookies_collected"]
    mock_db_session.add.assert_called_once()
    mock_db_session.commit.assert_called_once()
    mock_db_session.refresh.assert_called_once()


def test_save_game_state_update(client: TestClient, mock_db_session: MagicMock):
    """
    Test updating an existing game state for a player.
    Endpoint: POST /save/
    """
    player_id = 1
    existing_game_state = GameState(
        id=1,
        player_id=player_id,
        spiritual_harmony=100,
        dough_essence=50,
        current_dough_form="Ancient Earth Dough",
        wisdom_cookies_collected=["Initial Proverb"],
        dojo_enhancements_owned=[],
        last_played=datetime(2023, 10, 26, 10, 0, 0)
    )
    update_data = {
        "player_id": player_id,
        "spiritual_harmony": 300,
        "dough_essence": 150,
        "current_dough_form": "Flowing Water Dough",
        "wisdom_cookies_collected": ["Initial Proverb", "New Insight"],
        "dojo_enhancements_owned":["Singing Bowl of Serenity"],
        "last_played": datetime.now().isoformat()
    }

    mock_player_instance = Player(id=player_id, player_name="ZenMaster")
    mock_db_session.query.return_value.filter.return_value.first.side_effect = [
        mock_player_instance,
        existing_game_state 
    ]
    
    mock_db_session.commit.return_value = None
    mock_db_session.refresh.return_value = existing_game_state

    response = client.post("/save/", json=update_data)

    assert response.status_code == 200
    assert response.json()["spiritual_harmony"] == 300
    assert response.json()["current_dough_form"] == "Flowing Water Dough"
    assert "New Insight" in response.json()["wisdom_cookies_collected"]
    mock_db_session.commit.assert_called_once()
    mock_db_session.refresh.assert_called_once()
    mock_db_session.add.assert_not_called()


def test_save_game_state_player_not_found(client: TestClient, mock_db_session: MagicMock):
    """
    Test saving game state for a non-existent player.
    """
    player_id = 999
    game_state_data = {
        "player_id": player_id,
        "spiritual_harmony": 10,
        "dough_essence": 5,
        "current_dough_form": "Ancient Earth Dough",
        "wisdom_cookies_collected": [],
        "dojo_enhancements_owned": [],
        "last_played": datetime.now().isoformat()
    }
    
    mock_db_session.query.return_value.filter.return_value.first.return_value = None

    response = client.post("/save/", json=game_state_data)
    assert response.status_code == 404
    assert "Player not found" in response.json()["detail"]


def test_load_game_state(client: TestClient, mock_db_session: MagicMock):
    """
    Test loading a game state.
    Endpoint: GET /load/{player_id}
    """
    player_id = 1
    mock_game_state_instance = GameState(
        id=1,
        player_id=player_id,
        spiritual_harmony=200,
        dough_essence=100,
        current_dough_form="Celestial Cloud Dough",
        wisdom_cookies_collected=["Proverb of Calm", "Insightful Mandala", "Cosmic Whisper"],
        dojo_enhancements_owned=["Singing Bowl of Serenity", "Tranquil Rock Garden", "Tea Ceremony Set"],
        last_played=datetime(2023, 10, 27, 10, 0, 0)
    )
    
    mock_db_session.query.return_value.filter.return_value.first.return_value = mock_game_state_instance

    response = client.get(f"/load/{player_id}")

    assert response.status_code == 200
    assert response.json()["player_id"] == player_id
    assert response.json()["spiritual_harmony"] == 200
    assert "Cosmic Whisper" in response.json()["wisdom_cookies_collected"]
    assert "Tranquil Rock Garden" in response.json()["dojo_enhancements_owned"]


def test_load_game_state_not_found(client: TestClient, mock_db_session: MagicMock):
    """
    Test loading game state for a non-existent player or no saved state.
    """
    player_id = 999
    response = client.get(f"/load/{player_id}")
    assert response.status_code == 404
    assert "Game state not found for this player" in response.json()["detail"]