---
name: add-npp-to-facility-analysis
description: Analyze the HCP ADD NPP to Facility feature using the approved business requirements. Use this skill when reviewing repository code, architecture, UI flows, APIs, validation, routing, PAF tasks, CACTUS integration, or test coverage related to the ADD NPP to Facility PAF.
---

# ADD NPP to Facility – Repository Analysis Skill

## Purpose

Use this skill to analyze an existing codebase against the business requirements for the **HCP Non-Privileged Practitioner (NPP) PAF – ADD NPP to Facility** feature.

The primary source of truth is:

**HCP NPP PAF Type Requirements V5 – 06/2026**

Do not invent requirements that are not supported by the source document. If the repository behavior and requirements differ, explicitly identify the difference as a gap, deviation, or uncertainty.

---

# 1. Business Context

The feature introduces a new PAF type:

**ADD NPP to Facility**

The business goal is to allow an MSP user to submit a Non-Privileged Practitioner request when the facility is responsible for performing the required verification activities instead of the CPC performing the normal NPP verification process.

The MSP submits practitioner information and Primary Source Verification (PSV) evidence through HCP. Normal submissions are intended to be auto-accepted and result in CACTUS updates. Defined exception scenarios are routed to CPC for manual processing.

Do not describe this as a normal privileged-practitioner credentialing workflow.

---

# 2. Important Business Terms

Always explain these terms before using them heavily in analysis.

### HCP
The application/workflow platform used by MSP users to search practitioners, initiate PAFs, complete task cards, upload documents, validate information, and submit requests.

### MSP
Medical Staff Professional. The user role responsible for submitting ADD NPP to Facility PAF requests.

### CPC
Centralized credentialing/processing team. CPC handles defined exception/manual-processing scenarios.

### CACTUS
The credentialing database/system where practitioner, facility assignment, address, specialty, license, NPI, and sanctions-related records are created or updated.

### PAF
Provider Action Form. The workflow/request type used to perform provider-related actions.

### NPP
Non-Privileged Practitioner.

### PSV
Primary Source Verification evidence/documentation.

### CVI
Credential Verification Issue used for CPC processing when an ADD NPP request meets defined exception criteria.

### Net New Practitioner
A practitioner who does not currently reside in CACTUS.

### Existing Active Practitioner
A practitioner in CACTUS with at least one active facility where the practitioner holds privileges.

### Existing Inactive Practitioner
A practitioner in CACTUS with no active facilities.

---

# 3. Core Business Flow

Use this mental model when analyzing the repository:

Normal flow:

MSP
→ HCP
→ Practitioner Search
→ New/Existing Practitioner determination
→ ADD NPP to Facility PAF
→ Demographic / Address / Specialty / ADD NPP tasks
→ License + PSV validation
→ Submit
→ Auto-Accept
→ CACTUS updates
→ Completed Queue

Exception flow:

MSP
→ HCP
→ ADD NPP workflow
→ Exception condition
→ PAF auto-acceptance where specified
→ CVI creation
→ CPC processing
→ CVI completion

The business requirements state that ADD NPP is not part of PAF merging, packet determination, credentialing packet creation, or normal CVI creation except where explicitly required by CPC-processing rules.

---

# 4. Analysis Order

When analyzing a repository, follow this order.

## Step 1 – Locate the PAF framework

Find:

- PAF type definitions
- PAF action definitions
- PAF task/card configuration
- Practitioner action selection
- PAF routing
- PAF submission
- PAF completion
- PAF PDF generation
- PAF history
- Auto-acceptance logic

Determine how an existing PAF type such as **Add Practitioner to Facility** is implemented.

Prefer reusing established patterns instead of proposing a new architecture.

---

## Step 2 – Find MSP role authorization

Verify that:

- ADD NPP to Facility is available only to MSP users.
- Users can select only one PAF action at a time.
- The action is available only for a net-new practitioner or an existing practitioner who is inactive with the entity.

Identify where role checks and practitioner eligibility checks are implemented.

---

# 5. Practitioner Search Requirements

## Normal Search

NPI is the primary and required search criterion.

Requirements:

- Valid 10-digit NPI.
- NPI validation before normal search.
- Duplicate checking against CACTUS records.

## No-NPI Exception Search

The UI must provide:

**“No NPI available? Search without NPI”**

Required fields:

- Reason
- First Name
- Last Name
- State License Number
- State of License

Validation:

