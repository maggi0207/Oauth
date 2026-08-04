You are a Senior React UI Architect specializing in enterprise trading platforms and complex desktop applications.

# Objective

Investigate and permanently fix the desktop layout of the **AsianTradeDialog**.

This is an enterprise trading application and the dialog must maintain a consistent, professional appearance across all supported desktop resolutions.

Do NOT redesign the UI.

Do NOT modify business logic.

Only improve the layout architecture.

---

# Component Scope

Start your investigation from the popup entry component and trace the complete rendering hierarchy.

ActionsMenu
└── AsianTradeDialog
    ├── AsianTradeDetailsTab.tsx
    ├── AveragingScheduleTab.tsx
    ├── AuditTrailTab.tsx
    ├── detailsShared.tsx
    └── FormFields.tsx

Also inspect:

- Shared layout components
- CSS Modules / SCSS
- Styled Components
- Material UI Dialog
- Material UI Grid / Stack / Box
- Theme spacing
- Responsive breakpoints
- Shared utility classes

---

# Current Problem

The popup looks correct on a 1920×1080 monitor.

However, on larger desktop resolutions (2400, 2560, 2880, 3440 ultrawide, and 3840 4K), the layout becomes visually inconsistent.

Observed issues include:

- Dialog stretches too much.
- Columns become disproportionately wide.
- Inputs become unnecessarily large.
- Trade Summary panel moves too far away.
- Excess whitespace appears.
- Vertical spacing changes.
- Layout proportions differ from the original design.

The goal is for the dialog to look nearly identical on every supported desktop resolution.

---

# Investigation First (Mandatory)

Before modifying any code:

1. Trace the complete component hierarchy.
2. Explain how the dialog width is calculated.
3. Explain how the dialog height is calculated.
4. Identify every parent container affecting layout.
5. Identify the exact root cause.
6. Explain why the layout changes on higher-resolution monitors.

Do not assume the issue exists in only one component.

Investigate the complete rendering hierarchy.

---

# Inspect Everything

Carefully inspect:

- Dialog sizing
- DialogContent
- DialogActions
- Parent wrappers
- Nested wrappers
- CSS Grid
- Flexbox
- Material UI Grid
- Material UI Stack
- Material UI Box
- width
- max-width
- min-width
- height
- max-height
- min-height
- flex-grow
- flex-shrink
- flex-basis
- overflow
- overflow-y
- overflow-x
- grid-template-columns
- grid-template-rows
- percentage widths
- viewport units
- shared layout utilities
- inherited CSS
- reusable wrappers
- theme breakpoints

---

# Layout Requirements

The popup should maintain the same visual appearance on:

- 1920×1080
- 2560×1440
- 2880×1800
- 3440×1440
- 3840×2160

Specifically:

- Font sizes must remain identical.
- Label spacing must remain identical.
- Input heights must remain identical.
- Button sizes must remain identical.
- Section spacing must remain identical.
- Three-column proportions must remain consistent.
- Trade Summary width must remain visually stable.
- Dialog should not continuously stretch as the viewport becomes larger.
- Dialog should remain centered.
- Header should remain fixed.
- Footer should remain fixed.

---

# Empty Space Requirements

Do not introduce unnecessary blank areas.

Avoid:

- Empty space below forms.
- Empty space below tables.
- Empty space above the footer.
- Empty space beside the Trade Summary panel.
- Large unused white regions anywhere in the dialog.

The dialog should feel compact and balanced.

Every section should naturally occupy its required space.

---

# Scrollbar Requirements

Do not introduce unwanted scrollbars.

Specifically:

- No horizontal scrollbar.
- No nested vertical scrollbars.
- No page scrollbar caused by the popup.
- Only the dialog content area should scroll if required.
- Header must remain fixed.
- Tabs must remain fixed.
- Footer must remain fixed.

If content exceeds available height:

- Only the content area should scroll.
- Tables should manage their own scrolling correctly.

---

# Averaging Schedule Requirements

The Generated Averaging Schedule must always render correctly.

Requirements:

- Table must occupy the available vertical space.
- Table rows must always be visible.
- Table must not collapse.
- Toolbar must remain visible.
- Footer must remain visible.
- Only the table body should scroll when necessary.
- Existing functionality must remain unchanged.

The same applies to the Audit Trail table.

---

# Preserve Existing Functionality

Do NOT modify:

- Business logic
- API calls
- Validation
- State management
- Event handlers
- Existing workflows
- Data loading
- Component behavior

Only improve the layout architecture.

---

# Do NOT Use

Do NOT use:

- transform: scale()
- zoom
- arbitrary media queries
- hardcoded pixel positioning
- random max-width values
- CSS hacks
- quick fixes

Every layout change must have a clear architectural reason.

---

# Preferred Solution

Use modern React and CSS best practices.

Prefer:

- CSS Grid where appropriate
- Proper Flexbox layouts
- min-width
- max-width
- min-height
- max-height
- flex layouts
- grid layouts
- consistent spacing tokens

The solution should be maintainable and scalable.

---

# Deliverables

## Phase 1 – Investigation

Explain:

- Current component hierarchy.
- Width calculation.
- Height calculation.
- Layout flow.
- Root cause.
- Every problematic container.
- Every problematic CSS rule.

## Phase 2 – Solution Design

Explain:

- Why the proposed solution fixes the issue.
- Why it remains stable across all desktop resolutions.
- Why it avoids future regressions.

## Phase 3 – Implementation

Modify only the necessary files.

Keep the changes minimal.

Maintain existing functionality.

Add comments where appropriate.

## Phase 4 – Validation

Verify the dialog on:

- 1920×1080
- 2560×1440
- 2880×1800
- 3440×1440
- 3840×2160

Confirm:

✓ No layout stretching.
✓ Stable typography.
✓ Stable spacing.
✓ Stable column proportions.
✓ Trade Summary correctly aligned.
✓ No excessive whitespace.
✓ No unwanted horizontal scrollbar.
✓ No unwanted vertical scrollbar.
✓ Header fixed.
✓ Footer fixed.
✓ Dialog centered.
✓ Generated Averaging Schedule table displays all rows correctly.
✓ Audit Trail table renders correctly.
✓ No regressions in Details, Averaging Schedule, or Audit Trail tabs.

Do not consider the task complete until all three tabs have been tested and the layout is visually consistent across all supported desktop resolutions.
