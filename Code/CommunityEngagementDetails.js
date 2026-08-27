
//  Conditional show/hide 

function InitializeEndDate(s, id) { }

function showRow(id, show) {
    var row = document.getElementById(id);
    if (row) {
        row.style.display = show ? "" : "none";
    }

    var controlNames = rowControlMap[id];
    if (controlNames) {
        for (var i = 0; i < controlNames.length; i++) {
            var control = window[controlNames[i]];
            if (control && control.SetVisible) {
                control.SetVisible(show);
            }
        }
    }
}

var rowControlMap = {
    "trWho": ["cbWho"],
    "trParentOrLegalGuardian": ["cbParentOrLegalGuardian"],
    "trProvideCare": ["cbWhenLegalGuardianProvideCare"],
    "trStopProvidingCareDate": ["ddeStopLegalGuardianProvideCare"],
    "trCareTakerRelationship": ["cbCareTakerRelationship"],
    "trProvideCare1": ["cbWhenCareTakerRelationship"],
    "trStopProvidingCareDate2": ["ddeStopProvidingCareDateTime"],
    "trLiveWithPersonBeingCaredFor": ["cbLiveWithPersonBeingCaredFor"],
    "trStopLivingwithPersonDate": ["ddeStopLivingwithPersonDate"],
    "trStopProvidingCareDateMain": ["ddeStopTakingCareDate"],
    "trStopLivingWithPersonWhileGivingCareDate": ["ddeStopLivingWithPersonWhileGivingCareDate"],
    "trStopTakingCarePersonDate": ["ddeStopTakingCarePersonDate"],
    "trReceivedProvidingCare": ["cbReceivedProvidingCare"],
    "trReleasedDate": ["ddeCorrectionalReleasedDates"],
    "trSeriousMedicalconditionStatus": ["cbSeriousMedicalconditionStatusCode"],
    "trEndSeriousConditionDate": ["ddeEndSeriousConditionDate"],
    "trSubstanceUseDisorderStatus": ["cbSubstanceUseDisorderStatusCode"],
    "trEndSubstanceDisorderDate": ["ddeEndSubstanceDisorderDate"],
    "trWhenDetermined": ["cbWhenDetermined"],
    "trEndSSADeterminationDate": ["ddeEndSSADeterminationDate"],
    "trWhenDisablingMentalDisorder": ["cbWhenDisablingMentalDisorder"],
    "trEndDisablingMentalDisorderDate": ["ddeEndDisablingMentalDisorderDate"],
    "trWhenPhysicalDisability": ["cbWhenPhysicalDisability"],
    "trEndPhysicalDisabilityDate": ["ddeEndPhysicalDisabilityDate"]
};

function setRequired(control, required) {
    if (control && control.SetRequired) {
        control.SetRequired(required);
    }
}

function isYes(combo) {
    if (!combo) return false;
    var value = combo.GetValue();
    if (value == null) return false;
    if (typeof value === "boolean") return value === true;
    var text = value.toString().toLowerCase();
    return text === "yes" || text === "true" || text === "1";
}

function textContains(combo, keyword) {
    if (!combo) return false;
    var text = combo.GetText();
    return text != null && text.toLowerCase().indexOf(keyword) !== -1;
}

function clearCombo(combo) {
    if (combo) combo.SetValue(null);
}

function clearDate(dateEdit) {
    if (dateEdit) dateEdit.SetDate(null);
}

function RegularTakecareIndicator(s) {
    var showChildren = isYes(s);
    showRow("trWho", showChildren);
    setRequired(window.cbWho, showChildren);
    showRow("trParentOrLegalGuardian", showChildren);
    setRequired(window.cbParentOrLegalGuardian, showChildren);
    if (!showChildren) {
        clearCombo(window.cbWho);
        clearCombo(window.cbParentOrLegalGuardian);
        fncParentOrLegalGuardianChange();
    }
}

