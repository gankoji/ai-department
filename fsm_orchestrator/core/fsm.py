"""
Hierarchical FSM engine and state/transition definitions.
Implements: state machine logic and serialization.
"""
from typing import Any, Dict, Optional, List
from .models import TransitionProposal

class State:
    """Abstract FSM state."""
    def enter(self, context: Dict[str, Any]):
        """Called when entering the state."""
        pass
    def exit(self, context: Dict[str, Any]):
        """Called when exiting the state."""
        pass
    def act(self, context: Dict[str, Any]):
        """Perform main state logic."""
        pass

class Transition:
    def __init__(self, from_state: str, to_state: str, confidence: float, metadata: Optional[dict] = None):
        self.from_state = from_state
        self.to_state = to_state
        self.confidence = confidence
        self.metadata = metadata or {}

class HierarchicalFSM:
    """Hierarchical FSM engine for managing nested states."""
    def __init__(self):
        self.states: Dict[str, State] = {}
        self.transitions: List[Transition] = []
        self.current_state: Optional[str] = None
        self.context: Dict[str, Any] = {}

    def add_state(self, name: str, state: State):
        self.states[name] = state

    def add_transition(self, transition: Transition):
        self.transitions.append(transition)

    def set_initial_state(self, state_name: str, context: Dict[str, Any] = None):
        self.current_state = state_name
        self.context = context or {}
        if state_name in self.states:
            self.states[state_name].enter(self.context)

    def next(self, state_name: str, context: Dict[str, Any]):
        """Return next TransitionProposal(s) for a given state and context."""
        proposals = []
        for t in self.transitions:
            if t.from_state == state_name:
                # Example: confidence could be computed or static
                proposals.append(
                    TransitionProposal(
                        from_state=t.from_state,
                        to_state=t.to_state,
                        confidence=t.confidence,
                        urgency=context.get('urgency', 0.0),
                        dependency=context.get('dependency', 0.0),
                        user_intent=context.get('user_intent', 0.0),
                        metadata=t.metadata
                    )
                )
        return proposals

    def advance(self, proposal: TransitionProposal):
        """Advance FSM to the next state based on a TransitionProposal."""
        if self.current_state:
            self.states[self.current_state].exit(self.context)
        self.current_state = proposal.to_state
        if self.current_state in self.states:
            self.states[self.current_state].enter(self.context)

    def serialize(self):
        """Serialize FSM state for DB storage."""
        return {
            'current_state': self.current_state,
            'context': self.context,
        }

    def deserialize(self, data):
        """Restore FSM state from DB."""
        self.current_state = data.get('current_state')
        self.context = data.get('context', {})
        if self.current_state in self.states:
            self.states[self.current_state].enter(self.context)
