# Practitioner Search Revamp – Blazor Implementation Specification

> **Purpose**: This document tells you exactly what to change in the existing Practitioner Search Blazor components. Follow each step literally. Do NOT add inline styles, new CSS, or new component libraries. Reuse only what already exists in the codebase.

> [!CAUTION]
> ## BEFORE YOU START — Read These Rules
> 1. **Open and read** `PractitionerSearchForm.razor` and `PractitionerSearchForm.razor.cs` fully before making any changes.
> 2. **Note the exact property names** from the model class (e.g., `PractitionerNpi`, `PractitionerSsn`). Use those exact names — do NOT guess.
> 3. **Note the exact element IDs** from the existing form fields (e.g., `practitionerNpi`, `practitionerSsn`). Use those exact IDs.
> 4. **Note the exact `OnChange` handler name** used on existing fields (e.g., `OnFieldChanged`). Reuse that same handler on existing fields.
> 5. **Search the codebase** for existing validation methods, model properties, Select/DatePicker components, and audit services before adding new ones. If something already exists, reuse it. Only add new code if nothing equivalent is found.
> 6. **Do NOT delete** any existing form fields, model properties, validation attributes, or event handlers. Only wrap existing code in `@if` conditionals to show/hide.

> [!IMPORTANT]
> ## Implementation Order — Follow This Sequence
> **Complete User Story 1 fully before starting User Story 2.** Do not mix changes from both stories in a single pass.
>
> **Step 1 — User Story 1 (NPI as Primary Search):**
> 1. Update `<PageDescription>` text in `PractitionerSearchPage.razor`
> 2. Add `SearchMode` enum and state variables to `PractitionerSearchForm.razor.cs`
> 3. Hide existing SSN, First Name, Last Name fields behind `@if (_currentSearchMode == SearchMode.ExceptionSearch)` conditional
> 4. Update NPI field label to `"NPI (Required)"` with placeholder `"Enter 10-digit NPI"`
> 5. Add NPI real-time validation and error message display
> 6. Add `Disabled` logic to Search button (disabled until valid 10-digit NPI)
> 7. Add `"No NPI available? Search without NPI"` link below Search button
> 8. **Verify** — build and confirm: only NPI field + disabled Search button + link are visible on page load
>
> **Step 2 — User Story 2 (Exception Workflow):**
> 1. Implement `SwitchToExceptionSearch()` and `SwitchToNpiSearch()` toggle methods
> 2. Make Card title dynamic based on search mode
> 3. Add description text for exception mode
> 4. Add new Reason dropdown field (hardcoded, one option: `"Practitioner does not have NPI"`)
> 5. Show existing First Name, Last Name, SSN fields in exception mode (with updated labels)
> 6. Add new DOB date picker field
> 7. Add SSN visibility toggle (eye icon)
> 8. Add Cancel button in exception mode (returns to NPI search)
> 9. Add exception mode validation and `IsSearchEnabled()` logic
> 10. Add audit logging in `OnFinish` for exception searches
> 11. Add new model properties (`DateOfBirth`, `ExceptionReason`, `SearchType`)
> 12. **Verify** — build and confirm: clicking the link switches to exception form, Cancel returns to NPI form

---

## 1. Existing Code Structure (Current State)

### File Locations

```
Hca.Credentialing/
└── Hca.Credentialing/
    └── Client/
        └── Pages/
            └── PractitionerSearch/
                ├── PractitionerSearchPage.razor          ← Main page
                ├── PractitionerSearchPage.razor.cs       ← Page code-behind
                └── Components/
                    ├── PractitionerSearchForm.razor       ← Search form (LEFT panel) — PRIMARY FILE TO MODIFY
                    ├── PractitionerSearchForm.razor.cs    ← Form code-behind
                    ├── PractitionerSearchTable.razor      ← Results table (RIGHT panel)
                    └── PractitionerSearchTable.razor.cs   ← Table code-behind
```

### Existing Component Library & Patterns Already in Use

The application uses **AntDesign Blazor** and custom wrapper components. **All new markup must reuse these same components and patterns:**

