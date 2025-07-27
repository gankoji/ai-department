"""
Cost monitoring and enforcement for token and $ budgets.
Implements: cost tracking and budget checks.
"""
from collections import defaultdict

class CostMonitor:
    """
    Tracks and enforces project cost budgets.
    """
    def __init__(self, token_budget=100000, dollar_budget=100.0):
        # Example budgets; adjust as needed
        self.token_budget = token_budget
        self.dollar_budget = dollar_budget
        self.usage = defaultdict(lambda: {"tokens": 0, "dollars": 0.0})

    def record(self, project_id, agent_id, tokens, dollars):
        """Record token and $ usage for a project/agent."""
        self.usage[project_id]["tokens"] += tokens
        self.usage[project_id]["dollars"] += dollars

    def will_exceed(self, project_id, est_tokens) -> bool:
        """
        Return True if the estimated tokens would exceed the budget for the project.
        """
        used = self.usage[project_id]["tokens"]
        return (used + est_tokens) > self.token_budget

    def report(self, project_id) -> dict:
        """Return a cost report for the project."""
        return {
            "tokens_used": self.usage[project_id]["tokens"],
            "dollars_used": self.usage[project_id]["dollars"],
            "token_budget": self.token_budget,
            "dollar_budget": self.dollar_budget,
        }
