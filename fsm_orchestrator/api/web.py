"""
FastAPI web server for orchestrator dashboard and API.
"""
from fastapi import FastAPI

app = FastAPI()

@app.get("/projects")
def list_projects():
    """List all projects."""
    pass

@app.get("/transitions")
def list_transitions():
    """List state transitions."""
    pass

@app.get("/metrics")
def get_metrics():
    """Return orchestrator metrics."""
    pass
