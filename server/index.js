The previous layout fix introduced a regression in the AveragingScheduleTab.

The data is loading correctly.

Evidence:
- Schedule Summary shows Total Observations = 12.
- Footer also shows 12 observations.
- The API returns the records correctly.

However, the Generated Averaging Schedule grid only displays the column headers.

The table rows are hidden because the grid/container height has collapsed.

This is NOT a data issue.

It is a layout issue.

Please investigate the following before making any changes:

1. Verify that the table receives all 12 rows.
2. Inspect the DataGrid/table container height.
3. Inspect every parent container from:
   - AsianTradeDialog
   - DialogContent
   - AveragingScheduleTab
   - Generated Schedule section
   - Table wrapper
   - DataGrid/Table component

Look specifically for:

- flex: 1 removed
- min-height: 0 missing
- height: 100% changed
- overflow: hidden
- overflow: auto
- display: flex
- flex-direction: column
- CSS Grid row sizing
- auto vs 1fr rows
- DialogContent height changes
- Parent container collapsing
- Table wrapper shrinking to header height

Expected layout:

Dialog
 ├── Header
 ├── Tabs
 ├── Schedule Generation (auto height)
 ├── Generated Averaging Schedule
 │      ├── Toolbar
 │      └── Data Grid (fills remaining space)
 └── Footer

The grid should consume all remaining vertical space.

Only the grid body should scroll.

The footer should remain fixed.

Do NOT fix this by assigning arbitrary pixel heights.

Do NOT use fixed heights.

Restore the proper flex/grid layout so the DataGrid automatically fills the available space.

Before changing code, explain exactly which parent container is collapsing and why the grid body height becomes zero while the header remains visible.
