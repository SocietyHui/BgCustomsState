using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IED
{
    public class WebBrowserUrl : CancelEventArgs
    {
        public String Url { get; }

        public String Frame { get; }

        public WebBrowserUrl(String url, String frame) : base()
        {
            this.Url = url;
            this.Frame = frame;
        }
    }

    public class WebBrowserUrl2
    {
        public String Url { get; }

        public String Frame { get; }

        public WebBrowserUrl2(String url, String frame)
        {
            this.Url = url;
            this.Frame = frame;
        }
    }
    public class WebBrowserEvent
    {
        public bool cancel;
    }



    public class NewWebBrwser : WebBrowser
    {
        System.Windows.Forms.AxHost.ConnectionPointCookie cookie;
        NewWebBrowserEvent events;

        public event EventHandler BeforeNavigate;

        //   public event EventHandler BeforeNewWindow;

        public delegate void OnNewWindow2(WebBrowserUrl2 webPra, WebBrowserEvent cancel);

        public event OnNewWindow2 BeforeNewWindow2;

        protected override void CreateSink()
        {
            base.CreateSink();//还是需要源
            events = new NewWebBrowserEvent(this);
            cookie = new System.Windows.Forms.AxHost.ConnectionPointCookie(this.ActiveXInstance, events, typeof(INewDWWebBrowserEvent));
        }
        protected override void DetachSink()
        {
            if (null != cookie)
            {
                cookie.Disconnect();
                cookie = null;
            }
            base.DetachSink();
        }
        public void OnBeforeNavigate(string url, string frame, out bool cancel)
        {
            var Arg = BeforeNavigate;
            WebBrowserUrl webBrowserUrl = new WebBrowserUrl(url, frame);
            Arg?.Invoke(this, webBrowserUrl);
            cancel = webBrowserUrl.Cancel;
        }
        public void OnBeforeNewWindow(string url, out bool cancel)
        {
            //var Arg = BeforeNewWindow;
            //WebBrowserUrl webBrowserUrl = new WebBrowserUrl(url, null);
            //Arg?.Invoke(this, webBrowserUrl);
            //cancel = webBrowserUrl.Cancel;
            var Arg = BeforeNewWindow2;
            WebBrowserUrl2 webBrowserUrl = new WebBrowserUrl2(url, null);
            WebBrowserEvent webBrowserEvent = new WebBrowserEvent();
            Arg?.Invoke(webBrowserUrl, webBrowserEvent);
            cancel = webBrowserEvent.cancel;
        }


    }
    public class NewWebBrowserEvent : System.Runtime.InteropServices.StandardOleMarshalObject, INewDWWebBrowserEvent
    {
        private NewWebBrwser webBrowser;

        public NewWebBrowserEvent(NewWebBrwser newWebBrowser)
        {
            webBrowser = newWebBrowser;
        }

        public void BeforeNavigate2(object pDisp, ref object urlObject, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel)
        {
            webBrowser.OnBeforeNavigate((string)urlObject, (string)targetFrameName, out cancel);
        }
        //官方说明此 事件是低于IE6时会引发
        public void NewWindow2(ref object ppDisp, ref bool cancel)
        {

            webBrowser.OnBeforeNewWindow(((WebBrowser)ppDisp).Url.ToString(), out cancel);
        }
        //当高于IE6时使用
        public void NewWindow3(object pDisp, ref bool cancel, ref object flags, ref object URLContext, ref object URL)
        {
            webBrowser.OnBeforeNewWindow((string)URL, out cancel);
        }
    }
    [System.Runtime.InteropServices.ComImport(), System.Runtime.InteropServices.Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D"),
     System.Runtime.InteropServices.InterfaceTypeAttribute(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIDispatch),
     System.Runtime.InteropServices.TypeLibType(System.Runtime.InteropServices.TypeLibTypeFlags.FHidden)]
    public interface INewDWWebBrowserEvent
    {
        [System.Runtime.InteropServices.DispId(250)]
        void BeforeNavigate2(object pDisp, ref object urlObject, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel);
        //官方说明此 事件是低于IE6时会引发
        [System.Runtime.InteropServices.DispId(251)]
        void NewWindow2(ref object ppDisp, ref bool cancel);
        //当高于IE6时使用
        [System.Runtime.InteropServices.DispId(273)]
        void NewWindow3(object pDisp, ref bool cancel, ref object flags, ref object URLContext, ref object URL);
    }

}