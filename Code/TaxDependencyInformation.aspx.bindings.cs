///////////////////////////////////////////////////////////////////////////////////////////////////////
//
// File:      TaxDependencyInformation.aspx.bindings.cs
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
///////////////////////////////////////////////////////////////////////////////////////////////////////

using DevExpress.Web.ASPxEditors;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Extensions;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Services;
using Dhss.Assist.WorkerWeb.Web.Intake.CommonMetadata;
using Dhss.Framework.DataAnnotations;
using Dhss.Framework.Web.UI.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{
    public partial class TaxDependencyInformation
    {
        private bool _isValidated = true;

        /// <summary>
        /// Validation and refernce table class
        /// </summary>
        public class TaxDependencyMetaData
        {
            /// <summary>
            /// Gets or Sets the ApplicationEntityId.
            /// </summary>
            /// <value>The ApplicationEntityId</value>
            [LookupTable(typeof(PersonNameWithAppEntityId))]
            public string ApplicationEntityId { get; set; }

            /// <summary>
            /// Gets or Sets the LstChosenIndividuals.
            /// </summary>
            /// <value>The LstChosenIndividuals.</value>
            [Required]
            public string LstChosenIndividuals { get; set; }

            /// <summary>
            /// Gets or Sets the BeginDate.
            /// </summary>
            /// <value>The BeginDate.</value>
            [Required]
            public DateTime BeginDate { get; set; }

            /// <summary>
            /// Gets or Sets the FileTaxReturnInCurrentYearIndicator.
            /// </summary>
            /// <value>The FileTaxReturnInCurrentYearIndicator.</value>
            [Required]
            [LookupTable("AERSPE", "RESPONSE-CD", "RESPONSE-DESC", typeof(ReferenceTableLookupContext))]
            public string FileTaxReturnInCurrentYearIndicator { get; set; }

            /// <summary>
            /// Gets or Sets the FileTaxReturnInCurrentYearIndicator.
            /// </summary>
            /// <value>The FileTaxReturnInCurrentYearIndicator.</value>
            [NotRequired]
            [LookupTable("AERSPE", "RESPONSE-CD", "RESPONSE-DESC", typeof(ReferenceTableLookupContext))]
            public string FileTaxReturnInCurrentYearIndicatorNr { get; set; }
        }

        /// <summary>
        /// Binding Entities 
        /// </summary>
        public override void BindEntities()
        {
            fvTechnical_TaxDependency.FindControl("ddeBeginDate").Bind<TaxDependencyMetaData>(b => b.BeginDate);
            if (IntakeContext.Instance.MAProgramCode)
            {
                fvTechnical_TaxDependency.FindControl("cbFileTaxReturnInCurrentYearIndicator").Bind<TaxDependencyMetaData>(b => b.FileTaxReturnInCurrentYearIndicator);
            }
            else { fvTechnical_TaxDependency.FindControl("cbFileTaxReturnInCurrentYearIndicator").Bind<TaxDependencyMetaData>(b => b.FileTaxReturnInCurrentYearIndicatorNr); }
            ApplyConditionValidation();
        }

        /// <summary>
        /// Save data on page.
        /// </summary>
        public override void SaveData()
        {
            using (var scope = new TransactionScope())
            {   
                if (fvTechnical_TaxDependency.Enabled)
                {
                    if (!IsBeginEndDateValid())
                        return;

                    fvTechnical_TaxDependency.UpdateItem(false);
                    bool isCurrentEntityCompleted = CurrentWorkflowPage.Completed;

                    if (!isCurrentEntityCompleted || IsIndividualsChanged())
                    {

                        UpdateTaxDependencyServiceCall();

                        ClearSessionVariables();
                        SetPageComplete();
                        //Reset Eligibility workflow
                        WorkflowScheduling.ResetElgibilityWorkFlow(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                        _isRefreshRecord = true;
                    }
                    else
                    {
                        _isRefreshRecord = false;
                    }
                    TechnicalSessionContext.Instance.IsSaved = true;
                }
                else
                {
                    _isRefreshRecord = false;
                }
                
                SetSummaryPageComplete();
                
            }
        }


        /// <summary>
        /// Call Service to Update Tax Dependency
        /// </summary>
        /// <param name="isListChanged"></param>
        private void UpdateTaxDependencyServiceCall()
        {
            List<int> listBoxValues = new List<int>();
            foreach (var values in lstChosenIndividuals.Items)
            {
                listBoxValues.Add(Convert.ToInt32((values as ListEditItemBase).Value));
            }
            UpdateServiceRequest.ApplicationEntityID = AnchorObject.ApplicationEntityID;
            ServicesApplicationHub.IntakeTechnical.UpdateTaxDependency(UpdateServiceRequest, listBoxValues, Convert.ToDecimal(WorkflowSession.Root["CaseNumber"]));
        }

        /// <summary>
        /// Call Institution Sync
        /// </summary>
        /// <param name="oldState"></param>
        private void SetSummaryPageComplete()
        {
            if (CurrentWorkflowPage.Context.Value.IsContextComplete())
            {
                SetPreviousPageComplete(true);
                SetPageComplete("TaxDependencyAE", true, true);
            }

        }
				
           

        

        /// <summary>
        /// Removes Session variables if page has already any.
        /// </summary>
        protected void ClearSessionVariables()
        {
            TechnicalSessionContext.Instance.TaxDependentID = 0;
        }

        /// <summary>
        /// Binds Primary Tax Filer conditionally.
        /// </summary>
        /// <param name="isResponse"></param>
        /// <history>
        /// Modified By             Date            Defect
        /// ================================================
        /// csmanoharan             10/24/2013      39474 - Default Q3 to No response and clear Q4 when Q1 is No response
        /// </history>
        private void BindPrimaryTaxFilerEntityConditionally(object isResponse)
        {
            // bind cbPrimaryTaxFilerIndicator
            ASPxComboBox cbPrimaryTaxFilerIndicator = fvTechnical_TaxDependency.FindControl("cbPrimaryTaxFilerIndicator") as ASPxComboBox;
            ASPxComboBox cbHasTaxDeductionIndicator = fvTechnical_TaxDependency.FindControl("cbHasTaxDeductionIndicator") as ASPxComboBox;
            if (Convert.ToString(isResponse) == "Y")
            {
                cbPrimaryTaxFilerIndicator.Bind<CommonDataValidation>(b => b.YesNoCodeBitRequired);
            }
            else
            {
                cbPrimaryTaxFilerIndicator.Value = isResponse == null ? cbPrimaryTaxFilerIndicator.Value : false;
                cbHasTaxDeductionIndicator.Value = isResponse == null ? cbHasTaxDeductionIndicator.Value : false;
                lstChosenIndividuals.Items.Clear();
                cbPrimaryTaxFilerIndicator.Bind<CommonDataValidation>(b => b.YesNoCodeBit);
            }

            if (isResponse != null)
            {
                cbPrimaryTaxFilerIndicator.ClientEnabled = (Convert.ToString(isResponse) == "Y"); 
                // Q3 question disabled or enabled as per the response of Q1
                cbHasTaxDeductionIndicator.ClientEnabled = (Convert.ToString(isResponse) == "Y"); 
            }
            else
            {
                cbPrimaryTaxFilerIndicator.ClientEnabled = true; //Sushil
                // Q3 question disabled or enabled as per the response of Q1
                cbHasTaxDeductionIndicator.ClientEnabled = true; //Sushil
            }

        }
        /// <summary>
        /// Binds Primary Tax Filer conditionally.
        /// </summary>
        /// <param name="isResponse"></param>
        ///  /// Modified By             Date            Defect
        /// ================================================
        /// csmanoharan             10/24/2013      39474 - Default Q3 to No response and clear Q4 when Q2 is No response
        private void BindHasTaxDeductionsEntityConditionally(bool isResponse)
        {
            ASPxComboBox cbHasTaxDeductionIndicator = fvTechnical_TaxDependency.FindControl("cbHasTaxDeductionIndicator") as ASPxComboBox;
            if (isResponse)
                cbHasTaxDeductionIndicator.Bind<CommonDataValidation>(b => b.YesNoCodeBitRequired);
            else
            {
                lstChosenIndividuals.Items.Clear();
                cbHasTaxDeductionIndicator.Bind<CommonDataValidation>(b => b.YesNoCodeBit);
            }
            if (isResponse != null)
            {
            btnIndividual.ClientEnabled = isResponse; //by sushil
            btnAll.ClientEnabled = isResponse; //by sushil
            btnClearAll.ClientEnabled = isResponse; //by sushil
            }
        }

        /// <summary>
        /// Navigates the previous page.
        /// </summary>
        public override void NavigatePrevious()
        {
            _isbackToSummaryOrPrevious = true;
            if (fvTechnical_TaxDependency.Enabled)
                fvTechnical_TaxDependency.UpdateItem(false);

            if (_isChangeMade || (fvTechnical_TaxDependency.Enabled && IsIndividualsChanged()))
            {
                TechnicalSessionContext.Instance.IsPreviousAction = true;
                ShowPopupInfo(IntakeResourceManager.SAVE_CHAGNES_ALERT);
            }
            else
            {
				ClearHistoryRecords();
                base.NavigatePrevious(n => n.Name != IntakeConstants.TAX_DEPENDENCY_SUMMARY_AE && !n.DetailScreen && n.Completed && n.Visible);
            }
        }

        /// <summary>
        /// Navigates to next active record if it exists in context else Next summary page in workflow.
        /// </summary>
        public override void NavigateNext()
        {
            if (_isValidated)
            {
                ClearSessionVariables();
                base.NavigateNext();
            }            
        }

        /// <summary>
        /// Clears History Records if it is redirecting to Other page Otherthan using BackToSummary
        /// </summary>
        protected void ClearHistoryRecords()
        {
            TechnicalSessionContext.Instance.BeginDate = null;
            TechnicalSessionContext.Instance.Enddate = null;
            TechnicalSessionContext.Instance.IsPersonDemoBackToSummary = false;
        }
    }
}

