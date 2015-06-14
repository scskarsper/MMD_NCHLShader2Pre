using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PMDEditor;
using System.Diagnostics;

namespace MMD_NCHLShader2Pre
{
    public partial class Form1 : Form
    {
        PmxFile Loader = new PmxFile();
        Pmx PModel = null;
        string PmxFilePath = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_OpenPmx_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Title = "打开MMD模型";
            ofd.AddExtension = true;
            ofd.DefaultExt = ".pmx";
            ofd.Filter = "*.pmx|*.pmx";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadPmd(ofd.FileName);
            }
        }

        static string GenRealDir(string BaseDir, string Dir)
        {
            if (Dir.Length > 1 && Dir[1] == ':')
            {
                return Dir;
            }
            string K = BaseDir + "\\" + Dir;
            return K.Replace("\\\\", "\\").Replace("/", "\\");
        }
        void LoadPmd(string FileName)
        {
            PmxFilePath = FileName;
            PModel = Loader.GetFile(FileName);
            System.IO.FileInfo fi = new System.IO.FileInfo(FileName);
            string BaseDir = fi.Directory.FullName;
            for(int i=0;i<PModel.MaterialList.Count;i++)//(PmxMaterial pxm in PModel.MaterialList)
            {
                string Tex = PModel.MaterialList[i].Tex;
                string Toon = PModel.MaterialList[i].Toon;
                string Spa = PModel.MaterialList[i].Sphere;
                string Name = PModel.MaterialList[i].Name.ToLower();
                PModel.MaterialList[i].SetFlag(PmxMaterial.MaterialFlags.SelfShadow, true);
                PModel.MaterialList[i].SetFlag(PmxMaterial.MaterialFlags.SelfShadowMap, true);
                if (Name == "") Name = PModel.MaterialList[i].NameE.ToLower();
                PmxMaterial.SphereModeType Spp = PModel.MaterialList[i].SphereMode;
                if (Tex != "")
                {
                    string F = GenRealDir(BaseDir, Tex);
                    if (!System.IO.File.Exists(F))
                    {
                        PModel.MaterialList[i].Tex = "";
                    }
                }
                if(Toon=="")
                {
                    if (Name.IndexOf("face") >= 0)
                    {
                        PModel.MaterialList[i].Toon = "toon02.bmp";
                    }
                    if (Name.IndexOf("body") >= 0)
                    {
                        PModel.MaterialList[i].Toon = "toon02.bmp";
                    }
                    if (Name.IndexOf("hand") >= 0)
                    {
                        PModel.MaterialList[i].Toon = "toon02.bmp";
                    }
                    if (Name.IndexOf("foot") >= 0)
                    {
                        PModel.MaterialList[i].Toon = "toon02.bmp";
                    }
                    if (Name.IndexOf("leg") >= 0)
                    {
                        PModel.MaterialList[i].Toon = "toon02.bmp";
                    }
                    if (Name.IndexOf("肌") >= 0)
                    {
                        PModel.MaterialList[i].Toon = "toon02.bmp";
                    }
                    if (Name.IndexOf("颜") >= 0)
                    {
                        PModel.MaterialList[i].Toon = "toon02.bmp";
                    }
                    if (Name.IndexOf("鼻") >= 0)
                    {
                        PModel.MaterialList[i].Toon = "toon02.bmp";
                    }
                    if (Name.IndexOf("足") >= 0)
                    {
                        PModel.MaterialList[i].Toon = "toon02.bmp";
                    }
                    if (Name.IndexOf("脚") >= 0)
                    {
                        PModel.MaterialList[i].Toon = "toon02.bmp";
                    }
                    if (Name.IndexOf("手") >= 0)
                    {
                        PModel.MaterialList[i].Toon = "toon02.bmp";
                    }
                }
                if (Toon != "")
                {
                    string K = Toon.ToLower().Replace(".bmp", "").Replace(".jpg", "").Replace(".jpeg", "").Replace(".png", "").Replace(".gif", "").Replace(".tga", "");
                    switch (K)
                    {
                        case "toon01": PModel.MaterialList[i].Toon = "toon01.bmp"; break;
                        case "toon02": PModel.MaterialList[i].Toon = "toon02.bmp"; break;
                        default:break;
                    }
                }
                if (Spa != "")
                {
                    if (Spp == PmxMaterial.SphereModeType.SubTex)
                    {
                        string K = Spa.ToLower().Replace(".bmp", "").Replace(".jpg", "").Replace(".jpeg", "").Replace(".png", "").Replace(".gif", "").Replace(".tga", "");
                        string F = GenRealDir(BaseDir, Spa);
                        if (System.IO.File.Exists(F))
                        {

                        }
                        else
                        {
                            PModel.MaterialList[i].Sphere = "";
                            PModel.MaterialList[i].SphereMode = PmxMaterial.SphereModeType.None;
                        }
                    }
                    else
                    {
                        PModel.MaterialList[i].Sphere = "";
                        PModel.MaterialList[i].SphereMode = PmxMaterial.SphereModeType.None;
                    }
                }
            }
            RefreshToonPage();
            RefreshSphTexPath();
        }

        void RefreshToonPage()
        {
            toon_Other.Items.Clear();
            toon_Skin.Items.Clear();
            toon_Space.Items.Clear();
            for (int i = 0; i < PModel.MaterialList.Count; i++)//(PmxMaterial pxm in PModel.MaterialList)
            {
                string Toon = PModel.MaterialList[i].Toon;
                switch (Toon)
                {
                    case "toon01.bmp": toon_Other.Items.Add(new MaterialItem(PModel.MaterialList[i],i)); break;
                    case "toon02.bmp": toon_Skin.Items.Add(new MaterialItem(PModel.MaterialList[i], i)); break;
                    case "": toon_Space.Items.Add(new MaterialItem(PModel.MaterialList[i], i)); break;
                    default: toon_Space.Items.Add(new MaterialItemEx(PModel.MaterialList[i], i)); break;
                }
            }
        }
        void RefreshSphTexPath()
        {
            btn_ReBuildShp.Enabled = false;
            btn_AddNRM.Enabled = false;
            button4.Enabled=false;
            button5.Enabled=false;
            lst_NoNRM.Items.Clear();
            lst_NRMInn.Items.Clear();
            for (int i = 0; i < PModel.MaterialList.Count; i++)
            {
                string Tex = PModel.MaterialList[i].Tex;
                if (Tex != "")
                {
                    string Sph = PModel.MaterialList[i].Sphere;
                    if(Sph=="")
                    {
                        MaterialItemNoSph OCO = new MaterialItemNoSph(PModel.MaterialList[i], i, PmxFilePath);
                        lst_NoNRM.Items.Add(OCO);
                        btn_AddNRM.Enabled = true;
                        if(OCO.CacheType==3 || OCO.CacheType==2)
                        {
                            button5.Enabled=true;
                        }
                        if(OCO.CacheType==3 || OCO.CacheType==1)
                        {
                            button4.Enabled=true;
                        }
                    }else
                    {
                        lst_NRMInn.Items.Add(new MaterialItemSph(PModel.MaterialList[i], i));
                        btn_ReBuildShp.Enabled = true;
                    }
                }
            }
        }

        private void toon_Space_Click(object sender, EventArgs e)
        {
        }

        private void toon_Space_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toon_Space.SelectedItems.Count == 1)
            {
                object OO = toon_Space.SelectedItems[0];
                MaterialItem Obj = null;
                if (OO.GetType() == typeof(MaterialItemEx))
                {
                    Obj = ((MaterialItemEx)OO).ToNormal;
                }
                else
                {
                    Obj = (MaterialItem)OO;
                }
                LView(Obj);
            }
        }
        void LView(MaterialItem Obj)
        {
            int idx = (int)Obj.Tag;
            PmxMaterial PM = PModel.MaterialList[idx];
            string F = "";
            if (PM.Tex != "")
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(PmxFilePath);
                string BaseDir = fi.Directory.FullName;
                F = GenRealDir(BaseDir, PM.Tex);
                tex_ViewBox.Image = ImageLoader.Load(F);
            }
            else
            {
                Color NewColor = Color.FromArgb(PM.Diffuse.ToArgb());
                Bitmap bmp = new Bitmap(10, 10);
                Graphics g = Graphics.FromImage(bmp);
                g.FillRectangle(new SolidBrush(NewColor), -100, -100, 300, 300);
                g.Dispose();
                tex_ViewBox.Image = bmp;
            }
            StringBuilder AMsg = new StringBuilder();
            AMsg.AppendLine("元件名称：" + PM.Name + "\n");
            AMsg.AppendLine("元件名称E：" + PM.NameE + "\n");
            AMsg.AppendLine("贴图：" + (F == "" ? "无" : F) + "\n");
            AMsg.AppendLine("Toon：" + PM.Toon + "(" +(PM.Toon=="toon01.bmp"?"其他元件":(PM.Toon=="toon02.bmp"?"皮肤元件":"其他/无Toon"))+ ")\n");
            button8.Enabled = true;
            button9.Enabled = true;
            button8.Tag = idx;
            button9.Tag = idx;
            tex_MsgBox.Text = AMsg.ToString();
        }

        private void toon_Other_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toon_Other.SelectedItems.Count == 1)
            {
                MaterialItem Obj = (MaterialItem)toon_Other.SelectedItems[0];
                LView(Obj);
            }
        }

        private void toon_Skin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toon_Skin.SelectedItems.Count == 1)
            {
                MaterialItem Obj = (MaterialItem)toon_Skin.SelectedItems[0];
                LView(Obj);
            }
        }

        private void btn_SpaceToOther_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection SOC=toon_Space.SelectedItems;
            List<string> CMap=new List<string>();
            for (int i = 0; i < SOC.Count; i++)
            {
                object OO = SOC[i];
                if (OO.GetType() == typeof(MaterialItemEx))
                {
                    CMap.Add(((MaterialItemEx)OO).Item.Name);
                }
            }
            if (CMap.Count > 0)
            {
                string OC = String.Join("," , CMap.ToArray());
                DialogResult DR=MessageBox.Show("选中列表中，以下元件包含Toon\r\n" + OC + "\r\n继续操作他们的材质会被替换和统一，确认要操作么？","询问",MessageBoxButtons.YesNo);
                if (DR == DialogResult.No)
                {
                    return;
                }
            }
            string Target = "toon01.bmp";
            for (int i = 0; i < SOC.Count; i++)
            {
                object OO = SOC[i];
                MaterialItem Obj = null;
                if (OO.GetType() == typeof(MaterialItemEx))
                {
                    Obj = ((MaterialItemEx)OO).ToNormal;
                }
                else
                {
                    Obj = (MaterialItem)OO;
                }
                int idx = (int)Obj.Tag;
                PModel.MaterialList[idx].Toon = Target;
            }
            RefreshToonPage();
        }

        private void btn_SpaceToSkin_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection SOC = toon_Space.SelectedItems;
            List<string> CMap = new List<string>();
            for (int i = 0; i < SOC.Count; i++)
            {
                object OO = SOC[i];
                if (OO.GetType() == typeof(MaterialItemEx))
                {
                    CMap.Add(((MaterialItemEx)OO).Item.Name);
                }
            }
            if (CMap.Count > 0)
            {
                string OC = String.Join(",", CMap.ToArray());
                DialogResult DR = MessageBox.Show("选中列表中，以下元件包含Toon\r\n" + OC + "\r\n继续操作他们的材质会被替换和统一，确认要操作么？", "询问", MessageBoxButtons.YesNo);
                if (DR == DialogResult.No)
                {
                    return;
                }
            }
            string Target = "toon02.bmp";
            for (int i = 0; i < SOC.Count; i++)
            {
                object OO = SOC[i];
                MaterialItem Obj = null;
                if (OO.GetType() == typeof(MaterialItemEx))
                {
                    Obj = ((MaterialItemEx)OO).ToNormal;
                }
                else
                {
                    Obj = (MaterialItem)OO;
                }
                int idx = (int)Obj.Tag;
                PModel.MaterialList[idx].Toon = Target;
            }
            RefreshToonPage();
        }

        private void btn_OtherToSkin_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection SOC = toon_Other.SelectedItems;
            string Target = "toon02.bmp";
            for (int i = 0; i < SOC.Count; i++)
            {
                MaterialItem Obj = (MaterialItem)SOC[i];
                int idx = (int)Obj.Tag;
                PModel.MaterialList[idx].Toon = Target;
            }
            RefreshToonPage();
        }

        private void btn_OtherToSpace_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection SOC = toon_Other.SelectedItems;
            string Target = "";
            for (int i = 0; i < SOC.Count; i++)
            {
                MaterialItem Obj = (MaterialItem)SOC[i];
                int idx = (int)Obj.Tag;
                PModel.MaterialList[idx].Toon = Target;
            }
            RefreshToonPage();
        }

        private void btn_SkinToOther_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection SOC = toon_Skin.SelectedItems;
            string Target = "toon01.bmp";
            for (int i = 0; i < SOC.Count; i++)
            {
                MaterialItem Obj = (MaterialItem)SOC[i];
                int idx = (int)Obj.Tag;
                PModel.MaterialList[idx].Toon = Target;
            }
            RefreshToonPage();
        }

        private void btn_SkinToSpace_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection SOC = toon_Skin.SelectedItems;
            string Target = "";
            for (int i = 0; i < SOC.Count; i++)
            {
                MaterialItem Obj = (MaterialItem)SOC[i];
                int idx = (int)Obj.Tag;
                PModel.MaterialList[idx].Toon = Target;
            }
            RefreshToonPage();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void lst_NoNRM_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_NoNRM.SelectedItems.Count == 1)
            {
                MaterialItem Obj = ((MaterialItemNoSph)lst_NoNRM.SelectedItems[0]).ToNormal;
                LViewSph(Obj,(int)Obj.Tag);
            }
        }

        private void lst_NRMInn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_NRMInn.SelectedItems.Count == 1)
            {
                MaterialItem Obj = ((MaterialItemSph)lst_NRMInn.SelectedItems[0]).ToNormal;
                LViewSph(Obj,(int)Obj.Tag);
            }
        }

        void LViewSph(MaterialItem Obj,int ObjIndex)
        {
            button3.Tag = -1;
            button3.Enabled = true;
            int idx = (int)Obj.Tag;
            PmxMaterial PM = PModel.MaterialList[idx];
            System.IO.FileInfo fi = new System.IO.FileInfo(PmxFilePath);
            string BaseDir = fi.Directory.FullName;
            if (PM.Tex != "")
            {
                string F = GenRealDir(BaseDir, PM.Tex);
                pic_Tex.Image = ImageLoader.Load(F);
                pic_Tex.Tag = F;
            }
            else
            {
                button3.Enabled = false;
                pic_Tex.Image = null;
            }
            if (PM.Sphere != "")
            {
                string F = GenRealDir(BaseDir, PM.Sphere);
                pic_Sph.Image = ImageLoader.Load(F);
                pic_Sph.Tag = F;
                btn_ReBuildShp.Enabled = true;
                button3.Tag = ObjIndex;
            }
            else
            {
                pic_Sph.Image = null;
                btn_ReBuildShp.Enabled = false;
                button3.Enabled = true;
                button3.Tag = ObjIndex;
            }
        }

        private void btn_SelectSame_NoNRM_Click(object sender, EventArgs e)
        {
            List<string> PicTex = new List<string>();
            foreach (MaterialItemNoSph m in lst_NoNRM.SelectedItems)
            {
                if (m.Item.Tex != "")
                {
                    if(!PicTex.Contains(m.Item.Tex))
                    {
                        PicTex.Add(m.Item.Tex);
                    }
                }
            }
            for (int i = 0; i < lst_NoNRM.Items.Count; i++)
            {
                MaterialItemNoSph m = (MaterialItemNoSph)lst_NoNRM.Items[i];
                if (PicTex.Contains(m.Item.Tex))
                {
                    lst_NoNRM.SetSelected(i, true);
                }
            }
        }

        private void btn_SelectSame_NRMInn_Click(object sender, EventArgs e)
        {
            List<string> PicTex = new List<string>();
            foreach (MaterialItemSph m in lst_NRMInn.SelectedItems)
            {
                if (m.Item.Tex != "")
                {
                    if (!PicTex.Contains(m.Item.Tex))
                    {
                        PicTex.Add(m.Item.Tex);
                    }
                }
            }
            for (int i = 0; i < lst_NRMInn.Items.Count; i++)
            {
                MaterialItemSph m = (MaterialItemSph)lst_NRMInn.Items[i];
                if (PicTex.Contains(m.Item.Tex))
                {
                    lst_NRMInn.SetSelected(i, true);
                }
            }
        }

        private void btn_RmvNRM_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lst_NRMInn.SelectedItems.Count; i++)
            {
                MaterialItemSph m = (MaterialItemSph)lst_NRMInn.SelectedItems[i];
                PModel.MaterialList[(int)m.Tag].SphereMode = PmxMaterial.SphereModeType.None;
                PModel.MaterialList[(int)m.Tag].Sphere = "";
            }
            RefreshSphTexPath();
        }

        private void btn_ReBuildShp_Click(object sender, EventArgs e)
        {
            btn_AddNRM.Enabled = false;
            btn_ReBuildShp.Enabled = false;
            btn_ReBuildShp.Text = "转换中";
            btn_AddNRM.Text = "转换中";
            for (int i = 0; i < lst_NRMInn.SelectedItems.Count; i++)
            {
                Application.DoEvents();
                MaterialItemSph m = (MaterialItemSph)lst_NRMInn.SelectedItems[i];
                string Tex = m.Item.Tex;
                System.IO.FileInfo fi = new System.IO.FileInfo(PmxFilePath);
                string BaseDir = fi.Directory.FullName;
                if (Tex != "")
                {
                    string F = GenRealDir(BaseDir, Tex);
                    string SPath = (string)pic_Tex.Tag;
                    bool K = (SPath == F);
                    string Sp2 = Tex.Substring(0, Tex.LastIndexOf('.')) + "_NRM.png";
                    string F2 = GenRealDir(BaseDir, Sp2);
                    string Spp = Tex.Substring(0, Tex.LastIndexOf('.')) + "_NRM";
                    string Fpp = GenRealDir(BaseDir, Spp);
                    string[] Exts = new string[] { ".jpg", ".png", ".bmp", ".tga", ".gif" };
                    try
                    {
                        foreach (string es in Exts)
                        {
                            if (System.IO.File.Exists(Fpp + es))
                            {
                                System.IO.File.Delete(Fpp + es);
                            }
                        }
                    }
                    catch { ;}
                    if (NRMBuilder.BuildNRM(F, F2, false))
                    {
                        Application.DoEvents();
                        PModel.MaterialList[(int)m.Tag].SphereMode = PmxMaterial.SphereModeType.SubTex;
                        PModel.MaterialList[(int)m.Tag].Sphere = Sp2;
                        if (K)
                        {
                            pic_Sph.Image = ImageLoader.Load(F2);
                        }
                    }
                }
            }
            btn_AddNRM.Enabled = true;
            btn_AddNRM.Text = "计算和生成法线贴图并绑定";
            btn_ReBuildShp.Enabled = true;
            btn_ReBuildShp.Text = "重新生成法线贴图";
            RefreshSphTexPath();
        }

        private void btn_AddNRM_Click(object sender, EventArgs e)
        {
            btn_AddNRM.Enabled = false;
            btn_ReBuildShp.Enabled = false;
            btn_ReBuildShp.Text = "转换中";
            btn_AddNRM.Text = "转换中";
            for (int i = 0; i <lst_NoNRM.SelectedItems.Count; i++)
            {
                Application.DoEvents();
                MaterialItemNoSph m = (MaterialItemNoSph)lst_NoNRM.SelectedItems[i];
                string Tex = m.Item.Tex;
                System.IO.FileInfo fi = new System.IO.FileInfo(PmxFilePath);
                string BaseDir = fi.Directory.FullName;
                if (Tex != "")
                {
                    string F = GenRealDir(BaseDir, Tex);
                    string Sp2 = Tex.Substring(0, Tex.LastIndexOf('.')) + "_NRM.png";
                    string F2 = GenRealDir(BaseDir, Sp2);
                    string Spp = Tex.Substring(0, Tex.LastIndexOf('.')) + "_NRM";
                    string Fpp = GenRealDir(BaseDir, Spp);
                    System.IO.FileInfo fii = new System.IO.FileInfo(Fpp);
                    string Kpp=fii.DirectoryName+"_CrazyBump\\"+fii.Name;
                    string KSp = Kpp.ToLower().Replace(BaseDir.ToLower(), "");
                    if(KSp[0]=='\\' || KSp[0]=='/') KSp=KSp.Substring(1);
                    if (System.IO.File.Exists(Fpp + ".png"))
                    {
                        Application.DoEvents();
                        PModel.MaterialList[(int)m.Tag].SphereMode = PmxMaterial.SphereModeType.SubTex;
                        PModel.MaterialList[(int)m.Tag].Sphere = Spp+".png";
                    }
                    else if (System.IO.File.Exists(Fpp + ".bmp"))
                    {
                        Application.DoEvents();
                        PModel.MaterialList[(int)m.Tag].SphereMode = PmxMaterial.SphereModeType.SubTex;
                        PModel.MaterialList[(int)m.Tag].Sphere = Spp + ".bmp";
                    }
                    else if (System.IO.File.Exists(Fpp + ".jpg"))
                    {
                        Application.DoEvents();
                        PModel.MaterialList[(int)m.Tag].SphereMode = PmxMaterial.SphereModeType.SubTex;
                        PModel.MaterialList[(int)m.Tag].Sphere = Spp + ".jpg";
                    }
                    else if (System.IO.File.Exists(Fpp + ".gif"))
                    {
                        Application.DoEvents();
                        PModel.MaterialList[(int)m.Tag].SphereMode = PmxMaterial.SphereModeType.SubTex;
                        PModel.MaterialList[(int)m.Tag].Sphere = Spp + ".gif";
                    }
                    else if (System.IO.File.Exists(Fpp + ".tga"))
                    {
                        Application.DoEvents();
                        PModel.MaterialList[(int)m.Tag].SphereMode = PmxMaterial.SphereModeType.SubTex;
                        PModel.MaterialList[(int)m.Tag].Sphere = Spp + ".tga";
                    }
                    else if (System.IO.File.Exists(Kpp + ".png"))
                    {
                        Application.DoEvents();
                        PModel.MaterialList[(int)m.Tag].SphereMode = PmxMaterial.SphereModeType.SubTex;
                        PModel.MaterialList[(int)m.Tag].Sphere = KSp + ".png";
                    }
                    else if (System.IO.File.Exists(Kpp + ".bmp"))
                    {
                        Application.DoEvents();
                        PModel.MaterialList[(int)m.Tag].SphereMode = PmxMaterial.SphereModeType.SubTex;
                        PModel.MaterialList[(int)m.Tag].Sphere = KSp + ".bmp";
                    }
                    else if (System.IO.File.Exists(Kpp + ".jpg"))
                    {
                        Application.DoEvents();
                        PModel.MaterialList[(int)m.Tag].SphereMode = PmxMaterial.SphereModeType.SubTex;
                        PModel.MaterialList[(int)m.Tag].Sphere = KSp + ".jpg";
                    }
                    else if (System.IO.File.Exists(Kpp + ".gif"))
                    {
                        Application.DoEvents();
                        PModel.MaterialList[(int)m.Tag].SphereMode = PmxMaterial.SphereModeType.SubTex;
                        PModel.MaterialList[(int)m.Tag].Sphere = KSp + ".gif";
                    }
                    else if (System.IO.File.Exists(Kpp + ".tga"))
                    {
                        Application.DoEvents();
                        PModel.MaterialList[(int)m.Tag].SphereMode = PmxMaterial.SphereModeType.SubTex;
                        PModel.MaterialList[(int)m.Tag].Sphere = KSp + ".tga";
                    }
                    else
                    {
                        if (NRMBuilder.BuildNRM(F, F2, false))
                        {
                            Application.DoEvents();
                            PModel.MaterialList[(int)m.Tag].SphereMode = PmxMaterial.SphereModeType.SubTex;
                            PModel.MaterialList[(int)m.Tag].Sphere = Sp2;
                        }
                    }
                }
            }
            btn_AddNRM.Enabled = true;
            btn_AddNRM.Text = "计算和生成法线贴图并绑定";
            btn_ReBuildShp.Enabled = true;
            btn_ReBuildShp.Text = "重新生成法线贴图";
            RefreshSphTexPath();
        }

        private void btn_SavePmx_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Title = "保存MMD模型";
            ofd.AddExtension = true;
            ofd.DefaultExt = ".pmx";
            ofd.Filter = "*.pmx|*.pmx";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                PModel.ToFile(ofd.FileName);
            }
        }

        private void btn_SpaceRev_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <toon_Space.Items.Count; i++)
            {
                toon_Space.SetSelected(i, !toon_Space.GetSelected(i));
            }
        }

        private void btn_SkinRev_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < toon_Skin.Items.Count; i++)
            {
                toon_Skin.SetSelected(i, !toon_Skin.GetSelected(i));
            }
        }

        private void btn_OtherRev_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < toon_Other.Items.Count; i++)
            {
                toon_Other.SetSelected(i, !toon_Other.GetSelected(i));
            }
        }

        private void btn_SkinSelectSame_Click(object sender, EventArgs e)
        {
            List<string> PicTex = new List<string>();
            foreach (MaterialItem m in toon_Skin.SelectedItems)
            {
                if (m.Item.Tex != "")
                {
                    if (!PicTex.Contains(m.Item.Tex))
                    {
                        PicTex.Add(m.Item.Tex);
                    }
                }
            }
            for (int i = 0; i < toon_Skin.Items.Count; i++)
            {
                MaterialItem m = (MaterialItem)toon_Skin.Items[i];
                if (PicTex.Contains(m.Item.Tex))
                {
                    toon_Skin.SetSelected(i, true);
                }
            }
        }

        private void btn_OtherSelectSame_Click(object sender, EventArgs e)
        {
            List<string> PicTex = new List<string>();
            foreach (MaterialItem m in toon_Other.SelectedItems)
            {
                if (m.Item.Tex != "")
                {
                    if (!PicTex.Contains(m.Item.Tex))
                    {
                        PicTex.Add(m.Item.Tex);
                    }
                }
            }
            for (int i = 0; i < toon_Other.Items.Count; i++)
            {
                MaterialItem m = (MaterialItem)toon_Other.Items[i];
                if (PicTex.Contains(m.Item.Tex))
                {
                    toon_Other.SetSelected(i, true);
                }
            }
        }

        private void btn_SpaceSelectSame_Click(object sender, EventArgs e)
        {
            List<string> PicTex = new List<string>();
            for (int i = 0; i < toon_Space.SelectedItems.Count; i++)
            {
                object OO = toon_Space.SelectedItems[i];
                MaterialItem Obj = null;
                if (OO.GetType() == typeof(MaterialItemEx))
                {
                    Obj = ((MaterialItemEx)OO).ToNormal;
                }
                else
                {
                    Obj = (MaterialItem)OO;
                }
                MaterialItem m = Obj;
                if (m.Item.Tex != "")
                {
                    if (!PicTex.Contains(m.Item.Tex))
                    {
                        PicTex.Add(m.Item.Tex);
                    }
                }
            }
            for (int i = 0; i < toon_Space.Items.Count; i++)
            {
                object OO = toon_Space.Items[i];
                MaterialItem Obj = null;
                if (OO.GetType() == typeof(MaterialItemEx))
                {
                    Obj = ((MaterialItemEx)OO).ToNormal;
                }
                else
                {
                    Obj = (MaterialItem)OO;
                }
                MaterialItem m = Obj;
                if (PicTex.Contains(m.Item.Tex))
                {
                    toon_Space.SetSelected(i, true);
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            for (int i = 0; i < lst_NRMInn.Items.Count; i++)
            {
                lst_NRMInn.SetSelected(i, !lst_NRMInn.GetSelected(i));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lst_NoNRM.Items.Count; i++)
            {
                lst_NoNRM.SetSelected(i, !lst_NoNRM.GetSelected(i));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if ((int)button3.Tag >= 0)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(PmxFilePath);
                string BaseDir = fi.Directory.FullName;
                string Tex = PModel.MaterialList[(int)button3.Tag].Tex;
                string Spp = Tex.Substring(0, Tex.LastIndexOf('.')) + "_NRM" + Tex.Substring(Tex.LastIndexOf('.'));

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                ofd.InitialDirectory = BaseDir;
                ofd.FileName = Spp.Replace("/","\\");
                ofd.AddExtension = true;
                ofd.Title = "打开法线贴图";
                ofd.Filter = "*.*|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    ofd.InitialDirectory = BaseDir;
                    string Fpp = GenRealDir(BaseDir, Spp);
                    string Spp2 = Tex.Substring(0, Tex.LastIndexOf('.')) + "_NRM";
                    string Fpp2 = GenRealDir(BaseDir, Spp2);
                    string[] Exts = new string[] { ".jpg", ".png", ".bmp", ".tga", ".gif" };
                    try
                    {
                        foreach (string es in Exts)
                        {
                            if (System.IO.File.Exists(Fpp2 + es))
                            {
                                if ((Fpp2 + es).ToLower() != ofd.FileName.ToLower())
                                {
                                    System.IO.File.Delete(Fpp2 + es);
                                }
                            }
                        }
                        System.IO.File.Copy(ofd.FileName, Fpp);
                    }
                    catch { ;}
                    Application.DoEvents();
                    PModel.MaterialList[(int)button3.Tag].Sphere = Spp;
                    if (PModel.MaterialList[(int)button3.Tag].SphereMode != PmxMaterial.SphereModeType.SubTex)
                    {
                        PModel.MaterialList[(int)button3.Tag].SphereMode = PmxMaterial.SphereModeType.SubTex;
                        RefreshSphTexPath();
                    }
                    pic_Sph.Image = ImageLoader.Load(Fpp);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(PmxFilePath);
            string BaseDir = fi.Directory.FullName;
            List<string> PicTex = new List<string>();
            foreach (MaterialItemNoSph m in lst_NoNRM.SelectedItems)
            {
                if (m.Item.Tex != "")
                {
                    string Tex = m.Item.Tex;
                    string Spp = Tex.Substring(0, Tex.LastIndexOf('.')) + "_NRM";
                    string Fpp = GenRealDir(BaseDir, Spp);
                    if (System.IO.File.Exists(Fpp + ".png"))
                    {
                        if (!PicTex.Contains(Spp+".png"))
                        {
                            PicTex.Add(Spp + ".png");
                        }
                    }
                    else if (System.IO.File.Exists(Fpp + ".bmp"))
                    {
                        if (!PicTex.Contains(Spp + ".bmp"))
                        {
                            PicTex.Add(Spp + ".bmp");
                        }
                    }
                    else if (System.IO.File.Exists(Fpp + ".jpg"))
                    {
                        if (!PicTex.Contains(Spp + ".jpg"))
                        {
                            PicTex.Add(Spp + ".jpg");
                        }
                    }
                    else if (System.IO.File.Exists(Fpp + ".gif"))
                    {
                        if (!PicTex.Contains(Spp + ".gif"))
                        {
                            PicTex.Add(Spp + ".gif");
                        }
                    }
                    else if (System.IO.File.Exists(Fpp + ".tga"))
                    {
                        if (!PicTex.Contains(Spp + ".tga"))
                        {
                            PicTex.Add(Spp + ".tga");
                        }
                    }

                }
            }
            List<string> CMap = new List<string>();
            foreach(PmxMaterial pmm in PModel.MaterialList)
            {
                if (PicTex.Contains(pmm.Sphere))
                {
                    CMap.Add(pmm.Name);
                }
            }
            if (CMap.Count > 0)
            {
                string OC = String.Join(",", CMap.ToArray());
                DialogResult DR = MessageBox.Show("选中列表中，以下元件包含该法线贴图\r\n" + OC + "\r\n继续操作他们的法线贴图会被清除，确认要操作么？", "询问", MessageBoxButtons.YesNo);
                if (DR == DialogResult.No)
                {
                    return;
                }
            }

            for (int i = 0; i < PModel.MaterialList.Count; i++)
            {
                PmxMaterial pmm = PModel.MaterialList[i];
                if (PicTex.Contains(pmm.Sphere))
                {
                    PModel.MaterialList[i].Sphere = "";
                    PModel.MaterialList[i].SphereMode = PmxMaterial.SphereModeType.None;
                }
            }
            foreach(string s in PicTex)
            {
                System.IO.File.Delete(GenRealDir(BaseDir, s));
            }
            RefreshSphTexPath();
        }

        private void tableLayoutPanel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(PmxFilePath);
            string BaseDir = fi.Directory.FullName;
            List<string> PicTex = new List<string>();
            foreach (MaterialItemNoSph m in lst_NoNRM.SelectedItems)
            {
                if (m.Item.Tex != "")
                {
                    string Tex = m.Item.Tex;
                    string Spp = Tex.Substring(0, Tex.LastIndexOf('.')) + "_NRM";
                    string Fpp = GenRealDir(BaseDir, Spp);

                    System.IO.FileInfo fii = new System.IO.FileInfo(Fpp);
                    string Kpp = fii.DirectoryName + "_CrazyBump\\" + fii.Name;
                    string KSp = Kpp.ToLower().Replace(BaseDir.ToLower(), "");
                    if (KSp[0] == '\\' || KSp[0] == '/') KSp = KSp.Substring(1);
                    Spp = KSp;
                    Fpp = Kpp;
                    
                    if (System.IO.File.Exists(Fpp + ".png"))
                    {
                        if (!PicTex.Contains(Spp + ".png"))
                        {
                            PicTex.Add(Spp + ".png");
                        }
                    }
                    else if (System.IO.File.Exists(Fpp + ".bmp"))
                    {
                        if (!PicTex.Contains(Spp + ".bmp"))
                        {
                            PicTex.Add(Spp + ".bmp");
                        }
                    }
                    else if (System.IO.File.Exists(Fpp + ".jpg"))
                    {
                        if (!PicTex.Contains(Spp + ".jpg"))
                        {
                            PicTex.Add(Spp + ".jpg");
                        }
                    }
                    else if (System.IO.File.Exists(Fpp + ".gif"))
                    {
                        if (!PicTex.Contains(Spp + ".gif"))
                        {
                            PicTex.Add(Spp + ".gif");
                        }
                    }
                    else if (System.IO.File.Exists(Fpp + ".tga"))
                    {
                        if (!PicTex.Contains(Spp + ".tga"))
                        {
                            PicTex.Add(Spp + ".tga");
                        }
                    }

                }
            }
            List<string> CMap = new List<string>();
            foreach (PmxMaterial pmm in PModel.MaterialList)
            {
                if (PicTex.Contains(pmm.Sphere))
                {
                    CMap.Add(pmm.Name);
                }
            }
            if (CMap.Count > 0)
            {
                string OC = String.Join(",", CMap.ToArray());
                DialogResult DR = MessageBox.Show("选中列表中，以下元件包含该法线贴图\r\n" + OC + "\r\n继续操作他们的法线贴图会被清除，确认要操作么？", "询问", MessageBoxButtons.YesNo);
                if (DR == DialogResult.No)
                {
                    return;
                }
            }

            for (int i = 0; i < PModel.MaterialList.Count; i++)
            {
                PmxMaterial pmm = PModel.MaterialList[i];
                if (PicTex.Contains(pmm.Sphere))
                {
                    PModel.MaterialList[i].Sphere = "";
                    PModel.MaterialList[i].SphereMode = PmxMaterial.SphereModeType.None;
                }
            }
            foreach (string s in PicTex)
            {
                System.IO.File.Delete(GenRealDir(BaseDir, s));
            }
            RefreshSphTexPath();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lst_NRMInn.Items.Count; i++)
            {
                lst_NRMInn.SetSelected(i,true);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lst_NoNRM.Items.Count; i++)
            {
                lst_NoNRM.SetSelected(i, true);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://tieba.baidu.com/p/3352449670");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if ((int)button8.Tag >= 0)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(PmxFilePath);
                string BaseDir = fi.Directory.FullName;
                string BaseToon = BaseDir + "\\Toon";

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                ofd.InitialDirectory = BaseDir;
                ofd.FileName = "";
                ofd.AddExtension = true;
                ofd.Title = "打开Toon贴图";
                ofd.Filter = "*.*|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string targetName = ofd.FileName.ToLower();
                    string ToonValue = "";
                    if (targetName.IndexOf(BaseDir.ToLower()) >= 0)
                    {
                        ToonValue = targetName.Replace(BaseDir.ToLower(), "");
                        if (ToonValue[0] == '\\' || ToonValue[0] == '/')
                        {
                            ToonValue = ToonValue.Substring(1);
                        }
                    }
                    else
                    {
                        if (!System.IO.Directory.Exists(BaseToon))
                        {
                            System.IO.Directory.CreateDirectory(BaseToon);
                        }
                        System.IO.FileInfo FI = new System.IO.FileInfo(targetName);
                        System.IO.File.Copy(targetName, BaseToon+"\\"+FI.Name);
                        ToonValue = "Toon\\" + FI.Name;
                    }
                    PModel.MaterialList[(int)button8.Tag].Toon = ToonValue;
                }
                RefreshToonPage();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if ((int)button8.Tag >= 0)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(PmxFilePath);
                string BaseDir = fi.Directory.FullName;
                string BaseToon = BaseDir + "\\Toon";

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                ofd.InitialDirectory = BaseDir;
                ofd.FileName = "";
                ofd.AddExtension = true;
                ofd.Title = "打开Toon贴图";
                ofd.Filter = "*.*|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string targetName = ofd.FileName.ToLower();
                    string ToonValue = "";
                    if (targetName.IndexOf(BaseDir.ToLower()) >= 0)
                    {
                        ToonValue = targetName.Replace(BaseDir.ToLower(), "");
                        if (ToonValue[0] == '\\' || ToonValue[0] == '/')
                        {
                            ToonValue = ToonValue.Substring(1);
                        }
                    }
                    else
                    {
                        if (!System.IO.Directory.Exists(BaseToon))
                        {
                            System.IO.Directory.CreateDirectory(BaseToon);
                        }
                        System.IO.FileInfo FI = new System.IO.FileInfo(targetName);
                        System.IO.File.Copy(targetName, BaseToon + "\\" + FI.Name);
                        ToonValue = "Toon\\" + FI.Name;
                    }
                    List<int> IndexMap = new List<int>();
                    for (int i = 0; i < PModel.MaterialList.Count; i++)
                    {
                        if (PModel.MaterialList[(int)button8.Tag].Tex != "")
                        {
                            if (PModel.MaterialList[i].Tex == PModel.MaterialList[(int)button8.Tag].Tex)
                            {
                                IndexMap.Add(i);
                            }
                        }
                        else
                        {
                            if (PModel.MaterialList[i].Diffuse.ToArgb() == PModel.MaterialList[(int)button8.Tag].Diffuse.ToArgb())
                            {
                                IndexMap.Add(i);
                            }
                        }
                    }
                    foreach (int Idx in IndexMap)
                    {
                        PModel.MaterialList[Idx].Toon = ToonValue;
                    }
                }
                RefreshToonPage();
            }
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }


    }
    public class MaterialItem
    {
        PmxMaterial item = null;
        object BM = null;
        public MaterialItem(PmxMaterial Item, object BringMsg)
        {
            this.item = Item;
            this.BM = BringMsg;
        }
        public override string ToString()
        {
            try
            {
                return item.Name == "" ? item.NameE : item.Name;
            }
            catch { ;}
            return base.ToString();
        }
        public object Tag
        {
            get
            {
                return BM;
            }
        }
        public PmxMaterial Item
        {
            get
            {
                return item;
            }
        }
    }

    public class MaterialItemNoSph
    {
        PmxMaterial item = null;
        object BM = null;
        string PmxFilePath="";
        int HC = 0;
        public MaterialItemNoSph(PmxMaterial Item, object BringMsg,string PmxFilePath)
        {
            this.item = Item;
            this.BM = BringMsg;
            this.PmxFilePath = PmxFilePath;
            HC = HaveCache();
        }

        public override string ToString()
        {
            try
            {
                string K = (item.Name == "" ? item.NameE : item.Name);
                string B = K;
                switch (HC)
                {
                    case 1: B = B + "[Cache]"; break;
                    case 2: B = B + "[CrazyBump]"; break;
                    case 3: B = B + "[Cache,CrazyBump]"; break;
                }
                return B;
            }
            catch { ;}
            return base.ToString();
        }

        public int CacheType
        {
            get
            {
                return HC;
            }
        }
        
        string GenRealDir(string BaseDir, string Dir)
        {
            if (Dir.Length > 1 && Dir[1] == ':')
            {
                return Dir;
            }
            string K = BaseDir + "\\" + Dir;
            return K.Replace("\\\\", "\\").Replace("/", "\\");
        }
        int HaveCache()
        {
                int CacheType = 0;
                string Tex = item.Tex;
                System.IO.FileInfo fi = new System.IO.FileInfo(PmxFilePath);
                string BaseDir = fi.Directory.FullName;
                if (Tex != "")
                {
                    string Spp = Tex.Substring(0, Tex.LastIndexOf('.')) + "_NRM";
                    string Fpp = GenRealDir(BaseDir, Spp);
                    System.IO.FileInfo fii = new System.IO.FileInfo(Fpp);
                    string Kpp = fii.DirectoryName + "_CrazyBump\\" + fii.Name;
                    string KSp = Kpp.ToLower().Replace(BaseDir.ToLower(), "");
                    if (KSp[0] == '\\' || KSp[0] == '/') KSp = KSp.Substring(1);
                    if (System.IO.File.Exists(Fpp + ".png"))
                    {
                        CacheType = 1;
                    }
                    else if (System.IO.File.Exists(Fpp + ".bmp"))
                    {
                        CacheType = 1;
                    }
                    else if (System.IO.File.Exists(Fpp + ".jpg"))
                    {
                        CacheType = 1;
                    }
                    else if (System.IO.File.Exists(Fpp + ".gif"))
                    {
                        CacheType = 1;
                    }
                    else if (System.IO.File.Exists(Fpp + ".tga"))
                    {
                        CacheType = 1;
                    }
                    if (System.IO.File.Exists(Kpp + ".png"))
                    {
                        CacheType = (CacheType == 1 ? 3 : 2);
                    }
                    else if (System.IO.File.Exists(Kpp + ".bmp"))
                    {
                        CacheType = (CacheType == 1 ? 3 : 2);
                    }
                    else if (System.IO.File.Exists(Kpp + ".jpg"))
                    {
                        CacheType = (CacheType == 1 ? 3 : 2);
                    }
                    else if (System.IO.File.Exists(Kpp + ".gif"))
                    {
                        CacheType = (CacheType == 1 ? 3 : 2);
                    }
                    else if (System.IO.File.Exists(Kpp + ".tga"))
                    {
                        CacheType = (CacheType == 1 ? 3 : 2);
                    }
                }
                return CacheType;
        }

        public object Tag
        {
            get
            {
                return BM;
            }
        }
        public PmxMaterial Item
        {
            get
            {
                return item;
            }
        }
        public MaterialItem ToNormal
        {
            get
            {
                return new MaterialItem(item, BM);
            }
        }
    }

    public class MaterialItemSph
    {
        PmxMaterial item = null;
        object BM = null;
        public MaterialItemSph(PmxMaterial Item, object BringMsg)
        {
            this.item = Item;
            this.BM = BringMsg;
        }
        public override string ToString()
        {
            try
            {
                string K = (item.Name == "" ? item.NameE : item.Name);
                return K+"["+item.Sphere+"]";
            }
            catch { ;}
            return base.ToString();
        }
        public object Tag
        {
            get
            {
                return BM;
            }
        }
        public PmxMaterial Item
        {
            get
            {
                return item;
            }
        }
        public MaterialItem ToNormal
        {
            get
            {
                return new MaterialItem(item, BM);
            }
        }
    }

    public class MaterialItemEx
    {
        PmxMaterial item = null;
        object BM = null;
        public MaterialItemEx(PmxMaterial Item, object BringMsg)
        {
            this.item = Item;
            this.BM = BringMsg;
        }
        public override string ToString()
        {
            try
            {
                string SI = item.Name == "" ? item.NameE : item.Name;
                return SI+"["+item.Toon+"]";
            }
            catch { ;}
            return base.ToString();
        }
        public object Tag
        {
            get
            {
                return BM;
            }
        }
        public PmxMaterial Item
        {
            get
            {
                return item;
            }
        }
        public MaterialItem ToNormal
        {
            get
            {
                return new MaterialItem(item, BM);
            }
        }
    }
}
