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
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Services;
using Dhss.Framework.Extensions;
using Dhss.Framework.Web.UI.Workflow;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.UI.WebControls;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{
    [Workflow]
    [ExcludeFromCodeCoverage]
    public partial class TechnicalQuestions : Dhss.Assist.WorkerWeb.Web.Infrastructure.Workflow.WorkflowPage<Technical_Case>
    {
        private const string DAEXQS02 = "DAEXQS02";

        public override string PageEntityName
        {
            get
            {
                return "Technical_HouseholdGeneralInfo";
            }
        }

        /// <summary>
        /// Event occurs on Page load.
        /// </summary>
        /// <param name="sender">Page</param>
        /// <param name="e">Page Event Args</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Master.FooterSectionConfigure(FooterBodyConfiguration.AddnoteSavePreviousNext);

            if (!IsPostBack)
            {
                if (!TechnicalSessionContext.Instance.IsTechnicalQuestionsAdded) //Avoiding subsequent calls to improve performance.
                {
                    TechnicalContextOperations.CheckIfRecordExistInHouseholdGeneralInfo(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                    TechnicalSessionContext.Instance.IsTechnicalQuestionsAdded = true;
                }
            }        
        }
        bool? txtFocus = false;
        /// <summary>
        /// Disable the control if the respective question response is "Yes".
        /// </summary>
        private void EnableDisableFields()
        {
            var control =
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdPregnant").As<ASPxComboBox>();

            if (TechnicalCommon.IsResponseYes(Convert.ToString(control.Value)))
            {
                control.Enabled = true;
            }
            else if (txtFocus == false)
            {
                txtFocus = true;
                control.Focus();
            }
            control =
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdLessThan1").As<ASPxComboBox>();
            if (TechnicalCommon.IsResponseYes(Convert.ToString(control.Value)))
            {
                control.Enabled = false;
            }
            else if (txtFocus == false)
            {
                txtFocus = true;
                control.Focus();
            }
            control =
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbDoesAnyoneInTheHouseholdReceiveS").As<ASPxComboBox>();
            if (TechnicalCommon.IsResponseYes(Convert.ToString(control.Value)))
            {
                control.Enabled = false;
            }
            else if (txtFocus == false)
            {
                txtFocus = true;
                control.Focus();
            }
            control = fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdNoLongerA")
                .As<ASPxComboBox>();
            if (TechnicalCommon.IsResponseYes(Convert.ToString(control.Value)))
            {
                control.Enabled = false;
            }
            else if (txtFocus == false)
            {
                txtFocus = true;
                control.Focus();
            }
            control =
                fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdApplyingF").As<ASPxComboBox>();
            if (TechnicalCommon.IsResponseYes(Convert.ToString(control.Value)))
            {
                control.Enabled = false;
            }
            else if (txtFocus == false)
            {
                txtFocus = true;
                control.Focus();
            }
            control = (fvdsTechnical_HouseholdGeneralInfo.FindControl("cbDoesAnyoneInYourHouseholdWhoIsAp")
                .As<ASPxComboBox>());
            if (TechnicalCommon.IsResponseYes(Convert.ToString(control.Value)))
            {
                control.Enabled = false;
            }
            else if (txtFocus == false)
            {
                txtFocus = true;
                control.Focus();
            }
            control = (fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdApplyingF1")
                .As<ASPxComboBox>());
            if (TechnicalCommon.IsResponseYes(Convert.ToString(control.Value)))
            {
                control.Enabled = false;
            }
            else if (txtFocus == false)
            {
                txtFocus = true;
                control.Focus();
            }
            control = (fvdsTechnical_HouseholdGeneralInfo.FindControl("cbHasAnyoneInYourHouseholdBeenRefe")
                .As<ASPxComboBox>());
            if (TechnicalCommon.IsResponseYes(Convert.ToString(control.Value)))
            {
                control.Enabled = false;
            }
            else if (txtFocus == false)
            {
                txtFocus = true;
                control.Focus();
            } 
        }

        /// <summary>
        /// Occurs on formview data bound.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FvdsTechnical_HouseholdGeneralInfo_DataBound(object sender, EventArgs e)
        {
            ValidNewBornDefaultValue();
            EnableDisableFields();
            SetPregnancyDefaultValue();
        }

        /// <summary>
        /// Validates new born.
        /// </summary>
        protected void ValidNewBornDefaultValue()
        {
            if (TechnicalContextOperations.IsNewBornAdded())
            {
                string newBorn =
                    Convert.ToString(
                        (fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdLessThan1")
                            .As<ASPxComboBox>()).Value);
                if ((newBorn.Trim() == string.Empty || newBorn == IntakeConstants.NO_CODE))
                {
                    WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                        n => n.Name == IntakeConstants.NEW_BORN_SUMMARY_AE).Visible = true;
                    SetPageComplete(IntakeConstants.NEW_BORN_SUMMARY_AE, false);
                    (fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdLessThan1")
                        .As<ASPxComboBox>()).Value = IntakeConstants.YES_CODE;
                }                
            }
            else
            {
                (fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdLessThan1")
                        .As<ASPxComboBox>()).Value = IntakeConstants.NO_CODE;
                (fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdLessThan1")
                    .As<ASPxComboBox>()).Enabled = false;
            }
        }
        /// <summary>
        /// Defect 39856 : If Female does not exist in the case ,  cbIsAnyoneInYourHouseholdPregnant default value should be NO and Disabled.
        /// </summary>
        protected void SetPregnancyDefaultValue()
        {
            ASPxComboBox cbIsAnyoneInYourHouseholdPregnant = fvdsTechnical_HouseholdGeneralInfo.FindControl("cbIsAnyoneInYourHouseholdPregnant").As<ASPxComboBox>();
            if (!TechnicalContextOperations.IsFemaleExists() && cbIsAnyoneInYourHouseholdPregnant != null)
            {
                cbIsAnyoneInYourHouseholdPregnant.Value = IntakeConstants.NO_CODE;
                cbIsAnyoneInYourHouseholdPregnant.Enabled = false;
                WorkflowSession.Instance.CurrentFrame.Workflow.Children.Single(
                    n => n.Name == IntakeConstants.PREGNANCY_SUMMARY_AE).Visible = false;


                TechnicalSessionContext.Instance.IsFemaleExists = false;
            }
            else
            {
                TechnicalSessionContext.Instance.IsFemaleExists = true;
            }

        }


        /// <summary>
        /// Event raises on LinqDataSource OnSelecting Event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DsTechnical_HouseholdGeneralInfo_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var technicalContext = ServicesDataHub.Technical;
            e.Result =
                technicalContext.Technical_HouseholdGeneralInfo.Where(
                    n => n.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }
    }
}

