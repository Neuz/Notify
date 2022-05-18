using Flurl.Http;
using Neuz.Notify.WXWork.Messages;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace Neuz.Notify.WXWork
{
    /// <summary>
    /// 企业微信
    /// </summary>
    public partial class WXWorkSender
    {
        #region 子类

        public class WXWorkResult
        {
            public bool Success { get; set; }
            public JObject? Raw { get; set; }
        }

        public class Auth
        {
            /// <summary>
            /// 企业ID
            /// <para>
            /// 获取方式参考 <see href="https://developer.work.weixin.qq.com/document/path/90665#corpid"/>
            /// </para>
            /// </summary>
            public string? Corpid { get; set; }


            /// <summary>
            /// 应用的凭证密钥
            /// <para>
            /// 获取方式参考 <see href="https://developer.work.weixin.qq.com/document/path/90665#secret"/>
            /// </para>
            /// </summary>
            public string? Corpsecret { get; set; }

            /// <summary>
            /// 应用ID
            /// <para>
            /// 获取方式参考 <see href="https://developer.work.weixin.qq.com/document/path/90665#agentid"/>
            /// </para>
            /// </summary>
            public int? AgentID { get; set; }
        }

        #endregion

        private Auth?     _auth;
        private IMessage? _message;


        /// <summary>
        /// 设置授权
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public WXWorkSender SetAuth(Action<Auth> action)
        {
            _auth ??= new Auth();
            action.Invoke(_auth);
            return this;
        }

        /// <summary>
        /// 设置消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public WXWorkSender SetMessage(IMessage message)
        {
            _message = message;
            return this;
        }

        /// <summary>
        /// 获取 AccessToken
        /// <para>
        /// 开发者需要缓存access_token，用于后续接口的调用（注意：不能频繁调用gettoken接口，否则会受到频率拦截）。当access_token失效或过期时，需要重新获取。<br/>
        /// access_token的有效期通过返回的expires_in来传达，正常情况下为7200秒（2小时），有效期内重复获取返回相同结果，过期后获取会返回新的access_token。<br/>
        /// 由于企业微信每个应用的access_token是彼此独立的，所以进行缓存时需要区分应用来进行存储。<br/>
        /// access_token至少保留512字节的存储空间。<br/>
        /// </para>
        /// <para>
        /// 详情参照 <see href="https://developer.work.weixin.qq.com/document/path/91039"/>
        /// </para>
        /// </summary>
        /// <returns></returns>
        private string GetAccessToken()
        {
            if (_auth == null) throw new ArgumentNullException(nameof(_auth), "授权信息为空");
            if (_auth.Corpid == null) throw new ArgumentNullException(nameof(_auth.Corpid), "企业ID为空");
            if (_auth.Corpsecret == null) throw new ArgumentNullException(nameof(_auth.Corpsecret), "应用的凭证密钥为空");
            if (_auth.AgentID == null) throw new ArgumentNullException(nameof(_auth.AgentID), "应用ID为空");

            var key         = $"{_auth.Corpid}_{_auth.Corpsecret}_{_auth.AgentID}";
            var accessToken = NotifyCache.Get(key);

            if (accessToken != null) return accessToken;

            // token 不存在或过期，重新获取
            var url    = $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={_auth.Corpid}&corpsecret={_auth.Corpsecret}";
            var result = url.GetJsonAsync<JObject>().Result;

            if (result["errcode"]?.Value<int>() == 0)
            {
                accessToken = result["access_token"].Value<string>();
                // 默认7200秒，2小时
                NotifyCache.Set(key, accessToken, DateTimeOffset.Now.AddHours(2));
            }
            else
            {
                throw new ApplicationException("获取AccessToken失败");
            }

            return accessToken;
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <returns></returns>
        public async Task<WXWorkResult> Send()
        {
            var accessToken = GetAccessToken();

            if (string.IsNullOrEmpty(accessToken)) throw new ApplicationException("AccessToken为空");

            var url      = $"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={accessToken}";
            var response = await url.PostJsonAsync(_message);
            var raw      = await response.GetJsonAsync<JObject>();

            return new WXWorkResult
            {
                Success = raw?["errcode"]?.Value<int>() == 0,
                Raw     = raw
            };
        }
    }

    public partial class WXWorkSender
    {
        #region TextMessage

        /// <summary>
        /// 设置文本消息
        /// <para>
        /// 详情参照 <see href="https://developer.work.weixin.qq.com/document/path/90236#%E6%96%87%E6%9C%AC%E6%B6%88%E6%81%AF"/>
        /// </para>
        /// </summary>
        /// <param name="content">
        /// 消息内容，最长不超过2048个字节，超过将截断（支持id转译）<br/>
        /// 其中text参数的content字段可以支持换行、以及A标签，即可打开自定义的网页 (注意：换行符请用转义过的\n)
        /// </param>
        /// <param name="toUser">
        /// 指定接收消息的成员，成员ID列表（多个接收者用‘|’分隔，最多支持1000个）。<br/>
        /// 特殊情况：指定为"@all"，则向该企业应用的全部成员发送
        /// </param>
        /// <param name="toParty">
        /// 指定接收消息的部门，部门ID列表，多个接收者用‘|’分隔，最多支持100个。<br/>
        /// 当 <see cref="toUser"/> 为"@all"时忽略本参数
        /// </param>
        /// <param name="toTag">
        /// 指定接收消息的标签，标签ID列表，多个接收者用‘|’分隔，最多支持100个。<br/>
        /// 当 <see cref="toUser"/> 为"@all"时忽略本参数
        /// </param>
        /// <returns></returns>
        public WXWorkSender SetTextMessage(string content, string toUser = "@all", string? toParty = null, string? toTag = null)
        {
            _message = new TextMessage
            {
                ToUser  = toUser,
                ToParty = toParty,
                ToTag   = toTag,
                AgentID = _auth?.AgentID,
                Text = new TextMessage.TextCls
                {
                    Content = content
                },
                Safe                   = null,
                EnableIdTrans          = null,
                EnableDuplicateCheck   = null,
                DuplicateCheckInterval = null
            };

            return this;
        }

        /// <summary>
        /// 设置文本消息
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public WXWorkSender SetTextMessage(Action<TextMessage> action)
        {
            var textMessage = new TextMessage {AgentID = _auth?.AgentID};
            action.Invoke(textMessage);
            _message = textMessage;
            return this;
        }

        #endregion

        #region TextCardMessage

        /// <summary>
        /// 设置文本卡片消息
        /// </summary>
        /// <para>
        /// 详情参照 <see href="https://developer.work.weixin.qq.com/document/path/90236#%E6%96%87%E6%9C%AC%E5%8D%A1%E7%89%87%E6%B6%88%E6%81%AF"/>
        /// </para>
        /// <param name="title">
        /// 必填<br/>
        /// 标题，不超过128个字节，超过会自动截断（支持id转译）
        /// </param>
        /// <param name="description">
        /// 必填<br/>
        /// 描述，不超过512个字节，超过会自动截断（支持id转译）
        /// </param>
        /// <param name="url">
        /// 必填<br/>
        /// 点击后跳转的链接。最长2048字节，请确保包含了协议头(http/https)
        /// </param>
        /// <param name="btnTxt">
        /// 非必填<br/>
        /// 按钮文字。 默认为“详情”， 不超过4个文字，超过自动截断。
        /// </param>
        /// <param name="toUser">
        /// 指定接收消息的成员，成员ID列表（多个接收者用‘|’分隔，最多支持1000个）。<br/>
        /// 特殊情况：指定为"@all"，则向该企业应用的全部成员发送
        /// </param>
        /// <param name="toParty">
        /// 指定接收消息的部门，部门ID列表，多个接收者用‘|’分隔，最多支持100个。<br/>
        /// 当 <see cref="toUser"/> 为"@all"时忽略本参数
        /// </param>
        /// <param name="toTag">
        /// 指定接收消息的标签，标签ID列表，多个接收者用‘|’分隔，最多支持100个。<br/>
        /// 当 <see cref="toUser"/> 为"@all"时忽略本参数
        /// </param>
        /// <returns></returns>
        public WXWorkSender SetTextCardMessage(string title, string description, string url, string? btnTxt = null, string toUser = "@all", string? toParty = null, string? toTag = null)
        {
            _message = new TextCardMessage
            {
                ToUser  = toUser,
                ToParty = toParty,
                ToTag   = toTag,
                AgentID = _auth?.AgentID,
                TextCard = new TextCardMessage.TextCardCls
                {
                    Title       = title,
                    Description = description,
                    Url         = url,
                    BtnTxt      = btnTxt
                },
                EnableIdTrans          = null,
                EnableDuplicateCheck   = null,
                DuplicateCheckInterval = null
            };

            return this;
        }

        /// <summary>
        /// 设置文本卡片消息
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public WXWorkSender SetTextCardMessage(Action<TextCardMessage> action)
        {
            var msg = new TextCardMessage {AgentID = _auth?.AgentID};
            action.Invoke(msg);
            _message = msg;
            return this;
        }

        #endregion

        #region Markdown

        /// <summary>
        /// 设置Markdown消息
        /// <para>
        /// 详情参照 <see href="https://developer.work.weixin.qq.com/document/path/90236#markdown%E6%B6%88%E6%81%AF"/>
        /// </para>
        /// </summary>
        /// <param name="content">
        /// markdown内容，最长不超过2048个字节，必须是utf8编码
        /// </param>
        /// <param name="toUser">
        /// 指定接收消息的成员，成员ID列表（多个接收者用‘|’分隔，最多支持1000个）。<br/>
        /// 特殊情况：指定为"@all"，则向该企业应用的全部成员发送
        /// </param>
        /// <param name="toParty">
        /// 指定接收消息的部门，部门ID列表，多个接收者用‘|’分隔，最多支持100个。<br/>
        /// 当 <see cref="toUser"/> 为"@all"时忽略本参数
        /// </param>
        /// <param name="toTag">
        /// 指定接收消息的标签，标签ID列表，多个接收者用‘|’分隔，最多支持100个。<br/>
        /// 当 <see cref="toUser"/> 为"@all"时忽略本参数
        /// </param>
        /// <returns></returns>
        public WXWorkSender SetMarkdownMessage(string content, string toUser = "@all", string? toParty = null, string? toTag = null)
        {
            _message = new MarkdownMessage
            {
                ToUser  = toUser,
                ToParty = toParty,
                ToTag   = toTag,
                AgentID = _auth?.AgentID,
                Markdown = new MarkdownMessage.MarkdownCls
                {
                    Content = content
                },
                EnableDuplicateCheck   = null,
                DuplicateCheckInterval = null
            };
            return this;
        }

        /// <summary>
        /// 设置Markdown消息
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public WXWorkSender SetMarkdownMessage(Action<MarkdownMessage> action)
        {
            var msg = new MarkdownMessage {AgentID = _auth?.AgentID};
            action.Invoke(msg);
            _message = msg;
            return this;
        }

        #endregion

        #region ImageMessage

        /// <summary>
        /// 设置图片消息
        /// <para>
        /// 详情参照 <see href="https://developer.work.weixin.qq.com/document/path/90236#%E5%9B%BE%E7%89%87%E6%B6%88%E6%81%AF"/>
        /// </para>
        /// </summary>
        /// <param name="imagePath">
        /// 图片路径
        /// </param>
        /// <param name="toUser">
        /// 指定接收消息的成员，成员ID列表（多个接收者用‘|’分隔，最多支持1000个）。<br/>
        /// 特殊情况：指定为"@all"，则向该企业应用的全部成员发送
        /// </param>
        /// <param name="toParty">
        /// 指定接收消息的部门，部门ID列表，多个接收者用‘|’分隔，最多支持100个。<br/>
        /// 当 <see cref="toUser"/> 为"@all"时忽略本参数
        /// </param>
        /// <param name="toTag">
        /// 指定接收消息的标签，标签ID列表，多个接收者用‘|’分隔，最多支持100个。<br/>
        /// 当 <see cref="toUser"/> 为"@all"时忽略本参数
        /// </param>
        /// <returns></returns>
        public WXWorkSender SetImageMessage(string imagePath, string toUser = "@all", string? toParty = null, string? toTag = null)
        {
            var mediaId = GetMediaId(imagePath);
            _message = new ImageMessage()
            {
                ToUser  = toUser,
                ToParty = toParty,
                ToTag   = toTag,
                AgentID = _auth?.AgentID,
                Image = new ImageMessage.ImageCls
                {
                    MediaId = mediaId
                },
                Safe                   = null,
                EnableDuplicateCheck   = null,
                DuplicateCheckInterval = null
            };

            return this;
        }

        /// <summary>
        /// 获取媒体文件ID
        /// <para>
        /// 使用上传临时素材接口<br/>
        /// 详情 <see href="https://developer.work.weixin.qq.com/document/path/90253"/>
        /// </para>
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private string GetMediaId(string filePath)
        {
            using var fs = File.OpenRead(filePath);

            var token = GetAccessToken();

            
            var url   = $"https://qyapi.weixin.qq.com/cgi-bin/media/upload?access_token={token}&type=file";

            // 构造form-data
            var boundary = $"----WebKitFormBoundary{DateTime.Now.Ticks:x}";
            var content  = new MultipartFormDataContent(boundary);
            content.Headers.Remove("Content-Type");
            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
            var sc = new StreamContent(fs, (int) fs.Length);
            sc.Headers.Add("Content-Type", "application/octet-stream");
            sc.Headers.Add("Content-Disposition", "form-data; name=\"filename\"; filename=\"img1.png\"");
            content.Add(sc);
            var result = url.PostAsync(content).Result.GetJsonAsync<JObject>().Result;
            if (result["errcode"]?.Value<int>() == 0) return result["media_id"].ToString();

            throw new ApplicationException("上传临时附件失败");
        }

        #endregion
    }
}