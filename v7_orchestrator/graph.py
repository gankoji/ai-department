from typing import TypedDict, List
from langgraph.graph import StateGraph, END
from project_manager import ProjectManager
from agents import Agent
import re

class V7State(TypedDict):
    project_manager: ProjectManager
    transcript: List[str]
    current_phase: str
    user_command: str
    plan: List[str] # List of file paths to be created
    llm: any

# --- Ideation Nodes ---
def creative_partner_node_1(state):
    print("--- Ideation: Creative Partner (Turn 1) ---")
    task = "Brainstorm initial concepts for the project. Focus on narrative, world-building, and player experience."
    if len(state["transcript"]) > 1 and "User Feedback" in state["transcript"][-1]:
         task = "Refine the game concept based on the latest user feedback in the transcript."

    creative_partner = Agent("creative_partner", state["llm"])
    cp_response = creative_partner.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Creative Partner]:**\n{cp_response}")
    return state

def game_designer_node(state):
    print("--- Ideation: Game Designer ---")
    task = "Elaborate on the Creative Partner's ideas, focusing on core gameplay mechanics, loops, and player progression."
    game_designer = Agent("game_designer", state["llm"])
    gd_response = game_designer.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Game Designer]:**\n{gd_response}")
    return state

def lead_engineer_challenge_node(state):
    print("--- Ideation: Lead Engineer Challenge ---")
    task = ("Review the Game Designer's proposed mechanics. Your task is to act as the 'Pragmatist' and run them through the 'Technical Gauntlet.' "
            "Ask probing questions about feasibility, complexity, and performance. Your output must be a numbered list of technical challenges.")
    
    lead_engineer_agent = Agent("lead_engineer", state["llm"])
    challenge_response = lead_engineer_agent.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Lead Engineer]:**\nI've reviewed the design. It's promising, but the following technical questions must be answered:\n{challenge_response}")
    return state

def game_designer_refinement_node(state):
    print("--- Ideation: Game Designer Refinement ---")
    task = ("The Lead Engineer has challenged your design. You must now refine your mechanics and systems to directly address their technical concerns. "
            "Provide a revised design that is both creative and technically feasible.")

    game_designer_agent = Agent("game_designer", state["llm"])
    refined_gd_response = game_designer_agent.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Game Designer]:**\nThank you for the technical feedback. Here is the revised, more grounded design:\n{refined_gd_response}")
    return state


def creative_partner_node_2(state):
    print("--- Ideation: Creative Partner (Turn 2) ---")
    task = "Refine the entire concept, weaving together the narrative and gameplay ideas into a cohesive vision. Summarize the core concept."
    creative_partner = Agent("creative_partner", state["llm"])
    cp_response = creative_partner.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Creative Partner]:**\n{cp_response}")
    return state

# --- Planning Nodes ---
def planning_node(state):
    print("--- Planning Phase: Initial Plan ---")
    task = ("Based on the approved concept, create a detailed implementation plan. "
            "The output MUST be a markdown list of the file paths that need to be created. "
            "Example: `* backend/main.py`")
    
    pm_agent = Agent("project_manager", state["llm"])
    plan_response = pm_agent.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Project Manager]:**\nHere is the proposed file structure:\n{plan_response}")

    file_list = re.findall(r'^\s*[*-+]\s*(`?)([^`]+)\1\s*$', plan_response, re.MULTILINE)
    state["plan"] = [item[1] for item in file_list]
    return state

def ceo_challenge_node(state):
    print("--- Planning Phase: CEO Challenge ---")
    task = ("Review the following plan from the Project Manager. Your task is to act as the 'Chief Inquisitor' and run it through 'The Gauntlet.' "
            "Ask the most piercing, relevant questions to expose weaknesses, assumptions, and risks. "
            "Your output must be a numbered list of challenges.")
    
    ceo_agent = Agent("ceo", state["llm"])
    challenge_response = ceo_agent.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Chief Executive Officer]:**\nI have reviewed the plan. It will not proceed until the following points are addressed:\n{challenge_response}")
    return state

def planning_refinement_node(state):
    print("--- Planning Phase: Plan Refinement ---")
    task = ("The CEO has challenged your initial plan. You must now create a revised and improved plan that directly addresses all of the CEO's questions and concerns. "
            "Your output MUST be a markdown list of the file paths for the *revised* plan.")

    pm_agent = Agent("project_manager", state["llm"])
    refined_plan_response = pm_agent.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Project Manager]:**\nThank you for the feedback. Here is the revised and strengthened plan:\n{refined_plan_response}")

    file_list = re.findall(r'^\s*[*-+]\s*(`?)([^`]+)\1\s*$', refined_plan_response, re.MULTILINE)
    state["plan"] = [item[1] for item in file_list]
    return state

