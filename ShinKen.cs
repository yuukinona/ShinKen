using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;   //调用WINDOWS API函数时要用到
using Microsoft.Win32;  //写入注册表时要用到
using System.Drawing.Imaging;


namespace ShinKen
{
    public partial class ShinKen : Form
    {
        private bool Event_Start=false;
        private int battleEnd = 0;
        private int battleBegin = 0;

        #region API
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
        [DllImport("user32")]
        public static extern int GetSystemMetrics(int nIndex);
        #endregion

        #region Mouse_API
        [DllImport("user32.dll")]
        static extern void mouse_event(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);
        [DllImport("user32.dll")]
        private static extern int SetCursorPos(int x, int y);
        [Flags]        
        enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }

        #endregion 

        #region Clear_Memory
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion

        #region Parameter
        private KeyboardHook k_hook = new KeyboardHook();
        PageJudgement PageJudge = new PageJudgement();
        PageJudgement.PageNo PageNo;
        Wiki wiki;
        Picture_capture PIC;
        Timer time1 = new Timer();
        Timer time2 = new Timer();
        public Bitmap btm;       

        private bool AutoStart = false;
        private bool MouseStart = false;
        private bool Key_Press = false;
        double BeginTime;
        public int width = Screen.PrimaryScreen.Bounds.Width;
        public int height = Screen.PrimaryScreen.Bounds.Height;        
        private int tick = 0;

        static int defaultwidth = 975;
        static int defaultheight = 677;
        static int defaultx = 30;
        static int defaulty = 61;
        private int zoom = 0;
        private int count = 0;
        public bool WikiActive = false;
        private int Repairing = 0;
        private byte repairNO = 1;
        private bool SetTop = false;      

        #endregion

        public bool IsTopActive(IntPtr handle)
        {
            IntPtr activeHandle = GetForegroundWindow();
            return (activeHandle == handle);
        }
        
        public ShinKen()
        {
            
            InitializeComponent();
            //this.TopMost = true;

            #region Page Bitmap

            PageJudge.MainPage = new Bitmap(@"MainPage.jpg");
            PageJudge.BigMap = new Bitmap(@"BigMap.jpg");
            PageJudge.BattlePage = new Bitmap(@"BattlePage.jpg");
            PageJudge.WinPage = new Bitmap(@"WinPage.jpg");
            PageJudge.LoadingPage = new Bitmap(@"LoadingPage.jpg");
            PageJudge.RepairPage = new Bitmap(@"Repair.jpg");
            PageJudge.Repairing = new Bitmap(@"Repairing.jpg");

            #endregion

            btm = new Bitmap(width, height);
            PIC = new Picture_capture();
            PIC.Left = this.Left;
            PIC.Top = this.Bottom;
            //PIC.Show();
            k_hook.KeyDownEvent += new KeyEventHandler(hook_KeyDown);//Key Pressed Hook
            k_hook.Start();//Start Keyboard Hook    

            #region Fiddler
            //クライアントへレスポンスを返した後に呼ばれるイベント
            Fiddler.FiddlerApplication.AfterSessionComplete
                        += new Fiddler.SessionStateHandler(FiddlerApplication_AfterSessionComplete);

            //Fiddlerを開始。ポートは自動選択
            Fiddler.FiddlerApplication.Startup(0, Fiddler.FiddlerCoreStartupFlags.ChainToUpstreamGateway);

            //当該プロセスのプロキシを設定する。WebBroweserコントロールはこの設定を参照
            Fiddler.URLMonInterop.SetProxyInProcess(string.Format("127.0.0.1:{0}",
                        Fiddler.FiddlerApplication.oProxy.ListenPort), "<local>");
            #endregion

            webBrowser1.Navigate("http://www.dmm.com/netgame/social/-/gadgets/=/app_id=319803/");
            wiki=new Wiki();
            //wiki = new Wiki(500, 500);
            this.MouseWheel += new MouseEventHandler(Form1_MouseWheel);
            
        }

