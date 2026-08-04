## Additional Issue - Excess Empty Space at the Bottom

There is a large unused white space below the form content and above the footer (Cancel / Save Trade).

This empty area is visible on all desktop resolutions and becomes more noticeable on larger monitors.

Expected behavior:

- The form content should occupy the available vertical space.
- The footer should sit immediately below the content with consistent spacing.
- There should not be a large blank region between the last form section and the footer.
- The popup should feel compact and balanced.

Investigate the root cause instead of hiding it with CSS.

Specifically inspect:

- Dialog height calculation
- DialogContent height
- Parent container height
- display:flex layouts
- flex-direction
- flex-grow
- justify-content
- align-content
- min-height
- max-height
- height:100%
- overflow:auto
- overflow-y
- Material UI DialogContent styles
- Any wrapper adding unnecessary vertical space
- CSS Grid rows
- Grid row sizing
- Fixed heights on parent containers

Determine whether:

- The dialog has an unnecessary fixed height.
- A wrapper is using flex:1 incorrectly.
- Grid rows are consuming extra height.
- The content area is not shrinking correctly.
- The footer is separated because of an oversized content container.

The solution should remove the unnecessary bottom whitespace by fixing the layout architecture, not by reducing padding or applying arbitrary height values.

After the fix:

✓ The last form section should sit naturally above the footer.
✓ Footer should remain bottom-aligned.
✓ Internal scrolling should work if content exceeds available height.
✓ No excessive blank space should appear at the bottom on 1920, 2560, 3440, or 4K monitors.
