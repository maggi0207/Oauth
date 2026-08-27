<%@ Page Language="C#" MasterPageFile="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" AutoEventWireup="True" CodeBehind="TaxDependencyInformationSummary.aspx.cs" Inherits="Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical.TaxDependencyInformationSummary" Title="Tax Dependency" %>

<%@ MasterType VirtualPath="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" %>
<asp:Content ID="ctPageBody" ContentPlaceHolderID="PageBodyContent" runat="server">
    <script type="text/javascript">
        //function initializeEndDate() {

        //    if (ddeStartDate.GetDate() != null) {
        //        ddeEndDate.SetMinDate(GetDateWithFirstLastDayOfMonth(ddeStartDate.GetDate(), true));
        //    }
        //}

        //function OnEndDateValidation(s, e) {
        //    var selectedDate = s.date;
        //    if (selectedDate == null || selectedDate == false)
        //        return;
        //    var startDate = GetDateWithFirstLastDayOfMonth(ddeStartDate.GetDate(), true); //forfirst day of month format ex: 01/05/0000
        //    selectedDate = GetDateWithFirstLastDayOfMonth(selectedDate, false); //for last day of month format ex: 31/05/0000

        //    if (startDate != null && selectedDate < startDate) {
        //        e.isValid = false;
        //        e.errorText = "History End Date can not be less than Begin Date.";
        //    }
        //}

        //function GetDateWithFirstLastDayOfMonth(date, IsFirstday) {

        //    if (date != null) {
        //        var month = date.getMonth() + 1; //jan seems to be 0
        //        var year = date.getFullYear();

        //        var lastday = LastDayOfMonth(year, month);

        //        if (IsFirstday)
        //            date = date.setDate(01);
        //        else
        //            date = date.setDate(lastday);
        //    }
        //    return date;
        //}


        //function LastDayOfMonth(Year, Month) {
        //    return (new Date((new Date(Year, Month, 1)) - 1)).getDate();
        //}


        // Double Click on selected row of Gridview
        function OnRedirect(index) {
            __doPostBack('__Page', index);
        }

    </script>
    <dhss:DataServiceLinqDataSource runat="server" ID="DsTaxDependancy"
        EnableUpdate="True"
        TableName="Technical_TaxDependency"
        EntityTypeName="Technical_TaxDependency"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.TechnicalContextImpl"
        OnSelecting="DsTaxDependancy_Selecting">
    </dhss:DataServiceLinqDataSource>
    <table class="ContentTable">
        <tr>
            <td>
                <dx:ASPxLabel ID="lblTaxDependencySummary" runat="server" Text="Tax Dependency Summary" SkinID="Header"></dx:ASPxLabel>
                <hr />
            </td>
        </tr>
        <tr>
            <td>
                <ul class="TopSectionCompactUL">
                    <li>
                        <dx:ASPxLabel ID="lblddeStartDate" runat="server" AssociatedControlID="ddeBeginDate" Text="History Begin"></dx:ASPxLabel>
                        <dx:ASPxDateEdit ID="ddeBeginDate" ClientIDMode="Static"  TabIndex="1" EditFormatString="MM/yyyy" DisplayFormatString="MM/yyyy" runat="server" EnableViewState="true" ClientInstanceName="ddeBeginDate" IgnoreFgs="T" SkinID="RetrieveClear">
                            <ClientSideEvents LostFocus="function(s,e) {HistoryDateLostFocus(s,'ddeBeginDate');}" DropDown="function(s,e){calenderClick(s,'ddeBeginDate');}" Init="function(s,e) {InitializeStartDate(s,'ddeBeginDate'); }" /> 
                        </dx:ASPxDateEdit>
                    </li>
                    <li>
                        <dx:ASPxLabel ID="lblddeEndDate" runat="server" AssociatedControlID="ddeEndDate" Text="History End"></dx:ASPxLabel>
                        <dx:ASPxDateEdit ID="ddeEndDate" ClientIDMode="Static"  TabIndex="2" EditFormatString="MM/yyyy" DisplayFormatString="MM/yyyy" runat="server" EnableViewState="true" ClientInstanceName="ddeEndDate" IgnoreFgs="T" SkinID="RetrieveClear">
                              <ClientSideEvents LostFocus="function(s,e) {HistoryDateLostFocus(s,'ddeEndDate');}" DropDown="function(s,e){calenderClick(s,'ddeEndDate');}" Init="function(s,e) {InitializeEndDate(s,'ddeEndDate');}" />
                        </dx:ASPxDateEdit>
                    </li>

                    <li class="TopSectionButtons">
                        <dx:ASPxButton ID="btnRetrieve"  TabIndex="3" runat="server" Text="Retrieve" SkinID="HyperLinkStyleBtn" OnClick="BtnRetrieve_Click" IgnoreFgs="T"></dx:ASPxButton>
                        <dx:ASPxLabel ID="lblSearch" runat="server" Text="|" CssClass="SearchLabel" />
                        <dx:ASPxButton ID="btnClear"  TabIndex="4" runat="server" Text="Clear" SkinID="HyperLinkStyleBtn" OnClick="BtnClear_Click" IgnoreFgs="T"></dx:ASPxButton>
                    </li>
                </ul>
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <br />
                <dx:ASPxGridView EnablePagingGestures="False" runat="server" ID="gvASPxGridView" OnCustomCallback="gvASPxGridView_CustomCallback" AutoGenerateColumns="false" KeyFieldName="TaxDependentID" Width="100%" DataSourceID="DsTaxDependancy"
                    ClientInstanceName="gvASPxGridView" OnDataBound="GvASPxGridView_DataBound" SettingsLoadingPanel-Mode="Disabled">
                    <ClientSideEvents RowDblClick="function(s,e){OnRedirect(e.visibleIndex)}" />
                    <Columns>
                        <dx:GridViewDataComboBoxColumn ReadOnly="true" Caption="Name" Settings-AllowSort="True" VisibleIndex="1" FieldName="ApplicationEntityID"></dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="FileTaxReturnInCurrentYearIndicator" Settings-AllowSort="True" Caption="Plan to file tax return for current year?" VisibleIndex="2"></dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="PrimaryTaxFilerIndicator" Settings-AllowSort="True" Caption="Are you a primary filer?" VisibleIndex="2"></dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="HasTaxDeductionIndicator" Settings-AllowSort="True" Caption="Do you have any tax deductions?" VisibleIndex="3"></dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataDateColumn Caption="Updated Date" ReadOnly="true" Settings-AllowSort="True" FieldName="DB2UpdatedDate" VisibleIndex="4"></dx:GridViewDataDateColumn>
                        <dx:GridViewDataColumn VisibleIndex="6" Visible="false" FieldName="SyncState"></dx:GridViewDataColumn>
                        <dx:GridViewDataHyperLinkColumn VisibleIndex="7" FieldName="">
                            <PropertiesHyperLinkEdit Text="Link" Target="_blank"></PropertiesHyperLinkEdit>
                            <EditFormSettings Visible="False" />
                            <DataItemTemplate>
                                <dx:ASPxButton ID="btnViewDetails"  TabIndex="5" ToolTip="Show Details" Width="10px" Height="10px" runat="server" SkinID="Search" OnClick="BtnViewDetails_Click" CommandArgument='<%# Eval("TaxDependentID")%>'></dx:ASPxButton>
                            </DataItemTemplate>
                        </dx:GridViewDataHyperLinkColumn>
                    </Columns>
                    <Styles Header-Wrap="True"></Styles>
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="True" ProcessSelectionChangedOnServer="true" />
                    <ClientSideEvents RowDblClick="function(s, e) { s.PerformCallback(s.GetRowKey(e.visibleIndex).toString());}" /> 
                </dx:ASPxGridView>
            </td>
        </tr>
    </table>
</asp:Content>