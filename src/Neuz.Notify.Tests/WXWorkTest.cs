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
                               .SetTextMessage("�����ǲ����ı�\n <a href=\"https://baidu.com\">��������</a> \n \n ����������")
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
                            }).SetTextCardMessage("�����Ǳ���", "��ϸ��ϸ��ϸ", "http://baidu.com", "�鿴����")
                           .Send()
                           .Result;
    }
}