function fncParentOrLegalGuardianChange() {
    var cb = window.cbParentOrLegalGuardian;
    var isYesAnswer = isYes(cb);
    var isNoAnswer = cb && cb.GetValue() != null && !isYesAnswer;

    showRow("trProvideCare", isYesAnswer);
    setRequired(window.cbWhenLegalGuardianProvideCare, isYesAnswer);
    if (!isYesAnswer) {
        showRow("trStopProvidingCareDate", false);
        setRequired(window.ddeStopLegalGuardianProvideCare, false);
        clearCombo(window.cbWhenLegalGuardianProvideCare);
        clearDate(window.ddeStopLegalGuardianProvideCare);
    } else {
        fncWhenLegalGuardianProvideCareChange();
    }

    showRow("trCareTakerRelationship", isNoAnswer);
    setRequired(window.cbCareTakerRelationship, isNoAnswer);
    if (!isNoAnswer) {
        showRow("trProvideCare1", false);
        showRow("trStopProvidingCareDate2", false);
        showRow("trLiveWithPersonBeingCaredFor", false);
        showRow("trStopLivingwithPersonDate", false);
        showRow("trStopProvidingCareDateMain", false);
        showRow("trStopLivingWithPersonWhileGivingCareDate", false);
        showRow("trStopTakingCarePersonDate", false);
        showRow("trReceivedProvidingCare", false);
        setRequired(window.cbWhenCareTakerRelationship, false);
        setRequired(window.ddeStopProvidingCareDateTime, false);
        setRequired(window.cbLiveWithPersonBeingCaredFor, false);
        setRequired(window.ddeStopLivingwithPersonDate, false);
        setRequired(window.ddeStopTakingCareDate, false);
        setRequired(window.ddeStopLivingWithPersonWhileGivingCareDate, false);
        setRequired(window.ddeStopTakingCarePersonDate, false);
        setRequired(window.cbReceivedProvidingCare, false);
        clearCombo(window.cbCareTakerRelationship);
        clearCombo(window.cbWhenCareTakerRelationship);
        clearDate(window.ddeStopProvidingCareDateTime);
        clearCombo(window.cbLiveWithPersonBeingCaredFor);
        clearDate(window.ddeStopLivingwithPersonDate);
        clearDate(window.ddeStopTakingCareDate);
        clearDate(window.ddeStopLivingWithPersonWhileGivingCareDate);
        clearDate(window.ddeStopTakingCarePersonDate);
        clearCombo(window.cbReceivedProvidingCare);
    } else {
        fncCareTakerRelationshipChange();
    }
    updateCaretakerVerifiedByRequired();
}

function fncWhenLegalGuardianProvideCareChange(s) {
    var cb = s || window.cbWhenLegalGuardianProvideCare;
    var showStopDate = textContains(cb, "no longer");
    showRow("trStopProvidingCareDate", showStopDate);
    setRequired(window.ddeStopLegalGuardianProvideCare, showStopDate);
    if (!showStopDate) {
        clearDate(window.ddeStopLegalGuardianProvideCare);
    }
    updateCaretakerVerifiedByRequired();
}

function fncCareTakerRelationshipChange(s) {
    var cb = s || window.cbCareTakerRelationship;
    var hasValue = cb && cb.GetValue() != null;
    var isOtherOrNotRelated = textContains(cb, "other relation") || textContains(cb, "not related");
    var isRelated = hasValue && !isOtherOrNotRelated;

    showRow("trProvideCare1", isRelated);
    setRequired(window.cbWhenCareTakerRelationship, isRelated);
    if (!isRelated) {
        showRow("trStopProvidingCareDate2", false);
        setRequired(window.ddeStopProvidingCareDateTime, false);
        clearCombo(window.cbWhenCareTakerRelationship);
        clearDate(window.ddeStopProvidingCareDateTime);
    } else {
        fncWhenCareTakerRelationshipChange();
    }

    showRow("trLiveWithPersonBeingCaredFor", isOtherOrNotRelated);
    setRequired(window.cbLiveWithPersonBeingCaredFor, isOtherOrNotRelated);
    if (!isOtherOrNotRelated) {
        showRow("trStopLivingwithPersonDate", false);
        showRow("trStopProvidingCareDateMain", false);
        showRow("trStopLivingWithPersonWhileGivingCareDate", false);
        showRow("trStopTakingCarePersonDate", false);
        showRow("trReceivedProvidingCare", false);
        setRequired(window.ddeStopLivingwithPersonDate, false);
        setRequired(window.ddeStopTakingCareDate, false);
        setRequired(window.ddeStopLivingWithPersonWhileGivingCareDate, false);
        setRequired(window.ddeStopTakingCarePersonDate, false);
        setRequired(window.cbReceivedProvidingCare, false);
        clearCombo(window.cbLiveWithPersonBeingCaredFor);
        clearDate(window.ddeStopLivingwithPersonDate);
        clearDate(window.ddeStopTakingCareDate);
        clearDate(window.ddeStopLivingWithPersonWhileGivingCareDate);
        clearDate(window.ddeStopTakingCarePersonDate);
        clearCombo(window.cbReceivedProvidingCare);
    } else {
        fncLiveWithPersonBeingCaredForChange();
    }
    updateCaretakerVerifiedByRequired();
}

