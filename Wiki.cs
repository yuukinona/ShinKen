using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShinKen
{
    public partial class Wiki : Form
    {
        public int X, Y;
        public bool active = false;
        private int PageNo = 0;
        string[] url=new string [2];

        public Wiki()
        {
            InitializeComponent();
            //クライアントへレスポンスを返した後に呼ばれるイベント
            url[0] = "http://wikiwiki.jp/sinken/";
            url[1] = "http://shinkenkr.com/";
            webBrowser1.Navigate(url[0]);
            this.Hide();
        }

        private void Wiki_Load(object sender, EventArgs e)
        {
            //this.Left = this.X;
            //this.Top = this.Y;
        }

        void FiddlerApplication_AfterSessionComplete(Fiddler.Session oSession)
        {
            //取り敢えずログを吐く
            System.Diagnostics.Debug.WriteLine(string.Format("Session {0}({3}):HTTP {1} for {2}",
                    oSession.id, oSession.responseCode, oSession.fullUrl, oSession.oResponse.MIMEType));
        }

        public void Wiki_Refresh()
        {
            webBrowser1.Navigate(url[PageNo]);
        }

        private void Wiki_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Hide();
            this.active = false;
        }

        private void Wiki_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
            //this.Hide();
        }

        private void Wiki_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.Hide();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void Wiki_Click(object sender, EventArgs e)
        {
            if (PageNo == 1)
            {
                webBrowser1.Navigate(url[0]);
                PageNo = 0;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (PageNo == 0)
            {
                webBrowser1.Navigate(url[1]);
                PageNo = 1;
            }
        }

        private void Wiki_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                webBrowser1.GoBack();
            }
        }
    }
}
