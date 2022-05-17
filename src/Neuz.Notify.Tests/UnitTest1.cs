using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Neuz.Notify.Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        NeuzNotify.WXWork.SetAuth(auth =>
                   {
                       auth.AgentID    = 1000002;
                       auth.Corpid     = "wwbefbb2e3cdd824b6";
                       auth.Corpsecret = "lpU9RzzOeTbFyLLkHW8PVkOVM4gExKshGx2LoSTQRp4";
                   })
                  .SetTextMessage("这里是测试文本\n <a href=\"https://baidu.com\">测试链接</a> \n \n 哈哈哈哈哈")
                  .Send();
    }
}