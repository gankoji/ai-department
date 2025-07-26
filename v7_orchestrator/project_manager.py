import os
import re

class ProjectManager:
    def __init__(self, project_name):
        sanitized_name = re.sub(r'\s+', '_', project_name.lower())
        self.root_dir = os.path.abspath(sanitized_name)
        os.makedirs(self.root_dir, exist_ok=True)
        self.log_path = os.path.join(self.root_dir, "project_log.md")

    def log(self, message):
        with open(self.log_path, "a") as f:
            f.write(message + "\n")

    def create_file(self, file_path, content):
        full_path = os.path.join(self.root_dir, file_path)
        os.makedirs(os.path.dirname(full_path), exist_ok=True)
        with open(full_path, "w") as f:
            f.write(content)
        self.log(f"Created file: {file_path}")
