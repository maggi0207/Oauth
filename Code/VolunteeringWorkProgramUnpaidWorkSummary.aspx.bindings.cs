using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using Dhss.Framework.DataAnnotations;
using Dhss.Framework.Web.UI.Workflow;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{
    public partial class VolunteeringWorkProgramUnpaidWorkSummary
    {
        protected class VolunteeringWorkProgramSummaryMetaData
        {
            [LookupTable("AEVPUW", "VERIF-CD", "DESC-TXT", typeof(ReferenceTableLookupContext))]
            public string ProgramTypeCode { get; set; }
        }

        public override void BindEntities()
        {

        }

        public override void SaveData()
        {

        }

        private void InitiateSession()
        {
            _applicationId = int.Parse(WorkflowSession.Instance.RootFrame.State.Key);
        }

        protected void ClearSessionVariables()
        {
            TechnicalSessionContext.Instance.VolunteeringWorkProgramID = 0;
            TechnicalSessionContext.Instance.IsVolunteeringWorkProgramBackToSummary = false;
        }
    }
}