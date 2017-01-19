﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CharaSimResource;
using WzComparerR2.CharaSim;
using TR = System.Windows.Forms.TextRenderer;
using TextFormatFlags = System.Windows.Forms.TextFormatFlags;

namespace WzComparerR2.CharaSimControl
{
    /// <summary>
    /// 提供一系列的静态Graphics工具，用来绘制物品tooltip。
    /// </summary>
    public static class GearGraphics
    {
        static GearGraphics()
        {
            TBrushes = new Dictionary<string, TextureBrush>();
            TBrushes["n"] = new TextureBrush(Resource.UIToolTip_img_Item_Frame2_n, WrapMode.Tile);
            TBrushes["ne"] = new TextureBrush(Resource.UIToolTip_img_Item_Frame2_ne, WrapMode.Clamp);
            TBrushes["e"] = new TextureBrush(Resource.UIToolTip_img_Item_Frame2_e, WrapMode.Tile);
            TBrushes["se"] = new TextureBrush(Resource.UIToolTip_img_Item_Frame2_se, WrapMode.Clamp);
            TBrushes["s"] = new TextureBrush(Resource.UIToolTip_img_Item_Frame2_s, WrapMode.Tile);
            TBrushes["sw"] = new TextureBrush(Resource.UIToolTip_img_Item_Frame2_sw, WrapMode.Clamp);
            TBrushes["w"] = new TextureBrush(Resource.UIToolTip_img_Item_Frame2_w, WrapMode.Tile);
            TBrushes["nw"] = new TextureBrush(Resource.UIToolTip_img_Item_Frame2_nw, WrapMode.Clamp);
            TBrushes["c"] = new TextureBrush(Resource.UIToolTip_img_Item_Frame2_c, WrapMode.Tile);
            SetFontFamily("돋움");
        }

        public static readonly Dictionary<string, TextureBrush> TBrushes;
        public static readonly Font ItemNameFont = new Font("돋움", 14f, FontStyle.Bold, GraphicsUnit.Pixel);
        public static readonly Font ItemDetailFont = new Font("돋움", 12f, GraphicsUnit.Pixel);
        public static readonly Font EquipDetailFont = new Font("돋움", 11f, GraphicsUnit.Pixel);
        public static readonly Font EpicGearDetailFont = new Font("돋움", 11f, GraphicsUnit.Pixel);
        public static readonly Font TahomaFont = new Font("Tahoma", 12f, GraphicsUnit.Pixel);
        public static readonly Font SetItemPropFont = new Font("돋움", 11f, GraphicsUnit.Pixel);
        public static readonly Font ItemReqLevelFont = new Font("돋움", 11f, GraphicsUnit.Pixel);

        public static Font ItemNameFont2 { get; private set; }
        public static Font ItemDetailFont2 { get; private set; }
        public static Font EquipDetailFont2 { get; private set; }

        public static void SetFontFamily(string fontName)
        {
            if (ItemNameFont2 != null)
            {
                ItemNameFont2.Dispose();
                ItemNameFont2 = null;
            }
            ItemNameFont2 = new Font(fontName, 14f, FontStyle.Bold, GraphicsUnit.Pixel);

            if (ItemDetailFont2 != null)
            {
                ItemDetailFont2.Dispose();
                ItemDetailFont2 = null;
            }
            ItemDetailFont2 = new Font(fontName, 12f, GraphicsUnit.Pixel);

            if (EquipDetailFont2 != null)
            {
                EquipDetailFont2.Dispose();
                EquipDetailFont2 = null;
            }
            EquipDetailFont2 = new Font(fontName, 11f, GraphicsUnit.Pixel);
        }

        public static readonly Color GearBackColor = Color.FromArgb(204, 0, 51, 85);
        public static readonly Color EpicGearBackColor = Color.FromArgb(170, 68, 0, 0);
        public static readonly Color GearIconBackColor = Color.FromArgb(238, 187, 204, 221);
        public static readonly Color EpicGearIconBackColor = Color.FromArgb(221, 204, 187, 187);

