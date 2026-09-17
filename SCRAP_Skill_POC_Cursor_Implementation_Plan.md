# SCRAP Skill POC --- Cursor Implementation Plan

## 1. Purpose

Build the first POC for evolving the existing SSC Planning / Runtime
SCRAP Agent toward the skill-based architecture discussed in the
architecture meeting.

**Critical scope decision:** do not rebuild the existing Runtime SCRAP
Agent. Reuse its MCP integrations, retrieval, routing, configuration,
tracing, and tests wherever possible.

The POC adds a **SCRAP Analysis Skill** around the existing capability
and demonstrates:

-   skill instructions and business context
-   LLM-based capability/intent understanding
-   orchestration of existing MCP/runtime capabilities
-   LLM analysis of retrieved data
-   a structured response contract for future dynamic UI
-   a small end-to-end demo

The meeting distinguishes a Skill from a low-level MCP tool: a Skill is
a higher-level, orchestrated business capability that can use domain
knowledge and multiple tools.

------------------------------------------------------------------------

## 2. Target Architecture

``` text
                           USER
                             |
                             v
                    +------------------+
                    |     My Agent     |
                    |  Conversational  |
                    |       UI         |
                    +--------+---------+
                             |
                             v
                    +------------------+
                    |    Super Agent    |
                    | Intent / Routing |
                    +--------+---------+
                             |
                             v
                    +------------------+
                    | Planning Agent   |
                    | Planning Domain  |
                    +--------+---------+
                             |
                             v
                    +------------------+
                    |   SCRAP Skill    |  <-- POC focus
                    +--------+---------+
                             |
             +---------------+---------------+
             |               |               |
             v               v               v
       Existing MCP     Existing MCP    Existing MCP
          Tool #1          Tool #2          Tool #3
          Search          Inventory        Planning
             |               |               |
             +---------------+---------------+
                             |
                             v
                    Enterprise Data
                             |
                             v
                    +------------------+
                    |   LLM Analysis   |
                    | Aggregate/Analyze|
                    | Generate insight |
                    +--------+---------+
                             |
                             v
                    +------------------+
                    | Skill Response   |
                    |    UI Schema     |
                    +--------+---------+
                             |
              +--------------+--------------+
              |              |              |
              v              v              v
            Text           Table          Chart
                         + Insights
```

------------------------------------------------------------------------

## 3. Current State vs POC

### Current Runtime SCRAP Agent

``` text
User Query
    |
Keyword / Intent Matching
    |
Corresponding MCP
    |
Business Data
    |
Response
```

### POC

``` text
User Query
    |
    v
SCRAP Skill
    |
    +--> Skill instructions
    +--> Business rules
    +--> LLM reasoning
    +--> Tool orchestration
    |
    v
Existing Runtime / MCP capabilities
    |
    v
Retrieved data
    |
    v
LLM analysis
    |
    v
Structured Skill Response
    |
    +--> Text
    +--> Table
    +--> Chart
    +--> Insights
```

The main change is the introduction of the Skill layer. Existing MCP
implementations should remain reusable.

------------------------------------------------------------------------

## 4. POC Objective

Create one skill:

`SCRAP Analysis Skill`

Support a small representative set of questions:

### Query A

> What SCRAP items are pending?

Expected flow:

``` text
User
 -> SCRAP Skill
 -> Existing SCRAP retrieval capability
 -> Result
 -> Table + summary
```

### Query B

> Show SCRAP trend by region.

Expected flow:

``` text
User
 -> SCRAP Skill
 -> Existing SCRAP data
 -> Aggregate / analyze
 -> Chart/table response
```

### Query C

> Why are SCRAP items increasing?

Expected flow:

``` text
User
 -> SCRAP Skill
 -> Determine required information
 -> Existing SCRAP capability
 -> Optional inventory/planning capability
 -> LLM analysis
 -> Insights + supporting data
```

Use only capabilities that actually exist in the repository. Do not
invent business data or pretend a missing MCP exists.

------------------------------------------------------------------------

## 5. What NOT to Build in POC-1

Do not implement yet:

-   full enterprise Super Agent
-   full My Agent integration
-   A2A
-   multiple domain agents
-   large skill catalog
-   20+ skills
-   complete dynamic UI framework
-   replacement of existing MCP tools
-   production data services
-   complex graph execution engine
-   speculative business rules
-   broad refactoring of the existing runtime

The goal is to prove the Skill concept.

------------------------------------------------------------------------

# 6. Skill Contract

Create a machine-readable definition similar to:

``` yaml
name: scrap_analysis
version: "0.1"
description: >
  Analyze SCRAP information and provide business-oriented
  answers, summaries, tables, and insights.

when_to_use:
  - pending SCRAP questions
  - SCRAP trend questions
  - SCRAP regional analysis
  - questions asking why SCRAP is increasing/decreasing

inputs:
  - user_query

capabilities:
  - retrieve_scrap_data
  - analyze_scrap_data
  - summarize_scrap_data
  - generate_table
  - generate_chart

tools:
  - existing_scrap_search
  - existing_inventory
  - existing_planning

output_formats:
  - text
  - table
  - chart
  - insight

requires_llm_reasoning: true
```

**Important:** actual tool names must be discovered from the repository.
Never invent names in implementation.

------------------------------------------------------------------------

# 7. Skill Instructions

Create a human-readable instruction file, for example:

`skills/scrap_analysis/instructions.md`

Suggested content:

``` markdown
# SCRAP Analysis Skill

## Purpose

Provide business-oriented analysis of SCRAP information.

## When to use

Use this skill when the user asks about:
- pending SCRAP
- SCRAP trends
- regional SCRAP
- reasons for SCRAP changes
- SCRAP-related analysis

## Execution principles

1. Understand the user question.
2. Determine required information.
3. Reuse existing runtime/MCP capabilities.
4. Retrieve factual data.
5. Do not invent unavailable data.
6. Analyze retrieved information.
7. Identify patterns only when supported by data.
8. Return a concise business explanation.
9. Select an appropriate output format.

## Output

Prefer:
- summary for simple questions
- table for record-level data
- chart for trends/breakdowns
- insights for analytical questions
```

Do not invent domain rules. Existing implementation/documentation must
be the source for actual SCRAP business rules.

------------------------------------------------------------------------

# 8. Implementation Steps

## Step 1 --- Repository discovery

Before changing code:

1.  Identify the Runtime SCRAP Agent entry point.
2.  Identify current keyword/intent matching.
3.  Identify MCP client/service abstractions.
4.  Identify all relevant SCRAP MCP tools.
5.  Identify inventory/planning MCP capabilities.
6.  Identify LLM/Circuit integration.
7.  Identify request/response models.
8.  Identify configuration/environment conventions.
9.  Identify feature flags.
10. Identify logging/tracing.
11. Identify existing tests.
12. Identify the safest insertion point for the Skill layer.

Create:

`docs/scrap-skill-poc-discovery.md`

Include:

``` text
Current request flow
Current routing
Relevant modules
Existing MCP capabilities
Existing LLM capabilities
Existing response contract
Recommended Skill insertion point
Files that should not be modified
Risks / assumptions
```

**Stop after discovery and report findings. Do not implement yet.**

------------------------------------------------------------------------

## Step 2 --- Define the Skill

Add `SCRAP Analysis Skill` as a first-class capability.

Preferred structure, adapted to the existing repository:

``` text
skills/
  scrap_analysis/
    skill.yaml
    instructions.md
    orchestrator.py
    analyzer.py
    schemas.py
```

If an equivalent abstraction already exists, reuse it instead of
creating a duplicate.

------------------------------------------------------------------------

## Step 3 --- Skill selection

For POC, support one skill:

``` text
User Query
    |
    v
Skill Selector
    |
    +--> SCRAP query -> SCRAP Analysis Skill
    |
    +--> unsupported -> existing safe behavior
```

Reuse existing intent/LLM classification if available.

Do not create a second LLM unnecessarily.

Keep the selector extensible for future skills.

------------------------------------------------------------------------

## Step 4 --- LLM reasoning

Use the existing LLM/Circuit integration if present.

The model should help determine:

