///////////////////////////////////////////////////////////////////////////////////////////////////////
//
// File:      TechnicalQuestions.aspx.cs
//
// Created On: Friday, March 1, 2013  3:40:55 PM
// Created By: Suresh.Padarthi
//
// This file may contain sensitive and/or confidential information and may not be
// distributed without written permission of Delaware Department of Health and 
// Social Services.
//
// #      Type        User                    Date        Comment                                      
// ------ ----------- ----------------------- ----------- -------------------------------------------- 
// 7094	add	        Suresh.Padarthi        3/1/2013         Added Technical pages
///////////////////////////////////////////////////////////////////////////////////////////////////////

using DevExpress.Web.ASPxEditors;
using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Extensions;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Services;
using Dhss.Framework.DataAnnotations;
using Dhss.Framework.Extensions;
using Dhss.Framework.Web.UI.Workflow;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Transactions;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{
    public partial class TechnicalQuestions
    {
        /// <summary>
        /// TechnicalQuestionsMetadata
        /// </summary>
        protected class TechnicalQuestionsMetadata
        {
            /// <summary>
            /// Gets or Sets the TechnicalQuest.
            /// </summary>
            /// <value>The TechnicalQuest.</value>
            [LookupTable("AERSPE", "RESPONSE-CD", "RESPONSE-DESC", typeof (ReferenceTableLookupContext))]
            [Required]
            public string TechnicalQuest { get; set; }
        }

        protected class TechnicalQuestionsNotRequiredMetadata
        {
            /// <summary>
            /// Gets or Sets the TechnicalQuest.
            /// </summary>
            [LookupTable("AERSPE", "RESPONSE-CD", "RESPONSE-DESC", typeof(ReferenceTableLookupContext))]
            [NotRequired]
            public string TechnicalQuest { get; set; }
        }

        /// <summary>
        /// DataAnnotation binding, while applying validation for fields.
        /// </summary>
        public override void BindEntities()
        {
            if (IntakeContext.Instance.CCProgramCode && !IntakeContext.Instance.FBProgramCode && !IntakeContext.Instance.CAProgramCode && !IntakeContext.Instance.MAProgramCode)
            {
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdPregnant").Bind<TechnicalQuestionsNotRequiredMetadata>(b => b.TechnicalQuest);
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbDoesAnyoneInTheHouseholdReceiveS").Bind<TechnicalQuestionsNotRequiredMetadata>(b => b.TechnicalQuest);
            }
            else
            {
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdPregnant").Bind<TechnicalQuestionsMetadata>(b => b.TechnicalQuest);
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbDoesAnyoneInTheHouseholdReceiveS").Bind<TechnicalQuestionsMetadata>(b => b.TechnicalQuest);
            }
            if (IntakeContext.Instance.CAProgramCode || IntakeContext.Instance.MAProgramCode)
            {
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdNoLongerA").Bind<TechnicalQuestionsMetadata>(b => b.TechnicalQuest);
            }
            else
            {
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdNoLongerA").Bind<TechnicalQuestionsNotRequiredMetadata>(b => b.TechnicalQuest);
            }
            if (IntakeContext.Instance.MAProgramCode)
            {
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdLessThan1").Bind<TechnicalQuestionsMetadata>(b => b.TechnicalQuest);                
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdApplyingF").Bind<TechnicalQuestionsMetadata>(b => b.TechnicalQuest);
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbDoesAnyoneInYourHouseholdWhoIsAp").Bind<TechnicalQuestionsMetadata>(b => b.TechnicalQuest);
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdApplyingF1").Bind<TechnicalQuestionsMetadata>(b => b.TechnicalQuest);
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbHasAnyoneInYourHouseholdBeenRefe").Bind<TechnicalQuestionsMetadata>(b => b.TechnicalQuest);
            }
            else
            {
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdLessThan1").Bind<TechnicalQuestionsNotRequiredMetadata>(b => b.TechnicalQuest);                
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdApplyingF").Bind<TechnicalQuestionsNotRequiredMetadata>(b => b.TechnicalQuest);
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbDoesAnyoneInYourHouseholdWhoIsAp").Bind<TechnicalQuestionsNotRequiredMetadata>(b => b.TechnicalQuest);
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdApplyingF1").Bind<TechnicalQuestionsNotRequiredMetadata>(b => b.TechnicalQuest);
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbHasAnyoneInYourHouseholdBeenRefe").Bind<TechnicalQuestionsNotRequiredMetadata>(b => b.TechnicalQuest);
            }

            if (TechnicalSessionContext.Instance.IsFemaleExists)
            {
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdPregnant").Bind<TechnicalQuestionsMetadata>(b => b.TechnicalQuest);
            }
            else
            {
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdPregnant").Bind<TechnicalQuestionsNotRequiredMetadata>(b => b.TechnicalQuest);
            }
        }

        /// <summary>
        /// Save Button click, saves the data on the screen
        /// </summary>
        public override void SaveData()
        {
            if (hdIsPageChange.Value == IntakeConstants.YES_INDC)
            {
                using (var scope = new TransactionScope())
                {
                    fvdsTechnical_HouseholdGeneralInfo.UpdateItem(false);
                    //WWSyncPoints.Technical4ServiceCallToSync(DAEXQS02);
                    //Reset Eligibility workflow
                    WorkflowScheduling.ResetElgibilityWorkFlow(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                    IsTechnicalQuesYes();
                    hdIsPageChange.Value = null;
                }
            }
            SetPageComplete();
        }

        /// <summary>
        /// Schedule the Page If Technical Question is Yes Otherwise Unschedule.
        /// </summary>
        private void IsTechnicalQuesYes()
        {
            VerifyPregnancyQuesResp();
            VerifyNewBornQuesResponse();
            VerifyDisabilityQuesResp();
            VerifyProtectedSsiQuesResp();
            VerifyHcbsQuesResp();
            VerifySpousaQuesResp();
            VerifyCrdpQuesResp();
            VerifyBccQuesResp();
        }

        /// <summary>
        /// If Response is Yes , schedule the page in left menu 
        /// </summary>
        private void VerifyPregnancyQuesResp()
        {
            if (
                TechnicalCommon.IsResponseYes(
                    Convert.ToString(
                        fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdPregnant")
                            .As<ASPxComboBox>()
                            .Value)))
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.PREGNANCY_SUMMARY_AE).Visible = true;
                if (
                    fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdPregnant")
                        .As<ASPxComboBox>()
                        .Enabled)
                {
                    SetPageComplete(IntakeConstants.PREGNANCY_SUMMARY_AE, false);
                }
            }
            else
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.PREGNANCY_SUMMARY_AE).Visible = false;
            }
        }

        /// <summary>
        /// If Response is Yes , schedule the page in left menu.
        /// </summary>
        private void VerifyNewBornQuesResponse()
        {
            if (
                TechnicalCommon.IsResponseYes(
                    Convert.ToString(
                        fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdLessThan1")
                            .As<ASPxComboBox>()
                            .Value)) && IntakeContext.Instance.MAProgramCode)
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.NEW_BORN_SUMMARY_AE).Visible = true;
                if (
                    fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdLessThan1")
                        .As<ASPxComboBox>()
                        .Enabled)
                    SetPageComplete(IntakeConstants.NEW_BORN_SUMMARY_AE, false);
            }
            else
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.NEW_BORN_SUMMARY_AE).Visible = false;
            }
        }

        /// <summary>
        /// If Response is Yes , schedule the page in left menu 
        /// </summary>
        private void VerifyDisabilityQuesResp()
        {
            if (
                TechnicalCommon.IsResponseYes(
                    Convert.ToString(
                        fvdsTechnical_HouseholdGeneralInfo.FindControl("cbDoesAnyoneInTheHouseholdReceiveS")
                            .As<ASPxComboBox>()
                            .Value)))
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.DISABILITY_SUMMARY_AE).Visible = true;
                if (
                    fvdsTechnical_HouseholdGeneralInfo.FindControl("cbDoesAnyoneInTheHouseholdReceiveS")
                        .As<ASPxComboBox>()
                        .Enabled)
                {
                    SetPageComplete(IntakeConstants.DISABILITY_SUMMARY_AE, false);
                }
            }
            else
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.DISABILITY_SUMMARY_AE).Visible = false;
            }
        }

        /// <summary>
        /// If Response is Yes , schedule the page in left menu 
        /// </summary>
        private void VerifyProtectedSsiQuesResp()
        {
            bool isPageSchedule = ApplicationEntryDataServiceLinqDataSource.IsScheduledByCaseMode();

            if (isPageSchedule && 
                TechnicalCommon.IsResponseYes(
                    Convert.ToString(
                        fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdNoLongerA")
                            .As<ASPxComboBox>()
                            .Value)))
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.PROTECTED_SSI_SUMMARY_AE).Visible = true;
                if (
                    fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdNoLongerA")
                        .As<ASPxComboBox>()
                        .Enabled)
                {
                    SetPageComplete(IntakeConstants.PROTECTED_SSI_SUMMARY_AE, false);
                }
            }
            else
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.PROTECTED_SSI_SUMMARY_AE).Visible = false;
            }
        }

        /// <summary>
        /// If Response is Yes , schedule the page in left menu 
        /// </summary>
        private void VerifyHcbsQuesResp()
        {
            bool isPageSchedule = ApplicationEntryDataServiceLinqDataSource.IsScheduledByCaseMode();

            if (isPageSchedule &&
                TechnicalCommon.IsResponseYes(
                    Convert.ToString(
                        fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdApplyingF")
                            .As<ASPxComboBox>()
                            .Value)))
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.HOME_COMMU_BASED_SER_SUMMARY_AE).Visible = true;
                if (
                    fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdApplyingF")
                        .As<ASPxComboBox>()
                        .Enabled)
                {
                    SetPageComplete(IntakeConstants.HOME_COMMU_BASED_SER_SUMMARY_AE, false);
                }
            }
            else
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.HOME_COMMU_BASED_SER_SUMMARY_AE).Visible = false;
            }
        }

        /// <summary>
        /// If Response is Yes , schedule the page in left menu 
        /// </summary>
        private void VerifySpousaQuesResp()
        {
            if (
                TechnicalCommon.IsResponseYes(
                    Convert.ToString(
                        fvdsTechnical_HouseholdGeneralInfo.FindControl("cbDoesAnyoneInYourHouseholdWhoIsAp")
                            .As<ASPxComboBox>()
                            .Value)))
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.SPOUSAL_IMP_SUMMARY_AE).Visible = true;
                if (
                    fvdsTechnical_HouseholdGeneralInfo.FindControl("cbDoesAnyoneInYourHouseholdWhoIsAp")
                        .As<ASPxComboBox>()
                        .Enabled)
                {
                    SetPageComplete(IntakeConstants.SPOUSAL_IMP_SUMMARY_AE, false);
                }
            }
            else
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.SPOUSAL_IMP_SUMMARY_AE).Visible = false;
            }
        }

        /// <summary>
        /// If Response is Yes , schedule the page in left menu 
        /// </summary>
        private void VerifyCrdpQuesResp()
        {
            if (
                TechnicalCommon.IsResponseYes(
                    Convert.ToString(
                        fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdApplyingF1")
                            .As<ASPxComboBox>()
                            .Value)))
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.CRDP_INFO_SUMMARY_AE).Visible = true;
                if (
                    fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdApplyingF1")
                        .As<ASPxComboBox>()
                        .Enabled)
                {
                    SetPageComplete(IntakeConstants.CRDP_INFO_SUMMARY_AE, false);
                }
            }
            else
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.CRDP_INFO_SUMMARY_AE).Visible = false;
            }
        }

        /// <summary>
        /// If Response is Yes , schedule the page in left menu 
        /// </summary>
        private void VerifyBccQuesResp()
        {
            bool isPageSchedule = ApplicationEntryDataServiceLinqDataSource.IsScheduledByCaseMode();

            if (isPageSchedule &&            
                TechnicalCommon.IsResponseYes(
                    Convert.ToString(
                        fvdsTechnical_HouseholdGeneralInfo.FindControl("cbHasAnyoneInYourHouseholdBeenRefe")
                            .As<ASPxComboBox>()
                            .Value)))
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.BREASTAND_CERVICAL_CANCER_INFO_SUMMARY_AE).Visible = true;
                if (
                    fvdsTechnical_HouseholdGeneralInfo.FindControl("cbHasAnyoneInYourHouseholdBeenRefe")
                        .As<ASPxComboBox>()
                        .Enabled)
                {
                    SetPageComplete(IntakeConstants.BREASTAND_CERVICAL_CANCER_INFO_SUMMARY_AE, false);
                }
            }
            else
            {
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.BREASTAND_CERVICAL_CANCER_INFO_SUMMARY_AE).Visible = false;
            }
        }

        /// <summary>
        /// Navigates the previous page.
        /// </summary>
        public override void NavigatePrevious()
        {
            base.NavigatePrevious(n => n.Visible && n.Completed && !n.DetailScreen);
        }

        /// <summary>
        /// Depending on the Technical Question Response Schedules the Page.
        /// </summary>
        /// <param name="applicationId"></param>
        public void TechnicalQuestionResponse(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            Technical_HouseholdGeneralInfo techQuestion =
                context.Technical_HouseholdGeneralInfo.Where(n => n.ApplicationID == applicationId).First();

            if (techQuestion.IsAnyonePregnantIndicator == IntakeConstants.YES_CODE)
            {
                SetPageComplete(IntakeConstants.PREGNANCY_SUMMARY_AE, false);
                if (IsEntityExistsInState(IntakeConstants.PREGNANCY_DETAILS_AE))
                    SetPageComplete(IntakeConstants.PREGNANCY_DETAILS_AE, false, true);
            }
            if (techQuestion.ReceiveDisablityPaymentIndicator == IntakeConstants.YES_CODE)
            {
                SetPageComplete(IntakeConstants.DISABILITY_SUMMARY_AE, false);
                if (IsEntityExistsInState(IntakeConstants.DISABILITY_AE))
                    SetPageComplete(IntakeConstants.DISABILITY_AE, false, true);
            }
            if (techQuestion.HasHCBSWaiverIndicator == IntakeConstants.YES_CODE)
            {
                SetPageComplete(IntakeConstants.HOME_COMMU_BASED_SER_SUMMARY_AE, false);
                if (IsEntityExistsInState(IntakeConstants.HOME_COMMU_BASED_SER_AE))
                    SetPageComplete(IntakeConstants.HOME_COMMU_BASED_SER_AE, false, true);
            }
            TechQuestionsCheck(techQuestion);
        }
        /// <summary>
        /// Depending on the Technical Question Response Schedules the Page.
        /// </summary>
        /// <param name="techQuestion"></param>
        private void TechQuestionsCheck(Technical_HouseholdGeneralInfo techQuestion)
        {
            if (techQuestion.HasLTCwithSpouseinCommunity == IntakeConstants.YES_CODE)
            {
                SetPageComplete(IntakeConstants.SPOUSAL_IMP_SUMMARY_AE, false);
                if (IsEntityExistsInState(IntakeConstants.SPOUSAL_IMP_AE))
                    SetPageComplete(IntakeConstants.SPOUSAL_IMP_AE, false, true);
            }
            if (techQuestion.HadSSIRecipientIndicator == IntakeConstants.YES_CODE)
            {
                SetPageComplete(IntakeConstants.PROTECTED_SSI_SUMMARY_AE, false);
                if (IsEntityExistsInState(IntakeConstants.PROTECTED_SSIAE))
                    SetPageComplete(IntakeConstants.PROTECTED_SSIAE, false, true);
            }
            if (techQuestion.Haslessthan13monthschildIndicator == IntakeConstants.YES_CODE)
            {
                SetPageComplete(IntakeConstants.NEW_BORN_SUMMARY_AE, false);
                if (IsEntityExistsInState(IntakeConstants.NEWBORNS_AE))
                    SetPageComplete(IntakeConstants.NEWBORNS_AE, false, true);
            }
            if (techQuestion.HasChronicRenalDiseaseProgramParticipantIndicator == IntakeConstants.YES_CODE)
            {
                SetPageComplete(IntakeConstants.CRDP_INFO_SUMMARY_AE, false);
                if (IsEntityExistsInState(IntakeConstants.CRDP_INFO_AE))
                    SetPageComplete(IntakeConstants.CRDP_INFO_AE, false, true);
            }
            if (techQuestion.IsReferredByDPHIndicator == IntakeConstants.YES_CODE)
            {
                SetPageComplete(IntakeConstants.BREASTAND_CERVICAL_CANCER_INFO_SUMMARY_AE, false);
                if (IsEntityExistsInState(IntakeConstants.BREASTAND_CERVICAL_CANCER_INFO_AE))
                    SetPageComplete(IntakeConstants.BREASTAND_CERVICAL_CANCER_INFO_AE, false, true);
            }
        }
    }
}

