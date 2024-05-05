﻿using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Tapo.Application.Test;


[TestClass]
public class SetDeviceStateTest
{
    [TestMethod]
    public void EqualityComparison()
    {
        var hex = new TapoSetBulbState(TapoColor.FromHex("34eba4"), true);
        var rgb = new TapoSetBulbState(TapoColor.FromRgb("52, 235, 164"), true);

        Assert.AreEqual(hex, rgb);
    }

    [TestMethod]
    public void NotEqualityComparison()
    {
        var hex = new TapoSetBulbState(TapoColor.FromHex("eb34ae"), true);
        var rgb = new TapoSetBulbState(TapoColor.FromRgb("52, 235, 164"), true);

        Assert.AreNotEqual(hex, rgb);
    }

    [TestMethod]
    public void InheritedEqualityComparison()
    {
        TapoSetDeviceState hex = new TapoSetBulbState(TapoColor.FromHex("34eba4"), true);
        TapoSetBulbState rgb = new TapoSetBulbState(TapoColor.FromRgb("52, 235, 164"), true);

        Assert.AreEqual(hex, rgb);
    }

    [TestMethod]
    public void DifferntTypesEqualityComparison()
    {
        var hex = new TapoSetPlugState(true);
        var rgb = new TapoSetBulbState(TapoColor.FromRgb("52, 235, 164"), true);

        Assert.AreNotEqual(hex, rgb);
    }
}
