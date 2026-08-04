The previous implementations do not meet the requirement.

Please revert to the approved desktop layout and implement the AsianTradeDialog using fixed desktop dimensions while preserving the existing UI.

## Objective

The AsianTradeDialog is an enterprise desktop popup and should maintain a consistent appearance across all supported desktop resolutions.

The dialog should NOT resize or stretch based on the viewport.

The approved 1920×1080 layout is the reference and must remain unchanged.

---

## Investigation First

Before making any changes:

1. Inspect the Material UI Dialog configuration.
2. Inspect the Dialog Paper sizing configuration.
3. Identify where the dialog width and height are currently calculated.
4. Explain why the dialog expands on larger resolutions.
5. Replace the responsive sizing strategy with a fixed desktop sizing strategy.

Do not modify the internal layout unless absolutely necessary.

---

## Dialog Size Requirements

The dialog must use:

- Fixed desktop width
- Fixed pixel height

The dialog must remain centered.

Extra monitor space should remain outside the dialog.

The dialog should appear visually identical on:

- 1920×1080
- 2560×1440
- 2880×1800
- 3440×1440
- 3840×2160

---

## Height Requirement

The current implementation uses viewport units (`vh`) for the dialog height.

Replace the viewport-based height with a **fixed pixel height**.

Remove all usage of:

- vh
- vw
- %
- 100%
- 100vh
- 100vw

Do not calculate the dialog height using viewport units or percentages.

Use an appropriate fixed pixel height that matches the approved desktop design.

---

## Preserve Existing Layout

Do NOT change the approved 1920px layout.

Preserve exactly:

- Font sizes
- Text box widths
- Text box heights
- Dropdown sizes
- Date picker sizes
- Button sizes
- Label spacing
- Margins
- Padding
- Section spacing
- Trade Summary width
- Three-column proportions

The goal is to preserve the existing UI exactly, while preventing the dialog from resizing across desktop resolutions.

---

## Scrolling Behavior

The Header must always remain visible.

The Footer must always remain visible.

Only the dialog content should scroll if required.

Do not introduce:

- Horizontal scrollbars
- Nested vertical scrollbars
- Page scrollbars caused by the dialog

For tables:

- Generated Averaging Schedule must continue to occupy the available content area.
- Audit Trail must continue to occupy the available content area.
- Table rows must always be visible.
- Only the table body should scroll when necessary.

---

## Empty Space

Do not introduce:

- Empty space below forms
- Empty space above the footer
- Empty space below tables
- Empty space beside the Trade Summary
- Large unused whitespace anywhere inside the dialog

The popup should remain compact and balanced.

---

## Do NOT

Do NOT:

- Redesign the UI
- Change business logic
- Change API calls
- Change component behavior
- Change validation
- Change spacing
- Change typography
- Change control sizes
- Use transform: scale()
- Use zoom
- Use viewport-based sizing
- Introduce CSS hacks

Only update the Dialog and Dialog Paper sizing strategy to use fixed desktop dimensions while preserving the existing internal layout.

---

## Validation

After implementation, verify on:

- 1920×1080
- 2560×1440
- 2880×1800
- 3440×1440
- 3840×2160

Confirm:

✓ Dialog remains centered.
✓ Fixed desktop width.
✓ Fixed pixel height.
✓ No usage of vh, vw, or percentage-based sizing.
✓ No layout stretching.
✓ No oversized controls.
✓ No changes to spacing.
✓ No changes to typography.
✓ No excessive whitespace.
✓ No unwanted horizontal scrollbar.
✓ No unwanted vertical scrollbar.
✓ Header remains fixed.
✓ Footer remains fixed.
✓ Generated Averaging Schedule displays all rows correctly.
✓ Audit Trail displays correctly.
✓ No regressions in Asian Trade Details, Averaging Schedule, or Audit Trail tabs.

Do not consider the task complete until all three tabs have been tested and the dialog appears visually identical across all supported desktop resolutions while using fixed desktop dimensions.
