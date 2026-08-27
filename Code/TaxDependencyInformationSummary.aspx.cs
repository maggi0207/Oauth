///////////////////////////////////////////////////////////////////////////////////////////////////////
//
// File:      TaxDependencyInformationSummary.aspx.cs
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
///////////////////////////////////////////////////////////////////////////////////////////////////////

using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Extensions;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Services;
using Dhss.Framework.Web.UI.Workflow;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.UI.WebControls;
using Dhss.Framework.Extensions;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{
    [Workflow]
    [ExcludeFromCodeCoverage]
    public partial class TaxDependencyInformationSummary : Dhss.Assist.WorkerWeb.Web.Infrastructure.Workflow.WorkflowPage<Technical_Case>
    {
        /// <summary>
        /// Handles Page_Load event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ClearSessionVariables();

            Master.Master.FooterSectionConfigure(FooterBodyConfiguration.AddnotePreviousNext);

            //Creates new records if the PersonId is not found in the PersonDemographics table
            if (!IsPostBack)
            {
                HistorySearch.LoadHistorySearchSession(ddeBeginDate, ddeEndDate, Request.UrlReferrer, typeof(TaxDependencyInformation).Name);
                if (!TechnicalSessionContext.Instance.IsTaxDependencyAdded) //Avoiding subsequent calls to improve performance.
                {
                    TechnicalContextOperations.CreateNewIndividualTax(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                    TechnicalSessionContext.Instance.IsTaxDependencyAdded = true;
                    
                }
                ddeBeginDate.Focus();
            }
            else
            {
                /*
                string postBackStr = Request.Params.Get("__EVENTARGUMENT");
                if (!string.IsNullOrEmpty(postBackStr) && postBackStr != "BC:0" && !postBackStr.Contains("CLICK:") && !postBackStr.Contains("TxtRFACaseNumber_ButtonClick"))
                {
                    NavigateToNextPage();
                }
                */
            }
        }

        protected void gvASPxGridView_CustomCallback(object sender, DevExpress.Web.ASPxGridView.ASPxGridViewCustomCallbackEventArgs e)
        {
            NavigateToNextPage();
        }

        #region TopDateRow
        /// <summary>
        /// Creates a new Protected SSI record and assignes the new Key
        /// </summary>
        /// <param name="sender">Button btnAddNew</param>
        /// <param name="e"> click-event</param>
        protected void BtnRetrieve_Click(object sender, EventArgs e)
        {
            HistorySearch.SetHistorySearchSession(ddeBeginDate.Value.AsDateTime(), ddeEndDate.Value.AsDateTime());
            RetrieveHistoryRecords();
        }
        /// <summary>
        /// Clears date fields
        /// </summary>
        /// <param name="sender">Button btnAddNew</param>
        /// <param name="e"> click-event</param>
        protected void BtnClear_Click(object sender, EventArgs e)
        {

            ddeBeginDate.Text = string.Empty;
            ddeEndDate.Text = string.Empty;
            ddeBeginDate.Value = null;
            ddeEndDate.Value = null;
            HistorySearch.ClearHistorySearchSession();
            RetrieveHistoryRecords();
        }
        /// <summary>
        /// Retrives History records and Binds the grid. 
        /// </summary>
        private void RetrieveHistoryRecords()
        {
            ServicesTracingHub.TraceWriter.WriteLine("TaxDependencyInformationSummary.RetrieveHistoryRecords - Start");
            gvASPxGridView.DataBind();
            ServicesTracingHub.TraceWriter.WriteLine("TaxDependencyInformationSummary.RetrieveHistoryRecords - End");
        }
        #endregion

        /// <summary>
        /// Handles DsTaxDependancy_Selecting event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>      
        protected void DsTaxDependancy_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("TaxDependencyInformationSummary.DsTaxDependancy_Selecting - Start");
            e.Result = GetResult();
            ServicesTracingHub.TraceWriter.WriteLine("TaxDependencyInformationSummary.DsTaxDependancy_Selecting - End");
        }

        /// <summary>
        /// Gets TaxDependency of an individual.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Technical_TaxDependency> GetResult()
        {
            //Getting ApplicationID from Session                        
            int applicationId = Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key);
            IEnumerable<Technical_TaxDependency> result;
            result = TechnicalCommon.IsHistorySearchMode(ddeBeginDate.Value, ddeEndDate.Value)
                ? TechnicalContextOperations.GetHistoryRecordsTaxDependency(applicationId, ddeBeginDate.Value,
                    ddeEndDate.Value)
                : TechnicalContextOperations.GetAllActiveRecordsTaxDependency(applicationId);

            return result;
        }

        /// <summary>
        /// Raises Click on Show Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnViewDetails_Click(object sender, EventArgs e)
        {
            NavigateToNextPage();
        }

        /// <summary>
        ///  Sets the Selected value from the Gridview on the focused row and Navigate to next page.
        /// </summary>
        protected void NavigateToNextPage()
        {
            if (gvASPxGridView.FocusedRowIndex != -1)
            {
                int taxDependentID = Convert.ToInt32(gvASPxGridView.GetRowValues(gvASPxGridView.FocusedRowIndex, "TaxDependentID"));
                TechnicalSessionContext.Instance.TaxDependentID = taxDependentID;
                NavigateTo(n => n.Name == IntakeConstants.TAX_DEPENDENCY_AE, p => ((Technical_TaxDependency)p).TaxDependentID == TechnicalSessionContext.Instance.TaxDependentID);
            }
        }

        /// <summary>
        /// Raises On DataBound Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GvASPxGridView_DataBound(object sender, EventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("TaxDependencyInformationSummary.GvASPxGridView_DataBound - Start");
            gvASPxGridView.Bind<TaxDependencyInformationSummaryMetadata>(b => b.ApplicationEntityID);
            gvASPxGridView.Bind<TaxDependencyInformationSummaryMetadata>(b => b.FileTaxReturnInCurrentYearIndicator);
            gvASPxGridView.Bind<TaxDependencyInformationSummaryMetadata>(b => b.HasTaxDeductionIndicator);
            gvASPxGridView.Bind<TaxDependencyInformationSummaryMetadata>(b => b.PrimaryTaxFilerIndicator);
            ServicesTracingHub.TraceWriter.WriteLine("TaxDependencyInformationSummary.GvASPxGridView_DataBound - End");
        }

        /// <summary>
        /// Navigates to next screen
        /// </summary>
        public override void NavigateNext()
        {
            int taxDependentId = GetIncompleteFirstRecord();
            if (TechnicalContextOperations.IsCaseRenewalOrReactivate() && !CurrentWorkflowPage.Completed && gvASPxGridView.VisibleRowCount > 0)
            {
                SetPageComplete(false);
                NavigateToNextPage();
            }
            else if (taxDependentId != 0)
            {
                SetPageComplete(false);
                NavigateTo(n => n.Name == IntakeConstants.TAX_DEPENDENCY_AE, p => ((Technical_TaxDependency)p).TaxDependentID == taxDependentId);
            }
            else
            {
                if (!CurrentWorkflowPage.Completed && !IntakeContext.Instance.IsRenewal) base.NavigateNext();
                SetPageComplete();
                base.NavigateNext();
            }
        }

        /// <summary>
        /// Returns first incompleted record.
        /// </summary>
        /// <returns></returns>
        private int GetIncompleteFirstRecord()
        {
            int taxDependentId = 0;

            for (int rwCount = 0; rwCount < gvASPxGridView.VisibleRowCount; rwCount++)
            {
                Int16 syncState = Convert.ToInt16(gvASPxGridView.GetRowValues(rwCount, "SyncState"));
                if (syncState == 0 || syncState == 1)
                {
                    taxDependentId = Convert.ToInt32(gvASPxGridView.GetRowValues(rwCount, "TaxDependentID"));
                    break;
                }
            }
            return taxDependentId;
        }
    }
}