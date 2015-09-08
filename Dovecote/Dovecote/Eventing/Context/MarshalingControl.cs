using System;
using System.Windows.Forms;

namespace Dovecote.Eventing.Context
{
    public class MarshalingControl : Control
    {
        public MarshalingControl()
        {
            Visible = false;
            SetTopLevel(true);
            CreateControl();
            CreateHandle();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
        }

        protected override void OnSizeChanged(EventArgs e)
        {
        }
    }
}