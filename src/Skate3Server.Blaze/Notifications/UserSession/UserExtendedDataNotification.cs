﻿using System.Collections.Generic;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Notifications.UserSession
{
    [BlazeNotification(BlazeComponent.UserSession, (ushort)UserSessionNotification.UserExtendedData)]
    public class UserExtendedDataNotification : IBlazeNotification
    {
        [TdfField("DATA")]
        public ExtendedData Data { get; set; }

        [TdfField("USID")]
        public uint UserId { get; set; }
    }

    public class ExtendedData
    {
        [TdfField("ADDR")]
        public KeyValuePair<NetworkAddressType, string> Address { get; set; } //TODO this can also be an address pair

        [TdfField("BPS")]
        public string Bps { get; set; } //TODO bits per second?

        [TdfField("CTY")]
        public string Cty { get; set; } //TODO city?

        [TdfField("DMAP")]
        public Dictionary<uint, int> Dmap { get; set; } //TODO destination map?

        [TdfField("HWFG")]
        public uint HardwareFlags { get; set; }

        [TdfField("QDAT")]
        public QosNetworkData NetworkData { get; set; }

        [TdfField("UATT")]
        public ulong Uatt { get; set; } //TODO
    }
}
