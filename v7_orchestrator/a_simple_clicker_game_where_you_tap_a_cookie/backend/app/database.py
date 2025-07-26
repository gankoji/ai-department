import os
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker, declarative_base
from sqlalchemy.exc import SQLAlchemyError
from typing import Generator

# Database connection URL from environment variables for flexibility and security.
# Default to an in-memory SQLite for local development if DATABASE_URL is not set.
DATABASE_URL = os.getenv("DATABASE_URL", "sqlite:///./sql_app.db")

# Create the SQLAlchemy engine
# connect_args are specific to SQLite to allow multiple threads to access the same connection.
# For PostgreSQL or other databases, this might not be needed or would be different.
engine = create_engine(
    DATABASE_URL, connect_args={"check_same_thread": False} if "sqlite" in DATABASE_URL else {}
)

# Create a SessionLocal class
# Each instance of SessionLocal will be a database session.
# The `autocommit=False` means that the session will not commit changes to the database automatically.
# The `autoflush=False` means that the session will not flush changes to the database automatically.
# `bind=engine` connects the session to our database engine.
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

# Create a Base class for declarative models
# This will be inherited by all of our database models.
Base = declarative_base()

def get_db() -> Generator:
    """
    Dependency to get a database session.

    This function provides a database session that can be used within FastAPI path operations.
    It ensures that the session is properly closed after the request is finished,
    even if errors occur.
    """
    db = SessionLocal()
    try:
        yield db
    except SQLAlchemyError as e:
        # Log the error for debugging purposes
        print(f"Database error: {e}")
        db.rollback() # Rollback on error
        raise
    finally:
        db.close()

def init_db():
    """
    Initializes the database by creating all tables defined in Base.
    This function should be called once on application startup.
    """
    Base.metadata.create_all(bind=engine)
    print("Database tables created (if not already existing).")

if __name__ == "__main__":
    # Example usage for direct database initialization
    # In a production FastAPI app, this would typically be called on startup.
    print(f"Attempting to initialize database at: {DATABASE_URL}")
    init_db()