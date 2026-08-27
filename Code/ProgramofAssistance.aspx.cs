///////////////////////////////////////////////////////////////////////////////////////////////////////
//
// File:      PrimaryPersonAssignment.aspx.cs
//
// Created On: Wednesday, February 27, 2013 4:43:02 PM
// Created By: Keerthi.Pathipaka
//
// This file may contain sensitive and/or confidential information and may not be
// distributed without written permission of Delaware Department of Health and 
// Social Services.
//6823	add	Keerthi.Pathipaka	2/27/2013 4:43:02 PM	$/DCIS Modernization/Release1/Dev/w1/WebSites/WorkerWeb/Intake/ApplicationEntry/Technical/ProgramofAssistance.aspx.cs	newprgm

// #      Type        User                    Date        Comment                                      
// ------ ----------- ----------------------- ----------- -------------------------------------------- 
// 6823	   add	       Keerthi.Pathipaka        2/27/2013  newprgm
//                     Anbu                    11/5/2013   Implemeted Work Item Creation         
// 242269  CR          Saicharitha Movva       07/07/2024  CLASI: RC214 -Modify how and when we set RC 214 (Short Term Fix)
///////////////////////////////////////////////////////////////////////////////////////////////////////

using DevExpress.Web.ASPxEditors;
using Dhss.Assist.WorkerWeb.BusinessLogic;
using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Entity.ImageIntegration;
using Dhss.Assist.WorkerWeb.Entity.SharedErrorMessages;
using Dhss.Assist.WorkerWeb.Web.ImageIntegration;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Controls;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Extensions;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Services;
using Dhss.Assist.WorkerWeb.Web.Services.Application.WorkerDashboard;
using Dhss.Framework;
using Dhss.Framework.Extensions;
using Dhss.Framework.Web.UI.Workflow;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Dhss.Assist.WorkerWeb.Entity.DTO.Common;
using System.Web.UI;
using System.IO;
using Dhss.Assist.WorkerWeb.Web.Services.Application.CaseloadManagement;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{
    [Workflow]
    [ExcludeFromCodeCoverage]
    public partial class ProgramofAssistance : Dhss.Assist.WorkerWeb.Web.Infrastructure.Workflow.WorkflowPage<Technical_ProgramDetail>
    {
        int _programDetailId;  //TODO: Replace with Session after workflow Integration       
        bool _request = false;
        bool _isRefreshRecord = true;
        int _requester;
        bool _validate = true;
        string _programCode = string.Empty;
        private const string RENEWAL_APPLICATION_TYPE = "R";
        private const string WORK_ITEM_OPEN_STATUS = "O";
        private const int DEFAULT_POOL_NUMBER = -1;
        private const string SWTSPI_RETRO_MA_INIT_CODE = "R2";
        private const string RETRO_MA_MAX_MONTHS_ERROR = "The maximum number of months that can be requested to Retro MA field is 2.";
        private const string RETRO_MSP_MAX_MONTHS_ERROR = "The maximum number of months that can be requested to Retro MSP field is 2.";
        private int _applicationId;
        private bool _isChangeMade; // Tracks if any edit or change made is on the formview 
        private bool _isbackToSummaryOrPrevious; // Back-to-summary or Previous button clicked flag
        private List<int> _newIndivRequested;
        private bool _isShowAGReviewDuePopup;

        /// <summary>
        /// Occurs on Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            _applicationId = Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key);
            InitiazeSessions();
            Master.Master.FooterSectionConfigure(FooterBodyConfiguration.AddnoteSavePreviousNext);
            if (IsPostBack)
            {

                ASPxButton btnPageSaveData = Master.ViewBodyActionBar.FindControl("btnPageSave") as ASPxButton;
                if (btnPageSaveData != null)
                {
                    btnPageSaveData.Click += RefreshAnchorObject;
                }
            }
            else
            {
                ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Page_Load.EnableProgramDetails - Start");
                IntakeContext.Instance.CaseRemarkDetails = null;
                EnableProgramDetails();
                ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Page_Load.EnableProgramDetails - End");
                fvTechnical_ProgramDetail.FindControl("cbCashRequester").Focus();
                if (TechnicalSessionContext.Instance.IsShowAGReviewPopUp)
                {
                    TechnicalSessionContext.Instance.IsShowAGReviewPopUp = false;
                    ShowErrPopupInformation(ErrorMessages.WWPOA1);
                }

            }

        }


        /// <summary>
        /// InitiazeSessions
        /// </summary>
        protected void InitiazeSessions()
        {
            _programDetailId = this.AnchorObject.ProgramDetailID;
            if (!IsPostBack)
            {
                _programCode = this.AnchorObject.ProgramCode;
                hfProgramCode.Value = _programCode;
            }
            else
            {
                _programCode = hfProgramCode.Value;
            }
            if (_programCode.IsNullOrEmpty()) _programCode = this.AnchorObject.ProgramCode;
            _request = Convert.ToBoolean(this.AnchorObject.Request);
            _requester = Convert.ToInt32(this.AnchorObject.RequesterNumber);
            var technicalContext = new TechnicalContextImpl();
            var Entity = technicalContext.Technical_ApplicationEntity
                                                   .Where(p => p.ApplicationEntityID == this.AnchorObject.RequesterNumber)
                                                   .Select(n => new { n.EntityID })
                                                   .FirstOrDefault();
            Master.CurrentPersonSelectedId = Entity.EntityID;

        }

        /// <summary>
        /// Reloads disabled record after synchronization with DB2.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RefreshAnchorObject(object sender, EventArgs e)
        {
            RefreshObject();
        }
        /// <summary>
        /// 
        /// </summary>
        private void RefreshObject()
        {
            if (_isRefreshRecord)
            {
                ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.RefreshAnchorObject - Start");
                Technical_ProgramDetail progDetailRec = GetProgramDetailRecord();
                //If request is No and changing the requester name.
                if (!_request)
                    TechnicalSessionContext.Instance.ProgramDetailID = progDetailRec.ProgramDetailID;
                ReloadAnchorObject();
                int? personid = this.AnchorObject.ProgramDetailID; //Executing query atleast one time.
                ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.RefreshAnchorObject - End");
                NavigateTo(progDetailRec);
            }
            else
            {
                //If request is No and changing the requester name.
                if (!_request)
                    TechnicalSessionContext.Instance.ProgramDetailID = _programDetailId;
            }
        }

        /// <summary>
        /// Set Dates to current date and due date to 1 year from current date.
        /// </summary>
        private void SetMaxDate()
        {
            if (!(TechnicalContextOperations.IsSDXCase() && _programCode == "MA")) //For SDX case mode & MA  do not validate for future date
            {
                ASPxDateEdit dtCashFilingDate = fvTechnical_ProgramDetail.FindControl("dtCashFilingDate") as ASPxDateEdit;
                if (dtCashFilingDate != null)
                    dtCashFilingDate.MaxDate = SystemDateTime.Now;
            }
        }

        /// <summary>
        /// Set Dates to current date and due date to 1 year from current date.
        /// </summary>
        private void SetFoodBenefitsMaxDate()
        {
            ASPxDateEdit dtProtectedFilingDate = fvTechnical_FoodBenefits.FindControl("dtProtectedFilingDate") as ASPxDateEdit;
            if (dtProtectedFilingDate != null)
                dtProtectedFilingDate.MaxDate = SystemDateTime.Now;
            dtProtectedFilingDate.MinDate = Convert.ToDateTime(IntakeContext.Instance.CaseFilingDate); //"The Protected Filing Date must be equal to or greater than the Case Filing Date")

        }

        /// <summary>
        /// Occurs on Page LoadComplete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Page_LoadComplete.BindIndividuals - Start");
            BindIndividuals();
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Page_LoadComplete.BindIndividuals - End");
        }

        /// <summary>
        /// ProgramBenefitReadOnly
        /// </summary>
        protected void ProgramBenefitReadOnly()
        {
            fvTechnical_ProgramDetail.Enabled = false;
            QMBCheckList.Disabled = true;
            lstChosenIndividuals.Enabled = false;
            ContentPlaceHolder mpContentPlaceHolder = Master.ViewBodyActionBar;
            (mpContentPlaceHolder.FindControl("btnPageSave") as ASPxButton).Enabled = false;
        }

        /// <summary>
        /// Raises on LinqDataSource OnSelecting event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DsTechnical_ProgramDetail_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.DsTechnical_ProgramDetail_Selecting - Start");
            e.Result = GetProgramDetailRecord();
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistanceDsTechnical_ProgramDetail_Selecting - End");
        }

        /// <summary>
        ///  Gets Program of assistance record.
        /// </summary>
        /// <returns></returns>
        private Technical_ProgramDetail GetProgramDetailRecord()
        {
            if (TechnicalSessionContext.Instance.ProgramDetailID != 0)
                return TechnicalContextOperations.ProgramDetailContext(TechnicalSessionContext.Instance.ProgramDetailID).FirstOrDefault();
            else
                return TechnicalContextOperations.ProgramDetailContext(_programCode).FirstOrDefault();
        }

        /// <summary>
        /// Handles Technical_DisabledChildren_Selecting event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Technical_DisabledChildren_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_DisabledChildren_Selecting - Start");
            if (TechnicalBusinessLogic.IsDisabledChildrenBenefitsProgram(_programDetailId, _programCode))
            {
                e.Result = TechnicalContextOperations.DisabledContext(_programDetailId, _requester);
            }
            else
                e.Result = null;
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_DisabledChildren_Selecting - End");
        }

        /// <summary>
        /// Occurs on LinqDataSource selecting event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Technical_FoodBenefits_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_FoodBenefits_Selecting - Start");
            if (TechnicalBusinessLogic.IsFoodBenefitsProgram(_programDetailId, _programCode))
            {
                e.Result = TechnicalContextOperations.FoodBenefitsContext(_programDetailId, _requester);
            }
            else
                e.Result = null;
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_FoodBenefits_Selecting - End");
        }

        /// <summary>
        /// Occurs on LinqDataSource selecting event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Technical_MedicalAssistanceProgram_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_MedicalAssistanceProgram_Selecting - Start");
            if (TechnicalBusinessLogic.IsMedicalAssistanceProgram(_programDetailId, _programCode))
            {
                e.Result = TechnicalContextOperations.MedicalAssistanceContext(_programDetailId, _requester);
            }
            else
                e.Result = null;
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_MedicalAssistanceProgram_Selecting - End");
        }

        /// <summary>
        /// Occurs on LinqDataSource selecting event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Technical_QMBProgram_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_QMBProgram_Selecting - Start");
            if (TechnicalBusinessLogic.IsQMBProgram(_programDetailId, _programCode))
            {
                e.Result = TechnicalContextOperations.QmbProgramDetails(_programDetailId, _requester);
            }
            else
                e.Result = null;
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_QMBProgram_Selecting - End");
        }

        /// <summary>
        /// Bind Individuals to the checkBoxList
        /// </summary>
        private void BindIndividuals()
        {
            PersonWithApplicationEntityId personAppEntityId = new PersonWithApplicationEntityId();
            cklIndividuals.DataSource = personAppEntityId.Values;
            cklIndividuals.DataBind();
        }

        /// <summary>
        ///Flag for Cash controls to visible
        /// </summary>
        private void CashVisibleFlag()
        {
            cashLabel.Visible = true;
        }

        /// <summary>
        /// Flag for Child care controls to visible
        /// </summary>
        private void ChildCareVisibleFlag()
        {
            childcareLabel.Visible = true;
        }

        /// <summary>
        /// Flag for Disabled Children controls to visible
        /// </summary>
        private void DisabledVisibleFlag()
        {
            DisabledChildrenLabel.Visible = true;
            fvTechnical_DisabledChildren.Visible = true;
        }

        /// <summary>
        /// Flag for FoodBenefits controls to invisible
        /// </summary>
        private void FoodBenefitVisibleFlag()
        {
            FBBenefits.Visible = true;
            fvTechnical_FoodBenefits.Visible = true;
        }
        /// <summary>
        /// Flag for MedicalBenefits controls to invisible
        /// </summary>
        private void MedicalBenefitVisibleFlag()
        {
            MedicalLabel.Visible = true;
            fvTechnical_MedicalAssistance.Visible = true;
        }

        /// <summary>
        /// Flag for Qualified Medicare Beneficiary Controls to invisible
        /// </summary>
        private void QMBVisibleFlag()
        {
            QMBLabel.Visible = true;
            fvTechnical_QMB.Visible = true;
        }

        /// <summary>
        /// Switch cases for Benefits calls the methods for controls to invisible
        /// </summary>
        protected void EnableProgramDetails()
        {

            if (string.IsNullOrEmpty(_programCode))
                throw new Exception(IntakeResourceManager.PROGRAM_CODE_CANNOT_NULLEMPTY);
            switch (_programCode)
            {
                case "CA":
                    {
                        CashVisibleFlag();
                        BindSelectedIndividuals(TechnicalContextOperations.LoadCashIndividuals(_programDetailId));
                        break;
                    }
                case "CC":
                    {
                        ChildCareVisibleFlag();
                        BindSelectedIndividuals(TechnicalContextOperations.LoadChildCareIndividuals(_programDetailId));
                        break;
                    }
                case "DC":
                    {
                        DisabledVisibleFlag();
                        BindSelectedIndividuals(TechnicalContextOperations.LoadDisabledChildrenIndividuals(_programDetailId));
                        break;
                    }
                case "FS":
                    {
                        FoodBenefitVisibleFlag();
                        BindSelectedIndividuals(TechnicalContextOperations.LoadFoodBenefitsIndividuals(_programDetailId));
                        break;
                    }
                case "MA":
                    {
                        MedicalBenefitVisibleFlag();
                        BindSelectedIndividuals(TechnicalContextOperations.LoadMedicalAssistanceIndividuals(_programDetailId));

                        break;
                    }
                case "QM":
                    {
                        QMBVisibleFlag();
                        BindSelectedIndividuals(TechnicalContextOperations.LoadQualifiedMemberBeneficiaryIndividuals(_programDetailId));
                        break;
                    }
            }

        }

        /// <summary>
        /// Binds List with selected individuals.
        /// </summary>
        /// <param name="appPersons"></param>
        private void BindSelectedIndividuals(IList<KeyValuePair<string, string>> appPersons)
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.BindSelectedIndividuals - Start");

            lstChosenIndividuals.DataSource = appPersons;
            lstChosenIndividuals.DataBind();
            TechnicalSessionContext.Instance.IndividualsRequested = appPersons.Select(x => x.Key).ToList().Select(int.Parse).ToList();

            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.BindSelectedIndividuals - End");
        }

        /// <summary>
        /// Handles Technical_FoodBenefits_DataBound event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Technical_FoodBenefits_DataBound(object sender, EventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_FoodBenefits_DataBound - Start");
            if (fvTechnical_FoodBenefits.Visible)
            {
                BindFoodBenefitsEntities();
                SetFoodBenefitsMaxDate();
            }
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_FoodBenefits_DataBound - End");
        }
        /// <summary>
        /// Handles Technical_DisabledChildren_DataBound event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Technical_DisabledChildren_DataBound(object sender, EventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_DisabledChildren_DataBound - Start");
            if (fvTechnical_DisabledChildren.Visible)
                BindDisabledChildrenEntities();
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_DisabledChildren_DataBound - End");
        }

        /// <summary>
        /// Handles Technical_MedicalAssistance_DataBound event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Technical_MedicalAssistance_DataBound(object sender, EventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_MedicalAssistance_DataBound - Start");
            if (fvTechnical_MedicalAssistance.Visible)
                BindMedicalAssistanceEntities();
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_MedicalAssistance_DataBound - End");
        }

        /// <summary>
        /// Handles Technical_QMB_DataBound event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Technical_QMB_DataBound(object sender, EventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_QMB_DataBound - Start");
            if (fvTechnical_QMB.Visible)
                BindQMBEntities();
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_QMB_DataBound - End");
        }

        /// <summary>
        /// Handles Technical_ProgramDetail_DataBound event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Technical_ProgramDetail_DataBound(object sender, EventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_ProgramDetail_DataBound - Start");
            SetMaxDate();
            DisableHistoryRecord();
            RenewalModeScenario();

            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.Technical_ProgramDetail_DataBound - End");
        }

        /// <summary>
        /// Required verification dates are blanked out in Renewal Mode for the first time
        /// </summary>
        protected void RenewalModeScenario()
        {
            if (IntakeContext.Instance.CaseMode == "R" && !CurrentWorkflowPage.Completed && !IsSummaryPagesComplete())
            {
                ASPxDateEdit dtCashFilingDate = fvTechnical_ProgramDetail.FindControl("dtCashFilingDate") as ASPxDateEdit;
                dtCashFilingDate.Text = string.Empty;
                dtCashFilingDate.Value = null;
                dtCashFilingDate.MinDate = SystemDateTime.Now.AddDays(-60); //In EligRev mode, filing date cannot be < than 60 days from current date
            }
        }

        /// <summary>
        /// Disable History Record
        /// </summary>
        private void DisableHistoryRecord()
        {
            Int16 syncState = Convert.ToInt16(this.AnchorObject.SyncState);
            bool isHistoryRecord = TechnicalCommon.IsHistoryRecord((fvTechnical_ProgramDetail.FindControl("hfHistoryCode") as HiddenField).Value);
            if (isHistoryRecord || !_request)
            {
                fvTechnical_ProgramDetail.Enabled = !isHistoryRecord;
                fvTechnical_DisabledChildren.Enabled = false;
                fvTechnical_FoodBenefits.Enabled = false;
                fvTechnical_MedicalAssistance.Enabled = false;
                fvTechnical_QMB.Enabled = false;
                ASPxPopupClientControl.Enabled = false;
                QMBCheckList.Disabled = true;
                lstChosenIndividuals.Enabled = false;
                ASPxLabel7.Enabled = false;
                ContentPlaceHolder mpContentPlaceHolder = Master.ViewBodyActionBar;
                (mpContentPlaceHolder.FindControl("btnPageSave") as ASPxButton).Enabled = !isHistoryRecord;
                (fvTechnical_ProgramDetail.FindControl("dtCashFilingDate") as ASPxDateEdit).Enabled = false;
                (fvTechnical_ProgramDetail.FindControl("dtCashLastVerificationDate") as ASPxDateEdit).Enabled = false;
            }
            else
            {
                RenewalModeScenario();
            }
            //Setting Page Complete for synced records.
            //SetCompletePage(syncState);
        }

        /// <summary>
        /// Sets the page complete based on syncstate.
        /// </summary>
        private void SetCompletePage(Int16 syncState)
        {
            if (TechnicalContextOperations.IsCaseRenewalOrReactivate() && IsSummaryPagesComplete() && syncState == 3)
                SetPageComplete(true);
            else if (!TechnicalContextOperations.IsCaseRenewalOrReactivate() && syncState == 3)
                SetPageComplete(true);
            else if (syncState == 1)
                SetPageComplete(false);
        }


        /// <summary>
        /// Returns true if the page is modified.
        /// </summary>
        /// <returns></returns>
        private bool IsIndividualsChanged()
        {
            return (hfIsPageModified.Value == "Y");
        }

        #region ConditionalValidation
        /// <summary>
        /// Handles CbFBIdentity_SelectedIndexChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbFBIdentity_SelectedIndexChanged(object sender, EventArgs e)
        {
            ASPxComboBox cbFBIdentity = sender as ASPxComboBox;
            if (cbFBIdentity != null)
            {
                BindFBIdentityConditionally(Convert.ToString(cbFBIdentity.Value));
            }
        }

        /// <summary>
        /// Handles CbFBIdentity_DataBound event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbFBIdentity_DataBound(object sender, EventArgs e)
        {
            ASPxComboBox cbFBIdentity = sender as ASPxComboBox;
            if (cbFBIdentity != null)
            {
                BindFBIdentityConditionally(Convert.ToString(cbFBIdentity.Value));
            }
        }

        #endregion

        /// <summary>
        /// Handles DelayBefenifitsQuest_DataBound event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DelayBefenifitsQuest_DataBound(object sender, EventArgs e)
        {
            DelayBefenifitsQuestMandatoryConditionally();
        }

        /// <summary>
        /// Handles Page_PreRenderComplete event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            if (fvTechnical_FoodBenefits.Visible)
            {
                ASPxComboBox cbCashRequester = (ASPxComboBox)fvTechnical_ProgramDetail.FindControl("cbCashRequester");
                ASPxLabel lbDelayBefenifitsQuest = (ASPxLabel)fvTechnical_FoodBenefits.FindControl("lbDelayBefenifitsQuest");
                if (cbCashRequester != null && (cbCashRequester).SelectedIndex != -1)
                {
                    string m_Name = cbCashRequester.SelectedItem.Text;
                    if (!string.IsNullOrEmpty(m_Name) && lbDelayBefenifitsQuest != null)
                    {
                        lbDelayBefenifitsQuest.Text = lbDelayBefenifitsQuest.Text.Replace("{Name}", m_Name);
                    }
                }
                ApplyConditionValidation();
            }
        }

        /// <summary>
        /// Navigates the previous page.
        /// </summary>
        public override void NavigatePrevious()
        {
            _isbackToSummaryOrPrevious = true;
            if (fvTechnical_ProgramDetail.Enabled)
                _isChangeMade = IsProgDetailsModified();
            else
                _isChangeMade = false;

            if (_isChangeMade)
            {
                TechnicalSessionContext.Instance.IsPreviousAction = true;
                ShowPopupInfo(IntakeResourceManager.SAVE_CHAGNES_ALERT);
            }
            else
            {
                CleanUpSessionVariables();
                base.NavigatePrevious(n => n.Visible && n.Enabled && n.Name != IntakeConstants.PROGRAM_OF_ASSISTANCE_SUMMARY_AE);
            }
        }

        /// <summary>
        /// ProcessWorkItem
        /// </summary>
        private void ProcessWorkItem()
        {
            TechnicalContextImpl context = new TechnicalContextImpl();
            WorkItemCreationRequest request = new WorkItemCreationRequest();
            request.WorkItemCreationCriterias = new WorkItemCreationCriteria();
            request.WorkItemCreationCriterias.ApplicationId = _applicationId;
            request.WorkItemCreationCriterias.WorkItemStatusCode = WORK_ITEM_OPEN_STATUS;
            request.WorkItemCreationCriterias.AssignedBy = WorkerSessionContext.Instance.LoggedInWorkerDetails.WorkerId;
            request.UserName = HttpContext.Current.User.Identity.Name;

            /*Create workitems only for non-self service applications..Compare with bot*/
            int assistApplication = context.Technical_ASSISTMain.Where(a => a.CaseApplicationID == _applicationId && a.ASSISTNumber > 0).Count();
            if (assistApplication == 0)
            {
                Technical_Case caseRecord = context.Technical_Case.Where(a => a.ApplicationID == _applicationId).FirstOrDefault();
                if (caseRecord != null)
                    assistApplication = context.Technical_ASSISTMain.Where(a => (a.DCISIICaseNumber == caseRecord.CaseNumber || a.CASENumber == caseRecord.CaseNumber) && a.ASSISTNumber > 0).Count();
            }
            if (assistApplication == 0)
            {
                /* As per the business case should exist, if the user reach here. Should not handle null exception */
                Technical_Case applicationCase = context.Technical_Case.Where(a => a.ApplicationID == _applicationId).FirstOrDefault();
                request.WorkItemCreationCriterias.WorkItemTypeCode = (applicationCase != null && applicationCase.CaseModeCode == RENEWAL_APPLICATION_TYPE) ? ReferenceTableConstants.WorkItemType.Renewals.Value() :
                                               ReferenceTableConstants.WorkItemType.Applications.Value();

                int? poolNumber = DEFAULT_POOL_NUMBER;

                Technical_Application application = context.Technical_Application.Where(a => a.ApplicationID == _applicationId).FirstOrDefault();
                IQueryable<Technical_CaseLoadWorker> caseloadExists = context.Technical_CaseLoadWorker.Where(x => x.CaseLoadWorkerID == application.CaseLoadWorkerIDNO);
                if (caseloadExists.Count() > 0)
                {
                    Technical_CaseLoadWorker caseloadWorker = caseloadExists.FirstOrDefault();
                    var workerDetailsRequest = new WorkerDetails()
                    {
                        IsDashboardWorker = false
                    };
                    //Getting worker details from the worker ODATA service
                    WorkerProfile worker = new WorkerProfile();
                    if (caseloadWorker.WorkerID != null)
                        worker = ServicesApplicationHub.WorkerInformation.GetWorkerDetailsByWorkerId(Convert.ToInt32(caseloadWorker.WorkerID));

                    if (worker != null)
                    {
                        if (Convert.ToInt32(worker.PoolID) > 0)
                            poolNumber = Convert.ToInt32(worker.PoolID);
                    }
                }
                GetPrograms(context, request, poolNumber, applicationCase.CaseNumber);
            }
        }

        private void GetPrograms(TechnicalContextImpl context, WorkItemCreationRequest request, int? poolNumber, decimal? _caseNumber)
        {
            /* Get only the programs that are requested */
            //var programCodes1 = (from programDetail in context.Technical_ProgramDetail
            //                    where (programDetail.Request == true && programDetail.ApplicationEntity.ApplicationID == _applicationId && programDetail.HistoryCode != IntakeConstants.HISTORY_RECORD_CODE)
            //                    select new { ProgramCode = programDetail.ProgramCode, ProgramFilingDate = programDetail.ProgramFilingDate }).ToList();

            var programCodes = ServicesApplicationHub.IntakeTechnical.GetProgramofAssistanceRequestedPrograms(_applicationId);



            DateTime? newestProgramFilingDate = null;
            /* Get all the programs applied and the oldest program filing date among those */
            request.WorkItemCreationCriterias.WorkItemSubCategoryTypeCode = new List<string>();
            if (programCodes.Count > 0)
            {
                foreach (var programCode in programCodes)
                {
                    if (newestProgramFilingDate == null)
                        newestProgramFilingDate = programCode.ProgramFilingDate;
                    else
                    {
                        if (newestProgramFilingDate < programCode.ProgramFilingDate)
                            newestProgramFilingDate = programCode.ProgramFilingDate;
                    }
                    if (!request.WorkItemCreationCriterias.WorkItemSubCategoryTypeCode.Contains(programCode.ProgramCode))
                        request.WorkItemCreationCriterias.WorkItemSubCategoryTypeCode.Add(programCode.ProgramCode);
                }

                request.WorkItemCreationCriterias.WorkItemStartDate = (DateTime)newestProgramFilingDate;
                request.WorkItemCreationCriterias.PoolNumber = int.Parse(Convert.ToString(poolNumber));
                request.WorkItemCreationCriterias.CaseNumber = _caseNumber;
                if (WorkerDashboardSessionContext.Instance.IsSelfAssignedProcess)
                {
                    request.WorkItemCreationCriterias.IsSelfAssignedProcess = WorkerDashboardSessionContext.Instance.IsSelfAssignedProcess;
                    request.WorkItemCreationCriterias.IsSelfAssigned = WorkerDashboardSessionContext.Instance.IsSelfAssigned;
                    request.WorkItemCreationCriterias.UgtWorkItemId = WorkerDashboardSessionContext.Instance.UgtWorkItemId;
                }
                CreateWorkItemDetailsResponse response = ServicesApplicationHub.WorkerDashboard.ProcessWorkItem(request);
                //Resetting values to back
                WorkerDashboardSessionContext.Instance.IsSelfAssigned = false;
                WorkerDashboardSessionContext.Instance.UgtWorkItemId = 0;
                WorkerDashboardSessionContext.Instance.IsSelfAssignedProcess = false;
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Navigates to Next Page.
        /// </summary>
        public override void NavigateNext()
        {
            if (_validate)
            {
                if (CurrentWorkflowPage.Context.Value.IsContextComplete() && Convert.ToInt16(this.AnchorObject.SyncState) == 0 && !ApplicationEntryDataServiceLinqDataSource.IsProgramDetailsEnabled(null))
                {
                    SetPreviousPageComplete(true);
                    SetPageComplete(IntakeConstants.PROGRAM_OF_ASSISTANCE_AE, true, true);
                }
                CleanUpSessionVariables();
                if (TechnicalSessionContext.Instance.IsShowAGReviewPopUp)
                {
                    TechnicalSessionContext.Instance.IsShowAGReviewPopUp = false;
                    ShowInformationPopUp(ErrorMessages.WWPOA1);
                }
                else
                {
                    base.NavigateNext();
                }

            }
        }

        /// <summary>
        /// Event raises on List dataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LstChosenIndividuals_DataBound(object sender, EventArgs e)
        {
            if (lstChosenIndividuals.Items.Count > 0)
            {
                lstChosenIndividuals.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Clean Up page level Session variables.
        /// </summary>
        private void CleanUpSessionVariables()
        {
            TechnicalSessionContext.Instance.ProgramDetailID = 0;
            IntakeContext.Instance.CaseRemarkDetails = null;
        }

        /// <summary>
        /// IsFilingdtValid
        /// </summary>
        /// <returns></returns>
        protected bool IsFilingdtValid()
        {
            ASPxDateEdit filingDate = fvTechnical_ProgramDetail.FindControl("dtCashFilingDate") as ASPxDateEdit;
            return TechnicalBusinessLogic.IsFilingDatevalid(filingDate.Value, IntakeContext.Instance.CaseFilingDate);
        }

        /// <summary>
        /// Error PopUpAlert
        /// </summary>
        /// <param name="stralertmsg"></param>
        private void ShowErrPopupAlert(string stralertmsg)
        {
            dxPopupErr.ShowOnPageLoad = true;
            dxPopupErr.HeaderText = "Validation Error";
            ((ASPxLabel)dxPopupErr.FindControl("lblErrmessage")).Text = stralertmsg;
        }

        /// <summary>
        /// Information PopUpAlert
        /// </summary>
        /// <param name="stralertmsg"></param>
        private void ShowInformationPopUp(string stralertmsg)
        {
            poainformationpopup.ShowOnPageLoad = true;
            poainformationpopup.HeaderText = "Information";
            ((ASPxLabel)poainformationpopup.FindControl("lblInformationMessage")).Text = stralertmsg;
        }

        /// <summary>
        /// IsVerificationdtValid
        /// </summary>
        /// <returns></returns>
        protected bool IsVerificationdtValid()
        {
            ASPxDateEdit verificationdate = fvTechnical_ProgramDetail.FindControl("dtCashLastVerificationDate") as ASPxDateEdit;
            ASPxDateEdit filingDate = fvTechnical_ProgramDetail.FindControl("dtCashFilingDate") as ASPxDateEdit;
            return TechnicalBusinessLogic.IsVerificationDatevalid(filingDate.Value, verificationdate.Value);
        }

        /// <summary>
        /// Blocks Retro MA / Retro MSP value 3 when program Filing Date is on or after SWTSPI PROG-BGN-DT for INIT-CD R2.
        /// Option 3 remains in the AERTMA dropdown for Filing Dates before that configuration date.
        /// </summary>
        private void ValidateRetroMAMonths()
        {
            ASPxDateEdit filingDateEdit = fvTechnical_ProgramDetail.FindControl("dtCashFilingDate") as ASPxDateEdit;
            if (filingDateEdit == null || filingDateEdit.Value == null)
                return;

            string bgnDateValue = ReferenceTableHelper.GetReferenceTableValue("SWTSPI", "INIT-CD", "PROG-BGN-DT", SWTSPI_RETRO_MA_INIT_CODE);
            DateTime configurationStartDate;
            if (string.IsNullOrWhiteSpace(bgnDateValue) || !DateTime.TryParse(bgnDateValue, out configurationStartDate))
                return;

            if (filingDateEdit.Date.Date < configurationStartDate.Date)
                return;

            if (_programCode == "DC" && fvTechnical_DisabledChildren.Visible
                && IsRetroMAValueThree(fvTechnical_DisabledChildren.FindControl("cbDisabledRetroMA") as ASPxComboBox))
            {
                _validate = false;
                ShowErrPopupAlert(RETRO_MA_MAX_MONTHS_ERROR);
                return;
            }

            if (_programCode == "MA" && fvTechnical_MedicalAssistance.Visible
                && IsRetroMAValueThree(fvTechnical_MedicalAssistance.FindControl("cbMedicalRetroMA") as ASPxComboBox))
            {
                _validate = false;
                ShowErrPopupAlert(RETRO_MA_MAX_MONTHS_ERROR);
                return;
            }

            if (_programCode == "QM" && fvTechnical_QMB.Visible
                && IsRetroMAValueThree(fvTechnical_QMB.FindControl("cbQMBProgramRetroMA") as ASPxComboBox))
            {
                _validate = false;
                ShowErrPopupAlert(RETRO_MSP_MAX_MONTHS_ERROR);
            }
        }

        private static bool IsRetroMAValueThree(ASPxComboBox retroMaCombo)
        {
            return retroMaCombo != null && retroMaCombo.Value != null
                && Convert.ToString(retroMaCombo.Value).Trim() == "3";
        }

        /// <summary>
        /// Handles DtFilingDate_DateChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DtFilingDate_DateChanged(object sender, EventArgs e)
        {
            ValidateFilingDate();
        }

        /// <summary>
        /// Validated Filing Date.
        /// </summary>
        private void ValidateFilingDate()
        {
            if (_request)
            {


                if (!TechnicalContextOperations.IsSDXCase() && !IsFilingdtValid())
                {
                    _validate = false;
                    ShowErrPopupAlert(IntakeResourceManager.REQUESTFILINGDATE_VALIDATION);
                    return;
                }
                else
                {
                    ASPxDateEdit dtCashFilingDate = fvTechnical_ProgramDetail.FindControl("dtCashFilingDate") as ASPxDateEdit;
                    if (TechnicalContextOperations.IsSDXCase())
                    {
                        if (TechnicalBusinessLogic.IsFilingDatevalidForSDX(dtCashFilingDate.Value, IntakeContext.Instance.CaseFilingDate))
                        {
                            _validate = false;
                            ShowErrPopupAlert(IntakeResourceManager.REQUESTFILINGDATE_GRATER_THEN_CASE_FILINGDATE);
                            return;
                        }
                    }
                    if (IntakeContext.Instance.CaseMode == "R")
                    {

                        if (dtCashFilingDate.Date < SystemDateTime.Now.AddDays(-60))
                        {
                            _validate = false;
                            ShowErrPopupAlert("The Request Filing Date cannot be less than 60 days from current date.");
                            return;
                        }
                    }
                    DelayBefenifitsQuestMandatoryConditionally();
                }
            }
        }

        /// <summary>
        /// Handles DtLastVerificationDate_DateChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DtLastVerificationDate_DateChanged(object sender, EventArgs e)
        {
            ValidateVerficationDate();
        }

        /// <summary>
        /// Validates Verification Date.
        /// </summary>
        private void ValidateVerficationDate()
        {
            if (_request)
            {
                ASPxDateEdit filingDate = fvTechnical_ProgramDetail.FindControl("dtCashFilingDate") as ASPxDateEdit;

                string verificationDateMsg = IntakeResourceManager.LAST_VERIFICATION_DATE_VALIDATION_I;
                bool isValid = true;
                ASPxDateEdit verificationdate = fvTechnical_ProgramDetail.FindControl("dtCashLastVerificationDate") as ASPxDateEdit;
                if (!verificationdate.Value.IsNull())
                {
                    //ASPxDateEdit filingDate = fvTechnical_ProgramDetail.FindControl("dtCashFilingDate") as ASPxDateEdit;
                    if (!(TechnicalContextOperations.IsSDXCase() && _programCode == "MA"))
                        isValid = TechnicalBusinessLogic.IsVerificationDatevalid(filingDate.Value, verificationdate.Value);
                    else
                    {
                        isValid = Convert.ToDateTime(verificationdate.Value) >= Convert.ToDateTime(filingDate.Value);
                        verificationDateMsg = IntakeResourceManager.LAST_VERIFICATION_DATE_VALIDATION_II;
                    }
                    if (!isValid)
                    {
                        verificationDateMsg = _validate == false ? IntakeResourceManager.LAST_VERIFICATION_DATE_VALIDATION_III + verificationDateMsg : verificationDateMsg;
                        _validate = false;
                        ShowErrPopupAlert(verificationDateMsg);
                        return;
                    }
                }

                if (_programCode == "FS")
                {
                    ASPxDateEdit dtCallBackDate = fvTechnical_FoodBenefits.FindControl("dtCallBackDate") as ASPxDateEdit;
                    if (dtCallBackDate.Value != null && Convert.ToDateTime(dtCallBackDate.Value) < Convert.ToDateTime(filingDate.Value))
                    {
                        string fsErrorMsg = IntakeResourceManager.LAST_VERIFICATION_DATE_VALIDATION_IV;
                        _validate = false;
                        ShowErrPopupAlert(fsErrorMsg);
                    }
                    DelayBefenifitsQuestMandatoryConditionally();
                }
            }
            if (string.IsNullOrEmpty(_programCode))
                throw new Exception(IntakeResourceManager.PROGRAM_CODE_CANNOT_NULLEMPTY);
            btnIndividual.Focus();
            if (_programCode == "MA")
            {
                ASPxComboBox ma = fvTechnical_MedicalAssistance.FindControl("cbMedicalCRDP") as ASPxComboBox;
                ma.Focus();
            }
            if (_programCode == "DC")
            {
                ASPxComboBox dc = fvTechnical_DisabledChildren.FindControl("cbDisabledCRDP") as ASPxComboBox;
                dc.Focus();
            }
            if (_programCode == "QM")
            {
                ASPxComboBox qm = fvTechnical_QMB.FindControl("cbQMBProgramCRDP") as ASPxComboBox;
                qm.Focus();
            }
            if (_programCode == "FS")
            {
                ASPxDateEdit fs = fvTechnical_FoodBenefits.FindControl("dtProtectedFilingDate") as ASPxDateEdit;
                fs.Focus();
            }
        }

        /// <summary>
        /// Handles FvTechnical_ProgramDetail_ItemUpdating event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvTechnical_ProgramDetail_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            if (!_isbackToSummaryOrPrevious)
            {
                if (WorkflowSession.Instance.CurrentFrame.CurrentEntity.Completed)
                {
                    SetPageComplete(!TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues));
                }
                e.Cancel = WorkflowSession.Instance.CurrentFrame.CurrentEntity.Completed;
            }
            else
            {
                _isChangeMade = TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Updating Disabled children page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvTechnical_DisabledChildren_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            if (!_isbackToSummaryOrPrevious)
            {
                if (fvTechnical_DisabledChildren.Visible)
                {
                    int requesterID = GetRequesterNumber();
                    if (_requester == requesterID)
                    {
                        if (WorkflowSession.Instance.CurrentFrame.CurrentEntity.Completed)
                        {
                            // if values are changed in the formview, the function will set page complete to false
                            SetPageComplete(!TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues));
                        }
                        //cancel update operation if nothing is changed
                        e.Cancel = WorkflowSession.Instance.CurrentFrame.CurrentEntity.Completed;
                    }
                    else
                    {
                        SetPageComplete(false);
                        TechnicalContextOperations.UpdateDisabledChildrenRequesterDetails(_programDetailId, requesterID, (OrderedDictionary)e.NewValues);
                        e.Cancel = true;
                    }
                }
            }
            else
            {
                _isChangeMade = TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Handles FvTechnical_FoodBenefits_ItemUpdating event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvTechnical_FoodBenefits_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            if (!_isbackToSummaryOrPrevious)
            {
                if (fvTechnical_FoodBenefits.Visible)
                {
                    int requesterID = GetRequesterNumber();
                    if (_requester == requesterID)
                    {
                        if (WorkflowSession.Instance.CurrentFrame.CurrentEntity.Completed)
                        {
                            // if values are changed in the formview, the function will set page complete to false
                            SetPageComplete(!TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues));
                        }
                        //cancel update operation if nothing is changed
                        e.Cancel = WorkflowSession.Instance.CurrentFrame.CurrentEntity.Completed;
                    }
                    else
                    {
                        SetPageComplete(false);
                        TechnicalContextOperations.UpdateFoodBenefitsRequesterDetails(_programDetailId, requesterID, (OrderedDictionary)e.NewValues);
                        e.Cancel = true;
                    }
                }
            }
            else
            {
                _isChangeMade = TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Handles FvTechnical_MedicalAssistance_ItemUpdating event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvTechnical_MedicalAssistance_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            if (!_isbackToSummaryOrPrevious)
            {
                if (fvTechnical_MedicalAssistance.Visible)
                {
                    int requesterID = GetRequesterNumber();
                    if (_requester == requesterID)
                    {
                        if (WorkflowSession.Instance.CurrentFrame.CurrentEntity.Completed)
                        {
                            // if values are changed in the formview, the function will set page complete to false
                            SetPageComplete(!TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues));
                        }
                        //cancel update operation if nothing is changed
                        e.Cancel = WorkflowSession.Instance.CurrentFrame.CurrentEntity.Completed;
                    }
                    else
                    {
                        SetPageComplete(false);
                        TechnicalContextOperations.UpdateMedicalAssistanceRequesterDetails(_programDetailId, requesterID, (OrderedDictionary)e.NewValues);
                        e.Cancel = true;
                    }
                }
            }
            else
            {
                _isChangeMade = TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Handles FvTechnical_QMB_ItemUpdating event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvTechnical_QMB_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            if (!_isbackToSummaryOrPrevious)
            {
                if (fvTechnical_QMB.Visible)
                {
                    int requesterID = GetRequesterNumber();
                    if (_requester == requesterID)
                    {
                        if (WorkflowSession.Instance.CurrentFrame.CurrentEntity.Completed)
                        {
                            // if values are changed in the formview, the function will set page complete to false
                            SetPageComplete(!TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues));
                        }
                        //cancel update operation if nothing is changed
                        e.Cancel = WorkflowSession.Instance.CurrentFrame.CurrentEntity.Completed;
                    }
                    else
                    {
                        SetPageComplete(false);
                        TechnicalContextOperations.UpdateQualifiedMemberBeneficiaryRequesterDetails(_programDetailId, requesterID, (OrderedDictionary)e.NewValues);
                        e.Cancel = true;
                    }
                }
            }
            else
            {
                _isChangeMade = TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Get RequesterNumber.
        /// </summary>
        /// <returns></returns>
        private int GetRequesterNumber()
        {
            return Convert.ToInt32((fvTechnical_ProgramDetail.FindControl("cbCashRequester") as ASPxComboBox).Value);
        }

        /// <summary>
        /// Handles BtnBackToSummary_Click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnBackToSummary_Click(object sender, EventArgs e)
        {
            _isbackToSummaryOrPrevious = true;
            if (fvTechnical_ProgramDetail.Enabled)
                _isChangeMade = IsProgDetailsModified();
            else
                _isChangeMade = false;

            if (_isChangeMade)
            {
                TechnicalSessionContext.Instance.IsPreviousAction = false;
                ShowPopupInfo(IntakeResourceManager.SAVE_CHAGNES_ALERT);
            }
            else
            {
                CleanUpSessionVariables();
                NavigatePrevious(n => n.Name == IntakeConstants.PROGRAM_OF_ASSISTANCE_SUMMARY_AE);
            }
        }

        /// <summary>
        /// Determines whether page is modified or not.
        /// </summary>
        /// <returns></returns>
        private bool IsProgDetailsModified()
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.IsProgDetailsModified - Start");
            if (_isChangeMade == false && _programCode == "CA")
            {
                fvTechnical_ProgramDetail.UpdateItem(false);
                if (!_isChangeMade)
                {
                    _isChangeMade = IsIndividualsChanged();
                }
            }
            if (_isChangeMade == false && _programCode == "CC")
            {
                fvTechnical_ProgramDetail.UpdateItem(false);
                if (!_isChangeMade)
                {
                    _isChangeMade = IsIndividualsChanged();
                }
            }

            if (_isChangeMade == false && _programCode == "DC")
            {
                fvTechnical_ProgramDetail.UpdateItem(false);
                if (!_isChangeMade)
                {
                    fvTechnical_DisabledChildren.UpdateItem(false);

                    if (!_isChangeMade)
                    {
                        _isChangeMade = IsIndividualsChanged();
                    }
                }
            }
            if (_isChangeMade == false && _programCode == "FS")
            {
                fvTechnical_ProgramDetail.UpdateItem(false);
                if (!_isChangeMade)
                {
                    fvTechnical_FoodBenefits.UpdateItem(false);

                    if (!_isChangeMade)
                    {
                        _isChangeMade = IsIndividualsChanged();
                    }
                }
            }

            if (_isChangeMade == false && _programCode == "MA")
            {
                fvTechnical_ProgramDetail.UpdateItem(false);
                if (!_isChangeMade)
                {
                    fvTechnical_MedicalAssistance.UpdateItem(false);

                    if (!_isChangeMade)
                    {
                        _isChangeMade = IsIndividualsChanged();
                    }
                }
            }
            if (_isChangeMade == false && _programCode == "QM")
            {
                fvTechnical_ProgramDetail.UpdateItem(false);
                if (!_isChangeMade)
                {
                    fvTechnical_QMB.UpdateItem(false);

                    if (!_isChangeMade)
                    {
                        _isChangeMade = IsIndividualsChanged();
                    }
                }
            }
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.IsProgDetailsModified - End");
            return _isChangeMade;
        }

        /// <summary>
        /// Shows PopupInfo.
        /// </summary>
        /// <param name="message"></param>
        private void ShowPopupInfo(string message)
        {
            //new code added for defect 38960
            dxPopupInfo.ShowOnPageLoad = true;
            ASPxLabel lblErrMessage1 = (ASPxLabel)dxPopupInfo.FindControl("lblErrMessage1");
            lblErrMessage1.Text = message;
        }

        /// <summary>
        /// Pop Up Panel Button Yes Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnPopUpYes_Click(object sender, EventArgs e)
        {
            //new code added for defect 38960
            dxPopupInfo.ShowOnPageLoad = false;
            if (TechnicalSessionContext.Instance.IsPreviousAction)
            {
                TechnicalSessionContext.Instance.IsPreviousAction = false;
                base.NavigatePrevious(n => n.Visible && n.Completed && !n.DetailScreen && n.Name != IntakeConstants.PROGRAM_OF_ASSISTANCE_SUMMARY_AE);
            }
            else
            {
                base.NavigatePrevious(n => n.Name == IntakeConstants.PROGRAM_OF_ASSISTANCE_SUMMARY_AE);
            }
        }

        /// <summary>
        /// Attaching Javascript based validation for Conditional validations
        /// </summary>
        private void ApplyConditionValidation()
        {
            var cbFBIdentity = fvTechnical_FoodBenefits.FindControl("cbFBIdentity").As<ASPxEdit>();
            var cbFSIdentityVerificationCode = fvTechnical_FoodBenefits.FindControl("cbFSIdentityVerificationCode").As<ASPxEdit>();
            var lbVerifiedBy = fvTechnical_FoodBenefits.FindControl("lbVerifiedBy").As<ASPxLabel>();
            ConditionalJavaScript.ConditionalValidation(this,
               cbFBIdentity,
               cbFSIdentityVerificationCode,
               lbVerifiedBy,
               IntakeResourceManager.REQUIRED_VERIFIED_BY,
               true,
               "Y");
        }

        /// <summary>
        /// Document Imaging Verification Button Click Event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnDocumentImagingVerification_Click(object sender, EventArgs e)
        {
            decimal caseNumber = Convert.ToDecimal(WorkflowSession.Root["CaseNumber"]);
            bool isValidRequest = DocumentHelper.IsValidRequest(Convert.ToString(caseNumber), Path.GetFileName(Request.Url.AbsolutePath));

            if (isValidRequest)
            {
                string url = WWScreenConstant.IMAGE_VERIFICATION_PATH;

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenDocumentImagingVerification", "OpenDocumentImagingVerification('" + url + "')", true);
            }
            else
            {
                ShowErrPopupAlert(ErrorMessages.WW_DIS_NOACCESS);
                return;
            }
        }
        /// <summary>
        /// Requesting the Cash Program.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbCashRequester_SelectedIndexChanged(object sender, EventArgs e)
        {
            ASPxComboBox cbRequester = (ASPxComboBox)fvTechnical_ProgramDetail.FindControl("cbCashRequester");
            var technicalContext = ServicesDataHub.Technical;

            if (cbRequester != null)
            {
                int requesterAppEntityId = Convert.ToInt32(cbRequester.Value);
                var applicationEntity = technicalContext.Technical_ApplicationEntity.Where(n => n.ApplicationEntityID == requesterAppEntityId).FirstOrDefault();
                if (applicationEntity != null)
                {
                    int personId = applicationEntity.EntityID;

                    if (personId > 0)
                    {

                        var personCount = technicalContext.Technical_ApplicationEntity.Where(n => n.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)
                                                                                                 && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)
                                                                                                 && (n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE)).Count();

                        if ((personCount > 1) && (TechnicalContextOperations.IsLivingAsCorrectional(personId, _applicationId)))
                        {
                            ShowErrPopupInformation(IntakeConstants.ERROR_BENEFIT_RQ_IN_CORR_FACILITY);
                        }
                    }
                }
            }
        }





        /// <summary>
        /// Error PopUpAlert
        /// </summary>
        /// <param name="stralertmsg"></param>
        private void ShowErrPopupInformation(string stralertmsg)
        {
            popUpWindow.ShowOnPageLoad = true;
            popUpWindow.HeaderText = IntakeConstants.ERROR_HEADER;
            ((ASPxLabel)popUpWindow.FindControl("lblMessage")).Text = stralertmsg;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnOk_Click(object sender, EventArgs e)
        {
            base.NavigateNext();
        }

        protected void AddCommentPopUpPOADetail_WindowCallback(object source, DevExpress.Web.ASPxPopupControl.PopupWindowCallbackArgs e)
        {
            CaseRemarkDetails m_CaseRemarkDetails = new CaseRemarkDetails();
            m_CaseRemarkDetails.RemarkTitle =  lblTitleValue.Text;
            m_CaseRemarkDetails.RemarkDate = System.DateTime.Now;
            m_CaseRemarkDetails.RemarkNotes = MemRemark.Text;
            m_CaseRemarkDetails.PageName = "W_AENPA_PROG_OF_ASST";// Convert.ToString(lblRemarkPageNameValue.Value);
            m_CaseRemarkDetails.AlertIndicator = false;
            m_CaseRemarkDetails.ApplicationId = Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key);
            m_CaseRemarkDetails.WorkerId = WorkerSessionContext.Instance.LoggedInWorkerDetails.WorkerId;
            m_CaseRemarkDetails.AlertType = string.Empty;

            m_CaseRemarkDetails.WorkerId = m_CaseRemarkDetails.WorkerId;
            IntakeContext.Instance.CaseRemarkDetails = m_CaseRemarkDetails;

            _validate = true;
            _isRefreshRecord = false;
        }
    }
}