import pytest
from fsm_orchestrator.scheduler.scheduler import SchedulerService
from fsm_orchestrator.scheduler.queue import TaskQueue

class DummyCostMonitor:
    def __init__(self, exceed_projects=None):
        self.exceed_projects = set(exceed_projects or [])
    def will_exceed(self, project_id, est_tokens):
        return project_id in self.exceed_projects

@pytest.fixture
def scheduler():
    sched = SchedulerService()
    sched.queue = TaskQueue()
    sched.cost_monitor = DummyCostMonitor()
    sched.project_token_caps = {"p1": 100, "p2": 100}
    sched.project_usage = {"p1": 0, "p2": 0}
    return sched

def make_task(pid, tid, est_tokens):
    return {"project_id": pid, "id": tid, "est_tokens": est_tokens}

def test_wrr_and_dispatch(monkeypatch, scheduler):
    dispatched = []
    monkeypatch.setattr(scheduler, "dispatch", lambda task: dispatched.append(task))
    scheduler.queue.enqueue(make_task("p1", "t1", 10))
    scheduler.queue.enqueue(make_task("p2", "t2", 20))
    scheduler.run()
    assert {t["id"] for t in dispatched} == {"t1", "t2"}

def test_backpressure_token_cap(monkeypatch, scheduler):
    dispatched = []
    monkeypatch.setattr(scheduler, "dispatch", lambda task: dispatched.append(task))
    scheduler.project_usage["p1"] = 81  # >80% of 100
    scheduler.queue.enqueue(make_task("p1", "t1", 10))
    scheduler.queue.enqueue(make_task("p2", "t2", 10))
    scheduler.run()
    assert {t["id"] for t in dispatched} == {"t2"}

def test_backpressure_cost_monitor(monkeypatch, scheduler):
    dispatched = []
    scheduler.cost_monitor = DummyCostMonitor(["p1"])
    monkeypatch.setattr(scheduler, "dispatch", lambda task: dispatched.append(task))
    scheduler.queue.enqueue(make_task("p1", "t1", 10))
    scheduler.queue.enqueue(make_task("p2", "t2", 10))
    scheduler.run()
    assert {t["id"] for t in dispatched} == {"t2"}
