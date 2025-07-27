import pytest
from fsm_orchestrator.core.arbiter import TransitionArbiter
from fsm_orchestrator.core.models import TransitionProposal

def make_proposal(urgency, dependency, user_intent):
    return TransitionProposal(
        from_state="A",
        to_state="B",
        confidence=0.9,
        urgency=urgency,
        dependency=dependency,
        user_intent=user_intent,
        metadata={}
    )

def test_score_weights():
    arbiter = TransitionArbiter()
    p = make_proposal(0.5, 0.7, 0.2)
    score = arbiter.score(p)
    expected = 0.3*0.5 + 0.4*0.7 + 0.3*0.2
    assert abs(score - expected) < 1e-6

def test_select_highest_score():
    arbiter = TransitionArbiter()
    proposals = [
        make_proposal(0.1, 0.1, 0.1),
        make_proposal(0.9, 0.1, 0.1),
        make_proposal(0.1, 0.9, 0.1),
        make_proposal(0.1, 0.1, 0.9),
    ]
    winner = arbiter.select("proj1", proposals)
    scores = [arbiter.score(p) for p in proposals]
    assert winner == proposals[scores.index(max(scores))]

def test_select_empty():
    arbiter = TransitionArbiter()
    assert arbiter.select("proj1", []) is None
