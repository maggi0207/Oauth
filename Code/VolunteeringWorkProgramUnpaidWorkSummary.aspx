<%@ Page Language="C#" MasterPageFile="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" AutoEventWireup="True" CodeBehind="VolunteeringWorkProgramUnpaidWorkSummary.aspx.cs" Inherits="Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical.VolunteeringWorkProgramUnpaidWorkSummary" Title="Volunteering / Work Program / Unpaid Work Summary" %>

<%@ MasterType VirtualPath="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" %>

<asp:Content ID="ctPageBody" ContentPlaceHolderID="PageBodyContent" runat="server">

    <script type="text/javascript">
        function OnRedirect(index) {
            __doPostBack('__Page', index);
        }
    </script>
    <dhss:DataServiceLinqDataSource
        runat="server"
        ID="dsTechnical_VolunteeringWorkProgram"
        EnableUpdate="True"
        TableName="Technical_VolunteeringWorkProgram"
        EntityTypeName="Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Technical.Technical_VolunteeringWorkProgram"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.TechnicalContextImpl"
        OnSelecting="DsTechnical_VolunteeringWorkProgram_Selecting" />
    <table class="ContentTable">
        <tr>
            <td>
                <dx:ASPxLabel ID="lblVolunteeringWorkProgramSummary" runat="server" Text="Volunteering / Work Program / Unpaid Work Summary" SkinID="Header"></dx:ASPxLabel>
                <hr />
            </td>
        </tr>
        <tr>
            <td>
                <ul class="TopSectionCompactUL">
                    <li>
                        <dx:ASPxLabel ID="lblHistoryBegin" runat="server" AssociatedControlID="ddeBeginDate" Text="History Begin"></dx:ASPxLabel>
                        <dx:ASPxDateEdit ID="ddeBeginDate" ClientIDMode="Static" TabIndex="1" EditFormat="Custom" EditFormatString="MM/yyyy" DisplayFormatString="MM/yyyy" runat="server" ClientInstanceName="ddeBeginDate" IgnoreFgs="T" EnableViewState="true" Width="90px" SkinID="RetrieveClear">
                            <ClientSideEvents LostFocus="function(s,e) {HistoryDateLostFocus(s,'ddeBeginDate');}" DropDown="function(s,e){calenderClick(s,'ddeBeginDate');}" Init="function(s,e) {InitializeStartDate(s,'ddeBeginDate'); }" />
                        </dx:ASPxDateEdit>
                    </li>
                    <li>
                        <dx:ASPxLabel ID="lblHistoryEnd" runat="server" AssociatedControlID="ddeEndDate" Text="History End"></dx:ASPxLabel>
                        <dx:ASPxDateEdit ID="ddeEndDate" ClientIDMode="Static" TabIndex="2" EditFormatString="MM/yyyy" DisplayFormatString="MM/yyyy" EditFormat="Custom" runat="server" ClientInstanceName="ddeEndDate" IgnoreFgs="T" EnableViewState="true" Width="90px" SkinID="RetrieveClear">
                            <ClientSideEvents LostFocus="function(s,e) {HistoryDateLostFocus(s,'ddeEndDate');}" DropDown="function(s,e){calenderClick(s,'ddeEndDate');}" Init="function(s,e) {InitializeEndDate(s,'ddeEndDate');}" />
                        </dx:ASPxDateEdit>
                    </li>
                    <li class="TopSectionButtons">
                        <dx:ASPxButton ID="btnRetrieve" runat="server" TabIndex="3" Text="Retrieve" SkinID="HyperLinkStyleBtn" OnClick="BtnRetrieve_Click" IgnoreFgs="T">
                        </dx:ASPxButton>
                        <dx:ASPxLabel ID="lblSeperator" runat="server" Text="|" CssClass="SearchLabel" />
                        <dx:ASPxButton ID="btnClear" runat="server" TabIndex="4" Text="Clear" SkinID="HyperLinkStyleBtn" CausesValidation="false" OnClick="BtnClear_Click" IgnoreFgs="T"></dx:ASPxButton>
                    </li>
                </ul>
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <dx:ASPxGridView EnablePagingGestures="False" ID="gvASPxGridView" runat="server" DataSourceID="dsTechnical_VolunteeringWorkProgram"
                    KeyFieldName="VolunteeringWorkProgramID" EnableRowsCache="false" OnCustomCallback="GvASPxGridView_CustomCallback" OnDataBound="GvASPxGridView_DataBound">

                    <ClientSideEvents RowDblClick="function(s,e){OnRedirect(e.visibleIndex)}" />
                    <Columns>
                        <dx:GridViewDataColumn Caption="PersonId" FieldName="Person.PersonID" Width="10%" VisibleIndex="1" Visible="false"></dx:GridViewDataColumn>
                        <dx:GridViewDataTextColumn Caption="Name" FieldName="Person.FirstName" VisibleIndex="1" ReadOnly="true" Settings-AllowSort="True" Width="25%">
                            <DataItemTemplate>
                                <span><%# Eval("Person.FirstName") + " " + Eval("Person.MiddleName") + " " +  Eval("Person.LastName") %></span>
                            </DataItemTemplate>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="ProgramTypeCode" Caption="Type of Program" VisibleIndex="2" ReadOnly="true" Width="25%" Settings-AllowSort="True" Settings-SortMode="DisplayText" CellStyle-HorizontalAlign="Left">
                        </dx:GridViewDataTextColumn>

                        <dx:GridViewDataTextColumn Caption="Name of Program" VisibleIndex="3" FieldName="ProgramNameText" ReadOnly="true" Settings-AllowSort="True" Width="27%" CellStyle-HorizontalAlign="Left"></dx:GridViewDataTextColumn>

                        <dx:GridViewDataTextColumn Caption="Record Number" VisibleIndex="4" FieldName="SequenceNumber" ReadOnly="true" Settings-AllowSort="True" Width="13%" CellStyle-HorizontalAlign="Left"></dx:GridViewDataTextColumn>

                        <dx:GridViewDataDateColumn Caption="Updated Date" VisibleIndex="5" FieldName="UpdatedDateTime" ReadOnly="true" Settings-AllowSort="True" Width="10%" CellStyle-HorizontalAlign="Left">
                            <PropertiesDateEdit DisplayFormatString="MM/dd/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>

                        <dx:GridViewDataHyperLinkColumn VisibleIndex="6" FieldName="" Width="5%">
                            <PropertiesHyperLinkEdit Text="Link" Target="_blank"></PropertiesHyperLinkEdit>
                            <EditFormSettings Visible="False" />
                            <DataItemTemplate>
                                <dx:ASPxButton ID="btnShowDetails" runat="server" TabIndex="5" SkinID="Search" ToolTip="Show Details" Width="10px" OnClick="BtnShowDetails_Click"></dx:ASPxButton>
                            </DataItemTemplate>
                        </dx:GridViewDataHyperLinkColumn>
                    </Columns>
                    <Styles Header-Wrap="True"></Styles>
                    <SettingsBehavior AllowSelectSingleRowOnly="true" AllowFocusedRow="true" AllowSelectByRowClick="true" />
                </dx:ASPxGridView>
            </td>
        </tr>
        <tr>
            <td class="AddNewBtn">
                <dx:ASPxButton ID="btnAddNew" runat="server" Text="+ Add New" OnClick="BtnAddNew_Click" ToolTip="Add New Volunteering/Work Program/Unpaid Work" CausesValidation="false" AutoPostBack="true" ignorefgs="T" TabIndex="6" SkinID="HyperLinkStyleBtn"></dx:ASPxButton>
            </td>
        </tr>
    </table>
    <dx:ASPxPopupControl ID="dxPopupErr" ClientInstanceName="dxPopupErr" SkinID="ErrorPopUp" Modal="true" CloseAction="CloseButton" runat="server" ShowOnPageLoad="false"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" AllowDragging="true" Width="400px" ShowFooter="true" HeaderText="Error Title">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl2" runat="server">
                <dx:ASPxLabel ID="lblErrmessage" runat="server" />
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterTemplate>
            <div style="float: right; margin: 3px">
                <asp:Panel ID="PanelFocus" runat="server" DefaultButton="btnOk">
                    <dx:ASPxButton ID="btnOk" runat="server" Text="OK" ClientSideEvents-Click="function(s,e) {dxPopupErr.Hide();}" AutoPostBack="false" IgnoreFgs="T" SkinID="footerPrimary" CausesValidation="false" ClientInstanceName="btnOk"></dx:ASPxButton>
                </asp:Panel>
            </div>
        </FooterTemplate>
        <ClientSideEvents Shown="function(s, e) {btnOk.Focus(); return false;}" />
    </dx:ASPxPopupControl>
</asp:Content>
