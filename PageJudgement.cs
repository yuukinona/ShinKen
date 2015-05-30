using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.IO;
using System.Windows.Forms;

namespace ShinKen
{
    public class PageJudgement
    {
        #region PageNo
        public enum PageNo : uint
        {
            Main_Page=0,
            Scroll_Page=1,
            Repair_Page=2,
            Select_BigMap_Page=3,
            Select_Map_Page=4,
            Choose_Go_Page=5,
            Battle_Page=6,
            Win_Page=7,
            Loading_Page=8,
            Repairing_Page=9,
            Unknow_Page=255
        }
        #endregion
        public Bitmap MainPage, BigMap, BattlePage, WinPage, LoadingPage, RepairPage,Repairing; //ChooseMap, GoPage, ScrollPage1, ScrollPage2, ScrollPage3, ScrollPage4,
        public int LocationX, LocationY;

        public PageJudgement()
        {
            

        }


        
        public Color GetPoint(Bitmap btm,int x, int y)
        {
            return btm.GetPixel(x,y);
        }

        public PageNo Judge(Bitmap pic)
        {
            if (pic == null) return PageNo.Unknow_Page;
            int[] x = new int[3];
            int[] y = new int[3];
            
            //Main Page
            x[0] = 809; y[0] = 149;
            x[1] = 866; y[1] = 145;
            if (Compare(pic, MainPage, x, y, 1)) return PageNo.Main_Page;

            //Repair Page
            x[0] = 94; y[0] = 162;
            x[1] = 436; y[1] = 163;
            x[2] = 591; y[2] = 287;
            if (Compare(pic, RepairPage, x, y, 2)) return PageNo.Repair_Page;

            //Big Map
            x[0] = 128; y[0] = 641;
            x[1] = 231; y[1] = 292;
            if (Compare(pic, BigMap, x, y, 1)) return PageNo.Select_BigMap_Page;
            
            //BattlePage
            x[0] = 104; y[0] = 284;
            x[1] = 931; y[1] = 190;
            x[2] = 929; y[2] = 543;
            if (Compare(pic, BattlePage, x, y, 2)) return PageNo.Battle_Page;

            //WinPage
            x[0] = 193; y[0] = 57;
            x[1] = 280; y[1] = 636;
            x[2] = 237; y[2] = 145;
            if (Compare(pic, WinPage, x, y, 2)) return PageNo.Win_Page;
            
            //LoadingPage
            x[0] = 75; y[0] = 379;
            x[1] = 194; y[1] = 187;
            x[2] = 839; y[2] = 511;
            if (Compare(pic, LoadingPage, x, y, 2)) return PageNo.Loading_Page;

            /*
            //ChooseMap
            x[0] = 489; y[0] = 364;
            x[1] = 77; y[1] = 410;
            x[2] = 152; y[2] = 650;
            if (Compare(pic, ChooseMap, x, y, 2)) return 4;

            //GoPage
            x[0] = 673; y[0] = 402;
            x[1] = 73; y[1] = 440;
            x[2] = 798; y[2] = 207;
            if (Compare(pic, GoPage, x, y, 2)) return 5;
              
            //Scroll1 Page
            x[0] = 787; y[0] = 364;
            x[1] = 12; y[1] = 365;
            x[2] = 304; y[2] = 619;
            if (Compare(pic, ScrollPage1, x, y, 2)) return 1;

            //Scroll2 Page
            if (Compare(pic, ScrollPage2, x, y, 2)) return 1;

            //Scroll3 Page
            if (Compare(pic, ScrollPage3, x, y, 2)) return 1;

            //Scroll4 Page
            if (Compare(pic, ScrollPage4, x, y, 2)) return 1;

             */
            //Repairing Page
            x[0] = 861; y[0] = 399;
            if (Compare(pic, RepairPage, x, y, 0))
            {
                x[0] = 466; y[0] = 483;
                if (Compare(pic, RepairPage, x, y, 0))
                {
                    x[0] = 94; y[0] = 162;
                    x[1] = 436; y[1] = 163;
                    x[2] = 126; y[2] = 333;
                    if (!Compare(pic, RepairPage, x, y,2 )) return PageNo.Repairing_Page;
                }
                
            }

            return PageNo.Unknow_Page;
        }

        private bool Compare(Bitmap a, Bitmap b, int[] x, int[] y,int t)
        {
            for (int i = 0; i <= t; i++)
            {
                //if (GetPoint(a, LocationX + x[i], LocationY + y[i])!=GetPoint(b, x[i], y[i])) return false;
                if (!Compare_Color(GetPoint(a, LocationX + x[i], LocationY + y[i]), GetPoint(b, x[i], y[i]))) return false;
            }
            return true;
        }

        private bool Compare_Color(Color a, Color b)
        {
            if ((Abs((a.R - b.R)) <= 0.1) && (Abs((a.G - b.G)) <= 0.1) && (Abs((a.B - b.B)) <= 0.1)) return true;
            else return false;
        }

        public float Abs(float i)
        {
            if (i < 0L)
                return i * (-1.0f);
            return i;
        }
    }
}