- Reason selected.
- First name contains at least 1 alpha character.
- Last name contains at least 2 alpha characters.
- State selected.
- State license number has at least 2 characters.

License lookup should support normalized/“like” matching behavior as specified by the requirements.

When analyzing code, identify:

- UI component
- validation schema
- API
- search service
- lookup source
- audit logging

---

# 6. Practitioner Type Branching

Always distinguish:

### Net New

Practitioner does not exist in CACTUS.

Required task cards:

- Demographic
- Manage Addresses
- Specialties
- ADD NPP to Facility

### Existing

For existing practitioner:

- Demographic: Optional
- Manage Addresses: Optional
- Specialties: Optional unless no primary specialty exists
- ADD NPP to Facility: Required

Delegate and Facility Specific Questions cards must be suppressed for ADD NPP.

---

# 7. ADD NPP Task Card

The task card must:

- Be titled “ADD NPP to Facility”
- Be visible in PAF Tasks
- Be Required
- Have Work action
- Open the ADD NPP workflow
- Prevent Review & Submit while required work remains incomplete

Expected description:

“Upload and complete required Non-Privileged Practitioner (NPP) verification information and supporting PSV documentation.”

When analyzing repository code, trace:

Task registration
→ task visibility
→ requiredness
→ task completion state
→ navigation
→ submit gating

---

# 8. Demographic Rules

For ADD NPP:

Optional:

- Email
- Cell
- DOB/SSN
- Gender

Required:

- First Name
- Last Name
- Degree
- Provider Category
- Individual NPI

For existing practitioners:

- Existing email is read-only.
- Blank email is editable and optional.
- Email format validation applies only when a value is entered.

If CACTUS data exists, demographic values should prepopulate from the corresponding provider fields.

Do not recommend allowing edits to existing values unless the requirement permits it.

---

# 9. Address Rules

Do not show:

- Home address
- Credentialing address
- Alternate address

Primary address:

### Active primary address + active facility affiliation
Show the address and disable Add Address.

Expected business message:

“The practitioner currently holds privileges at one or more HCA facilities. The primary address cannot be changed.”

### No primary address
Allow Add Address.

### Active primary address but no other active affiliations
Allow Add Address.

Required address fields include:

- Address
- City
- State
- Zip Code
- Country
- Phone
- Fax

Optional:

- Contact
- Address Line 2
- Phone Extension
- Fax Extension

---

# 10. Specialty Rules

### Net New
Specialty is required.

### Existing
Specialty is optional unless:

- No primary specialty exists, or
- Existing primary specialty record is inactive.

If an active primary specialty exists:

- Do not allow editing.
- Show the required explanatory hover behavior.

Secondary and alternate specialties must not be displayed.

### Professional Practice Interest (PPI)

If the user cannot find an appropriate specialty, provide a comment field for professional practice interest/focus.

At least one must be supplied:

- Primary specialty
OR
- Professional practice interest/focus

Expected validation:

“A primary specialty or professional practice interest or focus is required. Please provide either a primary specialty or a professional practice interest or focus.”

---

# 11. ADD NPP Business Workflow

The new facility card must communicate that the MSP is adding an NPP and uploading PSV evidence.

Practitioner Type:

- NPP only
- Required
- User cannot select another practitioner type

Then ask:

**“Is the practitioner an active duty military member or practicing at the VA?”**

## VA = YES

Ask:

**“Is the Practitioner a PA?”**

### VA = YES + PA = YES

Requirements indicate:

- Hide State License section.
- Require Sanctions PSV.
- Require NPI PSV.
- Route to CPC.
- PAF handling must follow the specified CPC-processing behavior.

### VA = YES + PA = NO

Require:

- State License
- License PSV
- Sanctions PSV
- NPI PSV

### VA = NO

Skip PA question.

Automatically show State License.

Require:

- State License
- License PSV
- Sanctions PSV
- NPI PSV

---

# 12. Existing Practitioner License Workflow

For an existing practitioner:

- Recall active/existing licenses.
- Display State.
- Display License Number.
- Display Expiration Date.
- Allow selection.

Provide:

**“None of these apply. Add New State License Instead.”**

If the user adds a new license:

- Compare state + license number against existing displayed licenses.
- Do not create duplicates.
- Show the defined duplicate message.
- Return the user to existing license selection where specified.

Existing practitioner recalled records can bypass standard validation rules as specified by the requirements.

If selected license is inactive or expired/matured:

- Route to CPC.
- Display the defined informational message.

---

# 13. State License Validation

