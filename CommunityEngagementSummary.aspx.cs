using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxGridView;
using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Extensions;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Services;
using Dhss.Framework.DataAnnotations;
using Dhss.Framework.Web.UI.Workflow;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.UI.WebControls;


namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{
    [Workflow]
    [ExcludeFromCodeCoverage]
    public partial class CommunityEngagementSummary : Infrastructure.Workflow.WorkflowPage<Technical_CommunityEngagementSummary>
    {
        public partial class TechnicalCommunityEngagementSummaryMetaData
        {

            /// <summary>
            /// Gets or Sets the PersonId.
            /// </summary>
            /// <value>The PersonId.</value>
            [LookupTable(typeof(PersonNameWithPersonId))]
            public string PersonID { get; set; }
        }
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Master.FooterSectionConfigure(FooterBodyConfiguration.AddnotePreviousNext);
            if (!IsPostBack)
            {
                int applicationId = Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key);
                TechnicalContextOperations.EnsureCommunityEngagementRecordsForApplication(applicationId);
                ddeBeginDate.Focus();
            }
        }

        public override void SaveData()
        {
        }
        /// <summary>
        /// BtnRetrieve_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected void BtnRetrieve_Click(object sender, EventArgs e)
        {
            RetrieveHistoryRecords();
        }
        /// <summary>
        /// BtnClear_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnClear_Click(object sender, EventArgs e)
        {
            ddeBeginDate.Text = string.Empty;
            ddeEndDate.Text = string.Empty;
            ddeBeginDate.Value = null;
            ddeEndDate.Value = null;
            RetrieveHistoryRecords();
        }

        /// <summary>
        /// RetrieveHistoryRecords
        /// </summary>
        private void RetrieveHistoryRecords()
        {
            gvASPxGridView.DataBind();
        }
        /// <summary>
        /// BtnShowDetails_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnShowDetails_Click(object sender, EventArgs e) { }
        /// <summary>
        /// btnCaseComment_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCaseComment_Click(object sender, EventArgs e) { }
        protected void btnPrevious_Click(object sender, EventArgs e) { }

        /// <summary>
        /// btnNext_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNext_Click(object sender, EventArgs e)
        {
            NavigateToNextPage();
        }
        protected void GvASPxGridView_DataBound(object sender, EventArgs e)
        {
            gvASPxGridView.Bind<TechnicalCommunityEngagementSummaryMetaData>(x => x.PersonID);
        }
        /// <summary>
        /// BtnViewDetails Click  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnViewDetails_Click(object sender, EventArgs e)
        {
            TechnicalSessionContext.Instance.CommunityEngagementSummaryID = int.Parse(((ASPxButton)sender).CommandArgument);
            NavigateTo(n => n.Name == IntakeConstants.COMMUNITYENGAGEMENT_DETAILS_AE, p => ((Technical_CommunityEngagement)p).CommunityEngagementSummaryID == TechnicalSessionContext.Instance.CommunityEngagementSummaryID);
        }
        /// <summary>
        /// gvASPxGridView_CustomCallback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvASPxGridView_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            NavigateToNextPage();
        }
        /// <summary>
        /// DsCommunityEngagementSummary_Selecting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DsCommunityEngagementSummary_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            ServicesTracingHub.TraceWriter.WriteLine("CommunityEngagementSummary.DsCommunityEngagement_Selecting - Start");
            e.Result = GetResult();
            ServicesTracingHub.TraceWriter.WriteLine("CommunityEngagementSummary.DsCommunityEngagement_Selecting - End");
        }

        /// <summary>
        /// GetResult
        /// </summary>
        private IEnumerable<Technical_CommunityEngagementSummary> GetResult()
        {
            int applicationId = Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key);

            return TechnicalCommon.IsHistorySearchMode(ddeBeginDate.Value, ddeEndDate.Value)
                ? TechnicalContextOperations.GetHistoryRecordsCommunityEngagementSummary(applicationId, ddeBeginDate.Value, ddeEndDate.Value)
                : TechnicalContextOperations.GetAllActiveRecordsCommunityEngagementSummary(applicationId);
        }
        /// <summary>
        /// NavigateToNextPage
        /// </summary>
        protected void NavigateToNextPage()
        {

            if (gvASPxGridView.FocusedRowIndex != -1)
            {
                TechnicalSessionContext.Instance.CommunityEngagementSummaryID = Convert.ToInt32(gvASPxGridView.GetRowValues(gvASPxGridView.FocusedRowIndex, "CommunityEngagementSummaryID"));
                NavigateTo(n => n.Name == IntakeConstants.COMMUNITYENGAGEMENT_DETAILS_AE, p => ((Technical_CommunityEngagement)p).CommunityEngagementSummaryID == TechnicalSessionContext.Instance.CommunityEngagementSummaryID);
            }
        }
    }
}