using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
// ReSharper disable InconsistentNaming

namespace Neuz.Notify.WXWork.Messages
{
    /// <summary>
    /// 图片消息
    /// <para>
    /// 详细信息参见 <see href="https://developer.work.weixin.qq.com/document/path/90236#%E5%9B%BE%E7%89%87%E6%B6%88%E6%81%AF"/>
    /// </para>
    /// </summary>
    public class ImageMessage : IMessage
    {
        /// <summary>
        /// 非必填<br/>
        /// 指定接收消息的成员，成员ID列表，多个接收者用‘|’分隔，最多支持1000个。<br/>
        /// 特殊情况：指定为"@all"，则向该企业应用的全部成员发送
        /// <para>
        /// 注意！
        /// <see cref="ToUser"/>、<see cref="ToParty"/>、<see cref="ToTag"/>不能同时为空
        /// </para>
        /// </summary>
        [JsonProperty(PropertyName = "touser", NullValueHandling = NullValueHandling.Ignore)]
        public string? ToUser { get; set; }

        /// <summary>
        /// 非必填<br/>
        /// 指定接收消息的部门，部门ID列表，多个接收者用‘|’分隔，最多支持100个。<br/>
        /// 当 <see cref="ToUser"/> 为"@all"时忽略本参数
        /// <para>
        /// 注意！
        /// <see cref="ToUser"/>、<see cref="ToParty"/>、<see cref="ToTag"/>不能同时为空
        /// </para>
        /// </summary>
        [JsonProperty(PropertyName = "toparty", NullValueHandling = NullValueHandling.Ignore)]
        public string? ToParty { get; set; }

        /// <summary>
        /// 非必填<br/>
        /// 指定接收消息的标签，标签ID列表，多个接收者用‘|’分隔，最多支持100个。<br/>
        /// 当 <see cref="ToUser"/> 为"@all"时忽略本参数
        /// <para>
        /// 注意！
        /// <see cref="ToUser"/>、<see cref="ToParty"/>、<see cref="ToTag"/>不能同时为空
        /// </para>
        /// </summary>

        [JsonProperty(PropertyName = "totag", NullValueHandling = NullValueHandling.Ignore)]
        public string? ToTag { get; set; }

        /// <summary>
        /// 必填<br/>
        /// 消息类型，此时固定为：image
        /// </summary>
        [JsonProperty(PropertyName = "msgtype")]
        public string MsgType = "image";

        /// <summary>
        /// 必填<br/>
        /// 企业应用的id，整型。
        /// </summary>
        [JsonProperty(PropertyName = "agentid")]
        public int? AgentID { get; set; }

        /// <summary>
        /// 必填<br/>
        /// 图片媒体文件id
        /// </summary>
        [JsonProperty(PropertyName = "image")]
        public ImageCls Image { get; set; } = new ImageCls();

        public class ImageCls
        {
            /// <summary>
            /// 图片媒体文件id，可以调用上传临时素材接口获取
            /// </summary>
            [JsonProperty(PropertyName = "media_id")]
            public string MediaId { get; set; } = string.Empty;
        }

        /// <summary>
        /// 非必填<br/>
        /// 表示是否是保密消息，0表示可对外分享，1表示不能分享且内容显示水印，默认为0
        /// </summary>
        [JsonProperty(PropertyName = "safe",NullValueHandling = NullValueHandling.Ignore)]
        public int? Safe { get; set; }

        /// <summary>
        /// 非必填<br/>
        /// 表示是否开启重复消息检查，0表示否，1表示是，默认0
        /// </summary>
        [JsonProperty(PropertyName = "enable_duplicate_check", NullValueHandling = NullValueHandling.Ignore)]
        public int? EnableDuplicateCheck { get; set; }


        /// <summary>
        /// 非必填<br/>
        /// 表示是否重复消息检查的时间间隔，默认1800s，最大不超过4小时
        /// </summary>
        [JsonProperty(PropertyName = "duplicate_check_interval", NullValueHandling = NullValueHandling.Ignore)]
        public int? DuplicateCheckInterval { get; set; }
    }
}