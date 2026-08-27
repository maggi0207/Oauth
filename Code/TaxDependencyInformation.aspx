<%@ Page Language="C#" MasterPageFile="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" AutoEventWireup="True" CodeBehind="TaxDependencyInformation.aspx.cs"
    Inherits="Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical.TaxDependencyInformation"
    Title="Tax Dependency" %>

<%@ MasterType VirtualPath="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" %>

<asp:Content ID="ctPageBody" ContentPlaceHolderID="PageBodyContent" runat="server">
    <script src='<%= ResolveUrl(Dhss.Assist.WorkerWeb.Web.Infrastructure.Helpers.JsVersioningHelper.Tag("~/Intake/ApplicationEntry/Technical/TechnicalCommon.js")) %>'></script>

    <script type="text/javascript">
        var selectedElementIndex;
        function popup_Popup(s, e) {
            var popupElement = s.GetCurrentPopupElement();
            DisplayPopupAtIndex(popupElement)
        }

        //$('#CheckListBlockTable').find('input, textarea, button, select').removeAttr("disabled");

        function DisplayPopupAtIndex(popupElement) {
            if (typeof (cklIndividuals) != "undefined") {
                if (popupElement.id == btnIndividual.name) {
                    DisplayListItems(cklIndividuals, lstChosenIndividuals);
                }
            }

        }
        function SelectAllAtIndex(selectedIndex) {
            if (typeof (cklIndividuals) != "undefined") {
                MoveAllItems(cklIndividuals, lstChosenIndividuals);
            }
        }
        function ClearAllAtIndex(selectedIndex) {
            if (typeof (cklIndividuals) != "undefined") {
                ClearAllItems(cklIndividuals, lstChosenIndividuals);
            }
        }

        function SelectUnselectIndividual(s) {
            if (typeof (cklIndividuals) != "undefined") {
                if (cklIndividuals.GetSelectedItems().length > 0) {
                    MoveSelectedItems(cklIndividuals, lstChosenIndividuals);
                }
                else {
                    lstChosenIndividuals.ClearItems();
                }
            }
        }
        //Move all items
        function MoveAllItems(srcListBox, dstListBox) {
            var count = srcListBox.GetItemCount();
            dstListBox.ClearItems();
            for (var i = 0; i < count; i++) {
                var item = srcListBox.GetItem(i);
                dstListBox.AddItem(item.text, item.value);
            }
            //Check all
            dstListBox.SetSelectedIndex(0);
            srcListBox.SelectAll();
        }

        //Clear All items
        function ClearAllItems(srcListBox, dstListBox) {
            dstListBox.ClearItems();
            //Uncheck all

            srcListBox.UnselectAll();
        }
        //Move Selected items
        function MoveSelectedItems(srcListBox, dstListBox) {
            var items = srcListBox.GetSelectedItems();
            dstListBox.ClearItems();
            for (var i = 0; i <= items.length - 1; i = i + 1) {
                dstListBox.AddItem(items[i].text, items[i].value);
                dstListBox.SetSelectedIndex(0);
            }
        }
        //Select already available items
        function DisplayListItems(srcListBox, dstListBox) {
            var count = dstListBox.GetItemCount();
            var indx = new Array();
            srcListBox.UnselectAll();
            for (var i = 0; i < count; i++) {
                var item = dstListBox.GetItem(i);
                indx[i] = srcListBox.FindItemIndexByValue(item.value);
            }
            dstListBox.SetSelectedIndex(0);
            srcListBox.SelectIndices(indx);
        }

        function FileTaxReturnInCurrentYearIndicator(s) {

            if (cbFileTaxReturnInCurrentYearIndicator.GetValue() != null & cbFileTaxReturnInCurrentYearIndicator.GetValue() == "Y") {

                cbPrimaryTaxFilerIndicator.SetValue(null);
                cbHasTaxDeductionIndicator.SetValue(null);

                cbPrimaryTaxFilerIndicator.SetEnabled(true);
                cbHasTaxDeductionIndicator.SetEnabled(true);
            }
            else if (cbFileTaxReturnInCurrentYearIndicator.GetValue() != null) {

                cbPrimaryTaxFilerIndicator.SetValue("0");
                cbHasTaxDeductionIndicator.SetValue("0");

                cbPrimaryTaxFilerIndicator.SetEnabled(false);
                cbHasTaxDeductionIndicator.SetEnabled(false);

                ClearAllAtIndex(0);

                btnIndividual.SetEnabled(false);
                btnAll.SetEnabled(false);
                btnClearAll.SetEnabled(false);
            }
            else {

                cbPrimaryTaxFilerIndicator.SetValue(null);
                cbHasTaxDeductionIndicator.SetValue(null);

                ClearAllAtIndex(0);
                btnIndividual.SetEnabled(false);
                btnAll.SetEnabled(false);
                btnClearAll.SetEnabled(false);
            }
        }

        function PrimaryTaxFilerIndicator(s) {

            if (cbPrimaryTaxFilerIndicator.GetValue() != null & cbPrimaryTaxFilerIndicator.GetValue() == true) {
                btnIndividual.SetEnabled(true);
                btnAll.SetEnabled(true);
                btnClearAll.SetEnabled(true);
            }
            else if (cbPrimaryTaxFilerIndicator.GetValue() != null & cbPrimaryTaxFilerIndicator.GetValue() == false) {

                btnIndividual.SetEnabled(false);
                btnAll.SetEnabled(false);
                btnClearAll.SetEnabled(false);
                ClearAllAtIndex(0);
            }
        }

    </script>

    <dx:ASPxPopupControl ID="ASPxPopupClientControl" runat="server" CloseAction="OuterMouseClick" ShowOnPageLoad="False"
        PopupElementID="btnIndividual"
        PopupVerticalAlign="Below" PopupHorizontalAlign="LeftSides" AllowDragging="True"
        Width="250px" Height="130px" HeaderText="Individuals" ClientInstanceName="ASPxPopupClientControl1">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl" runat="server">
                <div style="vertical-align: middle">
                    <dx:ASPxCheckBoxList ID="cklIndividuals" runat="server" ClientInstanceName="cklIndividuals" ValueField="ApplicationEntityID" TextField="Name" ImageUrlField="ImgSrc" ItemImage-Height="16" ItemImage-Width="16">
                        <ClientSideEvents SelectedIndexChanged="function(s, e) { SelectUnselectIndividual(s); }" />
                    </dx:ASPxCheckBoxList>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
        <ClientSideEvents PopUp="popup_Popup" />
    </dx:ASPxPopupControl>
    <dhss:DataServiceLinqDataSource runat="server" ID="DsTechnical_TaxDependency" EnableUpdate="True" OnSelecting="DsTechnical_TaxDependency_Selecting"
        TableName="Technical_TaxDependency" EntityTypeName="Technical_TaxDependency"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.TechnicalContextImpl">
    </dhss:DataServiceLinqDataSource>
   
    <table class="ContentTable">
        <tr>
            <td>
                <dx:ASPxLabel ID="lblTaxDependencyDetails" runat="server" Text="Tax Dependency Details" SkinID="Header"></dx:ASPxLabel>
                <hr />
            </td>
        </tr>
        <tr>
            <td>
                <dx:ASPxButton runat="server"
                    ID="btnBackToSummary"
                    SkinID="HyperLinkStyleBtn"
                    OnClick="BtnBackToSummary_Click"
                    EncodeHtml="false"
                    CausesValidation="false" IgnoreFgs="T"
                    Text="< Back to Summary" />
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <asp:FormView runat="server" ID="fvTechnical_TaxDependency" DefaultMode="Edit" DataSourceID="DsTechnical_TaxDependency" DataKeyNames="TaxDependentID" OnDataBound="FvTechnical_TaxDependency_DataBound" OnItemUpdating="FvTechnical_TaxDependency_ItemUpdating">
                    <EditItemTemplate>
                        <asp:HiddenField runat="server" ID="serverdt" EnableViewState="true" ClientIDMode="Static" />
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
                                    <dx:ASPxLabel ID="lblRecordUpdatedDate1" runat="server" Text='<%# Eval("DB2UpdatedDate","{0:MM/dd/yyyy}") %>' SkinID="LeftLabel"></dx:ASPxLabel>
                                </td>
                            </tr>
                        </table>
                        <hr />
                        <tr>
                            <td>
                                <table class="SectionTable">
                                    <tr class="spaceUnder">
                                        <td>
                                            <dx:ASPxLabel ID="lblName" runat="server" Text="Name" AssociatedControlID="lblName1"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblName1" runat="server" Value='<%# Eval("ApplicationEntityID") %>' SkinID="LeftLabel"></dx:ASPxLabel>
                                        </td>
                                    </tr>
                                    <tr class="spaceUnder">
                                        <td>
                                            <dx:ASPxLabel ID="lblBeginDate" AssociatedControlID="ddeBeginDate" runat="server" Text="Begin Date" EncodeHtml="false" Width="65px" />
                                        </td>
                                        <td>
                                            <dx:ASPxDateEdit ID="ddeBeginDate" ClientIDMode="Static" EditFormatString="MM/yyyy" runat="server" Value='<%# Bind("BeginDate") %>' ClientInstanceName="ddeBeginDate" AutoPostBack="false" SkinID="RetrieveClear" TabIndex="1">
                                               <ClientSideEvents LostFocus="function(s,e) {var x = true; x = DateLostFocus(s,'ddeBeginDate'); e.processOnServer = x; if(x){InitializeEndDate(s,'ddeBeginDate');}}"  DropDown="function(s,e){calenderClick(s,'ddeBeginDate'); InitializeEndDate(s,'ddeBeginDate');}" Init="function(s,e) {InitializeStartDate(s,'ddeBeginDate') }"   />
                                            </dx:ASPxDateEdit>
                                        </td>
                                        <td colspan="13">
                                            <dx:ASPxLabel ID="lblEndDate" AssociatedControlID="ddeEndDate" runat="server" Text="End Date" Width="70px"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxDateEdit ID="ddeEndDate" ClientIDMode="Static" EditFormatString="MM/yyyy" runat="server" Value='<%# Bind("EndDate") %>' SkinID="RetrieveClear" ClientInstanceName="ddeEndDate" TabIndex="2">
                                                <ClientSideEvents LostFocus="function(s,e) {DateLostFocus(s,'ddeEndDate');}"   DropDown="function(s,e){calenderClick(s,'ddeEndDate');}" Init="function(s,e) {InitializeEndDate(s,'ddeEndDate');}" />
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
                                        <td class="lengthyLabelTD">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <dx:ASPxLabel ID="lblFileTaxReturnInCurrentYearIndicator" AssociatedControlID="cbFileTaxReturnInCurrentYearIndicator" runat="server" Text="Plan to file tax return for current year?" EncodeHtml="false" ClientInstanceName="lblFileTaxReturnInCurrentYearIndicator"></dx:ASPxLabel>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="lengthyLabelControlTD">
                                            <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" ID="cbFileTaxReturnInCurrentYearIndicator" runat="server" ValueType="System.String" Value='<%# Bind("FileTaxReturnInCurrentYearIndicator") %>' AutoPostBack="false" OnSelectedIndexChanged="CbFileTaxReturnInCurrentYearIndicator_SelectedIndexChanged" ClientInstanceName="cbFileTaxReturnInCurrentYearIndicator" TabIndex="3">
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { FileTaxReturnInCurrentYearIndicator(s); }" />
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
                                        <td class="lengthyLabelTD">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <dx:ASPxLabel ID="lblPrimaryTaxFilerIndicator" AssociatedControlID="cbPrimaryTaxFilerIndicator" runat="server" Text="Are you a primary filer?" EncodeHtml="false" ClientInstanceName="lblPrimaryTaxFilerIndicator"></dx:ASPxLabel>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="lengthyLabelControlTD">
                                            <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" ID="cbPrimaryTaxFilerIndicator" runat="server" ValueType="System.Boolean" Value='<%# Bind("PrimaryTaxFilerIndicator") %>' AutoPostBack="false" OnSelectedIndexChanged="CbPrimaryTaxFilerIndicator_SelectedIndexChanged" ClientInstanceName="cbPrimaryTaxFilerIndicator" TabIndex="4">
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { PrimaryTaxFilerIndicator(s); }" />
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
                                        <td class="lengthyLabelTD">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <dx:ASPxLabel ID="lblHasTaxDeductionIndicator" AssociatedControlID="cbHasTaxDeductionIndicator" ClientInstanceName="lblHasTaxDeductionIndicator" runat="server" Text="Do you have any tax deductions?" EncodeHtml="false"></dx:ASPxLabel>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td class="lengthyLabelControlTD">
                                            <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" Enabled="true" ID="cbHasTaxDeductionIndicator" ClientInstanceName="cbHasTaxDeductionIndicator" runat="server" ValueType="System.Boolean" Value='<%# Bind("HasTaxDeductionIndicator") %>' TabIndex="5"></dx:ASPxComboBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </EditItemTemplate>
                </asp:FormView>
            </td>
        </tr>
        <tr id="CheckListBlock" runat="server">
            <td>
                <br />
                <table class="AssistanceSection">
                    <tr class="SpaceUnder">
                        <td class="AssistanceSectionLeftTD">
                            <dx:ASPxLabel ID="lblPurchasePrepMealsWith" runat="server" Text="Who are you claiming as your tax dependent(s)?" EncodeHtml="false" AssociatedControlID="lstChosenIndividuals"></dx:ASPxLabel>
                        </td>
                        <td class="AssistanceSectionRightTD">
                            <table class="AssistanceSection">
                                <tr>
                                    <td>
                                        <dx:ASPxLabel ID="ASPxLabel1" runat="server" Text="Select: "></dx:ASPxLabel>
                                    </td>
                                    <td>
                                        <dx:ASPxButton ID="btnIndividual" ClientInstanceName="btnIndividual" runat="server" RenderMode="Link" Text="Individual(s)" SkinID="HyperLinkStyleBtn" AutoPostBack="false" CausesValidation="false" TabIndex="6"></dx:ASPxButton>
                                    </td>
                                    <td class="seperatorPadding">
                                        <dx:ASPxLabel ID="lblSeparator1" runat="server" Text=" | " ForeColor="#8A2A2A"></dx:ASPxLabel>
                                    </td>
                                    <td>
                                        <dx:ASPxButton ID="btnAll" runat="server" ClientInstanceName="btnAll" Text="All" RenderMode="Link"  SkinID="HyperLinkStyleBtn" AutoPostBack="false" CausesValidation="false" TabIndex="7">
                                            <ClientSideEvents Click="function(s, e) { SelectAllAtIndex(0); }" />
                                        </dx:ASPxButton>
                                    </td>
                                    <td class="seperatorPadding">
                                        <dx:ASPxLabel ID="lblSeparator2" runat="server" Text=" | " ForeColor="#8A2A2A"></dx:ASPxLabel>
                                    </td>
                                    <td>
                                        <dx:ASPxButton ID="btnClearAll" runat="server" ClientInstanceName="btnClearAll"  RenderMode="Link" Text="Clear All" SkinID="HyperLinkStyleBtn" AutoPostBack="false" CausesValidation="false" TabIndex="8">
                                            <ClientSideEvents Click="function(s, e) { ClearAllAtIndex(0); }" />
                                        </dx:ASPxButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxListBox ID="lstChosenIndividuals" runat="server" ValueType="System.String" ClientInstanceName="lstChosenIndividuals" TextField="Value" ValueField="Key"
                                CssClass="assistanceIndividualDisplayListLabel" Rows="5" Height="200px" Width="200px">
                            </dx:ASPxListBox>
                        </td>
                        <td id="Td6" class="assistanceRecordDisplayLink01"></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <dx:ASPxPopupControl ID="popUpWindow" ClientInstanceName="popUpWindow" runat="server" ShowCloseButton="false" SkinID="ErrorPopup"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ShowFooter="true">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl2" runat="server">
                <dx:ASPxLabel ID="lblMessage" ClientInstanceName="lblMessage" runat="server" />
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterContentTemplate>
            <div style="float: right;">
                 <asp:Panel ID="btnokpanel" DefaultButton="btnok" runat="server">
                <dx:ASPxButton ID="btnOk" ClientInstanceName="btnOk" runat="server" Text="OK" CausesValidation="false" AutoPostBack="false" ClientSideEvents-Click="function(s,e) {popUpWindow.Hide();}" IgnoreFgs="T" SkinID="footerPrimary">
                </dx:ASPxButton>
             </asp:Panel>
            </div>
        </FooterContentTemplate>
        <ClientSideEvents Shown="function(s, e) {btnok.Focus(); return false;}" /> 
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
                <dx:ASPxLabel runat="server" ID="lblmessage1" Width="280px" ClientInstanceName="lblmessage1" />
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
       <dx:ASPxPopupControl ID="dxPopupErr" ClientInstanceName="pcerrorpopup" SkinID="ErrorPopUp" Modal="true" CloseAction="CloseButton" runat="server" ShowOnPageLoad="false"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" AllowDragging="true" Width="400px" ShowFooter="true" HeaderText="Error Title">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
                <dx:ASPxLabel ID="lblErrmessage" runat="server" />
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterTemplate>
            <div style="float: right; margin: 3px">
                <asp:Panel ID="PanelFocus" runat="server" DefaultButton="btnOk">
                    <dx:ASPxButton ID="btnOk" runat="server" Text="OK" ClientSideEvents-Click="function(s,e) {pcerrorpopup.Hide()}" AutoPostBack="false" IgnoreFgs="T" SkinID="footerPrimary" CausesValidation="false" ClientInstanceName="btnOk"></dx:ASPxButton>
                </asp:Panel>
            </div>
        </FooterTemplate>
        <ClientSideEvents Shown="function(s, e) {btnOk.Focus(); return false;}" />
    </dx:ASPxPopupControl>
</asp:Content>