        private void ShinKen_Refresh()
        {
            webBrowser1.Navigate("http://www.dmm.com/netgame/social/-/gadgets/=/app_id=319803/");  
        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            //判断按下的键（F8）
            if (this.IsTopActive(this.Handle) && e.KeyValue == (int)Keys.F5)
            {                
                this.webBrowser1.Document.Window.ScrollTo(30, 61);
                this.ShinKen_Refresh();
                if (zoom==2)
                {
                    webBrowser1.Document.Body.Style = "zoom:0.3";
                    this.webBrowser1.Document.Window.ScrollTo(17, 19);
                }
                if (zoom == 1)
                {
                    webBrowser1.Document.Body.Style = "zoom:0.6";
                    this.webBrowser1.Document.Window.ScrollTo(23, 37);
                }
            }
            if (this.IsTopActive(wiki.Handle) && e.KeyValue == (int)Keys.F5)
            {
                wiki.Wiki_Refresh();
            }

            if (e.KeyValue == (int)Keys.F8)
            {
                if (this.IsTopActive(this.Handle))
                {
                    zoom++;
                    if (zoom == 3) zoom = 0;
                    if (zoom==0)
                    {
                        webBrowser1.Document.Body.Style = "zoom:1.0";
                        this.webBrowser1.Document.Window.ScrollTo(defaultx, defaulty);
                        this.Height = (int)(defaultheight * 1)-1;
                        this.Width = (int)(defaultwidth * 1);
                        this.Show();
                        //SwitchToThisWindow(this.Handle, true);
                    }
                    else
                        if (zoom == 1)
                        {
                            webBrowser1.Document.Body.Style = "zoom:0.6";
                            this.webBrowser1.Document.Window.ScrollTo(23, 37);
                            this.Height = (int)(defaultheight * 0.6) + 14;
                            this.Width = (int)(defaultwidth * 0.6+5);
                            this.Show();
                            wiki.Hide();
                        }
                        else
                        {
                            webBrowser1.Document.Body.Style = "zoom:0.3";
                            this.webBrowser1.Document.Window.ScrollTo(17, 19);
                            this.Height = (int)(defaultheight * 0.3) + 25;
                            this.Width = (int)(defaultwidth * 0.3)+11;
                            this.Show();
                            wiki.Hide();
                        }
                }
                else
                {
                    this.Show();
                    //SwitchToThisWindow(this.Handle, true);
                }
            }
            if (e.KeyValue == (int)Keys.F9)
            {
                MouseStart = !MouseStart;
                if (MouseStart)
                {
                    this.time2 = new Timer();
                    this.time2.Interval = 1;
                    this.time2.Enabled = true;
                    this.time2.Tick += new System.EventHandler(this.time2_Tick);
                    this.time2.Start();
                }
                else
                {
                    time2.Stop();
                    time2.Dispose();
                }
                
            }

            if (e.KeyValue == (int)Keys.F10)
            {
                if (Repairing == 0)
                {
                    AutoStart = !AutoStart;
                    if (AutoStart)
                    {
                        count = 0;
                        //BeginTime = DateTime.Now.TimeOfDay.TotalSeconds;
                        BeginTime = 0;
                    }
                }
                else
                {
                    Repairing = 0;
                }
                /*
                Repairing = 1-Repairing;
                count = 0;*/
            }

            if (e.KeyValue == (int)Keys.F11)
            {
                if (Repairing == 0)
                {
                    Event_Start = !Event_Start;
                    if (Event_Start)
                    {
                        count = 0;
                        repairNO = 2;
                        //BeginTime = DateTime.Now.TimeOfDay.TotalSeconds;
                        BeginTime = 0;
                    }
                }
                else
                {
                    Repairing = 0;
                }
                /*
                Repairing = 1-Repairing;
                count = 0;*/
            }

            if (e.KeyValue == (int)Keys.Escape)
            {
                if (this.IsTopActive(this.Handle))
                {
                    this.Hide();
                    /*
                    webBrowser1.Document.Body.Style = "zoom:0.3";
                    zoom = true;
                    this.webBrowser1.Document.Window.ScrollTo(16, 19);
                    this.Height = (int)(defaultheight * 0.3)+25;
                    this.Width = (int)(defaultwidth * 0.3);
                    this.Show();
                    wiki.Hide();*/
                } 
                if (this.IsTopActive(wiki.Handle))
                {
                    wiki.Hide();
                }
            }
            if ((e.KeyValue == (int)Keys.H) && this.IsTopActive(this.Handle))
            {
                if (richTextBox1.Visible) richTextBox1.Visible=false;
                else richTextBox1.Visible=true;
            }
            if ((e.KeyValue == (int)Keys.W) && this.IsTopActive(this.Handle))
            {
                /*
                if (wiki == null || wiki.IsDisposed)
                {
                    wiki = new Wiki();
                    WikiActive = false;
                }
                else wiki.WindowState = FormWindowState.Normal; */
                if (WikiActive && wiki.Visible)
                {
                    wiki.Hide();
                    WikiActive = false;
                    return;
                }
                wiki.Left = this.Right;
                wiki.Top = this.Top;
                wiki.Show();
                WikiActive = true;
            }
            if ((e.KeyValue == (int)Keys.P) && this.IsTopActive(this.Handle))
            {
                this.PIC.pictureBox1.Image.Save(@"C:\" + tick.ToString() + ".jpg");
            }
            if ((e.KeyValue == (int)Keys.T) && this.IsTopActive(this.Handle))
            {
                this.TopMost=!this.TopMost;
            }
            
        }


       // private void button1_Click(object sender, EventArgs e)
       // {
            //WebBrowserコントロールに指定URIを開かせる
      //      webBrowser1.Navigate(textBox1.Text);
      //  }

        void Button1_cliclk(object sender, EventArgs e)
        {
           // webBrowser1.Navigate(textBox1.Text);
        }

        void FiddlerApplication_AfterSessionComplete(Fiddler.Session oSession)
        {
            //取り敢えずログを吐く
            System.Diagnostics.Debug.WriteLine(string.Format("Session {0}({3}):HTTP {1} for {2}",
                    oSession.id, oSession.responseCode, oSession.fullUrl, oSession.oResponse.MIMEType));
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //プロキシ設定を外す
            Fiddler.URLMonInterop.ResetProxyInProcessToDefault();

            //Fiddlerを終了させる
            Fiddler.FiddlerApplication.Shutdown();
        }

        private void Mouse_Click(int x, int y)
        {
            int mousex=Control.MousePosition.X;
            int mousey = Control.MousePosition.Y;
            SetCursorPos(this.Left + x, this.Top + y);
            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
            //SetCursorPos(mousex, mousey);

        }

        private void time1_Tick(object sender, EventArgs e)
        {
            this.TopMost = true;
            //if (!this.IsTopActive(this.Handle) || 
            if(this.WindowState==FormWindowState.Minimized) return;
            tick++;
            if (tick == 100)
            {
                ClearMemory();
                tick = 0;
            }
            //Snapshot(new Rectangle(this.Left+27,this.Top+116,this.Left+676,this.Top+276));
            PageNo=PageJudge.Judge(btm);
            Color c = PageJudge.GetPoint(btm, 626 + this.Left, 389 + this.Top);
            //richTextBox1.Text = this.ClientSize.Width.ToString() + "," + this.ClientSize.Height.ToString();
            richTextBox1.Text=PageNo.ToString()+" t:"+BeginTime.ToString()+","+battleBegin.ToString()+","+battleEnd.ToString();
            //richTextBox1.Text = Repairing.ToString()+","+count.ToString();
            //richTextBox1.Text = SetTop.ToString()+"\n"+this.TopMost.ToString();
            
            PIC.richTextBox1.Text = c.ToString()+"       "+PageNo.ToString();
            if (PageNo == PageJudgement.PageNo.Battle_Page)
            {
                
                if ((c.R - c.G > 70) || (c.R - c.B > 70))
                {
                    richTextBox1.Text = "Monster Coming!";
                }
                else richTextBox1.Text = "NULL";
            }
            //Snapshot(new Rectangle(this.Left+626 , this.Top+389,60,60));
            Snapshot(new Rectangle(this.Left, this.Top, defaultwidth, defaultheight));
            //Snapshot(new Rectangle(this.Left + 11, this.Top + 33, this.ClientSize.Width, this.ClientSize.Height));
            //Snapshot(new Rectangle(this.Left+8, this.Top+29, this.ClientSize.Width, this.ClientSize.Height));
            //Bitmap b = new Bitmap(PIC.pictureBox1.Image);
            //PIC.richTextBox1.Text = b.GetPixel(257 - 27, 237 - 116).ToString();
            //b.Dispose();

            Repair();

            if (AutoStart)
            {
                Battle_Start();
            }
            if (Event_Start)
            {
                EventBattle();
            }
        }

        private void time2_Tick(object sender, EventArgs e)
        {
            if (!this.IsTopActive(this.Handle)) return;
            if (MouseStart)
            {
                //SetCursorPos(Control.MousePosition.X, Control.MousePosition.Y);
                for (int i = 0; i <= 5; i++)
                {
                    mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
                    mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Left = Screen.PrimaryScreen.WorkingArea.Left;
            this.Top = Screen.PrimaryScreen.WorkingArea.Top;


            this.time1.Interval = 500;
            this.time1.Tick += new System.EventHandler(this.time1_Tick);
            this.time1.Start();
            
            
            richTextBox1.Visible = false;
            
            
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (zoom==0) this.webBrowser1.Document.Window.ScrollTo(defaultx, defaulty);
            if (zoom==1)
            {
                webBrowser1.Document.Body.Style = "zoom:0.6";
                this.webBrowser1.Document.Window.ScrollTo(23, 37);
            }
            if (zoom == 2)
            {
                webBrowser1.Document.Body.Style = "zoom:0.3";
                this.webBrowser1.Document.Window.ScrollTo(17, 19);
            }
            //else this.webBrowser1.Document.Window.ScrollTo(1, 1);
            //this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void ShinKen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Hide();
        }

        
        private void ShinKen_FormClosing(object sender, FormClosingEventArgs e)
        {
            k_hook.Stop();
            wiki.Dispose();
        }

        
        private void Snapshot(Rectangle range)
        {

            using (Graphics g = Graphics.FromImage(btm))
            {
                g.CopyFromScreen(0, 0, 0, 0, Screen.AllScreens[0].Bounds.Size);
                g.Dispose();
                //this.PIC.pictureBox1.Image = CutImage(btm,range);
                //btm.Save("D:\\a.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        

        Bitmap CutImage(Image img, Rectangle rect)
        {
            Bitmap b = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(b);
            g.DrawImage(img, 0, 0, rect, GraphicsUnit.Pixel);
            g.Dispose();
            return b;
            b.Dispose();
        }

        private int min(int x, int y)
        {
            if (x > y) return y;
            else return x;
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            int mousex = Control.MousePosition.X;
            int mousey = Control.MousePosition.Y;
            if (PageNo == PageJudgement.PageNo.Main_Page)
            {
                //SetCursorPos(this.Left + 9, this.Top + 350);
                mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
                if (e.Delta > 0)
                {
                    Mouse_Wheel(0, 25);
                }
                else
                {
                    Mouse_Wheel(0,-25);
                }
                mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                //SetCursorPos(mousex, mousey);
            }
            else
            {            
                mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
                if (e.Delta > 0)
                {
                    Mouse_Wheel(45, 0);
                }
                else
                {
                    Mouse_Wheel(-45, 0);
                }
                mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                SetCursorPos(mousex, mousey);
            }
        }

        private void Mouse_Wheel(int x, int y)
        {
            mouse_event(MouseEventFlag.Move, x, 1, 0, UIntPtr.Zero);
        }

        private void ShinKen_MouseEnter(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void ShinKen_Move(object sender, EventArgs e)
        {
            PageJudge.LocationX = this.Left;
            PageJudge.LocationY = this.Top;
            wiki.Left = this.Right;
            wiki.Top = this.Top;
            Snapshot(new Rectangle(this.Left, this.Top, defaultwidth, defaultheight));
        }

        private void ShinKen_MouseClick(object sender, MouseEventArgs e)
        {
            MessageBox.Show("click");
        }

        private void Repair()
        {
            if (Repairing == 1)
            {
                //if (count<=2) Mouse_Click(780, 609);
                count++;
                if (count < 10) return;
                if (repairNO == 1)
                {
                    switch (count)
                    {
                        case 15: { Mouse_Click(324, 642); } break; //enter repair page
                        case 25: { Mouse_Click(139, 172); } break; //click first repair button
                        case 27: { Mouse_Click(463, 164); } break;//click menu bar
                        case 30: { Mouse_Click(448, 362); } break;//select display

                        case 33: { Mouse_Click(152, 214); } break;//choose first
                        case 36: { Mouse_Click(660, 557); } break;//start
                        case 39: { Mouse_Click(122, 353); } break;//click second repair button
                        case 42: { Mouse_Click(160, 276); } break;//choose second
                        case 44: { Mouse_Click(660, 557); } break;//start
                    }
                }
                else
                {
                    switch (count)
                    {
                        case 15: { Mouse_Click(324, 642); } break; //enter repair page
                        case 25: { Mouse_Click(139, 172); } break; //click first repair button
                        case 27: { Mouse_Click(463, 164); } break;//click menu bar
                        case 30: { Mouse_Click(448, 362); } break;//select display

                        case 33: { Mouse_Click(152, 214); } break;//choose first
                        case 36: { Mouse_Click(660, 557); } break;//start

                        case 39: { Mouse_Click(499, 172); } break;//click second repair button
                        case 42: { Mouse_Click(160, 276); } break;//choose second
                        case 45: { Mouse_Click(660, 557); } break;//start

                        case 48: { Mouse_Click(122, 353); } break;//click third repair button
                        case 51: { Mouse_Click(191, 347); } break;//choose third
                        case 54: { Mouse_Click(191, 410); } break;//choose forth
                        case 57: { Mouse_Click(660, 557); } break;//start
                    }
                }
                if ((count > 60) && (PageNo == PageJudgement.PageNo.Repair_Page))
                {
                    Repairing = 2;
                    count = 0;
                }

            }
            if (Repairing == 2)
            {
                if (count <= 0) Mouse_Click(143, 654);
                count++;
                if (count == 10)
                {
                    if (repairNO == 2)
                    {
                        Repairing = 0;
                        Event_Start = true;
                        count = 0;
                        BeginTime = 0;
                    }
                    else
                    {
                        Repairing = 0;
                        AutoStart = true;
                        count = 0;
                        BeginTime = 0;
                    }
                }
            }
        }

        private void Battle_Start()
        {
            BeginTime++;
            int t = Convert.ToInt32(BeginTime);//Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds - BeginTime);

            if (zoom == 0)
            {
                switch (t)
                {
                    case 3 * 2:
                        {
                            Mouse_Click(241, 589);
                        } break;
                    case 5 * 2:
                        {
                            Mouse_Click(146, 400);
                        } break;
                    case 7 * 2:
                        {
                            Mouse_Click(656, 428);
                            Mouse_Click(390, 410);
                        } break;
                    case 15:
                        {
                            
                        } break;
                    case 21 * 2:
                        {
                            Mouse_Click(228, 586);
                            Mouse_Click(481, 435);
                            Mouse_Click(896, 639);
                        } break;
                    case 26 * 2:
                        {
                            Mouse_Click(142, 593);
                            Mouse_Click(469, 376);
                        } break;
                    case 91 * 2:
                        {
                            Mouse_Click(474, 358);
                            Mouse_Click(474, 358);
                        } break;
                    case 112 * 2:
                        {
                            Mouse_Click(474, 358);
                        } break;
                    case 115 * 2:
                        {
                            Mouse_Click(897, 618);
                        } break;
                    case 120 * 2:
                        {

                            //BeginTime = DateTime.Now.TimeOfDay.TotalSeconds;
                            if (count % 2 == 0)
                            {
                                BeginTime = 0;
                                count++;
                                if (count == 15)
                                {
                                    //System.Windows.Forms.MessageBox.Show("研磨してください！");
                                    Repairing = 1;
                                    AutoStart = false;
                                    count = 0;
                                }
                            }
                            else Mouse_Click(49, 647);
                        } break;
                    case 125 * 2: //Clear Mission
                        {
                            Mouse_Click(570, 182);
                        } break;
                    case 127 * 2:  //Click
                        {
                            Mouse_Click(94, 385);
                        } break;
                    case 129 * 2:  //Open Mission Menu
                        {
                            Mouse_Click(545, 315);
                        } break;
                    case 131 * 2:  //Open Mission 
                        {
                            Mouse_Click(245, 553);
                        } break;
                    case 133 * 2:  //Choose people
                        {
                            Mouse_Click(152, 214);
                        } break;
                    case 135 * 2:  //Go
                        {
                            Mouse_Click(660, 557);
                        } break;
                    case 137 * 2:  //Battle Map
                        {
                            Mouse_Click(149, 651);
                        } break;
                    case 140 * 2:
                        {
                            BeginTime = 0;
                            count++;
                        } break;
                }
            }
            if (zoom == 1)
            {
                switch (t)
                {
                    case 3:
                        {
                            Mouse_Click(142, 352);
                        } break;
                    case 5:
                        {
                            Mouse_Click(73, 254);
                        } break;
                    case 7:
                        {
                            Mouse_Click(400, 276);
                        } break;

                    case 18:
                        {
                            Mouse_Click(136, 378);
                            Mouse_Click(278, 273);
                            Mouse_Click(540, 385);
                        } break;
                    case 25:
                        {
                            Mouse_Click(79, 382);
                            Mouse_Click(275, 237);
                        } break;
                    case 91:
                        {
                            Mouse_Click(280, 235);
                            Mouse_Click(280, 235);
                        } break;
                    case 112:
                        {
                            Mouse_Click(279, 220);
                        } break;
                    case 115:
                        {
                            Mouse_Click(538, 383);
                        } break;
                    case 120:
                        {
                            BeginTime = DateTime.Now.TimeOfDay.TotalSeconds;
                            count++;
                            if (count == 15)
                            {
                                System.Windows.Forms.MessageBox.Show("研磨してください！");
                                AutoStart = false;
                                count = 0;
                            }
                        } break;

                }
            }
            if (zoom == 2)
            {
                switch (t)
                {
                    case 3:
                        {
                            Mouse_Click(75, 194);
                        } break;
                    case 5:
                        {
                            Mouse_Click(38, 138);
                        } break;
                    case 7:
                        {
                            Mouse_Click(205, 156);
                        } break;

                    case 18:
                        {
                            Mouse_Click(72, 205);
                            Mouse_Click(141, 150);
                            Mouse_Click(273, 203);
                        } break;
                    case 25:
                        {
                            Mouse_Click(48, 205);
                            Mouse_Click(143, 131);
                        } break;
                    case 91:
                        {
                            Mouse_Click(142, 122);
                            Mouse_Click(142, 122);
                        } break;
                    case 112:
                        {
                            Mouse_Click(142, 122);
                        } break;
                    case 115:
                        {
                            Mouse_Click(276, 204);
                        } break;
                    case 120:
                        {
                            //BeginTime = DateTime.Now.TimeOfDay.TotalSeconds;
                            BeginTime = 0;
                            count++;
                            if (count == 15)
                            {
                                System.Windows.Forms.MessageBox.Show("研磨してください！");
                                AutoStart = false;
                                count = 0;
                            }
                        } break;

                }
            }

            richTextBox1.Text = "Round : " + count.ToString() + "\n" + "Time : " + t.ToString(); 
        }

        private void EventBattle()
        {
            BeginTime++;
            int t = Convert.ToInt32(BeginTime);//Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds - BeginTime);

            if (zoom == 0)
            {
                switch (t)
                {
                    case 3 * 2:
                        {
                            Mouse_Click(349, 301);
                        } break;
                    case 5 * 2:
                        {
                            SetCursorPos(this.Left+565,this.Top+ 583);
                            mouse_event(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
                        } break;
                    case 20 * 2:
                        {
                            mouse_event(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                        } break;
                    case 22 * 2:
                        {
                            Mouse_Click(185, 579);
                        } break;
                    case 24 * 2:
                        {
                            Mouse_Click(656, 428);  //Go
                        } break;
                }
                if(t<200 && t>51 && PageNo!=PageJudgement.PageNo.Loading_Page && battleBegin==0)
                {
                    battleBegin=Convert.ToInt32(BeginTime);
                    battleEnd = 0;
                }
                if (battleBegin != 0)
                {
                    switch (Convert.ToInt32(t - battleBegin))
                    {
                        case 3 * 2: //girl1
                            {
                                Mouse_Click(896, 639);
                                Mouse_Click(368, 661);
                                Mouse_Click(904, 564);
                            } break;
                        case 4 * 2://girl2
                            {
                                Mouse_Click(309, 621);
                                Mouse_Click(913, 504);
                            } break;
                        case 5 * 2://gilr3
                            {
                                Mouse_Click(217, 611);
                                Mouse_Click(919, 310);
                            } break;
                        case 6 * 2://girl4
                            {
                                Mouse_Click(124, 629);
                                Mouse_Click(909, 277);
                            } break;
                        case 40 * 2:
                            {
                                Mouse_Click(909, 277);
                                Mouse_Click(909, 277);
                            } break;
                    }
                }
                if(PageNo==PageJudgement.PageNo.Win_Page && battleEnd==0)
                {
                    battleBegin = 0;
                    battleEnd=Convert.ToInt32(BeginTime);
                    Mouse_Click(388, 358);
                }
                if (battleEnd != 0)
                {
                    switch (Convert.ToInt32(t - battleEnd))
                    {
                        case 2 * 2:
                            {
                                Mouse_Click(805, 623);
                            } break;
                        case 8 * 2: //Clear Mission
                            {
                                Mouse_Click(570, 182);
                            } break;
                        case 10 * 2:  //Click
                            {
                                Mouse_Click(94, 385);
                            } break;
                        case 12 * 2:  //Open Mission Menu
                            {
                                Mouse_Click(545, 315);
                            } break;
                        case 14 * 2:  //Open Mission 
                            {
                                Mouse_Click(245, 553);
                            } break;
                        case 16 * 2:  //Choose people
                            {
                                Mouse_Click(152, 214);
                            } break;
                        case 18 * 2:  //Go
                            {
                                Mouse_Click(660, 557);
                            } break;
                        case 20 * 2:  //Battle Map
                            {
                                Mouse_Click(149, 651);
                            } break;
                        case 32 * 2:
                            {
                                battleEnd = 0;
                                BeginTime = 0;
                                count++;
                                if (count == 6)
                                {
                                    //System.Windows.Forms.MessageBox.Show("研磨してください！");
                                    Repairing = 1;
                                    Event_Start = false;
                                    count = 0;
                                }
                            } break;

                    }
                }
            }
        }

    }
}
