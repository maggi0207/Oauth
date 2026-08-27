---
name: community-engagement
description: Community Engagement Details/Summary save, retrieve, Next redirect, and volunteering schedule. Use when the user mentions Community Engagement, hardship waiver, Volunteering/Work Program/Unpaid Work, CE retrieve grid empty, save not persisting, or Details reloading on Next.
---

# Community Engagement

Work in this snapshot. List changed files so they can be copied into the original app.

## Files

| File | Role |
|---|---|
| `CommunityEngagementDetails.aspx.cs` | Save/retrieve + Next redirect + checkmark |
| `CommunityEngagementSummary.aspx.cs` | Grid retrieve; do not override `NavigateNext` |
| `ApplicationEntryDataServiceLinqDataSource.cs` | Keep working copy: `IsVolunteeringWorkProgramEnabled` returns `true` |

## Allowed Details extras

After successful `SaveData()`:

```csharp
SetPageComplete();
SetPageComplete(IntakeConstants.COMMUNITYENGAGEMENT_SUMMARY_AE, true);
```

`NavigateNext` must not call `base.NavigateNext()`. Use `ScheduleVolWorkUnPaidScreen` (set in `SaveDataCEMHDeatils`):

```csharp
if (ScheduleVolWorkUnPaidScreen)
{
    ScheduleVolWorkUnPaidScreen = false;
    TechnicalSessionContext.Instance.IsVolunteeringWorkProgramBackToSummary = false;
    Response.Redirect("~/Intake/ApplicationEntry/Technical/VolunteeringWorkProgramUnpaidWorkSummary.aspx");
    return;
}
Response.Redirect("~/Intake/ApplicationEntry/Technical/CommunityEngagementSummary.aspx");
```

## Do not reintroduce

- Broad `ValidatePage()` that can abort save
- Filtering `GetCommunityEngagementDetails(int applicationId)` (workflow context)
- Making `IsVolunteeringWorkProgramEnabled` query summaries
- `page.Visible = true` / reflection on `IWorkflowPage`
- Syncing `CommunityEngagementSummaryID` from `AnchorObject`
- Tax-style `NavigateNext` on Summary
- Hardship popup holding Next (`NavigateNextPending`) unless the user asks

## Retrieve

Summary `BtnRetrieve_Click` → `gvASPxGridView.DataBind()` → `DsCommunityEngagementSummary_Selecting` → `TechnicalContextOperations.GetAllActiveRecordsCommunityEngagementSummary`. If Retrieve broke after a navigation-only change, restore Summary and DataSource first.

## Workflow names (XAML)

`CommunityEngagementSummaryAE`, `CommunityEngagementDetailsAE`, `VolunteeringWorkProgramUnpaidWorkSummaryAE`, `VolunteeringWorkProgramUnpaidWorkAE`. Already present; do not duplicate.
