import pytest
from fsm_orchestrator.core.memory import MemoryManager, HOT_MEMORY, COMPRESSED_MEMORY

@pytest.fixture(autouse=True)
def clear_memory():
    HOT_MEMORY.clear()
    COMPRESSED_MEMORY.clear()

def test_fetch_hot():
    mm = MemoryManager()
    HOT_MEMORY["p1"] = [{"content": f"msg{i}"} for i in range(30)]
    hot = mm.fetch_hot("p1", k=10)
    assert len(hot) == 10
    assert hot == [{"content": f"msg{i}"} for i in range(20, 30)]

def test_compress_and_store():
    mm = MemoryManager()
    messages = [{"content": "hello"}, {"content": "world"}]
    compressed = mm.compress(messages)
    assert "summary" in compressed
    assert "embedding" in compressed
    COMPRESSED_MEMORY["p1"] = [compressed]
    assert COMPRESSED_MEMORY["p1"][0]["summary"] == "hello world"

def test_retrieve_mrag():
    mm = MemoryManager()
    HOT_MEMORY["p1"] = [
        {"content": "foo"},
        {"content": "@DECISION(reason=bar) important decision"},
        {"content": "baz"},
    ]
    COMPRESSED_MEMORY["p1"] = [
        {"summary": "summary1", "embedding": [1, 0, 0]},
        {"summary": "summary2", "embedding": [0, 1, 0]},
        {"summary": "summary3", "embedding": [0, 0, 1]},
    ]
    query_embedding = [1, 0, 0]
    result = mm.retrieve("p1", query_embedding, top=2)
    # Should include all hot, tagged, and top-N compressed
    assert any("@DECISION" in m.get("content", "") for m in result)
    assert len(result) >= 3
