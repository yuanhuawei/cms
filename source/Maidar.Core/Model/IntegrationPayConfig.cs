﻿using Newtonsoft.Json;

namespace MaiDar.Core.Model
{
    [JsonObject(MemberSerialization.OptOut)]
    public class IntegrationPayConfig
    {
        public bool IsAlipayPc { get; set; }
        public bool AlipayPcIsMApi { get; set; }
        public string AlipayPcAppId { get; set; }
        public string AlipayPcPid { get; set; }
        public string AlipayPcMd5 { get; set; }
        public string AlipayPcPublicKey { get; set; }
        public string AlipayPcPrivateKey { get; set; }

        public bool IsAlipayMobi { get; set; }
        public bool AlipayMobiIsMApi { get; set; }
        public string AlipayMobiAppId { get; set; }
        public string AlipayMobiPid { get; set; }
        public string AlipayMobiMd5 { get; set; }
        public string AlipayMobiPublicKey { get; set; }
        public string AlipayMobiPrivateKey { get; set; }

        public bool IsWeixin { get; set; }
        public string WeixinAppId { get; set; }
        public string WeixinAppSecret { get; set; }
        public string WeixinMchId { get; set; }
        public string WeixinKey { get; set; }

        public bool IsUnionpayPc { get; set; }

        public bool IsUnionpayMobi { get; set; }
    }
}