| Component | Usage Pattern | Notes |
|---|---|---|
| `<Card>` | `<Card Hoverable="false" Title="...">` | Card container with `<Body>` slot |
| `<Form>` | `<Form @ref="_form" Model="@(_model)" Layout="@FormLayout.Vertical" OnFinish="OnFinish" ...>` | Form wrapper with vertical layout |
| `<FormItem>` | Wraps each field with `<LabelTemplate>` and `<ChildContent>` | Standard field wrapper — reuse this exact pattern for ALL new fields |
| `<InputHca>` | `<InputHca Id="..." @bind-Value="@_model.Property" OnChange="OnFieldChanged" />` | Custom text input component — reuse for all text inputs |
| `<Button>` | AntDesign `<Button Type="@ButtonType.Primary" HtmlType="submit">` | Reuse existing button types and properties |
| `<Row>` / `<Col>` | `<Row><Col Span="6">...<Col Span="18">` | Grid layout — keep existing spans |
| `<CredentialingPageLayout>` | Page-level layout with `<PageDescription>` and `<ChildContent>` slots | Do not modify layout structure |
| `<Icon>` | AntDesign `<Icon Type="..." Theme="..." />` | Use for any icons needed |
| `<label>` | `<label for="elementId">Field Name</label>` inside `<LabelTemplate>` | Reuse this exact label pattern |

### Existing Coding Patterns to Follow

**Model binding pattern:**
```razor
<InputHca Id="practitionerNpi" @bind-Value="@_model.PractitionerNpi" OnChange="OnFieldChanged" />
```
→ All new fields MUST follow this same `@bind-Value="@_model.PropertyName"` pattern.

**Label pattern:**
```razor
<FormItem>
    <LabelTemplate>
        <label for="practitionerNpi">NPI</label>
    </LabelTemplate>
    <ChildContent>
        <InputHca Id="practitionerNpi" @bind-Value="@_model.PractitionerNpi" OnChange="OnFieldChanged" />
    </ChildContent>
</FormItem>
```
→ All new fields MUST follow this same `<FormItem>` → `<LabelTemplate>` → `<ChildContent>` structure.

**Validation pattern:**
The form uses `ValidateOnChange="false"` and `OnFinish="OnFinish"` — validation fires on submit. The existing `OnFieldChanged` handler is used for field-level change tracking. Follow the same pattern.

**Callback pattern:**
```razor
<PractitionerSearchForm OnSearchCallback="p => OnSearch(p)"/>
```
→ The form exposes an `EventCallback` parameter. Keep this same callback pattern.

**Logger pattern:**
```csharp
@inject ILogger<PractitionerSearchForm> _logger
```
→ Use the existing `_logger` for audit logging.

### Current Form Fields (Known from Screenshot)

| Field | Label | Model Property | Component | ID |
|---|---|---|---|---|
| NPI | `NPI` | `_model.PractitionerNpi` | `<InputHca>` | `practitionerNpi` |
| SSN | `SSN` | `_model.PractitionerSsn` | `<InputHca>` | `practitionerSsn` |
| First Name | `First Name` | **Read from code** — open `PractitionerSearchForm.razor`, find the First Name `<FormItem>`, copy the exact `@bind-Value` property name and `Id` | `<InputHca>` | **Read from code** |
| Last Name | `Last Name` | **Read from code** — open `PractitionerSearchForm.razor`, find the Last Name `<FormItem>`, copy the exact `@bind-Value` property name and `Id` | `<InputHca>` | **Read from code** |

> **Action required**: Open `PractitionerSearchForm.razor`. Find the First Name and Last Name `<FormItem>` blocks. Copy their exact `Id`, `@bind-Value`, and `OnChange` values. Use those exact values in all code below where you see `_model.FirstName`, `_model.LastName`, `existingFirstNameId`, or `existingLastNameId`.

### Current Search Button

**Action**: Open `PractitionerSearchForm.razor`, scroll to the bottom of the `<Form>`, find the existing Search `<Button>`. Note its exact markup. You will modify that button by adding `Disabled="@(!IsSearchEnabled())"` to it. Keep everything else the same.

---

## 2. What Needs to Change — Overview

Transform the Practitioner Search form from showing all fields simultaneously to a **two-mode workflow**:

1. **Default Mode (NPI Search)**: Show ONLY the NPI field + Search button (disabled until valid) + "No NPI available?" link.
2. **Exception Mode (No NPI Available)**: Show Reason dropdown + existing First Name + existing Last Name + new DOB + existing SSN fields — triggered by clicking the link.

### Key Principle
**Reuse everything.** The SSN, First Name, and Last Name `<FormItem>` blocks already exist — **wrap them in `@if` conditionals** to show/hide based on mode. Use the same `<FormItem>`, `<LabelTemplate>`, `<InputHca>`, `<Button>` patterns for any new fields.

