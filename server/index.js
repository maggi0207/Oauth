The overall popup layout is now correct across all desktop resolutions.

Please DO NOT modify the dialog width, height, or overall layout anymore.

There is one remaining UI issue inside the form.

## Issue

The form controls are too tightly packed.

Compared to the original design, there is not enough spacing between consecutive text fields and dropdowns.

The UI looks compressed.

## Expected

Increase the vertical spacing between form controls slightly.

Examples:

- Customer → Strategy
- Strategy → Call/Put
- Buy/Sell → Exercise Type
- Trade Date → Expiry Date
- Avg. Frequency → Roll Day

Every field should have a small amount of breathing room similar to the original design.

Do NOT make the spacing excessive.

The goal is to match the original enterprise UI.

## Requirements

- Keep the current dialog size unchanged.
- Keep column widths unchanged.
- Keep font sizes unchanged.
- Keep input heights unchanged.
- Keep section positions unchanged.
- Only adjust spacing between controls.

Inspect:

- Material UI Grid spacing
- Stack spacing
- rowGap
- columnGap
- margin-bottom
- FormControl margins
- Shared FormFields component
- detailsShared.tsx spacing utilities

Use the existing design system spacing tokens if available.

Target a subtle increase (approximately 4–8px) in vertical spacing between controls.

Do not introduce additional whitespace at the bottom of sections or around the dialog.

The result should look closer to the original design while preserving the current responsive layout.
