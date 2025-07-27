"""
Transition arbitration logic for resolving agent proposals.
Implements: scoring and selection of transitions.
"""
from typing import List
from .models import TransitionProposal

class TransitionArbiter:
    """
    Scores and selects among competing transition proposals.
    Implements: urgency/dependency/user_intent weighted scoring.
    """
    def score(self, proposal: TransitionProposal) -> float:
        """Score a proposal based on weighted factors (urgency 0.3, dependency 0.4, user_intent 0.3)."""
        return (
            0.3 * proposal.urgency +
            0.4 * proposal.dependency +
            0.3 * proposal.user_intent
        )

    def select(self, project_id, proposals: List[TransitionProposal]):
        """Select the winning transition proposal for a project based on highest score."""
        if not proposals:
            return None
        scored = [(self.score(p), p) for p in proposals]
        scored.sort(reverse=True, key=lambda x: x[0])
        return scored[0][1]
