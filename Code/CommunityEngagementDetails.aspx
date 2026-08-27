<%@ Page Title="Community Engagement Details" Language="C#" MasterPageFile="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" AutoEventWireup="true" CodeBehind="CommunityEngagementDetails.aspx.cs" Inherits=" Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical.CommunityEngagementDetails" %>

<%@ MasterType VirtualPath="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageBodyContent" runat="server">
    <script src='<%=ResolveClientUrl("~/Assets/js/dateValidation.js") %>'></script>
    <asp:HiddenField ID="HospitalizedBeginDateHidden" ClientIDMode="Static" EnableViewState="true" runat="server"></asp:HiddenField>
    <asp:HiddenField ID="TravelOutOfAreaMedicalBeginDateHidden" ClientIDMode="Static" EnableViewState="true" runat="server"></asp:HiddenField>
    <asp:HiddenField ID="DisasterDeclarationBeginDateHidden" ClientIDMode="Static" EnableViewState="true" runat="server"></asp:HiddenField>
    <asp:HiddenField ID="UnemploymentLevelBeginDateHidden" ClientIDMode="Static" EnableViewState="true" runat="server"></asp:HiddenField>
    <style type="text/css">
        .mylengthyLabelTD label, .mylengthyLabelTD span {
            float: left;
            text-align: right;
            width: 200px;
        }

        .verifiedBy label, .VerifiedBy span {
            float: left;
            text-align: right;
            width: 80px;
        }

        .tooltip-container {
            position: relative;
            display: inline-block;
            width: 20px;
        }

        .icon {
            font-size: 8px;
            cursor: help;
        }

        .tooltip-text {
            display: none;
            position: absolute;
            bottom: 125%;
            left: 20%;
            color: #201f35 !important;
            width: 300px;
            justify-content: left;
            align-items: left;
            height: auto;
            transform: translateX(-50%);
            font: 11px Verdana;
            background-color: white;
            border: 1px solid rgb(157, 160, 170);
            border: 1px solid rgba(0, 0, 0, 0.35);
            padding: 6px 10px 6px 10px;
            text-align: left;
            cursor: default;
            -moz-box-shadow: 0px 2px 10px rgba(0, 0, 0, 0.35);
            -webkit-box-shadow: 0px 2px 10px rgba(0, 0, 0, 0.35);
            box-shadow: 0px 2px 10px rgba(0, 0, 0, 0.35);
        }

        .tooltip-container:hover .tooltip-text {
            display: block;
            border-radius: 5px;
            display: flex;
            justify-content: left;
            align-items: left;
            color: #fff;
            font-size: 24px;
        }

        .tooltip {
            background-image: url(../../../images/infoIcon.png) !important;
            color: #00529b !important;
            height: 23px;
            width: 25px !important;
            transform: scale(0.5);
        }
    </style>
    <script src='<%= ResolveUrl(Dhss.Assist.WorkerWeb.Web.Infrastructure.Helpers.JsVersioningHelper.Tag("~/Intake/ApplicationEntry/Technical/CommunityEngagementDetails.js")) %>'></script>
    <dhss:DataServiceLinqDataSource runat="server"
        ID="DsTechnical_CommunityEngagementDetails"
        EnableUpdate="True"
        TableName="Technical_CommunityEngagement" OnSelecting="DsTechnical_CommunityEngagementDetails_Selecting"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.TechnicalContextImpl">
    </dhss:DataServiceLinqDataSource>
    <asp:FormView ID="fvTechnical_CommunityEngagement" runat="server" DefaultMode="Edit" DataKeyNames="CommunityEngagementID" DataSourceID="DsTechnical_CommunityEngagementDetails" 
        OnDataBound="FvTechnical_CommunityEngagement_DataBound" OnItemUpdating="FvTechnical_CommunityEngagement_ItemUpdating">
        <EditItemTemplate>
            <table class="ContentTable">
                <tr>
                    <td>
                        <dx:ASPxLabel ID="lblCommunityEngagementDetails" runat="server" Text="Community Engagement Details" SkinID="Header"></dx:ASPxLabel>
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td>
                        <dx:ASPxButton runat="server" ID="btnBackToSummary" SkinID="HyperLinkStyleBtn" OnClick="BtnBackToSummary_Click"
                            EncodeHtml="false" CausesValidation="false" IgnoreFgs="T" Text="< Back to Summary" />
                        <br />
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTableThreeColumns">
                            <asp:HiddenField runat="server" ID="hfHistoryCode" Value='<%# Bind("HistoryCode") %>' />
                            <asp:HiddenField runat="server" ID="hfDeleteReasonCode" Value='<%# Bind("DeleteReasonCode") %>' />
                            <tr>
                                <td>
                                    <dx:ASPxLabel ID="lblSequenceNumber" runat="server" Text="Record Number" Enabled="false" AssociatedControlID="lblSequenceNumber1"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxLabel ID="lblSequenceNumber1" runat="server" Text='<%# Eval("SequenceNumber") %>' SkinID="LeftLabel"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxLabel ID="lblHistorySequenceNumber" runat="server" Text="Record History Number" Enabled="false" AssociatedControlID="lblHistorySequenceNumber1"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxLabel ID="lblHistorySequenceNumber1" runat="server" Text='<%# Eval("HistorySequenceNumber") %>' SkinID="LeftLabel"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxLabel ID="lblRecordUpdatedDate" runat="server" Text="Record Updated Date" Enabled="false" AssociatedControlID="lblRecordUpdatedDate1"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxLabel ID="lblRecordUpdatedDate1" runat="server" Text='<%# Eval("UpdatedDateTime","{0:MM/dd/yyyy}") %>' SkinID="LeftLabel"></dx:ASPxLabel>
                                </td>
                            </tr>
                        </table>
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr class="spaceUnder">
                                <td style="text-align: left">
                                    <dx:ASPxLabel ID="lblName" runat="server" Text="Name" Enabled="false" AssociatedControlID="lblNameValue"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxLabel ID="lblNameValue" runat="server" SkinID="LeftLabel"></dx:ASPxLabel>
                                </td>
                            </tr>
                            <tr class="spaceUnder">
                                <td style="text-align: left">
                                    <dx:ASPxLabel ID="lblBeginDate" AssociatedControlID="ddeCEBeginDate" runat="server" Text="Begin Date" EncodeHtml="false" Width="65px" />
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="ddeCEBeginDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server" Value='<%# Bind("BeginDate") %>' 
                                        ClientInstanceName="ddeCEBeginDate" 
                                        AutoPostBack="false" SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents 
                                            LostFocus="function(s,e) {var x = true; x = DateLostFocus(s,'ddeCEBeginDate', false); e.processOnServer = x; checkLegacyBeginYear(s); }" 
                                            DropDown="function(s,e){ calenderClick(s,'ddeCEBeginDate'); }" 
                                            Init="function(s,e) { InitializeStartDate(s,'ddeCEBeginDate'); }" 
                                            Validation="function(s,e) { validateBeginMonthRules(s, e); }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblRegularlyTakeCareOfDependent" CssClass="required" AssociatedControlID="cbRegularlyTakeCareOfDependent"
                                                    runat="server" Text="Do you or did you regularly take care of a dependent child who is 13 or younger, or a person with a physical or mental disability that makes everyday activities difficult?"
                                                    EncodeHtml="false" ClientInstanceName="lblRegularlyTakeCareOfDependent"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>

                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" ID="cbRegularlyTakeCareOfDependent" name="cbRegularlyTakeCareOfDependent" runat="server" 
                                        ValueType="System.Boolean" Value='<%# Bind("RegularTakecareIndicator") %>' AutoPostBack="false" 
                                        OnSelectedIndexChanged="CbRegularlyTakeCareOfDependent_SelectedIndexChanged" ClientInstanceName="cbRegularlyTakCareOfDependent" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { RegularTakecareIndicator(s); }" Init="function(s,e) { RegularTakecareIndicator(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trWho" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblWho" AssociatedControlID="cbWho" runat="server" Text="Who?" EncodeHtml="false" 
                                                    ClientInstanceName="lblWho"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" ID="cbWho" runat="server" ValueType="System.Int32"
                                        Value='<%# Bind("CareTakerPersonID") %>' AutoPostBack="false" OnSelectedIndexChanged="CbWho_SelectedIndexChanged" 
                                        ClientInstanceName="cbWho" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { fncWho(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trParentOrLegalGuardian" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblParentOrLegalGuardian" CssClass="required" AssociatedControlID="cbParentOrLegalGuardian" 
                                                    runat="server" Text="Are you the parent or legal guardian of the person that is or was being cared for?" 
                                                    EncodeHtml="false" ClientInstanceName="lblParentOrLegalGuardian"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" ID="cbParentOrLegalGuardian" runat="server" ValueType="System.Boolean" 
                                        Value='<%# Bind("ParentOrLegalGuardianIndicator") %>' AutoPostBack="false" OnSelectedIndexChanged="CbParentOrLegalGuardian_SelectedIndexChanged" 
                                        ClientInstanceName="cbParentOrLegalGuardian" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { fncParentOrLegalGuardianChange(s); }" 
                                            Init="function(s,e) { fncParentOrLegalGuardianChange(); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trProvideCare" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblWhenLegalGuardianProvideCare" CssClass="required" AssociatedControlID="cbWhenLegalGuardianProvideCare"
                                                    ClientInstanceName="lblWhenLegalGuardianProvideCare" runat="server" Text="When did you provide care?" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbWhenLegalGuardianProvideCare" 
                                        ClientInstanceName="cbWhenLegalGuardianProvideCare" runat="server" ValueType="System.String" Value='<%# Bind("WhenLegalGuardianProvideCareCode") %>'
                                        TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { fncWhenLegalGuardianProvideCareChange(s); }" 
                                            Init="function(s,e) { fncWhenLegalGuardianProvideCareChange(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trStopProvidingCareDate" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblStopLegalGuardianProvideCare" CssClass="required" AssociatedControlID="dateStopLegalGuardianProvideCare"
                                                    runat="server" Text="When did you stop providing care?" EncodeHtml="false" ClientInstanceName="lblStopLegalGuardianProvideCare"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dateStopLegalGuardianProvideCare" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" 
                                        runat="server" Value='<%# Bind("StopLegalGuardianProvideCareDate") %>' ClientInstanceName="ddeStopLegalGuardianProvideCare" 
                                        AutoPostBack="false" SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents DropDown="function(s,e){calenderClick(s,'ddeStopLegalGuardianProvideCare');}" 
                                            Init="function(s,e) {InitializeStartDate(s,'ddeStopLegalGuardianProvideCare') }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trCareTakerRelationship">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblCareTakerRelationship" CssClass="required" AssociatedControlID="cbCareTakerRelationship" 
                                                    ClientInstanceName="lblCareTakerRelationship" runat="server" 
                                                    Text="What is your relationship with the person that is or was being cared for?" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbCareTakerRelationship" ClientInstanceName="cbCareTakerRelationship" 
                                        runat="server" ValueType="System.String" Value='<%# Bind("CareTakerRelationshipCode") %>' TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { fncCareTakerRelationshipChange(s); }" 
                                            Init="function(s,e) { fncCareTakerRelationshipChange(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trProvideCare1" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblWhenCareTakerRelationship" CssClass="required" AssociatedControlID="cbWhenCareTakerRelationship"
                                                    ClientInstanceName="lblWhenCareTakerRelationship" runat="server" Text="When did you provide care?" 
                                                    EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbWhenCareTakerRelationship" 
                                        ClientInstanceName="cbWhenCareTakerRelationship" runat="server" ValueType="System.String" Value='<%# Bind("WhenCareTakerRelationshipCode") %>' 
                                        TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { fncWhenCareTakerRelationshipChange(s); }" 
                                            Init="function(s,e) { fncWhenCareTakerRelationshipChange(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trStopProvidingCareDate2" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblStopProvidingCareDateTime" CssClass="required" AssociatedControlID="dateStopProvidingCareDateTime" 
                                                    runat="server" Text="When did you stop providing care?" EncodeHtml="false" ClientInstanceName="lblStopProvidingCareDateTime"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dateStopProvidingCareDateTime" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server" 
                                        ClientInstanceName="ddeStopProvidingCareDateTime" Value='<%# Bind("StopProvidingCareDate") %>' AutoPostBack="false" SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents DropDown="function(s,e){calenderClick(s,'ddeStopProvidingCareDateTime');}" 
                                            Init="function(s,e) {InitializeStartDate(s,'ddeStopProvidingCareDateTime') }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trLiveWithPersonBeingCaredFor">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblLiveWithPersonBeingCaredFor" CssClass="required" AssociatedControlID="cbLiveWithPersonBeingCaredFor"
                                                    ClientInstanceName="lblLiveWithPersonBeingCaredFor" runat="server" Text="Do you or did you live with the person while giving care?" 
                                                    EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>

                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbLiveWithPersonBeingCaredFor" 
                                        ClientInstanceName="cbLiveWithPersonBeingCaredFor" runat="server" ValueType="System.String" Value='<%# Bind("LiveWithPersonBeingCaredForCode") %>' 
                                        TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { fncLiveWithPersonBeingCaredForChange(s); }" 
                                            Init="function(s,e) { fncLiveWithPersonBeingCaredForChange(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trStopLivingwithPersonDate" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblStopLivingwithPersonDate" CssClass="required" AssociatedControlID="dateStopLivingwithPersonDate" 
                                                    runat="server" Text="When did you stop living with the person?" EncodeHtml="false" 
                                                    ClientInstanceName="lblStopLivingwithPersonDate"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dateStopLivingwithPersonDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server" 
                                        Value='<%# Bind("StopLivingwithPersonDate") %>' 
                                        ClientInstanceName="ddeStopLivingwithPersonDate" AutoPostBack="false" SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents DropDown="function(s,e){calenderClick(s,'ddeStopLivingwithPersonDate');}" 
                                            Init="function(s,e) {InitializeStartDate(s,'ddeStopLivingwithPersonDate') }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trStopProvidingCareDateMain">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblStopTakingCareDate" CssClass="required" AssociatedControlID="dateStopTakingCareDate" 
                                                    runat="server" Text="When did you stop taking care of the person?" EncodeHtml="false" 
                                                    ClientInstanceName="lblStopTakingCareDate"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dateStopTakingCareDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server" 
                                        ClientInstanceName="ddeStopTakingCareDate" Value='<%# Bind("StopTakingCareDate") %>' AutoPostBack="false" 
                                        SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents DropDown="function(s,e){calenderClick(s,'ddeStopTakingCareDate');}" 
                                            Init="function(s,e) {InitializeStartDate(s,'ddeStopTakingCareDate') }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trStopLivingWithPersonWhileGivingCareDate" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblStopLivingWithPersonWhileGivingCareDate" CssClass="required" 
                                                    AssociatedControlID="dateStopLivingWithPersonWhileGivingCareDate" runat="server" 
                                                    Text="When did you stop living with the person?" EncodeHtml="false" 
                                                    ClientInstanceName="lblStopLivingWithPersonWhileGivingCareDate"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dateStopLivingWithPersonWhileGivingCareDate" ClientIDMode="Static"
                                        EditFormatString="MM/dd/yyyy" runat="server" ClientInstanceName="ddeStopLivingWithPersonWhileGivingCareDate"
                                        Value='<%# Bind("StopLivingWithPersonWhileGivingCareDate") %>' AutoPostBack="false" SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents DropDown="function(s,e){calenderClick(s,'dateStopLivingWithPersonWhileGivingCareDate');}" 
                                            Init="function(s,e) {InitializeStartDate(s,'dateStopLivingWithPersonWhileGivingCareDate') }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trStopTakingCarePersonDate" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblStopTakingCarePersonDate" CssClass="required" AssociatedControlID="dateStopTakingCarePersonDate" 
                                                    runat="server" Text="When did you stop taking care of the person?" EncodeHtml="false" 
                                                    ClientInstanceName="lblStopTakingCarePersonDate"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dateStopTakingCarePersonDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" 
                                        runat="server" ClientInstanceName="dateStopTakingCarePersonDate" Value='<%# Bind("StopTakingCarePersonDate") %>' 
                                        AutoPostBack="false" SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents DropDown="function(s,e){calenderClick(s,'dateStopTakingCarePersonDate');}"
                                            Init="function(s,e) {InitializeStartDate(s,'dateStopTakingCarePersonDate') }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trReceivedProvidingCare">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblReceivedProvidingCare" CssClass="required" AssociatedControlID="cbReceivedProvidingCare" 
                                                    ClientInstanceName="lblReceivedProvidingCare" runat="server" 
                                                    Text="Do you or did you get paid or receive something in exchange for providing care?" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbReceivedProvidingCare" 
                                        ClientInstanceName="cbReceivedProvidingCare" runat="server" ValueType="System.String" Value='<%# Bind("ReceivedProvidingCareCode") %>' 
                                        TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { fncReceivedProvidingCareChange(s); }"
                                            Init="function(s,e) { fncReceivedProvidingCareChange(s); }" />
                                    </dx:ASPxComboBox>
                                    <div id="divReceivedProvidingCareHelp" class="notice" style="display: none; text-align: left;"></div>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblReceivedProvidingCareVerifiedBy" AssociatedControlID="cbReceivedProvidingCareVerifiedBy" 
                                                    ClientInstanceName="lblReceivedProvidingCareVerifiedBy" runat="server" Text="Verified By" 
                                                    EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbReceivedProvidingCareVerifiedBy" 
                                        ClientInstanceName="cbReceivedProvidingCareVerifiedBy" runat="server" ValueType="System.String"
                                        Value='<%# Bind("ReceivedProvidingCareVerifiedByCode") %>' TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblCorrectionalInLast12Months" CssClass="required" AssociatedControlID="cbCorrectionalInLast12Months" 
                                                    ClientInstanceName="lblCorrectionalInLast12Months" runat="server" 
                                                    Text="Were you released from a correctional facility or incarcerated in the last 12 months?" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbCorrectionalInLast12Months"
                                        ClientInstanceName="cbCorrectionalInLast12Months" runat="server" ValueType="System.Boolean" 
                                        Value='<%# Bind("CorrectionalInLast12MonthsIndicator") %>' TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { fncCorrectionalInLast12MonthsChange(s); }"
                                            Init="function(s,e) { fncCorrectionalInLast12MonthsChange(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblCorrectionalInLast12MonthsVerifiedBy" AssociatedControlID="cbCorrectionalInLast12MonthsVerifiedBy" 
                                                    ClientInstanceName="lblCorrectionalInLast12MonthsVerifiedBy" runat="server" Text="Verified By" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbCorrectionalInLast12MonthsVerifiedBy" 
                                        ClientInstanceName="cbCorrectionalInLast12MonthsVerifiedBy" runat="server" ValueType="System.String" 
                                        Value='<%# Bind("CorrectionalInLast12MonthsVerifiedByCode") %>' TabIndex="0"></dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trReleasedDate" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblCorrectionalReleasedDate" CssClass="required" AssociatedControlID="dateCorrectionalReleasedDate" 
                                                    runat="server" Text="When were you released?" EncodeHtml="false" ClientInstanceName="lblCorrectionalReleasedDate"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dateCorrectionalReleasedDate" ClientIDMode="Static" EditFormat="Custom" EditFormatString="MM/dd/yyyy" 
                                        DisplayFormatString="MM/dd/yyyy" runat="server" 
                                        ClientInstanceName="ddeCorrectionalReleasedDates" Value='<%# Bind("CorrectionalReleasedDate") %>' AutoPostBack="false" 
                                        SkinID="RetrieveClear" TabIndex="0">
                                        <ClientSideEvents DropDown="function(s,e){ calenderClick(s,'dateCorrectionalReleasedDate'); }"
                                            Init="function(s,e) { InitializeStartDate(s,'dateCorrectionalReleasedDate'); }"
                                            Validation="function(s,e) { validateWithinPast12Months(s,e); }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblParticipatingInWorkProgram" CssClass="required" AssociatedControlID="cbParticipatingInWorkProgram" 
                                                    runat="server" Text="Are you currently or were you participating in a Work Program?" EncodeHtml="false" 
                                                    ClientInstanceName="lblParticipatingInWorkProgram"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" ID="cbParticipatingInWorkProgram" runat="server" ValueType="System.Boolean" 
                                        Value='<%# Bind("ParticipatingInWorkProgramIndicator") %>' AutoPostBack="false" ClientInstanceName="cbParticipatingInWorkProgram" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { ParticipatingInWorkProgramIndicator(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblParticipatingInUnpaidWork" CssClass="required" AssociatedControlID="cbParticipatingInUnpaidWork" 
                                                    runat="server" Text="Are you currently or were you Volunteering or participating in Unpaid Work?" EncodeHtml="false" 
                                                    ClientInstanceName="lblParticipatingInUnpaidWork"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" ID="cbParticipatingInUnpaidWork" runat="server" ValueType="System.Boolean" 
                                        Value='<%# Bind("ParticipatingInUnpaidWorkIndicator") %>' AutoPostBack="false" ClientInstanceName="cbParticipatingInUnpaidWork" 
                                        TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { ParticipatingInUnpaidWorkIndicator(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </EditItemTemplate>
    </asp:FormView>
    <%--------------------------------------------------------MEDICAL DETAILS-----------------------------------------------------------------%>
    <br />
    <br clear="left" />
    <dhss:DataServiceLinqDataSource runat="server"
        ID="DsTechnical_CommunityEngagementMedicalDetails"
        EnableUpdate="True"
        TableName="Technical_CommunityEngagementMedicalDetails" OnSelecting="DsTechnical_CommunityEngagementMedicalDetails_Selecting"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.TechnicalContextImpl">
    </dhss:DataServiceLinqDataSource>
    <asp:FormView ID="fvTechnical_CommunityEngagementMedicalDetails" runat="server" DefaultMode="Edit" DataKeyNames="CommunityEngagementMedicalDetailsID" 
        DataSourceID="DsTechnical_CommunityEngagementMedicalDetails" OnDataBound="FvTechnical_CommunityEngagementMedicalDetails_DataBound" 
        OnItemUpdating="FvTechnical_CommunityEngagementMedicalDetails_ItemUpdating">
        <EditItemTemplate>
            <table class="ContentTable">
                <tr>
                    <td style="height: 20px;"></td>
                </tr>
                <tr>
                    <td>
                        <dx:ASPxLabel ID="lblCommunityEngagementMedicalDetails" runat="server" Text="Medical Details" SkinID="Header"></dx:ASPxLabel>
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblSeriousMedicalCondition" CssClass="required" AssociatedControlID="cbSeriousMedicalCondition"
                                                    ClientInstanceName="lblSeriousMedicalConditionIndicator" runat="server" 
                                                    Text="Do you or did you have a serious or complex medical condition that makes it hard to complete activities such as working, schooling or volunteering?" 
                                                    EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbSeriousMedicalCondition" 
                                        ClientInstanceName="cbSeriousMedicalConditionIndicator" runat="server" ValueType="System.Boolean" 
                                        Value='<%# Bind("SeriousMedicalConditionIndicator") %>' TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { fncSeriousMedicalConditionChange(s); }" 
                                            Init="function(s,e) { fncSeriousMedicalConditionChange(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblSeriousMedicalConditionVerifiedBy" AssociatedControlID="cbSeriousMedicalConditionVerifiedBy" 
                                                    ClientInstanceName="lblSeriousMedicalConditionVerifiedByCode" runat="server" Text="Verified By" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbSeriousMedicalConditionVerifiedBy" 
                                        ClientInstanceName="cbSeriousMedicalConditionVerifiedByCode" runat="server" ValueType="System.String" 
                                        Value='<%# Bind("SeriousMedicalConditionVerifiedByCode") %>' TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trSeriousMedicalconditionStatus" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblSeriousMedicalconditionStatus" CssClass="required" AssociatedControlID="cbSeriousMedicalconditionStatus" 
                                                    runat="server" Text="When did you have this condition?" EncodeHtml="false" ClientInstanceName="lblSeriousMedicalconditionStatusCode"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" ID="cbSeriousMedicalconditionStatus" runat="server" ValueType="System.String" 
                                        Value='<%# Bind("SeriousMedicalconditionStatusCode") %>' AutoPostBack="false" 
                                        OnSelectedIndexChanged="CbSeriousMedicalconditionStatus_SelectedIndexChanged" ClientInstanceName="cbSeriousMedicalconditionStatusCode" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { SeriousMedicalconditionStatusCode(s); }" 
                                            Init="function(s,e) { SeriousMedicalconditionStatusCode(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trEndSeriousConditionDate" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblEndSeriousConditionDate" CssClass="required" AssociatedControlID="dateEndSeriousConditionDate" 
                                                    runat="server" Text="End Date of this condition" EncodeHtml="false" ClientInstanceName="lblEndSeriousConditionDate"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dateEndSeriousConditionDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server" 
                                        ClientInstanceName="ddeEndSeriousConditionDate" Value='<%# Bind("EndSeriousConditionDate") %>' AutoPostBack="false" 
                                        SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents DropDown="function(s,e){calenderClick(s,'dateEndSeriousConditionDate');}" 
                                            Init="function(s,e) {InitializeStartDate(s,'dateEndSeriousConditionDate') }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblSubstanceUseDisorder" CssClass="required" AssociatedControlID="cbSubstanceUseDisorder" 
                                                    ClientInstanceName="lblSubstanceUseDisorderIndicator" runat="server" Text="Do you or did you have substance use disorder?" 
                                                    EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbSubstanceUseDisorder" 
                                        ClientInstanceName="cbSubstanceUseDisorderIndicator" runat="server" ValueType="System.Boolean" Value='<%# Bind("SubstanceUseDisorderIndicator") %>' 
                                        TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { fncSubstanceUseDisorderChange(s); }" 
                                            Init="function(s,e) { fncSubstanceUseDisorderChange(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblSubstanceUseDisorderVerifiedBy" AssociatedControlID="cbSubstanceUseDisorderVerifiedBy" 
                                                    ClientInstanceName="lblSubstanceUseDisorderVerifiedBy" runat="server" Text="Verified By" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbSubstanceUseDisorderVerifiedBy" 
                                        ClientInstanceName="cbSubstanceUseDisorderVerifiedBy" runat="server" ValueType="System.String" 
                                        Value='<%# Bind("SubstanceUseDisorderVerifiedByCode") %>' 
                                        TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trSubstanceUseDisorderStatus" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblSubstanceUseDisorderStatus" CssClass="required" AssociatedControlID="cbSubstanceUseDisorderStatus" 
                                                    runat="server" Text="When did you have this condition?" EncodeHtml="false" 
                                                    ClientInstanceName="lblSubstanceUseDisorderStatusCode"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" ID="cbSubstanceUseDisorderStatus" runat="server" ValueType="System.String" 
                                        Value='<%# Bind("SubstanceUseDisorderStatusCode") %>' AutoPostBack="false" 
                                        OnSelectedIndexChanged="CbSubstanceUseDisorderStatus_SelectedIndexChanged" ClientInstanceName="cbSubstanceUseDisorderStatusCode" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { SubstanceUseDisorderStatusCode(s); }" 
                                            Init="function(s, e) { SubstanceUseDisorderStatusCode(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trEndSubstanceDisorderDate" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblEndSubstanceDisorderDate" CssClass="required" AssociatedControlID="dateEndSubstanceDisorderDate" 
                                                    runat="server" Text="End Date of this condition" EncodeHtml="false" 
                                                    ClientInstanceName="lblEndSubstanceDisorderDate"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dateEndSubstanceDisorderDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server" 
                                        Value='<%# Bind("EndSubstanceDisorderDate") %>' ClientInstanceName="ddeEndSubstanceDisorderDate" AutoPostBack="false"
                                        SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents DropDown="function(s,e){calenderClick(s,'ddeEndSubstanceDisorderDate');}" 
                                            Init="function(s,e) {InitializeStartDate(s,'ddeEndSubstanceDisorderDate') }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblDisabledBySSA" CssClass="required" AssociatedControlID="cbDisabledBySSA" 
                                                    ClientInstanceName="lblDisabledBySSAIndicator" 
                                                    runat="server" Text="Are you or were you determined disabled by the Social Security Administration (SSA)?"
                                                    EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbDisabledBySSA" 
                                        ClientInstanceName="cbDisabledBySSAIndicator" runat="server" ValueType="System.Boolean" Value='<%# Bind("DisabledBySSAIndicator") %>' TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { fncDisabledBySSAChange(s); }" 
                                            Init="function(s,e) { fncDisabledBySSAChange(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblDisabledBySSAVerifiedBy" AssociatedControlID="cbDisabledBySSAVerifiedBy" 
                                                    ClientInstanceName="lblDisabledBySSAVerifiedByCode" runat="server" Text="Verified By" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbDisabledBySSAVerifiedBy" 
                                        ClientInstanceName="cbDisabledBySSAVerifiedByCode" runat="server" ValueType="System.String" Value='<%# Bind("DisabledBySSAVerifiedByCode") %>' 
                                        TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trWhenDetermined" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblWhenDetermined" CssClass="required" AssociatedControlID="cbWhenDetermined" 
                                                    runat="server" Text="When were you determined?" EncodeHtml="false" ClientInstanceName="lblWhenDetermined"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" ID="cbWhenDetermined" runat="server" ValueType="System.String"
                                        Value='<%# Bind("WhenDeterminedCode") %>' AutoPostBack="false" OnSelectedIndexChanged="CbWhenDetermined_SelectedIndexChanged" ClientInstanceName="cbWhenDetermined" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { WhenDetermined(s); }" 
                                            Init="function(s, e) { WhenDetermined(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trEndSSADeterminationDate" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblEndSSADeterminationDate" CssClass="required" AssociatedControlID="dateEndSSADeterminationDate"
                                                    runat="server" Text="End Date of SSA determination" EncodeHtml="false" ClientInstanceName="lblEndSSADeterminationDate"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dateEndSSADeterminationDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" 
                                        runat="server" ClientInstanceName="ddeEndSSADeterminationDate" Value='<%# Bind("EndSSADeterminationDate") %>' 
                                        AutoPostBack="false" SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents DropDown="function(s,e){calenderClick(s,'ddeEndSSADeterminationDate');}" 
                                            Init="function(s,e) {InitializeStartDate(s,'ddeEndSSADeterminationDate') }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblDisablingMentalDisorder" CssClass="required" AssociatedControlID="cbDisablingMentalDisorder"
                                                    ClientInstanceName="lblDisablingMentalDisorderIndicator" runat="server"
                                                    Text="Do you or did you have a disabling mental disorder that makes it hard to complete activities such as working, schooling or volunteering?"
                                                    EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbDisablingMentalDisorder"
                                        ClientInstanceName="cbDisablingMentalDisorderIndicator" runat="server" ValueType="System.Boolean" 
                                        Value='<%# Bind("DisablingMentalDisorderIndicator") %>' TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { fncDisablingMentalDisorderChange(s); }" 
                                            Init="function(s,e) { fncDisablingMentalDisorderChange(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblDisablingMentalDisorderVerifiedBy" AssociatedControlID="cbDisablingMentalDisorderVerifiedBy"
                                                    ClientInstanceName="lblDisablingMentalDisorderVerifiedByCode" runat="server" Text="Verified By"
                                                    EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbDisablingMentalDisorderVerifiedBy"
                                        ClientInstanceName="cbDisablingMentalDisorderVerifiedByCode" runat="server" ValueType="System.String" 
                                        Value='<%# Bind("DisablingMentalDisorderVerifiedByCode") %>' TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trWhenDisablingMentalDisorder" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblWhenDisablingMentalDisorder" CssClass="required" 
                                                    AssociatedControlID="cbWhenDisablingMentalDisorder" runat="server" Text="When did you have this condition?"
                                                    EncodeHtml="false" ClientInstanceName="lblWhenDisablingMentalDisorder"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" ID="cbWhenDisablingMentalDisorder" 
                                        runat="server" ValueType="System.String" Value='<%# Bind("WhenDisablingmentalDisorderCode") %>' 
                                        AutoPostBack="false" OnSelectedIndexChanged="CbWhenDisablingMentalDisorder_SelectedIndexChanged"
                                        ClientInstanceName="cbWhenDisablingMentalDisorder" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { DisablingMentalDisorderStatus(s); }"
                                            Init="function(s, e) { DisablingMentalDisorderStatus(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trEndDisablingMentalDisorderDate" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblEndDisablingMentalDisorderDate" CssClass="required" 
                                                    AssociatedControlID="dateEndDisablingMentalDisorderDate" runat="server" Text="End Date of this condition"
                                                    EncodeHtml="false" ClientInstanceName="lblEndDisablingMentalDisorderDate"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dateEndDisablingMentalDisorderDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" 
                                        runat="server" ClientInstanceName="ddeEndDisablingMentalDisorderDate" AutoPostBack="false" Value='<%# Bind("EndDisablingMentalDisorderDate") %>' 
                                        SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents DropDown="function(s,e){calenderClick(s,'ddeEndDisablingMentalDisorderDate');}" 
                                            Init="function(s,e) {InitializeStartDate(s,'ddeEndDisablingMentalDisorderDate') }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblPhysicalDisability" CssClass="required" AssociatedControlID="cbPhysicalDisability"
                                                    ClientInstanceName="lblPhysicalDisabilityIndicator"
                                                    runat="server" Text="Do you or did you have a physical, intellectual or developmental disability that significantly impairs your ability to perform one or more activities of daily living?"
                                                    EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbPhysicalDisability" ClientInstanceName="cbPhysicalDisabilityIndicator"
                                        runat="server" ValueType="System.Boolean" Value='<%# Bind("PhysicalDisabilityIndicator") %>' TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { fncPhysicalDisabilityChange(s); }"
                                            Init="function(s,e) { fncPhysicalDisabilityChange(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblPhysicalDisabilityVerifiedBy" AssociatedControlID="cbPhysicalDisabilityVerifiedBy" 
                                                    ClientInstanceName="lblPhysicalDisabilityVerifiedBy" runat="server" Text="Verified By" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbPhysicalDisabilityVerifiedBy" 
                                        ClientInstanceName="cbPhysicalDisabilityVerifiedByCode" runat="server" ValueType="System.String" 
                                        Value='<%# Bind("PhysicalDisabilityVerifiedByCode") %>' TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trWhenPhysicalDisability" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblWhenPhysicalDisability" CssClass="required" AssociatedControlID="cbWhenPhysicalDisability"
                                                    runat="server" Text="When did you have this condition?" EncodeHtml="false" ClientInstanceName="lblWhenPhysicalDisability"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" ID="cbWhenPhysicalDisability" runat="server" 
                                        ValueType="System.String" Value='<%# Bind("WhenPhysicalDisabilityCode") %>' AutoPostBack="false" 
                                        OnSelectedIndexChanged="CbWhenPhysicalDisability_SelectedIndexChanged" ClientInstanceName="cbWhenPhysicalDisability" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { WhenPhysicalDisabilityIndicator(s); }" 
                                            Init="function(s, e) { WhenPhysicalDisabilityIndicator(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trEndPhysicalDisabilityDate" style="display: none">
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblEndPhysicalDisabilityDate" CssClass="required" AssociatedControlID="dateEndPhysicalDisabilityDate" 
                                                    runat="server" Text="End Date of this condition" EncodeHtml="false" ClientInstanceName="lblEndPhysicalDisabilityDate"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dateEndPhysicalDisabilityDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server" 
                                        ClientInstanceName="ddeEndPhysicalDisabilityDate" AutoPostBack="false" Value='<%# Bind("EndPhysicalDisabilityDate") %>'
                                        SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                            <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents DropDown="function(s,e){calenderClick(s,'ddeEndPhysicalDisabilityDate');}" 
                                            Init="function(s,e) {InitializeStartDate(s,'ddeEndPhysicalDisabilityDate') }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblIndividualAddDate" AssociatedControlID="dateIndividualAddedDate" runat="server"
                                                    Text="Date Individual Added to Case" EncodeHtml="false" ClientInstanceName="lblFileTaxReturnInCurrentYearIndicator"></dx:ASPxLabel>
                                                <div class="tooltip-container">
                                                    <span class="tooltip"></span>
                                                    <div class="tooltip-text">
                                                        <ul>
                                                            <li style="margin-left: 12px; font-size: 7pt;">
                                                                <p style="margin-left: 12px; font-family: vegurregular; font-style: italic;
font-size: 9pt;">This field is applicable when a new individual is added to an Open Medical Assistance case. This date is the new individual's Filing Date.</p>
                                                            </li>
                                                        </ul>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dateIndividualAddedDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server"
                                        Value='<%# Bind("IndividualAddedDate") %>' ClientInstanceName="ddeIndividualAddedDate" AutoPostBack="false" SkinID="RetrieveClear" TabIndex="0">
                                        <ClientSideEvents DropDown="function(s,e){calenderClick(s,'dateIndividualAddedDate');}" 
                                            Init="function(s,e) {InitializeStartDate(s,'dateIndividualAddedDate') }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </EditItemTemplate>
    </asp:FormView>


    <%---------------------------------------------------------------HARDSHIP WAIVER---------------------------------------------------------------------------%>
    <br />
    <br />
    <dhss:DataServiceLinqDataSource runat="server"
        ID="DsTechnical_CommunityEngagementHardshipWaiver"
        EnableUpdate="True"
        TableName="Technical_CommunityEngagementHardshipWaiver" OnSelecting="DsTechnical_CommunityEngagementHardshipWaiver_Selecting"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.TechnicalContextImpl">
    </dhss:DataServiceLinqDataSource>
    <asp:FormView ID="fvTechnical_CommunityEngagementHardshipWaiver" runat="server" DefaultMode="Edit" DataKeyNames="CommunityEngagementHardshipWaiverID"
        DataSourceID="DsTechnical_CommunityEngagementHardshipWaiver"
        OnItemUpdating="FvTechnical_CommunityEngagementHardshipWaiver_ItemUpdating">
        <EditItemTemplate>
            <table class="ContentTable">
                <tr>
                    <td style="height: 20px;"></td>
                </tr>
                <tr>
                    <td>
                        <dx:ASPxLabel ID="lblCommunityEngagementHardshipWaiver" runat="server" Text="Hardship Waiver" SkinID="Header"></dx:ASPxLabel>
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblHospitalizedSeriousCondition" AssociatedControlID="cbHospitalizedSeriousCondition" 
                                                    ClientInstanceName="lblHospitalizedSeriousCondition" runat="server" Text="Recently hospitalized for a serious medical condition?" 
                                                    EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbHospitalizedSeriousCondition"
                                        ClientInstanceName="cbHospitalizedSeriousCondition" runat="server" ValueType="System.Boolean"
                                        Value='<%# Bind("HospitalizedSeriousConditionIndicator") %>' TabIndex="0">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { fncHospitalizedSeriousConditionChange(s); }" 
                                            Init="function(s,e) { fncHospitalizedSeriousConditionChange(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblHospitalizedSeriousConditionVerifiedBy" CssClass="required" 
                                                    AssociatedControlID="cbHospitalizedSeriousConditionVerifiedBy" ClientInstanceName="lblHospitalizedSeriousConditionVerifiedByCode"
                                                    runat="server" Text="Verified By" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbHospitalizedSeriousConditionVerifiedBy" 
                                        ClientInstanceName="cbHospitalizedSeriousConditionVerifiedByCode" runat="server" ValueType="System.String" 
                                        Value='<%# Bind("HospitalizedSeriousConditionVerifiedByCode") %>' TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td style="text-align: right;">
                                                <dx:ASPxLabel ID="lblHospitalizedBeginDate" CssClass="required" AssociatedControlID="ddeHospitalizedBeginDate"
                                                    runat="server" Text="Begin Date" EncodeHtml="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 120px; height: 24px;">
                                    <dx:ASPxDateEdit ID="ddeHospitalizedBeginDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server"
                                        Value='<%# Bind("HospitalizedBeginDate") %>' ClientInstanceName="ddeHospitalizedBeginDate" AutoPostBack="false"
                                        SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents LostFocus="function(s,e) {var x = true; x = DateLostFocus(s,'ddeHospitalizedBeginDate', false); e.processOnServer = x;}" 
                                            DropDown="function(s,e){calenderClick(s,'ddeHospitalizedBeginDate');}" />
                                    </dx:ASPxDateEdit>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblHospitalizedEndDate" CssClass="required" AssociatedControlID="ddeHospitalizedEndDate" runat="server" 
                                                    Text="End Date"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="ddeHospitalizedEndDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server" 
                                        Value='<%# Bind("HospitalizedEndDate") %>' SkinID="RetrieveClear" ClientInstanceName="ddeHospitalizedEndDate" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents LostFocus="function(s,e) {DateLostFocus(s,'ddeHospitalizedEndDate', false);}" 
                                            DropDown="function(s,e){calenderClick(s,'ddeHospitalizedEndDate');}"
                                            Init="function(s,e){ InitializeStartDate(s,'ddeHospitalizedEndDate'); }"
                                            Validation="function(s,e) { validateEndDateNotBeforeBegin(s,e,'ddeHospitalizedBeginDate'); }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <dx:ASPxLabel ID="lblHospitalizedSupervisorDecision" runat="server" Text="Supervisor Decision:" 
                                        Font-Bold="true" EncodeHtml="false"></dx:ASPxLabel>
                                </td>
                            </tr>
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblHospitalizedSupervisorApprovedIndicator" AssociatedControlID="chkHospitalizedSupervisorApprovedIndicator"
                                                    runat="server" Text="Approved" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxCheckBox ID="chkHospitalizedSupervisorApprovedIndicator" ClientInstanceName="chkHospitalizedSupervisorApprovedIndicator" 
                                        Value='<%# Bind("HospitalizedSupervisorApprovedIndicator") %>' runat="server" TabIndex="0"></dx:ASPxCheckBox>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblHospitalizedSupervisorRejectedIndicator" AssociatedControlID="chkHospitalizedSupervisorRejectedIndicator"
                                                    runat="server" Text="Rejected" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxCheckBox ID="chkHospitalizedSupervisorRejectedIndicator" ClientInstanceName="chkHospitalizedSupervisorRejectedIndicator" 
                                        Value='<%# Bind("HospitalizedSupervisorRejectedIndicator") %>' runat="server" TabIndex="0">
                                        <ClientSideEvents CheckedChanged="function(s,e) { setRequired(window.cbHospitalizedJustification, s.GetChecked()); }" />
                                    </dx:ASPxCheckBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <dx:ASPxLabel ID="lblHospitalizedJustification" AssociatedControlID="cbHospitalizedJustification" runat="server" Text="Justification:" 
                                        EncodeHtml="false"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbHospitalizedJustification" 
                                        ClientInstanceName="cbHospitalizedJustification" runat="server" ValueType="System.String" Value='<%# Bind("HospitalizedJustificationCode") %>' 
                                        OnSelectedIndexChanged="CbHospitalizedJustification_SelectedIndexChanged" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblTravelOutOfAreaMedical" AssociatedControlID="cbTravelOutOfAreaMedical" 
                                                    ClientInstanceName="lblTravelOutOfAreaMedical" runat="server" 
                                                    Text="Recently traveled out of the area for extended period to receive medical care for a serious medical condition?" 
                                                    EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbTravelOutOfAreaMedical" 
                                        ClientInstanceName="cbTravelOutOfAreaMedical" runat="server" ValueType="System.Boolean" Value='<%# Bind("TravelOutOfAreaMedicalIndicator") %>' 
                                        TabIndex="0">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { fncTravelOutOfAreaMedicalChange(s); }" 
                                            Init="function(s,e) { fncTravelOutOfAreaMedicalChange(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblTravelOutOfAreaMedicalVerifiedBy" AssociatedControlID="cbTravelOutOfAreaMedicalVerifiedBy"
                                                    ClientInstanceName="lblTravelOutOfAreaMedicalVerifiedBy" runat="server" Text="Verified By" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbTravelOutOfAreaMedicalVerifiedBy" 
                                        ClientInstanceName="cbTravelOutOfAreaMedicalVerifiedBy" runat="server" ValueType="System.String"
                                        Value='<%# Bind("TravelOutOfAreaMedicalVerifiedByCode") %>' TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td style="text-align: right;">
                                                <dx:ASPxLabel ID="lblTravelOutOfAreaMedicalBeginDate" AssociatedControlID="ddeTravelOutOfAreaMedicalBeginDate"
                                                    runat="server" Text="Begin Date" EncodeHtml="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 120px; height: 24px;">
                                    <dx:ASPxDateEdit ID="ddeTravelOutOfAreaMedicalBeginDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server"
                                        Value='<%# Bind("TravelOutOfAreaMedicalBeginDate") %>' ClientInstanceName="ddeTravelOutOfAreaMedicalBeginDate" 
                                        AutoPostBack="false"
                                        SkinID="RetrieveClear" TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents LostFocus="function(s,e) {var x = true; x = DateLostFocus(s,'ddeTravelOutOfAreaMedicalBeginDate', false); e.processOnServer = x;}" 
                                            DropDown="function(s,e){calenderClick(s,'ddeTravelOutOfAreaMedicalBeginDate');}" />
                                    </dx:ASPxDateEdit>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblTravelOutOfAreaMedicalEndDate" AssociatedControlID="ddeTravelOutOfAreaMedicalEndDate" 
                                                    runat="server" Text="End Date"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="ddeTravelOutOfAreaMedicalEndDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" 
                                        runat="server" Value='<%# Bind("TravelOutOfAreaMedicalEndDate") %>' SkinID="RetrieveClear" ClientInstanceName="ddeTravelOutOfAreaMedicalEndDate"
                                        TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                        <ClientSideEvents LostFocus="function(s,e) {DateLostFocus(s,'ddeTravelOutOfAreaMedicalEndDate', false);}" 
                                            DropDown="function(s,e){calenderClick(s,'ddeTravelOutOfAreaMedicalEndDate');}"
                                            Init="function(s,e){ InitializeStartDate(s,'ddeTravelOutOfAreaMedicalEndDate'); }"
                                            Validation="function(s,e) { validateEndDateNotBeforeBegin(s,e,'ddeTravelOutOfAreaMedicalBeginDate'); }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <dx:ASPxLabel ID="lblTravelOutOfAreaSupervisorDecision" runat="server" Text="Supervisor Decision:" Font-Bold="true" EncodeHtml="false"></dx:ASPxLabel>
                                </td>
                            </tr>
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblTravelOutOfAreaSupervisorApprovedIndicator" AssociatedControlID="chkTravelOutOfAreaSupervisorApprovedIndicator"
                                                    runat="server" Text="Approved" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxCheckBox ID="chkTravelOutOfAreaSupervisorApprovedIndicator" ClientInstanceName="chkTravelOutOfAreaSupervisorApprovedIndicator"
                                        Value='<%# Bind("TravelOutOfAreaSupervisorApprovedIndicator") %>' runat="server" TabIndex="0"></dx:ASPxCheckBox>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblTravelOutOfAreaSupervisorRejectedIndicator" AssociatedControlID="chkTravelOutOfAreaSupervisorRejectedIndicator" 
                                                    runat="server" Text="Rejected" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>

                                    <dx:ASPxCheckBox ID="chkTravelOutOfAreaSupervisorRejectedIndicator" ClientInstanceName="chkTravelOutOfAreaSupervisorRejectedIndicator" 
                                        Value='<%# Bind("TravelOutOfAreaSupervisorRejectedIndicator") %>' runat="server" TabIndex="0">
                                        <ClientSideEvents CheckedChanged="function(s,e) { setRequired(window.cbTravelOutOfAreaMedicalJustification, s.GetChecked()); }" />
                                    </dx:ASPxCheckBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <dx:ASPxLabel ID="lblTravelOutOfAreaMedicalJustification" AssociatedControlID="cbTravelOutOfAreaMedicalJustification" runat="server" 
                                        Text="Justification:" EncodeHtml="false"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbTravelOutOfAreaMedicalJustification" 
                                        ClientInstanceName="cbTravelOutOfAreaMedicalJustification" runat="server" ValueType="System.String" 
                                        Value='<%# Bind("TravelOutOfAreaMedicalJustificationCode") %>' OnSelectedIndexChanged="CbTravelOutOfAreaMedicalJustification_SelectedIndexChanged" 
                                        TabIndex="0">
                                        <ValidationSettings SetFocusOnError="True">
                                             <RequiredField IsRequired="True" ErrorText="Please fill in all mandatory fields." />
                                        </ValidationSettings>
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="height: 20px;"></td>
                </tr>
                <tr>
                    <td>
                        <dx:ASPxLabel ID="lblDisasterDeclarationHeader" runat="server" Text="Hardship waiver applied due to disaster declaration" Font-Bold="true" 
                            EncodeHtml="false"></dx:ASPxLabel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblDisasterClientRequestsRemovalIndicator" AssociatedControlID="chkDisasterClientRequestsRemovalIndicator"
                                                    runat="server" Text="Client Requests Removal" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxCheckBox ID="chkDisasterClientRequestsRemovalIndicator" ClientInstanceName="chkDisasterClientRequestsRemovalIndicator" 
                                        Value='<%# Bind("DisasterClientRequestsRemovalIndicator") %>' runat="server" TabIndex="0"></dx:ASPxCheckBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td style="text-align: right;">
                                                <dx:ASPxLabel ID="lblDisasterDeclarationBeginDate" AssociatedControlID="ddeDisasterDeclarationBeginDate"
                                                    runat="server" Text="Begin Date" EncodeHtml="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 120px; height: 24px;">
                                    <dx:ASPxDateEdit ID="ddeDisasterDeclarationBeginDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server" ReadOnly="true"
                                        Value='<%# Bind("DisasterDeclarationBeginDate") %>' ClientInstanceName="ddeDisasterDeclarationBeginDate" AutoPostBack="false"
                                        SkinID="RetrieveClear" TabIndex="0">
                                        <ClientSideEvents LostFocus="function(s,e) {var x = true; x = DateLostFocus(s,'ddeDisasterDeclarationBeginDate', false); e.processOnServer = x;}"
                                            DropDown="function(s,e){calenderClick(s,'ddeDisasterDeclarationBeginDate');}" />
                                    </dx:ASPxDateEdit>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblDisasterDeclarationEndDate" AssociatedControlID="ddeDisasterDeclarationEndDate" runat="server" 
                                                    Text="End Date"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="ddeDisasterDeclarationEndDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server" 
                                        ReadOnly="true" Value='<%# Bind("DisasterDeclarationEndDate") %>' SkinID="RetrieveClear" ClientInstanceName="ddeDisasterDeclarationEndDate" 
                                        TabIndex="0">
                                        <ClientSideEvents LostFocus="function(s,e) {DateLostFocus(s,'ddeDisasterDeclarationEndDate', false);}" 
                                            DropDown="function(s,e){calenderClick(s,'ddeDisasterDeclarationEndDate');}"
                                            Init="function(s,e){ InitializeStartDate(s,'ddeDisasterDeclarationEndDate'); }"
                                            Validation="function(s,e) { validateEndDateNotBeforeBegin(s,e,'ddeDisasterDeclarationBeginDate'); }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="height: 20px;"></td>
                </tr>
                <tr>
                    <td>
                        <dx:ASPxLabel ID="lblUnemploymentLevelHeader" runat="server" Text="Hardship waiver applied due to unemployment level" Font-Bold="true" 
                            EncodeHtml="false"></dx:ASPxLabel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblUnemploymentClientRequestsRemovalIndicator" AssociatedControlID="chkUnemploymentClientRequestsRemovalIndicator"
                                                    runat="server" Text="Client Requests Removal" EncodeHtml="false"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxCheckBox ID="chkUnemploymentClientRequestsRemovalIndicator" ClientInstanceName="chkUnemploymentClientRequestsRemovalIndicator" 
                                        Value='<%# Bind("UnemploymentClientRequestsRemovalIndicator") %>' runat="server" TabIndex="0"></dx:ASPxCheckBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="mylengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td style="text-align: right;">
                                                <dx:ASPxLabel ID="lblUnemploymentLevelBeginDate" AssociatedControlID="ddeUnemploymentLevelBeginDate" runat="server" 
                                                    Text="Begin Date" EncodeHtml="false" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="width: 120px; height: 24px;">
                                    <dx:ASPxDateEdit ID="ddeUnemploymentLevelBeginDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server" ReadOnly="true"
                                        Value='<%# Bind("UnemploymentAreaBeginDate") %>' ClientInstanceName="ddeUnemploymentLevelBeginDate" AutoPostBack="false"
                                        SkinID="RetrieveClear" TabIndex="0">
                                        <ClientSideEvents LostFocus="function(s,e) {var x = true; x = DateLostFocus(s,'ddeUnemploymentLevelBeginDate', false); e.processOnServer = x;}" 
                                            DropDown="function(s,e){calenderClick(s,'ddeUnemploymentLevelBeginDate');}" />
                                    </dx:ASPxDateEdit>
                                </td>
                                <td>
                                    <table>
                                        <tr>
                                            <td class="verifiedBy">
                                                <dx:ASPxLabel ID="lblUnemploymentLevelEndDate" AssociatedControlID="ddeUnemploymentLevelEndDate" runat="server" Text="End Date"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="ddeUnemploymentLevelEndDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server" ReadOnly="true" 
                                        Value='<%# Bind("UnemploymentAreaEndDate") %>' SkinID="RetrieveClear" ClientInstanceName="ddeUnemploymentLevelEndDate" TabIndex="0">
                                        <ClientSideEvents LostFocus="function(s,e) {DateLostFocus(s,'ddeUnemploymentLevelEndDate', false);}"
                                            DropDown="function(s,e){calenderClick(s,'ddeUnemploymentLevelEndDate');}"
                                            Init="function(s,e){ InitializeStartDate(s,'ddeUnemploymentLevelEndDate'); }"
                                            Validation="function(s,e) { validateEndDateNotBeforeBegin(s,e,'ddeUnemploymentLevelBeginDate'); }" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </EditItemTemplate>
    </asp:FormView>

    <dx:ASPxPopupControl ID="popupHardshipWaiverPendingApproval" runat="server" CloseAction="CloseButton"
        AllowResize="false" FooterText=" " PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        AllowDragging="True" Width="600px" ShowOnPageLoad="false" Modal="true"
        HeaderText="Hardship Waiver Pending Approval" ClientInstanceName="popupHardshipWaiverPendingApproval"
        OnWindowCallback="PopupHardshipWaiverPendingApproval_WindowCallback">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupHardshipWaiverContent" runat="server">
                <div style="color: red; font-weight: bold;">
                    <p>A Short-Term Hardship Waiver has been entered on this case. This waiver will not be applied until a supervisor reviews and approves it.</p>
                    <p>You must enter a case comment below to justify the hardship waiver exemption before continuing.</p>
                </div>
                <br />
                <table class="SectionTable">
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblHWWorkerName" runat="server" Text="Worker Name" EncodeHtml="false"></dx:ASPxLabel>
                        </td>
                        <td>
                            <dx:ASPxLabel ID="lblHWWorkerNameValue" runat="server" CssClass="leftLabel"></dx:ASPxLabel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblHWTitle" runat="server" Text="Title" EncodeHtml="false"></dx:ASPxLabel>
                        </td>
                        <td>
                            <dx:ASPxLabel ID="lblHWTitleValue" runat="server" Text="Hardship Waiver newly added/updated" CssClass="leftLabel"></dx:ASPxLabel>
                        </td>
                        <td>
                            <dx:ASPxLabel ID="lblHWDate" runat="server" Text="Date" EncodeHtml="false"></dx:ASPxLabel>
                        </td>
                        <td>
                            <dx:ASPxDateEdit ID="dteHWDate" runat="server" EditFormatString="MM/dd/yyyy" ReadOnly="true" ValidationSettings-CausesValidation="false"></dx:ASPxDateEdit>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblHWPageName" runat="server" Text="Page Name" EncodeHtml="false"></dx:ASPxLabel>
                        </td>
                        <td>
                            <dx:ASPxLabel ID="lblHWPageNameValue" runat="server" Text="Community Engagement" CssClass="leftLabel"></dx:ASPxLabel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblHWNotes" runat="server" Text="Notes" CssClass="required" AssociatedControlID="memHWNotes"></dx:ASPxLabel>
                        </td>
                        <td colspan="3">
                            <dx:ASPxMemo ID="memHWNotes" ClientInstanceName="memHWNotesCL" runat="server" Height="100px" Width="100%" Rows="6" AutoPostBack="false" MaxLength="500">
                                <ValidationSettings SetFocusOnError="True" ValidationGroup="HardshipWaiverPopupGroup" CausesValidation="false">
                                     <RequiredField IsRequired="True" ErrorText="Notes are required." />
                                </ValidationSettings>
                            </dx:ASPxMemo>

                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td style="padding-left: 350px">
                            <dx:ASPxButton ID="btnHWCommentSave" runat="server" Text="Save" Width="80px" AutoPostBack="false" CausesValidation="false"
                                ClientSideEvents-Click="function(s,e){ if (ASPxClientEdit.ValidateEditorsInContainer(popupHardshipWaiverPendingApproval.GetMainElement(), 'HardshipWaiverPopupGroup')) { popupHardshipWaiverPendingApproval.PerformCallback('save'); } }" />
                        </td>
                        <td style="padding-left: 10px">
                            <dx:ASPxButton ID="btnHWCommentCancel" runat="server" Text="Cancel" SkinID="SecondaryButton" AutoPostBack="false" CausesValidation="false"
                                ClientSideEvents-Click="function(s,e){ ASPxClientEdit.ClearGroup('HardshipWaiverPopupGroup'); popupHardshipWaiverPendingApproval.Hide(); }" Width="80px" />
                        </td>
                    </tr>
                </table>
            </dx:PopupControlContentControl>
        </ContentCollection>
        <ClientSideEvents EndCallback="function(s,e){ popupHardshipWaiverPendingApproval.Hide(); }" />
    </dx:ASPxPopupControl>

    <dx:ASPxPopupControl runat="server"
        ID="dxPopupInfo" ClientInstanceName="pcerrorpopup2"
        SkinID="ErrorPopUp"
        Modal="true"
        CloseAction="CloseButton"
        ShowOnPageLoad="false"
        PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter"
        AllowDragging="true"
        Width="300px"
        Height="120px"
        ShowFooter="true"
        HeaderText="Question"
        ShowPageScrollbarWhenModal="True">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server" ID="pucalertmessagecontent">
                <dx:ASPxLabel runat="server" ID="lblErrMessage1" Width="280px" ClientInstanceName="lblErrMessage1" />
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterTemplate>
            <div style="float: right; margin: 3px;">
                <table style="float: right;">
                    <tr>
                        <td style="padding-right: 3px">
                            <dx:ASPxButton runat="server"
                                ID="btnYes"
                                Text="Yes"
                                OnClick="BtnPopUpYes_Click"
                                CausesValidation="false"
                                IgnoreFgs="T" SkinID="footerPrimary" />
                        </td>
                        <td>
                            <dx:ASPxButton runat="server"
                                ID="btnNo"
                                Text="No"
                                AutoPostBack="false"
                                CausesValidation="false"
                                IgnoreFgs="T" SkinID="footerPrimary"
                                ClientSideEvents-Click="function(s,e){pcerrorpopup2.Hide(); e.processOnServer = false;}" />
                        </td>
                    </tr>
                </table>
            </div>
        </FooterTemplate>
        <FooterStyle>
            <Paddings PaddingBottom="12px" PaddingTop="8px" />
        </FooterStyle>
    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl runat="server"
        ID="dxPopupSaveConfirmation" ClientInstanceName="pcsaveconfirmation"
        Modal="true"
        CloseAction="CloseButton"
        ShowOnPageLoad="false"
        PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter"
        AllowDragging="true"
        Width="300px"
        Height="120px"
        ShowFooter="true"
        HeaderText="Community Engagement"
        ShowPageScrollbarWhenModal="true">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server" ID="pucsaveconfirmationcontent">
                <dx:ASPxLabel runat="server" ID="lblSaveConfirmationMessage1" Width="280px" />
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterTemplate>
            <div>
                <dx:ASPxButton runat="server" ID="btnSaveConfirmationOk" Text="OK"
                    AutoPostBack="false" CausesValidation="false" IgnoreFgs="T" SkinID="footerPrimary"
                    ClientSideEvents-Click="function(s,e){pcsaveconfirmation.Hide();e.processOnServer=false;}" />
            </div>
        </FooterTemplate>
        <FooterStyle>
            <Paddings PaddingBottom="12px" PaddingTop="8px" />
        </FooterStyle>

    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl runat="server"
        ID="ASPxPopupBeginDateConfirm" ClientInstanceName="pcbegindateconfirm"
        Modal="true"
        CloseAction="CloseButton"
        ShowOnPageLoad="false"
        PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter"
        AllowDragging="true"
        Width="300px"
        Height="130px"
        ShowFooter="true"
        HeaderText="Community Begin Date"
        ShowPageScrollbarWhenModal="true">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server" ID="pucbegindateconfirmcontent">
                <dx:ASPxLabel runat="server" ID="lblBeginDateConfirmMessage1" Width="330px" Text="The effective begin date entered is between 1990 and 1997. Do you want save this effective begin date?" />
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterTemplate>
            <div style="float: right; margin: 3px;">
                <table style="float: right;">
                    <tr>
                        <td style="padding-right: 3px">
                            <dx:ASPxButton runat="server" ID="btnBeginDateConfirmYes" Text="Yes"
                                AutoPostBack="false" CausesValidation="false" IgnoreFgs="T" SkinID="footerPrimary"
                                ClientSideEvents-Click="function(s,e){ pcbegindateconfirm.Hide() ;e.processOnServer=false;}" />
                        </td>
                        <td>
                            <dx:ASPxButton runat="server" ID="btnBeginDateConfirmNo" Text="No"
                                AutoPostBack="false" CausesValidation="false" IgnoreFgs="T" SkinID="footerPrimary"
                                ClientSideEvents-Click="function(s,e){ ddeCEBeginDate.SetDate(null); ddeCEBeginDate.Focus(); pcbegindateconfirm.Hide() ;e.processOnServer=false;}" />
                        </td>
                    </tr>
                </table>
            </div>
        </FooterTemplate>
        <FooterStyle>
            <Paddings PaddingBottom="12px" PaddingTop="8px" />
        </FooterStyle>

    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl runat="server"
    ID="ASPxPopupMandatoryFields" ClientInstanceName="pcmandatoryfields"
    Modal="true"
    CloseAction="CloseButton"
    ShowOnPageLoad="false"
    PopupHorizontalAlign="WindowCenter"
    PopupVerticalAlign="WindowCenter"
    AllowDragging="true"
    Width="300px"
    Height="120px"
    ShowFooter="true"
    HeaderText="Page Validation Summary"
    ShowPageScrollbarWhenModal="true">
    <ContentCollection>
        <dx:PopupControlContentControl runat="server" ID="pcmandatoryfieldscontent">
            <dx:ASPxLabel runat="server" ID="lblMandatoryFieldsMessage" Width="330px" 
                Text="Page fill in all mdnatory fields." />
        </dx:PopupControlContentControl>
    </ContentCollection>
    <FooterTemplate>
        <div>
            <dx:ASPxButton runat="server" ID="btnMandatoryFieldsOk" Text="OK"
                AutoPostBack="false" CausesValidation="false" IgnoreFgs="T" SkinID="footerPrimary"
                ClientSideEvents-Click="function(s,e){ pcmandatoryfields.Hide() ;e.processOnServer=false;}" />
        </div>
    </FooterTemplate>
    <FooterStyle>
        <Paddings PaddingBottom="12px" PaddingTop="8px" />
    </FooterStyle>

</dx:ASPxPopupControl>
</asp:Content>

