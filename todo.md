# Jake's Todos

Here is a summary of the V7-Orchestrator project and some future directions for our multi-agent system.

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