> [!CAUTION]
> ## Check Before Adding — Do NOT Duplicate Existing Code
> Before adding ANY new validation logic, helper method, property, component, or CSS class — **first search the existing codebase** to check if it already exists. Only add new code if nothing equivalent is found.
>
> **Specifically check for:**
> - **NPI validation**: Search for existing NPI validation methods, `[Validation]` attributes, or regex patterns in the model class and form code-behind. If NPI validation already exists, **reuse it** — do NOT write a duplicate.
> - **SSN masking/validation**: Search for existing SSN masking, formatting, or visibility toggle logic. If the app already masks SSN inputs somewhere, **reuse that approach**.
> - **DOB validation**: Search for existing date-of-birth validation or date picker usage. Reuse any existing `<DatePicker>` wrapper or validation.
> - **Name field validation**: Existing First Name / Last Name validation (character restrictions, min length) must stay as-is. Do NOT add new validation for these fields.
> - **Model properties**: Check if `DateOfBirth`, `SearchType`, or similar properties already exist on the model before adding them.
> - **Error message CSS classes**: Search for existing form error/validation CSS classes before using `ant-form-item-explain-error`. Use whatever class the app already uses.
> - **Select/Dropdown component**: Search for existing `<Select>` or `<SelectHca>` usage and follow that exact pattern.
> - **DatePicker component**: Search for existing `<DatePicker>` or `<DatePickerHca>` usage and follow that exact pattern.
> - **Button spacing pattern**: Search for how existing forms lay out multiple buttons side by side.
> - **Audit logging**: Search for existing audit service (`IAuditService`, `AuditLog`) before using `_logger` for audit entries.
>
> **Rule: If it exists in the codebase, reuse it. If it doesn't exist, then and only then add it.**

---

## 3. Changes to PractitionerSearchPage.razor

### 3.1 Update Page Description Text Only

**Find the existing `<PageDescription>` block and change only the text content:**

**Current:**
```razor
<PageDescription>
    To begin creating a PAF, please search for and select a Practitioner below. To search, either enter a full 9-digit SSN, a full 10-digit NPI, or at least 1 character for First Name and 2 characters for Last Name.
</PageDescription>
```

**Change to:**
```razor
<PageDescription>
    To begin creating a PAF, search for a Practitioner by entering a valid 10-digit NPI.
</PageDescription>
```

**No other changes to this file.** Keep the `<Row>`, `<Col>` layout, `<PractitionerSearchForm>`, and `<PractitionerSearchTable>` exactly as they are.

---

## 4. Changes to PractitionerSearchForm.razor.cs (Code-Behind)

### 4.1 Add Search Mode Enum and State Variables

Add these to the existing code-behind class, alongside the existing fields:

```csharp
// ── Search Mode ──
private enum SearchMode { NpiSearch, ExceptionSearch }
private SearchMode _currentSearchMode = SearchMode.NpiSearch;

// ── NPI Validation State ──
private string _npiValidationMessage = string.Empty;
private bool _showNpiValidationError = false;

// ── Exception Search State ──
private string _selectedReason = string.Empty;
private bool _showExceptionValidationError = false;

// ── SSN Visibility Toggle ──
private bool _isSsnVisible = false;
```

### 4.2 Add Search Mode Toggle Methods

```csharp
private void SwitchToExceptionSearch()
{
    _currentSearchMode = SearchMode.ExceptionSearch;
    _model.PractitionerNpi = string.Empty;
    _showNpiValidationError = false;
    _npiValidationMessage = string.Empty;
    StateHasChanged();
}

private void SwitchToNpiSearch()
{
    _currentSearchMode = SearchMode.NpiSearch;
    _selectedReason = string.Empty;
    _model.PractitionerSsn = string.Empty;
    // Clear First Name, Last Name, DOB using the actual property names from the model
    // _model.FirstName = string.Empty;
    // _model.LastName = string.Empty;
    // _model.DateOfBirth = null;
    _showExceptionValidationError = false;
    _isSsnVisible = false;
    StateHasChanged();
}
```

### 4.3 NPI Validation — Check Existing First, Only Add if Missing

> **FIRST**: Search the codebase for existing NPI validation:
> 1. Check the model class for `[Validation]`, `[Required]`, `[RegularExpression]`, or custom validation attributes on `PractitionerNpi`.
> 2. Check `PractitionerSearchForm.razor.cs` for any existing NPI validation method (e.g., `ValidateNpi`, `IsValidNpi`, or NPI-related logic inside `OnFieldChanged`).
> 3. Check for shared validation helper classes (e.g., `NpiValidator`, `ValidationHelper`).
>
> **If existing NPI validation is found** → Reuse it. Call the existing method and wire its result to control the Search button's `Disabled` state. Only add the `_showNpiValidationError` / `_npiValidationMessage` display logic if there is no existing way to show validation errors inline.
>
> **If NO existing NPI validation is found** → Then and only then add the method below, following the existing `OnFieldChanged` pattern:

```csharp
private void OnNpiFieldChanged(ChangeEventArgs args)
{
    // Call existing OnFieldChanged first if it does other work
    // OnFieldChanged(args);

    var value = _model.PractitionerNpi ?? string.Empty;

    if (string.IsNullOrEmpty(value))
    {
        _showNpiValidationError = false;
        _npiValidationMessage = string.Empty;
    }
    else if (!value.All(char.IsDigit) || value.Length != 10)
    {
        _showNpiValidationError = true;
        _npiValidationMessage = "Please enter a valid 10-digit NPI.";
    }
    else if (value[0] != '1' && value[0] != '2')
    {
        _showNpiValidationError = true;
        _npiValidationMessage = "NPI must start with a 1 or 2";
    }
    else
    {
        _showNpiValidationError = false;
        _npiValidationMessage = string.Empty;
    }

    StateHasChanged();
}
```

### 4.4 Add Search Enabled Check

```csharp
private bool IsSearchEnabled()
{
    if (_currentSearchMode == SearchMode.NpiSearch)
    {
        var npi = _model.PractitionerNpi ?? string.Empty;
        return npi.Length == 10
            && npi.All(char.IsDigit)
            && (npi[0] == '1' || npi[0] == '2');
    }
    else
    {
        // Exception mode: Reason + Last Name + DOB are required
        return !string.IsNullOrWhiteSpace(_selectedReason)
            && !string.IsNullOrWhiteSpace(_model.LastName)     // use actual property name
            && _model.DateOfBirth.HasValue;                     // use actual property name
    }
}
```

### 4.5 Modify Existing OnFinish Method

Find the existing `OnFinish` method and add search-mode handling:

```csharp
private async Task OnFinish()
{
    if (_currentSearchMode == SearchMode.ExceptionSearch)
    {
        // Validate required exception fields
        if (!IsSearchEnabled())
        {
            _showExceptionValidationError = true;
            StateHasChanged();
            return;
        }

        // Set search type for backend
        _model.SearchType = "Exception";
        _model.ExceptionReason = _selectedReason;

        // Audit log using existing _logger
        _logger.LogInformation(
            "Exception Practitioner Search: User performed search without NPI. Reason={Reason}, Criteria=[LastName={LastName}, DOB={DOB}, SSN={HasSSN}]",
            _selectedReason,
            _model.LastName,                                    // use actual property name
            _model.DateOfBirth?.ToString("MM/dd/yyyy"),         // use actual property name
            !string.IsNullOrEmpty(_model.PractitionerSsn));
    }
    else
    {
        _model.SearchType = "NPI";
    }

    // Call existing search callback (keep this line as-is from existing code)
    await OnSearchCallback.InvokeAsync(_model);
}
```

---

## 5. Changes to PractitionerSearchForm.razor (Markup)

### 5.1 Make Card Title Dynamic

**Find:**
```razor
<Card Hoverable="false" Title="Practitioner Search">
```

**Replace with:**
```razor
<Card Hoverable="false" Title="@(_currentSearchMode == SearchMode.NpiSearch ? "Practitioner Search" : "Exception Search (No NPI Available)")">
```

### 5.2 Add Description Text for Exception Mode

**Inside `<Body>`, before the `<Form>`, add:**
```razor
@if (_currentSearchMode == SearchMode.ExceptionSearch)
{
    <p>Please provide a reason and additional information to search for the practitioner.</p>
}
```

> **Styling**: Do NOT add inline styles. This `<p>` tag will inherit the existing Card body typography. If the application has a CSS class for description text (check existing `.razor` files for similar patterns), use that class.

### 5.3 NPI Field — Wrap in Conditional, Update Label

**Find the existing NPI `<FormItem>` block and wrap it:**

```razor
@if (_currentSearchMode == SearchMode.NpiSearch)
{
    <FormItem>
        <LabelTemplate>
            <label for="practitionerNpi">NPI (Required)</label>
        </LabelTemplate>
        <ChildContent>
            <InputHca Id="practitionerNpi"
                      @bind-Value="@_model.PractitionerNpi"
                      Placeholder="Enter 10-digit NPI"
                      OnChange="OnNpiFieldChanged" />
            @if (_showNpiValidationError && !string.IsNullOrEmpty(_npiValidationMessage))
            {
                <span class="ant-form-item-explain ant-form-item-explain-error">
                    <Icon Type="close-circle" Theme="fill" />
                    @_npiValidationMessage
                </span>
            }
        </ChildContent>
    </FormItem>
}
```

