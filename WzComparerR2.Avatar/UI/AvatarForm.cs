using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.Editors;
using DevComponents.DotNetBar.Controls;
using WzComparerR2.CharaSim;
using WzComparerR2.Common;
using WzComparerR2.WzLib;
using WzComparerR2.PluginBase;
using WzComparerR2.Config;
using WzComparerR2.Controls;

namespace WzComparerR2.Avatar.UI
{
    internal partial class AvatarForm : DevComponents.DotNetBar.OfficeForm
    {
        public AvatarForm()
        {
            InitializeComponent();
            this.avatar = new AvatarCanvas();
            this.animator = new Animator();
            btnReset_Click(btnReset, EventArgs.Empty);
            FillWeaponIdx();
            FillEarSelection();
        }

        public SuperTabControlPanel GetTabPanel()
        {
            this.TopLevel = false;
            this.Dock = DockStyle.Fill;
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            var pnl = new SuperTabControlPanel();
            pnl.Controls.Add(this);
            pnl.Padding = new System.Windows.Forms.Padding(1);
            this.Visible = true;
            return pnl;
        }

        public Entry PluginEntry { get; set; }

        AvatarCanvas avatar;
        bool inited;
        string partsTag;
        bool suspendUpdate;
        bool needUpdate;
        Animator animator;

        /// <summary>
        /// wz1节点选中事件。
        /// </summary>
        public void OnSelectedNode1Changed(object sender, WzNodeEventArgs e)
        {
            if (PluginEntry.Context.SelectedTab != PluginEntry.Tab || e.Node == null
                || this.btnLock.Checked)
            {
                return;
            }

            Wz_File file = e.Node.GetNodeWzFile();
            if (file == null)
            {
                return;
            }

            switch (file.Type)
            {
                case Wz_Type.Character: //读取装备
                    Wz_Image wzImg = e.Node.GetValue<Wz_Image>();
                    if (wzImg != null && wzImg.TryExtract())
                    {
                        this.SuspendUpdateDisplay();
                        LoadPart(wzImg.Node);
                        this.ResumeUpdateDisplay();
                    }
                    break;
            }
        }

        /// <summary>
        /// wz2节点选中事件。
        /// </summary>
        public void OnSelectedNode2Changed(object sender, WzNodeEventArgs e)
        {
            if (PluginEntry.Context.SelectedTab != PluginEntry.Tab || e.Node == null
                || this.btnLock.Checked)
            {
                return;
            }

            Wz_File file = e.Node.GetNodeWzFile();
            if (file == null)
            {
                return;
            }

            switch (file.Type)
            {
                case Wz_Type.Skill:
                    Wz_Node skillNode = e.Node;
                    if (Int32.TryParse(skillNode.Text, out int skillID))
                    {
                        int tamingMobID = skillNode.Nodes["vehicleID"].GetValueEx<int>(0);
                        if (tamingMobID == 0)
                        {
                            tamingMobID = PluginBase.PluginManager.FindWz(string.Format(@"Skill\RidingSkillInfo.img\{0:D7}\vehicleID", skillID)).GetValueEx<int>(0);
                        }
                        if (tamingMobID != 0)
                        {
                            var tamingMobNode = PluginBase.PluginManager.FindWz(string.Format(@"Character\TamingMob\{0:D8}.img", tamingMobID));
                            if (tamingMobNode != null)
                            {
                                this.SuspendUpdateDisplay();
                                LoadTamingPart(tamingMobNode, BitmapOrigin.CreateFromNode(skillNode.Nodes["icon"], PluginBase.PluginManager.FindWz) , skillID, true);
                                this.ResumeUpdateDisplay();
                            }
                        }
                    }
                    break;

                case Wz_Type.Item:
                    Wz_Node itemNode = e.Node;
                    if (Int32.TryParse(itemNode.Text, out int itemID))
                    {
                        int tamingMobID = itemNode.FindNodeByPath("info\\tamingMob").GetValueEx<int>(0);
                        if (tamingMobID != 0)
                        {
                            var tamingMobNode = PluginBase.PluginManager.FindWz(string.Format(@"Character\TamingMob\{0:D8}.img", tamingMobID));
                            if (tamingMobNode != null)
                            {
                                this.SuspendUpdateDisplay();
                                LoadTamingPart(tamingMobNode, BitmapOrigin.CreateFromNode(itemNode.FindNodeByPath("info\\icon"), PluginBase.PluginManager.FindWz), itemID, false);
                                this.ResumeUpdateDisplay();
                            }
                        }
                        else // 의자 아이템, 아이템 코드나 bodyRelMove과 sitAction 속성 유무로 결정
                        {
                            Wz_Vector brm = itemNode.FindNodeByPath("info\\bodyRelMove").GetValueEx<Wz_Vector>(null);
                            bool isSitActionExists = itemNode.FindNodeByPath("info\\sitAction").GetValueEx<string>(null) != null;
                            if (itemID / 10000 == 301 || itemID / 1000 == 5204 || brm != null || isSitActionExists)
                            {
                                bool fb = false;
                                if (brm == null)
                                {
                                    fb = false;
                                    brm = new Wz_Vector(0, 0);
                                }
                                else if (isSitActionExists)
                                {
                                    fb = true;
                                }

                                this.SuspendUpdateDisplay();
                                LoadChairPart(itemNode, BitmapOrigin.CreateFromNode(itemNode.FindNodeByPath("info\\icon"), PluginBase.PluginManager.FindWz), itemID, brm, fb);
                                this.ResumeUpdateDisplay();
                            }
                        }
                    }
                    break;
            }
        }

        public void OnWzClosing(object sender, WzStructureEventArgs e)
        {
            bool hasChanged = false;
            for (int i = 0; i < avatar.Parts.Length; i++)
            {
                var part = avatar.Parts[i];
                if (part != null)
                {
                    var wzFile = part.Node.GetNodeWzFile();
                    if (wzFile != null && e.WzStructure.wz_files.Contains(wzFile))//将要关闭文件 移除
                    {
                        avatar.Parts[i] = null;
                        hasChanged = true;
                    }
                }
            }

            if (hasChanged)
            {
                this.FillAvatarParts();
                UpdateDisplay();
            }
        }

        /// <summary>
        /// 初始化纸娃娃资源。
        /// </summary>
        private bool AvatarInit()
        {
            this.inited = this.avatar.LoadZ()
                && this.avatar.LoadActions()
                && this.avatar.LoadEmotions();

            if (this.inited)
            {
                this.FillBodyAction();
                this.FillEmotion();
            }
            return this.inited;
        }

        /// <summary>
        /// 加载装备部件。
        /// </summary>
        /// <param name="imgNode"></param>
        private void LoadPart(Wz_Node imgNode)
        {
            if (!this.inited && !this.AvatarInit() && imgNode == null)
            {
                return;
            }

            AvatarPart part = this.avatar.AddPart(imgNode);
            if (part != null)
            {
                OnNewPartAdded(part);
                FillAvatarParts();
                UpdateDisplay();
            }
        }

        private void LoadTamingPart(Wz_Node imgNode, BitmapOrigin forceIcon, int forceID, bool isSkill)
        {
            if (!this.inited && !this.AvatarInit() && imgNode == null)
            {
                return;
            }

            AvatarPart part = this.avatar.AddTamingPart(imgNode, forceIcon, forceID, isSkill);
            if (part != null)
            {
                OnNewPartAdded(part);
                FillAvatarParts();
                UpdateDisplay();
            }
        }

