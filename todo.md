# Jake's Todos

Here is a summary of the V7-Orchestrator project and some future directions for our multi-agent system.

---

### 1. DONE: V7-Orchestrator Evaluation

**Project Summary:**
I have created the **V7-Orchestrator**, a terminal-based application in Python that uses a multi-agent system to automate the game development lifecycle. It takes a project description from you, moves through Ideation, Planning, and Implementation phases, and generates a complete code repository for a new game project. The agents collaborate using a shared transcript, and you guide the process with feedback and approval commands.

**How to Evaluate:**
Here are the steps to test if the system meets your requirements:

1.  **Navigate to the directory:** `cd v7_orchestrator`
2.  **Install dependencies:** `pip install -r requirements.txt`
3.  **Set up your API Key:** Create a `.env` file and add your Google API key like this: `GOOGLE_API_KEY="your_key_here"`
4.  **Run the application:** `python main.py`
5.  **Start a test project:** At the `>` prompt, type `!start a simple clicker game where you tap a cookie`.
6.  **Test the Ideation phase:** The Game Designer and Creative Partner will provide initial ideas. Give them some feedback (e.g., `I think it should be a cat instead of a cookie`).
7.  **Test the approval flow:** Once you're happy with the refined idea, type `!approve` to move to the Planning phase.
8.  **Review the plan:** The Project Manager will propose a file structure. You can either approve it or give feedback to revise it.
9.  **Approve for implementation:** Type `!approve` again.
10. **Verify the output:** The system will generate all the files. Once it's done, check the new directory (e.g., `simple_clicker_game/`) to see if the code and structure are reasonable.

---

### 2. TODO: Create the V7 Strategy & Management Team

**Vision:** Assemble a team of high-level executive agents to handle business operations, strategy, and market analysis.

*   **DONE: Define Executive Personas:**
    *   **Chief Executive Officer (CEO):** Sets the high-level vision, makes final strategic decisions, and defines company-wide goals.
    *   **Chief Financial Officer (CFO):** Focuses on financial planning, budgeting, and monetization strategies for games.
    *   **Chief Marketing Officer (CMO):** Builds on the Marketing Specialist persona to own high-level market analysis, user acquisition strategy, and brand positioning.
*   **Develop New Workflows:**
    *   **Business Plan Generation:** A workflow where the CEO, CFO, and CMO collaborate to produce a comprehensive business plan for a new game.
    *   **Quarterly Goal Setting:** An interactive process where the agents propose and refine quarterly business goals based on market data.
*   **Integrate External Data:**
    *   Give agents the ability to use web search tools to fetch real-time market data, trend analysis, and competitor information to inform their strategies.

---

### 3. TODO: Create the V7 Art Department

**Vision:** Build a team of creative agents capable of generating production-quality visual assets by integrating with state-of-the-art generative models.

*   **DONE: Define Artist Personas:**
    *   **Art Director:** Defines the overall visual style and mood of a project. Acts as the final quality gate for all visual assets.
    *   **Concept Artist:** Generates initial sketches, character designs, and environment concepts based on text prompts.
    *   **2D/Sprite Artist:** Creates game-ready sprites, icons, and UI elements from concepts.
    *   **Texture Artist:** Generates seamless textures for 3D models and environments.
*   **Integrate Generative Models:**
    *   **Image Generation:** Connect the artist agents to a powerful text-to-image diffusion model (e.g., Stable Diffusion via an API, or another modern equivalent). The agent's role is to translate a creative brief into an effective image prompt.
    *   **Video Generation:** Explore text-to-video models for creating short promotional clips or animated visual effects.
*   **Develop an Asset Pipeline:**
    *   **Asset Library:** Create a system for storing, tagging, and managing generated assets.
    *   **Iterative Refinement:** Build a workflow where the Art Director can provide feedback on a generated image, which the Concept Artist then uses to refine the prompt and generate new variations.

### 4. TODO: Infrastructure & Architecture Design

**Vision:** Design the technical foundation for autonomous agent operation with robust infrastructure and clear architectural patterns.

* **Research Technical Requirements:**
  * Document compute, memory, and storage requirements for multi-agent systems
  * Analyze latency and reliability needs for real-time agent collaboration
  * Evaluate containerization and orchestration options (Docker, Kubernetes)

* **Compare Deployment Options:**
  * Create cost/performance analysis comparing local vs API-based agents
  * Test local model performance on target hardware configurations  
  * Document hybrid approach: local for frequent tasks, API for complex reasoning

* **Design System Architecture:**
  * Create technical diagrams for agent communication protocols
  * Design database schema for agent state, conversation history, and project artifacts
  * Specify API contracts between orchestrator, agents, and external systems

---

### 5. TODO: Autonomous Operation System

**Vision:** Transform the interactive V7-Orchestrator into a background service that operates with minimal supervision through asynchronous communication.

