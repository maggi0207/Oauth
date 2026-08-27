///////////////////////////////////////////////////////////////////////////////////////////////////////
//
// File:      TechnicalContextOperations.cs
//
// Created On: Thursday, March 18, 2013  10:01:03 AM
// Created By: Suresh.Padarthi
//
// This file may contain sensitive and/or confidential information and may not be
// distributed without written permission of Delaware Department of Health and 
// Social Services.
//
// #      Type        User                    Date        Comment                                      
// ------ ----------- ----------------------- ----------- -------------------------------------------- 
// 10164	add	        Suresh.Padarthi        3/28/2013    Fixed conficts
// 185822	edit	    sanjay.menon           10/15/2020   Linq Optimisation. 
// 234335   add        Peyton.McCutcheon       12/10/2024   Added WICPastEnrolled logic
///////////////////////////////////////////////////////////////////////////////////////////////////////

using Dhss.Assist.WorkerWeb.BusinessLogic.Intake.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Income;
using Dhss.Assist.WorkerWeb.Entity.ApplicationEntry.Technical;
using Dhss.Assist.WorkerWeb.Entity.DataTypes;
using Dhss.Assist.WorkerWeb.Web.Implementation.Managers;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Helpers;
using Dhss.Assist.WorkerWeb.Web.Infrastructure.Services;
using Dhss.Framework;
using Dhss.Framework.Extensions;
using Dhss.Framework.Security;
using Dhss.Framework.Web.UI.Workflow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Services.Client;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{
    [ExcludeFromCodeCoverage]
    public class TechnicalContextOperations
    {

        #region Technical Common Constants
        public const string BEGIN_AND_END_MONTH_SHOULD_BE_SAME = "Effective Begin Month and End Month should be same, as history reason is not an “Administrative Error”.";
        public const string PENDING_VERIFICATION_SHOULD_END_WITH_AE = "Pending information can be ended only with a history reason of 'Administrative Error'.";
        public const string RETRO_ADDITION_NOT_ALLOWED = "Retro addition is not allowed if history reason code exists.";
        public const string livingArrangementCorrectional = "11";
        public const string FacilityName_Mandatory = "Correctional Facility Name is mandatory.";
        #endregion
        /// <summary>
        /// Local variables declaration.
        /// </summary>
        private static string LoginUserId
        {
            get { return SystemPrincipal.Current.Identity.Name; }
        }

        #region "Individual Benefits"

        /// <summary>
        /// Gets the IndividualBenefits of an individual.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Technical_IndividualBenefits> GetIndividualBenefits(int individualBenefitsId)
        {
            if (individualBenefitsId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_IndividualBenefits> result = (DataServiceQuery<Technical_IndividualBenefits>)context.Technical_IndividualBenefits
                .Where(n => n.IndividualBenefitsID == individualBenefitsId);

            return result;
        }

        /// <summary>
        /// Will create new Individual Benefits
        /// </summary>
        /// <param name="applicationId"></param>
        public static void CreateNewIndividualBenefits(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            var appEntity = context.Technical_ApplicationEntity.Where(p => p.ApplicationID == applicationId).Select(n => new { n.EntityID }).ToList();
            var isInserted = false;

            if (appEntity.Count() > 0)
            {
                var personBenefit = context.Technical_IndividualBenefits.WhereIn(
                                       appEntity
                                           .Select(p =>
                                               new
                                               {
                                                   PersonID = p.EntityID
                                               }).ToList())
                                            .Where(n => (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                                            .Select(p => new { p.PersonID }).ToList();

                foreach (var person in appEntity)
                {
                    if (personBenefit.Count() == 0 || (!personBenefit.Any(n => n.PersonID == person.EntityID)))
                    {
                        var newPerson = CreateNewIndividualBenefitsEntity(person.EntityID);
                        context.AddToTechnical_IndividualBenefits(newPerson);
                        isInserted = true;
                    }
                }
            }

            if (isInserted)
                context.SaveChanges();
        }

        /// <summary>
        /// Creates object of Technical_IndividualBenefits.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>Returns object of Technical_IndividualBenefits</returns>
        private static Technical_IndividualBenefits CreateNewIndividualBenefitsEntity(int personId)
        {
            if (personId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var newEntity = new Technical_IndividualBenefits
            {
                PersonID = personId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1,
                SequenceNumber = 1
            };

            return newEntity;
        }

        /// <summary>
        /// Gets All History Records
        /// </summary>
        /// <param name="applicationId">ApplicationID</param>
        /// <param name="beginDate">BeginDate</param>
        /// <param name="endDate">EndDate</param>
        /// <returns>Returns Object of Technical_IndividualBenefits</returns>
        public static IQueryable<Technical_IndividualBenefits> GetIndividualBenefitsHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            if (beginDate != null && endDate != null)
            {
                return context.Technical_IndividualBenefits
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                              && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                              .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                return context.Technical_IndividualBenefits
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                       n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                return context.Technical_IndividualBenefits
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                       n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetIndividualBenefitsAllActiveRecords(applicationId);
            }
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IQueryable<Technical_IndividualBenefits> GetIndividualBenefitsAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IQueryable<Technical_IndividualBenefits> allActiveRecords =
                context.Technical_IndividualBenefits
				                 .Where(                        n =>                            n.Person.ApplicationEntity.Any(	  p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&   (p.HistoryCode == null || 
					p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||  p.HistoryCode.Trim() == string.Empty)) &&
                            (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                            (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                             n.HistoryCode.Trim() == string.Empty))
                            .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }

        /// <summary>
        ///Returns ID of the IndividualBenefits Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static int GetIndividualBenefitsEndedRecId(int personId, Int16 historySeqNum)
        {
            var context = ServicesDataHub.Technical;
            var endedRec =
                context.Technical_IndividualBenefits.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)
                        && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();
            return endedRec.IndividualBenefitsID;
        }

        #endregion

        #region "Alien Refugee"

        /// <summary>
        /// Get all History Records
        /// </summary>
        /// <param name="applicationId">ApplicationID</param>
        /// <param name="beginDate">BeginDate</param>
        /// <param name="endDate">EndDate</param>
        /// <returns>Returns Object of Technical_PersonRefugee</returns>
        public static IEnumerable<Technical_PersonRefugee> GetAlienRefugeeHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_PersonRefugee> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_PersonRefugee
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId) &&
                                n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                               && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                              .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_PersonRefugee
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId) &&
                      n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_PersonRefugee
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId) &&
                       n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetAlienRefugeeAllActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>       
        /// <param name="applicationId">ApplicationID</param>
        /// <returns>Returns Object of Technical_PersonRefugee</returns>
        public static IEnumerable<Technical_PersonRefugee> GetAlienRefugeeAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_PersonRefugee> allActiveRecords = context.Technical_PersonRefugee.
                Where(
                    n =>
                        n.Person.ApplicationEntity.Any(
                            p => p.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key) &&
                                 (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                 (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                  p.HistoryCode.Trim() == string.Empty)) &&
                        (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty))
                        .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }

        /// <summary>
        /// Check for Person Refugee Exists for selected ApplicationEntityID
        /// </summary>
        /// <param name="personId">personID</param>
        /// <returns>Returns object of Technical_PersonRefugee</returns>
        public static bool IsPersonRefugeeExist(int personId)
        {
            if (personId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var techcontext = ServicesDataHub.Technical;
            return techcontext.Technical_PersonRefugee.Where(n => (n.PersonID == personId) &&
                                                                  ((n.DeleteReasonCode.Trim() == string.Empty ||
                                                                    n.DeleteReasonCode == null) &&
                                                                   (n.HistoryCode == null || n.HistoryCode.Trim() == string.Empty ||
                                                                    n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE)))
                .Count() > 0;
        }

        /// <summary>
        /// Creates New Record in PersonRefugee Table
        /// </summary>
        /// <returns>PersonRefugeeID</returns>
        public static Technical_PersonRefugee CreateNewPersonRefugeeObject()
        {
            var technicalContext = ServicesDataHub.Technical;
            var personRefugee = new Technical_PersonRefugee
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            }; //, AlienNumber = m_AlienNumber 
            technicalContext.AddToTechnical_PersonRefugee(personRefugee);
            technicalContext.SaveChanges();
            return personRefugee;
        }

        /// <summary>
        /// Creates records for individuals who have  a "NO" response for "US Citizen or National?" question on the Additional Individual Demogrphic page
        /// </summary>
        public static void CreateAlienRefugeeRecords()
        {
            using (var techcontext = ServicesDataHub.Technical)
            {
                var isRecordInserted = false;
                IEnumerable<Technical_PersonDemographics> personDemographics =
                    techcontext.Technical_PersonDemographics.Where(
                        n =>
                            (n.Person.ApplicationEntity.Any(
                                p => p.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)
                                     && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                     (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                      p.HistoryCode.Trim() == string.Empty)))
                            && (n.USCitizenNationalIndicator == false || n.USCitizenNationalIndicator == null) &&
                            ((n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                             (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                              n.HistoryCode.Trim() == string.Empty)));
                foreach (var personDemographic in personDemographics)
                {
                    if (!IsPersonRefugeeExist(Convert.ToInt32(personDemographic.PersonID)))
                    {
                        isRecordInserted = true;
                        techcontext.AddToTechnical_PersonRefugee(
                            CreateNewPersonRefugeeObject(Convert.ToInt32(personDemographic.PersonID)));
                    }
                }
                //If there is at least one record to save.
                if (isRecordInserted)
                    techcontext.SaveChanges();
            }
        }

        /// <summary>
        /// Checks if the Elien's citizenship has been changed by flipping  the response from "NO" to "YES" for "US Citizen or National?" question on the Additional Individual Demogrphic page
        /// </summary>
        public static string CheckAlienRefugeeCitizenshipChanged()
        {
            using (var techcontext = ServicesDataHub.Technical)
            {
                IEnumerable<Technical_PersonDemographics> personDemographics =
                    techcontext.Technical_PersonDemographics.Expand("Person")
                        .Where(
                            n =>
                                (n.Person.ApplicationEntity.Any(
                                    p =>
                                        p.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)
                                        && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                        (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                         p.HistoryCode.Trim() == string.Empty)))
                                && (n.USCitizenNationalIndicator == true) &&
                                ((n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                                 (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                  n.HistoryCode.Trim() == string.Empty)));
                var infoMsg = string.Empty;
                foreach (var personDemographic in personDemographics)
                {
                    if (IsPersonRefugeeExist(Convert.ToInt32(personDemographic.PersonID)))
                    {
                        infoMsg = infoMsg + personDemographic.Person.FirstName + ' ' + personDemographic.Person.LastName +
                                  " is a citizen. Alien information can be cleared.\n";
                    }
                }
                return infoMsg;
            }
        }

        /// <summary>
        /// Creates New Record in PersonRefugee Table
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>PersonRefugeeID</returns>
        public static Technical_PersonRefugee CreateNewPersonRefugeeObject(int personId)
        {
            var personRefugee = new Technical_PersonRefugee
            {
                PersonID = personId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = GetMaxHistorySeqNumOfPersonRefugeeRec(personId),
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            return personRefugee;
        }

        /// <summary>
        /// Gets Person Alien Status
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static string GetPersonAlienStatus(string personId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var personrefugee = technicalContext.Technical_PersonRefugee.Where(n => n.PersonID == Convert.ToInt32(personId));
            if (personrefugee != null && personrefugee.Count() > 0)
            {
                return personrefugee.First().AlienRegistrationStatusCode;
            }
            return "SA";
        }
        /// <summary>
        /// Gets Alien Person Date of Entry to United States
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static string GetDateOfEntry(string personId)
        {
            var technicalContext = ServicesDataHub.Technical;
            var personrefugee = technicalContext.Technical_PersonRefugee.Where(n => n.PersonID == Convert.ToInt32(personId));
            if (personrefugee != null && personrefugee.Count() > 0)
            {
                return personrefugee.First().LawfulEntryDate.ToString();
            }
            return "SA";
        }

        /// <summary>
        ///Returns ID of the Alien Refugee Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static Technical_PersonRefugee GetPersonRefugeeEndedRecID(int personId, Int16 historySeqNum)
        {
            var context = ServicesDataHub.Technical;
            //Modified for Defect 38880
            var AlienRefugeeRec = context.Technical_PersonRefugee.Where(
                n =>
                    n.PersonID == personId &&
                    (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                     n.HistoryCode.Trim() == string.Empty)
                ).OrderByDescending(n => n.HistorySequenceNumber);
            return AlienRefugeeRec.First();
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfPersonRefugeeRec(int personId)
        {
            Int16 historySeqNum = 1;
            var techcontext = ServicesDataHub.Technical;
            var maxPersonRefuggeRecord = techcontext.Technical_PersonRefugee.Where(n => n.PersonID == personId).OrderByDescending(n => n.HistorySequenceNumber);
            if (maxPersonRefuggeRecord.Count() > 0)
            {
                historySeqNum = Convert.ToInt16(maxPersonRefuggeRecord.First().HistorySequenceNumber);
                historySeqNum++;
            }
            return historySeqNum;
        }

        /// <summary>
        /// Updates AlienRegistrationTypeCode after sync is called as this field doesn't have DB2 column it is synced manually on SQL
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <param name="alienRegistrationTypeCode"></param>
        public static void UpdateAlienRegistrationTypeCode(int personId, Int16 historySeqNum, string alienRegistrationTypeCode)
        {
            var context = ServicesDataHub.Technical;
            var personRefugee = context.Technical_PersonRefugee.Where(n => n.PersonID == personId && n.HistorySequenceNumber == historySeqNum).FirstOrDefault();

            if (personRefugee != null)
            {
                personRefugee.AlienRegistrationTypeCode = alienRegistrationTypeCode;
                personRefugee.SyncState = 4; // sync state = 4, so the sync state need not be updated by the Z-service
                context.UpdateObject(personRefugee);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Gets AlienRefugee current record
        /// </summary>
        /// <param name="context"></param>
        /// <param name="personId"></param>
        /// <returns></returns>       
        public static Technical_PersonRefugee GetAlienRegisrationCurrentRecord(TechnicalContextImpl context, int personId)
        {
            var personRefugee =
                context.Technical_PersonRefugee.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)).FirstOrDefault();
            return personRefugee;
        }

        /// <summary>
        /// Gets Alien Registration status code value from Database, if any 
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static string GetAlienRegistrationStatusCode(Int32 personId)
        {
            var context = ServicesDataHub.Technical;

            var alienRefugeeContext = TechnicalSessionContext.Instance.PersonRefugeeID != 0
                ? context.Technical_PersonRefugee.Where(
                    n => n.PersonRefugeeID == TechnicalSessionContext.Instance.PersonRefugeeID).FirstOrDefault()
                : context.Technical_PersonRefugee.Where(
                    n =>
                        n.PersonID == personId &&
                        ((n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty) &&
                         (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty))).FirstOrDefault();

            if (alienRefugeeContext != null && !String.IsNullOrEmpty(alienRefugeeContext.AlienRegistrationStatusCode))
            {
                return alienRefugeeContext.AlienRegistrationStatusCode;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets Alien Registration Type code from Database, if any 
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static string GetAlienRegistrationTypeCode(Int32 personId)
        {
            var context = ServicesDataHub.Technical;
            var alienRefugeeContext = TechnicalSessionContext.Instance.PersonRefugeeID != 0
                ? context.Technical_PersonRefugee.Where(
                    n => n.PersonRefugeeID == TechnicalSessionContext.Instance.PersonRefugeeID).FirstOrDefault()
                : context.Technical_PersonRefugee.Where(
                    n =>
                        n.PersonID == personId &&
                        ((n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                          n.HistoryCode.Trim() == string.Empty) &&
                         (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty))).FirstOrDefault();
            if (alienRefugeeContext != null && !String.IsNullOrEmpty(alienRefugeeContext.AlienRegistrationTypeCode))
            {
                return alienRefugeeContext.AlienRegistrationTypeCode;
            }
            return string.Empty;
        }

        #endregion

        #region "Pregnancy"

        /// <summary>
        /// Get all History Records
        /// </summary>
        /// <param name="applicationId">ApplicationID</param>
        /// <param name="beginDate">BeginDate</param>
        /// <param name="endDate">EndDate</param>
        /// <returns>Returns Object of Technical_Pregnancy</returns>
        public static IEnumerable<Technical_Pregnancy> GetPregnancyHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_Pregnancy> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords =
                    context.Technical_Pregnancy.Expand("Person,Person/PersonAdditionalAttributes")
                        .Where(
                            n =>
                                n.Person.ApplicationEntity.Any(
                                    p =>
                                        p.ApplicationID == applicationId &&
                                        (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                        (p.HistoryCode == null ||
                                         p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                         p.HistoryCode.Trim() == string.Empty)) &&
                                n.BeginDate >=
                                TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                &&
                                n.BeginDate <=
                                TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                                                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);

            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords =
                    context.Technical_Pregnancy.Expand("Person,Person/PersonAdditionalAttributes")
                        .Where(
                            n =>
                                n.Person.ApplicationEntity.Any(
                                    p =>
                                        p.ApplicationID == applicationId &&
                                        (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                        (p.HistoryCode == null ||
                                         p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                         p.HistoryCode.Trim() == string.Empty)) &&
                                n.BeginDate >=
                                TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);

            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords =
                    context.Technical_Pregnancy.Expand("Person,Person/PersonAdditionalAttributes")
                        .Where(
                            n =>
                                n.Person.ApplicationEntity.Any(
                                    p =>
                                        p.ApplicationID == applicationId &&
                                        (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                        (p.HistoryCode == null ||
                                         p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                         p.HistoryCode.Trim() == string.Empty)) &&
                                n.BeginDate <=
                                TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetPregnancyAllActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>       
        /// <param name="applicationId">ApplicationID</param>
        /// <returns>Returns Object of Technical_PersonRefugee</returns>
        public static IEnumerable<Technical_Pregnancy> GetPregnancyAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_Pregnancy> allActiveRecords =
                context.Technical_Pregnancy.Expand("Person,Person/PersonAdditionalAttributes")
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                                    &&
                                                                    (p.DeleteReasonCode == null ||
                                                                     p.DeleteReasonCode.Trim() == string.Empty) &&
                                                                    (p.HistoryCode == null ||
                                                                     p.HistoryCode ==
                                                                     IntakeConstants.ACTIVE_RECORD_CODE ||
                                                                     p.HistoryCode.Trim() == string.Empty)) &&
                                (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                                (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                 n.HistoryCode.Trim() == string.Empty))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }

        /// <summary>
        /// Verifies for an active record exists for the selected individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static bool IsPregnancyRecordExists(int personId)
        {
            if (personId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var techcontext = ServicesDataHub.Technical;
            return techcontext.Technical_Pregnancy.Where(n => n.PersonID == personId &&
                                                              (n.DeleteReasonCode == null ||
                                                               n.DeleteReasonCode.Trim() == string.Empty) &&
                                                              (n.HistoryCode == null ||
                                                               n.HistoryCode.Trim() == string.Empty ||
                                                               n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE))
                .Count() > 0;
        }

        /// <summary>
        ///Returns ID of the pregnancy Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static Technical_Pregnancy GetPregnancyEndedRecId(int personId, Int16 historySeqNum)
        {
            var context = ServicesDataHub.Technical;
            var pregRec =
                context.Technical_Pregnancy.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)
                        && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();
            return pregRec;
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfPregnancyRec(int personId)
        {
            Int16 historySeqNum = 1;
            var techcontext = ServicesDataHub.Technical;
            var maxPregRecord = techcontext.Technical_Pregnancy.Where(n => n.PersonID == personId).OrderByDescending(n => n.HistorySequenceNumber);
            if (maxPregRecord.Count() > 0)
            {
                historySeqNum = Convert.ToInt16(maxPregRecord.First().HistorySequenceNumber);
                historySeqNum++;
            }
            return historySeqNum;
        }

        /// <summary>
        /// Adding New Pregency Details
        /// </summary>
        /// <returns></returns>
        public static Technical_Pregnancy CreateNewPregnancyEntity()
        {
            var technicalContext = ServicesDataHub.Technical;
            var pregnancy = new Technical_Pregnancy
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                SequenceNumber = 1
            };
            technicalContext.AddToTechnical_Pregnancy(pregnancy);
            technicalContext.SaveChanges();
            return pregnancy;
        }

        /// <summary>
        /// Delete newly added Pregnancy record When Click on Oops
        /// </summary>
        /// <param name="pregnancyId"></param>
        /// <returns></returns>
        public static void DeletePregnancyRecord(int pregnancyId)
        {
            var techcontext = ServicesDataHub.Technical;
            var pregnancy = techcontext.Technical_Pregnancy.Where(n => n.PregnancyID == pregnancyId).First();
            techcontext.UsePostTunneling = true;
            techcontext.DeleteObject(pregnancy);
            techcontext.SaveChanges();
        }

        /// <summary>
        /// Disable Oops button for Existing record Otherwise Enable
        /// </summary>
        public static bool IsEnableOopsPregnancy()
        {
            var techcontext = ServicesDataHub.Technical;
            IEnumerable<Technical_Pregnancy> pregnancy = ((DataServiceQuery<Technical_Pregnancy>)techcontext.Technical_Pregnancy).
                Where(
                    n =>
                        n.Person.ApplicationEntity.Any(
                            p => p.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)
                                 && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                 (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                  p.HistoryCode.Trim() == string.Empty)) &&
                        (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty));
            return !(pregnancy.Count() > 0);
        }

        #endregion

        #region "Technical Questions"

        /// <summary>
        /// Check if the application has any record in HouseholdgeneralInfo table
        /// </summary>
        /// <param name="applicationId"></param>
        public static void CheckIfRecordExistInHouseholdGeneralInfo(int applicationId)
        {
            if (applicationId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var technicalContext = ServicesDataHub.Technical;
            var householdGeneralInfoCount =
                technicalContext.Technical_HouseholdGeneralInfo.Where(n => n.ApplicationID == applicationId).Count();

            if (householdGeneralInfoCount == 0)
            {
                var generalInfo = new Technical_HouseholdGeneralInfo
                {
                    ApplicationID = applicationId,
                    FirstInsertedByID = LoginUserId,
                    LastSavedByID = LoginUserId,
                    HistorySequenceNumber = 1,
                    HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                    SequenceNumber = 1
                };
                technicalContext.AddToTechnical_HouseholdGeneralInfo(generalInfo);
                technicalContext.SaveChanges();
            }
        }

        #endregion

        #region "Living Arrangements"

        /// <summary>
        /// Checks if the record exists for an Individual in the database, if not then inserts new record.
        /// </summary>
        /// <param name="applicationId"></param>
        public static void CreateNewLivingArrangements(int applicationId)
        {
            var context = ServicesDataHub.Technical;

            var appEntity = context.Technical_ApplicationEntity.Where(p => p.ApplicationID == applicationId).Select(n => new { n.EntityID }).ToList();
            var isInserted = false;

            if (appEntity.Count() > 0)
            {
                var personDemo = context.Technical_LivingArrangement.WhereIn(
                                       appEntity
                                           .Select(p =>
                                               new
                                               {
                                                   PersonID = p.EntityID
                                               }).ToList())
                                            .Where(n => (n.ApplicationID == applicationId && (n.HistoryCode == IntakeConstants.ONE_WHITE_SPACE || n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)))
                                            .Select(p => new { p.PersonID }).ToList();

                foreach (var person in appEntity)
                {
                    if (personDemo.Count() == 0 || (!personDemo.Any(n => n.PersonID == person.EntityID)))
                    {
                        var newPerson = CreateTechnicalLivingArrangementEntity(Convert.ToInt32(person.EntityID), applicationId);
                        context.AddToTechnical_LivingArrangement(newPerson);
                        isInserted = true;
                    }
                }
            }

            if (isInserted)
                context.SaveChanges();
        }

        /// <summary>
        /// Creates an object of type Technical_LivingArrangement.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        private static Technical_LivingArrangement CreateTechnicalLivingArrangementEntity(int personId, int applicationId)
        {
            if (personId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var livingArrangement = new Technical_LivingArrangement
            {
                FirstInsertedByID = LoginUserId, //TODO: Replace with current logged in UserID.
                LastSavedByID = LoginUserId,  //TODO: Replace with current logged in UserID.
                PersonID = personId,
                LivingArrngmtTypeCode = string.Empty,
                LivingArrngmtTypeVerificationCode = string.Empty,
                LivingWithVerificationCode = string.Empty,
                LivingWithCode = string.Empty,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                ApplicationID = applicationId
            };

            return livingArrangement;
        }


        /// <summary>
        /// Get all History Records
        /// </summary>
        /// <param name="applicationId">ApplicationID</param>
        /// <param name="beginDate">BeginDate</param>
        /// <param name="endDate">EndDate</param>
        /// <returns>Returns Object of Technical_LivingArrangement</returns>
        public static IEnumerable<Technical_LivingArrangement> GetLivingArrangementHistoryRecords(int applicationId,
            object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_LivingArrangement> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = (DataServiceQuery<Technical_LivingArrangement>)context.Technical_LivingArrangement
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId) &&
                            n.ApplicationID == applicationId &&
                            n.EffectiveLivingDate >=
                            TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                            &&
                            n.EffectiveLivingDate <=
                            TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                    .OrderBy(n => n.Person.PersonAdditionalAttributes.MCINumber).ThenBy(n => n.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = (DataServiceQuery<Technical_LivingArrangement>)context.Technical_LivingArrangement
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId) &&
                            n.ApplicationID == applicationId &&
                            n.EffectiveLivingDate >=
                            TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                    .OrderBy(n => n.Person.PersonAdditionalAttributes.MCINumber).ThenBy(n => n.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = (DataServiceQuery<Technical_LivingArrangement>)context.Technical_LivingArrangement
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId) &&
                            n.ApplicationID == applicationId &&
                            n.EffectiveLivingDate <=
                            TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                    .OrderBy(n => n.Person.PersonAdditionalAttributes.MCINumber).ThenBy(n => n.HistorySequenceNumber);
            }
            else
            {
                return GetLivingArrangementAllActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>       
        /// <param name="applicationId">ApplicationID</param>
        /// <returns></returns>
        public static IEnumerable<Technical_LivingArrangement> GetLivingArrangementAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_LivingArrangement> allActiveRecords =
                context.Technical_LivingArrangement
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId) &&
                            n.ApplicationID == applicationId &&
                            (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                            (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                             n.HistoryCode.Trim() == string.Empty))
                    .OrderBy(n => n.Person.PersonAdditionalAttributes.MCINumber).ThenBy(n => n.SequenceNumber).ThenBy(n => n.HistorySequenceNumber);
            return allActiveRecords;
        }

        #endregion

        #region "Common"

        /// <summary>
        /// Returns true if all the programs completed and syncState is 3
        /// </summary>       
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPageIndivContextComplete<T>(string key = null, int value = 0)
        {
            using (var context = ServicesDataHub.Technical)
            {
                context.SendingRequest2 += (sender, eventArgs) =>
                {
                    eventArgs.RequestMessage.SetHeader("DataServiceVersion", "3.0;NetFx"); //TODO:Move the version in Config or make it dynamic with current version.
                };
                //TODO: Make it dynamic later using Params Parameter with key and value.
                var queryParameters = "Person/ApplicationEntity/any(p:p/ApplicationID eq " + Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key) + ") and (DeleteReasonCode eq null or trim(DeleteReasonCode) eq '') and (HistoryCode eq null or HistoryCode eq '0' or trim(HistoryCode) eq '') and (SyncState eq null or SyncState ne 3)";

                if (key != null && key.Trim() != string.Empty && value != 0)
                    queryParameters = queryParameters + " and " + key + " ne " + value;

                var query = context.CreateQuery<T>(typeof(T).Name).AddQueryOption("$filter", queryParameters);
                var cnt = query.Count();
                return !(cnt > 0);
            }
        }

        /// <summary>
        /// Returns true if all the programs completed and syncState is 3
        /// </summary>       
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPageCaseIndivContextComplete<T>(string key = null, int value = 0)
        {
            using (var context = new TechnicalContextImpl())
            {
                context.SendingRequest2 += (sender, eventArgs) =>
                {
                    eventArgs.RequestMessage.SetHeader("DataServiceVersion", "3.0;NetFx"); //TODO:Move the version in Config or make it dynamic with current version.
                };
                //TODO: Make it dynamic later using Params Parameter with key and value.
                var queryParameters = "ApplicationEntity/ApplicationID eq " + Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key) + " and (DeleteReasonCode eq null or trim(DeleteReasonCode) eq '') and (HistoryCode eq null or HistoryCode eq '0' or trim(HistoryCode) eq '') and (SyncState eq null or SyncState ne 3)";

                if (key != null && key.Trim() != string.Empty && value != 0)
                    queryParameters = queryParameters + " and " + key + " ne " + value;

                var query = context.CreateQuery<T>(typeof(T).Name).AddQueryOption("$filter", queryParameters);
                var cnt = query.Count();
                return !(cnt > 0);
            }
        }

      

        /// <summary>
        /// Get All Persons in the Application.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static DataServiceQuery<Technical_Person> GetAllPersonsinApplication(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            return
                (DataServiceQuery<Technical_Person>)
                    context.Technical_Entity.OfType<Technical_Person>()
                        .Where(
                            n =>
                                n.ApplicationEntity.Any(
                                    p =>
                                        p.ApplicationID == applicationId &&
                                        (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                        (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                         p.HistoryCode.Trim() == string.Empty)));
        }

        #endregion

        #region "Program Of Assistance"

        #region "Program Detail"

        /// <summary>
        /// Get all History Records
        /// </summary>
        /// <param name="applicationId">ApplicationID</param>
        /// <param name="beginDate">BeginDate</param>
        /// <param name="endDate">EndDate</param>
        /// <returns>Returns Object of Technical_ProgramDetail</returns>
        public static IEnumerable<Technical_ProgramDetail> GetProgramOfAssistanceHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_ProgramDetail> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_ProgramDetail
                    .Where(n => (n.ApplicationEntity.ApplicationID == applicationId
                                 &&
                                 (n.ApplicationEntity.DeleteReasonCode == null ||
                                  n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                 &&
                                 (n.ApplicationEntity.HistoryCode == null ||
                                  n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                  n.ApplicationEntity.HistoryCode.Trim() == string.Empty))
                                &&
                                n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_ProgramDetail
                    .Where(n => (n.ApplicationEntity.ApplicationID == applicationId
                                 &&
                                 (n.ApplicationEntity.DeleteReasonCode == null ||
                                  n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                 &&
                                 (n.ApplicationEntity.HistoryCode == null ||
                                  n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                  n.ApplicationEntity.HistoryCode.Trim() == string.Empty)) &&
                                n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)));
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_ProgramDetail
                    .Where(n => (n.ApplicationEntity.ApplicationID == applicationId
                                 &&
                                 (n.ApplicationEntity.DeleteReasonCode == null ||
                                  n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                 &&
                                 (n.ApplicationEntity.HistoryCode == null ||
                                  n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                  n.ApplicationEntity.HistoryCode.Trim() == string.Empty))
                                && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }
            else
            {
                return GetProgramOfAssistanceAllActiveRecords(applicationId);
            }
            return historyRecords.OrderBy(n => n.ProgramCode);
        }


        /// <summary>
        /// Gets all active records.
        /// </summary>       
        /// <param name="applicationId">ApplicationID</param>
        /// <returns></returns>
        public static IEnumerable<Technical_ProgramDetail> GetProgramOfAssistanceAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;

            var allActiveAppEntity =
                context.Technical_ApplicationEntity.Where(n => n.ApplicationID == applicationId)
                    .Select(n => new { n.ApplicationEntityID })
                    .ToList();
            if (allActiveAppEntity.Count() > 0)
            {
                var progDetails = context.Technical_ProgramDetail.WhereIn(
                    allActiveAppEntity
                        .Select(p =>
                            new
                            {
                                RequesterNumber = p.ApplicationEntityID
                            }).ToList())
                    .Where(
                        n =>
                            (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                             n.HistoryCode.Trim() == string.Empty));
                return progDetails.OrderBy(n => n.ProgramCode);
            }
            return new List<Technical_ProgramDetail>();
        }


        /// <summary>
        /// Gets all active of Programs.
        /// </summary>       
        /// <param name="applicationId">ApplicationID</param>
        /// <returns></returns>
        public static IQueryable<Technical_ProgramDetail> GetAllProgramBenefits(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            var allActiveAppEntity =
                context.Technical_ApplicationEntity.Where(n => n.ApplicationID == applicationId)
                    .Select(n => new { n.ApplicationEntityID })
                    .ToList();
            if (allActiveAppEntity.Count() > 0)
            {
                var progDetails = context.Technical_ProgramDetail.WhereIn(
                    allActiveAppEntity
                        .Select(p =>
                            new
                            {
                                RequesterNumber = p.ApplicationEntityID
                            }).ToList())
                    .Where(
                        n =>
                            (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                             n.HistoryCode.Trim() == string.Empty));
                return progDetails;
            }
            return new List<Technical_ProgramDetail>().AsQueryable();
        }

        /// <summary>
        ///  Returns true if MA or DC or QMB program requested for a specific case.
        /// </summary>       
        /// <param name="applicationId">ApplicationID</param>
        /// <returns></returns>
        public static bool IsRequestedPrograms(int applicationId)
        {
            return ServicesApplicationHub.IntakeTechnical.IsRequestedPrograms(applicationId);            
        }

        /// <summary>
        /// Checking if the Program of Assistance programs is Enabled.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="prgcode"></param>
        /// <returns></returns>
        public static bool IsRequiredField(int applicationId, string prgcode)
        {
            var count = 0;
            var applicationPrograms = GetProgramOfAssistanceAllActiveRecords(applicationId);
            foreach (var appliedProgram in applicationPrograms)
            {
                count = ((appliedProgram.ProgramCode == prgcode) && (appliedProgram.Request == true)) ? ++count : count;
            }
            return count > 0;
        }

        /// <summary>
        /// Returns true if all the programs completed and syncState is 3
        /// </summary>       
        /// <param name="programDetailId">ApplicationID</param>
        /// <returns></returns>
        public static bool IsProgramOfAssistanceContextComplete(int programDetailId)
        {
            using (var context = ServicesDataHub.Technical)
            {
                return
                    !(context.Technical_ApplicationEntity.Where(
                        ape => ape.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)
                               && ape.ProgramDetail.Any(p =>
                                   p.Request == true && (p.SyncState == null || p.SyncState != 3) &&
                                   p.ProgramDetailID != programDetailId
                                   && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                   &&
                                   (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                    p.HistoryCode.Trim() == string.Empty))
                        ).Count() > 0);
            }
        }

        /// <summary>
        /// Returns true if all the programs completed and syncState is 3
        /// </summary>       
        /// <returns></returns>
        public static bool IsProgramOfAssistanceContextComplete()
        {
            using (var context = ServicesDataHub.Technical)
            {
                return
                    !(context.Technical_ApplicationEntity.Where(
                        ape => ape.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)
                               && ape.ProgramDetail.Any(p =>
                                   p.Request == true && (p.SyncState == null || p.SyncState != 3)
                                   && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                   &&
                                   (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                    p.HistoryCode.Trim() == string.Empty))
                        ).Count() > 0);
            }
        }

        /// <summary>
        /// Update all active records with new Case Filing date and verification Date.
        /// </summary>       
        /// <param name="applicationId">ApplicationID</param>
        /// <param name="caseFilingDate"></param>
        /// <returns></returns>
        public static void UpdateProgramOfAssistanceFilingDate(int applicationId, DateTime caseFilingDate)
        {
            var recordsUpdated = false;
            var context = ServicesDataHub.Technical;
            var allActivePrograms =
                (DataServiceQuery<Technical_ProgramDetail>)
                    context.Technical_ProgramDetail.Where(n => (n.ApplicationEntity.ApplicationID == applicationId
                                                                &&
                                                                (n.ApplicationEntity.DeleteReasonCode == null ||
                                                                 n.ApplicationEntity.DeleteReasonCode.Trim() ==
                                                                 string.Empty)
                                                                &&
                                                                (n.ApplicationEntity.HistoryCode == null ||
                                                                 n.ApplicationEntity.HistoryCode ==
                                                                 IntakeConstants.ACTIVE_RECORD_CODE ||
                                                                 n.ApplicationEntity.HistoryCode.Trim() == string.Empty))
                                                               &&
                                                               (n.DeleteReasonCode == null ||
                                                                n.DeleteReasonCode.Trim() == string.Empty) &&
                                                               (n.HistoryCode == null ||
                                                                n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                                                n.HistoryCode.Trim() == string.Empty));

            foreach (var program in allActivePrograms)
            {
                program.ProgramFilingDate = program.ProgramFilingDate == null ||
                                            Convert.ToDateTime(program.ProgramFilingDate) > caseFilingDate
                    ? program.ProgramFilingDate
                    : caseFilingDate;
                program.LastVerificationDate = program.LastVerificationDate == null ||
                                               Convert.ToDateTime(program.LastVerificationDate) > caseFilingDate
                    ? program.LastVerificationDate
                    : caseFilingDate;
                context.UpdateObject(program);
                recordsUpdated = true;
            }

            if (recordsUpdated)
                context.SaveChanges();
        }

        /// <summary>
        /// Create Records for benefits
        /// </summary>
        /// <param name="primaryApplicationEntityId"></param>
        /// <param name="applicationId"></param>
        public static void CreateIndividualProgramDetailRecords(int primaryApplicationEntityId, int applicationId, int count)
        {
            var context = ServicesDataHub.Technical;
            var programFilingDate = GetCaseFilingDate(context); //Default it to Case Filing Date.    
            if (count == 0)
            {
                var cashProgram = CreateNewProgramEntity("CA", primaryApplicationEntityId, programFilingDate);
                var childCareProgram = CreateNewProgramEntity("CC", primaryApplicationEntityId, programFilingDate);
                var disabledChildrenProgram = CreateNewProgramEntity("DC", primaryApplicationEntityId, programFilingDate);
                var foodBenefitsProgram = CreateNewProgramEntity("FS", primaryApplicationEntityId, programFilingDate);
                var medicalAssistanceProgram = CreateNewProgramEntity("MA", primaryApplicationEntityId, programFilingDate);
                var qmbProgram = CreateNewProgramEntity("QM", primaryApplicationEntityId, programFilingDate);

                context.AddToTechnical_ProgramDetail(cashProgram);
                context.AddToTechnical_ProgramDetail(childCareProgram);
                context.AddToTechnical_ProgramDetail(disabledChildrenProgram);
                context.AddToTechnical_ProgramDetail(foodBenefitsProgram);
                context.AddToTechnical_ProgramDetail(medicalAssistanceProgram);
                context.AddToTechnical_ProgramDetail(qmbProgram);
                context.SaveChanges();
            }
            else if (count < 6)
            {
                CreateMissingProgramDetailsRecord(primaryApplicationEntityId, programFilingDate, applicationId);
            }
            CreateAllIndividualRecords(applicationId);
        }

        /// <summary>
        /// If there is any missing program, then this method will create a record in program details
        /// </summary>
        /// <param name="primaryApplicationEntityId"></param>
        /// <param name="programFilingDate"></param>
        /// <param name="applicationId"></param>
        private static void CreateMissingProgramDetailsRecord(int primaryApplicationEntityId, DateTime? programFilingDate, int applicationId)
        {
            var activePrograms = GetProgramOfAssistanceAllActiveRecords(applicationId).ToList();
            var context = ServicesDataHub.Technical;
           if(!activePrograms.Any(n => n.ProgramCode == IntakeConstants.PROGRAM_CASH_ASSISTANCE))                    
                 context.AddToTechnical_ProgramDetail(CreateNewProgramEntity(IntakeConstants.PROGRAM_CASH_ASSISTANCE, primaryApplicationEntityId, programFilingDate));
           if(!activePrograms.Any(n => n.ProgramCode == IntakeConstants.PROGRAM_CHILD_CARE))                    
                 context.AddToTechnical_ProgramDetail(CreateNewProgramEntity(IntakeConstants.PROGRAM_CHILD_CARE, primaryApplicationEntityId, programFilingDate));
           if(!activePrograms.Any(n => n.ProgramCode == IntakeConstants.PROGRAM_DISABLED_CHILDREN))                    
                 context.AddToTechnical_ProgramDetail(CreateNewProgramEntity(IntakeConstants.PROGRAM_DISABLED_CHILDREN, primaryApplicationEntityId, programFilingDate));
           if(!activePrograms.Any(n => n.ProgramCode == IntakeConstants.PROGRAM_FOOD_STAMP))                    
                 context.AddToTechnical_ProgramDetail(CreateNewProgramEntity(IntakeConstants.PROGRAM_FOOD_STAMP, primaryApplicationEntityId, programFilingDate));
           if(!activePrograms.Any(n => n.ProgramCode == IntakeConstants.PROGRAM_MEDICAL_ASSISTANCE))                    
                 context.AddToTechnical_ProgramDetail(CreateNewProgramEntity(IntakeConstants.PROGRAM_MEDICAL_ASSISTANCE, primaryApplicationEntityId, programFilingDate));
           if(!activePrograms.Any(n => n.ProgramCode == IntakeConstants.PROGRAM_QUALIFIED_MEMBER_BENEFICIARY))                    
                 context.AddToTechnical_ProgramDetail(CreateNewProgramEntity(IntakeConstants.PROGRAM_QUALIFIED_MEMBER_BENEFICIARY, primaryApplicationEntityId, programFilingDate));
            context.SaveChanges();
        }

        /// <summary>
        /// Creating the Individual records.
        /// </summary>
        /// <param name="applicationId"></param>
        public static void CreateAllIndividualRecords(int applicationId)
        {
            ServicesApplicationHub.Intake.AddIndividualToPrograms(applicationId);
        }

        /// <summary>
        /// Creates Individual records in Each program table.
        /// </summary>
        /// <param name="programcode"></param>
        /// <param name="programdetailId"></param>
        /// <param name="applicationId"></param>
        private static void CreateIndividualBenefitsRecords(string programcode, int programdetailId, int applicationId)
        {
            var context =  ServicesDataHub.Technical;
            var appEntity = context.Technical_ApplicationEntity.
                Where(
                    p =>
                        p.ApplicationID == applicationId &&
                        (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                        &&
                        (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         p.HistoryCode.Trim() == string.Empty)
                ).Select(n => new { n.ApplicationEntityID });

            foreach (var ape in appEntity)
            {
                var progRequestorId = ape.ApplicationEntityID;
                switch (programcode)
                {
                    case "CA":
                        CashRecord(context, programdetailId, progRequestorId);
                        break;
                    case "CC":
                        ChildCareRecord(context, programdetailId, progRequestorId);
                        break;
                    case "DC":
                        DisabledChildrenRecord(context, programdetailId, progRequestorId);
                        break;
                    case "FS":
                        FoodBenefitsRecord(context, programdetailId, progRequestorId);
                        break;
                    case "MA":
                        MedicalAssistanceRecord(context, programdetailId, progRequestorId);
                        break;
                    case "QM":
                        QualifiedMemberBeneficiaryRecord(context, programdetailId, progRequestorId);
                        break;
                }
            }
        }

        /// <summary>
        /// Updates all individuals of a specific program.
        /// </summary>
        /// <param name="programcode"></param>
        /// <param name="programdetailId"></param>
        public static void UpdateProgramDetails(string programcode, int programdetailId)
        {
            switch (programcode)
            {
                case "CA":
                    UpdateCashRecord(programdetailId);
                    break;
                case "CC":
                    UpdateChildCareRecord(programdetailId);
                    break;
                case "DC":
                    UpdateDisabledChildrenRecord(programdetailId);
                    break;
                case "FS":
                    UpdateFoodBenefitsRecord(programdetailId);
                    break;
                case "MA":
                    UpdateMedicalAssistanceRecord(programdetailId);
                    break;
                case "QM":
                    UpdateQmbRecord(programdetailId);
                    break;
            }
        }
       /// <summary>
        /// Updates individual as Not requested program.
        /// </summary>
        /// <param name="programdetailId"></param>
        private static void UpdateCashRecord(int programdetailId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_CashProgram> progPersons = context.Technical_CashProgram.Where(n => n.ProgramDetailID == programdetailId);
            foreach (var progPerson in progPersons)
            {
                progPerson.Request = false;
                context.UpdateObject(progPerson);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Updates individual as Not requested program.
        /// </summary>
        /// <param name="programdetailId"></param>
        private static void UpdateChildCareRecord(int programdetailId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_ChildCareProgram> progPersons = context.Technical_ChildCareProgram.Where(n => n.ProgramDetailID == programdetailId);
            foreach (var progPerson in progPersons)
            {
                progPerson.Request = false;
                context.UpdateObject(progPerson);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Checks the case is in SDX Mode.
        /// </summary>
        /// <returns></returns>
        public static bool IsSDXCase()
        {
            return IntakeContext.Instance.CaseMode == "X";
        }

        /// <summary>
        /// Checks the case is in SDX Mode.
        /// </summary>
        /// <returns></returns>
        public static bool IsCaseRenewalOrReactivate()
        {
            return (IntakeContext.Instance.CaseMode == "R" || (IntakeContext.Instance.CaseMode == "O" && IntakeContext.Instance.CaseStatus == "P"));
        }

        /// <summary>
        /// Updates individual as Not requested program.
        /// </summary>
        /// <param name="programdetailId"></param>
        private static void UpdateDisabledChildrenRecord(int programdetailId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_DisabledChildrenProgram> progPersons = context.Technical_DisabledChildrenProgram.Where(n => n.ProgramDetailID == programdetailId);
            foreach (var progPerson in progPersons)
            {
                progPerson.Request = false;
                context.UpdateObject(progPerson);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Updates individual as Not requested program.
        /// </summary>
        /// <param name="programdetailId"></param>
        private static void UpdateFoodBenefitsRecord(int programdetailId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_FoodBenefitsProgram> progPersons = context.Technical_FoodBenefitsProgram.Where(n => n.ProgramDetailID == programdetailId);
            foreach (var progPerson in progPersons)
            {
                progPerson.Request = false;
                context.UpdateObject(progPerson);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Updates individual as Not requested program.
        /// </summary>
        /// <param name="programdetailId"></param>
        private static void UpdateMedicalAssistanceRecord(int programdetailId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Techincal_MedicalAssistanceProgram> progPersons = context.Techincal_MedicalAssistanceProgram.Where(n => n.ProgramDetailID == programdetailId);
            foreach (var progPerson in progPersons)
            {
                progPerson.Request = false;
                context.UpdateObject(progPerson);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Updates individual as Not requested program.
        /// </summary>
        /// <param name="programdetailId"></param>
        private static void UpdateQmbRecord(int programdetailId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_QualifiedMemberBeneficiaryProgram> progPersons = context.Technical_QualifiedMemberBeneficiaryProgram.Where(n => n.ProgramDetailID == programdetailId);
            foreach (var progPerson in progPersons)
            {
                progPerson.Request = false;
                context.UpdateObject(progPerson);
            }
            context.SaveChanges();
        }

        /// <summary>
        /// Requesting Medical Assistance Program.
        /// </summary>
        /// <param name="programDetailId">ProgramDetailID</param>
        /// <returns></returns>
        public static void RequestMAProgram(int programDetailId)
        {
            var context = ServicesDataHub.Technical;
            var programDetail = context.Technical_ProgramDetail.Where(n => n.ProgramDetailID == programDetailId).FirstOrDefault();
            if (programDetail != null)
            {
                programDetail.Request = true;
                context.UpdateObject(programDetail);
                context.SaveChanges();
            }
            else
            {
                throw new Exception("There was no program exist with your criteria."); //TODO: Change exception message if required accordingly.
            }
        }

        /// <summary>
        /// Creates ProgramDetailID based on programCode before load
        /// </summary>
        /// <param name="programCode">ProgramCode</param>
        /// <param name="primaryApplicationEntityId">ApplicationEntityID</param>
        /// <param name="programFilingDate"></param>
        /// <returns></returns>
        public static Technical_ProgramDetail CreateNewProgramEntity(string programCode, int primaryApplicationEntityId, DateTime? programFilingDate)
        {
            if (programCode == string.Empty || primaryApplicationEntityId == 0)
            {
                throw new ArgumentException("Arguments can not be zero or empty string");
            }
            var program = new Technical_ProgramDetail
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                RequesterNumber = primaryApplicationEntityId,
                ProgramCode = programCode,
                Request = false,
                HistorySequenceNumber = 1,
                ProgramFilingDate = programFilingDate,
                LastVerificationDate = SystemDateTime.Now,
                SequenceNumber = 1,
                BeginDate = programFilingDate,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            return program;
        }

        /// <summary>
        /// Returns Case filing date of an application.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static DateTime? GetCaseFilingDate(TechnicalContextImpl context)
        {
            return
                context.Technical_Case.Where(
                    n => n.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key))
                    .Select(a => a.CaseFileDate)
                    .FirstOrDefault();
        }

        /// <summary>
        /// Program details
        /// </summary>
        /// <param name="programDetailId">ProgramDetailID</param>
        /// <returns></returns>
        public static IEnumerable<Technical_ProgramDetail> ProgramDetailContext(int programDetailId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_ProgramDetail> programDetail =
                context.Technical_ProgramDetail.
                    Where(n => n.ProgramDetailID == programDetailId);
            return programDetail;
        }

        /// <summary>
        /// Program Details Context
        /// </summary>
        /// <param name="progCode">ProgramDetailID</param>
        /// <returns></returns>
        public static IEnumerable<Technical_ProgramDetail> ProgramDetailContext(string progCode)
        {
            var programDetail =
                GetAllProgramBenefits(Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key))
                    .Where(n => n.ProgramCode == progCode);
            return programDetail;
        }

        /// <summary>
        /// Checking if Program is Requested.
        /// </summary>
        /// <param name="programDetailId"></param>
        /// <returns></returns>
        public static bool IsCheckProgramRequest(int programDetailId)
        {
            var context = ServicesDataHub.Technical;
            var programDetail =
                context.Technical_ProgramDetail.Where(n => n.ProgramDetailID == programDetailId).FirstOrDefault();
            return programDetail != null && Convert.ToBoolean(programDetail.Request);
        }

        #endregion

        #region "cash"

        /// <summary>
        /// Create Cash record for particular ProgramDetailID
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="programDetailId">programDetailID</param>
        /// <param name="progRequestorId"></param>
        public static void CashRecord(TechnicalContextImpl context, int programDetailId, int progRequestorId)
        {
            if (context.Technical_CashProgram.Where(p => p.ProgramDetailID == programDetailId && p.ApplicationEntityID == progRequestorId).Count() == 0)
            {
                context.AddToTechnical_CashProgram(CreateCashProgramEntity(progRequestorId, programDetailId));
                context.SaveChanges();
                UpdateProgDetailsSyncState(programDetailId);
            }
        }

        /// <summary>
        ///  Create Cash record for particular ProgramDetailID
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <param name="programDetailsId"></param>
        /// <returns></returns>
        public static Technical_CashProgram CreateCashProgramEntity(int appEntityId, int programDetailsId)
        {
            if (appEntityId == 0 || programDetailsId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var cashProg = new Technical_CashProgram
            {
                ApplicationEntityID = appEntityId,
                ProgramDetailID = programDetailsId,
                FirstInsertedByID = LoginUserId, //TODO: Replace this with userid
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1,
                Request = false
            };

            return cashProg;
        }

        /// <summary>
        /// Updates Request for unselected individuals in cash program.
        /// </summary>
        /// <param name="technicalContext"></param>
        /// <param name="programDetailId"></param>
        public static void UpdateCashProgDeleteReasonCode(TechnicalContextImpl technicalContext, int programDetailId)
        {
            IEnumerable<Technical_CashProgram> progPersons = technicalContext.Technical_CashProgram.
                Where(n => n.ProgramDetailID == programDetailId);
            if (progPersons.FirstOrDefault() != null)
            {
                foreach (var prog in progPersons)
                {
                    prog.Request = false; //TODO: Temp Request flag
                    technicalContext.UpdateObject(prog);
                }
                technicalContext.SaveChanges();
            }
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="programDetailId"></param>
        /// <returns></returns>
        public static IQueryable<T> GetProgramRequestedIndividuals<T>(int programDetailId)
        {
            using (var techcontext = ServicesDataHub.Technical)
            {
                var query = techcontext.CreateQuery<T>(typeof(T).Name).AddQueryOption("$filter", "ProgramDetailID eq " + programDetailId);
                return query;
            }
        }

        /// <summary>
        /// Updates Sync state in parent table if child record modified
        /// </summary>
        /// <param name="programDetailId"></param>
        /// <param name="syncState"></param>
        public static void UpdateProgDetailsSyncState(int programDetailId, Int16 syncState = 1)
        {
            using (var context = ServicesDataHub.Technical)
            {
                var programDetail = context.Technical_ProgramDetail.Where(n => n.ProgramDetailID == programDetailId).FirstOrDefault();
                if (programDetail != null && programDetail.SyncState != null)
                {
                    programDetail.SyncState = syncState;
                    context.UpdateObject(programDetail);
                    context.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Updates Sync state in parent table if child record modified
        /// </summary>
        /// <param name="incompletedPrograms"></param>
        /// <param name="isBeforeSync"></param>
        public static void UpdateIncompletedProgDetailsAfterBeforeSync(List<Technical_ProgramDetail> incompletedPrograms, bool isBeforeSync)
        {
            var isRecordUpdated = false;
            using (var context = ServicesDataHub.Technical)
            {
                for (var indx = 0; indx < incompletedPrograms.Count; indx++)
                {
                    var programDetail = context.Technical_ProgramDetail.Where(n => n.ProgramDetailID == incompletedPrograms[indx].ProgramDetailID).FirstOrDefault();
                    if (programDetail != null && programDetail.SyncState != null)
                    {
                        isRecordUpdated = true;
                        programDetail.SyncState = isBeforeSync == true ? Convert.ToInt16(4) : incompletedPrograms[indx].SyncState;
                        programDetail.BeginDate = incompletedPrograms[indx].BeginDate;
                        programDetail.Request = incompletedPrograms[indx].Request;
                        context.UpdateObject(programDetail);
                    }
                }
                if (isRecordUpdated)  //If there is atleast one record to update.
                    context.SaveChanges();
            }
        }

        /// <summary>
        /// Gets Sync state of Program Detail table for Retro Adds.
        /// </summary>
        /// <param name="programDetailId"></param>
        public static Int16 GetProgDetailsSyncState(int programDetailId)
        {
            using (var context = ServicesDataHub.Technical)
            {
                var syncState = Convert.ToInt16(context.Technical_ProgramDetail.Where(n => n.ProgramDetailID == programDetailId).FirstOrDefault().SyncState);
                return syncState;
            }
        }

        /// <summary>
        /// Loads selected indivduals for cash program.
        /// </summary>
        public static IList<KeyValuePair<string, string>> LoadCashIndividuals(int programDetailId)
        {
            var context = ServicesDataHub.Technical;
            var progPersons = context.Technical_CashProgram.
                 Where(n => (n.ProgramDetailID == programDetailId) && (n.DeleteReasonCode == null || n.DeleteReasonCode == string.Empty) && n.Request == true).Select(n => new { n.ApplicationEntityID });
            var appPersons = new List<KeyValuePair<string, string>>();
            var personAppEntityId = new PersonNameWithAppEntityId();
            var caseIndiv = personAppEntityId.Values.ToList();
            foreach (var prog in progPersons)
            {
                var newIndiv = caseIndiv.Find(n => n.Key == prog.ApplicationEntityID.AsString());
                if (newIndiv.Key != null)
                    appPersons.Add(newIndiv);
            }

            return appPersons;
        }

        #endregion

        #region "childcare"

        /// <summary>
        /// Create ChildCare record for particular ProgramDetailID
        /// </summary>
        /// <param name="context"></param>
        /// <param name="programDetailId"></param>
        /// <param name="progRequestorId"></param>
        public static void ChildCareRecord(TechnicalContextImpl context, int programDetailId, int progRequestorId)
        {
            if (context.Technical_ChildCareProgram.Where(p => p.ProgramDetailID == programDetailId && p.ApplicationEntityID == progRequestorId).Count() == 0)
            {
                context.AddToTechnical_ChildCareProgram(CreateChildcareProgramEntity(progRequestorId, programDetailId));
                context.SaveChanges();
                UpdateProgDetailsSyncState(programDetailId);
            }
        }

        /// <summary>
        ///  Create childcare entity record for particular ProgramDetailID
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <param name="programDetailsId"></param>
        /// <returns></returns>
        public static Technical_ChildCareProgram CreateChildcareProgramEntity(int appEntityId, int programDetailsId)
        {
            if (appEntityId == 0 || programDetailsId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var childcareProg = new Technical_ChildCareProgram
            {
                ApplicationEntityID = appEntityId,
                ProgramDetailID = programDetailsId,
                FirstInsertedByID = LoginUserId, //TODO: Replace this with userid
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1,
                Request = false
            };

            return childcareProg;
        }

        /// <summary>
        /// Updates Request for unselected individuals in ChildCare program.
        /// </summary>
        /// <param name="technicalContext"></param>
        /// <param name="programDetailId"></param>
        public static void UpdateChildcareProgDeleteReasonCode(TechnicalContextImpl technicalContext, int programDetailId)
        {
            IEnumerable<Technical_ChildCareProgram> progPersons =
                technicalContext.Technical_ChildCareProgram.
                    Where(n => n.ProgramDetailID == programDetailId);
            if (progPersons.FirstOrDefault() != null)
            {
                foreach (var prog in progPersons)
                {
                    prog.Request = false; //TODO: Temp Request flag
                    technicalContext.UpdateObject(prog);
                }
                technicalContext.SaveChanges();
            }
        }

        /// <summary>
        /// Loads selected indivduals for child care program.
        /// </summary>
        /// <param name="programDetailId"></param>
        public static IList<KeyValuePair<string, string>> LoadChildCareIndividuals(int programDetailId)
        {
            var context = ServicesDataHub.Technical;
            var progPersons = context.Technical_ChildCareProgram.
                Where(
                    n =>
                        n.ProgramDetailID == programDetailId && (n.DeleteReasonCode == null || n.DeleteReasonCode == string.Empty) &&
                        n.Request == true).Select(n => new { n.ApplicationEntityID });
            var appPersons = new List<KeyValuePair<string, string>>();
            var personAppEntityId = new PersonNameWithAppEntityId();
            var caseIndiv = personAppEntityId.Values.ToList();
            foreach (var prog in progPersons)
            {
                var newIndiv = caseIndiv.Find(n => n.Key == prog.ApplicationEntityID.AsString());
                if (newIndiv.Key != null)
                    appPersons.Add(newIndiv);
            }

            return appPersons;
        }

        #endregion

        #region "Disabled Children"

        /// <summary>
        /// Disabled Context
        /// </summary>
        /// <param name="programDetailId">ProgramDetailID</param>
        /// <param name="progRequestorId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_DisabledChildrenProgram> DisabledContext(int programDetailId, int progRequestorId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_DisabledChildrenProgram> diabledProgram =
                context.Technical_DisabledChildrenProgram.
                    Where(n => n.ProgramDetailID == programDetailId && n.ApplicationEntityID == progRequestorId);
            return diabledProgram;
        }

        /// <summary>
        /// Create Disabled Children record for particular ProgramDetailID
        /// </summary>
        /// <param name="context"></param>
        /// <param name="programDetailId"></param>
        /// <param name="progRequestorId"></param>
        public static void DisabledChildrenRecord(TechnicalContextImpl context, int programDetailId, int progRequestorId)
        {
            if (context.Technical_DisabledChildrenProgram.Where(p => p.ProgramDetailID == programDetailId && p.ApplicationEntityID == progRequestorId).Count() == 0)
            {
                context.AddToTechnical_DisabledChildrenProgram(CreateDisabledChildrenProgramEntity(progRequestorId, programDetailId));
                context.SaveChanges();
                UpdateProgDetailsSyncState(programDetailId);
            }
        }

        /// <summary>
        /// Updates Disabled Children record on change of requester.
        /// </summary>
        /// <param name="programDetailId"></param>
        /// <param name="progRequestorId"></param>
        /// <param name="newValues"></param>
        public static void UpdateDisabledChildrenRequesterDetails(int programDetailId, int progRequestorId, OrderedDictionary newValues)
        {
            var context = ServicesDataHub.Technical;
            var program =
                context.Technical_DisabledChildrenProgram.Where(
                    n => n.ProgramDetailID == programDetailId && n.ApplicationEntityID == progRequestorId)
                    .FirstOrDefault();
            if (program != null)
            {
                program.CRDPCode = Convert.ToString(newValues["CRDPCode"]);
                program.RetroMACode = Convert.ToString(newValues["RetroMACode"]);
                context.UpdateObject(program);
                context.SaveChanges();
            }
            else
            {
                throw new Exception("There is no record found.");
            }
        }

        /// <summary>
        ///  Create DisabledChildren entity record for particular ProgramDetailID
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <param name="programDetailsId"></param>
        /// <returns></returns>
        public static Technical_DisabledChildrenProgram CreateDisabledChildrenProgramEntity(int appEntityId, int programDetailsId)
        {
            if (appEntityId == 0 || programDetailsId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var disabledchildProg = new Technical_DisabledChildrenProgram
            {
                ApplicationEntityID = appEntityId,
                ProgramDetailID = programDetailsId,
                FirstInsertedByID = LoginUserId, //TODO: Replace this with userid
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                CRDPCode = "N",
                RetroMACode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1,
                Request = false
            };

            return disabledchildProg;
        }

        /// <summary>
        /// Updates Request for unselected individuals in disabled children program.
        /// </summary>
        /// <param name="technicalContext"></param>
        /// <param name="programDetailId"></param>
        public static void UpdateDisabledChildrenProgDeleteReasonCode(TechnicalContextImpl technicalContext, int programDetailId)
        {
            IEnumerable<Technical_DisabledChildrenProgram> progPersons = technicalContext.Technical_DisabledChildrenProgram.
                Where(n => n.ProgramDetailID == programDetailId);
            if (progPersons.FirstOrDefault() != null)
            {
                foreach (var prog in progPersons)
                {
                    prog.Request = false; //TODO: Temp Request flag
                    technicalContext.UpdateObject(prog);
                }
                technicalContext.SaveChanges();
            }
        }

        /// <summary>
        /// Loads selected indivduals for disabled children program.
        /// </summary>
        public static IList<KeyValuePair<string, string>> LoadDisabledChildrenIndividuals(int programDetailId)
        {
            var context = ServicesDataHub.Technical;
            var progPersons = context.Technical_DisabledChildrenProgram.
                Where(
                    n =>
                        (n.ProgramDetailID == programDetailId) &&
                        (n.DeleteReasonCode == null || n.DeleteReasonCode == string.Empty) && n.Request == true)
                .Select(n => new { n.ApplicationEntityID });
            var appPersons = new List<KeyValuePair<string, string>>();
            var personAppEntityId = new PersonNameWithAppEntityId();
            var caseIndiv = personAppEntityId.Values.ToList();
            foreach (var prog in progPersons)
            {
                var newIndiv = caseIndiv.Find(n => n.Key == prog.ApplicationEntityID.AsString());
                if (newIndiv.Key != null)
                    appPersons.Add(newIndiv);
            }

            return appPersons;
        }

        #endregion

        #region "Food Benefits"

        /// <summary>
        /// Food Benefits Context
        /// </summary>
        /// <param name="programDetailId">ProgramDetailID</param>
        /// <param name="progRequestorId">ProgramDetailID</param>
        /// <returns></returns>
        public static IEnumerable<Technical_FoodBenefitsProgram> FoodBenefitsContext(int programDetailId, int progRequestorId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_FoodBenefitsProgram> foodBenefitsProgram = context.Technical_FoodBenefitsProgram.
                Where(n => n.ProgramDetailID == programDetailId && n.ApplicationEntityID == progRequestorId);
            return foodBenefitsProgram;
        }

        /// <summary>
        /// Create FoodBenefits record for particular ProgramDetailID
        /// </summary>
        /// <param name="context"></param>
        /// <param name="programDetailId"></param>
        /// <param name="progRequestorId"></param>
        public static void FoodBenefitsRecord(TechnicalContextImpl context, int programDetailId, int progRequestorId)
        {
            if (context.Technical_FoodBenefitsProgram.Where(p => p.ProgramDetailID == programDetailId && p.ApplicationEntityID == progRequestorId).Count() == 0)
            {
                context.AddToTechnical_FoodBenefitsProgram(CreateFoodBenefitsProgramEntity(progRequestorId, programDetailId));
                context.SaveChanges();
                UpdateProgDetailsSyncState(programDetailId);
            }
        }

        /// <summary>
        /// Updates FoodBenefits record on change of requester.
        /// </summary>
        /// <param name="programDetailId"></param>
        /// <param name="progRequestorId"></param>
        /// <param name="newValues"></param>
        public static void UpdateFoodBenefitsRequesterDetails(int programDetailId, int progRequestorId, OrderedDictionary newValues)
        {
            var context =  ServicesDataHub.Technical;
            var program = context.Technical_FoodBenefitsProgram.Where(n => n.ProgramDetailID == programDetailId && n.ApplicationEntityID == progRequestorId).FirstOrDefault();
            if (program != null)
            {
                if (newValues["ProtectedFilingDate"] == null)
                    program.ProtectedFilingDate = null;
                else
                    program.ProtectedFilingDate = Convert.ToDateTime(newValues["ProtectedFilingDate"]);

                if (newValues["DenialDate"] == null)
                    program.DenialDate = null;
                else
                    program.DenialDate = Convert.ToDateTime(newValues["DenialDate"]);

                if (newValues["CallBackDate"] == null)
                    program.CallBackDate = null;
                else
                    program.CallBackDate = Convert.ToDateTime(newValues["CallBackDate"]);

                program.FSIdentityCode = Convert.ToString(newValues["FSIdentityCode"]);
                program.FSIdentityVerificationCode = Convert.ToString(newValues["FSIdentityVerificationCode"]);
                program.UnabletoPrepareMealsIndicator = Convert.ToBoolean(newValues["UnabletoPrepareMealsIndicator"]);
                program.DSSDelayReasonIndicator = Convert.ToBoolean(newValues["DSSDelayReasonIndicator"]);
                context.UpdateObject(program);
                context.SaveChanges();
            }
            else
            {
                throw new Exception("There is no record found.");
            }
        }

        /// <summary>
        ///  Create Food benefits entity record for particular ProgramDetailID
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <param name="programDetailsId"></param>
        /// <returns></returns>
        public static Technical_FoodBenefitsProgram CreateFoodBenefitsProgramEntity(int appEntityId, int programDetailsId)
        {
            if (appEntityId == 0 || programDetailsId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var foodBenefitProg = new Technical_FoodBenefitsProgram
            {
                ApplicationEntityID = appEntityId,
                ProgramDetailID = programDetailsId,
                FirstInsertedByID = LoginUserId, //TODO: Replace this with userid
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1,
                Request = false,
                UnabletoPrepareMealsIndicator = false

            };

            return foodBenefitProg;
        }

        /// <summary>
        /// Updates Request for unselected individuals in food benefits  program.
        /// </summary>
        /// <param name="technicalContext"></param>
        /// <param name="programDetailId"></param>
        public static void UpdateFoodBenefitsProgDeleteReasonCode(TechnicalContextImpl technicalContext, int programDetailId)
        {
            IEnumerable<Technical_FoodBenefitsProgram> progPersons = technicalContext.Technical_FoodBenefitsProgram.
                Where(n => n.ProgramDetailID == programDetailId);
            if (progPersons.FirstOrDefault() != null)
            {
                foreach (var prog in progPersons)
                {
                    prog.Request = false; //TODO: Temp Request flag
                    technicalContext.UpdateObject(prog);
                }
                technicalContext.SaveChanges();
            }
        }

        /// <summary>
        /// Loads selected indivduals for Food benifits program.
        /// </summary>
        /// /// <param name="programDetailId"></param>
        public static IList<KeyValuePair<string, string>> LoadFoodBenefitsIndividuals(int programDetailId)
        {
            var context = ServicesDataHub.Technical;
            var progPersons = context.Technical_FoodBenefitsProgram.
                Where(n => (n.ProgramDetailID == programDetailId) && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) && n.Request == true).Select(n => new { n.ApplicationEntityID });
            var appPersons = new List<KeyValuePair<string, string>>();
            var personAppEntityId = new PersonNameWithAppEntityId();
            var caseIndiv = personAppEntityId.Values.ToList();
            foreach (var prog in progPersons)
            {
                var newIndiv = caseIndiv.Find(n => n.Key == prog.ApplicationEntityID.AsString());
                if (newIndiv.Key != null)
                    appPersons.Add(newIndiv);
            }

            return appPersons;
        }

        #endregion

        #region "Medical Assistance"

        /// <summary>
        /// Medical Assistance Context.
        /// </summary>
        /// <param name="programDetailId"></param>
        /// <param name="progRequestorId"></param>
        /// <returns></returns>
        public static IEnumerable<Techincal_MedicalAssistanceProgram> MedicalAssistanceContext(int programDetailId, int progRequestorId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Techincal_MedicalAssistanceProgram> medicalAssistanceProgram = context.Techincal_MedicalAssistanceProgram.
                Where(n => n.ProgramDetailID == programDetailId && n.ApplicationEntityID == progRequestorId);
            return medicalAssistanceProgram;
        }

        /// <summary>
        /// Create Medical Assistance record for particular ProgramDetailID
        /// </summary>
        /// <param name="context"></param>
        /// <param name="programDetailId"></param>
        /// <param name="progRequestorId"></param>
        public static void MedicalAssistanceRecord(TechnicalContextImpl context, int programDetailId, int progRequestorId)
        {
            if (context.Techincal_MedicalAssistanceProgram.Where(p => p.ProgramDetailID == programDetailId && p.ApplicationEntityID == progRequestorId).Count() == 0)
            {
                context.AddToTechincal_MedicalAssistanceProgram(CreateMedicalAssistanceProgramEntity(progRequestorId, programDetailId));
                context.SaveChanges();
                UpdateProgDetailsSyncState(programDetailId);
            }
        }

        /// <summary>
        /// Updates Medical Assistance record on change of requester.
        /// </summary>
        /// <param name="programDetailId"></param>
        /// <param name="progRequestorId"></param>
        /// <param name="newValues"></param>
        public static void UpdateMedicalAssistanceRequesterDetails(int programDetailId, int progRequestorId, OrderedDictionary newValues)
        {
            var context = ServicesDataHub.Technical;
            var program = context.Techincal_MedicalAssistanceProgram.Where(n => n.ProgramDetailID == programDetailId && n.ApplicationEntityID == progRequestorId).FirstOrDefault();
            if (program != null)
            {
                program.CRDPCode = Convert.ToString(newValues["CRDPCode"]);
                program.RetroMACode = Convert.ToString(newValues["RetroMACode"]);
                context.UpdateObject(program);
                context.SaveChanges();
            }
            else
            {
                throw new Exception("There is no record found.");
            }
        }

        /// <summary>
        ///  Create Medical Assistance entity record for particular ProgramDetailID
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <param name="programDetailsId"></param>
        /// <returns></returns>
        public static Techincal_MedicalAssistanceProgram CreateMedicalAssistanceProgramEntity(int appEntityId, int programDetailsId)
        {
            if (appEntityId == 0 || programDetailsId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var maProg = new Techincal_MedicalAssistanceProgram
            {
                ApplicationEntityID = appEntityId,
                ProgramDetailID = programDetailsId,
                FirstInsertedByID = LoginUserId, //TODO: Replace this with userid
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1,
                RetroMACode = IntakeConstants.ACTIVE_RECORD_CODE,
                CRDPCode = "N",
                Request = false
            };

            return maProg;
        }

        /// <summary>
        /// Updates Request for unselected individuals in medical assistance  program.
        /// </summary>
        /// <param name="technicalContext"></param>
        /// <param name="programDetailId"></param>
        public static void UpdateMedicalAssistanceProgDeleteReasonCode(TechnicalContextImpl technicalContext, int programDetailId)
        {
            IEnumerable<Techincal_MedicalAssistanceProgram> progPersons =
                technicalContext.Techincal_MedicalAssistanceProgram.
                    Where(n => n.ProgramDetailID == programDetailId);
            if (progPersons.FirstOrDefault() != null)
            {
                foreach (var prog in progPersons)
                {
                    prog.Request = false; //TODO: Temp Request flag
                    technicalContext.UpdateObject(prog);
                }
                technicalContext.SaveChanges();
            }
        }

        /// <summary>
        /// Loads selected indivduals for Medical Assistance  program.
        /// </summary>
        public static IList<KeyValuePair<string, string>> LoadMedicalAssistanceIndividuals(int programDetailId)
        {
            var context = ServicesDataHub.Technical;
            var progPersons = context.Techincal_MedicalAssistanceProgram.
                Where( 
				n =>(
				n.ProgramDetailID == programDetailId) &&
				 (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) && n.Request == true).Select(n => new { n.ApplicationEntityID });
            ;
            var appPersons = new List<KeyValuePair<string, string>>();
            var personAppEntityId = new PersonNameWithAppEntityId();
            var caseIndiv = personAppEntityId.Values.ToList();
            foreach (var prog in progPersons)
            {
                var newIndiv = caseIndiv.Find(n => n.Key == prog.ApplicationEntityID.AsString());
                if (newIndiv.Key != null)
                    appPersons.Add(newIndiv);
            }

            return appPersons;
        }

        #endregion

        #region "QMB"

        /// <summary>
        /// QMB Program Details
        /// </summary>
        /// <param name="programDetailId">ProgramDetailId</param>
        /// <param name="progRequestorId">ProgramRequestorId</param>
        /// <returns></returns>
        public static IEnumerable<Technical_QualifiedMemberBeneficiaryProgram> QmbProgramDetails(int programDetailId, int progRequestorId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_QualifiedMemberBeneficiaryProgram> qmbProgram = context.Technical_QualifiedMemberBeneficiaryProgram.
                Where(n => n.ProgramDetailID == programDetailId && n.ApplicationEntityID == progRequestorId);
            return qmbProgram;
        }

        /// <summary>
        /// Create Qualified Member Beneficiary record for particular ProgramDetailID
        /// </summary>
        /// <param name="context"></param>
        /// <param name="programDetailId">ProgramDetailId</param>
        /// <param name="progRequestorId">ProgramRequestorId</param>
        public static void QualifiedMemberBeneficiaryRecord(TechnicalContextImpl context, int programDetailId, int progRequestorId)
        {
            if (context.Technical_QualifiedMemberBeneficiaryProgram.Where(p => p.ProgramDetailID == programDetailId && p.ApplicationEntityID == progRequestorId).Count() == 0)
            {
                context.AddToTechnical_QualifiedMemberBeneficiaryProgram(CreateQualifiedMemberBeneficiaryProgramEntity(progRequestorId, programDetailId));
                context.SaveChanges();
                UpdateProgDetailsSyncState(programDetailId);
            }

        }

        /// <summary>
        /// Updates Qualified Member Beneficiary record on change of requester.
        /// </summary>
        /// <param name="programDetailId"></param>
        /// <param name="progRequestorId"></param>
        /// <param name="newValues"></param>
        public static void UpdateQualifiedMemberBeneficiaryRequesterDetails(int programDetailId, int progRequestorId, OrderedDictionary newValues)
        {
            var context = ServicesDataHub.Technical;
            var program = context.Technical_QualifiedMemberBeneficiaryProgram.Where(n => n.ProgramDetailID == programDetailId && n.ApplicationEntityID == progRequestorId).FirstOrDefault();
            if (program != null)
            {
                program.CRDPCode = Convert.ToString(newValues["CRDPCode"]);
                program.RetroMACode = Convert.ToString(newValues["RetroMACode"]);
                context.UpdateObject(program);
                context.SaveChanges();
            }
            else
            {
                throw new Exception("There is no record found.");
            }
        }

        /// <summary>
        ///  Create QMB benefits entity record for particular ProgramDetailID
        /// </summary>
        /// <param name="mAppEntityId"></param>
        /// <param name="programDetailsId"></param>
        /// <returns></returns>
        public static Technical_QualifiedMemberBeneficiaryProgram CreateQualifiedMemberBeneficiaryProgramEntity(int mAppEntityId, int programDetailsId)
        {
            if (mAppEntityId == 0 || programDetailsId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var qmbProg = new Technical_QualifiedMemberBeneficiaryProgram
            {
                ApplicationEntityID = mAppEntityId,
                ProgramDetailID = programDetailsId,
                FirstInsertedByID = LoginUserId, //TODO: Replace this with userid
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                CRDPCode = "N",
                RetroMACode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1,
                Request = false
            };

            return qmbProg;
        }

        /// <summary>
        /// Updates Request for unselected individuals in medical assistance  program.
        /// </summary>
        /// <param name="technicalContext"></param>
        /// <param name="programDetailId"></param>
        public static void UpdateQualifiedMemberBeneficiaryProgDeleteReasonCode(TechnicalContextImpl technicalContext, int programDetailId)
        {
            IEnumerable<Technical_QualifiedMemberBeneficiaryProgram> progPersons =
                technicalContext.Technical_QualifiedMemberBeneficiaryProgram.
                    Where(n => n.ProgramDetailID == programDetailId);
            if (progPersons.FirstOrDefault() != null)
            {
                foreach (var prog in progPersons)
                {
                    prog.Request = false; //TODO: Temp Request flag
                    technicalContext.UpdateObject(prog);
                }
                technicalContext.SaveChanges();
            }
        }

        /// <summary>
        /// Loads selected indivduals for QMB  program.
        /// </summary>
        /// <param name="programDetailId"></param>
        public static IList<KeyValuePair<string, string>> LoadQualifiedMemberBeneficiaryIndividuals(int programDetailId)
        {
            var context = ServicesDataHub.Technical;
            var progPersons = context.Technical_QualifiedMemberBeneficiaryProgram.
                Where(
                    n =>
                        (n.ProgramDetailID == programDetailId) &&
                        (n.DeleteReasonCode == null || n.DeleteReasonCode == string.Empty) && n.Request == true)
                .Select(n => new { n.ApplicationEntityID });
            var appPersons = new List<KeyValuePair<string, string>>();
            var personAppEntityId = new PersonNameWithAppEntityId();
            var caseIndiv = personAppEntityId.Values.ToList();
            foreach (var prog in progPersons)
            {
                var newIndiv = caseIndiv.Find(n => n.Key == prog.ApplicationEntityID.AsString());
                if (newIndiv.Key != null)
                    appPersons.Add(newIndiv);
            }

            return appPersons;
        }

        #endregion

        #endregion

        #region "Foster Care Adoption"
        /// <summary>
        /// Adding the New Foster Care Adoption details.
        /// </summary>
        /// <returns></returns>
        public static Technical_FosterCare CreateNewFosterCareAdoptionEntity()
        {
            var technicalContext = ServicesDataHub.Technical;
            var fosterCareAdoption = new Technical_FosterCare
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            technicalContext.AddToTechnical_FosterCare(fosterCareAdoption);
            technicalContext.SaveChanges();
            return fosterCareAdoption;
        }

        /// <summary>
        /// Verifies for an active record exists for the selected individual.
        /// </summary>
        /// <param name="personId">ApplicationEntityID</param>
        /// <returns></returns>
        public static bool IsFosterCareAdoptionRecordExists(int personId)
        {
            if (personId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var techcontext = ServicesDataHub.Technical;
            return techcontext.Technical_FosterCare.Where(n => n.PersonID == personId &&
                                                               (n.DeleteReasonCode == null ||
                                                                n.DeleteReasonCode.Trim() == string.Empty) &&
                                                               (n.HistoryCode == null ||
                                                                n.HistoryCode.Trim() == string.Empty ||
                                                                n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE))
                .Count() > 0;
        }

        /// <summary>
        /// Get all History Records
        /// </summary>
        /// <param name="applicationId">ApplicationID</param>
        /// <param name="beginDate">BeginDate</param>
        /// <param name="endDate">EndDate</param>
        /// <returns>Returns Object of Technical_FosterCare</returns>
        public static IEnumerable<Technical_FosterCare> GetFosterCareHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_FosterCare> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_FosterCare
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                    &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                            && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                              .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_FosterCare
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                    &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_FosterCare
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                    &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetFosterCareAllActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>      
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_FosterCare> GetFosterCareAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_FosterCare> allActiveRecords =
                context.Technical_FosterCare.Where(
                    n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId &&
                                                             (p.DeleteReasonCode == null ||
                                                              p.DeleteReasonCode.Trim() == string.Empty) &&
                                                             (p.HistoryCode == null ||
                                                              p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                                              p.HistoryCode.Trim() == string.Empty))
                         && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                         (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                          n.HistoryCode.Trim() == string.Empty))
                 .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }

        /// <summary>
        ///Returns ID of the FosterCare Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static int GetFosterCareEndedRecId(int personId, Int16 historySeqNum)
        {
            var context = ServicesDataHub.Technical;
            var endedRec =
                context.Technical_FosterCare.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)
                        && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();

            return endedRec.FosterCareID;
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfFosterCareRec(int personId)
        {
            Int16 historySeqNum = 1;

            var techcontext = ServicesDataHub.Technical;
            var maxRecord = techcontext.Technical_FosterCare.Where(n => n.PersonID == personId).OrderByDescending(n => n.HistorySequenceNumber);
            if (maxRecord.Count() > 0)
            {
                historySeqNum = Convert.ToInt16(maxRecord.First().HistorySequenceNumber);
                historySeqNum++;
            }

            return historySeqNum;
        }

        #endregion

        #region "Household Relations"

        /// <summary>
        /// Creates Blank record before going to Details page
        /// </summary>
        /// <param name="applicationEntityId"></param>
        /// <param name="applicationId"></param>
        public static void CreateHouseholdRelations(int applicationEntityId, int applicationId)
        {

            //CreateNew reocrds.
            var context = ServicesDataHub.Technical;
            var isRecordInserted = false;
            IEnumerable<Technical_ApplicationEntity> personRelations =
                context.Technical_ApplicationEntity.Where(
                    p => p.ApplicationID == applicationId && p.ApplicationEntityID != applicationEntityId
                         && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                         (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                          p.HistoryCode.Trim() == string.Empty));

            foreach (var appEntityitem in personRelations)
            {
                if (!IsHouseholdRelationExists(applicationEntityId, appEntityitem.ApplicationEntityID))
                {
                    var newPersonRelation = CreateNewPersonRelation(applicationEntityId,
                        appEntityitem.ApplicationEntityID);
                    isRecordInserted = true;
                    context.AddToTechnical_PersonRelation(newPersonRelation);
                }
            }
            //Atleast one record is in context to save.
            if (isRecordInserted)
                context.SaveChanges();
        }

        /// <summary>
        /// Checking the Relationships.
        /// </summary>
        /// <param name="applicationEntityId"></param>
        /// <param name="relatedAppEntityId"></param>
        /// <returns></returns>
        private static bool IsHouseholdRelationExists(int applicationEntityId, int relatedAppEntityId)
        {
            var context = ServicesDataHub.Technical;
            var appEntityRelations = context.Technical_PersonRelation.Where(n => n.ApplicationEntityID == applicationEntityId && n.RelatedApplicationEntityID == relatedAppEntityId &&
                 (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)).FirstOrDefault();

            return appEntityRelations != null;
        }

        /// <summary>
        /// Creates new object of Technical_PersonRelation
        /// </summary>
        /// <param name="applicationEntityId"></param>
        /// <param name="relatedApplicationEntityId"></param>
        /// <returns></returns>
        private static Technical_PersonRelation CreateNewPersonRelation(int applicationEntityId, int relatedApplicationEntityId)
        {
            if (applicationEntityId == 0 || relatedApplicationEntityId == 0)
                throw new ArgumentException("Arguments can not be zero");

            var personRelation = new Technical_PersonRelation
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                ApplicationEntityID = applicationEntityId,
                RelatedApplicationEntityID = relatedApplicationEntityId,
                HistorySequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                SequenceNumber = 1
            };

            return personRelation;
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfPersonRelationRec(int appEntityId)
        {
            Int16 historySeqNum = 1;
            var techcontext = ServicesDataHub.Technical;
            var maxRecord = techcontext.Technical_PersonRelation.Where(n => n.ApplicationEntityID == appEntityId).OrderByDescending(n => n.HistorySequenceNumber);
            if (maxRecord.Count() > 0)
            {
                historySeqNum = Convert.ToInt16(maxRecord.First().HistorySequenceNumber);
            }
            return historySeqNum;
        }

        /// <summary>
        ///  Saves the data using Context object and returns 1 if success else 0.
        /// </summary>
        /// <param name="personRelationObject"></param>
        public static void UpdateHouseholdRelationsData(Technical_PersonRelation personRelationObject)
        {
            ServicesTracingHub.TraceWriter.WriteLine("HouseholdRelationships.SaveData.IsUpdatedHouseholdRelations UpdateHouseholdRelationsData() - Start");

            if (personRelationObject == null)
                throw new ArgumentNullException("Argument null exception.");

            if (personRelationObject.PersonRelationID != 0)
            {
                var context = new TechnicalContextImpl();
                var personRelation = context.Technical_PersonRelation.Where(n => n.PersonRelationID == personRelationObject.PersonRelationID).FirstOrDefault();

                if (personRelation != null)
                {
                    personRelation.BeginDate = personRelationObject.BeginDate;
                    personRelation.DB2UpdatedDate = personRelationObject.DB2UpdatedDate;        // Defect42445 - PSS04072014
                    personRelation.RelationCode = personRelationObject.RelationCode;
                    personRelation.RelationVerificatonCode = personRelationObject.RelationVerificatonCode;
                    personRelation.PurchasePrepareMealsIndicator = personRelationObject.PurchasePrepareMealsIndicator;
                    personRelation.CaresforIndicator = personRelationObject.CaresforIndicator;
                    personRelation.UnablechildcareIndicator = personRelationObject.UnablechildcareIndicator;
                    personRelation.ParentalRoleForIndicator = personRelationObject.ParentalRoleForIndicator;
                    personRelation.CaretakerofIndicator = personRelationObject.CaretakerofIndicator;
                    personRelation.TaxdependentIndicator = personRelationObject.TaxdependentIndicator;
                    context.UpdateObject(personRelation);
                    context.SaveChanges();
                    HouseholRelationsReciprocalRelations(personRelation);
                }
            }
            else
            {
                throw new ArgumentException("Argument can not zero.");
            }
            ServicesTracingHub.TraceWriter.WriteLine("HouseholdRelationships.SaveData.IsUpdatedHouseholdRelations UpdateHouseholdRelationsData() - End");
        }

        /// <summary>
        /// Reciprocal Relations.
        /// </summary>
        /// <param name="personRelationObject"></param>
        public static void HouseholRelationsReciprocalRelations(Technical_PersonRelation personRelationObject)
        {
            if (personRelationObject == null)
                throw new ArgumentNullException("Argument null exception.");

            if (personRelationObject.PersonRelationID != 0)
            {
                var context = new TechnicalContextImpl();
                var personRelation =
                    context.Technical_PersonRelation.Where(
                        n => n.ApplicationEntityID == personRelationObject.RelatedApplicationEntityID &&
                             n.RelatedApplicationEntityID == personRelationObject.ApplicationEntityID
                             &&
                             (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                              n.HistoryCode.Trim() == string.Empty)
                             && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty))
                        .FirstOrDefault();

                var recRelCode = GetReciprocalRelationCode(personRelationObject.RelationCode,
                    Convert.ToInt32(personRelation.ApplicationEntityID));
                //Get Relation Code from Receprocal Reference Table.
                // Defect 39873 - Verified By Should Be Updated According to Reciprocal Relationships
                var relVerifyCode = personRelationObject.RelationVerificatonCode;
                string beginDate = Convert.ToString(personRelationObject.BeginDate);
                if (!string.IsNullOrEmpty(recRelCode) && !string.IsNullOrEmpty(relVerifyCode) && (personRelation.RelationCode != recRelCode.Trim() || personRelation.RelationVerificatonCode != relVerifyCode.Trim()) || (!string.IsNullOrEmpty(beginDate) || Convert.ToString(personRelation.BeginDate) != beginDate))
                {
                    personRelation.BeginDate = personRelationObject.BeginDate;
                    personRelation.RelationCode = recRelCode;
                    personRelation.RelationVerificatonCode = personRelationObject.RelationVerificatonCode;
                    context.UpdateObject(personRelation);
                    context.SaveChanges();
                }
            }
            else
            {
                throw new ArgumentException("Argument can not zero.");
            }
        }

        /// <summary>
        /// Get the relationship Name for the relationship code
        /// </summary>
        /// <param name="relCode"></param>
        /// <returns></returns>
        public static string GetRelationNameByCode(string relCode)
        {
            var result = ReferenceTableManager.ValueForKey("AERLCD", "REL-CD", "DESC-TXT", relCode);
            return !string.IsNullOrEmpty(result) ? result : relCode;
        }

        /// <summary>
        /// Get Reciprocal Relation Code.
        /// </summary>
        /// <param name="sourceRelCode"></param>
        /// <param name="relatedAppEntityId"></param>
        public static string GetReciprocalRelationCode(string sourceRelCode, int relatedAppEntityId)
        {
            string relationCode;
            var context = new TechnicalContextImpl();
            var person = ((DataServiceQuery<Technical_Person>)context.Technical_Entity.OfType<Technical_Person>()).Expand("PersonAdditionalAttributes")
                .Where(n => n.ApplicationEntity.Any(p => p.ApplicationEntityID == relatedAppEntityId)).FirstOrDefault();
            if (person != null)
            {
                var reciprocalContext = new HousholdReciprocalRelationsLookupContext();
                var reciprocalRelations = reciprocalContext.Values.Where(n => n.TargerSexCd == person.PersonAdditionalAttributes.GenderCode && n.SourceRelCd == sourceRelCode).FirstOrDefault();
                relationCode = reciprocalRelations != null ? reciprocalRelations.TargetRelCd : string.Empty;
            }
            else
                relationCode = string.Empty;

            return relationCode;
        }

        /// <summary>
        /// Update sync state to 1 if there is any update in reciprocal relations.
        /// </summary>
        /// <param name="applEntityId"></param>
        /// <param name="context"></param>
        public static void UpdateSyncState(int applEntityId, TechnicalContextImpl context)
        {
            var personRelations =
                context.Technical_PersonRelation.Where(
                    n =>
                        n.ApplicationEntityID == applEntityId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty) &&
                        (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty));
            foreach (var personRelation in personRelations)
            {
                personRelation.SyncState = 1;
                context.UpdateObject(personRelation);
            }
        }

        /// <summary>
        /// Validates if the selected relationship is valid
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <param name="relcode"></param>
        /// <param name="relatedApplicationEntityId"></param>
        /// <returns>Boolean</returns>
        /// <remarks>Defect 37423</remarks>
        public static bool IsValidRelation(int appEntityId, string relcode, int relatedApplicationEntityId)
        {
            var count = 0;
            if (appEntityId == 0 && relatedApplicationEntityId == 0)
                throw new ArgumentException();
            var context = ServicesDataHub.Technical;

            count = context.Technical_PersonRelation
                .Where(
                    n =>
                        n.RelatedApplicationEntityID == relatedApplicationEntityId &&
                        n.ApplicationEntityID != appEntityId && n.RelationCode == relcode &&
                        (n.ApplicationEntity.DeleteReasonCode == null ||
                         n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                        &&
                        (n.ApplicationEntity.HistoryCode == null ||
                         n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                        &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty) &&
                        (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)).Count();
            return count == 0;
        }

        /// <summary>
        /// Get the Person Full Name from Person Table using ApplicationEntityID
        /// </summary>
        /// <param name="applicationEntityId">ApplicationEntityID</param>
        /// <returns>Returns Full Name</returns>
        public static string GetPersonNameByAppEntityId(int applicationEntityId)
        {
            string name;

            if (applicationEntityId == 0)
                throw new ArgumentException("Argument can not zero.");

            var personAppEntityId = new PersonWithApplicationEntityId();
            var appPersons = personAppEntityId.Values.Where(n => n.ApplicationEntityId == applicationEntityId).FirstOrDefault();
            if (appPersons != null)
            {
                name = appPersons.Name;
                name = string.IsNullOrEmpty(name) ? "" : name.Replace("\n", string.Empty);
            }
            else
                name = string.Empty;

            return name;
        }

        /// <summary>
        /// Get the Person ID from Person Table using ApplicationEntityID
        /// </summary>
        /// <param name="applicationEntityId">ApplicationEntityID</param>
        /// <returns>Returns Full Name</returns>
        public static int GetPersonIdByAppEntityId(int applicationEntityId)
        {
            if (applicationEntityId == 0)
                throw new ArgumentException("Argument can not zero.");

            var personAppEntityId = new PersonWithApplicationEntityId();
            var appPerson = personAppEntityId.Values.Where(n => n.ApplicationEntityId == applicationEntityId).FirstOrDefault();
            int personId = appPerson != null ? appPerson.PersonId : 0;
            return personId;
        }

        /// <summary>
        /// Get all Household relations History Records.
        /// </summary>
        /// <param name="applicationId">ApplicationID</param>
        /// <param name="beginDate">BeginDate</param>
        /// <param name="endDate">EndDate</param>
        /// <returns>Returns collection of Household relations.</returns>
        public static IEnumerable<Technical_PersonRelation> GetHouseholdRelationsHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            var householdRelations = new List<Technical_PersonRelation>();
            IEnumerable<Technical_PersonRelation> personRelations;

            if (beginDate != null && endDate != null)
            {
                personRelations = context.Technical_PersonRelation.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                        && n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                                        && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                                                        .OrderBy(n => n.ApplicationEntityID).ThenBy(n => n.SequenceNumber).ThenBy(n => n.HistorySequenceNumber);   
            }
            else if (beginDate != null && endDate == null)
            {
                personRelations = context.Technical_PersonRelation.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                     && n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                                                                     .OrderBy(n => n.ApplicationEntityID).ThenBy(n => n.SequenceNumber).ThenBy(n => n.HistorySequenceNumber);   
            }
            else if (beginDate == null && endDate != null)
            {
                personRelations = context.Technical_PersonRelation.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                    && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                    .OrderBy(n => n.ApplicationEntityID).ThenBy(n => n.SequenceNumber).ThenBy(n => n.HistorySequenceNumber);   
            }
            else
            {
                return GetHouseholdRelationsActiveRecords(applicationId);
            }

            foreach (var relation in personRelations)
            {
                if (householdRelations.Count == 0 || (householdRelations.Count > 0 && !householdRelations.Any(n => n.HistorySequenceNumber == relation.HistorySequenceNumber && n.ApplicationEntityID == relation.ApplicationEntityID)))
                {
                    householdRelations.Add(relation);
                }
            }

            return householdRelations;
        }

        /// <summary>
        /// Creates Households relations.
        /// </summary>
        public static void CreateHouseholdRelationsActiveRecords(int applicationId)
        {
            var isRecordInserted = false;
            var context = ServicesDataHub.Technical;
            var casePersons = context.Technical_ApplicationEntity.Where(p => p.ApplicationID == applicationId).Select(p => new { p.ApplicationEntityID }).ToList(); ;

            //If there are more than one indivdual then create HH relations.
            if (casePersons.Count > 1)
            {
                //Looping through all individuals if there is any new individual is added or to check the relation exist for thise individual.
                foreach (var appEntityitem in casePersons)
                {
                    var relatedPersons = casePersons.Where(p => p.ApplicationEntityID != appEntityitem.ApplicationEntityID);
                    var personRelations = context.Technical_PersonRelation.Where(n => n.ApplicationEntityID == appEntityitem.ApplicationEntityID &&
                                      (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                                      (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                                      .Select(p => new { p.RelatedApplicationEntityID }).ToList();


                    //If relation does not exist in Person Relation table, then creates new entry.
                    foreach (var relatedPerson in relatedPersons)
                    {
                        if (personRelations.Count == 0 || !personRelations.Any(n => n.RelatedApplicationEntityID == relatedPerson.ApplicationEntityID))
                        {
                            var newPersonRelation = CreateNewPersonRelation(appEntityitem.ApplicationEntityID, relatedPerson.ApplicationEntityID);
                            context.AddToTechnical_PersonRelation(newPersonRelation);
                            isRecordInserted = true;
                        }
                    }

                }

                //Atleast one record is in context to save.
                if (isRecordInserted)
                    context.SaveChanges();
            }
        }

        /// <summary>
        /// To update the PersonRelation_T when a person is deleted after blank rows are created.
        /// </summary>
        /// <param name="applicationEntityId"></param>
        /// <param name="deleteReasonCode"></param>
        /// <history>
        /// Created by          Date            Defect
        /// ===========================================
        /// smanoharan          11/07/2013      40109 - Displays blank row for deleted individual
        /// </history>
        public static void DeleteHouseholdRelations(int applicationEntityId, string deleteReasonCode)
        {
            var context = ServicesDataHub.Technical;
            var personRelation = context.Technical_PersonRelation.Where(n => (n.ApplicationEntityID == applicationEntityId || n.RelatedApplicationEntityID == applicationEntityId)
                                                                            && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
                                                                            && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty));
            foreach (var person in personRelation)
            {
                if (Convert.ToInt16(person.SyncState) == 0) //If the record does not exist in DB2 Delete it from SQL server else update history reason to avoid blank records.
                    context.DeleteObject(person);
                else
                {
                    person.DeleteReasonCode = deleteReasonCode;
                    person.SyncState = 4;
                    context.UpdateObject(person);
                }
            }
            context.UsePostTunneling = true;
            context.SaveChanges();
        }

        /// <summary>
        /// Gets all Person Relations All Active Records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_PersonRelation> GetHouseholdRelationsActiveRecords(int applicationId)
        {
            //var context = ServicesDataHub.Technical;
            var householdRelations = new List<Technical_PersonRelation>();
            //IEnumerable<Technical_PersonRelation> allRecords = context.Technical_PersonRelation.Where(n => n.ApplicationEntity.ApplicationID == applicationId
            //        && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)
            //        && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)).OrderByDescending(o => o.ApplicationEntity.PrimaryPersonIndicator).ThenBy(o => o.PersonRelationID);
            var allRecords = ServicesApplicationHub.IntakeTechnical.GetPersonRelationActiveRecords(applicationId);
            foreach (var relation in allRecords)
            {
                if (householdRelations.Count == 0 || (householdRelations.Count > 0 && !householdRelations.Any(n => n.ApplicationEntityID == relation.ApplicationEntityID)))
                {
                    householdRelations.Add(relation);
                }
            }
            return householdRelations.OrderBy(n => n.ApplicationEntityID).ThenBy(n => n.SequenceNumber);
        }

        /// <summary>
        /// Gets th
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <returns></returns>
        public static DateTime GetIndividualDateofBirth(int appEntityId)
        {
            var dateOfBirth = DateTime.Today;
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_Person> person = ((DataServiceQuery<Technical_Person>)context.Technical_Entity.OfType<Technical_Person>()).Expand("PersonAdditionalAttributes").
                Where(n => n.ApplicationEntity.Any(p => p.ApplicationEntityID == appEntityId));
            foreach (var personadd in person)
            {
                dateOfBirth = Convert.ToDateTime(personadd.PersonAdditionalAttributes.DateOfBirthDate);

            }
            return dateOfBirth;
        }

        #endregion

        #region "Teen Parent Exemption"

        /// <summary>
        /// Checks if the record exists for an Individual in the database, if not then inserts new record.
        /// Creates new TeenParentExemption records for those who born on or after 12/31/1998 and age should be less than 18 and “Teen parent exemption” answered “Yes” on Additional individual demographics. 
        /// </summary>
        /// <param name="applicationId"></param>
        public static void CreateNewTeenParentExemption(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_PersonDemographics> teenParents = context.Technical_PersonDemographics.Expand(
                "Person/TeenParentExemption")
                .Where(p => p.Person.ApplicationEntity.Any(n => n.ApplicationID == applicationId
                                                                &&
                                                                (n.DeleteReasonCode == null ||
                                                                 n.DeleteReasonCode.Trim() == string.Empty) &&
                                                                (n.HistoryCode == null ||
                                                                 n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                                                 n.HistoryCode.Trim() == string.Empty))
                            && p.TeenParentPolicyExemptIndicator == true &&
                            p.Person.PersonAdditionalAttributes.DateOfBirthDate > Convert.ToDateTime("12/30/1998")
                            && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                            (p.HistoryCode == IntakeConstants.ONE_WHITE_SPACE || p.HistoryCode == null ||
                             p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)
                );

            var isNewRecord = false;

            foreach (var person in teenParents)
            {
                // Check if the individual age is less than 18
                var dob = GetPersonDOB(person.PersonID.GetValueOrDefault());
                if (TechnicalCommon.AgeOfIndividual(dob) < 18)
                {
                    if (person.Person.TeenParentExemption.Count == 0)
                    {
                        context.AddToTechnical_TeenParentExemption(
                            CreateNewTeenParentExemptionEntity((int)person.PersonID,
                                (bool)person.TeenParentPolicyExemptIndicator));
                        isNewRecord = true;
                    }
                    else
                    {
                        //Checks any active record  exists.                     
                        var activeRecord =
                            person.Person.TeenParentExemption.Where(
                                n => (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                                     (n.HistoryCode == IntakeConstants.ONE_WHITE_SPACE || n.HistoryCode == null ||
                                      n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                      n.HistoryCode.Trim() == string.Empty)).FirstOrDefault();
                        if (activeRecord == null)
                        {
                            context.AddToTechnical_TeenParentExemption(
                                CreateNewTeenParentExemptionEntity((int)person.PersonID,
                                    (bool)person.TeenParentPolicyExemptIndicator));
                            isNewRecord = true;
                        }
                    }
                }
            }
            if (isNewRecord)
                context.SaveChanges();

        }

        /// <summary>
        /// Creates object of Technical_TeenParentExemption.
        /// </summary>     
        /// <param name="personId"></param>
        /// <param name="isPolicyExemptIndicator"></param>
        /// <returns>Returns TeenParentExemption object.</returns>
        private static Technical_TeenParentExemption CreateNewTeenParentExemptionEntity(int personId, bool isPolicyExemptIndicator)
        {
            if (personId == 0)
                throw new ArgumentException("Arguments can not be zero");

            var teenParent = new Technical_TeenParentExemption
            {

                PersonID = personId,
                TeenParentPolicyExemptionCode = isPolicyExemptIndicator,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1,
                SequenceNumber = 1
            };

            return teenParent;
        }

        /// <summary>
        /// Gets All History Records
        /// </summary>
        /// <param name="applicationId">applicationID</param>
        /// <param name="beginDate">beginDate</param>
        /// <param name="endDate">endDate</param>
        /// <returns>Returns Object of Technical_TeenParentExemption</returns>
        public static IEnumerable<Technical_TeenParentExemption> GetTeenParentHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_TeenParentExemption> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_TeenParentExemption
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                                    && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                && n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)) &&
                                n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_TeenParentExemption
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_TeenParentExemption
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetTeenParentAllActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>        
        /// <param name="applicationId">ApplicationID</param>
        /// <returns>Returns Object of Technical_TeenParentExemption</returns>
        public static IEnumerable<Technical_TeenParentExemption> GetTeenParentAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_TeenParentExemption> allActiveRecords = context.Technical_TeenParentExemption
                .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                            (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                            (n.HistoryCode == IntakeConstants.ONE_WHITE_SPACE || n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }

        #endregion

        #region Alien/Refugee Sponsor

        /// <summary>
        /// Creates New record for Alien/Sponsor if no Active record exists
        /// </summary>
        /// <returns></returns>
        public static Technical_Sponsor CreateNewAlienRefugeeSponsorObject()
        {
            var techContext = ServicesDataHub.Technical;
            var address = new Technical_Address
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            techContext.AddToTechnical_Address(address);


            var sponsor = new Technical_Sponsor
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            techContext.AddToTechnical_Sponsor(sponsor);


            var sponsorOrg = new Technical_SponsorOrganization
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            techContext.AddRelatedObject(address, "SponsorOrganization", sponsorOrg);

            address.SponsorOrganization.Add(sponsorOrg);
            techContext.AddLink(sponsorOrg, "Sponsor", sponsor);
            techContext.SaveChanges();
            return sponsor;
        }


        /// <summary>
        /// Check for Sponsor Exists for selected ApplicationEntityID
        /// </summary>
        /// <param name="personId">appEntityID</param>
        /// <returns>bool</returns>
        public static bool IsSponsorExist(int personId)
        {
            if (personId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var context = ServicesDataHub.Technical;

            return context.Technical_Sponsor.Where(n => n.PersonID == personId &&
                                                        (n.DeleteReasonCode == string.Empty || n.DeleteReasonCode == null) &&
                                                        (n.HistoryCode == null || n.HistoryCode.Trim() == string.Empty ||
                                                         n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE)).Count() > 0;
        }

        /// <summary>
        /// Get all History Records
        /// </summary>
        /// <param name="applicationId">ApplicationID</param>
        /// <param name="beginDate">BeginDate</param>
        /// <param name="endDate">EndDate</param>
        /// <returns>Returns Object of Technical_PersonRefugee</returns>
        public static IEnumerable<Technical_Sponsor> GetAlienRefugeeSponsorHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_Sponsor> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_Sponsor
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_Sponsor
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_Sponsor
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetAllAlienRefugeeSponsorActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Gets all Alien Sponsor details.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_Sponsor> GetAllAlienRefugeeSponsorActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_Sponsor> allActiveRecords =
                context.Technical_Sponsor.Where(
                    n =>
                        (n.Person.ApplicationEntity.Any(
                            p =>
                                p.ApplicationID == applicationId &&
                                (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                 p.HistoryCode.Trim() == string.Empty))
                         &&
                         (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                          n.HistoryCode.Trim() == string.Empty) &&
                         (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)) ||
                        n.SponsorID == TechnicalSessionContext.Instance.SponsorID)
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxSeqNumOfSponsorRec(int personId)
        {
            Int16 seqNum = 1;

            var techcontext = ServicesDataHub.Technical;
            var maxSponsorRecord = techcontext.Technical_Sponsor.Where(n => n.PersonID == personId).OrderByDescending(n => n.SequenceNumber);
            if (maxSponsorRecord.Count() > 0)
            {
                seqNum = Convert.ToInt16(maxSponsorRecord.First().SequenceNumber);
                seqNum++;
            }

            return seqNum;
        }

        /// <summary>
        ///Returns ID of the Sponsor Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static Technical_Sponsor GetSponsorEndedRecId(int personId, Int16 historySeqNum)
        {
            var context = new TechnicalContextImpl();
            var sponsorRec =
                context.Technical_Sponsor.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)
                        && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();

            return sponsorRec;
        }

        #endregion

        # region "Child Care Additional Demographics"

        /// <summary>
        /// Creates records for  Technical_ChildCareAdditionalDemographics the first time. If the Technical_ChildCareAdditionalDemographics is null(Empty) or without any record
        /// </summary>
        public static void CreateNewChildCareAdditionalDemoRecord(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_Person> personDemo = ((DataServiceQuery<Technical_Person>)
                context.Technical_Entity.OfType<Technical_Person>()).Expand("ChildCareAdditionalDemographics").
                Where(
                    n =>
                        n.ApplicationEntity.Any(
                            p =>
                                p.ApplicationID == applicationId &&
                                (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                 p.HistoryCode.Trim() == string.Empty)));
            var isInserted = false;
            foreach (var person in personDemo)
            {
                if (person.ChildCareAdditionalDemographics.Count == 0 ||
                    (person.ChildCareAdditionalDemographics.Count != 0 && person.ChildCareAdditionalDemographics.
                        Where(n => (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                                   (n.HistoryCode == IntakeConstants.ONE_WHITE_SPACE || n.HistoryCode == null ||
                                    n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                        .FirstOrDefault() == null))
                {
                    var personDemographicsProvider = CreateChildCareAdditionalDemographicsObject(person.EntityID);
                    context.AddToTechnical_ChildCareAdditionalDemographics(personDemographicsProvider);
                    isInserted = true;
                }
            }
            if (isInserted)
                context.SaveChanges();
        }

        /// <summary>
        /// Creates a new object type of Technical_ChildCareAdditionalDemographics
        /// </summary>
        ///  /// <returns></returns>
        protected static Technical_ChildCareAdditionalDemographics CreateChildCareAdditionalDemographicsObject(int entityId)
        {
            if (entityId == 0)
                throw new ArgumentException("Arguments can not be zero");

            var personDemographicsProvider = new Technical_ChildCareAdditionalDemographics
            {
                PersonID = entityId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                SequenceNumber = 1,
                SpecialNeedIndicator = false,
                DFSReferralIndicator = false,
                ParentFeeNonPaymentIndicator = false,
                BeginDate = null                
            };
            return personDemographicsProvider;
        }

        /// <summary>
        /// Gets the ChildCare Additional Demographics details.
        /// </summary>
        /// <returns></returns>
        public static Technical_ChildCareAdditionalDemographics GetChildCareAdditionalDemographics(int personId)
        {
            var context = ServicesDataHub.Technical;
            Technical_ChildCareAdditionalDemographics childCareAddDemo;
            childCareAddDemo = TechnicalSessionContext.Instance.ChildCareDemographicsID != 0
                ? context.Technical_ChildCareAdditionalDemographics.Where(
                    n => n.ChildCareDemographicsID == TechnicalSessionContext.Instance.ChildCareDemographicsID)
                    .FirstOrDefault()
                : context.Technical_ChildCareAdditionalDemographics.Where(
                    n =>
                        n.PersonID == personId &&
                        ((n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                          n.HistoryCode.Trim() == string.Empty) &&
                         (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty))).FirstOrDefault();
            return childCareAddDemo;
        }

        /// <summary>
        /// Gets history records based on the start date and end day values
        /// </summary>
        /// <param name="applicationId">applicaionID, Start date, End date</param>
        /// <param name="beginDate"> start day for search</param>
        /// <param name="endDate">end day for the search</param>
        /// <returns>IEnumerable<Technical_ChildCareAdditionalDemographics/> type</returns>
        public static IEnumerable<Technical_ChildCareAdditionalDemographics> GetHistoryRecordsChildCare(int applicationId, Object beginDate, Object endDate)
        {
            var context = new TechnicalContextImpl();
            IEnumerable<Technical_ChildCareAdditionalDemographics> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_ChildCareAdditionalDemographics.Expand("Person,Person/PersonAdditionalAttributes").
                                   Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                   && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                    && (n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                    && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate))))
                                    .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_ChildCareAdditionalDemographics.Expand("Person,Person/PersonAdditionalAttributes").
                                    Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                    && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                    && (n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))))
                                    .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_ChildCareAdditionalDemographics.Expand("Person,Person/PersonAdditionalAttributes").
                                    Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                    && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                    && (n.BeginDate <= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(endDate))))
                                    .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetAllActiveRecordsChildCare(applicationId);
            }

            return historyRecords;
        }

        /// <summary>
        /// Gets all active records with History code IntakeConstants.ACTIVE_RECORD_CODE or empty string and with null DeleteReasoncode
        /// </summary>
        /// <param name="applicationId"> Application ID</param>
        /// <returns>IEnumerable<Technical_ChildCareAdditionalDemographics/> type</returns>
        public static IEnumerable<Technical_ChildCareAdditionalDemographics> GetAllActiveRecordsChildCare(int applicationId)
        {
            var context =new TechnicalContextImpl();
            IEnumerable<Technical_ChildCareAdditionalDemographics> childCareDemog = context.Technical_ChildCareAdditionalDemographics.Expand("Person,Person/PersonAdditionalAttributes").
                Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                  && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                                (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                                .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return childCareDemog;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static DateTime PersonDateOfBirth(int personId)
        {
            var context = ServicesDataHub.Technical;
            var personAdditional = context.Technical_PersonAdditionalAttributes.Where(n => n.PersonID == personId).FirstOrDefault();
            var dob = personAdditional.DateOfBirthDate;
            return Convert.ToDateTime(dob);
        }


        # endregion

        #region "Disability"

        /// <summary>
        /// Creats a new record
        /// </summary>
        /// <returns></returns>
        public static Technical_Disability CreateNewDisabilityRecords()
        {
            var context = ServicesDataHub.Technical;
            var disabilityProvider = CreateDisabilityObject();
            context.AddToTechnical_Disability(disabilityProvider);
            context.SaveChanges();
            return disabilityProvider;
        }

        /// <summary>
        /// Create a Technical_Disability type of object
        /// </summary>
        /// <returns></returns>
        private static Technical_Disability CreateDisabilityObject()
        {
            var disabilityProvider = new Technical_Disability
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };

            return disabilityProvider;
        }

        /// <summary>
        ///Returns ID of the disability Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static Technical_Disability GetDisabilityEndedRecId(int personId, Int16 historySeqNum)
        {
            var context = ServicesDataHub.Technical;
            var disabilityRec =
                context.Technical_Disability.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)
                        && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();

            return disabilityRec;
        }

        /// <summary>
        /// Checks an individual has an active record.
        /// </summary>
        /// <param name="personId">personID</param>
        /// <returns>Returns value true if record exists else false. </returns>
        public static bool IsDisabilityRecordExist(int personId)
        {
            if (personId == 0)
                throw new ArgumentException("Argument can not be zero.");
            var context = ServicesDataHub.Technical;

            return context.Technical_Disability
                .Where(
                    n =>
                        n.PersonID == personId &&
                        (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)).Count() > 0;
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfDisabilityRec(int personId)
        {
            Int16 historySeqNum = 1;

            var techcontext = ServicesDataHub.Technical;
            var maxRecord = techcontext.Technical_Disability.Where(n => n.PersonID == personId).OrderByDescending(n => n.HistorySequenceNumber);
            if (maxRecord.Count() > 0)
            {
                historySeqNum = Convert.ToInt16(maxRecord.First().HistorySequenceNumber);
                historySeqNum++;
            }

            return historySeqNum;
        }

        /// <summary>
        /// Gets the search result from the history records for the date/s values entered
        /// </summary>
        /// <param name="applicationId">ApplicationID</param>
        /// <param name="beginDate">BeginDate</param>
        /// <param name="endDate">EndDate</param>
        /// <returns>Returns Object Of Technical_Disability</returns>
        public static IEnumerable<Technical_Disability> GetDisablilityHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_Disability> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_Disability
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                                    && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_Disability
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                                    && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_Disability
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                                    && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetDisabilityAllActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>      
        /// <param name="applicationId"></param>
        /// <returns>Returns Object Of Technical_Disability</returns>
        public static IEnumerable<Technical_Disability> GetDisabilityAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_Disability> allActiveRecords = context.Technical_Disability.Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                                                                                                             && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }

        /// <summary>
        /// Delete newly added Disability record When Click on Oops
        /// </summary>
        /// <param name="disabilityId"></param>
        /// <returns></returns>
        public static void DeleteDisabilityRecord(int disabilityId)
        {
            var techcontext = ServicesDataHub.Technical;
            var disability = techcontext.Technical_Disability.Where(n => n.DisabilityID == disabilityId).First();
            techcontext.UsePostTunneling = true;
            techcontext.DeleteObject(disability);
            techcontext.SaveChanges();
        }


        #endregion

        #region "Institution Information"

        /// <summary>
        /// Checks if an individual has active record.
        /// </summary>
        /// <param name="personId">Application ID</param>
        /// <returns>Returns value true if record exists else false. </returns>
        public static bool IsInstitutionRecordExist(int personId)
        {
            using (var context = ServicesDataHub.Technical)
            {
                return context.Technical_InstitutionInfo
                    .Where(
                        n =>
                            n.PersonID == personId &&
                            (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                            (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                             n.HistoryCode.Trim() == string.Empty)).Count() > 0;
            }
        }

        /// <summary>
        /// Gets the data for the linqdatasource 
        /// </summary>
        /// <param name="institutionInfoId"> institution info table's InstitutionInfoID </param>
        /// <returns><Technical_InstitutionInfo/></returns>
        public static Technical_InstitutionInfo DataSourceTechnicalInstitutionDetails(int institutionInfoId)
        {
            var context = ServicesDataHub.Technical;
            Technical_InstitutionInfo techicalInstitutionInfo;
            techicalInstitutionInfo = context.Technical_InstitutionInfo.Where(n => n.InstitutionInfoID == institutionInfoId).FirstOrDefault();
            return techicalInstitutionInfo;
        }

        /// <summary>
        /// Gets all history records.
        /// </summary>
        /// <param name="applicationId">appliation id</param>
        /// <param name="beginDate"> start dat eof the search</param>
        /// <param name="endDate">end date of the search</param>
        /// <returns>IEnumerable<Technical_InstitutionInfo/></returns>
        public static IEnumerable<Technical_InstitutionInfo> GetHistoryRecordsInstitution(int applicationId, Object beginDate, Object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_InstitutionInfo> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_InstitutionInfo.
                                                          Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId &&
                                                              (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                          && (n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                                          && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate))))
                                                          .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_InstitutionInfo.
                                                         Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                             && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                                         n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                                                         .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_InstitutionInfo.
                                                         Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                             && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                                         n.BeginDate <= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(endDate)))
                                                         .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetAllActiveRecordsInstitution(applicationId);
            }

            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns>IEnumerable<Technical_InstitutionInfo/></returns>
        public static IEnumerable<Technical_InstitutionInfo> GetAllActiveRecordsInstitution(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_InstitutionInfo> activeInstitution = context.Technical_InstitutionInfo.
                                                          Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                              && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                                         (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                                                         (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                                                         .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return activeInstitution;
        }

        /// <summary>
        /// Creats a new record
        /// </summary>
        /// <returns></returns>
        public static Technical_InstitutionInfo CreateNewRecords()
        {
            var context = ServicesDataHub.Technical;
            var institutionProvider = CreateInstitutionObject();
            context.AddToTechnical_InstitutionInfo(institutionProvider);
            context.SaveChanges();
            return institutionProvider;
        }
           
        /// <summary>
        /// Creats a Technical_Disability type of object
        /// </summary> 
        /// <returns>new Technical_InstitutionInfo type of object</returns>
        protected static Technical_InstitutionInfo CreateInstitutionObject()
        {
            var institutionProvider = new Technical_InstitutionInfo
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                SequenceNumber = 1
            };
            return institutionProvider;
        }

       
        /// <summary>
        ///Returns ID of the Institution Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static int GetInstitutionEndedRecId(int personId, Int16 historySeqNum)
        {
            var context = ServicesDataHub.Technical;
            var endedRec =
                context.Technical_InstitutionInfo.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)).
                        OrderByDescending(i=>i.HistorySequenceNumber).FirstOrDefault();

            return endedRec.InstitutionInfoID;
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfInstitutionRec(int personId)
        {
            Int16 historySeqNum = 1;

            var techcontext = ServicesDataHub.Technical;
            var maxRecord = techcontext.Technical_InstitutionInfo.Where(n => n.PersonID == personId).OrderByDescending(n => n.HistorySequenceNumber);
            if (maxRecord.Count() > 0)
            {
                historySeqNum = Convert.ToInt16(maxRecord.First().HistorySequenceNumber);
                historySeqNum++;
            }

            return historySeqNum;
        }

        /// <summary>
        /// Gets the institution Info current record 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Technical_InstitutionInfo GetInstitutionCurrentRecord(TechnicalContextImpl context, int personId)
        {
            Technical_InstitutionInfo institution = context.Technical_InstitutionInfo.Where(
                n =>
                    n.PersonID == personId &&
                    (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                     n.HistoryCode.Trim() == string.Empty)).FirstOrDefault();
            return institution;
        }

        #endregion

        #region "ProtectedSSI Information"

        /// <summary>
        /// Checks if an individual has active record.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>Returns value true if record exists else false. </returns>
        public static bool IsProtectedSSIRecordExist(int personId)
        {
            if (personId == 0)
                throw new ArgumentException("Argument can not be zero.");
            var context = ServicesDataHub.Technical;

            return context.Technical_ProtectedSSI
                .Where(
                    n =>
                        n.PersonID == personId &&
                        (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)).Count() > 0;
        }

        /// <summary>
        /// Gets all history records.
        /// </summary>
        /// <param name="applicationId">appliation id</param>
        /// <param name="beginDate"> start dat eof the search</param>
        /// <param name="endDate">end date of the search</param>
        /// <returns>IEnumerable<Technical_ProtectedSSI/></returns>
        public static IEnumerable<Technical_ProtectedSSI> GetHistoryRecordsProtectedSSI(int applicationId, Object beginDate, Object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_ProtectedSSI> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_ProtectedSSI.
                                                           Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId)
                                                          && (n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                                          && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate))))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_ProtectedSSI.
                                                           Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId)
                                                               && n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_ProtectedSSI.
                                                          Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId) &&
                                                         n.BeginDate <= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(endDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetAllActiveRecordsProtectedSSI(applicationId);
            }

            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns>IEnumerable<Technical_ProtectedSSI/></returns>
        public static IEnumerable<Technical_ProtectedSSI> GetAllActiveRecordsProtectedSSI(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_ProtectedSSI> activeProtectedSSI = context.Technical_ProtectedSSI.
                                                           Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                               && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                                         (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                                                         (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return activeProtectedSSI;
        }

        /// <summary>
        /// Creats a new record
        /// </summary>
        /// <returns></returns>
        public static Technical_ProtectedSSI CreateNewProtectedSSIRecords()
        {
            var context = ServicesDataHub.Technical;
            var protectedSSIProvider = CreateProtectedSSIObject();
            context.AddToTechnical_ProtectedSSI(protectedSSIProvider);
            context.SaveChanges();
            return protectedSSIProvider;
        }

        /// <summary>
        /// Creats a Technical_Disability type of object
        /// </summary>
        /// <returns>new Technical_ProtectedSSI type of object</returns>
        protected static Technical_ProtectedSSI CreateProtectedSSIObject()
        {
            var protectedSSIProvider = new Technical_ProtectedSSI
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                SequenceNumber = 1
            };
            return protectedSSIProvider;
        }

        /// <summary>
        /// Delete newly added ProtectedSSI record When Click on Oops
        /// </summary>
        /// <param name="protectedSsiId"></param>
        /// <returns></returns>
        public static void DeleteProtectedSSIRecord(int protectedSsiId)
        {
            var techcontext = ServicesDataHub.Technical;
            var protectedSsi = techcontext.Technical_ProtectedSSI.Where(n => n.ProtectedSSIID == protectedSsiId).First();
            techcontext.DeleteObject(protectedSsi);
            techcontext.UsePostTunneling = true;
            techcontext.SaveChanges();
        }

        /// <summary>
        /// Disable Oops button for Existing record Otherwise Enable
        /// </summary>
        public static bool IsEnableOopsProtectedSSI()
        {
            var techcontext = ServicesDataHub.Technical;
            IEnumerable<Technical_ProtectedSSI> protectedSSI = techcontext.Technical_ProtectedSSI.
                                                           Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)
                                                               && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                                         (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                                                         (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty));

            return !(protectedSSI.Count() > 0);
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfProtectedImpRec(int personId)
        {
            Int16 historySeqNum = 1;

            var techcontext = ServicesDataHub.Technical;
            var maxSpousalImpRecord = techcontext.Technical_ProtectedSSI.Where(n => n.PersonID == personId).OrderByDescending(n => n.HistorySequenceNumber);
            if (maxSpousalImpRecord.Count() > 0)
            {
                historySeqNum = Convert.ToInt16(maxSpousalImpRecord.First().HistorySequenceNumber);
                historySeqNum++;
            }

            return historySeqNum;
        }

        /// <summary>
        ///Returns ID of the ProtectedSSI Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static Technical_ProtectedSSI GetProtectedSsiEndedRecId(int personId, Int16 historySeqNum)
        {
            var context = ServicesDataHub.Technical;
            var protectedSSIRec =
                context.Technical_ProtectedSSI.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)
                        && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();

            return protectedSSIRec;
        }

        #endregion

        #region "TaxDependency Information"


        /// <summary>
        /// Getting the Tax Dependency Histroy Records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_TaxDependency> GetHistoryRecordsTaxDependency(int applicationId, Object beginDate, Object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_TaxDependency> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_TaxDependency
                    .Where(n => n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                && n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                .OrderBy(n => n.ApplicationEntityID).ThenBy(n => n.HistorySequenceNumber);  
                
            }
            else if (beginDate != null && endDate == null)
            {

                historyRecords = context.Technical_TaxDependency
                    .Where(n => n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty) &&
                                n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                       .OrderBy(n => n.ApplicationEntityID).ThenBy(n => n.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_TaxDependency
                    .Where(n => n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty) &&
                                n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                       .OrderBy(n => n.ApplicationEntityID).ThenBy(n => n.HistorySequenceNumber);
            }
            else
            {
                return GetAllActiveRecordsTaxDependency(applicationId);
            }

            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns>IEnumerable<Technical_TaxDependency/></returns>
        public static IEnumerable<Technical_TaxDependency> GetAllActiveRecordsTaxDependency(int applicationId)
        {
            TechnicalContextImpl context = ServicesDataHub.Technical;
            //IEnumerable<Technical_TaxDependency> activeTaxDependency = (((DataServiceQuery<Technical_TaxDependency>)context.Technical_TaxDependency).
            //Where(n => n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
            //                && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
            //    && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty)))
            //    .OrderByDescending(n => n.ApplicationEntity.PrimaryPersonIndicator).ThenBy(n => n.ApplicationEntityID);;
            var activeTaxDependency = ServicesApplicationHub.IntakeTechnical.GetAllActiveRecordsTaxDependency(applicationId);
            return activeTaxDependency;
        }

        /// <summary>
        /// Creates new records if the PersonId is not found in the PersonDemographics table
        /// </summary>
        /// <param name="applicationId"></param>
        public static void CreateNewIndividualTax(int applicationId)
        {
            var isInserted = false;
            var context = ServicesDataHub.Technical;
            var allActiveAppEntity =
                context.Technical_ApplicationEntity.Where(n => n.ApplicationID == applicationId)
                    .Select(n => new { n.ApplicationEntityID })
                    .ToList();
            if (allActiveAppEntity.Count() > 0)
            {
                var taxDependency = context.Technical_TaxDependency.WhereIn(
                    allActiveAppEntity
                        .Select(p =>
                            new
                            {
                                ApplicationEntityID = p.ApplicationEntityID
                            }).ToList())
                    .Where(
                        n =>
                            (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                             n.HistoryCode.Trim() == string.Empty) && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)).Select(n => new { n.ApplicationEntityID}).ToList();

                allActiveAppEntity.ForEach(n => 
                {
                    if(!taxDependency.Any(p=>p.ApplicationEntityID == n.ApplicationEntityID))
                    {
                        context.AddToTechnical_TaxDependency(CreateNewTaxDependencyEntity(n.ApplicationEntityID, 1));
                        isInserted = true;
                    }
                });
                
            }
            if (isInserted)
                context.SaveChanges();
        }

        /// <summary>
        /// Checks the Deleted Individual Exists or not.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="personId"></param>
        /// <param name="applicationEntityId"></param>
        /// <returns></returns>
        public static bool IsDeletedIndividualExist(int applicationId, int personId, int applicationEntityId)
        {
            var initiateContext = ServicesDataHub.InitiateInterview;
            var count = initiateContext.InitiateInterview_ApplicationEntity.Where(n => n.ApplicationID == applicationId && n.EntityID == personId && n.ApplicationEntityID != applicationEntityId).Count();

            return count > 0;
        }

        /// <summary>
        /// Creates object of Technical_IndividualBenefits.
        /// </summary>
        /// <param name="applicationEntityId">ApplicationEntityID</param>
        /// <param name="historySeqNum"></param>
        /// <returns>Returns object of Technical_IndividualBenefits</returns>
        public static Technical_TaxDependency CreateNewTaxDependencyEntity(int applicationEntityId, Int16 historySeqNum)
        {
            if (applicationEntityId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var newEntity = new Technical_TaxDependency
            {
                ApplicationEntityID = applicationEntityId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = historySeqNum,
                SequenceNumber = 1
            };

            return newEntity;
        }

        /// <summary>
        /// Creates object of Technical_TaxDependencyDetails.
        /// </summary>
        /// <param name="taxDependentId"></param>
        /// <param name="applicationEntityId"></param>
        /// <param name="maxHisSeq"></param>
        /// <returns>Returns object of Technical_IndividualBenefits</returns>
        public static Technical_TaxDependencyDetails CreateNewTaxDependencyDetailEntity(int taxDependentId, int applicationEntityId, Int16 maxHisSeq)
        {
            if (applicationEntityId == 0)
                throw new ArgumentException("Argument can not be zero.");

            if (taxDependentId == 0)
                throw new ArgumentException("Argument can not be zero.");

            maxHisSeq = maxHisSeq != 0 ? ++maxHisSeq : GetMaxHistorySeqNumOfTaxDep(taxDependentId);

            var newEntity = new Technical_TaxDependencyDetails
            {
                TaxDependentID = taxDependentId,
                DependentApplicationEntityID = applicationEntityId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = maxHisSeq,
                SequenceNumber = 1
            };

            return newEntity;
        }

        /// <summary>
        /// To Get Sequence Number
        /// </summary>   
        private static Int16 GetMaxHistorySeqNumOfTaxDep(int taxDependentId)
        {
            Int16 historySeqNum = 1;

            using (var techcontext = ServicesDataHub.Technical)
            {
                var maxRecord = techcontext.Technical_TaxDependencyDetails.Where(n => n.TaxDependentID == taxDependentId).OrderByDescending(n => n.HistorySequenceNumber);
                if (maxRecord.Count() > 0)
                {
                    historySeqNum = Convert.ToInt16(maxRecord.First().HistorySequenceNumber);
                    historySeqNum++;
                }

                return historySeqNum;
            }
        }

        /// <summary>
        /// To Get Sequence Number
        /// </summary>   
        private static Int16 GetMaxHistorySeqNumOfTaxDependency(int appEntityId)
        {
            Int16 historySeqNum = 1;

            TechnicalContextImpl techcontext = ServicesDataHub.Technical;
            {
                var maxRecord = techcontext.Technical_TaxDependency.Where(n => n.ApplicationEntityID == appEntityId).OrderByDescending(n => n.HistorySequenceNumber);
                if (maxRecord.Count() > 0)
                {
                    var taxDep = maxRecord.First();
                    historySeqNum = Convert.ToInt16(taxDep.HistorySequenceNumber);
                    historySeqNum++;
                    //Update old record to HistoryCode 9
                    taxDep.HistoryCode = "9";
                    techcontext.UpdateObject(taxDep);
                    techcontext.SaveChanges();
                }

                return historySeqNum;
            }

        }
        /// <summary>
        /// Checks whether Tax Dependency Exists.
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <returns></returns>
        private static bool IsTaxDependencyExist(int appEntityId)
        {
            using (var techcontext = ServicesDataHub.Technical)
            {
                return techcontext.Technical_TaxDependency.Where(n => n.ApplicationEntityID == appEntityId).Count() > 0;
            }
        }

        /// <summary>
        /// Gets the data for the linqdatasource 
        /// </summary>
        /// <param name="taxDependentId"> ProtectedSSI info table's ProtectedSSIInfoID </param>
        /// <returns>IEnumerable<Technical_TaxDependency/></returns>
        public static IEnumerable<Technical_TaxDependency> DataSourceTaxDependencyDetails(int taxDependentId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_TaxDependency> techicalTaxDependencyDetails = context.Technical_TaxDependency.
                Where(n => n.TaxDependentID == taxDependentId);
            return techicalTaxDependencyDetails;
        }

        /// <summary>
        /// Updates delete reason for unselected individuals in cash program.
        /// </summary>
        /// <param name="technicalContext"></param>
        /// <param name="applicationId"></param>
        public static void UpdateTaxDependentsDeleteReasonCode(TechnicalContextImpl technicalContext, int applicationId)
        {

            IEnumerable<Technical_TaxDependency> taxDependants = technicalContext.Technical_TaxDependency.
            Where(n => n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                            && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty));

            if (taxDependants.FirstOrDefault() != null)
            {
                foreach (var dependant in taxDependants)
                {

                    dependant.FileTaxReturnInCurrentYearIndicator = "N"; //Missing business rules - Assuming
                    dependant.PrimaryTaxFilerIndicator = false; //Missing business rules - Assuming
                    technicalContext.UpdateObject(dependant);
                }
                technicalContext.SaveChanges();
            }
        }

        /// <summary>
        /// Updates UpdateTaxDeductionIndicator.
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <param name="historySeqNum"></param>
        /// <param name="taxDeductionIndc"></param>
        public static void UpdateTaxDeductionIndicator(int appEntityId, Int16 historySeqNum, bool taxDeductionIndc)
        {
            var context = ServicesDataHub.Technical;
            var taxDependent = context.Technical_TaxDependency.Where(n => n.ApplicationEntityID == appEntityId && n.HistorySequenceNumber == historySeqNum).FirstOrDefault();

            if (taxDependent != null)
            {
                taxDependent.HasTaxDeductionIndicator = taxDeductionIndc; //TODO: After adding this column in DB2 side, need to remove this.//Missing business rules  - Temp
                taxDependent.SyncState = 4;
                context.UpdateObject(taxDependent);
                context.SaveChanges();
            }
        }
        /// <summary>
        /// Checking the Tax Dependency Indicator.
        /// </summary>
        /// <param name="appEntityID"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        private static bool IsGetTaxDeductionIndicator(int appEntityID, Int16 historySeqNum)
        {
            bool hasTaxDeduction = false;
            TechnicalContextImpl techcontext = ServicesDataHub.Technical;
            {
                return hasTaxDeduction = Convert.ToBoolean(techcontext.Technical_TaxDependency.Where(n => n.ApplicationEntityID == appEntityID && n.HistorySequenceNumber == historySeqNum - 1).FirstOrDefault().HasTaxDeductionIndicator);
            }
        }
        /// <summary>
        /// Loads selected indivduals for TaxDependencyDetails Page
        /// </summary>
        /// <param name="taxDependentId"></param>
        public static IList<KeyValuePair<string, string>> LoadTaxDependencyIndividuals(int taxDependentId, bool isHistory)
        {
            var taxDependencydetails = GetTaxDependencyDetails(taxDependentId, isHistory).Select(n => new { n.DependentApplicationEntityID });
            var persons = new List<KeyValuePair<string, string>>();
            var personPersonId = new PersonNameWithAppEntityId();
            var personList = new List<KeyValuePair<string, string>>();
            foreach (var persontaxDetails in taxDependencydetails)
            {
                if (personList.Count == 0)
                    personList = personPersonId.Values.ToList();

                var newPer = personList.Find(n => n.Key == persontaxDetails.DependentApplicationEntityID.AsString()); ;
                if (newPer.Key != null)
                    persons.Add(newPer);
            }

            return persons;
        }
        /// <summary>
        /// Updates delete reason for unselected individuals in food benefits  program.
        /// </summary>
        /// <param name="taxDependentId"></param>
        public static void UpdateTaxDeleteReasonCode(int taxDependentId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_TaxDependencyDetails> taxPersons = context.Technical_TaxDependencyDetails.
                Where(n => n.TaxDependentID == taxDependentId);
            if (taxPersons.FirstOrDefault() != null)
            {
                foreach (var dependent in taxPersons)
                {

                    dependent.HistoryCode = "9";
                    dependent.DeleteReasonCode = "D"; //TODO: Logical Delete of Dependant.
                    context.UpdateObject(dependent);
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates delete reason for unselected individuals in food benefits  program.
        /// </summary>
        /// <param name="taxDependentId"></param>
        public static IEnumerable<Technical_TaxDependencyDetails> GetTaxDependencyDetails(int taxDependentId, bool isHistory = false)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_TaxDependencyDetails> taxdependencydetails;
            if (!isHistory) // Active record dependency.
            {
                taxdependencydetails = ((DataServiceQuery<Technical_TaxDependencyDetails>)context.Technical_TaxDependencyDetails).
                                       Where(n => (n.TaxDependentID == taxDependentId) && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) && (n.HistoryCode == null || n.HistoryCode == "0" || n.HistoryCode.Trim() == string.Empty));
            }
            else // history Record dependencies
            {
                taxdependencydetails = ((DataServiceQuery<Technical_TaxDependencyDetails>)context.Technical_TaxDependencyDetails).Where(n => (n.TaxDependentID == taxDependentId));
            }
            return taxdependencydetails;
        }

        /// <summary>
        /// Updates Sync state in parent table if child record modified
        /// </summary>
        /// <param name="taxDependentId"></param>
        public static void UpdateTaxDependencySyncState(int taxDependentId)
        {
            using (var context = ServicesDataHub.Technical)
            {
                var taxDependency = context.Technical_TaxDependency.Where(n => n.TaxDependentID == taxDependentId).FirstOrDefault();
                if (taxDependency != null && taxDependency.SyncState != null)
                {
                    taxDependency.SyncState = 1;
                    context.UpdateObject(taxDependency);
                    context.SaveChanges();
                }
            }
        }

        #endregion

        #region Additional Program details"

        /// <summary>
        /// Gets All History Records
        /// </summary>
        /// <param name="applicationId">applicationID</param>
        /// <param name="beginDate">beginDate</param>
        /// <param name="endDate">endDate</param>
        /// <returns>Returns Object of Technical_ProgramDetail</returns>
        public static IEnumerable<Technical_ProgramDetail> GetAdditionalprogramHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_ProgramDetail> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_ProgramDetail
                    .Where(n => n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty) &&
                                n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)) &&
                                n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_ProgramDetail
                    .Where(n => n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty) &&
                                n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)));
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_ProgramDetail
                    .Where(n => n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty) &&
                                n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }
            else
            {
                return GetAllAdditionalProgramActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Creates Additional Program Details for all households.
        /// </summary>
        public static void CreateAdditionalProgramDetailRecords(int applicationId)
        {

            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_ApplicationEntity> appEntity = context.Technical_ApplicationEntity.
                Expand("AdditionalCashProgram,AdditionalChildCareProgram,AdditionalDisabledChildrenProgram,AdditionalFoodBenefitsProgram,AdditionalMedicalAssistanceProgram,AdditionalQMBProgram").Where(p => p.ApplicationID == applicationId
                                                                                                                                                                                                              && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty));
            var isNewRecordInserted = false;
            foreach (var ape in appEntity)
            {
                //Create Cash
                if (ape.AdditionalCashProgram.Count == 0)
                {
                    context.AddToTechnical_AdditionalCashProgram(CreateAdditionalCashProgramEntity(ape.ApplicationEntityID, "CA")); isNewRecordInserted = true;
                }

                //Create Child Care
                if (ape.AdditionalChildCareProgram.Count == 0)
                {
                    context.AddToTechnical_AdditionalChildCareProgram(CreateAdditionalChildcareProgramEntity(ape.ApplicationEntityID, "CC")); isNewRecordInserted = true;
                }

                if (ape.AdditionalDisabledChildrenProgram.Count == 0)
                {
                    context.AddToTechnical_AdditionalDisabledChildrenProgram(CreateAdditionalDisabledChildrenProgramEntity(ape.ApplicationEntityID, "DC")); isNewRecordInserted = true;
                }

                if (ape.AdditionalFoodBenefitsProgram.Count == 0)
                {
                    context.AddToTechnical_AdditionalFoodBenefitsProgram(CreateAdditionalFoodBenefitsProgramEntity(ape.ApplicationEntityID, "FS")); isNewRecordInserted = true;
                }

                if (ape.AdditionalMedicalAssistanceProgram.Count == 0)
                {
                    context.AddToTechnical_AdditionalMedicalAssistanceProgram(CreateAdditionalMedicalAssistanceProgramEntity(ape.ApplicationEntityID, "MA")); isNewRecordInserted = true;
                }

                if (ape.AdditionalQMBProgram.Count == 0)
                {
                    context.AddToTechnical_AdditionalQMBProgram(CreateAdditionalQualifiedMemberBeneficiaryProgramEntity(ape.ApplicationEntityID, "QM")); isNewRecordInserted = true;
                }

            }

            if (isNewRecordInserted)
                context.SaveChanges();
        }

        /// <summary>
        ///  Create Additional Cash record 
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <param name="progCode"></param>
        /// <returns></returns>
        public static Technical_AdditionalCashProgram CreateAdditionalCashProgramEntity(int appEntityId, string progCode)
        {
            var cashProg = new Technical_AdditionalCashProgram
            {
                ApplicationEntityID = appEntityId,
                ProgramCode = progCode,
                FirstInsertedByID = LoginUserId, //TODO: Replace this with userid
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1
            };

            return cashProg;
        }

        /// <summary>
        ///  Create Additional childcare entity
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <param name="progCode"></param>
        /// <returns></returns>
        public static Technical_AdditionalChildCareProgram CreateAdditionalChildcareProgramEntity(int appEntityId, string progCode)
        {
            var childcareProg = new Technical_AdditionalChildCareProgram
            {
                ApplicationEntityID = appEntityId,
                ProgramCode = progCode,
                FirstInsertedByID = LoginUserId, //TODO: Replace this with userid
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1
            };
            return childcareProg;
        }

        /// <summary>
        ///  Create Additional DisabledChildren entity
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <param name="progCode"></param>
        /// <returns></returns>
        public static Technical_AdditionalDisabledChildrenProgram CreateAdditionalDisabledChildrenProgramEntity(int appEntityId, string progCode)
        {
            var disabledchildProg = new Technical_AdditionalDisabledChildrenProgram
            {
                ApplicationEntityID = appEntityId,
                ProgramCode = progCode,
                FirstInsertedByID = LoginUserId, //TODO: Replace this with userid
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1
            };

            return disabledchildProg;

        }

        /// <summary>
        ///  Create Food benefits entity 
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <param name="progCode"></param>
        /// <returns></returns>
        public static Technical_AdditionalFoodBenefitsProgram CreateAdditionalFoodBenefitsProgramEntity(int appEntityId, string progCode)
        {
            var foodBenefitProg = new Technical_AdditionalFoodBenefitsProgram
            {
                ApplicationEntityID = appEntityId,
                ProgramCode = progCode,
                FirstInsertedByID = LoginUserId, //TODO: Replace this with userid
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1
            };

            return foodBenefitProg;
        }

        /// <summary>
        ///  Create Medical Assistance entity 
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <param name="progCode"></param>
        /// <returns></returns>
        public static Technical_AdditionalMedicalAssistanceProgram CreateAdditionalMedicalAssistanceProgramEntity(int appEntityId, string progCode)
        {
            var maProg = new Technical_AdditionalMedicalAssistanceProgram
            {
                ApplicationEntityID = appEntityId,
                ProgramCode = progCode,
                FirstInsertedByID = LoginUserId, //TODO: Replace this with userid
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1
            };

            return maProg;
        }

        /// <summary>
        ///  Create QMB benefits entity 
        /// </summary>
        /// <param name="appEntityId"></param>
        /// <param name="progCode"></param>
        /// <returns></returns>
        public static Technical_AdditionalQMBProgram CreateAdditionalQualifiedMemberBeneficiaryProgramEntity(int appEntityId, string progCode)
        {
            var qmbProg = new Technical_AdditionalQMBProgram
            {
                ApplicationEntityID = appEntityId,
                ProgramCode = progCode,
                FirstInsertedByID = LoginUserId, //TODO: Replace this with userid
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1
            };
            return qmbProg;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>       
        /// <param name="applicationId">applicationID</param>
        /// <returns>allActiveRecords</returns>
        public static IEnumerable<Technical_ProgramDetail> GetAllAdditionalProgramActiveRecords(int applicationId)
        {
            return GetProgramOfAssistanceAllActiveRecords(applicationId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalCashProgram> CashProgram(string programCode, int applicationId)
        {            
            var cashProgram = ServicesApplicationHub.IntakeTechnical.GetAdditionalCashProgram(applicationId);          
            return cashProgram;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalCashProgram> CashProgramHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;

            IEnumerable<Technical_AdditionalCashProgram> historyRecords = null;

            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_AdditionalCashProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                    && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                    && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                    && n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                                                                    && n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_AdditionalCashProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                    && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                    && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                    && n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)));
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_AdditionalCashProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                    && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                    && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                    && n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }

            return historyRecords;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalChildCareProgram> ChildCareProgramHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;

            IEnumerable<Technical_AdditionalChildCareProgram> historyRecords = null;

            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_AdditionalChildCareProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                         && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                         && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                         && n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                                                                         && n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_AdditionalChildCareProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                         && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                         && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                         && n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)));
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_AdditionalChildCareProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                         && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                         && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                         && n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }

            return historyRecords;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalDisabledChildrenProgram> DisabledProgramHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;

            IEnumerable<Technical_AdditionalDisabledChildrenProgram> historyRecords = null;

            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_AdditionalDisabledChildrenProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                                && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                                && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                                && n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                                                                                && n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_AdditionalDisabledChildrenProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                                && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                                && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                                && n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)));
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_AdditionalDisabledChildrenProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                                && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                                && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                                && n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }

            return historyRecords;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalFoodBenefitsProgram> FoodBenefitsProgramHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;

            IEnumerable<Technical_AdditionalFoodBenefitsProgram> historyRecords = null;

            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_AdditionalFoodBenefitsProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                            && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                            && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                            && n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                                                                            && n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_AdditionalFoodBenefitsProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                            && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                            && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                            && n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)));
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_AdditionalFoodBenefitsProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                            && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                            && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                            && n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }

            return historyRecords;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalMedicalAssistanceProgram> MedicalProgramHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;

            IEnumerable<Technical_AdditionalMedicalAssistanceProgram> historyRecords = null;

            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_AdditionalMedicalAssistanceProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                                 && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                                 && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                                 && n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                                                                                 && n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_AdditionalMedicalAssistanceProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                                 && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                                 && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                                 && n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)));
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_AdditionalMedicalAssistanceProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                                 && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                                 && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                                 && n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }

            return historyRecords;

        }

        /// <summary>
        /// Getting the QM program History records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalQMBProgram> QMProgramHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;

            IEnumerable<Technical_AdditionalQMBProgram> historyRecords = null;

            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_AdditionalQMBProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                   && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                   && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                   && n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                                                                   && n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_AdditionalQMBProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                   && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                   && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                   && n.DB2UpdatedDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)));
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_AdditionalQMBProgram.Where(n => n.ApplicationEntity.ApplicationID == applicationId
                                                                                   && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                   && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                   && n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }

            return historyRecords;

        }

        /// <summary>
        /// cash program detail.
        /// </summary>
        /// <param name="cashprogramId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalCashProgram> CashProgramdetail(int cashprogramId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_AdditionalCashProgram> cashProgram = context.Technical_AdditionalCashProgram.
                Where(n => n.AdditionalCashProgramID == cashprogramId);
            return cashProgram;

        }
        /// <summary>
        /// Child care Program.
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalChildCareProgram> ChildCareProgram(string programCode, int applicationId)
        {
            var childcareProgram = ServicesApplicationHub.IntakeTechnical.GetAdditionalChildCareProgram(applicationId);
            return childcareProgram;
        }

        /// <summary>
        /// Child Care Program detail.
        /// </summary>
        /// <param name="childCareprogramId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalChildCareProgram> ChildCareProgramDetails(int childCareprogramId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_AdditionalChildCareProgram> childcareProgram = context.Technical_AdditionalChildCareProgram.
                Where(n => n.AdditionalChildCareProgramID == childCareprogramId);
            return childcareProgram;
        }

        /// <summary>
        /// Disabled Child program.
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalDisabledChildrenProgram> DisabledChildrenProgram(string programCode, int applicationId)
        {
            var disabledChildrenProgram = ServicesApplicationHub.IntakeTechnical.GetAdditionalDisableChildrenProgram(applicationId);
            return disabledChildrenProgram;
        }

        /// <summary>
        /// Disabled Child program Detail.
        /// </summary>
        /// <param name="diasabledChildrenProgramId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalDisabledChildrenProgram> DisabledChildrenProgramDetail(int diasabledChildrenProgramId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_AdditionalDisabledChildrenProgram> disabledChildrenProgram = context.Technical_AdditionalDisabledChildrenProgram.
                Where(n => n.AdditionalDiasabledChildrenProgramID == diasabledChildrenProgramId);
            return disabledChildrenProgram;
        }

        /// <summary>
        /// Food Benefits Program.
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalFoodBenefitsProgram> FoodBenefitsProgram(string programCode, int applicationId)
        {
            var foodBenefitsProgram = ServicesApplicationHub.IntakeTechnical.GetAdditionalFoodBenefitsProgram(applicationId);
            return foodBenefitsProgram;
        }

        /// <summary>
        /// Food Benefits Program detail.
        /// </summary>
        /// <param name="foodBenefitProgramId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalFoodBenefitsProgram> FoodBenefitsProgramDetails(int foodBenefitProgramId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_AdditionalFoodBenefitsProgram> foodBenefitsProgram = context.Technical_AdditionalFoodBenefitsProgram.
                Where(n => n.AdditionalFoodBenefitProgramID == foodBenefitProgramId);
            return foodBenefitsProgram;
        }

        /// <summary>
        /// Medical Assistance Program.
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalMedicalAssistanceProgram> MedicalAssistanceProgram(string programCode, int applicationId)
        {
            var medicalAssistanceProgram = ServicesApplicationHub.IntakeTechnical.GetAdditionalMedicalAssistanceProgram(applicationId);
            return medicalAssistanceProgram;
        }

        /// <summary>
        /// Medical Assistance Program Detail.
        /// </summary>
        /// <param name="medicalAssistanceProgramId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalMedicalAssistanceProgram> MedicalAssistanceProgramDetail(int medicalAssistanceProgramId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_AdditionalMedicalAssistanceProgram> medicalAssistanceProgram = context.Technical_AdditionalMedicalAssistanceProgram.
                Where(n => n.AdditionalMedicalAssistanceProgramID == medicalAssistanceProgramId);
            return medicalAssistanceProgram;
        }

        /// <summary>
        /// Qualified Member Beneficiary Program
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalQMBProgram> QualifiedMemberBeneficiaryProgram(string programCode, int applicationId)
        {
            var qmbProgram = ServicesApplicationHub.IntakeTechnical.GetAdditionalQMBProgram(applicationId);
            return qmbProgram;
        }

        /// <summary>
        /// Qualified Member Beneficiary Program Detail
        /// </summary>
        /// <param name="qmbProgramId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_AdditionalQMBProgram> QualifiedMemberBeneficiaryProgramDetail(int qmbProgramId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_AdditionalQMBProgram> qmbProgram = context.Technical_AdditionalQMBProgram.
                Where(n => n.AdditionalQMBProgramID == qmbProgramId);
            return qmbProgram;
        }

        /// <summary>
        /// Sets the value for  WithdrawalRequestIndicator of  Cash Program 
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="applicationId"></param>
        /// <returns>cashProgram</returns>
        public static void CashWithdraw(string programCode, int applicationId)
        {
            var techContext = ServicesDataHub.Technical;
            IEnumerable<Technical_AdditionalCashProgram> cashProgram = techContext.Technical_AdditionalCashProgram.Where(n => n.ProgramCode == programCode &&
                                                                                                                              n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                                                              && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                                                              && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty));
            foreach (var cash in cashProgram)
            {
                cash.WithdrawalRequestIndicator = true;
                techContext.UpdateObject(cash);
                techContext.SaveChanges();
            }
        }

        /// <summary>
        /// Sets the value for  WithdrawalRequestIndicator of ChildCareProgram
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="applicationId"></param>
        /// <returns>childCareProgram</returns>
        public static void ChildCareWithdraw(string programCode, int applicationId)
        {
            var techContext = ServicesDataHub.Technical;
            IEnumerable<Technical_AdditionalChildCareProgram> childCareProgram = techContext.Technical_AdditionalChildCareProgram.Where(n => n.ProgramCode == programCode &&
                                                                                                                                             n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                                                                             && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                                                                             && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty));
            foreach (var childcare in childCareProgram)
            {
                childcare.WithdrawalRequestIndicator = true;
                techContext.UpdateObject(childcare);
                techContext.SaveChanges();
            }
        }

        /// <summary>
        /// Sets the value for  WithdrawalRequestIndicator of DisabledChildrenProgram
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="applicationId"></param>
        /// <returns>diabledProgram</returns>
        public static void DisabledWithdraw(string programCode, int applicationId)
        {
            var techContext = ServicesDataHub.Technical;
            IEnumerable<Technical_AdditionalDisabledChildrenProgram> diabledProgram = techContext.Technical_AdditionalDisabledChildrenProgram.Where(n => n.ProgramCode == programCode &&
                                                                                                                                                         n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                                                                                         && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                                                                                         && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty));
            foreach (var disabled in diabledProgram)
            {
                disabled.WithdrawalRequestIndicator = true;
                techContext.UpdateObject(disabled);
                techContext.SaveChanges();
            }
        }

        /// <summary>
        /// Sets the value for  WithdrawalRequestIndicator of FoodBenefitsProgram
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="applicationId"></param>
        /// <returns>fbProgram</returns>
        public static void FoodBenefitsWithdraw(string programCode, int applicationId)
        {
            var techContext = ServicesDataHub.Technical;
            IEnumerable<Technical_AdditionalFoodBenefitsProgram> fbProgram = techContext.Technical_AdditionalFoodBenefitsProgram.Where(n => n.ProgramCode == programCode &&
                                                                                                                                            n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                                                                            && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                                                                            && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty));
            foreach (var foodBenefit in fbProgram)
            {
                foodBenefit.WithdrawalRequestIndicator = true;
                techContext.UpdateObject(foodBenefit);
                techContext.SaveChanges();
            }
        }

        /// <summary>
        /// Sets the value for  WithdrawalRequestIndicator of MedicalAssistanceProgram
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="applicationId"></param>
        /// <returns>medicalAssistanceProgram</returns>
        public static void MedicalWithdraw(string programCode, int applicationId)
        {
            var techContext = ServicesDataHub.Technical;
            IEnumerable<Technical_AdditionalMedicalAssistanceProgram> medicalAssistanceProgram =
                techContext.Technical_AdditionalMedicalAssistanceProgram.Where(n => n.ProgramCode == programCode &&
                                                                                    n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                                    && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                                    && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty));
            foreach (var medical in medicalAssistanceProgram)
            {
                medical.WithdrawalRequestIndicator = true;
                techContext.UpdateObject(medical);
                techContext.SaveChanges();
            }
        }

        /// <summary>
        /// Sets the value for WithdrawalRequestIndicator of QualifiedMemberBeneficiaryProgram
        /// </summary>
        /// <param name="programCode"></param>
        /// <param name="applicationId"></param>
        /// <returns>qmbProgram</returns>
        public static void QualifiedMemberBeneficiaryProgramWithdraw(string programCode, int applicationId)
        {
            var techContext = new TechnicalContextImpl();
            IEnumerable<Technical_AdditionalQMBProgram> qmbProgram =
                techContext.Technical_AdditionalQMBProgram.Where(n => n.ProgramCode == programCode &&
                                                                      n.ApplicationEntity.ApplicationID == applicationId && (n.ApplicationEntity.DeleteReasonCode == null || n.ApplicationEntity.DeleteReasonCode.Trim() == string.Empty)
                                                                      && (n.ApplicationEntity.HistoryCode == null || n.ApplicationEntity.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.ApplicationEntity.HistoryCode.Trim() == string.Empty)
                                                                      && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty));

            foreach (var qmb in qmbProgram)
            {
                qmb.WithdrawalRequestIndicator = true;
                techContext.UpdateObject(qmb);
                techContext.SaveChanges();
            }
        }

        #endregion

        #region "Additional Individual Demographics"

        /// <summary>
        /// Creates new records if the PersonId is not found in the PersonDemographics table
        /// </summary>
        /// <param name="applicationId"></param>
        public static void CreateNewAdditionalIndivRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;

            var appEntity =
                context.Technical_ApplicationEntity.Where(p => p.ApplicationID == applicationId)
                    .Select(n => new { n.EntityID })
                    .ToList();
            var isInserted = false;

            if (appEntity.Count() > 0)
            {
                var personDemo = context.Technical_PersonDemographics.WhereIn(
                    appEntity
                        .Select(p =>
                            new
                            {
                                PersonID = p.EntityID
                            }).ToList())
                    .Where(
                        n =>
                            (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                             n.HistoryCode.Trim() == string.Empty))
                    .Select(p => new { p.PersonID }).ToList();
                foreach (var person in appEntity)
                {
                    if (personDemo.Count() == 0 || (!personDemo.Any(n => n.PersonID == person.EntityID)))
                    {
                        var newPerson = CreatePersonDemographicsObject(person.EntityID);
                        context.AddToTechnical_PersonDemographics(newPerson);
                        isInserted = true;
                    }
                }
            }

            if (isInserted)
                context.SaveChanges();

        }

        /// <summary>
        /// Creates a new object type of Technical_PersonDemographics
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Technical_PersonDemographics CreatePersonDemographicsObject(int personId)
        {
            var personDemographicsProvider = new Technical_PersonDemographics
            {
                PersonID = personId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            return personDemographicsProvider;
        }

        /// <summary>
        /// Gets Additional Individual Demographics History records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns>Returns an Object of Technical_PersonDemographics</returns>
        public static IQueryable<Technical_PersonDemographics> GetAdditionalIndivDemographicsHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var technicalContext = new TechnicalContextImpl();
            IQueryable<Technical_PersonDemographics> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = technicalContext.Technical_PersonDemographics
                                                 .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                     && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                     && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                     && n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                                     && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                                                 .OrderBy(k => k.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null)
            {
                historyRecords = technicalContext.Technical_PersonDemographics
                                                 .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                     && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                     && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                     && n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                                                 .OrderBy(k => k.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (endDate != null)
            {
                historyRecords = technicalContext.Technical_PersonDemographics
                                                 .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                     && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                     && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                     && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                                                 .OrderBy(k => k.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetAdditionalIndivDemographicsAllActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IQueryable<Technical_PersonDemographics> GetAdditionalIndivDemographicsAllActiveRecords(int applicationId)
        {
            var technicalContext = ServicesDataHub.Technical;
            IQueryable<Technical_PersonDemographics> allActiveRecords = technicalContext.Technical_PersonDemographics
                                                                                        .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId 
                                                                                            && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                                                            && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) 
                                                                                            &&(n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)
                                                                                            && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                                                                                        .OrderBy(k => k.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets the ChildCare Additional Demographics details.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Technical_PersonDemographics> GetAdditionalIndivDemographicsDetailsRecord(int additionalDemographicsId)
        {
            if (additionalDemographicsId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_PersonDemographics> result = context.Technical_PersonDemographics
                .Where(n => n.PersonDemographicsID == additionalDemographicsId);
            return result;
        }

        #endregion

        #region "School Enrollment"

        /// <summary>
        /// Creates new School Enrollment
        /// </summary>
        public static void CreateNewSchoolEnrollment(int applicationId)
        {
            var context = ServicesDataHub.Technical;

            var appEntity =
                context.Technical_ApplicationEntity.Where(p => p.ApplicationID == applicationId)
                    .Select(n => new { n.EntityID })
                    .ToList();
            var isInserted = false;

            if (appEntity.Count() > 0)
            {
                var personSchool = context.Technical_SchoolEnrollment.WhereIn(
                    appEntity
                        .Select(p =>
                            new
                            {
                                PersonID = p.EntityID
                            }).ToList())
                    .Where(
                        n =>
                            (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                             n.HistoryCode.Trim() == string.Empty))
                    .Select(p => new { p.PersonID }).ToList();
                foreach (var person in appEntity)
                {
                    if (personSchool.Count() == 0 || (!personSchool.Any(n => n.PersonID == person.EntityID)))
                    {
                        var newPerson = CreateNewSchoolEnrollmentEntity(person.EntityID);
                        context.AddToTechnical_SchoolEnrollment(newPerson);
                        isInserted = true;
                    }
                }
            }

            if (isInserted)
                context.SaveChanges();
        }

        /// <summary>
        /// Creates object of Technical_SchoolEnrollment.
        /// </summary>
        /// <param name="personId">ApplicationEntityID</param>
        /// <returns>Returns object of Technical_SchoolEnrollment</returns>
        private static Technical_SchoolEnrollment CreateNewSchoolEnrollmentEntity(int personId)
        {
            if (personId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var newEntity = new Technical_SchoolEnrollment
            {
                PersonID = personId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1,
                SequenceNumber = 1
            };

            return newEntity;
        }

        /// <summary>
        /// Gets all history records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_SchoolEnrollment> GetSchoolEnrollmentHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_SchoolEnrollment> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_SchoolEnrollment
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                            && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_SchoolEnrollment
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_SchoolEnrollment
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetAllSchoolEnrollmentActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_SchoolEnrollment> GetAllSchoolEnrollmentActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_SchoolEnrollment> allActiveRecords =
                context.Technical_SchoolEnrollment.Where(
                    n =>
                        n.Person.ApplicationEntity.Any(
                            p =>
                                p.ApplicationID == applicationId &&
                                (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                 p.HistoryCode.Trim() == string.Empty))
                        && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static bool DoesEntityHaveActiveSchoolEnrollmentRecords(int applicationId,int personId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_SchoolEnrollment> allActiveRecords =
                context.Technical_SchoolEnrollment.Where(
                    n =>
                        n.Person.ApplicationEntity.Any(
                            p =>
                                p.ApplicationID == applicationId && 
                                (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                 p.HistoryCode.Trim() == string.Empty))
                        && (n.PersonID==personId) && (n.EnrollmentStatusCode != IntakeConstants.SCHOOL_ENROLLMENT_NOTENROLLED) && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty));
                      
            return allActiveRecords.Any();
        }


        /// <summary>
        /// Gets the DOB of an Individual
        /// </summary>
        /// <param name="appEntityId">ApplicationEntityID</param>
        /// <returns>DOB</returns>
        public static DateTime GetIndividualDob(int appEntityId)
        {
            var context = ServicesDataHub.Technical;
            var personAdditionalAttributes =
                context.Technical_PersonAdditionalAttributes
                    .Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationEntityID == appEntityId))
                    .FirstOrDefault();
            return Convert.ToDateTime(personAdditionalAttributes.DateOfBirthDate);
        }

        /// <summary>
        ///Returns ID of the SchoolEnrollment Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static int GetSchoolEnrollmentEndedRecId(int personId, Int16 historySeqNum)
        {
            var context = ServicesDataHub.Technical;
            var endedRec =
                context.Technical_SchoolEnrollment.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)
                        && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();

            return endedRec.SchoolEnrollmentID;
        }

        /// <summary>
        /// Gets School enrollment current record
        /// </summary>
        /// <param name="context"></param>
        /// <param name="personId"></param>
        /// <returns></returns>       
        public static Technical_SchoolEnrollment GetSchoolEnrollmentCurrentRecord(TechnicalContextImpl context, int personId)
        {
            var schoolEnrollment = context.Technical_SchoolEnrollment.Where(
                n =>
                    n.PersonID == personId &&
                    (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                     n.HistoryCode.Trim() == string.Empty)).FirstOrDefault();
            return schoolEnrollment;
        }

        #endregion

        #region "Loss of Health Insurance"

        /// <summary>
        /// Creats a new Loss of Health Insurance record
        /// </summary>
        /// <returns></returns>
        public static Technical_HealthInsuranceLoss CreateLossOfHealthInsuranceRecord()
        {
            var context = ServicesDataHub.Technical;
            var newLossOfHealthIns = CreateLossOfHealthInsuranceObject();
            context.AddToTechnical_HealthInsuranceLoss(newLossOfHealthIns);
            context.SaveChanges();
            return newLossOfHealthIns;
        }

        /// <summary>
        /// Create a Technical_Disability type of object
        /// </summary>
        /// <returns></returns>
        private static Technical_HealthInsuranceLoss CreateLossOfHealthInsuranceObject()
        {
            var newEntity = new Technical_HealthInsuranceLoss
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };

            return newEntity;
        }

        /// <summary>
        /// Gets all history records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_HealthInsuranceLoss> GetLossOfHealthInsuranceHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = new TechnicalContextImpl();
            IEnumerable<Technical_HealthInsuranceLoss> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_HealthInsuranceLoss
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                            && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                        .OrderBy(n => n.Person.PersonAdditionalAttributes.MCINumber).ThenBy(n => n.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_HealthInsuranceLoss
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                    .OrderBy(n => n.Person.PersonAdditionalAttributes.MCINumber).ThenBy(n => n.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_HealthInsuranceLoss
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                    .OrderBy(n => n.Person.PersonAdditionalAttributes.MCINumber).ThenBy(n => n.HistorySequenceNumber);
            }
            else
            {
                return GetLossOfHealthInsuranceAllActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_HealthInsuranceLoss> GetLossOfHealthInsuranceAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_HealthInsuranceLoss> allActiveRecords =
                context.Technical_HealthInsuranceLoss.Where(
                    n =>
                        n.Person.ApplicationEntity.Any(
                            p =>
                                p.ApplicationID == applicationId &&
                                (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                 p.HistoryCode.Trim() == string.Empty))
                        && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty))
                .OrderBy(n => n.Person.PersonAdditionalAttributes.MCINumber).ThenBy(n => n.SequenceNumber).ThenBy(n => n.HistorySequenceNumber);
            return allActiveRecords;
        }

        /// <summary>
        /// Verifies for an active record exists for the selected individual.
        /// </summary>
        /// <param name="personId">ApplicationEntityID</param>
        /// <returns></returns>
        public static bool IsHealthInsuranceRecordExists(int personId)
        {

            var techcontext = ServicesDataHub.Technical;
            return techcontext.Technical_HealthInsuranceLoss.Where(n => n.PersonID == personId &&
                                                                        (n.DeleteReasonCode == null ||
                                                                         n.DeleteReasonCode.Trim() == string.Empty) &&
                                                                        (n.HistoryCode == null ||
                                                                         n.HistoryCode.Trim() == string.Empty ||
                                                                         n.HistoryCode ==
                                                                         IntakeConstants.ACTIVE_RECORD_CODE)).Count() >
                   0;
        }

        /// <summary>
        ///Returns ID of the HealthInsuranceLoss Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static int GetHealthInsuranceLossEndedRecId(int personId, Int16 historySeqNum)
        {
            var context = ServicesDataHub.Technical;
            var endedRec =
                context.Technical_HealthInsuranceLoss.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)
                        && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();

            return endedRec.HealthInsuranceLossID;
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfHealthInsuranceLossRec(int personId)
        {
            Int16 historySeqNum = 1;

            var techcontext = ServicesDataHub.Technical;
            var maxRecord =
                techcontext.Technical_HealthInsuranceLoss.Where(n => n.PersonID == personId)
                    .OrderByDescending(n => n.HistorySequenceNumber);
            if (maxRecord.Count() > 0)
            {
                historySeqNum = Convert.ToInt16(maxRecord.First().HistorySequenceNumber);
                historySeqNum++;
            }

            return historySeqNum;
        }

        #endregion

        #region "Spousal Impoverishment"
        /// <summary>
        /// Creats a new record for spousal impoverishment
        /// </summary>
        /// <returns>SpousalImpoverishmentID</returns>
        public static Technical_SpousalImpoverishment CreateNewRecordSpousal()
        {
            var context = ServicesDataHub.Technical;
            var spousalProvider = CreateSpousalObject();
            context.AddToTechnical_SpousalImpoverishment(spousalProvider);
            context.SaveChanges();
            return spousalProvider;
        }

        /// <summary>
        /// Creates a new  Technical_InstitutionInfo type of object
        /// </summary>
        /// <returns>Technical_SpousalImpoverishment type of object</returns>
        private static Technical_SpousalImpoverishment CreateSpousalObject()
        {
            var spousalProvider = new Technical_SpousalImpoverishment
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                SequenceNumber = 1,
                MakeAvailableMaxIncome_AMNT = Convert.ToDecimal(0.00),
                CourtOrderOrFairHearingAllocaitonAmount = Convert.ToDecimal(0.00),
                CourtOrderOrFairHearingAssetShareAmount = Convert.ToDecimal(0.00)
            };
            return spousalProvider;
        }

        /// <summary>
        /// Gets all history records.
        /// </summary>
        /// <param name="applicationId">appliation id</param>
        /// <param name="beginDate"> start date of the search</param>
        /// <param name="endDate">end date of the search</param>
        /// <returns>IEnumerable<Technical_InstitutionInfo/></returns>
        public static IEnumerable<Technical_SpousalImpoverishment> GetHistoryRecordsSpousal(int applicationId, Object beginDate, Object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_SpousalImpoverishment> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_SpousalImpoverishment.
                    Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                                   &&
                                                                   (p.DeleteReasonCode == null ||
                                                                    p.DeleteReasonCode.Trim() == string.Empty) &&
                                                                   (p.HistoryCode == null ||
                                                                    p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                                                    p.HistoryCode.Trim() == string.Empty))
                               &&
                               (n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate))))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_SpousalImpoverishment.
                    Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                                   &&
                                                                   (p.DeleteReasonCode == null ||
                                                                    p.DeleteReasonCode.Trim() == string.Empty) &&
                                                                   (p.HistoryCode == null ||
                                                                    p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                                                    p.HistoryCode.Trim() == string.Empty))
                               &&
                               n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_SpousalImpoverishment.
                    Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty))
                            && n.BeginDate <= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(endDate)))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetAllActiveRecordsSpousal(applicationId);
            }

            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>
        /// <returns>IEnumerable<Technical_SpousalImpoverishment/></returns>
        public static IEnumerable<Technical_SpousalImpoverishment> GetAllActiveRecordsSpousal(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_SpousalImpoverishment> activeSpousal = context.Technical_SpousalImpoverishment.
                Where(
                    n =>
                        n.Person.ApplicationEntity.Any(
                            p =>
                                p.ApplicationID == applicationId &&
                                (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                 p.HistoryCode.Trim() == string.Empty))
                        && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty))
                      .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return activeSpousal;
        }

        /// <summary>
        /// Verifies for an active record exists for the selected individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static bool IsSpousalImpRecordExists(int personId)
        {
            if (personId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var techcontext = ServicesDataHub.Technical;

            return techcontext.Technical_SpousalImpoverishment.Where(n => n.PersonID == personId &&
                                                                          (n.DeleteReasonCode == null ||
                                                                           n.DeleteReasonCode.Trim() == string.Empty) &&
                                                                          (n.HistoryCode == null ||
                                                                           n.HistoryCode ==
                                                                           IntakeConstants.ACTIVE_RECORD_CODE ||
                                                                           n.HistoryCode.Trim() == string.Empty))
                .Count() > 0;
        }

        /// <summary>
        /// Delete newly added SpousalImpoverishment record When Click on Oops
        /// </summary>
        /// <param name="spousalImpoverishmentId"></param>
        /// <returns></returns>
        public static void DeleteSpousalImpoverishmentRecord(int spousalImpoverishmentId)
        {
            var techcontext = ServicesDataHub.Technical;
            var spousalImpoverishment = techcontext.Technical_SpousalImpoverishment.Where(n => n.SpousalImpoverishmentID == spousalImpoverishmentId).First();
            techcontext.DeleteObject(spousalImpoverishment);
            techcontext.UsePostTunneling = true;
            techcontext.SaveChanges();
        }

        /// <summary>
        ///Returns ID of the Spousal Impoverishment Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static Technical_SpousalImpoverishment GetSpousalImpEndedRecId(int personId, Int16 historySeqNum)
        {
            var context = new TechnicalContextImpl();
            var spousalImpRec =
                context.Technical_SpousalImpoverishment.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)
                        && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();
            return spousalImpRec;
        }

        /// <summary>
        /// Disable Oops button for Existing record Otherwise Enable
        /// </summary>
        public static bool IsEnableOopsSpousalImp()
        {
            var techcontext = ServicesDataHub.Technical;
            IEnumerable<Technical_SpousalImpoverishment> spousalImpoverishment = techcontext
                .Technical_SpousalImpoverishment.
                Where(
                    n =>
                        n.Person.ApplicationEntity.Any(
                            p =>
                                p.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key) &&
                                (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                 p.HistoryCode.Trim() == string.Empty))
                        && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty));

            return !(spousalImpoverishment.Count() > 0);
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfSpousalImpRec(int personId)
        {
            Int16 historySeqNum = 1;

            var techcontext = ServicesDataHub.Technical;
            var maxSpousalImpRecord = techcontext.Technical_SpousalImpoverishment.Where(n => n.PersonID == personId).OrderByDescending(n => n.HistorySequenceNumber);
            if (maxSpousalImpRecord.Count() > 0)
            {
                historySeqNum = Convert.ToInt16(maxSpousalImpRecord.First().HistorySequenceNumber);
                historySeqNum++;
            }
            return historySeqNum;
        }

        #endregion

        #region "Home Community Based Services"

        /// <summary>
        /// Creats a new record
        /// </summary>
        /// <returns></returns>
        public static Technical_HomeCommunityBasedService CreateNewHomeCommunityRecord()
        {
            var context = ServicesDataHub.Technical;
            var homeCommunityBasedServiceProvider = CreateHomeCommunityObject();
            context.AddToTechnical_HomeCommunityBasedService(homeCommunityBasedServiceProvider);
            context.SaveChanges();
            return homeCommunityBasedServiceProvider;
        }

        /// <summary>
        /// Creats new Technical_HomeCommunityBasedService type of object.
        /// </summary>
        /// <returns></returns>
        protected static Technical_HomeCommunityBasedService CreateHomeCommunityObject()
        {
            var homeCommunityBasedServiceProvider = new Technical_HomeCommunityBasedService
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            return homeCommunityBasedServiceProvider;
        }

        /// <summary>
        /// Verifies for an active record exists for the selected individual.
        /// </summary>
        /// <param name="personId">ApplicationEntityID</param>
        /// <returns></returns>
        public static bool IsHomeCommunityBasedRecordExists(int personId)
        {
            if (personId == 0)
                throw new ArgumentException("Argument can not be zero.");
            var techcontext = ServicesDataHub.Technical;
            return techcontext.Technical_HomeCommunityBasedService.Where(n => n.PersonID == personId &&
                                                                              (n.DeleteReasonCode == null ||
                                                                               n.DeleteReasonCode.Trim() == string.Empty) &&
                                                                              (n.HistoryCode == null ||
                                                                               n.HistoryCode.Trim() == string.Empty ||
                                                                               n.HistoryCode ==
                                                                               IntakeConstants.ACTIVE_RECORD_CODE))
                .Count() > 0;
        }

        /// <summary>
        /// Gets all history records.
        /// </summary>
        /// <param name="applicationId">appliation id</param>
        /// <param name="beginDate"> start dat eof the search</param>
        /// <param name="endDate">end date of the search</param>
        /// <returns>IEnumerable<Technical_HomeCommunityBasedService/></returns>
        public static IEnumerable<Technical_HomeCommunityBasedService> GetHomeCommunityHistoryRecords(int applicationId, Object beginDate, Object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_HomeCommunityBasedService> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_HomeCommunityBasedService
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty))
                            && (n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate))))
                .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_HomeCommunityBasedService
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                    .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_HomeCommunityBasedService
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate <= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                    .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);

            }
            else
            {
                return GetHomeCommunityActiveRecords(applicationId);
            }

            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns><Technical_HomeCommunityBasedService/></returns>
        public static IEnumerable<Technical_HomeCommunityBasedService> GetHomeCommunityActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_HomeCommunityBasedService> activeInstitution = context
                .Technical_HomeCommunityBasedService
                .Where(
                    n =>
                        n.Person.ApplicationEntity.Any(
                            p =>
                                p.ApplicationID == applicationId &&
                                (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                 p.HistoryCode.Trim() == string.Empty)) &&
                        (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty))
            .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return activeInstitution;
        }

        /// <summary>
        /// Delete newly added HomeCommunityBasedService record When Click on Oops
        /// </summary>
        /// <param name="homeCommunityBasedServiceId"></param>
        /// <returns></returns>
        public static void DeleteHomeCommunityBasedServiceRecord(int homeCommunityBasedServiceId)
        {
            var techcontext = ServicesDataHub.Technical;
            var homeCommunityBasedService =
                techcontext.Technical_HomeCommunityBasedService.Where(
                    n => n.HomeCommunityBasedServiceID == homeCommunityBasedServiceId).First();
            techcontext.UsePostTunneling = true;
            techcontext.DeleteObject(homeCommunityBasedService);
            techcontext.SaveChanges();
        }

        /// <summary>
        /// Disable Oops button for Existing record Otherwise Enable
        /// </summary>
        public static IEnumerable<Technical_HomeCommunityBasedService> EnableDisableOopsHomeCommunityBasedService()
        {
            var techcontext = ServicesDataHub.Technical;
            IEnumerable<Technical_HomeCommunityBasedService> homeCommunityBasedService = techcontext
                .Technical_HomeCommunityBasedService.
                Where(
                    n =>
                        n.Person.ApplicationEntity.Any(
                            p => p.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key)
                                 && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                 (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                  p.HistoryCode.Trim() == string.Empty)) &&
                        (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty));

            return homeCommunityBasedService;
        }

        /// <summary>
        ///Returns ID of the HomeCommunity Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static Technical_HomeCommunityBasedService GetHomeCommunityEndedRecId(int personId, Int16 historySeqNum)
        {
            var context = ServicesDataHub.Technical;
            var endedRec =
                context.Technical_HomeCommunityBasedService.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)
                        && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();

            return endedRec;
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfHomeCommunityRec(int personId)
        {
            Int16 historySeqNum = 1;

            var techcontext = ServicesDataHub.Technical;
            var maxRecord = techcontext.Technical_HomeCommunityBasedService.Where(n => n.PersonID == personId).OrderByDescending(n => n.HistorySequenceNumber);
            if (maxRecord.Count() > 0)
            {
                historySeqNum = Convert.ToInt16(maxRecord.First().HistorySequenceNumber);
                historySeqNum++;
            }

            return historySeqNum;
        }

        #endregion

        #region "New Born"

        /// <summary>
        /// Add New Born Details
        /// </summary>
        /// <returns></returns>
        public static Technical_ContinuouslyEligibleNewborn CreateNewNewBornEntity()
        {
            var context = ServicesDataHub.Technical;
            var newBorn = CreateNewBornObject();
            context.AddToTechnical_ContinuouslyEligibleNewborn(newBorn);
            context.SaveChanges();
            return newBorn;
        }

        /// <summary>
        /// Create a Technical_Disability type of object
        /// </summary>
        /// <returns></returns>
        private static Technical_ContinuouslyEligibleNewborn CreateNewBornObject()
        {
            var newBorn = new Technical_ContinuouslyEligibleNewborn
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };

            return newBorn;
        }


        /// <summary>
        /// Gets all history records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_ContinuouslyEligibleNewborn> GetNewBornHistoryRecords(int applicationId,
            object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_ContinuouslyEligibleNewborn> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_ContinuouslyEligibleNewborn
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                            && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_ContinuouslyEligibleNewborn
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                    .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_ContinuouslyEligibleNewborn
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetNewBornAllActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>     
        /// <param name="applicationId">ApplicationID</param>
        /// <returns></returns>
        public static IEnumerable<Technical_ContinuouslyEligibleNewborn> GetNewBornAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_ContinuouslyEligibleNewborn> allActiveRecords =
                context.Technical_ContinuouslyEligibleNewborn.Where(
                    n =>
                        n.Person.ApplicationEntity.Any(
                            p =>
                                p.ApplicationID == applicationId &&
                                (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                 p.HistoryCode.Trim() == string.Empty))
                        && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty))
                .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }

        /// <summary>
        /// Verifies for an active record exists for the selected individual.
        /// </summary>
        /// <param name="personId">PersonID</param>
        /// <returns></returns>
        public static bool IsNewBornPersonExists(int personId)
        {
            if (personId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var techcontext = ServicesDataHub.Technical;
            return techcontext.Technical_ContinuouslyEligibleNewborn.Where(n => n.PersonID == personId &&
                                                                                (n.DeleteReasonCode == null ||
                                                                                 n.DeleteReasonCode.Trim() ==
                                                                                 string.Empty) &&
                                                                                (n.HistoryCode == null ||
                                                                                 n.HistoryCode == string.Empty ||
                                                                                 n.HistoryCode ==
                                                                                 IntakeConstants.ACTIVE_RECORD_CODE))
                .Count() > 0;
        }

        /// <summary>
        ///Returns ID of the pregnancy Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static Technical_ContinuouslyEligibleNewborn GetNewbornEndedRecId(int personId, Int16 historySeqNum)
        {
            using (var context = ServicesDataHub.Technical)
            {
                var endedRec =
                    context.Technical_ContinuouslyEligibleNewborn.Where(
                        n =>
                            n.PersonID == personId &&
                            (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                             n.HistoryCode.Trim() == string.Empty)
                            && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();

                return endedRec;
            }
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfNewbornRec(int personId)
        {
            Int16 historySeqNum = 1;

            using (var techcontext = ServicesDataHub.Technical)
            {
                var maxRecord = techcontext.Technical_ContinuouslyEligibleNewborn.Where(n => n.PersonID == personId).OrderByDescending(n => n.HistorySequenceNumber);
                if (maxRecord.Count() > 0)
                {
                    historySeqNum = Convert.ToInt16(maxRecord.First().HistorySequenceNumber);
                    historySeqNum++;
                }
            }
            return historySeqNum;
        }

        /// <summary>
        /// Gets the DOB of an Individual
        /// </summary>
        /// <param name="personId">PersonID</param>
        /// <returns>DOB</returns>
        public static DateTime GetPersonDOB(int personId)
        {
            return ServicesApplicationHub.Intake.GetDateOfBirth(personId);
        }

        /// <summary>
        /// Delete newly added Newborn record When Click on Oops
        /// </summary>
        /// <param name="newborn"></param>
        /// <returns></returns>
        public static void DeleteNebornRecord(int newborn)
        {
            var techcontext = ServicesDataHub.Technical;
            var protectedSsi = techcontext.Technical_ContinuouslyEligibleNewborn.Where(n => n.ContinuouslyEligibleNewbornID == newborn).First();
            techcontext.UsePostTunneling = true;
            techcontext.DeleteObject(protectedSsi);
            techcontext.SaveChanges();
        }

        #endregion

        #region "Breast and Cervical Cancer"
        /// <summary>
        /// Creates new records if the PersonId is not found in the BreastAndCervicalCancer table
        /// </summary>
        public static Technical_BreastAndCervicalCancer CreateNewBreastAndCervicalRecords()
        {
            var context = ServicesDataHub.Technical;
            var breastAndCervicalCancerPerson = CreateBreastAndCervicalCancerObject();
            context.AddToTechnical_BreastAndCervicalCancer(breastAndCervicalCancerPerson);
            context.SaveChanges();
            return breastAndCervicalCancerPerson;
        }

        /// <summary>
        /// Creates a new object type of Technical_BreastAndCervicalCancer
        /// </summary>
        /// <returns></returns>
        public static Technical_BreastAndCervicalCancer CreateBreastAndCervicalCancerObject()
        {
            var breastAndCervicalCancerPerson = new Technical_BreastAndCervicalCancer
            {

                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            return breastAndCervicalCancerPerson;
        }

        /// <summary>
        /// Checks an individual has an active record.
        /// </summary>
        /// <param name="personId">personID</param>
        /// <returns>Returns value true if record exists else false. </returns>
        public static bool IsBreastAndCervicalCancerRecordExist(int personId)
        {
            var techContext = ServicesDataHub.Technical;
            return techContext.Technical_BreastAndCervicalCancer.Where(n => n.PersonID == personId &&
                                                                            (n.DeleteReasonCode == null ||
                                                                             n.DeleteReasonCode.Trim() == string.Empty) &&
                                                                            (n.HistoryCode == null ||
                                                                             n.HistoryCode.Trim() == string.Empty ||
                                                                             n.HistoryCode ==
                                                                             IntakeConstants.ACTIVE_RECORD_CODE))
                .Count() > 0;
        }

        /// <summary>
        ///Returns ID of the BreastAndCancer Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static Technical_BreastAndCervicalCancer GetBreastAndCervicalCancerEndedRecId(int personId, Int16 historySeqNum)
        {
            var context = ServicesDataHub.Technical;
            var endedRec =
                context.Technical_BreastAndCervicalCancer.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)
                        && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();

            return endedRec;
        }


        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfBreastAndCancerRec(int personId)
        {
            Int16 historySeqNum = 1;

            var techcontext = ServicesDataHub.Technical;
            var maxRecord = techcontext.Technical_BreastAndCervicalCancer.Where(n => n.PersonID == personId).OrderByDescending(n => n.HistorySequenceNumber);
            if (maxRecord.Count() > 0)
            {
                historySeqNum = Convert.ToInt16(maxRecord.First().HistorySequenceNumber);
                historySeqNum++;
            }
            return historySeqNum;
        }

        /// <summary>
        /// Get Breast and Cervical Cancer History Records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns>Returns an Object of Technical_BreastAndCervicalCancer</returns>
        public static IEnumerable<Technical_BreastAndCervicalCancer> GetBreastAndCervicalCancerHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_BreastAndCervicalCancer> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_BreastAndCervicalCancer
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                            && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_BreastAndCervicalCancer
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_BreastAndCervicalCancer
                    .Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetBreastAndCervicalCancerAllActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_BreastAndCervicalCancer> GetBreastAndCervicalCancerAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_BreastAndCervicalCancer> allActiveRecords = context.Technical_BreastAndCervicalCancer
                .Where(
                    n =>
                        n.Person.ApplicationEntity.Any(
                            p =>
                                p.ApplicationID == applicationId &&
                                (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                 p.HistoryCode.Trim() == string.Empty)) &&
                        (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                        (n.HistoryCode == IntakeConstants.ONE_WHITE_SPACE || n.HistoryCode == null ||
                         n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                  .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }

        /// <summary>
        /// Delete newly added Breast and Cervical Cancer Information record When Click on Oops
        /// </summary>
        /// <param name="bccInfoId"></param>
        /// <returns></returns>
        public static void DeleteBCCInfoRecord(int bccInfoId)
        {
            var techcontext = ServicesDataHub.Technical;
            var bccinfo = techcontext.Technical_BreastAndCervicalCancer.Where(n => n.BreastAndCervicalCancerID == bccInfoId).FirstOrDefault();
            techcontext.UsePostTunneling = true;
            techcontext.DeleteObject(bccinfo);
            techcontext.SaveChanges();
        }

        #endregion

        #region "CRDP Information"


        /// <summary>
        /// Gets all history records.
        /// </summary>
        /// <param name="applicationId">appliation id</param>
        /// <param name="beginDate"> start dat eof the search</param>
        /// <param name="endDate">end date of the search</param>
        /// <returns>IEnumerable Technical_InstitutionInfo </returns>
        public static IEnumerable<Technical_CRDPInfo> GetHistoryRecordsCrdpInfo(int applicationId, Object beginDate, Object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_CRDPInfo> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_CRDPInfo.
                                                          Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                          && (n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                                          && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate))))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_CRDPInfo.
                                                          Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                                         n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_CRDPInfo.
                                                          Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                                         n.BeginDate <= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(endDate)))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetAllActiveRecordsCrdpInfo(applicationId);
            }

            return historyRecords;
        }

        /// <summary>
        /// Gets all active records.
        /// </summary>      
        /// <param name="applicationId"></param>
        /// <returns>Returns Object Of Technical_Disability</returns>
        public static IEnumerable<Technical_CRDPInfo> GetAllActiveRecordsCrdpInfo(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_CRDPInfo> allActiveRecords = context.Technical_CRDPInfo.Where
                (n =>
                    n.Person.ApplicationEntity.Any(
                        p =>
                            p.ApplicationID == applicationId &&
                            (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                            (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                             p.HistoryCode.Trim() == string.Empty)) &&
                    (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                    (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                     n.HistoryCode.Trim() == string.Empty))
                       .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }

        /// <summary>
        /// Creates a new record
        /// </summary>
        /// <returns>CRDP info Id </returns>
        public static Technical_CRDPInfo CreateNewCrdpInfoRecord()
        {
            var context = ServicesDataHub.Technical;
            var crdpInfoProvider = CreateCrdpInfoObject();
            context.AddToTechnical_CRDPInfo(crdpInfoProvider);
            context.SaveChanges();
            return crdpInfoProvider;
        }

        /// <summary>
        ///  Create a new Technical_CRDPInfo type of object
        /// </summary>
        /// <returns>Technical_CRDPInfo type of object</returns>
        private static Technical_CRDPInfo CreateCrdpInfoObject()
        {
            var crdpInfoProvider = new Technical_CRDPInfo
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            return crdpInfoProvider;
        }

        /// <summary>
        /// Checks if an individual has active record.
        /// </summary>
        /// <param name="personId">Application ID</param>
        /// <returns>Returns value true if record exists else false. </returns>
        public static bool IsCrdpInformationRecordExist(int personId)
        {
            using (var context = ServicesDataHub.Technical)
            {
                return context.Technical_CRDPInfo.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)
                        &&
                        (n.HistoryCode == null || n.HistoryCode.Trim() == string.Empty ||
                         n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE)).Count() > 0;
            }
        }

        /// <summary>
        ///Returns ID of the Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static Technical_CRDPInfo GetCRDPEndedRecID(int personId, Int16 historySeqNum)
        {
            var context = ServicesDataHub.Technical;
            return
                context.Technical_CRDPInfo.Where(
                    n =>
                        n.PersonID == personId &&
                        (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                         n.HistoryCode.Trim() == string.Empty)
                        && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfCRDPRec(int personId)
        {
            Int16 historySeqNum = 1;
            var techcontext = ServicesDataHub.Technical;
            var maxRecord = techcontext.Technical_CRDPInfo.Where(n => n.PersonID == personId).OrderByDescending(n => n.HistorySequenceNumber);
            if (maxRecord.Count() > 0)
            {
                historySeqNum = Convert.ToInt16(maxRecord.First().HistorySequenceNumber);
                historySeqNum++;
            }
            return historySeqNum;
        }

        /// <summary>
        /// Delete newly added CRDP record When Click on Oops
        /// </summary>
        /// <param name="protectedSsi"></param>
        /// <returns></returns>
        public static void DeleteCRDPRecord(int protectedSsi)
        {
            var techcontext = ServicesDataHub.Technical;
            var crdpInfo = techcontext.Technical_CRDPInfo.Where(n => n.CRDPInfoID == protectedSsi).First();
            techcontext.UsePostTunneling = true;
            techcontext.DeleteObject(crdpInfo);
            techcontext.SaveChanges();
        }
        #endregion

        #region "Additional Case Details"

        /// <summary>
        /// Creetes Blank Record in DisasterBenefitInfo table if no record exists.
        /// </summary>
        /// <param name="applicationId">ApplicationID</param>
        public static void CreateNewDisasterBenefitInfo(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            var appEntity = context.Technical_Application.
                Expand("DisasterBenefitInfo").Where(p => p.ApplicationID == applicationId).FirstOrDefault();

            var isNewRecord = false;

            if (appEntity != null && appEntity.DisasterBenefitInfo.Count == 0)
            {
                context.AddToTechnical_DisasterBenefitInfo(CreateNewDisasterBenefitInfoEntity(appEntity.ApplicationID));
                isNewRecord = true;
            }
            else
            {
                //Checks active record   if there are any existing records            
                if (appEntity != null)
                {
                    var activeRecord =
                        appEntity.DisasterBenefitInfo.Where(
                            n => (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                                 (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                  n.HistoryCode.Trim() == string.Empty)).FirstOrDefault();
                    if (activeRecord == null)
                    {
                        context.AddToTechnical_DisasterBenefitInfo(
                            CreateNewDisasterBenefitInfoEntity(appEntity.ApplicationID));
                        isNewRecord = true;
                    }
                }
            }

            //If there is atleast one record in the context to save.
            if (isNewRecord)
                context.SaveChanges();
        }

        /// <summary>
        /// Creates object of Technical_DisasterBenefitInfo.
        /// </summary>
        /// <param name="applicationId">ApplicationID</param>
        /// <returns>Returns object of Technical_DisasterBenefitInfo</returns>
        private static Technical_DisasterBenefitInfo CreateNewDisasterBenefitInfoEntity(int applicationId)
        {
            var newEntity = new Technical_DisasterBenefitInfo
            {
                ApplicationID = applicationId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = GetMaxHistorySeqNumOfDisasterBenefitInfo(applicationId),
                SequenceNumber = 1,

            };

            return newEntity;
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfDisasterBenefitInfo(int applicationId)
        {
            Int16 historySeqNum = 1;
            using (var techcontext = ServicesDataHub.Technical)
            {
                var maxRecord = techcontext.Technical_DisasterBenefitInfo.Where(n => n.ApplicationID == applicationId).OrderByDescending(n => n.HistorySequenceNumber);
                if (maxRecord.Count() > 0)
                {
                    historySeqNum = Convert.ToInt16(maxRecord.First().HistorySequenceNumber);
                    historySeqNum++;
                }
                return historySeqNum;
            }
        }

        /// <summary>
        ///Returns ID of the  Ended record.
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static int GetDisasterEndedRecId(int applicationId, Int16 historySeqNum)
        {
            using (var context = ServicesDataHub.Technical)
            {
                return
                    context.Technical_DisasterBenefitInfo.Where(
                        n =>
                            n.ApplicationID == applicationId &&
                            (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                             n.HistoryCode.Trim() == string.Empty)
                            && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault().DisasterBenefitInfoID;
            }
        }

        /// <summary>
        /// Gets all history records of DisasterBenefitInfo
        /// </summary>
        /// <param name="applicationId">ApplicationID</param>
        /// <param name="beginDate">BeginDate</param>
        /// <param name="endDate">EndDate</param>
        /// <returns>Returns object of Technical_DisasterBenefitInfo </returns>
        public static IEnumerable<Technical_DisasterBenefitInfo> GetDisasterBenefitsInfoHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_DisasterBenefitInfo> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_DisasterBenefitInfo
                    .Where(n => n.ApplicationID == applicationId &&
                                n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));

            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_DisasterBenefitInfo
                    .Where(n => n.ApplicationID == applicationId &&
                                n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)));
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_DisasterBenefitInfo
                    .Where(n => n.ApplicationID == applicationId &&
                                n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)));
            }
            else
            {
                return GetDisasterBeneftitsAllActiveRecords(applicationId);
            }
            return historyRecords;
        }

        /// <summary>
        /// Gets all active records of DisasterBenefitInfo.
        /// </summary>     
        /// <param name="applicationId">ApplicationID</param>
        /// <returns></returns>
        public static IEnumerable<Technical_DisasterBenefitInfo> GetDisasterBeneftitsAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_DisasterBenefitInfo> allActiveRecords =
                context.Technical_DisasterBenefitInfo.Where(n => n.ApplicationID == applicationId
                                                                 &&
                                                                 (n.DeleteReasonCode == null ||
                                                                  n.DeleteReasonCode.Trim() == string.Empty) &&
                                                                 (n.HistoryCode == null ||
                                                                  n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                                                  n.HistoryCode.Trim() == string.Empty));
            return allActiveRecords;
        }

        #endregion

        #region "Benefit Cap Information"

        /// <summary>
        /// Checks if the record exists for an Individual in the database, if not then inserts new record.
        /// </summary>
        /// <param name="applicationId"></param>
        public static void CreateNewBenefitCapInfo(int applicationId)
        {
            var techContext = ServicesDataHub.Technical;
            var isInserted = false;
            IEnumerable<Technical_Person> appPersons = ((DataServiceQuery<Technical_Person>)
                techContext.Technical_Entity.OfType<Technical_Person>()).
                Expand("BenefitCapInfo")
                .Where(
                    n =>
                        n.ApplicationEntity.Any(
                            p =>
                                p.ApplicationID == applicationId &&
                                (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                &&
                                (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                 p.HistoryCode.Trim() == string.Empty)));


            foreach (var person in appPersons)
            {
                //If Person is new or does not have an entry in Benefit Cap Information.
                if (person.BenefitCapInfo.Count == 0 ||
                    (person.BenefitCapInfo.Count != 0 &&
                     person.BenefitCapInfo.Where(
                         n => (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                              (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                               n.HistoryCode.Trim() == string.Empty)).FirstOrDefault() == null))
                {
                    techContext.AddToTechnical_BenefitCapInfo(CreateTechnicalBenefitCapInfo(person.EntityID));
                    isInserted = true;
                }
            }
            //Calling SaveChanges if there are new records inserted.
            if (isInserted)
                techContext.SaveChanges();
        }


        /// <summary>
        /// Creates an object of type Technical_LivingArrangement.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        private static Technical_BenefitCapInfo CreateTechnicalBenefitCapInfo(int personId)
        {
            if (personId == 0)
                throw new ArgumentException("Argument can not be zero.");

            var benefitCapInfo = new Technical_BenefitCapInfo
            {
                FirstInsertedByID = LoginUserId, //TODO: Replace with current logged in UserID.
                LastSavedByID = LoginUserId,  //TODO: Replace with current logged in UserID.
                PersonID = personId,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                HistorySequenceNumber = 1,
                SequenceNumber = 1
            };

            return benefitCapInfo;
        }

        /// <summary>
        /// Gets all history records or else active records
        /// </summary>
        /// <param name="applicationId">appliation id<</param>
        /// <param name="beginDate"> start date of the search</param>
        /// <param name="endDate">End date of the search</param>      
        /// <returns></returns>
        public static IEnumerable<Technical_BenefitCapInfo> GetBenefitCapInfoHistoryRecords(int applicationId,
            object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_BenefitCapInfo> historyRecords;
            if (beginDate != null && endDate != null)
            {
                //Unlike other hisory searches, this page doesn't have begin date at its details page therefore for 
                //history search "LastSavedDateTime" is used instead of "BeginDate"

                historyRecords = context.Technical_BenefitCapInfo.
                    Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty))
                            &&
                            (n.DB2UpdatedDate >=
                             TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                             &&
                             n.DB2UpdatedDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate))))
                                                          .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_BenefitCapInfo.
                    Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.DB2UpdatedDate >=
                            TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                                                         .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_BenefitCapInfo.
                    Where(
                        n =>
                            n.Person.ApplicationEntity.Any(
                                p =>
                                    p.ApplicationID == applicationId &&
                                    (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                                    (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                     p.HistoryCode.Trim() == string.Empty)) &&
                            n.DB2UpdatedDate <= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(endDate)))
                                                         .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetBenefitCapInfoAllActiveRecords(applicationId);
            }

            return historyRecords;
        }

        /// <summary>
        /// Gets all active records
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_BenefitCapInfo> GetBenefitCapInfoAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            var date19Years = SystemDateTime.Now.AddYears(-19);

            IEnumerable<Technical_BenefitCapInfo> allActiveRecords = context.Technical_BenefitCapInfo.Where
                (n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId
                                                          &&
                                                          (p.DeleteReasonCode == null ||
                                                           p.DeleteReasonCode.Trim() == string.Empty) &&
                                                          (p.HistoryCode == null ||
                                                           p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                                           p.HistoryCode.Trim() == string.Empty))
                      && n.Person.PersonAdditionalAttributes.DateOfBirthDate > date19Years
                      && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)
                      &&
                      (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                       n.HistoryCode.Trim() == string.Empty))
                                           .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }

        #endregion

        #region "Common Methods"

        /// <summary>
        /// Shows Disability Schedule Page.
        /// </summary>
        public static void DisabilitySchedulePage()
        {
            var technicalContext = ServicesDataHub.Technical;
            var housholdGeneralInfo =
                technicalContext.Technical_HouseholdGeneralInfo.Where(
                    n => n.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key))
                    .FirstOrDefault();
            if (housholdGeneralInfo != null && housholdGeneralInfo.IsDisabledWithWorkExpenseIndicator != "Y")
            {
                housholdGeneralInfo.IsDisabledWithWorkExpenseIndicator = "Y";
                technicalContext.UpdateObject(housholdGeneralInfo);
                technicalContext.SaveChanges();
                TechnicalQuesServiceCall();
            }
        }

        /// <summary>
        /// Technical Service Call
        /// </summary>
        public static void TechnicalQuesServiceCall()
        {
           // WWSyncPoints.TechnicalServiceCallToSync("DAEXQS02");
        }

        /// <summary>
        /// Flip Technical Questions
        /// </summary>
        /// <param name="technicalType"></param>
        /// <returns></returns>
        public static void FlipTechnicalQuestion(TechnicalType technicalType)
        {
            var context = ServicesDataHub.Technical;
            var techQuests = context.Technical_HouseholdGeneralInfo.Where(n => n.ApplicationID == int.Parse(WorkflowSession.Instance.RootFrame.State.Key)).First();
            switch (technicalType)
            {
                case TechnicalType.P:
                    techQuests.IsAnyonePregnantIndicator = "N";
                    break;
                case TechnicalType.D:
                    techQuests.ReceiveDisablityPaymentIndicator = "N";
                    break;
                case TechnicalType.HCBS:
                    techQuests.HasHCBSWaiverIndicator = "N";
                    break;
                case TechnicalType.SI:
                    techQuests.HasLTCwithSpouseinCommunity = "N";
                    break;
                case TechnicalType.PSSI:
                    techQuests.HadSSIRecipientIndicator = "N";
                    break;
                case TechnicalType.NB:
                    techQuests.Haslessthan13monthschildIndicator = "N";
                    break;
                case TechnicalType.CRDP:
                    techQuests.HasChronicRenalDiseaseProgramParticipantIndicator = "N";
                    break;
                case TechnicalType.BCC:
                    techQuests.IsReferredByDPHIndicator = "N";
                    break;

            }
            context.UpdateObject(techQuests);
            context.SaveChanges();
        }

        /// <summary>
        /// techincal Types
        /// </summary>
        public enum TechnicalType
        {
            [Description("Pregnancy")]
            P,
            [Description("Disability")]
            D,
            [Description("HomeCommunityBasedServices")]
            HCBS,
            [Description("SpousalImpoverishment")]
            SI,
            [Description("ProtectedSSI")]
            PSSI,
            [Description("ContinuouslyEligibleNewborns")]
            NB,
            [Description("CRDPInformation")]
            CRDP,
            [Description("BreastandCervicalCancerInformation")]
            BCC,
        }
        /// <summary>
        /// Compares two dictionaries and returns true if there is at least one differet pair
        /// </summary>
        /// <param name="oldDictionary"></param>
        /// <param name="newDictionary"></param>
        /// <returns></returns>
        public static bool IsCollectionSame(OrderedDictionary oldDictionary, OrderedDictionary newDictionary)
        {
            var status = false;
            var oldValues = oldDictionary.Cast<DictionaryEntry>().ToDictionary(k => k.Key, v => v.Value);
            var newValues = newDictionary.Cast<DictionaryEntry>().ToDictionary(k => k.Key, v => v.Value);
            var diffValue = newValues.Where(n => oldValues[n.Key].DefaultIfNull("str").ToString() != n.Value.DefaultIfNull("str").ToString()).ToDictionary(n => n.Key, n => n.Value);
            if (diffValue.Count() == 0) { status = true; }
            return status;
        }

        /// <summary>
        /// Compares two dictionaries and returns true if there is at least one differet pair
        /// </summary>
        /// <param name="oldValues"></param>
        /// <param name="newValues"></param>
        /// <returns></returns>
        public static bool IsUpdatedFormview(OrderedDictionary oldValues, OrderedDictionary newValues)
        {
            var status = false;

            var count = newValues.Count;

            for (var i = 0; i < count; i++)
            {
                var currentType = typeof(Nullable);

                if (oldValues[i] != null)
                {
                    currentType = oldValues[i].GetType();
                }
                else if (newValues[i] != null)
                {
                    currentType = newValues[i].GetType();
                }
                else
                {
                    continue;
                }

                if (currentType != typeof(Nullable))
                {
                    if (currentType == typeof(DateTime))
                    {
                        status = !DateTime.Equals(Convert.ToDateTime(oldValues[i]), Convert.ToDateTime(newValues[i]));
                    }
                    else if (currentType == typeof(Boolean))
                    {
                        status = (oldValues[i] == null && newValues[i] != null) ? true : !Boolean.Equals(Convert.ToBoolean(oldValues[i]), Convert.ToBoolean(newValues[i]));
                    }
                    else if (currentType == typeof(decimal))
                    {
                        status = !decimal.Equals(Convert.ToDecimal(oldValues[i]), Convert.ToDecimal(newValues[i]));
                    }
                    else if (currentType == typeof(double))
                    {
                        status = !decimal.Equals(Convert.ToDouble(oldValues[i]), Convert.ToDouble(newValues[i]));
                    }
                    else if (currentType == typeof(short))
                    {
                        status = !int.Equals(Convert.ToInt16(oldValues[i]), Convert.ToInt16(newValues[i]));
                    }
                    else if (currentType == typeof(int))
                    {
                        status = !int.Equals(Convert.ToInt64(oldValues[i]), Convert.ToInt64(newValues[i]));
                    }
                    else if (currentType == typeof(char))
                    {
                        status = !char.Equals(Convert.ToChar(oldValues[i]), Convert.ToChar(newValues[i]));
                    }
                    else if (currentType == typeof(string))
                    {
                        status = !string.Equals(Convert.ToString(oldValues[i]).Trim(), Convert.ToString(newValues[i]).Trim());
                    }

                    //break the loop if at least one value is changed
                    if (status)
                        break;
                }
            }

            return status;
        }

        /// <summary>
        /// Compares two dictionaries and returns true if there is at least one differet pair
        /// </summary>
        /// <param name="oldValues"></param>
        /// <param name="newValues"></param>
        /// <returns></returns>
        public static bool IsWICUpdatedFormview(OrderedDictionary oldValues, OrderedDictionary newValues)
        {
            var status = false;
            var IsWICReferred = false;
            var IsWICEnrolled = false;
            var IsBreastfeeding = false;
            var Ispregnant = false;

            IsWICReferred = (oldValues[IntakeConstants.WICReferred] == null && newValues[IntakeConstants.WICReferred] != null) ? true : !Boolean.Equals(Convert.ToBoolean(oldValues[IntakeConstants.WICReferred]), Convert.ToBoolean(newValues[IntakeConstants.WICReferred]));
            IsWICEnrolled = (oldValues[IntakeConstants.WICEnrolled] == null && newValues[IntakeConstants.WICEnrolled] != null) ? true : !Boolean.Equals(Convert.ToBoolean(oldValues[IntakeConstants.WICEnrolled]), Convert.ToBoolean(newValues[IntakeConstants.WICEnrolled]));
            IsBreastfeeding  = (oldValues[IntakeConstants.IsBreastFeeding] == null && newValues[IntakeConstants.IsBreastFeeding] != null) ? true : !Boolean.Equals(Convert.ToBoolean(oldValues[IntakeConstants.IsBreastFeeding]), Convert.ToBoolean(newValues[IntakeConstants.IsBreastFeeding]));
            Ispregnant = (oldValues[IntakeConstants.Ispregnant] == null && newValues[IntakeConstants.Ispregnant] != null) ? true : !Boolean.Equals(Convert.ToBoolean(oldValues[IntakeConstants.Ispregnant]), Convert.ToBoolean(newValues[IntakeConstants.Ispregnant]));

            if (IsWICReferred || IsWICEnrolled || IsBreastfeeding || Ispregnant)
            {
                status = true;
            }

            return status;
        }

        /// <summary>
        /// returns true if Female exists in the case otherwise false
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsFemaleExists()
        {
            var context = new TechnicalContextImpl();
            var person = context.Technical_Entity.OfType<Technical_Person>().
                Where(n => n.ApplicationEntity.Any(p => p.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key) && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                    && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))).Select(n => new { n.EntityID, n.PersonAdditionalAttributes.GenderCode });
            foreach (var personadd in person)
            {
                if (personadd.GenderCode.Trim() == "F")
                    return true;
            }
            return false;
        }

        public static bool IsFemaleExistsWIC(int personId)
        {
            var context = new TechnicalContextImpl();
            //var person = context.Technical_PersonAdditionalAttributes.OfType<Technical_PersonAdditionalAttributes>().Where(n => n.PersonID == personId).Select(n => new { n.PersonID, n.GenderCode });

            var person = context.Technical_PersonAdditionalAttributes.Where(n => n.PersonID == personId).First();

         
            
                if (person.GenderCode.Trim() == "F") { 
                    return true;
            } 
                return false;
            
            
        }

        /// <summary>
        ///  Defect - 82818 : Standard Zipcode Validation.
        /// </summary>
        /// <param name="zipCode"></param>
        /// <param name="stateCode"></param>
        /// <returns>string</returns>
        public static string ValidateZipCode(string zipCode, string stateCode)
        {
            string message = string.Empty;
            int contains = zipCode.Length;
            zipCode = zipCode.Replace("-", "");


            if (!String.IsNullOrEmpty(zipCode))
            {
                if (!zipCode.All(char.IsDigit))
                {
                    message = IntakeResourceManager.ZIPCODE_INVALID;
                    return message;
                }

                ReferenceTableLookupContext Reference = new ReferenceTableLookupContext();
                Reference.TableName = IntakeConstants.TABLE_NAME;
                Reference.ValueField = IntakeConstants.VALUE_FIELD;
                Reference.DisplayField = IntakeConstants.DISPLAY_FIELD;
                var tabledata = new System.Collections.Generic.List<KeyValuePair<string, string>>();
                tabledata = Reference.Values.ToList();
                bool Exists = false;

                if (contains >= 5)
                    Exists = tabledata.Exists(p => p.Key == zipCode.Substring(0, 5)); // Compare only first 5 digits of zipcode.

                if (Convert.ToInt32(zipCode) == 0)
                {
                    message = IntakeResourceManager.ZIPCODE_NOT_ZERO;
                    return message;
                }
                if ((contains != 5 && contains != 9) || (contains != 9 && contains != 5))
                {
                    message = IntakeResourceManager.ZIPCODE_LENGTH;
                    return message;
                }
                else if (stateCode == IntakeConstants.STATE_DE)
                {
                    if (!Exists)
                    {
                        message = IntakeResourceManager.ZIPCODE_NOT_DE;
                        return message;
                    }
                }
                else if (Exists)
                {
                    message = IntakeResourceManager.ZIPCODE_INVALID;
                    return message;
                }
            }

            return message;

        }            

        #endregion

        #region "Technical Questions"

        /// <summary>
        /// returns true if Individual Age is less than 13 months.
        /// </summary>
        /// <returns></returns>
        public static bool IsNewBornAdded()
        {
            var context = ServicesDataHub.Technical;
            var person = context.Technical_Entity.OfType<Technical_Person>().
                Where(n => n.ApplicationEntity.Any(p => p.ApplicationID == Convert.ToInt32(WorkflowSession.Instance.RootFrame.State.Key) && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                    && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))).Select(n => new { n.EntityID, n.PersonAdditionalAttributes.DateOfBirthDate });
            foreach (var personadd in person)
            {

                if (TechnicalBusinessLogic.IsNewBornEligible(Convert.ToDateTime(personadd.DateOfBirthDate)))
                    return true;
            }
            return false;
        }

        #endregion

        #region Volunteering / Work Program / Unpaid Work 
        
        public static IEnumerable<Technical_VolunteeringWorkProgram> GetVolunteeringWorkProgramAllActiveRecords(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_VolunteeringWorkProgram> allActiveRecords = context.Technical_VolunteeringWorkProgram.Expand("Person").
                Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId &&
                (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                .OrderBy(k => k.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return allActiveRecords;
        }
        public static IEnumerable<Technical_VolunteeringWorkProgram> GetVolunteeringWorkProgramAllHistoryRecords(int applicationId, object beginDate, object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_VolunteeringWorkProgram> historyRecords;

            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_VolunteeringWorkProgram.
                Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId &&
                (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
                (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty) &&
                 n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)) &&
                 n.BeginDate <= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(endDate)))
                .OrderBy(k => k.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_VolunteeringWorkProgram.
              Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId &&
              (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
              (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
              (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
              (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty) &&
                 n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
              .OrderBy(k => k.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_VolunteeringWorkProgram.
              Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId &&
              (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) &&
              (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
              (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
              (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty) &&
                 n.BeginDate <= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(endDate)))
              .OrderBy(k => k.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                historyRecords = GetVolunteeringWorkProgramAllActiveRecords(applicationId);

            }
            return historyRecords;
        }

        public static List<Technical_VolunteeringWorkProgramDetails> GetMonthlyParticipationRecords(int volunteeringWorkProgramId)
        {
            var context = ServicesDataHub.Technical;
            return context.Technical_VolunteeringWorkProgramDetails
                .Where(n => n.VolunteeringWorkProgramID == volunteeringWorkProgramId
                && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)
                && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                .ToList()
                .OrderBy(n => n.ProgramMonthDate)
                .ToList();
        }

        public static bool MonthlyParticipationExists(int volunteeringWorkProgramId, DateTime programMonth,int excludeDetailsId)
        {
            var context = ServicesDataHub.Technical;
            return context.Technical_VolunteeringWorkProgramDetails
                .Where(n => n.VolunteeringWorkProgramID == volunteeringWorkProgramId
                && (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)
                && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                .ToList()
                .Any(n => n.VolunteeringWorkProgramDetailsID != excludeDetailsId
                && n.ProgramMonthDate.HasValue
                && n.ProgramMonthDate.Value.Year == programMonth.Year
                && n.ProgramMonthDate.Value.Month == programMonth.Month);
        }

        public static void InsertMonthlyParticipation(int volunteeringWorkProgramId, DateTime programMonth, short hours)
        {
            var context = ServicesDataHub.Technical;
            DateTime now = SystemDateTime.Now;
            var entity = new Technical_VolunteeringWorkProgramDetails
            {
                VolunteeringWorkProgramID = volunteeringWorkProgramId,
                ProgramMonthDate = programMonth,
                HoursNumber = hours,
                SequenceNumber = 1,
                HistorySequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                FirstInsertedByID = LoginUserId,
                FirstInsertedDateTime = now,
                LastSavedByID = LoginUserId,
                LastSavedDateTime = now,
                UpdatedByText = LoginUserId,
                UpdatedDateTime = now
            };
            context.UsePostTunneling = true;
            context.AddToTechnical_VolunteeringWorkProgramDetails(entity);
            context.SaveChanges();
        }

        public static void UpdateMonthlyParticipation(int detailsId, DateTime programMonth, short hours)
        {
            var context = ServicesDataHub.Technical;
            var entity = context.Technical_VolunteeringWorkProgramDetails
                .Where(n => n.VolunteeringWorkProgramDetailsID == detailsId).First();
            entity.ProgramMonthDate = programMonth;
            entity.HoursNumber = hours;
            entity.LastSavedByID = LoginUserId;
            entity.LastSavedDateTime = SystemDateTime.Now;
            entity.UpdatedByText = LoginUserId;
            entity.UpdatedDateTime = SystemDateTime.Now;

            context.UsePostTunneling = true;
            context.UpdateObject(entity);
            context.SaveChanges();
        }

        public static void DeleteMonthlyParticipation(int detailsId)
        {
            var context = ServicesDataHub.Technical;
            var entity = context.Technical_VolunteeringWorkProgramDetails
                .Where(n => n.VolunteeringWorkProgramDetailsID == detailsId).First();

            entity.HistoryCode = HistoryCodeConstants.HISTORY;
            entity.DeleteReasonCode = HistoryCodeConstants.HISTORY;
            entity.LastSavedByID = LoginUserId;
            entity.LastSavedDateTime = SystemDateTime.Now;
            entity.UpdatedByText = LoginUserId;
            entity.UpdatedDateTime = SystemDateTime.Now;

            context.UsePostTunneling = true;
            context.UpdateObject(entity);
            context.SaveChanges();
        }

        public static void DeleteVolunteeringWorkRecord(int volunteeringId)
        {
            var techcontext = ServicesDataHub.Technical;
            var volunteeringRecord = techcontext.Technical_VolunteeringWorkProgram.Where(n => n.VolunteeringWorkProgramID == volunteeringId).First();
            techcontext.UsePostTunneling = true;
            techcontext.DeleteObject(volunteeringRecord);
            techcontext.SaveChanges();
        }

        public static Technical_VolunteeringWorkProgram CreateNewVolunteeringWorkProgramRecord(int personId)
        {
            var context = ServicesDataHub.Technical;
            var record = new Technical_VolunteeringWorkProgram
            {
                PersonID = personId,
                BeginDate = SystemDateTime.Now.Date,
                ProgramTypeCode = string.Empty,
                ProgramNameText = string.Empty,
                SequenceNumber = 1,
                HistorySequenceNumber = 1,
                HistoryCode = HistoryCodeConstants.ACTIVE
            };
            context.UsePostTunneling = true;
            context.AddToTechnical_VolunteeringWorkProgram(record);
            context.SaveChanges();
            return record;
        }

        public static Technical_VolunteeringWorkProgram InsertVolunteeringWorkProgram(Technical_VolunteeringWorkProgram request)
        {
            if(request == null)
            {
                throw new ArgumentNullException("request");
            }
            if(request.PersonID <= 0)
            {
                throw new InvalidOperationException("A person must be selected.");
            }
            if(request.BeginDate == DateTime.MinValue)
            {
                throw new InvalidOperationException("Begin Date is required.");
            }
            if (string.IsNullOrWhiteSpace(request.ProgramTypeCode))
            {
                throw new InvalidOperationException("Type of program is required.");
            }
            if (string.IsNullOrWhiteSpace(request.ProgramNameText))
            {
                throw new InvalidOperationException("Name of program is required.");
            }
            var technicalContext = ServicesDataHub.Technical;
            DateTime now = SystemDateTime.Now;
            var entity = new Technical_VolunteeringWorkProgram
            {
                PersonID = request.PersonID,
                BeginDate = request.BeginDate,
                EndDate = request.EndDate,
                ProgramTypeCode = request.ProgramTypeCode.Trim(),
                ProgramNameText = request.ProgramNameText.Trim(),
                VerifiedByCode = request.VerifiedByCode,
                DeleteReasonCode = request.DeleteReasonCode,
                SequenceNumber = 1,
                HistorySequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                FirstInsertedByID = LoginUserId,
                FirstInsertedDateTime = now,
                LastSavedByID = LoginUserId,
                LastSavedDateTime = now,
                UpdatedByText = LoginUserId,
                UpdatedDateTime = now
            };
            technicalContext.AddToTechnical_VolunteeringWorkProgram(entity);
            technicalContext.SaveChanges();
            return entity;
        }

        public static int GetPrimaryPersonId(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            var primaryPerson = context.Technical_PrimaryPerson
                .Where(n => n.ApplicationID == applicationId)
                .ToList()
                .FirstOrDefault(n => n.HistoryCode == null
                || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE
                || n.HistoryCode.Trim() == string.Empty);

            return primaryPerson == null ? 0 : primaryPerson.PersonID;
        }

        #endregion


        #region "Incarceration Details"

        /// <summary>
        /// Creats a new Incarceration record
        /// </summary>
        /// <returns></returns>
        ////// <summary>
        /// Creats a new record
        /// </summary>
        public static Technical_IncarcerationDetails CreateNewIncarcerationRecord()
        {
            var context = ServicesDataHub.Technical;
            //Delete duplicate records created by same user before creating new
            DeleteExistingduplicateRecords();
            var incarceratedRecord = CreateIncarcerationObject();
            context.AddToTechnical_IncarcerationDetails(incarceratedRecord);
            context.SaveChanges();
            return incarceratedRecord;

        }

        /// <summary>
        /// Deletes duplicate records created by same user 
        /// </summary> 
        /// <returns></returns>
        private static void DeleteExistingduplicateRecords()
        {
            var context = ServicesDataHub.Technical;
            var tobeDeletedRecords = context.Technical_IncarcerationDetails.Where (n => n.BeginDate == null && n.FirstInsertedByID == LoginUserId).ToList();
            if (tobeDeletedRecords.Count > 0)
            {
                foreach (var deleteRecord in tobeDeletedRecords)
                {
                    context.DeleteObject(deleteRecord);
                }
            }
            context.UsePostTunneling = true;
            context.SaveChanges();
        }

        /// <summary>
        /// Creats a Technical_IncarcerationDetails type of object
        /// </summary> 
        /// <returns>new Technical_InstitutionInfo type of object</returns>
        protected static Technical_IncarcerationDetails CreateIncarcerationObject()
        {
            var incarcerationRecord = new Technical_IncarcerationDetails
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                SequenceNumber = 1
            };
            return incarcerationRecord;
        }

        /// <summary>
        ///Returns ID of the Institution Ended record.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="historySeqNum"></param>
        /// <returns></returns>
        public static int GetIncarcerationEndedRecId(int personId, Int16 historySeqNum)
        {
            var context = ServicesDataHub.Technical;
            var endedRec = context.Technical_IncarcerationDetails.Where(n =>
                                                                    n.PersonID == personId &&
                                                                    (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                                                                     n.HistoryCode.Trim() == string.Empty)
                                                                    && (n.HistorySequenceNumber == historySeqNum + 1)).FirstOrDefault();

            return endedRec.IncarcerationDetailsID ;
        }


        /// <summary>
        /// Checks if an individual has active record.
        /// </summary>
        /// <param name="personId">Application ID</param>
        /// <returns>Returns value true if record exists else false. </returns>
        public static bool IsIncarcerationRecordExist(int personId)
        {
            using (var context = ServicesDataHub.Technical)
            {
                return context.Technical_IncarcerationDetails
                    .Where(
                        n =>
                            n.PersonID == personId &&
                            (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                            (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                             n.HistoryCode.Trim() == string.Empty)).Count() > 0;
            }
        }

        /// <summary>
        /// Checks if an individual has active record correctional Facility.
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static bool IsLivingAsCorrectional(int personId, int applicationId)
        {
            using (var context = ServicesDataHub.Technical)
            {
                return context.Technical_LivingArrangement 
                    .Where(
                        n =>
                            n.PersonID == personId && n.ApplicationID == applicationId && n.LivingArrngmtTypeCode == livingArrangementCorrectional &&
                            (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                            (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE ||
                             n.HistoryCode.Trim() == string.Empty)).Count() > 0;
            }
        }

        /// <summary>
        /// Returns max history sequence number of an individual.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public static Int16 GetMaxHistorySeqNumOfIncarcerationRec(int personId)
        {
            Int16 historySeqNum = 1;

            var techcontext = ServicesDataHub.Technical;
            var maxRecord = techcontext.Technical_IncarcerationDetails.Where(n => n.PersonID == personId).OrderByDescending(n => n.HistorySequenceNumber);
            if (maxRecord.Count() > 0)
            {
                historySeqNum = Convert.ToInt16(maxRecord.First().HistorySequenceNumber);
                historySeqNum++;
            }

            return historySeqNum;
        }

        public static IEnumerable<Technical_IncarcerationDetails> GetHistoryRecordsIncarceration(int applicationId, Object beginDate, Object endDate)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_IncarcerationDetails> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_IncarcerationDetails.
                                                          Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId &&
                                                              (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty) && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty))
                                                          && (n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                                                          && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate))))
                                                          .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_IncarcerationDetails.
                                                         Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                             && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                                         n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                                                         .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_IncarcerationDetails.
                                                         Where(n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                             && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                                         n.BeginDate <= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(endDate)))
                                                         .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.HistorySequenceNumber);
            }
            else
            {
                return GetAllActiveRecordsIncarceration(applicationId);
            }

            return historyRecords;
        }
        public static IEnumerable<Technical_IncarcerationDetails> GetAllActiveRecordsIncarceration(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            IEnumerable<Technical_IncarcerationDetails> activeIncarceration = context.Technical_IncarcerationDetails.Where
                                                          (n => n.Person.ApplicationEntity.Any(p => p.ApplicationID == applicationId && (p.DeleteReasonCode == null || p.DeleteReasonCode.Trim() == string.Empty)
                                                              && (p.HistoryCode == null || p.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || p.HistoryCode.Trim() == string.Empty)) &&
                                                         (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty) &&
                                                         (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                                                         .OrderBy(K => K.Person.PersonAdditionalAttributes.MCINumber).ThenBy(k => k.SequenceNumber).ThenBy(k => k.HistorySequenceNumber);
            return activeIncarceration;
        }

        #endregion

        #region "Community Engagement" 
        /// <summary>
        /// GetHistoryRecordsCommunityEngagementSummary
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_CommunityEngagementSummary> GetHistoryRecordsCommunityEngagementSummary(int applicationId, Object beginDate, Object endDate)
        {
            var context = ServicesDataHub.Technical;
            var personIds = GetApplicationPersonIdsForCommunityEngagement(applicationId);
            IEnumerable<Technical_CommunityEngagementSummary> historyRecords;
            if (beginDate != null && endDate != null)
            {
                historyRecords = context.Technical_CommunityEngagementSummary
                    .Where(n => n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate))
                             && n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                    .ToList()
                    .Where(n => personIds.Contains(n.PersonID))
                    .OrderBy(n => n.PersonID).ThenBy(n => n.HistorySequenceNumber);
            }
            else if (beginDate != null && endDate == null)
            {
                historyRecords = context.Technical_CommunityEngagementSummary
                    .Where(n => n.BeginDate >= TechnicalCommon.GetDateWithFirstDayOfMonth(Convert.ToDateTime(beginDate)))
                    .ToList()
                    .Where(n => personIds.Contains(n.PersonID))
                    .OrderBy(n => n.PersonID).ThenBy(n => n.HistorySequenceNumber);
            }
            else if (beginDate == null && endDate != null)
            {
                historyRecords = context.Technical_CommunityEngagementSummary
                    .Where(n => n.BeginDate <= TechnicalCommon.GetDateWithLastDayOfMonth(Convert.ToDateTime(endDate)))
                    .ToList()
                    .Where(n => personIds.Contains(n.PersonID))
                    .OrderBy(n => n.PersonID).ThenBy(n => n.HistorySequenceNumber);
            }
            else
            {
                return GetAllActiveRecordsCommunityEngagementSummary(applicationId);
            }

            return historyRecords;
        }

        /// <summary>
        /// GetAllActiveRecordsCommunityEngagementSummary
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static IEnumerable<Technical_CommunityEngagementSummary> GetAllActiveRecordsCommunityEngagementSummary(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            var personIds = GetApplicationPersonIdsForCommunityEngagement(applicationId);
            var activeRecords = context.Technical_CommunityEngagementSummary
                .Where(n => (n.DeleteReasonCode == null || n.DeleteReasonCode.Trim() == string.Empty)
                         && (n.HistoryCode == null || n.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || n.HistoryCode.Trim() == string.Empty))
                .ToList()
                .Where(n => personIds.Contains(n.PersonID))
                .OrderBy(n => n.PersonID).ThenBy(n => n.HistorySequenceNumber);
            return activeRecords;
        }

        /// <summary>
        /// GetApplicationPersonIdsForCommunityEngagement
        /// </summary>
        private static List<int> GetApplicationPersonIdsForCommunityEngagement(int applicationId)
        {
            var context = ServicesDataHub.Technical;
            var appEntity = context.Technical_ApplicationEntity
                .Where(a => a.ApplicationID == applicationId
                         && (a.DeleteReasonCode == null || a.DeleteReasonCode.Trim() == string.Empty)
                         && (a.HistoryCode == null || a.HistoryCode == IntakeConstants.ACTIVE_RECORD_CODE || a.HistoryCode.Trim() == string.Empty))
                .Select(a => new { a.EntityID })
                .ToList();
            return appEntity.Select(a => a.EntityID).ToList();
        }
        public static Technical_CommunityEngagementMedicalDetails CreateNewCommunityEngagementMedicalDetails(int communityEngagementSummaryID)
        {
            var techContext = ServicesDataHub.Technical;

            var communityEngagementMedicalDetails = new Technical_CommunityEngagementMedicalDetails
            {
                CommunityEngagementSummaryID = communityEngagementSummaryID,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };

            techContext.AddToTechnical_CommunityEngagementMedicalDetails(communityEngagementMedicalDetails);
            techContext.SaveChanges();
            return communityEngagementMedicalDetails;
        }
        public static Technical_CommunityEngagementHardshipWaiver CreateNewCommunityEngagementHardshipWaiver(int communityEngagementSummaryID)
        {
            var techContext = ServicesDataHub.Technical;

            var communityEngagementHardshipWaiver = new Technical_CommunityEngagementHardshipWaiver
            {
                CommunityEngagementSummaryID = communityEngagementSummaryID,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            techContext.AddToTechnical_CommunityEngagementHardshipWaiver(communityEngagementHardshipWaiver);
            techContext.SaveChanges();
            return communityEngagementHardshipWaiver;
        }

        public static Technical_CommunityEngagementSummary CreateNewCommunityEngagementObject()
        {
            var techContext = ServicesDataHub.Technical;
            var communityEngagementSummary = new Technical_CommunityEngagementSummary
            {
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            techContext.AddToTechnical_CommunityEngagementSummary(communityEngagementSummary);
            techContext.SaveChanges();
            var csId = communityEngagementSummary.CommunityEngagementSummaryID;
            var communityEngagement = new Technical_CommunityEngagement
            {
                CommunityEngagementSummaryID = csId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            techContext.AddToTechnical_CommunityEngagement(communityEngagement);


            var communityEngagementMedicalDetails = new Technical_CommunityEngagementMedicalDetails
            {
                CommunityEngagementSummaryID = csId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            techContext.AddToTechnical_CommunityEngagementMedicalDetails(communityEngagementMedicalDetails);
            var communityEngagementHardshipWaiver = new Technical_CommunityEngagementHardshipWaiver
            {
                CommunityEngagementSummaryID = csId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            techContext.AddToTechnical_CommunityEngagementHardshipWaiver(communityEngagementHardshipWaiver);

            communityEngagementSummary.CommunityEngagement.Add(communityEngagement);
            communityEngagementSummary.CommunityEngagementHardshipWaiver.Add(communityEngagementHardshipWaiver);
            communityEngagementSummary.CommunityEngagementMedicalDetails.Add(communityEngagementMedicalDetails);
            techContext.AddLink(communityEngagementSummary, "CommunityEngagement", communityEngagement);
            techContext.AddLink(communityEngagementSummary, "CommunityEngagementHardshipWaiver", communityEngagementHardshipWaiver);
            techContext.AddLink(communityEngagementSummary, "CommunityEngagementMedicalDetails", communityEngagementMedicalDetails);
            techContext.SaveChanges();
            return communityEngagementSummary;
        }

        /// <summary>
        /// CreateNewCommunityEngagement
        /// </summary>
        public static Technical_CommunityEngagementSummary CreateNewCommunityEngagement(int personId)
        {
            var techContext = ServicesDataHub.Technical;
            var communityEngagementSummary = new Technical_CommunityEngagementSummary
            {
                PersonID = personId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            techContext.AddToTechnical_CommunityEngagementSummary(communityEngagementSummary);
            techContext.SaveChanges();
            var csId = communityEngagementSummary.CommunityEngagementSummaryID;

            var communityEngagement = new Technical_CommunityEngagement
            {
                CommunityEngagementSummaryID = csId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            techContext.AddToTechnical_CommunityEngagement(communityEngagement);

            var communityEngagementMedicalDetails = new Technical_CommunityEngagementMedicalDetails
            {
                CommunityEngagementSummaryID = csId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            techContext.AddToTechnical_CommunityEngagementMedicalDetails(communityEngagementMedicalDetails);

            var communityEngagementHardshipWaiver = new Technical_CommunityEngagementHardshipWaiver
            {
                CommunityEngagementSummaryID = csId,
                FirstInsertedByID = LoginUserId,
                LastSavedByID = LoginUserId,
                HistorySequenceNumber = 1,
                SequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE
            };
            techContext.AddToTechnical_CommunityEngagementHardshipWaiver(communityEngagementHardshipWaiver);

            communityEngagementSummary.CommunityEngagement.Add(communityEngagement);
            communityEngagementSummary.CommunityEngagementHardshipWaiver.Add(communityEngagementHardshipWaiver);
            communityEngagementSummary.CommunityEngagementMedicalDetails.Add(communityEngagementMedicalDetails);
            techContext.AddLink(communityEngagementSummary, "CommunityEngagement", communityEngagement);
            techContext.AddLink(communityEngagementSummary, "CommunityEngagementHardshipWaiver", communityEngagementHardshipWaiver);
            techContext.AddLink(communityEngagementSummary, "CommunityEngagementMedicalDetails", communityEngagementMedicalDetails);
            techContext.SaveChanges();
            return communityEngagementSummary;
        }

        /// <summary>
        /// EnsureCommunityEngagementRecordsForApplication
        /// </summary>
        public static void EnsureCommunityEngagementRecordsForApplication(int applicationId)
        {
            var applicationPersonIds = GetApplicationPersonIdsForCommunityEngagement(applicationId).Distinct().ToList();
            if (applicationPersonIds.Count == 0) return;

            var existingPersonIds = GetAllActiveRecordsCommunityEngagementSummary(applicationId)
                .Select(n => n.PersonID)
                .Distinct()
                .ToList();

            foreach (var personId in applicationPersonIds)
            {
                if (!existingPersonIds.Contains(personId))
                {
                    CreateNewCommunityEngagement(personId);
                }
            }
        }

        /// <summary>
        /// Create OR Update CommunityEngagementObject
        /// </summary>
        /// <param name="communityEngagementObject"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public static Technical_CommunityEngagement CreateOrUpdateCommunityEngagementObject(Technical_CommunityEngagement communityEngagementObject, int communityEngagementID, int applicationId)
        {
            var techContext = ServicesDataHub.Technical;
            var communityEngagementDetailsRecord = new Technical_CommunityEngagement
            {
                CommunityEngagementSummaryID = communityEngagementObject.CommunityEngagementSummaryID,
                BeginDate = communityEngagementObject.BeginDate,
                RegularTakecareIndicator = communityEngagementObject.RegularTakecareIndicator,
                ParentOrLegalGuardianIndicator = communityEngagementObject.ParentOrLegalGuardianIndicator,
                CareTakerRelationshipCode = communityEngagementObject.CareTakerRelationshipCode,
                ReceivedProvidingCareCode = communityEngagementObject.ReceivedProvidingCareCode,
                CorrectionalInLast12MonthsIndicator = communityEngagementObject.CorrectionalInLast12MonthsIndicator,
                LiveWithPersonBeingCaredForCode = communityEngagementObject.LiveWithPersonBeingCaredForCode,
                CorrectionalInLast12MonthsVerifiedByCode = communityEngagementObject.CorrectionalInLast12MonthsVerifiedByCode,
                ReceivedProvidingCareVerifiedByCode = communityEngagementObject.ReceivedProvidingCareVerifiedByCode,
                ParticipatingInWorkProgramIndicator = communityEngagementObject.ParticipatingInWorkProgramIndicator,
                ParticipatingInUnpaidWorkIndicator = communityEngagementObject.ParticipatingInUnpaidWorkIndicator,
                FirstInsertedByID = LoginUserId,
                FirstInsertedDateTime = DateTime.Now,//communityEngagementObject.FirstInsertedDateTime,
                StopProvidingCareDate = communityEngagementObject.StopProvidingCareDate,
                LastSavedByID = LoginUserId,
                LastSavedDateTime = DateTime.Now,
                HistorySequenceNumber = 1,
                HistoryCode = IntakeConstants.ACTIVE_RECORD_CODE,
                SequenceNumber = 1,
            };
            techContext.AddToTechnical_CommunityEngagement(communityEngagementDetailsRecord);
            techContext.SaveChanges();

            return communityEngagementDetailsRecord;
        }
        #endregion 
    }
}