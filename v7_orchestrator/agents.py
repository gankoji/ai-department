from langchain_google_genai import ChatGoogleGenerativeAI

class Agent:
    def __init__(self, persona_name, llm):
        with open(f"personas/master_system_prompt.md", "r") as f:
            self.master_prompt = f.read()
        with open(f"personas/{persona_name}_persona.md", "r") as f:
            self.persona_prompt = f.read()
        self.llm = llm

    def invoke(self, conversation_history, task):
        final_prompt = f"{self.master_prompt}\n\n{self.persona_prompt}\n\nConversation History:\n{'\n'.join(conversation_history)}\n\nTask: {task}"
        response = self.llm.invoke(final_prompt)
        return response.content
