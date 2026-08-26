using DevExpress.Data.WcfLinq.Helpers;
using DevExpress.Web.ASPxEditors;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Extensions;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Services;
using Dhss.Assist.WorkerWeb.Web.Intake.CommonMetadata;
using Dhss.Assist.WorkerWeb.Web.Services.Application.CaseloadManagement;
using Dhss.Framework.DataAnnotations;
using Dhss.Framework.Security;
using Dhss.Framework.Web.State;
using Dhss.Framework.Web.UI.Workflow;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Transactions;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{

    public partial class TechnicalCommunityEngagementDetailsMetaData
    {
        [LookupTable("AECEPR", "VERIF-CD", "DESC-TXT", typeof(ReferenceTableLookupContext))]
        [MaxLength(60)]
        public string RelationName { get; set; }

        [LookupTable("AEBRDD", "VERIF-CD", "DESC-TXT", typeof(ReferenceTableLookupContext))]
        [MaxLength(60)]
        public string VerifyBy { get; set; }

        [LookupTable("AECECL", "VERIF-CD", "DESC-TXT", typeof(ReferenceTableLookupContext))]
        [MaxLength(60)]
        public string LivingStatus { get; set; }
        [LookupTable("AECEGP", "VERIF-CD", "DESC-TXT", typeof(ReferenceTableLookupContext))]
        [MaxLength(60)]
        public string CarePayment { get; set; }

        [LookupTable("AECESD", "VERIF-CD", "DESC-TXT", typeof(ReferenceTableLookupContext))]
        [MaxLength(60)]
        public string Justification { get; set; }

        [LookupTable("AECECP", "VERIF-CD", "DESC-TXT", typeof(ReferenceTableLookupContext))]
        [MaxLength(60)]
        public string ProvideCare { get; set; }
        [LookupTable("AECECD", "VERIF-CD", "DESC-TXT", typeof(ReferenceTableLookupContext))]
        [MaxLength(60)]
        public string Determined { get; set; }

        [LookupTable("AEBFDS", "BLIND_CONDITION_CD", "DESC-TXT", typeof(ReferenceTableLookupContext))]
        [MaxLength(60)]
        public string ConditionStatus { get; set; }

        [LookupTable(typeof(PersonNameWithNotListedOption))]
        public int CareTakerPersonID { get; set; }

    }
    [Workflow]
    [ExcludeFromCodeCoverage]
    public partial class CommunityEngagementDetails : Infrastructure.Workflow.WorkflowPage<Technical_CommunityEngagement>
    {
        private bool _isChangeMade;
        private bool _isbackToSummaryOrPrevious;

        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>View
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Master.FooterSectionConfigure(FooterBodyConfiguration.AddnoteSavePreviousNext);
            if (!IsPostBack)
            {
                if (!TechnicalSessionContext.Instance.IsTechnicalQuestionsAdded) //Avoiding subsequent calls to improve performance.
                {
                    TechnicalSessionContext.Instance.IsTechnicalQuestionsAdded = true;
                }
            }
            fvTechnical_CommunityEngagementHardshipWaiver.DataBound += FvTechnical_CommunityEngagementHardshipWaiver_DataBound;

            var mpContentPlaceHolder = Master.ViewBodyActionBar;
            if (mpContentPlaceHolder != null)
            {
                var btnPageSaveData = (ASPxButton)mpContentPlaceHolder.FindControl("btnPageSave");
                if (btnPageSaveData != null)
                {
                    btnPageSaveData.Click += RefreshAnchorObject;
                    //btnPageSaveData.ClientSideEvents.Click = "function(s,e) { setTimeout(function() { if (window.focusFirstInvalidEditor) window.focusFirstInvalidEditor(); }, 100); }";
                }
            }
            if (Session["CE_ShowHardshipWaiverPopup"] != null)
            {
                Session.Remove("CE_ShowHardshipWaiverPopup");
                if (!IsPostBack)
                {
                    lblHWWorkerNameValue.Text = SystemPrincipal.Current.Identity.Name;
                    dteHWDate.Date = DateTime.Now;
                    memHWNotes.Text = string.Empty;
                    popupHardshipWaiverPendingApproval.ShowOnPageLoad = true;
                }
            }
            //dateCorrectionalReleasedDate.MinDate = DateTime.Now.AddYears(-1);
            //dateCorrectionalReleasedDate.MaxDate = DateTime.Now;
        }

        private void RefreshAnchorObject(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles DsTechnical_CommunityEngagementDetails_Selecting event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DsTechnical_CommunityEngagementDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetCommunityEngagementDetails();
        }
        /// <summary>
        /// GetCommunityEngagementDetails
        /// </summary>
        /// <returns></returns>
        public Technical_CommunityEngagement GetCommunityEngagementDetails()
        {
            if (TechnicalSessionContext.Instance.CommunityEngagementSummaryID == 0)
            {
                CreateNew();
            }
            var technicalContext = ServicesDataHub.Technical;
            int communityEngagementSummaryID = TechnicalSessionContext.Instance.CommunityEngagementSummaryID;
            return technicalContext.Technical_CommunityEngagement.Where(n => n.CommunityEngagementSummaryID == communityEngagementSummaryID).FirstOrDefault();
        }

        /// <summary>
        /// CreateNew
        /// </summary>
        public void CreateNew()
        {
            int applicationId = Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key);
            TechnicalContextOperations.EnsureCommunityEngagementRecordsForApplication(applicationId);
            var summary = TechnicalContextOperations.GetAllActiveRecordsCommunityEngagementSummary(applicationId).FirstOrDefault();
            if (summary != null)
            {
                TechnicalSessionContext.Instance.CommunityEngagementSummaryID = summary.CommunityEngagementSummaryID;
            }
        }
        /// <summary>
        /// FvdsTechnical_CommunityEngagement_DataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvdsTechnical_CommunityEngagement_DataBound(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// BtnBackToSummary_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnBackToSummary_Click(object sender, EventArgs e)
        {
            _isbackToSummaryOrPrevious = true;
            if (fvTechnical_CommunityEngagement.Enabled)
                fvTechnical_CommunityEngagement.UpdateItem(false);
            if (fvTechnical_CommunityEngagementMedicalDetails.Enabled)
                fvTechnical_CommunityEngagementMedicalDetails.UpdateItem(false);
            if (fvTechnical_CommunityEngagementHardshipWaiver.Enabled)
                fvTechnical_CommunityEngagementHardshipWaiver.UpdateItem(false);

            if (_isChangeMade)
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
        /// Redirects to the Community Engagement Summary page.
        /// </summary>
        private void NavigateToSummary()
        {
            NavigatePrevious(n => n.Name == IntakeConstants.COMMUNITYENGAGEMENT_SUMMARY_AE);
        }

        /// <summary>
        /// Shows the error confirmation popup.
        /// </summary>
        private void ShowPopupInfo(string message)
        {
            dxPopupInfo.ShowOnPageLoad = true;
            var lblErrMessage = (ASPxLabel)dxPopupInfo.FindControl("lblErrMessage1");
            lblErrMessage.Text = message;
        }

        /// <summary>
        /// Shows the error confirmation popup.
        /// </summary>
        //private void ShowPopupMessage(string message)
        //{
        //    dxPopupInfo.ShowOnPageLoad = true;
        //    var lblErrMessage = (ASPxLabel)dxPopupInfo.FindControl("lblSaveConfirmationMessage1");
        //    lblErrMessage.Text = message;
        //}

        private void RefreshRecordDetails(Technical_CommunityEngagement ce)
        {
            if (ce == null) return;

            var lblUpdatedDate = fvTechnical_CommunityEngagement.FindControl("lblRecordUpdatedDate1") as ASPxLabel;
            if (lblUpdatedDate != null)
            {
                lblUpdatedDate.Text = ce.UpdatedDateTime == null ? string.Empty : Convert.ToDateTime(ce.UpdatedDateTime).ToString("MM/dd/yyyy");
            }

            var lblSquence = fvTechnical_CommunityEngagement.FindControl("lblSequenceNumber1") as ASPxLabel;
            if (lblSquence != null)
            {
                lblSquence.Text = Convert.ToString(ce.SequenceNumber);
            }
            var lblHistorySequence = fvTechnical_CommunityEngagement.FindControl("lblHistorySequenceNumber1") as ASPxLabel;
            if (lblHistorySequence != null)
            {
                lblHistorySequence.Text = Convert.ToString(ce.HistorySequenceNumber);
            }
        }

        /// <summary>
        /// BtnPopUpYes_Click
        /// </summary>
        protected void BtnPopUpYes_Click(object sender, EventArgs e)
        {
            dxPopupInfo.ShowOnPageLoad = false;
            if (TechnicalSessionContext.Instance.IsPreviousAction)
            {
                TechnicalSessionContext.Instance.IsPreviousAction = false;
                base.NavigatePrevious(n => n.Visible && n.Completed && !n.DetailScreen && n.Name != IntakeConstants.COMMUNITYENGAGEMENT_SUMMARY_AE);
            }
            else
            {
                NavigateToSummary();
            }
        }
        /// <summary>
        /// btnCaseComment_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCaseComment_Click(object sender, EventArgs e) { }
        /// <summary>
        /// btnPrevious_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Intake/ApplicationEntry/Technical/CommunityEngagementSummary.aspx");
        }
        /// <summary>
        /// btnNext_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNext_Click(object sender, EventArgs e) { }
        /// <summary>
        /// CbRegularlyTakeCareOfDependent_SelectedIndexChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbRegularlyTakeCareOfDependent_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void CbWhenDetermined_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void CbWhenDisablingMentalDisorder_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void CbWhenPhysicalDisability_SelectedIndexChanged(object sender, EventArgs e) { }

        protected void CbParentOrLegalGuardian_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// FvTechnical_CommunityEngagement_DataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvTechnical_CommunityEngagement_DataBound(object sender, EventArgs e)
        {
            var lblNameValue = fvTechnical_CommunityEngagement.FindControl("lblNameValue") as ASPxLabel;
            if (lblNameValue != null)
            {
                lblNameValue.Text = GetPersonNameForCurrentSummary();
            }
            BindCEEntities();
        }

        /// <summary>
        /// GetPersonNameForCurrentSummary
        /// </summary>
        /// <returns></returns>
        private string GetPersonNameForCurrentSummary()
        {
            var technicalContext = ServicesDataHub.Technical;
            int communityEngagementSummaryID = TechnicalSessionContext.Instance.CommunityEngagementSummaryID;
            var summary = technicalContext.Technical_CommunityEngagementSummary.Where(n => n.CommunityEngagementSummaryID == communityEngagementSummaryID).FirstOrDefault();
            if (summary == null) return string.Empty;

            var person = technicalContext.Technical_Entity.OfType<Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Technical.Technical_Person>()
                .Where(p => p.EntityID == summary.PersonID)
                .Select(p => new { p.FirstName, p.MiddleName, p.LastName, p.SuffixNameCode })
                .ToList()
                .FirstOrDefault();
            if (person == null) return string.Empty;

            return string.Join(" ", new[] { person.FirstName, person.MiddleName, person.LastName, person.SuffixNameCode }
                .Where(part => !string.IsNullOrWhiteSpace(part)));
        }
        /// <summary>
        /// FvTechnical_CommunityEngagement_ItemUpdating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvTechnical_CommunityEngagement_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            if (_isbackToSummaryOrPrevious && TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues))
            {
                _isChangeMade = true;
            }
            e.Cancel = true;
        }


        protected void FvTechnical_CommunityEngagementMedicalDetails_DataBound(object sender, EventArgs e)
        {
            BindCEMedicalDetailsEntities();
        }
        protected void FvTechnical_CommunityEngagementMedicalDetails_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            if (_isbackToSummaryOrPrevious && TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues))
            {
                _isChangeMade = true;
            }
            e.Cancel = true;
        }
        /// <summary>
        /// FvTechnical_CommunityEngagementHardshipWaiver_DataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvTechnical_CommunityEngagementHardshipWaiver_DataBound(object sender, EventArgs e)
        {
            BindHardshipWaiverEntities();
        }
        protected void FvTechnical_CommunityEngagementHardshipWaiver_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            if (_isbackToSummaryOrPrevious && TechnicalContextOperations.IsUpdatedFormview((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues))
            {
                _isChangeMade = true;
            }
            e.Cancel = true;
        }

        /// <summary>
        /// DsTechnical_CommunityEngagementMedicalDetails_Selecting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DsTechnical_CommunityEngagementMedicalDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var technicalContext = ServicesDataHub.Technical;
            int communityEngagementSummaryID = TechnicalSessionContext.Instance.CommunityEngagementSummaryID;
            e.Result = technicalContext.Technical_CommunityEngagementMedicalDetails.Where(n => n.CommunityEngagementSummaryID == communityEngagementSummaryID).FirstOrDefault();
        }
        protected void DsTechnical_CommunityEngagementHardshipWaiver_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var technicalContext = ServicesDataHub.Technical;
            int communityEngagementSummaryID = TechnicalSessionContext.Instance.CommunityEngagementSummaryID;
            e.Result = technicalContext.Technical_CommunityEngagementHardshipWaiver.Where(n => n.CommunityEngagementSummaryID == communityEngagementSummaryID).FirstOrDefault();
        }

        protected void CbSeriousMedicalconditionStatus_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void CbSubstanceUseDisorderStatus_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void CbDisabledBySSAStatus_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void CbDisablingMentalDisorderStatus_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void CbPhysicalDisabilityStatus_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void CbHospitalizedJustification_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void CbWho_SelectedIndexChanged(object sender, EventArgs e) { }
        protected void CbTravelOutOfAreaMedicalJustification_SelectedIndexChanged(object sender, EventArgs e) { }


        public void BindCEEntities()
        {
            fvTechnical_CommunityEngagement.FindControl("cbRegularlyTakeCareOfDependent").Bind<CommonDataValidation>(x => x.YesNoCodeBit);
            fvTechnical_CommunityEngagement.FindControl("cbParentOrLegalGuardian").Bind<CommonDataValidation>(x => x.YesNoCodeBit);
            System.Web.HttpContext.Current.Cache.Remove("ReferenceTable_AECEPR_VERIF-CD_DESC-TXT");

            var cbWho = fvTechnical_CommunityEngagement.FindControl("cbWho") as ASPxComboBox;
            cbWho.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.CareTakerPersonID);
            RefreshComboText(cbWho);


            var cbCareTakerRelationship = fvTechnical_CommunityEngagement.FindControl("cbCareTakerRelationship") as ASPxComboBox;
            cbCareTakerRelationship.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.RelationName);
            RefreshComboText(cbCareTakerRelationship);

            var cbLiveWithPersonBeingCaredFor = fvTechnical_CommunityEngagement.FindControl("cbLiveWithPersonBeingCaredFor") as ASPxComboBox;
            cbLiveWithPersonBeingCaredFor.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.LivingStatus);
            RefreshComboText(cbLiveWithPersonBeingCaredFor);

            var cbReceivedProvidingCare = fvTechnical_CommunityEngagement.FindControl("cbReceivedProvidingCare") as ASPxComboBox;
            cbReceivedProvidingCare.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.CarePayment);
            RefreshComboText(cbReceivedProvidingCare);

            fvTechnical_CommunityEngagement.FindControl("cbParticipatingInWorkProgram").Bind<CommonDataValidation>(x => x.YesNoCodeBit);
            fvTechnical_CommunityEngagement.FindControl("cbParticipatingInUnpaidWork").Bind<CommonDataValidation>(x => x.YesNoCodeBit);
            fvTechnical_CommunityEngagement.FindControl("cbCorrectionalInLast12Months").Bind<CommonDataValidation>(x => x.YesNoCodeBit);

            var cbReceivedProvidingCareVerifiedBy = fvTechnical_CommunityEngagement.FindControl("cbReceivedProvidingCareVerifiedBy") as ASPxComboBox;
            cbReceivedProvidingCareVerifiedBy.Bind<TechnicalCommunityEngagementDetailsMetaData>(y => y.VerifyBy);
            RefreshComboText(cbReceivedProvidingCareVerifiedBy);

            var cbCorrectionalInLast12MonthsVerifiedBy = fvTechnical_CommunityEngagement.FindControl("cbCorrectionalInLast12MonthsVerifiedBy") as ASPxComboBox;
            cbCorrectionalInLast12MonthsVerifiedBy.Bind<TechnicalCommunityEngagementDetailsMetaData>(y => y.VerifyBy);
            RefreshComboText(cbCorrectionalInLast12MonthsVerifiedBy);

            var cbWhenCareTakerRelationship = fvTechnical_CommunityEngagement.FindControl("cbWhenCareTakerRelationship") as ASPxComboBox;
            cbWhenCareTakerRelationship.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.ProvideCare);
            RefreshComboText(cbWhenCareTakerRelationship);

            var cbWhenLegalGuardianProvideCare = fvTechnical_CommunityEngagement.FindControl("cbWhenLegalGuardianProvideCare") as ASPxComboBox;
            cbWhenLegalGuardianProvideCare.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.ProvideCare);
            RefreshComboText(cbWhenLegalGuardianProvideCare);
        }

        /// <summary>
        /// BindCEMedicalDetailsEntities
        /// </summary>
        public void BindCEMedicalDetailsEntities()
        {
            var fvMD = fvTechnical_CommunityEngagementMedicalDetails;
            fvMD.FindControl("cbSeriousMedicalCondition").Bind<CommonDataValidation>(x => x.YesNoCodeBit);

            var cbSeriousMedicalConditionVerifiedBy = fvMD.FindControl("cbSeriousMedicalConditionVerifiedBy") as ASPxComboBox;
            cbSeriousMedicalConditionVerifiedBy.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.VerifyBy);
            RefreshComboText(cbSeriousMedicalConditionVerifiedBy);

            var cbSeriousMedicalconditionStatus = fvMD.FindControl("cbSeriousMedicalconditionStatus") as ASPxComboBox;
            cbSeriousMedicalconditionStatus.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.ConditionStatus);
            RefreshComboText(cbSeriousMedicalconditionStatus);

            fvMD.FindControl("cbSubstanceUseDisorder").Bind<CommonDataValidation>(x => x.YesNoCodeBit);

            var cbSubstanceUseDisorderVerifiedBy = fvMD.FindControl("cbSubstanceUseDisorderVerifiedBy") as ASPxComboBox;
            cbSubstanceUseDisorderVerifiedBy.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.VerifyBy);
            RefreshComboText(cbSubstanceUseDisorderVerifiedBy);

            var cbSubstanceUseDisorderStatus = fvMD.FindControl("cbSubstanceUseDisorderStatus") as ASPxComboBox;
            cbSubstanceUseDisorderStatus.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.ConditionStatus);
            RefreshComboText(cbSubstanceUseDisorderStatus);

            fvMD.FindControl("cbDisabledBySSA").Bind<CommonDataValidation>(x => x.YesNoCodeBit);

            var cbDisabledBySSAVerifiedBy = fvMD.FindControl("cbDisabledBySSAVerifiedBy") as ASPxComboBox;
            cbDisabledBySSAVerifiedBy.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.VerifyBy);
            RefreshComboText(cbDisabledBySSAVerifiedBy);

            var cbWhenDetermined = fvMD.FindControl("cbWhenDetermined") as ASPxComboBox;
            cbWhenDetermined.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.Determined);
            RefreshComboText(cbWhenDetermined);

            fvMD.FindControl("cbDisablingMentalDisorder").Bind<CommonDataValidation>(x => x.YesNoCodeBit);

            var cbDisablingMentalDisorderVerifiedBy = fvMD.FindControl("cbDisablingMentalDisorderVerifiedBy") as ASPxComboBox;
            cbDisablingMentalDisorderVerifiedBy.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.VerifyBy);
            RefreshComboText(cbDisablingMentalDisorderVerifiedBy);

            var cbWhenDisablingMentalDisorder = fvMD.FindControl("cbWhenDisablingMentalDisorder") as ASPxComboBox;
            cbWhenDisablingMentalDisorder.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.ConditionStatus);
            RefreshComboText(cbWhenDisablingMentalDisorder);

            fvMD.FindControl("cbPhysicalDisability").Bind<CommonDataValidation>(x => x.YesNoCodeBit);

            var cbPhysicalDisabilityVerifiedBy = fvMD.FindControl("cbPhysicalDisabilityVerifiedBy") as ASPxComboBox;
            cbPhysicalDisabilityVerifiedBy.Bind<TechnicalCommunityEngagementDetailsMetaData>(y => y.VerifyBy);
            RefreshComboText(cbPhysicalDisabilityVerifiedBy);

            var cbWhenPhysicalDisability = fvMD.FindControl("cbWhenPhysicalDisability") as ASPxComboBox;
            cbWhenPhysicalDisability.Bind<TechnicalCommunityEngagementDetailsMetaData>(y => y.ConditionStatus);
            RefreshComboText(cbWhenPhysicalDisability);
        }

        /// <summary>
        /// BindHardshipWaiverEntities
        /// </summary>
        public void BindHardshipWaiverEntities()
        {
            var fvHW = fvTechnical_CommunityEngagementHardshipWaiver;
            fvHW.FindControl("cbHospitalizedSeriousCondition").Bind<CommonDataValidation>(x => x.YesNoCodeBit);
            fvHW.FindControl("cbTravelOutOfAreaMedical").Bind<CommonDataValidation>(x => x.YesNoCodeBit);

            var cbHospitalizedJustification = fvHW.FindControl("cbHospitalizedJustification") as ASPxComboBox;
            cbHospitalizedJustification.Bind<TechnicalCommunityEngagementDetailsMetaData>(y => y.Justification);
            RefreshComboText(cbHospitalizedJustification);

            var cbTravelOutOfAreaMedicalJustification = fvHW.FindControl("cbTravelOutOfAreaMedicalJustification") as ASPxComboBox;
            cbTravelOutOfAreaMedicalJustification.Bind<TechnicalCommunityEngagementDetailsMetaData>(y => y.Justification);
            RefreshComboText(cbTravelOutOfAreaMedicalJustification);

            var cbTravelOutOfAreaMedicalVerifiedBy = fvHW.FindControl("cbTravelOutOfAreaMedicalVerifiedBy") as ASPxComboBox;
            cbTravelOutOfAreaMedicalVerifiedBy.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.VerifyBy);
            RefreshComboText(cbTravelOutOfAreaMedicalVerifiedBy);

            var cbHospitalizedSeriousConditionVerifiedBy = fvHW.FindControl("cbHospitalizedSeriousConditionVerifiedBy") as ASPxComboBox;
            cbHospitalizedSeriousConditionVerifiedBy.Bind<TechnicalCommunityEngagementDetailsMetaData>(x => x.VerifyBy);
            RefreshComboText(cbHospitalizedSeriousConditionVerifiedBy);

            string jobFunctionCode = WorkerSessionContext.Instance.LoggedInWorkerDetails.JobFunction;
            bool canDecideSupervisorApproval = jobFunctionCode == IntakeConstants.CCM_JobFunction_ES
                || jobFunctionCode == IntakeConstants.WORKER_JOBFUNCTIONCODE_MA
                || jobFunctionCode == IntakeConstants.WORKER_JOBFUNCTIONCODE_MF;
            if (!canDecideSupervisorApproval)
            {
                (fvHW.FindControl("chkHospitalizedSupervisorApprovedIndicator") as ASPxCheckBox).Enabled = false;
                (fvHW.FindControl("chkHospitalizedSupervisorRejectedIndicator") as ASPxCheckBox).Enabled = false;
                (fvHW.FindControl("chkTravelOutOfAreaSupervisorApprovedIndicator") as ASPxCheckBox).Enabled = false;
                (fvHW.FindControl("chkTravelOutOfAreaSupervisorRejectedIndicator") as ASPxCheckBox).Enabled = false;
            }
        }

        private readonly List<string> _validationErrors = new List<string>();

        private void RequiredCombo(ASPxComboBox cb, string label, bool applies = true)
        {
            if (!applies) return;
            if (cb == null || cb.Value == null || string.IsNullOrWhiteSpace(cb.Value.ToString()))
            {
                _validationErrors.Add(label);
            }
        }
        private void RequiredCombo(ASPxDateEdit de, string label, bool applies = true)
        {
            if (!applies) return;
            if (de == null || de.Date == DateTime.MinValue)
            {
                _validationErrors.Add(label);
            }
        }

        private static bool ComboTextContains(ASPxComboBox cb, string keyword)
        {
            return cb != null && !string.IsNullOrEmpty(cb.Text)
                && cb.Text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RequireEndOnOrAfterBegin(ASPxDateEdit begin, ASPxDateEdit end, string label)
        {
            if (begin == null || end == null) return;
            if (begin.Date == DateTime.MinValue || end.Date == DateTime.MinValue) return;
            if (end.Date.Date < begin.Date.Date)
            {
                _validationErrors.Add(label);
            }
        }

        private bool ValidatePage()
        {
            _validationErrors.Clear();
            var fv = fvTechnical_CommunityEngagement;
            var fvMD = fvTechnical_CommunityEngagementMedicalDetails;
            var fvHW = fvTechnical_CommunityEngagementHardshipWaiver;

            var ddeBeginDate = fv.FindControl("ddeCEBeginDate") as ASPxDateEdit;
            RequiredCombo(ddeBeginDate, "Begin Date");
            if (ddeBeginDate != null && ddeBeginDate.Date != DateTime.MinValue)
            {
                if (ddeBeginDate.Date.Year <= 1989)
                {
                    _validationErrors.Add("Begin Date year cannot be 1989 or earlier");
                }
                var maxMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(2);
                var enteredMonth = new DateTime(ddeBeginDate.Date.Year, ddeBeginDate.Date.Month, 1);
                if (enteredMonth > maxMonth)
                {
                    _validationErrors.Add("Begin Date cannot be greater than 2 months from the current date");
                }
            }

            var cbCare = fv.FindControl("cbRegularlyTakeCareOfDependent") as ASPxComboBox;
            RequiredCombo(cbCare, "Regularly take care of a dependent");
            if (IsYes(cbCare))
            {
                RequiredCombo(fv.FindControl("cbWho") as ASPxComboBox, "Who");
                var cbParent = fv.FindControl("cbParentOrLegalGuardian") as ASPxComboBox;
                RequiredCombo(cbParent, "Parent or legal guardian");
                if (IsYes(cbParent))
                {
                    var cbWhenLg = fv.FindControl("cbWhenLegalGuardianProvideCare") as ASPxComboBox;
                    RequiredCombo(cbWhenLg, "When did you provide care");
                    RequiredCombo(fv.FindControl("dateStopLegalGuardianProvideCare") as ASPxDateEdit, "When did you stop providing care", ComboTextContains(cbWhenLg, "no longer"));
                }
                else if (cbParent != null && cbParent.Value != null)
                {
                    var cbRel = fv.FindControl("cbCareTakerRelationship") as ASPxComboBox;
                    RequiredCombo(cbRel, "Relationship to the person you care for");
                    bool otherOrNotRelated = ComboTextContains(cbRel, "other relation") || ComboTextContains(cbRel, "not related");
                    if (cbRel != null && cbRel.Value != null && !otherOrNotRelated)
                    {
                        var cbWhenRel = fv.FindControl("cbWhenCareTakerRelationship") as ASPxComboBox;
                        RequiredCombo(cbWhenRel, "When did you provide care");
                        RequiredCombo(fv.FindControl("dateStopProvidingCareDateTime") as ASPxDateEdit, "When did you stop providing care", ComboTextContains(cbWhenRel, "no longer"));
                    }
                    if (otherOrNotRelated)
                    {
                        var cbLive = fv.FindControl("cbLiveWithPersonBeingCaredFor") as ASPxComboBox;
                        RequiredCombo(cbLive, "Live with the person while giving care");
                    }
                }
            }

            var cbCorrectional = fv.FindControl("cbCorrectionalInLast12Months") as ASPxComboBox;
            RequiredCombo(cbCorrectional, "Correctional facility in the last 12 months");
            if (IsYes(cbCorrectional))
            {
                RequiredCombo(fv.FindControl("cbCorrectionalInLast12MonthsVerifiedBy") as ASPxComboBox, "Correctional Verified By");
                var released = fv.FindControl("dateCorrectionalReleasedDate") as ASPxDateEdit;
                RequiredCombo(released, "When were you released");
                if (released != null && released.Date != DateTime.MinValue)
                {
                    var twelveMonthsAgo = DateTime.Today.AddMonths(-12);
                    if (released.Date.Date < twelveMonthsAgo || released.Date.Date > DateTime.Today)
                    {
                        _validationErrors.Add("Release date must be within the past 12 months");
                    }
                }
            }

            RequiredCombo(fv.FindControl("cbParticipatingInWorkProgram") as ASPxComboBox, "Participating in a Work Program");
            RequiredCombo(fv.FindControl("cbParticipatingInUnpaidWork") as ASPxComboBox, "Volunteering or Participating in Unpaid Work");

            ValidateMedicalYesBranch(fvMD, "cbSeriousMedicalCondition", "cbSeriousMedicalConditionVerifiedBy", "cbSeriousMedicalconditionStatus", "dateEndSeriousConditionDate", "Serious medical condition");
            ValidateMedicalYesBranch(fvMD, "cbSubstanceUseDisorder", "cbSubstanceUseDisorderVerifiedBy", "cbSubstanceUseDisorderStatus", "dateEndSubstanceDisorderDate", "Substance use disorder");
            ValidateMedicalYesBranch(fvMD, "cbDisabledBySSA", "cbDisabledBySSAVerifiedBy", "cbWhenDetermined", "dateEndSSADeterminationDate", "Disabled by SSA");
            ValidateMedicalYesBranch(fvMD, "cbDisablingMentalDisorder", "cbDisablingMentalDisorderVerifiedBy", "cbWhenDisablingMentalDisorder", "dateEndDisablingMentalDisorderDate", "Disabling mental disorder");
            ValidateMedicalYesBranch(fvMD, "cbPhysicalDisability", "cbPhysicalDisabilityVerifiedBy", "cbWhenPhysicalDisability", "dateEndPhysicalDisabilityDate", "Physical/Intellectual/developmental disability");

            var cbHospitalized = fvHW.FindControl("cbHospitalizedSeriousCondition") as ASPxComboBox;
            RequiredCombo(cbHospitalized, "Hospitalized for a serious condition");
            if (IsYes(cbHospitalized))
            {
                RequiredCombo(fvHW.FindControl("cbHospitalizedSeriousConditionVerifiedBy") as ASPxComboBox, "Hospitalized Verified By");
                var hospBegin = fvHW.FindControl("ddeHospitalizedBeginDate") as ASPxDateEdit;
                var hospEnd = fvHW.FindControl("ddeHospitalizedEndDate") as ASPxDateEdit;
                RequiredCombo(hospBegin, "Hospitalized Begin Date");
                RequiredCombo(hospEnd, "Hospitalized End Date");
                RequireEndOnOrAfterBegin(hospBegin, hospEnd, "Hospitalized end date must be on or after begin date");
            }

            var cbTravel = fvHW.FindControl("cbTravelOutOfAreaMedical") as ASPxComboBox;
            RequiredCombo(cbTravel, "Traveled out of the area for medical care");
            if (IsYes(cbTravel))
            {
                RequiredCombo(fvHW.FindControl("cbTravelOutOfAreaMedicalVerifiedBy") as ASPxComboBox, "Travel Verified By");
                var travelBegin = fvHW.FindControl("ddeTravelOutOfAreaMedicalBeginDate") as ASPxDateEdit;
                var travelEnd = fvHW.FindControl("ddeTravelOutOfAreaMedicalEndDate") as ASPxDateEdit;
                RequiredCombo(travelBegin, "Travel Begin Date");
                RequiredCombo(travelEnd, "Travel End Date");
                RequireEndOnOrAfterBegin(travelBegin, travelEnd, "Travel end date must be on or after begin date");
            }

            RequireEndOnOrAfterBegin(fvHW.FindControl("ddeDisasterDeclarationBeginDate") as ASPxDateEdit, fvHW.FindControl("ddeDisasterDeclarationEndDate") as ASPxDateEdit, "Disaster end date must be on or after begin date");
            RequireEndOnOrAfterBegin(fvHW.FindControl("ddeUnemploymentLevelBeginDate") as ASPxDateEdit, fvHW.FindControl("ddeUnemploymentLevelEndDate") as ASPxDateEdit, "Unemployment end date must be on or after begin date");

            return _validationErrors.Count == 0;
        }

        private void ValidateMedicalYesBranch(Control fvMD, string yesComboId, string verifiedById, string whenId, string endDateId, string label)
        {
            var cb = fvMD.FindControl(yesComboId) as ASPxComboBox;
            RequiredCombo(cb, label);
            if (!IsYes(cb)) return;
            RequiredCombo(fvMD.FindControl(verifiedById) as ASPxComboBox, label + " Verified By");
            var cbWhen = fvMD.FindControl(whenId) as ASPxComboBox;
            RequiredCombo(cbWhen, "When did you have this condition (" + label + ")");
            RequiredCombo(fvMD.FindControl(endDateId) as ASPxDateEdit, "End date (" + label + ")", ComboTextContains(cbWhen, "no longer"));
        }

        /// <summary>
        /// SaveData
        /// </summary>
        public override void SaveData()
        {
            if (!ValidatePage())
            {
                _pageValidationFailed = true;
                ShowPopupInfo(IntakeResourceManager.MANDATORY_FIELDS_MESSAGE);
                return;
            }

            _pageValidationFailed = false;
            using (var scope = new TransactionScope())
            {
                SaveDataCEMHDeatils();
                scope.Complete();
            }

            _isChangeMade = false;
            fvTechnical_CommunityEngagement.DataBind();
            fvTechnical_CommunityEngagementMedicalDetails.DataBind();
            fvTechnical_CommunityEngagementHardshipWaiver.DataBind();

            var cbWorkProgram = fvTechnical_CommunityEngagement.FindControl("cbParticipatingInWorkProgram") as ASPxComboBox;
            var cbUnpaidWork = fvTechnical_CommunityEngagement.FindControl("cbParticipatingInUnpaidWork") as ASPxComboBox;
            if (IsYes(cbWorkProgram) || IsYes(cbUnpaidWork))
            {
                ScheduleVolunteeringWorkProgramPage();
            }

            SetPageComplete();
            SetSummaryPageComplete();
        }

        /// <summary>
        /// Completes the Community Engagement summary when every person in context is done,
        /// same pattern as Tax Dependency SetSummaryPageComplete.
        /// </summary>
        private void SetSummaryPageComplete()
        {
            if (CurrentWorkflowPage.Context.Value.IsContextComplete())
            {
                SetPreviousPageComplete(true);
            }
        }

        /// <summary>
        /// IsYes
        /// </summary>
        private static bool IsYes(ASPxComboBox cb)
        {
            return cb != null && cb.Value != null && Convert.ToBoolean(cb.Value);
        }

        /// <summary>
        /// Re-resolves a combo's displayed Text against its already-bound Value now that Items are populated,
        /// without calling DataBind() (which would re-trigger the control's own markup Bind() expression
        /// outside the FormView's binding-container context and throw InvalidOperationException).
        /// </summary>
        private static void RefreshComboText(ASPxComboBox cb)
        {
            if (cb == null || cb.Value == null) return;
            var item = cb.Items.FindByValue(cb.Value);
            if (item == null)
            {
                var value = cb.Value.ToString().Trim();
                item = cb.Items.Cast<DevExpress.Web.ASPxEditors.ListEditItem>()
                    .FirstOrDefault(i => string.Equals(i.Value != null ? i.Value.ToString().Trim() : null, value, StringComparison.OrdinalIgnoreCase));
            }
            if (item != null) cb.Text = item.Text;
        }

        /// <summary>
        /// SaveDataCEMHDeatils
        /// </summary>
        public void SaveDataCEMHDeatils()
        {
            var fv = fvTechnical_CommunityEngagement;
            var fvMD = fvTechnical_CommunityEngagementMedicalDetails;
            var fvHW = fvTechnical_CommunityEngagementHardshipWaiver;

            var ddeBeginDate = fv.FindControl("ddeCEBeginDate") as ASPxDateEdit;
            var ddeEndDate = fv.FindControl("ddeEndDate") as ASPxDateEdit;
            var ddeHospitalizedBeginDate = fvHW.FindControl("ddeHospitalizedBeginDate") as ASPxDateEdit;
            var ddeHospitalizedEndDate = fvHW.FindControl("ddeHospitalizedEndDate") as ASPxDateEdit;
            var ddeTravelOutOfAreaMedicalBeginDate = fvHW.FindControl("ddeTravelOutOfAreaMedicalBeginDate") as ASPxDateEdit;
            var ddeTravelOutOfAreaMedicalEndDate = fvHW.FindControl("ddeTravelOutOfAreaMedicalEndDate") as ASPxDateEdit;
            var ddeDisasterDeclarationBeginDate = fvHW.FindControl("ddeDisasterDeclarationBeginDate") as ASPxDateEdit;
            var ddeDisasterDeclarationEndDate = fvHW.FindControl("ddeDisasterDeclarationEndDate") as ASPxDateEdit;
            var ddeUnemploymentLevelBeginDate = fvHW.FindControl("ddeUnemploymentLevelBeginDate") as ASPxDateEdit;
            var ddeUnemploymentLevelEndDate = fvHW.FindControl("ddeUnemploymentLevelEndDate") as ASPxDateEdit;
            var dateIndividualAddedDate = fvMD.FindControl("dateIndividualAddedDate") as ASPxDateEdit;
            var dateStopLegalGuardianProvideCare = fv.FindControl("dateStopLegalGuardianProvideCare") as ASPxDateEdit;
            var dateStopLivingwithPersonDate = fv.FindControl("dateStopLivingwithPersonDate") as ASPxDateEdit;
            var dateStopTakingCareDate = fv.FindControl("dateStopTakingCareDate") as ASPxDateEdit;
            var dateStopLivingWithPersonWhileGivingCareDate = fv.FindControl("dateStopLivingWithPersonWhileGivingCareDate") as ASPxDateEdit;
            var dateStopTakingCarePersonDate = fv.FindControl("dateStopTakingCarePersonDate") as ASPxDateEdit;
            var dateCorrectionalReleasedDate = fv.FindControl("dateCorrectionalReleasedDate") as ASPxDateEdit;
            var dateEndSeriousConditionDate = fvMD.FindControl("dateEndSeriousConditionDate") as ASPxDateEdit;
            var dateEndSubstanceDisorderDate = fvMD.FindControl("dateEndSubstanceDisorderDate") as ASPxDateEdit;
            var dateEndSSADeterminationDate = fvMD.FindControl("dateEndSSADeterminationDate") as ASPxDateEdit;
            var dateEndDisablingMentalDisorderDate = fvMD.FindControl("dateEndDisablingMentalDisorderDate") as ASPxDateEdit;
            var dateEndPhysicalDisabilityDate = fvMD.FindControl("dateEndPhysicalDisabilityDate") as ASPxDateEdit;
            var dateStopProvidingCareDateTime = fv.FindControl("dateStopProvidingCareDateTime") as ASPxDateEdit;

            DateTime? beginDate = (ddeBeginDate.Date == DateTime.MinValue) ? null : (DateTime?)ddeBeginDate.Date;
            DateTime? dateHospitalizedBeginDate = (ddeHospitalizedBeginDate.Date == DateTime.MinValue) ? null : (DateTime?)ddeHospitalizedBeginDate.Date;
            DateTime? dateHospitalizedEndDate = (ddeHospitalizedEndDate.Date == DateTime.MinValue) ? null : (DateTime?)ddeHospitalizedEndDate.Date;
            DateTime? dateTravelOutOfAreaMedicalBeginDate = (ddeTravelOutOfAreaMedicalBeginDate.Date == DateTime.MinValue) ? null : (DateTime?)ddeTravelOutOfAreaMedicalBeginDate.Date;
            DateTime? dateTravelOutOfAreaMedicalEndDate = (ddeTravelOutOfAreaMedicalEndDate.Date == DateTime.MinValue) ? null : (DateTime?)ddeTravelOutOfAreaMedicalEndDate.Date;
            DateTime? dateDisasterDeclarationBeginDate = (ddeDisasterDeclarationBeginDate.Date == DateTime.MinValue) ? null : (DateTime?)ddeDisasterDeclarationBeginDate.Date;
            DateTime? dateDisasterDeclarationEndDate = (ddeDisasterDeclarationEndDate.Date == DateTime.MinValue) ? null : (DateTime?)ddeDisasterDeclarationEndDate.Date;
            DateTime? dateUnemploymentLevelBeginDate = (ddeUnemploymentLevelBeginDate.Date == DateTime.MinValue) ? null : (DateTime?)ddeUnemploymentLevelBeginDate.Date;
            DateTime? dateUnemploymentLevelEndDate = (ddeUnemploymentLevelEndDate.Date == DateTime.MinValue) ? null : (DateTime?)ddeUnemploymentLevelEndDate.Date;
            DateTime? dtIndividualAddedDate = (dateIndividualAddedDate.Date == DateTime.MinValue) ? null : (DateTime?)dateIndividualAddedDate.Date;
            DateTime? dtStopLegalGuardianProvideCare = (dateStopLegalGuardianProvideCare.Date == DateTime.MinValue) ? null : (DateTime?)dateStopLegalGuardianProvideCare.Date;
            DateTime? dtStopLivingwithPersonDate = (dateStopLivingwithPersonDate.Date == DateTime.MinValue) ? null : (DateTime?)dateStopLivingwithPersonDate.Date;
            DateTime? dtStopTakingCareDate = (dateStopTakingCareDate.Date == DateTime.MinValue) ? null : (DateTime?)dateStopTakingCareDate.Date;
            DateTime? dtStopLivingWithPersonWhileGivingCareDate = (dateStopLivingWithPersonWhileGivingCareDate.Date == DateTime.MinValue) ? null : (DateTime?)dateStopLivingWithPersonWhileGivingCareDate.Date;
            DateTime? dtStopTakingCarePersonDate = (dateStopTakingCarePersonDate.Date == DateTime.MinValue) ? null : (DateTime?)dateStopTakingCarePersonDate.Date;
            DateTime? dtCorrectionalReleasedDate = (dateCorrectionalReleasedDate.Date == DateTime.MinValue) ? null : (DateTime?)dateCorrectionalReleasedDate.Date;
            DateTime? dtEndSeriousConditionDate = (dateEndSeriousConditionDate.Date == DateTime.MinValue) ? null : (DateTime?)dateEndSeriousConditionDate.Date;
            DateTime? dtEndSubstanceDisorderDate = (dateEndSubstanceDisorderDate.Date == DateTime.MinValue) ? null : (DateTime?)dateEndSubstanceDisorderDate.Date;
            DateTime? dtEndSSADeterminationDate = (dateEndSSADeterminationDate.Date == DateTime.MinValue) ? null : (DateTime?)dateEndSSADeterminationDate.Date;
            DateTime? dtEndDisablingMentalDisorderDate = (dateEndDisablingMentalDisorderDate.Date == DateTime.MinValue) ? null : (DateTime?)dateEndDisablingMentalDisorderDate.Date;
            DateTime? dtEndPhysicalDisabilityDate = (dateEndPhysicalDisabilityDate.Date == DateTime.MinValue) ? null : (DateTime?)dateEndPhysicalDisabilityDate.Date;
            DateTime? dtStopProvidingCareDateTime = (dateStopProvidingCareDateTime.Date == DateTime.MinValue) ? null : (DateTime?)dateStopProvidingCareDateTime.Date;

            ASPxComboBox cbCareTakerRelationship = fv.FindControl("cbCareTakerRelationship") as ASPxComboBox;
            ASPxComboBox cbWho = fv.FindControl("cbWho") as ASPxComboBox;
            ASPxComboBox cbCorrectionalInLast12MonthsVerifiedBy = fv.FindControl("cbCorrectionalInLast12MonthsVerifiedBy") as ASPxComboBox;
            ASPxComboBox cbParentOrLegalGuardian = fv.FindControl("cbParentOrLegalGuardian") as ASPxComboBox;
            ASPxComboBox cbLiveWithPersonBeingCaredFor = fv.FindControl("cbLiveWithPersonBeingCaredFor") as ASPxComboBox;
            ASPxComboBox cbParticipatingInUnpaidWork = fv.FindControl("cbParticipatingInUnpaidWork") as ASPxComboBox;
            ASPxComboBox cbParticipatingInWorkProgram = fv.FindControl("cbParticipatingInWorkProgram") as ASPxComboBox;
            ASPxComboBox cbRegularlyTakeCareOfDependent = fv.FindControl("cbRegularlyTakeCareOfDependent") as ASPxComboBox;
            ASPxComboBox cbReceivedProvidingCare = fv.FindControl("cbReceivedProvidingCare") as ASPxComboBox;
            ASPxComboBox cbCorrectionalInLast12Months = fv.FindControl("cbCorrectionalInLast12Months") as ASPxComboBox;
            ASPxComboBox cbReceivedProvidingCareVerifiedBy = fv.FindControl("cbReceivedProvidingCareVerifiedBy") as ASPxComboBox;
            ASPxComboBox cbWhenLegalGuardianProvideCare = fv.FindControl("cbWhenLegalGuardianProvideCare") as ASPxComboBox;
            ASPxComboBox cbWhenCareTakerRelationship = fv.FindControl("cbWhenCareTakerRelationship") as ASPxComboBox;


            ASPxComboBox cbSeriousMedicalCondition = fvMD.FindControl("cbSeriousMedicalCondition") as ASPxComboBox;
            ASPxComboBox cbSeriousMedicalConditionVerifiedBy = fvMD.FindControl("cbSeriousMedicalConditionVerifiedBy") as ASPxComboBox;
            ASPxComboBox cbSeriousMedicalconditionStatus = fvMD.FindControl("cbSeriousMedicalconditionStatus") as ASPxComboBox;
            ASPxComboBox cbSubstanceUseDisorderStatus = fvMD.FindControl("cbSubstanceUseDisorderStatus") as ASPxComboBox;
            ASPxComboBox cbSubstanceUseDisorder = fvMD.FindControl("cbSubstanceUseDisorder") as ASPxComboBox;
            ASPxComboBox cbSubstanceUseDisorderVerifiedBy = fvMD.FindControl("cbSubstanceUseDisorderVerifiedBy") as ASPxComboBox;
            ASPxComboBox cbDisabledBySSA = fvMD.FindControl("cbDisabledBySSA") as ASPxComboBox;
            ASPxComboBox cbDisabledBySSAVerifiedBy = fvMD.FindControl("cbDisabledBySSAVerifiedBy") as ASPxComboBox;
            ASPxComboBox cbDisablingMentalDisorder = fvMD.FindControl("cbDisablingMentalDisorder") as ASPxComboBox;
            ASPxComboBox cbDisablingMentalDisorderVerifiedBy = fvMD.FindControl("cbDisablingMentalDisorderVerifiedBy") as ASPxComboBox;
            ASPxComboBox cbPhysicalDisability = fvMD.FindControl("cbPhysicalDisability") as ASPxComboBox;
            ASPxComboBox cbPhysicalDisabilityVerifiedBy = fvMD.FindControl("cbPhysicalDisabilityVerifiedBy") as ASPxComboBox;
            ASPxComboBox cbHospitalizedSeriousCondition = fvHW.FindControl("cbHospitalizedSeriousCondition") as ASPxComboBox;
            ASPxComboBox cbHospitalizedSeriousConditionVerifiedBy = fvHW.FindControl("cbHospitalizedSeriousConditionVerifiedBy") as ASPxComboBox;
            ASPxComboBox cbHospitalizedJustification = fvHW.FindControl("cbHospitalizedJustification") as ASPxComboBox;
            ASPxComboBox cbTravelOutOfAreaMedical = fvHW.FindControl("cbTravelOutOfAreaMedical") as ASPxComboBox;
            ASPxComboBox cbTravelOutOfAreaMedicalVerifiedBy = fvHW.FindControl("cbTravelOutOfAreaMedicalVerifiedBy") as ASPxComboBox;
            ASPxComboBox cbWhenDetermined = fvMD.FindControl("cbWhenDetermined") as ASPxComboBox;
            ASPxComboBox cbWhenDisablingMentalDisorder = fvMD.FindControl("cbWhenDisablingMentalDisorder") as ASPxComboBox;
            ASPxComboBox cbWhenPhysicalDisability = fvMD.FindControl("cbWhenPhysicalDisability") as ASPxComboBox;


            ASPxCheckBox chkTravelOutOfAreaSupervisorApprovedIndicator = fvHW.FindControl("chkTravelOutOfAreaSupervisorApprovedIndicator") as ASPxCheckBox;
            ASPxComboBox cbTravelOutOfAreaMedicalJustification = fvHW.FindControl("cbTravelOutOfAreaMedicalJustification") as ASPxComboBox;
            ASPxCheckBox chkUnemploymentClientRequestsRemovalIndicator = fvHW.FindControl("chkUnemploymentClientRequestsRemovalIndicator") as ASPxCheckBox;
            ASPxCheckBox chkHospitalizedSupervisorApprovedIndicator = fvHW.FindControl("chkHospitalizedSupervisorApprovedIndicator") as ASPxCheckBox;
            ASPxCheckBox chkHospitalizedSupervisorRejectedIndicator = fvHW.FindControl("chkHospitalizedSupervisorRejectedIndicator") as ASPxCheckBox;
            ASPxCheckBox chkTravelOutOfAreaSupervisorRejectedIndicator = fvHW.FindControl("chkTravelOutOfAreaSupervisorRejectedIndicator") as ASPxCheckBox;
            ASPxCheckBox chkDisasterClientRequestsRemovalIndicator = fvHW.FindControl("chkDisasterClientRequestsRemovalIndicator") as ASPxCheckBox;


            if (TechnicalSessionContext.Instance.CommunityEngagementSummaryID == 0)
            {
                CreateNew();
            }
            else if (TechnicalSessionContext.Instance.CommunityEngagementSummaryID > 0)
            {
                var technicalContext = ServicesDataHub.Technical;
                int communityEngagementSummaryID = TechnicalSessionContext.Instance.CommunityEngagementSummaryID;

                var ceSummary = technicalContext.Technical_CommunityEngagementSummary.Where(x => x.CommunityEngagementSummaryID == communityEngagementSummaryID).FirstOrDefault();
                var ce = technicalContext.Technical_CommunityEngagement.Where(x => x.CommunityEngagementSummaryID == communityEngagementSummaryID).FirstOrDefault();
                var ceMD = technicalContext.Technical_CommunityEngagementMedicalDetails.Where(x => x.CommunityEngagementSummaryID == communityEngagementSummaryID).FirstOrDefault();
                var ceHW = technicalContext.Technical_CommunityEngagementHardshipWaiver.Where(x => x.CommunityEngagementSummaryID == communityEngagementSummaryID).FirstOrDefault();

                ce.CommunityEngagementSummaryID = TechnicalSessionContext.Instance.CommunityEngagementSummaryID;
                ce.BeginDate = beginDate;
                ce.UpdatedDateTime = DateTime.Now;
                ce.CareTakerRelationshipCode = cbCareTakerRelationship.Value?.ToString();
                ce.CareTakerPersonID = cbWho.Value != null ? (int?)Convert.ToInt32(cbWho.Value) : null;
                ce.CorrectionalInLast12MonthsVerifiedByCode = cbCorrectionalInLast12MonthsVerifiedBy.Value?.ToString();
                ce.ParentOrLegalGuardianIndicator = IsYes(cbParentOrLegalGuardian);
                ce.LiveWithPersonBeingCaredForCode = cbLiveWithPersonBeingCaredFor.Value?.ToString();
                ce.ParticipatingInUnpaidWorkIndicator = IsYes(cbParticipatingInUnpaidWork);
                ce.ParticipatingInWorkProgramIndicator = IsYes(cbParticipatingInWorkProgram);
                ce.RegularTakecareIndicator = IsYes(cbRegularlyTakeCareOfDependent);
                ce.ReceivedProvidingCareCode = cbReceivedProvidingCare.Value?.ToString();
                ce.CorrectionalInLast12MonthsIndicator = IsYes(cbCorrectionalInLast12Months);
                ce.ReceivedProvidingCareVerifiedByCode = cbReceivedProvidingCareVerifiedBy.Value?.ToString();
                ce.WhenLegalGuardianProvideCareCode = cbWhenLegalGuardianProvideCare.Value?.ToString();
                ce.StopLegalGuardianProvideCareDate = dtStopLegalGuardianProvideCare;
                ce.WhenCareTakerRelationshipCode = cbWhenCareTakerRelationship.Value?.ToString();
                ce.StopLivingwithPersonDate = dtStopLivingwithPersonDate;
                ce.StopTakingCareDate = dtStopTakingCareDate;
                ce.StopLivingWithPersonWhileGivingCareDate = dtStopLivingWithPersonWhileGivingCareDate;
                ce.StopTakingCarePersonDate = dtStopTakingCarePersonDate;
                ce.CorrectionalReleasedDate = dtCorrectionalReleasedDate;
                ce.StopProvidingCareDate = dtStopProvidingCareDateTime;


                if (ceMD == null)
                {
                    ceMD = TechnicalContextOperations.CreateNewCommunityEngagementMedicalDetails(TechnicalSessionContext.Instance.CommunityEngagementSummaryID);
                }
                ceMD.CommunityEngagementSummaryID = TechnicalSessionContext.Instance.CommunityEngagementSummaryID;
                ceMD.SeriousMedicalConditionIndicator = IsYes(cbSeriousMedicalCondition);
                ceMD.SeriousMedicalConditionVerifiedByCode = cbSeriousMedicalConditionVerifiedBy.Value?.ToString();
                ceMD.SeriousMedicalconditionStatusCode = cbSeriousMedicalconditionStatus.Value?.ToString();
                ceMD.SubstanceUseDisorderStatusCode = cbSubstanceUseDisorderStatus.Value?.ToString();
                ceMD.SubstanceUseDisorderIndicator = IsYes(cbSubstanceUseDisorder);
                ceMD.SubstanceUseDisorderVerifiedByCode = cbSubstanceUseDisorderVerifiedBy.Value?.ToString();
                ceMD.DisabledBySSAIndicator = IsYes(cbDisabledBySSA);
                ceMD.DisabledBySSAVerifiedByCode = cbDisabledBySSAVerifiedBy.Value?.ToString();
                ceMD.DisablingMentalDisorderIndicator = IsYes(cbDisablingMentalDisorder);
                ceMD.DisablingMentalDisorderVerifiedByCode = cbDisablingMentalDisorderVerifiedBy.Value?.ToString();
                ceMD.PhysicalDisabilityIndicator = IsYes(cbPhysicalDisability);
                ceMD.PhysicalDisabilityVerifiedByCode = cbPhysicalDisabilityVerifiedBy.Value?.ToString();
                ceMD.IndividualAddedDate = dtIndividualAddedDate;
                ceMD.WhenDeterminedCode = cbWhenDetermined.Value?.ToString();
                ceMD.EndSeriousConditionDate = dtEndSeriousConditionDate;
                ceMD.EndSubstanceDisorderDate = dtEndSubstanceDisorderDate;
                ceMD.EndSSADeterminationDate = dtEndSSADeterminationDate;
                ceMD.WhenDisablingmentalDisorderCode = cbWhenDisablingMentalDisorder.Value?.ToString();
                ceMD.EndDisablingMentalDisorderDate = dtEndDisablingMentalDisorderDate;
                ceMD.WhenPhysicalDisabilityCode = cbWhenPhysicalDisability.Value?.ToString();
                ceMD.EndPhysicalDisabilityDate = dtEndPhysicalDisabilityDate;


                if (ceHW == null)
                {
                    ceHW = TechnicalContextOperations.CreateNewCommunityEngagementHardshipWaiver(TechnicalSessionContext.Instance.CommunityEngagementSummaryID);
                }

                bool wasHospitalizedYes = ceHW.HospitalizedSeriousConditionIndicator == true;
                bool wasTravelYes = ceHW.TravelOutOfAreaMedicalIndicator == true;
                bool newHospitalizedYes = IsYes(cbHospitalizedSeriousCondition);
                bool newTravelYes = IsYes(cbTravelOutOfAreaMedical);
                _showHardshipWaiverPopup = (!wasHospitalizedYes && newHospitalizedYes)
                    || (!wasTravelYes && newTravelYes)
                    || ceHW.HospitalizedBeginDate != dateHospitalizedBeginDate
                    || ceHW.HospitalizedEndDate != dateHospitalizedEndDate
                    || ceHW.TravelOutOfAreaMedicalBeginDate != dateTravelOutOfAreaMedicalBeginDate
                    || ceHW.TravelOutOfAreaMedicalEndDate != dateTravelOutOfAreaMedicalEndDate;

                ceHW.CommunityEngagementSummaryID = TechnicalSessionContext.Instance.CommunityEngagementSummaryID;
                ceHW.HospitalizedSeriousConditionIndicator = IsYes(cbHospitalizedSeriousCondition);
                ceHW.HospitalizedSeriousConditionVerifiedByCode = cbHospitalizedSeriousConditionVerifiedBy.Value?.ToString();
                ceHW.HospitalizedBeginDate = dateHospitalizedBeginDate;
                ceHW.HospitalizedEndDate = dateHospitalizedEndDate;
                ceHW.HospitalizedSupervisorApprovedIndicator = chkHospitalizedSupervisorApprovedIndicator.Checked;
                ceHW.HospitalizedJustificationCode = cbHospitalizedJustification.Value?.ToString();
                ceHW.TravelOutOfAreaMedicalIndicator = IsYes(cbTravelOutOfAreaMedical);
                ceHW.TravelOutOfAreaMedicalVerifiedByCode = cbTravelOutOfAreaMedicalVerifiedBy.Value?.ToString();
                ceHW.TravelOutOfAreaMedicalBeginDate = dateTravelOutOfAreaMedicalBeginDate;
                ceHW.TravelOutOfAreaMedicalEndDate = dateTravelOutOfAreaMedicalEndDate;
                ceHW.TravelOutOfAreaSupervisorApprovedIndicator = chkTravelOutOfAreaSupervisorApprovedIndicator.Checked;
                ceHW.TravelOutOfAreaMedicalJustificationCode = cbTravelOutOfAreaMedicalJustification.Value?.ToString();
                ceHW.DisasterClientRequestsRemovalIndicator = chkDisasterClientRequestsRemovalIndicator.Checked;
                ceHW.DisasterDeclarationBeginDate = dateDisasterDeclarationBeginDate;
                ceHW.DisasterDeclarationEndDate = dateDisasterDeclarationEndDate;
                ceHW.UnemploymentAreaBeginDate = dateUnemploymentLevelBeginDate;
                ceHW.UnemploymentAreaEndDate = dateUnemploymentLevelEndDate;
                ceHW.UnemploymentClientRequestsRemovalIndicator = chkUnemploymentClientRequestsRemovalIndicator.Checked;
                ceHW.HospitalizedSupervisorRejectedIndicator = chkHospitalizedSupervisorRejectedIndicator.Checked;
                ceHW.TravelOutOfAreaSupervisorRejectedIndicator = chkTravelOutOfAreaSupervisorRejectedIndicator.Checked;

                ceSummary.BeginDate = beginDate;
                ceSummary.EndDate = dateHospitalizedEndDate;
                ceSummary.EffectiveDate = beginDate;
                ceSummary.CaretakerIndicator = IsYes(cbRegularlyTakeCareOfDependent);
                ceSummary.WorkProgramIndicator = IsYes(cbParticipatingInWorkProgram);
                ceSummary.UnpaidWorkIndicator = IsYes(cbParticipatingInUnpaidWork);

                technicalContext.UpdateObject(ceSummary);
                technicalContext.UpdateObject(ce);
                technicalContext.UpdateObject(ceMD);
                technicalContext.UpdateObject(ceHW);
                technicalContext.SaveChanges();
                RefreshRecordDetails(ce);

                if (_showHardshipWaiverPopup)
                {
                    lblHWWorkerNameValue.Text = SystemPrincipal.Current.Identity.Name;
                    dteHWDate.Date = DateTime.Now;
                    memHWNotes.Text = string.Empty;
                    //ScriptManager.RegisterStartupScript(this, GetType(), "ShowHardshipWaiverPopup", "popupHardshipWaiverPendingApproval.Show();", true);
                    popupHardshipWaiverPendingApproval.ShowOnPageLoad = true;
                    Session["CE_ShowHardshipWaiverPopup"] = true;
                }
            }
        }

        private bool _showHardshipWaiverPopup;


        /// <summary>
        /// PopupHardshipWaiverPendingApproval_WindowCallback
        /// </summary>
        protected void PopupHardshipWaiverPendingApproval_WindowCallback(object source, DevExpress.Web.ASPxPopupControl.PopupWindowCallbackArgs e)
        {
            if (e.Parameter != "save") return;

            var caseRemarkDetails = new CaseRemarkDetails
            {
                RemarkNotes = memHWNotes.Text.Length > 500 ? memHWNotes.Text.Substring(0, 500) : memHWNotes.Text,
                RemarkTitle = "Hardship Waiver Added/Updated",
                PageName = "Community Engagement",
                WorkerId = WorkerSessionContext.Instance.LoggedInWorkerDetails.WorkerId,
                ApplicationId = Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key),
                CaseNumber = CaseloadManagementSessionContext.Instance.CaseNumber,
                RemarkDate = DateTime.Now.Date
            };
            int caseRemarkId = 0;
            try
            {
                caseRemarkId = ServicesApplicationHub.CaseloadContextClient.SaveCaseRemark(caseRemarkDetails);
                if (caseRemarkId != 0)
                {
                    CaseloadManagementSessionContext.Instance.CaseRemarkList = null;
                    if (NavigateNextPending)
                    {
                        NavigateNextPending = false;
                        ResumeNavigation();
                    }
                }
                else
                {
                    Debug.WriteLine("HW> SaveCaseRemark returned 0 - case comment not saved");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HW> SaveCaseRemark failed: " + ex.Message);
            }

        }
        /// <summary>
        /// Same pattern as Tax Deductions: Yes on Work Program / Unpaid Work
        /// schedules the Volunteering child page as incomplete.
        /// </summary>
        private void ScheduleVolunteeringWorkProgramPage()
        {
            string pageName = IntakeConstants.VOLUNTEERING_WORK_PROGRAM_UNPAID_WORK_SUMMARY_AE;
            SetWorkflowPageVisible(pageName, true);
            SetPageComplete(pageName, false);
        }

        private void SetWorkflowPageVisible(string pageName, bool visible)
        {
            if (TrySetWorkflowPageVisible(WorkflowSession.Instance.CurrentFrame.Workflow.Children, pageName, visible))
            {
                return;
            }
            TrySetWorkflowPageVisible(WorkflowSession.Instance.RootFrame.Workflow.Children, pageName, visible);
        }

        private static bool TrySetWorkflowPageVisible(System.Collections.IEnumerable children, string pageName, bool visible)
        {
            if (children == null)
            {
                return false;
            }
            foreach (var child in children)
            {
                var nameProperty = child.GetType().GetProperty("Name");
                var visibleProperty = child.GetType().GetProperty("Visible");
                var childName = nameProperty == null ? null : nameProperty.GetValue(child, null) as string;
                if (string.Equals(childName, pageName, StringComparison.Ordinal))
                {
                    if (visibleProperty != null)
                    {
                        visibleProperty.SetValue(child, visible, null);
                    }
                    return true;
                }
                var childrenProperty = child.GetType().GetProperty("Children");
                var nested = childrenProperty == null ? null : childrenProperty.GetValue(child, null) as System.Collections.IEnumerable;
                if (TrySetWorkflowPageVisible(nested, pageName, visible))
                {
                    return true;
                }
            }
            return false;
        }

        private bool _pageValidationFailed;

        /// <summary>
        /// Navigates to next active record if it exists in context else the summary page,
        /// same as Tax Dependency Details.
        /// </summary>
        public override void NavigateNext()
        {
            if (_pageValidationFailed) return;

            if (_showHardshipWaiverPopup)
            {
                NavigateNextPending = true;
                return;
            }
            ResumeNavigation();
        }
        private void ResumeNavigation()
        {
            base.NavigateNext();
        }
        private bool NavigateNextPending
        {
            get { return Session["CE_NavigateNextPending"] != null && (bool)Session["CE_NavigateNextPending"]; }
            set
            {
                if (value)
                {
                    Session["CE_NavigateNextPending"] = true;
                }
                else
                {
                    Session.Remove("CE_NavigateNextPending");
                }
            }
        }
        private void CollectInvalidEditors(Control parent, List<string> names)
        {
            foreach (Control c in parent.Controls)
            {
                var editor = c as ASPxEdit;
                if (editor != null && !editor.IsValid)
                {
                    names.Add(editor.ID);
                }
                if (c.HasControls())
                {
                    CollectInvalidEditors(c, names);
                }
            }
        }

        public override void NavigatePrevious()
        {
            _isbackToSummaryOrPrevious = true;
            if (fvTechnical_CommunityEngagement.Enabled)
                fvTechnical_CommunityEngagement.UpdateItem(false);
            if (fvTechnical_CommunityEngagementMedicalDetails.Enabled)
                fvTechnical_CommunityEngagementMedicalDetails.UpdateItem(false);
            if (fvTechnical_CommunityEngagementHardshipWaiver.Enabled)
                fvTechnical_CommunityEngagementHardshipWaiver.UpdateItem(false);

            if (_isChangeMade)
            {
                TechnicalSessionContext.Instance.IsPreviousAction = true;
                ShowPopupInfo(IntakeResourceManager.SAVE_CHAGNES_ALERT);
            }
            else
            {
                base.NavigatePrevious(n => n.Name != IntakeConstants.COMMUNITYENGAGEMENT_SUMMARY_AE && !n.DetailScreen && n.Completed && n.Visible);
            }
        }

        //private bool _showUnsavedChagnesPopup;

        //private string _pedingNavigation
        //{
        //    get { return Session["CE_PendingNavigation"] as string; }
        //    set
        //    {
        //        if (string.IsNullOrEmpty(value))
        //            Session.Remove("CE_PendingNavigation");
        //        else
        //            Session["CE_PendingNavigation"] = value;
        //    }
        //}

    }
}