function fncWhenCareTakerRelationshipChange(s) {
    var cb = s || window.cbWhenCareTakerRelationship;
    var showStopDate = textContains(cb, "no longer");
    showRow("trStopProvidingCareDate2", showStopDate);
    setRequired(window.ddeStopProvidingCareDateTime, showStopDate);
    if (!showStopDate) {
        clearDate(window.ddeStopProvidingCareDateTime);
    }
    updateCaretakerVerifiedByRequired();
}


function fncLiveWithPersonBeingCaredForChange(s) {
    var cb = s || window.cbLiveWithPersonBeingCaredFor;
    var noLongerLive = textContains(cb, "no longer live") || textContains(cb, "does not live");
    var endedCaregiving = textContains(cb, "ended");

    var showBothStopDates = noLongerLive && endedCaregiving;
    var showStopLivingOnly = noLongerLive && !endedCaregiving;
    var showStopCaregivingOnly = endedCaregiving && !noLongerLive;

    showRow("trStopLivingwithPersonDate", showStopLivingOnly);
    setRequired(window.ddeStopLivingwithPersonDate, showStopLivingOnly);
    showRow("trStopProvidingCareDateMain", showStopCaregivingOnly);
    setRequired(window.ddeStopTakingCareDate, showStopCaregivingOnly);
    showRow("trStopLivingWithPersonWhileGivingCareDate", showBothStopDates);
    setRequired(window.ddeStopLivingWithPersonWhileGivingCareDate, showBothStopDates);
    showRow("trStopTakingCarePersonDate", showBothStopDates);
    setRequired(window.ddeStopTakingCarePersonDate, showBothStopDates);
    showRow("trReceivedProvidingCare", noLongerLive);
    setRequired(window.cbReceivedProvidingCare, showBothStopDates);

    if (!showStopLivingOnly) clearDate(window.ddeStopLivingwithPersonDate);
    if (!showStopCaregivingOnly) clearDate(window.ddeStopTakingCareDate);
    if (!showBothStopDates) {
        clearDate(window.ddeStopLivingWithPersonWhileGivingCareDate);
        clearDate(window.ddeStopTakingCarePersonDate);
    }
    if (!noLongerLive) {
        clearCombo(window.cbReceivedProvidingCare);
        var help = document.getElementById("divReceivedProvidingCareHelp");
        if (help) { help.style.display = "none"; help.innerHTML = ""; }
    } else {
        fncReceivedProvidingCareChange();
    }
    updateCaretakerVerifiedByRequired();
}


function updateCaretakerVerifiedByRequired() {
    var relationshipAnswered = window.cbCareTakerRelationship && window.cbCareTakerRelationship.GetValue() != null
        && !textContains(window.cbCareTakerRelationship, "other") && !textContains(window.cbCareTakerRelationship, "not related");
    var required = isYes(window.cbRegularlyTakeCareOfDependent)
        || textContains(window.cbWhenLegalGuardianProvideCare, "currently")
        || relationshipAnswered
        || textContains(window.cbWhenCareTakerRelationship, "currently")
        || (textContains(window.cbLiveWithPersonBeingCaredFor, "currently lives") && textContains(window.cbLiveWithPersonBeingCaredFor, "currently takes care"));
    setRequired(window.cbReceivedProvidingCareVerifiedBy, required);
}

function fncReceivedProvidingCareChange(s) {
    var cb = s || window.cbReceivedProvidingCare;
    var help = document.getElementById("divReceivedProvidingCareHelp");
    if (!help) return;

    var message = "";
    if (textContains(cb, "no payment")) {
        message = "Enter as unpaid work on the Volunteering/Work Program/Unpaid Work screen.";
    } else if (textContains(cb, "other than money")) {
        message = "Enter as in-kind work on the Volunteering/Work Program/Unpaid Work screen.";
    } else if (textContains(cb, "paid")) {
        message = "Enter as earned income on the Employment Details screen.";
    }

    if (message) {
        help.innerHTML = message;
        help.style.display = "";
    } else {
        help.innerHTML = "";
        help.style.display = "none";
    }
}

function fncWho(s) { }
function ParticipatingInWorkProgramIndicator(s) {
    fncVolunteeringWorkProgramChange();
}
function ParticipatingInUnpaidWorkIndicator(s) {
    fncVolunteeringWorkProgramChange();
}

function fncCorrectionalInLast12MonthsChange(s) {
    var showReleasedDate = isYes(s);
    showRow("trReleasedDate", showReleasedDate);
    setRequired(window.ddeCorrectionalReleasedDates, showReleasedDate);
    setRequired(window.cbCorrectionalInLast12MonthsVerifiedBy, showReleasedDate);
    if (!showReleasedDate) {
        clearDate(window.ddeCorrectionalReleasedDates);
    }
}


