using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Dovecote;
using Dovecote.Eventing.Attributes;
using Dovecote.Eventing.Context;
using Dovecote.Eventing.Interfaces;
using Retlang.Channels;
using Retlang.Core;
using Retlang.Fibers;

namespace Spikes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //TestRelang.Start();
            
            var container = Bootstrapper.Start();
            container
                .AddFacility<Castle.Facilities.Startable.StartableFacility>()
                .Register(
                    Component.For<LongRunning>(),
                    Component.For<TestController>()
                );

            var longrunning = container.Resolve<LongRunning>();
            var testcontroller = container.Resolve<TestController>();

            Application.Run();
        }
    }

    [HandleOnSpecificContext(ContextType.Gui)]
    public class TestController: IListenFor<TextMessage>, IListenFor<WaitMessage>, IListenFor<LongRunningFinishedMessage>
    {
        private readonly IPublisher _publisher;
        private TestForm testForm;

        public TestController(IPublisher publisher)
        {
            _publisher = publisher;
            testForm = new TestForm();

            testForm.LongRunnerButton.Click += new EventHandler(LongRunnerButton_Click);
            testForm.Closed += new EventHandler((sender, args) => Application.Exit());
            testForm.Show();
        }

        void LongRunnerButton_Click(object sender, EventArgs e)
        {
            _publisher.SendMessage(new StartLongRunnerMessage());
        }

        public void Handle(TextMessage message)
        {
            testForm.UpdateTextBox(message.Message);
        }

        public void Handle(WaitMessage message)
        {
            MessageBox.Show(message.Message);
        }
        
        public void Handle(LongRunningFinishedMessage message)
        {
            MessageBox.Show("Long Runner is done...");
        }
    }

    public class LongRunning : IListenFor<StartLongRunnerMessage>, IListenFor<LRInternal>
    {
        private readonly IPublisher _publisher;
        private bool _inprogress = false;

        public LongRunning(IPublisher publisher)
        {
            _publisher = publisher;
        }

        [HandleOnSpecificContext(ContextType.New)]
        public void Handle(StartLongRunnerMessage message)
        {
            if(_inprogress)
            {
                _publisher.SendMessage(new WaitMessage("Running... Please wait..."));
                return;
            }

            _publisher.SendMessage(new LRInternal());

        }

        [HandleOnSpecificContext(ContextType.New)]
        public void Handle(LRInternal message)
        {
            _inprogress = true;
            for (var i = 0; i <= 100; i++)
            {
                _publisher.SendMessage(new TextMessage("From long runner: message number: " + i + "\n"));
                Thread.Sleep(50);
            }
            _inprogress = false;
            _publisher.SendMessage(new LongRunningFinishedMessage());
        }

        //public void Start(){ }
    }

    

    public class WaitMessage {
        public string Message { get; private set; }

        public WaitMessage(string message)
        {
            Message = message;
        }

    }

    public class LRInternal {/* empty class just for signalling .. no data to pass. */}

    public class StartLongRunnerMessage { /* empty class just for signalling .. no data to pass. */}

    public class LongRunningFinishedMessage {/* empty class just for signalling .. no data to pass. */}

    public class TextMessage
    {
        public string Message { get; private set; }

        public TextMessage(string message)
        {
            Message = message;
        }
    }

    public static class Bootstrapper
    {

        public static IWindsorContainer Start()
        {
            IWindsorContainer container = new WindsorContainer();

            // event aggregator registration
            container.Install(new DovecoteInstaller());

            return container;
            
        }
    }

    public static class TestRelang
    {
        public static void Start()
        {

            var poolfiber = new PoolFiber();
            var guifiber = new FormFiber(new MarshalingControl(), new BatchAndSingleExecutor());

            poolfiber.Start();
            guifiber.Start();

            var pubandsub = new Channel<int>();
            var sub = new Channel<string>();

            var form = new TestForm();

            sub.SubscribeToBatch(guifiber, delegate(IList<string> messages)
            {
                form.UpdateTextBox("Here's a batch of messages:\n");
                foreach (var message in messages)
                {
                    form.UpdateTextBox("\t" + message + "\n");
                }

            }, 2000);

            pubandsub.Subscribe(poolfiber, delegate
            {
                Console.WriteLine("pub and sub");
                for (var i = 0; i <= 1500; i++)
                {
                    sub.Publish("Message from another thread: " + i);
                    Thread.Sleep(50);
                }
            });



            form.Show();


            //sub.Publish("astring");
            pubandsub.Publish(0);
        }
    }
}