        public static readonly Brush GearBackBrush = new SolidBrush(GearBackColor);
        public static readonly Brush EpicGearBackBrush = new SolidBrush(EpicGearBackColor);
        public static readonly Pen GearBackPen = new Pen(GearBackColor);
        public static readonly Pen EpicGearBackPen = new Pen(EpicGearBackColor);
        public static readonly Brush GearIconBackBrush = new SolidBrush(GearIconBackColor);
        public static readonly Brush GearIconBackBrush2 = new SolidBrush(Color.FromArgb(187, 238, 238, 238));
        public static readonly Brush EpicGearIconBackBrush = new SolidBrush(EpicGearIconBackColor);
        public static readonly Brush StatDetailGrayBrush = new SolidBrush(Color.FromArgb(85, 85, 85));

        public static readonly Color OrangeBrushColor = Color.FromArgb(255, 153, 0);
        /// <summary>
        /// 表示物品说明中带有#c标识的橙色字体画刷。
        /// </summary>
        public static readonly Brush OrangeBrush = new SolidBrush(OrangeBrushColor);
        /// <summary>
        /// 表示物品附加属性中橙色字体画刷。
        /// </summary>
        public static readonly Brush OrangeBrush2 = new SolidBrush(Color.FromArgb(255, 170, 0));
        /// <summary>
        /// 表示装备职业额外说明中使用的橙黄色画刷。
        /// </summary>
        public static readonly Brush OrangeBrush3 = new SolidBrush(Color.FromArgb(255, 204, 0));
        /// <summary>
        /// 表示装备属性额外说明中使用的绿色画刷。
        /// </summary>
        public static readonly Brush GreenBrush2 = new SolidBrush(Color.FromArgb(204, 255, 0));
        /// <summary>
        /// 表示用于绘制“攻击力提升”文字的灰色画刷。
        /// </summary>
        public static readonly Brush GrayBrush2 = new SolidBrush(Color.FromArgb(153, 153, 153));
        /// <summary>
        /// 表示套装名字的绿色画刷。
        /// </summary>
        public static readonly Brush SetItemNameBrush = new SolidBrush(Color.FromArgb(119, 255, 0));
        /// <summary>
        /// 表示套装属性不可用的灰色画刷。
        /// </summary>
        public static readonly Brush SetItemGrayBrush = new SolidBrush(Color.FromArgb(119, 136, 153));
        /// <summary>
        /// 表示装备tooltip中金锤子描述文字的颜色画刷。
        /// </summary>
        public static readonly Brush GoldHammerBrush = new SolidBrush(Color.FromArgb(255, 238, 204));
        /// <summary>
        /// 表示灰色品质的装备名字画刷，额外属性小于0。
        /// </summary>
        public static readonly Brush GearNameBrushA = new SolidBrush(Color.FromArgb(187, 187, 187));
        /// <summary>
        /// 表示白色品质的装备名字画刷，额外属性为0~5。
        /// </summary>
        public static readonly Brush GearNameBrushB = new SolidBrush(Color.FromArgb(255, 255, 255));
        /// <summary>
        /// 表示橙色品质的装备名字画刷，额外属性为0~5，并且已经附加卷轴。
        /// </summary>
        public static readonly Brush GearNameBrushC = new SolidBrush(Color.FromArgb(255, 136, 17));
        private static Color gearBlueColor = Color.FromArgb(102, 255, 255);
        /// <summary>
        /// 表示蓝色品质的装备名字画刷，额外属性为6~22。
        /// </summary>
        public static readonly Brush GearNameBrushD = new SolidBrush(gearBlueColor);
        private static Color gearPurpleColor = Color.FromArgb(153, 102, 255);
        /// <summary>
        /// 表示紫色品质的装备名字画刷，额外属性为23~39。
        /// </summary>
        public static readonly Brush GearNameBrushE = new SolidBrush(gearPurpleColor);
        private static Color gearGoldColor = Color.FromArgb(255, 204, 0);
        /// <summary>
        /// 表示金色品质的装备名字画刷，额外属性为40~54。
        /// </summary>
        public static readonly Brush GearNameBrushF = new SolidBrush(gearGoldColor);
        private static Color gearGreenColor = Color.FromArgb(204, 255, 0);
        /// <summary>
        /// 表示绿色品质的装备名字画刷，额外属性为55~69。
        /// </summary>
        public static readonly Brush GearNameBrushG = new SolidBrush(gearGreenColor);
        /// <summary>
        /// 表示红色品质的装备名字画刷，额外属性为70以上。
        /// </summary>
        public static readonly Brush GearNameBrushH = new SolidBrush(Color.FromArgb(255, 0, 119));
        public static readonly Brush MasterLabelBrush = new SolidBrush(Color.FromArgb(0, 204, 255));