function fncSeriousMedicalConditionChange(s) {
    var showWhen = isYes(s);
    showRow("trSeriousMedicalconditionStatus", showWhen);
    setRequired(window.cbSeriousMedicalConditionVerifiedByCode, showWhen);
    if (!showWhen) {
        clearCombo(window.cbSeriousMedicalconditionStatusCode);
        showRow("trEndSeriousConditionDate", false);
        setRequired(window.ddeEndSeriousConditionDate, false);
        clearDate(window.ddeEndSeriousConditionDate);
    } else {
        SeriousMedicalconditionStatusCode(window.cbSeriousMedicalconditionStatusCode);
    }
}

function SeriousMedicalconditionStatusCode(s) {
    var showEndDate = textContains(s, "no longer");
    showRow("trEndSeriousConditionDate", showEndDate);
    setRequired(window.ddeEndSeriousConditionDate, showEndDate);
    if (!showEndDate) {
        clearDate(window.ddeEndSeriousConditionDate);
    }
}

function fncSubstanceUseDisorderChange(s) {
    var showWhen = isYes(s);
    showRow("trSubstanceUseDisorderStatus", showWhen);
    setRequired(window.cbSubstanceUseDisorderVerifiedBy, showWhen);
    if (!showWhen) {
        clearCombo(window.cbSubstanceUseDisorderStatusCode);
        showRow("trEndSubstanceDisorderDate", false);
        setRequired(window.ddeEndSubstanceDisorderDate, false);
        clearDate(window.ddeEndSubstanceDisorderDate);
    } else {
        SubstanceUseDisorderStatusCode(window.cbSubstanceUseDisorderStatusCode);
    }
}

function SubstanceUseDisorderStatusCode(s) {
    var showEndDate = textContains(s, "no longer");
    showRow("trEndSubstanceDisorderDate", showEndDate);
    setRequired(window.ddeEndSubstanceDisorderDate, showEndDate);
    if (!showEndDate) {
        clearDate(window.ddeEndSubstanceDisorderDate);
    }
}

function fncDisabledBySSAChange(s) {
    var showWhen = isYes(s);
    showRow("trWhenDetermined", showWhen);
    setRequired(window.cbDisabledBySSAVerifiedByCode, showWhen);
    if (!showWhen) {
        clearCombo(window.cbWhenDetermined);
        showRow("trEndSSADeterminationDate", false);
        setRequired(window.ddeEndSSADeterminationDate, false);
        clearDate(window.ddeEndSSADeterminationDate);
    } else {
        WhenDetermined(window.cbWhenDetermined);
    }
}

function WhenDetermined(s) {
    var showEndDate = textContains(s, "no longer");
    showRow("trEndSSADeterminationDate", showEndDate);
    setRequired(window.ddeEndSSADeterminationDate, showEndDate);
    if (!showEndDate) {
        clearDate(window.ddeEndSSADeterminationDate);
    }
}

function fncDisablingMentalDisorderChange(s) {
    var showWhen = isYes(s);
    showRow("trWhenDisablingMentalDisorder", showWhen);
    setRequired(window.cbDisablingMentalDisorderVerifiedByCode, showWhen);
    if (!showWhen) {
        clearCombo(window.cbWhenDisablingMentalDisorder);
        showRow("trEndDisablingMentalDisorderDate", false);
        setRequired(window.ddeEndDisablingMentalDisorderDate, false);
        clearDate(window.ddeEndDisablingMentalDisorderDate);
    } else {
        DisablingMentalDisorderStatus(window.cbWhenDisablingMentalDisorder);
    }
}

function DisablingMentalDisorderStatus(s) {
    var showEndDate = textContains(s, "no longer");
    showRow("trEndDisablingMentalDisorderDate", showEndDate);
    setRequired(window.ddeEndDisablingMentalDisorderDate, showEndDate);
    if (!showEndDate) {
        clearDate(window.ddeEndDisablingMentalDisorderDate);
    }
}

function fncPhysicalDisabilityChange(s) {
    var showWhen = isYes(s);
    showRow("trWhenPhysicalDisability", showWhen);
    setRequired(window.cbPhysicalDisabilityVerifiedByCode, showWhen);
    if (!showWhen) {
        clearCombo(window.cbWhenPhysicalDisability);
        showRow("trEndPhysicalDisabilityDate", false);
        setRequired(window.ddeEndPhysicalDisabilityDate, false);
        clearDate(window.ddeEndPhysicalDisabilityDate);
    } else {
        WhenPhysicalDisabilityIndicator(window.cbWhenPhysicalDisability);
    }
}

