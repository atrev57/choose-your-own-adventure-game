using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ScriptedDieTests
{
    [TestMethod]
    public void Roll_OutOfScriptedRolls_FailsClearly()
    {
        ScriptedDie die = new ScriptedDie(4);
        Assert.AreEqual(4, die.Roll(20));

        InvalidOperationException ex = Assert.ThrowsException<InvalidOperationException>(() => die.Roll(20));
        StringAssert.Contains(ex.Message, "ran out of scripted rolls");
    }
}