> **Validation error styling**: Search the codebase for existing validation error CSS classes by running: search for `ant-form-item-explain` or `validation-error` or `error-message` in `.razor` and `.css` files. Use the exact class name you find. If you find nothing, use `ant-form-item-explain ant-form-item-explain-error` as shown above.

### 5.4 Reason Dropdown — New Field, Exception Mode Only

Add this `<FormItem>` **after** the NPI block, using the **exact same `<FormItem>` pattern** as existing fields:

```razor
@if (_currentSearchMode == SearchMode.ExceptionSearch)
{
    <FormItem>
        <LabelTemplate>
            <label for="exceptionReason">Reason (Required)</label>
        </LabelTemplate>
        <ChildContent>
            <Select @bind-Value="@_selectedReason"
                    Placeholder="Select a reason"
                    Id="exceptionReason"
                    TItem="string"
                    TItemValue="string">
                <SelectOptions>
                    <SelectOption TItem="string" TItemValue="string"
                                  Value="@("Practitioner does not have NPI")"
                                  Label="Practitioner does not have NPI" />
                </SelectOptions>
            </Select>
        </ChildContent>
    </FormItem>
}
```

> **This is a brand-new field** — it does NOT exist in the current form. It is NOT backed by a database lookup or API call. The dropdown has exactly **one hardcoded option**: `"Practitioner does not have NPI"`. Do NOT fetch options from any API.
>
> **Action**: Search the codebase for `<Select` or `<SelectHca`. If you find a custom `<SelectHca>` component, use that instead of `<Select>`. If you only find `<Select>`, use `<Select>` as shown above. Copy the exact syntax pattern from the existing usage you find.

### 5.5 Existing First Name Field — Wrap in Conditional

**Steps:**
1. Open `PractitionerSearchForm.razor`.
2. Find the existing First Name `<FormItem>` block (the entire block from `<FormItem>` to `</FormItem>`).
3. Wrap the entire block inside `@if (_currentSearchMode == SearchMode.ExceptionSearch) { ... }`.
4. Keep the existing `label for`, `Id`, `@bind-Value`, and `OnChange` values exactly as they are. Do NOT change them.
5. Add `Placeholder="Enter first name"` to the `<InputHca>` if it does not already have a Placeholder.
6. Keep the label text as `"First Name"` — this field is optional, do NOT add "(Required)".

**Example (replace `existingFirstNameId` and `_model.FirstName` with the ACTUAL values from the existing code):**

```razor
@if (_currentSearchMode == SearchMode.ExceptionSearch)
{
    @* This is the EXISTING First Name FormItem — only wrapped in @if, nothing else changed *@
    <FormItem>
        <LabelTemplate>
            <label for="ACTUAL_EXISTING_ID">First Name</label>
        </LabelTemplate>
        <ChildContent>
            <InputHca Id="ACTUAL_EXISTING_ID"
                      @bind-Value="@_model.ACTUAL_EXISTING_PROPERTY"
                      Placeholder="Enter first name"
                      OnChange="ACTUAL_EXISTING_HANDLER" />
        </ChildContent>
    </FormItem>
}
```

### 5.6 Existing Last Name Field — Wrap in Conditional, Update Label

**Steps:**
1. Open `PractitionerSearchForm.razor`.
2. Find the existing Last Name `<FormItem>` block.
3. Wrap the entire block inside `@if (_currentSearchMode == SearchMode.ExceptionSearch) { ... }`.
4. Change ONLY the label text from `"Last Name"` to `"Last Name (Required)"`. Do NOT change anything else.
5. Add `Placeholder="Enter last name"` to the `<InputHca>` if it does not already have a Placeholder.
6. Keep the existing `Id`, `@bind-Value`, and `OnChange` values exactly as they are.

**Example (replace `ACTUAL_EXISTING_*` with the ACTUAL values from the existing code):**

```razor
@if (_currentSearchMode == SearchMode.ExceptionSearch)
{
    @* This is the EXISTING Last Name FormItem — only label text changed + wrapped in @if *@
    <FormItem>
        <LabelTemplate>
            <label for="ACTUAL_EXISTING_ID">Last Name (Required)</label>
        </LabelTemplate>
        <ChildContent>
            <InputHca Id="ACTUAL_EXISTING_ID"
                      @bind-Value="@_model.ACTUAL_EXISTING_PROPERTY"
                      Placeholder="Enter last name"
                      OnChange="ACTUAL_EXISTING_HANDLER" />
        </ChildContent>
    </FormItem>
}
```

### 5.7 DOB Field — New Field, Exception Mode Only

Add a new `<FormItem>` for DOB using the **same pattern** as existing fields:

```razor
@if (_currentSearchMode == SearchMode.ExceptionSearch)
{
    <FormItem>
        <LabelTemplate>
            <label for="practitionerDob">DOB (Required)</label>
        </LabelTemplate>
        <ChildContent>
            <DatePicker @bind-Value="@_model.DateOfBirth"
                        Id="practitionerDob"
                        Placeholder="@("MM/DD/YYYY")"
                        Format="MM/dd/yyyy" />
        </ChildContent>
    </FormItem>
}
```

> **Action**: Search the codebase for `<DatePicker` or `<DatePickerHca`. If you find a custom `<DatePickerHca>` component, use that instead of `<DatePicker>`. Copy the exact syntax pattern from the existing usage you find. If no date picker exists in the codebase, use AntDesign's `<DatePicker>` as shown above.

### 5.8 Existing SSN Field — Wrap in Conditional, Update Label, Add Visibility Toggle

**Find the existing SSN `<FormItem>` and modify it:**

```razor
@if (_currentSearchMode == SearchMode.ExceptionSearch)
{
    <FormItem>
        <LabelTemplate>
            <label for="practitionerSsn">SSN (Optional)</label>  @* Add "(Optional)" to label *@
        </LabelTemplate>
        <ChildContent>
            <InputHca Id="practitionerSsn"
                      @bind-Value="@_model.PractitionerSsn"
                      Placeholder="***-**-****"
                      Type="@(_isSsnVisible ? "text" : "password")"
                      OnChange="OnFieldChanged">  @* Reuse existing OnFieldChanged handler *@
                <Suffix>
                    <Icon Type="@(_isSsnVisible ? "eye" : "eye-invisible")"
                          @onclick="() => { _isSsnVisible = !_isSsnVisible; StateHasChanged(); }" />
                </Suffix>
            </InputHca>
        </ChildContent>
    </FormItem>
}
```

> **Action**: Open the `InputHca` component source file. Check if it has a `<Suffix>` render fragment and a `Type` parameter. If `InputHca` does NOT support `<Suffix>`, then use AntDesign's `<Input>` component directly for this SSN field instead. If `InputHca` does NOT support `Type`, remove the `Type` attribute and handle masking a different way — search the codebase for how SSN is masked in other forms and copy that approach.

### 5.9 Existing Search Button — Add Disabled Logic + Exception Mode Cancel Button

**Find the existing Search button and modify:**

```razor
@if (_currentSearchMode == SearchMode.NpiSearch)
{
    @* NPI mode: single Search button, disabled until valid *@
    <FormItem>
        <Button Type="@ButtonType.Primary"
                HtmlType="submit"
                Disabled="@(!IsSearchEnabled())"
                Block>
            Search
        </Button>
    </FormItem>
}
else
{
    @* Exception mode: Search + Cancel buttons side by side *@
    <FormItem>
        <Space>
            <SpaceItem>
                <Button Type="@ButtonType.Primary"
                        HtmlType="submit"
                        Disabled="@(!IsSearchEnabled())">
                    Search
                </Button>
            </SpaceItem>
            <SpaceItem>
                <Button Danger
                        @onclick="SwitchToNpiSearch">
                    Cancel
                </Button>
            </SpaceItem>
        </Space>
    </FormItem>
}
```

> **Action**: Search the codebase for `<Space>`. If `<Space>` is used elsewhere, use it as shown above. If `<Space>` is NOT used anywhere in the codebase, place the two buttons in a simple `<div>` with the two buttons as children. The `Danger` property on `<Button>` gives the red/orange outlined Cancel button style.

### 5.10 Add "No NPI Available?" Link — NPI Mode Only

**Add after the Search button, inside the NPI mode conditional:**

```razor
@if (_currentSearchMode == SearchMode.NpiSearch)
{
    @* ... Search button above ... *@

    <FormItem>
        <div>
            <Icon Type="info-circle" Theme="fill" />
            <a href="javascript:void(0)" @onclick="SwitchToExceptionSearch">
                No NPI available? Search without NPI
            </a>
        </div>
    </FormItem>
}
```

> The `<a>` tag gets blue link styling from the application's existing CSS automatically. The `<Icon Type="info-circle">` is a built-in AntDesign icon. Do NOT add any `style=` attributes here.

### 5.11 Exception Validation Error — Reuse Existing Error Pattern

**Add inside the exception mode section, before the buttons:**

```razor
@if (_currentSearchMode == SearchMode.ExceptionSearch && _showExceptionValidationError)
{
    <FormItem>
        <span class="ant-form-item-explain ant-form-item-explain-error">
            <Icon Type="close-circle" Theme="fill" />
            Additional practitioner information is required.
        </span>
    </FormItem>
}
```

