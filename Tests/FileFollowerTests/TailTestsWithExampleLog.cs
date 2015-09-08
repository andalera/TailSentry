using System;
using System.IO;
using System.Threading;
using FileMonitoring;
using NUnit.Framework;
using Retlang.Channels;
using Retlang.Fibers;

namespace Tests.FileFollowerTests
{
    [TestFixture]
    public class TailTestsWithExampleLog
    {
        [Test, Ignore("Long running test.")]
        public void TestTailWithExampleLog()
        {
            File.Delete("test.log");

            var tailfiber = new PoolFiber();
            var reset = new AutoResetEvent(false);

            using (var tail = new Tail())
            {
                var tailchannel = new Channel<string>();
                tailchannel.Subscribe(tailfiber, str =>
                                                     {
                                                         tail.OutputRecievedEvent += OnOutputRecievedEvent;
                                                         tail.Start("test.log");
                                                     });

                tailfiber.Start();

                using (var testlog = new CreateTestLog("test.log"))
                {
                    tailchannel.Publish("");
                    testlog.StartWritingToTestLog(10);
                    testlog.Stop(); //don't really need to call this, but heh, what the...
                }

                tail.Stop();
                while(tail.State != TailState.Stopped){/*wait for tail to stop*/}
            }

            File.Delete("test.log");
        }

        void OnOutputRecievedEvent(object sender, OutputRecievedEvent e)
        {
            Console.Write(e.Output + "\n");
        }
    }
}