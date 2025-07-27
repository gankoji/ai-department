"""
Base agent interface and shared helpers.
"""
from typing import Any

class BaseAgent:
    """
    Base class for all agents. Provides plan(), tools, and token estimation.
    """
    def plan(self, task: Any):
        """Plan next action or transition proposal for a task."""
        pass
    @property
    def tools(self):
        """Return available tools for the agent."""
        return []
    def estimate_tokens(self, payload: Any) -> int:
        """Estimate token usage for a given payload."""
        pass
