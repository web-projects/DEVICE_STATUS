using DeviceStatus.Helpers.api;
using System.Collections.Generic;

namespace DeviceStatus.Helpers
{
    public class TCLinkResponse
    {
        public string MessageID { get; set; }
        public List<Response> Responses { get; set; }
        public List<ErrorValue> Errors { get; set; }
    }

    public class Response
    {
        public string MessageID { get; set; }
        public PaymentResponse PaymentResponse { get; set; }
        public PaymentUpdateResponse PaymentUpdateResponse { get; set; }
        public DALResponse DALResponse { get; set; }
        public SessionResponse SessionResponse { get; set; }
        public EventResponse EventResponse { get; set; }
        public string RequestID { get; set; }
        public List<ErrorValue> Errors { get; set; }
        public DALActionResponse DALActionResponse { get; set; }
    }

    public class PaymentUpdateResponse
    {
        List<ErrorValue> Errors { get; set; }
        public string Status { get; set; }
        public string Timestamp { get; set; }
        public string TCTimestamp { get; set; }
        public int? RequestedAmount { get; set; }
    }

    public class DALActionResponse
    {
        public List<ErrorValue> Errors { get; set; }
        public string Status { get; set; }
        public string Value { get; set; }
        public bool? CardPresented { get; set; }
    }

    public class SessionResponse
    {
        public string SessionID { get; set; }
        public List<ErrorValue> Errors { get; set; }
    }

    public class PaymentResponse
    {
        public List<ErrorValue> Errors { get; set; }
        public string Status { get; set; }
        public string EntryModeStatus { get; set; }
        public List<TCLinkRawResponse> TCLinkResponse { get; set; }
        public string TCTransactionID { get; set; }
        public string Timestamp { get; set; }
        public string TCTimestamp { get; set; }
        public int? CollectedAmount { get; set; }
        public CardResponse CardResponse { get; set; }
        public EMVResponse EMVResponse { get; set; }
        public string BillingID { get; set; }
        public BankAccountResponse AccountResponse { get; set; }
        public ReceiptResponse ReceiptResponse { get; set; }
    }

    public class TCLinkRawResponse
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class CardResponse
    {
        public string AuthorizationCode { get; set; }
        public string LeadingMaskedPAN { get; set; }
        public string TrailingMaskedPAN { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public bool? SignatureRequested { get; set; }
        public bool? DebitCard { get; set; }
        public string EntryMode { get; set; }
        public string CardholderName { get; set; }
        public string TenderType { get; set; }
        public EMVResponse EMVData { get; set; }
        public string AVSStatus { get; set; }
        public string CommercialCard { get; set; }
        public string CardIdentifier { get; set; }
        public string HeldCardDataID { get; set; }
    }

    public class BankAccountResponse
    {
        public string AccountType { get; set; }
        public string RoutingNumber { get; set; }
        public string TrailingAccountNumber { get; set; }
        public string CheckNumber { get; set; }
        public string HolderName { get; set; }
    }

    public class EMVResponse
    {
        public string ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationResponse { get; set; }
        public string AuthorizationMode { get; set; }
        public string IssuerApplicationData { get; set; }
        public string CardHolderVerification { get; set; }
        public string TerminalVerificationResults { get; set; }
        public string TransactionStatusInformation { get; set; }
    }

    public class DALResponse
    {
        public LinkDALIdentifier DALIdentifier { get; set; }
        public bool? OnlineStatus { get; set; }
        public bool? AvailableStatus { get; set; }
        public List<DeviceResponse> Devices { get; set; }
        public List<ErrorValue> Errors { get; set; }
    }

    public class DeviceResponse
    {
        public List<ErrorValue> Errors { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string SerialNumber { get; set; }
        public string Port { get; set; }
        public List<string> Features { get; set; }
        public List<string> Configurations { get; set; }
        //public LinkCardWorkflowControls CardWorkflowControls { get; set; }
    }

    public class EventResponse
    {
        public string EventType { get; set; }
        public string EventCode { get; set; }
        public string EventID { get; set; }
        public int? OrdinalID { get; set; }
        public List<string> EventData { get; set; }

        public string Description { get; set; }
    }

    public class ErrorValue
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class ReceiptResponse
    {
        public string CardholderPANFirst6 { get; set; }
        public string CardholderPANLast4 { get; set; }
        public string CardExpirationDate { get; set; }
        public string MerchantTerminalID { get; set; }
        public string TransactionAmount { get; set; }
        public string TransactionCurrencyCode { get; set; }
        public string ApplicationPreferredName { get; set; }
        public string PaymentNetwork { get; set; }
        public string ApplicationID { get; set; }
        public string CardEntryMethod { get; set; }
        public string CardholderVerificationMethod { get; set; }
        public string AuthorizationMode { get; set; }
        public string CardholderName { get; set; }
        public string ApplicationLabel { get; set; }
        public string PANSequenceNumber { get; set; }
        public string ApplicationInterchangeProfile { get; set; }
        public string TerminalVerificationResults { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string AmountAuthorized { get; set; }
        public string OtherAmount { get; set; }
        public string ApplicationUsageControl { get; set; }
        public ActionCode IssuerActionCode { get; set; }
        public string TerminalCountryCode { get; set; }
        public string ApplicationCryptogram { get; set; }
        public string CryptogramInformationData { get; set; }
        public string CardholderVerificationMethodResults { get; set; }
        public string ApplicationTransactionCounter { get; set; }
        public string UnpredictableNumber { get; set; }
        public ActionCode TerminalActionCode { get; set; }
    }

    public class ActionCode
    {
        public string Default { get; set; }
        public string Denial { get; set; }
        public string Online { get; set; }
    }
}
