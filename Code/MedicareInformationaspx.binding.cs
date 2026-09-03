///////////////////////////////////////////////////////////////////////////////////////////////////////
//
// File:      MedicareInformation.aspx.cs
//
// Created On: Friday, February 15, 2013 2:41:06 PM
// Created By: Mohammad.Ali
//
// This file may contain sensitive and/or confidential information and may not be
// distributed without written permission of Delaware Department of Health and 
// Social Services.
//
// #      Type        User                    Date        Comment                                      
// ------ ----------- ----------------------- ----------- -------------------------------------------- 
// 5210   add	       Mohammad.Ali            2/15/2013     Update
// 176485 modify       Sushma.Karumanchi       04/21/2020   SKarumanchi - Changes for CR 176485
// 194237 modify       Sanjay.Menon            07/12/2021   NET error - GetKeys() Workflow error across the workerweb pages
// 272075 modify       Suneel.vanka            05/29/2026   CR 272075 AWW Expense and Shelter screen to update historical records as Admin Error
///////////////////////////////////////////////////////////////////////////////////////////////////////

using DevExpress.Web.ASPxEditors;
using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Expense;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Expense;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Extensions;
using Dhss.Framework.Security;
using Resources;
using System;
using System.Transactions;
using System.Web.UI.WebControls;
using System.Linq;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Services;
using Dhss.Framework.Web.UI.Workflow;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using System.Web.UI;
using DevExpress.Web.ASPxClasses;
using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Income;
using Dhss.Framework.Extensions;
using System.Collections.Generic;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Expense
{
    public partial class MedicareInformation
    {
        /// <summary>
        /// Workflow method Bind Entities.
        /// </summary>
        public override void BindEntities()
        {
            BindMedicarePreminumPartBPaidBy();
            FvExpense_MedicareExpensePartABDetailBBindEntities();
            if (IntakeContext.Instance.MAProgramCode || IntakeContext.Instance.FBProgramCode)
            {              
                fvExpense_MedicareExpensePartDDetail.FindControl("cbEnrolled").Bind<ExpenseMetaDataRequired>(x => x.MedicareEnrolledIndicatorD);
            }
            else
            {                
                fvExpense_MedicareExpensePartDDetail.FindControl("cbEnrolled").Bind<ExpenseMetaDataNotRequired>(x => x.MedicareEnrolledIndicatorD);

            }
            if (IntakeContext.Instance.FBProgramCode)
            {                
                fvExpense_MedicareExpensePartDDetail.FindControl("txtTotalPremium").Bind<ExpenseMetaDataRequired>(x => x.TotalPremium);
                fvExpense_MedicareExpensePartDDetail.FindControl("txtClientPays").Bind<ExpenseMetaDataRequired>(x => x.PaidBy);
            }
            else
            {                
                fvExpense_MedicareExpensePartDDetail.FindControl("txtTotalPremium").Bind<ExpenseMetaDataNotRequired>(x => x.TotalPremium);
                fvExpense_MedicareExpensePartDDetail.FindControl("txtClientPays").Bind<ExpenseMetaDataNotRequired>(x => x.PaidBy);
            }

            if (!IntakeContext.Instance.IsUpdateDeleteReasonAdminMode)
                ApplyConditionalValidation();
        }

        /// <summary>
        /// Workflow method SaveData.
        /// </summary>
        public override void SaveData()
        {
            if (Infrastructure.Context.ExpenseSessionContext.Instance.MedicareName == false)
            {
                using (var scope = new TransactionScope())
                {
                    if (!IsBeginEndDateValid())
                        return;
                    ASPxComboBox cbVerifiedByA = (ASPxComboBox)fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicareEntitlementVeificatonCodeA");
                    ASPxComboBox cbVerifiedByB = (ASPxComboBox)fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitlementVeificatonCodeB");
                    ASPxComboBox cbVerifiedByD = (ASPxComboBox)fvExpense_MedicareExpensePartDDetail.FindControl("cbMedicareEntitlementVeificatonCode2");
                    ASPxComboBox cmbHistoryReason = (ASPxComboBox)fvExpense_MedicareExpense.FindControl("cbDeleteReasonCode");
                    if (cmbHistoryReason.Value != null)
                    {
                        //SKarumanchi - Changes for CR 176485
                        if (Convert.ToString(cmbHistoryReason.Value) != IntakeConstants.ADMINISTRATIVE_ERROR_ABBREVIATION && 
                            ((Convert.ToString(cbVerifiedByA.Value) == IntakeConstants.QUESTION) || (Convert.ToString(cbVerifiedByB.Value).Trim() == IntakeConstants.QUESTION) || (Convert.ToString(cbVerifiedByD.Value).Trim() == IntakeConstants.QUESTION) ||
                            (Convert.ToString(cbVerifiedByA.Value) == IntakeConstants.NOT_VERIFIED) || (Convert.ToString(cbVerifiedByB.Value).Trim() == IntakeConstants.NOT_VERIFIED) || (Convert.ToString(cbVerifiedByD.Value).Trim() == IntakeConstants.NOT_VERIFIED) ||
                            (Convert.ToString(cbVerifiedByA.Value) == IntakeConstants.SELF_ATTESTED) || (Convert.ToString(cbVerifiedByB.Value).Trim() == IntakeConstants.SELF_ATTESTED) || (Convert.ToString(cbVerifiedByD.Value).Trim() == IntakeConstants.SELF_ATTESTED)))
                        {
                            if((Convert.ToString(cbVerifiedByA.Value) == IntakeConstants.QUESTION) || (Convert.ToString(cbVerifiedByB.Value).Trim() == IntakeConstants.QUESTION) || (Convert.ToString(cbVerifiedByD.Value).Trim() == IntakeConstants.QUESTION))
                            {
                                Infrastructure.Context.ExpenseSessionContext.Instance.IsThereAnyPendingVerification = true;
                            }
                            ShowErrPopupAlert(IntakeResourceManager.PENDING_NV_SA_INFORMATION_HISTORY_REASON_ADMINISTRATIVE_ERROR, IntakeResourceManager.ERROR_TITLE507);
                            return;
                        }
                    }
                    //if (Infrastructure.Context.ExpenseSessionContext.Instance.MedicareInformationVerifiedBy)
                    //{
                    //    if (((ASPxComboBox)(fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicareEntitlementVeificatonCodeA"))).Value != null)
                    //    {
                    //        if (((ASPxComboBox)(fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicareEntitlementVeificatonCodeA"))).Value.ToString() == "FP")
                    //        {
                    //            ShowErrPopupAlert(IntakeResourceManager.FEDERAL_CANNOT_BE_SELECTED, "Error");
                    //            return;
                    //        }
                    //    }
                    //}
                    if (IntakeContext.Instance.IsUpdateDeleteReasonAdminMode)
                    {
                        if (IntakeContext.Instance.MedicareInformationDeleteReasonCodePreviousValue != Convert.ToString(cmbHistoryReason.Value))
                        {
                            if (Convert.ToString(cmbHistoryReason.Value) != ExpenseBusinessLogicConstants.AE)
                            {
                                ShowErrPopupAlert(IntakeResourceManager.INCOME_SCREENS_HISTORY_DATA_UPDATE_ERROR, IntakeResourceManager.ERROR_TITLE);
                                _isValidated = false;
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    fvExpense_MedicareExpense.UpdateItem(false);
                    fvExpense_MedicareExpensePartABDetailA.UpdateItem(false);
                    fvExpense_MedicareExpensePartABDetailB.UpdateItem(false);
                    fvExpense_MedicareExpensePartDDetail.UpdateItem(false);
                    if (!_isValidated) return;
                    SetPageCompleteness();
                    ExpenseSessionContext.Instance.IsExpense_Datachanged = false;

                }
            }
            else
            {
                ShowErrPopupAlert(IntakeResourceManager.RECORD_EXISTS_WITH_EXPENSE, ErrorMessages.MedicareInformation_Record_Exists);
            }
        }

        protected void SetPageCompleteness()
        {
            if (!CurrentWorkflowPage.Completed && _isValidated)
            {
                //UpdateBeginEndDatesInChildTable();
                ServicesApplicationHub.IntakeExpense.IsUpdateMedicareExpenseInformation(UpdateServiceRequest, int.Parse(WorkflowSession.Instance.RootFrame.State.Key), Convert.ToDecimal(WorkflowSession.Root["CaseNumber"]));
                SetPageComplete(true);
                IntakeContext.Instance.IsUpdateDeleteReasonAdminMode = false;
                //Reset Eligibility workflow
                WorkflowScheduling.ResetElgibilityWorkFlow(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                if (CurrentWorkflowPage.Context.Value.IsContextComplete())
                {
                    SetPreviousPageComplete(true);
                    SetPageComplete(IntakeConstants.MEDICARE_INFORMATION, true, true);
                }
                Infrastructure.Context.ExpenseSessionContext.Instance.IsAddNew = false;
                Infrastructure.Context.ExpenseSessionContext.Instance.MedicareExpRecs = 0;
                Infrastructure.Context.ExpenseSessionContext.Instance.MedicareExpesneID = 0;
            }
            else if (CurrentWorkflowPage.Context.Value.IsContextComplete())
            {
                SetPreviousPageComplete(true);
                SetPageComplete(IntakeConstants.MEDICARE_INFORMATION, true, true);
            }
        }
        /// <summary>
        /// Workflow method NavigateNext.
        /// </summary>
        public override void NavigateNext()
        {
            ASPxComboBox cbVerifiedByA = (ASPxComboBox)fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicareEntitlementVeificatonCodeA");
            ASPxComboBox cbVerifiedByB = (ASPxComboBox)fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitlementVeificatonCodeB");
            ASPxComboBox cbVerifiedByD = (ASPxComboBox)fvExpense_MedicareExpensePartDDetail.FindControl("cbMedicareEntitlementVeificatonCode2");
            ASPxComboBox cmbHistoryReason = (ASPxComboBox)fvExpense_MedicareExpense.FindControl("cbDeleteReasonCode");
            if (cmbHistoryReason.Value != null)
            {
                //SKarumanchi - Changes for CR 176485
                if (Convert.ToString(cmbHistoryReason.Value) != IntakeConstants.ADMINISTRATIVE_ERROR_ABBREVIATION &&
                    ((Convert.ToString(cbVerifiedByA.Value) == IntakeConstants.QUESTION) || (Convert.ToString(cbVerifiedByB.Value).Trim() == IntakeConstants.QUESTION) || (Convert.ToString(cbVerifiedByD.Value).Trim() == IntakeConstants.QUESTION) ||
                    (Convert.ToString(cbVerifiedByA.Value) == IntakeConstants.NOT_VERIFIED) || (Convert.ToString(cbVerifiedByB.Value).Trim() == IntakeConstants.NOT_VERIFIED) || (Convert.ToString(cbVerifiedByD.Value).Trim() == IntakeConstants.NOT_VERIFIED) ||
                    (Convert.ToString(cbVerifiedByA.Value) == IntakeConstants.SELF_ATTESTED) || (Convert.ToString(cbVerifiedByB.Value).Trim() == IntakeConstants.SELF_ATTESTED) || (Convert.ToString(cbVerifiedByD.Value).Trim() == IntakeConstants.SELF_ATTESTED)))
                {
                    if ((Convert.ToString(cbVerifiedByA.Value) == IntakeConstants.QUESTION) || (Convert.ToString(cbVerifiedByB.Value).Trim() == IntakeConstants.QUESTION) || (Convert.ToString(cbVerifiedByD.Value).Trim() == IntakeConstants.QUESTION))
                    {
                        Infrastructure.Context.ExpenseSessionContext.Instance.IsThereAnyPendingVerification = true;
                    }
                    ShowErrPopupAlert(IntakeResourceManager.PENDING_NV_SA_INFORMATION_HISTORY_REASON_ADMINISTRATIVE_ERROR, IntakeResourceManager.ERROR_TITLE507);
                    return;
                }
            }

            if (_isValidated)
            base.NavigateNext();
        }

        #region Bind Entities

        /// <summary>
        /// Handles FvExpense_MedicareExpenseBindEntities event.
        /// </summary>
        public void FvExpense_MedicareExpenseBindEntities()
        {
            fvExpense_MedicareExpense.FindControl("cbName").Bind<ExpensePersonIDMetaData>(x => x.PersonID);
            fvExpense_MedicareExpense.FindControl("ddeBeginDate").Bind<ExpenseMetaDataRequired>(x => x.MedicareStartDate);
            fvExpense_MedicareExpense.FindControl("cbDeleteReasonCode").Bind<ExpenseGridMetaData>(x => x.DeleteReasonCode);
            fvExpense_MedicareExpense.FindControl("ddeEndDate").Bind<ExpenseMetaDataNotRequired>(x => x.EndDate);
            txtMedicareNumber.Bind<ExpenseMetaDataNotRequired>(x => x.MedicareNumber);            

            ApplyHistoryReasonRule();
            CheckNonAdminErrorEnded();

            ASPxDateEdit begindate = (ASPxDateEdit)fvExpense_MedicareExpense.FindControl("ddeBeginDate");
            ASPxDateEdit enddate = (ASPxDateEdit)fvExpense_MedicareExpense.FindControl("ddeEndDate");
            EndDateSetting(begindate, enddate);

            HiddenField hdMedicareNumber = (HiddenField)fvExpense_MedicareExpense.FindControl("hdMedicareNumber");
            if (hdMedicareNumber != null)
                txtMedicareNumber.Text = hdMedicareNumber.Value;

            HiddenField hdMedicareBuyingEffectiveDate =
                (HiddenField)fvExpense_MedicareExpense.FindControl("hdMedicareBuyingEffectiveDate");
            if (hdMedicareBuyingEffectiveDate != null && hdMedicareBuyingEffectiveDate.Value != string.Empty)
                dtBuyInEffectiveDate.Date = Convert.ToDateTime(hdMedicareBuyingEffectiveDate.Value);
        }

        /// <summary>
        /// Handles FvExpense_MedicareExpensePartABDetailABindEntities event.
        /// </summary>
        public void FvExpense_MedicareExpensePartABDetailABindEntities()
        {
            if( IntakeContext.Instance.MAProgramCode == true )
                fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicareEntitledIndicatorA").Bind<ExpenseMetaDataRequired>(x => x.IsYesNoCodeBit);
            else
                fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicareEntitledIndicatorA").Bind<ExpenseMetaDataNotRequired>(x => x.IsYesNoCodeBit);

            fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicareEntitlementVeificatonCodeA").Bind<ExpenseMetaDataNotRequired>(x => x.MedicareInformationVerifiedBy);
            fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicarePaidByCodeA").Bind<ExpenseMetaDataNotRequired>(x => x.MedicarePaidByCode);

            var mBegindateA = (ASPxDateEdit)fvExpense_MedicareExpensePartABDetailA.FindControl("dtMedicareStartDate");
            var enddateA = (ASPxDateEdit)fvExpense_MedicareExpensePartABDetailA.FindControl("dtEndDate");
            EndDateSetting(mBegindateA, enddateA);
        }

        /// <summary>
        /// Handles FvExpense_MedicareExpensePartABDetailBBindEntities event.
        /// </summary>
        public void FvExpense_MedicareExpensePartABDetailBBindEntities()
        {
            if (IntakeContext.Instance.MAProgramCode || IntakeContext.Instance.FBProgramCode)
            {
                fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitledIndicatorB").Bind<ExpenseMetaDataRequired>(x => x.IsYesNoCodeBit);
            }
            else
            {
                fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitledIndicatorB").Bind<ExpenseMetaDataRequired>(x => x.IsYesNoCodeBitNotRequired);
            }

            //if (IntakeContext.Instance.FBProgramCode)
            //{
            //    fvExpense_MedicareExpensePartABDetailB.FindControl("txtMedicarePremiumAmountB").Bind<ExpenseMetaDataRequired>(x => x.Premium);
            //    fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicarePaidByCodeB").Bind<ExpenseMetaDataRequired>(x => x.PaidBy);
            //}
            //else
            //{
            //    fvExpense_MedicareExpensePartABDetailB.FindControl("txtMedicarePremiumAmountB").Bind<ExpenseMetaDataNotRequired>(x => x.Premium);
            //    fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicarePaidByCodeB").Bind<ExpenseMetaDataNotRequired>(x => x.PaidBy);
            //}
            fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitlementVeificatonCodeB").Bind<ExpenseMetaDataNotRequired>(x => x.MedicareInformationVerifiedBy);
            CheckPartBMandatory();
            var mBegindateB = (ASPxDateEdit)fvExpense_MedicareExpensePartABDetailB.FindControl("dtMedicareStartDate1");
            var enddateB = (ASPxDateEdit)fvExpense_MedicareExpensePartABDetailB.FindControl("dtEndDate1");
            EndDateSetting(mBegindateB, enddateB);

        }
        /// <summary>
        /// Handles FvExpense_MedicareExpensePartDDetailBindEntities event.
        /// </summary>
        public void FvExpense_MedicareExpensePartDDetailBindEntities()
        {
            //if (IntakeContext.Instance.MAProgramCode == true || IntakeContext.Instance.FBProgramCode == true)
            //    fvExpense_MedicareExpensePartDDetail.FindControl("cbEnrolled").Bind<ExpenseMetaDataRequired>(x => x.MedicareEnrolledIndicatorD);
            //else
            //    fvExpense_MedicareExpensePartDDetail.FindControl("cbEnrolled").Bind<ExpenseMetaDataNotRequired>(x => x.MedicareEnrolledIndicatorD);

            fvExpense_MedicareExpensePartDDetail.FindControl("cbMedicareEntitlementVeificatonCode2").Bind<ExpenseMetaDataNotRequired>(x => x.MedicareInformationVerifiedBy);
            fvExpense_MedicareExpensePartDDetail.FindControl("cbCreditableCoverageIndicator").Bind<ExpenseMetaDataNotRequired>(x => x.CreditableCoverageIndicatorD);


            //if( IntakeContext.Instance.FBProgramCode)
            //    fvExpense_MedicareExpensePartDDetail.FindControl("txtTotalPremium").Bind<ExpenseMetaDataRequired>(x => x.TotalPremium);
            //else
            //    fvExpense_MedicareExpensePartDDetail.FindControl("txtTotalPremium").Bind<ExpenseMetaDataNotRequired>(x => x.TotalPremium);



            var begindateD = (ASPxDateEdit)fvExpense_MedicareExpensePartDDetail.FindControl("dtMedicareStartDate2");
            var enddateD = (ASPxDateEdit)fvExpense_MedicareExpensePartDDetail.FindControl("dtEndDate2");
            EndDateSetting(begindateD, enddateD);
        }


        #endregion

        /// <summary>
        /// Applys HistoryReasonRule.
        /// </summary>
        private void ApplyHistoryReasonRule()
        {
            var cmbHistoryReason = (ASPxComboBox)fvExpense_MedicareExpense.FindControl("cbDeleteReasonCode");
            var cbPerson = (ASPxComboBox)fvExpense_MedicareExpense.FindControl("cbName");
            cmbHistoryReason.ClientEnabled = cbPerson.Value != null;
            //var ctx = new ExpenseContextImpl();
            //var medicareExpense = ctx.Expense_MedicareExpense.Where(m => m.ExpenseID == AnchorObject.ExpenseID).First();
            //var syncState = Convert.ToInt16(medicareExpense.SyncState);
            //cmbHistoryReason.Enabled = syncState != 0;
        }

        /// <summary>
        /// Checks NonAdminErrorEnded.
        /// </summary>
        private void CheckNonAdminErrorEnded()
        {
            string historyCode = (fvExpense_MedicareExpense.FindControl("hfHistoryCode") as HiddenField).Value;
            var cmbHistoryReason = (ASPxComboBox)fvExpense_MedicareExpense.FindControl("cbDeleteReasonCode");
            var ddeEndDate = (ASPxDateEdit)fvExpense_MedicareExpense.FindControl("ddeEndDate");
            var mpContentPlaceHolder = Master.ViewBodyActionBar;
            var btnPageSave = (ASPxButton)mpContentPlaceHolder.FindControl("btnPageSave");
            char roleSeparator = Convert.ToChar(WebConfigContext.SecurityRoleSeparator);
            List<string> headerRoles = new List<string>();
            if (!string.IsNullOrEmpty(Convert.ToString(Request.Headers["ROLES"])))
            {
                headerRoles = Convert.ToString(Request.Headers["ROLES"]).Split(roleSeparator).ToList();
            }
            List<string> accessRoles = WebConfigContext.ExpenseScreensHistoryDataUpdateRole.Split(roleSeparator).ToList();
            bool checkRole = IncomeCommon.CheckRoles(headerRoles, accessRoles);

            if ((ExpenseBusinessLogic.IsControlNull(ddeEndDate) && cmbHistoryReason.Value != null) || historyCode == IntakeConstants.HISTORY_RECORD_CODE)
            {
                if (ExpenseBusinessLogic.IsValidAdminError(ddeEndDate.Text, (cmbHistoryReason.SelectedItem != null ? Convert.ToString(cmbHistoryReason.SelectedItem.Value) : null)) || historyCode == IntakeConstants.HISTORY_RECORD_CODE)
                {
                    if ((cmbHistoryReason.Value != null && cmbHistoryReason.Value.ToString() == ExpenseBusinessLogicConstants.AE) || (historyCode != null && historyCode == IntakeConstants.HISTORY_RECORD_CODE) || !checkRole)
                    {
                        fvExpense_MedicareExpense.Enabled = false;
                        fvExpense_MedicareExpensePartABDetailA.Enabled = false;
                        fvExpense_MedicareExpensePartABDetailB.Enabled = false;
                        fvExpense_MedicareExpensePartDDetail.Enabled = false;
                        lblMedicareNumber.Enabled = false;
                        lblBuyInEffectiveDate.Enabled = false;
                        dtBuyInEffectiveDate.Enabled = false;
                        txtMedicareNumber.Enabled = false;

                        if (btnPageSave != null)
                        {
                            btnPageSave.Enabled = false;
                        }
                    }
                    else
                    {
                        DisableWebControl(fvExpense_MedicareExpense, checkRole);
                        fvExpense_MedicareExpensePartABDetailA.Enabled = false;
                        fvExpense_MedicareExpensePartABDetailB.Enabled = false;
                        fvExpense_MedicareExpensePartDDetail.Enabled = false;
                        lblMedicareNumber.Enabled = false;
                        lblBuyInEffectiveDate.Enabled = false;
                        dtBuyInEffectiveDate.Enabled = false;
                        txtMedicareNumber.Enabled = false;
                        if (btnPageSave != null)
                        {
                            if (checkRole && cmbHistoryReason.Value != null && cmbHistoryReason.Value.ToString() != ExpenseBusinessLogicConstants.AE)
                            {
                                btnPageSave.Enabled = true;
                                IntakeContext.Instance.IsUpdateDeleteReasonAdminMode = true;
                            }
                            else
                                btnPageSave.Enabled = false;
                        }
                    }
                }
                else
                {
                    fvExpense_MedicareExpense.Enabled = true;
                    fvExpense_MedicareExpensePartABDetailA.Enabled = true;
                    fvExpense_MedicareExpensePartABDetailB.Enabled = true;
                    fvExpense_MedicareExpensePartDDetail.Enabled = true;
                    dtBuyInEffectiveDate.Enabled = true;
                    txtMedicareNumber.Enabled = true;
                    lblMedicareNumber.Enabled = true;
                    lblBuyInEffectiveDate.Enabled = true;

                    if (btnPageSave != null)
                    {
                        btnPageSave.Enabled = true;
                    }
                }
            }

        }

        /// <summary>
        /// EndDate Setting.
        /// </summary>
        /// <param name="ctlBegindate"></param>
        /// <param name="ctlEndDate"></param>
        private void EndDateSetting(ASPxDateEdit ctlBegindate, ASPxDateEdit ctlEndDate)
        {
            ctlEndDate.MinDate = ctlBegindate.Date;
        }

        /// <summary>
        /// The Method checks the record exists in the Expense.
        /// </summary>
        /// <param name="expenseTypeCode"></param>
        /// <returns></returns>
        public bool IsCheckPrimary(string expenseTypeCode)
        {
            var cbperson = (ASPxComboBox)(this.fvExpense_MedicareExpense.FindControl("cbName"));
            int personId = Convert.ToInt32(cbperson.SelectedItem.Value);
            var ctx = new ExpenseContextImpl();
            var dupCount = ctx.Expense_Expense.Where(m => m.PersonID == personId && m.ExpenseTypeCode == expenseTypeCode && m.DeleteReasonCode == null && m.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE).Count();

            if (dupCount > 0)
            {
                ShowErrPopupAlert(IntakeResourceManager.RECORD_EXISTS_WITH_EXPENSE, ErrorMessages.MedicareInformation_Record_Exists);
                Infrastructure.Context.ExpenseSessionContext.Instance.MedicareName = true;
                return true;
            }
            Infrastructure.Context.ExpenseSessionContext.Instance.MedicareName = false;
            dxPopupErr.ShowOnPageLoad = false;
            return false;
        }

        /// <summary>
        /// Refreshs AnchorObject.
        /// </summary>
        /// <param name="expenseId"></param>
        private void RefreshAnchorObject(Int32 expenseId)
        {
            //var expCont = new ExpenseContextImpl();
            var medicareExpense = new Expense_MedicareExpense() { ExpenseID =expenseId};
            ReloadAnchorObject();
            int? temp = this.AnchorObject.ExpenseID;
            NavigateTo(medicareExpense);
        }

        /// <summary>
        /// Updates BeginEndDatesInChildTable.
        /// </summary>
        private void UpdateBeginEndDatesInChildTable()
        {
            Int16 syncState = 1;
            var beginDate = (ASPxDateEdit)fvExpense_MedicareExpense.FindControl("ddeBeginDate");
            var endDate = (ASPxDateEdit)fvExpense_MedicareExpense.FindControl("ddeEndDate");
            var ctx = new ExpenseContextImpl();
            var medicareExpenseObejct = ctx.Expense_MedicareExpense.Where(n => n.ExpenseID == AnchorObject.ExpenseID).First();
            if (Infrastructure.Context.ExpenseSessionContext.Instance.IsRetro) { syncState = 2; Infrastructure.Context.ExpenseSessionContext.Instance.IsRetro = false; }
            if (medicareExpenseObejct.SyncState != null) medicareExpenseObejct.SyncState = syncState;
            medicareExpenseObejct.LastSavedByID = SystemPrincipal.Current.Identity.Name;
            ctx.UpdateObject(medicareExpenseObejct);

            var expenseObejct = ctx.Expense_Expense.Where(n => n.ExpenseID == AnchorObject.ExpenseID).First();
            expenseObejct.BeginDate = beginDate.Date;
            if (Convert.ToDateTime(endDate.Value) != DateTime.MinValue) expenseObejct.EndDate = endDate.Date;
            if (expenseObejct.SyncState != null) expenseObejct.SyncState = syncState;
            expenseObejct.LastSavedByID = SystemPrincipal.Current.Identity.Name;
            ctx.UpdateObject(expenseObejct);

            var amedicareExpensePartAbDetailObejct = ctx.Expense_MedicareExpensePartABDetail.Where(n => n.MedicareExpense.ExpenseID == AnchorObject.ExpenseID && n.MedicareTypeCode == IntakeConstants.MEDICARE_TYPE_CODE_A).First();
            amedicareExpensePartAbDetailObejct.BeginDate = beginDate.Date;
            amedicareExpensePartAbDetailObejct.BeginDate = beginDate.Date;
            if (amedicareExpensePartAbDetailObejct.SyncState != null) amedicareExpensePartAbDetailObejct.SyncState = syncState;
            amedicareExpensePartAbDetailObejct.LastSavedByID = SystemPrincipal.Current.Identity.Name;
            ctx.UpdateObject(amedicareExpensePartAbDetailObejct);

            var bmedicareExpensePartAbDetailObejct = ctx.Expense_MedicareExpensePartABDetail.Where(n => n.MedicareExpense.ExpenseID == AnchorObject.ExpenseID && n.MedicareTypeCode == IntakeConstants.MEDICARE_TYPE_CODE_A).First();
            bmedicareExpensePartAbDetailObejct.BeginDate = beginDate.Date;
            bmedicareExpensePartAbDetailObejct.BeginDate = beginDate.Date;
            if (bmedicareExpensePartAbDetailObejct.SyncState != null) bmedicareExpensePartAbDetailObejct.SyncState = syncState;
            bmedicareExpensePartAbDetailObejct.LastSavedByID = SystemPrincipal.Current.Identity.Name;
            ctx.UpdateObject(bmedicareExpensePartAbDetailObejct);

            var medicareExpensePartDDetailObejct = ctx.Expense_MedicareExpensePartDDetail.Where(n => n.MedicareExpense.ExpenseID == AnchorObject.ExpenseID).First();
            medicareExpensePartDDetailObejct.BeginDate = beginDate.Date;
            medicareExpensePartDDetailObejct.BeginDate = beginDate.Date;
            if (medicareExpensePartDDetailObejct.SyncState != null) medicareExpensePartDDetailObejct.SyncState = syncState;
            medicareExpensePartDDetailObejct.LastSavedByID = SystemPrincipal.Current.Identity.Name;
            ctx.UpdateObject(medicareExpensePartDDetailObejct);

            ctx.SaveChanges();
            Infrastructure.Context.ExpenseSessionContext.Instance.MedicareExpesneID = 0;
        }

        /// <summary>
        /// Shows PopupAlert.
        /// </summary>
        /// <param name="stralertmsg"></param>
        /// <param name="errorMsg"></param>
        private void ShowErrPopupAlert(string stralertmsg, string errorMsg)
        {
            dxPopupErr.ShowOnPageLoad = true;
            var btnok = (ASPxButton)dxPopupErr.FindControl("btnok");
            btnok.Focus();
            dxPopupErr.HeaderText = errorMsg;
            var lblalertmessage = (ASPxLabel)dxPopupErr.FindControl("lblErrmessage");
            lblalertmessage.Text = stralertmsg;
        }

        /// <summary>
        /// Handles Btnok_Click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btnok_Click(object sender, EventArgs e)
        {
            dxPopupErr.ShowOnPageLoad = false;
        }

        #region Create New Expense
        /// <summary>
        /// Creates NewExpense.
        /// </summary>
        /// <returns></returns>
        private int CreateNewExpense()
        {
            int expenseId = 0;
            var ctx = new ExpenseContextImpl();
            Expense_Expense expense = ExpenseEntitiesCreation.GetExpenseObject(ExpenseBusinessLogicConstants.MEDICARE_INFORMATION);
            ctx.AddObject("Expense_Expense", expense);
            ctx.SaveChanges();
            expenseId = expense.ExpenseID;
            ctx.AddObject("Expense_MedicareExpense", ExpenseEntitiesCreation.GetMedicareExpenseObject(expenseId));
            ctx.SaveChanges();
            ctx.AddObject("Expense_MedicareExpensePartABDetail", ExpenseEntitiesCreation.GetMedicareExpensePartABDetailObject(expenseId, IntakeConstants.MEDICARE_TYPE_CODE_A));
            ctx.SaveChanges();
            ctx.AddObject("Expense_MedicareExpensePartABDetail", ExpenseEntitiesCreation.GetMedicareExpensePartABDetailObject(expenseId, IntakeConstants.MEDICARE_TYPE_CODE_B));
            ctx.SaveChanges();
            ctx.AddObject("Expense_MedicareExpensePartDDetail", ExpenseEntitiesCreation.GetMedicareExpensePartDDetailObject(expenseId));
            ctx.SaveChanges();
            return expenseId;
        }
        #endregion

        /// <summary>
        /// Diabale the Medicare Information historical records screen based on DeleteReasonCode
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="checkRole"></param>
        private void DisableWebControl(Control parent, bool checkRole)
        {
            var mpContentPlaceHolder = Master.ViewBodyActionBar;
            foreach (Control control in parent.Controls)
            {
                if (control is ASPxWebControl dxControl)
                {
                    dxControl.Enabled = false;
                    var cbHistoryReason = parent.FindControl("cbDeleteReasonCode").As<ASPxComboBox>();
                    if (cbHistoryReason != null && cbHistoryReason.Value != null)
                    {
                        if (checkRole && cbHistoryReason.Value.ToString() != ExpenseBusinessLogicConstants.AE)
                        {
                            cbHistoryReason.Enabled = true;
                        }
                        else
                        {
                            cbHistoryReason.Enabled = false;
                        }
                    }
                }
                if (control.HasControls())
                    DisableWebControl(control, checkRole);
            }

        }
    }

}

