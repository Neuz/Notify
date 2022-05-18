using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace Neuz.Notify.WXWork.Messages
{
    /// <summary>
    /// markdown消息
    /// <para>
    /// 详细信息参见 <see href="https://developer.work.weixin.qq.com/document/path/90236#markdown%E6%B6%88%E6%81%AF"/>
    /// </para>
    /// </summary>
    public class MarkdownMessage : IMessage
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
        /// 消息类型，此时固定为：markdown
        /// </summary>
        [JsonProperty(PropertyName = "msgtype")]
        public string MsgType = "markdown";

        /// <summary>
        /// 必填<br/>
        /// 企业应用的id，整型。
        /// </summary>
        [JsonProperty(PropertyName = "agentid")]
        public int? AgentID { get; set; }

        /// <summary>
        /// 必填<br/>
        /// 文本消息
        /// </summary>
        [JsonProperty(PropertyName = "markdown")]
        public MarkdownCls Markdown { get; set; } = new MarkdownCls();

        public class MarkdownCls
        {
            /// <summary>
            /// 必填<br/>
            /// markdown内容，最长不超过2048个字节，必须是utf8编码
            /// </summary>
            [JsonProperty(PropertyName = "content")]
            public string Content { get; set; }
        }

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