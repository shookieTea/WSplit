namespace WSplitTimer
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public class ColorPicker : Form
    {
        private Button button1;
        private Button button2;
        private NumericUpDown colorBbox;
        private PictureBox colorBox;
        private NumericUpDown colorGbox;
        private NumericUpDown colorRbox;
        private TextBox hexBox;
        public HSV hsvColor;
        private PictureBox hueArrow;
        private PictureBox hueArrow2;
        private readonly Bitmap hues = new Bitmap(20, 0x100);
        private PictureBox hueSlider;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private readonly Bitmap leftArrow = new Bitmap(5, 10);
        private readonly Bitmap palette = new Bitmap(0x100, 0x100);
        private PictureBox paletteBox;
        public Color rgbColor;
        private readonly Bitmap rightArrow = new Bitmap(5, 10);

        public ColorPicker()
        {
            InitializeComponent();
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            SetColorRGB(0, 0, 0);
            using (Graphics graphics = Graphics.FromImage(hues))
            {
                for (int i = 0; i < (hues.Height - 2); i++)
                {
                    using (SolidBrush brush = new SolidBrush(HSVColor(0xff - i, 0xff, 0xff)))
                    {
                        graphics.FillRectangle(brush, 0, i, hues.Width, 1);
                    }
                }
            }
            using (Graphics graphics2 = Graphics.FromImage(leftArrow))
            {
                PointF[] points = new PointF[] { new Point(0, 0), new Point(5, 5), new Point(0, 10), new Point(0, 0) };
                GraphicsPath path = new GraphicsPath();
                path.AddLines(points);
                graphics2.FillPath(Brushes.Black, path);
            }
            using (Graphics graphics3 = Graphics.FromImage(rightArrow))
            {
                PointF[] tfArray2 = new PointF[] { new Point(5, 0), new Point(0, 5), new Point(5, 10), new Point(5, 0) };
                GraphicsPath path2 = new GraphicsPath();
                path2.AddLines(tfArray2);
                graphics3.FillPath(Brushes.Black, path2);
            }
            paletteBox.BackgroundImage = palette;
            hueSlider.BackgroundImage = hues;
            UpdatePalette();
        }

        private void colorBbox_ValueChanged(object sender, EventArgs e)
        {
            if (colorBbox.Value != rgbColor.B)
            {
                SetColorRGB(rgbColor.R, rgbColor.G, (int)colorBbox.Value);
            }
        }

        private void colorGbox_ValueChanged(object sender, EventArgs e)
        {
            if (colorGbox.Value != rgbColor.G)
            {
                SetColorRGB(rgbColor.R, (int)colorGbox.Value, rgbColor.B);
            }
        }

        private void colorRbox_ValueChanged(object sender, EventArgs e)
        {
            if (colorRbox.Value != rgbColor.R)
            {
                SetColorRGB((int)colorRbox.Value, rgbColor.G, rgbColor.B);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public static HSV GetHSV(Color C)
        {
            double num7;
            double num8;
            double num4 = C.R / 255.0;
            double num5 = C.G / 255.0;
            double num6 = C.B / 255.0;
            double num = Math.Min(Math.Min(num4, num5), num6);
            double num2 = Math.Max(Math.Max(num4, num5), num6);
            double num9 = num2;
            double num3 = num2 - num;
            if ((num2 == 0.0) || (num3 == 0.0))
            {
                num8 = 0.0;
                num7 = 0.0;
            }
            else
            {
                num8 = num3 / num2;
                if (num4 == num2)
                {
                    num7 = (num5 - num6) / num3;
                }
                else if (num5 == num2)
                {
                    num7 = 2.0 + ((num6 - num4) / num3);
                }
                else
                {
                    num7 = 4.0 + ((num4 - num5) / num3);
                }
            }
            num7 *= 60.0;
            if (num7 < 0.0)
            {
                num7 += 360.0;
            }
            return new HSV { H = (int)((num7 / 360.0) * 255.0), S = (int)(num8 * 255.0), V = (int)(num9 * 255.0) };
        }

        private void hexBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    Color c = ColorTranslator.FromHtml("#" + hexBox.Text);
                    if (rgbColor != c)
                    {
                        SetColor(c);
                    }
                    e.SuppressKeyPress = true;
                }
                catch
                {
                }
            }
        }

        private void hexBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !Regex.IsMatch(e.KeyChar.ToString(), "[0-9A-Fa-f]"))
            {
                e.Handled = true;
            }
        }

        private void hexBox_Leave(object sender, EventArgs e)
        {
            try
            {
                Color c = ColorTranslator.FromHtml("#" + hexBox.Text);
                if (rgbColor != c)
                {
                    SetColor(c);
                }
            }
            catch
            {
            }
        }

        private void hexBox_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(hexBox.Text, "[^0-9A-Fa-f]"))
            {
                hexBox.Text = Regex.Replace(hexBox.Text, "[^0-9A-Fa-f]", "");
            }
        }

        public static Color HSVColor(int H, int S, int V)
        {
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            double num = ((H / 255.0) * 360.0) % 360.0;
            double num2 = S / 255.0;
            double num3 = V / 255.0;
            if (num2 == 0.0)
            {
                num4 = num3;
                num5 = num3;
                num6 = num3;
            }
            else
            {
                double d = num / 60.0;
                int num11 = (int)Math.Floor(d);
                double num10 = d - num11;
                double num7 = num3 * (1.0 - num2);
                double num8 = num3 * (1.0 - (num2 * num10));
                double num9 = num3 * (1.0 - (num2 * (1.0 - num10)));
                switch (num11)
                {
                    case 0:
                        num4 = num3;
                        num5 = num9;
                        num6 = num7;
                        break;

                    case 1:
                        num4 = num8;
                        num5 = num3;
                        num6 = num7;
                        break;

                    case 2:
                        num4 = num7;
                        num5 = num3;
                        num6 = num9;
                        break;

                    case 3:
                        num4 = num7;
                        num5 = num8;
                        num6 = num3;
                        break;

                    case 4:
                        num4 = num9;
                        num5 = num7;
                        num6 = num3;
                        break;

                    case 5:
                        num4 = num3;
                        num5 = num7;
                        num6 = num8;
                        break;
                }
            }
            return Color.FromArgb(255, (int)(num4 * 255.0), (int)(num5 * 255.0), (int)(num6 * 255.0));
        }

        private void hueArrow_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(leftArrow, 0, 0xff - hsvColor.H);
        }

        private void hueArrow2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(rightArrow, 0, 0xff - hsvColor.H);
        }

        private void hueSlider_MouseDown(object sender, MouseEventArgs e)
        {
            int num = Math.Max(0, Math.Min(0xff - e.Y, 0xff));
            if (hsvColor.H != num)
            {
                hsvColor.H = num;
                SetColorHSV(hsvColor.H, hsvColor.S, hsvColor.V);
            }
        }

        private void hueSlider_MouseMove(object sender, MouseEventArgs e)
        {
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
            {
                int num = Math.Max(0, Math.Min(0xff - e.Y, 0xff));
                if (hsvColor.H != num)
                {
                    hsvColor.H = num;
                    SetColorHSV(hsvColor.H, hsvColor.S, hsvColor.V);
                }
            }
        }

        private void InitializeComponent()
        {
            paletteBox = new PictureBox();
            hueSlider = new PictureBox();
            hueArrow = new PictureBox();
            hueArrow2 = new PictureBox();
            colorRbox = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            colorGbox = new NumericUpDown();
            colorBbox = new NumericUpDown();
            colorBox = new PictureBox();
            button1 = new Button();
            button2 = new Button();
            hexBox = new TextBox();
            label4 = new Label();
            ((ISupportInitialize)paletteBox).BeginInit();
            ((ISupportInitialize)hueSlider).BeginInit();
            ((ISupportInitialize)hueArrow).BeginInit();
            ((ISupportInitialize)hueArrow2).BeginInit();
            colorRbox.BeginInit();
            colorGbox.BeginInit();
            colorBbox.BeginInit();
            ((ISupportInitialize)colorBox).BeginInit();
            base.SuspendLayout();
            paletteBox.BorderStyle = BorderStyle.FixedSingle;
            paletteBox.Location = new Point(12, 12);
            paletteBox.Name = "paletteBox";
            paletteBox.Size = new Size(0x100, 0x100);
            paletteBox.TabIndex = 0;
            paletteBox.TabStop = false;
            paletteBox.Paint += new PaintEventHandler(paletteBox_Paint);
            paletteBox.MouseDown += new MouseEventHandler(paletteBox_MouseDown);
            paletteBox.MouseMove += new MouseEventHandler(paletteBox_MouseMove);
            hueSlider.BorderStyle = BorderStyle.FixedSingle;
            hueSlider.Location = new Point(0x116, 12);
            hueSlider.Name = "hueSlider";
            hueSlider.Size = new Size(0x12, 0x100);
            hueSlider.TabIndex = 1;
            hueSlider.TabStop = false;
            hueSlider.MouseDown += new MouseEventHandler(hueSlider_MouseDown);
            hueSlider.MouseMove += new MouseEventHandler(hueSlider_MouseMove);
            hueArrow.Location = new Point(0x111, 7);
            hueArrow.Name = "hueArrow";
            hueArrow.Size = new Size(5, 0x10a);
            hueArrow.TabIndex = 3;
            hueArrow.TabStop = false;
            hueArrow.Paint += new PaintEventHandler(hueArrow_Paint);
            hueArrow2.Location = new Point(0x128, 7);
            hueArrow2.Name = "hueArrow2";
            hueArrow2.Size = new Size(5, 0x10a);
            hueArrow2.TabIndex = 4;
            hueArrow2.TabStop = false;
            hueArrow2.Paint += new PaintEventHandler(hueArrow2_Paint);
            colorRbox.Location = new Point(0x147, 0x30);
            int[] bits = new int[4];
            bits[0] = 0xff;
            colorRbox.Maximum = new decimal(bits);
            colorRbox.Name = "colorRbox";
            colorRbox.Size = new Size(40, 20);
            colorRbox.TabIndex = 5;
            int[] numArray2 = new int[4];
            numArray2[0] = 0xff;
            colorRbox.Value = new decimal(numArray2);
            colorRbox.ValueChanged += new EventHandler(colorRbox_ValueChanged);
            label1.AutoSize = true;
            label1.Location = new Point(0x132, 0x30);
            label1.MinimumSize = new Size(0, 20);
            label1.Name = "label1";
            label1.Size = new Size(15, 20);
            label1.TabIndex = 6;
            label1.Text = "R";
            label1.TextAlign = ContentAlignment.MiddleRight;
            label2.AutoSize = true;
            label2.Location = new Point(0x132, 0x4a);
            label2.MinimumSize = new Size(0, 20);
            label2.Name = "label2";
            label2.Size = new Size(15, 20);
            label2.TabIndex = 7;
            label2.Text = "G";
            label2.TextAlign = ContentAlignment.MiddleRight;
            label3.AutoSize = true;
            label3.Location = new Point(0x133, 100);
            label3.MinimumSize = new Size(0, 20);
            label3.Name = "label3";
            label3.Size = new Size(14, 20);
            label3.TabIndex = 8;
            label3.Text = "B";
            label3.TextAlign = ContentAlignment.MiddleRight;
            colorGbox.Location = new Point(0x147, 0x4a);
            int[] numArray3 = new int[4];
            numArray3[0] = 0xff;
            colorGbox.Maximum = new decimal(numArray3);
            colorGbox.Name = "colorGbox";
            colorGbox.Size = new Size(40, 20);
            colorGbox.TabIndex = 9;
            int[] numArray4 = new int[4];
            numArray4[0] = 0xff;
            colorGbox.Value = new decimal(numArray4);
            colorGbox.ValueChanged += new EventHandler(colorGbox_ValueChanged);
            colorBbox.Location = new Point(0x147, 100);
            int[] numArray5 = new int[4];
            numArray5[0] = 0xff;
            colorBbox.Maximum = new decimal(numArray5);
            colorBbox.Name = "colorBbox";
            colorBbox.Size = new Size(40, 20);
            colorBbox.TabIndex = 10;
            int[] numArray6 = new int[4];
            numArray6[0] = 0xff;
            colorBbox.Value = new decimal(numArray6);
            colorBbox.ValueChanged += new EventHandler(colorBbox_ValueChanged);
            colorBox.BorderStyle = BorderStyle.FixedSingle;
            colorBox.Location = new Point(0x132, 12);
            colorBox.Name = "colorBox";
            colorBox.Size = new Size(0x3d, 30);
            colorBox.TabIndex = 11;
            colorBox.TabStop = false;
            button1.DialogResult = DialogResult.OK;
            button1.Location = new Point(0x132, 0xd8);
            button1.Name = "button1";
            button1.Size = new Size(0x3d, 0x17);
            button1.TabIndex = 13;
            button1.Text = "OK";
            button1.UseVisualStyleBackColor = true;
            button2.DialogResult = DialogResult.Cancel;
            button2.Location = new Point(0x132, 0xf5);
            button2.Name = "button2";
            button2.Size = new Size(0x3d, 0x17);
            button2.TabIndex = 14;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            hexBox.Location = new Point(0x132, 0xa5);
            hexBox.MaxLength = 6;
            hexBox.Name = "hexBox";
            hexBox.Size = new Size(0x3d, 20);
            hexBox.TabIndex = 12;
            hexBox.Text = "FFFFFF";
            hexBox.TextChanged += new EventHandler(hexBox_TextChanged);
            hexBox.KeyDown += new KeyEventHandler(hexBox_KeyDown);
            hexBox.KeyPress += new KeyPressEventHandler(hexBox_KeyPress);
            hexBox.Leave += new EventHandler(hexBox_Leave);
            label4.AutoSize = true;
            label4.Location = new Point(0x12f, 0x95);
            label4.Name = "label4";
            label4.Size = new Size(0x1a, 13);
            label4.TabIndex = 15;
            label4.Text = "Hex";
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x17b, 280);
            base.Controls.Add(label4);
            base.Controls.Add(hexBox);
            base.Controls.Add(button2);
            base.Controls.Add(button1);
            base.Controls.Add(colorBox);
            base.Controls.Add(colorBbox);
            base.Controls.Add(colorGbox);
            base.Controls.Add(label3);
            base.Controls.Add(label2);
            base.Controls.Add(label1);
            base.Controls.Add(colorRbox);
            base.Controls.Add(hueArrow2);
            base.Controls.Add(hueArrow);
            base.Controls.Add(hueSlider);
            base.Controls.Add(paletteBox);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "ColorPicker";
            Text = "Choose color...";
            ((ISupportInitialize)paletteBox).EndInit();
            ((ISupportInitialize)hueSlider).EndInit();
            ((ISupportInitialize)hueArrow).EndInit();
            ((ISupportInitialize)hueArrow2).EndInit();
            colorRbox.EndInit();
            colorGbox.EndInit();
            colorBbox.EndInit();
            ((ISupportInitialize)colorBox).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void paletteBox_MouseDown(object sender, MouseEventArgs e)
        {
            int s = Math.Max(0, Math.Min(e.X, 0xff));
            int v = Math.Max(0, Math.Min(0xff - e.Y, 0xff));
            if ((s != hsvColor.S) || (v != hsvColor.V))
            {
                SetColorHSV(hsvColor.H, s, v);
            }
        }

        private void paletteBox_MouseMove(object sender, MouseEventArgs e)
        {
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
            {
                int s = Math.Max(0, Math.Min(e.X, 0xff));
                int v = Math.Max(0, Math.Min(0xff - e.Y, 0xff));
                if ((s != hsvColor.S) || (v != hsvColor.V))
                {
                    SetColorHSV(hsvColor.H, s, v);
                }
            }
        }

        private void paletteBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawEllipse(Pens.Black, hsvColor.S - 5, (0xff - hsvColor.V) - 5, 10, 10);
            e.Graphics.DrawEllipse(Pens.White, (float)(hsvColor.S - 4.5f), (float)((0xff - hsvColor.V) - 4.5f), (float)9f, (float)9f);
        }

        public void SetColor(Color c)
        {
            rgbColor = c;
            hsvColor = GetHSV(rgbColor);
            colorRbox.Value = rgbColor.R;
            colorGbox.Value = rgbColor.G;
            colorBbox.Value = rgbColor.B;
            colorBox.BackColor = rgbColor;
            UpdatePalette();
        }

        public void SetColorHSV(int H, int S, int V)
        {
            hsvColor.H = H;
            hsvColor.S = S;
            hsvColor.V = V;
            rgbColor = HSVColor(H, S, V);
            colorRbox.Value = rgbColor.R;
            colorGbox.Value = rgbColor.G;
            colorBbox.Value = rgbColor.B;
            colorBox.BackColor = rgbColor;
            UpdatePalette();
        }

        public void SetColorRGB(int R, int G, int B)
        {
            rgbColor = Color.FromArgb(R, G, B);
            hsvColor = GetHSV(rgbColor);
            colorRbox.Value = rgbColor.R;
            colorGbox.Value = rgbColor.G;
            colorBbox.Value = rgbColor.B;
            colorBox.BackColor = rgbColor;
            UpdatePalette();
        }

        private void UpdatePalette()
        {
            for (int i = 0; i < (palette.Width - 2); i++)
            {
                Color introduced3 = HSVColor(hsvColor.H, i, 0xff);
                using (LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(i, 0, 1, 0x100), introduced3, HSVColor(hsvColor.H, i, 0), 90f))
                {
                    using (Graphics graphics = Graphics.FromImage(palette))
                    {
                        graphics.FillRectangle(brush, i, 0, 1, 0x100);
                    }
                }
            }
            hexBox.Text = string.Format("{0:X2}{1:X2}{2:X2}", rgbColor.R, rgbColor.G, rgbColor.B);
            paletteBox.Invalidate();
            hueArrow.Invalidate();
            hueArrow2.Invalidate();
            base.Update();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HSV
        {
            public int H;
            public int S;
            public int V;
        }
    }
}