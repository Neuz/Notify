using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Neuz.Notify.Tests;

[TestClass]
public class WXWorkTest
{
    [TestMethod]
    public void TestMethod1()
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
    public void TestMethod2()
    {
        NotifyCache.Set("aa", "aaaa", DateTimeOffset.Now.AddSeconds(3));
        var cc = NotifyCache.Get("aa");
    }

    [TestMethod]
    public void TestMethod3()
    {
        var rr = NeuzNotify.WXWork.SetAuth(auth =>
                            {
                                auth.AgentID    = 1000002;
                                auth.Corpid     = "wwbefbb2e3cdd824b6";
                                auth.Corpsecret = "lpU9RzzOeTbFyLLkHW8PVkOVM4gExKshGx2LoSTQRp4";
                            }).SetTextCardMessage("这里是标题", "明细明细明细", "http://baidu.com", "查看详情")
                           .Send()
                           .Result;
    }
}