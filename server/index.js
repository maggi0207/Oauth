You are a Senior React UI Architect specializing in enterprise trading platforms.

## Objective

Investigate and permanently fix the desktop layout issue in the **AsianTradeDialog** popup.

The issue occurs inside the **AsianTradeDetailsTab.tsx** component.

This is NOT a mobile responsive issue.

The popup looks correct on a 1920px monitor but becomes visually inconsistent on larger desktop resolutions (2400, 2560, 3440, 4K).

I want you to perform a root cause analysis before making any code changes.

---

## Components to Investigate

Start from the popup entry point and trace the complete rendering hierarchy.

ActionsMenu
└── AsianTradeDialog
    ├── AsianTradeDetailsTab.tsx   ← Primary focus
    ├── AveragingScheduleTab.tsx
    ├── AuditTrailTab.tsx
    ├── detailsShared.tsx
    └── FormFields.tsx

Also inspect any shared layout components, wrappers, hooks, CSS modules, theme files, styled components, utility classes, and Material UI components used by these files.

---

## Investigation Checklist

Trace the layout from the Dialog root down to every section.

Identify:

- Dialog width calculation
- Parent container constraints
- Nested wrappers
- CSS Grid configuration
- Flexbox configuration
- Material UI Grid/Stack/Box usage
- Shared FormField layout
- detailsShared layout helpers
- Width propagation
- Overflow behavior
- Scroll container
- Theme spacing
- Responsive breakpoints
- Shared enterprise layout utilities

Determine exactly how width is inherited through the component tree.

---

## Specifically Look For

Search for:

- width: 100%
- max-width
- min-width
- flex: 1
- flex-grow
- flex-basis
- justify-content: space-between
- align-stretch
- grid-template-columns
- auto-fit
- auto-fill
- minmax()
- percentage widths
- viewport units (vw)
- Dialog maxWidth/fullWidth props
- container width calculations
- unnecessary wrapper divs
- inherited CSS rules
- reusable layout abstractions

Do not assume the issue is inside AsianTradeDetailsTab.

The root cause may exist in:

- AsianTradeDialog
- detailsShared.tsx
- FormFields.tsx
- Shared layout components
- Global styles
- Theme configuration

---

## Current Problem

On high-resolution monitors:

- Popup stretches too much.
- Left and middle columns become too wide.
- Trade Summary moves too far away.
- Inputs become excessively wide.
- White space increases significantly.
- Section alignment changes.
- Overall proportions no longer match the expected enterprise design.

---

## Expected Behaviour

Regardless of monitor resolution:

1920
2400
2560
2880
3440
3840

The popup should maintain nearly identical proportions.

The dialog should not simply expand because additional screen width is available.

The UI should resemble a professional institutional trading platform where dialogs have stable dimensions and balanced spacing.

---

## Do NOT

Do NOT immediately add max-width values.

Do NOT add random media queries.

Do NOT use CSS hacks.

Do NOT use transform: scale().

Do NOT use zoom.

Do NOT hardcode widths without justification.

Every modification must solve the architectural root cause.

---

## Deliverables

### Phase 1 - Investigation

Explain:

- Component hierarchy
- Width propagation
- Layout architecture
- Root cause
- Every problematic container
- Every problematic CSS rule

### Phase 2 - Solution Design

Explain why the proposed layout architecture fixes the issue.

Explain why it will remain stable across all desktop resolutions.

### Phase 3 - Implementation

Modify only the required files.

Keep business logic untouched.

Maintain existing functionality.

Use modern React and CSS best practices.

### Phase 4 - Validation

Validate the popup at:

- 1920×1080
- 2560×1440
- 3440×1440
- 3840×2160

Confirm:

- Column proportions remain stable.
- Inputs do not stretch.
- Trade Summary remains aligned.
- Consistent spacing.
- Header and footer remain fixed.
- No regressions introduced.

Do not finish until the layout has been verified across all supported desktop resolutions.
