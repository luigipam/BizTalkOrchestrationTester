using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTSCodeGen
{
    public class BTS
    {
        public static ContextProperty AckFailureCategory = new ContextProperty() { Name = "AckFailureCategory", Type = typeof(string) };
        public static ContextProperty AckFailureCode = new ContextProperty() { Name = "AckFailureCode", Type = typeof(string) };
        public static ContextProperty AckID = new ContextProperty() { Name = "AckID", Type = typeof(string) };
        public static ContextProperty AckInboundTransportLocation = new ContextProperty() { Name = "AckInboundTransportLocation", Type = typeof(string) };
        public static ContextProperty AckOutboundTransportLocation = new ContextProperty() { Name = "AckOutboundTransportLocation", Type = typeof(string) };
        public static ContextProperty AckOwnerID = new ContextProperty() { Name = "AckOwnerID", Type = typeof(string) };
        public static ContextProperty AckReceivePortID = new ContextProperty() { Name = "AckReceivePortID", Type = typeof(string) };
        public static ContextProperty AckReceivePortName = new ContextProperty() { Name = "AckReceivePortName", Type = typeof(string) };
        public static ContextProperty AckSendPortID = new ContextProperty() { Name = "AckSendPortID", Type = typeof(string) };
        public static ContextProperty AckSendPortName = new ContextProperty() { Name = "AckSendPortName", Type = typeof(string) };
        public static ContextProperty AckType = new ContextProperty() { Name = "AckType", Type = typeof(string) };
        public static ContextProperty ActionOnFailure = new ContextProperty() { Name = "ActionOnFailure", Type = typeof(string) };
        public static ContextProperty CorrelationToken = new ContextProperty() { Name = "CorrelationToken", Type = typeof(string) };
        public static ContextProperty EpmRRCorrelationToken = new ContextProperty() { Name = "EpmRRCorrelationToken", Type = typeof(string) };
        public static ContextProperty InboundTransportLocation = new ContextProperty() { Name = "InboundTransportLocation", Type = typeof(string) };
        public static ContextProperty InboundTransportType = new ContextProperty() { Name = "InboundTransportType", Type = typeof(string) };
        public static ContextProperty InterchangeSequenceNumber = new ContextProperty() { Name = "InterchangeSequenceNumber", Type = typeof(string) };
        public static ContextProperty MessageDestination = new ContextProperty() { Name = "MessageDestination", Type = typeof(string) };
        public static ContextProperty MessageType = new ContextProperty() { Name = "MessageType", Type = typeof(string) };
        public static ContextProperty OutboundTransportLocation = new ContextProperty() { Name = "OutboundTransportLocation", Type = typeof(string) };
        public static ContextProperty OutboundTransportType = new ContextProperty() { Name = "OutboundTransportType", Type = typeof(string) };
        public static ContextProperty ReceiveLocationName = new ContextProperty() { Name = "ReceiveLocationName", Type = typeof(string) };
        public static ContextProperty ReceivePortID = new ContextProperty() { Name = "ReceivePortID", Type = typeof(string) };
        public static ContextProperty ReceivePortName = new ContextProperty() { Name = "ReceivePortName", Type = typeof(string) };
        public static ContextProperty RouteDirectToTP = new ContextProperty() { Name = "RouteDirectToTP", Type = typeof(string) };
        public static ContextProperty SPGroupID = new ContextProperty() { Name = "SPGroupID", Type = typeof(string) };
        public static ContextProperty SPID = new ContextProperty() { Name = "SPID", Type = typeof(string) };
        public static ContextProperty SPTransportBackupID = new ContextProperty() { Name = "SPTransportBackupID", Type = typeof(string) };
        public static ContextProperty SPTransportID = new ContextProperty() { Name = "SPTransportID", Type = typeof(string) };
        public static ContextProperty SuspendAsNonResumable = new ContextProperty() { Name = "SuspendAsNonResumable", Type = typeof(string) };
        public static ContextProperty SuspendMessageOnRoutingFailure = new ContextProperty() { Name = "SuspendMessageOnRoutingFailure", Type = typeof(string) };

        public static ContextProperty AckDescription = new ContextProperty() { Name = "AckDescription", Type = typeof(string) };
        public static ContextProperty EncryptionCert = new ContextProperty() { Name = "EncryptionCert", Type = typeof(string) };
        public static ContextProperty InterchangeID = new ContextProperty() { Name = "InterchangeID", Type = typeof(string) };
        public static ContextProperty Loopback = new ContextProperty() { Name = "Loopback", Type = typeof(string) };
        public static ContextProperty SignatureCertificate = new ContextProperty() { Name = "SignatureCertificate", Type = typeof(string) };
        public static ContextProperty SourcePartyID = new ContextProperty() { Name = "SourcePartyID", Type = typeof(string) };
        public static ContextProperty SSOTicket = new ContextProperty() { Name = "SSOTicket", Type = typeof(string) };
        public static ContextProperty WindowsUser = new ContextProperty() { Name = "WindowsUser", Type = typeof(string) };        
    }
}