``` text
user query
   |
   +--> intent
   +--> required capability
   +--> filters
   +--> required data/tools
   +--> output format
```

Example internal representation:

``` json
{
  "skill": "scrap_analysis",
  "intent": "regional_trend",
  "filters": {
    "region": "Texas"
  },
  "required_capabilities": [
    "retrieve_scrap_data"
  ],
  "output_format": "chart"
}
```

The LLM must not execute arbitrary code. Tool calls go through
controlled existing interfaces.

------------------------------------------------------------------------

## Step 5 --- Tool orchestration

The Skill should use existing MCP/runtime capabilities.

Example:

``` text
SCRAP Skill
    |
    +--> SCRAP Search
    |
    +--> Inventory
    |
    +--> Planning
    |
    v
Combined factual context
    |
    v
LLM Analysis
```

Do not call every tool for every query.

Example:

``` text
"What are pending SCRAP items?"
    -> SCRAP search may be enough

"Why are pending SCRAP items increasing?"
    -> SCRAP + inventory/planning may be required
```

Record which tools were selected.

------------------------------------------------------------------------

## Step 6 --- Data validation

Before analysis:

-   preserve source data
-   detect tool errors
-   detect empty results
-   preserve provenance/metadata when available
-   do not silently replace missing values
-   prevent the LLM from manufacturing facts

If required data is unavailable, return a clear limitation.

------------------------------------------------------------------------

## Step 7 --- LLM analysis

Provide:

``` text
Original user question
+
Skill instructions
+
Applicable business rules
+
Retrieved tool results
```

Generate:

``` text
summary
key findings
supporting data
suggested output format
```

Do not turn analysis into unsupported business recommendations.

------------------------------------------------------------------------

## Step 8 --- Structured response

Add or extend a response model similar to:

``` json
{
  "skill": "scrap_analysis",
  "summary": "SCRAP analysis completed.",
  "insights": [
    {
      "text": "..."
    }
  ],
  "table": {
    "columns": [],
    "rows": []
  },
  "visualizations": [
    {
      "type": "bar_chart",
      "title": "SCRAP by Region",
      "x_field": "region",
      "y_field": "count",
      "data": []
    }
  ],
  "metadata": {
    "tools_used": []
  }
}
```

Reuse an existing response contract if one exists.

The response should be suitable for a future My Agent UI to render
text/table/chart components.

------------------------------------------------------------------------

## Step 9 --- Backward compatibility

Existing behavior must continue:

``` text
Existing client
    |
    v
Existing Runtime SCRAP Agent
    |
    v
Existing MCP
```

New POC path:

``` text
New skill request
    |
    v
SCRAP Skill
    |
    v
Existing runtime/MCP capability
```

If the repository already has a feature-flag mechanism, use it. If
appropriate, introduce:

`SCRAP_SKILL_POC_ENABLED=true`

Do not create a new configuration framework if one already exists.

------------------------------------------------------------------------

## Step 10 --- Observability

Reuse existing logging/tracing.

Capture where appropriate:

``` text
request
  |
  +-- selected skill
  |
  +-- selected intent
  |
  +-- tools selected
  |
  +-- tool execution
  |
  +-- analysis
  |
  +-- response type
```

Do not log secrets or unnecessary sensitive payloads.

If LangSmith/tracing already exists, extend the existing trace.

------------------------------------------------------------------------

## Step 11 --- Tests

Unit tests:

-   skill loading
-   skill selection
-   intent handling
-   tool selection
-   multi-tool orchestration
-   empty results
-   tool failure
-   LLM output validation
-   response schema
-   unsupported query

Representative cases:

``` text
1. "What SCRAP items are pending?"
   -> scrap_analysis
   -> pending_scrap
   -> SCRAP retrieval

2. "Show SCRAP trend by region"
   -> scrap_analysis
   -> regional_trend
   -> SCRAP retrieval
   -> chart response

3. "Why are SCRAP items increasing?"
   -> scrap_analysis
   -> analytical intent
   -> required tools
   -> analysis response

4. Unsupported question
   -> no incorrect skill selection

5. MCP failure
   -> controlled error
   -> no fabricated result
```

Run the existing regression suite.

------------------------------------------------------------------------

