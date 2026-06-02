# NPI Section — UI Fixes to Match Figma Design

> **Purpose**: This document lists all UI fixes needed in the NPI section of the "Add New Practitioner" modal. The current implementation is close but has 6 issues that need fixing to match the Figma design exactly.

---

## Fix 1: Border Color

- **Current**: Thin light grey border around the NPI container
- **Required**: Dark navy blue border matching the modal header color (approximately `#1B2A4A` or the app's primary dark color)
- **Action**: Find the NPI section container element. Change its border color from grey to the same dark navy blue used in the modal header bar. Keep border width at 1-2px and keep the existing border-radius.

---

## Fix 2: NPI Input Placeholder Text

- **Current**: Shows `1861482259` as pre-filled value or sample data
- **Required**: Empty input with grey placeholder text `######## (10 digits)`
- **Action**: Clear any default value from the NPI input. Set the placeholder attribute to `"######## (10 digits)"`. The input should be empty by default with this grey hint text visible.

---

## Fix 3: Info Text Wording

- **Current**: `Select "No NPI" if the practitioner does not have an NPI.`
- **Required**: `Select this option if the practitioner does not have an NPI.`
- **Action**: Change `"No NPI"` to `this option` in the info text string. Keep the ℹ info icon before the text.

---

## Fix 4: Radio Button Spacing

- **Current**: Both radio buttons ("Enter NPI" and "No NPI") are close together on the left side
- **Required**: "Enter NPI" on the LEFT side, "No NPI" on the RIGHT side — spread apart with a clear gap, roughly 50/50 split of the container width

**Current (wrong):**
```
(●) Enter NPI  (○) No NPI                          ← too close
```

**Required (correct):**
```
(●) Enter NPI                    (○) No NPI         ← spread apart
```

- **Action**: Use a two-column grid or flex layout for the radio buttons row. Place "Enter NPI" in the left column and "No NPI" in the right column. Each column takes approximately 50% of the container width.

---

## Fix 5: NPI Input Field Alignment

- **Current**: Input field is on the left but not visually connected to the "Enter NPI" radio above it
- **Required**: Input field sits **directly below** the "Enter NPI" radio button, left-aligned in the **left column**

- **Action**: Place the NPI input field inside the same left column as the "Enter NPI" radio. It should be directly below the radio button with a small vertical gap (~8-12px).

---

## Fix 6: Info Text Alignment

- **Current**: Info text (ℹ icon + message) is floating to the right of the input, not aligned with "No NPI" radio
- **Required**: Info text sits **directly below** the "No NPI" radio button, in the **right column**

- **Action**: Place the info text (ℹ icon + text) inside the same right column as the "No NPI" radio. It should be directly below the radio button, vertically aligned with the NPI input field on the left.

---

## Correct Layout — Two Column Grid

The NPI section should use a **two-column layout** for the radio + content area:

```
┌──────────────────────────────────────────────────────────────────┐
│                                                                  │
│  NPI *                            ← LABEL: full width, top-left │
│                                                                  │
│  ┌──── LEFT COLUMN (50%) ────┐   ┌──── RIGHT COLUMN (50%) ────┐│
│  │                           │   │                             ││
│  │  (●) Enter NPI            │   │  (○) No NPI                ││
│  │                           │   │                             ││
│  │  ┌──────────────────────┐ │   │  ℹ Select this option if   ││
│  │  │ ######## (10 digits) │ │   │    the practitioner does   ││
│  │  └──────────────────────┘ │   │    not have an NPI.        ││
│  │                           │   │                             ││
│  └───────────────────────────┘   └─────────────────────────────┘│
│                                                                  │
│  Either enter a valid NPI or select "No NPI" to continue.       │
│  ↑ HELPER TEXT: full width, bottom of container                  │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

### Structure breakdown:

```
NPI Container (bordered box, dark navy border)
├── Row 1: "NPI *" label (full width)
├── Row 2: Two-column grid
│   ├── Left Column (50%)
│   │   ├── (●) Enter NPI radio button
│   │   └── NPI text input (placeholder: "######## (10 digits)")
│   │       └── Visible only when "Enter NPI" is selected
│   └── Right Column (50%)
│       ├── (○) No NPI radio button
│       └── ℹ "Select this option if the practitioner does not have an NPI."
└── Row 3: Helper text (full width)
    └── "Either enter a valid NPI or select 'No NPI' to continue."
```

---

## All 6 Fixes — Quick Reference

| # | Element | Current (Wrong) | Required (Correct) |
|---|---|---|---|
| 1 | Border color | Light grey | Dark navy blue (`#1B2A4A`) |
| 2 | NPI placeholder | `1861482259` | `######## (10 digits)` |
| 3 | Info text | `Select "No NPI" if...` | `Select this option if...` |
| 4 | Radio spacing | Close together on left | Spread apart — left/right columns |
| 5 | Input position | Not aligned to radio | Directly below "Enter NPI" radio (left column) |
| 6 | Info text position | Floating right of input | Directly below "No NPI" radio (right column) |

---

## State B Reminder — When "No NPI" is Selected

When user clicks "No NPI" radio:
- The NPI input field **hides** (left column becomes empty or collapses)
- The ℹ info text **stays visible** (right column unchanged)
- The helper text **stays visible** (bottom row unchanged)
- The NPI value is **cleared**

```
┌──────────────────────────────────────────────────────────────────┐
│  NPI *                                                          │
│                                                                  │
│  ┌──── LEFT COLUMN ──────────┐   ┌──── RIGHT COLUMN ──────────┐│
│  │  (○) Enter NPI            │   │  (●) No NPI                ││
│  │                           │   │                             ││
│  │                           │   │  ℹ Select this option if   ││
│  │  (input hidden)           │   │    the practitioner does   ││
│  │                           │   │    not have an NPI.        ││
│  └───────────────────────────┘   └─────────────────────────────┘│
│                                                                  │
│  Either enter a valid NPI or select "No NPI" to continue.       │
└──────────────────────────────────────────────────────────────────┘
```
