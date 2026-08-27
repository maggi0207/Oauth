///////////////////////////////////////////////////////////////////////////////////////////////////////
//
// File:      TaxDependencyInformation.aspx.cs
//
// Created On: Monday, April 15, 2013 9:00:01 AM
// Created By: Keerthi.Pathipaka
//
// This file may contain sensitive and/or confidential information and may not be
// distributed without written permission of Delaware Department of Health and 
// Social Services.
//
// #      Type        User                    Date        Comment                                      
// ------ ----------- ----------------------- ----------- -------------------------------------------- 
// 12036	   add	   Keerthi.Pathipaka        4/15/2013   Added new files in Technical 
//      	   edit	   Sushil.Kumar            11/04/2013   Fixed performance issues 
// 164507      Edit    Devi.Yerramsetti        06/17/2019   Fixed Duplicate tax DependencyRows  
///////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxEditors;
using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Controls;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Extensions;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Services;
using Dhss.Framework;
using Dhss.Framework.Extensions;
using Dhss.Framework.Web.UI.Workflow;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{
    [Workflow]
    [ExcludeFromCodeCoverage]
    public partial class TaxDependencyInformation : Dhss.Assist.WorkerWeb.Web.Infrastructure.Workflow.WorkflowPage<Technical_TaxDependency>
    {
        private Technical_TaxDependency UpdateServiceRequest { get; set; }

        bool _isRefreshRecord;
        private int _applicationEntityId;
        private int _taxDependentId;
        Int16 _historySeqNumber;
        Int16 _syncState = 0;
        bool _taxDeductionIndicatorNew;
        private bool _isChangeMade; // Tracks if any edit or change made is on the formview 
        private bool _isbackToSummaryOrPrevious; // Back-to-summary or Previous button clicked flag
        

        /// <summary>
        /// Occurs on Page Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            SetSelectedIndividual();
            // On Sync Error Navigates to Summary page. 
            if (IsPostBack)
            {
                OnSyncErrorNavigateToSummary();
            }
            //Defect 39224 - No Add New button should be displayed in footer. 
            Master.Master.FooterSectionConfigure(FooterBodyConfiguration.AddnoteSavePreviousNext);
            var btnPageSaveData = (ASPxButton)Master.ViewBodyActionBar.FindControl("btnPageSave");
            ((ASPxButton)Master.ViewBodyActionBar.FindControl("btnPageAddNote")).TabIndex = 9;
            btnPageSaveData.TabIndex = 10;
            ((ASPxButton)Master.ViewBodyActionBar.FindControl("btnPagePrevious")).TabIndex = 11;
            ((ASPxButton)Master.ViewBodyActionBar.FindControl("btnPageNext")).TabIndex = 12;
            if (btnPageSaveData != null)
            {
                btnPageSaveData.Click += RefreshAnchorObject;
            }
            // For begin and end date logic Defect Id: 39246
            var serverdt = fvTechnical_TaxDependency.FindControl("serverdt") as HiddenField;
            if (serverdt != null)
            {
                serverdt.Value = SystemDateTime.Now.ToString();
            }
            ASPxDateEdit beginDate = (ASPxDateEdit)fvTechnical_TaxDependency.FindControl("ddeBeginDate");
            beginDate.Focus();
        }

        /// <summary>
        ///  On Sync Error Navigates to Summary page. 
        /// </summary>
        public void OnSyncErrorNavigateToSummary()
        {
            string postBackStr = Request.Params.Get("__EVENTARGUMENT");
            if (postBackStr != null && postBackStr.Contains(IntakeConstants.SYNC_ERROR_GOTO_SUMMARY))
            {
                NavigateToSummary();
            }
        }

        /// <summary>
        /// Reloads the syncronized record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshAnchorObject(object sender, EventArgs e)
        {
            if (_isRefreshRecord)
            {
                Technical_TaxDependency taxDependency = GetTaxDependency();
                ReloadAnchorObject();
                int? id = this.AnchorObject.ApplicationEntityID;
                NavigateTo(taxDependency);
            }
        }

        /// <summary>
        /// Sets selected person and highlights the person on Case Summary Information.
        /// </summary>
        private void SetSelectedIndividual()
        {
            if (this.AnchorObject.IsNull())
                NavigateToSummary();
            _applicationEntityId = Convert.ToInt32(this.AnchorObject.ApplicationEntityID);
            Master.CurrentPersonSelectedId = TechnicalContextOperations.GetPersonIdByAppEntityId(_applicationEntityId);
            _taxDependentId = Convert.ToInt32(this.AnchorObject.TaxDependentID);
            _historySeqNumber = Convert.ToInt16(this.AnchorObject.HistorySequenceNumber);
            _syncState = Convert.ToInt16(this.AnchorObject.SyncState);
        }

        /// <summary>
        /// Navigates to summary page.
        /// </summary>
        private void NavigateToSummary()
        {
            ClearSessionVariables();
            NavigatePrevious(n => n.Name == IntakeConstants.TAX_DEPENDENCY_SUMMARY_AE);
        }

        /// <summary>
        /// Bind Individuals to the checkBoxList
        /// </summary>
        private void BindIndividuals()
        {
            var personPersonId = new PersonWithApplicationEntityId();
            cklIndividuals.DataSource = personPersonId.Values.Where(p => p.ApplicationEntityId != _applicationEntityId);
            cklIndividuals.DataBind();
            lstChosenIndividuals.SelectedIndex = 0;
        }

        /// <summary>
        /// Binds List with selected individuals.
        /// </summary>
        /// <param name="appPersons"></param>
        private void BindSelectedIndividuals(IList<KeyValuePair<string, string>> appPersons)
        {
            lstChosenIndividuals.DataSource = appPersons;
            lstChosenIndividuals.DataBind();
        }



        /// <summary>
        /// Checks if there is any individual added or removed from tax dependent individuals.
        /// </summary>
        /// <returns></returns>
        private bool IsIndividualsChanged()
        {
            bool isIndivChanged = false;
            var taxPersons = TechnicalContextOperations.GetTaxDependencyDetails(UpdateServiceRequest.TaxDependentID).Select(n => new { n.DependentApplicationEntityID });

            foreach (var person in taxPersons)
            {
                if ((lstChosenIndividuals.Items.FindByValue(person.DependentApplicationEntityID.ToString()) == null))
                {
                    isIndivChanged = true;
                    break;
                }
            }
            //Compare with selected individuals.
            if (!isIndivChanged)
            {
                var appPersons = taxPersons.ToList();

                if (appPersons.Count() == 0)
                {
                    isIndivChanged = lstChosenIndividuals.Items.Count > 0;
                }
                else
                {
                    if (lstChosenIndividuals.Items.Count == 0)
                        isIndivChanged = true;
                    else
                    {
                        foreach (ListEditItem listItem in lstChosenIndividuals.Items)
                        {
                            if (!appPersons.Any(n => (n.DependentApplicationEntityID == Convert.ToInt32(listItem.Value))))
                            {
                                isIndivChanged = true;
                                break;
                            }
                        }
                    }
                }
            }
            return isIndivChanged;
        }

        /// <summary>
        /// Raises on LinqDataSource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DsTechnical_TaxDependency_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetTaxDependency();
        }

        /// <summary>
        /// Gets the person's SchoolEnrollment information
        /// </summary>
        /// <returns></returns>
        private Technical_TaxDependency GetTaxDependency()
        {
            var context = ServicesDataHub.Technical;
            Technical_TaxDependency taxDependency;
            if (TechnicalSessionContext.Instance.TaxDependentID != 0)
                taxDependency =
                    context.Technical_TaxDependency.Where(
                        n => n.TaxDependentID == TechnicalSessionContext.Instance.TaxDependentID).FirstOrDefault();
            else
                taxDependency =
                    context.Technical_TaxDependency.Where(
                        n =>
                            n.ApplicationEntityID == _applicationEntityId &&
                            ((n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty) &&
                             (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)))
                        .FirstOrDefault();
            return taxDependency;
        }

        /// <summary>
        ///  Popup Save confirmation window on Back to Summary click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <History>
        ///  Created:       10/25/2013   Defect 39733-On click of Back to Summary the page directed to Summary screen with saving/completing the page
        /// </History>
        protected void BtnBackToSummary_Click(object sender, EventArgs e)
        {
            _isbackToSummaryOrPrevious = true;
            if (fvTechnical_TaxDependency.Enabled)
                fvTechnical_TaxDependency.UpdateItem(false);

            if (_isChangeMade || (fvTechnical_TaxDependency.Enabled && IsIndividualsChanged()))
            {
                TechnicalSessionContext.Instance.IsPreviousAction = false;
                ShowPopupInfo(IntakeResourceManager.SAVE_CHAGNES_ALERT);
            }
            else
            {
                NavigateToSummary();
            }
        }

        /// <summary>
        /// To show Save confirmation Popup
        /// </summary>
        /// <param name="message"></param>
        ///  <History>
        ///  Created:       10/25/2013   Defect 39733-On click of Back to Summary the page directed to Summary screen with saving/completing the page
        /// </History>
        private void ShowPopupInfo(string message)
        {
            dxPopupInfo.ShowOnPageLoad = true;
            var lblmessage = (ASPxLabel)dxPopupInfo.FindControl("lblmessage1");
            lblmessage.Text = message;
        }

        /// <summary>
        /// Pop Up Panel Button Yes Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <History>
        ///  Created:       10/25/2013   Defect 39733-On click of Back to Summary the page directed to Summary screen with saving/completing the page
        /// </History>
        protected void BtnPopUpYes_Click(object sender, EventArgs e)
        {
            dxPopupInfo.ShowOnPageLoad = false;
            if (TechnicalSessionContext.Instance.IsPreviousAction)
            {
                TechnicalSessionContext.Instance.IsPreviousAction = false;
                base.NavigatePrevious(n => n.Visible && n.Completed && !n.DetailScreen && n.Name != IntakeConstants.TAX_DEPENDENCY_SUMMARY_AE);
            }
            else
            {
                NavigateToSummary();
            }
        }

        /// <summary>
        /// Popup Panel No Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <History>
        ///  Created:       10/25/2013   Defect 39733-On click of Back to Summary the page directed to Summary screen with saving/completing the page
        /// </History>
        protected void BtnPopUpNo_Click(object sender, EventArgs e)
        {
            dxPopupInfo.ShowOnPageLoad = false;
        }

        /// <summary>
        /// Occurs on databound of the formview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvTechnical_TaxDependency_DataBound(object sender, EventArgs e)
        {
            ASPxDateEdit dteBeginDate = (ASPxDateEdit)fvTechnical_TaxDependency.FindControl("ddeBeginDate");
            ASPxComboBox cmbFileTaxReturnInCurrentYearIndicator = (ASPxComboBox)fvTechnical_TaxDependency.FindControl("cbFileTaxReturnInCurrentYearIndicator");
            if ((IntakeContext.Instance.CaseMode == "L" || IntakeContext.Instance.CaseMode == IntakeConstants.CASEMODE_SDX) && cmbFileTaxReturnInCurrentYearIndicator.Value == null)
            {
                dteBeginDate.Date = Convert.ToDateTime(IntakeContext.Instance.CaseFilingDate);
                cmbFileTaxReturnInCurrentYearIndicator.Value = "N";
                BindPrimaryTaxFilerEntityConditionally(cmbFileTaxReturnInCurrentYearIndicator.Value);
                ASPxComboBox cbPrimaryTaxFilerIndicator = fvTechnical_TaxDependency.FindControl("cbPrimaryTaxFilerIndicator") as ASPxComboBox;
                BindHasTaxDeductionsEntityConditionally(Convert.ToBoolean(cbPrimaryTaxFilerIndicator.Value));
                ASPxComboBox cbHasTaxDeductionIndicator = fvTechnical_TaxDependency.FindControl("cbHasTaxDeductionIndicator") as ASPxComboBox;
                cbHasTaxDeductionIndicator.Value = false;
            }
            else
            {
                BindEntitiesConditionally();
            }

            ASPxLabel lblNameChildCare = (ASPxLabel)fvTechnical_TaxDependency.FindControl("lblName1");
            string individualName = TechnicalContextOperations.GetPersonNameByAppEntityId(Convert.ToInt32(lblNameChildCare.Value));
            lblNameChildCare.Text = individualName;
            BindIndividuals();
            BindSelectedIndividuals(TechnicalContextOperations.LoadTaxDependencyIndividuals(_taxDependentId,
                                                                                            TechnicalCommon.IsHistoryRecord((fvTechnical_TaxDependency.FindControl("hfDeleteReasonCode") as HiddenField).Value, (fvTechnical_TaxDependency.FindControl("hfHistoryCode") as HiddenField).Value)
                                                                                           )
                                    );

            EnableDisableFormview();
        }

        /// <summary>
        /// Attaching Javascript based validation for Conditional validations
        /// </summary>
        private void ApplyConditionValidation()
        {
            var fv = fvTechnical_TaxDependency;
            var cbFileTaxReturnInCurrentYearIndicator =
                fv.FindControl("cbFileTaxReturnInCurrentYearIndicator").As<ASPxEdit>();
            var cbPrimaryTaxFilerIndicator = fv.FindControl("cbPrimaryTaxFilerIndicator").As<ASPxEdit>();
            var lblPrimaryTaxFilerIndicator = fv.FindControl("lblPrimaryTaxFilerIndicator").As<ASPxLabel>();
            ConditionalJavaScript.ConditionalValidation(this,
                cbFileTaxReturnInCurrentYearIndicator,
                cbPrimaryTaxFilerIndicator,
                lblPrimaryTaxFilerIndicator,
                "Primary Tax Filer is Required",
                true,
                "Y");
            //TODO: Temp Fix. Need to check with Ramesh/Eric for this issue. - Pls Dont remove - Suresh
            var cbFileTaxReturn = cbFileTaxReturnInCurrentYearIndicator.As<ASPxComboBox>();
            var custjavaScriptFunctionName = "FileTaxReturnInCurrentYearIndicator(s)";
            if (!cbFileTaxReturn.ClientSideEvents.SelectedIndexChanged.Contains(custjavaScriptFunctionName))
                cbFileTaxReturn.ClientSideEvents.SelectedIndexChanged = cbFileTaxReturn.ClientSideEvents
                    .SelectedIndexChanged.Replace("}", "{0}; }}".FormatWith(custjavaScriptFunctionName));

            var cbHasTaxDeductionIndicator = fv.FindControl("cbHasTaxDeductionIndicator").As<ASPxEdit>();
            var lblHasTaxDeductionIndicator = fv.FindControl("lblHasTaxDeductionIndicator").As<ASPxLabel>();
            ConditionalJavaScript.ConditionalValidation(this,
                cbPrimaryTaxFilerIndicator,
                cbHasTaxDeductionIndicator,
                lblHasTaxDeductionIndicator,
                "Do you have any tax deductions is Required",
                true,
                "1");
            //TODO: Temp Fix. Need to check with Ramesh/Eric for this issue. Pls Dont remove - Suresh
            var cbPrimaryTaxFiler = cbPrimaryTaxFilerIndicator.As<ASPxComboBox>();
            var cbFileTaxjavaScriptFunctionName = "PrimaryTaxFilerIndicator(s)";
            if (!cbPrimaryTaxFiler.ClientSideEvents.SelectedIndexChanged.Contains(custjavaScriptFunctionName))
                cbPrimaryTaxFiler.ClientSideEvents.SelectedIndexChanged = cbPrimaryTaxFiler.ClientSideEvents
                    .SelectedIndexChanged.Replace(
                        "}",
                        "{0}; }}".FormatWith(cbFileTaxjavaScriptFunctionName));
        }

        /// <summary>
        /// Disable Details for History Record
        /// </summary>
        private void EnableDisableFormview()
        {
            if (TechnicalCommon.IsHistoryRecord((fvTechnical_TaxDependency.FindControl("hfDeleteReasonCode") as HiddenField).Value,
                   (fvTechnical_TaxDependency.FindControl("hfHistoryCode") as HiddenField).Value))
            {
                fvTechnical_TaxDependency.Enabled = false;
                btnIndividual.ClientEnabled = false; //by sushil
                btnAll.ClientEnabled = false; //by sushil
                btnClearAll.ClientEnabled = false; //by sushil
                ContentPlaceHolder mpContentPlaceHolder = (ContentPlaceHolder)Master.ViewBodyActionBar;
                (mpContentPlaceHolder.FindControl("btnPageSave") as ASPxButton).Enabled = false;
            }
            else
            {
                fvTechnical_TaxDependency.Enabled = true;
                //defect# 42089 
                RenewalModeScenario();
            }

            var syncState = Convert.ToInt16(this.AnchorObject.SyncState);
            //Setting Page Complete for synced records.
            SetCompletePage(syncState);
        }

        /// <summary>
        /// SetCompletePage
        /// </summary>
        /// <param name="syncState"></param>
        private void SetCompletePage(Int16 syncState)
        {
            if (TechnicalContextOperations.IsCaseRenewalOrReactivate() && IsSummaryPagesComplete() && syncState == 3)
                SetPageComplete(true);
            else if (!TechnicalContextOperations.IsCaseRenewalOrReactivate() && syncState == 3)
                SetPageComplete(true);
            else if (syncState == 1)
                SetPageComplete(false);
        }

        #region "Conditional Validations"
        /// <summary>
        /// 
        /// </summary>
        protected void BindEntitiesConditionally()
        {
            var cbFileTaxReturnInCurrentYearIndicator = fvTechnical_TaxDependency.FindControl("cbFileTaxReturnInCurrentYearIndicator") as ASPxComboBox;
            BindPrimaryTaxFilerEntityConditionally(cbFileTaxReturnInCurrentYearIndicator.Value);

            var cbPrimaryTaxFilerIndicator = fvTechnical_TaxDependency.FindControl("cbPrimaryTaxFilerIndicator") as ASPxComboBox;
            BindHasTaxDeductionsEntityConditionally(Convert.ToBoolean(cbPrimaryTaxFilerIndicator.Value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbFileTaxReturnInCurrentYearIndicator_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbFileTaxReturnInCurrentYearIndicator = sender as ASPxComboBox;
            BindPrimaryTaxFilerEntityConditionally(cbFileTaxReturnInCurrentYearIndicator.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbPrimaryTaxFilerIndicator_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbPrimaryTaxFilerIndicator = sender as ASPxComboBox;
            BindHasTaxDeductionsEntityConditionally(Convert.ToBoolean(cbPrimaryTaxFilerIndicator.Value));
        }

        #endregion

        /// <summary>
        /// Handles FvTechnical_TaxDependency_ItemUpdating event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvTechnical_TaxDependency_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            if (!_isbackToSummaryOrPrevious)
            {
                // if values are changed in the formview, the function will set page complete value to false
                bool isUpdated = TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues);
                //cancel update operation if nothing is changed
                SetPageComplete(!isUpdated);
                if (!CurrentWorkflowPage.Completed) ResetTaxDeductionsPage(Convert.ToBoolean(e.NewValues["HasTaxDeductionIndicator"]), Convert.ToBoolean(e.OldValues["HasTaxDeductionIndicator"]));

            }
            else
            {
                _isChangeMade = TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues);
            }
            AssignValuesToRequest(e.NewValues, Convert.ToInt32((sender as FormView).DataKey.Value), e.OldValues);

            e.Cancel = true;
        }

        private void ResetTaxDeductionsPage(bool taxDeductionNewValue, bool taxDeductionOldValue)
        {
            if (taxDeductionNewValue && !taxDeductionOldValue)
            {
                if (IsEntityExistsInState("TaxDeductionsSummary")) SetPageComplete("TaxDeductionsSummary", false);
            }
        }

        /// <summary>
        /// Assign values from Formview to request's object with key value of formview.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyValue"></param>
        private void AssignValuesToRequest(IOrderedDictionary source, int keyValue, IOrderedDictionary oldValues)
        {
            var taxDependency = new Technical_TaxDependency();
            source.CopyValuesTo(taxDependency);
            taxDependency.TaxDependentID = keyValue;
            if (source["HasTaxDeductionIndicator"] != null)
                taxDependency.HasTaxDeductionIndicator = Convert.ToBoolean(source["HasTaxDeductionIndicator"]);
            else
                taxDependency.HasTaxDeductionIndicator = null;

            UpdateServiceRequest = taxDependency;
        }

        // Required verification dates are blanked out in Renewal Mode for the first time
        // Defect# 42089 
        /// <summary>
        /// 
        /// </summary>
        protected void RenewalModeScenario()
        {
            if (IntakeContext.Instance.CaseMode == "R" && !CurrentWorkflowPage.Completed && !IsSummaryPagesComplete())
            {
                ASPxDateEdit ddeBeginDate = fvTechnical_TaxDependency.FindControl("ddeBeginDate") as ASPxDateEdit;
                ddeBeginDate.Text = string.Empty;
                ddeBeginDate.Value = null;

                ASPxDateEdit ddeEndDate = fvTechnical_TaxDependency.FindControl("ddeEndDate") as ASPxDateEdit;
                ddeEndDate.Text = string.Empty;
                ddeEndDate.Value = null;

                ASPxComboBox cbFileTaxReturnInCurrentYearIndicator = fvTechnical_TaxDependency.FindControl("cbFileTaxReturnInCurrentYearIndicator") as ASPxComboBox;
                cbFileTaxReturnInCurrentYearIndicator.Text = string.Empty;
                cbFileTaxReturnInCurrentYearIndicator.Value = null;

                ASPxComboBox cbPrimaryTaxFilerIndicator = fvTechnical_TaxDependency.FindControl("cbPrimaryTaxFilerIndicator") as ASPxComboBox;
                cbPrimaryTaxFilerIndicator.Text = string.Empty;
                cbPrimaryTaxFilerIndicator.Value = null;

                ASPxComboBox cbHasTaxDeductionIndicator = fvTechnical_TaxDependency.FindControl("cbHasTaxDeductionIndicator") as ASPxComboBox;
                cbHasTaxDeductionIndicator.Text = string.Empty;
                cbHasTaxDeductionIndicator.Value = null;

                lstChosenIndividuals.Items.Clear();
                cklIndividuals.UnselectAll();
            }
        }


        /// <summary>
        /// Chcks if Begin and End dates are valid
        /// </summary>
        /// <returns></returns>
        protected bool IsBeginEndDateValid()
        {
            bool retVal = true;
            var ddeBeginDate = fvTechnical_TaxDependency.FindControl("ddeBeginDate") as ASPxDateEdit;
            var ddeEndDate = fvTechnical_TaxDependency.FindControl("ddeEndDate") as ASPxDateEdit;
            if (ddeBeginDate != null && ddeEndDate != null && ddeBeginDate.Date != DateTime.MinValue && ddeEndDate.Date != DateTime.MinValue &&
                TechnicalCommon.GetDateWithFirstDayOfMonth(ddeBeginDate.Date) > TechnicalCommon.GetDateWithLastDayOfMonth(ddeEndDate.Date))
            {
                _isValidated = false;
                ShowErrPopupAlert(IntakeConstants.ERROR_BEGINDATAANDENDDATE, IntakeConstants.ERROR_HEADER_INVALIDDATE);
                retVal = false;                
            }
            return retVal;

        }

        /// <summary>
        /// Error PopUpAlert
        /// </summary>
        /// <param name="stralertmsg"></param>
        /// <param name="headerText"></param>
        private void ShowErrPopupAlert(string stralertmsg, string headerText)
        {
            dxPopupErr.ShowOnPageLoad = true;
            ((ASPxLabel)dxPopupErr.FindControl("lblErrmessage")).Text = stralertmsg;
            dxPopupErr.HeaderText = headerText;
            var btnOk = (ASPxButton)dxPopupErr.FindControl("btnOk");
            btnOk.Focus();
        }
    }
}

