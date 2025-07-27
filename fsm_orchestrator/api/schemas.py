"""
Pydantic schemas for API requests and responses.
"""
from pydantic import BaseModel
from typing import Any, Optional

class ProjectRequest(BaseModel):
    name: str

class ProjectResponse(BaseModel):
    id: str
    name: str
    status: str

class TransitionResponse(BaseModel):
    from_state: str
    to_state: str
    confidence: float
    metadata: Optional[dict] = None