License fields:

- State
- Effective Date
- License Number
- Status
- Expiration Date
- Field of Licensure

Net-new license validation must enforce the applicable rules.

For facilities/entities in:

- CA
- LA
- NV
- KY
- TX

the practitioner license state must match the entity/facility state.

If not, block submission.

Expected error:

“A state license in the state of ‘XX’ is required for PAF submission.”

When analyzing code, find where entity state and license state are obtained and where this validation is enforced.

---

# 14. License Status and Routing

The defined STATUS_RTK values include:

- Active
- Temporary Permit
- Active Military
- Active-Compact

Routing requirements include:

- PA = YES → CPC
- License status not Active → CPC
- Expired/matured license → CPC
- Net-new licenses → normal validation
- Existing recalled licenses → existing-practitioner rules

Do not assume every non-Active status has identical handling without checking the exact business rule and implementation.

---

# 15. PSV Upload Requirements

License PSV is required where the workflow requires a license.

Accepted upload types:

- DOC
- DOCX
- PDF
- JPG
- TIFF

The system must:

- Reject unsupported types.
- Convert/store as PDF/HTML as required.
- Attach the image to the appropriate CACTUS record.
- Populate required image metadata.
- Audit image creation.

Also trace NPI PSV and Sanctions PSV handling separately.

---

# 16. Auto-Acceptance

Normal ADD NPP submissions should be auto-accepted.

After acceptance:

- CACTUS updates are performed.
- PAF goes to Completed queue.
- PAF history uses HCP System User as specified.
- Audit logs use the MSP submitter identity and HCA Corporate as specified.
- PAF PDF is attached to Provider Record unless a CVI is created.

If CVI criteria apply:

- PAF PDF goes to the CVI instead of Provider Record.

---

# 17. CPC / CVI Processing

When the workflow explicitly requires CPC processing:

1. PAF is auto-accepted as specified.
2. User receives the manual-processing message.
3. CVI is created.
4. CVI type = Facility Undefined Practitioner.
5. Initial status = UDP Facility Request.
6. CVI notes identify the triggering criteria.
7. Due date is PAF auto-acceptance date + 3 business days.
8. CVI remains open until CPC completes processing.
9. Completion status = UDP Facility Request Complete.
10. PAF PDF is attached to CVI.

Trace both PAF and CVI code paths.

---

# 18. CACTUS Update Analysis

For every CACTUS update, identify:

- API/service
- request model
- mapping layer
- database/repository
- error handling
- transaction behavior
- duplicate checking
- audit logging

Expected areas include:

### Provider
For net-new practitioners:

- First Name
- Last Name
- Suffix
- Degree
- Provider Category
- NPI

Existing provider record: no changes unless explicitly required.

### Entity Assignment

For net-new practitioner:

- Create entity assignment using existing logic.
- Status should become NPP – Active when applicable.
- Follow specified security rules.

### Address

Create primary address if no existing record.

### Specialty

Net-new/inactive:

- Add primary specialty record when selected.
- Set specialty type to Primary.
- Set Active.

PPI:

- Specialty = APP-Other.
- Status = Not Certified.

### State License

Check existing license using:

- License type
- License number
- License state

Do not update existing recalled license in normal auto-acceptance cases.

Create new license when appropriate.

### NPI

Attach NPI image to Provider Record using the required naming/type/notes rules.

### Sanctions

Find/create Sanction Check record as specified and attach sanctions image.

---

# 19. Audit and Data Integrity

Always check:

- Duplicate practitioner detection.
- Duplicate license detection.
- Audit logging.
- Image audit logging.
- Auto-accept history.
- Correct submitting MSP identity.
- Correct entity.

The requirements explicitly state that audit logs must be generated for system updates.

---

# 20. PAF PDF

The generated PAF PDF must show:

**ADD NPP to Facility**

PDF history must record the required HCP System User/MSP submitter identity.

Trace:

- PDF template
- PAF type mapping
- PDF generation service
- attachment destination
- history generation

---

# 21. Reports

The business requires a monthly report containing:

- Total ADD NPP requests received.
- Total ADD NPP requests routed to CPC.
- Percentage routed to CPC, where applicable.

The report is distributed to:

- Reporting Team
- CPC Distribution List

Also, CVIs with:

**CVI Type = Facility Undefined Practitioner**

must be excluded from MOR statistical calculations.

---

# 22. Repository Analysis Method

When asked to analyze the repository, do NOT immediately start changing code.