# --- Implementation Node ---
def implementation_node(state):
    print("--- Implementation Phase ---")
    pm = state["project_manager"]

    for file_path in state["plan"]:
        if ".cs" in file_path: agent_name = "csharp_unity_developer"
        elif ".py" in file_path: agent_name = "python_backend_developer"
        elif "Dockerfile" in file_path or ".tf" in file_path: agent_name = "devops_engineer"
        else: agent_name = "project_manager"

        print(f"Generating {file_path} with {agent_name}...")
        developer_agent = Agent(agent_name, state["llm"])
        task = f"Generate the complete code for `{file_path}` based on the plan. Only output raw code."
        code_content = developer_agent.invoke(state["transcript"], task)
        cleaned_code = re.sub(r"^(?:```|''')[a-zA-Z]*\n?|\n?(?:```|''')$", '', code_content).strip()
        pm.create_file(file_path, cleaned_code)
        state["transcript"].append(f"**[{agent_name.replace('_', ' ').title()}]:** Created `{file_path}`.")
    return state

# --- Graph Definitions ---

def create_ideation_graph(llm):
    graph = StateGraph(V7State)
    graph.add_node("creative_partner_node_1", creative_partner_node_1)
    graph.add_node("game_designer_node", game_designer_node)
    graph.add_node("lead_engineer_challenge", lead_engineer_challenge_node)
    graph.add_node("game_designer_refinement", game_designer_refinement_node)
    graph.add_node("creative_partner_node_2", creative_partner_node_2)
    graph.set_entry_point("creative_partner_node_1")
    graph.add_edge("creative_partner_node_1", "game_designer_node")
    graph.add_edge("game_designer_node", "lead_engineer_challenge")
    graph.add_edge("lead_engineer_challenge", "game_designer_refinement")
    graph.add_edge("game_designer_refinement", "creative_partner_node_2")
    graph.add_edge("creative_partner_node_2", END)
    return graph.compile()

def create_planning_graph(llm):
    graph = StateGraph(V7State)
    graph.add_node("planning", planning_node)
    graph.add_node("ceo_challenge", ceo_challenge_node)
    graph.add_node("planning_refinement", planning_refinement_node)
    graph.set_entry_point("planning")
    graph.add_edge("planning", "ceo_challenge")
    graph.add_edge("ceo_challenge", "planning_refinement")
    graph.add_edge("planning_refinement", END)
    return graph.compile()

def create_implementation_graph(llm):
    graph = StateGraph(V7State)
    graph.add_node("implementation", implementation_node)
    graph.set_entry_point("implementation")
    graph.add_edge("implementation", END)
    return graph.compile()

# --- Business Plan Generation Nodes ---

def ceo_business_vision_node(state):
    print("--- Business Planning: CEO Strategic Vision ---")
    task = ("As CEO, provide the high-level strategic vision for this game concept. Define the company-wide goals, "
            "target market positioning, and how this fits into V7 Games' overall strategy. Set the strategic framework "
            "that the CFO and CMO will build upon.")
    
    ceo_agent = Agent("ceo", state["llm"])
    ceo_response = ceo_agent.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Chief Executive Officer]:**\n{ceo_response}")
    return state

def cfo_financial_analysis_node(state):
    print("--- Business Planning: CFO Financial Analysis ---")
    task = ("Based on the CEO's strategic vision, create a comprehensive financial analysis and business model. "
            "Include revenue projections, cost structure, monetization strategy, budget requirements, and ROI analysis. "
            "Present your findings in clear financial models with key metrics.")
    
    cfo_agent = Agent("cfo", state["llm"])
    cfo_response = cfo_agent.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Chief Financial Officer]:**\n{cfo_response}")
    return state

def cmo_market_strategy_node(state):
    print("--- Business Planning: CMO Marketing Strategy ---")
    task = ("Building on the CEO's vision and CFO's financial framework, develop a comprehensive marketing and "
            "go-to-market strategy. Include target audience analysis, competitive positioning, marketing channels, "
            "user acquisition strategy, and brand positioning. Provide a detailed marketing plan with budget allocation.")
    
    cmo_agent = Agent("cmo", state["llm"])
    cmo_response = cmo_agent.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Chief Marketing Officer]:**\n{cmo_response}")
    return state

