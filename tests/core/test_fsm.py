import pytest
from fsm_orchestrator.core.fsm import HierarchicalFSM, State, Transition
from fsm_orchestrator.core.models import TransitionProposal

class DummyState(State):
    def __init__(self):
        self.entered = False
        self.exited = False
        self.acted = False
    def enter(self, context):
        self.entered = True
    def exit(self, context):
        self.exited = True
    def act(self, context):
        self.acted = True

@pytest.fixture
def fsm():
    fsm = HierarchicalFSM()
    fsm.add_state("A", DummyState())
    fsm.add_state("B", DummyState())
    fsm.add_transition(Transition("A", "B", 0.9))
    fsm.add_transition(Transition("B", "A", 0.8))
    return fsm

def test_add_state_and_transition(fsm):
    assert "A" in fsm.states
    assert "B" in fsm.states
    assert len(fsm.transitions) == 2

def test_set_initial_state(fsm):
    fsm.set_initial_state("A")
    assert fsm.current_state == "A"
    assert fsm.states["A"].entered

def test_next_proposals(fsm):
    context = {"urgency": 0.5, "dependency": 0.7, "user_intent": 0.2}
    proposals = fsm.next("A", context)
    assert len(proposals) == 1
    p = proposals[0]
    assert isinstance(p, TransitionProposal)
    assert p.from_state == "A"
    assert p.to_state == "B"
    assert p.urgency == 0.5
    assert p.dependency == 0.7
    assert p.user_intent == 0.2

def test_advance_state(fsm):
    fsm.set_initial_state("A")
    context = {"urgency": 0.5, "dependency": 0.7, "user_intent": 0.2}
    proposal = fsm.next("A", context)[0]
    fsm.advance(proposal)
    assert fsm.current_state == "B"
    assert fsm.states["A"].exited
    assert fsm.states["B"].entered

def test_serialize_deserialize(fsm):
    fsm.set_initial_state("A", {"foo": "bar"})
    data = fsm.serialize()
    assert data["current_state"] == "A"
    assert data["context"]["foo"] == "bar"
    fsm.set_initial_state("B")
    fsm.deserialize(data)
    assert fsm.current_state == "A"
    assert fsm.context["foo"] == "bar"

def test_next_no_transitions(fsm):
    context = {"urgency": 0.1, "dependency": 0.1, "user_intent": 0.1}
    proposals = fsm.next("B", context)  # Only B->A exists, so A->B from B should be empty
    assert proposals
    proposals = fsm.next("C", context)  # No such state
    assert proposals == []
