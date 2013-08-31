using System;
using BomberEngine.Core.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BomberEngineTests
{
    [TestClass]
    public class CommandLineTest
    {
        [TestMethod]
        public void TestParse()
        {
            String[] args = { "-d", "demo" };

            CommandLine cmd = new CommandLine();
            cmd.Register(new CmdEntry1());
            cmd.Parse(args);


        }
    }

    class CmdEntry1 : CommandLineEntry
    {
        public CmdEntry1()
            : base("d", "demo")
        {
        }

        public override void  Parse(ArgsIterator iter)
        {
 	        throw new NotImplementedException();
        }
    }
}
