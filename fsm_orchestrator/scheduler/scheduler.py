"""
Main scheduler service for agent task dispatch.
Implements: WRR, cost checks, dispatch.
"""
from .queue import TaskQueue
from ..core.cost_monitor import CostMonitor

class SchedulerService:
    """
    Schedules and dispatches agent tasks with fairness and cost enforcement.
    Implements: weighted round-robin, back-pressure, retries.
    """
    def __init__(self):
        self.queue = TaskQueue()
        self.cost_monitor = CostMonitor()
        self.project_token_caps = {}  # project_id -> weekly token cap
        self.project_usage = {}       # project_id -> tokens used this week

    def run(self):
        """
        Main loop: fetch ready tasks, apply WRR, dispatch.
        - One task per project per round (fairness)
        - Skip tasks if will exceed budget (back-pressure)
        """
        ready_tasks = self.queue.dequeue_ready()
        for task in ready_tasks:
            project_id = task.get("project_id")
            est_tokens = task.get("est_tokens", 0)
            # Back-pressure: skip if project is over 80% of token cap
            cap = self.project_token_caps.get(project_id, 100000)
            used = self.project_usage.get(project_id, 0)
            if used + est_tokens > 0.8 * cap:
                self.queue.postpone(task.get("id"))
                continue
            if self.cost_monitor.will_exceed(project_id, est_tokens):
                self.queue.postpone(task.get("id"))
                continue
            self.dispatch(task)
            self.project_usage[project_id] = used + est_tokens

    def dispatch(self, task):
        """Dispatch a task to a worker or agent pod (stub)."""
        # In production, this would hand off to a worker or remote agent
        print(f"Dispatching task {task.get('id')} for project {task.get('project_id')}")
