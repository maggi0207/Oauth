We already have an existing high-level architecture document:

HCA Credentialing 2.0 — Architecture & Project Guide.md

DO NOT recreate that architecture documentation.

That document already explains:
- overall HCP architecture
- solution/project structure
- Client/Server
- PAF API
- CACTUS API
- Packet API
- Azure Functions
- Service Bus
- databases
- authentication
- shared libraries
- high-level data flows

Your task is different.

========================================================
TASK
========================================================

Perform a READ-ONLY, deep code-level investigation of the
existing HCP repository specifically for the future
"ADD NPP to Facility" implementation.

Create ONE new Markdown file:

HCP_ADD_NPP_CODE_LEVEL_ARCHITECTURE.md

DO NOT modify application source code.

DO NOT implement ADD NPP.

DO NOT refactor.

DO NOT change tests.

DO NOT create new architecture.

The purpose of this document is to tell another senior engineer:

"Here are the EXACT existing HCP files, classes, components,
methods, services, APIs and tests that ADD NPP should reuse
or extend."

========================================================
IMPORTANT
========================================================

Use the existing high-level architecture document as the
starting map.

Do NOT repeat its project-level explanations unless needed
to explain a code-level dependency.

Go deeper into the actual repository.

Every repository claim must be verified from actual source code.

Never invent:
- file paths
- classes
- components
- methods
- APIs
- services
- database objects
- configuration
- relationships

If something cannot be confirmed, put it under:

"Repository Evidence Gap"

========================================================
1. BEGIN PAF — CODE LEVEL
========================================================

Trace the existing Begin PAF flow from the MSP Dashboard.

Document the actual call chain.

Example structure:

MSP Dashboard
→ Razor component
→ button/event handler
→ state/service
→ HTTP client
→ API endpoint
→ backend controller
→ application/service layer
→ database/downstream

For every step document:

| Layer | Exact File | Class/Component | Method | Purpose |
|------|------------|-----------------|--------|---------|

Also identify:
- route
- authorization
- tests
- navigation
- state management

========================================================
2. ENFORCE NPI SEARCH — CODE LEVEL
========================================================

Find and trace the existing Enforce NPI Search implementation.

Document:

- exact Razor/component files
- models
- validators
- handlers
- services
- HTTP clients
- API endpoints
- backend services
- repository/data access
- duplicate practitioner logic
- error handling
- tests

Create the exact call chain.

========================================================
3. PRACTITIONER SEARCH — CODE LEVEL
========================================================

Trace actual implementation.

Identify:

- UI
- request model
- response model
- validator
- HTTP client
- API controller
- service
- repository
- CACTUS calls
- duplicate detection
- result handling
- tests

Document exact files and methods.

========================================================
4. ADD NEW PRACTITIONER — CODE LEVEL
========================================================

Trace the complete existing Add New Practitioner workflow.

Focus on:

- entry point
- component
- state/model
- PAF creation
- practitioner creation
- demographics
- address
- specialty
- facility
- tasks
- validation
- CACTUS
- authorization
- tests

For every major step provide exact code locations.

========================================================
5. ADD PRACTITIONER TO FACILITY — MOST IMPORTANT
========================================================

Perform the deepest investigation here.

This is the primary existing implementation pattern for
ADD NPP.

Trace the complete workflow:

Search
→ Practitioner selection
→ Add Practitioner to Facility
→ PAF creation
→ PAF type
→ PAF action
→ PAF tasks
→ Practitioner information
→ Demographics
→ Address
→ Specialty
→ Facility
→ License
→ PSV if applicable
→ Submit
→ Processing
→ CACTUS
→ History
→ PDF
→ Audit

For every stage identify:

- exact file
- component/class
- method
- model
- API
- service
- database access
- validation
- tests

Clearly mark:

DIRECTLY REUSABLE

EXTENDABLE

NOT REUSABLE

UNKNOWN

Do not make an ADD NPP design yet.

========================================================
6. PAF CREATION ARCHITECTURE
========================================================

Trace the actual code responsible for:

- creating PAF
- PAF type
- PAF actions
- PAF tasks
- required/optional tasks
- task completion
- Review & Submit
- submission
- status changes
- history
- PDF
- attachments

Identify the extension points used by existing PAF types.

========================================================
7. PAF TASK IMPLEMENTATION
========================================================

Find exactly how tasks are represented and rendered.

Document:

- task model
- task type
- task registration
- task component
- task state
- required flag
- completion
- persistence
- validation
- Review & Submit blocking
- tests

========================================================
8. PRACTITIONER INFORMATION CARDS
========================================================

Trace actual code for:

- Add New Practitioner card
- Existing Practitioner card
- Demographics
- Address
- Specialty
- Facility-related cards