        private void LoadChairPart(Wz_Node imgNode, BitmapOrigin forceIcon, int forceID, Wz_Vector brm, bool forceAct) // 의자 아이템 패널 표시
        {
            if (!this.inited && !this.AvatarInit() && imgNode == null)
            {
                return;
            }

            AvatarPart part = this.avatar.AddChairPart(imgNode, forceIcon, forceID, brm, forceAct);
            if (part != null)
            {
                OnNewPartAdded(part);
                FillAvatarParts();
                UpdateDisplay();
            }
        }

        private void OnNewPartAdded(AvatarPart part)
        {
            if (part == null)
            {
                return;
            }

            if (part == avatar.Body) //同步head
            {
                int headID = 10000 + part.ID.Value % 10000;
                if (avatar.Head == null || avatar.Head.ID != headID)
                {
                    var headImgNode = PluginBase.PluginManager.FindWz(string.Format("Character\\{0:D8}.img", headID));
                    if (headImgNode != null)
                    {
                        this.avatar.AddPart(headImgNode);
                    }
                }
            }
            else if (part == avatar.Head) //同步body
            {
                int bodyID = part.ID.Value % 10000;
                if (avatar.Body == null || avatar.Body.ID != bodyID)
                {
                    var bodyImgNode = PluginBase.PluginManager.FindWz(string.Format("Character\\{0:D8}.img", bodyID));
                    if (bodyImgNode != null)
                    {
                        this.avatar.AddPart(bodyImgNode);
                    }
                }
            }
            else if (part == avatar.Face) //同步表情
            {
                this.avatar.LoadEmotions();
                FillEmotion();
            }
            else if (part == avatar.Taming) //同步座驾动作
            {
                if (part.bodyRelMove == null)
                {
                    this.avatar.LoadTamingActions();
                    FillTamingAction();
                    SetTamingDefaultBodyAction();
                    SetTamingDefault();
                }
                else // 의자 아이템의 경우 taming과 분리
                {
                    this.avatar.LoadChairActions();
                    FillTamingAction();
                    SetChairDefaultBodyAction();
                    SetChairDefault();
                }
            }
            else if (part == avatar.Weapon) //同步武器类型
            {
                FillWeaponTypes();
            }
            else if (part == avatar.Pants || part == avatar.Coat) //隐藏套装
            {
                if (avatar.Longcoat != null)
                {
                    avatar.Longcoat.Visible = false;
                }
            }
            else if (part == avatar.Longcoat) //还是。。隐藏套装
            {
                if (avatar.Pants != null && avatar.Pants.Visible
                    || avatar.Coat != null && avatar.Coat.Visible)
                {
                    avatar.Longcoat.Visible = false;
                }
            }
        }

        private void SuspendUpdateDisplay()
        {
            this.suspendUpdate = true;
            this.needUpdate = false;
        }

        private void ResumeUpdateDisplay()
        {
            if (this.suspendUpdate)
            {
                this.suspendUpdate = false;
                if (this.needUpdate)
                {
                    this.UpdateDisplay();
                }
            }
        }

        /// <summary>
        /// 更新画布。
        /// </summary>
        private void UpdateDisplay()
        {
            if (suspendUpdate)
            {
                this.needUpdate = true;
                return;
            }

            string newPartsTag = GetAllPartsTag();
            if (this.partsTag != newPartsTag)
            {
                this.partsTag = newPartsTag;
                this.avatarContainer1.ClearAllCache();
            }

            ComboItem selectedItem;
            //同步角色动作
            selectedItem = this.cmbActionBody.SelectedItem as ComboItem;
            this.avatar.ActionName = selectedItem != null ? selectedItem.Text : null;
            //同步表情
            selectedItem = this.cmbEmotion.SelectedItem as ComboItem;
            this.avatar.EmotionName = selectedItem != null ? selectedItem.Text : null;
            //同步骑宠动作
            selectedItem = this.cmbActionTaming.SelectedItem as ComboItem;
            this.avatar.TamingActionName = selectedItem != null ? selectedItem.Text : null;

            //获取动作帧
            this.GetSelectedBodyFrame(out int bodyFrame, out _);
            this.GetSelectedEmotionFrame(out int emoFrame, out _);
            this.GetSelectedTamingFrame(out int tamingFrame, out _);

            //获取武器状态
            selectedItem = this.cmbWeaponType.SelectedItem as ComboItem;
            this.avatar.WeaponType = selectedItem != null ? Convert.ToInt32(selectedItem.Text) : 0;

            selectedItem = this.cmbWeaponIdx.SelectedItem as ComboItem;
            this.avatar.WeaponIndex = selectedItem != null ? Convert.ToInt32(selectedItem.Text) : 0;

            //获取耳朵状态
            selectedItem = this.cmbEar.SelectedItem as ComboItem;
            this.avatar.EarType = selectedItem != null ? Convert.ToInt32(selectedItem.Text) : 0;

            if (bodyFrame < 0 && emoFrame < 0 && tamingFrame < 0)
            {
                return;
            }

            string actionTag = string.Format("{0}:{1},{2}:{3},{4}:{5},{6},{7},{8},{9},{10}",
                this.avatar.ActionName,
                bodyFrame,
                this.avatar.EmotionName,
                emoFrame,
                this.avatar.TamingActionName,
                tamingFrame,
                this.avatar.HairCover ? 1 : 0,
                this.avatar.ShowHairShade ? 1 : 0,
                this.avatar.EarType,
                this.avatar.WeaponType,
                this.avatar.WeaponIndex);

            if (!avatarContainer1.HasCache(actionTag))
            {
                try
                {
                    var actionFrames = avatar.GetActionFrames(avatar.ActionName);
                    var bone = avatar.CreateFrame(bodyFrame, emoFrame, tamingFrame);
                    var layers = avatar.CreateFrameLayers(bone);
                    avatarContainer1.AddCache(actionTag, layers);
                }
                catch
                {
                }
            }

            avatarContainer1.SetKey(actionTag);
        }

        private string GetAllPartsTag()
        {
            string[] partsID = new string[avatar.Parts.Length];
            for (int i = 0; i < avatar.Parts.Length; i++)
            {
                var part = avatar.Parts[i];
                if (part != null && part.Visible)
                {
                    partsID[i] = (part.IsSkill ? "s" : "") + part.ID.ToString();
                    if (part.IsMixing)
                    {
                        partsID[i] += "+" + part.MixColor + "*" + part.MixOpacity;
                    }
                }
            }
            return string.Join(",", partsID);
        }

        void AddPart(string imgPath)
        {
            Wz_Node imgNode = PluginManager.FindWz(imgPath);
            if (imgNode != null)
            {
                this.avatar.AddPart(imgNode);
            }
        }

        private void SelectBodyAction(string actionName)
        {
            for (int i = 0; i < cmbActionBody.Items.Count; i++)
            {
                ComboItem item = cmbActionBody.Items[i] as ComboItem;
                if (item != null && item.Text == actionName)
                {
                    cmbActionBody.SelectedIndex = i;
                    return;
                }
            }
        }

        private void SelectEmotion(string emotionName)
        {
            for (int i = 0; i < cmbEmotion.Items.Count; i++)
            {
                ComboItem item = cmbEmotion.Items[i] as ComboItem;
                if (item != null && item.Text == emotionName)
                {
                    cmbEmotion.SelectedIndex = i;
                    return;
                }
            }
        }

