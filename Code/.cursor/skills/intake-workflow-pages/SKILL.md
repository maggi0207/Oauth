---
name: intake-workflow-pages
description: Fix DHSS Assist WorkerWeb intake WorkflowPage screens (Web Forms + DevExpress) in this snapshot. Use when editing Technical .aspx.cs pages, SaveData, NavigateNext, SetPageComplete, left-nav checkmarks, workflow XAML, or when Next reloads the same details page.
---

# Intake WorkflowPage pages

## Stack

ASP.NET Web Forms, DevExpress editors, `Infrastructure.Workflow.WorkflowPage<T>`. Not ASP.NET Core.

This workspace is the snapshot to **edit directly**. After changes, list files for the user to copy into the original WorkerWeb app.

## Before editing

1. Find the sibling that already works (usually Tax Dependency or Technical Questions).
2. Touch the smallest surface: page `.aspx.cs` first. Leave `ApplicationEntryDataServiceLinqDataSource.cs`, `TechnicalContextOperations.cs`, and `Intake-*.xaml` alone unless retrieve/save is proven broken there.

## Save vs Next

| Method | Job |
|---|---|
| `SaveData()` | Persist DB, rebind FormViews, `SetPageComplete` |
| `NavigateNext()` | Leave or advance records |
| `NavigatePrevious()` / Back To Summary | Unsaved-changes popup, then summary |

Footer Save+Next typically runs `SaveData` then `NavigateNext` in one postback.

## Detail vs summary Next

Detail pages have `Context="{n:DataSource …}"`. `base.NavigateNext()` moves to the **next entity in that context** and reloads the same `.aspx`. That looks like a refresh.

When the worker must leave Details, use the same pattern as Back To Summary:

```csharp
Response.Redirect("~/Intake/ApplicationEntry/Technical/NextPage.aspx");
```

Do not use `base.NavigateNext()` on that path.

## Checkmarks

- Current (hidden) details page: `SetPageComplete()`.
- Left-nav summary item: `SetPageComplete("PageNameSummaryAE", true)`.
- `SetPreviousPageComplete(true)` only when `IsContextComplete()` is true (all household members). That will not check off after the first person.

## Visibility / scheduling

XAML `Visible="{n:Method … IsXxxEnabled}"` wins every request. Do not set `page.Visible = true` (`Visible` is `Field<bool>`). Prefer `SetPageComplete(pageName, false)` to mark a sibling incomplete, matching Tax Dependency scheduling Tax Deductions.

## Popups and callbacks

`Response.Redirect` inside a DevExpress `WindowCallback` does not navigate. Use `ASPxWebControl.RedirectOnCallback(url)` only there. Do not hold Next for a popup unless `ShowOnPageLoad` is true.

## Session keys

Pages that select by `TechnicalSessionContext` ids (for example `CommunityEngagementSummaryID`) must keep the id the summary grid set. Overwriting from `AnchorObject` can save onto the wrong person.
