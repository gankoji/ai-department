import os
import readline # For better input handling
from dotenv import load_dotenv
from langchain_google_genai import ChatGoogleGenerativeAI
from project_manager import ProjectManager
from graph import (
    create_ideation_graph,
    create_planning_graph,
    create_implementation_graph,
    create_business_plan_graph,
    create_quarterly_goals_graph,
)

# --- Setup ---
load_dotenv()
llm = ChatGoogleGenerativeAI(model="gemini-2.5-flash", google_api_key=os.getenv("GOOGLE_API_KEY"))
ideation_app = create_ideation_graph(llm)
planning_app = create_planning_graph(llm)
implementation_app = create_implementation_graph(llm)
business_plan_app = create_business_plan_graph(llm)
quarterly_goals_app = create_quarterly_goals_graph(llm)

# --- State ---
current_state = None

# --- Helper Function ---
def run_phase(graph_app, state):
    """Helper to run a graph and print the output."""
    print("\nOrchestrator is thinking...")
    response = graph_app.invoke(state)
    
    print("\n--- Latest Updates ---")
    # Find the index of the last user/system message to print everything that followed.
    last_context_message_index = -1
    for i, msg in reversed(list(enumerate(response["transcript"]))):
        if msg.startswith(("**[User Feedback]**", "**Project Goal**", "**[System]**")):
            last_context_message_index = i
            break
    
    messages_to_show = response["transcript"][last_context_message_index + 1:]
    for message in messages_to_show:
        print(message)
        
    return response

# --- Main REPL Loop ---
print("--- V7 Games Orchestrator ---")
print("Welcome! Let's build games and grow our business.")
print("Commands:")
print("  !start [project idea] - Start a new game project")
print("  !business [game concept] - Create a business plan for a game")
print("  !goals - Set quarterly business goals")
print("  !approve - Approve current phase")
print("  !exit - Exit the orchestrator")

while True:
    try:
        # 1. Always wait for user input
        if current_state:
            print(f"\n--- Waiting for input for {current_state['current_phase']} phase ---")
            print("You can provide feedback to iterate, or type `!approve` to proceed.")
        
        user_input = input("> ")
        if not user_input:
            continue

        # 2. Handle commands and execute one action
        if user_input.lower() == "!exit":
            print("Exiting. Goodbye!")
            break

        elif user_input.startswith("!start"):
            project_name = user_input.split(" ", 1)[1]
            pm = ProjectManager(project_name)
            current_state = {
                "project_manager": pm,
                "transcript": [f"**Project Goal:** {project_name}"],
                "current_phase": "Ideation",
                "plan": [],
                "llm": llm
            }
            print(f"--- New project started: '{project_name}' ---")
            print("--- Running Initial Ideation ---")
            current_state = run_phase(ideation_app, current_state)

        elif user_input.startswith("!business"):
            game_concept = user_input.split(" ", 1)[1] if len(user_input.split(" ", 1)) > 1 else "general game concept"
            current_state = {
                "project_manager": None,
                "transcript": [f"**Business Plan Request:** Create a comprehensive business plan for: {game_concept}"],
                "current_phase": "Business_Planning",
                "plan": [],
                "llm": llm
            }
            print(f"--- Business Plan Generation Started for: '{game_concept}' ---")
            current_state = run_phase(business_plan_app, current_state)

        elif user_input.startswith("!goals"):
            current_state = {
                "project_manager": None,
                "transcript": [f"**Quarterly Goals:** Collaborate to set strategic quarterly business goals for V7 Games"],
                "current_phase": "Quarterly_Goals",
                "plan": [],
                "llm": llm
            }
            print("--- Quarterly Goals Setting Started ---")
            current_state = run_phase(quarterly_goals_app, current_state)

        elif not current_state:
            print("Please start a project first with `!start [description]`")

        elif user_input.lower() == "!approve":
            if current_state["current_phase"] == "Ideation":
                print("\n--- Ideation Approved! Moving to Planning. ---")
                current_state["current_phase"] = "Planning"
                current_state["transcript"].append("**[System]:** Ideation approved by user. The Project Manager will now create a file plan.")
                current_state = run_phase(planning_app, current_state)

            elif current_state["current_phase"] == "Planning":
                print("\n--- Plan Approved! Moving to Implementation. ---")
                current_state["current_phase"] = "Implementation"
                current_state["transcript"].append("**[System]:** Plan approved by user. The developers will now generate the code.")
                current_state = run_phase(implementation_app, current_state)
                print("\n--- Project Implementation Complete! --- ")
                current_state = None # Reset for a new project
                print("You can start a new project with `!start`.")

            elif current_state["current_phase"] == "Business_Planning":
                print("\n--- Business Plan Approved and Complete! ---")
                current_state = None
                print("You can start a new workflow with `!start`, `!business`, or `!goals`.")

            elif current_state["current_phase"] == "Quarterly_Goals":
                print("\n--- Quarterly Goals Approved and Complete! ---")
                current_state = None
                print("You can start a new workflow with `!start`, `!business`, or `!goals`.")
        
        else: # Handle user feedback
            print(f"\n--- Feedback received! Re-running the {current_state['current_phase']} phase. ---")
            current_state["transcript"].append(f"**[User Feedback]:**\n{user_input}")
            
            if current_state["current_phase"] == "Ideation":
                current_state = run_phase(ideation_app, current_state)
            elif current_state["current_phase"] == "Planning":
                current_state = run_phase(planning_app, current_state)
            elif current_state["current_phase"] == "Business_Planning":
                current_state = run_phase(business_plan_app, current_state)
            elif current_state["current_phase"] == "Quarterly_Goals":
                current_state = run_phase(quarterly_goals_app, current_state)

    except (KeyboardInterrupt, EOFError):
        print("\nExiting. Goodbye!")
        break
