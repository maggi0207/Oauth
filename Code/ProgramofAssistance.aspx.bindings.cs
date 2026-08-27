///////////////////////////////////////////////////////////////////////////////////////////////////////
//
// File:      PrimaryPersonAssignment.aspx.bindings.cs
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
using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Technical;
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
using Dhss.Framework.Extensions;
using System.Web.UI.WebControls;
using System.Linq;
using Dhss.Assist.WorkerWeb.Web.CaseloadManagement;
using Dhss.Framework.Security;
using Dhss.Assist.WorkerWeb.Web.CaseloadManagement.CaseloadManagementReport;
using Dhss.Framework;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{
    public partial class ProgramofAssistance
    {
        /// <summary>
        /// Intake_ProgramofAssistanceMetadata
        /// </summary>
        protected class Intake_ProgramofAssistanceMetadata
        {
            /// <summary>
            /// Gets or Sets the ProgramFilingDate.
            /// </summary>
            /// <value>The ProgramFilingDate.</value>
            [Required]
            public DateTime ProgramFilingDate { get; set; }

            /// <summary>
            /// Gets or Sets the LastVerificationDate.
            /// </summary>
            /// <value>The LastVerificationDate.</value>
            [Required]
            public DateTime LastVerificationDate { get; set; }

            /// <summary>
            /// Gets or Sets the LastVerificationDate.
            /// </summary>
            /// <value>The LastVerificationDate.</value>   
            [NotRequired]
            public DateTime LastVerificationDateNotReq { get; set; }

            /// <summary>
            /// Gets or Sets the CRDPCode.
            /// </summary>
            /// <value>The CRDPCode.</value>
            [Required]
            [LookupTable("YESNO", "CODE-TXT", "DESC-TXT", typeof(ReferenceTableLookupContext))]
            public string CRDPCode { get; set; }

            /// <summary>
            /// Gets or Sets the ProtectedFilingDate.
            /// </summary>
            /// <value>The ProtectedFilingDate.</value>

            [LookupTable("YESNO", "CODE-TXT", "DESC-TXT", typeof(ReferenceTableLookupContext))]
            public string CRDP { get; set; }

            /// <summary>
            /// Gets or Sets the ProtectedFilingDate.
            /// </summary>
            /// <value>The ProtectedFilingDate.</value> 
            [Required]
            public DateTime ProtectedFilingDate { get; set; }

            /// <summary>
            /// Gets or Set the FSIdentityCode.
            /// </summary>
            /// <value>The FSIdentityCode.</value>
            [Required]
            [LookupTable("AERSPE", "RESPONSE-CD", "RESPONSE-DESC", typeof(ReferenceTableLookupContext))]
            public string FSIdentityCode { get; set; }

            /// <summary>
            /// Gets or Sets the FSIdentityVerificationCode.
            /// </summary>
            /// <value>The FSIdentityVerificationCode.</value>
            [Required]
            [LookupTable("AEIDVC", "VER-CD", "DESC-TXT", typeof(ReferenceTableLookupContext))]
            public string FSIdentityVerificationCode { get; set; }

            /// <summary>
            /// Gets or Sets the RetroMACode.
            /// </summary>
            /// <value>The RetroMACode.</value>

            [LookupTable("AERTMA", "DATA-TXT", "DESC-TXT", typeof(ReferenceTableLookupContext))]
            public string RetroMACode { get; set; }

            /// <summary>SWTSPI
            /// Gets or Sets the RetroMASwitch.
            /// </summary>
            /// <value>The RetroMASwitch.</value>

            [LookupTable("SWTSPI", "INIT-CD", "PROG-BGN-DT", typeof(ReferenceTableLookupContext))]
            public string RetroMASwitch { get; set; }



            /// <summary>
            /// Gets or Sets the IsYesNoCodeBit.
            /// </summary>
            /// <value>The IsYesNoCodeBit.</value>
            [LookupTable("SWYSNO", "CODE-TXT", "DESC-TXT", typeof(ReferenceTableLookupContext))]
            public bool IsYesNoCodeBit { get; set; }

            /// <summary>
            /// Gets or Sets the RequesterNumber.
            /// </summary>
            /// <value>The RequesterNumber.</value>
            [Required]
            [LookupTable(typeof(PersonNameWithAppEntityId))]
            public string RequesterNumber { get; set; }

            /// <summary>
            /// Gets or Sets the HouseHoldDetailsIndc.
            /// </summary>
            /// <value>The HouseHoldDetailsIndc.</value>
            [Required]
            public string HouseHoldDetailsIndc { get; set; }
        }

        /// <summary>
        /// Intake_ProgramofAssistanceMetadataNotRequired
        /// </summary>
        protected class Intake_ProgramofAssistanceMetadataNotRequired
        {
            /// <summary>
            /// Gets or Sets the ProgramFilingDate.
            /// </summary>
            /// <value>The ProgramFilingDate.</value>
            [NotRequired]
            public DateTime ProgramFilingDate { get; set; }

            /// <summary>
            /// Gets or Sets the FSIdentityVerificationCode.
            /// </summary>
            /// <value>The FSIdentityVerificationCode.</value>
            [NotRequired]
            [LookupTable("AEIDVC", "VER-CD", "DESC-TXT", typeof(ReferenceTableLookupContext))]
            public string FSIdentityVerificationCode { get; set; }

            /// <summary>
            /// Gets or Sets the RequesterNumber.
            /// </summary>
            /// <value>The RequesterNumber.</value>
            [NotRequired]
            [LookupTable(typeof(PersonNameWithAppEntityId))]
            public string RequesterNumber { get; set; }

            /// <summary>
            /// Gets or Sets the HouseHoldDetailsIndc.
            /// </summary>
            /// <value>The HouseHoldDetailsIndc.</value>
            [NotRequired]
            public string HouseHoldDetailsIndc { get; set; }

            /// <summary>
            /// Gets or Sets the ProtectedFilingDate.
            /// </summary>
            /// <value>The ProtectedFilingDate.</value> 
            [NotRequired]
            public DateTime ProtectedFilingDate { get; set; }

            /// <summary>
            /// Gets or Set the FSIdentityCode.
            /// </summary>
            /// <value>The FSIdentityCode.</value>
            [NotRequired]
            [LookupTable("AERSPE", "RESPONSE-CD", "RESPONSE-DESC", typeof(ReferenceTableLookupContext))]
            public string FSIdentityCode { get; set; }
        }

        /// <summary>
        /// Bind Entities.
        /// </summary>
        public override void BindEntities()
        {

            fvTechnical_ProgramDetail.FindControl("cbCashRequester").Bind<Intake_ProgramofAssistanceMetadata>(x => x.RequesterNumber);
            fvTechnical_ProgramDetail.FindControl("dtCashFilingDate").Bind<Intake_ProgramofAssistanceMetadata>(x => x.ProgramFilingDate);
            if (IntakeContext.Instance.CAProgramCode || _programCode == "QM")
                fvTechnical_ProgramDetail.FindControl("dtCashLastVerificationDate").Bind<Intake_ProgramofAssistanceMetadata>(x => x.LastVerificationDate);
            else
                fvTechnical_ProgramDetail.FindControl("dtCashLastVerificationDate").Bind<Intake_ProgramofAssistanceMetadata>(x => x.LastVerificationDateNotReq);
            lstChosenIndividuals.Bind<Intake_ProgramofAssistanceMetadata>(x => x.HouseHoldDetailsIndc);

            if (!(_request))
            {
                fvTechnical_ProgramDetail.FindControl("dtCashLastVerificationDate").Bind<Intake_ProgramofAssistanceMetadata>(x => x.LastVerificationDateNotReq);
                fvTechnical_ProgramDetail.FindControl("cbCashRequester").Bind<Intake_ProgramofAssistanceMetadataNotRequired>(x => x.RequesterNumber);
                fvTechnical_ProgramDetail.FindControl("dtCashFilingDate").Bind<Intake_ProgramofAssistanceMetadataNotRequired>(x => x.ProgramFilingDate);
                lstChosenIndividuals.Bind<Intake_ProgramofAssistanceMetadataNotRequired>(x => x.HouseHoldDetailsIndc);
            }
            if (!(_request) && _programCode == "DC")
            {
                fvTechnical_DisabledChildren.FindControl("cbDisabledCRDP").Bind<Intake_ProgramofAssistanceMetadata>(x => x.CRDP);
                fvTechnical_DisabledChildren.FindControl("cbDisabledRetroMA").Bind<Intake_ProgramofAssistanceMetadata>(x => x.RetroMACode);

            }
            if (!(_request) && _programCode == "FS")
            {
                fvTechnical_FoodBenefits.FindControl("dtProtectedFilingDate").Bind<Intake_ProgramofAssistanceMetadataNotRequired>(x => x.ProtectedFilingDate);
                fvTechnical_FoodBenefits.FindControl("cbFBIdentity").Bind<Intake_ProgramofAssistanceMetadataNotRequired>(x => x.FSIdentityCode);
                fvTechnical_FoodBenefits.FindControl("cbSeperateFBUnabletoPrepareMeals").Bind<Intake_ProgramofAssistanceMetadata>(x => x.IsYesNoCodeBit);
                fvTechnical_FoodBenefits.FindControl("cbFSIdentityVerificationCode").Bind<Intake_ProgramofAssistanceMetadataNotRequired>(x => x.FSIdentityVerificationCode);
            }
            if (!(_request) && _programCode == "MA")
            {
                fvTechnical_MedicalAssistance.FindControl("cbMedicalCRDP").Bind<Intake_ProgramofAssistanceMetadata>(x => x.CRDP);
                fvTechnical_MedicalAssistance.FindControl("cbMedicalRetroMA").Bind<Intake_ProgramofAssistanceMetadata>(x => x.RetroMACode);

            }
            if (!(_request) && _programCode == "QM")
            {
                fvTechnical_QMB.FindControl("cbQMBProgramCRDP").Bind<Intake_ProgramofAssistanceMetadata>(x => x.CRDP);
                fvTechnical_QMB.FindControl("cbQMBProgramRetroMA").Bind<Intake_ProgramofAssistanceMetadata>(x => x.RetroMACode);
            }

        }

        /// <summary>
        /// Binds entities of Disabled Children
        /// </summary>
        private void BindDisabledChildrenEntities()
        {
            DateTime? bgnDate = Convert.ToDateTime(ReferenceTableHelper.GetReferenceTableValue("SWTSPI", "INIT-CD", "PROG-BGN-DT", "R2"));
            DateTime? endDate = Convert.ToDateTime(ReferenceTableHelper.GetReferenceTableValue("SWTSPI", "INIT-CD", "PROG-END-DT", "R2"));

            fvTechnical_DisabledChildren.FindControl("cbDisabledCRDP").Bind<Intake_ProgramofAssistanceMetadata>(x => x.CRDP);
            fvTechnical_DisabledChildren.FindControl("cbDisabledRetroMA").Bind<Intake_ProgramofAssistanceMetadata>(x => x.RetroMACode);
            if (SystemDateTime.Now >= bgnDate && (endDate == null || SystemDateTime.Now <= endDate))
            {
                (fvTechnical_DisabledChildren.FindControl("cbDisabledRetroMA") as ASPxComboBox).Enabled = true;
            }
            else
            {
                (fvTechnical_DisabledChildren.FindControl("cbDisabledRetroMA") as ASPxComboBox).Enabled = false;
            }
        }

        /// <summary>
        /// Binds entities of Food Benefits
        /// </summary>
        private void BindFoodBenefitsEntities()
        {
            fvTechnical_FoodBenefits.FindControl("dtProtectedFilingDate").Bind<Intake_ProgramofAssistanceMetadata>(x => x.ProtectedFilingDate);
            fvTechnical_FoodBenefits.FindControl("cbFBIdentity").Bind<Intake_ProgramofAssistanceMetadata>(x => x.FSIdentityCode);
            fvTechnical_FoodBenefits.FindControl("cbSeperateFBUnabletoPrepareMeals").Bind<Intake_ProgramofAssistanceMetadata>(x => x.IsYesNoCodeBit);
            ASPxComboBox cbFSIdentityVerificationCode = fvTechnical_FoodBenefits.FindControl("cbFSIdentityVerificationCode") as ASPxComboBox;
            if (cbFSIdentityVerificationCode != null && cbFSIdentityVerificationCode.Value != null && Convert.ToString(cbFSIdentityVerificationCode.Value).Trim() == "?")
                cbFSIdentityVerificationCode.Value = String.Empty;
        }

        /// <summary>
        /// Binds entities of Medical Assistance
        /// </summary>
        private void BindMedicalAssistanceEntities()
        {
            fvTechnical_MedicalAssistance.FindControl("cbMedicalCRDP").Bind<Intake_ProgramofAssistanceMetadata>(x => x.CRDP);
            fvTechnical_MedicalAssistance.FindControl("cbMedicalRetroMA").Bind<Intake_ProgramofAssistanceMetadata>(x => x.RetroMACode);
        }

        /// <summary>
        /// Binds entities of // Qualified Member Beneficiary
        /// </summary>
        private void BindQMBEntities()
        {
            DateTime? bgnDate = Convert.ToDateTime(ReferenceTableHelper.GetReferenceTableValue("SWTSPI", "INIT-CD", "PROG-BGN-DT", "R2"));
            DateTime? endDate = Convert.ToDateTime(ReferenceTableHelper.GetReferenceTableValue("SWTSPI", "INIT-CD", "PROG-END-DT", "R2"));

            fvTechnical_QMB.FindControl("cbQMBProgramCRDP").Bind<Intake_ProgramofAssistanceMetadata>(x => x.CRDP);
            fvTechnical_QMB.FindControl("cbQMBProgramRetroMA").Bind<Intake_ProgramofAssistanceMetadata>(x => x.RetroMACode);
            fvTechnical_ProgramDetail.FindControl("dtCashLastVerificationDate").Bind<Intake_ProgramofAssistanceMetadata>(x => x.LastVerificationDate);
            if (SystemDateTime.Now >= bgnDate && (endDate == null || SystemDateTime.Now <= endDate))
            {
                (fvTechnical_QMB.FindControl("cbQMBProgramRetroMA") as ASPxComboBox).Enabled = true;
            }
            else
            {
                (fvTechnical_QMB.FindControl("cbQMBProgramRetroMA") as ASPxComboBox).Enabled = false;
            }

        }

        /// <summary>
        /// if cbFBIdentity is Yes , FSIdentityVerificationCode is Required Otherwise Not Required
        /// </summary>
        private void BindFBIdentityConditionally(String response)
        {
            if (response == "Y")
                fvTechnical_FoodBenefits.FindControl("cbFSIdentityVerificationCode").Bind<Intake_ProgramofAssistanceMetadata>(x => x.FSIdentityVerificationCode);
            else
                fvTechnical_FoodBenefits.FindControl("cbFSIdentityVerificationCode").Bind<Intake_ProgramofAssistanceMetadataNotRequired>(x => x.FSIdentityVerificationCode);
        }

        /// <summary>
        /// Conditional Validation: If verification date is greater than 30 days from the filing date, then DelayBefenifitsQuest field is required
        /// </summary>
        private void DelayBefenifitsQuestMandatoryConditionally()
        {
            if (_programCode == IntakeConstants.PROGRAM_FOOD_STAMP)
            {
                ASPxDateEdit dtCashFilingDate = (ASPxDateEdit)fvTechnical_ProgramDetail.FindControl("dtCashFilingDate");
                ASPxDateEdit dtCashLastVerificationDate = (ASPxDateEdit)fvTechnical_ProgramDetail.FindControl("dtCashLastVerificationDate");
                bool isDiffGreaterThan30days = TechnicalBusinessLogic.IsDifferenceOfFilingAndVerifDates(dtCashFilingDate.Text, dtCashLastVerificationDate.Text);
                if (isDiffGreaterThan30days)
                {
                    fvTechnical_FoodBenefits.FindControl("cbDelayBefenifitsQuest").Bind<CommonDataValidation>(x => x.YesNoCodeBitRequired);
                }
                else
                {
                    fvTechnical_FoodBenefits.FindControl("cbDelayBefenifitsQuest").Bind<CommonDataValidation>(x => x.IsYesNoCodeBit);
                }
            }
        }

        /// <summary>
        /// Save data on the page.
        /// </summary>
        public override void SaveData()
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.SaveData - Start");

            //CR 242269 - POA request removal Short term fix
            //check for individual removal
            List<int> listBoxValues = new List<int>();
            foreach (var values in lstChosenIndividuals.Items)
            {
                listBoxValues.Add(Convert.ToInt32((values as ListEditItemBase).Value));
            }
            var caseMode = IntakeContext.Instance.CaseMode;
            var caseStatus = IntakeContext.Instance.CaseStatus;
            // If case not in intake pending and the individuals are removed from the program request, prompt the user to enter a comment before saving.
            if (!(caseMode == "I" && caseStatus == "P")
                && TechnicalSessionContext.Instance.IndividualsRequested.Except(listBoxValues).Count() > 0
                && IntakeContext.Instance.CaseRemarkDetails == null)
            {
                AddCommentPopUpPOADetail.ShowOnPageLoad = true;
                lblTitleValue.Text = "POA Request Removed";
                lblRemarkPageNameValue.Text = "Program Of Assistance";
                lblCurrentUserID.Text = SystemPrincipal.Current.Identity.Name;
                dtRemarkDate.Date = System.DateTime.Now;
                _validate = false;
                _isRefreshRecord = false;
                return;
            }

            using (TransactionScope scope = new TransactionScope())
            {
                if (fvTechnical_ProgramDetail.Enabled)
                {
                    ValidateFilingDate();
                    ValidateVerficationDate();
                    if (_validate)
                        ValidateRetroMAMonths();
                    if (_validate)
                    {
                        if (!SaveProgramDetails())
                        {
                            ShowErrPopupAlert("‘Unknown’ Race/Ethnicity is not valid for other programs. Please update.");
                            SetPageComplete(false);
                            _validate = false;
                            _isRefreshRecord = false;
                            return;
                        }
                        if (IntakeContext.Instance.CaseMode != "R" && IsNewIndivRequested())
                        {
                            var programDetailRecord = new Technical_ProgramDetail();
                            programDetailRecord.ProgramDetailID = Convert.ToInt32(fvTechnical_ProgramDetail.DataKey["ProgramDetailID"]);
                            programDetailRecord.ProgramCode = _programCode;
                            TechnicalSessionContext.Instance.IsShowAGReviewPopUp = ServicesApplicationHub.IntakeTechnical.IsCaseToRunInReviewMode(programDetailRecord, Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key), false);


                        }

                    }
                    else
                        _isRefreshRecord = false;
                }
                else
                {
                    SetProgramDetailsContextComplete();
                    SetPageComplete();
                }
            }
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.SaveData - End");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsNewIndivRequested()
        {
            bool newIndivRequested = false;
            if (_newIndivRequested != null)
            {
                var result = _newIndivRequested.Except(TechnicalSessionContext.Instance.IndividualsRequested).ToList();
                if (result.Count() > 0)
                    newIndivRequested = true;
            }
            return newIndivRequested;
        }


        /// <summary>
        /// Saves Program Details
        /// </summary>
        protected bool SaveProgramDetails()
        {
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.SaveProgramDetails - Start");

            bool oldState = CurrentWorkflowPage.Completed;

            if (IsProgDetailsModified() || !WorkflowSession.Instance.CurrentFrame.CurrentEntity.Completed)
            {
                if (!UpdateProgramDetailService(WorkflowSession.Instance.CurrentFrame.CurrentEntity.Completed)) return false;
                List<int> listBoxValues = new List<int>();
                foreach (var values in lstChosenIndividuals.Items)
                {
                    listBoxValues.Add(Convert.ToInt32((values as ListEditItemBase).Value));
                }
                if (IntakeContext.Instance.CaseRemarkDetails != null && TechnicalSessionContext.Instance.IndividualsRequested.Except(listBoxValues).Count() > 0)
                {
                    var m_CaseRemarkDetails = IntakeContext.Instance.CaseRemarkDetails;
                    var caseRemarkId = ServicesApplicationHub.CaseloadContextClient.SaveCaseRemark(m_CaseRemarkDetails);
                    if (caseRemarkId != 0)
                    {
                        IntakeContext.Instance.CaseRemarkDetails = null;
                        CaseloadManagementSessionContext.Instance.CaseRemarkList = null;
                    }
                }

                hfIsPageModified.Value = "N"; //Resetting page status after Save.
                SetPageComplete();

                //Reset Eligibility workflow
                WorkflowScheduling.ResetElgibilityWorkFlow(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
            }
            else
            {
                CleanUpSessionVariables();
                _isRefreshRecord = false;
            }
            ProcessFlowOperationsContext.SetProgramCodeSwitchesForProcessFlow(_applicationId, false);

            if (CurrentWorkflowPage.Context.Value.IsContextComplete())
            {
                SetPreviousPageComplete(true);
                SetPageComplete(IntakeConstants.PROGRAM_OF_ASSISTANCE_AE, true, true);
                ProcessWorkItem();
            }
            ServicesTracingHub.TraceWriter.WriteLine("ProgramofAssistance.SaveProgramDetails - End");
            return true;
        }

        /// <summary>
        /// Program details along with selected indivdiuals are updated
        /// </summary>
        /// <param name="isCurrentEntityCompleted"></param>
        private bool UpdateProgramDetailService(bool isCurrentEntityCompleted)
        {
            var programDetail = new Technical_ProgramDetail();
            programDetail.BeginDate = (fvTechnical_ProgramDetail.FindControl("ASPxDateEdit1") as ASPxDateEdit).Date;
            programDetail.LastVerificationDate = (fvTechnical_ProgramDetail.FindControl("dtCashLastVerificationDate") as ASPxDateEdit).Date;
            programDetail.ProgramFilingDate = (fvTechnical_ProgramDetail.FindControl("dtCashFilingDate") as ASPxDateEdit).Date;
            programDetail.RequesterNumber = Convert.ToInt32((fvTechnical_ProgramDetail.FindControl("cbCashRequester") as ASPxComboBox).Value);
            programDetail.ProgramDetailID = Convert.ToInt32(fvTechnical_ProgramDetail.DataKey["ProgramDetailID"]);
            programDetail.ProgramCode = _programCode;

            CheckProgramCode(programDetail);

            List<int> listBoxValues = new List<int>();
            foreach (var values in lstChosenIndividuals.Items)
            {
                listBoxValues.Add(Convert.ToInt32((values as ListEditItemBase).Value));
            }
            _newIndivRequested = listBoxValues;
            bool autoCaseSwitch = Convert.ToBoolean(IntakeContext.Instance.AutoCaseCreationIndicator);
            if (IntakeContext.Instance.MAProgramCode && !IntakeContext.Instance.CAProgramCode && !IntakeContext.Instance.CCProgramCode && !IntakeContext.Instance.FBProgramCode && !IntakeContext.Instance.QMProgramCode && !IntakeContext.Instance.DCProgramCode)
                autoCaseSwitch = false;


            return ServicesApplicationHub.IntakeTechnical.ProgramOfAssistanceDetails(programDetail, listBoxValues, isCurrentEntityCompleted, hfIsPageModified.Value, Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key), autoCaseSwitch);
        }

        private void CheckProgramCode(Technical_ProgramDetail programDetail)
        {
            switch (_programCode)
            {
                case "CC":
                    programDetail.ChildCareProgram = new List<Technical_ChildCareProgram>();
                    var childCareProgramDetail = UpdateChildCareProgramDetails();
                    programDetail.ChildCareProgram.Add(childCareProgramDetail);
                    break;
                case "DC":
                    programDetail.DisabledChildrenProgram = new List<Technical_DisabledChildrenProgram>();
                    var disabledChildrenProgramDetail = UpdateDisabledChildrenProgramDetails();
                    programDetail.DisabledChildrenProgram.Add(disabledChildrenProgramDetail);
                    CaseSummaryInfo.ClearCaseSummaryCache();
                    var eligibilitySummaryCount_dc = ServicesApplicationHub.Intake.GetEligibilityDCSummaryCount(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                    var currentdisabledRetroMAflag = ((fvTechnical_DisabledChildren.FindControl("hfCurrentDisabledRetroMAflag") as HiddenField).Value);
                    if ((disabledChildrenProgramDetail.RetroMACode == "1" || disabledChildrenProgramDetail.RetroMACode == "2" || disabledChildrenProgramDetail.RetroMACode == "3") && (eligibilitySummaryCount_dc == 0) && (currentdisabledRetroMAflag != disabledChildrenProgramDetail.RetroMACode))
                    {
                        var programofAssistanceSummary = new ProgramofAssistanceSummary();
                        programofAssistanceSummary.IncompleteMAPages();

                    }
                    break;
                case "FS":
                    programDetail.FoodBenefitsProgram = new List<Technical_FoodBenefitsProgram>();
                    var foodBenefitsProgramDetail = UpdateFoodBenefitsProgramDetails();
                    programDetail.FoodBenefitsProgram.Add(foodBenefitsProgramDetail);
                    break;
                case "MA":
                    programDetail.MedicalAssistanceProgram = new List<Techincal_MedicalAssistanceProgram>();
                    var medicalAssistanceProgramDetail = UpdateMedicalAssistanceProgramDetails();
                    programDetail.MedicalAssistanceProgram.Add(medicalAssistanceProgramDetail);
                    CaseSummaryInfo.ClearCaseSummaryCache();
                    var eligibilitySummaryCount = ServicesApplicationHub.Intake.GetEligibilitySummaryCount(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                    var currentRetroMAflag = ((fvTechnical_MedicalAssistance.FindControl("hfCurrentRetroMAflag") as HiddenField).Value);
                    if ((medicalAssistanceProgramDetail.RetroMACode == "1" || medicalAssistanceProgramDetail.RetroMACode == "2" || medicalAssistanceProgramDetail.RetroMACode == "3") && (eligibilitySummaryCount == 0) && (currentRetroMAflag != medicalAssistanceProgramDetail.RetroMACode))
                    {
                        var programofAssistanceSummary = new ProgramofAssistanceSummary();
                        programofAssistanceSummary.IncompleteMAPages();

                    }
                    break;
                case "QM":
                    programDetail.QualifiedMemberBeneficiaryProgram = new List<Technical_QualifiedMemberBeneficiaryProgram>();
                    var qualifiedMemberBeneficiaryProgramDetail = UpdateQualifiedMemberBeneficiaryProgramDetails();
                    programDetail.QualifiedMemberBeneficiaryProgram.Add(qualifiedMemberBeneficiaryProgramDetail);
                    CaseSummaryInfo.ClearCaseSummaryCache();
                    var eligibilityQMBSummaryCount = ServicesApplicationHub.Intake.GetEligibilityQMBSummaryCount(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                    var currentQMBRetroMAflag = ((fvTechnical_QMB.FindControl("hfCurrentQMBRetroMAflag") as HiddenField).Value);
                    if ((qualifiedMemberBeneficiaryProgramDetail.RetroMACode == "1" || qualifiedMemberBeneficiaryProgramDetail.RetroMACode == "2" || qualifiedMemberBeneficiaryProgramDetail.RetroMACode == "3") && (eligibilityQMBSummaryCount == 0) && (currentQMBRetroMAflag != qualifiedMemberBeneficiaryProgramDetail.RetroMACode))
                    {
                        var programofAssistanceSummary = new ProgramofAssistanceSummary();
                        programofAssistanceSummary.IncompleteMAPages();

                    }
                    break;
            }
        }

        private Technical_ChildCareProgram UpdateChildCareProgramDetails()
        {
            var childCare = new Technical_ChildCareProgram();

            return childCare;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Technical_QualifiedMemberBeneficiaryProgram UpdateQualifiedMemberBeneficiaryProgramDetails()
        {
            var qualifiedMemberBeneficiary = new Technical_QualifiedMemberBeneficiaryProgram();

            qualifiedMemberBeneficiary.CRDPCode = (fvTechnical_QMB.FindControl("cbQMBProgramCRDP") as ASPxComboBox).Value != null ? (fvTechnical_QMB.FindControl("cbQMBProgramCRDP") as ASPxComboBox).Value.AsString() : string.Empty;
            qualifiedMemberBeneficiary.RetroMACode = (fvTechnical_QMB.FindControl("cbQMBProgramRetroMA") as ASPxComboBox).Value != null ? (fvTechnical_QMB.FindControl("cbQMBProgramRetroMA") as ASPxComboBox).Value.AsString() : string.Empty;
            qualifiedMemberBeneficiary.ApplicationEntityID = Convert.ToInt32((fvTechnical_ProgramDetail.FindControl("cbCashRequester") as ASPxComboBox).Value);
            return qualifiedMemberBeneficiary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Techincal_MedicalAssistanceProgram UpdateMedicalAssistanceProgramDetails()
        {
            var medicalAssistance = new Techincal_MedicalAssistanceProgram();

            medicalAssistance.CRDPCode = (fvTechnical_MedicalAssistance.FindControl("cbMedicalCRDP") as ASPxComboBox).Value != null ? (fvTechnical_MedicalAssistance.FindControl("cbMedicalCRDP") as ASPxComboBox).Value.AsString() : string.Empty;
            medicalAssistance.RetroMACode = (fvTechnical_MedicalAssistance.FindControl("cbMedicalRetroMA") as ASPxComboBox).Value != null ? (fvTechnical_MedicalAssistance.FindControl("cbMedicalRetroMA") as ASPxComboBox).Value.AsString() : string.Empty;
            medicalAssistance.ApplicationEntityID = Convert.ToInt32((fvTechnical_ProgramDetail.FindControl("cbCashRequester") as ASPxComboBox).Value);
            return medicalAssistance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Technical_FoodBenefitsProgram UpdateFoodBenefitsProgramDetails()
        {
            var foodBenefits = new Technical_FoodBenefitsProgram();

            foodBenefits.ProtectedFilingDate = (fvTechnical_FoodBenefits.FindControl("dtProtectedFilingDate") as ASPxDateEdit).Date;
            foodBenefits.CallBackDate = (fvTechnical_FoodBenefits.FindControl("dtCallBackDate") as ASPxDateEdit).Date;
            foodBenefits.FSIdentityCode = (fvTechnical_FoodBenefits.FindControl("cbFBIdentity") as ASPxComboBox).Value != null ? (fvTechnical_FoodBenefits.FindControl("cbFBIdentity") as ASPxComboBox).Value.AsString() : string.Empty;
            foodBenefits.FSIdentityVerificationCode = (fvTechnical_FoodBenefits.FindControl("cbFSIdentityVerificationCode") as ASPxComboBox).Value != null ? (fvTechnical_FoodBenefits.FindControl("cbFSIdentityVerificationCode") as ASPxComboBox).Value.AsString() : string.Empty;
            if ((fvTechnical_FoodBenefits.FindControl("cbSeperateFBUnabletoPrepareMeals") as ASPxComboBox).Value == null)
            {
                foodBenefits.UnabletoPrepareMealsIndicator = null;
            }
            else
            {
                foodBenefits.UnabletoPrepareMealsIndicator = Convert.ToBoolean((fvTechnical_FoodBenefits.FindControl("cbSeperateFBUnabletoPrepareMeals") as ASPxComboBox).Value.AsString());
            }
            if ((fvTechnical_FoodBenefits.FindControl("cbDelayBefenifitsQuest") as ASPxComboBox).Value == null)
            {
                foodBenefits.DSSDelayReasonIndicator = null;
            }
            else
            {
                foodBenefits.DSSDelayReasonIndicator = Convert.ToBoolean((fvTechnical_FoodBenefits.FindControl("cbDelayBefenifitsQuest") as ASPxComboBox).Value.AsString());
            }
            foodBenefits.ApplicationEntityID = Convert.ToInt32((fvTechnical_ProgramDetail.FindControl("cbCashRequester") as ASPxComboBox).Value);
            return foodBenefits;
        }

        private Technical_DisabledChildrenProgram UpdateDisabledChildrenProgramDetails()
        {
            var disabledChildren = new Technical_DisabledChildrenProgram();
            disabledChildren.CRDPCode = (fvTechnical_DisabledChildren.FindControl("cbDisabledCRDP") as ASPxComboBox).Value != null ? (fvTechnical_DisabledChildren.FindControl("cbDisabledCRDP") as ASPxComboBox).Value.AsString() : string.Empty;
            disabledChildren.RetroMACode = (fvTechnical_DisabledChildren.FindControl("cbDisabledRetroMA") as ASPxComboBox).Value != null ? (fvTechnical_DisabledChildren.FindControl("cbDisabledRetroMA") as ASPxComboBox).Value.AsString() : string.Empty;
            disabledChildren.ApplicationEntityID = Convert.ToInt32((fvTechnical_ProgramDetail.FindControl("cbCashRequester") as ASPxComboBox).Value);
            return disabledChildren;
        }

        /// <summary>
        /// Sets all records to completed if the record has syncState 3
        /// </summary>
        protected void SetProgramDetailsContextComplete()
        {
            if ((!TechnicalContextOperations.IsCaseRenewalOrReactivate() && TechnicalContextOperations.IsProgramOfAssistanceContextComplete())
                || (TechnicalContextOperations.IsCaseRenewalOrReactivate() && IsSummaryPagesComplete()))
            {
                SetPreviousPageComplete();
                SetPageComplete(IntakeConstants.PROGRAM_OF_ASSISTANCE_AE, true, true);
            }
        }
    }
}

