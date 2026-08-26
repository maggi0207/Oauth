///////////////////////////////////////////////////////////////////////////////////////////////////////
//
// File:      ApplicationEntryDataServiceLinqDataSource.cs
//
// Created On: Monday, March 18, 2013 12:40:33 PM
// Created By: Suresh.Padarthi
//
// This file may contain sensitive and/or confidential information and may not be
// distributed without written permission of Delaware Department of Health and 
// Social Services.
//
// #      Type        User                    Date        Comment                                      
// ------ ----------- ----------------------- ----------- -------------------------------------------- 
// 8834	   add	       Suresh.Padarthi         3/12/2013   Added History Begin and End date format class file.
///////////////////////////////////////////////////////////////////////////////////////////////////////

using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Entity.CaseloadManagement;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Helpers;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Services;
using Dhss.Framework.Web.Context;
using Dhss.Framework.Web.UI.Workflow;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{
    [ExcludeFromCodeCoverage]
    public class ApplicationEntryDataServiceLinqDataSource
    {
        private const string YVALUE = "Y";
        private const string IVALUE = "I";
        private const string VOLUNTEERING_SCHEDULE_KEY = "IsVolunteeringWorkProgramEnabled";

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_ProgramDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetProgramOfAssistance(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_AdditionalProgramDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetAdditionalProgramDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_PersonDemographics_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetAdditionalIndivDemographics(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_TaxDependencyDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetTaxDependencyDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_ChildCareAdditionalDemographics_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetChildCareAdditionalDemographics(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_PersonRelations_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetPersonRelations(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_PersonRefugee_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetPersonRefugee(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_SponsorDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetAlienSponsor(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_HealthInsuranceLossDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetHealthInsuranceLossDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_InstitutionInfoDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetInstitutionInfoDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on IncarcerationDetails OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_IncarcerationDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetIncarcerationDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_IndividualBenefitsDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetIndividualBenefitsDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_FosterCareDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetFosterCareDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_SchoolEnrollmentDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetSchoolEnrollmentDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_PregnancyDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetPregnancyDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }
        public static void Technical_CommunityEngagementDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetCommunityEngagementDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }
        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_DisabilityDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetDisabilityDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_HomeCommunityBasedServiceDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetHomeCommunityBasedServiceDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_SpousalImpoverishmentDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetSpousalImpoverishmentDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_ProtectedSSIDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetProtectedSsiDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_ContinuouslyEligibleNewbornDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetContinuouslyEligibleNewbornDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_CRDPInfoDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetCrdpInfoDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_BreastAndCervicalCancerDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetBreastAndCervicalCancerDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Raises on Linqdatasource OnSelecting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Technical_DisasterBenefitInfoDetails_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = GetDisasterBenefitInfoDetails(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>       
        /// <param name="applicationId">ApplicationID</param>
        /// <returns></returns>
        public static IEnumerable<Technical_ProgramDetail> GetProgramOfAssistance(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            
            var allActiveRecords = new List<Technical_ProgramDetail>();
            allActiveRecords = ServicesApplicationHub.IntakeTechnical.GetProgramOfAssistance(applicationId);
            //For Request "No" or History Records.
            if (TechnicalSessionContext.Instance.ProgramDetailID != 0 && (allActiveRecords.Count == 0 || !allActiveRecords.Any(n => n.ProgramDetailID == TechnicalSessionContext.Instance.ProgramDetailID)))
            {
                allActiveRecords.Add(technicalContext.Technical_ProgramDetail.Where(n => n.ProgramDetailID == TechnicalSessionContext.Instance.ProgramDetailID).FirstOrDefault());
            }
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>       
        /// <param name="applicationId">ApplicationID</param>
        /// <returns></returns>
        public static IEnumerable<Technical_ProgramDetail> GetAdditionalProgramDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = new List<Technical_ProgramDetail>();
            allActiveRecords = ServicesApplicationHub.IntakeTechnical.GetAdditionalProgramDetails(applicationId);
            //For  History Records.
            if (TechnicalSessionContext.Instance.ProgramDetailID != 0 && (allActiveRecords.Count == 0 || !allActiveRecords.Any(n => n.ProgramDetailID == TechnicalSessionContext.Instance.ProgramDetailID)))
            {
                allActiveRecords.Add(technicalContext.Technical_ProgramDetail.Where(n => n.ProgramDetailID == TechnicalSessionContext.Instance.ProgramDetailID).FirstOrDefault());
            }
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all active Additional Indiv Demographics records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_PersonDemographics> GetAdditionalIndivDemographics(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            IEnumerable<Technical_PersonDemographics> allActiveRecords = technicalContext.Technical_PersonDemographics
                                                                                         .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                                                             && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                                                             && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                                                             && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty) || n.PersonDemographicsID == TechnicalSessionContext.Instance.PersonDemographicsID);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all active Child care Additional Demographics records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_ChildCareAdditionalDemographics> GetChildCareAdditionalDemographics(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            IEnumerable<Technical_ChildCareAdditionalDemographics> allActiveRecords = technicalContext.Technical_ChildCareAdditionalDemographics
                                                                                                      .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                                                                          && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                                                                          && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                                                                          && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty) || n.ChildCareDemographicsID == TechnicalSessionContext.Instance.ChildCareDemographicsID);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all Person Relations All Active Records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_PersonRelation> GetPersonRelations(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var householdRelations = new List<Technical_PersonRelation>();
            //var allRecords = technicalContext.Technical_PersonRelation
            //                                 .Where(n => n.ApplicationEntity.ApplicationID == applicationId
            //                                     && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
            //                                     && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
            //                                     && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty) || 
            //                                     ((n.PersonRelationID == TechnicalSessionContext.Instance.PersonRelationID)))
            //                                 .OrderByDescending(o => o.ApplicationEntity.PrimaryPersonIndicator)
            //                                 .ThenBy(o => o.PersonRelationID);
            var allRecords = ServicesApplicationHub.IntakeTechnical.GetPersonRelationActiveRecords(applicationId);
            //For History search
            if (TechnicalSessionContext.Instance.PersonRelationID > 0 && !allRecords.Any(n => n.PersonRelationID == TechnicalSessionContext.Instance.PersonRelationID))
            {
              var historyRecord =  technicalContext.Technical_PersonRelation
                                            .Where(n => n.PersonRelationID == TechnicalSessionContext.Instance.PersonRelationID).FirstOrDefault();
              if (historyRecord != null)
                  allRecords.Add(historyRecord);
            }
            foreach (Technical_PersonRelation relation in allRecords)
            {
                if (TechnicalSessionContext.Instance.PersonRelationID == relation.PersonRelationID && (householdRelations.Count > 0 && householdRelations.Any(n => n.ApplicationEntityID == relation.ApplicationEntityID)))
                {
                    householdRelations.Remove(householdRelations.Where(n => n.ApplicationEntityID == relation.ApplicationEntityID).FirstOrDefault());
                    householdRelations.Add(relation);
                }
                else if (householdRelations.Count == 0 || (householdRelations.Count > 0 && !householdRelations.Any(n => n.ApplicationEntityID == relation.ApplicationEntityID)))
                {
                    householdRelations.Add(relation);
                }
            }
            return householdRelations;
        }

        /// <summary>
        /// Gets all Alien refugee Active Records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_PersonRefugee> GetPersonRefugee(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_PersonRefugee.Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty) && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.PersonRefugeeID == TechnicalSessionContext.Instance.PersonRefugeeID);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all Alien Sponsor details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_Sponsor> GetAlienSponsor(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_Sponsor
                                                   .Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.SponsorID == TechnicalSessionContext.Instance.SponsorID);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets Health InsuranceLoss Details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_HealthInsuranceLoss> GetHealthInsuranceLossDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_HealthInsuranceLoss
                                                   .Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || (n.HealthInsuranceLossID == TechnicalSessionContext.Instance.HealthInsuranceLossID));
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all Institution details Records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_InstitutionInfo> GetInstitutionInfoDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_InstitutionInfo
                                                   .Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.InstitutionInfoID == TechnicalSessionContext.Instance.InstitutionInfoIDSelected);
            return allActiveRecords;
        }
        /// <summary>
        /// Gets all Incarceration details Records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_IncarcerationDetails> GetIncarcerationDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_IncarcerationDetails
                                                   .Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.IncarcerationDetailsID == TechnicalSessionContext.Instance.IncarcerationDetailsIDSelected);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all Institution details Records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_IndividualBenefits> GetIndividualBenefitsDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_IndividualBenefits
                                                   .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty) || n.IndividualBenefitsID == TechnicalSessionContext.Instance.IndividualBenefitsID);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all Institution details Records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_FosterCare> GetFosterCareDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_FosterCare
                                                   .Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.FosterCareID == TechnicalSessionContext.Instance.FosterCareID);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all SchoolEnrollment details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_SchoolEnrollment> GetSchoolEnrollmentDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_SchoolEnrollment
                                                   .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty) || n.SchoolEnrollmentID == TechnicalSessionContext.Instance.SchoolEnrollmentID);
            return allActiveRecords;
        }

        /// <summary>
        /// Schedule Volunteering/Work Program/Unpaid Work when Community Engagement answered "Yes"
        /// to the Work Program or the Unpaid Work question for any person, Otherwise Unschedule.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsVolunteeringWorkProgramEnabled(object sender)
        {
            var value = RequestContext.ItemGet(VOLUNTEERING_SCHEDULE_KEY);
            bool returnValue;
            if (string.IsNullOrEmpty(value))
            {
                int applicationId = Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key);
                returnValue = TechnicalContextOperations.GetAllActiveRecordsCommunityEngagementSummary(applicationId)
                    .Any(n => n.WorkProgramIndicator == true || n.UnpaidWorkIndicator == true);
                RequestContext.ItemSet(returnValue, VOLUNTEERING_SCHEDULE_KEY);
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }

            return returnValue;
        }

        /// <summary>
        /// Overwrites the cached schedule decision after Community Engagement is saved, so the left
        /// menu in the same request reflects the answers that were just entered.
        /// </summary>
        /// <param name="isEnabled"></param>
        public static void ResetVolunteeringWorkProgramSchedule(bool isEnabled)
        {
            RequestContext.ItemSet(isEnabled, VOLUNTEERING_SCHEDULE_KEY);
        }

        /// <summary>
        /// Gets the volunteering work program details.
        /// </summary>
        /// <param name="applicationId">The application identifier.</param>
        /// <returns></returns>
        public static IEnumerable<Technical_VolunteeringWorkProgram> GetVolunteeringWorkProgramDetails(int applicationId)
        {
            //var technicalContext = ServicesDataHub.Technical;
            //var result = technicalContext.Technical_VolunteeringWorkProgram.Where(record => record.Person.ApplicationEntity.Any(a => a.ApplicationID == applicationId)).ToList();
            //return result;

            var records = ServicesApplicationHub.IntakeTechnical.GetHistoryRecordsVolunteeringWorkProgram(applicationId, null, null);
            return records ?? new List<Technical_VolunteeringWorkProgram>();
        }

        public static IEnumerable<Technical_VolunteeringWorkProgram> GetVolunteeringProDetailsSelected()
        {
            int applicationId = Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key);
            return GetVolunteeringWorkProgramDetails((int)applicationId);
        }

        /// <summary>
        /// Gets all Pregnancy details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_Pregnancy> GetPregnancyDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_Pregnancy
                                                   .Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.PregnancyID == TechnicalSessionContext.Instance.PregnancyID);
            return allActiveRecords;
        }
        public static IEnumerable<Technical_CommunityEngagement> GetCommunityEngagementDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_CommunityEngagement
                                                   .Where(p => (
                                                        (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                        && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                        //&& (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)) || p.CommunityEngagementID == TechnicalSessionContext.Instance.CommunityEngagementID)
                                                        );
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all Disability details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_Disability> GetDisabilityDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_Disability
                                                   .Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.DisabilityID == TechnicalSessionContext.Instance.DisabilityID);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all Home Community Based Service details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_HomeCommunityBasedService> GetHomeCommunityBasedServiceDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_HomeCommunityBasedService
                                                   .Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.HomeCommunityBasedServiceID == TechnicalSessionContext.Instance.HomeCommunityBasedServiceID);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all Spousal Impoverishment Details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_SpousalImpoverishment> GetSpousalImpoverishmentDetails(int applicationId)
        {
            var technicalContext = new TechnicalContextImpl();
            var allActiveRecords = technicalContext.Technical_SpousalImpoverishment
                                                   .Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.SpousalImpoverishmentID == TechnicalSessionContext.Instance.SpousalImpoverishmentIDSelected);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all Protected SSI details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_ProtectedSSI> GetProtectedSsiDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_ProtectedSSI
                                                   .Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId 
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) 
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty) 
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.ProtectedSSIID == TechnicalSessionContext.Instance.ProtectedSSIInfoIDSelected);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all Continuously Eligible Newborn details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_ContinuouslyEligibleNewborn> GetContinuouslyEligibleNewbornDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_ContinuouslyEligibleNewborn
                                                   .Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.ContinuouslyEligibleNewbornID == TechnicalSessionContext.Instance.ContinuouslyEligibleNewbornID);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all CRDP Info  details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_CRDPInfo> GetCrdpInfoDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_CRDPInfo
                                                   .Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.CRDPInfoID == TechnicalSessionContext.Instance.CrdpInformationIDSelected);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all Breast And Cervical Cancer details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_BreastAndCervicalCancer> GetBreastAndCervicalCancerDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_BreastAndCervicalCancer
                                                   .Where(n => (n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                       && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                       && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.BreastAndCervicalCancerID == TechnicalSessionContext.Instance.BreastAndCervicalCancerID);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all Breast And Cervical Cancer details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_DisasterBenefitInfo> GetDisasterBenefitInfoDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var allActiveRecords = technicalContext.Technical_DisasterBenefitInfo
                                                   .Where(n => (n.ApplicationID == applicationId
                                                       && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                       && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) || n.DisasterBenefitInfoID == TechnicalSessionContext.Instance.DisasterBenefitInfoID);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all TaxDependency  details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_TaxDependency> GetTaxDependencyDetails(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            //var allActiveRecords = technicalContext.Technical_TaxDependency
            //                                       .Where(n => (n.ApplicationEntity.ApplicationID == applicationId
            //                                           && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
            //                                           && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
            //                                           && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
            //                                           && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty))|| n.TaxDependentID == TechnicalSessionContext.Instance.TaxDependentID)
            //                                           .OrderByDescending(n => n.ApplicationEntity.PrimaryPersonIndicator)
            //                                            .ThenBy(n => n.ApplicationEntityID).ToList();
            var allActiveRecords = ServicesApplicationHub.IntakeTechnical.GetAllActiveRecordsTaxDependency(applicationId);
           
            if (TechnicalSessionContext.Instance.TaxDependentID != 0 && (allActiveRecords.Count == 0 || !allActiveRecords.Any(n => n.TaxDependentID == TechnicalSessionContext.Instance.TaxDependentID)))
            {
                allActiveRecords.Add(technicalContext.Technical_TaxDependency.Where(n => n.TaxDependentID == TechnicalSessionContext.Instance.TaxDependentID).FirstOrDefault());
            }
            return allActiveRecords;
        }

        #region Schedule Pages Conditionally
        /// <summary>
        /// Schedule TaxDependency Page if ProgramDetail for MA request is true Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsTaxDependencyEnabled(object sender)
        {
            string key = "IsTaxDependencyEnabled";
            var value = RequestContext.ItemGet(key);
            bool returnValue;
            if (string.IsNullOrEmpty(value))
            {
                returnValue = ServicesApplicationHub.PageScheduler.IsTaxDependencyEnabled(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                RequestContext.ItemSet(returnValue, key);
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }

            return returnValue;       
        }

        /// <summary>
        /// Schedule ChildCareDemographics Page if ProgramDetail for ChildCare (CC) request is true Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsChildCareAdditionalDemoEnabled(object sender)
        {
            string key = "IsChildCareAdditionalDemoEnabled";
            var value = RequestContext.ItemGet(key);
            bool returnValue;
            if (string.IsNullOrEmpty(value))
            {
                returnValue = ServicesApplicationHub.PageScheduler.IsChildCareAdditionalDemoEnabled(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                RequestContext.ItemSet(returnValue, key);
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }

            return returnValue;             
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsAdditionalIndividualDemographics(object sender)
        {
            return true;
        }
        /// <summary>
        /// This proerty cache Technical_HouseholdGeneralInfo for the application id in Http context.
        /// </summary>
        private static Technical_HouseholdGeneralInfo HouseholdInfo
        {
            get
            {
                var instance = HttpContext.Current.Items["DS_Technical_HouseholdGeneralInfo_Cache"] as Technical_HouseholdGeneralInfo;
                if (instance == null || instance.ApplicationID != Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key))
                {
                    using (var technicalContext = ServicesDataHub.Technical)
                    {
                        if (technicalContext.Technical_HouseholdGeneralInfo.Where(n => n.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)).Count() > 0)
                        {
                            instance = (from n in technicalContext.Technical_HouseholdGeneralInfo
                                        where n.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)
                                        select n).FirstOrDefault();
                        }
                        HttpContext.Current.Items["DS_Technical_HouseholdGeneralInfo_Cache"] = instance;
                    }
                }
                return instance == null ? new Technical_HouseholdGeneralInfo() : instance;
            }
        }

        /// <summary>
        /// Schedule Pregnancy Page if response in Technical Question for "Is anyone in your household Pregnant?" is Yes, Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsPregnancyEnabled(object sender)
        {
            return HouseholdInfo.IsAnyonePregnantIndicator == YVALUE;
        }

        /// <summary>
        /// Schedule New Born Page if response in Technical Question for "Is anyone in your household less than 13 months Old?" is Yes, Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsNewBornEnabled(object sender)
        {
            return HouseholdInfo.Haslessthan13monthschildIndicator == YVALUE && IntakeContext.Instance.MAProgramCode;
        }
        public static bool IsCommunityEngagementEnabled(object sender)
        {
            return true;
        }

        /// <summary>
        /// Schedule Disability Page if response in Technical Question for Disability is "Yes", Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsDisabilityEnabled(object sender)
        {
            return HouseholdInfo.ReceiveDisablityPaymentIndicator == YVALUE;
        }

        /// <summary>
        /// Schedule ProtectedSSI Page if response in Technical Question for ProtectedSSI is "Yes", Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsProtectedSSIEnabled(object sender)
        {
            bool isPageSchedule = IsScheduledByCaseMode();

            return (isPageSchedule ? (HouseholdInfo.HadSSIRecipientIndicator == YVALUE)  : false);
        }

        /// <summary>
        /// Schedule Home Based Community Services Page if response in Technical Question for HCBS is "Yes", Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsHBCSEnabled(object sender)
        {
            bool isPageSchedule = IsScheduledByCaseMode();

            return (isPageSchedule ? (HouseholdInfo.HasHCBSWaiverIndicator == YVALUE)  : false);
        }

        /// <summary>
        /// Schedule Spousal Impoverishment Page if response in Technical Question for Spousal Impoverishment is "Yes", Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsSpousalImpoverishmentEnabled(object sender)
        {
            return HouseholdInfo.HasLTCwithSpouseinCommunity == YVALUE;
        }

        /// <summary>
        /// Schedule CRDP Information Page if response in Technical Question for CRDP Information is "Yes", Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsCRDPInfoEnabled(object sender)
        {
            return HouseholdInfo.HasChronicRenalDiseaseProgramParticipantIndicator == YVALUE;
        }

        /// <summary>
        /// Schedule Breast and Cervical Cancer Page if response in Technical Question for Breast and Cervical Cancer is "Yes", Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsBCCPEnabled(object sender)
        {
            bool isPageSchedule = IsScheduledByCaseMode();
            
            return (isPageSchedule ? (HouseholdInfo.IsReferredByDPHIndicator == YVALUE) : false);
        }

        /// <summary>
        /// Schedule Institution Page if response in  LivingArrangementCode have value , Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsInstitutionEnabled(object sender)
        {
            string key = "IsInstitutionEnabled";
            var value = RequestContext.ItemGet(key);
            bool returnValue;
            if (string.IsNullOrEmpty(value))
            {
                returnValue = ServicesApplicationHub.PageScheduler.IsInstitutionEnabled(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                RequestContext.ItemSet(returnValue, key);
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }
            return returnValue;          
           
        }
        /// <summary>
        /// Schedule Incarcerated Page if response in  LivingArrangementCode have value , Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsIncarcerationEnabled(object sender)
        {
            string key = "IsIncarcerationEnabled";
            var value = RequestContext.ItemGet(key);
            bool returnValue;
            if (string.IsNullOrEmpty(value))
            {
                returnValue = ServicesApplicationHub.PageScheduler.IsIncarcerationEnabled(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                RequestContext.ItemSet(returnValue, key);
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }
            return returnValue;
        }

        

        public static bool IsPreprocessingEnabled(object sender)
        {
            string key = "IsPreprocessingEnabled";
            var value = RequestContext.ItemGet(key);
            bool returnValue=false;
            if (string.IsNullOrEmpty(value))
            {
                if (string.IsNullOrEmpty(IntakeContext.Instance.HAS_SNAP_BENEFIT))
                {
                    var foodBenefit = ServicesApplicationHub.IntakeAssistMain.CheckAssistProgramCode(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key), IntakeConstants.PROGRAM_FOOD_STAMP);
                    IntakeContext.Instance.HAS_SNAP_BENEFIT = string.IsNullOrEmpty(foodBenefit) ? "NO" : "YES";
                }
                if (IntakeContext.Instance.HAS_SNAP_BENEFIT == "YES")
                {
                    returnValue = ServicesApplicationHub.PageScheduler.IsPreprocessingEnabled(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                   
                }
                else
                {
                    returnValue = false;
                }
                RequestContext.ItemSet(returnValue, key);
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }
            return returnValue;
        }

        /// <summary>
        /// IS process flow for insitution is MA Program.
        /// </summary>
        /// <param name="maProgramCode"></param>
        /// <returns></returns>
        private static bool IsProcessFlowForInstitution(bool maProgramCode)
        {
            return maProgramCode;
        }
        /// <summary>
        /// Schedule HouseholdRelationship Screen If established individuals in an application is >1 Otherwise UnSchedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsHouseholdRelationshipEnable(object sender)
        {
            string key = "IsHouseholdRelationshipEnable";
            var value = RequestContext.ItemGet(key);
            bool returnValue;
            if (string.IsNullOrEmpty(value))
            {
                using (var technicalContext = ServicesDataHub.Technical)
                {
                    IEnumerable<Technical_ApplicationEntity> appEntity = ((DataServiceQuery<Technical_ApplicationEntity>)technicalContext.Technical_ApplicationEntity
                                                                                                                                         .Where(p => p.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)
                                                                                                                                             && p.EstablishedIndicator == true));

                    returnValue = IsProcessFlowForHouseholdRelationshipEnabled(appEntity);
                    RequestContext.ItemSet(returnValue, key);
                }
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }
            return returnValue;  

           
        }

         
        /// </summary>
        /// <param name="appEntity"></param>
        /// <returns></returns>
        private static bool IsProcessFlowForHouseholdRelationshipEnabled(IEnumerable<Technical_ApplicationEntity> appEntity)
        {
            return appEntity.Count() > 1;
        }
        /// Schedule Alien Refugee Screen and Alien Refugee Sponsor Screen  if response for  US Citizen or National is "NO" in Additional individual demographics screen, Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsAlienRefugeeEnabled(object sender)
        {
            string key = "IsAlienRefugeeEnabled";
            var value = RequestContext.ItemGet(key);
            bool returnValue;
            if (string.IsNullOrEmpty(value))
            {
                returnValue = ServicesApplicationHub.PageScheduler.IsAlienRefugeeEnabled(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                RequestContext.ItemSet(returnValue, key);
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }
            return returnValue;   
        }

        /// <summary>
        /// Schedule  Alien Refugee Sponsor Screen  if response for  US Citizen or National is "NO" in Additional individual demographics screen, Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsAlienRefugeeSponsorEnabled(object sender)
        {
            string key = "IsAlienRefugeeSponsorEnabled";
            var value = RequestContext.ItemGet(key);
            bool returnValue;
            if (string.IsNullOrEmpty(value))
            {
                returnValue = (ServicesApplicationHub.PageScheduler.IsAlienRefugeeSponsorEnabled(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)));
                RequestContext.ItemSet(returnValue, key);
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }
            return returnValue;              
        }

        /// <summary>
        /// Schedule Loss of Health Insurance  Screen and Alien Refugee Sponsor Screen  if response for  HealthInsuranceLoss6MnthIndicator is "yes" in Additional individual demographics screen, Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsLossOfHealthInsuranceEnabled(object sender)
        {
            string key = "IsLossOfHealthInsuranceEnabled";
            var value = RequestContext.ItemGet(key);
            bool returnValue;
            if (string.IsNullOrEmpty(value))
            {
                returnValue = (ServicesApplicationHub.PageScheduler.IsLossOfHealthInsuranceEnabled(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)));
                RequestContext.ItemSet(returnValue, key);
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }
            return returnValue;           
        }

        /// <summary>
        /// Schedule Teen Parent  if response for  TeenParentPolicyExemptIndicator is "yes" in Additional individual demographics screen, Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsTeenparentEnabled(object sender)
        {
            string key = "IsTeenparentEnabled";
            var value = RequestContext.ItemGet(key);
            bool returnValue;
            if (string.IsNullOrEmpty(value))
            {
                returnValue = IsProcessFlowForTeenParent(IntakeContext.Instance.CAProgramCode) ? (ServicesApplicationHub.PageScheduler.IsTeenparentEnabled(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)))
                                                                                    : false;
                RequestContext.ItemSet(returnValue, key);
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }
            return returnValue;  
            
        }
        /// <summary>
        /// If process flow for teen parent is true than its CA program.
        /// </summary>
        /// <param name="caProgramCode"></param>
        /// <returns></returns>
        private static bool IsProcessFlowForTeenParent(bool caProgramCode)
        {
            return caProgramCode;
        }
        /// <summary>
        /// Schedule Foster Care/ Adoption page  if response for  FosterCareOrAdoptionIndicator is "yes" in Individual benefits Received screen, Otherwise Unschedule
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsFosterCareEnabled(object sender)
        {
            string key = "IsFosterCareEnabled";
            var value = RequestContext.ItemGet(key);
            bool returnValue;
            if (string.IsNullOrEmpty(value))
            {
                returnValue = (IsProcessFlowForFosterCareEnabled(IntakeContext.Instance.MAProgramCode, IntakeContext.Instance.FBProgramCode) 
			                    && ServicesApplicationHub.PageScheduler.IsFosterCareEnabled(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)));
                RequestContext.ItemSet(returnValue, key);
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }
            return returnValue; 
        }
        /// <summary>
        /// If process flow for foster case enabled returns the Medical Assistance program and Food Benefits.
        /// </summary>
        /// <param name="maProgramCode"></param>
        /// <param name="fbProgramCode"></param>
        /// <returns></returns>
 		private static bool IsProcessFlowForFosterCareEnabled(bool maProgramCode, bool fbProgramCode)
        {
            return (maProgramCode || fbProgramCode);
            
        }        
        /// <summary>
        /// Schedule Additional Program Details in Ongoing Workflow and Cash is requested
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        /// <history>
        /// =============================================================
        /// Modified By             Date                    Defect
        /// =============================================================
        /// [Sushil]           11/23/2013              39430 - Additional Program Detail summary and Additional Program Detail will be displayed
        ///                                                         when the case has its CaseMode is Ongoing, StatusMode is Open and the CA program 
        ///                                                         is selected newly
        /// </history>
        public static bool IsAdditionalProgramDetailsEnabled(object sender)
        {
            TechnicalContextImpl technicalContext = ServicesDataHub.Technical;
            bool flag = false;
            
            if (IsProcessFlowForAdditionalProgramDetails(IntakeContext.Instance.CaseMode, IntakeContext.Instance.CAProgramCode))
            {
                flag = true;//(new AdditionalProgramDetailsSummary().IsAdditionalProgramDetailScheduled()) || ServicesApplicationHub.PageScheduler.IsAdditionalProgramDetailsEnabled(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
            }
            return flag;
        }
		
	 	private static bool IsProcessFlowForAdditionalProgramDetails(string caseMode, bool caProgramCode)
        {
            return caseMode != null && caProgramCode;
        }
        /// <summary>
        /// Schedule Additional Program Details in Ongoing Workflow and Cash is requested
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsProgramDetailsEnabled(object sender)
        {
            string key = "IsProgramDetailsEnabled";
            var value = RequestContext.ItemGet(key);
            bool returnValue;
            if (string.IsNullOrEmpty(value))
            {
                returnValue = ServicesApplicationHub.PageScheduler.IsProgramDetailsEnabled(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key));
                RequestContext.ItemSet(returnValue, key);
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }
            return returnValue;          
        }

        /// <summary>
        /// Schedule Benefit Cap if any individual Age < 19
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsBenefitCapEnabled(object sender)
        {
            string key = "IsBenefitCapEnabled";
            var value = RequestContext.ItemGet(key);
            bool returnValue;
            if (string.IsNullOrEmpty(value))
            {
                returnValue = (IntakeContext.Instance.CAProgramCode && ServicesApplicationHub.PageScheduler.IsBenefitCapEnabled(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)));
                RequestContext.ItemSet(returnValue, key);
            }
            else
            {
                returnValue = Convert.ToBoolean(value);
            }
            return returnValue;            
        }

        /// <summary>
        /// Checks for the Individual Benefits Recevied.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsIndividualbenefitsReceived(object sender)
        {
            return true;
        }
        /// <summary>
        /// Disabled School Enrollment for MA and will be enabled for all other program codes
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsSchoolEnrollmentEnabled(object sender)
        {
            return IsProcessFlowForSchoolEnrollment(IntakeContext.Instance.CAProgramCode, IntakeContext.Instance.FBProgramCode, IntakeContext.Instance.CCProgramCode);            
        }
        /// <summary>
        /// Returns the program code for the Process flow school enrollment.
        /// </summary>
        /// <param name="caProgramCode"></param>
        /// <param name="fbProgramCode"></param>
        /// <param name="ccProgramCode"></param>
        /// <returns></returns>
        private static bool IsProcessFlowForSchoolEnrollment(bool caProgramCode, bool fbProgramCode, bool ccProgramCode)
        {
            return caProgramCode || fbProgramCode || ccProgramCode;
        }
        /// <summary>
        /// Returns the Programs codes if Technical Questions are Enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsTechnicalQuestionsEnabled(object sender)
        {
            return IsProcessFlowForTechnicalQuestions(IntakeContext.Instance.CAProgramCode, IntakeContext.Instance.FBProgramCode, IntakeContext.Instance.MAProgramCode);
        }
        /// <summary>
        /// Returns the program code for the Process flow Technical Questions.
        /// </summary>
        /// <param name="caProgramCode"></param>
        /// <param name="fbProgramCode"></param>
        /// <param name="maProgramCode"></param>
        /// <returns></returns>
        private static bool IsProcessFlowForTechnicalQuestions(bool caProgramCode, bool fbProgramCode, bool maProgramCode)
        {
            return caProgramCode || fbProgramCode || maProgramCode;
        }
        /// <summary>
        /// Checking the Renewal if Primary person Enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsPrimaryPersonEnabled(object sender)
        {
            return (IsProcessFlowForPrimaryPerson(IntakeContext.Instance.IsRenewal));
        }
        /// <summary>
        /// Check the Renewal flag if Process flow for primary person
        /// </summary>
        /// <param name="renewalFlag"></param>
        /// <returns></returns>
        private static bool IsProcessFlowForPrimaryPerson(bool renewalFlag)
        {
            return !renewalFlag;
        }
        /// <summary>
        /// Checks the Case Status
        /// </summary>
        /// <returns></returns>
        public static bool IsCaseIntakePending()
        {
            if (IntakeContext.Instance.CaseMode == TechnicalBusinessLogicConstants.Intake && IntakeContext.Instance.CaseStatus == TechnicalBusinessLogicConstants.Pending)
                return true;
            else
                return false;

        }
        /// <summary>
        /// Checks the Application Voice signature Enabled and Returns MA programs and Case Status
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static bool IsVoiceSignatureEnabled(object sender)
        {
            return (IsCaseIntakePending() && IsOnlyMA());
        }
        /// <summary>
        /// Retuns only the Medical Assistance Programs
        /// </summary>
        /// <returns></returns>
        public static bool IsOnlyMA()
        {
            return (IntakeContext.Instance.MAProgramCode && !IntakeContext.Instance.CAProgramCode && !IntakeContext.Instance.CCProgramCode && !IntakeContext.Instance.DCProgramCode
                    && !IntakeContext.Instance.FBProgramCode && !IntakeContext.Instance.QMProgramCode);
        }

        /// <summary>
        /// To define the work flow page should be scheduled based on Case Mode 
        /// </summary>
        /// <returns>True to schedule the work flow page</returns>
        public static bool IsScheduledByCaseMode()
        {
            return ((IntakeContext.Instance.CaseMode == IntakeConstants.CASEMODE_RESOURCE_ASSESSMENT)
                                  || (IntakeContext.Instance.CaseMode == IntakeConstants.CASEMODE_INTAKE)
                                  || (IntakeContext.Instance.CaseMode == IntakeConstants.CASEMODE_LIS)
                                  || (IntakeContext.Instance.CaseMode == IntakeConstants.CASEMODE_SDX));
        }


        //
        /// <summary>
        /// Check if application has a spouse
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static bool IsSpousePresent(int applicationId)
        {
            bool returnValue;
            var _technicalContext = new TechnicalContextImpl();
            var count = 0;
            count =
                _technicalContext.Technical_PersonRelation.Where(
                    n =>
                        n.ApplicationEntity.ApplicationID == applicationId 
                        && (n.RelationCode == "WIF" || n.RelationCode == "HUS")
                        && (n.DeleteReasonCode == null || n.DeleteReasonCode == string.Empty) 
                        && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode == string.Empty)
                    ).Count();

            if (count > 0)
            {

                returnValue = true;
            }
            else
            {
                returnValue = false;
            }

            return returnValue;
        }
         /// <summary>
         /// Gets MCI number
         /// </summary>
         /// <param name="applicationId"></param>
         /// <returns></returns>
        public static decimal GetResourceAssesmentMciNumber(int applicationId)
        {
            
            decimal mciNumb = 0;
            var technicalContext = new TechnicalContextImpl();

            var entities =
            technicalContext.Technical_PersonRelation.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                        && (n.RelationCode == "WIF" || n.RelationCode == "HUS")
                        && (n.DeleteReasonCode == null || n.DeleteReasonCode == string.Empty)
                        && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode == string.Empty)
                        && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode == string.Empty)
                        && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode == string.Empty)
                         ).Select(n => new { n.ApplicationEntity.EntityID }).ToList();

            if(entities.Count > 0)
            {
                var person = technicalContext.Technical_PersonAdditionalAttributes.OfType<Technical_PersonAdditionalAttributes>()
                                              .WhereIn(entities.Select(p => new { PersonID = p.EntityID }).ToList())
                                              .Select(n => new { n.MCINumber,n.DateOfBirthDate}).ToList().OrderBy(n=> n.DateOfBirthDate).FirstOrDefault() ;

                if (person != null)
                {
                    mciNumb = person.MCINumber;
                }
            }

            return mciNumb;                                          
        }

            #endregion
        }
    }
