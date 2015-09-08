using System;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace Tests.FileFollowerTests
{
    [TestFixture]
    public class TestLogTestsFromExampleLog
    {
        [Test]
        public void CreateATestLogLineByLine()
        {
            //new Random().Next(0, 0)
            File.Delete("CreateATestLogLineByLine.log");
            new CreateTestLog("CreateATestLogLineByLine.log").StartWritingToTestLog(0);
            File.Delete("CreateATestLogLineByLine.log");
        }

        [Test]
        public void VerifyFileIsCreatedOnInstantiatonOfTestLog()
        {
            File.Delete("isfilecreated.txt");
            using(var testLog = new CreateTestLog("isfilecreated.txt"))
            {}
            Assert.IsTrue(File.Exists("isfilecreated.txt"));
            File.Delete("isfilecreated.txt");
        }

    }

    public class CreateTestLog : IDisposable
    {
        private readonly string _filetowrite;
        readonly StreamWriter _writer = null;
        private StreamReader _file = null;
        bool _keeprunning = true;

        public CreateTestLog(string filetowrite)
        {
            _filetowrite = filetowrite;
            _writer = new StreamWriter(filetowrite, false);
        }

        public void Stop()
        {
            _keeprunning = false;
        }

        public void StartWritingToTestLog(int delayInMilliseconds)
        {
            const string examplefile = @"ExampleFiles\ExampleLogFiles\LotroLog.txt";
            string line;

            // Read the _file and write it to another _file line by line..
            _file = new System.IO.StreamReader(examplefile);
            while ((line = _file.ReadLine()) != null)
            {
                if(!_keeprunning)
                    break;
                
                _writer.WriteLine(line);
                Thread.Sleep(delayInMilliseconds);
            }

            _file.Close();
            _writer.Close();
        }

        public void Dispose()
        {
            if (_file != null) _file.Close();
            if (_writer != null) _writer.Close();
        }
    }
}