First produce:

## A. Business requirement

What the requirement says.

## B. Existing implementation

Where the repository currently implements the behavior.

Include:

- File path
- Class/component/service
- Method/function
- Relevant behavior

## C. Gap

Choose one:

- Implemented
- Partially implemented
- Not implemented
- Different behavior
- Cannot determine

## D. Impact

Explain which part of the ADD NPP workflow is affected.

## E. Recommended implementation

Only after identifying the existing pattern.

Prefer extending an existing PAF implementation instead of duplicating code.

---

# 23. Search Strategy

Start broad, then narrow.

Useful search terms:

- ADD NPP
- Add NPP
- NPP
- Non Privileged
- Non-Privileged
- PAF
- PAFType
- PAF Type
- Practitioner Action
- Add Practitioner to Facility
- MSP
- Facility Undefined Practitioner
- UDP Facility Request
- CVI
- Auto Accept
- Auto-Accept
- CACTUS
- Provider
- Provider Specialty
- Provider License
- Sanction
- NPI
- PSV
- State License
- Entity Assignment
- STATUS_RTK
- NPDSTATEx
- NPDBSTATEx

Also search for the existing **Add Practitioner to Facility** implementation because the requirements explicitly state that ADD NPP routing should follow the same path/logic where applicable.

---

# 24. Do Not Make These Assumptions

Do not assume:

- HCP is the CACTUS database.
- CPC manually approves every ADD NPP request.
- Every existing practitioner can be edited.
- Every license must be newly entered.
- Every exception creates a CVI unless the requirements specify CPC processing.
- ADD NPP creates credentialing packets.
- ADD NPP participates in PAF merging.
- Facility Specific Questions are required.
- Delegate card is required.

Use the requirements as the source of truth.

---

# 25. Expected Analysis Output

When the user asks:

“Analyze ADD NPP implementation”

return a structured analysis using this order:

1. Business flow
2. Relevant repository modules
3. Existing Add Practitioner pattern
4. Practitioner search
5. MSP eligibility
6. PAF action
7. Task cards
8. Net-new vs existing behavior
9. Demographics
10. Addresses
11. Specialties
12. VA/PA workflow
13. State license
14. PSV uploads
15. Validation
16. Routing
17. Auto-acceptance
18. CVI/CPC processing
19. CACTUS updates
20. PDF/history
21. Audit logging
22. Reporting
23. Gaps
24. Risks
25. Recommended implementation sequence
26. Test scenarios

For every important finding, provide the exact repository file path and symbol when available.

---

# 26. Requirement Traceability

When possible, create a table:

| Requirement | Repository Location | Status | Evidence | Gap |
|---|---|---|---|---|
| MSP-only action | path/file | Implemented/Gap | code behavior | details |
| NPI validation | path/file | ... | ... | ... |
| ADD NPP task | path/file | ... | ... | ... |
| License validation | path/file | ... | ... | ... |
| Auto acceptance | path/file | ... | ... | ... |
| CACTUS update | path/file | ... | ... | ... |
| CVI routing | path/file | ... | ... | ... |

Never claim “implemented” without repository evidence.

---

# 27. Important Business Distinctions

Always keep these distinctions clear:

### ADD NPP vs Add Practitioner to Facility
ADD NPP is a new PAF type. It may reuse existing framework/routing patterns, but its business rules are different.

### Net New vs Existing Active vs Existing Inactive
These determine requiredness, editability, and CACTUS update behavior.

### Normal vs Exception
Normal → auto-accept and CACTUS update.

Exception → CPC processing/CVI behavior according to the requirement.

### Existing License vs New License
Existing recalled licenses follow different validation/update rules from newly entered licenses.

### HCP vs CACTUS
HCP is the workflow/application layer used by MSP. CACTUS is the credentialing data system being updated.

---

# 28. Safety Rule for Analysis

If repository code contradicts the requirements:

Do not silently modify your interpretation.

Report:

**Requirement says:** X

**Code currently does:** Y

**Impact:** Z

**Recommendation:** determine whether Y is intentional legacy behavior or requires change.

If the requirements are ambiguous, explicitly say:

**“The source requirement does not provide enough information to determine this.”**

Do not invent business rules.

---

# 29. Primary Source

The primary source for this skill is:

**HCP NPP PAF Type Requirements V5 – Version 5, 06/2026 – Credentialing Product Development.**

When the source requirement is available in the analysis environment, use it as the authoritative business reference for ADD NPP behavior.