function WhenPhysicalDisabilityIndicator(s) {
    var showEndDate = textContains(s, "no longer");
    showRow("trEndPhysicalDisabilityDate", showEndDate);
    setRequired(window.ddeEndPhysicalDisabilityDate, showEndDate);
    if (!showEndDate) {
        clearDate(window.ddeEndPhysicalDisabilityDate);
    }
}

function fncHospitalizedSeriousConditionChange(s) {
    var required = isYes(s);
    setRequired(window.cbHospitalizedSeriousConditionVerifiedByCode, required);
    setRequired(window.ddeHospitalizedBeginDate, required);
    setRequired(window.ddeHospitalizedEndDate, required);
}

function fncTravelOutOfAreaMedicalChange(s) {
    var required = isYes(s);
    setRequired(window.cbTravelOutOfAreaMedicalVerifiedBy, required);
    setRequired(window.ddeTravelOutOfAreaMedicalBeginDate, required);
    setRequired(window.ddeTravelOutOfAreaMedicalEndDate, required);
}

function validateEndDateNotBeforeBegin(s, e, beginDateClientInstanceName) {
    var beginControl = window[beginDateClientInstanceName];
    if (!beginControl) return;

    var beginDate = beginControl.GetDate();
    var endDate = s.GetDate();
    if (beginDate != null && endDate != null && endDate < beginDate) {
        e.isValid = false;
        e.errorText = "End date should be the same as or after the begin date.";
    }
}

function validateWithinPast12Months(s, e) {
    var selectedDate = s.GetDate();
    if (selectedDate == null) return;

    var today = new Date();
    var twelveMonthsAgo = new Date(today.getFullYear(), today.getMonth() - 12, today.getDate());
    if (selectedDate < twelveMonthsAgo || selectedDate > today) {
        e.isValid = false;
        e.errorText = "Please select a date that is within the past 12 months";
    }

}

function validateNotFutureDate(s, e) {
    var selectedDate = s.GetDate();
    if (selectedDate == null) return;

    var today = new Date();
    today.setHours(23, 50, 50, 999);
    if (selectedDate > today) {
        e.isValid = false;
        e.errorText = "Date cannot be in the future.";
    }
}
function validateBeginMonthRules(s, e) {
    var d = s.GetDate();
    if (d == null) return;

    if (d.getFullYear() <= 1989) {
        e.isValid = false;
        e.errorText = "Any year on or before 1989 is invalid.";
        return;
    }
    var today = new Date();
    var maxMonth = new Date(today.getFullYear(), today.getMonth() + 2, 1);
    var enteredMonth = new Date(d.getFullYear(), d.getMonth(), 1);
    if (enteredMonth > maxMonth) {
        e.isValid = false;
        e.errorText = "Effective begin date cannot be greater than 2 months from the current date.";
    }
}
function checkLegacyBeginYear(s) {
    var d = s.GetDate();
    if (d == null) return;
    var y = d.getFullYear();
    if (y >= 1990 && y <= 1997) {
        pcbegindateconfirm.Show();
    }
}
function fncVolunteeringWorkProgramChange() {
    setRequired(window.cbParticipatingInWorkProgram, true);
    setRequired(window.cbParticipatingInUnpaidWork, true);
}
function fncValidateCEPage() {
    ASPxClientEdit.ValidateEditorsInContainer(null, null, false);
    if (ASPxClientEdit.AreEditorsValid(null, null, false)) return true;

    if (window.pcmandatoryfields) {
        pcmandatoryfields.Show();
    }
    return false;
}
var __ceOriginalDoPostBack = null;
var __ceSkipValidationTargets = ["btnBackToSUmmary", "btnClear", "btnCaseComment", "btnPrevious"];
function fncHookCEValidation() {
    if (__ceOriginalDoPostBack || typeof window.__doPostBack !== "funciton") return;
    __ceOriginalDoPostBack = window.__doPostBack;
    window.__doPostBack = function (traget, arg) {
        var t = (traget || "").toString();
        for (var i = 0; i < __ceSkipValidationTargets.length; i++) {
            if (t.indexOf(__ceSkipValidationTargets[i]) !== -i) {
                return __ceOriginalDoPostBack(target, arg);
            }
        }
        if (!fncValidateCEPage()) retur;
        return __ceOriginalDoPostBack(target, arg);
    }
}
if (window.addEventListener) {
    window.addEventListener("load", fncHookCEValidation, false);
} else {
    window.attachEvent("onload", fncHookCEValidation);
}