* **Design Email-Based Approval Workflow:**
  * Create email templates for different approval types (concept, plan, implementation)
  * Implement secure token-based approval links in emails
  * Build web interface for reviewing and approving agent proposals

* **Build Background Orchestration Service:**
  * Convert current terminal app into a daemon/service architecture
  * Implement project queue management with priority handling
  * Create monitoring dashboard for tracking active projects and agent status

* **Implement Agent Scheduling:**
  * Design task distribution algorithm across available agents
  * Build retry and error handling for failed agent operations
  * Create notification system for project milestones and completion

---

### 6. TODO: Self-Improvement Framework

**Vision:** Establish systematic processes for agents to continuously improve their performance, reduce costs, and enhance output quality.

* **Research Organizational Improvement Processes:**
  * Study retrospective, code review, and performance evaluation practices
  * Adapt peer review processes for agent-to-agent feedback
  * Design metrics framework for measuring agent effectiveness

* **Implement Question-Asking Protocols:**
  * Create templates for agents to ask clarifying questions
  * Build feedback loop system for improving prompt quality
  * Design conversation analysis tools to identify successful interaction patterns

* **Build Cost Optimization Pipeline:**
  * Implement local model integration with fallback to API providers
  * Create cost tracking and budgeting system per project
  * Design model selection logic based on task complexity and cost constraints

* **Create Performance Monitoring:**
  * Build agent performance dashboards with key metrics
  * Implement A/B testing framework for agent prompt variations
  * Design continuous learning system to update agent behaviors based on outcomes

### 7. TODO: Develop a policy for maintaining and updating local models on all devices

* Android already has Gemini. How good is it offline? 
* Mac maybe has Siri? But should I invest in something else? 
* How can you improve local models to work at the quality of the best frontier models? Or at least close?
  * Not every task requires the absolute best intellect. Can I find small, open source models that I can run *tons* of on modest hardware, and see how that goes? Similar to the approach we used in the papers on MEMS IMUs back in the day? 
* Answer the question: what is the current SoTA in open source models?
  * Does the answer to that change if you limit the set to models that are feasible to run on a single, consumer-grade GPU?
  * Is anyone actively iterating on this? 
    * It would seem like Meta is (or was, really), but their focus has recently shifted rather drastically. 
    * What about the chinese models? I'm not opposed to running deepseek if I can keep the data local.

---

### 8. TODO: Implement Non-Deterministic FSM-Based Orchestrator Architecture

**Vision:** Transform the V7-Orchestrator from a linear graph-based system to a sophisticated finite state machine where each agent persona represents a state, enabling dynamic multi-agent conversations with multiple feedback cycles per layer.

**Current Architecture Analysis:**
* **Existing System:** Linear DAG-based workflow (Ideation → Planning → Implementation) with single-pass agent interactions
* **Limitation:** Each agent gets one turn per phase, limited feedback loops, rigid progression
* **Opportunity:** Enable agents to dynamically handoff to each other, have multi-turn conversations, and provide richer collaborative experiences

**Research Findings:**

*From 2024 Multi-Agent Research:*
* **Orchestration Patterns:** Centralized (hub-based), Dynamic (adaptive role assignment), Graph-based (DAG construction)
* **State Management:** Shared belief states, memory/reflection mechanisms, dynamic agent selection
* **Communication Structures:** Hierarchical, decentralized, and centralized coordination models
* **Key Architecture:** LLMO (LLM Orchestration) as finite-state Markov chain with proven convergence properties

*From Gemini CLI Architecture:*
* **Core Pattern:** Separation of CLI (frontend) and Core (backend) with central orchestrator
* **State Management:** Conversation/session state management in core package
* **Tool Integration:** Dynamic tool execution with user approval workflows
* **Modularity:** Event-driven, state-transition model with extensible tool system

**Implementation Strategy:**

* **Phase 1: FSM Design & Architecture (POC Focus)**
  * **FSM State Mapping:**
    * Each agent persona becomes an FSM state: `CEO_State`, `CFO_State`, `CMO_State`, `GameDesigner_State`, `CreativePartner_State`, `LeadEngineer_State`, etc.
    * Additional system states: `RespondToUser_State`, `WaitForApproval_State`, `WorkflowComplete_State`
    * User interaction only occurs in explicit user-involved states (`RespondToUser_State`, `WaitForApproval_State`)
  * **Agent Transition Logic:**
    * Agents make explicit handoff decisions: `APPROVE_AND_HANDOFF`, `REQUEST_REWORK`, `HANDOFF_TO_AGENT`
    * Each agent can approve its inputs (proceed) or request rework from previous agent
    * Agent responses include transition recommendations: "I approve this concept. Handing off to CFO for financial analysis."
  * **State Transition Rules:**
    * Agent-driven transitions: Agents explicitly request next state in their response
    * User-driven transitions: Only from user-involved states (`!approve`, `!handoff CEO`, `!rework`)
    * System-driven transitions: Automatic progression to user states when required
  * **V2 Architecture:**
    * Create `V7OrchestratorV2` with FSM core alongside existing linear system
    * Add mode selection: `!simple` (current DAG) vs `!fsm` (new state machine)
    * Maintain backward compatibility with existing workflows

