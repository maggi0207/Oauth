# Add New Practitioner — NPI Section UI Redesign

> **Purpose**: This document describes exactly how to modify the "Add New Practitioner" modal to redesign the NPI field area. An AI reading this should be able to implement the UI changes precisely.

---

## 1. What This Task Is About

The "Add New Practitioner" modal currently has the NPI field inline with other fields (Degree, Category) and a simple checkbox for "No NPI". This needs to be redesigned into a **dedicated NPI section** with radio button selection, input field, info text, and helper text — all inside a visually distinct bordered container.

---

## 2. BEFORE — Current UI (What Exists Today)

### Modal: "Add New Practitioner"

**Layout — 4 rows of fields inside a modal dialog:**

```
┌─────────────────────────────────────────────────────────────────────┐
│  Add New Practitioner                                          [X] │
│                                                                     │
│  First Name *      Middle Name      Last Name *      Suffix        │
│  ┌──────────┐     ┌──────────┐     ┌──────────────┐  ┌────────┐   │
│  │First Name│     │Middle Nam│     │Last Name     │  │Suffix  │   │
│  └──────────┘     └──────────┘     └──────────────┘  └────────┘   │
│                                                                     │
│  Date of Birth         Gender              SSN                     │
│  ┌──────────── 📅┐   ┌──────────────── ▼┐  ┌──────────────┐       │
│  │mm/dd/yyyy     │   │Select a Gender   │  │###-###-####  │       │
│  └───────────────┘   └─────────────────-┘  └──────────────┘       │
│                                                                     │
│  Email                 Cell                                        │
│  ┌──────────────┐     ┌──────────────────┐                         │
│  │Email         │     │(###) ###-####    │                         │
│  └──────────────┘     └──────────────────┘                         │
│                                                                     │
│  Degree *              Category *          NPI                     │
│  ┌──────────────┐     ┌──────────────── ▼┐ ┌──────────────┐       │
│  │Degree        │     │Select a Category │ │##########    │       │
│  └──────────────┘     └─────────────────-┘ └──────────────┘       │
│                                              ☐ No NPI              │
│                                                                     │
│                                  [Verify & Start PAF]  [Cancel]    │
└─────────────────────────────────────────────────────────────────────┘
```

### Current NPI area details:
- **NPI label**: `"NPI"` — no asterisk, not marked as required
- **NPI input**: Standard text input, same row as Degree and Category
- **NPI placeholder**: `"##########"` (10 hash symbols)
- **No NPI**: A **checkbox** (`☐ No NPI`) positioned directly below the NPI input field
- **No info text**: No explanation about what "No NPI" means
- **No helper text**: No guidance about what the user should do

### Current required fields (marked with *):
- First Name *
- Last Name *
- Degree *
- Category *

### Current NON-required fields (no asterisk):
- Middle Name, Suffix, Date of Birth, Gender, SSN, Email, Cell, NPI

---

## 3. AFTER — New UI (What It Should Look Like)

### Modal: "Add New Practitioner"

**Layout — 5 rows of fields. Row 5 is the new dedicated NPI section:**

