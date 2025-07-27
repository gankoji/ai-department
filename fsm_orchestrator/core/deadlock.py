"""
Deadlock detection for FSM transitions.
Implements: loop detection and escalation.
"""
from collections import defaultdict, Counter

class DeadlockDetector:
    """
    Detects repeated transitions and escalates deadlocks.
    """
    def __init__(self, threshold=3):
        # threshold: number of identical transitions to trigger deadlock
        self.threshold = threshold
        self.transition_history = defaultdict(list)  # project_id -> list of transitions

    def observe(self, project_id, transition):
        """Observe a transition for deadlock detection."""
        self.transition_history[project_id].append((transition.from_state, transition.to_state))

    def detect(self, project_id) -> bool:
        """
        Return True if a deadlock is detected for the project (K or more identical transitions).
        """
        history = self.transition_history[project_id]
        if not history:
            return False
        counter = Counter(history)
        for count in counter.values():
            if count >= self.threshold:
                return True
        return False
