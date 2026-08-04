The current approach of making the dialog fluid across different desktop resolutions is causing layout inconsistencies.

Instead, implement a fixed desktop layout for the AsianTradeDialog.

## Goal

The dialog should look visually identical on:

- 1920×1080
- 2560×1440
- 2880×1800
- 3440×1440
- 3840×2160

The dialog should NOT continue stretching as the monitor width increases.

Only the surrounding page should become larger.

The dialog should remain centered with the same proportions, spacing, typography, and control sizes.

## Requirements

Use a fixed or constrained dialog width.

Example:

- Fixed desktop width (based on the current approved 1920px design)
- max-width: 95vw so it still fits on smaller screens
- Appropriate fixed height/max-height with internal scrolling

The dialog should maintain:

- Same font sizes
- Same textbox widths
- Same textbox heights
- Same dropdown sizes
- Same button sizes
- Same section spacing
- Same margins
- Same paddings
- Same three-column proportions
- Same Trade Summary width

Do NOT allow controls to become larger on bigger monitors.

Do NOT allow additional whitespace between sections.

Do NOT allow columns to stretch.

## Scrolling

- Header must remain fixed.
- Footer must remain fixed.
- Only the dialog content should scroll if necessary.
- Tables (Generated Averaging Schedule and Audit Trail) should correctly fill the available space and manage their own scrolling.
- No unnecessary horizontal or vertical scrollbars.

## Important

Do not redesign the UI.

Do not modify business logic.

Do not modify functionality.

Do not change component behavior.

Only refactor the dialog layout so that it uses a stable desktop width while preserving the approved 1920px layout.

Before implementing, identify the current Dialog component (Material UI Dialog/Paper) and update its sizing strategy so the dialog behaves as a fixed desktop dialog rather than a fluid responsive dialog.

After implementation, validate that the dialog appears visually identical on:

- 1920×1080
- 2560×1440
- 3440×1440
- 3840×2160

The implementation is complete only if there is no noticeable difference in spacing, font size, control size, or overall layout between these desktop resolutions.