```
┌─────────────────────────────────────────────────────────────────────┐
│  Add New Practitioner                                          [X] │
│                                                                     │
│  First Name *      Middle Name      Last Name *      Suffix        │
│  ┌──────────┐     ┌──────────┐     ┌──────────────┐  ┌────────┐   │
│  │First Name│     │Middle Nam│     │Last Name     │  │Suffix  │   │
│  └──────────┘     └──────────┘     └──────────────┘  └────────┘   │
│                                                                     │
│  Date of Birth *       Gender *            SSN *                   │
│  ┌──────────── 📅┐   ┌──────────────── ▼┐  ┌──────────────┐       │
│  │mm/dd/yyyy     │   │Select a Gender   │  │###-##-####   │       │
│  └───────────────┘   └─────────────────-┘  └──────────────┘       │
│                                                                     │
│  Email *               Cell *                                      │
│  ┌──────────────┐     ┌──────────────────┐                         │
│  │Email         │     │(###) ###-####    │                         │
│  └──────────────┘     └──────────────────┘                         │
│                                                                     │
│  Degree *              Category *                                  │
│  ┌──────────────┐     ┌──────────────── ▼┐                         │
│  │Degree        │     │Select a Category │                         │
│  └──────────────┘     └─────────────────-┘                         │
│                                                                     │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  NPI *                                                      │   │
│  │                                                             │   │
│  │  (●) Enter NPI              (○) No NPI                     │   │
│  │                                                             │   │
│  │  ┌────────────────────┐     ℹ Select this option if the    │   │
│  │  │######## (10 digits)│       practitioner does not have   │   │
│  │  └────────────────────┘       an NPI.                      │   │
│  │                                                             │   │
│  │  Either enter a valid NPI or select "No NPI" to continue.  │   │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
│                                  [Verify & Start PAF]  [Cancel]    │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 4. Exact Changes — What Moved, What's New, What Changed

### Change 1: NPI field REMOVED from Row 4
- **Before**: Row 4 had 3 fields: `Degree *` | `Category *` | `NPI`
- **After**: Row 4 has 2 fields only: `Degree *` | `Category *`
- The NPI field is no longer inline with Degree and Category.

### Change 2: "No NPI" checkbox REMOVED
- **Before**: A checkbox `☐ No NPI` sat below the NPI input field
- **After**: The checkbox is gone. Replaced by radio buttons in the new NPI section.

### Change 3: NEW dedicated NPI section added as Row 5
A new **bordered container/card** is added spanning the full width of the modal, below the Degree/Category row. This container has:
- A visible **border** (solid, light grey or dark blue — matching the mockup's border style) around the entire section
- **Padding** inside the bordered box
- All NPI-related elements are inside this box

### Change 4: Fields that gained required asterisks (*)
These fields did NOT have asterisks before but NOW they do:

| Field | Before | After |
|---|---|---|
| Date of Birth | `Date of Birth` | `Date of Birth *` |
| Gender | `Gender` | `Gender *` |
| SSN | `SSN` | `SSN *` |
| Email | `Email` | `Email *` |
| Cell | `Cell` | `Cell *` |
| NPI | `NPI` | `NPI *` |

Fields that already had asterisks and remain unchanged:
- First Name * (no change)
- Last Name * (no change)
- Degree * (no change)
- Category * (no change)

Fields that remain without asterisks:
- Middle Name (no change)
- Suffix (no change)

---

## 5. NPI Section — Element-by-Element Specification

### 5.1 Container
- **Type**: A bordered box / card / outlined container
- **Border**: Solid border, approximately 1-2px, dark blue or navy color (matching the modal header color `#1B2A4A` or similar from the app's theme)
- **Border radius**: Slight rounding (~4-6px), matching the app's existing card/border radius
- **Width**: Full width of the modal's content area (same width as the field rows above)
- **Padding**: ~16-20px inside the container on all sides
- **Margin top**: ~12-16px gap between the Degree/Category row and this NPI section
- **Background**: White (same as modal background)

### 5.2 Section Label
- **Text**: `"NPI *"` (with red asterisk indicating required)
- **Position**: Top-left inside the container, acts as the section heading
- **Style**: Same bold/dark label style as other field labels in the form (e.g., "First Name *", "Degree *")

### 5.3 Radio Buttons
Two radio buttons displayed horizontally, on the same line:

**Radio 1: "Enter NPI"**
- **Label text**: `"Enter NPI"`
- **Position**: Left side
- **Default state**: **Selected** (filled radio dot ●)
- **Behavior**: When selected, the NPI input field is visible and enabled below

**Radio 2: "No NPI"**
- **Label text**: `"No NPI"`
- **Position**: Right side, same line as "Enter NPI"
- **Default state**: Unselected (empty radio circle ○)
- **Behavior**: When selected, the NPI input field is hidden or disabled. The info text next to it explains why.

### 5.4 NPI Input Field (visible when "Enter NPI" is selected)
- **Type**: Text input
- **Placeholder**: `"######## (10 digits)"` — 8 hash symbols followed by the text "(10 digits)" to guide the user
- **Position**: Below the radio buttons, left side
- **Width**: Approximately 40-50% of the container width (not full width)
- **Validation**: Accepts only numeric, exactly 10 digits (existing NPI validation rules apply)
- **Visibility**: Shown only when "Enter NPI" radio is selected. Hidden when "No NPI" radio is selected.

### 5.5 Info Text (next to "No NPI" option)
- **Icon**: Blue info circle icon (ℹ) — use the same info icon used elsewhere in the app
- **Text**: `"Select this option if the practitioner does not have an NPI."`
- **Position**: To the right of the NPI input field area, vertically aligned with the input field. It sits next to / below the "No NPI" radio button.
- **Style**: Small font (~13px), grey or muted text color, info icon is blue
- **Always visible**: This text is always shown regardless of which radio is selected

