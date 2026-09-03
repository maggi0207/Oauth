<%@ Page Title="Medicare Information Details" Language="C#" MasterPageFile="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" AutoEventWireup="True" CodeBehind="MedicareInformation.aspx.cs" Inherits="Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Expense.MedicareInformation" %>

<%@ MasterType VirtualPath="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" %>
<asp:Content ID="ctPageBody" ContentPlaceHolderID="PageBodyContent" runat="server">
    <script src='<%= ResolveUrl(Dhss.Assist.WorkerWeb.Web.Infrastructure.Helpers.JsVersioningHelper.Tag("~/Intake/ApplicationEntry/Technical/TechnicalCommon.js")) %>'></script>
    <script type="text/javascript">
        function ValidateSelection(s, e) {
            if (s.GetValue() != null && (s.GetValue() == "CV" || s.GetValue() == "FF" || s.GetValue() == "FP")) {
                s.SetValue(null);
                lblMessage.SetText("Conversion, Federal Hub or Failed Federal Hub cannot be selected. Please choose other Option.\n")
                popUpWindow.SetHeaderText('Error Message');
                popUpWindow.Show();
                popUpWindow.Focus();
            }
        }
        var msg;

        //Part A end date validation
        function CheckPartAEndDate(e) {
            if (dtMedicareStartDate.GetDate() != null) {
                if (dtEndDate.GetDate() != null) {
                    if (dtEndDate.GetDate() < dtMedicareStartDate.GetDate()) {
                        msg = 'End date cannot be less than start date.';
                        ShowPopup(msg);
                        dtEndDate.SetText(null);
                    }
                }
            }
            else {
                if (dtEndDate.GetDate() != null) {
                    var msg = 'Start date cannot be empty.';
                    currentID = "dtMedicareStartDate";
                    ShowPopup(msg);
                    dtEndDate.SetText(null);
                }
            }
        }

        //Part A start date validation
        function CheckPartAStartDate(e) {
            if (dtMedicareStartDate.GetDate() != null) {
                if (dtEndDate.GetDate() != null) {
                    if (dtEndDate.GetDate() < dtMedicareStartDate.GetDate()) {
                        msg = 'End date cannot be less than start date.';
                        dtMedicareStartDate.Focus();
                        ShowPopup(msg);
                        dtMedicareStartDate.SetText(null);
                    }
                }
            }
        }

        //Part B end date validation
        function CheckPartBEndDate(e) {
            if (dtMedicareStartDate1.GetDate() != null) {
                if (dtEndDate1.GetDate() != null) {
                    if (dtEndDate1.GetDate() < dtMedicareStartDate1.GetDate()) {
                        msg = 'End date cannot be less than start date.';
                        ShowPopup(msg);
                        dtEndDate1.SetText(null);
                    }
                }
            }
            else {
                if (dtEndDate1.GetDate() != null) {
                    msg = 'Start date cannot be empty.';
                    ShowPopup(msg);
                    currentID = "dtMedicareStartDate1";
                    dtEndDate1.SetText(null);
                }
            }
        }

        //Part B start date validation
        function CheckPartBStartDate(e) {
            if (dtMedicareStartDate1.GetDate() != null) {
                if (dtEndDate1.GetDate() != null) {
                    if (dtEndDate1.GetDate() < dtMedicareStartDate1.GetDate()) {
                        msg = 'End date cannot be less than start date.';
                        dtMedicareStartDate1.Focus();
                        ShowPopup(msg);
                        dtMedicareStartDate1.SetText(null);
                    }
                }
            }
        }


        //Part D end date validation
        function CheckPartDEndDate(e) {
            if (dtMedicareStartDate2.GetDate() != null) {
                if (dtEndDate2.GetDate() != null) {
                    if (dtEndDate2.GetDate() < dtMedicareStartDate2.GetDate()) {
                        msg = 'End date cannot be less than start date.';
                        ShowPopup(msg);
                        dtEndDate2.SetText(null);
                    }
                }
            }
            else {
                if (dtEndDate2.GetDate() != null) {
                    msg = 'Start date cannot be empty.';
                    ShowPopup(msg);
                    currentID = "dtMedicareStartDate2";
                    dtEndDate2.SetText(null);
                }
            }
        }

        //Part D start date validation
        function CheckPartDStartDate(e) {
            var minDate = new Date('January 01, 2006');
            if (dtMedicareStartDate2.GetDate() != null) {
                if (dtMedicareStartDate2.GetDate() < minDate && dtMedicareStartDate2.GetDate() != null) {
                    var msg = 'The Part D Starting date should be greater than or equal to 01/01/2006';
                    currentID = "dtMedicareStartDate2";
                    beforeID = "dtMedicareStartDate2";
                    dtMedicareStartDate2.SetText(null);
                    dtMedicareStartDate2.Focus();
                    ShowPopup(msg);
                }
                else {
                    if (dtEndDate2.GetDate() != null) {
                        if (dtEndDate2.GetDate() < dtMedicareStartDate2.GetDate()) {
                            msg = 'End date cannot be less than start date.';
                            currentID = "dtMedicareStartDate2";
                            beforeID = "dtMedicareStartDate2";
                            dtMedicareStartDate2.SetText(null);
                            dtMedicareStartDate2.Focus();
                            ShowPopup(msg);

                        }
                    }
                }
            }

        }

        //initializing Part A end date
        function InitializePartAEndDate(s, id) {

            var startdate = new Date();
            startdate = dtMedicareStartDate.GetDate();
            if (startdate != null) {
                dtEndDate.SetMinDate(GetDateWithFirstOrLastDayOfMonth(startdate, true));
            }
            else {
                dtEndDate.SetMinDate(GetDateWithFirstOrLastDayOfMonth(new Date(), true));
            }
        }

        //initializing Part B end date
        function InitializePartBEndDate(s, id) {

            var startdate = new Date();
            startdate = dtMedicareStartDate1.GetDate();
            if (startdate != null) {
                dtEndDate1.SetMinDate(GetDateWithFirstOrLastDayOfMonth(startdate, true));
            }
            else {
                dtEndDate1.SetMinDate(GetDateWithFirstOrLastDayOfMonth(new Date(), true));
            }
        }

        //initializing Part C end date
        function InitializePartDEndDate(s, id) {

            var startdate = new Date();
            startdate = dtMedicareStartDate2.GetDate();
            if (startdate != null) {
                dtEndDate2.SetMinDate(GetDateWithFirstOrLastDayOfMonth(startdate, true));
            }
            else {
                dtEndDate2.SetMinDate(GetDateWithFirstOrLastDayOfMonth(new Date(), true));
            }
        }

    </script>

    <dhss:DataServiceLinqDataSource
        runat="server"
        ID="dsExpense_MedicareExpense" EnableInsert="True" EnableUpdate="True"
        TableName="Expense_MedicareExpense"
        EntityTypeName="Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Expense.MedicareExpense.Expense_MedicareExpense"
        Expand="Expense"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.ExpenseContextImpl"
        OnSelecting="DsExpense_MedicareExpense_Selecting"
        OnUpdating="DsExpense_MedicareExpense_Updating">
    </dhss:DataServiceLinqDataSource>
    <dhss:DataServiceLinqDataSource
        runat="server"
        ID="dsExpense_MedicareExpensePartABDetailA" EnableInsert="True" EnableUpdate="True"
        TableName="Expense_MedicareExpensePartABDetail"
        EntityTypeName="Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Expense.MedicareExpense.Expense_MedicareExpensePartABDetail"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.ExpenseContextImpl"
        OnSelecting="DsExpense_MedicareExpensePartABDetailA_Selecting">
    </dhss:DataServiceLinqDataSource>
    <dhss:DataServiceLinqDataSource
        runat="server"
        ID="dsExpense_MedicareExpensePartABDetailB" EnableInsert="True" EnableUpdate="True"
        TableName="Expense_MedicareExpensePartABDetail"
        EntityTypeName="Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Expense.MedicareExpense.Expense_MedicareExpensePartABDetail"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.ExpenseContextImpl"
        OnSelecting="DsExpense_MedicareExpensePartABDetailB_Selecting">
    </dhss:DataServiceLinqDataSource>
    <dhss:DataServiceLinqDataSource
        runat="server"
        ID="dsExpense_MedicareExpensePartDDetail" EnableInsert="True" EnableUpdate="True"
        TableName="Expense_MedicareExpensePartDDetail"
        EntityTypeName="Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Expense.MedicareExpense.Expense_MedicareExpensePartDDetail"
        Expand="MedicareExpense/Expense"
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.ExpenseContextImpl"
        OnSelecting="DsExpense_MedicareExpensePartDDetail_Selecting">
    </dhss:DataServiceLinqDataSource>
                    <table class="ContentTable">
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblMedicareDetails" runat="server" Text="Medicare Details" SkinID="Header"></dx:ASPxLabel>
                        </td>
                        <td style="text-align:right">
                             <dx:ASPxButton runat="server" ID="btnDocumentImagingVerification" Visible="true" SkinID="HyperLinkStyleBtn" CausesValidation="false" AutoPostBack="true" OnClick="BtnDocumentImagingVerification_Click" Text="Document Imaging Verification"></dx:ASPxButton>
                        </td>
                    </tr>
                </table>
    <table class="ContentTable">
        <tr>
            <td>
                
                <hr />
            </td>
        </tr>
        <tr>
            <td class="floatLeft">
                <dx:ASPxButton ID="btnHyperlink" runat="server" Text="< Back to Summary" CausesValidation="false" Enabled="true" EncodeHtml="false" SkinID="HyperLinkStyleBtn" OnClick="BtnBackToSummary_Click" IgnoreFgs="T"></dx:ASPxButton>
            </td>
            <td class="floatRight">
                <dx:ASPxButton ID="btnOops" runat="server" Text="Oops >" Enabled="false" CausesValidation="false" SkinID="HyperLinkStyleBtn" EncodeHtml="false" OnClick="BtnOops_Click"></dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td>
                <asp:FormView runat="server" ID="fvExpense_MedicareExpense" DefaultMode="Edit" DataSourceID="dsExpense_MedicareExpense" DataKeyNames="ExpenseID" OnDataBound="FvExpense_MedicareExpense_DataBound" OnItemUpdating="fvExpense_MedicareExpense_ItemUpdating">
                    <EditItemTemplate>
                        <asp:HiddenField runat="server" ID="serverdt" EnableViewState="true" ClientIDMode="Static" />
                        <tr>
                            <td class="lengthyLabelControlTD">
                                <asp:HiddenField ID="hdMedicareNumber" Value='<%# Bind("MedicareNumber") %>' runat="server" />
                                <asp:HiddenField ID="hdMedicareBuyingEffectiveDate" Value='<%# Bind("MedicareBuyingEffectiveDate") %>' runat="server" />
                                <asp:HiddenField runat="server" ID="hfHistoryCode" Value='<%# Bind("HistoryCode") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table class="SectionTable">
                                    <tr>
                                        <td>
                                            <dx:ASPxLabel ID="lblHistorySequenceNumber" runat="server" Text="Record History Number:" AssociatedControlID="lblRecordHistoryNum"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblRecordHistoryNum" SkinID="LeftLabel" runat="server" Enabled="false" Text='<%# Eval("HistorySequenceNumber") %>'></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblRecordUpdatedDate" runat="server" Text="Record Updated Date:" AssociatedControlID="lblRecordUpdateDt"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblRecordUpdateDt" runat="server" SkinID="LeftLabel" Text='<%# String.Format("{0:MM/dd/yyyy}",Eval("DB2UpdatedDate"))%>' Width="80px"></dx:ASPxLabel>
                                        </td>
                                    </tr>
                                </table>
                                <hr />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table class="SectionTableThreeColumns">
                                    <tr>
                                        <td>
                                            <dx:ASPxLabel ID="lblName" runat="server" Text="Name" AssociatedControlID="cbName" EncodeHtml="false" />
                                        </td>
                                        <td>
                                            <dx:ASPxComboBox ID="cbName" TabIndex="1" runat="server" ClientEnabled="false" ClientInstanceName="cbName" OnSelectedIndexChanged="CbName_SelectedIndexChanged" AutoPostBack="true" ValueType="System.Int32" Value='<%# Eval("Expense.PersonID") %>'></dx:ASPxComboBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <dx:ASPxLabel ID="lblBeginDate" runat="server" Text="Begin Date" AssociatedControlID="ddeBeginDate" />
                                        </td>
                                        <td>
                                            <dx:ASPxDateEdit ID="ddeBeginDate" ClientIDMode="Static" TabIndex="2" runat="server" ClientInstanceName="ddeBeginDate" EditFormatString="MM/yyyy" Value='<%# Bind("BeginDate") %>' OnDataBound="ddeBeginDate_DataBound">
                                                <ClientSideEvents ValueChanged="function(s,e){var x = false; x =  formatmmyyyy(s, e, 'ddeBeginDate'); e.processOnServer = false;}" LostFocus="function(s,e) {DateLostFocus(s,'ddeBeginDate'); InitializeEndDate(s,'ddeEndDate');}"  DropDown="function(s,e){calenderClick(s,'ddeBeginDate');}" Init="function(s,e) {InitializeStartDate(s,'ddeBeginDate') }"   />
                                            </dx:ASPxDateEdit>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblEndDate" runat="server" Text="End Date" AssociatedControlID="ddeEndDate"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxDateEdit ID="ddeEndDate" ClientIDMode="Static" TabIndex="3" runat="server" EditFormatString="MM/yyyy" ClientInstanceName="ddeEndDate" Value='<%# Bind("EndDate") %>'>
                                                <ClientSideEvents LostFocus="function(s,e) {DateLostFocus(s,'ddeEndDate');}"   DropDown="function(s,e){calenderClick(s,'ddeEndDate');}" Init="function(s,e) {InitializeEndDate(s,'ddeEndDate');}"   />
                                            </dx:ASPxDateEdit>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblDeleteReasonCode" runat="server" Text="History Reason" AssociatedControlID="cbDeleteReasonCode"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxComboBox ID="cbDeleteReasonCode" TabIndex="4" ClientInstanceName="cbDeleteReasonCode" runat  ="server" Width="90px"  IncrementalFilteringMode="StartsWith" Value='<%# Bind("DeleteReasonCode") %>' OnDataBound="cbDeleteReasonCode_DataBound"></dx:ASPxComboBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </EditItemTemplate>
                </asp:FormView>
            </td>
        </tr>
        <tr>
            <td>
                <dx:ASPxLabel ID="lblPartAInformation" runat="server" Text="Part A Information" SkinID="InnerHeader"></dx:ASPxLabel>
                <hr />
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <asp:FormView runat="server" ID="fvExpense_MedicareExpensePartABDetailA" DefaultMode="Edit" DataSourceID="dsExpense_MedicareExpensePartABDetailA" OnItemUpdating="fvExpense_MedicareExpensePartABDetailA_ItemUpdating" DataKeyNames="ExpenseID" OnDataBound="FvExpense_MedicareExpensePartABDetailA_DataBound">
                    <EditItemTemplate>
                        <asp:HiddenField ID="hdMedicareIdPartA" Value='<%# Eval("MedicareExpenseDtlID") %>' runat="server" />
                        <tr>
                            <td>
                                <table class="SectionTable">
                                    <tr>
                                        <td>
                                            <dx:ASPxLabel ID="lblMedicareEntitledIndicator" runat="server" Text="Entitled?" AssociatedControlID="cbMedicareEntitledIndicatorA"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxComboBox ID="cbMedicareEntitledIndicatorA" TabIndex="5" runat="server" IncrementalFilteringMode="StartsWith" ValueType="System.Boolean" OnSelectedIndexChanged="CbMedicareEntitledIndicatorA_SelectedIndexChanged" AutoPostBack="true" Value='<%# Bind("MedicareEntitledIndicator") %>'></dx:ASPxComboBox>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblMedicareEntitlementVeificatonCodeA" runat="server" Text="Verified By" AssociatedControlID="cbMedicareEntitlementVeificatonCodeA"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <asp:HiddenField ID="hdMedicareEntitlementVeificatonCodeA" Value='<%# Eval("MedicareEntitlementVeificatonCode") %>' runat="server" />

                                            <dx:ASPxComboBox ID="cbMedicareEntitlementVeificatonCodeA" TabIndex="6" runat="server" IncrementalFilteringMode="StartsWith" ValueType="System.String" OnSelectedIndexChanged="CbMedicareEntitlementVeificatonCodeA_SelectedIndexChanged" Value='<%# Bind("MedicareEntitlementVeificatonCode") %>'>
                                                <ClientSideEvents SelectedIndexChanged="function(s,e){ ValidateSelection(s, e); }" />
                                            </dx:ASPxComboBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <dx:ASPxLabel ID="lblMedicareStartDate" runat="server" Text="Start Date" AssociatedControlID="dtMedicareStartDate"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxDateEdit ID="dtMedicareStartDate" ClientIDMode="Static" TabIndex="7" runat="server" AutoPostBack="false" ClientInstanceName="dtMedicareStartDate" EditFormatString="MM/dd/yyyy" Value='<%# Bind("MedicareStartDate") %>'>
                                                <ClientSideEvents ValueChanged="function(s,e) {CheckPartAStartDate(e);}" LostFocus="function(s,e) {DateLostFocus(s,'dtMedicareStartDate',false);  InitializePartAEndDate(s,'dtEndDate');}"  DropDown="function(s,e){calenderClick(s,'dtMedicareStartDate');}"   />
                                            </dx:ASPxDateEdit>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblEndDate1" runat="server" Text="End Date" AssociatedControlID="dtEndDate"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxDateEdit ID="dtEndDate" ClientIDMode="Static" TabIndex="8" runat="server" EditFormatString="MM/dd/yyyy" Value='<%# Bind("MedicareEndDate") %>' ClientInstanceName="dtEndDate">
                                                <ClientSideEvents ValueChanged="function(s,e) {CheckPartAEndDate(e);}" LostFocus="function(s,e) {DateLostFocus(s,'dtEndDate',false);}"  DropDown="function(s,e){calenderClick(s,'dtEndDate');}"   />
                                            </dx:ASPxDateEdit>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <dx:ASPxLabel ID="lblMedicarePremiumAmountA" runat="server" Text="Premium" AssociatedControlID="txtMedicarePremiumAmountA"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxTextBox ID="txtMedicarePremiumAmountA" TabIndex="9" runat="server" AutoPostBack="true" MaskSettings-Mask="$<0..999999g>.<00..99>" MaskSettings-IncludeLiterals="DecimalSymbol" OnTextChanged="TxtMedicarePremiumAmountA_TextChanged" Text='<%# Bind("MedicarePremiumAmount") %>'></dx:ASPxTextBox>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblMedicarePaidByCodeA" runat="server" Text="Paid By" AssociatedControlID="cbMedicarePaidByCodeA" EncodeHtml="false"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxComboBox ID="cbMedicarePaidByCodeA" TabIndex="10" runat="server" ValueType="System.String" IncrementalFilteringMode="StartsWith" Value='<%# Bind("MedicarePaidByCode") %>'></dx:ASPxComboBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </EditItemTemplate>
                </asp:FormView>
            </td>
        </tr>
        <tr>
            <td>
                <dx:ASPxLabel ID="lblPartBInformation" runat="server" Text="Part B Information" SkinID="InnerHeader"></dx:ASPxLabel>
                <hr />
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <asp:FormView runat="server" ID="fvExpense_MedicareExpensePartABDetailB" DefaultMode="Edit" DataSourceID="dsExpense_MedicareExpensePartABDetailB" OnItemUpdating="fvExpense_MedicareExpensePartABDetailB_ItemUpdating" DataKeyNames="ExpenseID" OnDataBound="FvExpense_MedicareExpensePartABDetailB_DataBound">
                    <EditItemTemplate>
                        <asp:HiddenField ID="hdMedicareIdPartB" Value='<%# Eval("MedicareExpenseDtlID") %>' runat="server" />
                        <tr>
                            <td>
                                <table class="SectionTable">
                                    <tr>
                                        <td>
                                            <dx:ASPxLabel ID="lblMedicareEntitledIndicatorB" runat="server" Text="Entitled?" AssociatedControlID="cbMedicareEntitledIndicatorB"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxComboBox ID="cbMedicareEntitledIndicatorB" TabIndex="11" runat="server" AutoPostBack="true" IncrementalFilteringMode="StartsWith" OnSelectedIndexChanged="CbMedicareEntitledIndicatorB_SelectedIndexChanged" ValueType="System.Boolean" Value='<%# Bind("MedicareEntitledIndicator") %>'></dx:ASPxComboBox>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblMedicareEntitlementVeificatonCodeB" runat="server" Text="Verified By" EncodeHtml="false" AssociatedControlID="cbMedicareEntitlementVeificatonCodeB"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <asp:HiddenField ID="hdMedicareEntitlementVeificatonCodeB" Value='<%# Eval("MedicareEntitlementVeificatonCode") %>' runat="server" />
                                            <dx:ASPxComboBox ID="cbMedicareEntitlementVeificatonCodeB" TabIndex="12" runat="server" IncrementalFilteringMode="StartsWith" OnSelectedIndexChanged="CbMedicareEntitlementVeificatonCodeB_SelectedIndexChanged" ValueType="System.String" Value='<%# Bind("MedicareEntitlementVeificatonCode") %>'>
                                                <ClientSideEvents SelectedIndexChanged="function(s,e){ ValidateSelection(s, e); }" />
                                            </dx:ASPxComboBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <dx:ASPxLabel ID="lblMedicareStartDate1" runat="server" Text="Start Date" AssociatedControlID="dtMedicareStartDate1"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                             <dx:ASPxDateEdit ID="dtMedicareStartDate1" ClientIDMode="Static" TabIndex="13" runat="server"   EditFormatString="MM/dd/yyyy" Value='<%# Bind("MedicareStartDate") %>' ClientInstanceName="dtMedicareStartDate1">                                                 
                                                  <ClientSideEvents ValueChanged="function(s,e) {CheckPartBStartDate(e);}" LostFocus="function(s,e) {DateLostFocus(s,'dtMedicareStartDate1',false); InitializePartBEndDate(s,'dtEndDate1');}"  DropDown="function(s,e){calenderClick(s,'dtMedicareStartDate1');}"   />
                                            </dx:ASPxDateEdit>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblEndDate2" runat="server" Text="End Date" AssociatedControlID="dtEndDate1"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxDateEdit ID="dtEndDate1" ClientIDMode="Static" TabIndex="14" runat="server" EditFormatString="MM/dd/yyyy" Value='<%# Bind("MedicareEndDate") %>' ClientInstanceName="dtEndDate1">
                                                 <ClientSideEvents ValueChanged="function(s,e) {CheckPartBEndDate(e);}" LostFocus="function(s,e) {DateLostFocus(s,'dtEndDate1',false);}"  DropDown="function(s,e){calenderClick(s,'dtEndDate1');}"   />
                                            </dx:ASPxDateEdit>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <dx:ASPxLabel ID="lblPremiumB" runat="server" Text="Premium" AssociatedControlID="txtMedicarePremiumAmountB"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxTextBox ID="txtMedicarePremiumAmountB" TabIndex="15" MaskSettings-Mask="$<0..999999g>.<00..99>" MaskSettings-IncludeLiterals="DecimalSymbol" OnTextChanged="TxtMedicarePremiumAmountB_TextChanged" AutoPostBack="true" runat="server" Text='<%# Bind("MedicarePremiumAmount") %>'></dx:ASPxTextBox>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblMedicarePaidByCodeB" runat="server" Text="Paid By" AssociatedControlID="cbMedicarePaidByCodeB"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxComboBox ID="cbMedicarePaidByCodeB" TabIndex="16" runat="server" IncrementalFilteringMode="StartsWith" ValueType="System.String" Value='<%# Bind("MedicarePaidByCode") %>'></dx:ASPxComboBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </EditItemTemplate>
                </asp:FormView>
            </td>
        </tr>
        <tr>
            <td>
                <dx:ASPxLabel ID="lblPartDInformation" runat="server" Text="Part D Information" SkinID="InnerHeader"></dx:ASPxLabel>
                <hr />
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <asp:FormView runat="server" ID="fvExpense_MedicareExpensePartDDetail" DefaultMode="Edit" DataSourceID="dsExpense_MedicareExpensePartDDetail" OnItemUpdating="fvExpense_MedicareExpensePartDDetail_ItemUpdating" DataKeyNames="ExpenseID" OnDataBound="FvExpense_MedicareExpensePartDDetail_DataBound">
                    <EditItemTemplate>
                        <tr>
                            <td>
                                <table class="SectionTable">
                                    <tr>
                                        <td>
                                            <dx:ASPxLabel ID="lblEnrolled" runat="server" Text="Enrolled?" EncodeHtml="false" AssociatedControlID="cbEnrolled"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxComboBox ID="cbEnrolled" TabIndex="17" runat="server" ValueType="System.String" IncrementalFilteringMode="StartsWith" AutoPostBack="true" OnSelectedIndexChanged="CbEnrolled_SelectedIndexChanged" Value='<%# Bind("MedicareEnrolledIndicator") %>'></dx:ASPxComboBox>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblMedicareEntitlementVeificatonCode2" runat="server" Text="Verified By" AssociatedControlID="cbMedicareEntitlementVeificatonCode2"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <asp:HiddenField ID="hdMedicareEntitlementVeificatonCode2" Value='<%# Eval("MedicareEntitlementVeificatonCode") %>' runat="server" />
                                            <dx:ASPxComboBox ID="cbMedicareEntitlementVeificatonCode2" TabIndex="18" runat="server" IncrementalFilteringMode="StartsWith" OnSelectedIndexChanged="CbMedicareEntitlementVeificatonCode2_SelectedIndexChanged" ValueType="System.String" Value='<%# Bind("MedicareEntitlementVeificatonCode") %>'>
                                                 <ClientSideEvents SelectedIndexChanged="function(s,e){ ValidateSelection(s, e); }" />
                                            </dx:ASPxComboBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <dx:ASPxLabel ID="lblMedicareStartDate2" runat="server" Text="Start Date" AssociatedControlID="dtMedicareStartDate2"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                           <dx:ASPxDateEdit ID="dtMedicareStartDate2" ClientIDMode="Static" TabIndex="19" runat="server"   EditFormatString="MM/dd/yyyy" Value='<%# Bind("MedicareStartDate") %>'  ClientInstanceName="dtMedicareStartDate2">                                               
                                                <ClientSideEvents ValueChanged="function(s,e) {CheckPartDStartDate(e);}" LostFocus="function(s,e) {DateLostFocus(s,'dtMedicareStartDate2',false); InitializePartDEndDate(s,'dtEndDate2');}"  DropDown="function(s,e){calenderClick(s,'dtMedicareStartDate2');}"   />
                                            </dx:ASPxDateEdit>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblEndDate3" runat="server" Text="End Date" AssociatedControlID="dtEndDate2"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                              <dx:ASPxDateEdit ID="dtEndDate2" ClientIDMode="Static" TabIndex="20" runat="server" EditFormatString="MM/dd/yyyy" Value='<%# Bind("MedicareEndDate") %>' ClientInstanceName="dtEndDate2">
                                                  <ClientSideEvents ValueChanged="function(s,e) {CheckPartDEndDate(e);}" LostFocus="function(s,e) {DateLostFocus(s,'dtEndDate2',false);}"  DropDown="function(s,e){calenderClick(s,'dtEndDate2');}"   />
                                            </dx:ASPxDateEdit>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <dx:ASPxLabel ID="lblCreditableCoverageIndicator" runat="server" Text="Creditable Coverage Indicator" AssociatedControlID="cbCreditableCoverageIndicator"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxComboBox ID="cbCreditableCoverageIndicator" TabIndex="21" runat="server" IncrementalFilteringMode="StartsWith" ValueType="System.String" Value='<%# Bind("CreditCoverageIndicator") %>'></dx:ASPxComboBox>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblTotalPremium" runat="server" Text="Total Premium" AssociatedControlID="txtTotalPremium"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxTextBox ID="txtTotalPremium" TabIndex="22" runat="server" MaskSettings-Mask="$<0..999999g>.<00..99>" MaskSettings-IncludeLiterals="DecimalSymbol" Text='<%# Bind("TotalPremiumAmount") %>'></dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td>
                                            <dx:ASPxLabel ID="lblBasicPremiumAmount" runat="server" Text="Basic Premium" AssociatedControlID="txtBasicPremiumAmount"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxTextBox ID="txtBasicPremiumAmount" TabIndex="23" runat="server" MaskSettings-Mask="$<0..999999g>.<00..99>" MaskSettings-IncludeLiterals="DecimalSymbol" Text='<%# Bind("BasicPremiumAmount") %>'></dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr class="">
                                        <td>
                                            <dx:ASPxLabel ID="lblPrescriptionDrugPlan" runat="server" Text="Prescription Drug Plan" AssociatedControlID="txtPrescriptionDrugPlan"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxTextBox ID="txtPrescriptionDrugPlan" TabIndex="24" runat="server" MaxLength="45" Text='<%# Bind("PrescriptionDrugPlan") %>'>
                                                <ValidationSettings>
                                                    <RegularExpression ValidationExpression="[a-zA-Z0-9\s]+" ErrorText="Only characters and numbers are allowed." />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </td>
                                        <td>
                                            <dx:ASPxLabel ID="lblClientPays" runat="server" Text="Client Pays" AssociatedControlID="txtClientPays"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxTextBox ID="txtClientPays" TabIndex="25" runat="server" MaskSettings-Mask="$<0..999999g>.<00..99>" MaskSettings-IncludeLiterals="DecimalSymbol" Text='<%# Bind("ClientPaymentAmount") %>'></dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </EditItemTemplate>
                </asp:FormView>
            </td>
        </tr>
        <tr>
            <td>
                <dx:ASPxLabel ID="lblBuyInInformation" runat="server" Text="Buy-In Information" SkinID="InnerHeader"></dx:ASPxLabel>
                <hr />
            </td>
        </tr>
        <tr>
            <td>
                <table class="SectionTable">
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblMedicareNumber" runat="server" Text="Medicare Number" AssociatedControlID="txtMedicareNumber"></dx:ASPxLabel>
                        </td>
                        <td>
                            <dx:ASPxTextBox ID="txtMedicareNumber" TabIndex="26" runat="server" MaxLength="15" OnValueChanged="txtMedicareNumber_ValueChanged"></dx:ASPxTextBox>
                        </td>
                        <td>
                            <dx:ASPxLabel ID="lblBuyInEffectiveDate" runat="server" Text="Buy-In Effective Date" AssociatedControlID="dtBuyInEffectiveDate"></dx:ASPxLabel>
                        </td>
                        <td>
                            <dx:ASPxDateEdit ID="dtBuyInEffectiveDate" ClientIDMode="Static" TabIndex="27" runat="server" AutoPostBack="true" EnableViewState="true" DisplayFormatString="MM/dd/yyyy" OnLoad="DtBuyInEffectiveDate_Load" OnDateChanged="DtBuyInEffectiveDate_ValueChanged" ClientInstanceName="dtBuyInEffectiveDate">
                                <ClientSideEvents LostFocus="function(s,e) {DateLostFocus(s,'dtBuyInEffectiveDate',false);}"  DropDown="function(s,e){calenderClick(s,'dtBuyInEffectiveDate');}"   />                           
                            </dx:ASPxDateEdit>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td></td>
        </tr>
    </table>
    <dx:ASPxPopupControl ID="dxPopupErr" ClientInstanceName="pcerrorpopup" SkinID="ErrorPopUp"
        Modal="true" CloseAction="CloseButton" runat="server" ShowOnPageLoad="false"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" AllowDragging="true" Width="400px" ShowFooter="true" HeaderText="Error Title" ShowPageScrollbarWhenModal="True">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl" runat="server">
                <dx:ASPxLabel ID="lblErrmessage" runat="server" />
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterTemplate>
            <div style="float: right; margin: 3px;">
                <asp:Panel ID="btnOkpanel" DefaultButton="btnok" runat="server"> 
                <dx:ASPxButton ID="btnok" ClientInstanceName="btnok" runat="server" Text="OK" OnClick="Btnok_Click" SkinID="footerPrimary" IgnoreFgs="T" CausesValidation="false" />
                    </asp:Panel>
            </div>
        </FooterTemplate>
        <ClientSideEvents Shown="function(s, e) {btnok.Focus(); return false;}" />  
		<FooterStyle>
			<Paddings PaddingBottom="40px" PaddingTop="2px" />
		</FooterStyle>
    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl ID="dxPopupError" ClientInstanceName="pcerrorpopup" SkinID="ErrorPopUp"
        Modal="true" CloseAction="CloseButton" runat="server" ShowOnPageLoad="false" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        AllowDragging="true" Width="300px" Height="120px" ShowFooter="true" HeaderText="Question" ShowPageScrollbarWhenModal="True">
        <ContentCollection>
            <dx:PopupControlContentControl ID="pucalertmessagecontent" runat="server">
                <dx:ASPxLabel ID="lblmessage1" runat="server" Width="200px" />
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterTemplate>
            <table class="SectionTable" style="float: right;">
                <tr>
                    <td align="right">
                        <dx:ASPxButton ID="btnYes" runat="server" Text="Yes" OnClick="BtnPopUpYes_Click" CausesValidation="false" IgnoreFgs="T" Style="padding-left: 40px;" />
                    </td>
                    <td align="right">
                        <dx:ASPxButton ID="btnNo" runat="server" Text="No" OnClick="BtnPopUpNo_Click" CausesValidation="false" IgnoreFgs="T" Style="padding-left: 40px;" />
                    </td>
                </tr>
            </table>
        </FooterTemplate>
        <FooterStyle>
            <Paddings PaddingBottom="12px" PaddingTop="8px" />
        </FooterStyle>
    </dx:ASPxPopupControl>
    <dx:ASPxPopupControl ID="popUpWindow" ClientInstanceName="popUpWindow" runat="server" ShowCloseButton="false" SkinID="ErrorPopup"
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
 <dx:ASPxPopupControl ID="dxPopupErrorBuyInDate" ClientInstanceName="dxPopupErrorBuyInDate" SkinID="ErrorPopUp"
        Modal="true" CloseAction="CloseButton" runat="server" ShowOnPageLoad="false" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        AllowDragging="true" Width="300px" Height="120px" ShowFooter="true" HeaderText="Error Message" ShowPageScrollbarWhenModal="True">
        <ContentCollection>
            <dx:PopupControlContentControl ID="PopupControlContentControl1" runat="server">
                <dx:ASPxLabel ID="lblpopupBuyInDate" runat="server" Width="280px" ClientInstanceName="lblpopupBuyInDate" />
            </dx:PopupControlContentControl>
        </ContentCollection>
        <FooterTemplate>
             <div style="float: rifght; margin: 3px;">
            <table  style="float: right;">
                <tr>
                    <td align="right">
                        <dx:ASPxButton ID="btnYes" runat="server" Text="Yes"  CausesValidation="false" IgnoreFgs="T" SkinID="footerPrimary" ClientSideEvents-Click="function(s,e){dxPopupErrorBuyInDate.Hide(); e.processOnServer = false;}" />
                    </td>
                    <td align="right">
                        <dx:ASPxButton ID="btnNo" runat="server" Text="No" CausesValidation="false" IgnoreFgs="T" SkinID="footerPrimary" OnClick="btnNo_Click"/>
                    </td>
                </tr>
            </table>
           </div>
        </FooterTemplate>
         <FooterStyle>
			<Paddings PaddingBottom="40px" PaddingTop="2px" />
		</FooterStyle>
     </dx:ASPxPopupControl>
</asp:Content>

