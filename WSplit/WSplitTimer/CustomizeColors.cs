namespace WSplitTimer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    public class CustomizeColors : Form
    {
        // Data
        private readonly Font clockL = new Font("Arial", 20f, FontStyle.Bold, GraphicsUnit.Pixel);

        private readonly Font clockM = new Font("Arial", 16f, FontStyle.Bold, GraphicsUnit.Pixel);
        private readonly Font displayFont = new Font("Arial", 9.333333f, GraphicsUnit.Pixel);
        private readonly Font timeFont = new Font("Arial", 10.66667f, FontStyle.Bold, GraphicsUnit.Pixel);
        private ClockType previewClockType;
        private readonly List<SettingPair> ColorSettings = new List<SettingPair>();
        private readonly ColorConverter converter = new ColorConverter();
        private readonly int segHeight = 14;

        // Controls
        private PictureBox picBoxPreview;

        private Label labelPreview;
        private CheckBox checkBoxPlainBg;
        private Button buttonDefaultColors;
        private Button buttonLoad;
        private Button buttonSave;
        private Button buttonCancel;
        private Button buttonOk;

        private TabControl colorTabs;
        private TabPage tabPageClockColors;
        private TabPage tabPageSegColors;
        private TabPage tabPageDetailedViewColors;

        // Timer Tab
        private Label labelColumnFore;

        private Label labelColumnColors;
        private Label labelColumnPlain;

        private GroupBox groupBoxBackground;

        private Label labelAhead;
        private PictureBox picBoxAheadFore;
        private PictureBox picBoxAheadBack;
        private PictureBox picBoxAheadBack2;
        private PictureBox picBoxAheadBackPlain;

        private Label labelAheadLosing;
        private PictureBox picBoxAheadLosingFore;
        private PictureBox picBoxAheadLosingBack;
        private PictureBox picBoxAheadLosingBack2;
        private PictureBox picBoxAheadLosingBackPlain;

        private Label labelBehind;
        private PictureBox picBoxBehindFore;
        private PictureBox picBoxBehindBack;
        private PictureBox picBoxBehindBack2;
        private PictureBox picBoxBehindBackPlain;

        private Label labelBehindLosing;
        private PictureBox picBoxBehindLosingFore;
        private PictureBox picBoxBehindLosingBack;
        private PictureBox picBoxBehindLosingBack2;
        private PictureBox picBoxBehindLosingBackPlain;

        private Label labelNoLoaded;
        private PictureBox picBoxNoLoadedBack;
        private PictureBox picBoxNoLoadedBack2;
        private PictureBox picBoxNoLoadedBackPlain;
        private PictureBox picBoxNoLoadedFore;

        private Label labelFinished;
        private PictureBox picBoxFinishedFore;
        private PictureBox picBoxFinishedBack;
        private PictureBox picBoxFinishedBack2;
        private PictureBox picBoxFinishedBackPlain;

        private Label labelRecord;
        private PictureBox picBoxRecordFore;
        private PictureBox picBoxRecordBack;
        private PictureBox picBoxRecordBack2;
        private PictureBox picBoxRecordBackPlain;

        private Label labelDelay;
        private PictureBox picBoxDelayFore;
        private PictureBox picBoxDelayBack;
        private PictureBox picBoxDelayBack2;
        private PictureBox picBoxDelayBackPlain;

        private Label labelPaused;
        private PictureBox picBoxPaused;

        private Label labelFlash;
        private PictureBox picBoxFlash;

        private Label labelStatusBar;
        private PictureBox picBoxStatusBarFore;
        private PictureBox picBoxStatusBarBack;
        private PictureBox picBoxStatusBarBack2;
        private PictureBox picBoxStatusBarBackPlain;

        private Label labelRunTitle;
        private PictureBox picBoxRunTitleFore;
        private PictureBox picBoxRunTitleBack;
        private PictureBox picBoxRunTitleBack2;
        private PictureBox picBoxRunTitleBackPlain;
        //private PictureBox picturebox1;

        // Segment Tab
        private Label labelColumnSegColor;

        private Label labelColumnSegColor2;
        private Label labelColumnSegPlain;

        private Label labelSegBackground;
        private PictureBox picBoxSegBackground;
        private PictureBox picBoxSegBackground2;
        private PictureBox picBoxSegBackgroundPlain;

        private Label labelSegHighlight;
        private PictureBox picBoxSegHighlight;
        private PictureBox picBoxSegHighlight2;
        private PictureBox picBoxSegHighlightPlain;

        private Label labelSegHighlightBorder;
        private PictureBox picBoxSegHighlightBorder;

        private Label labelSegPastText;
        private PictureBox picBoxSegPastText;

        private Label labelSegLiveText;
        private PictureBox picBoxSegLiveText;

        private Label labelSegFutureText;
        private PictureBox picBoxSegFutureText;

        private Label labelSegFutureTime;
        private PictureBox picBoxSegFutureTime;

        private Label labelSegNewTime;
        private PictureBox picBoxSegNewTime;

        private Label labelSegMissing;
        private PictureBox picBoxSegMissing;

        private Label labelSegBestSegment;
        private PictureBox picBoxSegBestSegment;

        private Label labelSegAheadGain;
        private PictureBox picBoxSegAheadGain;

        private Label labelSegAheadLoss;
        private PictureBox picBoxSegAheadLoss;

        private Label labelSegBehindGain;
        private PictureBox picBoxSegBehindGain;

        private Label labelSegBehindLoss;
        private PictureBox picBoxSegBehindLoss;

        // Detailed view tab
        private CheckBox checkBoxDViewUsePrimary;

        private GroupBox groupBoxDViewClock;
        private GroupBox groupBoxDViewSegments;
        private GroupBox groupBoxGraph;

        private Label labelDViewAhead;
        private PictureBox picBoxDViewAhead;

        private Label labelDViewAheadLosing;
        private PictureBox picBoxDViewAheadLosing;

        private Label labelDViewBehind;
        private PictureBox picBoxDViewBehind;

        private Label labelDViewBehindLosing;
        private PictureBox picBoxDViewBehindLosing;

        private Label labelDViewFinished;
        private PictureBox picBoxDViewFinished;

        private Label labelDViewRecord;
        private PictureBox picBoxDViewRecord;

        private Label labelDViewDelay;
        private PictureBox picBoxDViewDelay;

        private Label labelDViewPaused;
        private PictureBox picBoxDViewPaused;

        private Label labelDViewFlash;
        private PictureBox picBoxDViewFlash;

        private Label labelDViewSegCurrentText;
        private PictureBox picBoxDViewSegCurrentText;

        private Label labelDViewSegDefaultText;
        private PictureBox picBoxDViewSegDefaultText;

        private Label labelDViewSegMissingTime;
        private PictureBox picBoxDViewSegMissingTime;

        private Label labelDViewSegBestSegment;
        private PictureBox picBoxDViewSegBestSegment;

        private Label labelDViewSegAheadGain;
        private PictureBox picBoxDViewSegAheadGain;

        private Label labelDViewSegAheadLoss;
        private PictureBox picBoxDViewSegAheadLoss;

        private Label labelDViewSegBehindGain;
        private PictureBox picBoxDViewSegBehindGain;

        private Label labelDViewSegBehindLoss;
        private PictureBox picBoxDViewSegBehindLoss;

        private Label labelDViewSegHighlight;
        private PictureBox picBoxDViewSegHighlight;

        private Label labelGraphAhead;
        private PictureBox picBoxGraphAhead;

        private Label labelGraphBehind;
        private PictureBox picBoxGraphBehind;

        private PictureBox picturebox1;

        public CustomizeColors(bool selectDViewTab = false)
        {
            InitializeComponent();
            PopulateSettings();
            PopulateColors();
            if (selectDViewTab)
                colorTabs.SelectedTab = tabPageDetailedViewColors;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // Global Controls:
            picBoxPreview = new PictureBox();

            labelPreview = new Label();
            checkBoxPlainBg = new CheckBox();
            buttonDefaultColors = new Button();
            buttonLoad = new Button();
            buttonSave = new Button();
            buttonCancel = new Button();
            buttonOk = new Button();

            colorTabs = new TabControl();
            tabPageClockColors = new TabPage();
            tabPageSegColors = new TabPage();
            tabPageDetailedViewColors = new TabPage();

            // Timer tab:
            labelColumnFore = new Label();
            labelColumnColors = new Label();
            labelColumnPlain = new Label();

            groupBoxBackground = new GroupBox();

            labelAhead = new Label();
            picBoxAheadFore = new PictureBox();
            picBoxAheadBack = new PictureBox();
            picBoxAheadBack2 = new PictureBox();
            picBoxAheadBackPlain = new PictureBox();

            labelAheadLosing = new Label();
            picBoxAheadLosingFore = new PictureBox();
            picBoxAheadLosingBack = new PictureBox();
            picBoxAheadLosingBack2 = new PictureBox();
            picBoxAheadLosingBackPlain = new PictureBox();

            labelBehind = new Label();
            picBoxBehindFore = new PictureBox();
            picBoxBehindBack = new PictureBox();
            picBoxBehindBack2 = new PictureBox();
            picBoxBehindBackPlain = new PictureBox();

            labelBehindLosing = new Label();
            picBoxBehindLosingFore = new PictureBox();
            picBoxBehindLosingBack = new PictureBox();
            picBoxBehindLosingBack2 = new PictureBox();
            picBoxBehindLosingBackPlain = new PictureBox();

            labelNoLoaded = new Label();
            picBoxNoLoadedFore = new PictureBox();
            picBoxNoLoadedBack = new PictureBox();
            picBoxNoLoadedBack2 = new PictureBox();
            picBoxNoLoadedBackPlain = new PictureBox();

            labelFinished = new Label();
            picBoxFinishedFore = new PictureBox();
            picBoxFinishedBack = new PictureBox();
            picBoxFinishedBack2 = new PictureBox();
            picBoxFinishedBackPlain = new PictureBox();

            labelRecord = new Label();
            picBoxRecordFore = new PictureBox();
            picBoxRecordBack = new PictureBox();
            picBoxRecordBack2 = new PictureBox();
            picBoxRecordBackPlain = new PictureBox();

            labelDelay = new Label();
            picBoxDelayFore = new PictureBox();
            picBoxDelayBack = new PictureBox();
            picBoxDelayBack2 = new PictureBox();
            picBoxDelayBackPlain = new PictureBox();

            labelPaused = new Label();
            picBoxPaused = new PictureBox();

            labelFlash = new Label();
            picBoxFlash = new PictureBox();

            labelStatusBar = new Label();
            picBoxStatusBarFore = new PictureBox();
            picBoxStatusBarBack = new PictureBox();
            picBoxStatusBarBack2 = new PictureBox();
            picBoxStatusBarBackPlain = new PictureBox();

            labelRunTitle = new Label();
            picBoxRunTitleFore = new PictureBox();
            picBoxRunTitleBack = new PictureBox();
            picBoxRunTitleBack2 = new PictureBox();
            picBoxRunTitleBackPlain = new PictureBox();

            // Segment Tab:
            labelColumnSegColor = new Label();
            labelColumnSegColor2 = new Label();
            labelColumnSegPlain = new Label();

            labelSegBackground = new Label();
            picBoxSegBackground = new PictureBox();
            picBoxSegBackground2 = new PictureBox();
            picBoxSegBackgroundPlain = new PictureBox();

            labelSegHighlight = new Label();
            picBoxSegHighlight = new PictureBox();
            picBoxSegHighlight2 = new PictureBox();
            picBoxSegHighlightPlain = new PictureBox();

            labelSegHighlightBorder = new Label();
            picBoxSegHighlightBorder = new PictureBox();

            labelSegPastText = new Label();
            picBoxSegPastText = new PictureBox();

            labelSegLiveText = new Label();
            picBoxSegLiveText = new PictureBox();

            labelSegFutureText = new Label();
            picBoxSegFutureText = new PictureBox();

            labelSegFutureTime = new Label();
            picBoxSegFutureTime = new PictureBox();

            labelSegNewTime = new Label();
            picBoxSegNewTime = new PictureBox();

            labelSegMissing = new Label();
            picBoxSegMissing = new PictureBox();

            labelSegBestSegment = new Label();
            picBoxSegBestSegment = new PictureBox();

            labelSegAheadGain = new Label();
            picBoxSegAheadGain = new PictureBox();

            labelSegAheadLoss = new Label();
            picBoxSegAheadLoss = new PictureBox();

            labelSegBehindGain = new Label();
            picBoxSegBehindGain = new PictureBox();

            labelSegBehindLoss = new Label();
            picBoxSegBehindLoss = new PictureBox();

            labelGraphAhead = new Label();
            picBoxGraphAhead = new PictureBox();

            labelGraphBehind = new Label();
            picBoxGraphBehind = new PictureBox();

            picturebox1 = new PictureBox();

            // Detailed View tab:
            checkBoxDViewUsePrimary = new CheckBox();

            groupBoxDViewClock = new GroupBox();
            groupBoxDViewSegments = new GroupBox();
            groupBoxGraph = new GroupBox();

            labelDViewAhead = new Label();
            picBoxDViewAhead = new PictureBox();

            labelDViewAheadLosing = new Label();
            picBoxDViewAheadLosing = new PictureBox();

            labelDViewBehind = new Label();
            picBoxDViewBehind = new PictureBox();

            labelDViewBehindLosing = new Label();
            picBoxDViewBehindLosing = new PictureBox();

            labelDViewFinished = new Label();
            picBoxDViewFinished = new PictureBox();

            labelDViewRecord = new Label();
            picBoxDViewRecord = new PictureBox();

            labelDViewDelay = new Label();
            picBoxDViewDelay = new PictureBox();

            labelDViewPaused = new Label();
            picBoxDViewPaused = new PictureBox();

            labelDViewFlash = new Label();
            picBoxDViewFlash = new PictureBox();

            labelDViewSegCurrentText = new Label();
            picBoxDViewSegCurrentText = new PictureBox();

            labelDViewSegDefaultText = new Label();
            picBoxDViewSegDefaultText = new PictureBox();

            labelDViewSegMissingTime = new Label();
            picBoxDViewSegMissingTime = new PictureBox();

            labelDViewSegBestSegment = new Label();
            picBoxDViewSegBestSegment = new PictureBox();

            labelDViewSegAheadGain = new Label();
            picBoxDViewSegAheadGain = new PictureBox();

            labelDViewSegAheadLoss = new Label();
            picBoxDViewSegAheadLoss = new PictureBox();

            labelDViewSegBehindGain = new Label();
            picBoxDViewSegBehindGain = new PictureBox();

            labelDViewSegBehindLoss = new Label();
            picBoxDViewSegBehindLoss = new PictureBox();

            labelDViewSegHighlight = new Label();
            picBoxDViewSegHighlight = new PictureBox();

            // Starting the set up:
            colorTabs.SuspendLayout();
            tabPageClockColors.SuspendLayout();
            tabPageSegColors.SuspendLayout();
            tabPageDetailedViewColors.SuspendLayout();
            groupBoxBackground.SuspendLayout();
            groupBoxDViewClock.SuspendLayout();
            groupBoxDViewSegments.SuspendLayout();
            groupBoxGraph.SuspendLayout();
            ((ISupportInitialize)picBoxAheadBackPlain).BeginInit();
            ((ISupportInitialize)picBoxRunTitleBackPlain).BeginInit();
            ((ISupportInitialize)picBoxAheadLosingBackPlain).BeginInit();
            ((ISupportInitialize)picBoxBehindBackPlain).BeginInit();
            ((ISupportInitialize)picBoxBehindLosingBackPlain).BeginInit();
            ((ISupportInitialize)picBoxStatusBarBackPlain).BeginInit();
            ((ISupportInitialize)picBoxNoLoadedBackPlain).BeginInit();
            ((ISupportInitialize)picBoxFinishedBackPlain).BeginInit();
            ((ISupportInitialize)picBoxRecordBackPlain).BeginInit();
            ((ISupportInitialize)picBoxDelayBackPlain).BeginInit();
            ((ISupportInitialize)picBoxAheadBack2).BeginInit();
            ((ISupportInitialize)picBoxRunTitleBack2).BeginInit();
            ((ISupportInitialize)picBoxAheadLosingBack2).BeginInit();
            ((ISupportInitialize)picBoxBehindBack2).BeginInit();
            ((ISupportInitialize)picBoxBehindLosingBack2).BeginInit();
            ((ISupportInitialize)picBoxStatusBarBack2).BeginInit();
            ((ISupportInitialize)picBoxNoLoadedBack2).BeginInit();
            ((ISupportInitialize)picBoxFinishedBack2).BeginInit();
            ((ISupportInitialize)picBoxRecordBack2).BeginInit();
            ((ISupportInitialize)picBoxDelayBack2).BeginInit();
            ((ISupportInitialize)picBoxAheadBack).BeginInit();
            ((ISupportInitialize)picBoxRunTitleBack).BeginInit();
            ((ISupportInitialize)picBoxAheadLosingBack).BeginInit();
            ((ISupportInitialize)picBoxBehindBack).BeginInit();
            ((ISupportInitialize)picBoxBehindLosingBack).BeginInit();
            ((ISupportInitialize)picBoxStatusBarBack).BeginInit();
            ((ISupportInitialize)picBoxNoLoadedBack).BeginInit();
            ((ISupportInitialize)picBoxFinishedBack).BeginInit();
            ((ISupportInitialize)picBoxRecordBack).BeginInit();
            ((ISupportInitialize)picBoxDelayBack).BeginInit();
            ((ISupportInitialize)picBoxRunTitleFore).BeginInit();
            ((ISupportInitialize)picBoxStatusBarFore).BeginInit();
            ((ISupportInitialize)picBoxDelayFore).BeginInit();
            ((ISupportInitialize)picBoxRecordFore).BeginInit();
            ((ISupportInitialize)picBoxFinishedFore).BeginInit();
            ((ISupportInitialize)picBoxFlash).BeginInit();
            ((ISupportInitialize)picBoxPaused).BeginInit();
            ((ISupportInitialize)picBoxNoLoadedFore).BeginInit();
            ((ISupportInitialize)picBoxBehindLosingFore).BeginInit();
            ((ISupportInitialize)picBoxBehindFore).BeginInit();
            ((ISupportInitialize)picBoxAheadLosingFore).BeginInit();
            ((ISupportInitialize)picBoxAheadFore).BeginInit();
            ((ISupportInitialize)picBoxSegHighlightPlain).BeginInit();
            ((ISupportInitialize)picBoxSegBackgroundPlain).BeginInit();
            ((ISupportInitialize)picBoxSegHighlight2).BeginInit();
            ((ISupportInitialize)picBoxSegBackground2).BeginInit();
            ((ISupportInitialize)picBoxSegHighlightBorder).BeginInit();
            ((ISupportInitialize)picBoxSegBehindLoss).BeginInit();
            ((ISupportInitialize)picBoxSegBehindGain).BeginInit();
            ((ISupportInitialize)picBoxSegAheadLoss).BeginInit();
            ((ISupportInitialize)picBoxSegAheadGain).BeginInit();
            ((ISupportInitialize)picBoxSegMissing).BeginInit();
            ((ISupportInitialize)picBoxSegNewTime).BeginInit();
            ((ISupportInitialize)picBoxSegBestSegment).BeginInit();
            ((ISupportInitialize)picBoxSegFutureTime).BeginInit();
            ((ISupportInitialize)picBoxSegFutureText).BeginInit();
            ((ISupportInitialize)picBoxSegLiveText).BeginInit();
            ((ISupportInitialize)picBoxSegPastText).BeginInit();
            ((ISupportInitialize)picBoxSegHighlight).BeginInit();
            ((ISupportInitialize)picBoxSegBackground).BeginInit();
            ((ISupportInitialize)picBoxDViewAhead).BeginInit();
            ((ISupportInitialize)picBoxDViewAheadLosing).BeginInit();
            ((ISupportInitialize)picBoxDViewBehind).BeginInit();
            ((ISupportInitialize)picBoxDViewBehindLosing).BeginInit();
            ((ISupportInitialize)picBoxDViewFinished).BeginInit();
            ((ISupportInitialize)picBoxDViewRecord).BeginInit();
            ((ISupportInitialize)picBoxDViewDelay).BeginInit();
            ((ISupportInitialize)picBoxDViewPaused).BeginInit();
            ((ISupportInitialize)picBoxDViewFlash).BeginInit();
            ((ISupportInitialize)picBoxDViewSegCurrentText).BeginInit();
            ((ISupportInitialize)picBoxDViewSegDefaultText).BeginInit();
            ((ISupportInitialize)picBoxDViewSegMissingTime).BeginInit();
            ((ISupportInitialize)picBoxDViewSegBestSegment).BeginInit();
            ((ISupportInitialize)picBoxDViewSegAheadGain).BeginInit();
            ((ISupportInitialize)picBoxDViewSegAheadLoss).BeginInit();
            ((ISupportInitialize)picBoxDViewSegBehindGain).BeginInit();
            ((ISupportInitialize)picBoxDViewSegBehindLoss).BeginInit();
            ((ISupportInitialize)picBoxDViewSegHighlight).BeginInit();
            ((ISupportInitialize)picturebox1).BeginInit();
            ((ISupportInitialize)picBoxPreview).BeginInit();
            base.SuspendLayout();

            // ----------------------------------------
            // Setting up dialog:
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x19b, 0x1b5);
            base.Controls.AddRange(new Control[]
            {
                colorTabs,
                labelPreview, picBoxPreview, checkBoxPlainBg,
                buttonDefaultColors, buttonSave, buttonLoad,
                buttonOk, buttonCancel
            });
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "CustomizeColors";
            Text = "Set colors...";

            // ----------------------------------------
            // Setting up globale controls:
            //
            // labelPreview
            //
            labelPreview.AutoSize = true;
            labelPreview.Location = new Point(0x110, 0x12);
            labelPreview.Name = "labelPreview";
            labelPreview.Size = new Size(0x2d, 13);
            labelPreview.TabIndex = 2;
            labelPreview.Text = "Preview";
            //
            // picBoxPreview
            //
            picBoxPreview.Location = new Point(0x113, 0x22);
            picBoxPreview.Name = "picBoxPreview";
            picBoxPreview.Size = new Size(0x7c, 0x119);
            picBoxPreview.TabIndex = 1;
            picBoxPreview.TabStop = false;
            picBoxPreview.Paint += previewBox_Paint;
            //
            // checkBoxPlainBg
            //
            checkBoxPlainBg.AutoSize = true;
            checkBoxPlainBg.Location = new Point(0x113, 0x141);
            checkBoxPlainBg.Name = "checkBoxPlainBg";
            checkBoxPlainBg.Size = new Size(0x6c, 0x11);
            checkBoxPlainBg.TabIndex = 6;
            checkBoxPlainBg.Text = "Preview Plain BG";
            checkBoxPlainBg.UseVisualStyleBackColor = true;
            checkBoxPlainBg.CheckedChanged += plainBg_CheckedChanged;

            //
            // buttonDefaultColors
            //
            buttonDefaultColors.Location = new Point(0x113, 0x158);
            buttonDefaultColors.Name = "buttonDefaultColors";
            buttonDefaultColors.Size = new Size(0x7c, 0x17);
            buttonDefaultColors.TabIndex = 5;
            buttonDefaultColors.Text = "Restore Defaults";
            buttonDefaultColors.UseVisualStyleBackColor = true;
            buttonDefaultColors.Click += buttonDefaultColors_Click;
            //
            // buttonSave
            //
            buttonSave.Location = new Point(0x113, 0x175);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(0x3b, 0x17);
            buttonSave.TabIndex = 7;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            //
            // buttonLoad
            //
            buttonLoad.Location = new Point(340, 0x175);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(0x3b, 0x17);
            buttonLoad.TabIndex = 8;
            buttonLoad.Text = "Load";
            buttonLoad.UseVisualStyleBackColor = true;
            buttonLoad.Click += buttonLoad_Click;
            //
            // buttonOk
            //
            buttonOk.Location = new Point(0x113, 0x192);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(0x3b, 0x17);
            buttonOk.TabIndex = 3;
            buttonOk.Text = "OK";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += buttonOk_Click;
            //
            // buttonCancel
            //
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(340, 0x192);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(0x3b, 0x17);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;

            // ----------------------------------------
            // Setting up Tabs:
            //
            // colorTabs
            //
            colorTabs.Controls.Add(tabPageClockColors);
            colorTabs.Controls.Add(tabPageSegColors);
            colorTabs.Controls.Add(tabPageDetailedViewColors);
            colorTabs.Location = new Point(12, 12);
            colorTabs.Name = "colorTabs";
            colorTabs.SelectedIndex = 0;
            colorTabs.Size = new Size(0x101, 0x19d);
            colorTabs.TabIndex = 0;
            //
            // tabPageClockColors
            //
            tabPageClockColors.Controls.AddRange(new Control[]
            {
                labelColumnFore, groupBoxBackground,
                labelAhead, picBoxAheadFore,
                labelAheadLosing, picBoxAheadLosingFore,
                labelBehind, picBoxBehindFore,
                labelBehindLosing, picBoxBehindLosingFore,
                labelNoLoaded, picBoxNoLoadedFore,
                labelFinished, picBoxFinishedFore,
                labelRecord, picBoxRecordFore,
                labelDelay, picBoxDelayFore,
                labelPaused, picBoxPaused,
                labelFlash, picBoxFlash,
                labelStatusBar, picBoxStatusBarFore,
                labelRunTitle, picBoxRunTitleFore,
            });
            tabPageClockColors.Name = "tabPageClockColors";
            tabPageClockColors.Padding = new Padding(3);
            tabPageClockColors.TabIndex = 0;
            tabPageClockColors.Text = "Clock/Status";
            tabPageClockColors.UseVisualStyleBackColor = true;
            //
            // tabPageSegColors
            //
            tabPageSegColors.Controls.AddRange(new Control[]
            {
                labelColumnSegColor, labelColumnSegColor2, labelColumnSegPlain,
                labelSegBackground, picBoxSegBackground, picBoxSegBackground2, picBoxSegBackgroundPlain,
                labelSegHighlight, picBoxSegHighlight, picBoxSegHighlight2, picBoxSegHighlightPlain,
                labelSegHighlightBorder, picBoxSegHighlightBorder,
                labelSegPastText, picBoxSegPastText,
                labelSegLiveText, picBoxSegLiveText,
                labelSegFutureText, picBoxSegFutureText,
                labelSegFutureTime, picBoxSegFutureTime,
                labelSegNewTime, picBoxSegNewTime,
                labelSegMissing, picBoxSegMissing,
                labelSegBestSegment, picBoxSegBestSegment,
                labelSegAheadGain, picBoxSegAheadGain,
                labelSegAheadLoss, picBoxSegAheadLoss,
                labelSegBehindGain, picBoxSegBehindGain,
                labelSegBehindLoss, picBoxSegBehindLoss, picturebox1
            });
            tabPageSegColors.Name = "segColorTab";
            tabPageSegColors.Padding = new Padding(3);
            tabPageSegColors.TabIndex = 1;
            tabPageSegColors.Text = "Segments";
            tabPageSegColors.UseVisualStyleBackColor = true;
            //
            // tabPageDetailedView
            //
            tabPageDetailedViewColors.Controls.AddRange(new Control[]
            {
                checkBoxDViewUsePrimary,
                groupBoxDViewClock,
                groupBoxDViewSegments,
                groupBoxGraph
            });
            tabPageDetailedViewColors.Name = "tabPageDetailedView";
            tabPageDetailedViewColors.Padding = new Padding(3);
            tabPageDetailedViewColors.TabIndex = 2;
            tabPageDetailedViewColors.Text = "Detailed View";
            tabPageDetailedViewColors.UseVisualStyleBackColor = true;

            // ----------------------------------------
            // Clock tab:
            //
            // labelColumnFore
            //
            labelColumnFore.AutoSize = true;
            labelColumnFore.Location = new Point(0x6d, 0x16);
            labelColumnFore.Name = "labelColumnFore";
            labelColumnFore.Size = new Size(0x1c, 13);
            labelColumnFore.Text = "Fore";
            labelColumnColors.AutoSize = true;
            labelColumnColors.Location = new Point(0x11, 0x10);
            labelColumnColors.Name = "labelColumnColors";
            labelColumnColors.Size = new Size(0x24, 13);
            labelColumnColors.Text = "Colors";
            labelColumnPlain.AutoSize = true;
            labelColumnPlain.Location = new Point(0x43, 0x10);
            labelColumnPlain.Name = "labelColumnPlain";
            labelColumnPlain.Size = new Size(30, 13);
            labelColumnPlain.Text = "Plain";
            //
            // groupBoxBackground
            //
            groupBoxBackground.Controls.AddRange(new Control[]
            {
                labelColumnColors, labelColumnPlain,
                picBoxAheadBack, picBoxAheadBack2, picBoxAheadBackPlain,
                picBoxAheadLosingBack, picBoxAheadLosingBack2, picBoxAheadLosingBackPlain,
                picBoxBehindBack, picBoxBehindBack2, picBoxBehindBackPlain,
                picBoxBehindLosingBack, picBoxBehindLosingBack2, picBoxBehindLosingBackPlain,
                picBoxNoLoadedBack, picBoxNoLoadedBack2, picBoxNoLoadedBackPlain,
                picBoxFinishedBack, picBoxFinishedBack2, picBoxFinishedBackPlain,
                picBoxRecordBack, picBoxRecordBack2, picBoxRecordBackPlain,
                picBoxDelayBack, picBoxDelayBack2, picBoxDelayBackPlain,
                picBoxStatusBarBack, picBoxStatusBarBack2, picBoxStatusBarBackPlain,
                picBoxRunTitleBack, picBoxRunTitleBack2, picBoxRunTitleBackPlain
            });
            groupBoxBackground.Location = new Point(0x8b, 6);
            groupBoxBackground.Name = "groupBoxBackground";
            groupBoxBackground.Size = new Size(0x68, 0x15d);
            groupBoxBackground.TabStop = false;
            groupBoxBackground.Text = "Background";

            //
            // labelAhead
            //
            labelAhead.AutoSize = true;
            labelAhead.Cursor = Cursors.Hand;
            labelAhead.Location = new Point(7, 0x26);
            labelAhead.MinimumSize = new Size(100, 20);
            labelAhead.Name = "labelAhead";
            labelAhead.Text = "Ahead";
            labelAhead.TextAlign = ContentAlignment.MiddleRight;
            labelAhead.Click += labelAhead_Click;
            //
            // picBoxAheadFore
            //
            picBoxAheadFore.BorderStyle = BorderStyle.FixedSingle;
            picBoxAheadFore.Cursor = Cursors.Hand;
            picBoxAheadFore.Location = new Point(0x71, 0x26);
            picBoxAheadFore.Name = "AheadFore";
            picBoxAheadFore.Size = new Size(20, 20);
            picBoxAheadFore.TabStop = false;
            picBoxAheadFore.Click += SetPictureBoxColor;
            //
            // picBoxAheadBack
            //
            picBoxAheadBack.BorderStyle = BorderStyle.FixedSingle;
            picBoxAheadBack.Cursor = Cursors.Hand;
            picBoxAheadBack.Location = new Point(12, 0x20);
            picBoxAheadBack.Name = "picBoxAheadBack";
            picBoxAheadBack.Size = new Size(20, 20);
            picBoxAheadBack.TabStop = false;
            picBoxAheadBack.Click += SetPictureBoxColor;
            //
            // picBoxAheadBack2
            //
            picBoxAheadBack2.BorderStyle = BorderStyle.FixedSingle;
            picBoxAheadBack2.Cursor = Cursors.Hand;
            picBoxAheadBack2.Location = new Point(0x26, 0x20);
            picBoxAheadBack2.Name = "picBoxAheadBack2";
            picBoxAheadBack2.Size = new Size(20, 20);
            picBoxAheadBack2.TabStop = false;
            picBoxAheadBack2.Click += SetPictureBoxColor;
            //
            // picBoxAheadBackPlain
            //
            picBoxAheadBackPlain.BorderStyle = BorderStyle.FixedSingle;
            picBoxAheadBackPlain.Cursor = Cursors.Hand;
            picBoxAheadBackPlain.Location = new Point(0x48, 0x20);
            picBoxAheadBackPlain.Name = "picBoxAheadBackPlain";
            picBoxAheadBackPlain.Size = new Size(20, 20);
            picBoxAheadBackPlain.TabStop = false;
            picBoxAheadBackPlain.Click += SetPictureBoxColor;

            //
            // labelAheadLosing
            //
            labelAheadLosing.AutoSize = true;
            labelAheadLosing.Cursor = Cursors.Hand;
            labelAheadLosing.Location = new Point(7, 0x40);
            labelAheadLosing.MinimumSize = new Size(100, 20);
            labelAheadLosing.Name = "labelAheadLosing";
            labelAheadLosing.Text = "Ahead (losing time)";
            labelAheadLosing.TextAlign = ContentAlignment.MiddleRight;
            labelAheadLosing.Click += labelAheadLosing_Click;
            //
            // picBoxAheadLosingFore
            //
            picBoxAheadLosingFore.BorderStyle = BorderStyle.FixedSingle;
            picBoxAheadLosingFore.Cursor = Cursors.Hand;
            picBoxAheadLosingFore.Location = new Point(0x71, 0x40);
            picBoxAheadLosingFore.Name = "picBoxAheadLosingFore";
            picBoxAheadLosingFore.Size = new Size(20, 20);
            picBoxAheadLosingFore.TabStop = false;
            picBoxAheadLosingFore.Click += SetPictureBoxColor;
            //
            // picBoxAheadLosingBack
            //
            picBoxAheadLosingBack.BorderStyle = BorderStyle.FixedSingle;
            picBoxAheadLosingBack.Cursor = Cursors.Hand;
            picBoxAheadLosingBack.Location = new Point(12, 0x3a);
            picBoxAheadLosingBack.Name = "picBoxAheadLosingBack";
            picBoxAheadLosingBack.Size = new Size(20, 20);
            picBoxAheadLosingBack.TabStop = false;
            picBoxAheadLosingBack.Click += SetPictureBoxColor;
            //
            // picBoxAheadLosingBack2
            //
            picBoxAheadLosingBack2.BorderStyle = BorderStyle.FixedSingle;
            picBoxAheadLosingBack2.Cursor = Cursors.Hand;
            picBoxAheadLosingBack2.Location = new Point(0x26, 0x3a);
            picBoxAheadLosingBack2.Name = "picBoxAheadLosingBack2";
            picBoxAheadLosingBack2.Size = new Size(20, 20);
            picBoxAheadLosingBack2.TabStop = false;
            picBoxAheadLosingBack2.Click += SetPictureBoxColor;
            //
            // picBoxAheadLosingBackPlain
            //
            picBoxAheadLosingBackPlain.BorderStyle = BorderStyle.FixedSingle;
            picBoxAheadLosingBackPlain.Cursor = Cursors.Hand;
            picBoxAheadLosingBackPlain.Location = new Point(0x48, 0x3a);
            picBoxAheadLosingBackPlain.Name = "picBoxAheadLosingBackPlain";
            picBoxAheadLosingBackPlain.Size = new Size(20, 20);
            picBoxAheadLosingBackPlain.TabStop = false;
            picBoxAheadLosingBackPlain.Click += SetPictureBoxColor;

            //
            // labelBehind
            //
            labelBehind.AutoSize = true;
            labelBehind.Cursor = Cursors.Hand;
            labelBehind.Location = new Point(7, 90);
            labelBehind.MinimumSize = new Size(100, 20);
            labelBehind.Name = "labelBehind";
            labelBehind.Text = "Behind";
            labelBehind.TextAlign = ContentAlignment.MiddleRight;
            labelBehind.Click += labelBehind_Click;
            //
            // picBoxBehindFore
            //
            picBoxBehindFore.BorderStyle = BorderStyle.FixedSingle;
            picBoxBehindFore.Cursor = Cursors.Hand;
            picBoxBehindFore.Location = new Point(0x71, 90);
            picBoxBehindFore.Name = "picBoxBehindFore";
            picBoxBehindFore.Size = new Size(20, 20);
            picBoxBehindFore.TabStop = false;
            picBoxBehindFore.Click += SetPictureBoxColor;
            //
            // picBoxBehindBack
            //
            picBoxBehindBack.BorderStyle = BorderStyle.FixedSingle;
            picBoxBehindBack.Cursor = Cursors.Hand;
            picBoxBehindBack.Location = new Point(12, 0x54);
            picBoxBehindBack.Name = "picBoxBehindBack";
            picBoxBehindBack.Size = new Size(20, 20);
            picBoxBehindBack.TabStop = false;
            picBoxBehindBack.Click += SetPictureBoxColor;
            //
            // picBoxBehindBack2
            //
            picBoxBehindBack2.BorderStyle = BorderStyle.FixedSingle;
            picBoxBehindBack2.Cursor = Cursors.Hand;
            picBoxBehindBack2.Location = new Point(0x26, 0x54);
            picBoxBehindBack2.Name = "picBoxBehindBack2";
            picBoxBehindBack2.Size = new Size(20, 20);
            picBoxBehindBack2.TabStop = false;
            picBoxBehindBack2.Click += SetPictureBoxColor;
            //
            // picBoxBehindBackPlain
            //
            picBoxBehindBackPlain.BorderStyle = BorderStyle.FixedSingle;
            picBoxBehindBackPlain.Cursor = Cursors.Hand;
            picBoxBehindBackPlain.Location = new Point(0x48, 0x54);
            picBoxBehindBackPlain.Name = "picBoxBehindBackPlain";
            picBoxBehindBackPlain.Size = new Size(20, 20);
            picBoxBehindBackPlain.TabStop = false;
            picBoxBehindBackPlain.Click += SetPictureBoxColor;

            //
            // labelBehindLosing
            //
            labelBehindLosing.AutoSize = true;
            labelBehindLosing.Cursor = Cursors.Hand;
            labelBehindLosing.Location = new Point(7, 0x74);
            labelBehindLosing.MinimumSize = new Size(100, 20);
            labelBehindLosing.Name = "labelBehindLosing";
            labelBehindLosing.Text = "Behind (losing time)";
            labelBehindLosing.TextAlign = ContentAlignment.MiddleRight;
            labelBehindLosing.Click += labelBehindLosing_Click;
            //
            // picBoxBehindLosingFore
            //
            picBoxBehindLosingFore.BorderStyle = BorderStyle.FixedSingle;
            picBoxBehindLosingFore.Cursor = Cursors.Hand;
            picBoxBehindLosingFore.Location = new Point(0x71, 0x74);
            picBoxBehindLosingFore.Name = "picBoxBehindLosingFore";
            picBoxBehindLosingFore.Size = new Size(20, 20);
            picBoxBehindLosingFore.TabStop = false;
            picBoxBehindLosingFore.Click += SetPictureBoxColor;
            //
            // picBoxBehindLosingBack
            //
            picBoxBehindLosingBack.BorderStyle = BorderStyle.FixedSingle;
            picBoxBehindLosingBack.Cursor = Cursors.Hand;
            picBoxBehindLosingBack.Location = new Point(12, 110);
            picBoxBehindLosingBack.Name = "picBoxBehindLosingBack";
            picBoxBehindLosingBack.Size = new Size(20, 20);
            picBoxBehindLosingBack.TabStop = false;
            picBoxBehindLosingBack.Click += SetPictureBoxColor;
            //
            // picBoxBehindLosingBack2
            //
            picBoxBehindLosingBack2.BorderStyle = BorderStyle.FixedSingle;
            picBoxBehindLosingBack2.Cursor = Cursors.Hand;
            picBoxBehindLosingBack2.Location = new Point(0x26, 110);
            picBoxBehindLosingBack2.Name = "picBoxBehindLosingBack2";
            picBoxBehindLosingBack2.Size = new Size(20, 20);
            picBoxBehindLosingBack2.TabStop = false;
            picBoxBehindLosingBack2.Click += SetPictureBoxColor;
            //
            // picBoxBehindLosingBackPlain
            //
            picBoxBehindLosingBackPlain.BorderStyle = BorderStyle.FixedSingle;
            picBoxBehindLosingBackPlain.Cursor = Cursors.Hand;
            picBoxBehindLosingBackPlain.Location = new Point(0x48, 110);
            picBoxBehindLosingBackPlain.Name = "picBoxBehindLosingBackPlain";
            picBoxBehindLosingBackPlain.Size = new Size(20, 20);
            picBoxBehindLosingBackPlain.TabStop = false;
            picBoxBehindLosingBackPlain.Click += SetPictureBoxColor;

            //
            // labelNoLoaded
            //
            labelNoLoaded.AutoSize = true;
            labelNoLoaded.Cursor = Cursors.Hand;
            labelNoLoaded.Location = new Point(7, 0x8e);
            labelNoLoaded.MinimumSize = new Size(100, 20);
            labelNoLoaded.Name = "labelNoLoaded";
            labelNoLoaded.Text = "No run loaded";
            labelNoLoaded.TextAlign = ContentAlignment.MiddleRight;
            labelNoLoaded.Click += labelNoLoaded_Click;
            //
            // picBoxNoLoadedFore
            //
            picBoxNoLoadedFore.BorderStyle = BorderStyle.FixedSingle;
            picBoxNoLoadedFore.Cursor = Cursors.Hand;
            picBoxNoLoadedFore.Location = new Point(0x71, 0x8e);
            picBoxNoLoadedFore.Name = "picBoxNoLoadedFore";
            picBoxNoLoadedFore.Size = new Size(20, 20);
            picBoxNoLoadedFore.TabStop = false;
            picBoxNoLoadedFore.Click += SetPictureBoxColor;
            //
            // picBoxNoLoadedBack
            //
            picBoxNoLoadedBack.BorderStyle = BorderStyle.FixedSingle;
            picBoxNoLoadedBack.Cursor = Cursors.Hand;
            picBoxNoLoadedBack.Location = new Point(12, 0x88);
            picBoxNoLoadedBack.Name = "picBoxNoLoadedBack";
            picBoxNoLoadedBack.Size = new Size(20, 20);
            picBoxNoLoadedBack.TabStop = false;
            picBoxNoLoadedBack.Click += SetPictureBoxColor;
            //
            // picBoxNoLoadedBack2
            //
            picBoxNoLoadedBack2.BorderStyle = BorderStyle.FixedSingle;
            picBoxNoLoadedBack2.Cursor = Cursors.Hand;
            picBoxNoLoadedBack2.Location = new Point(0x26, 0x88);
            picBoxNoLoadedBack2.Name = "picBoxNoLoadedBack2";
            picBoxNoLoadedBack2.Size = new Size(20, 20);
            picBoxNoLoadedBack2.TabStop = false;
            picBoxNoLoadedBack2.Click += SetPictureBoxColor;
            //
            // picBoxNoLoadedBackPlain
            //
            picBoxNoLoadedBackPlain.BorderStyle = BorderStyle.FixedSingle;
            picBoxNoLoadedBackPlain.Cursor = Cursors.Hand;
            picBoxNoLoadedBackPlain.Location = new Point(0x48, 0x88);
            picBoxNoLoadedBackPlain.Name = "picBoxNoLoadedBackPlain";
            picBoxNoLoadedBackPlain.Size = new Size(20, 20);
            picBoxNoLoadedBackPlain.TabStop = false;
            picBoxNoLoadedBackPlain.Click += SetPictureBoxColor;

            //
            // labelFinished
            //
            labelFinished.AutoSize = true;
            labelFinished.Cursor = Cursors.Hand;
            labelFinished.Location = new Point(7, 0xa8);
            labelFinished.MinimumSize = new Size(100, 20);
            labelFinished.Name = "labelFinished";
            labelFinished.Text = "Finished";
            labelFinished.TextAlign = ContentAlignment.MiddleRight;
            labelFinished.Click += labelFinished_Click;
            //
            // picBoxFinishedFore
            //
            picBoxFinishedFore.BorderStyle = BorderStyle.FixedSingle;
            picBoxFinishedFore.Cursor = Cursors.Hand;
            picBoxFinishedFore.Location = new Point(0x71, 0xa8);
            picBoxFinishedFore.Name = "picBoxFinishedFore";
            picBoxFinishedFore.Size = new Size(20, 20);
            picBoxFinishedFore.TabStop = false;
            picBoxFinishedFore.Click += SetPictureBoxColor;
            //
            // picBoxFinishedBack
            //
            picBoxFinishedBack.BorderStyle = BorderStyle.FixedSingle;
            picBoxFinishedBack.Cursor = Cursors.Hand;
            picBoxFinishedBack.Location = new Point(12, 0xa2);
            picBoxFinishedBack.Name = "picBoxFinishedBack";
            picBoxFinishedBack.Size = new Size(20, 20);
            picBoxFinishedBack.TabStop = false;
            picBoxFinishedBack.Click += SetPictureBoxColor;
            //
            // picBoxFinishedBack2
            //
            picBoxFinishedBack2.BorderStyle = BorderStyle.FixedSingle;
            picBoxFinishedBack2.Cursor = Cursors.Hand;
            picBoxFinishedBack2.Location = new Point(0x26, 0xa2);
            picBoxFinishedBack2.Name = "picBoxFinishedBack2";
            picBoxFinishedBack2.Size = new Size(20, 20);
            picBoxFinishedBack2.TabStop = false;
            picBoxFinishedBack2.Click += SetPictureBoxColor;
            //
            // picBoxFinishedBackPlain
            //
            picBoxFinishedBackPlain.BorderStyle = BorderStyle.FixedSingle;
            picBoxFinishedBackPlain.Cursor = Cursors.Hand;
            picBoxFinishedBackPlain.Location = new Point(0x48, 0xa2);
            picBoxFinishedBackPlain.Name = "picBoxFinishedBackPlain";
            picBoxFinishedBackPlain.Size = new Size(20, 20);
            picBoxFinishedBackPlain.TabStop = false;
            picBoxFinishedBackPlain.Click += SetPictureBoxColor;

            //
            // labelRecord
            //
            labelRecord.AutoSize = true;
            labelRecord.Cursor = Cursors.Hand;
            labelRecord.Location = new Point(7, 0xc2);
            labelRecord.MinimumSize = new Size(100, 20);
            labelRecord.Name = "labelRecord";
            labelRecord.Text = "New record";
            labelRecord.TextAlign = ContentAlignment.MiddleRight;
            labelRecord.Click += labelRecord_Click;
            //
            // picBoxRecordFore
            //
            picBoxRecordFore.BorderStyle = BorderStyle.FixedSingle;
            picBoxRecordFore.Cursor = Cursors.Hand;
            picBoxRecordFore.Location = new Point(0x71, 0xc2);
            picBoxRecordFore.Name = "picBoxRecordFore";
            picBoxRecordFore.Size = new Size(20, 20);
            picBoxRecordFore.TabStop = false;
            picBoxRecordFore.Click += SetPictureBoxColor;
            //
            // picBoxRecordBack
            //
            picBoxRecordBack.BorderStyle = BorderStyle.FixedSingle;
            picBoxRecordBack.Cursor = Cursors.Hand;
            picBoxRecordBack.Location = new Point(12, 0xbc);
            picBoxRecordBack.Name = "picBoxRecordBack";
            picBoxRecordBack.Size = new Size(20, 20);
            picBoxRecordBack.TabStop = false;
            picBoxRecordBack.Click += SetPictureBoxColor;
            //
            // picBoxRecordBack2
            //
            picBoxRecordBack2.BorderStyle = BorderStyle.FixedSingle;
            picBoxRecordBack2.Cursor = Cursors.Hand;
            picBoxRecordBack2.Location = new Point(0x26, 0xbc);
            picBoxRecordBack2.Name = "picBoxRecordBack2";
            picBoxRecordBack2.Size = new Size(20, 20);
            picBoxRecordBack2.TabStop = false;
            picBoxRecordBack2.Click += SetPictureBoxColor;
            //
            // picBoxRecordBackPlain
            //
            picBoxRecordBackPlain.BorderStyle = BorderStyle.FixedSingle;
            picBoxRecordBackPlain.Cursor = Cursors.Hand;
            picBoxRecordBackPlain.Location = new Point(0x48, 0xbc);
            picBoxRecordBackPlain.Name = "picBoxRecordBackPlain";
            picBoxRecordBackPlain.Size = new Size(20, 20);
            picBoxRecordBackPlain.TabStop = false;
            picBoxRecordBackPlain.Click += SetPictureBoxColor;

            //
            // labelDelay
            //
            labelDelay.AutoSize = true;
            labelDelay.Cursor = Cursors.Hand;
            labelDelay.Location = new Point(7, 220);
            labelDelay.MinimumSize = new Size(100, 20);
            labelDelay.Name = "labelDelay";
            labelDelay.Text = "Delay";
            labelDelay.TextAlign = ContentAlignment.MiddleRight;
            labelDelay.Click += labelDelay_Click;
            //
            // picBoxDelayFore
            //
            picBoxDelayFore.BorderStyle = BorderStyle.FixedSingle;
            picBoxDelayFore.Cursor = Cursors.Hand;
            picBoxDelayFore.Location = new Point(0x71, 220);
            picBoxDelayFore.Name = "picBoxDelayFore";
            picBoxDelayFore.Size = new Size(20, 20);
            picBoxDelayFore.TabStop = false;
            picBoxDelayFore.Click += SetPictureBoxColor;
            //
            // picBoxDelayBack
            //
            picBoxDelayBack.BorderStyle = BorderStyle.FixedSingle;
            picBoxDelayBack.Cursor = Cursors.Hand;
            picBoxDelayBack.Location = new Point(12, 0xd6);
            picBoxDelayBack.Name = "picBoxDelayBack";
            picBoxDelayBack.Size = new Size(20, 20);
            picBoxDelayBack.TabStop = false;
            picBoxDelayBack.Click += SetPictureBoxColor;
            //
            // picBoxDelayBack2
            //
            picBoxDelayBack2.BorderStyle = BorderStyle.FixedSingle;
            picBoxDelayBack2.Cursor = Cursors.Hand;
            picBoxDelayBack2.Location = new Point(0x26, 0xd6);
            picBoxDelayBack2.Name = "picBoxDelayBack2";
            picBoxDelayBack2.Size = new Size(20, 20);
            picBoxDelayBack2.TabStop = false;
            picBoxDelayBack2.Click += SetPictureBoxColor;
            //
            // picBoxDelayBackPlain
            //
            picBoxDelayBackPlain.BorderStyle = BorderStyle.FixedSingle;
            picBoxDelayBackPlain.Cursor = Cursors.Hand;
            picBoxDelayBackPlain.Location = new Point(0x48, 0xd6);
            picBoxDelayBackPlain.Name = "picBoxDelayBackPlain";
            picBoxDelayBackPlain.Size = new Size(20, 20);
            picBoxDelayBackPlain.TabStop = false;
            picBoxDelayBackPlain.Click += SetPictureBoxColor;

            //
            // labelPaused
            //
            labelPaused.AutoSize = true;
            labelPaused.Cursor = Cursors.Hand;
            labelPaused.Location = new Point(7, 0xf6);
            labelPaused.MinimumSize = new Size(100, 20);
            labelPaused.Name = "labelPaused";
            labelPaused.Text = "Paused";
            labelPaused.TextAlign = ContentAlignment.MiddleRight;
            labelPaused.Click += labelPaused_Click;
            //
            // picBoxPaused
            //
            picBoxPaused.BorderStyle = BorderStyle.FixedSingle;
            picBoxPaused.Cursor = Cursors.Hand;
            picBoxPaused.Location = new Point(0x71, 0xf6);
            picBoxPaused.Name = "picBoxPaused";
            picBoxPaused.Size = new Size(20, 20);
            picBoxPaused.TabStop = false;
            picBoxPaused.Click += SetPictureBoxColor;

            //
            // labelFlash
            //
            labelFlash.AutoSize = true;
            labelFlash.Cursor = Cursors.Hand;
            labelFlash.Location = new Point(7, 0x110);
            labelFlash.MinimumSize = new Size(100, 20);
            labelFlash.Name = "labelFlash";
            labelFlash.Text = "Split flash";
            labelFlash.TextAlign = ContentAlignment.MiddleRight;
            labelFlash.Click += labelFlash_Click;
            //
            // picBoxFlash
            //
            picBoxFlash.BorderStyle = BorderStyle.FixedSingle;
            picBoxFlash.Cursor = Cursors.Hand;
            picBoxFlash.Location = new Point(0x71, 0x110);
            picBoxFlash.Name = "picBoxFlash";
            picBoxFlash.Size = new Size(20, 20);
            picBoxFlash.TabStop = false;
            picBoxFlash.Click += SetPictureBoxColor;

            //
            // labelStatusBar
            //
            labelStatusBar.AutoSize = true;
            labelStatusBar.Location = new Point(7, 0x12a);
            labelStatusBar.MinimumSize = new Size(100, 20);
            labelStatusBar.Name = "labelStatusBar";
            labelStatusBar.Text = "Status bar";
            labelStatusBar.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxStatusBarFore
            //
            picBoxStatusBarFore.BorderStyle = BorderStyle.FixedSingle;
            picBoxStatusBarFore.Cursor = Cursors.Hand;
            picBoxStatusBarFore.Location = new Point(0x71, 0x12a);
            picBoxStatusBarFore.Name = "picBoxStatusBarFore";
            picBoxStatusBarFore.Size = new Size(20, 20);
            picBoxStatusBarFore.TabStop = false;
            picBoxStatusBarFore.Click += SetPictureBoxColor;
            //
            // picBoxStatusBarBack
            //
            picBoxStatusBarBack.BorderStyle = BorderStyle.FixedSingle;
            picBoxStatusBarBack.Cursor = Cursors.Hand;
            picBoxStatusBarBack.Location = new Point(12, 0x124);
            picBoxStatusBarBack.Name = "picBoxStatusBarBack";
            picBoxStatusBarBack.Size = new Size(20, 20);
            picBoxStatusBarBack.TabStop = false;
            picBoxStatusBarBack.Click += SetPictureBoxColor;
            //
            // picBoxStatusBarBack2
            //
            picBoxStatusBarBack2.BorderStyle = BorderStyle.FixedSingle;
            picBoxStatusBarBack2.Cursor = Cursors.Hand;
            picBoxStatusBarBack2.Location = new Point(0x26, 0x124);
            picBoxStatusBarBack2.Name = "picBoxStatusBarBack2";
            picBoxStatusBarBack2.Size = new Size(20, 20);
            picBoxStatusBarBack2.TabStop = false;
            picBoxStatusBarBack2.Click += SetPictureBoxColor;
            //
            // picBoxStatusBarBackPlain
            //
            picBoxStatusBarBackPlain.BorderStyle = BorderStyle.FixedSingle;
            picBoxStatusBarBackPlain.Cursor = Cursors.Hand;
            picBoxStatusBarBackPlain.Location = new Point(0x48, 0x124);
            picBoxStatusBarBackPlain.Name = "picBoxStatusBarBackPlain";
            picBoxStatusBarBackPlain.Size = new Size(20, 20);
            picBoxStatusBarBackPlain.TabStop = false;
            picBoxStatusBarBackPlain.Click += SetPictureBoxColor;

            //
            // labelRunTitle
            //
            labelRunTitle.AutoSize = true;
            labelRunTitle.Location = new Point(7, 0x144);
            labelRunTitle.MinimumSize = new Size(100, 20);
            labelRunTitle.Name = "labelRunTitle";
            labelRunTitle.Text = "Run title";
            labelRunTitle.TextAlign = ContentAlignment.MiddleRight;
            picBoxRunTitleFore.BorderStyle = BorderStyle.FixedSingle;
            //
            // picBoxRunTitleFore
            //
            picBoxRunTitleFore.Cursor = Cursors.Hand;
            picBoxRunTitleFore.Location = new Point(0x71, 0x144);
            picBoxRunTitleFore.Name = "picBoxRunTitleFore";
            picBoxRunTitleFore.Size = new Size(20, 20);
            picBoxRunTitleFore.TabStop = false;
            picBoxRunTitleFore.Click += SetPictureBoxColor;
            //
            // picBoxRunTitleBack
            //
            picBoxRunTitleBack.BorderStyle = BorderStyle.FixedSingle;
            picBoxRunTitleBack.Cursor = Cursors.Hand;
            picBoxRunTitleBack.Location = new Point(12, 0x13e);
            picBoxRunTitleBack.Name = "picBoxRunTitleBack";
            picBoxRunTitleBack.Size = new Size(20, 20);
            picBoxRunTitleBack.TabStop = false;
            picBoxRunTitleBack.Click += SetPictureBoxColor;
            //
            // picBoxRunTitleBack2
            //
            picBoxRunTitleBack2.BorderStyle = BorderStyle.FixedSingle;
            picBoxRunTitleBack2.Cursor = Cursors.Hand;
            picBoxRunTitleBack2.Location = new Point(0x26, 0x13e);
            picBoxRunTitleBack2.Name = "picBoxRunTitleBack2";
            picBoxRunTitleBack2.Size = new Size(20, 20);
            picBoxRunTitleBack2.TabStop = false;
            picBoxRunTitleBack2.Click += SetPictureBoxColor;
            //
            // picBoxRunTitleBackPlain
            //
            picBoxRunTitleBackPlain.BorderStyle = BorderStyle.FixedSingle;
            picBoxRunTitleBackPlain.Cursor = Cursors.Hand;
            picBoxRunTitleBackPlain.Location = new Point(0x48, 0x13e);
            picBoxRunTitleBackPlain.Name = "picBoxRunTitleBackPlain";
            picBoxRunTitleBackPlain.Size = new Size(20, 20);
            picBoxRunTitleBackPlain.TabStop = false;
            picBoxRunTitleBackPlain.Click += SetPictureBoxColor;

            // ----------------------------------------
            // Segment Tab:
            //
            // labelColumnSegColor
            //
            labelColumnSegColor.AutoSize = true;
            labelColumnSegColor.Location = new Point(0x8d, 3);
            labelColumnSegColor.Name = "labelColumnSegColor";
            labelColumnSegColor.Size = new Size(0x1f, 13);
            labelColumnSegColor.Text = "Color";
            //
            // labelColumnSegColor2
            //
            labelColumnSegColor2.AutoSize = true;
            labelColumnSegColor2.Location = new Point(0xac, 3);
            labelColumnSegColor2.Name = "labelColumnSegColor2";
            labelColumnSegColor2.Size = new Size(0x25, 13);
            labelColumnSegColor2.Text = "Color2";
            //
            // labelColumnSegPlain
            //
            labelColumnSegPlain.AutoSize = true;
            labelColumnSegPlain.Location = new Point(0xd1, 3);
            labelColumnSegPlain.Name = "labelColumnSegPlain";
            labelColumnSegPlain.Size = new Size(30, 13);
            labelColumnSegPlain.Text = "Plain";

            //
            // labelSegBackground
            //
            labelSegBackground.AutoSize = true;
            labelSegBackground.Location = new Point(12, 0x13);
            labelSegBackground.MinimumSize = new Size(0x80, 20);
            labelSegBackground.Name = "labelSegBackground";
            labelSegBackground.Text = "Background";
            labelSegBackground.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegBackground
            //
            picBoxSegBackground.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegBackground.Cursor = Cursors.Hand;
            picBoxSegBackground.Location = new Point(0x92, 0x13);
            picBoxSegBackground.Name = "picBoxSegBackground";
            picBoxSegBackground.Size = new Size(20, 20);
            picBoxSegBackground.TabStop = false;
            picBoxSegBackground.Click += SetPictureBoxColor;
            //
            // picBoxSegBackground2
            //
            picBoxSegBackground2.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegBackground2.Cursor = Cursors.Hand;
            picBoxSegBackground2.Location = new Point(180, 0x13);
            picBoxSegBackground2.Name = "picBoxSegBackground2";
            picBoxSegBackground2.Size = new Size(20, 20);
            picBoxSegBackground2.TabStop = false;
            picBoxSegBackground2.Click += SetPictureBoxColor;
            //
            // picBoxSegBackgroundPlain
            //
            picBoxSegBackgroundPlain.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegBackgroundPlain.Cursor = Cursors.Hand;
            picBoxSegBackgroundPlain.Location = new Point(0xd6, 0x13);
            picBoxSegBackgroundPlain.Name = "picBoxSegBackgroundPlain";
            picBoxSegBackgroundPlain.Size = new Size(20, 20);
            picBoxSegBackgroundPlain.TabStop = false;
            picBoxSegBackgroundPlain.Click += SetPictureBoxColor;

            //
            // labelSegHighlight
            //
            labelSegHighlight.AutoSize = true;
            labelSegHighlight.Location = new Point(12, 0x2d);
            labelSegHighlight.MinimumSize = new Size(0x80, 20);
            labelSegHighlight.Name = "labelSegHighlight";
            labelSegHighlight.Text = "Current Highlight";
            labelSegHighlight.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegHighlight
            //
            picBoxSegHighlight.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegHighlight.Cursor = Cursors.Hand;
            picBoxSegHighlight.Location = new Point(0x92, 0x2d);
            picBoxSegHighlight.Name = "picBoxSegHighlight";
            picBoxSegHighlight.Size = new Size(20, 20);
            picBoxSegHighlight.TabStop = false;
            picBoxSegHighlight.Click += SetPictureBoxColor;
            //
            // picBoxSegHighlight2
            //
            picBoxSegHighlight2.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegHighlight2.Cursor = Cursors.Hand;
            picBoxSegHighlight2.Location = new Point(180, 0x2d);
            picBoxSegHighlight2.Name = "picBoxSegHighlight2";
            picBoxSegHighlight2.Size = new Size(20, 20);
            picBoxSegHighlight2.TabStop = false;
            picBoxSegHighlight2.Click += SetPictureBoxColor;
            //
            // picBoxSegHighlightPlain
            //
            picBoxSegHighlightPlain.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegHighlightPlain.Cursor = Cursors.Hand;
            picBoxSegHighlightPlain.Location = new Point(0xd6, 0x2d);
            picBoxSegHighlightPlain.Name = "picBoxSegHighlightPlain";
            picBoxSegHighlightPlain.Size = new Size(20, 20);
            picBoxSegHighlightPlain.TabStop = false;
            picBoxSegHighlightPlain.Click += SetPictureBoxColor;

            //
            // labelSegHighlightBorder
            //
            labelSegHighlightBorder.AutoSize = true;
            labelSegHighlightBorder.Location = new Point(12, 0x47);
            labelSegHighlightBorder.MinimumSize = new Size(0x80, 20);
            labelSegHighlightBorder.Name = "labelSegHighlightBorder";
            labelSegHighlightBorder.Text = "Current Highlight Border";
            labelSegHighlightBorder.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegHighlightBorder
            //
            picBoxSegHighlightBorder.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegHighlightBorder.Cursor = Cursors.Hand;
            picBoxSegHighlightBorder.Location = new Point(0x92, 0x47);
            picBoxSegHighlightBorder.Name = "picBoxSegHighlightBorder";
            picBoxSegHighlightBorder.Size = new Size(20, 20);
            picBoxSegHighlightBorder.TabStop = false;
            picBoxSegHighlightBorder.Click += SetPictureBoxColor;

            //
            // labelSegPastText
            //
            labelSegPastText.AutoSize = true;
            labelSegPastText.Location = new Point(12, 0x61);
            labelSegPastText.MinimumSize = new Size(0x80, 20);
            labelSegPastText.Name = "labelSegPastText";
            labelSegPastText.Text = "Past Segment Text";
            labelSegPastText.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegPastText
            //
            picBoxSegPastText.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegPastText.Cursor = Cursors.Hand;
            picBoxSegPastText.Location = new Point(0x92, 0x61);
            picBoxSegPastText.Name = "picBoxSegPastText";
            picBoxSegPastText.Size = new Size(20, 20);
            picBoxSegPastText.TabStop = false;
            picBoxSegPastText.Click += SetPictureBoxColor;

            //
            // labelSegLiveText
            //
            labelSegLiveText.AutoSize = true;
            labelSegLiveText.Location = new Point(12, 0x7b);
            labelSegLiveText.MinimumSize = new Size(0x80, 20);
            labelSegLiveText.Name = "labelSegLiveText";
            labelSegLiveText.Text = "Live Segment Text";
            labelSegLiveText.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegLiveText
            //
            picBoxSegLiveText.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegLiveText.Cursor = Cursors.Hand;
            picBoxSegLiveText.Location = new Point(0x92, 0x7b);
            picBoxSegLiveText.Name = "picBoxSegLiveText";
            picBoxSegLiveText.Size = new Size(20, 20);
            picBoxSegLiveText.TabStop = false;
            picBoxSegLiveText.Click += SetPictureBoxColor;

            //
            // labelSegFutureText
            //
            labelSegFutureText.AutoSize = true;
            labelSegFutureText.Location = new Point(12, 0x95);
            labelSegFutureText.MinimumSize = new Size(0x80, 20);
            labelSegFutureText.Name = "labelSegFutureText";
            labelSegFutureText.Text = "Future Segment Title";
            labelSegFutureText.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegFutureText
            //
            picBoxSegFutureText.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegFutureText.Cursor = Cursors.Hand;
            picBoxSegFutureText.Location = new Point(0x92, 0x95);
            picBoxSegFutureText.Name = "picBoxSegFutureText";
            picBoxSegFutureText.Size = new Size(20, 20);
            picBoxSegFutureText.TabStop = false;
            picBoxSegFutureText.Click += SetPictureBoxColor;

            //
            // labelSegFutureTime
            //
            labelSegFutureTime.AutoSize = true;
            labelSegFutureTime.Location = new Point(12, 0xaf);
            labelSegFutureTime.MinimumSize = new Size(0x80, 20);
            labelSegFutureTime.Name = "labelSegFutureTime";
            labelSegFutureTime.Text = "Future Segment Time";
            labelSegFutureTime.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegFutureTime
            //
            picBoxSegFutureTime.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegFutureTime.Cursor = Cursors.Hand;
            picBoxSegFutureTime.Location = new Point(0x92, 0xaf);
            picBoxSegFutureTime.Name = "picBoxSegFutureTime";
            picBoxSegFutureTime.Size = new Size(20, 20);
            picBoxSegFutureTime.TabStop = false;
            picBoxSegFutureTime.Click += SetPictureBoxColor;

            //
            // labelSegNewTime
            //
            labelSegNewTime.AutoSize = true;
            labelSegNewTime.Location = new Point(12, 0xc9);
            labelSegNewTime.MinimumSize = new Size(0x80, 20);
            labelSegNewTime.Name = "labelSegNewTime";
            labelSegNewTime.Text = "New Time";
            labelSegNewTime.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegNewTime
            //
            picBoxSegNewTime.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegNewTime.Cursor = Cursors.Hand;
            picBoxSegNewTime.Location = new Point(0x92, 0xc9);
            picBoxSegNewTime.Name = "picBoxSegNewTime";
            picBoxSegNewTime.Size = new Size(20, 20);
            picBoxSegNewTime.TabStop = false;
            picBoxSegNewTime.Click += SetPictureBoxColor;

            //
            // labelSegMissing
            //
            labelSegMissing.AutoSize = true;
            labelSegMissing.Location = new Point(12, 0xe3);
            labelSegMissing.MinimumSize = new Size(0x80, 20);
            labelSegMissing.Name = "labelSegMissing";
            labelSegMissing.Text = "Missing Time/Delta";
            labelSegMissing.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegMissing
            //
            picBoxSegMissing.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegMissing.Cursor = Cursors.Hand;
            picBoxSegMissing.Location = new Point(0x92, 0xe3);
            picBoxSegMissing.Name = "picBoxSegMissing";
            picBoxSegMissing.Size = new Size(20, 20);
            picBoxSegMissing.TabStop = false;
            picBoxSegMissing.Click += SetPictureBoxColor;

            //
            // labelSegBestSegment
            //
            labelSegBestSegment.AutoSize = true;
            labelSegBestSegment.Location = new Point(12, 0xfd);
            labelSegBestSegment.MinimumSize = new Size(0x80, 20);
            labelSegBestSegment.Name = "labelSegBestSegment";
            labelSegBestSegment.Text = "New Best Segment";
            labelSegBestSegment.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegBestSegment
            //
            picBoxSegBestSegment.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegBestSegment.Cursor = Cursors.Hand;
            picBoxSegBestSegment.Location = new Point(0x92, 0xfd);
            picBoxSegBestSegment.Name = "SegBestSegment";
            picBoxSegBestSegment.Size = new Size(20, 20);
            picBoxSegBestSegment.TabStop = false;
            picBoxSegBestSegment.Click += SetPictureBoxColor;

            //
            // labelSegAheadGain
            //
            labelSegAheadGain.AutoSize = true;
            labelSegAheadGain.Location = new Point(12, 0x117);
            labelSegAheadGain.MinimumSize = new Size(0x80, 20);
            labelSegAheadGain.Name = "labelSegAheadGain";
            labelSegAheadGain.Text = "Ahead (gained time)";
            labelSegAheadGain.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegAheadGain
            //
            picBoxSegAheadGain.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegAheadGain.Cursor = Cursors.Hand;
            picBoxSegAheadGain.Location = new Point(0x92, 0x117);
            picBoxSegAheadGain.Name = "picBoxSegAheadGain";
            picBoxSegAheadGain.Size = new Size(20, 20);
            picBoxSegAheadGain.TabStop = false;
            picBoxSegAheadGain.Click += SetPictureBoxColor;

            //
            // labelSegAheadLoss
            //
            labelSegAheadLoss.AutoSize = true;
            labelSegAheadLoss.Location = new Point(12, 0x131);
            labelSegAheadLoss.MinimumSize = new Size(0x80, 20);
            labelSegAheadLoss.Name = "labelSegAheadLoss";
            labelSegAheadLoss.Text = "Ahead (lost time)";
            labelSegAheadLoss.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegAheadLoss
            //
            picBoxSegAheadLoss.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegAheadLoss.Cursor = Cursors.Hand;
            picBoxSegAheadLoss.Location = new Point(0x92, 0x131);
            picBoxSegAheadLoss.Name = "picBoxSegAheadLoss";
            picBoxSegAheadLoss.Size = new Size(20, 20);
            picBoxSegAheadLoss.TabStop = false;
            picBoxSegAheadLoss.Click += SetPictureBoxColor;

            //
            // labelSegBehindGain
            //
            labelSegBehindGain.AutoSize = true;
            labelSegBehindGain.Location = new Point(12, 0x14b);
            labelSegBehindGain.MinimumSize = new Size(0x80, 20);
            labelSegBehindGain.Name = "labelSegBehindGain";
            labelSegBehindGain.Text = "Behind (gained time)";
            labelSegBehindGain.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegBehindGain
            //
            picBoxSegBehindGain.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegBehindGain.Cursor = Cursors.Hand;
            picBoxSegBehindGain.Location = new Point(0x92, 0x14b);
            picBoxSegBehindGain.Name = "picBoxSegBehindGain";
            picBoxSegBehindGain.Size = new Size(20, 20);
            picBoxSegBehindGain.TabStop = false;
            picBoxSegBehindGain.Click += SetPictureBoxColor;

            //
            // labelSegBehindLoss
            //
            labelSegBehindLoss.AutoSize = true;
            labelSegBehindLoss.Location = new Point(12, 0x165);
            labelSegBehindLoss.MinimumSize = new Size(0x80, 20);
            labelSegBehindLoss.Name = "labelSegBehindLoss";
            labelSegBehindLoss.Text = "Behind (lost time)";
            labelSegBehindLoss.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxSegBehindLoss
            //
            picBoxSegBehindLoss.BorderStyle = BorderStyle.FixedSingle;
            picBoxSegBehindLoss.Cursor = Cursors.Hand;
            picBoxSegBehindLoss.Location = new Point(0x92, 0x165);
            picBoxSegBehindLoss.Name = "picBoxSegBehindLoss";
            picBoxSegBehindLoss.Size = new Size(20, 20);
            picBoxSegBehindLoss.TabStop = false;
            picBoxSegBehindLoss.Click += SetPictureBoxColor;

            // ----------------------------------------
            // Detailed View tab:
            //
            // checkBoxDViewUsePrimary
            //
            checkBoxDViewUsePrimary.AutoSize = true;
            checkBoxDViewUsePrimary.Location = new Point(12, 10);
            checkBoxDViewUsePrimary.MinimumSize = new Size(128, 20);
            checkBoxDViewUsePrimary.Name = "checkBoxDViewUsePrimary";
            checkBoxDViewUsePrimary.Text = "Use colors from primary window";
            checkBoxDViewUsePrimary.TextAlign = ContentAlignment.BottomLeft;
            checkBoxDViewUsePrimary.CheckedChanged += checkBoxDViewUsePrimary_CheckedChanged;

            //
            // groupBoxDViewClock
            //
            groupBoxDViewClock.Controls.AddRange(new Control[]
            {
                labelDViewAhead, picBoxDViewAhead, labelDViewAheadLosing, picBoxDViewAheadLosing,
                labelDViewBehind, picBoxDViewBehind, labelDViewBehindLosing, picBoxDViewBehindLosing,
                labelDViewFinished, picBoxDViewFinished, labelDViewRecord, picBoxDViewRecord,
                labelDViewDelay, picBoxDViewDelay, labelDViewPaused, picBoxDViewPaused,
                labelDViewFlash, picBoxDViewFlash
            });
            groupBoxDViewClock.Location = new Point(5, 30);
            groupBoxDViewClock.Name = "groupBoxDViewClock";
            groupBoxDViewClock.Size = new Size(237, 148);
            groupBoxDViewClock.TabStop = false;
            groupBoxDViewClock.Text = "Clock colors";
            //
            // groupBoxDViewSegments
            //
            groupBoxDViewSegments.Controls.AddRange(new Control[]
            {
                labelDViewSegHighlight, picBoxDViewSegHighlight, labelDViewSegDefaultText, picBoxDViewSegDefaultText,
                labelDViewSegCurrentText, picBoxDViewSegCurrentText, labelDViewSegMissingTime, picBoxDViewSegMissingTime,
                labelDViewSegBestSegment, picBoxDViewSegBestSegment, labelDViewSegAheadGain, picBoxDViewSegAheadGain,
                labelDViewSegAheadLoss, picBoxDViewSegAheadLoss, labelDViewSegBehindGain, picBoxDViewSegBehindGain,
                labelDViewSegBehindLoss, picBoxDViewSegBehindLoss
            });
            groupBoxDViewSegments.Location = new Point(5, 183);
            groupBoxDViewSegments.Name = "groupBoxDViewSegments";
            groupBoxDViewSegments.Size = new Size(237, 148);
            groupBoxDViewSegments.TabStop = false;
            groupBoxDViewSegments.Text = "Segments colors";
            //
            // groupBoxGraph
            //
            groupBoxGraph.Controls.AddRange(new Control[]
            {
                labelGraphAhead, picBoxGraphAhead, labelGraphBehind, picBoxGraphBehind
            });
            groupBoxGraph.Location = new Point(5, 336);
            groupBoxGraph.Name = "groupBoxGraph";
            groupBoxGraph.Size = new Size(237, 44);
            groupBoxGraph.TabStop = false;
            groupBoxGraph.Text = "Graph";

            //
            // labelDViewAhead
            //
            labelDViewAhead.AutoSize = true;
            labelDViewAhead.Location = new Point(5, 15);
            labelDViewAhead.MinimumSize = new Size(84, 20);
            labelDViewAhead.Name = "labelDViewAhead";
            labelDViewAhead.Text = "Ahead (gaining)";
            labelDViewAhead.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxDViewAhead
            //
            picBoxDViewAhead.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewAhead.Cursor = Cursors.Hand;
            picBoxDViewAhead.Location = new Point(95, 15);
            picBoxDViewAhead.Name = "picBoxDViewAhead";
            picBoxDViewAhead.Size = new Size(20, 20);
            picBoxDViewAhead.TabStop = false;
            picBoxDViewAhead.Click += SetPictureBoxColor;

            //
            // labelDViewAheadLosing
            //
            labelDViewAheadLosing.AutoSize = true;
            labelDViewAheadLosing.Location = new Point(148, 15);
            labelDViewAheadLosing.MinimumSize = new Size(84, 20);
            labelDViewAheadLosing.Name = "labelDViewAheadLosing";
            labelDViewAheadLosing.Text = "Ahead (losing)";
            labelDViewAheadLosing.TextAlign = ContentAlignment.MiddleLeft;
            //
            // picBoxDViewAheadLosing
            //
            picBoxDViewAheadLosing.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewAheadLosing.Cursor = Cursors.Hand;
            picBoxDViewAheadLosing.Location = new Point(122, 15);
            picBoxDViewAheadLosing.Name = "picBoxDViewAheadLosing";
            picBoxDViewAheadLosing.Size = new Size(20, 20);
            picBoxDViewAheadLosing.TabStop = false;
            picBoxDViewAheadLosing.Click += SetPictureBoxColor;

            //
            // labelDViewBehind
            //
            labelDViewBehind.AutoSize = true;
            labelDViewBehind.Location = new Point(5, 41);
            labelDViewBehind.MinimumSize = new Size(84, 20);
            labelDViewBehind.Name = "labelDViewBehind";
            labelDViewBehind.Text = "Behind (gaining)";
            labelDViewBehind.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxDViewBehind
            //
            picBoxDViewBehind.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewBehind.Cursor = Cursors.Hand;
            picBoxDViewBehind.Location = new Point(95, 41);
            picBoxDViewBehind.Name = "picBoxDViewBehind";
            picBoxDViewBehind.Size = new Size(20, 20);
            picBoxDViewBehind.TabStop = false;
            picBoxDViewBehind.Click += SetPictureBoxColor;

            //
            // labelDViewBehindLosing
            //
            labelDViewBehindLosing.AutoSize = true;
            labelDViewBehindLosing.Location = new Point(148, 41);
            labelDViewBehindLosing.MinimumSize = new Size(84, 20);
            labelDViewBehindLosing.Name = "labelDViewBehindLosing";
            labelDViewBehindLosing.Text = "Behind (losing)";
            labelDViewBehindLosing.TextAlign = ContentAlignment.MiddleLeft;
            //
            // picBoxDViewBehindLosing
            //
            picBoxDViewBehindLosing.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewBehindLosing.Cursor = Cursors.Hand;
            picBoxDViewBehindLosing.Location = new Point(122, 41);
            picBoxDViewBehindLosing.Name = "picBoxDViewBehindLosing";
            picBoxDViewBehindLosing.Size = new Size(20, 20);
            picBoxDViewBehindLosing.TabStop = false;
            picBoxDViewBehindLosing.Click += SetPictureBoxColor;

            //
            // labelDViewFinished
            //
            labelDViewFinished.AutoSize = true;
            labelDViewFinished.Location = new Point(5, 67);
            labelDViewFinished.MinimumSize = new Size(84, 20);
            labelDViewFinished.Name = "labelDViewFinished";
            labelDViewFinished.Text = "Finished";
            labelDViewFinished.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxDViewFinished
            //
            picBoxDViewFinished.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewFinished.Cursor = Cursors.Hand;
            picBoxDViewFinished.Location = new Point(95, 67);
            picBoxDViewFinished.Name = "picBoxDViewFinished";
            picBoxDViewFinished.Size = new Size(20, 20);
            picBoxDViewFinished.TabStop = false;
            picBoxDViewFinished.Click += SetPictureBoxColor;

            //
            // labelDViewRecord
            //
            labelDViewRecord.AutoSize = true;
            labelDViewRecord.Location = new Point(148, 67);
            labelDViewRecord.MinimumSize = new Size(84, 20);
            labelDViewRecord.Name = "labelDViewRecord";
            labelDViewRecord.Text = "New Record";
            labelDViewRecord.TextAlign = ContentAlignment.MiddleLeft;
            //
            // picBoxDViewRecord
            //
            picBoxDViewRecord.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewRecord.Cursor = Cursors.Hand;
            picBoxDViewRecord.Location = new Point(122, 67);
            picBoxDViewRecord.Name = "picBoxDViewRecord";
            picBoxDViewRecord.Size = new Size(20, 20);
            picBoxDViewRecord.TabStop = false;
            picBoxDViewRecord.Click += SetPictureBoxColor;

            //
            // labelDViewDelay
            //
            labelDViewDelay.AutoSize = true;
            labelDViewDelay.Location = new Point(5, 93);
            labelDViewDelay.MinimumSize = new Size(84, 20);
            labelDViewDelay.Name = "labelDViewDelay";
            labelDViewDelay.Text = "Delay";
            labelDViewDelay.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxDViewDelay
            //
            picBoxDViewDelay.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewDelay.Cursor = Cursors.Hand;
            picBoxDViewDelay.Location = new Point(95, 93);
            picBoxDViewDelay.Name = "picBoxDViewDelay";
            picBoxDViewDelay.Size = new Size(20, 20);
            picBoxDViewDelay.TabStop = false;
            picBoxDViewDelay.Click += SetPictureBoxColor;

            //
            // labelDViewPaused
            //
            labelDViewPaused.AutoSize = true;
            labelDViewPaused.Location = new Point(148, 93);
            labelDViewPaused.MinimumSize = new Size(84, 20);
            labelDViewPaused.Name = "labelDViewPaused";
            labelDViewPaused.Text = "Paused";
            labelDViewPaused.TextAlign = ContentAlignment.MiddleLeft;
            //
            // picBoxDViewPaused
            //
            picBoxDViewPaused.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewPaused.Cursor = Cursors.Hand;
            picBoxDViewPaused.Location = new Point(122, 93);
            picBoxDViewPaused.Name = "picBoxDViewPaused";
            picBoxDViewPaused.Size = new Size(20, 20);
            picBoxDViewPaused.TabStop = false;
            picBoxDViewPaused.Click += SetPictureBoxColor;

            //
            // labelDViewFlash
            //
            labelDViewFlash.AutoSize = true;
            labelDViewFlash.Location = new Point(5, 119);
            labelDViewFlash.MinimumSize = new Size(84, 20);
            labelDViewFlash.Name = "labelDViewFlash";
            labelDViewFlash.Text = "Flashing";
            labelDViewFlash.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxDViewFlash
            //
            picBoxDViewFlash.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewFlash.Cursor = Cursors.Hand;
            picBoxDViewFlash.Location = new Point(95, 119);
            picBoxDViewFlash.Name = "picBoxDViewFlash";
            picBoxDViewFlash.Size = new Size(20, 20);
            picBoxDViewFlash.TabStop = false;
            picBoxDViewFlash.Click += SetPictureBoxColor;

            //
            // labelDViewSegCurrentText
            //
            labelDViewSegCurrentText.AutoSize = true;
            labelDViewSegCurrentText.Location = new Point(5, 15);
            labelDViewSegCurrentText.MinimumSize = new Size(84, 20);
            labelDViewSegCurrentText.Name = "labelDViewSegCurrentText";
            labelDViewSegCurrentText.Text = "Comparison text";
            labelDViewSegCurrentText.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxDViewSegCurrentText
            //
            picBoxDViewSegCurrentText.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewSegCurrentText.Cursor = Cursors.Hand;
            picBoxDViewSegCurrentText.Location = new Point(95, 15);
            picBoxDViewSegCurrentText.Name = "picBoxDViewSegCurrentText";
            picBoxDViewSegCurrentText.Size = new Size(20, 20);
            picBoxDViewSegCurrentText.TabStop = false;
            picBoxDViewSegCurrentText.Click += SetPictureBoxColor;

            //
            // labelDViewSegDefaultText
            //
            labelDViewSegDefaultText.AutoSize = true;
            labelDViewSegDefaultText.Location = new Point(148, 15);
            labelDViewSegDefaultText.MinimumSize = new Size(84, 20);
            labelDViewSegDefaultText.Name = "labelDViewSegDefaultText";
            labelDViewSegDefaultText.Text = "Default text";
            labelDViewSegDefaultText.TextAlign = ContentAlignment.MiddleLeft;
            //
            // picBoxDViewSegDefaultText
            //
            picBoxDViewSegDefaultText.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewSegDefaultText.Cursor = Cursors.Hand;
            picBoxDViewSegDefaultText.Location = new Point(122, 15);
            picBoxDViewSegDefaultText.Name = "picBoxDViewSegDefaultText";
            picBoxDViewSegDefaultText.Size = new Size(20, 20);
            picBoxDViewSegDefaultText.TabStop = false;
            picBoxDViewSegDefaultText.Click += SetPictureBoxColor;

            //
            // labelDViewSegMissingTime
            //
            labelDViewSegMissingTime.AutoSize = true;
            labelDViewSegMissingTime.Location = new Point(5, 41);
            labelDViewSegMissingTime.MinimumSize = new Size(84, 20);
            labelDViewSegMissingTime.Name = "labelDViewSegMissingTime";
            labelDViewSegMissingTime.Text = "Missing time";
            labelDViewSegMissingTime.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxDViewSegMissingTime
            //
            picBoxDViewSegMissingTime.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewSegMissingTime.Cursor = Cursors.Hand;
            picBoxDViewSegMissingTime.Location = new Point(95, 41);
            picBoxDViewSegMissingTime.Name = "picBoxDViewSegMissingTime";
            picBoxDViewSegMissingTime.Size = new Size(20, 20);
            picBoxDViewSegMissingTime.TabStop = false;
            picBoxDViewSegMissingTime.Click += SetPictureBoxColor;

            //
            // labelDViewSegBestSegment
            //
            labelDViewSegBestSegment.AutoSize = true;
            labelDViewSegBestSegment.Location = new Point(148, 41);
            labelDViewSegBestSegment.MinimumSize = new Size(84, 20);
            labelDViewSegBestSegment.Name = "labelDViewSegBestSegment";
            labelDViewSegBestSegment.Text = "Best segment";
            labelDViewSegBestSegment.TextAlign = ContentAlignment.MiddleLeft;
            //
            // picBoxDViewSegBestSegment
            //
            picBoxDViewSegBestSegment.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewSegBestSegment.Cursor = Cursors.Hand;
            picBoxDViewSegBestSegment.Location = new Point(122, 41);
            picBoxDViewSegBestSegment.Name = "picBoxDViewSegBestSegment";
            picBoxDViewSegBestSegment.Size = new Size(20, 20);
            picBoxDViewSegBestSegment.TabStop = false;
            picBoxDViewSegBestSegment.Click += SetPictureBoxColor;

            //
            // labelDViewSegAheadGain
            //
            labelDViewSegAheadGain.AutoSize = true;
            labelDViewSegAheadGain.Location = new Point(5, 67);
            labelDViewSegAheadGain.MinimumSize = new Size(84, 20);
            labelDViewSegAheadGain.Name = "labelDViewSegAheadGain";
            labelDViewSegAheadGain.Text = "Ahead (gained)";
            labelDViewSegAheadGain.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxDViewSegAheadGain
            //
            picBoxDViewSegAheadGain.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewSegAheadGain.Cursor = Cursors.Hand;
            picBoxDViewSegAheadGain.Location = new Point(95, 67);
            picBoxDViewSegAheadGain.Name = "picBoxDViewSegAheadGain";
            picBoxDViewSegAheadGain.Size = new Size(20, 20);
            picBoxDViewSegAheadGain.TabStop = false;
            picBoxDViewSegAheadGain.Click += SetPictureBoxColor;

            //
            // labelDViewSegAheadLoss
            //
            labelDViewSegAheadLoss.AutoSize = true;
            labelDViewSegAheadLoss.Location = new Point(148, 67);
            labelDViewSegAheadLoss.MinimumSize = new Size(84, 20);
            labelDViewSegAheadLoss.Name = "labelDViewSegAheadLoss";
            labelDViewSegAheadLoss.Text = "Ahead (lost)";
            labelDViewSegAheadLoss.TextAlign = ContentAlignment.MiddleLeft;
            //
            // picBoxDViewSegAheadLoss
            //
            picBoxDViewSegAheadLoss.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewSegAheadLoss.Cursor = Cursors.Hand;
            picBoxDViewSegAheadLoss.Location = new Point(122, 67);
            picBoxDViewSegAheadLoss.Name = "picBoxDViewSegAheadLoss";
            picBoxDViewSegAheadLoss.Size = new Size(20, 20);
            picBoxDViewSegAheadLoss.TabStop = false;
            picBoxDViewSegAheadLoss.Click += SetPictureBoxColor;

            //
            // labelDViewSegBehindGain
            //
            labelDViewSegBehindGain.AutoSize = true;
            labelDViewSegBehindGain.Location = new Point(5, 93);
            labelDViewSegBehindGain.MinimumSize = new Size(84, 20);
            labelDViewSegBehindGain.Name = "labelDViewSegBehindGain";
            labelDViewSegBehindGain.Text = "Behind (gained)";
            labelDViewSegBehindGain.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxDViewSegBehindGain
            //
            picBoxDViewSegBehindGain.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewSegBehindGain.Cursor = Cursors.Hand;
            picBoxDViewSegBehindGain.Location = new Point(95, 93);
            picBoxDViewSegBehindGain.Name = "picBoxDViewSegBehindGain";
            picBoxDViewSegBehindGain.Size = new Size(20, 20);
            picBoxDViewSegBehindGain.TabStop = false;
            picBoxDViewSegBehindGain.Click += SetPictureBoxColor;

            //
            // labelDViewSegBehindLoss
            //
            labelDViewSegBehindLoss.AutoSize = true;
            labelDViewSegBehindLoss.Location = new Point(148, 93);
            labelDViewSegBehindLoss.MinimumSize = new Size(84, 20);
            labelDViewSegBehindLoss.Name = "labelDViewSegBehindLoss";
            labelDViewSegBehindLoss.Text = "Behind (lost)";
            labelDViewSegBehindLoss.TextAlign = ContentAlignment.MiddleLeft;
            //
            // picBoxDViewSegBehindLoss
            //
            picBoxDViewSegBehindLoss.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewSegBehindLoss.Cursor = Cursors.Hand;
            picBoxDViewSegBehindLoss.Location = new Point(122, 93);
            picBoxDViewSegBehindLoss.Name = "picBoxDViewSegBehindLoss";
            picBoxDViewSegBehindLoss.Size = new Size(20, 20);
            picBoxDViewSegBehindLoss.TabStop = false;
            picBoxDViewSegBehindLoss.Click += SetPictureBoxColor;

            //
            // labelDViewSegHighlight
            //
            labelDViewSegHighlight.AutoSize = true;
            labelDViewSegHighlight.Location = new Point(5, 119);
            labelDViewSegHighlight.MinimumSize = new Size(84, 20);
            labelDViewSegHighlight.Name = "labelDViewSegHighlight";
            labelDViewSegHighlight.Text = "Current highlight";
            labelDViewSegHighlight.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxDViewSegHighlight
            //
            picBoxDViewSegHighlight.BorderStyle = BorderStyle.FixedSingle;
            picBoxDViewSegHighlight.Cursor = Cursors.Hand;
            picBoxDViewSegHighlight.Location = new Point(95, 119);
            picBoxDViewSegHighlight.Name = "picBoxDViewSegHighlight";
            picBoxDViewSegHighlight.Size = new Size(20, 20);
            picBoxDViewSegHighlight.TabStop = false;
            picBoxDViewSegHighlight.Click += SetPictureBoxColor;

            //
            // labelGraphAhead
            //
            labelGraphAhead.AutoSize = true;
            labelGraphAhead.Location = new Point(5, 15);
            labelGraphAhead.MinimumSize = new Size(84, 20);
            labelGraphAhead.Name = "labelGraphAhead";
            labelGraphAhead.Text = "Ahead";
            labelGraphAhead.TextAlign = ContentAlignment.MiddleRight;
            //
            // picBoxGraphAhead
            //
            picBoxGraphAhead.BorderStyle = BorderStyle.FixedSingle;
            picBoxGraphAhead.Cursor = Cursors.Hand;
            picBoxGraphAhead.Location = new Point(95, 15);
            picBoxGraphAhead.Name = "picBoxGraphAhead";
            picBoxGraphAhead.Size = new Size(20, 20);
            picBoxGraphAhead.TabStop = false;
            picBoxGraphAhead.Click += SetPictureBoxColor;

            //
            // labelGraphBehind
            //
            labelGraphBehind.AutoSize = true;
            labelGraphBehind.Location = new Point(148, 15);
            labelGraphBehind.MinimumSize = new Size(84, 20);
            labelGraphBehind.Name = "labelGraphBehind";
            labelGraphBehind.Text = "Behind";
            labelGraphBehind.TextAlign = ContentAlignment.MiddleLeft;
            //
            // picBoxGraphBehind
            //
            picBoxGraphBehind.BorderStyle = BorderStyle.FixedSingle;
            picBoxGraphBehind.Cursor = Cursors.Hand;
            picBoxGraphBehind.Location = new Point(122, 15);
            picBoxGraphBehind.Name = "picBoxGraphBehind";
            picBoxGraphBehind.Size = new Size(20, 20);
            picBoxGraphBehind.TabStop = false;
            picBoxGraphBehind.Click += SetPictureBoxColor;
            //
            // pictureBox1
            //
            picturebox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            picturebox1.Cursor = System.Windows.Forms.Cursors.Hand;
            picturebox1.Location = new System.Drawing.Point(180, 97);
            picturebox1.Name = "picturebox1";
            picturebox1.Size = new System.Drawing.Size(20, 20);
            picturebox1.TabIndex = 35;
            picturebox1.TabStop = false;
            picturebox1.Click += SetPictureBoxColor;

            colorTabs.ResumeLayout(false);
            tabPageClockColors.ResumeLayout(false);
            tabPageClockColors.PerformLayout();
            tabPageSegColors.ResumeLayout(false);
            tabPageSegColors.PerformLayout();
            tabPageDetailedViewColors.ResumeLayout(false);
            tabPageDetailedViewColors.PerformLayout();
            groupBoxBackground.ResumeLayout(false);
            groupBoxBackground.PerformLayout();
            groupBoxDViewClock.ResumeLayout(false);
            groupBoxDViewClock.PerformLayout();
            groupBoxDViewSegments.ResumeLayout(false);
            groupBoxDViewSegments.PerformLayout();
            groupBoxGraph.ResumeLayout(false);
            groupBoxGraph.PerformLayout();
            ((ISupportInitialize)picBoxAheadBackPlain).EndInit();
            ((ISupportInitialize)picBoxRunTitleBackPlain).EndInit();
            ((ISupportInitialize)picBoxAheadLosingBackPlain).EndInit();
            ((ISupportInitialize)picBoxBehindBackPlain).EndInit();
            ((ISupportInitialize)picBoxBehindLosingBackPlain).EndInit();
            ((ISupportInitialize)picBoxStatusBarBackPlain).EndInit();
            ((ISupportInitialize)picBoxNoLoadedBackPlain).EndInit();
            ((ISupportInitialize)picBoxFinishedBackPlain).EndInit();
            ((ISupportInitialize)picBoxRecordBackPlain).EndInit();
            ((ISupportInitialize)picBoxDelayBackPlain).EndInit();
            ((ISupportInitialize)picBoxAheadBack2).EndInit();
            ((ISupportInitialize)picBoxRunTitleBack2).EndInit();
            ((ISupportInitialize)picBoxAheadLosingBack2).EndInit();
            ((ISupportInitialize)picBoxBehindBack2).EndInit();
            ((ISupportInitialize)picBoxBehindLosingBack2).EndInit();
            ((ISupportInitialize)picBoxStatusBarBack2).EndInit();
            ((ISupportInitialize)picBoxNoLoadedBack2).EndInit();
            ((ISupportInitialize)picBoxFinishedBack2).EndInit();
            ((ISupportInitialize)picBoxRecordBack2).EndInit();
            ((ISupportInitialize)picBoxDelayBack2).EndInit();
            ((ISupportInitialize)picBoxAheadBack).EndInit();
            ((ISupportInitialize)picBoxRunTitleBack).EndInit();
            ((ISupportInitialize)picBoxAheadLosingBack).EndInit();
            ((ISupportInitialize)picBoxBehindBack).EndInit();
            ((ISupportInitialize)picBoxBehindLosingBack).EndInit();
            ((ISupportInitialize)picBoxStatusBarBack).EndInit();
            ((ISupportInitialize)picBoxNoLoadedBack).EndInit();
            ((ISupportInitialize)picBoxFinishedBack).EndInit();
            ((ISupportInitialize)picBoxRecordBack).EndInit();
            ((ISupportInitialize)picBoxDelayBack).EndInit();
            ((ISupportInitialize)picBoxRunTitleFore).EndInit();
            ((ISupportInitialize)picBoxStatusBarFore).EndInit();
            ((ISupportInitialize)picBoxDelayFore).EndInit();
            ((ISupportInitialize)picBoxRecordFore).EndInit();
            ((ISupportInitialize)picBoxFinishedFore).EndInit();
            ((ISupportInitialize)picBoxFlash).EndInit();
            ((ISupportInitialize)picBoxPaused).EndInit();
            ((ISupportInitialize)picBoxNoLoadedFore).EndInit();
            ((ISupportInitialize)picBoxBehindLosingFore).EndInit();
            ((ISupportInitialize)picBoxBehindFore).EndInit();
            ((ISupportInitialize)picBoxAheadLosingFore).EndInit();
            ((ISupportInitialize)picBoxAheadFore).EndInit();
            ((ISupportInitialize)picBoxSegHighlightPlain).EndInit();
            ((ISupportInitialize)picBoxSegBackgroundPlain).EndInit();
            ((ISupportInitialize)picBoxSegHighlight2).EndInit();
            ((ISupportInitialize)picBoxSegBackground2).EndInit();
            ((ISupportInitialize)picBoxSegHighlightBorder).EndInit();
            ((ISupportInitialize)picBoxSegBehindLoss).EndInit();
            ((ISupportInitialize)picBoxSegBehindGain).EndInit();
            ((ISupportInitialize)picBoxSegAheadLoss).EndInit();
            ((ISupportInitialize)picBoxSegAheadGain).EndInit();
            ((ISupportInitialize)picBoxSegMissing).EndInit();
            ((ISupportInitialize)picBoxSegNewTime).EndInit();
            ((ISupportInitialize)picBoxSegBestSegment).EndInit();
            ((ISupportInitialize)picBoxSegFutureTime).EndInit();
            ((ISupportInitialize)picBoxSegFutureText).EndInit();
            ((ISupportInitialize)picBoxSegLiveText).EndInit();
            ((ISupportInitialize)picBoxSegPastText).EndInit();
            ((ISupportInitialize)picBoxSegHighlight).EndInit();
            ((ISupportInitialize)picBoxSegBackground).EndInit();
            ((ISupportInitialize)picBoxDViewAhead).EndInit();
            ((ISupportInitialize)picBoxDViewAheadLosing).EndInit();
            ((ISupportInitialize)picBoxDViewBehind).EndInit();
            ((ISupportInitialize)picBoxDViewBehindLosing).EndInit();
            ((ISupportInitialize)picBoxDViewFinished).EndInit();
            ((ISupportInitialize)picBoxDViewRecord).EndInit();
            ((ISupportInitialize)picBoxDViewDelay).EndInit();
            ((ISupportInitialize)picBoxDViewPaused).EndInit();
            ((ISupportInitialize)picBoxDViewFlash).EndInit();
            ((ISupportInitialize)picBoxDViewSegCurrentText).EndInit();
            ((ISupportInitialize)picBoxDViewSegDefaultText).EndInit();
            ((ISupportInitialize)picBoxDViewSegMissingTime).EndInit();
            ((ISupportInitialize)picBoxDViewSegBestSegment).EndInit();
            ((ISupportInitialize)picBoxDViewSegAheadGain).EndInit();
            ((ISupportInitialize)picBoxDViewSegAheadLoss).EndInit();
            ((ISupportInitialize)picBoxDViewSegBehindGain).EndInit();
            ((ISupportInitialize)picBoxDViewSegBehindLoss).EndInit();
            ((ISupportInitialize)picBoxDViewSegHighlight).EndInit();
            ((ISupportInitialize)picturebox1).EndInit();
            ((ISupportInitialize)picBoxPreview).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        public void PopulateColors()
        {
            foreach (SettingPair setting in ColorSettings)
            {
                setting.pb.BackColor = (Color)Properties.ColorSettings.Profile[setting.name];
            }
            checkBoxDViewUsePrimary.Checked = Properties.ColorSettings.Profile.DViewUsePrimary;
        }

        private void PopulateSettings()
        {
            ColorSettings.AddRange(new SettingPair[]
            {
                new SettingPair("AheadFore", picBoxAheadFore),
                new SettingPair("AheadBack", picBoxAheadBack),
                new SettingPair("AheadBack2", picBoxAheadBack2),
                new SettingPair("AheadBackPlain", picBoxAheadBackPlain),
                new SettingPair("AheadLosingFore", picBoxAheadLosingFore),
                new SettingPair("AheadLosingBack", picBoxAheadLosingBack),
                new SettingPair("AheadLosingBack2", picBoxAheadLosingBack2),
                new SettingPair("AheadLosingBackPlain", picBoxAheadLosingBackPlain),
                new SettingPair("BehindFore", picBoxBehindFore),
                new SettingPair("BehindBack", picBoxBehindBack),
                new SettingPair("BehindBack2", picBoxBehindBack2),
                new SettingPair("BehindBackPlain", picBoxBehindBackPlain),
                new SettingPair("BehindLosingFore", picBoxBehindLosingFore),
                new SettingPair("BehindLosingBack", picBoxBehindLosingBack),
                new SettingPair("BehindLosingBack2", picBoxBehindLosingBack2),
                new SettingPair("BehindLosingBackPlain", picBoxBehindLosingBackPlain),
                new SettingPair("WatchFore", picBoxNoLoadedFore),
                new SettingPair("SegPastTime", picturebox1),
                new SettingPair("WatchBack", picBoxNoLoadedBack),
                new SettingPair("WatchBack2", picBoxNoLoadedBack2),
                new SettingPair("WatchBackPlain", picBoxNoLoadedBackPlain),
                new SettingPair("Paused", picBoxPaused),
                new SettingPair("Flash", picBoxFlash),
                new SettingPair("FinishedFore", picBoxFinishedFore),
                new SettingPair("FinishedBack", picBoxFinishedBack),
                new SettingPair("FinishedBack2", picBoxFinishedBack2),
                new SettingPair("FinishedBackPlain", picBoxFinishedBackPlain),
                new SettingPair("RecordFore", picBoxRecordFore),
                new SettingPair("RecordBack", picBoxRecordBack),
                new SettingPair("RecordBack2", picBoxRecordBack2),
                new SettingPair("RecordBackPlain", picBoxRecordBackPlain),
                new SettingPair("DelayFore", picBoxDelayFore),
                new SettingPair("DelayBack", picBoxDelayBack),
                new SettingPair("DelayBack2", picBoxDelayBack2),
                new SettingPair("DelayBackPlain", picBoxDelayBackPlain),
                new SettingPair("StatusFore", picBoxStatusBarFore),
                new SettingPair("StatusBack", picBoxStatusBarBack),
                new SettingPair("StatusBack2", picBoxStatusBarBack2),
                new SettingPair("StatusBackPlain", picBoxStatusBarBackPlain),
                new SettingPair("TitleFore", picBoxRunTitleFore),
                new SettingPair("TitleBack", picBoxRunTitleBack),
                new SettingPair("TitleBack2", picBoxRunTitleBack2),
                new SettingPair("TitleBackPlain", picBoxRunTitleBackPlain),
                new SettingPair("SegBack", picBoxSegBackground),
                new SettingPair("SegBack2", picBoxSegBackground2),
                new SettingPair("SegBackPlain", picBoxSegBackgroundPlain),
                new SettingPair("SegHighlight", picBoxSegHighlight),
                new SettingPair("SegHighlight2", picBoxSegHighlight2),
                new SettingPair("SegHighlightPlain", picBoxSegHighlightPlain),
                new SettingPair("SegHighlightBorder", picBoxSegHighlightBorder),
                new SettingPair("PastSeg", picBoxSegPastText),
                new SettingPair("LiveSeg", picBoxSegLiveText),
                new SettingPair("FutureSegName", picBoxSegFutureText),
                new SettingPair("FutureSegTime", picBoxSegFutureTime),
                new SettingPair("SegNewTime", picBoxSegNewTime),
                new SettingPair("SegMissingTime", picBoxSegMissing),
                new SettingPair("SegAheadGain", picBoxSegAheadGain),
                new SettingPair("SegAheadLoss", picBoxSegAheadLoss),
                new SettingPair("SegBehindGain", picBoxSegBehindGain),
                new SettingPair("SegBehindLoss", picBoxSegBehindLoss),
                new SettingPair("SegBestSegment", picBoxSegBestSegment),
                new SettingPair("DViewAhead", picBoxDViewAhead),
                new SettingPair("DViewAheadLosing", picBoxDViewAheadLosing),
                new SettingPair("DViewBehind", picBoxDViewBehind),
                new SettingPair("DViewBehindLosing", picBoxDViewBehindLosing),
                new SettingPair("DViewFinished", picBoxDViewFinished),
                new SettingPair("DViewRecord", picBoxDViewRecord),
                new SettingPair("DViewDelay", picBoxDViewDelay),
                new SettingPair("DViewPaused", picBoxDViewPaused),
                new SettingPair("DViewFlash", picBoxDViewFlash),
                new SettingPair("DViewSegCurrentText", picBoxDViewSegCurrentText),
                new SettingPair("DViewSegDefaultText", picBoxDViewSegDefaultText),
                new SettingPair("DViewSegMissingTime", picBoxDViewSegMissingTime),
                new SettingPair("DViewSegBestSegment", picBoxDViewSegBestSegment),
                new SettingPair("DViewSegAheadGain", picBoxDViewSegAheadGain),
                new SettingPair("DViewSegAheadLoss", picBoxDViewSegAheadLoss),
                new SettingPair("DViewSegBehindGain", picBoxDViewSegBehindGain),
                new SettingPair("DViewSegBehindLoss", picBoxDViewSegBehindLoss),
                new SettingPair("DViewSegHighlight", picBoxDViewSegHighlight),
                new SettingPair("GraphAhead", picBoxGraphAhead),
                new SettingPair("GraphBehind", picBoxGraphBehind)
            });
        }

        public void LoadColors()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string str;
                    StreamReader reader = new StreamReader(dialog.FileName);
                    List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                    while ((str = reader.ReadLine()) != null)
                    {
                        string[] strArray = str.Split('=');
                        if (strArray.Length == 2)
                        {
                            list.Add(new KeyValuePair<string, string>(strArray[0], strArray[1]));
                        }
                    }
                    /*using (List<KeyValuePair<String, String>>.Enumerator enumerator = list.GetEnumerator())
                    {
                        KeyValuePair<String, String> sp;
                        while (enumerator.MoveNext())
                        {
                            sp = enumerator.Current;
                            foreach (SettingPair setting in from cs in this.ColorSettings
                                                             where cs.name == sp.Key
                                                             select cs)
                            {
                                setting.pb.BackColor = ColorTranslator.FromHtml(sp.Value);
                            }
                        }
                    }*/

                    foreach (KeyValuePair<string, string> pair in list)
                    {
                        foreach (SettingPair sp in from cs in ColorSettings
                                                   where cs.name == pair.Key
                                                   select cs)
                        {
                            sp.pb.BackColor = ColorTranslator.FromHtml(pair.Value);
                        }
                    }
                }
                finally
                {
                    picBoxPreview.Invalidate();
                }
            }
        }

        public void SaveColors()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamWriter writer = new StreamWriter(dialog.FileName);
                    foreach (SettingPair setting in ColorSettings)
                    {
                        writer.WriteLine(setting.name + "=" + ColorTranslator.ToHtml(setting.pb.BackColor));
                    }
                    writer.Close();
                }
                catch
                {
                }
            }
        }

        private void labelAhead_Click(object sender, EventArgs e)
        {
            previewClockType = ClockType.Ahead;
            picBoxPreview.Invalidate();
        }

        private void labelAheadLosing_Click(object sender, EventArgs e)
        {
            previewClockType = ClockType.AheadLosing;
            picBoxPreview.Invalidate();
        }

        private void labelBehind_Click(object sender, EventArgs e)
        {
            previewClockType = ClockType.Behind;
            picBoxPreview.Invalidate();
        }

        private void labelBehindLosing_Click(object sender, EventArgs e)
        {
            previewClockType = ClockType.BehindLosing;
            picBoxPreview.Invalidate();
        }

        private void labelNoLoaded_Click(object sender, EventArgs e)
        {
            previewClockType = ClockType.NoRun;
            picBoxPreview.Invalidate();
        }

        private void labelFinished_Click(object sender, EventArgs e)
        {
            previewClockType = ClockType.Finished;
            picBoxPreview.Invalidate();
        }

        private void labelRecord_Click(object sender, EventArgs e)
        {
            previewClockType = ClockType.NewRecord;
            picBoxPreview.Invalidate();
        }

        private void labelDelay_Click(object sender, EventArgs e)
        {
            previewClockType = ClockType.Delay;
            picBoxPreview.Invalidate();
        }

        private void labelPaused_Click(object sender, EventArgs e)
        {
            previewClockType = ClockType.Paused;
            picBoxPreview.Invalidate();
        }

        private void labelFlash_Click(object sender, EventArgs e)
        {
            previewClockType = ClockType.Flash;
            picBoxPreview.Invalidate();
        }

        private void checkBoxDViewUsePrimary_CheckedChanged(object sender, EventArgs e)
        {
            bool state = !checkBoxDViewUsePrimary.Checked;
            int alpha = (state) ? 255 : 128;

            groupBoxDViewClock.Enabled = state;
            groupBoxDViewSegments.Enabled = state;

            picBoxDViewAhead.BackColor = Color.FromArgb(alpha, picBoxDViewAhead.BackColor);
            picBoxDViewAheadLosing.BackColor = Color.FromArgb(alpha, picBoxDViewAheadLosing.BackColor);
            picBoxDViewBehind.BackColor = Color.FromArgb(alpha, picBoxDViewBehind.BackColor);
            picBoxDViewBehindLosing.BackColor = Color.FromArgb(alpha, picBoxDViewBehindLosing.BackColor);
            picBoxDViewFinished.BackColor = Color.FromArgb(alpha, picBoxDViewFinished.BackColor);
            picBoxDViewRecord.BackColor = Color.FromArgb(alpha, picBoxDViewRecord.BackColor);
            picBoxDViewDelay.BackColor = Color.FromArgb(alpha, picBoxDViewDelay.BackColor);
            picBoxDViewPaused.BackColor = Color.FromArgb(alpha, picBoxDViewPaused.BackColor);
            picBoxDViewFlash.BackColor = Color.FromArgb(alpha, picBoxDViewFlash.BackColor);
            picBoxDViewSegCurrentText.BackColor = Color.FromArgb(alpha, picBoxDViewSegCurrentText.BackColor);
            picBoxDViewSegDefaultText.BackColor = Color.FromArgb(alpha, picBoxDViewSegDefaultText.BackColor);
            picBoxDViewSegMissingTime.BackColor = Color.FromArgb(alpha, picBoxDViewSegMissingTime.BackColor);
            picBoxDViewSegBestSegment.BackColor = Color.FromArgb(alpha, picBoxDViewSegBestSegment.BackColor);
            picBoxDViewSegAheadGain.BackColor = Color.FromArgb(alpha, picBoxDViewSegAheadGain.BackColor);
            picBoxDViewSegAheadLoss.BackColor = Color.FromArgb(alpha, picBoxDViewSegAheadLoss.BackColor);
            picBoxDViewSegBehindGain.BackColor = Color.FromArgb(alpha, picBoxDViewSegBehindGain.BackColor);
            picBoxDViewSegBehindLoss.BackColor = Color.FromArgb(alpha, picBoxDViewSegBehindLoss.BackColor);
            picBoxDViewSegHighlight.BackColor = Color.FromArgb(alpha, picBoxDViewSegHighlight.BackColor);
        }

        private void plainBg_CheckedChanged(object sender, EventArgs e)
        {
            picBoxPreview.Invalidate();
        }

        private void buttonDefaultColors_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx.Show(this, "Are you sure?", "Restore Defaults", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                bool flag = false;
                foreach (SettingsPropertyValue v in Properties.ColorSettings.Profile.PropertyValues)
                {
                    foreach (SettingPair setting in from cs in ColorSettings
                                                    where cs.name == v.Name
                                                    select cs)
                    {
                        try
                        {
                            setting.pb.BackColor = (Color)converter.ConvertFrom(null, CultureInfo.GetCultureInfo(""), v.Property.DefaultValue);
                        }
                        catch
                        {
                            flag = true;
                        }
                    }
                }

                if (flag)
                    MessageBoxEx.Show(this, "A problem was encountered while attempting to restore color defaults.");

                checkBoxDViewUsePrimary.Checked = Properties.ColorSettings.Profile.DViewUsePrimary;
                picBoxPreview.Invalidate();
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveColors();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            LoadColors();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Properties.ColorSettings.Profile.DViewUsePrimary = checkBoxDViewUsePrimary.Checked;
            foreach (SettingPair setting in ColorSettings)
            {
                Properties.ColorSettings.Profile[setting.name] = Color.FromArgb(255, setting.pb.BackColor);
            }
            base.DialogResult = DialogResult.OK;
        }

        private void previewBox_Paint(object sender, PaintEventArgs e)
        {
            Brush brush;
            Color backColor;
            Color color2;
            Color color3;
            Graphics graphics = e.Graphics;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.Clear(Color.Black);
            Rectangle rect = new Rectangle(0, (picBoxPreview.Height - 0x12) - 0x1a, 0x7c, 0x1a);
            switch (previewClockType)
            {
                case ClockType.Ahead:
                    brush = new SolidBrush(picBoxAheadFore.BackColor);
                    backColor = picBoxAheadBack.BackColor;
                    color2 = picBoxAheadBack2.BackColor;
                    color3 = picBoxAheadBackPlain.BackColor;
                    break;

                case ClockType.AheadLosing:
                    brush = new SolidBrush(picBoxAheadLosingFore.BackColor);
                    backColor = picBoxAheadLosingBack.BackColor;
                    color2 = picBoxAheadLosingBack2.BackColor;
                    color3 = picBoxAheadLosingBackPlain.BackColor;
                    break;

                case ClockType.Behind:
                    brush = new SolidBrush(picBoxBehindFore.BackColor);
                    backColor = picBoxBehindBack.BackColor;
                    color2 = picBoxBehindBack2.BackColor;
                    color3 = picBoxBehindBackPlain.BackColor;
                    break;

                case ClockType.BehindLosing:
                    brush = new SolidBrush(picBoxBehindLosingFore.BackColor);
                    backColor = picBoxBehindLosingBack.BackColor;
                    color2 = picBoxBehindLosingBack2.BackColor;
                    color3 = picBoxBehindLosingBackPlain.BackColor;
                    break;

                case ClockType.Delay:
                    brush = new SolidBrush(picBoxDelayFore.BackColor);
                    backColor = picBoxDelayBack.BackColor;
                    color2 = picBoxDelayBack2.BackColor;
                    color3 = picBoxDelayBackPlain.BackColor;
                    break;

                case ClockType.NoRun:
                    brush = new SolidBrush(picBoxNoLoadedFore.BackColor);
                    backColor = picBoxNoLoadedBack.BackColor;
                    color2 = picBoxNoLoadedBack2.BackColor;
                    color3 = picBoxNoLoadedBackPlain.BackColor;
                    break;

                case ClockType.Paused:
                    brush = new SolidBrush(picBoxPaused.BackColor);
                    backColor = picBoxAheadBack.BackColor;
                    color2 = picBoxAheadBack2.BackColor;
                    color3 = picBoxAheadBackPlain.BackColor;
                    break;

                case ClockType.NewRecord:
                    brush = new SolidBrush(picBoxRecordFore.BackColor);
                    backColor = picBoxRecordBack.BackColor;
                    color2 = picBoxRecordBack2.BackColor;
                    color3 = picBoxRecordBackPlain.BackColor;
                    break;

                case ClockType.Finished:
                    brush = new SolidBrush(picBoxFinishedFore.BackColor);
                    backColor = picBoxFinishedBack.BackColor;
                    color2 = picBoxFinishedBack2.BackColor;
                    color3 = picBoxFinishedBackPlain.BackColor;
                    break;

                case ClockType.Flash:
                    brush = new SolidBrush(picBoxFlash.BackColor);
                    backColor = picBoxAheadBack.BackColor;
                    color2 = picBoxAheadBack2.BackColor;
                    color3 = picBoxAheadBackPlain.BackColor;
                    break;

                default:
                    brush = new SolidBrush(picBoxAheadFore.BackColor);
                    backColor = picBoxAheadBack.BackColor;
                    color2 = picBoxAheadBack2.BackColor;
                    color3 = picBoxAheadBackPlain.BackColor;
                    break;
            }
            if (checkBoxPlainBg.Checked)
            {
                graphics.FillRectangle(new SolidBrush(color3), rect);
            }
            else
            {
                graphics.FillRectangle(new LinearGradientBrush(rect, backColor, color2, 0f), rect);
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(0x56, color2)), rect.X, rect.Y, rect.Width, rect.Height / 2);
            }
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            StringFormat format = new StringFormat
            {
                LineAlignment = StringAlignment.Center
            };
            Rectangle layoutRectangle = new Rectangle(0, rect.Top + 1, 0x5f, 0x1a);
            Rectangle rectangle3 = new Rectangle(layoutRectangle.Right - 5, layoutRectangle.Y + 5, 30, 0x12);
            graphics.DrawString(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, clockM, brush, rectangle3, format);
            rectangle3.X = layoutRectangle.Right;
            format.Alignment = StringAlignment.Far;
            graphics.DrawString("8:88", clockL, brush, layoutRectangle, format);
            format.Alignment = StringAlignment.Near;
            graphics.DrawString("88", clockM, brush, rectangle3, format);
            Rectangle rectangle4 = new Rectangle(0, rect.Bottom, picBoxPreview.Width, 0x12);
            Rectangle rectangle5 = new Rectangle(1, rect.Bottom + 2, picBoxPreview.Width - 1, 0x10);
            if (checkBoxPlainBg.Checked)
            {
                graphics.FillRectangle(new SolidBrush(picBoxStatusBarBackPlain.BackColor), rectangle4);
                graphics.FillRectangle(new SolidBrush(picBoxRunTitleBackPlain.BackColor), 0, 0, picBoxPreview.Width, 0x12);
            }
            else
            {
                graphics.FillRectangle(new LinearGradientBrush(rectangle4, picBoxStatusBarBack.BackColor, picBoxStatusBarBack2.BackColor, 0f), rectangle4);
                graphics.FillRectangle(new SolidBrush(picBoxRunTitleBack.BackColor), 0, 0, picBoxPreview.Width, 0x12);
                graphics.FillRectangle(new SolidBrush(picBoxRunTitleBack2.BackColor), 0, 0, picBoxPreview.Width, 9);
            }
            StringFormat format2 = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
            StringFormat format3 = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                Alignment = StringAlignment.Far
            };
            StringFormat format4 = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            graphics.DrawString("segment delta", displayFont, new SolidBrush(picBoxStatusBarFore.BackColor), rectangle5, format4);
            graphics.DrawString("Run Title", displayFont, new SolidBrush(picBoxRunTitleFore.BackColor), new Rectangle(0, 1, picBoxPreview.Width, 0x11), format2);
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.DrawString("-8:88", timeFont, new SolidBrush(picBoxSegAheadGain.BackColor), rectangle5, format3);
            int y = 0x12;
            Rectangle rectangle6 = new Rectangle(0, y, picBoxPreview.Width, ((picBoxPreview.Height - rect.Height) - 0x13) - y);
            if (checkBoxPlainBg.Checked)
            {
                graphics.FillRectangle(new SolidBrush(picBoxSegBackgroundPlain.BackColor), rectangle6);
                graphics.FillRectangle(new SolidBrush(picBoxStatusBarBackPlain.BackColor), 0, rect.Y - 3, picBoxPreview.Width, 3);
            }
            else
            {
                graphics.FillRectangle(new LinearGradientBrush(rectangle6, picBoxSegBackground.BackColor, picBoxSegBackground2.BackColor, 0f), rectangle6);
                graphics.FillRectangle(new LinearGradientBrush(rectangle6, picBoxStatusBarBack.BackColor, picBoxStatusBarBack2.BackColor, 0f), 0, rect.Y - 3, picBoxPreview.Width, 3);
            }
            StringFormat format5 = new StringFormat
            {
                Trimming = StringTrimming.EllipsisCharacter,
                LineAlignment = StringAlignment.Center
            };
            StringFormat format6 = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Far
            };
            Rectangle rectangle7 = new Rectangle(0, 0, 0, 1);
            string[] strArray = new string[] { "Past Segment", "Best Segment", "Ahead, gained", "Ahead, lost", "Behind, gained", "Behind, lost", "New time", "Live segment", "Missing time", "Future segment", "Future segment", "Future segment" };
            string[] strArray2 = new string[] { "17:17", "-88.8", "-88.8", "-88.8", "+88.8", "+88.8", "8:88", "-", "8:88.8", "8:88", "8:88", "8:88" };
            Color[] colorArray = new Color[] { picBoxSegPastText.BackColor, picBoxSegPastText.BackColor, picBoxSegPastText.BackColor, picBoxSegPastText.BackColor, picBoxSegPastText.BackColor, picBoxSegPastText.BackColor, picBoxSegPastText.BackColor, picBoxSegLiveText.BackColor, picBoxSegPastText.BackColor, picBoxSegFutureText.BackColor, picBoxSegFutureText.BackColor, picBoxSegFutureText.BackColor };
            Color[] colorArray2 = new Color[] { picturebox1.BackColor, picBoxSegBestSegment.BackColor, picBoxSegAheadGain.BackColor, picBoxSegAheadLoss.BackColor, picBoxSegBehindGain.BackColor, picBoxSegBehindLoss.BackColor, picBoxSegNewTime.BackColor, picBoxSegLiveText.BackColor, picBoxSegMissing.BackColor, picBoxSegFutureTime.BackColor, picBoxSegFutureTime.BackColor, picBoxSegFutureTime.BackColor };
            for (int i = 0; i < 12; i++)
            {
                Rectangle rectangle8 = new Rectangle(0, y, picBoxPreview.Width, segHeight);
                Rectangle rectangle9 = new Rectangle(0, y + 2, picBoxPreview.Width, segHeight);
                Rectangle rectangle10 = new Rectangle(0, y + 1, picBoxPreview.Width, segHeight);
                rectangle9.Y = (y + (segHeight / 2)) - 5;
                rectangle9.Height = 13;
                if (i == 7)
                {
                    rectangle7 = rectangle8;
                    if (checkBoxPlainBg.Checked)
                    {
                        graphics.FillRectangle(new SolidBrush(picBoxSegHighlightPlain.BackColor), rectangle8);
                    }
                    else
                    {
                        graphics.FillRectangle(new LinearGradientBrush(rectangle8, picBoxSegHighlight.BackColor, picBoxSegHighlight2.BackColor, 0f), rectangle8);
                    }
                }
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                graphics.DrawString(strArray[i], displayFont, new SolidBrush(colorArray[i]), rectangle9, format5);
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                graphics.DrawString(strArray2[i], timeFont, new SolidBrush(colorArray2[i]), rectangle10, format6);
                y += segHeight;
            }
            Pen pen = new Pen(new SolidBrush(picBoxSegHighlightBorder.BackColor));
            graphics.DrawLine(pen, rectangle7.Left, rectangle7.Top, rectangle7.Right, rectangle7.Top);
            graphics.DrawLine(pen, rectangle7.Left, rectangle7.Bottom, rectangle7.Right, rectangle7.Bottom);
        }

        public void SetPictureBoxColor(object sender, EventArgs e)
        {
            ColorPicker picker = new ColorPicker();
            picker.SetColor(((PictureBox)sender).BackColor);
            if (picker.ShowDialog() == DialogResult.OK)
            {
                ((PictureBox)sender).BackColor = picker.rgbColor;
                picBoxPreview.Invalidate();
            }
        }

        private enum ClockType
        {
            Ahead,
            AheadLosing,
            Behind,
            BehindLosing,
            Delay,
            NoRun,
            Paused,
            NewRecord,
            Finished,
            Flash
        }

        private class SettingPair
        {
            public string name;
            public PictureBox pb;

            public SettingPair(string Name, PictureBox Box)
            {
                name = Name;
                pb = Box;
            }
        }
    }
}