### 5.6 Helper Text (bottom of the NPI section)
- **Text**: `"Either enter a valid NPI or select \"No NPI\" to continue."`
- **Position**: Bottom of the container, below both the input field and the info text
- **Style**: Small font (~12-13px), grey/muted text color. Same style as any existing helper/hint text in the app.
- **Always visible**: This text is always shown regardless of which radio is selected

---

## 6. Radio Button Behavior — State Changes

### State A: "Enter NPI" selected (DEFAULT)
```
┌─────────────────────────────────────────────────────────────┐
│  NPI *                                                      │
│                                                             │
│  (●) Enter NPI              (○) No NPI                     │
│                                                             │
│  ┌────────────────────┐     ℹ Select this option if the    │
│  │######## (10 digits)│       practitioner does not have   │
│  └────────────────────┘       an NPI.                      │
│                                                             │
│  Either enter a valid NPI or select "No NPI" to continue.  │
└─────────────────────────────────────────────────────────────┘
```
- NPI input field is **visible and enabled**
- User types a 10-digit NPI
- "Verify & Start PAF" button requires a valid 10-digit NPI to proceed

### State B: "No NPI" selected
```
┌─────────────────────────────────────────────────────────────┐
│  NPI *                                                      │
│                                                             │
│  (○) Enter NPI              (●) No NPI                     │
│                                                             │
│                              ℹ Select this option if the    │
│                                practitioner does not have   │
│                                an NPI.                      │
│                                                             │
│  Either enter a valid NPI or select "No NPI" to continue.  │
└─────────────────────────────────────────────────────────────┘
```
- NPI input field is **hidden** (not shown at all)
- The NPI value is cleared
- "Verify & Start PAF" button can proceed without an NPI
- The info text and helper text remain visible

---

## 7. Summary of All Changes

| # | What | Before | After |
|---|---|---|---|
| 1 | NPI field position | Row 4, inline with Degree & Category | Row 5, own dedicated bordered section |
| 2 | NPI selection method | Checkbox `☐ No NPI` | Radio buttons: `(●) Enter NPI` / `(○) No NPI` |
| 3 | NPI default state | Input visible, checkbox unchecked | "Enter NPI" radio selected, input visible |
| 4 | NPI placeholder | `##########` | `######## (10 digits)` |
| 5 | NPI required indicator | No asterisk | `NPI *` with asterisk |
| 6 | Info text | None | `"Select this option if the practitioner does not have an NPI."` with ℹ icon |
| 7 | Helper text | None | `"Either enter a valid NPI or select 'No NPI' to continue."` |
| 8 | NPI container | No border/box | Bordered container around entire NPI section |
| 9 | Date of Birth label | `Date of Birth` | `Date of Birth *` (added required) |
| 10 | Gender label | `Gender` | `Gender *` (added required) |
| 11 | SSN label | `SSN` | `SSN *` (added required) |
| 12 | Email label | `Email` | `Email *` (added required) |
| 13 | Cell label | `Cell` | `Cell *` (added required) |
| 14 | Row 4 fields | Degree, Category, NPI (3 fields) | Degree, Category (2 fields only) |

---

## 8. What Does NOT Change

- Modal title: `"Add New Practitioner"` — no change
- X close button — no change
- First Name *, Middle Name, Last Name *, Suffix — no change
- Degree *, Category * — no change (just NPI removed from their row)
- "Verify & Start PAF" button — no change to styling
- "Cancel" button — no change
- Modal size/width — no change
- All existing validation rules for other fields — no change
- Form submission behavior — no change (except NPI is now handled via radio selection)

---

## 9. Implementation Checklist

- [ ] Remove NPI input field from the Degree/Category row (Row 4)
- [ ] Remove the "No NPI" checkbox
- [ ] Add a new bordered container below the Degree/Category row
- [ ] Inside the container: add `"NPI *"` label
- [ ] Inside the container: add two radio buttons — "Enter NPI" (default selected) and "No NPI"
- [ ] Inside the container: add NPI input field (shown when "Enter NPI" selected, hidden when "No NPI" selected)
- [ ] Inside the container: add info text with ℹ icon: `"Select this option if the practitioner does not have an NPI."`
- [ ] Inside the container: add helper text: `"Either enter a valid NPI or select 'No NPI' to continue."`
- [ ] Update NPI placeholder from `"##########"` to `"######## (10 digits)"`
- [ ] Add required asterisk (*) to: Date of Birth, Gender, SSN, Email, Cell, NPI
- [ ] Wire radio button toggle: "Enter NPI" shows input, "No NPI" hides input and clears value
- [ ] Ensure "Verify & Start PAF" works: requires valid NPI when "Enter NPI" selected, allows proceed when "No NPI" selected
