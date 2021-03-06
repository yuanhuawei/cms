﻿using System.Collections.Generic;
using MaiDar.Core.Integration;
using MaiDarServer.Plugin.Apis;

namespace MaiDarServer.CMS.Plugin.Apis
{
    public class SmsApi : ISmsApi
    {
        private SmsApi() { }

        public static SmsApi Instance { get; } = new SmsApi();

        public bool IsReady()
        {
            return SmsManager.IsReady();
        }

        public bool Send(string mobile, string tplId, Dictionary<string, string> parameters, out string errorMessage)
        {
            return SmsManager.Send(mobile, tplId, parameters, out errorMessage);
        }

        public bool SendCode(string mobile, int code, string tplId, out string errorMessage)
        {
            return SmsManager.SendCode(mobile, code, tplId, out errorMessage);
        }
    }
}
