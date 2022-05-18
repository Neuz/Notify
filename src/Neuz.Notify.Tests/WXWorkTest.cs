using Flurl.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net.Http;


namespace Neuz.Notify.Tests;

[TestClass]
public class WXWorkTest
{
    [TestMethod]
    public void Test()
    {
        var token =
            "YBeTRhVr98Oxclp14vkh8plkD-SSrUp8GJWooa4QfobGXDsMDv4tmwtxmFvz-zJ_ypt5KHUbu6Nr7eyR-6YCezRMBncrCQ0vRKP_mIV-verao46YsSf0To_U5Kga4LiCaVHn0P64XBfLTAPjbic2TsyubiiX_UJlHB5y4snW633UMVE_qlqJ3G1fSznYbtY1FtyYeX_DUiRhsiyPu67mUg";

        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", "img1.png");
        var fs   = File.OpenRead(path);

        var uri = $"https://qyapi.weixin.qq.com/cgi-bin/media/upload?access_token={token}&type=file&debug=1";

        var boundary = $"----WebKitFormBoundary{DateTime.Now.Ticks:x}";
        var content  = new MultipartFormDataContent(boundary);
        content.Headers.Remove("Content-Type");
        content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
        var sc = new StreamContent(fs, (int) fs.Length);
        sc.Headers.Add("Content-Type", "application/octet-stream");
        sc.Headers.Add("Content-Disposition", "form-data; name=\"filename\"; filename=\"img1.png\"");
        content.Add(sc);
        var ccc = uri.PostAsync(content).Result.GetStringAsync().Result;
    }

    [TestMethod]
    public void TextMessage()
    {
        var result = NeuzNotify.WXWork
                               .SetAuth(auth =>
                                {
                                    auth.AgentID    = 1000002;
                                    auth.Corpid     = "wwbefbb2e3cdd824b6";
                                    auth.Corpsecret = "lpU9RzzOeTbFyLLkHW8PVkOVM4gExKshGx2LoSTQRp4";
                                })
                               .SetTextMessage("这里是测试文本\n <a href=\"https://baidu.com\">测试链接</a> \n \n 哈哈哈哈哈")
                               .Send()
                               .Result;
        Assert.IsTrue(result.Success);
    }


    [TestMethod]
    public void ImageMessage()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", "img1.png");
        var rr = NeuzNotify.WXWork.SetAuth(auth =>
                            {
                                auth.AgentID    = 1000002;
                                auth.Corpid     = "wwbefbb2e3cdd824b6";
                                auth.Corpsecret = "lpU9RzzOeTbFyLLkHW8PVkOVM4gExKshGx2LoSTQRp4";
                            })
                           .SetImageMessage(path)
                           .Send().Result;
    }

    [TestMethod]
    public void TextCardMessage()
    {
        var rr = NeuzNotify.WXWork.SetAuth(auth =>
                            {
                                auth.AgentID    = 1000002;
                                auth.Corpid     = "wwbefbb2e3cdd824b6";
                                auth.Corpsecret = "lpU9RzzOeTbFyLLkHW8PVkOVM4gExKshGx2LoSTQRp4";
                            })
                           .SetTextCardMessage("这里是标题", "明细明细明细", "http://baidu.com", "查看详情")
                           .Send()
                           .Result;
    }

    [TestMethod]
    public void MarkdownMessage()
    {
        var content = "您的会议室已经预定，稍后会同步到`邮箱` \r\n" +
                      "**事项详情** \r\n" +
                      ">事　项：<font color=\"info\">开会</font> \r\n" +
                      ">组织者：@miglioguan \r\n" +
                      "如需修改会议信息，请点击：[修改会议信息](https://work.weixin.qq.com) \r\n";
        var rr = NeuzNotify.WXWork.SetAuth(auth =>
                            {
                                auth.AgentID    = 1000002;
                                auth.Corpid     = "wwbefbb2e3cdd824b6";
                                auth.Corpsecret = "lpU9RzzOeTbFyLLkHW8PVkOVM4gExKshGx2LoSTQRp4";
                            }).SetMarkdownMessage(content)
                           .Send()
                           .Result;
    }
}