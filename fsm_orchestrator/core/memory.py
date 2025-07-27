"""
Memory manager for compression, retrieval, and MRAG.
Implements: dual-layer store, tagging, and embedding retrieval.
"""
from typing import List, Any, Dict
from ..util.embedding import embed

# Simulated in-memory stores for demonstration
HOT_MEMORY: Dict[str, List[dict]] = {}
COMPRESSED_MEMORY: Dict[str, List[dict]] = {}
COLD_MEMORY: Dict[str, List[dict]] = {}

class MemoryManager:
    """
    Handles memory compression, retrieval, and storage for agent context.
    Implements: hot/warm/cold layers, MRAG, and tagging.
    """
    def fetch_hot(self, project_id, k=20) -> List[Any]:
        """Fetch hot window messages for a project (most recent k)."""
        return HOT_MEMORY.get(project_id, [])[-k:]

    def compress(self, messages: List[Any]) -> dict:
        """Compress messages into summary and embedding (simple join + embed)."""
        # In production, use a real summarizer and embedding model
        summary = " ".join(m["content"] for m in messages)
        embedding = embed(summary)
        compressed = {"summary": summary, "embedding": embedding}
        return compressed

    def store_cold(self, project_id, blob):
        """Archive cold memory to object store (simulated)."""
        if project_id not in COLD_MEMORY:
            COLD_MEMORY[project_id] = []
        COLD_MEMORY[project_id].append(blob)

    def retrieve(self, project_id, query_embedding, top=3):
        """
        Retrieve MRAG chunks for LLM context:
        - hot window
        - tagged decisions
        - top-N embedding matches from compressed memory
        """
        hot = self.fetch_hot(project_id)
        # Find tagged decisions in hot memory
        tagged = [m for m in hot if "@DECISION" in m.get("content", "")]
        # Find top-N similar compressed memories (simulated cosine similarity)
        compressed = COMPRESSED_MEMORY.get(project_id, [])
        def cosine_sim(a, b):
            # Dummy similarity for demonstration
            return sum(x*y for x, y in zip(a, b)) / (1 + sum(x*x for x in a)**0.5 * sum(y*y for y in b)**0.5)
        if compressed and query_embedding:
            scored = sorted(
                compressed,
                key=lambda c: cosine_sim(c["embedding"], query_embedding),
                reverse=True
            )
            retrieved = scored[:top]
        else:
            retrieved = []
        return hot + tagged + retrieved
