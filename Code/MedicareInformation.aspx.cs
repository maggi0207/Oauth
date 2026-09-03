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
// 151973 modify       Sanjay.Menon            01/29/2021   WW_SelfEmployment_BeginDate_please change to 3/2018_Tawanna Purnell HD 734142 Case 3007190131
// 183292 Modify       Sanjay.menon            10/28/2021   WW_AE_Utility Cost Details page_Must Save Twice - Same issue on multiple AE pages
// 173244 Modify       Sanjay.menon            04/04/2022   WW_AE_Medicare Details page_Alphanumeric First Name Message, But Name Does Not Contain Numbers
// 272075 modify       Suneel.vanka            05/29/2026   CR 272075 AWW Expense and Shelter screen to update historical records as Admin Error
///////////////////////////////////////////////////////////////////////////////////////////////////////

using DevExpress.Web.ASPxEditors;
using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Expense;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Expense;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Controls;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Extensions;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Services;
using Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Income;
using Dhss.Assist.WorkerWeb.Web.VerifyNonEsiMecService;
using Dhss.Framework;
using Dhss.Framework.Extensions;
using Dhss.Framework.Web.UI.Workflow;
using Resources;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Services.Client;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Dhss.Assist.WorkerWeb.Web.ImageIntegration;
using Dhss.Assist.WorkerWeb.Entity.ImageIntegration;
using System.Web.UI;
using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Resources;
using Dhss.Assist.WorkerWeb.BusinessLogic;
using System.IO;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Expense
{
    [Workflow]
    [ExcludeFromCodeCoverage]
    public partial class MedicareInformation : Dhss.Assist.WorkerWeb.Web.Infrastructure.Workflow.WorkflowPage<Expense_MedicareExpense>
    {
        private const string DAEEMD02 = "DAEEMD02";
        private const string PVALUE = "P";
        private Expense_MedicareExpense UpdateServiceRequest { get; set; }
        int _expenseId;
        DateTime? _db2UpdatedDate;
        bool _isValidated = true;
        private readonly ResourcesBusinessLogic _resourceBlogic = new ResourcesBusinessLogic();

        /// <summary>
        /// Handles Page_Load event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            IntakeContext.Instance.FBProgramCode = false;
            InitializeSession();
            var mpContentPlaceHolder = (ContentPlaceHolder)Master.ViewBodyActionBar;
            var btnPageAddNew = (ASPxButton)mpContentPlaceHolder.FindControl("btnPageAddNew");
            if (btnPageAddNew != null)
            {
                btnPageAddNew.Click += BtnPageAddNew_OnClick;
            }
            var btnPageSaveData = (ASPxButton)mpContentPlaceHolder.FindControl("btnPageSave");
            if (btnPageSaveData != null)
            {
                btnPageSaveData.Click += RefreshPageData;
            }
            if (!IsPostBack)
            {
                //fvExpense_MedicareExpense.DataBind();
            }
            PageLoadValCont();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeSession()
        {
            _expenseId = AnchorObject.ExpenseID;
            _db2UpdatedDate = AnchorObject.DB2UpdatedDate;
        }
        /// <summary>
        /// 
        /// </summary>
        private void PageLoadValCont()
        {
            var cbperson = (ASPxComboBox)(this.fvExpense_MedicareExpense.FindControl("cbName"));
            if (Infrastructure.Context.ExpenseSessionContext.Instance.IsAddNew && !(Infrastructure.Context.ExpenseSessionContext.Instance.MedicareExpRecs > 0))
            {
                if (ExpenseEntitiesCreation.DeleteMedicareExpenseRecord(_expenseId, true))
                    btnOops.Enabled = true;
                cbperson.ClientEnabled = true;
                SetPageComplete(false);
            }
            else
            {
                if (Infrastructure.Context.ExpenseSessionContext.Instance.IsAddNew) cbperson.ClientEnabled = true;
                btnOops.Enabled = false;
            }
            Master.CurrentPersonSelectedId = Convert.ToInt32(cbperson.Value);
            //Global defect 36243 - Set focus 
            if (!IsPostBack)
            {
                ASPxDateEdit EndDate = (ASPxDateEdit)fvExpense_MedicareExpense.FindControl("ddeEndDate");
                if (cbperson.ClientEnabled == false)
                    EndDate.Focus();
                else cbperson.Focus();
                ASPxComboBox cbDeleteReasonCode = (ASPxComboBox)fvExpense_MedicareExpense.FindControl("cbDeleteReasonCode");
                IntakeContext.Instance.MedicareInformationDeleteReasonCodePreviousValue = cbDeleteReasonCode.Value != null ? Convert.ToString(cbDeleteReasonCode.Value) : null;
            }
            
            var serverdt = fvExpense_MedicareExpense.FindControl("serverdt") as HiddenField;
            if (serverdt != null)
            {
                serverdt.Value = SystemDateTime.Now.ToString();
            }
            SelfAttestedValidation();
        }

        /// <summary>
        /// Will do required validation for selft attested validation
        /// </summary>
        void SelfAttestedValidation()
        {
            //self attested pending verificaiton start
            if (SelfAttestedConditionalValidation.IsSelfAttestedValidationRequired(_db2UpdatedDate))
            {
                var hdMedicareEntitlementVeificatonCodeA = fvExpense_MedicareExpensePartABDetailA.FindControl("hdMedicareEntitlementVeificatonCodeA") as HiddenField;
                if (hdMedicareEntitlementVeificatonCodeA != null && hdMedicareEntitlementVeificatonCodeA.Value != null)
                {
                    var cbVerifiedByA = fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicareEntitlementVeificatonCodeA") as ASPxComboBox;
                    SelfAttestedConditionalValidation.AttachSelfAttestedValidation(Page, cbVerifiedByA, hdMedicareEntitlementVeificatonCodeA.Value);
                }

                var hdMedicareEntitlementVeificatonCodeB = fvExpense_MedicareExpensePartABDetailB.FindControl("hdMedicareEntitlementVeificatonCodeB") as HiddenField;
                if (hdMedicareEntitlementVeificatonCodeB != null && hdMedicareEntitlementVeificatonCodeB.Value != null)
                {
                    var cbVerifiedByB = fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitlementVeificatonCodeB") as ASPxComboBox;
                    SelfAttestedConditionalValidation.AttachSelfAttestedValidation(Page, cbVerifiedByB, hdMedicareEntitlementVeificatonCodeB.Value);
                }

                var hdMedicareEntitlementVeificatonCode2 = fvExpense_MedicareExpensePartDDetail.FindControl("hdMedicareEntitlementVeificatonCode2") as HiddenField;
                if (hdMedicareEntitlementVeificatonCode2 != null && hdMedicareEntitlementVeificatonCode2.Value != null)
                {
                    var cbVerifiedByD = fvExpense_MedicareExpensePartDDetail.FindControl("cbMedicareEntitlementVeificatonCode2") as ASPxComboBox;
                    SelfAttestedConditionalValidation.AttachSelfAttestedValidation(Page, cbVerifiedByD, hdMedicareEntitlementVeificatonCode2.Value);
                }
            }
        }

        /// <summary>
        /// Refreshs PageData.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RefreshPageData(object sender, EventArgs e)
        {
            if (Infrastructure.Context.ExpenseSessionContext.Instance.MedicareName == false)
            {
                if (ExpenseEntitiesCreation.IsValidationCheck(dxPopupErr.ShowOnPageLoad))
                {
                    //var ctx = new ExpenseContextImpl();
                    //var cbperson = (ASPxComboBox)(fvExpense_MedicareExpense.FindControl("cbName"));
                    //int personId = Convert.ToInt32(cbperson.Value);
                    //Expense_Expense newexpense = ctx.Expense_Expense.Where(n => (n.ExpenseTypeCode == ExpenseDataContext.ExpesneType.MDI.ToString() && n.PersonID == personId)).OrderByDescending(m => m.HistorySequenceNumber).First();
                    Infrastructure.Context.ExpenseSessionContext.Instance.MedicareExpesneID = this._expenseId;
                    RefreshAnchorObject(Infrastructure.Context.ExpenseSessionContext.Instance.MedicareExpesneID);
                }
            }
        }

        /// <summary>
        /// Handles BtnPageAddNew_OnClick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPageAddNew_OnClick(object sender, EventArgs e)
        {
            if (_isValidated)
            {
                Infrastructure.Context.ExpenseSessionContext.Instance.MedicareExpesneID = CreateNewExpense();
                Infrastructure.Context.ExpenseSessionContext.Instance.IsAddNew = true;
                Infrastructure.Context.ExpenseSessionContext.Instance.MedicareName = false;
                Infrastructure.Context.ExpenseSessionContext.Instance.MedicareExpRecs += 1;

                RefreshAnchorObject(Infrastructure.Context.ExpenseSessionContext.Instance.MedicareExpesneID);
            }
        }

        #region Hidden field binding
        /// <summary>
        /// Match txtMedicareNumber.
        /// </summary>
        protected void MatchtxtMedicareNumber()
        {
            var hdMedicareNumber = (HiddenField)fvExpense_MedicareExpense.FindControl("hdMedicareNumber");
            hdMedicareNumber.Value = txtMedicareNumber.Text;
        }


        /// <summary>
        /// Handles DtBuyInEffectiveDate_ValueChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DtBuyInEffectiveDate_ValueChanged(object sender, EventArgs e)
        {
            var hdMedicareBuyingEffectiveDate = (HiddenField)fvExpense_MedicareExpense.FindControl("hdMedicareBuyingEffectiveDate");
            hdMedicareBuyingEffectiveDate.Value = dtBuyInEffectiveDate.Text;

            var buyInEffectiveDate = sender as ASPxDateEdit;
            DateTime dt = DateTime.Now.AddMonths(-3);
            if (buyInEffectiveDate != null && buyInEffectiveDate.Date != null && dt.Date != null && buyInEffectiveDate.Date < dt.Date)
            {
                // "BuyInEffectiveDate Valiadation - Defect 43917"
                ShowPopupInfoValid("You entered the buy-in effective date for more than 90 days, is this correct?");
                return;
            }

        }

        private void ShowPopupInfoValid(string message)
        {
            dxPopupErrorBuyInDate.ShowOnPageLoad = true;
            var lblmessage = (ASPxLabel)dxPopupErrorBuyInDate.FindControl("lblpopupBuyInDate");
            lblmessage.Text = message;
        }

        /// <summary>
        /// Handles FvExpense_MedicareExpense_DataBound event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvExpense_MedicareExpense_DataBound(object sender, EventArgs e)
        {
            FvExpense_MedicareExpenseBindEntities();

            if (!Infrastructure.Context.ExpenseSessionContext.Instance.IsAddNew)
            {
                Int16 syncState = Convert.ToInt16(this.AnchorObject.SyncState);
                { SetPageComplete(syncState == 3 || (syncState > 0 && WorkflowSession.Instance.CurrentFrame.CurrentEntity.Completed)); }

            }
            var cbVerifiedByA = (ASPxComboBox)fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicareEntitlementVeificatonCodeA");
            var cbVerifiedByB = (ASPxComboBox)fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitlementVeificatonCodeB");
            var cbVerifiedByD = (ASPxComboBox)fvExpense_MedicareExpensePartDDetail.FindControl("cbMedicareEntitlementVeificatonCode2");
            if (Convert.ToString(cbVerifiedByA.Value).Trim() == IntakeConstants.QUESTION || Convert.ToString(cbVerifiedByB.Value).Trim() == IntakeConstants.QUESTION || Convert.ToString(cbVerifiedByD.Value).Trim() == IntakeConstants.QUESTION)
            {
                var beginDate = (ASPxDateEdit)fvExpense_MedicareExpense.FindControl("ddeBeginDate");
                beginDate.ClientEnabled = false;
            }
            BindMedicarePreminumPaidBy();
            BindMedicarePreminumPartBPaidBy();
        }

        /// <summary>
        /// Handles DtBuyInEffectiveDate_Load event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DtBuyInEffectiveDate_Load(object sender, EventArgs e)
        {
            MakeMedicareNumberReq(sender, e);
        }
        #endregion

        #region Conditional validation for Buy-in Date
        /// <summary>
        /// Handles MakeMedicareNumberReq event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MakeMedicareNumberReq(object sender, EventArgs e)
        {
            var mDtBuyInEffectiveDate = dtBuyInEffectiveDate;

            if (mDtBuyInEffectiveDate != null && mDtBuyInEffectiveDate.Text != String.Empty)
            {
                txtMedicareNumber.Bind<ExpenseMetaDataRequired>(b => b.MedicareNumber);
            }
            else
            {
                txtMedicareNumber.Bind<ExpenseMetaDataNotRequired>(b => b.MedicareNumber);
            }
        }
        #endregion

        /// <summary>
        /// Handles FvExpense_MedicareExpensePartABDetailA_DataBound event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvExpense_MedicareExpensePartABDetailA_DataBound(object sender, EventArgs e)
        {
            FvExpense_MedicareExpensePartABDetailABindEntities();
        }

        /// <summary>
        /// handles FvExpense_MedicareExpensePartABDetailB_DataBound event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvExpense_MedicareExpensePartABDetailB_DataBound(object sender, EventArgs e)
        {
            FvExpense_MedicareExpensePartABDetailBBindEntities();
        }

        /// <summary>
        /// Handles FvExpense_MedicareExpensePartDDetail_DataBound event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvExpense_MedicareExpensePartDDetail_DataBound(object sender, EventArgs e)
        {
            FvExpense_MedicareExpensePartDDetailBindEntities();
        }

        /// <summary>
        /// Handles DsExpense_MedicareExpense_Updating event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DsExpense_MedicareExpense_Updating(object sender, LinqDataSourceUpdateEventArgs e)
        {
            //int personId = 0;
            //var expenseContextImpl = new ExpenseContextImpl();
            //var cbPerson = (ASPxComboBox)fvExpense_MedicareExpense.FindControl("cbName");
            //Expense_Expense expns = expenseContextImpl.Expense_Expense.Where(x => x.ExpenseID == AnchorObject.ExpenseID).FirstOrDefault();

            //if (cbPerson != null)
            //{
            //    personId = Convert.ToInt32(cbPerson.SelectedItem.Value);
            //    if (expns != null) expns.PersonID = personId;
            //}

            //if (Infrastructure.Context.ExpenseSessionContext.Instance.IsAddNew)
            //{
            //    Int16? sequenceNumber = 0;
            //    var expense = ((DataServiceQuery<Expense_Expense>)expenseContextImpl.Expense_Expense
            //                                                      .Where(m => m.PersonID == personId && m.ExpenseTypeCode == ExpenseDataContext.ExpesneType.MDI.ToString() && m.SyncState == 3))
            //                                                      .OrderByDescending(m => m.HistorySequenceNumber);
            //    if (expense.Count() > 0)
            //    {
            //        sequenceNumber = expense.First().SequenceNumber;
            //        var historySequenceNumber = expense.First().HistorySequenceNumber;
            //        historySequenceNumber++;
            //        expns.HistorySequenceNumber = historySequenceNumber;

            //    }
            //    sequenceNumber++;
            //    if (expns != null) expns.SequenceNumber = sequenceNumber;
            //}

            //expenseContextImpl.UpdateObject(expns);
            //expenseContextImpl.SaveChanges();
        }

        #region Conditional Validation for Entitled/Enrolled
        /// <summary>
        /// Handles CbMedicareEntitledIndicatorA_SelectedIndexChanged events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbMedicareEntitledIndicatorA_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cb = sender as ASPxComboBox;
            ASPxComboBox partBentitled = (ASPxComboBox)fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitledIndicatorB");
            ASPxComboBox partBVerifiedby = (ASPxComboBox)fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitlementVeificatonCodeB");

            if (cb != null && cb.SelectedItem.Text == IntakeConstants.YES_STRING)
            {
                fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicareEntitlementVeificatonCodeA").Bind<ExpenseMetaDataRequired>(b => b.MedicareInformationVerifiedBy);
                partBentitled.Text = IntakeConstants.YES_STRING;
                if (partBVerifiedby.Value == null)
                    partBVerifiedby.Value = "AF";
                fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitlementVeificatonCodeB").Bind<ExpenseMetaDataRequired>(b => b.MedicareInformationVerifiedBy);
                //fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicarePaidByCodeB").Bind<ExpenseMetaDataRequired>(b => b.MedicarePaidByCode);
            }
            else
            {
                fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicareEntitlementVeificatonCodeA").Bind<ExpenseMetaDataNotRequired>(b => b.MedicareInformationVerifiedBy);
                fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitlementVeificatonCodeB").Bind<ExpenseMetaDataNotRequired>(b => b.MedicareInformationVerifiedBy);
                //fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicarePaidByCodeB").Bind<ExpenseMetaDataNotRequired>(b => b.MedicarePaidByCode);
                // partBentitled.Text = IntakeConstants.NO_STRING;
            }
        }

        /// <summary>
        /// Handles CbMedicareEntitledIndicatorB_SelectedIndexChanged events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbMedicareEntitledIndicatorB_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckPartBMandatory();
        }

        /// <summary>
        /// Handles CbEnrolled_SelectedIndexChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbEnrolled_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cb = sender as ASPxComboBox;
            if (cb.SelectedItem.Text == IntakeConstants.YES_STRING)
            {
                fvExpense_MedicareExpensePartDDetail.FindControl("cbMedicareEntitlementVeificatonCode2").Bind<ExpenseMetaDataRequired>(b => b.MedicareInformationVerifiedBy);
            }
            else
            {
                fvExpense_MedicareExpensePartDDetail.FindControl("cbMedicareEntitlementVeificatonCode2").Bind<ExpenseMetaDataNotRequired>(b => b.MedicareInformationVerifiedBy);
            }
        }

        #endregion


        #region Conditional Validation for Part A and B Premium
        /// <summary>
        /// Handles TxtMedicarePremiumAmountA_TextChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TxtMedicarePremiumAmountA_TextChanged(object sender, EventArgs e)
        {
            BindMedicarePreminumPaidBy();
        }

        private void BindMedicarePreminumPaidBy()
        {
            var txtMedicarePremiumAmountA = (ASPxTextBox)fvExpense_MedicareExpensePartABDetailA.FindControl("txtMedicarePremiumAmountA");

            if (txtMedicarePremiumAmountA != null && txtMedicarePremiumAmountA.Text == "")
            {
                txtMedicarePremiumAmountA.Text = "0.00";
            }


            if ((txtMedicarePremiumAmountA != null && Convert.ToDecimal(txtMedicarePremiumAmountA.Text) > 0))
            {
                fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicarePaidByCodeA").Bind<ExpenseMetaDataRequired>(b => b.MedicarePaidByCode);
            }
            else
            {
                fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicarePaidByCodeA").Bind<ExpenseMetaDataNotRequired>(b => b.MedicarePaidByCode);
            }
        }

        private void BindMedicarePreminumPartBPaidBy()
        {
            var txtMedicarePremiumAmountB = (ASPxTextBox)fvExpense_MedicareExpensePartABDetailB.FindControl("txtMedicarePremiumAmountB");

            if ((txtMedicarePremiumAmountB != null && Convert.ToDecimal(txtMedicarePremiumAmountB.Text) > 0))
            {
                fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicarePaidByCodeB").Bind<ExpenseMetaDataRequired>(b => b.MedicarePaidByCode);
            }
            else
            {
                fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicarePaidByCodeB").Bind<ExpenseMetaDataNotRequired>(b => b.MedicarePaidByCode);
            }
        }

        /// <summary>
        /// Handles TxtMedicarePremiumAmountB_TextChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TxtMedicarePremiumAmountB_TextChanged(object sender, EventArgs e)
        {
            BindMedicarePreminumPartBPaidBy();
        }

        #endregion

        /// <summary>
        /// Handles CbMedicareEntitlementVeificatonCodeA_SelectedIndexChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbMedicareEntitlementVeificatonCodeA_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckPending("cbMedicareEntitlementVeificatonCodeA", "dtEndDate", fvExpense_MedicareExpensePartABDetailA);
        }

        /// <summary>
        /// Handles CbMedicareEntitlementVeificatonCodeB_SelectedIndexChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbMedicareEntitlementVeificatonCodeB_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckPending("cbMedicareEntitlementVeificatonCodeB", "dtEndDate1", fvExpense_MedicareExpensePartABDetailB);
        }

        /// <summary>
        /// Handles CbMedicareEntitlementVeificatonCode2_SelectedIndexChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbMedicareEntitlementVeificatonCode2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckPending("cbMedicareEntitlementVeificatonCode2", "dtEndDate2", fvExpense_MedicareExpensePartDDetail);
        }

        /// <summary>
        /// CheckPending
        /// </summary>
        /// <param name="cmbVerifyBy"></param>
        /// <param name="ddeEndDate"></param>
        /// <param name="mFormview"></param>
        protected void CheckPending(string cmbVerifyBy, string ddeEndDate, FormView mFormview)
        {
            var mVerifiedByB = (ASPxComboBox)mFormview.FindControl(cmbVerifyBy);
            var mDdeEndDateB = (ASPxDateEdit)mFormview.FindControl(ddeEndDate);
            mDdeEndDateB.Enabled = mVerifiedByB == null || mVerifiedByB.SelectedIndex == -1 ||
                                   mVerifiedByB.SelectedItem.Text !=
                                   ErrorMessages.MedicareInformation_Pending_Verification;
        }

        /// <summary>
        /// Handles CbName_SelectedIndexChanged events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CbName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsCheckPrimary(ExpenseBusinessLogicConstants.MEDICARE_INFORMATION))
            {
                VenemConditions();
                var cb = sender as ASPxComboBox;
                Master.CurrentPersonSelectedId = Convert.ToInt32(cb.Value);
            }
        }

        #region BeginDate/EndDate criteria

        /// <summary>
        /// Handles DtMedicareStartDate_DateChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DtMedicareStartDate_DateChanged(object sender, EventArgs e)
        {
            //Part A
            var begindateA = sender as ASPxDateEdit;
            var enddateA = (ASPxDateEdit)fvExpense_MedicareExpensePartABDetailA.FindControl("dtEndDate");
            EndDateSetting(begindateA, enddateA);
        }

        /// <summary>
        /// Handles DtMedicareStartDate1_DateChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DtMedicareStartDate1_DateChanged(object sender, EventArgs e)
        {
            //Part B
            var begindateB = sender as ASPxDateEdit;
            var enddateB = (ASPxDateEdit)fvExpense_MedicareExpensePartABDetailB.FindControl("dtEndDate1");
            EndDateSetting(begindateB, enddateB);
        }

        /// <summary>
        /// Handles DtMedicareStartDate2_DateChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DtMedicareStartDate2_DateChanged(object sender, EventArgs e)
        {
            //Part D
            //var begindateD = sender as ASPxDateEdit;
            //var enddateD = (ASPxDateEdit)fvExpense_MedicareExpensePartDDetail.FindControl("dtEndDate2");
            //EndDateSetting(begindateD, enddateD);
            DateTime startdate = ((ASPxDateEdit)fvExpense_MedicareExpensePartDDetail.FindControl("dtMedicareStartDate2")).Date;
            if (startdate < Convert.ToDateTime("1/1/2006"))
            {
                ShowPopup("The Part D Starting date should be greater than or equal to 01/01/2006");
            }
        }

        #endregion

        #region "VENEM"
        /// <summary>
        /// Venem Conditions
        /// Edited date:11/8/2016
        /// Author: Harish
        /// Change: While sending the request to Venum service Pad left SSN with zeros if lenght is less than 9
        /// </summary>
        public void VenemConditions()
        {
            var expenseContext = new ExpenseContextImpl();
            var cbName = (ASPxComboBox)fvExpense_MedicareExpense.FindControl("cbName");
            var dtStartDate = (ASPxDateEdit)fvExpense_MedicareExpensePartABDetailA.FindControl("dtMedicareStartDate");
            var dtEndDate = (ASPxDateEdit)fvExpense_MedicareExpensePartABDetailA.FindControl("dtEndDate");
            var cmbVerifiedByA = (ASPxComboBox)fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicareEntitlementVeificatonCodeA");
            var person = expenseContext.Expense_Person.Expand("PersonAdditionalAttributes").Where(n => n.PersonID == Convert.ToInt32(cbName.Value)).First();
            Expense_HouseholdGeneralInfo medReceiveIndc = expenseContext.Expense_HouseholdGeneralInfo.Where(n => n.ApplicationID == int.Parse(WorkflowSession.Instance.RootFrame.State.Key)).First();
            Regex rLn = new Regex(@"^[a-zA-Z\s\-]{1,21}$");

            if (!rLn.IsMatch(person.FirstName) || !rLn.IsMatch(person.LastName))
            {
                ShowErrPopupAlert(IntakeResourceManager.FEDERAL_SERVICE_CALL_ERROR, IntakeConstants.VALIDATION);
                return;
            }

            if (person.PersonAdditionalAttributes.SocialSecurityNumberNumber != 0)
            {
                var verifyNonEsiMecService = new VerifyNonEsiMecService();
                var verifyNonEsiMecRequest = new VerifyNonEsiMecRequest();
                var verifyNonEsiMecRequestIndividualRequest = new VerifyNonEsiMecRequestIndividualRequest
                {
                    Applicant = new VerifyNonEsiMecRequestIndividualRequestApplicant
                    {
                        Person = new VerifyNonEsiMecRequestIndividualRequestApplicantPerson
                        {
                            GivenName = Convert.ToString(person.FirstName),
                            MiddleName = Convert.ToString(person.MiddleName),
                            SurName = Convert.ToString(person.LastName),
                            BirthDate = Convert.ToDateTime(person.PersonAdditionalAttributes.DateOfBirth),
                            SSNIdentification = (Convert.ToString(person.PersonAdditionalAttributes.SocialSecurityNumberNumber)).PadLeft(9, '0')
                        }
                    },
                    InsurancePolicyEffectiveDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    InsurancePolicyExpirationDate = new DateTime(DateTime.Now.Year + 1, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year + 1, DateTime.Now.Month))
                };
                var indvarray = new VerifyNonEsiMecRequestIndividualRequest[1];
                indvarray[0] = verifyNonEsiMecRequestIndividualRequest;
                verifyNonEsiMecRequest.VerifyNonEsiMecRequest1 = indvarray;
                string vnmResponse = null;
                try
                {
                    var venemResponse = verifyNonEsiMecService.VenemService(verifyNonEsiMecRequest, Convert.ToString(cmbVerifiedByA.Value));
                    Infrastructure.Context.ExpenseSessionContext.Instance.MedicareInformationVerifiedBy = true;
                    #region Date Change acoording to VENEM response
                    if (venemResponse != null)
                    {
                        vnmResponse = venemResponse.OtherCoverage.MECCoverage.MECVerificationCode;
                    }
                    if (medReceiveIndc.ReceiveMedicareBenefitIndicator == IntakeConstants.YES_CODE && vnmResponse == IntakeConstants.YES_CODE)
                    {
                        cmbVerifiedByA.SelectedIndex = cmbVerifiedByA.Items.FindByValue(IntakeConstants.FP).Index;
                        dtStartDate.Value = venemResponse.OtherCoverage.MECCoverage.Insurance.First().InsuranceEffectiveDate;
                        dtEndDate.Value = venemResponse.OtherCoverage.MECCoverage.Insurance.First().InsuranceEndDate;
                    }
                    else if (medReceiveIndc.ReceiveMedicareBenefitIndicator == IntakeConstants.NO_CODE && vnmResponse == IntakeConstants.YES_CODE)
                    {
                        cmbVerifiedByA.SelectedIndex = cmbVerifiedByA.Items.FindByValue(IntakeConstants.QUESTION).Index;
                    }
                    else if (medReceiveIndc.ReceiveMedicareBenefitIndicator == IntakeConstants.YES_CODE && vnmResponse == IntakeConstants.NO_CODE)
                    {
                        cmbVerifiedByA.SelectedIndex = cmbVerifiedByA.Items.FindByValue(IntakeConstants.QUESTION).Index;
                    }
                    else if (medReceiveIndc.ReceiveMedicareBenefitIndicator == IntakeConstants.NO_CODE && vnmResponse == IntakeConstants.NO_CODE)
                    {
                        cmbVerifiedByA.SelectedIndex = cmbVerifiedByA.Items.FindByValue(IntakeConstants.FP).Index;
                    }
                    else if (medReceiveIndc.ReceiveMedicareBenefitIndicator == IntakeConstants.YES_CODE && vnmResponse == PVALUE)
                    {
                        cmbVerifiedByA.SelectedIndex = cmbVerifiedByA.Items.FindByValue(IntakeConstants.FP).Index;
                        dtStartDate.Value = venemResponse.OtherCoverage.MECCoverage.Insurance.First().InsuranceEffectiveDate;
                        dtEndDate.Value = venemResponse.OtherCoverage.MECCoverage.Insurance.First().InsuranceEndDate;
                    }
                    else if (medReceiveIndc.ReceiveMedicareBenefitIndicator == IntakeConstants.NO_CODE && vnmResponse == PVALUE)
                    {
                        cmbVerifiedByA.SelectedIndex = cmbVerifiedByA.Items.FindByValue(IntakeConstants.QUESTION).Index;
                    }
                    #endregion
                }
                catch (Exception)
                {
                    ShowErrPopupAlert(IntakeResourceManager.VENUM_SERVICE_ERROR, IntakeConstants.ERROR);
                }
            }
            else
            {
                cmbVerifiedByA.SelectedIndex = cmbVerifiedByA.Items.FindByValue(IntakeConstants.QUESTION).Index;
            }
        }
        #endregion

        /// <summary>
        /// Handles BtnOops_Click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnOops_Click(object sender, EventArgs e)
        {
            CallOops(true);
        }

        /// <summary>
        /// CallOops
        /// </summary>
        /// <param name="service"></param>
        protected void CallOops(bool service)
        {

            if (ExpenseEntitiesCreation.DeleteMedicareExpenseRecord(AnchorObject.ExpenseID, false))
            {
                UnSchedulePage(IntakeConstants.MEDICARE_INFORMATION_SUMMARY);
                UnSchedulePage(IntakeConstants.MEDICARE_INFORMATION);
            }
            if (service)
            {
                ExpenseDataContext.FlipExpenseQuestion(ExpenseDataContext.ExpesneType.MDI);
                base.NavigateNext();
            }
            else
            {
                base.NavigatePrevious(n => n.Visible && n.Enabled && n.DetailScreen == false && n.Name != IntakeConstants.MEDICARE_INFORMATION_SUMMARY);
            }

        }

        /// <summary>
        /// Handles DsExpense_MedicareExpense_Selecting event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DsExpense_MedicareExpense_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var ctx = new ExpenseContextImpl();
            e.Result = ctx.Expense_MedicareExpense.Where(n => n.ExpenseID == _expenseId);
        }

        /// <summary>
        /// Handles DsExpense_MedicareExpensePartABDetailA_Selecting event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DsExpense_MedicareExpensePartABDetailA_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var ctx = new ExpenseContextImpl();
            e.Result = ctx.Expense_MedicareExpensePartABDetail.Where(n => n.ExpenseID == _expenseId && n.MedicareTypeCode == IntakeConstants.MEDICARE_TYPE_CODE_A);
        }

        /// <summary>
        /// Handles DsExpense_MedicareExpensePartABDetailB_Selecting event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DsExpense_MedicareExpensePartABDetailB_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var ctx = new ExpenseContextImpl();
            e.Result = ctx.Expense_MedicareExpensePartABDetail.Where(n => n.ExpenseID == _expenseId && n.MedicareTypeCode == IntakeConstants.MEDICARE_TYPE_CODE_B);
        }

        /// <summary>
        /// Handles DsExpense_MedicareExpensePartDDetail_Selecting event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DsExpense_MedicareExpensePartDDetail_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var ctx = new ExpenseContextImpl();
            e.Result = ctx.Expense_MedicareExpensePartDDetail.Where(n => n.ExpenseID == _expenseId);
        }

        /// <summary>
        /// Handles fvExpense_MedicareExpense_ItemUpdating event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void fvExpense_MedicareExpense_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            var hdMedicareBuyingEffectiveDate = (HiddenField)fvExpense_MedicareExpense.FindControl("hdMedicareBuyingEffectiveDate");
            if (!ExpenseSessionContext.Instance.IsExpense_BackToSummary_Clicked)
            {
                //Date validations
                if (!IsBeginEndDateValid())
                {
                    e.Cancel = true;
                    return;
                }
                var beginDate = (ASPxDateEdit)fvExpense_MedicareExpense.FindControl("ddeBeginDate");
                var enddate = (ASPxDateEdit)fvExpense_MedicareExpense.FindControl("ddeEndDate");
                EndDateSetting(beginDate, enddate);
                var cmbHistoryReason = (ASPxComboBox)fvExpense_MedicareExpense.FindControl("cbDeleteReasonCode");
                //var ctx = new ExpenseContextImpl();
                //var medicareExpenseObejct = ctx.Expense_MedicareExpense.Where(n => n.ExpenseID == AnchorObject.ExpenseID).First();
                if (ExpenseEntitiesCreation.SetRetroOrShowMessage(Convert.ToDateTime(e.OldValues["BeginDate"]), beginDate.Date, Convert.ToString(cmbHistoryReason.Text)))
                {
                    beginDate.Date = Convert.ToDateTime(e.OldValues["BeginDate"]);
                    cmbHistoryReason.Text = string.Empty;
                    _isValidated = false;
                    fvExpense_MedicareExpense.FindControl("ddeEndDate").Bind<ExpenseMetaDataNotRequired>(b => b.EndDate);
                    e.Cancel = true;
                    ShowErrPopupAlert(IntakeResourceManager.RETRO_ADDITION_NOT_ALLOWED_HISTORY_REASON_CODE_EXIST, IntakeResourceManager.RETRO_ERROR);
                    return;
                }
                if (ExpenseEntitiesCreation.ValidateAdminErrorDeleteReasonCode(beginDate.Date, enddate.Date, Convert.ToString(cmbHistoryReason.Value)))
                {
                    enddate.Text = string.Empty;
                    _isValidated = false;
                    e.Cancel = true;
                    ShowErrPopupAlert(IntakeResourceManager.ENDED_RECORDS_BEGIN_AND_END_DATES_SAME, IntakeResourceManager.BEGIN_DATE_ERROR);
                    return;
                }

                if (!string.IsNullOrEmpty(e.OldValues["BeginDate"].AsString()) && CommonUtility.IsRetro(Convert.ToDateTime(e.OldValues["BeginDate"]), Convert.ToDateTime(e.NewValues["BeginDate"])))
                {
                    List<ASPxComboBox> verifiedbyCombos = new List<ASPxComboBox> {
                    (ASPxComboBox)fvExpense_MedicareExpensePartABDetailA.FindControl("cbMedicareEntitlementVeificatonCodeA"),
                    (ASPxComboBox)fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitlementVeificatonCodeB"),
                    (ASPxComboBox)fvExpense_MedicareExpensePartDDetail.FindControl("cbMedicareEntitlementVeificatonCode2")
                    };
                    if (CommonUtility.CheckPendingVerificationList(verifiedbyCombos))
                    {
                        e.Cancel = true;
                        _isValidated = false;
                        ShowErrPopupAlert(IntakeResourceManager.PENDINGVERIFICATION_RETRO, IntakeResourceManager.RETRO_ERROR);
                        return;
                    }
                }

                if ((enddate != null && enddate.Text != string.Empty) || (cmbHistoryReason != null && cmbHistoryReason.SelectedIndex > 0))
                {
                    fvExpense_MedicareExpense.FindControl("ddeEndDate").Bind<ExpenseMetaDataRequired>(b => b.EndDate);
                }

                if (CurrentWorkflowPage.Completed)
                {
                    bool status = string.Equals(Convert.ToString(e.OldValues["MedicareBuyingEffectiveDate"]), Convert.ToString(e.NewValues["MedicareBuyingEffectiveDate"]));
                    SetPageComplete(status);
                    if (status)
                        SetPageComplete(!IncomeContextOperations.IsCollectionDifferent((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues));
                }
            }
            else
            {

                if (!Convert.ToBoolean(ExpenseSessionContext.Instance.IsExpense_Datachanged))
                {
                    ExpenseSessionContext.Instance.IsExpense_Datachanged = IncomeContextOperations.IsCollectionDifferent((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues);
                }
            }
            e.NewValues["MedicareBuyingEffectiveDate"] = dtBuyInEffectiveDate.Value;
            AssignMedExpenseValuesToRequest(e.NewValues, Convert.ToInt32((sender as FormView).DataKey.Value));
            //e.Cancel = CurrentWorkflowPage.Completed;
            e.Cancel = true;
        }

        /// <summary>
        /// Assign Med Expense Values ToRequest
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyValue"></param>
        private void AssignMedExpenseValuesToRequest(IOrderedDictionary source, int keyValue)
        {
            var medExpense = new Expense_MedicareExpense() { Expense = new Expense_Expense() { PersonID = Master.CurrentPersonSelectedId } };
            source.CopyValuesTo(medExpense);
            medExpense.ExpenseID = keyValue;
            UpdateServiceRequest = medExpense;
        }

        /// <summary>
        /// fvExpense_MedicareExpensePartABDetailA_ItemUpdating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void fvExpense_MedicareExpensePartABDetailA_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            if (CurrentWorkflowPage.Completed)
            {
                SetPageComplete(ExpenseEntitiesCreation.IsCollectionSame((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues));
            }
            if (_isValidated)
            {
                AssignMedExpensePartADetailValuesToRequest(e.NewValues, Convert.ToInt32((sender as FormView).DataKey.Value));
            }
            e.Cancel = true;
            //e.Cancel = CurrentWorkflowPage.Completed; 
        }

        /// <summary>
        /// Assign Med Expense PartA Detail Values ToRequest
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyValue"></param>
        private void AssignMedExpensePartADetailValuesToRequest(IOrderedDictionary source, int keyValue)
        {
            var medExpensePartA = new Expense_MedicareExpensePartABDetail();
            source.CopyValuesTo(medExpensePartA);
            medExpensePartA.ExpenseID = keyValue;
            HiddenField hdMedicareIdPartA = (HiddenField)fvExpense_MedicareExpensePartABDetailA.FindControl("hdMedicareIdPartA");
            medExpensePartA.MedicareExpenseDtlID = Convert.ToInt32(hdMedicareIdPartA.Value);
            medExpensePartA.MedicareTypeCode = "A";
            if (UpdateServiceRequest.MedicareExpensePartABDetail == null)
            {
                UpdateServiceRequest.MedicareExpensePartABDetail = new List<Expense_MedicareExpensePartABDetail>();
            }
            UpdateServiceRequest.MedicareExpensePartABDetail.Add(medExpensePartA);
        }

        /// <summary>
        /// Handles fvExpense_MedicareExpensePartABDetailB_ItemUpdating event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void fvExpense_MedicareExpensePartABDetailB_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            if (!ExpenseSessionContext.Instance.IsExpense_BackToSummary_Clicked)
            {
                if (CurrentWorkflowPage.Completed)
                {
                    SetPageComplete(!IncomeContextOperations.IsCollectionDifferent((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues));
                }
            }
            else
            {
                if (!Convert.ToBoolean(ExpenseSessionContext.Instance.IsExpense_Datachanged))
                {
                    ExpenseSessionContext.Instance.IsExpense_Datachanged = IncomeContextOperations.IsCollectionDifferent((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues);
                }
            }
            if (_isValidated)
            {
                AssignMedExpensePartBDetailValuesToRequest(e.NewValues, Convert.ToInt32((sender as FormView).DataKey.Value));
            }
            e.Cancel = true;
            //e.Cancel = CurrentWorkflowPage.Completed;
        }

        /// <summary>
        /// Assign Med Expense PartB Detail Values ToRequest
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyValue"></param>
        private void AssignMedExpensePartBDetailValuesToRequest(IOrderedDictionary source, int keyValue)
        {
            var medExpensePartB = new Expense_MedicareExpensePartABDetail();
            source.CopyValuesTo(medExpensePartB);
            medExpensePartB.ExpenseID = keyValue;
            HiddenField hdMedicareIdPartB = (HiddenField)fvExpense_MedicareExpensePartABDetailB.FindControl("hdMedicareIdPartB");
            medExpensePartB.MedicareExpenseDtlID = Convert.ToInt32(hdMedicareIdPartB.Value);
            medExpensePartB.MedicareTypeCode = "B";
            if (UpdateServiceRequest.MedicareExpensePartABDetail == null)
            {
                UpdateServiceRequest.MedicareExpensePartABDetail = new List<Expense_MedicareExpensePartABDetail>();
            }
            UpdateServiceRequest.MedicareExpensePartABDetail.Add(medExpensePartB);
        }
        /// <summary>
        /// Handles fvExpense_MedicareExpensePartDDetail_ItemUpdating event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void fvExpense_MedicareExpensePartDDetail_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            if (!ExpenseSessionContext.Instance.IsExpense_BackToSummary_Clicked)
            {
                if (CurrentWorkflowPage.Completed)
                {
                    SetPageComplete(!IncomeContextOperations.IsCollectionDifferent((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues));
                }
            }
            else
            {
                if (!Convert.ToBoolean(ExpenseSessionContext.Instance.IsExpense_Datachanged))
                {
                    ExpenseSessionContext.Instance.IsExpense_Datachanged = IncomeContextOperations.IsCollectionDifferent((OrderedDictionary)e.OldValues, (OrderedDictionary)e.NewValues);
                }
            }
            if (_isValidated)
            {
                AssignMedExpensePartDDetailValuesToRequest(e.NewValues, Convert.ToInt32((sender as FormView).DataKey.Value));
            }
            e.Cancel = true;
            //e.Cancel = CurrentWorkflowPage.Completed;
        }

        /// <summary>
        /// Assign Med Expense PartD Detail Values ToRequest
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keyValue"></param>
        private void AssignMedExpensePartDDetailValuesToRequest(IOrderedDictionary source, int keyValue)
        {
            var medExpensePartD = new Expense_MedicareExpensePartDDetail();
            source.CopyValuesTo(medExpensePartD);
            medExpensePartD.ExpenseID = keyValue;
            UpdateServiceRequest.MedicareExpensePartDDetail = medExpensePartD;
        }
        /// <summary>
        /// Handles BtnBackToSummary_Click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnBackToSummary_Click(object sender, EventArgs e)
        {
            Infrastructure.Context.ExpenseSessionContext.Instance.IsBtnPreviousCalled = false;
            ExpenseSessionContext.Instance.IsExpense_BackToSummary_Clicked = true;
            fvExpense_MedicareExpense.UpdateItem(false);
            fvExpense_MedicareExpensePartABDetailB.UpdateItem(false);
            fvExpense_MedicareExpensePartDDetail.UpdateItem(false);
            if (ExpenseSessionContext.Instance.IsExpense_Datachanged)
            {
                ExpenseSessionContext.Instance.IsExpense_BackToSummary_Clicked = false;
                ShowPopupInfo(IntakeResourceManager.SAVE_CHAGNES_ALERT);
            }
            else
            {
                ExpenseSessionContext.Instance.IsExpense_BackToSummary_Clicked = false;
                NavigatePrevious(n => n.Name == IntakeConstants.MEDICARE_INFORMATION_SUMMARY);
            }
            ExpenseSessionContext.Instance.IsExpense_BackToSummary_Clicked = false;
        }

        /// <summary>
        /// Shows PopupInfo.
        /// </summary>
        /// <param name="message"></param>
        private void ShowPopupInfo(string message)
        {
            dxPopupError.ShowOnPageLoad = true;
            var lblmessage = (ASPxLabel)dxPopupError.FindControl("lblmessage1");
            lblmessage.Text = message;
        }

        private void ShowPopup(string message)
        {
            dxPopupErr.ShowOnPageLoad = true;
            dxPopupErr.HeaderText = "Invalid Date";
            var lblmessage = (ASPxLabel)dxPopupErr.FindControl("lblErrmessage");
            lblmessage.Text = message;
            var btnok = (ASPxButton)dxPopupErr.FindControl("btnok");
            btnok.Focus();
        }

        /// <summary>
        /// Handles BtnPopUpYes_Click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnPopUpYes_Click(object sender, EventArgs e)
        {
            var ctx = new ExpenseContextImpl();
            dxPopupError.ShowOnPageLoad = false;
            Infrastructure.Context.ExpenseSessionContext.Instance.IsAddNew = false;
            ExpenseSessionContext.Instance.IsExpense_Datachanged = false;
            Infrastructure.Context.ExpenseSessionContext.Instance.MedicareExpRecs = 0;

            if (btnOops.Enabled)
                CallOops(false);
            else
            {
                if (AnchorObject.SyncState == null)
                {
                    var medicareExpensePartAbDetailObejct = ctx.Expense_MedicareExpensePartABDetail.Where(n => n.MedicareExpense.ExpenseID == AnchorObject.ExpenseID);
                    var medicareExpensePartDDetailObejct = ctx.Expense_MedicareExpensePartDDetail.Where(n => n.MedicareExpense.ExpenseID == AnchorObject.ExpenseID);
                    var medicareExpenseObejct = ctx.Expense_MedicareExpense.Where(n => n.ExpenseID == AnchorObject.ExpenseID);
                    var expenseObejct = ctx.Expense_Expense.Where(n => n.ExpenseID == AnchorObject.ExpenseID);
                    if (medicareExpensePartAbDetailObejct.Count() > 0)
                    {
                        foreach (var obj in medicareExpensePartAbDetailObejct)
                        {
                            ctx.DeleteObject(obj);
                        }
                    }
                    if (medicareExpensePartDDetailObejct.Count() > 0)
                    {
                        foreach (var obj in medicareExpensePartDDetailObejct)
                        {
                            ctx.DeleteObject(obj);
                        }
                    }
                    ctx.DeleteObject(medicareExpenseObejct.First());
                    ctx.DeleteObject(expenseObejct.First());
                    ctx.SaveChanges();
                    UnSchedulePageEntity(IntakeConstants.MEDICARE_INFORMATION, AnchorObject.ExpenseID.ToString());
                }
                SetCurrPrevPages();
            }
        }
        /// <summary>
        /// set current and previous pages
        /// </summary>
        private void SetCurrPrevPages()
        {
            if (CurrentWorkflowPage.Context.Value.IsContextComplete())
            {
                SetPreviousPageComplete(true);
            }
            if (Infrastructure.Context.ExpenseSessionContext.Instance.IsBtnPreviousCalled == false)
            {
                NavigatePrevious(n => n.Name == IntakeConstants.MEDICARE_INFORMATION_SUMMARY);
            }
            else
            {
                base.NavigatePrevious(n => n.Visible && n.Enabled && n.DetailScreen == false && n.Name != IntakeConstants.MEDICARE_INFORMATION_SUMMARY);
            }
        }

        /// <summary>
        /// Handles BtnPopUpNo_Click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnPopUpNo_Click(object sender, EventArgs e)
        {
            dxPopupError.ShowOnPageLoad = false;
            ExpenseSessionContext.Instance.IsExpense_Datachanged = false;
        }

        /// <summary>
        /// NavigatePrevious
        /// </summary>
        public override void NavigatePrevious()
        {
            Infrastructure.Context.ExpenseSessionContext.Instance.IsBtnPreviousCalled = true;
            if (!CurrentWorkflowPage.Completed) { ShowPopupInfo(IntakeResourceManager.SAVE_CHAGNES_ALERT); } else { base.NavigatePrevious(n => n.Visible && n.Enabled && n.DetailScreen == false && n.Name != IntakeConstants.MEDICARE_INFORMATION_SUMMARY); }
        }


        /// <summary>
        /// Handles txtMedicareNumber_ValueChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtMedicareNumber_ValueChanged(object sender, EventArgs e)
        {
            var hdMedicareNumber = (HiddenField)fvExpense_MedicareExpense.FindControl("hdMedicareNumber");
            hdMedicareNumber.Value = txtMedicareNumber.Text;
        }

        /// <summary>
        /// This is to Check if PartB Mandatory.
        /// </summary>
        protected void CheckPartBMandatory()
        {
            if (fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitledIndicatorB") != null)
            {
                var cb = (ASPxComboBox)fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitledIndicatorB");
                if (cb.SelectedItem != null)
                {

                    if (cb.SelectedItem.Text == IntakeConstants.YES_STRING)
                    {
                        fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitlementVeificatonCodeB").Bind<ExpenseMetaDataRequired>(x => x.MedicareInformationVerifiedBy);
                        //fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicarePaidByCodeB").Bind<ExpenseMetaDataRequired>(x => x.MedicarePaidByCode);
                        fvExpense_MedicareExpensePartABDetailB.FindControl("txtMedicarePremiumAmountB").Bind<ExpenseMetaDataRequired>(b => b.Premium);
                    }
                    else
                    {
                        fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicareEntitlementVeificatonCodeB").Bind<ExpenseMetaDataNotRequired>(x => x.MedicareInformationVerifiedBy);
                        // fvExpense_MedicareExpensePartABDetailB.FindControl("cbMedicarePaidByCodeB").Bind<ExpenseMetaDataNotRequired>(x => x.MedicarePaidByCode);
                        fvExpense_MedicareExpensePartABDetailB.FindControl("txtMedicarePremiumAmountB").Bind<ExpenseMetaDataNotRequired>(b => b.Premium);
                    }
                }
            }
        }
        /// <summary>
        /// Attaching Javascript based validation for Conditional validations
        /// </summary>
        private void ApplyConditionalValidation()
        {
            var fv = fvExpense_MedicareExpensePartABDetailB;
            //Premium Amount
            var cbMedicareEntitledIndicatorB = fv.FindControl("cbMedicareEntitledIndicatorB").As<ASPxEdit>();
            var txtMedicarePremiumAmountB = fv.FindControl("txtMedicarePremiumAmountB").As<ASPxEdit>();
            var lblMedicareEntitledIndicatorB = fv.FindControl("lblMedicareEntitledIndicatorB").As<ASPxLabel>();
            var lblPremiumB = fv.FindControl("lblPremiumB").As<ASPxLabel>();

            ConditionalJavaScript.ConditionalValidation(this, cbMedicareEntitledIndicatorB, txtMedicarePremiumAmountB, lblMedicareEntitledIndicatorB, lblPremiumB, true, "1");


            //Paid by
            var cbMedicarePaidByCodeB = fv.FindControl("cbMedicarePaidByCodeB").As<ASPxEdit>();
            var lblMedicarePaidByCodeB = fv.FindControl("lblMedicarePaidByCodeB").As<ASPxLabel>();


            ConditionalJavaScript.ConditionalValidation(this, cbMedicareEntitledIndicatorB, cbMedicarePaidByCodeB, lblMedicareEntitledIndicatorB, lblMedicarePaidByCodeB, true, "1");

            //History code validation
            var cbDeleteReasonCode = fvExpense_MedicareExpense.FindControl("cbDeleteReasonCode").As<ASPxEdit>();
            var ddeEndDate = fvExpense_MedicareExpense.FindControl("ddeEndDate").As<ASPxEdit>();
            var lblEndDate = fvExpense_MedicareExpense.FindControl("lblEndDate").As<ASPxLabel>();
            if (cbDeleteReasonCode.IsEnabled())
            {
                ConditionalJavaScript.ConditionalValidation(this,
                    cbDeleteReasonCode,
                    ddeEndDate,
                    lblEndDate,
                    IntakeResourceManager.END_DATE_MANDATORY,
                    false,
                    null);
            }
        }

        protected void btnNo_Click(object sender, EventArgs e)
        {
            dxPopupErrorBuyInDate.ShowOnPageLoad = false;
            dtBuyInEffectiveDate.Value = string.Empty;
            var hdMedicareBuyingEffectiveDate = (HiddenField)fvExpense_MedicareExpense.FindControl("hdMedicareBuyingEffectiveDate");
            hdMedicareBuyingEffectiveDate.Value = dtBuyInEffectiveDate.Text;
            dtBuyInEffectiveDate.Focus();
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
                ShowErrPopupAlert(IntakeResourceManager.ERROR_TITLE, Dhss.Assist.WorkerWeb.Entity.SharedErrorMessages.ErrorMessages.WW_DIS_NOACCESS);
                return;
            }
        }

        protected void ddeBeginDate_DataBound(object sender, EventArgs e)
        {
            var maxDate = SystemDateTime.Now.AddMonths(2);
            fvExpense_MedicareExpense.FindControl("ddeBeginDate").As<ASPxDateEdit>().MaxDate = maxDate;
        }

        /// <summary>
        /// Chcks if Begin and End dates are valid
        /// </summary>
        /// <returns></returns>
        protected bool IsBeginEndDateValid()
        {
            bool retVal = true;
            var ddeBeginDate = fvExpense_MedicareExpense.FindControl("ddeBeginDate") as ASPxDateEdit;
            var ddeEndDate = fvExpense_MedicareExpense.FindControl("ddeEndDate") as ASPxDateEdit;
            if (ddeBeginDate != null && ddeEndDate != null && ddeBeginDate.Date != DateTime.MinValue && ddeEndDate.Date != DateTime.MinValue &&
                TechnicalCommon.GetDateWithFirstDayOfMonth(ddeBeginDate.Date) > TechnicalCommon.GetDateWithLastDayOfMonth(ddeEndDate.Date))
            {
                _isValidated = false;
                ShowErrPopupAlert(IntakeConstants.ERROR_BEGINDATAANDENDDATE, IntakeConstants.ERROR_HEADER_INVALIDDATE);
                retVal = false;
            }
            return retVal;
        }

        protected void cbDeleteReasonCode_DataBound(object sender, EventArgs e)
        {
            var historyRsn = (ASPxComboBox)sender;
            MandateEndDate(_resourceBlogic.IsHistoryReasonSelected(historyRsn.SelectedIndex));
        }

        /// <summary>
        /// MandateEndDate
        /// </summary>
        /// <param name="status"></param>
        private void MandateEndDate(bool status)
        {
            if (status)
            {
                fvExpense_MedicareExpense.FindControl("ddeEndDate").Bind<ExpenseMetaDataRequired>(b => b.EndDate);
            }
        }
    }
}
