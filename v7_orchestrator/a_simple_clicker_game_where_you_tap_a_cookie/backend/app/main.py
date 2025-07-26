from fastapi import FastAPI, Depends, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from sqlalchemy.orm import Session
from contextlib import asynccontextmanager

from . import models, database
from .routers import player_data, save_load
from .dependencies import get_db

# Define CORS origins (adjust as needed for production)
# For development, allowing all origins might be acceptable, but narrow this down for production.
origins = [
    "http://localhost",
    "http://localhost:8000",
    "http://localhost:3000", # Example for a web client
    # Add specific origins for your game client or web frontend
    "unitydl.games.v7.com", # Example for Unity WebGL builds
    "https://unitydl.games.v7.com",
    "v7games.com", # Example for your main website
    "https://v7games.com",
    "*.v7games.com", # Wildcard for subdomains if needed
]

@asynccontextmanager
async def lifespan(app: FastAPI):
    """
    Handles startup and shutdown events for the FastAPI application.
    On startup, it ensures all database tables are created.
    """
    # Create database tables if they don't exist
    # This is suitable for development and simple deployments.
    # For production, consider using Alembic for migrations.
    print("Backend service starting up...")
    models.Base.metadata.create_all(bind=database.engine)
    print("Database tables checked/created.")
    yield
    print("Backend service shutting down.")


app = FastAPI(
    title="V7 Games - Ancestral Dough Guardian Backend",
    description="API services for managing player data, game state, and progression for 'The Ancestral Dough Guardian'.",
    version="1.0.0",
    lifespan=lifespan,
)

# Add CORS middleware
app.add_middleware(
    CORSMiddleware,
    allow_origins=origins,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Include routers for different API functionalities
app.include_router(player_data.router, prefix="/player", tags=["Player Data"])
app.include_router(save_load.router, prefix="/game", tags=["Game Save/Load"])

@app.get("/", summary="Root endpoint for health check")
async def root():
    """
    A simple health check endpoint to confirm the API is running.
    """
    return {"message": "Welcome to the V7 Games - Ancestral Dough Guardian Backend! May your journey be filled with serene kneads."}

@app.get("/health", summary="Detailed health check")
async def health_check(db: Session = Depends(get_db)):
    """
    Performs a detailed health check, including database connectivity.
    """
    try:
        # Attempt to query the database to check connectivity
        db.execute(database.text("SELECT 1"))
        return {"status": "healthy", "database_connection": "ok"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Database connection failed: {e}")