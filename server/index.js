The previous implementation is incorrect.

The dialog is now expanding to almost the full screen.

This is NOT the expected behavior.

## Requirements

The AsianTradeDialog must remain a centered modal dialog.

Do NOT convert it into a full-screen dialog.

Do NOT use:

- fullScreen
- width: 100%
- height: 100%
- maxWidth={false} with width:100%
- Paper width: 100%
- 100vw
- 100vh

The dialog should retain approximately the same size as the current approved 1920px design.

Only prevent it from stretching on larger monitors.

## Desired Behavior

- Dialog remains centered.
- Dialog keeps nearly the same width as on a 1920px monitor.
- On larger monitors, extra screen space remains outside the dialog.
- Internal layout remains unchanged.
- Font sizes remain unchanged.
- Textbox sizes remain unchanged.
- Spacing remains unchanged.
- Three-column layout remains unchanged.

The goal is to constrain the dialog's maximum width, **not** make it fill the screen.

## Before making changes

Inspect the Material UI Dialog configuration and identify why it is expanding to full screen.

Only update the Dialog/Paper sizing configuration.

Do not modify the internal layout unless absolutely necessary.

After implementation, verify that the dialog remains centered and has approximately the same dimensions on:

- 1920×1080
- 2560×1440
- 3440×1440
- 3840×2160

The dialog should never appear as a full-screen window.