Identify shared components and reusable services.

========================================================
9. LICENSE / PSV ARCHITECTURE
========================================================

Trace existing implementation for:

- practitioner licenses
- license selection
- license creation
- license validation
- license PSV
- document upload
- attachment processing
- sanctions PSV
- NPI PSV

Identify:

UI
→ API
→ service
→ model
→ database
→ document storage
→ audit

Do not assume ADD NPP uses the same behavior.

Only document existing implementation.

========================================================
10. AUTO ACCEPTANCE
========================================================

Find the actual code implementing existing auto-acceptance.

Document:

- trigger
- service
- validation
- decision logic
- status transition
- CACTUS update
- queue/event
- history
- audit
- tests

========================================================
11. CPC / MANUAL PROCESSING
========================================================

Trace existing CPC processing.

Document:

- routing
- queue
- dashboard
- review
- acceptance
- return
- status
- downstream processing
- tests

========================================================
12. CVI
========================================================

Trace existing CVI creation.

Document:

- trigger
- service
- model
- API
- database
- status
- notes
- due date
- attachment
- completion
- tests

Only document existing implementation.

========================================================
13. CACTUS
========================================================

Trace the exact HCP code path used to update CACTUS for:

- practitioner
- entity/facility
- address
- specialty
- PPI
- license
- NPI
- sanctions
- images/documents

Document exact:

HCP code
→ API
→ service
→ CACTUS API
→ database/repository

Identify reusable services.

========================================================
14. PDF / HISTORY / AUDIT
========================================================

Trace actual code for:

- PAF PDF generation
- PAF history
- attachments
- Provider Record
- audit
- image audit

Identify exact files and methods.

========================================================
15. AUTHORIZATION
========================================================

Trace actual authorization code relevant to:

- MSP
- CPC
- PAF
- Begin PAF
- dashboard
- APIs

Document:

- roles
- policies
- attributes
- route guards
- backend authorization
- tests

========================================================
16. TEST MAP
========================================================

Find existing tests for:

- Begin PAF
- NPI Search
- Add New Practitioner
- Add Practitioner to Facility
- PAF
- PAF Tasks
- License
- PSV
- CACTUS
- Auto Acceptance
- CPC
- CVI
- PDF
- Authorization

Use exact test file paths.

For each test area explain what behavior is already protected.

========================================================
17. CODE-LEVEL DEPENDENCY MAP
========================================================

Create a table:

| Feature | UI | Client | API | Service | Repository | DB/External | Tests |
|--------|----|--------|-----|---------|------------|-------------|-------|

Populate only with verified repository information.

========================================================
18. ADD NPP REUSE MAP
========================================================

DO NOT implement anything.

Instead, identify existing code that is likely to be reused.

Create:

| ADD NPP Requirement Area | Existing Code | Reuse Type | Evidence |
|---|---|---|---|
| Search | | Direct reuse / Extend | |
| Begin NPP PAF | | Extend | |
| PAF Action | | Extend | |
| PAF Tasks | | Extend | |
| Add New Practitioner | | Reuse / Extend | |
| Existing Practitioner | | Reuse / Extend | |
| Demographics | | | |
| Address | | | |
| Specialty | | | |
| License | | | |
| PSV | | | |
| Auto Accept | | | |
| CPC | | | |
| CVI | | | |
| CACTUS | | | |
| PDF | | | |
| Audit | | | |

IMPORTANT:

This is NOT a design decision.

It is a repository evidence map.

========================================================
19. EXACT FILE INDEX
========================================================

Create a consolidated index:

| # | Area | File Path | Type | Class/Component | Important Methods |
|---|------|-----------|------|-----------------|------------------|

Only verified files.

========================================================
20. REPOSITORY EVIDENCE GAPS
========================================================

List everything that could not be confirmed.

For each:

Unknown:
Why it matters:
Suggested next file/search:
Impact on ADD NPP analysis:

========================================================
21. EXECUTIVE SUMMARY
========================================================

At the end provide:

### Existing Architecture We Can Reuse

### Most Important Add Practitioner to Facility Extension Points

### Most Important PAF Extension Points

### Most Important CACTUS Integration Points

### Most Important Risks

### Repository Evidence Gaps

### Recommended Next Repository Investigation

Do NOT generate the ADD NPP implementation plan.

Do NOT generate Copilot implementation prompts.

This document is ONLY the code-level architecture discovery artifact.

Before completing the document:

1. Verify referenced files exist.
2. Verify class names.
3. Verify method names.
4. Avoid duplicate high-level architecture already covered by
   HCA Credentialing 2.0 — Architecture & Project Guide.md.
5. Clearly distinguish verified code from inference.
6. Do not modify application code.
