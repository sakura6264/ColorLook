using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

namespace ColorLook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class ColorInformation
    {
        //class to read json.
        public string value { get; set; }
        public string name { get; set; }
        public string read { get; set; }
        public int[] rgb { get; set; }
    }
    public partial class MainWindow : Window
    {
        private List<ColorInformation[]> Colorinfos { get; set; }    //list to store json data.
        private bool IfInit { get; set; } = false;//if program is initialized.
        private bool HasWnd { get; set; } = false;
        public MainWindow()
        {
            InitializeComponent();
            //scan , read and store json data.
            if (!System.IO.Directory.Exists(".\\colors")) { System.IO.Directory.CreateDirectory(".\\colors"); }
            string[] colorfiles = System.IO.Directory.GetFiles(".\\colors", "*.json");
            Colorinfos = new List<ColorInformation[]>();
            foreach ( string i in colorfiles)
            {
                Colorinfos.Add(System.Text.Json.JsonSerializer.Deserialize<ColorInformation[]>(System.IO.File.ReadAllText(i)));
                _ = item_colorpack.Items.Add(i.Replace(".json", "").Replace(".\\colors\\",""));
            }
            if (Colorinfos.Count <= 0) { _ = MessageBox.Show("无法找到颜色集.", "警告"); }
            else
            { item_colorpack.SelectedIndex = 0; }   //set colorpack.
            IfInit = true;
            CheckUI();
        }
        //export functions from dll.
        [DllImport("imgcreate.dll", CharSet = CharSet.Unicode)]
        private static extern bool IMGMonochromeShow(int width, int height, int R, int G, int B, string title);
        [DllImport("imgcreate.dll", CharSet = CharSet.Unicode)]
        private static extern bool IMGMonochromeSave(int width, int height, int R, int G, int B,string target, int opt = -1);
        [DllImport("imgcreate.dll", CharSet = CharSet.Unicode)]
        private static extern bool IMGGradientShow(int width, int height, int R0, int G0, int B0, int R1, int G1, int B1, double ang, string title);
        [DllImport("imgcreate.dll", CharSet = CharSet.Unicode)]
        private static extern bool IMGGradientSave(int width, int height, int R0, int G0, int B0, int R1, int G1, int B1, double ang, string target, int opt = -1);
        [DllImport("imgcreate.dll", CharSet = CharSet.Unicode)]
        private static extern bool IMGCenterShow(int width, int height, int R0, int G0, int B0, int R1, int G1, int B1, string title);
        [DllImport("imgcreate.dll", CharSet = CharSet.Unicode)]
        private static extern bool IMGCenterSave(int width, int height, int R0, int G0, int B0, int R1, int G1, int B1, string target, int opt = -1);
        private int LimitRange(int x,int max,int min)
        {
            //simple function to limit range.
            int result = x;
            result = result < max ? result : max;
            result = result > min ? result : min;
            return result;
        }
        private void CheckColor1Status(bool enabled)
        {
            //change color1 status.
            item_color1_customize.IsEnabled = enabled;
            item_color1_fill.IsEnabled = enabled;
            item_color1_show.IsEnabled = enabled;
            if (item_color1_customize.IsEnabled && item_color1_customize.IsChecked == true)
            {
                item_color1.IsReadOnly = false;
                item_color1_r.IsReadOnly = false;
                item_color1_g.IsReadOnly = false;
                item_color1_b.IsReadOnly = false;
            }
            else
            {
                item_color1.IsReadOnly = true;
                item_color1_r.IsReadOnly = true;
                item_color1_g.IsReadOnly = true;
                item_color1_b.IsReadOnly = true;
            }
        }
        private void CheckColor2Status(bool enabled)
        {
            //change color2 status.
            item_color2_customize.IsEnabled = enabled;
            item_color2_fill.IsEnabled = enabled;
            item_color2_show.IsEnabled = enabled;
            if (item_color2_customize.IsEnabled && item_color1_customize.IsChecked == true)
            {
                item_color2.IsReadOnly = false;
                item_color2_r.IsReadOnly = false;
                item_color2_g.IsReadOnly = false;
                item_color2_b.IsReadOnly = false;
            }
            else
            {
                item_color2.IsReadOnly = true;
                item_color2_r.IsReadOnly = true;
                item_color2_g.IsReadOnly = true;
                item_color2_b.IsReadOnly = true;
            }
        }
        private void SetColor1(int r, int g, int b)
        {
            //make color1's hex and r,g,b the same.
            string st = r.ToString("x").PadLeft(2, '0') + g.ToString("x").PadLeft(2, '0') + b.ToString("x").PadLeft(2, '0');
            if (r < 0 || g < 0 || b < 0 || r > 0xff || g > 0xff || b > 0xff)
            {
                return;
            }
            if (!(string.IsNullOrEmpty( item_color1_r.Text) ||
                string.IsNullOrEmpty(item_color1_g.Text) ||
                string.IsNullOrEmpty(item_color1_b.Text)))
            {

                if (r == Convert.ToInt32(item_color1_r.Text) &&
                    g == Convert.ToInt32(item_color1_g.Text) &&
                    b == Convert.ToInt32(item_color1_b.Text) &&
                    item_color1.Text == st)
                {
                    return;
                }
            }
            item_color1_r.Text = r.ToString();
            item_color1_g.Text = g.ToString();
            item_color1_b.Text = b.ToString();
            item_color1.Text = st;
        }
        private void SetColor2(int r, int g, int b)
        {
            //make color2's hex and r,g,b the same.
            string st = r.ToString("x").PadLeft(2, '0') + g.ToString("x").PadLeft(2, '0') + b.ToString("x").PadLeft(2, '0');
            if (r < 0 || g < 0 || b < 0 || r > 0xff || g > 0xff || b > 0xff)
            {
                return;
            }
            if (!(string.IsNullOrEmpty(item_color2_r.Text) ||
                string.IsNullOrEmpty(item_color2_g.Text) ||
                string.IsNullOrEmpty(item_color2_b.Text)))
            {

                if (r == Convert.ToInt32(item_color2_r.Text) &&
                    g == Convert.ToInt32(item_color2_g.Text) &&
                    b == Convert.ToInt32(item_color2_b.Text) &&
                    item_color1.Text == st)
                {
                    return;
                }
            }
            item_color2_r.Text = r.ToString();
            item_color2_g.Text = g.ToString();
            item_color2_b.Text = b.ToString();
            item_color2.Text = st;
        }
        private void CheckTextNumLimitDEC(ref TextBox textBox, int max, int min,ref string st)
        {
            //check limitrange for checkbox
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                string x = System.Text.RegularExpressions.Regex.Replace(textBox.Text, @"[^0-9]", string.Empty);
                if (textBox.Text != x)
                {
                    textBox.Text = x;
                }
                x = string.IsNullOrEmpty(x) ? string.Empty : LimitRange(Convert.ToInt32(x), max, min).ToString();
                if (textBox.Text != x)
                {
                    textBox.Text = st;
                }
                st = textBox.Text;
                textBox.Select(textBox.Text.Length, 0);
            }
        }
        private void CheckTextNumLimitHEX(ref TextBox textBox)
        {
            //check if hex is stable.
            if (textBox.Text != "")
            {
                string x = System.Text.RegularExpressions.Regex.Replace(textBox.Text, @"[^0-9a-f]", string.Empty);
                if (textBox.Text != x)
                {
                    textBox.Text = x;
                }
            }
        }
        private void CheckUI()
        {
            //if havent be initialized,do not excute.
            if (!IfInit)
            {
                return;
            }
            //check if all the items could be enable.
            if (string.IsNullOrEmpty(item_x.Text) ||
                string.IsNullOrEmpty(item_y.Text))//check save and show.
            {
                item_save.IsEnabled =false;
                item_show.IsEnabled = false;
            }
            else if (item_colormod.SelectedIndex >= 0)
            {
                bool st;
                switch (item_colormod.SelectedIndex)
                {
                    case 0:
                        st = !(string.IsNullOrEmpty(item_color1_r.Text) ||
                            string.IsNullOrEmpty(item_color1_g.Text) ||
                            string.IsNullOrEmpty(item_color1_b.Text));
                        item_save.IsEnabled = st && item_filetype.SelectedIndex >= 0 &&
                            (item_filetype.SelectedIndex == 2 || !string.IsNullOrEmpty(item_compresslevel.Text));
                        item_show.IsEnabled = st;
                        break;
                    case 1:
                        st = !(string.IsNullOrEmpty(item_color1_r.Text) ||
                            string.IsNullOrEmpty(item_color1_g.Text) ||
                            string.IsNullOrEmpty(item_color1_b.Text) ||
                            string.IsNullOrEmpty(item_color2_r.Text) ||
                            string.IsNullOrEmpty(item_color2_g.Text) ||
                            string.IsNullOrEmpty(item_color2_b.Text) ||
                            string.IsNullOrEmpty(item_angle.Text));
                        item_save.IsEnabled = st && item_filetype.SelectedIndex >= 0 &&
                            (item_filetype.SelectedIndex == 3 || !string.IsNullOrEmpty(item_compresslevel.Text));
                        item_show.IsEnabled = st;
                        break;
                    default:
                        st = !(string.IsNullOrEmpty(item_color1_r.Text) ||
                            string.IsNullOrEmpty(item_color1_g.Text) ||
                            string.IsNullOrEmpty(item_color1_b.Text) ||
                            string.IsNullOrEmpty(item_color2_r.Text) ||
                            string.IsNullOrEmpty(item_color2_g.Text) ||
                            string.IsNullOrEmpty(item_color2_b.Text));
                        item_save.IsEnabled = st && item_filetype.SelectedIndex >= 0 &&
                            (item_filetype.SelectedIndex == 3 || !string.IsNullOrEmpty(item_compresslevel.Text));
                        item_show.IsEnabled = st;
                        break;
                }
            }
            else
            {
                item_save.IsEnabled = false;
                item_show.IsEnabled = false;
            }
            if (item_colormod.SelectedIndex >= 0)//check color.
            {
                switch (item_colormod.SelectedIndex)
                {
                    case 0:
                        CheckColor1Status(true);
                        CheckColor2Status(false);
                        item_angle.IsEnabled = false;
                        break;
                    case 1:
                        CheckColor1Status(true);
                        CheckColor2Status(true);
                        item_angle.IsEnabled = true;
                        break;
                    default:
                        CheckColor1Status(true);
                        CheckColor2Status(true);
                        item_angle.IsEnabled = false;
                        break;
                }
            }
            else
            {
                CheckColor1Status(false);
                CheckColor2Status(false);
                item_angle.IsEnabled = false;
            }
            
            switch (item_filetype.SelectedIndex)//check compresslevel.
            {
                case 0:
                    item_compresslevel.IsEnabled = true;
                    if (!string.IsNullOrEmpty(item_compresslevel.Text) )
                    {
                        if (Convert.ToInt32(item_compresslevel.Text) > 9)
                        {
                            item_compresslevel.Text = "9";
                        }
                    }
                    item_compresslevel.MaxLength = 1;
                    break;
                case 1:
                    item_compresslevel.IsEnabled = true;
                    item_compresslevel.MaxLength = 3;
                    break;
                default:
                    item_compresslevel.IsEnabled = false;
                    break;
            }
            if (!(string.IsNullOrEmpty(item_color1_r.Text) ||
                            string.IsNullOrEmpty(item_color1_g.Text) ||
                            string.IsNullOrEmpty(item_color1_b.Text) ||
                            string.IsNullOrEmpty(item_color2_r.Text) ||
                            string.IsNullOrEmpty(item_color2_g.Text) ||
                            string.IsNullOrEmpty(item_color2_b.Text)))//check switch.
            {
                item_switch.IsEnabled = true;
            }
            else
            {
                item_switch.IsEnabled = false;
            }
        }
        //signals.
        private void item_colorlist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (item_colorpack.SelectedIndex < 0 || item_colorlist.SelectedIndex < 0)
            {
                item_colorinfo.Text = string.Empty;
                item_rect0.Fill = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xff));
                return;
            }
            item_colorinfo.Text = Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].name + "\r\n" +
                Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].read + "\r\nHEX:" +
                Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].value + "\r\nRGB:\r\n" +
                Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[0].ToString() + " , " +
                Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[1].ToString() + " , " +
                Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[2].ToString();
            try
            {
                item_rect0.Fill = new SolidColorBrush(Color.FromRgb(Convert.ToByte(Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[0]),
                Convert.ToByte(Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[1]),
                Convert.ToByte(Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[2])));
            }
            catch (InvalidCastException e1)
            {
                _ = MessageBox.Show($"当前颜色数据引发了错误{e1},请检查json是否正确.", "错误");
                Application.Current.Shutdown(-1);
            }
        }
        private void item_colorpack_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            item_colorlist.Items.Clear();
            foreach(ColorInformation i in Colorinfos[item_colorpack.SelectedIndex])
            { _ = item_colorlist.Items.Add(i.name); }
            if (item_colorpack.SelectedIndex < 0 || item_colorlist.SelectedIndex < 0)
            {
                item_colorinfo.Text = string.Empty;
                item_rect0.Fill = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xff));
            }
        }

        private void item_colorshow_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(item_x.Text) || string.IsNullOrEmpty(item_y.Text))
            {
                _ = MessageBox.Show("缺少x,y数据.", "警告");
                return;
            }
            else if (item_colorlist.SelectedIndex < 0 || item_colorpack.SelectedIndex < 0)
            {
                _ = MessageBox.Show("未选中颜色.", "警告");
                return;
            }
            else
            {
                if(HasWnd)
                {
                    _ = MessageBox.Show("请关闭当前预览窗口", "提示");
                    return;
                }
                HasWnd = true;
                bool ret = IMGMonochromeShow(Convert.ToInt32(item_x.Text), Convert.ToInt32(item_y.Text)
                    , Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[0]
                    , Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[1]
                    , Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[2]
                    , Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].name);
                HasWnd = false;
                if (!ret)
                {
                    _ = MessageBox.Show("生成错误.", "错误");
                    return;
                }
            }
        }

        private void item_color_copy_Click(object sender, RoutedEventArgs e)
        {
            if (item_colorlist.SelectedIndex >= 0 && item_colorpack.SelectedIndex >= 0)
            {
                Clipboard.SetText(Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].value);
            }
        }
        private string Store_x { get; set; }
        private void item_x_TextChanged(object sender, TextChangedEventArgs e)
        {
            string st = Store_x;
            CheckTextNumLimitDEC(ref item_x, 0x1000, 0, ref st);
            Store_x = st;
            CheckUI();
        }
        private string Store_y { get; set; }
        private void item_y_TextChanged(object sender, TextChangedEventArgs e)
        {
            string st = Store_y;
            CheckTextNumLimitDEC(ref item_y, 0x1000, 0, ref st);
            Store_y = st;
            CheckUI();
        }

        private void item_filetype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(item_compresslevel.Text))
            {
                switch (item_filetype.SelectedIndex)
                {
                    case 0:
                        item_compresslevel.Text = "1";
                        break;
                    case 1:
                    item_compresslevel.Text = "95";
                        break;
                    default:
                        break;
                }
            }
            CheckUI();
        }
        private string Store_compresslevel { get; set; }
        private void item_compresslevel_TextChanged(object sender, TextChangedEventArgs e)
        {
            string st = Store_compresslevel;
            CheckTextNumLimitDEC(ref item_compresslevel, item_filetype.SelectedIndex == 0 ? 9 : 100, 0, ref st);
            Store_compresslevel = st;
            CheckUI();
        }

        private void item_colormod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if (item_colormod.SelectedIndex == 1 && string.IsNullOrEmpty(item_angle.Text))
            {
                item_angle.Text = "45";
            }
            CheckUI();
        }

        private void item_color1_show_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(item_x.Text) || string.IsNullOrEmpty(item_y.Text))
            {
                _ = MessageBox.Show("缺少x,y数据.", "警告");
                return;
            }
            else if (string.IsNullOrEmpty(item_color1_r.Text) ||
                string.IsNullOrEmpty(item_color1_g.Text) ||
                string.IsNullOrEmpty(item_color1_b.Text))
            {
                _ = MessageBox.Show("颜色数据不完整.", "警告");
                return;
            }
            else
            {
                if (HasWnd)
                {
                    _ = MessageBox.Show("请关闭当前预览窗口", "提示");
                    return;
                }
                HasWnd = true;
                bool ret = IMGMonochromeShow(Convert.ToInt32(item_x.Text),
                    Convert.ToInt32(item_y.Text),
                    Convert.ToInt32(item_color1_r.Text),
                   Convert.ToInt32(item_color1_g.Text),
                   Convert.ToInt32(item_color1_b.Text),
                    "Color One");
                HasWnd = false;
                if (!ret)
                {
                    _ = MessageBox.Show("生成错误.", "错误");
                    return;
                }
            }
        }

        private void item_color2_show_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(item_x.Text) || string.IsNullOrEmpty(item_y.Text))
            {
                _ = MessageBox.Show("缺少x,y数据.", "警告");
                return;
            }
            else if (string.IsNullOrEmpty(item_color2_r.Text) ||
                string.IsNullOrEmpty(item_color2_g.Text) ||
                string.IsNullOrEmpty(item_color2_b.Text))
            {
                _ = MessageBox.Show("颜色数据不完整.", "警告");
                return;
            }
            else
            {
                if (HasWnd)
                {
                    _ = MessageBox.Show("请关闭当前预览窗口", "提示");
                    return;
                }
                HasWnd = true;
                bool ret = IMGMonochromeShow(Convert.ToInt32(item_x.Text), 
                   Convert.ToInt32(item_y.Text),
                   Convert.ToInt32(item_color2_r.Text),
                   Convert.ToInt32(item_color2_g.Text),
                   Convert.ToInt32(item_color2_b.Text),
                    "Color Two");
                HasWnd = false;
                if (!ret)
                {
                    _ = MessageBox.Show("生成错误.", "错误");
                    return;
                }
            }
        }

        private void item_show_Click(object sender, RoutedEventArgs e)
        {
            if (HasWnd)
            {
                _ = MessageBox.Show("请关闭当前预览窗口", "提示");
                return;
            }
            
            bool ret = true;
            switch (item_colormod.SelectedIndex)
            {
                case 0:
                    HasWnd = true;
                   ret = IMGMonochromeShow(Convert.ToInt32(item_x.Text), Convert.ToInt32(item_y.Text),
                        Convert.ToInt32(item_color1_r.Text), Convert.ToInt32(item_color1_g.Text), Convert.ToInt32(item_color1_b.Text), "Output");
                    HasWnd = false;
                    break;
                case 1:
                    HasWnd = true;
                    ret = IMGGradientShow(Convert.ToInt32(item_x.Text), Convert.ToInt32(item_y.Text),
                        Convert.ToInt32(item_color1_r.Text), Convert.ToInt32(item_color1_g.Text), Convert.ToInt32(item_color1_b.Text),
                        Convert.ToInt32(item_color2_r.Text), Convert.ToInt32(item_color2_g.Text), Convert.ToInt32(item_color2_b.Text),
                        Convert.ToInt32(item_angle.Text) * (Math.PI / 180), "Output");
                    HasWnd = false;
                    break;
                case 2:
                    HasWnd = true;
                    ret = IMGCenterShow(Convert.ToInt32(item_x.Text), Convert.ToInt32(item_y.Text),
                        Convert.ToInt32(item_color1_r.Text), Convert.ToInt32(item_color1_g.Text), Convert.ToInt32(item_color1_b.Text),
                        Convert.ToInt32(item_color2_r.Text), Convert.ToInt32(item_color2_g.Text), Convert.ToInt32(item_color2_b.Text)
                        , "Output");
                    HasWnd = false;
                    break;
                default:
                    _ = MessageBox.Show("参数错误.", "错误");
                    return;
            }
            if (!ret)
            {
                _ = MessageBox.Show("生成错误.", "错误");
                return;
            }
        }

        private void item_save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                AddExtension = true,
                InitialDirectory = "c:\\",
                RestoreDirectory = true,
                CheckPathExists = true
            };
            switch (item_filetype.SelectedIndex)
            {
                case 0:
                    saveFileDialog.Filter = "PNG|*.png";
                    saveFileDialog.DefaultExt = "*.png";
                    saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    break;
                case 1:
                    saveFileDialog.Filter = "JPEG|*.jpg;*.jpeg;*.jpe;*.jfif;*.jif";
                    saveFileDialog.DefaultExt = "*.jpg";
                    saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    break;
                case 2:
                    saveFileDialog.Filter = "BMP|*.bmp";
                    saveFileDialog.DefaultExt = "*.bmp";
                    saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    break;
                default:
                    return;
            }
            if (saveFileDialog.ShowDialog() == true)
            {
                int arg = -1;
                switch (item_filetype.SelectedIndex)
                {
                    case 0:
                        arg = Convert.ToInt32(item_compresslevel.Text);
                        break;
                    case 1:
                        arg = 100 - Convert.ToInt32(item_compresslevel.Text);
                        break;
                    default:
                        break;
                }
                switch (item_colormod.SelectedIndex)
                {
                    case 0:
                        _ = IMGMonochromeSave(Convert.ToInt32(item_x.Text), Convert.ToInt32(item_y.Text),
                        Convert.ToInt32(item_color1_r.Text), Convert.ToInt32(item_color1_g.Text), Convert.ToInt32(item_color1_b.Text),
                       saveFileDialog.FileName, arg);
                        break;
                    case 1:
                        _ = IMGGradientSave(Convert.ToInt32(item_x.Text), Convert.ToInt32(item_y.Text),
                        Convert.ToInt32(item_color1_r.Text), Convert.ToInt32(item_color1_g.Text), Convert.ToInt32(item_color1_b.Text),
                        Convert.ToInt32(item_color2_r.Text), Convert.ToInt32(item_color2_g.Text), Convert.ToInt32(item_color2_b.Text),
                        Convert.ToInt32(item_angle.Text) * (Math.PI / 180), saveFileDialog.FileName, arg);
                        break;
                    case 2:
                        _ = IMGCenterSave(Convert.ToInt32(item_x.Text), Convert.ToInt32(item_y.Text),
                        Convert.ToInt32(item_color1_r.Text), Convert.ToInt32(item_color1_g.Text), Convert.ToInt32(item_color1_b.Text),
                        Convert.ToInt32(item_color2_r.Text), Convert.ToInt32(item_color2_g.Text), Convert.ToInt32(item_color2_b.Text)
                        , saveFileDialog.FileName, arg);
                        break;
                    default:
                        return;
                }
            }
        }

        private void item_color2_fill_Click(object sender, RoutedEventArgs e)
        {
            SetColor2(Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[0],
                Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[1],
                Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[2]);
            CheckUI();
        }

        private void item_color1_fill_Click(object sender, RoutedEventArgs e)
        {
            SetColor1(Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[0],
                Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[1],
                Colorinfos[item_colorpack.SelectedIndex][item_colorlist.SelectedIndex].rgb[2]);
            CheckUI();
        }

        private void item_color1_customize_Checked(object sender, RoutedEventArgs e)
        {
            CheckUI();
        }

        private void item_color2_customize_Checked(object sender, RoutedEventArgs e)
        {
            CheckUI();
        }

        private void item_color1_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckTextNumLimitHEX(ref item_color1);
            if (item_color1.Text.Length == 6)
            {
                SetColor1(Convert.ToInt32(item_color1.Text.Substring(0, 2), 16),
                    Convert.ToInt32(item_color1.Text.Substring(2, 2), 16),
                    Convert.ToInt32(item_color1.Text.Substring(4, 2), 16));
            }
            item_rect1.Fill = item_color1.Text.Length == 6
                ? new SolidColorBrush(Color.FromRgb(Convert.ToByte(item_color1_r.Text),
                Convert.ToByte(item_color1_g.Text),
                Convert.ToByte(item_color1_b.Text)))
                : new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xff));
            CheckUI();
        }
        private string Store_color1_r { get; set; }
        private void item_color1_r_TextChanged(object sender, TextChangedEventArgs e)
        {
            string st = Store_color1_r;
            CheckTextNumLimitDEC(ref item_color1_r, 0xff, 0, ref st);
            Store_color1_r = st;
            if (!(string.IsNullOrEmpty(item_color1_r.Text) ||
                 string.IsNullOrEmpty(item_color1_g.Text) ||
                 string.IsNullOrEmpty(item_color1_b.Text)))
            {
                SetColor1(Convert.ToInt32(item_color1_r.Text), Convert.ToInt32(item_color1_g.Text), Convert.ToInt32(item_color1_b.Text));
            }
            CheckUI();
        }
        private string Store_color1_g { get; set; }
        private void item_color1_g_TextChanged(object sender, TextChangedEventArgs e)
        {
            string st = Store_color1_g;
            CheckTextNumLimitDEC(ref item_color1_g, 0xff, 0, ref st);
            Store_color1_g = st;
            if (!(string.IsNullOrEmpty(item_color1_r.Text) ||
                 string.IsNullOrEmpty(item_color1_g.Text) ||
                 string.IsNullOrEmpty(item_color1_b.Text)))
            {
                SetColor1(Convert.ToInt32(item_color1_r.Text), Convert.ToInt32(item_color1_g.Text), Convert.ToInt32(item_color1_b.Text));
            }
            CheckUI();
        }
        private string Store_color1_b { get; set; }
        private void item_color1_b_TextChanged(object sender, TextChangedEventArgs e)
        {
            string st = Store_color1_b;
            CheckTextNumLimitDEC(ref item_color1_b, 0xff, 0, ref st);
            Store_color1_b = st;
            if (!(string.IsNullOrEmpty(item_color1_r.Text) ||
                 string.IsNullOrEmpty(item_color1_g.Text) ||
                 string.IsNullOrEmpty(item_color1_b.Text)))
            {
                SetColor1(Convert.ToInt32(item_color1_r.Text), Convert.ToInt32(item_color1_g.Text), Convert.ToInt32(item_color1_b.Text));
            }
            CheckUI();
        }
        private string Store_angle { get; set; }
        private void item_angle_TextChanged(object sender, TextChangedEventArgs e)
        {
            string st = Store_angle;
            CheckTextNumLimitDEC(ref item_angle, 0xff, 0, ref st);
            Store_angle = st;
            CheckUI();
        }

        private void item_color2_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckTextNumLimitHEX(ref item_color2);
            if (item_color2.Text.Length == 6)
            {
                SetColor2(Convert.ToInt32(item_color2.Text.Substring(0, 2), 16),
                    Convert.ToInt32(item_color2.Text.Substring(2, 2), 16),
                    Convert.ToInt32(item_color2.Text.Substring(4, 2), 16));
            }
            item_rect2.Fill = item_color2.Text.Length == 6
                ? new SolidColorBrush(Color.FromRgb(Convert.ToByte(item_color2_r.Text),
                Convert.ToByte(item_color2_g.Text),
                Convert.ToByte(item_color2_b.Text)))
                : new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xff));
            CheckUI();
        }
        private string Store_color2_r { get; set; }
        private void item_color2_r_TextChanged(object sender, TextChangedEventArgs e)
        {
            string st = Store_color2_r;
            CheckTextNumLimitDEC(ref item_color2_r, 0xff, 0, ref st);
            Store_color2_r = st;
            if (!(string.IsNullOrEmpty(item_color2_r.Text) ||
                 string.IsNullOrEmpty(item_color2_g.Text) ||
                 string.IsNullOrEmpty(item_color2_b.Text)))
            {
                SetColor2(Convert.ToInt32(item_color2_r.Text), Convert.ToInt32(item_color2_g.Text), Convert.ToInt32(item_color2_b.Text));
            }
            CheckUI();
        }
        private string Store_color2_g { get; set; }
        private void item_color2_g_TextChanged(object sender, TextChangedEventArgs e)
        {
            string st = Store_color2_g;
            CheckTextNumLimitDEC(ref item_color2_g, 0xff, 0, ref st);
            Store_color2_g = st;
            if (!(string.IsNullOrEmpty(item_color2_r.Text) ||
                 string.IsNullOrEmpty(item_color2_g.Text) ||
                 string.IsNullOrEmpty(item_color2_b.Text)))
            {
                SetColor2(Convert.ToInt32(item_color2_r.Text), Convert.ToInt32(item_color2_g.Text), Convert.ToInt32(item_color2_b.Text));
            }
            CheckUI();
        }
        private string Store_color2_b { get; set; }
        private void item_color2_b_TextChanged(object sender, TextChangedEventArgs e)
        {
            string st = Store_color2_b;
            CheckTextNumLimitDEC(ref item_color2_b, 0xff, 0, ref st);
            Store_color2_b = st;
            if (!(string.IsNullOrEmpty(item_color2_r.Text) ||
                 string.IsNullOrEmpty(item_color2_g.Text) ||
                 string.IsNullOrEmpty(item_color2_b.Text)))
            {
                SetColor2(Convert.ToInt32(item_color2_r.Text), Convert.ToInt32(item_color2_g.Text), Convert.ToInt32(item_color2_b.Text));
            }
            CheckUI();
        }

        private void item_switch_Click(object sender, RoutedEventArgs e)
        {
            int r = Convert.ToInt32(item_color1_r.Text);
            int g = Convert.ToInt32(item_color1_g.Text);
            int b = Convert.ToInt32(item_color1_b.Text);
            SetColor1(Convert.ToInt32(item_color2_r.Text), Convert.ToInt32(item_color2_g.Text), Convert.ToInt32(item_color2_b.Text));
            SetColor2(r, g, b);
        }
    }
}
