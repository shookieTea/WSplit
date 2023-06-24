namespace WSplitTimer
{
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Linq;
    using System.Windows.Forms;

    public class DetailedView : Form
    {
        public const int HTCAPTION = 2;
        public const int HTCLIENT = 1;
        public const int HTLEFT = 10;
        public const int HTRIGHT = 11;
        public const int WM_NCHITTEST = 0x84;

        private IContainer components;

        private float plusPct = 0.5f;
        public int widthH = 1;
        public int widthHH = 1;
        public int widthHHH = 1;
        public int widthM = 1;
        public string clockText = "000:00:00.00";

        private readonly WSplit parent;

        private ContextMenuStrip contextMenuStrip;

        private ToolStripMenuItem menuItemSelectColumns;
        private ToolStripMenuItem menuItemSetColors;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem menuItemShowSegs;
        private ToolStripMenuItem menuItemMarkSegments;
        private ToolStripMenuItem menuItemAlwaysOnTop;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem menuItemClose;

        public Brush clockColor;
        public Font clockFont;

        public DataGridView segs;
        private DataGridViewTextBoxColumn SegName;
        private DataGridViewTextBoxColumn Old;
        private DataGridViewTextBoxColumn SumOfBests;
        private DataGridViewTextBoxColumn Best;
        private DataGridViewTextBoxColumn Live;
        private DataGridViewTextBoxColumn Delta;

        public DataGridView finalSeg;
        private DataGridViewTextBoxColumn finalSegName;
        private DataGridViewTextBoxColumn finalOld;
        private DataGridViewTextBoxColumn finalBest;
        private DataGridViewTextBoxColumn finalSumOfBests;
        private DataGridViewTextBoxColumn finalLive;
        private DataGridViewTextBoxColumn finalDelta;

        public List<PointF> deltaPoints = new List<PointF>();
        public List<double> Deltas = new List<double>();

        public Label displayTime;

        public DetailedView(RunSplits useSplits, WSplit callingForm)
        {
            base.Paint += new PaintEventHandler(dviewPaint);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            InitializeComponent();
            parent = callingForm;
            menuItemShowSegs.Checked = Settings.Profile.DViewShowSegs;
            menuItemMarkSegments.Checked = Settings.Profile.DViewDeltaMarks;
            menuItemAlwaysOnTop.Checked = Settings.Profile.DViewOnTop;
            base.TopMost = Settings.Profile.DViewOnTop;
            updateColumns();
            clockFont = displayTime.Font;
        }

        public void InitializeFonts()
        {
            FontFamily family = FontFamily.Families.FirstOrDefault(f => f.Name == Settings.Profile.FontFamilySegments);

            if (family == null || !family.IsStyleAvailable(FontStyle.Bold))
                displayTime.Font = new Font(FontFamily.GenericSansSerif, 17.33333f, FontStyle.Bold, GraphicsUnit.Pixel);
            else
                displayTime.Font = new Font(family, 21f, FontStyle.Bold, GraphicsUnit.Pixel);

            family = FontFamily.Families.FirstOrDefault(f => f.Name == Settings.Profile.FontFamilyDView);
            Font font;
            if (family == null || !family.IsStyleAvailable(FontStyle.Regular))
                font = new Font(FontFamily.GenericSansSerif, 10.5f, FontStyle.Regular, GraphicsUnit.Pixel);
            else
                font = new Font(Settings.Profile.FontFamilyDView, 10.5f, FontStyle.Regular, GraphicsUnit.Pixel);

            foreach (DataGridViewColumn c in segs.Columns)
                c.DefaultCellStyle.Font = font;

            foreach (DataGridViewColumn c in finalSeg.Columns)
                c.DefaultCellStyle.Font = font;

            widthM = TextRenderer.MeasureText("00:00.00", displayTime.Font).Width;
            widthH = TextRenderer.MeasureText("0:00:00.00", displayTime.Font).Width;
            widthHH = TextRenderer.MeasureText("00:00:00.00", displayTime.Font).Width;
            widthHHH = TextRenderer.MeasureText("000:00:00.00", displayTime.Font).Width;
        }

        private void menuItemAlwaysOnTop_Click(object sender, EventArgs e)
        {
            Settings.Profile.DViewOnTop = !Settings.Profile.DViewOnTop;
            menuItemAlwaysOnTop.Checked = Settings.Profile.DViewOnTop;
            base.TopMost = Settings.Profile.DViewOnTop;
        }

        private void menuItemClose_click(object sender, EventArgs e)
        {
            base.Hide();
            parent.advancedDetailButton.Checked = false;
        }

        private void DetailedView_Resize(object sender, EventArgs e)
        {
            base.Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void dviewPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            Rectangle layoutRectangle = new Rectangle(displayTime.Left, displayTime.Top, displayTime.Width, displayTime.Height);
            if (clockText.Length == 8)
            {
                layoutRectangle.Width = widthM + 6;
            }
            else if (clockText.Length == 10)
            {
                layoutRectangle.Width = widthH + 6;
            }
            else if (clockText.Length == 11)
            {
                layoutRectangle.Width = widthHH + 6;
            }
            else if (clockText.Length == 12)
            {
                layoutRectangle.Width = widthHHH + 6;
            }
            e.Graphics.DrawString(clockText, displayTime.Font, clockColor, layoutRectangle, format);
            int right = layoutRectangle.Right;
            int y = layoutRectangle.Top + 4;
            int width = (base.Width - right) - 6;
            int height = (base.Height - y) - 6;
            if (width >= 30)
            {
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                Pen pen = new Pen(Brushes.White, 1f);
                Pen pen2 = new Pen(new SolidBrush(Color.FromArgb(0x40, 0, 0, 0)), 1f);
                Pen pen3 = new Pen(new SolidBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff)), 1f);
                float num5 = height - (height * plusPct);
                e.Graphics.FillRectangle(new SolidBrush(ColorSettings.Profile.GraphBehind), (float)right, (float)y, (float)width, num5);
                e.Graphics.FillRectangle(new SolidBrush(ColorSettings.Profile.GraphAhead), (float)right, y + num5, (float)width, height - num5);
                for (int i = 1; i <= (width / 7); i++)
                {
                    PointF tf = new PointF((float)(right + (7 * i)), (float)y);
                    PointF tf2 = new PointF(tf.X, (float)(y + height));
                    e.Graphics.DrawLine(pen2, tf, tf2);
                }
                for (int j = 1; j <= (height / 7); j++)
                {
                    PointF tf3 = new PointF((float)right, (float)(y + (7 * j)));
                    PointF tf4 = new PointF((float)(right + width), tf3.Y);
                    e.Graphics.DrawLine(pen2, tf3, tf4);
                }
                e.Graphics.DrawRectangle(pen3, new Rectangle(right, y, width, height));
                if (deltaPoints.Count >= 1)
                {
                    List<PointF> list = new List<PointF> {
                        new PointF((float) right, y + num5)
                    };
                    foreach (PointF tf5 in deltaPoints)
                    {
                        float x = (tf5.X * width) + right;
                        float num9 = (height - (tf5.Y * height)) + y;
                        if (Settings.Profile.DViewDeltaMarks)
                        {
                            e.Graphics.FillEllipse(Brushes.White, (float)(x - 2f), (float)(num9 - 2f), (float)4f, (float)4f);
                        }
                        list.Add(new PointF(x, num9));
                    }
                    e.Graphics.DrawLines(pen, list.ToArray());
                }
            }
        }

        private void InitializeComponent()
        {
            components = new Container();

            contextMenuStrip = new ContextMenuStrip(components);

            menuItemSelectColumns = new ToolStripMenuItem();
            menuItemSetColors = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            menuItemShowSegs = new ToolStripMenuItem();
            menuItemMarkSegments = new ToolStripMenuItem();
            menuItemAlwaysOnTop = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            menuItemClose = new ToolStripMenuItem();

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            DataGridViewCellStyle style2 = new DataGridViewCellStyle();
            DataGridViewCellStyle style3 = new DataGridViewCellStyle();
            DataGridViewCellStyle style4 = new DataGridViewCellStyle();
            DataGridViewCellStyle style5 = new DataGridViewCellStyle();
            DataGridViewCellStyle style6 = new DataGridViewCellStyle();
            segs = new DataGridView();
            SegName = new DataGridViewTextBoxColumn();
            Old = new DataGridViewTextBoxColumn();
            Best = new DataGridViewTextBoxColumn();
            SumOfBests = new DataGridViewTextBoxColumn();
            Live = new DataGridViewTextBoxColumn();
            Delta = new DataGridViewTextBoxColumn();
            displayTime = new Label();
            finalSeg = new DataGridView();
            finalSegName = new DataGridViewTextBoxColumn();
            finalOld = new DataGridViewTextBoxColumn();
            finalBest = new DataGridViewTextBoxColumn();
            finalSumOfBests = new DataGridViewTextBoxColumn();
            finalLive = new DataGridViewTextBoxColumn();
            finalDelta = new DataGridViewTextBoxColumn();

            ((ISupportInitialize)segs).BeginInit();
            contextMenuStrip.SuspendLayout();
            ((ISupportInitialize)finalSeg).BeginInit();
            base.SuspendLayout();
            //
            // contextMenuStrip
            //
            contextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                menuItemSelectColumns,
                menuItemSetColors,
                toolStripSeparator1,
                menuItemShowSegs,
                menuItemMarkSegments,
                menuItemAlwaysOnTop,
                toolStripSeparator2,
                menuItemClose
            });
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new Size(280, 136);
            //
            // menuItemSelectColumns
            //
            menuItemSelectColumns.Name = "menuItemSelectColumns";
            menuItemSelectColumns.Size = new Size(280, 24);
            menuItemSelectColumns.Text = "Select columns...";
            menuItemSelectColumns.Click += menuItemSelectColumns_Click;
            //
            //
            //
            menuItemSetColors.Name = "menuItemSetColors";
            menuItemSetColors.Size = new Size(280, 24);
            menuItemSetColors.Text = "Set colors...";
            menuItemSetColors.Click += menuItemSetColors_Click;
            //
            // toolStripSeparator1
            //
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(276, 6);
            //
            // menuItemShowSegs
            //
            menuItemShowSegs.Name = "menuItemShowSegs";
            menuItemShowSegs.Size = new Size(280, 24);
            menuItemShowSegs.Text = "Show segment times";
            menuItemShowSegs.Click += menuItemShowSegs_Click;
            //
            // menuItemMarkSegments
            //
            menuItemMarkSegments.Name = "menuItemMarkSegments";
            menuItemMarkSegments.Size = new Size(280, 24);
            menuItemMarkSegments.Text = "Mark segments on delta graph";
            menuItemMarkSegments.Click += menuItemMarkSegments_Click;
            //
            // menuItemAlwaysOnTop
            //
            menuItemAlwaysOnTop.Name = "menuItemAlwaysOnTop";
            menuItemAlwaysOnTop.Size = new Size(280, 24);
            menuItemAlwaysOnTop.Text = "Always on top";
            menuItemAlwaysOnTop.Click += menuItemAlwaysOnTop_Click;
            //
            // toolStripSeparator2
            //
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(276, 6);
            //
            // menuItemClose
            //
            menuItemClose.Name = "menuItemClose";
            menuItemClose.Size = new Size(280, 24);
            menuItemClose.Text = "Close";
            menuItemClose.Click += menuItemClose_click;

            segs.AllowUserToAddRows = false;
            segs.AllowUserToDeleteRows = false;
            segs.AllowUserToResizeColumns = false;
            segs.AllowUserToResizeRows = false;
            style.BackColor = Color.Black;
            style.SelectionBackColor = Color.Black;
            segs.AlternatingRowsDefaultCellStyle = style;
            segs.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            segs.BackgroundColor = Color.Black;
            segs.BorderStyle = BorderStyle.None;
            segs.CellBorderStyle = DataGridViewCellBorderStyle.None;
            segs.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            segs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            segs.ColumnHeadersVisible = false;
            segs.Columns.AddRange(new DataGridViewColumn[] { SegName, Old, Best, SumOfBests, Live, Delta });
            style2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style2.BackColor = Color.Black;
            style2.ForeColor = Color.WhiteSmoke;
            style2.SelectionBackColor = Color.Black;
            style2.SelectionForeColor = Color.WhiteSmoke;
            style2.WrapMode = DataGridViewTriState.False;
            segs.DefaultCellStyle = style2;
            segs.Enabled = false;
            segs.GridColor = Color.Black;
            segs.Location = new Point(0, 0);
            segs.Margin = new Padding(0);
            segs.MultiSelect = false;
            segs.Name = "segs";
            segs.ReadOnly = true;
            segs.RowHeadersVisible = false;
            segs.RowTemplate.Height = 0x10;
            segs.ScrollBars = ScrollBars.None;
            segs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            segs.Size = new Size(0xad, 12);
            segs.TabIndex = 0;
            SegName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            SegName.HeaderText = "Segment";
            SegName.Name = "SegName";
            SegName.ReadOnly = true;
            Old.HeaderText = "Old";
            Old.MinimumWidth = 2;
            Old.Name = "Old";
            Old.ReadOnly = true;
            Old.Width = 2;
            Best.HeaderText = "Best";
            Best.MinimumWidth = 2;
            Best.Name = "Best";
            Best.ReadOnly = true;
            Best.Width = 2;
            SumOfBests.HeaderText = "SoB";
            SumOfBests.MinimumWidth = 2;
            SumOfBests.Name = "SumOfBests";
            SumOfBests.ReadOnly = true;
            SumOfBests.Width = 2;
            Live.HeaderText = "Live";
            Live.MinimumWidth = 2;
            Live.Name = "Live";
            Live.ReadOnly = true;
            Live.Width = 2;
            style3.Alignment = DataGridViewContentAlignment.BottomRight;
            Delta.DefaultCellStyle = style3;
            Delta.HeaderText = "Delta";
            Delta.MinimumWidth = 2;
            Delta.Name = "Delta";
            Delta.ReadOnly = true;
            Delta.Width = 2;
            displayTime.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            displayTime.AutoSize = true;
            displayTime.BackColor = Color.Black;
            displayTime.ForeColor = Color.PaleGoldenrod;
            displayTime.Location = new Point(0, 0x2e);
            displayTime.Margin = new Padding(0);
            displayTime.MinimumSize = new Size(0, 0x22);
            displayTime.Name = "displayTime";
            displayTime.Size = new Size(0x81, 0x22);
            displayTime.TabIndex = 2;
            displayTime.Text = "000:00:00.00";
            displayTime.TextAlign = ContentAlignment.MiddleLeft;
            displayTime.Visible = false;
            finalSeg.AllowUserToAddRows = false;
            finalSeg.AllowUserToDeleteRows = false;
            finalSeg.AllowUserToResizeColumns = false;
            finalSeg.AllowUserToResizeRows = false;
            style4.BackColor = Color.Black;
            style4.SelectionBackColor = Color.Black;
            finalSeg.AlternatingRowsDefaultCellStyle = style4;
            finalSeg.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            finalSeg.BackgroundColor = Color.Black;
            finalSeg.BorderStyle = BorderStyle.None;
            finalSeg.CellBorderStyle = DataGridViewCellBorderStyle.None;
            finalSeg.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            finalSeg.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            finalSeg.ColumnHeadersVisible = false;
            finalSeg.Columns.AddRange(new DataGridViewColumn[] { finalSegName, finalOld, finalBest, finalSumOfBests, finalLive, finalDelta });
            style5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style5.BackColor = Color.Black;
            style5.ForeColor = Color.WhiteSmoke;
            style5.SelectionBackColor = Color.Black;
            style5.SelectionForeColor = Color.WhiteSmoke;
            style5.WrapMode = DataGridViewTriState.False;
            finalSeg.DefaultCellStyle = style5;
            finalSeg.Enabled = false;
            finalSeg.GridColor = Color.Black;
            finalSeg.Location = new Point(0, 12);
            finalSeg.Margin = new Padding(0);
            finalSeg.MultiSelect = false;
            finalSeg.Name = "finalSeg";
            finalSeg.ReadOnly = true;
            finalSeg.RowHeadersVisible = false;
            finalSeg.RowTemplate.Height = 0x10;
            finalSeg.ScrollBars = ScrollBars.None;
            finalSeg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            finalSeg.Size = new Size(0xad, 12);
            finalSeg.TabIndex = 3;
            finalSegName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            finalSegName.HeaderText = "Segment";
            finalSegName.Name = "finalSegName";
            finalSegName.ReadOnly = true;
            finalOld.HeaderText = "Old";
            finalOld.MinimumWidth = 2;
            finalOld.Name = "finalOld";
            finalOld.ReadOnly = true;
            finalOld.Width = 2;
            finalBest.HeaderText = "Best";
            finalBest.MinimumWidth = 2;
            finalBest.Name = "finalBest";
            finalBest.ReadOnly = true;
            finalBest.Width = 2;
            finalSumOfBests.HeaderText = "Sum of Bests";
            finalSumOfBests.MinimumWidth = 2;
            finalSumOfBests.Name = "finalSumOfBests";
            finalSumOfBests.ReadOnly = true;
            finalSumOfBests.Width = 2;
            finalLive.HeaderText = "Live";
            finalLive.MinimumWidth = 2;
            finalLive.Name = "finalLive";
            finalLive.ReadOnly = true;
            finalLive.Width = 2;
            style6.Alignment = DataGridViewContentAlignment.BottomRight;
            finalDelta.DefaultCellStyle = style6;
            finalDelta.HeaderText = "Delta";
            finalDelta.MinimumWidth = 2;
            finalDelta.Name = "finalDelta";
            finalDelta.ReadOnly = true;
            finalDelta.Width = 2;
            base.AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Black;
            base.ClientSize = new Size(0xad, 80);
            ContextMenuStrip = contextMenuStrip;
            base.Controls.Add(segs);
            base.Controls.Add(finalSeg);
            base.Controls.Add(displayTime);
            base.FormBorderStyle = FormBorderStyle.None;
            base.Icon = Resources.AppIcon;
            base.Margin = new Padding(4, 4, 4, 4);
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            MinimumSize = new Size(0x6b, 0);
            base.Name = "DetailedView";
            Text = "Detailed View";
            base.Resize += new EventHandler(DetailedView_Resize);
            ((ISupportInitialize)segs).EndInit();
            contextMenuStrip.ResumeLayout(false);
            ((ISupportInitialize)finalSeg).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void menuItemMarkSegments_Click(object sender, EventArgs e)
        {
            Settings.Profile.DViewDeltaMarks = !Settings.Profile.DViewDeltaMarks;
            menuItemMarkSegments.Checked = Settings.Profile.DViewDeltaMarks;
            base.Invalidate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return parent.timerHotkey(keyData);
        }

        public void resetHeight()
        {
            finalSeg.Top = segs.Top + segs.Height;
            base.Height = finalSeg.Bottom + 0x22;
        }

        private void menuItemSelectColumns_Click(object sender, EventArgs e)
        {
            DViewSetColumnsDialog dialog = new DViewSetColumnsDialog();
            base.TopMost = false;
            parent.TopMost = false;
            parent.modalWindowOpened = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                parent.updateDetailed();
            }
            parent.TopMost = Settings.Profile.OnTop;
            base.TopMost = Settings.Profile.DViewOnTop;
            parent.modalWindowOpened = false;
        }

        private void menuItemSetColors_Click(object sender, EventArgs e)
        {
            CustomizeColors colorDialog = new CustomizeColors(true);
            parent.TopMost = false;
            base.TopMost = false;
            parent.modalWindowOpened = true;

            if (colorDialog.ShowDialog(this) == DialogResult.OK)
                parent.updateDetailed();

            parent.TopMost = Settings.Profile.OnTop;
            base.TopMost = Settings.Profile.DViewOnTop;
            parent.modalWindowOpened = false;
        }

        public void setDeltaPoints()
        {
            deltaPoints.Clear();
            double num = 0.0;
            double num2 = 0.0;
            foreach (double num3 in Deltas)
            {
                num = Math.Max(num3, num);
                num2 = Math.Min(num3, num2);
            }
            double num4 = parent.split.CompTime(parent.split.LastIndex);

            // "Temporary"? fix for the bug described below
            if (num4 != 0.0)
            {
                for (int i = 0; (i < Deltas.Count) && (i <= parent.split.LastIndex); i++)
                {
                    if ((parent.split.segments[i].LiveTime != 0.0) && (parent.split.CompTime(i) != 0.0))
                    {
                        // This next line causes a graphic crash if the last segment is empty and the segment i is not.
                        float x = (float)(parent.split.CompTime(i) / num4);
                        float y = 0.5f;
                        if ((num - num2) > 0.0)
                        {
                            y = (float)((Deltas[i] - num2) / (num - num2));
                        }
                        deltaPoints.Add(new PointF(x, y));
                    }
                }
            }

            if ((num - num2) > 0.0)
            {
                plusPct = (float)((0.0 - num2) / (num - num2));
            }
            else
            {
                plusPct = 0.5f;
            }
        }

        private void menuItemShowSegs_Click(object sender, EventArgs e)
        {
            Settings.Profile.DViewShowSegs = !Settings.Profile.DViewShowSegs;
            menuItemShowSegs.Checked = Settings.Profile.DViewShowSegs;
            parent.updateDetailed();
        }

        public void updateColumns()
        {
            int num = 0x2e;
            if ((segs.RowCount > 0) && (finalSeg.RowCount > 1))
            {
                if (Settings.Profile.DViewShowSegs)
                {
                    segs.Rows[0].Cells[2].Value = "Best [Seg]";
                    segs.Rows[0].Cells[4].Value = "Live [Seg]";
                }
                else
                {
                    segs.Rows[0].Cells[2].Value = "Best";
                    segs.Rows[0].Cells[4].Value = "Live";
                }

                // The detailed view used to only show a column if it had an ending time. I decided to change it, because
                // the user can still decide to show a column or not manually.
                if (Settings.Profile.DViewShowOld || (Settings.Profile.DViewShowComp && parent.split.ComparingType == RunSplits.CompareType.Old))
                {
                    segs.Columns[1].Visible = true;
                    num += 0x22;
                }
                else
                    segs.Columns[1].Visible = false;

                if (Settings.Profile.DViewShowBest || (Settings.Profile.DViewShowComp && parent.split.ComparingType == RunSplits.CompareType.Best))
                {
                    segs.Columns[2].Visible = true;
                    num += 0x22;
                    if (Settings.Profile.DViewShowSegs)
                    {
                        num += 0x24;
                    }
                }
                else
                    segs.Columns[2].Visible = false;

                if (Settings.Profile.DViewShowSumOfBests || (Settings.Profile.DViewShowComp && parent.split.ComparingType == RunSplits.CompareType.SumOfBests))
                {
                    segs.Columns[3].Visible = true;
                    num += 0x22;
                }
                else
                    segs.Columns[3].Visible = false;

                segs.Columns[4].Visible = Settings.Profile.DViewShowLive;
                if (segs.Columns[4].Visible)
                {
                    num += 0x22;
                    if (Settings.Profile.DViewShowSegs)
                        num += 0x24;
                }

                // Again, the deltas used to only show if the comparison time had an ending time. It got changed.
                segs.Columns[5].Visible = Settings.Profile.DViewShowDeltas;
                if (segs.Columns[5].Visible)
                    num += 0x20;
            }
            num = Math.Max(num, displayTime.Width);
            if (base.Size.Width == MinimumSize.Width)
            {
                MinimumSize = new Size(num, 0);
                base.Width = MinimumSize.Width;
            }
            else
            {
                int num2 = base.Width - MinimumSize.Width;
                MinimumSize = new Size(num, 0);
                base.Width = MinimumSize.Width + num2;
            }
            foreach (DataGridViewColumn column in finalSeg.Columns)
            {
                column.Visible = segs.Columns[column.Index].Visible;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                base.WndProc(ref m);
                Point point = base.PointToClient(new Point(m.LParam.ToInt32()));
                if ((point.X <= 5) && (point.X >= 0))
                {
                    m.Result = (IntPtr)10;
                }
                else if ((point.X >= (base.ClientSize.Width - 5)) && (point.X <= base.ClientSize.Width))
                {
                    m.Result = (IntPtr)11;
                }
                else if ((Control.MouseButtons != MouseButtons.Right) && (((int)m.Result) == 1))
                {
                    m.Result = (IntPtr)2;
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}