        public static Brush GetGearNameBrush(int diff, bool up, bool petEquip = false)
        {
            if (diff < 0)
                return GearNameBrushA;
            if (diff < 6 || petEquip)
            {
                if (!up)
                    return GearNameBrushB;
                else
                    return GearNameBrushC;
            }
            if (diff < 23)
                return GearNameBrushD;
            if (diff < 40)
                return GearNameBrushE;
            if (diff < 55)
                return GearNameBrushF;
            if (diff < 70)
                return GearNameBrushG;
            return GearNameBrushH;
        }

        public static readonly Pen GearItemBorderPenC = new Pen(Color.FromArgb(255, 0, 102));
        public static readonly Pen GearItemBorderPenB = new Pen(gearBlueColor);
        public static readonly Pen GearItemBorderPenA = new Pen(gearPurpleColor);
        public static readonly Pen GearItemBorderPenS = new Pen(gearGoldColor);
        public static readonly Pen GearItemBorderPenSS = new Pen(gearGreenColor);
        public static Pen GetGearItemBorderPen(GearGrade grade)
        {
            switch (grade)
            {
                case GearGrade.B:
                    return GearItemBorderPenB;
                case GearGrade.A:
                    return GearItemBorderPenA;
                case GearGrade.S:
                    return GearItemBorderPenS;
                case GearGrade.SS:
                    return GearItemBorderPenSS;
                default:
                    return null;
            }
        }

        public static Brush GetPotentialTextBrush(GearGrade grade)
        {
            switch (grade)
            {
                case GearGrade.B: return GearNameBrushD;
                case GearGrade.A: return GearNameBrushE;
                case GearGrade.S: return GearNameBrushF;
                case GearGrade.SS: return GreenBrush2;
            }
            return null;
        }

        /// <summary>
        /// 在指定区域绘制包含宏代码的字符串。
        /// </summary>
        /// <param Name="g">绘图所关联的graphics。</param>
        /// <param Name="s">要绘制的string。</param>
        /// <param Name="font">要绘制string的字体。</param>
        /// <param Name="x">起始的x坐标。</param>
        /// <param Name="X1">每行终止的x坐标。</param>
        /// <param Name="y">起始行的y坐标。</param>
        public static void DrawString(Graphics g, string s, Font font, int x, int x1, ref int y, int height, Color? defaultColor = null, Color? orangeColor = null)
        {
            if (s == null)
                return;

            using (var r = new FormattedTextRenderer())
            {
                r.WordWrapEnabled = false;
                r.UseGDIRenderer = true;
                r.DrawString(g, s, font, x, x1, ref y, height, defaultColor, orangeColor);
            }
        }

