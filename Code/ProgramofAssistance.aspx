<%@ Page Language="C#" MasterPageFile="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" AutoEventWireup="True" CodeBehind="ProgramofAssistance.aspx.cs" Inherits="Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical.ProgramofAssistance" Title="Program of Assistance" %>

<%@ MasterType VirtualPath="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" %>
<%@ Register TagPrefix="dx" Namespace="DevExpress.Web.ASPxEditors" Assembly="DevExpress.Web.v13.2" %>
<asp:Content ID="ctPageBody" ContentPlaceHolderID="PageBodyContent" runat="server">
    <%--<script src="ProgramofAssistance.js" type="text/javascript"></script>--%>
    <script src='<%= ResolveUrl(Dhss.Assist.WorkerWeb.Web.Infrastructure.Helpers.JsVersioningHelper.Tag("~/Intake/ApplicationEntry/Technical/ProgramofAssistance.js")) %>'></script>
    <dx:ASPxPopupControl ID="ASPxPopupClientControl" runat="server" CloseAction="OuterMouseClick" ShowOnPageLoad="False" ClientSideEvents-CloseButtonClick="function(s, e) { btnAll.SetFocus();}"
        PopupElementID="btnIndividual;btnIndividual1;btnIndividual2;btnIndividual3;btnIndividual4;btnIndividual5;"
        PopupVerticalAlign="Below" PopupHorizontalAlign="LeftSides" AllowDragging="True"
        Width="250px" Height="130px" HeaderText="Individuals" ClientInstanceName="ASPxPopupClientControl1">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl" runat="server">
                <div style="vertical-align: middle">
                    <dx:ASPxCheckBoxList ID="cklIndividuals" runat="server" ClientInstanceName="cklIndividuals" TextField="Name" ValueField="ApplicationEntityID" ValueType="System.Int32" ImageUrlField="ImgSrc" ItemImage-Height="16" ItemImage-Width="16">
                        <ClientSideEvents SelectedIndexChanged="function(s, e) { SelectUnselectIndividual(s); }" />
                    </dx:ASPxCheckBoxList>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
        <ClientSideEvents PopUp="popup_Popup" />
    </dx:ASPxPopupControl>
    <dhss:DataServiceLinqDataSource runat="server" ID="dsTechnical_ProgramDetail" EnableUpdate="True"
        TableName="Technical_ProgramDetail"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.TechnicalContextImpl" OnSelecting="DsTechnical_ProgramDetail_Selecting">
    </dhss:DataServiceLinqDataSource>
    <dhss:DataServiceLinqDataSource runat="server" ID="dsTechnical_DisabledChildren" EnableUpdate="True"
        TableName="Technical_DisabledChildrenProgram" EntityTypeName="Technical_DisabledChildrenProgram"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.TechnicalContextImpl" OnSelecting="Technical_DisabledChildren_Selecting" />
    <dhss:DataServiceLinqDataSource runat="server" ID="dsTechnical_FoodBenefits" EnableUpdate="True"
        TableName="Technical_FoodBenefitsProgram" EntityTypeName="Technical_FoodBenefitsProgram"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.TechnicalContextImpl" OnSelecting="Technical_FoodBenefits_Selecting" />
    <dhss:DataServiceLinqDataSource runat="server" ID="dsTechnical_MedicalAssistanceProgram" EnableUpdate="True"
        TableName="Techincal_MedicalAssistanceProgram" EntityTypeName="Techincal_MedicalAssistanceProgram"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.TechnicalContextImpl" OnSelecting="Technical_MedicalAssistanceProgram_Selecting" />
    <dhss:DataServiceLinqDataSource runat="server" ID="dsTechnical_QMBProgram" EnableUpdate="True"
        TableName="Technical_QualifiedMemberBeneficiaryProgram" EntityTypeName="Techincal_QualifiedMemberBeneficiaryProgram"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.TechnicalContextImpl" OnSelecting="Technical_QMBProgram_Selecting" />
    <script type="text/javascript">
        function ShowPopUp() {
            document.getElementById("RMA_popup").style.display = "block";
        }

        function HidePopUp() {
            document.getElementById("RMA_popup").style.display = "none";

        }

        function OnCommentSavedCl(s, e) {
            if (ASPxClientEdit.ValidateGroup('AddNotePopupError')) {
                AddCommentPopUpPOADetail.PerformCallback();
            }
            else { ppError.Hide(); }
        }

        function PopUpEndCallBack(s, e) {
            AddCommentPopUpPOADetail.Hide();
        }

    </script>
    <style>
        .retroMAHelpPopup {
            background-color: #d1e4f3 !important;
            color: #00529b !important;
            border: 1px solid #4d8fcb !important;
            top: 320px;
            width: 350px;
            line-height: normal;
            z-index: 1;
            position: absolute;
            padding-left: 20px;
        }
        
    </style>
    <table class="ContentTable">
        <tr>
            <td>
                <table style="width: 100%">
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="ASPxLabel15" runat="server" Text="Program of Assistance Details" SkinID="Header" EnableViewState="false"></dx:ASPxLabel>
                        </td>
                        <td class="floatRight">
                            <dx:ASPxButton runat="server" ID="btnDocumentImagingVerification" Visible="true" SkinID="HyperLinkStyleBtn" CausesValidation="false" AutoPostBack="true" OnClick="BtnDocumentImagingVerification_Click" Text="Document Imaging Verification"></dx:ASPxButton>
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
                        <td>
                            <dx:ASPxButton runat="server"
                                ID="btnBackToSummary"
                                SkinID="HyperLinkStyleBtn"
                                OnClick="BtnBackToSummary_Click"
                                EncodeHtml="false"
                                CausesValidation="false" IgnoreFgs="T"
                                Text="< Back to Summary" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="cashLabel" visible="false" runat="server">
            <td>
                <br />
                <dx:ASPxLabel ID="ASPxLabel35" runat="server" Text="Cash" SkinID="InnerHeader" EnableViewState="false"></dx:ASPxLabel>
                <hr />
            </td>
        </tr>
        <tr id="childcareLabel" visible="false" runat="server">
            <td>
                <br />
                <dx:ASPxLabel ID="ASPxLabel27" runat="server" Text="Child Care" SkinID="InnerHeader" EnableViewState="false">
                </dx:ASPxLabel>
                <hr />
            </td>
        </tr>
        <tr id="DisabledChildrenLabel" visible="false" runat="server">
            <td>
                <br />
                <dx:ASPxLabel ID="ASPxLabel34" runat="server" Text="Disabled Children" SkinID="InnerHeader" EnableViewState="false"></dx:ASPxLabel>
                <hr />
            </td>
        </tr>
        <tr id="FBBenefits" runat="server" visible="false">
            <td>
                <br />
                <dx:ASPxLabel ID="ASPxLabel36" runat="server" Text="Food Benefits" SkinID="InnerHeader" EnableViewState="false"></dx:ASPxLabel>
                <hr />
            </td>
        </tr>
        <tr id="MedicalLabel" runat="server" visible="false">
            <td>
                <br />
                <dx:ASPxLabel ID="ASPxLabel37" runat="server" Text="Medical Assistance" SkinID="InnerHeader" EnableViewState="false"></dx:ASPxLabel>
                <hr />
            </td>
        </tr>
        <tr id="QMBLabel" runat="server" visible="false">
            <td>
                <br />
                <dx:ASPxLabel ID="ASPxLabel38" runat="server" Text="Medicare Savings Programs" SkinID="InnerHeader" EnableViewState="false"></dx:ASPxLabel>
                <hr />
            </td>
        </tr>
        <tr>
            <td>
                <asp:HiddenField ID="hfIsPageModified" runat="server" Value="N" EnableViewState="true" ClientIDMode="Static"></asp:HiddenField>
                <asp:HiddenField ID="hfProgramCode" runat="server" EnableViewState="true" ClientIDMode="Static"></asp:HiddenField>
                <asp:FormView runat="server" ID="fvTechnical_ProgramDetail" CssClass="SectionTable" Visible="true"
                    DefaultMode="Edit" DataSourceID="dsTechnical_ProgramDetail" DataKeyNames="ProgramDetailID" OnDataBound="Technical_ProgramDetail_DataBound" OnItemUpdating="FvTechnical_ProgramDetail_ItemUpdating">
                    <EditItemTemplate>
                        <dx:ASPxHiddenField ID="hfCaseFilingDate" ClientInstanceName="hfCaseFilingDate" runat="server"></dx:ASPxHiddenField>
                        <asp:HiddenField runat="server" ID="hfHistoryCode" Value='<%# Bind("HistoryCode") %>' />
                        <table class="SectionTable">
                            <tr>
                                <td>
                                    <dx:ASPxLabel ID="lbRequester" runat="server" EncodeHtml="False" Text="Requester" AssociatedControlID="cbCashRequester"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxComboBox ID="cbCashRequester" TabIndex="1" runat="server" Value='<%# Bind("RequesterNumber") %>' ClientInstanceName="cbCashRequester" Visible="true" ValueType="System.Int32" TextField="Name" IncrementalFilteringMode="StartsWith"
                                        OnSelectedIndexChanged="cbCashRequester_SelectedIndexChanged" AutoPostBack="true">
                                        <ClientSideEvents SelectedIndexChanged="ComboRequesterChange" />
                                    </dx:ASPxComboBox>
                                </td>
                                <td class="SectionTableSingleColumn"></td>
                            </tr>
                            <tr>
                                <td>
                                    <dx:ASPxLabel ID="lblFilingDate" runat="server" EncodeHtml="False" Text="Filing Date" AssociatedControlID="dtCashFilingDate"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dtCashFilingDate" TabIndex="2" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" runat="server" Value='<%# Bind("ProgramFilingDate") %>' ClientInstanceName="dtCashFilingDate" OnDateChanged="DtFilingDate_DateChanged" AutoPostBack="true">
                                        <ClientSideEvents LostFocus="function(s,e) {DateLostFocus(s,'dtCashFilingDate',false);}" DropDown="function(s,e){calenderClick(s,'dtCashFilingDate');}" />
                                    </dx:ASPxDateEdit>
                                </td>
                                <td>
                                    <dx:ASPxLabel ID="lblCashLastVerificationDate" runat="server" EncodeHtml="False" Text="Last Verification Date" AssociatedControlID="dtCashLastVerificationDate"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dtCashLastVerificationDate" ClientIDMode="Static" EditFormatString="MM/dd/yyyy" TabIndex="3" runat="server" Value='<%# Bind("LastVerificationDate") %>' OnDateChanged="DtLastVerificationDate_DateChanged" AutoPostBack="true">
                                        <ClientSideEvents LostFocus="function(s,e) {DateLostFocus(s,'dtCashLastVerificationDate',false);}" DropDown="function(s,e){calenderClick(s,'dtCashLastVerificationDate');}" />
                                    </dx:ASPxDateEdit>
                                    <dx:ASPxDateEdit ID="ASPxDateEdit1" TabIndex="4" runat="server" Value='<%# Bind("BeginDate") %>' Visible="false"></dx:ASPxDateEdit>
                                </td>
                            </tr>
                        </table>
                    </EditItemTemplate>
                </asp:FormView>
            </td>
        </tr>
        <tr>
            <td>
                <asp:FormView runat="server" ID="fvTechnical_DisabledChildren" CssClass="SectionTable" Visible="false"
                    DefaultMode="Edit" DataSourceID="dsTechnical_DisabledChildren" DataKeyNames="DiasabledChildrenProgramID" OnDataBound="Technical_DisabledChildren_DataBound" OnItemUpdating="FvTechnical_DisabledChildren_ItemUpdating">
                    <EditItemTemplate>
                        <asp:HiddenField ID="hfCurrentDisabledRetroMAflag"  runat="server" Value='<%# Bind("RetroMACode") %>'></asp:HiddenField>
                        <asp:HiddenField ID ="hfDisabledEligibilitySummaryCount" runat="server" Value="" />
                        <table class="SectionTable">
                            <tr>
                                <td>
                                    <dx:ASPxLabel ID="lblDisabledCRDP" runat="server" Text="CRDP" AssociatedControlID="cbDisabledCRDP"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxComboBox ID="cbDisabledCRDP" TabIndex="5" runat="server" ValueType="System.String" Value='<%# Bind("CRDPCode") %>' IncrementalFilteringMode="StartsWith"></dx:ASPxComboBox>
                                </td>
                                 <td>
                                    <div id="dRMA" onmouseover="ShowPopUp()" onmouseout="HidePopUp()">
                                        <dx:ASPxLabel ID="lbDisabledRetroMA" runat="server" IncrementalFilteringMode="StartsWith" EncodeHtml="False" Text="Retro MA" AssociatedControlID="cbDisabledRetroMA"></dx:ASPxLabel>
                                    </div>                                               
                                    <div id="RMA_popup" class="retroMAHelpPopup" style="display: none;">
                                        <b>Retro MA</b>
                                        <br />
                                        <br />
                                        <ul>
                                           
                                            <li>
                                                Individuals may be eligible for Medicaid 3 months prior to their application date if they received a Medicaid covered service and were eligible at the time of the service.
                                            </li>
                                        </ul>
                                    </div>
                                </td>
                                <td>
                                 <dx:ASPxComboBox ID="cbDisabledRetroMA" TabIndex="6"   runat="server" IncrementalFilteringMode="StartsWith" ValueType="System.String" Value='<%# Bind("RetroMACode") %>'></dx:ASPxComboBox>
                                </td>
                               
                                <td class="SectionTableSingleColumn"></td>
                            </tr>
                        </table>
                    </EditItemTemplate>
                </asp:FormView>
            </td>
        </tr>
        <tr>
            <td>
                <asp:FormView runat="server" ID="fvTechnical_FoodBenefits" CssClass="SectionTable" Visible="false"
                    DefaultMode="Edit" DataSourceID="dsTechnical_FoodBenefits" DataKeyNames="FoodBenefitProgramID" OnDataBound="Technical_FoodBenefits_DataBound" OnItemUpdating="FvTechnical_FoodBenefits_ItemUpdating">
                    <EditItemTemplate>
                        <table class="SectionTable">
                            <tr>
                                <td>
                                    <dx:ASPxLabel ID="lbProtectedFilingDate" runat="server" EncodeHtml="False" Text="Protected Filing Date" AssociatedControlID="dtProtectedFilingDate"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dtProtectedFilingDate" ClientIDMode="Static" TabIndex="6" runat="server" EditFormatString="MM/dd/yyyy" Value='<%# Bind("ProtectedFilingDate") %>'>
                                        <ClientSideEvents LostFocus="function(s,e) {DateLostFocus(s,'dtProtectedFilingDate');}" DropDown="function(s,e){calenderClick(s,'dtProtectedFilingDate');}" />
                                    </dx:ASPxDateEdit>
                                </td>
                                <td>
                                    <dx:ASPxLabel ID="lbDenialDate" runat="server" Text="Denial Date" AssociatedControlID="dtDenialDate"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxTextBox ID="dtDenialDate" TabIndex="7" runat="server" Text='<%# string.Format ("{0:d}", Eval("DenialDate")) %>' Enabled="false" ReadOnly="true" DisplayFormatString="MM/dd/yyyy"></dx:ASPxTextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <dx:ASPxLabel ID="lbCallBackDate" runat="server" Text="Call Back Date" AssociatedControlID="dtCallBackDate"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxDateEdit ID="dtCallBackDate" ClientIDMode="Static" TabIndex="8" runat="server" EditFormatString="MM/dd/yyyy" ClientInstanceName="dtCallBackDate" Value='<%# Bind("CallBackDate") %>'>
                                        <ClientSideEvents LostFocus="function(s,e) {DateLostFocus(s,'dtCallBackDate');}" DropDown="function(s,e){calenderClick(s,'dtCallBackDate');}" />
                                    </dx:ASPxDateEdit>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <dx:ASPxLabel ID="lbFBIdentity" runat="server" Text="FB Identity" EncodeHtml="false" AssociatedControlID="cbFBIdentity"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxComboBox ID="cbFBIdentity" TabIndex="9" runat="server" ValueType="System.String" Value='<%# Bind("FSIdentityCode") %>' IncrementalFilteringMode="StartsWith" OnSelectedIndexChanged="CbFBIdentity_SelectedIndexChanged" OnDataBound="CbFBIdentity_DataBound" AutoPostBack="false" ClientInstanceName="cbFBIdentity"></dx:ASPxComboBox>
                                </td>
                                <td>
                                    <dx:ASPxLabel ID="lbVerifiedBy" runat="server" Text="Verified By" AssociatedControlID="cbFSIdentityVerificationCode" ClientInstanceName="lbVerifiedBy"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxComboBox ID="cbFSIdentityVerificationCode" TabIndex="10" runat="server" IncrementalFilteringMode="StartsWith" ValueType="System.String" ClientInstanceName="cbFSIdentityVerificationCode" Value='<%# Bind("FSIdentityVerificationCode") %>'></dx:ASPxComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <dx:ASPxLabel ID="lbSeperateFBUnabletoPrepareMeals" runat="server" Text="Separate FB - Unable to Prepare Meals" AssociatedControlID="cbSeperateFBUnabletoPrepareMeals"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxComboBox ID="cbSeperateFBUnabletoPrepareMeals" TabIndex="11" runat="server" IncrementalFilteringMode="StartsWith" ValueType="System.Boolean" Value='<%# Bind("UnabletoPrepareMealsIndicator") %>'></dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                        <br />
                        </td>
                        </tr>
                        <tr>
                            <td class="lengthyLabelTD">
                                <dx:ASPxLabel ID="lbDelayBefenifitsQuest" runat="server" Text="Did DSS cause the delay in processing benefits requested by <b>{Name}</b> by asking for additional information or another reason?" EncodeHtml="false" CssClass="lengthyLabel" AssociatedControlID="cbDelayBefenifitsQuest" ClientInstanceName="lbDelayBefenifitsQuest" OnDataBound="DelayBefenifitsQuest_DataBound"></dx:ASPxLabel>
                            </td>
                            <td class="asteriskTD"></td>
                            <td class="lengthyLabelControlTD">
                                <dx:ASPxComboBox ID="cbDelayBefenifitsQuest" TabIndex="12" runat="server" IncrementalFilteringMode="StartsWith" ValueType="System.Boolean" ClientInstanceName="cbDelayBefenifitsQuest" Value='<%# Bind("DSSDelayReasonIndicator") %>'>
                                    <ClientSideEvents Validation="ValidateDSSDelayBenefit" />
                                </dx:ASPxComboBox>
                            </td>
                        </tr>
                    </EditItemTemplate>
                </asp:FormView>
            </td>
        </tr>
        <tr>
            <td>
                <asp:FormView runat="server" ID="fvTechnical_MedicalAssistance" CssClass="SectionTable"
                    DefaultMode="Edit" DataSourceID="dsTechnical_MedicalAssistanceProgram" DataKeyNames="MedicalAssistanceProgramID" Visible="false" OnDataBound="Technical_MedicalAssistance_DataBound" OnItemUpdating="FvTechnical_MedicalAssistance_ItemUpdating">
                    <EditItemTemplate>
                        <asp:HiddenField ID="hfCurrentRetroMAflag" runat="server" Value='<%# Bind("RetroMACode") %>'></asp:HiddenField>
                        <asp:HiddenField ID="hfEligibilitySummaryCount" runat="server" Value="" />
                        <table class="SectionTable">
                            <tr>
                                <td>
                                    <dx:ASPxLabel ID="lblMedicalCRDP" runat="server" EncodeHtml="False" Text="CRDP" AssociatedControlID="cbMedicalCRDP"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxComboBox ID="cbMedicalCRDP" TabIndex="13" runat="server" IncrementalFilteringMode="StartsWith" ValueType="System.String" Value='<%# Bind("CRDPCode") %>'></dx:ASPxComboBox>
                                </td>
                                <td>
                                    <div id="dRMA" onmouseover="ShowPopUp()" onmouseout="HidePopUp()">
                                        <dx:ASPxLabel ID="lbMedicalRetroMA" runat="server" IncrementalFilteringMode="StartsWith" EncodeHtml="False" Text="Retro MA" AssociatedControlID="cbMedicalRetroMA"></dx:ASPxLabel>
                                    </div>
                                    <div id="RMA_popup" class="retroMAHelpPopup" style="display: none;">
                                        <b>Retro MA</b>
                                        <br />
                                        <br />
                                        <ul>

                                            <li>Individuals may be eligible for Medicaid 2 months prior to their application date if they received a Medicaid covered service and were eligible at the time of the service.
                                            </li>
                                        </ul>
                                    </div>
                                </td>
                                <td>
                                    <dx:ASPxComboBox ID="cbMedicalRetroMA" TabIndex="14"  runat="server" IncrementalFilteringMode="StartsWith" ValueType="System.String" Value='<%# Bind("RetroMACode") %>'></dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </EditItemTemplate>
                </asp:FormView>
            </td>
        </tr>
        <tr>
            <td>
                <asp:FormView runat="server" ID="fvTechnical_QMB" CssClass="SectionTable" Visible="false"
                    DefaultMode="Edit" DataSourceID="dsTechnical_QMBProgram" DataKeyNames="QMBProgramID" OnDataBound="Technical_QMB_DataBound" OnItemUpdating="FvTechnical_QMB_ItemUpdating">
                    <EditItemTemplate>
                         <asp:HiddenField ID="hfCurrentQMBRetroMAflag"  runat="server" Value='<%# Bind("RetroMACode") %>'></asp:HiddenField>
                          <asp:HiddenField ID ="hfQMBEligibilitySummaryCount" runat="server" Value="" />
                        <table class="SectionTable">
                            <tr>
                                <td>
                                    <dx:ASPxLabel ID="lblQMBCRDP" runat="server" Text="CRDP" AssociatedControlID="cbQMBProgramCRDP"></dx:ASPxLabel>
                                </td>
                                <td>
                                    <dx:ASPxComboBox ID="cbQMBProgramCRDP" TabIndex="15" runat="server" IncrementalFilteringMode="StartsWith" ValueType="System.String" Value='<%# Bind("CRDPCode") %>'></dx:ASPxComboBox>
                                </td>
                                   <td>
                                    <div id="dRMA" onmouseover="ShowPopUp()" onmouseout="HidePopUp()">
                                        <dx:ASPxLabel ID="lbQMBProgramRetroMA" runat="server" IncrementalFilteringMode="StartsWith" EncodeHtml="False" Text="Retro MSP (Except QMB)" AssociatedControlID="cbQMBProgramRetroMA"></dx:ASPxLabel>
                                    </div>                                               
                                    <div id="RMA_popup" class="retroMAHelpPopup" style="display: none;">
                                        <b>Retro MSP (Except QMB)</b>
                                        <br />
                                        <br />
                                        <ul>
                                           
                                            <li>
                                                Individuals may be eligible for Medicaid 3 months prior to their application date if they received a Medicaid covered service and were eligible at the time of the service.
                                            </li>
                                        </ul>
                                    </div>
                                </td>
                                <td>
                                 <dx:ASPxComboBox ID="cbQMBProgramRetroMA" TabIndex="16" runat="server" IncrementalFilteringMode="StartsWith" ValueType="System.String" Value='<%# Bind("RetroMACode") %>'></dx:ASPxComboBox>
                                </td>
                               <td class="SectionTableSingleColumn"></td>
                            </tr>
                        </table>
                    </EditItemTemplate>
                </asp:FormView>
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <table class="AssistanceSection">
                    <tr class="SpaceUnder">
                        <td class="AssistanceSectionLeftTD">
                            <dx:ASPxLabel ID="ASPxLabel7" runat="server" EncodeHtml="False" Text="Individuals" AssociatedControlID="lstChosenIndividuals" CssClass="assistanceIndividualDisplayLabel01"></dx:ASPxLabel>
                        </td>
                        <td class="AssistanceSectionRightTD">
                            <table class="AssistanceSection">
                                <tr id="QMBCheckList" runat="server">
                                    <td>
                                        <dx:ASPxLabel ID="lblSelect5" runat="server" Text="Select: " SkinID="RightAlignSelectLabel" EnableViewState="false"></dx:ASPxLabel>
                                    </td>
                                    <td>
                                        <dx:ASPxButton ID="btnIndividual" ClientInstanceName="btnIndividual" runat="server" Text="Individual(s)" SkinID="HyperLinkStyleBtn" RenderMode="Link" TabIndex="16" AutoPostBack="false" CausesValidation="false"></dx:ASPxButton>
                                    </td>
                                    <td class="seperatorPadding">
                                        <dx:ASPxLabel ID="lblSeparator11" runat="server" Text=" | " ForeColor="#8A2A2A" EnableViewState="false"></dx:ASPxLabel>
                                    </td>
                                    <td>
                                        <dx:ASPxButton ID="btnAll" runat="server" ClientInstanceName="btnAll" Text="All" SkinID="HyperLinkStyleBtn" AutoPostBack="false" CausesValidation="false" TabIndex="17" RenderMode="Link">
                                            <ClientSideEvents Click="function(s, e) { SelectAllAtIndex(0); }" />
                                        </dx:ASPxButton>
                                    </td>
                                    <td class="seperatorPadding">
                                        <dx:ASPxLabel ID="lblSeparator12" runat="server" Text=" | " ForeColor="#8A2A2A" EnableViewState="false"></dx:ASPxLabel>
                                    </td>
                                    <td>
                                        <dx:ASPxButton ID="btnClearAll" runat="server" ClientInstanceName="btnClearAll" Text="Clear All" SkinID="HyperLinkStyleBtn" AutoPostBack="false" RenderMode="Link" CausesValidation="false" TabIndex="18">
                                            <ClientSideEvents Click="function(s, e) { ClearAllAtIndex(0); }" />
                                        </dx:ASPxButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table class="AssistanceSection">
                    <tr>
                        <td colspan="2">
                            <dx:ASPxListBox ID="lstChosenIndividuals" runat="server" ValueType="System.String" ClientInstanceName="lstChosenIndividuals" TextField="Value" ValueField="Key" CssClass="assistanceIndividualDisplayListLabel" Rows="5" Height="200px" Width="200px" OnDataBound="LstChosenIndividuals_DataBound" ItemStyle-Wrap="True"></dx:ASPxListBox>
                        </td>
                        <td id="Td6" class="assistanceRecordDisplayLink01"></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <dx:ASPxPopupControl ID="dxPopupErr" ClientInstanceName="pcErrorPopup" SkinID="ErrorPopUp" Modal="true" CloseAction="CloseButton" runat="server" ShowOnPageLoad="false"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" AllowDragging="true" Width="400px" ShowFooter="true" HeaderText="Error Title">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
                <dx:ASPxLabel ID="lblErrmessage" runat="server" />
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterTemplate>
            <div style="float: right; margin: 3px">
                <dx:ASPxButton ID="btnOk" runat="server" Text="OK" ClientSideEvents-Click="function(s,e) {pcErrorPopup.Hide()}" AutoPostBack="false" IgnoreFgs="T" SkinID="footerPrimary" CausesValidation="false"></dx:ASPxButton>
            </div>
        </FooterTemplate>
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

    <dx:ASPxPopupControl ID="popUpWindow" ClientInstanceName="popUpWindow" runat="server" SkinID="ErrorPopup"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ShowFooter="true">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl2" runat="server">
                <dx:ASPxLabel ID="lblMessage" ClientInstanceName="lblMessage" runat="server" />
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterContentTemplate>
            <div style="float: right;">
                <asp:Panel ID="btnOkpanel" DefaultButton="btnOk" runat="server">
                    <dx:ASPxButton ID="btnOk" ClientInstanceName="btnOk" runat="server" Text="OK" CausesValidation="false" AutoPostBack="false" ClientSideEvents-Click="function(s,e) {popUpWindow.Hide();}" IgnoreFgs="T" SkinID="footerPrimary"></dx:ASPxButton>
                </asp:Panel>
            </div>
        </FooterContentTemplate>
        <ClientSideEvents Shown="function(s, e) {btnOk.Focus(); return false;}" />
    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl ID="poainformationpopup" ClientInstanceName="poainformationpopup" SkinID="ErrorPopUp" Modal="true" CloseAction="CloseButton" runat="server" ShowOnPageLoad="false"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" AllowDragging="true" Width="400px" ShowFooter="true" HeaderText="Error Title">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl3" runat="server">
                <dx:ASPxLabel ID="lblInformationMessage" runat="server" />
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterTemplate>
            <div style="float: right; margin: 3px">
                <dx:ASPxButton ID="btnOk" runat="server" Text="OK" ClientSideEvents-Click="function(s,e) {pcErrorPopup.Hide()}" AutoPostBack="true" IgnoreFgs="T" SkinID="footerPrimary" CausesValidation="false" OnClick="btnOk_Click"></dx:ASPxButton>
            </div>
        </FooterTemplate>
    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl ID="AddCommentPopUpPOADetail" runat="server" CloseAction="OuterMouseClick"
        AllowResize="false" FooterText=" " PopupElementID="ShowButton" PopupVerticalAlign="WindowCenter"
        PopupHorizontalAlign="WindowCenter" AllowDragging="True" Width="750px" ShowOnPageLoad="false"
        Height="350px" HeaderText="Program of Assistance Request Removed" Modal="true" ClientInstanceName="AddCommentPopUpPOADetail" OnWindowCallback="AddCommentPopUpPOADetail_WindowCallback"
        ClientSideEvents-EndCallback="PopUpEndCallBack">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl5" runat="server">
                <div style="color: red; font-weight: bold;">
                    <p>
                        You removed the Program of Assistance request for one or more individuals on this case. Do not use this feature in the following instances and 
                                update revelant information on associated screens & rerun eligibility instead or the client will receive an improper notice:
                    </p>

                    <ul style="list-style: disc; margin-left: 20px;">
                        <li>Individual deceased</li>
                        <li>Individual moved out of the State or out of the home</li>
                        <li>Individual is receiving benefits through their employer</li>
                        <li>Individual did not provide requested verificiation</li>
                    </ul>

                    <hr />

                    <p>
                        If an individual requested their case to be closed/withdraw their benefit request, you can still use the Program of Assistance Summary or 
                            Details Screens. Please enter a detailed Case Comment to proceed.
                    </p>

                </div>
                <br />
                <table class="SectionTable">

                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblWorkerName" runat="server" Text="Worker Name" EncodeHtml="false">
                            </dx:ASPxLabel>
                        </td>
                        <td>
                            <dx:ASPxLabel ID="lblCurrentUserID" runat="server" ClientInstanceName="lblCurrentUserID" CssClass="identificationRecordDisplayLabel leftLabel">
                            </dx:ASPxLabel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblTitle" runat="server" Text="Title" EncodeHtml="false">
                            </dx:ASPxLabel>
                        </td>
                        <td>
                            <dx:ASPxLabel ID="lblTitleValue" runat="server" ClientInstanceName="lblTitleValueCL" EncodeHtml="false" Text="POA Request Removed" CssClass="leftLabel">
                            </dx:ASPxLabel>
                            <%--<dx:ASPxTextBox ID="txtTitleCL" ClientInstanceName="txtTitleCL" runat="server" MaxLength="30" Width="305" SkinID="Address" AutoPostBack="false">
                    <ValidationSettings SetFocusOnError="True" ValidationGroup="AddNotePopupError" CausesValidation="false">
                        <RequiredField IsRequired="True" ErrorText="Title is Required" />
                    </ValidationSettings>
                </dx:ASPxTextBox>--%>
                        </td>
                        <td>
                            <dx:ASPxLabel ID="lblRemarkDate" runat="server" Text="Date" EncodeHtml="false">
                            </dx:ASPxLabel>
                        </td>
                        <td>
                            <dx:ASPxDateEdit ID="dtRemarkDate" runat="server" EditFormatString="MM/dd/yyyy" ClientInstanceName="dtRemarkDate" ValidationSettings-CausesValidation="false" Enabled="false">
                            </dx:ASPxDateEdit>
                            <%-- <dx:ASPxLabel ID="lblRemarkDateValue" runat="server" ClientInstanceName="lblRemarkDateValue" ></dx:ASPxLabel> --%>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblRemarkPageName" runat="server" Text="Page Name">
                            </dx:ASPxLabel>
                        </td>
                        <td>
                            <dx:ASPxLabel ID="lblRemarkPageNameValue" runat="server" ClientInstanceName="lblRemarkPageNameValueCL" Text="Program of Assistance" CssClass="leftLabel">
                            </dx:ASPxLabel>
                            <%--<dx:ASPxComboBox runat="server" ID="cmbPageNameCL" ValidationSettings-CausesValidation="false" ClientInstanceName="cmbPageNameCL" IncrementalFilteringMode="StartsWith" DropDownStyle="DropDownList"></dx:ASPxComboBox>--%>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblNotes" runat="server" Text="Notes" AssociatedControlID="MemRemark" CssClass="required">
                            </dx:ASPxLabel>
                        </td>
                        <td colspan="8" id="ContainerToCheck">
                            <dx:ASPxMemo ID="MemRemark" ClientInstanceName="MemRemarkCL" runat="server" Height="100Px" Width="100%" AutoPostBack="false" Columns="80" Rows="6" SkinID="Address">
                                <ValidationSettings SetFocusOnError="True" ValidationGroup="AddNotePopupError" CausesValidation="false">
                                    <RequiredField IsRequired="True" ErrorText="Comment is Required" />
                                </ValidationSettings>

                            </dx:ASPxMemo>
                        </td>
                        <td class="SectionTableSingleColumn"></td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td style="padding-left: 500px">
                            <dx:ASPxButton ID="CaseRemarkSave" runat="server" Text="Save" Width="80px" AutoPostBack="false" ClientSideEvents-Click="OnCommentSavedCl" CausesValidation="false" />
                        </td>
                        <td style="padding-left: 10px; padding-right: 20px">
                            <dx:ASPxButton AutoPostBack="false" ID="CaseRemarkCancel" CausesValidation="false" runat="server" Text="Cancel" SkinID="SecondaryButton" ClientSideEvents-Click="function(s,e) { e.processOnServer = false;  ASPxClientEdit.ClearGroup('AddNotePopupError');AddCommentPopUpPOADetail.Hide();}"
                                Width="80px" />
                        </td>
                    </tr>

                </table>
            </dx:PopupControlContentControl>

        </ContentCollection>
    </dx:ASPxPopupControl>
</asp:Content>
