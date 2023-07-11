namespace WSplitTimer
{
    using Properties;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public class RunEditorDialog : Form
    {
        public TextBox attemptsBox;
        private DataGridViewTextBoxColumn best;
        private DataGridViewTextBoxColumn bseg;
        private readonly int cellHeight;
        private Button discardButton;
        private Control eCtl;
        public List<SplitSegment> editList = new List<SplitSegment>();
        private DataGridViewImageColumn icon;
        private DataGridViewTextBoxColumn iconPath;
        private Button insertButton;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label lblGoal;
        private Button offsetButton;
        private DataGridViewTextBoxColumn old;
        private TextBox oldOffset;
        private OpenFileDialog openIconDialog;
        private Button resetButton;
        private DataGridView runView;
        private Button saveButton;
        private DataGridViewTextBoxColumn segment;
        public TextBox titleBox;
        public TextBox txtGoal;

        private Button buttonAutoFillBests;
        private Button buttonImport;
        private ContextMenuStrip contextMenuImport;
        private ToolStripMenuItem menuItemImportLlanfair;
        private ToolStripMenuItem menuItemImportSplitterZ;
        private ToolStripMenuItem menuItemImportLiveSplit;

        private OpenFileDialog openFileDialog;
        private LiveSplitXMLReader xmlReader;

        private readonly int windowHeight;
        public int startDelay; //Temporary until I refactor the whole application...

        public RunEditorDialog(RunSplits splits)
        {
            xmlReader = new LiveSplitXMLReader();
            InitializeComponent();
            cellHeight = runView.RowTemplate.Height;
            windowHeight = (base.Height - (runView.Height - cellHeight)) - 2;
            MaximumSize = new Size(500, (15 * cellHeight) + windowHeight);

            foreach (SplitSegment segment in splits.segments)
                editList.Add(segment);

            populateList(editList);
            runView.EditingControlShowing += runView_EditingControlShowing;
        }

        private void attemptsBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void attemptsBox_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(attemptsBox.Text, "[^0-9]"))
                attemptsBox.Text = Regex.Replace(attemptsBox.Text, "[^0-9]", "");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void eCtl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (runView.CurrentCell.ColumnIndex == 0)
            {
                if (e.KeyChar == ',')
                    e.Handled = true;
            }
            else if (((!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) && ((e.KeyChar != ':') && (e.KeyChar != '.'))) && (e.KeyChar != ','))
                e.Handled = true;
        }

        private void eCtl_TextChanged(object sender, EventArgs e)
        {
            if (runView.CurrentCell.ColumnIndex == 0)
            {
                if (Regex.IsMatch(eCtl.Text, ","))
                    eCtl.Text = Regex.Replace(eCtl.Text, ",", "");
            }
            else if (Regex.IsMatch(eCtl.Text, "[^0-9:.,]"))
                eCtl.Text = Regex.Replace(eCtl.Text, "[^0-9:.,]", "");
        }

        private void InitializeComponent()
        {
            runView = new DataGridView();
            segment = new DataGridViewTextBoxColumn();
            old = new DataGridViewTextBoxColumn();
            best = new DataGridViewTextBoxColumn();
            bseg = new DataGridViewTextBoxColumn();
            iconPath = new DataGridViewTextBoxColumn();
            icon = new DataGridViewImageColumn();
            saveButton = new Button();
            discardButton = new Button();
            resetButton = new Button();
            oldOffset = new TextBox();
            label1 = new Label();
            offsetButton = new Button();
            titleBox = new TextBox();
            txtGoal = new TextBox();
            lblGoal = new Label();
            label2 = new Label();
            insertButton = new Button();
            openIconDialog = new OpenFileDialog();
            label3 = new Label();
            attemptsBox = new TextBox();
            buttonAutoFillBests = new Button();
            buttonImport = new Button();
            contextMenuImport = new ContextMenuStrip();
            menuItemImportLlanfair = new ToolStripMenuItem();
            menuItemImportSplitterZ = new ToolStripMenuItem();
            menuItemImportLiveSplit = new ToolStripMenuItem();
            openFileDialog = new OpenFileDialog();

            ((ISupportInitialize)runView).BeginInit();
            base.SuspendLayout();

            runView.AllowUserToResizeRows = false;
            runView.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            runView.BackgroundColor = SystemColors.Window;
            runView.BorderStyle = BorderStyle.Fixed3D;
            runView.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
            runView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            runView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            runView.Columns.AddRange(new DataGridViewColumn[] { segment, old, best, bseg, iconPath, icon });
            runView.Location = new Point(12, 0x3a);
            runView.Name = "runView";
            runView.RowHeadersVisible = false;
            runView.Size = new Size(0x167, 0x2a);
            runView.TabIndex = 0;
            runView.CellDoubleClick += new DataGridViewCellEventHandler(runView_CellDoubleClick);
            runView.UserAddedRow += new DataGridViewRowEventHandler(runView_UserAddedRow);
            runView.UserDeletedRow += new DataGridViewRowEventHandler(runView_UserDeletedRow);
            runView.KeyDown += new KeyEventHandler(runView_KeyDown);

            segment.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            segment.HeaderText = "Segment";
            segment.Name = "segment";
            segment.SortMode = DataGridViewColumnSortMode.NotSortable;

            old.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            old.HeaderText = "Old Time";
            old.Name = "old";
            old.SortMode = DataGridViewColumnSortMode.NotSortable;
            old.Width = 0x37;

            best.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            best.HeaderText = "Best Time";
            best.Name = "best";
            best.SortMode = DataGridViewColumnSortMode.NotSortable;
            best.Width = 60;

            bseg.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            bseg.HeaderText = "Best Seg.";
            bseg.Name = "bseg";
            bseg.SortMode = DataGridViewColumnSortMode.NotSortable;
            bseg.Width = 0x3b;

            iconPath.HeaderText = "Icon Path";
            iconPath.Name = "iconPath";
            iconPath.Visible = false;

            icon.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            icon.HeaderText = "Icon";
            icon.ImageLayout = DataGridViewImageCellLayout.Zoom;
            icon.MinimumWidth = 40;
            icon.Name = "icon";
            icon.ReadOnly = true;
            icon.Resizable = DataGridViewTriState.False;
            icon.Width = 40;

            saveButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            saveButton.DialogResult = DialogResult.OK;
            saveButton.Location = new Point(266, 136);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(50, 23);
            saveButton.TabIndex = 1;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += new EventHandler(saveButton_Click);

            discardButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            discardButton.DialogResult = DialogResult.Cancel;
            discardButton.Location = new Point(322, 136);
            discardButton.Name = "discardButton";
            discardButton.Size = new Size(50, 23);
            discardButton.TabIndex = 2;
            discardButton.Text = "Cancel";
            discardButton.UseVisualStyleBackColor = true;

            resetButton.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            resetButton.Location = new Point(266, 106);
            resetButton.Name = "resetButton";
            resetButton.Size = new Size(106, 23);
            resetButton.TabIndex = 3;
            resetButton.Text = "Reset";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += new EventHandler(resetButton_Click);

            buttonAutoFillBests.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            buttonAutoFillBests.Location = new Point(88, 106);
            buttonAutoFillBests.Name = "buttonAutoFillBests";
            buttonAutoFillBests.Size = new Size(120, 23);
            buttonAutoFillBests.TabIndex = 4;
            buttonAutoFillBests.Text = "Auto-fill best segments";
            buttonAutoFillBests.UseVisualStyleBackColor = true;
            buttonAutoFillBests.Click += buttonAutoFillBests_Click;

            buttonImport.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            //this.buttonImport.ContextMenuStrip = this.contextMenuImport;
            buttonImport.Location = new Point(12, 106);
            buttonImport.Name = "buttonImport";
            buttonImport.Size = new Size(70, 23);
            buttonImport.TabIndex = 5;
            buttonImport.Text = "Import... ▼";
            buttonImport.UseVisualStyleBackColor = true;
            buttonImport.MouseUp += buttonImport_MouseUp;

            contextMenuImport.Items.Add(menuItemImportLlanfair);
            contextMenuImport.Items.Add(menuItemImportSplitterZ);
            contextMenuImport.Items.Add(menuItemImportLiveSplit);
            contextMenuImport.Name = "contextMenuImport";

            //this.menuItemImportLlanfair.Enabled = false;
            menuItemImportLlanfair.Name = "menuItemImportLlanfair";
            menuItemImportLlanfair.Text = "Import from Llanfair";
            menuItemImportLlanfair.Click += menuItemImportLlanfair_Click;

            //this.menuItemImportSplitterZ.Enabled = false;
            menuItemImportSplitterZ.Name = "menuItemImportSplitterZ";
            menuItemImportSplitterZ.Text = "Import from SplitterZ";
            menuItemImportSplitterZ.Click += menuItemImportSplitterZ_Click;

            menuItemImportLiveSplit.Name = "menuItemImportLiveSplit";
            menuItemImportLiveSplit.Text = "Import from LiveSplit";
            menuItemImportLiveSplit.Click += menuItemImportLiveSplit_Click;

            oldOffset.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            oldOffset.Location = new Point(230, 0x1f);
            oldOffset.Name = "oldOffset";
            oldOffset.Size = new Size(0x56, 20);
            oldOffset.TabIndex = 5;
            oldOffset.TextChanged += new EventHandler(oldOffset_TextChanged);
            oldOffset.KeyDown += new KeyEventHandler(oldOffset_KeyDown);
            oldOffset.KeyPress += new KeyPressEventHandler(oldOffset_KeyPress);

            label1.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Location = new Point(230, 15);
            label1.Name = "label1";
            label1.Size = new Size(0x4d, 13);
            label1.TabIndex = 6;
            label1.Text = "Old time offset:";

            offsetButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            offsetButton.Location = new Point(0x142, 0x1d);
            offsetButton.Name = "offsetButton";
            offsetButton.Size = new Size(50, 0x17);
            offsetButton.TabIndex = 7;
            offsetButton.Text = "Apply";
            offsetButton.UseVisualStyleBackColor = true;
            offsetButton.Click += new EventHandler(offsetButton_Click);

            titleBox.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            titleBox.Location = new Point(12, 0x1f);
            titleBox.Name = "titleBox";
            titleBox.Size = new Size(0x6a, 20);
            titleBox.TabIndex = 8;

            txtGoal.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            txtGoal.Location = new Point(titleBox.Right + 6, 0x1f);
            txtGoal.Name = "txtGoal";
            txtGoal.Size = new Size(0x64, 20);
            txtGoal.TabIndex = 9;

            label2.AutoSize = true;
            label2.Location = new Point(12, 15);
            label2.Name = "label2";
            label2.Size = new Size(0x31, 13);
            label2.TabIndex = 10;
            label2.Text = "Run title:";

            lblGoal.AutoSize = true;
            lblGoal.Location = new Point(txtGoal.Left, 15);
            lblGoal.Name = "lblGoal";
            lblGoal.Size = new Size(0x31, 13);
            lblGoal.Text = "Goal:";

            insertButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            insertButton.Location = new Point(12, 136);
            insertButton.Name = "insertButton";
            insertButton.Size = new Size(50, 0x17);
            insertButton.TabIndex = 11;
            insertButton.Text = "Insert";
            insertButton.UseVisualStyleBackColor = true;
            insertButton.Click += new EventHandler(insertButton_Click);

            openIconDialog.Filter = "Image files (*.bmp; *.gif; *.jpg; *.png; *.tiff)|*.bmp;*.gif;*.jpg;*.png;*.tiff";

            label3.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            label3.AutoSize = true;
            label3.Location = new Point(0x44, 141);
            label3.Name = "label3";
            label3.Size = new Size(0x33, 13);
            label3.TabIndex = 12;
            label3.Text = "Attempts:";

            attemptsBox.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            attemptsBox.Location = new Point(0x7d, 138);
            attemptsBox.Name = "attemptsBox";
            attemptsBox.Size = new Size(40, 20);
            attemptsBox.TabIndex = 13;
            attemptsBox.Text = "0";
            attemptsBox.TextAlign = HorizontalAlignment.Right;
            attemptsBox.TextChanged += new EventHandler(attemptsBox_TextChanged);
            attemptsBox.KeyPress += new KeyPressEventHandler(attemptsBox_KeyPress);

            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(384, 171);
            base.Controls.Add(attemptsBox);
            base.Controls.Add(label3);
            base.Controls.Add(insertButton);
            base.Controls.Add(label2);
            base.Controls.Add(titleBox);
            base.Controls.Add(txtGoal);
            base.Controls.Add(lblGoal);
            base.Controls.Add(offsetButton);
            base.Controls.Add(label1);
            base.Controls.Add(oldOffset);
            base.Controls.Add(resetButton);
            base.Controls.Add(discardButton);
            base.Controls.Add(saveButton);
            base.Controls.Add(runView);
            base.Controls.Add(buttonAutoFillBests);
            base.Controls.Add(buttonImport);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            MinimumSize = new Size(390, 120);
            base.Name = "RunEditorDialog";
            Text = "Run Editor";
            base.Shown += new EventHandler(RunEditor_Shown);
            ((ISupportInitialize)runView).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void menuItemImportSplitterZ_Click(object sender, EventArgs e)
        {
            // Imports a file from a SplitterZ run file
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    using (FileStream stream = File.OpenRead(openFileDialog.FileName))
                    {
                        var reader = new StreamReader(stream);

                        var newLine = reader.ReadLine();
                        var title = newLine.Split(',');
                        titleBox.Text = title[0].Replace(@"‡", @",");

                        List<SplitSegment> segmentList = new List<SplitSegment>();

                        while ((newLine = reader.ReadLine()) != null)
                        {
                            var segmentInfo = newLine.Split(',');
                            var name = segmentInfo[0].Replace(@"‡", @",");
                            double splitTime = timeParse(segmentInfo[1]);
                            double bestSegment = timeParse(segmentInfo[2]);

                            var newSegment = new SplitSegment(name, 0.0, splitTime, bestSegment);
                            if (segmentInfo.Length > 3)
                            {
                                newSegment.IconPath = segmentInfo[3].Replace(@"‡", @",");
                                newSegment.Icon = Image.FromFile(newSegment.IconPath);
                            }
                            segmentList.Add(newSegment);
                        }
                        populateList(segmentList);
                    }
                }
                catch (Exception)
                {
                    // An error has occured...
                }
            }
        }

        private void menuItemImportLlanfair_Click(object sender, EventArgs e)
        {
            // Imports a file from a Llanfair run file
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    using (FileStream stream = File.OpenRead(openFileDialog.FileName))
                    {
                        short strLength;
                        byte[] buffer = new byte[128];

                        // Finds the goal string in the file
                        stream.Seek(0xC5, SeekOrigin.Current);  // Skips to the length of the goal string

                        stream.Read(buffer, 0, 2);
                        strLength = BitConverter.ToInt16(toConverterEndianness(buffer, 0, 2), 0);
                        stream.Read(buffer, 0, strLength);

                        string strGoal = Encoding.UTF8.GetString(buffer, 0, strLength);

                        // Finds the title string in the file
                        stream.Seek(0x1, SeekOrigin.Current);   // Skips to the length of the title string

                        stream.Read(buffer, 0, 2);
                        strLength = BitConverter.ToInt16(toConverterEndianness(buffer, 0, 2), 0);
                        stream.Read(buffer, 0, strLength);

                        string strTitle = Encoding.UTF8.GetString(buffer, 0, strLength);

                        // Finds the number of elements in the segment list
                        stream.Seek(0x6, SeekOrigin.Current);

                        stream.Read(buffer, 0, 4);

                        int segmentListCount = BitConverter.ToInt32(toConverterEndianness(buffer, 0, 4), 0);

                        // The object header changes if there is no instance of one of the object used by the Run class.
                        // The 2 objects that can be affected are the Time object and the ImageIcon object.
                        // The next step of the import algorythm is to check for their presence.
                        bool timeObjectDeclarationEncountered = false;
                        bool iconObjectDeclarationEncountered = false;

                        List<SplitSegment> segmentList = new List<SplitSegment>();

                        // Seeks to the first byte of the first segment
                        stream.Seek(0x8F, SeekOrigin.Current);
                        for (int i = 0; i < segmentListCount; ++i)
                        {
                            long bestSegmentMillis = 0;
                            stream.Read(buffer, 0, 1);
                            if (buffer[0] != 0x70)
                            {
                                if (!timeObjectDeclarationEncountered)
                                {
                                    timeObjectDeclarationEncountered = true;

                                    // Seek past the object declaration
                                    stream.Seek(0x36, SeekOrigin.Current);
                                }
                                else
                                    stream.Seek(0x5, SeekOrigin.Current);

                                // Read the remaining 7 bytes of data in the buffer:
                                stream.Read(buffer, 0, 8);
                                bestSegmentMillis = BitConverter.ToInt64(toConverterEndianness(buffer, 0, 8), 0);
                            }

                            stream.Read(buffer, 0, 1);
                            if (buffer[0] != 0x70)
                            {
                                long seekOffsetBase;
                                if (!iconObjectDeclarationEncountered)
                                {
                                    iconObjectDeclarationEncountered = true;

                                    stream.Seek(0xBC, SeekOrigin.Current);
                                    seekOffsetBase = 0x25;
                                }
                                else
                                {
                                    stream.Seek(0x5, SeekOrigin.Current);
                                    seekOffsetBase = 0x18;
                                }

                                stream.Read(buffer, 0, 8);
                                int iconHeight = BitConverter.ToInt32(toConverterEndianness(buffer, 0, 4), 0);
                                int iconWidth = BitConverter.ToInt32(toConverterEndianness(buffer, 4, 4), 4);

                                // Seek past the image:
                                stream.Seek(seekOffsetBase + (iconHeight * iconWidth * 4), SeekOrigin.Current);
                            }

                            // Finds the name of the segment (can't be empty)
                            stream.Seek(0x1, SeekOrigin.Current);   // Skip to the length of the name string
                            stream.Read(buffer, 0, 2);
                            strLength = BitConverter.ToInt16(toConverterEndianness(buffer, 0, 2), 0);
                            stream.Read(buffer, 0, strLength);

                            string name = Encoding.UTF8.GetString(buffer, 0, strLength);

                            // Finds the best time of the segment
                            long bestTimeMillis = 0;
                            stream.Read(buffer, 0, 1);
                            if (buffer[0] == 0x71)
                            {
                                stream.Seek(0x4, SeekOrigin.Current);
                                bestTimeMillis = bestSegmentMillis;
                            }
                            else if (buffer[0] != 0x70)
                            {
                                // Since there is always a best segment when there is a best time in Llanfair,
                                // I assume that there will never be another Time object declaration before this data.
                                stream.Seek(0x5, SeekOrigin.Current);
                                stream.Read(buffer, 0, 8);
                                bestTimeMillis = BitConverter.ToInt64(toConverterEndianness(buffer, 0, 8), 0);
                            }

                            double bestTime = bestTimeMillis / 1000.0;

                            if (bestTimeMillis != 0)
                            {
                                for (int j = i - 1; j >= 0; --j)
                                {
                                    if (segmentList[j].BestTime != 0.0)
                                    {
                                        bestTime += segmentList[j].BestTime;
                                        break;
                                    }
                                }
                            }

                            segmentList.Add(new SplitSegment(name, 0.0, bestTime, bestSegmentMillis / 1000.0));

                            // Seek to the beginning of the next segment name
                            stream.Seek(0x6, SeekOrigin.Current);
                        }

                        // The only remaining thing in the file should be the window height and width for Llanfair usage.
                        // We don't need to extract it.

                        populateList(segmentList);
                        titleBox.Text = strTitle;
                        txtGoal.Text = strGoal;
                    }
                }
                catch (Exception)
                {
                    // An error has occured...
                }
            }
        }

        private void menuItemImportLiveSplit_Click(object sender, EventArgs e)
        {
            RunSplits split = null;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                using (FileStream stream = File.OpenRead(openFileDialog.FileName))
                {
                    xmlReader = new LiveSplitXMLReader();
                    split = xmlReader.ReadSplit(openFileDialog.FileName);
                }
            }
            if (split != null)
            {
                titleBox.Text = split.RunTitle;
                txtGoal.Text = split.RunGoal;
                attemptsBox.Text = split.AttemptsCount.ToString();
                populateList(split.segments);
                startDelay = split.StartDelay;
            }
            else
            {
                MessageBox.Show("The import from livesplit has failed.");
            }
        }

        private byte[] toConverterEndianness(byte[] array, int offset, int length)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] newArray = (byte[])array.Clone();
                Array.Reverse(newArray, offset, length);
                return newArray;
            }

            return array;
        }

        private void buttonImport_MouseUp(object sender, MouseEventArgs e)
        {
            contextMenuImport.Show(this, new Point(buttonImport.Location.X, buttonImport.Location.Y + (buttonImport.ClientRectangle.Height - 1)));
        }

        private void buttonAutoFillBests_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx.Show(this, "Are you sure?", "Auto-fill best segments", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                List<SplitSegment> splitList = new List<SplitSegment>();
                FillSplitList(ref splitList);

                for (int i = 0; i < splitList.Count; ++i)
                {
                    double segmentTime = 0.0;

                    if (i == 0)
                        segmentTime = splitList[i].BestTime;
                    else if (splitList[i].BestTime != 0.0 && splitList[i - 1].BestTime != 0.0)
                        segmentTime = splitList[i].BestTime - splitList[i - 1].BestTime;

                    if (splitList[i].BestSegment == 0.0 || (segmentTime != 0.0 && segmentTime < splitList[i].BestSegment))
                        splitList[i].BestSegment = segmentTime;
                }

                populateList(splitList);
            }
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            if (runView.SelectedCells.Count > 0)
            {
                new DataGridViewRow();
                runView.Rows.Insert(runView.SelectedCells[0].RowIndex, new object[0]);
                base.Height = (Math.Min(15, runView.Rows.Count) * cellHeight) + windowHeight;
                runView.CurrentCell = runView.Rows[runView.SelectedCells[0].RowIndex - 1].Cells[0];
            }
        }

        private void offsetButton_Click(object sender, EventArgs e)
        {
            if (oldOffset.Text.Length != 0)
            {
                foreach (DataGridViewRow row in runView.Rows)
                {
                    if (row.Cells[1].Value != null)
                        row.Cells[1].Value = timeFormat(Math.Max((double)0.0, (double)(timeParse(row.Cells[1].Value.ToString()) - timeParse(oldOffset.Text))));
                }
                oldOffset.Text = "";
            }
        }

        private void oldOffset_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (oldOffset.Text.Length != 0))
            {
                foreach (DataGridViewRow row in runView.Rows)
                {
                    if (row.Cells[1].Value != null)
                        row.Cells[1].Value = timeFormat(Math.Max((double)0.0, (double)(timeParse(row.Cells[1].Value.ToString()) - timeParse(oldOffset.Text))));
                }
                oldOffset.Text = "";
                e.SuppressKeyPress = true;
            }
        }

        private void oldOffset_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) && ((e.KeyChar != ':') && (e.KeyChar != '.'))) && (e.KeyChar != ','))
                e.Handled = true;
        }

        private void oldOffset_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(oldOffset.Text, "[^0-9:.,]"))
                oldOffset.Text = Regex.Replace(oldOffset.Text, "[^0-9:.,]", "");
        }

        private void populateList(List<SplitSegment> splitList)
        {
            runView.Rows.Clear();
            runView.Rows[0].Cells[5].Value = Resources.MissingIcon;
            foreach (SplitSegment segment in splitList)
            {
                if (segment.Name != null)
                {
                    string name = segment.Name;
                    string str2 = "";
                    string str3 = "";
                    string str4 = "";
                    string iconPath = "";
                    Image missingIcon = Resources.MissingIcon;

                    if (segment.OldTime != 0.0)
                        str2 = timeFormat(segment.OldTime);

                    if (segment.BestTime != 0.0)
                        str3 = timeFormat(segment.BestTime);

                    if (segment.BestSegment != 0.0)
                        str4 = timeFormat(segment.BestSegment);

                    if ((segment.IconPath != null) && (segment.IconPath.Length > 1))
                    {
                        iconPath = segment.IconPath;
                        try
                        {
                            missingIcon = Image.FromFile(segment.IconPath);
                        }
                        catch
                        { }
                    }
                    runView.Rows.Add(new object[] { name, str2, str3, str4, iconPath, missingIcon });
                }
            }

            base.Height = (Math.Min(15, runView.Rows.Count) * cellHeight) + windowHeight;
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            populateList(editList);
        }

        private void RunEditor_Shown(object sender, EventArgs e)
        {
            base.BringToFront();
        }

        private void runView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == 5) && (e.RowIndex >= 0))
            {
                if (runView.Rows[e.RowIndex].Cells[0].Value != null)
                    openIconDialog.Title = "Set Icon for " + runView.Rows[e.RowIndex].Cells[0].Value.ToString() + "...";
                else
                    openIconDialog.Title = "Set Icon...";

                if (openIconDialog.ShowDialog() == DialogResult.OK)
                {
                    runView.Rows[e.RowIndex].Cells[4].Value = openIconDialog.FileName;
                    Image missingIcon = Resources.MissingIcon;
                    try
                    {
                        missingIcon = Image.FromFile(openIconDialog.FileName);
                    }
                    catch
                    { }

                    runView.Rows[e.RowIndex].Cells[5].Value = missingIcon;
                }
            }
        }

        private void runView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            eCtl = e.Control;
            eCtl.TextChanged -= new EventHandler(eCtl_TextChanged);
            eCtl.KeyPress -= new KeyPressEventHandler(eCtl_KeyPress);
            eCtl.TextChanged += new EventHandler(eCtl_TextChanged);
            eCtl.KeyPress += new KeyPressEventHandler(eCtl_KeyPress);
        }

        private void runView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewCell cell in runView.SelectedCells)
                {
                    if (((cell.RowIndex >= 0) && (cell.ColumnIndex >= 0)) && !runView.Rows[cell.RowIndex].IsNewRow)
                    {
                        if (cell.ColumnIndex == 0)
                            runView.Rows.RemoveAt(cell.RowIndex);
                        else if (cell.ColumnIndex == 5)
                        {
                            cell.Value = Resources.MissingIcon;
                            runView.Rows[cell.RowIndex].Cells[4].Value = "";
                        }
                        else
                            cell.Value = null;
                    }
                }
                base.Height = (Math.Min(15, runView.Rows.Count) * cellHeight) + windowHeight;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                for (int i = runView.SelectedCells.Count - 1; i >= 0; i--)
                {
                    DataGridViewCell cell2 = runView.SelectedCells[i];
                    if ((cell2.RowIndex >= 0) && (cell2.ColumnIndex == 5))
                    {
                        if (runView.Rows[cell2.RowIndex].Cells[0].Value != null)
                            openIconDialog.Title = "Set Icon for " + runView.Rows[cell2.RowIndex].Cells[0].Value.ToString() + "...";
                        else
                            openIconDialog.Title = "Set Icon...";

                        if (openIconDialog.ShowDialog() != DialogResult.OK)
                            break;

                        runView.Rows[cell2.RowIndex].Cells[4].Value = openIconDialog.FileName;
                        Image missingIcon = Resources.MissingIcon;

                        try
                        {
                            missingIcon = Image.FromFile(openIconDialog.FileName);
                        }
                        catch
                        { }
                        cell2.Value = missingIcon;
                    }
                }
            }
        }

        private void runView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            base.Height = (Math.Min(15, runView.Rows.Count) * cellHeight) + windowHeight;
            e.Row.Cells[5].Value = Resources.MissingIcon;
        }

        private void runView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            base.Height = (Math.Min(15, runView.Rows.Count) * cellHeight) + windowHeight;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            editList.Clear();
            FillSplitList(ref editList);
        }

        private void FillSplitList(ref List<SplitSegment> splitList)
        {
            foreach (DataGridViewRow row in runView.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    Bitmap missingIcon = Resources.MissingIcon;
                    SplitSegment item = new SplitSegment(row.Cells[0].Value.ToString());
                    if (row.Cells[1].Value != null)
                        item.OldTime = timeParse(row.Cells[1].Value.ToString());

                    if (row.Cells[2].Value != null)
                        item.BestTime = timeParse(row.Cells[2].Value.ToString());

                    if (row.Cells[3].Value != null)
                        item.BestSegment = timeParse(row.Cells[3].Value.ToString());

                    if (row.Cells[4].Value != null)
                    {
                        item.IconPath = row.Cells[4].Value.ToString();
                        try
                        {
                            item.Icon = Image.FromFile(item.IconPath);
                        }
                        catch
                        {
                            item.Icon = Resources.MissingIcon;
                        }
                    }
                    else
                        item.Icon = Resources.MissingIcon;

                    splitList.Add(item);
                }
            }
        }

        private string timeFormat(double secs)
        {
            // TODO: determine if this should be updated to support displaying time in seconds whole, tenth, or miliseconds
            TimeSpan span = TimeSpan.FromSeconds(Math.Truncate(secs * 100) / 100);
            //TimeSpan span = TimeSpan.FromSeconds(Math.Round(secs, 2));
            if (span.TotalHours >= 1.0)
                return string.Format("{0}:{1:00}:{2:00.00}", Math.Floor(span.TotalHours), span.Minutes, span.Seconds + (span.Milliseconds / 1000.0));

            if (span.TotalMinutes >= 1.0)
                return string.Format("{0}:{1:00.00}", Math.Floor(span.TotalMinutes), span.Seconds + (span.Milliseconds / 1000.0));

            return string.Format("{0:0.00}", span.TotalSeconds);
        }

        private double timeParse(string timeString)
        {
            double num = 0.0;
            foreach (string str in timeString.Split(new char[] { ':' }))
            {
                double num2;
                if (double.TryParse(str, out num2))
                    num = (num * 60.0) + num2;
            }
            return num;
        }
    }
}