> **Action**: Search the codebase for `ant-form-item-explain`. If you find a different class name used for validation errors, use that class name instead. If `ant-form-item-explain` is what the app uses, keep it as shown.

---

## 6. Changes to Search Model Class

Find the existing search model class (likely in `Hca.Credentialing.Shared` or similar namespace). Verify the class name by checking what `_model` is typed as in `PractitionerSearchForm.razor.cs`.

### Keep All Existing Properties Unchanged

Do NOT modify, rename, or remove any existing properties. The existing properties (`PractitionerNpi`, `PractitionerSsn`, `FirstName`, `LastName`, etc.) must remain exactly as they are, including any existing `[Validation]` data annotations.

### Add New Properties — Only if They Don't Already Exist

> **FIRST**: Inspect the existing model class and check if any of these properties already exist (possibly under a different name). Search the model class file for `DateOfBirth`, `DOB`, `Dob`, `BirthDate`, `ExceptionReason`, `Reason`, `SearchType`, `Type`, etc.

**Only add properties that are NOT already present:**

```csharp
// Add to existing model class ONLY if not already present:
public DateTime? DateOfBirth { get; set; }   // Check for existing: DOB, Dob, BirthDate, DateOfBirth
public string ExceptionReason { get; set; }  // Check for existing: Reason, ReasonCode, ExceptionReason
public string SearchType { get; set; }       // Check for existing: SearchType, Type, SearchMode
```

> **If a property already exists under a different name** → Use the existing property name throughout the spec instead of adding a new one. Update all references in the form code-behind and markup to use the existing property name.

---

## 7. Changes to Backend / OnSearch Handler

### 7.1 PractitionerSearchPage.razor.cs — Update OnSearch

Find the existing `OnSearch` method in the page code-behind. It receives the model from the form callback. Update it to pass the `SearchType` to the backend service:

```csharp
private async Task OnSearch(PractitionerSearchModel model)  // use actual model type
{
    // The existing search logic should handle the new SearchType property
    // If SearchType == "NPI", search by NPI only (existing behavior)
    // If SearchType == "Exception", search by alternate fields (existing behavior, just routed differently)
    
    // Keep existing OnSearch logic — only add SearchType routing if needed
}
```

### 7.2 Audit Logging — Use Existing Logger

Use the existing `_logger` (already injected) for audit logging. Follow the same `_logger.LogInformation(...)` pattern used elsewhere in the application:

```csharp
_logger.LogInformation(
    "Exception Practitioner Search performed. Reason={Reason}, Criteria=[LastName={LastName}, DOB={DOB}]",
    _selectedReason,
    _model.LastName,
    _model.DateOfBirth?.ToString("MM/dd/yyyy"));
```

> **Action**: Search the codebase for `IAuditService`, `AuditLog`, or `audit`. If a dedicated audit service exists, use that for logging instead of `_logger`. If no audit service exists, use `_logger.LogInformation(...)` as shown above.

---

## 8. What NOT to Change

| Component/Area | Instruction |
|---|---|
| `PractitionerSearchTable.razor` | **NO CHANGES** — keep results table, columns, pagination, Cancel button, Add New Practitioner button all as-is |
| Page layout (`<Row>`, `<Col>` spans) | **NO CHANGES** — keep existing grid layout |
| `@attribute [Authorize]` | **NO CHANGES** — keep existing role authorization |
| Existing validation logic | **NO CHANGES** — all existing `[Validation]` attributes, `OnFieldChanged` handlers, and validation rules for SSN, First Name, Last Name remain unchanged |
| Duplicate practitioner detection | **NO CHANGES** — existing duplicate-check logic, matching, warnings, and backend services remain unchanged |
| Existing CSS / styling | **NO CHANGES** — do not add new CSS files or inline styles. Use existing classes and AntDesign component properties |
| Component library | **NO CHANGES** — do not install new packages. Use existing AntDesign Blazor and custom `Hca` wrapper components |
| `OnSearchCallback` parameter | **NO CHANGES** — keep existing `EventCallback` pattern |

---

## 9. Field Visibility by Mode — Summary Table