* **Phase 2: Agent Transition Logic**
  * Implement agent-initiated handoffs (e.g., "Let me bring in the CFO to discuss budget implications")
  * Enable context-aware state transitions based on conversation content
  * Add user commands for explicit state control (!call_agent, !switch_to, !continue)
  * Design conflict resolution for simultaneous agent activation requests

* **Phase 3: Enhanced Interaction Patterns**
  * Multi-turn agent conversations within single workflow phases
  * Agent-to-agent direct communication (bypassing user temporarily)
  * Dynamic workflow creation based on detected user needs
  * Parallel agent processing with synchronization points

* **Phase 4: Advanced Features**
  * Non-deterministic transitions allowing multiple valid next states
  * Agent confidence scoring to influence transition probabilities
  * Learning from conversation patterns to improve state selection
  * Integration with external data sources for context-aware transitions

**Technical Architecture:**

* **State Machine Core:**
  * `FSMOrchestrator` class managing global state and transitions
  * `AgentState` base class with entry/exit hooks and transition logic
  * `ConversationMemory` for cross-state context persistence
  * `TransitionEngine` for parsing agent responses and executing state changes
  * `StateVisualizer` for displaying current FSM state to user

* **Agent Integration:**
  * Refactor existing Agent class to inherit from `AgentState`
  * **Agent Response Format:** Structured responses with transition directives:
    ```
    {
      "content": "agent response text",
      "transition": "APPROVE_AND_HANDOFF",
      "next_agent": "CFO",
      "approval_status": "APPROVED" | "REQUEST_REWORK",
      "rework_reason": "optional explanation for rework request"
    }
    ```
  * **Agent Capabilities:**
    * `can_approve_input()`: Agent evaluates and approves/rejects previous agent's work
    * `determine_next_agent()`: Agent decides optimal next state based on conversation context
    * `request_rework()`: Agent can send work back to previous agent with specific feedback

* **State Transition Mechanics:**
  * **Agent-Driven:** Parse transition directives from agent responses
  * **Approval Chain:** Track approval status through agent chain, enable rework loops
  * **User-Involved States:** Automatic transition to `RespondToUser_State` when user input needed
  * **State History:** Maintain stack for rework operations (agent can return to previous state)

* **User Interface:**
  * Enhanced command system supporting FSM navigation
  * Real-time state visualization in terminal
  * Agent conversation threads with branching support
  * Approval workflows for state transitions vs. agent responses

**Complexity Assessment:**
* **High Complexity Task:** Requires fundamental architectural changes to current system
* **Estimated Scope:** 2-3 week implementation for core FSM, additional 1-2 weeks for advanced features
* **Prerequisites:** Deep understanding of current agent interaction patterns and user workflows
* **Risk Factors:** Potential increase in conversation complexity, need for sophisticated state management

**Success Metrics:**
* Agents can have 3+ turn conversations within single workflow phase
* User can interrupt and redirect agent conversations dynamically
* System supports 5+ simultaneous agent collaboration patterns
* Conversation quality improves through richer agent interactions
* Development workflow efficiency increases by 25%+ due to better agent coordination

**Questions for Implementation:**
1. Should state transitions be primarily agent-driven, user-driven, or context-driven?
2. How should we handle state loops and prevent infinite agent conversations?
3. What level of user control vs. autonomous agent behavior is optimal?
4. Should we maintain backward compatibility with current linear workflows?
5. How do we prevent the FSM from becoming overwhelming for users to navigate?

**Phase 1 POC Deliverables:**
* **Core FSM Implementation:**
  * `FSMOrchestrator` class with basic state management
  * 3-4 agent states (CEO, CFO, Game Designer, Creative Partner) + user/approval states
  * State transition engine that parses agent response directives
  * Simple state visualization in terminal (e.g., "Current State: [CEO_State] → Pending: CFO_State")

* **Agent Approval/Rework System:**
  * Modified agent prompts to include transition directives in responses
  * Rework loop capability (agent can reject previous agent's work and request changes)
  * Approval chain tracking through conversation flow

* **Dual Mode Support:**
  * Command to switch between `!simple` (current DAG) and `!fsm` (new state machine)
  * Maintain existing workflows for backward compatibility
  * New FSM workflow that demonstrates agent-to-agent handoffs with approval cycles

**Next Steps for Phase 1:**
* Create `v7_orchestrator_v2.py` with basic FSM infrastructure
* Implement `AgentState` base class and refactor 2-3 existing agents
* Build transition parsing system for agent responses
* Test basic handoff pattern: Creative Partner → Game Designer → Creative Partner (rework) → Game Designer (approve) → CEO
* Add state visualization and FSM command system to main CLI
