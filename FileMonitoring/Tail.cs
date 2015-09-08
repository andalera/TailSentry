using System;
using System.IO;
using System.Threading;

namespace FileMonitoring
{
    public class OutputRecievedEvent : EventArgs
    {
        public string Output { get; set; }
    }

    public enum TailState
    {
        Idle,
        Started,
        Stopped
    }

    public class Tail : IDisposable
    {
        private string _filename;
        private bool _stop;
        private TailState _state;

        public TailState State
        {
            get { return _state; }
        }

        public event EventHandler<OutputRecievedEvent> OutputRecievedEvent;

        public Tail()
        {
            _state = TailState.Idle;
        }

        private void InvokeOutputRecievedEvent(OutputRecievedEvent e)
        {
            EventHandler<OutputRecievedEvent> handler = OutputRecievedEvent;
            if (handler != null) handler(this, e);
        }

        public void Stop()
        {
            _stop = true;
        }

        public void Start(string filename)
        {
            _filename = filename;

            using (var reader = 
                new StreamReader(
                    new FileStream(
                        _filename,
                        FileMode.Open, 
                        FileAccess.Read,
                        FileShare.ReadWrite | FileShare.Delete
                        )
                    )
                )
            {
                _state = TailState.Started;
                //start at the end of the file
                long lastMaxOffset = reader.BaseStream.Length;

                while (true)
                {
                    if (_stop)
                    {
                        reader.BaseStream.Close();
                        reader.Close();
                        break;
                    }

                    Thread.Sleep(100);

                    //if the file size has not changed, idle

                    if (reader.BaseStream.Length == lastMaxOffset)
                        continue;

                    //seek to the last max offset

                    reader.BaseStream.Seek(lastMaxOffset, SeekOrigin.Begin);

                    //read out of the file until the EOF

                    string output;
                    while ((output = reader.ReadLine()) != null)
                        InvokeOutputRecievedEvent(new OutputRecievedEvent {Output = output});

                    //update the last max offset

                    lastMaxOffset = reader.BaseStream.Position;
                }
            }
            _state = TailState.Stopped;
        }

        public virtual void Dispose()
        {
            Stop();
            while(_state != TailState.Stopped)
            {
                //wait for Start method to finish..
                //there's a 100ms sleep in there.
            }
        }
    }
}