        private void SelectEmotionByIndex(int emotionIdx)
        {
            cmbEmotion.SelectedIndex = emotionIdx + 1;
        }

        #region 同步界面
        private void FillBodyAction()
        {
            var oldSelection = cmbActionBody.SelectedItem as ComboItem;
            int? newSelection = null;
            cmbActionBody.BeginUpdate();
            cmbActionBody.Items.Clear();
            foreach (var action in this.avatar.Actions)
            {
                ComboItem cmbItem = new ComboItem(action.Name);
                switch (action.Level)
                {
                    case 0:
                        cmbItem.FontStyle = FontStyle.Bold;
                        cmbItem.ForeColor = Color.Indigo;
                        break;

                    case 1:
                        cmbItem.ForeColor = Color.Indigo;
                        break;
                }
                cmbItem.Tag = action;
                cmbActionBody.Items.Add(cmbItem);

                if (newSelection == null && oldSelection != null)
                {
                    if (cmbItem.Text == oldSelection.Text)
                    {
                        newSelection = cmbActionBody.Items.Count - 1;
                    }
                }
            }

            if (cmbActionBody.Items.Count > 0)
            {
                cmbActionBody.SelectedIndex = newSelection ?? 0;
            }

            cmbActionBody.EndUpdate();
        }

        private void FillEmotion()
        {
            FillComboItems(cmbEmotion, avatar.Emotions);
        }

        private void FillTamingAction()
        {
            FillComboItems(cmbActionTaming, avatar.TamingActions);
        }

        private void FillWeaponTypes()
        {
            List<int> weaponTypes = avatar.GetCashWeaponTypes();
            FillComboItems(cmbWeaponType, weaponTypes.ConvertAll(i => i.ToString()));
        }

        private void SetTamingDefaultBodyAction()
        {
            string actionName;
            var tamingAction = (this.cmbActionTaming.SelectedItem as ComboItem)?.Text;
            switch (tamingAction)
            {
                case "ladder":
                case "rope":
                    actionName = tamingAction;
                    break;
                default:
                    actionName = "sit";
                    break;
            }
            SelectBodyAction(actionName);
        }

        private void SetChairDefaultBodyAction() // 의자 아이템의 기본 캐릭터 동작 = sit 또는 sitAction으로 설정된 값
        {
            string actionName = this.avatar.Taming.Node.FindNodeByPath("info\\sitAction").GetValueEx<string>("sit");
            SelectBodyAction(actionName);
        }

        private void SetTamingDefault()
        {
            if (this.avatar.Taming != null)
            {
                var tamingAction =  (this.cmbActionTaming.SelectedItem as ComboItem)?.Text;
                if (tamingAction != null)
                {
                    string forceAction = this.avatar.Taming.Node.FindNodeByPath($@"characterAction\{tamingAction}").GetValueEx<string>(null);
                    if (forceAction != null)
                    {
                        this.SelectBodyAction(forceAction);
                    }

                    string forceEmotion = this.avatar.Taming.Node.FindNodeByPath($@"characterEmotion\{tamingAction}").GetValueEx<string>(null);
                    if (forceEmotion != null)
                    {
                        this.SelectEmotion(forceEmotion);
                    }
                }
            }
        }

        private void SetChairDefault() // 의자 아이템의 기본 캐릭터 동작 = sit 또는 sitAction으로 설정된 값, 기본 표정
        {
            if (this.avatar.Taming != null)
            {
                string forceAction = this.avatar.Taming.Node.FindNodeByPath("info\\sitAction").GetValueEx<string>("sit");
                if (forceAction != null)
                {
                    this.SelectBodyAction(forceAction);
                }

                int forceEmotion = this.avatar.Taming.Node.FindNodeByPath("info\\sitEmotion").GetValueEx<int>(-1);
                this.SelectEmotionByIndex(forceEmotion);
            }
        }

        /// <summary>
        /// 更新当前显示部件列表。
        /// </summary>
        private void FillAvatarParts()
        {
            itemPanel1.BeginUpdate();
            itemPanel1.Items.Clear();
            foreach (var part in avatar.Parts)
            {
                if (part != null)
                {
                    var btn = new AvatarPartButtonItem(part.ID.Value, part.IsMixing ? part.MixColor : (int?)null, part.IsMixing ? part.MixOpacity : (int?)null);
                    this.SetButtonText(part, btn);
                    btn.SetIcon(part.Icon.Bitmap);
                    btn.Tag = part;
                    btn.Checked = part.Visible;
                    btn.btnItemShow.Click += BtnItemShow_Click;
                    btn.btnItemDel.Click += BtnItemDel_Click;
                    btn.CheckedChanged += Btn_CheckedChanged;
                    btn.rdoMixColor0.CheckedChanged += RadioMixColor0_CheckedChanged;
                    btn.rdoMixColor1.CheckedChanged += RadioMixColor1_CheckedChanged;
                    btn.rdoMixColor2.CheckedChanged += RadioMixColor2_CheckedChanged;
                    btn.rdoMixColor3.CheckedChanged += RadioMixColor3_CheckedChanged;
                    btn.rdoMixColor4.CheckedChanged += RadioMixColor4_CheckedChanged;
                    btn.rdoMixColor5.CheckedChanged += RadioMixColor5_CheckedChanged;
                    btn.rdoMixColor6.CheckedChanged += RadioMixColor6_CheckedChanged;
                    btn.rdoMixColor7.CheckedChanged += RadioMixColor7_CheckedChanged;
                    btn.sliderMixRatio.ValueChanged += SliderMixRatio_ValueChanged;
                    itemPanel1.Items.Add(btn);
                }
            }
            itemPanel1.EndUpdate();
        }

        private void BtnItemShow_Click(object sender, EventArgs e)
        {
            var btn = (sender as BaseItem).Parent as AvatarPartButtonItem;
            if (btn != null)
            {
                btn.Checked = !btn.Checked;
            }
        }

        private void BtnItemDel_Click(object sender, EventArgs e)
        {
            var btn = (sender as BaseItem).Parent as AvatarPartButtonItem;
            if (btn != null)
            {
                var part = btn.Tag as AvatarPart;
                if (part != null)
                {
                    int index = Array.IndexOf(this.avatar.Parts, part);
                    if (index > -1)
                    {
                        this.avatar.Parts[index] = null;
                        this.FillAvatarParts();
                        this.UpdateDisplay();
                    }
                }
            }
        }

        private void Btn_CheckedChanged(object sender, EventArgs e)
        {
            var btn = sender as AvatarPartButtonItem;
            if (btn != null)
            {
                var part = btn.Tag as AvatarPart;
                if (part != null)
                {
                    part.Visible = btn.Checked;
                    this.UpdateDisplay();
                }
            }
        }

