using DevExpress.Web;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxGridView;
using DevExpress.Xpo;
using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Income;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Entity.DataTypes;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Extensions;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Services;
using Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Income;
using Dhss.Framework;
using Dhss.Framework.Extensions;
using Dhss.Framework.Web.UI.Workflow;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.UI.WebControls;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{
    [Workflow]
    [ExcludeFromCodeCoverage]
    public partial class VolunteeringWorkProgramUnpaidWorkSummary : Dhss.Assist.WorkerWeb.Web.Infrastructure.Workflow.WorkflowPage<Technical_Case>
    {
        private int _applicationId;

        #region Page

        protected void Page_Load(object sender, EventArgs e)
        {
            InitiateSession();

            _applicationId = Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key);
            Master.Master.FooterSectionConfigure(FooterBodyConfiguration.AddnoteSavePreviousNext);

            if (!IsPostBack)
            {
                ClearSessionVariables();
                HistorySearch.LoadHistorySearchSession(ddeBeginDate, ddeEndDate, Request.UrlReferrer, typeof(VolunteeringWorkProgramUnpaidWorkDetails).Name);
                if (!TechnicalSessionContext.Instance.IsVolunteeringWorkProgramBackToSummary)
                    ClearHistoryRecords();
                else
                {
                    ddeBeginDate.Value = TechnicalSessionContext.Instance.BeginDate;
                    ddeEndDate.Value = TechnicalSessionContext.Instance.Enddate;
                }
                ddeBeginDate.Focus();

            }
        }

        #endregion Page

        #region DataSource

        private IEnumerable<Technical_VolunteeringWorkProgram> GetVWPResult()
        {
            int applicationId = Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key);
            bool isHistorySearchMode = TechnicalCommon.IsHistorySearchMode(ddeBeginDate.Value, ddeEndDate.Value);
            var result = isHistorySearchMode
                ? TechnicalContextOperations.GetVolunteeringWorkProgramAllHistoryRecords(applicationId, ddeBeginDate.Value, ddeEndDate.Value)
                : TechnicalContextOperations.GetVolunteeringWorkProgramAllActiveRecords(applicationId);
            return result;
        }
        protected void DsTechnical_VolunteeringWorkProgram_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetVWPResult();
        }

        private List<Technical_VolunteeringWorkProgram> GetResult()
        {
            _applicationId = Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key);
            DateTime? beginDate = ddeBeginDate.Value?.AsDateTime();
            DateTime? endDate = ddeEndDate.Value?.AsDateTime();
            bool isHistorySearch = TechnicalCommon.IsHistorySearchMode(ddeBeginDate.Value, ddeEndDate.Value);
            if (isHistorySearch)
            {
                return ServicesApplicationHub.IntakeTechnical.GetHistoryRecordsVolunteeringWorkProgram(_applicationId, beginDate, endDate);
            }

            return ServicesApplicationHub.IntakeTechnical.GetAllActiveRecordsVolunteeringWorkProgram(_applicationId);
        }

        #endregion DataSource

        #region History Search

        protected void BtnRetrieve_Click(object sender, EventArgs e)
        {

            HistorySearch.SetHistorySearchSession(ddeBeginDate.Value?.AsDateTime(), ddeEndDate.Value?.AsDateTime());
            TechnicalSessionContext.Instance.BeginDate = ddeBeginDate.Value?.AsDateTime();
            TechnicalSessionContext.Instance.Enddate = ddeEndDate.Value?.AsDateTime();
        }

        protected void BtnClear_Click(object sender, EventArgs e)
        {
            ClearHistoryRecords();
        }

        protected void BtnAddNew_Click(object sender, EventArgs e)
        {
            CreateNewVolunteeringWorkProgramRecord();
        }

        protected void CreateNewVolunteeringWorkProgramRecord()
        {
            int applicationId = Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key);
            int primaryPersonId = TechnicalContextOperations.GetPrimaryPersonId(applicationId);

            Technical_VolunteeringWorkProgram record = TechnicalContextOperations.CreateNewVolunteeringWorkProgramRecord(primaryPersonId);
            if (record == null || record.VolunteeringWorkProgramID <= 0)
            {
                showErrPopupAlert("Unable to create a Volunteering / Work Program record.");
                return;
            }
            TechnicalSessionContext.Instance.VolunteeringWorkProgramID = record.VolunteeringWorkProgramID;
            SetPageComplete(false);
            NavigateTo(n => n.Name == IntakeConstants.VOLUNTEERING_WORK_PROGRAM_UNPAID_WORK_AE,
                p => ((Technical_VolunteeringWorkProgram)p).VolunteeringWorkProgramID == record.VolunteeringWorkProgramID);
        }

        private void ClearHistoryRecords()
        {
            ddeBeginDate.Text = string.Empty;
            ddeEndDate.Text = string.Empty;
            ddeBeginDate.Value = null;
            ddeEndDate.Value = null;
            HistorySearch.ClearHistorySearchSession();
            TechnicalSessionContext.Instance.BeginDate = null;
            TechnicalSessionContext.Instance.Enddate = null;
        }

        #endregion History Search

        #region Private Methods

        protected void BtnShowDetails_Click(object sender, EventArgs e)
        {
            if (gvASPxGridView.FocusedRowIndex != -1)
            {
                int volunteeringWorkProgramID = Convert.ToInt32(gvASPxGridView.GetRowValues(gvASPxGridView.FocusedRowIndex, "VolunteeringWorkProgramID"));
                TechnicalSessionContext.Instance.VolunteeringWorkProgramID = volunteeringWorkProgramID;
                NavigateTo(n => n.Name == IntakeConstants.VOLUNTEERING_WORK_PROGRAM_UNPAID_WORK_AE);
                // NavigateTo(n => n.Name == IntakeConstants.VOLUNTEERING_WORK_PROGRAM_UNPAID_WORK_AE, p => ((Technical_VolunteeringWorkProgramDetails)p).VolunteeringWorkProgramID == volunteeringWorkProgramID);
            }
        }

        private void NavigateToRecord(int recordId)
        {
            if (recordId <= 0)
            {
                return;
            }
            SetPageComplete(false);
            NavigateTo(n => n.Name == IntakeConstants.VOLUNTEERING_WORK_PROGRAM_UNPAID_WORK_AE,
                context =>
                {
                    var record = context as Technical_VolunteeringWorkProgram;
                    return record != null && record.VolunteeringWorkProgramID == recordId;
                });
        }

        protected void GvASPxGridView_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            int recordId;
            if (int.TryParse(e.Parameters, out recordId))
            {
                NavigateToRecord(recordId);
            }
        }

        protected void GvASPxGridView_DataBound(object sender, EventArgs e)
        {
            gvASPxGridView.Bind<VolunteeringWorkProgramSummaryMetaData>(b => b.ProgramTypeCode);
        }

        #endregion Private Methods

        #region Navigation

        public override void NavigateNext()
        {
            NavigateToNextPage();
        }

        private void NavigateToNextPage()
        {
            int recordId = GetIncompleteFirstRecord();
            if (recordId != 0)
            {
                NavigateToRecord(recordId);
            }
            else if (TechnicalContextOperations.IsCaseRenewalOrReactivate() && !CurrentWorkflowPage.Completed && gvASPxGridView.VisibleRowCount > 0)
            {
                SetPageComplete();
                base.NavigateNext();
            }
            else
            {
                if (!CurrentWorkflowPage.Completed && !IntakeContext.Instance.IsRenewal) base.NavigateNext();
                SetPageComplete();
                base.NavigateNext();
            }
        }

        private void showErrPopupAlert(string stralertmsg)
        {
            dxPopupErr.ShowOnPageLoad = true;
            ((ASPxLabel)dxPopupErr.FindControl("lblErrmessage")).Text = stralertmsg;
            var btnok = (ASPxButton)dxPopupErr.FindControl("btnok");
            btnok.Focus();
        }

        private int GetIncompleteFirstRecord()
        {
            if (gvASPxGridView.FocusedRowIndex != -1)
            {
                return Convert.ToInt32(gvASPxGridView.GetRowValues(gvASPxGridView.FocusedRowIndex, "VolunteeringWorkProgramID"));
            }
            return 0;
        }

        #endregion Navigation
    }
}