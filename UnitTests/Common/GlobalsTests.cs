using DNN.Modules.DnnUserVoice.Common;
using System;
using Xunit;

namespace UnitTests.Common
{
    public class GlobalsTests
    {
        [Fact]
        public void ModulePrefixIsValid()
        {
            var modulePrefix = Globals.ModulePrefix;
            Assert.Equal(expected: "DNN_UserVoice_", modulePrefix);
        }
    }
}
