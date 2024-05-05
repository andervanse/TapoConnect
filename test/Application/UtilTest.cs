using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tapo.Application.Util;

namespace Tapo.Application.Test;

[TestClass]
public class UtilTest:BaseTest
{
    [TestMethod]
    public void GetIpAddressByMacAddress()
    {
        var device = AuthenticationConfig.Devices.FirstOrDefault();
        var ip = TapoUtils.GetIpAddressByMacAddress(device.MacAddress);

        Assert.AreEqual(device.IpAddress, ip);
    }
}