        private void RadioMixColor0_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBoxItem).Checked)
            {
                RadioMixColor_CheckedChanged(sender, 0);
            }
        }

        private void RadioMixColor1_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBoxItem).Checked)
            {
                RadioMixColor_CheckedChanged(sender, 1);
            }
        }

        private void RadioMixColor2_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBoxItem).Checked)
            {
                RadioMixColor_CheckedChanged(sender, 2);
            }
        }

        private void RadioMixColor3_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBoxItem).Checked)
            {
                RadioMixColor_CheckedChanged(sender, 3);
            }
        }

        private void RadioMixColor4_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBoxItem).Checked)
            {
                RadioMixColor_CheckedChanged(sender, 4);
            }
        }

        private void RadioMixColor5_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBoxItem).Checked)
            {
                RadioMixColor_CheckedChanged(sender, 5);
            }
        }

        private void RadioMixColor6_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBoxItem).Checked)
            {
                RadioMixColor_CheckedChanged(sender, 6);
            }
        }

        private void RadioMixColor7_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBoxItem).Checked)
            {
                RadioMixColor_CheckedChanged(sender, 7);
            }
        }

        private void RadioMixColor_CheckedChanged(object sender, int mixColor)
        {
            var btn = (sender as BaseItem).Parent as AvatarPartButtonItem;
            if (btn != null)
            {
                var part = btn.Tag as AvatarPart;
                if (part != null)
                {
                    part.MixColor = mixColor;
                    this.UpdateDisplay();
                    this.SetButtonText(part, btn);
                }
            }
        }

        private void SliderMixRatio_ValueChanged(object sender, EventArgs e)
        {
            var slider = sender as SliderItem;
            var btn = (sender as BaseItem).Parent as AvatarPartButtonItem;
            if (btn != null)
            {
                var part = btn.Tag as AvatarPart;
                if (part != null)
                {
                    part.MixOpacity = slider.Value;
                    this.UpdateDisplay();
                    this.SetButtonText(part, btn);
                }
            }
        }

        private void SetButtonText(AvatarPart part, AvatarPartButtonItem btn)
        {
            var stringLinker = this.PluginEntry.Context.DefaultStringLinker;
            StringResult sr;
            string text;
            if (part.ID != null && (stringLinker.StringEqp.TryGetValue(part.ID.Value, out sr) || stringLinker.StringSkill.TryGetValue(part.ID.Value, out sr) || stringLinker.StringItem.TryGetValue(part.ID.Value, out sr)))
            {
                text = string.Format("{0}\r\n{1}{2}", sr.Name, part.IsSkill ? "s" : "", part.ID);
                if (part.IsMixing)
                {
                    text = string.Format("{0} ( {1} {2} : {3} {4} )\r\n{5}+{6}*{7}",
                        Regex.Replace(sr.Name, "^([^ ]+색 )?", "믹스 "),
                        GetColorName(part.ID.Value),
                        100 - part.MixOpacity,
                        GetMixColorName(part.MixColor, part.ID.Value),
                        part.MixOpacity,
                        part.ID,
                        part.MixColor,
                        part.MixOpacity);
                }
            }
            else
            {
                text = string.Format("{0}\r\n{1}", "(null)", part.ID == null ? "-" : part.ID.ToString());
            }
            btn.Text = text;
            btn.NeedRecalcSize = true;
            btn.Refresh();
        }

        private string GetColorName(int ID)
        {
            GearType type = Gear.GetGearType(ID);
            if (Gear.IsFace(type))
            {
                return GetMixColorName(ID / 100 % 10, ID);
            }
            if (Gear.IsHair(type))
            {
                return GetMixColorName(ID % 10, ID);
            }
            return null;
        }

        private string GetMixColorName(int mixColor, int baseID)
        {
            GearType type = Gear.GetGearType(baseID);
            if (Gear.IsFace(type))
            {
                return AvatarPartButtonItem.LensColors[mixColor];
            }
            if (Gear.IsHair(type))
            {
                return AvatarPartButtonItem.HairColors[mixColor];
            }
            return null;
        }

        private void FillBodyActionFrame()
        {
            ComboItem actionItem = cmbActionBody.SelectedItem as ComboItem;
            if (actionItem != null)
            {
                var frames = avatar.GetActionFrames(actionItem.Text);
                FillComboItems(cmbBodyFrame, frames);
            }
            else
            {
                cmbBodyFrame.Items.Clear();
            }
        }

        private void FillEmotionFrame()
        {
            ComboItem emotionItem = cmbEmotion.SelectedItem as ComboItem;
            if (emotionItem != null)
            {
                var frames = avatar.GetFaceFrames(emotionItem.Text);
                FillComboItems(cmbEmotionFrame, frames);
            }
            else
            {
                cmbEmotionFrame.Items.Clear();
            }
        }

        private void FillTamingActionFrame()
        {
            ComboItem actionItem = cmbActionTaming.SelectedItem as ComboItem;
            if (actionItem != null)
            {
                var frames = avatar.GetTamingFrames(actionItem.Text);
                FillComboItems(cmbTamingFrame, frames);
            }
            else
            {
                cmbTamingFrame.Items.Clear();
            }
        }

        private void FillWeaponIdx()
        {
            FillComboItems(cmbWeaponIdx, 0, 4);
        }

        private void FillEarSelection()
        {
            FillComboItems(cmbEar, 0, 4);
        }

        private void FillComboItems(ComboBoxEx comboBox, int start, int count)
        {
            List<ComboItem> items = new List<ComboItem>(count);
            for (int i = 0; i < count; i++)
            {
                ComboItem item = new ComboItem();
                item.Text = (start + i).ToString();
                items.Add(item);
            }
            FillComboItems(comboBox, items);
        }

        private void FillComboItems(ComboBoxEx comboBox, IEnumerable<string> items)
        {
            List<ComboItem> _items = new List<ComboItem>();
            foreach (var itemText in items)
            {
                ComboItem item = new ComboItem();
                item.Text = itemText;
                _items.Add(item);
            }
            FillComboItems(comboBox, _items);
        }

        private void FillComboItems(ComboBoxEx comboBox, IEnumerable<ActionFrame> frames)
        {
            List<ComboItem> items = new List<ComboItem>();
            int i = 0;
            foreach (var f in frames)
            {
                ComboItem item = new ComboItem();
                item.Text = (i++).ToString();
                item.Tag = f;
                items.Add(item);
            }
            FillComboItems(comboBox, items);
        }

        private void FillComboItems(ComboBoxEx comboBox, IEnumerable<ComboItem> items)
        {
            //保持原有选项
            var oldSelection = comboBox.SelectedItem as ComboItem;
            int? newSelection = null;
            comboBox.BeginUpdate();
            comboBox.Items.Clear();

            foreach (var item in items)
            {
                comboBox.Items.Add(item);

                if (newSelection == null && oldSelection != null)
                {
                    if (item.Text == oldSelection.Text)
                    {
                        newSelection = comboBox.Items.Count - 1;
                    }
                }
            }

            //恢复原有选项
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = newSelection ?? 0;
            }

            comboBox.EndUpdate();
        }

        private bool GetSelectedActionFrame(ComboBoxEx comboBox, out int frameIndex, out ActionFrame actionFrame)
        {
            var selectedItem = comboBox.SelectedItem as ComboItem;
            if (selectedItem != null
                && int.TryParse(selectedItem.Text, out frameIndex)
                && selectedItem?.Tag is ActionFrame _actionFrame)
            {
                actionFrame = _actionFrame;
                return true;
            }
            else
            {
                frameIndex = -1;
                actionFrame = null;
                return false;
            }
        }

        private bool GetSelectedBodyFrame(out int frameIndex, out ActionFrame actionFrame)
        {
            return this.GetSelectedActionFrame(this.cmbBodyFrame, out frameIndex, out actionFrame);
        }

        private bool GetSelectedEmotionFrame(out int frameIndex, out ActionFrame actionFrame)
        {
            return this.GetSelectedActionFrame(this.cmbEmotionFrame, out frameIndex, out actionFrame);
        }

        private bool GetSelectedTamingFrame(out int frameIndex, out ActionFrame actionFrame)
        {
            return this.GetSelectedActionFrame(this.cmbTamingFrame, out frameIndex, out actionFrame);
        }
        #endregion

        private void cmbActionBody_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SuspendUpdateDisplay();
            FillBodyActionFrame();
            this.ResumeUpdateDisplay();
            UpdateDisplay();
        }

        private void cmbEmotion_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SuspendUpdateDisplay();
            FillEmotionFrame();
            this.ResumeUpdateDisplay();
            UpdateDisplay();
        }

        private void cmbActionTaming_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SuspendUpdateDisplay();
            FillTamingActionFrame();
            SetTamingDefaultBodyAction();
            SetTamingDefault();
            this.ResumeUpdateDisplay();
            UpdateDisplay();
        }

        private void cmbBodyFrame_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void cmbEmotionFrame_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void cmbTamingFrame_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void cmbWeaponType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void cmbWeaponIdx_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void cmbEar_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void chkBodyPlay_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBodyPlay.Checked)
            {
                if (!this.timer1.Enabled)
                {
                    AnimateStart();
                }

                if (this.GetSelectedBodyFrame(out _, out var actionFrame) && actionFrame.AbsoluteDelay > 0)
                {
                    this.animator.BodyDelay = actionFrame.AbsoluteDelay;
                }
            }
            else
            {
                this.animator.BodyDelay = -1;
                TimerEnabledCheck();
            }
        }

        private void chkEmotionPlay_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEmotionPlay.Checked)
            {
                if (!this.timer1.Enabled)
                {
                    AnimateStart();
                }

                if (this.GetSelectedEmotionFrame(out _, out var actionFrame) && actionFrame.AbsoluteDelay > 0)
                {
                    this.animator.EmotionDelay = actionFrame.AbsoluteDelay;
                }
            }
            else
            {
                this.animator.EmotionDelay = -1;
                TimerEnabledCheck();
            }
        }

        private void chkTamingPlay_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTamingPlay.Checked)
            {
                if (!this.timer1.Enabled)
                {
                    AnimateStart();
                }

                if (this.GetSelectedTamingFrame(out _, out var actionFrame) && actionFrame.AbsoluteDelay > 0)
                {
                    this.animator.TamingDelay = actionFrame.AbsoluteDelay;
                }
            }
            else
            {
                this.animator.TamingDelay = -1;
                TimerEnabledCheck();
            }
        }

        private void chkHairCover_CheckedChanged(object sender, EventArgs e)
        {
            avatar.HairCover = chkHairCover.Checked;
            UpdateDisplay();
        }

        private void chkHairShade_CheckedChanged(object sender, EventArgs e)
        {
            avatar.ShowHairShade = chkHairShade.Checked;
            UpdateDisplay();
        }

        private void chkApplyBRM_CheckedChanged(object sender, EventArgs e)
        {
            avatar.ApplyBRM = chkApplyBRM.Checked;
            this.avatarContainer1.ClearAllCache();
            UpdateDisplay();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.animator.Elapse(timer1.Interval);
            this.AnimateUpdate();
            int interval = this.animator.NextFrameDelay;

            if (interval <= 0)
            {
                this.timer1.Stop();
            }
            else
            {
                this.timer1.Interval = interval;
            }
        }

        private void AnimateUpdate()
        {
            this.SuspendUpdateDisplay();

            if (this.animator.BodyDelay == 0 && FindNextFrame(cmbBodyFrame) && this.GetSelectedBodyFrame(out _, out var bodyFrame))
            {
                this.animator.BodyDelay = bodyFrame.AbsoluteDelay;
            }

            if (this.animator.EmotionDelay == 0 && FindNextFrame(cmbEmotionFrame) && this.GetSelectedEmotionFrame(out _, out var emoFrame))
            {
                this.animator.EmotionDelay = emoFrame.AbsoluteDelay;
            }

            if (this.animator.TamingDelay == 0 && FindNextFrame(cmbTamingFrame) && this.GetSelectedTamingFrame(out _, out var tamingFrame))
            {
                this.animator.TamingDelay = tamingFrame.AbsoluteDelay;
            }

            this.ResumeUpdateDisplay();
        }

        private void AnimateStart()
        {
            TimerEnabledCheck();
            if (timer1.Enabled)
            {
                AnimateUpdate();
            }
        }

        private void TimerEnabledCheck()
        {
            if (chkBodyPlay.Checked || chkEmotionPlay.Checked || chkTamingPlay.Checked)
            {
                if (!this.timer1.Enabled)
                {
                    this.timer1.Interval = 1;
                    this.timer1.Start();
                }
            }
            else
            {
                AnimateStop();
            }
        }

        private void AnimateStop()
        {
            chkBodyPlay.Checked = false;
            chkEmotionPlay.Checked = false;
            chkTamingPlay.Checked = false;
            this.timer1.Stop();
        }

        private bool FindNextFrame(ComboBoxEx cmbFrames)
        {
            ComboItem item = cmbFrames.SelectedItem as ComboItem;
            if (item == null)
            {
                if (cmbFrames.Items.Count > 0)
                {
                    cmbFrames.SelectedIndex = 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            int selectedIndex = cmbFrames.SelectedIndex;
            int i = selectedIndex;
            do
            {
                i = (++i) % cmbFrames.Items.Count;
                item = cmbFrames.Items[i] as ComboItem;
                if (item != null && item.Tag is ActionFrame actionFrame && actionFrame.AbsoluteDelay > 0)
                {
                    cmbFrames.SelectedIndex = i;
                    return true;
                }
            }
            while (i != selectedIndex);

            return false;
        }

        private void btnCode_Click(object sender, EventArgs e)
        {
            var dlg = new AvatarCodeForm();
            string code = GetAllPartsTag();
            dlg.CodeText = code;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (dlg.CodeText != code && !string.IsNullOrEmpty(dlg.CodeText))
                {
                    LoadCode(dlg.CodeText, dlg.LoadType);
                }
            }
        }

        private void btnMale_Click(object sender, EventArgs e)
        {
            if (this.avatar.Parts.All(part => part == null) 
                || MessageBoxEx.Show("기본 남자 캐릭터를 불러오시겠습니까?", "확인") == DialogResult.OK)
            {
                LoadCode("2000,12000,20000,30000,1040036,1060026", 0);
            }
        }

        private void btnFemale_Click(object sender, EventArgs e)
        {
            if (this.avatar.Parts.All(part => part == null)
                || MessageBoxEx.Show("기본 여자 캐릭터를 불러오시겠습니까?", "확인") == DialogResult.OK)
            {
                LoadCode("2000,12000,21000,31000,1041046,1061039", 0);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.avatarContainer1.Origin = new Point(this.avatarContainer1.Width / 2, this.avatarContainer1.Height / 2 + 40);
            this.avatarContainer1.Invalidate();
        }

        private void btnSaveAsGif_Click(object sender, EventArgs e)
        {
            bool bodyPlaying = chkBodyPlay.Checked && cmbBodyFrame.Items.Count > 1;
            bool emoPlaying = chkEmotionPlay.Checked && cmbEmotionFrame.Items.Count > 1;
            bool tamingPlaying = chkTamingPlay.Checked && cmbTamingFrame.Items.Count > 1;

            int aniCount = new[] { bodyPlaying, emoPlaying, tamingPlaying }.Count(b => b);

            if (aniCount == 0)
            {
                this.GetSelectedBodyFrame(out int bodyFrame, out _);
                this.GetSelectedEmotionFrame(out int emoFrame, out _);
                this.GetSelectedTamingFrame(out int tamingFrame, out _);

                // no animation is playing, save as png
                var dlg = new SaveFileDialog()
                {
                    Title = "Save avatar frame",
                    Filter = "PNG (*.png)|*.png|*.*|*.*",
                    FileName = string.Format("avatar{0}{1}{2}.png",
                        string.IsNullOrEmpty(avatar.ActionName) ? "" : ("_" + avatar.ActionName + "(" + bodyFrame + ")"),
                        string.IsNullOrEmpty(avatar.EmotionName) ? "" : ("_" + avatar.EmotionName + "(" + emoFrame + ")"),
                        string.IsNullOrEmpty(avatar.TamingActionName) ? "" : ("_" + avatar.TamingActionName + "(" + tamingFrame + ")"))
                };

                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var bone = this.avatar.CreateFrame(bodyFrame, emoFrame, tamingFrame);
                var frame = this.avatar.DrawFrame(bone);
                frame.Bitmap.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                var config = ImageHandlerConfig.Default;
                var encParams = AnimateEncoderFactory.GetEncoderParams(config.GifEncoder.Value);

                var dlg = new SaveFileDialog()
                {
                    Title = "Save avatar",
                    Filter = string.Format("{0} (*{1})|*{1}|모든 파일(*.*)|*.*", encParams.FileDescription, encParams.FileExtension),
                    FileName = string.Format("avatar{0}{1}{2}{3}",
                        string.IsNullOrEmpty(avatar.ActionName) ? "" : ("_" + avatar.ActionName),
                        string.IsNullOrEmpty(avatar.EmotionName) ? "" : ("_" + avatar.EmotionName),
                        string.IsNullOrEmpty(avatar.TamingActionName) ? "" : ("_" + avatar.TamingActionName),
                        encParams.FileExtension)
                };

                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var actPlaying = new[] { bodyPlaying, emoPlaying, tamingPlaying };
                var actFrames = new[] { cmbBodyFrame, cmbEmotionFrame, cmbTamingFrame }
                    .Select((cmb, i) =>
                    {
                        if (actPlaying[i])
                        {
                            return cmb.Items.OfType<ComboItem>().Select(cmbItem => new
                            {
                                index = int.Parse(cmbItem.Text),
                                actionFrame = cmbItem.Tag as ActionFrame,
                            }).ToArray();
                        }
                        else if (this.GetSelectedActionFrame(cmb, out var index, out var actionFrame))
                        {
                            return new[] { new { index, actionFrame } };
                        }
                        else
                        {
                            return null;
                        }
                    }).ToArray();

                var gifLayer = new GifLayer();

                if (aniCount == 1)
                {
                    int aniActIndex = Array.FindIndex(actPlaying, b => b);
                    for (int fIdx = 0, fCnt = actFrames[aniActIndex].Length; fIdx < fCnt; fIdx++)
                    {
                        int[] actionIndices = new int[] { -1, -1, -1 };
                        int delay = 0;
                        for (int i = 0; i < actFrames.Length; i++)
                        {
                            var act = actFrames[i];
                            if (i == aniActIndex)
                            {
                                actionIndices[i] = act[fIdx].index;
                                delay = act[i].actionFrame.AbsoluteDelay;
                            }
                            else if (act != null)
                            {
                                actionIndices[i] = act[0].index;
                            }
                        }
                        var bone = this.avatar.CreateFrame(actionIndices[0], actionIndices[1], actionIndices[2]);
                        var frameData = this.avatar.DrawFrame(bone);
                        gifLayer.AddFrame(new GifFrame(frameData.Bitmap, frameData.Origin, delay));
                    }
                }
                else
                {
                    // more than 2 animating action parts, for simplicity, we use fixed frame delay.
                    var aniLength = actFrames.Max(layer => layer == null ? 0 : layer.Sum(f => f.actionFrame.AbsoluteDelay));
                    var aniDelay = 30;

                    // pipeline functions
                    IEnumerable<int> RenderDelay()
                    {
                        int t = 0;
                        while (t < aniLength)
                        {
                            int frameDelay = Math.Min(aniLength - t, aniDelay);
                            t += frameDelay;
                            yield return frameDelay;
                        }
                    }

                    IEnumerable<Tuple<int[], int>> GetFrameActionIndices(IEnumerable<int> delayEnumerator)
                    {
                        int[] time = new int[actFrames.Length];
                        int[] actionState = new int[actFrames.Length];
                        for (int i = 0; i < actionState.Length; i++)
                        {
                            actionState[i] = actFrames[i] != null ? 0 : -1;
                        }

                        foreach (int delay in delayEnumerator)
                        {
                            // return state
                            int[] actIndices = new int[actionState.Length];
                            for (int i = 0; i < actionState.Length; i++)
                            {
                                actIndices[i] = actionState[i] > -1 ? actFrames[i][actionState[i]].index : -1;
                            }
                            yield return Tuple.Create(actIndices, delay);

                            // update state
                            for (int i = 0; i < actionState.Length; i++)
                            {
                                if (actPlaying[i])
                                {
                                    var act = actFrames[i];
                                    time[i] += delay;
                                    int frameIndex = actionState[i];
                                    while (time[i] >= act[frameIndex].actionFrame.AbsoluteDelay)
                                    {
                                        time[i] -= act[frameIndex].actionFrame.AbsoluteDelay;
                                        frameIndex = (frameIndex + 1) % act.Length;
                                    }
                                    actionState[i] = frameIndex;
                                }
                            }
                        }
                    }

                    IEnumerable<Tuple<int[], int>> MergeFrames(IEnumerable<Tuple<int[], int>> frames)
                    {
                        int[] prevFrame = null;
                        int prevDelay = 0;

                        foreach (var frame in frames)
                        {
                            int[] currentFrame = frame.Item1;
                            int currentDelay = frame.Item2;

                            if (prevFrame == null)
                            {
                                prevFrame = currentFrame;
                                prevDelay = currentDelay;
                            }
                            else if (prevFrame.SequenceEqual(currentFrame))
                            {
                                prevDelay += currentDelay;
                            }
                            else
                            {
                                yield return Tuple.Create(prevFrame, prevDelay);
                                prevFrame = currentFrame;
                                prevDelay = currentDelay;
                            }
                        }

                        if (prevFrame != null)
                        {
                            yield return Tuple.Create(prevFrame, prevDelay);
                        }
                    }

                    GifFrame ApplyFrame(int[] actionIndices, int delay)
                    {
                        var bone = this.avatar.CreateFrame(actionIndices[0], actionIndices[1], actionIndices[2]);
                        var frameData = this.avatar.DrawFrame(bone);
                        return new GifFrame(frameData.Bitmap, frameData.Origin, delay);
                    }

                    // build pipeline
                    var step1 = RenderDelay();
                    var step2 = GetFrameActionIndices(step1);
                    var step3 = MergeFrames(step2);
                    var step4 = step3.Select(tp => ApplyFrame(tp.Item1, tp.Item2));

                    // run pipeline
                    foreach(var gifFrame in step4)
                    {
                        gifLayer.AddFrame(gifFrame);
                    }
                }

                if (gifLayer.Frames.Count <= 0)
                {
                    MessageBoxEx.Show(this, "计算动画数据失败。", "Error");
                    return;
                }

                Rectangle clientRect = gifLayer.Frames
                    .Select(f => new Rectangle(-f.Origin.X, -f.Origin.Y, f.Bitmap.Width, f.Bitmap.Height))
                    .Aggregate((rect1, rect2) =>
                    {
                        int left = Math.Min(rect1.X, rect2.X);
                        int top = Math.Min(rect1.Y, rect2.Y);
                        int right = Math.Max(rect1.Right, rect2.Right);
                        int bottom = Math.Max(rect1.Bottom, rect2.Bottom);
                        return new Rectangle(left, top, right - left, bottom - top);
                    });

                Brush CreateBackgroundBrush()
                {
                    switch (config.BackgroundType.Value)
                    {
                        default:
                        case ImageBackgroundType.Transparent:
                            return null;
                        case ImageBackgroundType.Color:
                            return new SolidBrush(config.BackgroundColor.Value);
                        case ImageBackgroundType.Mosaic:
                            int blockSize = Math.Max(1, config.MosaicInfo.BlockSize);
                            var texture = new Bitmap(blockSize * 2, blockSize * 2);
                            using (var g = Graphics.FromImage(texture))
                            using (var brush0 = new SolidBrush(config.MosaicInfo.Color0))
                            using (var brush1 = new SolidBrush(config.MosaicInfo.Color1))
                            {
                                g.FillRectangle(brush0, 0, 0, blockSize, blockSize);
                                g.FillRectangle(brush0, blockSize, blockSize, blockSize, blockSize);
                                g.FillRectangle(brush1, 0, blockSize, blockSize, blockSize);
                                g.FillRectangle(brush1, blockSize, 0, blockSize, blockSize);
                            }
                            return new TextureBrush(texture);
                    }
                }

                var bgBrush = CreateBackgroundBrush();
                using (var enc = AnimateEncoderFactory.CreateEncoder(dlg.FileName, clientRect.Width, clientRect.Height, config))
                {
                    foreach (IGifFrame gifFrame in gifLayer.Frames)
                    {
                        using (var bmp = new Bitmap(clientRect.Width, clientRect.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                        {
                            using (var g = Graphics.FromImage(bmp))
                            {
                                // draw background
                                if (bgBrush != null)
                                {
                                    g.FillRectangle(bgBrush, 0, 0, bmp.Width, bmp.Height);
                                }
                                gifFrame.Draw(g, clientRect);
                            }
                            enc.AppendFrame(bmp, Math.Max(10, gifFrame.Delay));
                        }
                    }
                }
                bgBrush?.Dispose();
            }
        }

        private void LoadCode(string code, int loadType)
        {
            //解析
            var matches = Regex.Matches(code, @"s?(\d+)(\+([0-7])\*(\d{1,2}))?([,\s]|$)");
            if (matches.Count <= 0)
            {
                MessageBoxEx.Show("아이템 코드에 해당되는 아이템이 없습니다.", "오류");
                return;
            }

            if (PluginManager.FindWz(Wz_Type.Base) == null)
            {
                MessageBoxEx.Show("Base.wz 파일을 열 수 없습니다.", "오류");
                return;
            }

            var characWz = PluginManager.FindWz(Wz_Type.Character);
            var skillWz = PluginManager.FindWz(Wz_Type.Skill);
            var itemWz = PluginManager.FindWz(Wz_Type.Item);

            //试图初始化
            if (!this.inited && !this.AvatarInit())
            {
                MessageBoxEx.Show("아바타 플러그인을 초기화할 수 없습니다.", "오류");
                return;
            }
            var sl = this.PluginEntry.Context.DefaultStringLinker;
            if (!sl.HasValues) //生成默认stringLinker
            {
                sl.Load(PluginManager.FindWz(Wz_Type.String).GetValueEx<Wz_File>(null), PluginManager.FindWz(Wz_Type.Item).GetValueEx<Wz_File>(null), PluginManager.FindWz(Wz_Type.Etc).GetValueEx<Wz_File>(null));
            }

            if (loadType == 0) //先清空。。
            {
                Array.Clear(this.avatar.Parts, 0, this.avatar.Parts.Length);
            }

            List<int> failList = new List<int>();

            foreach (Match m in matches)
            {
                int gearID;
                if (Int32.TryParse(m.Result("$1"), out gearID))
                {
                    Wz_Node imgNode = FindNodeByGearID(characWz, gearID);
                    if (imgNode != null)
                    {
                        var part = this.avatar.AddPart(imgNode);
                        if (m.Groups.Count >= 4 && Int32.TryParse(m.Result("$3"), out int mixColor) && Int32.TryParse(m.Result("$4"), out int mixOpacity))
                        {
                            part.MixColor = mixColor;
                            part.MixOpacity = mixOpacity;
                        }
                        OnNewPartAdded(part);
                        continue;
                    }
                    if (m.ToString().StartsWith("s"))
                    {
                        imgNode = FindNodeBySkillID(skillWz, gearID);
                        if (imgNode != null)
                        {
                            int tamingMobID = imgNode.Nodes["vehicleID"].GetValueEx<int>(0);
                            if (tamingMobID == 0)
                            {
                                tamingMobID = PluginBase.PluginManager.FindWz(string.Format(@"Skill\RidingSkillInfo.img\{0:D7}\vehicleID", gearID)).GetValueEx<int>(0);
                            }
                            if (tamingMobID != 0)
                            {
                                var tamingMobNode = PluginBase.PluginManager.FindWz(string.Format(@"Character\TamingMob\{0:D8}.img", tamingMobID));
                                if (tamingMobNode != null)
                                {
                                    var part = this.avatar.AddTamingPart(tamingMobNode, BitmapOrigin.CreateFromNode(imgNode.Nodes["icon"], PluginBase.PluginManager.FindWz), gearID, true);
                                    OnNewPartAdded(part);
                                }
                            }
                            continue;
                        }
                    }
                    imgNode = FindNodeByItemID(itemWz, gearID);
                    if (imgNode != null)
                    {
                        int tamingMobID = imgNode.FindNodeByPath("info\\tamingMob").GetValueEx<int>(0);
                        if (tamingMobID != 0)
                        {
                            var tamingMobNode = PluginBase.PluginManager.FindWz(string.Format(@"Character\TamingMob\{0:D8}.img", tamingMobID));
                            if (tamingMobNode != null)
                            {
                                var part = this.avatar.AddTamingPart(tamingMobNode, BitmapOrigin.CreateFromNode(imgNode.FindNodeByPath("info\\icon"), PluginBase.PluginManager.FindWz), gearID, false);
                                OnNewPartAdded(part);
                            }
                        }
                        continue;
                    }
                    // else
                    {
                        failList.Add(gearID);
                    }
                }
            }

            //刷新
            this.FillAvatarParts();
            this.UpdateDisplay();

            //其他提示
            if (failList.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("해당 아이템 코드를 찾을 수 없습니다 : ");
                foreach (var gearID in failList)
                {
                    sb.Append("  ").AppendLine(gearID.ToString("D8"));
                }
                MessageBoxEx.Show(sb.ToString(), "오류");
            }

        }

        private Wz_Node FindNodeByGearID(Wz_Node characWz, int id)
        {
            string imgName = id.ToString("D8") + ".img";
            Wz_Node imgNode = null;

            foreach (var node1 in characWz.Nodes)
            {
                if (node1.Text.Contains("_Canvas"))
                {
                    continue;
                }

                if (node1.Text == imgName)
                {
                    imgNode = node1;
                    break;
                }
                else if (node1.Nodes.Count > 0)
                {
                    foreach (var node2 in node1.Nodes)
                    {
                        if (node2.Text == imgName)
                        {
                            imgNode = node2;
                            break;
                        }
                    }
                    if (imgNode != null)
                    {
                        break;
                    }
                }
            }

            if (imgNode != null)
            {
                Wz_Image img = imgNode.GetValue<Wz_Image>();
                if (img != null && img.TryExtract())
                {
                    return img.Node;
                }
            }

            return null;
        }

        private Wz_Node FindNodeBySkillID(Wz_Node skillWz, int id)
        {
            string idName = id.ToString();

            foreach (var node1 in skillWz.Nodes)
            {
                if (idName.StartsWith(node1.Text.Replace(".img", "")))
                {
                    Wz_Image img = node1.GetValue<Wz_Image>();
                    if (img != null && img.TryExtract())
                    {
                        if (img.Node.Nodes["skill"].Nodes.Count > 0)
                        {
                            foreach (var skillNode in img.Node.Nodes["skill"].Nodes)
                            {
                                if (skillNode.Text == idName)
                                {
                                    return skillNode;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }


        private Wz_Node FindNodeByItemID(Wz_Node itemWz, int id)
        {
            string idName = id.ToString("D8");
            Wz_Node imgNode = null;

            foreach (var node1 in itemWz.Nodes)
            {
                if (node1.Nodes.Count > 0)
                {
                    foreach (var node2 in node1.Nodes)
                    {
                        if (idName.StartsWith(node2.Text.Replace(".img", "")))
                        {
                            imgNode = node2;
                            break;
                        }
                    }
                    if (imgNode != null)
                    {
                        break;
                    }
                }
            }

            if (imgNode != null)
            {
                Wz_Image img = imgNode.GetValue<Wz_Image>();
                if (img != null && img.TryExtract())
                {
                    if (img.Node.Nodes.Count > 0)
                    {
                        foreach (var itemNode in img.Node.Nodes)
                        {
                            if (itemNode.Text == idName)
                            {
                                return itemNode;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private class Animator
        {
            public Animator()
            {
                this.delays = new int[3] { -1, -1, -1 };
            }

            private int[] delays;

            public int NextFrameDelay { get; private set; }

            public int BodyDelay
            {
                get { return this.delays[0]; }
                set
                {
                    this.delays[0] = value;
                    Update();
                }
            }

            public int EmotionDelay
            {
                get { return this.delays[1]; }
                set
                {
                    this.delays[1] = value;
                    Update();
                }
            }

            public int TamingDelay
            {
                get { return this.delays[2]; }
                set
                {
                    this.delays[2] = value;
                    Update();
                }
            }

            public void Elapse(int millisecond)
            {
                for (int i = 0; i < delays.Length; i++)
                {
                    if (delays[i] >= 0)
                    {
                        delays[i] = delays[i] > millisecond ? (delays[i] - millisecond) : 0;
                    }
                }
            }

            private void Update()
            {
                int nextFrame = 0;
                foreach (int delay in this.delays)
                {
                    if (delay > 0)
                    {
                        nextFrame = nextFrame <= 0 ? delay : Math.Min(nextFrame, delay);
                    }
                }
                this.NextFrameDelay = nextFrame;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportAvatar(sender, e);
        }

        private void ExportAvatar(object sender, EventArgs e)
        {
            ComboItem selectedItem;
            //同步角色动作
            selectedItem = this.cmbActionBody.SelectedItem as ComboItem;
            this.avatar.ActionName = selectedItem != null ? selectedItem.Text : null;
            //同步表情
            selectedItem = this.cmbEmotion.SelectedItem as ComboItem;
            this.avatar.EmotionName = selectedItem != null ? selectedItem.Text : null;
            //同步骑宠动作
            this.avatar.TamingActionName = null;

            //获取动作帧
            this.GetSelectedBodyFrame(out int bodyFrame, out _);
            this.GetSelectedEmotionFrame(out int emoFrame, out _);
            this.GetSelectedTamingFrame(out int tamingFrame, out _);

            //获取武器状态
            selectedItem = this.cmbWeaponType.SelectedItem as ComboItem;
            this.avatar.WeaponType = selectedItem != null ? Convert.ToInt32(selectedItem.Text) : 0;

            selectedItem = this.cmbWeaponIdx.SelectedItem as ComboItem;
            this.avatar.WeaponIndex = selectedItem != null ? Convert.ToInt32(selectedItem.Text) : 0;

            if (this.avatar.ActionName == null)
            {
                MessageBoxEx.Show("캐릭터가 없습니다.");
                return;
            }

            var config = ImageHandlerConfig.Default;
            var encParams = AnimateEncoderFactory.GetEncoderParams(config.GifEncoder.Value);

            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "내보내고자 하는 폴더를 선택하세요.";

            async Task ExportGif(string actionName)
            {
                var actionFrames = avatar.GetActionFrames(actionName);
                var faceFrames = avatar.GetFaceFrames(avatar.EmotionName);
                var tamingFrames = avatar.GetTamingFrames(avatar.TamingActionName);
                if (emoFrame <= -1 || emoFrame >= faceFrames.Length)
                {
                    return;
                }

                Gif gif = new Gif();

                foreach (var frame in string.IsNullOrEmpty(avatar.TamingActionName) ? actionFrames : tamingFrames)
                {
                    if (frame.Delay != 0)
                    {
                        var bone = string.IsNullOrEmpty(avatar.TamingActionName) ? avatar.CreateFrame(frame, faceFrames[emoFrame], null) : avatar.CreateFrame(actionFrames[0], faceFrames[emoFrame], frame);
                        var bmp = avatar.DrawFrame(bone);

                        Point pos = bmp.OpOrigin;
                        pos.Offset(frame.Flip ? new Point(-frame.Move.X, frame.Move.Y) : frame.Move);
                        GifFrame f = new GifFrame(bmp.Bitmap, new Point(-pos.X, -pos.Y), Math.Abs(frame.Delay));
                        gif.Frames.Add(f);
                    }
                }

                string fileName = System.IO.Path.Combine(dlg.SelectedPath, actionName.Replace('\\', '.') + encParams.FileExtension);

                var tasks = new List<Task>();

                tasks.Add(Task.Run(() =>
                {
                    GifEncoder enc = AnimateEncoderFactory.CreateEncoder(fileName, gif.GetRect().Width, gif.GetRect().Height, config);
                    gif.SaveGif(enc, fileName, Color.Transparent);
                }));

                await Task.WhenAll(tasks);
            }

            async Task ExportJob(IProgressDialogContext context, CancellationToken cancellationToken)
            {
                IEnumerable<Action> actionEnumerator = avatar.Actions;
                var step1 = actionEnumerator.TakeWhile(_ => !cancellationToken.IsCancellationRequested);

                var step2 = step1.Select(item => ExportGif(item.Name));

                // run pipeline
                try
                {
                    this.Enabled = false;
                    context.ProgressMin = 0;
                    context.ProgressMax = avatar.Actions.Count;
                    foreach (var task in step2)
                    {
                        await task;
                        context.Progress++;
                    }
                }
                catch (Exception ex)
                {
                    context.Message = $"오류: {ex.Message}";
                    throw;
                }
                finally
                {
                    this.Enabled = true;
                }
            }

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ProgressDialog.Show(this.FindForm(), "내보내는 중...", avatar.Actions.Count + " 동작 내보내는 중...", true, false, ExportJob);
            }
        }
    }
}