# 9. Demo Plan

Create:

`docs/scrap-skill-poc-demo.md`

Demo 1:

> What SCRAP items are pending?

Show:

``` text
Query
 -> Skill
 -> Existing SCRAP capability
 -> Table
```

Demo 2:

> Show SCRAP trend by region.

Show:

``` text
Query
 -> Skill
 -> Existing data
 -> Analysis
 -> Chart + insight
```

Demo 3:

> Why is SCRAP increasing?

Show:

``` text
Query
 -> Skill
 -> SCRAP data
 -> Inventory/planning data if available
 -> LLM analysis
 -> Business insight
```

Demo 3 is especially useful for proving that a Skill is more than an MCP
wrapper.

------------------------------------------------------------------------

# 10. Future Architecture

After the POC:

``` text
                         My Agent
                            |
                            v
                       Super Agent
                            |
             +--------------+--------------+
             |              |              |
             v              v              v
       Planning Agent   Logistics Agent   CDM Agent
             |
      +------+------+
      |             |
      v             v
 SCRAP Skill   Other Skills
      |
      v
Existing Runtime / MCP
```

Future work may include multiple skills, formal Planning Agent, Super
Agent, A2A, My Agent integration, dynamic UI rendering, skill
discovery/catalog, skill packaging, and hierarchical agents.

These are **not POC-1 requirements**.

------------------------------------------------------------------------

# 11. Cursor Master Prompt

Copy the following into Cursor at the root of the existing Runtime SCRAP
Agent repository.

