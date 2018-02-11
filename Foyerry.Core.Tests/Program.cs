using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foyerry.Core.Quartz;

namespace Foyerry.Core.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            new QuartzJob().Start();

            FoyerryCoreQuartzTests.Add();
        }
    }
}
