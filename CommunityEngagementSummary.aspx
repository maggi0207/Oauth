<%@ Page Title="Community Engagement Summary" Language="C#" MasterPageFile="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" AutoEventWireup="true" CodeBehind="CommunityEngagementSummary.aspx.cs" Inherits="Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical.CommunityEngagementSummary" %>

<%@ MasterType VirtualPath="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageBodyContent" runat="server">
    <script src='<%=ResolveClientUrl("~/Assets/js/dateValidation.js") %>'></script>
    <script type="text/javascript">
        function OnRedirect(index) {
            __doPostBack('__Page', index);
        }
    </script>
    <dhss:dataservicelinqdatasource runat="server" id="DsCommunityEngagementSummary"
        enableupdate="True"
        tablename="Technical_CommunityEngagementSummary"
        entitytypename="Technical_CommunityEngagementSummary"
        contexttypename="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.TechnicalContextImpl"
        onselecting="DsCommunityEngagementSummary_Selecting">
    </dhss:dataservicelinqdatasource>
    <table class="ContentTable">
        <tr>
            <td>
                <dx:aspxlabel id="lblCommunityEngagementSummary" runat="server" text="Community Engagement Summary" skinid="Header"></dx:aspxlabel>
                <hr />
            </td>
        </tr>
        <tr>
            <td>
                <ul class="TopSectionCompactUL">
                    <li>
                        <dx:aspxlabel id="lblddeStartDate" runat="server" associatedcontrolid="ddeBeginDate" text="History Begin"></dx:aspxlabel>
                        <dx:aspxdateedit id="ddeBeginDate" clientidmode="Static" tabindex="1" editformatstring="MM/yyyy" displayformatstring="MM/yyyy" runat="server" enableviewstate="true"
                            clientinstancename="ddeBeginDate" ignorefgs="T" skinid="RetrieveClear">
                            <clientsideevents lostfocus="function(s,e) {HistoryDateLostFocus(s,'ddeBeginDate');}" dropdown="function(s,e){calenderClick(s,'ddeBeginDate');}"
                                init="function(s,e) {InitializeStartDate(s,'ddeBeginDate'); }" />
                        </dx:aspxdateedit>
                    </li>
                    <li>
                        <dx:aspxlabel id="lblddeEndDate" runat="server" associatedcontrolid="ddeEndDate" text="History End"></dx:aspxlabel>
                        <dx:aspxdateedit id="ddeEndDate" clientidmode="Static" tabindex="2" editformatstring="MM/yyyy" displayformatstring="MM/yyyy" runat="server" enableviewstate="true" clientinstancename="ddeEndDate" ignorefgs="T" skinid="RetrieveClear">
                            <ClientSideEvents LostFocus="function(s,e) {HistoryDateLostFocus(s,'ddeEndDate');}" DropDown="function(s,e){calenderClick(s,'ddeEndDate');}" 
                                 Init="function(s,e) {InitializeEndDate(s,'ddeEndDate');}" />
                        </dx:aspxdateedit>
                    </li>

                    <li class="TopSectionButtons">
                        <dx:aspxbutton id="btnRetrieve" tabindex="3" runat="server" text="Retrieve" skinid="HyperLinkStyleBtn" onclick="BtnRetrieve_Click" ignorefgs="T"></dx:aspxbutton>
                        <dx:aspxlabel id="lblSearch" runat="server" text="|" cssclass="SearchLabel" />
                        <dx:aspxbutton id="btnClear" tabindex="4" runat="server" text="Clear" causesvalidation="false" skinid="HyperLinkStyleBtn" onclick="BtnClear_Click" ignorefgs="T"></dx:aspxbutton>
                    </li>
                </ul>
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <br />
                <dx:aspxgridview enablepaginggestures="False" runat="server" id="gvASPxGridView" oncustomcallback="gvASPxGridView_CustomCallback" autogeneratecolumns="false" keyfieldname="CommunityEngagementSummaryID" width="100%"
                    clientinstancename="gvASPxGridView" ondatabound="GvASPxGridView_DataBound" settingsloadingpanel-mode="Disabled" datasourceid="DsCommunityEngagementSummary">
                    <columns>
                        <dx:gridviewdatacomboboxcolumn caption="Name" fieldname="PersonID" readonly="true" settings-allowsort="True"
                            width="180px" visibleindex="1">
                        </dx:gridviewdatacomboboxcolumn>
                        <dx:gridviewdatadatecolumn caption="Begin Date" readonly="true" settings-allowsort="True" fieldname="EffectiveDate" visibleindex="2"></dx:gridviewdatadatecolumn>
                        <dx:gridviewdatacomboboxcolumn fieldname="CaretakerIndicator" readonly="true" settings-allowsort="True" caption="Caretaker?" visibleindex="3">
                            <dataitemtemplate>
                                <%#(Eval("CaretakerIndicator") == null ? "" : (Convert.ToBoolean(Eval("CaretakerIndicator")) ? "Yes" : "No")) %>
                            </dataitemtemplate>
                        </dx:gridviewdatacomboboxcolumn>
                        <dx:gridviewdatacomboboxcolumn fieldname="WorkProgramIndicator" readonly="true" settings-allowsort="True" caption="Work Program?" visibleindex="4">
                            <dataitemtemplate>
                                <%#(Eval("WorkProgramIndicator") == null ? "" : (Convert.ToBoolean(Eval("WorkProgramIndicator")) ? "Yes" : "No"))%>
                            </dataitemtemplate>
                        </dx:gridviewdatacomboboxcolumn>
                        <dx:gridviewdatacomboboxcolumn fieldname="UnpaidWorkIndicator" readonly="true" settings-allowsort="True" caption="Unpaid Work?" visibleindex="5">
                            <dataitemtemplate>
                                <%#(Eval("UnpaidWorkIndicator") == null ? "" : (Convert.ToBoolean(Eval("UnpaidWorkIndicator")) ? "Yes" : "No")) %>
                            </dataitemtemplate>
                        </dx:gridviewdatacomboboxcolumn>
                        <dx:gridviewdatahyperlinkcolumn visibleindex="7" fieldname="">
                            <propertieshyperlinkedit text="Link" target="_blank"></propertieshyperlinkedit>
                            <editformsettings visible="False" />
                            <dataitemtemplate>
                                <dx:aspxbutton id="btnViewDetails" tabindex="5" tooltip="Show Details" width="10px" height="10px" runat="server" skinid="Search" onclick="BtnViewDetails_Click" commandargument='<%# Eval("CommunityEngagementSummaryID")%>'></dx:aspxbutton>
                            </dataitemtemplate>
                        </dx:gridviewdatahyperlinkcolumn>
                    </columns>
                    <styles header-wrap="True"></styles>
                    <settingsbehavior allowselectbyrowclick="true" allowfocusedrow="True" processselectionchangedonserver="true" />
                    <clientsideevents rowdblclick="function(s, e) { s.PerformCallback(s.GetRowKey(e.visibleIndex).toString());}" />
                </dx:aspxgridview>
            </td>
        </tr>
    </table>
</asp:Content>