``` text
You are working in an existing enterprise SSC Planning / Runtime SCRAP Agent repository.

We need to build a small POC for a skill-based SCRAP capability.

CRITICAL:
DO NOT rebuild the existing Runtime SCRAP Agent.
DO NOT replace existing MCP tools.
DO NOT create a parallel SCRAP agent.

First inspect the repository and understand the actual implementation.

The existing Runtime SCRAP Agent, MCP integrations, routing, data retrieval, configuration, tracing, and tests are reusable assets.

The purpose of this POC is to introduce a SCRAP Analysis Skill above the existing capability.

TARGET:

User
  |
  v
SCRAP Skill
  |
  +-- Skill instructions
  +-- Business rules
  +-- LLM reasoning
  +-- Tool orchestration
  |
  v
Existing Runtime SCRAP Agent / MCP capabilities
  |
  v
Enterprise data
  |
  v
LLM analysis
  |
  v
Structured Skill Response
  |
  +-- Text
  +-- Table
  +-- Chart
  +-- Insights

CURRENT IMPLEMENTATION ASSUMPTION:

User Query
  |
Keyword / intent matching
  |
Corresponding MCP
  |
Data
  |
Response

Do not assume this is exactly how the repository works. Verify it.

PHASE 1 — DISCOVERY ONLY

Before editing code:

1. Identify Runtime SCRAP Agent entry point.
2. Identify current keyword/intent routing.
3. Identify MCP client/service abstractions.
4. Identify relevant SCRAP MCP tools.
5. Identify inventory/planning MCP capabilities.
6. Identify LLM/Circuit integration.
7. Identify request/response models.
8. Identify configuration and feature flags.
9. Identify logging/tracing.
10. Identify tests.
11. Identify the safest insertion point for a Skill.

Create:
docs/scrap-skill-poc-discovery.md

Document:
- current flow
- relevant modules
- MCP capabilities
- LLM capabilities
- response contract
- recommended Skill insertion point
- files that should not be modified
- risks and assumptions

STOP after Phase 1 and report findings. Do not implement yet.

PHASE 2 — SKILL CONTRACT

After discovery, create or reuse the repository's existing capability abstraction.

Define:
SCRAP Analysis Skill

The Skill must describe:
- name
- description
- when to use
- inputs
- capabilities
- available tools
- execution instructions
- output formats
- LLM reasoning requirement

If an existing skill abstraction exists, reuse it.

PHASE 3 — SKILL SELECTION

For the POC, support one skill:

SCRAP-related query -> SCRAP Analysis Skill

Unsupported query -> existing safe behavior.

Reuse existing intent/LLM classification if available.

Do not create a second LLM unnecessarily.

PHASE 4 — LLM REASONING

Use the existing LLM/Circuit integration.

The LLM should determine:
- intent
- required capability
- filters
- required tools/data
- output format

The LLM must not execute arbitrary code.

Tool calls must use existing controlled interfaces.

PHASE 5 — TOOL ORCHESTRATION

The Skill must reuse existing MCP/runtime capabilities.

Do not invent MCP tools.

For simple pending-SCRAP queries, use only required capability.

For analytical questions, use multiple existing capabilities when they are actually available.

Record tools selected/executed.

PHASE 6 — ANALYSIS

Pass to the LLM:
- original user query
- skill instructions
- applicable existing business rules
- retrieved tool results

Generate:
- summary
- key findings
- supporting data
- output format

Never fabricate business data.

PHASE 7 — STRUCTURED RESPONSE

Create or reuse a response model that can represent:
- summary
- insights
- table
- visualization
- metadata/tools used

It must be suitable for a future My Agent UI.

Do not create duplicate response contracts.

PHASE 8 — BACKWARD COMPATIBILITY

Existing Runtime SCRAP Agent behavior must continue to work.

If the repository already has feature flags, use them.

If appropriate:
SCRAP_SKILL_POC_ENABLED=true

Do not introduce a new configuration framework.

PHASE 9 — TESTING

Add tests for:
- skill loading
- skill selection
- intent
- tool selection
- multi-tool orchestration
- empty results
- tool failure
- LLM output validation
- response schema
- unsupported queries

Run all existing regression tests.

PHASE 10 — DOCUMENTATION

Create:
docs/scrap-skill-poc-demo.md

Document these demos:

1. What SCRAP items are pending?
2. Show SCRAP trend by region.
3. Why are SCRAP items increasing?

For each show:
Query -> Skill -> Tools -> Data -> LLM analysis -> Response

DEFINITION OF DONE:

- Existing Runtime SCRAP Agent still works.
- Existing MCP tools still work.
- SCRAP Analysis Skill exists as a first-class capability.
- At least 2–3 representative queries work.
- Existing runtime/MCP capabilities are reused.
- At least one query demonstrates LLM-based analysis.
- At least one response supports table/chart/insight representation.
- MCP/tool failures are handled safely.
- No business data is fabricated.
- Tests pass.
- Documentation is complete.

ARCHITECTURAL RULES:

1. Reuse before rebuilding.
2. Skill is a business capability, not an MCP wrapper.
3. MCP is a low-level operation.
4. Skill owns orchestration.
5. LLM reasons over user intent and retrieved context.
6. Tools remain controlled.
7. Preserve backward compatibility.
8. Keep POC small.
9. Do not implement Super Agent yet.
10. Do not implement My Agent integration yet.
11. Do not implement A2A yet.
12. Do not perform broad refactoring.
13. Do not invent business rules or data.
14. Adapt to the actual repository instead of blindly following a proposed folder structure.

EXECUTION DISCIPLINE:

Work in small phases.

After every phase:
- run relevant tests
- list changed files
- explain why each file changed
- list assumptions
- list anything blocked by missing repository capabilities

START WITH PHASE 1 DISCOVERY ONLY.
```

------------------------------------------------------------------------

# 12. Success Criteria

The POC should let you demonstrate this transformation:

``` text
TODAY

User
  ↓
Keyword / Intent
  ↓
MCP
  ↓
Result


POC

User
  ↓
SCRAP Skill
  ↓
LLM reasoning
  ↓
Existing MCP / Runtime
  ↓
Data
  ↓
LLM analysis
  ↓
Insight + Table/Chart


FUTURE

My Agent
  ↓
Super Agent
  ↓
Planning Agent
  ↓
SCRAP Skill
  ↓
Existing Runtime / MCP
  ↓
Data
  ↓
LLM
  ↓
Dynamic UI
```

The architectural goal is therefore **not to replace your existing SCRAP
Agent**. It is to demonstrate how that existing capability can be
wrapped/evolved into a reusable **SCRAP Skill** and later become part of
the Planning Agent in the larger My Agent/Super Agent ecosystem.
