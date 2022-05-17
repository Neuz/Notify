using System;
using Flurl.Http;
using Neuz.Notify.WXWork.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Neuz.Notify.WXWork
{
    public partial class WXWorkSender
    {
        
        private string?   _accessToken = null;
        private Auth      _auth       = new Auth();
        private IMessage? _message;

        public WXWorkSender SetAuth(Action<Auth> action)
        {
            action.Invoke(_auth);
            return this;
        }

        public WXWorkSender SetMessage(IMessage message)
        {
            _message = message;
            return this;
        }

        public WXWorkSender SetTextMessage(string content, string toUser = "@all", string? toParty = null, string? toTag = null)
        {
            _message = new TextMessage
            {
                ToUser  = toUser,
                ToParty = toParty,
                ToTag   = toTag,
                AgentID = _auth.AgentID,
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

        public WXWorkSender SetTextMessage(Action<TextMessage> action)
        {
            var textMessage = new TextMessage
            {
                AgentID = _auth.AgentID
            };
            action.Invoke(textMessage);
            _message = textMessage;
            return this;
        }

        private string GetAccessToken(Auth auth)
        {
            var url    = $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={auth.Corpid}&corpsecret={auth.Corpsecret}";
            var result = url.GetJsonAsync<JObject>().Result;
            if (result["errcode"]?.Value<int>() == 0)
            {
                return result["access_token"].Value<string>();
            }

            return null;
        }

        public bool Send()
        {
            _accessToken = "mL8fhWjQXBNujEaRS4OzHfqJZ9RzaEMaL12dOQBzlN_DpG9wDPh-yE_M6jrVDUI3EQyqaK1Yal4un8WTM5yAmKKKzhg3szDbgI5DzwgJ-Kjdx4Edm_veWuUz6qA8lEQejJG1Wo1UnAwpCXUn7w-kHVR8k25L50DMiQtLJeoH9ZyiNW8Crd12p1Hufwdst8EbZxJmQsPIW81B8byquMJBqw";
            if (string.IsNullOrEmpty(_accessToken))
            {
                _accessToken = GetAccessToken(_auth);
            }
            var url = $"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={_accessToken}";

            var text = JsonConvert.SerializeObject(_message);

            var aa = url.PostJsonAsync(_message)
                        .Result
                        .GetJsonAsync<JObject>()
                        .Result;
            var aaa = aa.ToString();

            if (aa["errcode"]?.Value<int>() == 0)
            {
                return true;
            }
            return false;

        }
    }

    public partial class WXWorkSender
    {
        public class Auth
        {
            /// <summary>
            /// 企业ID
            /// <para>
            /// 获取方式参考 <see href="https://developer.work.weixin.qq.com/document/path/90665#corpid"/>
            /// </para>
            /// </summary>
            public string Corpid { get; set; }


            /// <summary>
            /// 应用的凭证密钥
            /// <para>
            /// 获取方式参考 <see href="https://developer.work.weixin.qq.com/document/path/90665#secret"/>
            /// </para>
            /// </summary>
            public string Corpsecret { get; set; }

            /// <summary>
            /// 应用ID
            /// <para>
            /// 获取方式参考 <see href="https://developer.work.weixin.qq.com/document/path/90665#agentid"/>
            /// </para>
            /// </summary>
            public int AgentID { get; set; }
        }
    }
}