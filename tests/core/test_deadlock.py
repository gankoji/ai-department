import pytest
from fsm_orchestrator.core.deadlock import DeadlockDetector
from fsm_orchestrator.core.fsm import Transition

def test_no_deadlock_initially():
    dd = DeadlockDetector(threshold=3)
    assert not dd.detect("p1")

def test_deadlock_detected():
    dd = DeadlockDetector(threshold=2)
    t = Transition("A", "B", 0.9)
    dd.observe("p1", t)
    assert not dd.detect("p1")
    dd.observe("p1", t)
    assert dd.detect("p1")

def test_multiple_transitions():
    dd = DeadlockDetector(threshold=2)
    t1 = Transition("A", "B", 0.9)
    t2 = Transition("B", "C", 0.8)
    dd.observe("p1", t1)
    dd.observe("p1", t2)
    assert not dd.detect("p1")
    dd.observe("p1", t1)
    assert dd.detect("p1")
