<%@ Page Language="C#" MasterPageFile="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" AutoEventWireup="True" CodeBehind="TechnicalQuestions.aspx.cs" Inherits="Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical.TechnicalQuestions" Title="Technical Questions" %>

<%@ MasterType VirtualPath="~/Intake/ApplicationEntry/ApplicationEntryLayout.master" %>
<asp:Content ID="ctPageBody" ContentPlaceHolderID="PageBodyContent" runat="server">
    <dhss:DataServiceLinqDataSource runat="server" ID="dsTechnical_HouseholdGeneralInfo"
        EnableUpdate="True" TableName="Technical_HouseholdGeneralInfo"
        EntityTypeName="Dhss.Assist.WorkerWeb.Web.Intake.ApplicationEntry.Technical.Technical_HouseholdGeneralInfo" 
        ContextTypeName="Dhss.Assist.WorkerWeb.Web.Infrastructure.Services.TechnicalContextImpl" OnSelecting="DsTechnical_HouseholdGeneralInfo_Selecting">
    </dhss:DataServiceLinqDataSource>
      <asp:HiddenField runat="server" ID="hdIsPageChange" ClientIDMode="Static" />
    <asp:FormView runat="server" ID="fvdsTechnical_HouseholdGeneralInfo" DefaultMode="Edit" DataSourceID="dsTechnical_HouseholdGeneralInfo" OnDataBound="FvdsTechnical_HouseholdGeneralInfo_DataBound"
        DataKeyNames="ApplicationID">
        <EditItemTemplate>
            <table class="ContentTable">
                <tr>
                    <td>
                        <dx:ASPxLabel ID="lblTechnicalQuestions" runat="server" Text="Technical Questions" SkinID="Header" EnableViewState="false"></dx:ASPxLabel>
                        <hr />
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:HiddenField runat="server" ID="hfApplicationID" Value='<%# Bind("ApplicationID") %>' />
                        <table class="SectionTable">
                            <tr>
                                <td class="lengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblIsAnyoneInYourHouseholdPregnant" runat="server" Text="Is anyone in your household pregnant or was pregnant in the past 12 months?" AssociatedControlID="cbIsAnyoneInYourHouseholdPregnant" width="400px"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="lengthyLabelControlTD">
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" ID="cbIsAnyoneInYourHouseholdPregnant" TabIndex="1" runat="server" Value='<%# Bind("IsAnyonePregnantIndicator") %>' ValueType="System.String"></dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="lengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblIsAnyoneInYourHouseholdLessThan1" runat="server" Text="Is anyone in your household less than 13 months old?" AssociatedControlID="cbIsAnyoneInYourHouseholdLessThan1" width="400px"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="lengthyLabelControlTD">
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" TabIndex="2" ID="cbIsAnyoneInYourHouseholdLessThan1" runat="server" ValueType="System.String" Value='<%# Bind("Haslessthan13monthschildIndicator") %>'></dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="lengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <br />
                                                <dx:ASPxLabel ID="lblDoesAnyoneInTheHouseholdReceiveS" width="400px" runat="server" Text="Does anyone in the household receive Supplemental Security Income, Social Security Disability, Government Retirement as permanently disabled, Veteran's Disability rated as total, totally disabled veteran's surviving spouse or child with a permanent disability, or Railroad Retirement Disability payments? "></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="lengthyLabelControlTD"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="lengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblOr" runat="server" Text="Or" AssociatedControlID="ASPxComboBox7" width="400px"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="lengthyLabelControlTD"></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="lengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td>
                                                <dx:ASPxLabel ID="lblIsAnyoneInYourHouseholdDisabledi" runat="server" width="400px" Text="Is anyone in your household disabled/incapacitated? " AssociatedControlID="cbDoesAnyoneInTheHouseholdReceiveS"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="lengthyLabelControlTD">
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" TabIndex="3" ID="cbDoesAnyoneInTheHouseholdReceiveS" runat="server" ValueType="System.String" Value='<%# Bind("ReceiveDisablityPaymentIndicator") %>'></dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="lengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td class="lengthyLabelCustomizedWidth4">
                                                <dx:ASPxLabel ID="lblIsAnyoneInYourHouseholdNoLongerA" runat="server" width="400px" Text="Is anyone in your household no longer a Supplemental Security Income recipient?" AssociatedControlID="cbIsAnyoneInYourHouseholdNoLongerA"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="lengthyLabelControlTD">
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" TabIndex="4" ID="cbIsAnyoneInYourHouseholdNoLongerA" runat="server" ValueType="System.String" Value='<%# Bind("HadSSIRecipientIndicator") %>'></dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="lengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td class="lengthyLabelCustomizedWidth4">
                                                <dx:ASPxLabel ID="lblIsAnyoneInYourHouseholdApplyingF" runat="server" width="400px" Text="Is anyone in your household applying for or in a Home and Community Based Services (HCBS) Waiver?" AssociatedControlID="cbIsAnyoneInYourHouseholdApplyingF"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="lengthyLabelControlTD">
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" TabIndex="5" ID="cbIsAnyoneInYourHouseholdApplyingF" runat="server" ValueType="System.String" Value='<%# Bind("HasHCBSWaiverIndicator") %>'></dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="lengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td class="lengthyLabelCustomizedWidth6">
                                                <dx:ASPxLabel ID="lblDoesAnyoneInYourHouseholdWhoIsAp" runat="server" width="400px" Text="Does anyone in your household who is applying for Long Term Care Services have a spouse in the community?" AssociatedControlID="cbDoesAnyoneInYourHouseholdWhoIsAp"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="lengthyLabelControlTD">
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" TabIndex="6" ID="cbDoesAnyoneInYourHouseholdWhoIsAp" runat="server" ValueType="System.String" Value='<%# Bind("HasLTCwithSpouseinCommunity") %>'></dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="lengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td class="lengthyLabelCustomizedWidth6">
                                                <dx:ASPxLabel ID="lblIsAnyoneInYourHouseholdApplyingF1" runat="server" Text="Is anyone in your household applying for the Chronic Renal Disease Program?" AssociatedControlID="cbIsAnyoneInYourHouseholdApplyingF1" Width="400px" ></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="lengthyLabelControlTD">
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" TabIndex="7" ID="cbIsAnyoneInYourHouseholdApplyingF1" runat="server" ValueType="System.String" Value='<%# Bind("HasChronicRenalDiseaseProgramParticipantIndicator") %>'></dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <table class="SectionTable">
                            <tr>
                                <td class="lengthyLabelTD">
                                    <table>
                                        <tr>
                                            <td class="lengthyLabelCustomizedWidth5">
                                                <dx:ASPxLabel ID="lblHasAnyoneInYourHouseholdBeenRefe" width="400px" runat="server" Text="Has anyone in your household been referred through DPH for the Breast and Cervical Cancer Program?" AssociatedControlID="cbHasAnyoneInYourHouseholdBeenRefe"></dx:ASPxLabel>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td class="lengthyLabelControlTD">
                                    <dx:ASPxComboBox IncrementalFilteringMode="StartsWith" TabIndex="8" ID="cbHasAnyoneInYourHouseholdBeenRefe" runat="server" ValueType="System.String" Value='<%# Bind("IsReferredByDPHIndicator") %>'></dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </EditItemTemplate>
    </asp:FormView>
</asp:Content>