The previous layout fix removed the excessive bottom whitespace, but it introduced a regression in the Averaging Schedule tab.

Please investigate only the AveragingScheduleTab layout.

Current issue:

- The Generated Averaging Schedule table no longer occupies the available vertical space.
- The table container has collapsed.
- Only a few rows are visible.
- The footer moved upward.
- The table should expand instead of shrinking.

Expected behavior:

The popup should use a proper vertical flex layout.

---------------------------------------------------
Header
Tabs

Schedule Generation

Generated Averaging Schedule
    Toolbar
    Table
    (Table fills remaining vertical space)

Footer
---------------------------------------------------

The table should consume all remaining vertical space between the Schedule Generation section and the footer.

Only the table body should scroll.

The footer must always remain visible.

Do NOT fix this using fixed pixel heights.

Do NOT use arbitrary min-height values.

Instead investigate:

- DialogContent layout
- flex-direction
- flex-grow
- flex-basis
- height:100%
- min-height:0
- overflow:auto
- CSS Grid row sizing
- DataGrid/Table container sizing
- Parent container constraints

Verify whether:

- The table wrapper lost flex:1.
- A parent flex container changed to auto height.
- A min-height:0 is missing.
- The DialogContent no longer fills available height.
- The table container is no longer participating in flex layout.

The correct layout should be:

Dialog
 ├── Header
 ├── Tabs
 ├── Content (display:flex; flex-direction:column)
 │      ├── Schedule Generation (auto height)
 │      └── Generated Schedule (flex:1; min-height:0)
 │              ├── Toolbar
 │              └── Table (fills remaining height)
 └── Footer

Please perform a root cause analysis before modifying the code.

Do not reintroduce the previous excessive bottom whitespace while fixing the table height.