| Field | NPI Mode (Default) | Exception Mode | Notes |
|---|---|---|---|
| NPI (Required) | ✅ VISIBLE | ❌ HIDDEN | Existing field — update label, add placeholder, add validation error display |
| Reason (Required) | ❌ HIDDEN | ✅ VISIBLE | **NEW field** — `<Select>` dropdown |
| First Name | ❌ HIDDEN | ✅ VISIBLE | Existing field — keep as-is, wrap in `@if` |
| Last Name (Required) | ❌ HIDDEN | ✅ VISIBLE | Existing field — update label to add "(Required)", wrap in `@if` |
| DOB (Required) | ❌ HIDDEN | ✅ VISIBLE | **NEW field** — `<DatePicker>` |
| SSN (Optional) | ❌ HIDDEN | ✅ VISIBLE | Existing field — update label to add "(Optional)", add eye toggle, wrap in `@if` |
| Search button | ✅ VISIBLE (disabled until valid NPI) | ✅ VISIBLE (disabled until required fields filled) | Existing button — add `Disabled` logic |
| Cancel button | ❌ HIDDEN | ✅ VISIBLE | **NEW button** — returns to NPI mode |
| "No NPI?" link | ✅ VISIBLE | ❌ HIDDEN | **NEW element** — switches to exception mode |

---

## 10. Acceptance Criteria

| AC | Description | Story |
|---|---|---|
| AC1 | NPI is required primary search; only NPI field shown by default | US-1 |
| AC2 | Same NPI-first behavior across all PAF types | US-1 |
| AC3 | SSN, First Name, Last Name hidden in standard workflow (not deleted) | US-1 |
| AC4 | "No NPI available?" link shows exception form with Reason, First Name, Last Name, DOB, SSN | US-2 |
| AC5 | Reason is required for exception search | US-2 |
| AC6 | All existing validation rules for SSN, names, DOB remain unchanged | US-2 |
| AC7 | Existing validation error messages display correctly | US-2 |
| AC8 | Exception searches are audit-logged with User ID, timestamp, reason, criteria | US-2 |

---

## 11. Error Messages (Keep Existing — No Changes)

| Scenario | Message | Source |
|---|---|---|
| NPI invalid format | `"Please enter a valid 10-digit NPI."` | New validation display |
| NPI must start with 1 or 2 | `"NPI must start with a 1 or 2"` | Existing message — keep |
| NPI must be 10 digits | `"NPI must be 10-digits"` | Existing message — keep |
| No results | `"No Results Found"` / `"There are no matching Practitioners found..."` | Existing message — keep |
| System unavailable | `"Practitioner search service is currently unavailable"` | Existing message — keep |
| Timeout | `"Search request timed out. Please try again"` | Existing message — keep |
| Missing exception fields | `"Additional practitioner information is required."` | New validation display |
| Invalid DOB/SSN/name | Existing validation messages | Existing — keep unchanged |

---

## 12. Applicability — All PAF Types

This NPI-first search applies to ALL PAF workflows that use the Practitioner Search page:
- Recruitment PAF
- NPP PAF
- Practitioner Personal Info
- Add Practitioner to Facility
- Processing Change
- Off-Cycle RRFC
- Off-Cycle PSV
- Add Privilege MSS-18
- Add Collaborating Practitioner
- Any future PAF workflows

Since all PAF types route to the same `PractitionerSearchPage.razor` (route: `/Practitioner/Search`), modifying this single page and its components will enforce the change across all workflows.

---

## 13. Files to Modify — Final Checklist

| File | Action | What Changes |
|---|---|---|
| `PractitionerSearchForm.razor` | **MODIFY** | Wrap existing fields in `@if` conditionals, update labels, add Reason `<Select>`, DOB `<DatePicker>`, "No NPI?" link, SSN eye toggle, Cancel button, validation error displays |
| `PractitionerSearchForm.razor.cs` | **MODIFY** | Add `SearchMode` enum + state variables, `SwitchToExceptionSearch()`, `SwitchToNpiSearch()`, `IsSearchEnabled()`, `OnNpiFieldChanged()`, audit logging in `OnFinish` |
| `PractitionerSearchPage.razor` | **MODIFY** | Update `<PageDescription>` text only |
| Search Model class | **MODIFY** | Add `DateOfBirth`, `ExceptionReason`, `SearchType` properties (keep all existing properties) |
| `PractitionerSearchTable.razor` | **NO CHANGE** | — |
| `PractitionerSearchTable.razor.cs` | **NO CHANGE** | — |
| `PractitionerSearchPage.razor.cs` | **MINOR** | May need to handle `SearchType` in `OnSearch` method |

---

> **End of specification.** All changes reuse the existing AntDesign Blazor components (`<Card>`, `<Form>`, `<FormItem>`, `<InputHca>`, `<Button>`, `<Icon>`, `<Select>`, `<DatePicker>`), existing CSS classes (`ant-form-item-explain-error`), existing model binding patterns (`@bind-Value`), existing event patterns (`OnChange`, `OnFinish`, `OnSearchCallback`), and existing logger (`_logger`). No new component libraries, no inline styles, no custom CSS.