        public static Bitmap EnlargeBitmap(Bitmap bitmap)
        {
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            IntPtr p = data.Scan0;
            byte[] origin = new byte[bitmap.Width * bitmap.Height * 4];
            Marshal.Copy(p, origin, 0, origin.Length);
            bitmap.UnlockBits(data);
            byte[] newByte = new byte[origin.Length * 4];
            byte[] buffer = new byte[4];
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    Array.Copy(origin, getOffset(j, i, bitmap.Width), buffer, 0, 4);
                    Array.Copy(buffer, 0, newByte, getOffset(2 * j, 2 * i, bitmap.Width * 2), 4);
                    Array.Copy(buffer, 0, newByte, getOffset(2 * j + 1, 2 * i, bitmap.Width * 2), 4);
                }
                Array.Copy(newByte, getOffset(0, 2 * i, bitmap.Width * 2), newByte, getOffset(0, 2 * i + 1, bitmap.Width * 2), bitmap.Width * 8);
            }
            Bitmap newBitmap = new Bitmap(bitmap.Width * 2, bitmap.Height * 2);
            data = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(newByte, 0, data.Scan0, newByte.Length);
            newBitmap.UnlockBits(data);
            return newBitmap;
        }

        private static int getOffset(int x, int y, int width, int unit = 4)
        {
            return (y * width + x) * unit;
        }

        public static Point[] GetBorderPath(int dx, int width, int height)
        {
            List<Point> pointList = new List<Point>(13);
            pointList.Add(new Point(dx + 1, 0));
            pointList.Add(new Point(dx + 1, 1));
            pointList.Add(new Point(dx + 0, 1));
            pointList.Add(new Point(dx + 0, height - 2));
            pointList.Add(new Point(dx + 1, height - 2));
            pointList.Add(new Point(dx + 1, height - 1));
            pointList.Add(new Point(dx + width - 2, height - 1));
            pointList.Add(new Point(dx + width - 2, height - 2));
            pointList.Add(new Point(dx + width - 1, height - 2));
            pointList.Add(new Point(dx + width - 1, 1));
            pointList.Add(new Point(dx + width - 2, 1));
            pointList.Add(new Point(dx + width - 2, 0));
            pointList.Add(new Point(dx + 1, 0));
            return pointList.ToArray();
        }

        public static Point[] GetIconBorderPath(int x, int y)
        {
            Point[] pointList = new Point[5];
            pointList[0] = new Point(x + 32, y + 31);
            pointList[1] = new Point(x + 32, y);
            pointList[2] = new Point(x, y);
            pointList[3] = new Point(x, y + 32);
            pointList[4] = new Point(x + 31, y + 32);
            return pointList;
        }

        public static void DrawGearDetailNumber(Graphics g, int x, int y, string num, bool can)
        {
            Bitmap bitmap;
            for (int i = 0; i < num.Length; i++)
            {
                switch (num[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        string resourceName = (can ? "ToolTip_Equip_Can_" : "ToolTip_Equip_Cannot_") + num[i];
                        bitmap = (Bitmap)Resource.ResourceManager.GetObject(resourceName);
                        g.DrawImage(bitmap, x, y);
                        x += bitmap.Width + 1;
                        break;
                    case '-':
                        bitmap = can ? Resource.ToolTip_Equip_Can_none : Resource.ToolTip_Equip_Cannot_none;
                        g.DrawImage(bitmap, x, y + 3);
                        x += bitmap.Width + 1;
                        break;
                    case '%':
                        bitmap = can ? Resource.ToolTip_Equip_Can_percent : Resource.ToolTip_Equip_Cannot_percent;
                        g.DrawImage(bitmap, x + 1, y);
                        x += bitmap.Width + 2;
                        break;
                }
            }
        }

        public static void DrawGearGrowthNumber(Graphics g, int x, int y, string num, bool can)
        {
            Bitmap bitmap;
            for (int i = 0; i < num.Length; i++)
            {
                switch (num[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        string resourceName = (can ? "ToolTip_Equip_GrowthEnabled_" : "ToolTip_Equip_Cannot_") + num[i];
                        bitmap = (Bitmap)Resource.ResourceManager.GetObject(resourceName);
                        g.DrawImage(bitmap, x, y);
                        x += bitmap.Width + 1;
                        break;
                    case '-':
                        bitmap = can ? Resource.ToolTip_Equip_GrowthDisabled_none : Resource.ToolTip_Equip_GrowthDisabled_none;
                        g.DrawImage(bitmap, x, y);
                        x += bitmap.Width + 1;
                        break;
                    case '%':
                        bitmap = can ? Resource.ToolTip_Equip_GrowthEnabled_percent : Resource.ToolTip_Equip_GrowthEnabled_percent;
                        g.DrawImage(bitmap, x + 7, y - 4);
                        x += bitmap.Width + 1;
                        break;
                    case 'm':
                        bitmap = can ? Resource.ToolTip_Equip_GrowthEnabled_max : Resource.ToolTip_Equip_GrowthEnabled_max;
                        g.DrawImage(bitmap, x, y);
                        x += bitmap.Width + 1;
                        break;
                }
            }
        }

        public static void DrawNewTooltipBack(Graphics g, int x, int y, int width, int height)
        {
            Dictionary<string, TextureBrush> res = TBrushes;
            //测算准线
            int[] guideX = new int[4] { 0, res["w"].Image.Width, width - res["e"].Image.Width, width };
            int[] guideY = new int[4] { 0, res["n"].Image.Height, height - res["s"].Image.Height, height };
            for (int i = 0; i < guideX.Length; i++) guideX[i] += x;
            for (int i = 0; i < guideY.Length; i++) guideY[i] += y;
            //绘制四角
            FillRect(g, res["nw"], guideX, guideY, 0, 0, 1, 1);
            FillRect(g, res["ne"], guideX, guideY, 2, 0, 3, 1);
            FillRect(g, res["sw"], guideX, guideY, 0, 2, 1, 3);
            FillRect(g, res["se"], guideX, guideY, 2, 2, 3, 3);
            //填充上下区域
            if (guideX[2] > guideX[1])
            {
                FillRect(g, res["n"], guideX, guideY, 1, 0, 2, 1);
                FillRect(g, res["s"], guideX, guideY, 1, 2, 2, 3);
            }
            //填充左右区域
            if (guideY[2] > guideY[1])
            {
                FillRect(g, res["w"], guideX, guideY, 0, 1, 1, 2);
                FillRect(g, res["e"], guideX, guideY, 2, 1, 3, 2);
            }
            //填充中心
            if (guideX[2] > guideX[1] && guideY[2] > guideY[1])
            {
                FillRect(g, res["c"], guideX, guideY, 1, 1, 2, 2);
            }
        }

        private static void FillRect(Graphics g, TextureBrush brush, int[] guideX, int[] guideY, int x0, int y0, int x1, int y1)
        {
            brush.ResetTransform();
            brush.TranslateTransform(guideX[x0], guideY[y0]);
            g.FillRectangle(brush, guideX[x0], guideY[y0], guideX[x1] - guideX[x0], guideY[y1] - guideY[y0]);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hwnd, UInt32 wMsg, IntPtr wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0xB;

        public static void SetRedraw(System.Windows.Forms.Control control, bool enable)
        {
            if (control != null)
            {
                SendMessage(control.Handle, WM_SETREDRAW, new IntPtr(enable ? 1 : 0), IntPtr.Zero);
            }
        }

        private class FormattedTextRenderer : IDisposable
        {
            public FormattedTextRenderer()
            {
                fmt = (StringFormat)StringFormat.GenericTypographic.Clone();
                sb = new StringBuilder();
            }

            public bool WordWrapEnabled { get; set; }
            public bool UseGDIRenderer { get; set; }

            const int MAX_RANGES = 32;
            StringFormat fmt;

            Graphics g;
            StringBuilder sb;
            Font font;
            RectangleF infinityRect;

            public void DrawString(Graphics g, string s, Font font, int x, int x1, ref int y, int height, Color? defaultColor = null, Color? orangeColor = null)
            {
                //初始化环境
                this.g = g;
                this.font = font;
                this.sb.Clear();
                this.sb.EnsureCapacity(s.Length);

                float fontLineHeight = GetFontLineHeight(font);
                this.infinityRect = new RectangleF(0, 0, ushort.MaxValue, fontLineHeight);

                //读取格式
                var runs = ParseFormat(s, defaultColor, orangeColor);

                //拆分成词
                runs = runs.SelectMany(run => SplitWords(run)).ToList();

                //对词进行measure
                MeasureRuns(runs);

                //直接绘制
                DrawRuns(runs, x, x1, ref y, height);
            }

            private List<Run> ParseFormat(string format, Color? defaultColor = null, Color? orangeColor = null)
            {
                List<Run> runs = new List<Run>();

                Stack<Color> colorStack = new Stack<Color>();
                colorStack.Push(defaultColor ?? Color.White);

                int strPos = 0;
                char curChar;

                int offset = 0;

                Action flushRun = () =>
                {
                    if (sb.Length > offset)
                    {
                        runs.Add(new Run(offset, sb.Length - offset) { ForeColor = colorStack.Peek() });
                        offset = sb.Length;
                    }
                };

                while (strPos < format.Length)
                {
                    curChar = format[strPos++];
                    if (curChar == '\\')
                    {
                        if (strPos < format.Length)
                        {
                            curChar = format[strPos++];
                            switch (curChar)
                            {
                                case 'r': curChar = '\r'; break;
                                case 'n':
                                    if (strPos <= 2)
                                        curChar = ' ';//替换文本第一个\n
                                    else
                                        curChar = '\n';
                                    break;
                            }
                        }
                        else //结束符处理
                        {
                            curChar = '#';
                        }
                    }

                    switch (curChar)
                    {
                        case '#':
                            if (strPos < format.Length && format[strPos] == 'c')//遇到#c 换橙刷子并flush
                            {
                                flushRun();
                                colorStack.Push(orangeColor ?? OrangeBrushColor);
                                strPos++;
                            }
                            else if (strPos < format.Length && format[strPos] == 'g')//遇到#c 换绿刷子并flush
                            {
                                flushRun();
                                colorStack.Push(GearGraphics.gearGreenColor);
                                strPos++;
                            }
                            else if (colorStack.Count == 1) //同#c
                            {
                                flushRun();
                                colorStack.Push(orangeColor ?? OrangeBrushColor);
                                //strPos++;
                            }
                            else//遇到# 换白刷子并flush
                            {
                                flushRun();
                                colorStack.Pop();
                            }
                            break;

                        case '\r': //忽略
                            break;

                        case '\n': //插入换行
                            flushRun();
                            runs.Add(new Run(offset, 0) { IsBreakLine = true });
                            break;

                        default:
                            sb.Append(curChar);
                            break;
                    }
                }

                flushRun();
                return runs;
            }

            private List<Run> SplitWords(Run run)
            {
                List<Run> runs = new List<Run>();

                if (run.IsBreakLine)
                {
                    runs.Add(run);
                }
                else
                {
                    for (int i = run.StartIndex, i0 = run.StartIndex + run.Length; i < i0; i++)
                    {
                        int start = i, len;
                        switch (sb[i])
                        {
                            case ' ':
                            case '\t':
                                while (++i < i0)
                                {
                                    if (!(sb[i] == ' ' || sb[i] == '\t'))
                                    {
                                        break;
                                    }
                                }
                                len = (i--) - start;
                                runs.Add(new Run(start, len) { IsWhiteSpace = true });
                                break;

                            case '\r':
                                if (i + 1 < i0 && sb[i + 1] == '\n')
                                {
                                    i++;
                                    goto case '\n';
                                }
                                else
                                {
                                    runs.Add(new Run(start, 1) { IsWhiteSpace = true });
                                }
                                break;

                            case '\n':
                                len = i - start + 1;
                                runs.Add(new Run(start, len) { IsBreakLine = true });
                                break;

                            default:
                                if (this.WordWrapEnabled)
                                {
                                    while (++i < i0)
                                    {
                                        if (sb[i] == ' ' || sb[i] == '\t' || sb[i] == '\r' || sb[i] == '\n')
                                        {
                                            break;
                                        }
                                    }

                                    len = (i--) - start;
                                    runs.Add(new Run(start, len) { ForeColor = run.ForeColor });
                                }
                                else
                                {
                                    runs.Add(new Run(start, 1) { ForeColor = run.ForeColor });
                                }
                                break;
                        }
                    }
                }
                return runs;
            }

            private float GetFontLineHeight(Font font)
            {
                var ff = font.FontFamily;
                return (float)Math.Ceiling(1.0 * font.Height * ff.GetLineSpacing(font.Style) / ff.GetEmHeight(font.Style));
            }

            private void MeasureRuns(List<Run> runs)
            {
                List<Run> tempRuns = new List<Run>(MAX_RANGES);

                foreach (var run in runs)
                {
                    tempRuns.Add(run);
                    if (tempRuns.Count >= MAX_RANGES)
                    {
                        MeasureBatch(tempRuns);
                        tempRuns.Clear();
                    }
                }

                MeasureBatch(tempRuns);

                //failed
                if (runs.Where(run => !run.IsBreakLine && run.Length > 0)
                    .All(run => run.Width == 0))
                {
                    float x = 0;
                    foreach (var run in runs.Where(r => !r.IsBreakLine))
                    {
                        run.X = (int)Math.Round(x);
                        float width = 0;
                        for (int i = 0; i < run.Length; i++)
                        {
                            var chr = this.sb[run.StartIndex + i];
                            width += chr > 0xff ? this.font.Size : (this.font.Size / 2);
                        }
                        run.Width = (int)Math.Round(x);
                        x += width;
                    }
                }
            }

            private void MeasureBatch(List<Run> runs)
            {
                string text = sb.ToString();
                if (runs.Count > 0)
                {
                    fmt.SetMeasurableCharacterRanges(runs.Select(r => new CharacterRange(r.StartIndex, r.Length)).ToArray());
                    var regions = g.MeasureCharacterRanges(text, font, infinityRect, fmt);
                    for (int i = 0; i < runs.Count; i++)
                    {
                        var layout = new RectangleF();
                        if (this.UseGDIRenderer)
                            layout = new RectangleF(new Point(TR.MeasureText(g, text.Substring(0, runs[i].StartIndex), font, Size.Round(infinityRect.Size), TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix).Width, 0), TR.MeasureText(g, text.Substring(runs[i].StartIndex, runs[i].Length), font, Size.Round(infinityRect.Size), TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix));
                        else
                            layout = regions[i].GetBounds(g);
                        runs[i].X = (int)Math.Round(layout.Left);
                        runs[i].Width = (int)Math.Round(layout.Width);
                        regions[i].Dispose();
                    }
                }
            }

            private Rectangle[] MeasureChars(int startIndex, int length)
            {
                string word = sb.ToString(startIndex, length);
                Rectangle[] rects = new Rectangle[length];

                for (int i = 0; i < length; i += MAX_RANGES)
                { //批次
                    int chrCount = Math.Min(length - i, MAX_RANGES);
                    fmt.SetMeasurableCharacterRanges(
                        Enumerable.Range(i, chrCount)
                        .Select(start => new CharacterRange(start, 1))
                        .ToArray());
                    var regions = g.MeasureCharacterRanges(word, font, infinityRect, fmt);
                    for (int i1 = 0; i1 < regions.Length; i1++)
                    {
                        var rect = new RectangleF();
                        if (this.UseGDIRenderer)
                            rect = new RectangleF(new Point(0, 0), TR.MeasureText(g, "" + word[i1], font, Size.Round(infinityRect.Size), TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix));
                        else
                            rect = regions[i1].GetBounds(g);
                        rects[i + i1] = new Rectangle(
                            (int)Math.Round(rect.Left),
                            (int)Math.Round(rect.Top),
                            (int)Math.Round(rect.Width),
                            (int)Math.Round(rect.Height)
                            );
                    }
                }

                //failed
                if (rects.All(rect => rect.Width == 0))
                {
                    float x = 0;
                    for (int i = 0; i < rects.Length; i++)
                    {
                        var chr = this.sb[startIndex + i];
                        var width = chr > 0xff ? this.font.Size : (this.font.Size / 2);
                        rects[i] = new Rectangle(
                            (int)Math.Round(x),
                            0,
                            (int)Math.Round(width),
                            font.Height
                            );
                    }
                }

                return rects;
            }

            private void DrawRuns(List<Run> runs, int x, int x1, ref int y, int lineHeight)
            {
                int drawX = x;
                int drawY = y;
                int width = x1 - x;
                int start = -1, end = -1;
                int xOffset = 0;

                int curX = drawX;

                Func<bool> hasContent = () => start > -1 && end > start;
                Color color = Color.Transparent;

                Action<bool> flush = (isNewLine) =>
                {
                    if (hasContent())
                    {
                        string content = sb.ToString(start, end - start);

                         if (this.UseGDIRenderer)
                        {
                            TR.DrawText(g, content, font, new Point(drawX, drawY), color, TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
                        }
                        else
                        {
                            using (var brush = new SolidBrush(color))
                            {
                                g.DrawString(content, font, brush, drawX, drawY, fmt);
                            }
                        }
                    }
                    if (isNewLine)
                    {
                        drawX = curX = x;
                        drawY += lineHeight;
                    }
                    else
                    {
                        drawX = curX;
                    }
                    start = end = -1;
                };

                for (int r = 0; r < runs.Count; r++)
                {
                    var run = runs[r];
                    if (run.IsBreakLine)
                    { //强行换行 并且flush
                        flush(true);
                        if( r < runs.Count - 1)
                        {
                            xOffset = runs[r + 1].X;
                        }
                    }
                    else
                    {
                        if (!run.IsWhiteSpace && run.ForeColor != color)
                        {
                            end = run.StartIndex;
                            curX = x + run.X - xOffset;
                            flush(false);
                            color = run.ForeColor;
                        }

                        if (start < 0)
                        {
                            start = run.StartIndex;
                        }

                        //if (!run.IsWhiteSpace)
                        { //非空 计算宽度
                            curX = x + run.X - xOffset;
                            if (this.WordWrapEnabled ? (x1 - curX < run.Width) : (curX >= x1))  //奇怪的算法 暂定
                            { //宽度不够
                                if (curX > x) //(hasContent())
                                { //有内容
                                    flush(true);
                                    start = run.StartIndex;
                                    xOffset = run.X;
                                }
                                if (x1 - curX < run.Width)
                                { //宽度还是不够 按字符拆分
                                    var rects = MeasureChars(run.StartIndex, run.Length);

                                    for (int i = 0, ir = run.StartIndex; i < rects.Length; i++, ir++)
                                    {
                                        rects[i].X += run.X;

                                        if (start < 0)
                                        {
                                            start = ir;
                                            xOffset = run.X;
                                        }

                                        if (rects[i].Right - xOffset > width)
                                        { //超宽 flush之前内容
                                            if (ir - start <= 0)
                                            { //限定至少输出一个字符
                                                end = start + 1;
                                                flush(true);
                                                xOffset = rects[i].Right;
                                                continue;
                                            }
                                            else
                                            {
                                                end = ir;
                                                flush(true);
                                                start = ir;
                                                xOffset = rects[i].X;
                                            }
                                        }
                                    }
                                    end = run.StartIndex + run.Length;
                                    curX = x + rects[rects.Length - 1].Right - xOffset;
                                    flush(false);

                                    continue;
                                }
                            }
                        }

                        //正常绘制
                        end = run.StartIndex + run.Length;

                    }
                }

                //输出结尾
                flush(true);
                y = drawY;
            }

            public void Dispose()
            {
                if (fmt != null)
                    fmt.Dispose();
            }
        }

        private class Run
        {
            public Run(int startIndex, int length)
            {
                this.StartIndex = startIndex;
                this.Length = length;
            }

            public int StartIndex;
            public int Length;
            public bool IsWhiteSpace;
            public bool IsBreakLine;
            public int X;
            public int Width;
            public Color ForeColor;
        }
    }
}
