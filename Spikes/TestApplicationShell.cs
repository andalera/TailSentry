using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dovecote.Eventing.Attributes;
using Dovecote.Eventing.Interfaces;

namespace Spikes
{
    


    public partial class TestApplicationShell : Form
    {
        //private IApplicationShellPresenter _presenter;
        
        //public TestApplicationShell()
        //{
        //    InitializeComponent();
        //}

        //public void InitializeLogFile(string path)
        //{
        //    LogFileField.Text = path;
        //}

        //private void BrowseLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    _presenter.GetLogFile();
        //}

        //private void ApplicationShell_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    _presenter.ExitApplication();
        //}

    }

    //public interface IApplicationShell : IView
    //{
    //    IApplicationShellPresenter Presenter { get; set; }
    //    void InitializeLogFile(string path);
    //}

    //public interface IView
    //{
    //    void Show();
    //    void Hide();
    //    void Close();
    //}

    //public interface IApplicationShellPresenter: IPresenter
    //{
    //    void ExitApplication();
    //    void GetLogFile();
    //}

    //public interface IPresenter
    //{

    //}

    //[SynchronizeHandlers]
    //public class ApplicationShellPresenter : IApplicationShellPresenter, 
    //    IListenFor<TestMessage>, IListenForInBatches<TestMessage>
    //{
    //    public void ExitApplication()
    //    {
    //        Application.Exit();
    //    }

    //    public void GetLogFile()
    //    {
    //        throw new NotImplementedException();
    //    }


    //    public void Handle(TestMessage message)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    [BatchOptions(Interval = 10)]
    //    public void Handle(IList<TestMessage> messages)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class TestMessage
    //{
    //    public string Message { get; private set; }
        
    //    public TestMessage(string message)
    //    {
    //        Message = message;
    //    }
    //}

    //public class SynchronizeHandlersAttribute : Attribute
    //{
    //}

}