def c_suite_synthesis_node(state):
    print("--- Business Planning: C-Suite Synthesis ---")
    task = ("Collaborate to synthesize the strategic vision, financial model, and marketing strategy into a "
            "comprehensive business plan. Address any conflicts between strategies and create unified recommendations. "
            "The CEO should lead this synthesis and present the final integrated business plan.")
    
    ceo_agent = Agent("ceo", state["llm"])
    synthesis_response = ceo_agent.invoke(state["transcript"], task)
    state["transcript"].append(f"**[C-Suite Synthesis]:**\n{synthesis_response}")
    return state

# --- Quarterly Goals Nodes ---

def ceo_goals_framework_node(state):
    print("--- Quarterly Goals: CEO Strategic Framework ---")
    task = ("As CEO, establish the strategic framework for our quarterly goals. Define the company's top 3-5 "
            "strategic priorities for the quarter, key performance indicators, and success metrics. Consider market "
            "conditions, competitive landscape, and company growth stage.")
    
    ceo_agent = Agent("ceo", state["llm"])
    ceo_response = ceo_agent.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Chief Executive Officer]:**\n{ceo_response}")
    return state

def cfo_financial_goals_node(state):
    print("--- Quarterly Goals: CFO Financial Targets ---")
    task = ("Based on the CEO's strategic framework, propose specific financial goals and targets for the quarter. "
            "Include revenue targets, cost management objectives, profitability goals, and key financial metrics. "
            "Ensure goals are measurable, achievable, and aligned with the strategic priorities.")
    
    cfo_agent = Agent("cfo", state["llm"])
    cfo_response = cfo_agent.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Chief Financial Officer]:**\n{cfo_response}")
    return state

def cmo_marketing_goals_node(state):
    print("--- Quarterly Goals: CMO Marketing Objectives ---")
    task = ("Aligned with the CEO's framework and CFO's financial targets, define specific marketing and growth "
            "objectives for the quarter. Include user acquisition targets, brand awareness goals, community growth "
            "metrics, and marketing campaign objectives. Ensure goals support the overall business targets.")
    
    cmo_agent = Agent("cmo", state["llm"])
    cmo_response = cmo_agent.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Chief Marketing Officer]:**\n{cmo_response}")
    return state

def quarterly_goals_finalization_node(state):
    print("--- Quarterly Goals: Final Goal Setting ---")
    task = ("Review all proposed goals from the CEO, CFO, and CMO. Synthesize them into a final set of quarterly "
            "objectives that are balanced, achievable, and mutually supportive. Present the final quarterly goals "
            "with clear ownership, timelines, and success metrics.")
    
    ceo_agent = Agent("ceo", state["llm"])
    final_response = ceo_agent.invoke(state["transcript"], task)
    state["transcript"].append(f"**[Quarterly Goals - Final]:**\n{final_response}")
    return state

# --- New Graph Definitions ---

def create_business_plan_graph(llm):
    graph = StateGraph(V7State)
    graph.add_node("ceo_business_vision", ceo_business_vision_node)
    graph.add_node("cfo_financial_analysis", cfo_financial_analysis_node)
    graph.add_node("cmo_market_strategy", cmo_market_strategy_node)
    graph.add_node("c_suite_synthesis", c_suite_synthesis_node)
    graph.set_entry_point("ceo_business_vision")
    graph.add_edge("ceo_business_vision", "cfo_financial_analysis")
    graph.add_edge("cfo_financial_analysis", "cmo_market_strategy")
    graph.add_edge("cmo_market_strategy", "c_suite_synthesis")
    graph.add_edge("c_suite_synthesis", END)
    return graph.compile()

def create_quarterly_goals_graph(llm):
    graph = StateGraph(V7State)
    graph.add_node("ceo_goals_framework", ceo_goals_framework_node)
    graph.add_node("cfo_financial_goals", cfo_financial_goals_node)
    graph.add_node("cmo_marketing_goals", cmo_marketing_goals_node)
    graph.add_node("quarterly_goals_finalization", quarterly_goals_finalization_node)
    graph.set_entry_point("ceo_goals_framework")
    graph.add_edge("ceo_goals_framework", "cfo_financial_goals")
    graph.add_edge("cfo_financial_goals", "cmo_marketing_goals")
    graph.add_edge("cmo_marketing_goals", "quarterly_goals_finalization")
    graph.add_edge("quarterly_goals_finalization", END)
    return graph.compile()