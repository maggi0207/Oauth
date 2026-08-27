///////////////////////////////////////////////////////////////////////////////////////////////////////
//
// File:      TaxDependencyInformationSummary.aspx.cs
//
// Created On: Monday, April 15, 2013 9:00:01 AM
// Created By: Keerthi.Pathipaka
//
// This file may contain sensitive and/or confidential information and may not be
// distributed without written permission of Delaware Department of Health and 
// Social Services.
//
// #      Type        User                    Date        Comment                                      
// ------ ----------- ----------------------- ----------- -------------------------------------------- 
// 12036	   add	   Keerthi.Pathipaka        4/15/2013   Added new files in Technical 
///////////////////////////////////////////////////////////////////////////////////////////////////////

using Dhss.Assist.WorkerWeb.Web.Infrastructure.Context;
using Dhss.Framework.DataAnnotations;

namespace Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical
{
    public partial class TaxDependencyInformationSummary
    {
        /// <summary>
        /// TaxDependencyInformationSummaryMetadata
        /// </summary>
        protected class TaxDependencyInformationSummaryMetadata
        {
            /// <summary>
            /// Gets or Sets the ApplicationEntityID.
            /// </summary>
            /// <value>The ApplicationEntityID.</value>
            [LookupTable(typeof(PersonNameWithAppEntityId))]
            public string ApplicationEntityID { get; set; }

            /// <summary>
            /// Gets or Sets the FileTaxReturnInCurrentYearIndicator.
            /// </summary>
            /// <value>The FileTaxReturnInCurrentYearIndicator.</value>
            [LookupTable("AERSPE", "RESPONSE-CD", "RESPONSE-DESC", typeof(ReferenceTableLookupContext))]
            public string FileTaxReturnInCurrentYearIndicator { get; set; }

            /// <summary>
            /// Gets or Sets the HasTaxDeductionIndicator.
            /// </summary>
            /// <value>The HasTaxDeductionIndicator.</value>
            [LookupTable("SWYSNO", "CODE-TXT", "DESC-TXT", typeof(ReferenceTableLookupContext))]
            public string HasTaxDeductionIndicator { get; set; }

            /// <summary>
            /// Gets or Sets the PrimaryTaxFilerIndicator.
            /// </summary>
            /// <value>The PrimaryTaxFilerIndicator.</value>
            [LookupTable("SWYSNO", "CODE-TXT", "DESC-TXT", typeof(ReferenceTableLookupContext))]
            public string PrimaryTaxFilerIndicator { get; set; }
        }

        /// <summary>
        /// Save data on page.
        /// </summary>
        public override void SaveData()
        {
        }

        /// <summary>
        /// Navigates the previous page.
        /// </summary>
        public override void NavigatePrevious()
        {
            ClearSessionVariables();
            TechnicalSessionContext.Instance.IsFromPrevious = "Y";
            base.NavigatePrevious(n => !n.DetailScreen && n.Completed && n.Visible);
        }

        /// <summary>
        /// Clears Session variable for history record
        /// </summary>
        protected void ClearSessionVariables()
        {
            TechnicalSessionContext.Instance.TaxDependentID = 0;
            TechnicalSessionContext.Instance.IsBackToSummary = false;
            TechnicalSessionContext.Instance.IsSaved = false;
        }
    }
}

