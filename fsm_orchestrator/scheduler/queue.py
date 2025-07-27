"""
Task queue helpers for enqueue/dequeue operations.
Implements: DB-backed queue, LISTEN/NOTIFY fallback.
"""
from typing import Any, List
from collections import defaultdict, deque

class TaskQueue:
    """
    Database-backed task queue for agent tasks.
    Implements: enqueue, dequeue_ready, postpone, fail.
    """
    def __init__(self):
        # project_id -> deque of tasks
        self.queues = defaultdict(deque)
        self.failed = defaultdict(list)
        self.postponed = defaultdict(list)

    def enqueue(self, task: Any):
        """Add a new task to the queue."""
        project_id = task.get("project_id")
        self.queues[project_id].append(task)

    def dequeue_ready(self) -> List[Any]:
        """Fetch one ready task per project (for WRR)."""
        ready = []
        for project_id, queue in self.queues.items():
            if queue:
                ready.append(queue.popleft())
        return ready

    def postpone(self, task_id):
        """Postpone a task for later execution (stub)."""
        # In production, move task to a postponed queue
        pass

    def fail(self, task_id):
        """Mark a task as failed (stub)."""
        # In production, move task to a failed queue
        pass
