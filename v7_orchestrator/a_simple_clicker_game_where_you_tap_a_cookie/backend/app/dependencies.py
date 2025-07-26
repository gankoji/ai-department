from typing import Generator
from sqlalchemy.orm import Session
from app.database import SessionLocal

def get_db() -> Generator[Session, None, None]:
    """
    Dependency to get a database session.

    Yields a SQLAlchemy SessionLocal instance and ensures it is closed
    after the request is processed.
    """
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()