"""
Pydantic domain models for orchestrator entities.
"""
from pydantic import BaseModel
from typing import Optional, Any

class Event(BaseModel):
    type: str
    payload: Any

class Project(BaseModel):
    id: str
    name: str
    status: str

class Agent(BaseModel):
    id: str
    persona: str
    model_name: str
    active: bool

class TransitionProposal(BaseModel):
    from_state: str
    to_state: str
    confidence: float
    urgency: float
    dependency: float
    user_intent: float
    metadata: Optional[dict] = None

class CostRecord(BaseModel):
    project_id: str
    agent_id: str
    tokens: int
    dollars: float

class Message(BaseModel):
    project_id: str
    agent_id: str
    role: str
    content: str
    token_count: int

class StateTransition(BaseModel):
    project_id: str
    from_state: str
    to_state: str
    confidence: float
    cost: float
