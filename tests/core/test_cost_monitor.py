import pytest
from fsm_orchestrator.core.cost_monitor import CostMonitor

def test_record_and_report():
    cm = CostMonitor(token_budget=100, dollar_budget=10.0)
    cm.record("p1", "a1", 10, 1.0)
    cm.record("p1", "a2", 20, 2.0)
    report = cm.report("p1")
    assert report["tokens_used"] == 30
    assert report["dollars_used"] == 3.0
    assert report["token_budget"] == 100
    assert report["dollar_budget"] == 10.0

def test_will_exceed():
    cm = CostMonitor(token_budget=50)
    cm.record("p1", "a1", 40, 1.0)
    assert not cm.will_exceed("p1", 5)
    assert cm.will_exceed("p1", 15)
