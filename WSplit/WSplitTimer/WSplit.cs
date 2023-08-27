using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WSplitTimer.Properties;

namespace WSplitTimer
{
    public class WSplit : Form
    {
        private ContextMenuStrip timerMenu;

        private ToolStripMenuItem newButton;
        private ToolStripMenuItem openButton;
        private ToolStripMenuItem openRecent;
        private ToolStripMenuItem saveButton;
        private ToolStripMenuItem saveAsButton;
        private ToolStripMenuItem reloadButton;
        private ToolStripMenuItem closeButton;

        private ToolStripSeparator toolStripSeparator1;

        private ToolStripMenuItem menuItemStartAt;
        private ToolStripMenuItem resetButton;
        private ToolStripMenuItem stopButton;
        private ToolStripMenuItem newOldButton;

        private ToolStripSeparator toolStripSeparator2;

        private ToolStripMenuItem menuItemSettings;

        private ToolStripMenuItem displaySettingsMenu;
        private ToolStripMenuItem alwaysOnTop;
        private ToolStripMenuItem showRunTitleButton;
        private ToolStripMenuItem showAttemptCount;
        private ToolStripMenuItem showRunGoalMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem displayTimerOnlyButton;
        private ToolStripMenuItem displayCompactButton;
        private ToolStripMenuItem displayDetailedButton;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem displayWideButton;
        private ToolStripMenuItem clockAppearanceToolStripMenuItem;
        private ToolStripMenuItem showDecimalSeparator;
        private ToolStripMenuItem digitalClockButton;
        private ToolStripMenuItem clockAccent;
        private ToolStripMenuItem plainBg;
        private ToolStripMenuItem blackBg;
        private ToolStripMenuItem customBg;
        private ToolStripMenuItem menuItemAdvancedDisplay;
        private ToolStripMenuItem setColorsButton;
        private ToolStripSeparator toolStripSeparator5;
        public ToolStripMenuItem advancedDetailButton;

        private ToolStripMenuItem compareMenu;
        private ToolStripMenuItem compareOldButton;
        private ToolStripMenuItem compareBestButton;
        private ToolStripMenuItem compareFastestButton;
        private ToolStripMenuItem compareSumBestButton;

        private ToolStripMenuItem trackBestMenu;
        private ToolStripMenuItem bestAsOverallButton;
        private ToolStripMenuItem bestAsSplitsButton;

        private ToolStripMenuItem layoutMenu;
        private ToolStripMenuItem prevsegButton;
        private ToolStripMenuItem timesaveButton;
        private ToolStripMenuItem sobButton;
        private ToolStripMenuItem predpbButton;
        private ToolStripMenuItem predbestButton;

        private ToolStripMenuItem gradientMenu;
        private ToolStripMenuItem horiButton;
        private ToolStripMenuItem vertButton;

        private ToolStripSeparator toolStripSeparator6;

        private ToolStripMenuItem aboutButton;
        private ToolStripMenuItem exitButton;

        public Font digitLarge;
        public Font digitMed;
        public Font clockLarge;
        public Font clockMed;
        public Font displayFont;
        public Font timeFont;
        private readonly PrivateFontCollection privateFontCollection = new PrivateFontCollection();

        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openFileDialog;
        private SettingsDialog settingsDialog;

        //private int attemptCount;
        private Size clockMinimumSize = new Size(120, 25);

        private Size clockMinimumSizeAbsolute = new Size(120, 25);
        private Rectangle clockRect;
        private bool clockResize;
        private IContainer components;
        private DisplayMode currentDispMode = DisplayMode.Null;
        private static readonly string numberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        private readonly string decimalChar = numberDecimalSeparator;
        private Size detailPreferredSize = new Size(120, 0x19);
        private bool detailResizing;
        private int detailResizingY;
        private Timer doubleTapDelay;
        private readonly DetailedView dview;
        private Timer flashDelay;

        //private int offsetStart;
        private DateTime offsetStartTime = new DateTime();

        //private string runFile;
        //private string runGoal;
        //private string runTitle = "";
        private int segHeight = 14;

        public RunSplits split = new RunSplits();
        private Timer startDelay;
        private Timer stopwatch;
        public DualStopwatch timer = new DualStopwatch(false);
        private bool wideResizing;
        private int wideResizingX;
        private int wideSegResizeWidth;
        private bool wideSegResizing;
        private int wideSegWidth = 100;
        private int wideSegWidthBase = 100;
        private int wideSegX;

        //public bool unsavedSplits;
        public bool modalWindowOpened;

        // Apparently unused variables...
        public const int HOTKEY_ID = 0x9d82;

        public KeyModifiers hotkeyMod;

        // The painter object is a sub object that has for only purpose to separate
        // the drawing code from the logic code, including the variables used for drawing.
        // The fact that all the drawing code is in a different object makes it possible to
        // have a modular drawing code without messing up this object more than it already was.
        //
        // NOTE: Currently, the painter object need access to the WSplit object it paints into
        // to be able to paint it correctly. Eventually, it would be a good thing to remove that
        // dependancy and have the WSplit object parameter the Painter object rather than have the
        // Painter object take values from the WSplit members.
        private readonly Painter painter;

        //test
        private int r = 255;

        private int g = 127;
        private int b = 255;
        private readonly bool rg = false;
        private bool gg = false;
        private readonly bool gb = false;
        private bool bb = false;
        private readonly bool br = false;
        private bool rr = true;

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(byte[] pbFont, int cbFont, IntPtr pdv, out uint pcFonts);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // If the Settings Dialog has not been created yet, this property will take care of creating it:
        // It has for a purpose to try and speed up the startup by not loading the window yet,
        // but to also speed up the opening of the settings window after the first time.
        // It is not known yet if it fulfills its purpose or if it is completely useless.
        private SettingsDialog SettingsDialog
        {
            get
            {
                if (settingsDialog == null)
                    settingsDialog = new SettingsDialog();
                return settingsDialog;
            }
        }

        public WSplit()
        {
            InitializeComponent();
            base.Paint += new PaintEventHandler(clockPaint);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            base.ResizeRedraw = true;

            // Eventually, the painter should not be dependant of the WSplit object...
            painter = new Painter(this);

            dview = new DetailedView(split, this);
            Initialize();
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            string str = Assembly.GetExecutingAssembly().GetName().Name + " v" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            DateTime buildDateTime = GetBuildDateTime(Assembly.GetExecutingAssembly());
            double driftMilliseconds = timer.driftMilliseconds;
            string str2 = "Current fallback timer: " + string.Format("{0:+0.000;-0.000}", driftMilliseconds / 1000.0) + "s";
            string str3 = "";
            if (timer.useFallback)
                str3 = " [Using Fallback]";

            MessageBoxEx.Show(this, str + Environment.NewLine +
                            "by Wodanaz@SDA until 1.4.4" + Environment.NewLine +
                            "by Nitrofski (twitch.tv/Nitrofski) until 1.5.3" + Environment.NewLine +
                            "currently maintained by pimittens (twitch.tv/pimittens) and shookie(twitch.tv/shookieTea)" + Environment.NewLine +
                            Environment.NewLine +
                            "Github repository location: https://github.com/Nitrofski/WSplit" + Environment.NewLine +
                            "Compiled: " + buildDateTime.ToString() +
                            Environment.NewLine +
                            Environment.NewLine +
                            str2 + str3 + Environment.NewLine +
                            "(difference between fallback and standard timing methods)",
                            "About", MessageBoxButtons.OK);
        }

        private void advancedDetailButton_Click(object sender, EventArgs e)
        {
            if (advancedDetailButton.Checked)
            {
                if (split.LiveRun)
                    dview.Show();
            }
            else

                dview.Hide();
        }

        private void alwaysOnTop_Click(object sender, EventArgs e)
        {
            Settings.Profile.OnTop = !Settings.Profile.OnTop;
            alwaysOnTop.Checked = Settings.Profile.OnTop;
            base.TopMost = Settings.Profile.OnTop;
        }

        private void bestAsOverallButton_Click(object sender, EventArgs e)
        {
            bestAsOverallButton.Checked = true;
            bestAsSplitsButton.Checked = false;
            Settings.Profile.BestAsOverall = true;
        }

        private void bestAsSplitsButton_Click(object sender, EventArgs e)
        {
            bestAsOverallButton.Checked = false;
            bestAsSplitsButton.Checked = true;
            Settings.Profile.BestAsOverall = false;
        }

        private void blackBg_Click(object sender, EventArgs e)
        {
            plainBg.Checked = false;
            Settings.Profile.BackgroundPlain = false;
            Settings.Profile.BackgroundBlack = !Settings.Profile.BackgroundBlack;
            blackBg.Checked = Settings.Profile.BackgroundBlack;
            painter.RequestBackgroundRedraw();
            base.Invalidate();
        }

        private void customBg_Click(object sender, EventArgs e)
        {
            if (settingsDialog == null)
            {
                settingsDialog = new SettingsDialog();
            }
            settingsDialog.OpenCustomBackgroundMenuItem();
        }

        private void clear_Click(object sender, EventArgs e)
        {
            Settings.Profile.RecentFiles.Clear();
            populateRecentFiles();
        }

        private void clearHotkeys()
        {
            for (int i = 0; i < 7; i++)
                UnregisterHotKey(base.Handle, 0x9d82 + i);
        }

        private void clockAccent_Click(object sender, EventArgs e)
        {
            Settings.Profile.ClockAccent = !Settings.Profile.ClockAccent;
            clockAccent.Checked = Settings.Profile.ClockAccent;
            painter.RequestBackgroundRedraw();
            base.Invalidate();
        }

        private void clockPaint(object sender, PaintEventArgs e)
        {
            // The painter object will take care of drawing the clock
            painter.PaintAll(e.Graphics);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            if (promptForSave())
            {
                closeFile();
            }
        }

        private void closeFile()
        {
            split.Clear();
            detailPreferredSize = clockMinimumSize;
            InitializeDisplay();
        }

        private void compareOldButton_Click(object sender, EventArgs e)
        {
            SetCompareOld();
        }

        private void compareBestButton_Click(object sender, EventArgs e)
        {
            SetCompareBest();
        }

        private void compareFastestButton_Click(object sender, EventArgs e)
        {
            SetCompareFastest();
        }

        private void compareSumBestButton_Click(object sender, EventArgs e)
        {
            SetCompareSoB();
        }

        private void SetCompareOld()
        {
            Settings.Profile.CompareAgainst = 1;
            compareOldButton.Checked = true;
            compareBestButton.Checked = false;
            compareFastestButton.Checked = false;
            compareSumBestButton.Checked = false;
            updateDisplay();
        }

        private void SetCompareBest()
        {
            Settings.Profile.CompareAgainst = 2;
            compareOldButton.Checked = false;
            compareBestButton.Checked = true;
            compareFastestButton.Checked = false;
            compareSumBestButton.Checked = false;
            updateDisplay();
        }

        private void SetCompareFastest()
        {
            Settings.Profile.CompareAgainst = 0;
            compareOldButton.Checked = false;
            compareBestButton.Checked = false;
            compareFastestButton.Checked = true;
            compareSumBestButton.Checked = false;
            updateDisplay();
        }

        private void SetCompareSoB()
        {
            Settings.Profile.CompareAgainst = 3;
            compareOldButton.Checked = false;
            compareBestButton.Checked = false;
            compareFastestButton.Checked = false;
            compareSumBestButton.Checked = true;
            updateDisplay();
        }

        private void SwitchComparisonType()
        {
            switch (Settings.Profile.CompareAgainst)
            {
                case 1: // Old
                    SetCompareBest();
                    break;

                case 3: // Sum of Bests
                    SetCompareOld();
                    break;

                default: // Fastest & Best
                    SetCompareSoB();
                    break;
            }
        }

        private void configure(int startingPage)
        {
            // Prepares the Main and DView Windows:
            clearHotkeys();

            base.TopMost = false;
            dview.TopMost = false;
            modalWindowOpened = true;

            // A few settings are necessary before calling the custom ShowDialog method
            SettingsDialog.StartDelay = timeFormatter(split.StartDelay / 1000.0, TimeFormat.Seconds);
            SettingsDialog.DetailedWidth = clockRect.Width;

            // Costum ShowDialog method...
            if (SettingsDialog.ShowDialog(this, startingPage) == DialogResult.OK)
            {
                SettingsDialog.ApplyChanges();

                split.StartDelay = Convert.ToInt32((double)(timeParse(SettingsDialog.StartDelay) * 1000.0));
                clockRect.Width = SettingsDialog.DetailedWidth;
                updateDetailed();
                InitializeSettings();
                InitializeFonts();
            }

            if (SettingsDialog.BackgroundSettingsChanged)
                InitializeBackground();

            // Some changes need to be applied live:
            base.Opacity = Settings.Profile.Opacity;
            base.TopMost = Settings.Profile.OnTop;
            dview.TopMost = Settings.Profile.DViewOnTop;

            modalWindowOpened = false;
            setHotkeys();
        }

        private void menuItemSettings_Click(object sender, EventArgs e)
        {
            configure(0);
        }

        private Color DarkenColor(Color original, double lightness)
        {
            lightness = Math.Max(Math.Min(lightness, 255.0), 0.0);
            return Color.FromArgb((int)(original.R * lightness), (int)(original.G * lightness), (int)(original.B * lightness));
        }

        private int detailSegCount()
        {
            int displaySegs = Settings.Profile.DisplaySegs;
            if (!Settings.Profile.DisplayBlankSegs)
            {
                displaySegs = Math.Min(split.Count, displaySegs);
            }
            return displaySegs;
        }

        private void digitalClockButton_Click(object sender, EventArgs e)
        {
            Settings.Profile.DigitalClock = !Settings.Profile.DigitalClock;
            digitalClockButton.Checked = Settings.Profile.DigitalClock;
            painter.RequestBackgroundRedraw();
            base.Invalidate();
        }

        private void displayCompact()
        {
            clockRect.Location = new Point(0, 15);
            clockMinimumSize = clockMinimumSizeAbsolute;
            if (Settings.Profile.SegmentIcons > 1)
            {
                clockMinimumSize.Width = (clockMinimumSizeAbsolute.Width + ((Settings.Profile.SegmentIcons + 1) * 8)) + 6;
            }
            if (currentDispMode != DisplayMode.Compact)
            {
                clockRect.Size = Settings.Profile.ClockSize;
                clockRect.Width += clockMinimumSize.Width - clockMinimumSizeAbsolute.Width;
            }
            if ((clockRect.Height < clockMinimumSize.Height) || (clockRect.Width < clockMinimumSize.Width))
            {
                clockRect.Size = clockMinimumSize;
            }
            currentDispMode = DisplayMode.Compact;
            base.Size = new Size(clockRect.Width, clockRect.Height + 0x1f);
        }

        private void displayCompactButton_Click(object sender, EventArgs e)
        {
            setDisplay(DisplayMode.Compact);
        }

        private void displayDetail()
        {
            int hh = 0;
            segHeight = Math.Max(14, (Settings.Profile.SegmentIcons + 1) * 8);
            clockMinimumSize = clockMinimumSizeAbsolute;
            if (currentDispMode != DisplayMode.Detailed)
            {
                clockRect.Size = detailPreferredSize;
            }
            if ((clockRect.Height < clockMinimumSize.Height) || (clockRect.Width < clockMinimumSize.Width))
            {
                clockRect.Size = clockMinimumSize;
            }
            int height = (clockRect.Height + (detailSegCount() * segHeight)) + 0x15;
            if (((split.RunTitle != "") && Settings.Profile.ShowTitle) && ((split.RunGoal != "") && Settings.Profile.ShowGoal))
            {
                height += 0x20;
            }
            else if ((split.RunTitle != "") && Settings.Profile.ShowTitle)
            {
                height += 0x12;
            }
            else if ((split.RunGoal != "") && Settings.Profile.ShowGoal)
            {
                height += 0x12;
            }
            /* component stuff here */
            if (Settings.Profile.ShowPrevSeg) { hh += 18; };
            if (Settings.Profile.ShowTimeSave) { hh += 18; };
            if (Settings.Profile.ShowSoB) { hh += 18; };
            if (Settings.Profile.PredPB) { hh += 18; };
            if (Settings.Profile.PredBest) { hh += 18; };
            height += hh - 18;
            clockRect.Location = new Point(0, (height - clockRect.Height) - hh);
            currentDispMode = DisplayMode.Detailed;
            base.Size = new Size(clockRect.Width, height);
        }

        private void displayDetailedButton_Click(object sender, EventArgs e)
        {
            setDisplay(DisplayMode.Detailed);
        }

        private int displaySegsWide()
        {
            int wideSegs = Settings.Profile.WideSegs;
            if (!Settings.Profile.WideSegBlanks)
            {
                wideSegs = Math.Min(split.Count, wideSegs);
            }
            return wideSegs;
        }

        private void displayTimer()
        {
            clockRect.Location = new Point(0, 0);
            clockMinimumSize = clockMinimumSizeAbsolute;
            if (currentDispMode != DisplayMode.Timer)
            {
                clockRect.Size = Settings.Profile.ClockSize;
            }
            if ((clockRect.Height < clockMinimumSize.Height) || (clockRect.Width < clockMinimumSize.Width))
            {
                clockRect.Size = clockMinimumSize;
            }
            currentDispMode = DisplayMode.Timer;
            base.Size = clockRect.Size;
        }

        private void displayTimerOnlyButton_Click(object sender, EventArgs e)
        {
            setDisplay(DisplayMode.Timer);
        }

        private void displayWide()
        {
            if (Settings.Profile.SegmentIcons >= 1)
            {
                wideSegWidth = wideSegWidthBase + ((Settings.Profile.SegmentIcons + 1) * 8);
            }
            else
            {
                wideSegWidth = wideSegWidthBase;
            }
            clockMinimumSize.Width = clockMinimumSizeAbsolute.Width;
            if (Settings.Profile.SegmentIcons == 3)
            {
                clockMinimumSize.Height = 0x20;
            }
            else
            {
                clockMinimumSize.Height = Settings.Profile.WideHeight; // make this changeable //
            }
            clockRect.Location = new Point(0, 0);
            if (((currentDispMode != DisplayMode.Wide) || (clockRect.Height != clockMinimumSize.Height)) || (clockRect.Width < clockMinimumSize.Width))
            {
                clockRect.Size = clockMinimumSize;
            }
            currentDispMode = DisplayMode.Wide;
            base.Size = new Size((clockRect.Width + 124) + (displaySegsWide() * wideSegWidth), clockRect.Height);
        }

        private void displayWideButton_Click(object sender, EventArgs e)
        {
            setDisplay(DisplayMode.Wide);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void doSplit()
        {
            double time = Math.Truncate(timer.Elapsed.TotalSeconds * 100) / 100;
            split.DoSplit(time);
            if (!split.Done)
            {
                flashClock();
            }
            else
            {
                stopwatch.Enabled = false;
                newOldButton.Enabled = true;
            }
            updateDisplay();
        }

        private void doubleTapDelay_Tick(object sender, EventArgs e)
        {
            doubleTapDelay.Dispose();
            doubleTapDelay = null;
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            if (promptForSave())
            {
                base.Close();
            }
        }

        private void flashClock()
        {
            /* if (this.flashDelay == null)
            {
                this.flashDelay = new Timer();
                this.flashDelay.Tick += new EventHandler(this.unflashClock);
                this.flashDelay.Interval = 750;
                this.flashDelay.Enabled = true;
                base.Invalidate();
            } */
        }

        private static DateTime GetBuildDateTime(Assembly assembly)
        {
            if (File.Exists(assembly.Location))
            {
                byte[] buffer = new byte[Math.Max(Marshal.SizeOf(typeof(_IMAGE_FILE_HEADER)), 4)];
                using (FileStream stream = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read))
                {
                    stream.Position = 60L;
                    stream.Read(buffer, 0, 4);
                    stream.Position = BitConverter.ToUInt32(buffer, 0);
                    stream.Read(buffer, 0, 4);
                    stream.Read(buffer, 0, buffer.Length);
                }
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    _IMAGE_FILE_HEADER _image_file_header = (_IMAGE_FILE_HEADER)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(_IMAGE_FILE_HEADER));
                    DateTime time2 = new DateTime(0x7b2, 1, 1);
                    return TimeZone.CurrentTimeZone.ToLocalTime(time2.AddSeconds(_image_file_header.TimeDateStamp));
                }
                finally
                {
                    handle.Free();
                }
            }
            return new DateTime();
        }

        private Color getDViewDeltaColor(double newDelta, double oldDelta)
        {
            if (newDelta > 0.0)
            {
                if (newDelta > oldDelta)
                {
                    return ColorSettings.Profile.UsedDViewSegBehindLoss;
                }
                return ColorSettings.Profile.UsedDViewSegBehindGain;
            }
            if (newDelta > oldDelta)
            {
                return ColorSettings.Profile.UsedDViewSegAheadLoss;
            }
            return ColorSettings.Profile.UsedDViewSegAheadGain;
        }

        private void InitializeComponent()
        {
            components = new Container();
            timerMenu = new ContextMenuStrip(components);
            newButton = new ToolStripMenuItem();
            openButton = new ToolStripMenuItem();
            openRecent = new ToolStripMenuItem();
            saveButton = new ToolStripMenuItem();
            saveAsButton = new ToolStripMenuItem();
            reloadButton = new ToolStripMenuItem();
            closeButton = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            resetButton = new ToolStripMenuItem();
            stopButton = new ToolStripMenuItem();
            newOldButton = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            menuItemSettings = new ToolStripMenuItem();
            displaySettingsMenu = new ToolStripMenuItem();
            alwaysOnTop = new ToolStripMenuItem();
            showRunTitleButton = new ToolStripMenuItem();
            showAttemptCount = new ToolStripMenuItem();
            showRunGoalMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            displayTimerOnlyButton = new ToolStripMenuItem();
            displayCompactButton = new ToolStripMenuItem();
            displayWideButton = new ToolStripMenuItem();
            displayDetailedButton = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            clockAppearanceToolStripMenuItem = new ToolStripMenuItem();
            showDecimalSeparator = new ToolStripMenuItem();
            digitalClockButton = new ToolStripMenuItem();
            clockAccent = new ToolStripMenuItem();
            plainBg = new ToolStripMenuItem();
            blackBg = new ToolStripMenuItem();
            customBg = new ToolStripMenuItem();
            menuItemAdvancedDisplay = new ToolStripMenuItem();
            setColorsButton = new ToolStripMenuItem();
            toolStripSeparator6 = new ToolStripSeparator();
            advancedDetailButton = new ToolStripMenuItem();
            compareMenu = new ToolStripMenuItem();
            compareOldButton = new ToolStripMenuItem();
            compareBestButton = new ToolStripMenuItem();
            compareSumBestButton = new ToolStripMenuItem();
            compareFastestButton = new ToolStripMenuItem();
            trackBestMenu = new ToolStripMenuItem();
            bestAsOverallButton = new ToolStripMenuItem();
            bestAsSplitsButton = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            aboutButton = new ToolStripMenuItem();
            exitButton = new ToolStripMenuItem();
            stopwatch = new Timer(components);
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            menuItemStartAt = new ToolStripMenuItem();
            layoutMenu = new ToolStripMenuItem();
            prevsegButton = new ToolStripMenuItem();
            timesaveButton = new ToolStripMenuItem();
            sobButton = new ToolStripMenuItem();
            predpbButton = new ToolStripMenuItem();
            predbestButton = new ToolStripMenuItem();
            gradientMenu = new ToolStripMenuItem();
            horiButton = new ToolStripMenuItem();
            vertButton = new ToolStripMenuItem();

            timerMenu.SuspendLayout();
            base.SuspendLayout();
            timerMenu.Items.AddRange(new ToolStripItem[] {
                newButton, openButton, openRecent, saveButton, saveAsButton, reloadButton, closeButton, toolStripSeparator2, menuItemStartAt, resetButton, stopButton, newOldButton, toolStripSeparator1, menuItemSettings, displaySettingsMenu, compareMenu, trackBestMenu, layoutMenu, gradientMenu,
                toolStripSeparator4, aboutButton, exitButton
             });
            timerMenu.Name = "timerMenu";
            timerMenu.Size = new Size(0xae, 0x18c);
            newButton.Name = "newButton";
            newButton.Size = new Size(0xad, 0x16);
            newButton.Text = "New/Edit";
            newButton.Click += new EventHandler(NewButton_Click);
            openButton.Name = "openButton";
            openButton.Size = new Size(0xad, 0x16);
            openButton.Text = "Open...";
            openButton.Click += new EventHandler(openButton_Click);
            openRecent.Name = "openRecent";
            openRecent.Size = new Size(0xad, 0x16);
            openRecent.Text = "Open recent...";
            saveButton.Enabled = false;
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(0xad, 0x16);
            saveButton.Text = "Save";
            saveButton.Click += new EventHandler(saveButton_Click);
            saveAsButton.Enabled = false;
            saveAsButton.Name = "saveAsButton";
            saveAsButton.Size = new Size(0xad, 0x16);
            saveAsButton.Text = "Save as...";
            saveAsButton.Click += new EventHandler(saveAsButton_Click);
            reloadButton.Enabled = false;
            reloadButton.Name = "reloadButton";
            reloadButton.Size = new Size(0xad, 0x16);
            reloadButton.Text = "Reload";
            reloadButton.Click += new EventHandler(reloadButton_Click);
            closeButton.Enabled = false;
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(0xad, 0x16);
            closeButton.Text = "Close";
            closeButton.Click += new EventHandler(closeButton_Click);
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(170, 6);
            menuItemStartAt.Name = "menuItemStartAt";
            menuItemStartAt.Size = new Size(0xad, 0x16);
            menuItemStartAt.Text = "Start at...";
            menuItemStartAt.Click += new EventHandler(menuItemStartAt_Click);
            resetButton.Name = "resetButton";
            resetButton.Size = new Size(0xad, 0x16);
            resetButton.Text = "Reset";
            resetButton.Click += new EventHandler(resetButton_Click);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(0xad, 0x16);
            stopButton.Text = "Stop";
            stopButton.Click += new EventHandler(stopButton_Click);
            newOldButton.Enabled = false;
            newOldButton.Name = "newOldButton";
            newOldButton.Size = new Size(0xad, 0x16);
            newOldButton.Text = "Set this run as old";
            newOldButton.Click += new EventHandler(NewOldButton_Click);
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(170, 6);
            menuItemSettings.Name = "menuItemSettings";
            menuItemSettings.Size = new Size(0xad, 0x16);
            menuItemSettings.Text = "Settings...";
            menuItemSettings.Click += new EventHandler(menuItemSettings_Click);
            displaySettingsMenu.DropDownItems.AddRange(new ToolStripItem[] {
                alwaysOnTop, showRunTitleButton, showAttemptCount, showRunGoalMenuItem, toolStripSeparator3, displayTimerOnlyButton, displayCompactButton, displayWideButton, displayDetailedButton, toolStripSeparator5, clockAppearanceToolStripMenuItem, plainBg, blackBg, customBg, menuItemAdvancedDisplay, setColorsButton, toolStripSeparator6,
                advancedDetailButton
             });
            displaySettingsMenu.Name = "displaySettingsMenu";
            displaySettingsMenu.Size = new Size(0xad, 0x16);
            displaySettingsMenu.Text = "Display settings";
            alwaysOnTop.Name = "alwaysOnTop";
            alwaysOnTop.Size = new Size(0xcc, 0x16);
            alwaysOnTop.Text = "Always on top";
            alwaysOnTop.Click += new EventHandler(alwaysOnTop_Click);
            showRunTitleButton.Name = "showRunTitleButton";
            showRunTitleButton.Size = new Size(0xcc, 0x16);
            showRunTitleButton.Text = "Show run title";
            showRunTitleButton.Click += new EventHandler(showRunTitleButton_Click);
            showAttemptCount.Name = "showAttemptCount";
            showAttemptCount.Size = new Size(0xcc, 0x16);
            showAttemptCount.Text = "Show attempt count";
            showAttemptCount.Click += new EventHandler(showAttemptCount_Click);
            showRunGoalMenuItem.Name = "showRunGoal";
            showRunGoalMenuItem.Size = new Size(0xcc, 0x16);
            showRunGoalMenuItem.Text = "Show run goal";
            showRunGoalMenuItem.Click += new EventHandler(showRunGoal_Click);
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(0xc9, 6);
            displayTimerOnlyButton.Name = "displayTimerOnlyButton";
            displayTimerOnlyButton.Size = new Size(0xcc, 0x16);
            displayTimerOnlyButton.Text = "Timer only";
            displayTimerOnlyButton.Click += new EventHandler(displayTimerOnlyButton_Click);
            displayCompactButton.Name = "displayCompactButton";
            displayCompactButton.Size = new Size(0xcc, 0x16);
            displayCompactButton.Text = "Compact";
            displayCompactButton.Click += new EventHandler(displayCompactButton_Click);
            displayWideButton.Name = "displayWideButton";
            displayWideButton.Size = new Size(0xcc, 0x16);
            displayWideButton.Text = "Wide";
            displayWideButton.Click += new EventHandler(displayWideButton_Click);
            displayDetailedButton.Name = "displayDetailedButton";
            displayDetailedButton.Size = new Size(0xcc, 0x16);
            displayDetailedButton.Text = "Detailed";
            displayDetailedButton.Click += new EventHandler(displayDetailedButton_Click);
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(0xc9, 6);
            clockAppearanceToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { showDecimalSeparator, digitalClockButton, clockAccent });
            clockAppearanceToolStripMenuItem.Name = "clockAppearanceToolStripMenuItem";
            clockAppearanceToolStripMenuItem.Size = new Size(0xcc, 0x16);
            clockAppearanceToolStripMenuItem.Text = "Clock appearance";
            showDecimalSeparator.Name = "showDecimalSeparator";
            showDecimalSeparator.Size = new Size(0xd0, 0x16);
            showDecimalSeparator.Text = "Show decimal separator";
            showDecimalSeparator.Click += new EventHandler(showDecimalSeparator_Click);
            digitalClockButton.Name = "digitalClockButton";
            digitalClockButton.Size = new Size(0xd0, 0x16);
            digitalClockButton.Text = "Digital clock font";
            digitalClockButton.Click += new EventHandler(digitalClockButton_Click);
            clockAccent.Name = "clockAccent";
            clockAccent.Size = new Size(0xd0, 0x16);
            clockAccent.Text = "Accent on vertical modes";
            clockAccent.Click += new EventHandler(clockAccent_Click);
            plainBg.Name = "plainBg";
            plainBg.Size = new Size(0xcc, 0x16);
            plainBg.Text = "Plain background";
            plainBg.Click += new EventHandler(plainBg_Click);
            blackBg.Name = "blackBg";
            blackBg.Size = new Size(0xcc, 0x16);
            blackBg.Text = "Black background";
            blackBg.Click += new EventHandler(blackBg_Click);
            customBg.Name = "customBg";
            customBg.Size = new Size(0xcc, 0x16);
            customBg.Text = "Background Image";
            customBg.Click += new EventHandler(customBg_Click);
            menuItemAdvancedDisplay.Name = "menuItemAdvancedDisplay";
            menuItemAdvancedDisplay.Size = new Size(0xcc, 0x16);
            menuItemAdvancedDisplay.Text = "Advanced...";
            menuItemAdvancedDisplay.Click += new EventHandler(MenuItemAdvancedDisplay_Click);
            setColorsButton.Name = "setColorsButton";
            setColorsButton.Size = new Size(0xcc, 0x16);
            setColorsButton.Text = "Set colors...";
            setColorsButton.Click += new EventHandler(setColorsButton_Click);
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(0xc9, 6);
            advancedDetailButton.CheckOnClick = true;
            advancedDetailButton.Name = "advancedDetailButton";
            advancedDetailButton.Size = new Size(0xcc, 0x16);
            advancedDetailButton.Text = "Advanced detail window";
            advancedDetailButton.Click += new EventHandler(advancedDetailButton_Click);
            compareMenu.DropDownItems.AddRange(new ToolStripItem[] { compareOldButton, compareBestButton, compareFastestButton, compareSumBestButton });
            compareMenu.Name = "compareMenu";
            compareMenu.Size = new Size(0xad, 0x16);
            compareMenu.Text = "Compare against...";
            compareOldButton.Name = "compareOldButton";
            compareOldButton.Size = new Size(0x90, 0x16);
            compareOldButton.Text = "Old run";
            compareOldButton.Click += new EventHandler(compareOldButton_Click);
            compareBestButton.Name = "compareBestButton";
            compareBestButton.Size = new Size(0x90, 0x16);
            compareBestButton.Text = "Personal best";
            compareBestButton.Click += new EventHandler(compareBestButton_Click);
            compareFastestButton.Name = "compareFastestButton";
            compareFastestButton.Size = new Size(0x90, 0x16);
            compareFastestButton.Text = "Fastest";
            compareFastestButton.Click += new EventHandler(compareFastestButton_Click);
            compareSumBestButton.Name = "compareSumBestButton";
            compareSumBestButton.Size = new Size(0x90, 0x16);
            compareSumBestButton.Text = "Sum of best segments";
            compareSumBestButton.Click += new EventHandler(compareSumBestButton_Click);
            trackBestMenu.DropDownItems.AddRange(new ToolStripItem[] { bestAsOverallButton, bestAsSplitsButton });
            trackBestMenu.Name = "trackBestMenu";
            trackBestMenu.Size = new Size(0xad, 0x16);
            trackBestMenu.Text = "Track best as...";
            bestAsOverallButton.Name = "bestAsOverallButton";
            bestAsOverallButton.Size = new Size(0xb1, 0x16);
            bestAsOverallButton.Text = "Fastest overall run";
            bestAsOverallButton.Click += new EventHandler(bestAsOverallButton_Click);
            bestAsSplitsButton.Name = "bestAsSplitsButton";
            bestAsSplitsButton.Size = new Size(0xb1, 0x16);
            bestAsSplitsButton.Text = "Fastest to each split";
            bestAsSplitsButton.Click += new EventHandler(bestAsSplitsButton_Click);
            // layout
            layoutMenu.DropDownItems.AddRange(new ToolStripItem[] { prevsegButton, timesaveButton, sobButton, predpbButton, predbestButton });
            layoutMenu.Name = "layoutMenu";
            layoutMenu.Size = new Size(0xad, 0x16);
            layoutMenu.Text = "Layout (detailed only)...";
            prevsegButton.Name = "prevsegButton";
            prevsegButton.Size = new Size(0xb1, 0x16);
            prevsegButton.Text = "Previous Segment";
            prevsegButton.Click += new EventHandler(prevsegButton_Click);
            //this.prevsegButton.Enabled = false;
            timesaveButton.Name = "timesaveButton";
            timesaveButton.Size = new Size(0xb1, 0x16);
            timesaveButton.Text = "Possible Time Save";
            timesaveButton.Click += new EventHandler(timesaveButton_Click);
            sobButton.Name = "sobButton";
            sobButton.Size = new Size(0xb1, 0x16);
            sobButton.Text = "Sum of Best Segments";
            sobButton.Click += new EventHandler(sobButton_Click);
            predpbButton.Name = "predpbButton";
            predpbButton.Size = new Size(0xb1, 0x16);
            predpbButton.Text = "Predicted Time (PB)";
            predpbButton.Click += new EventHandler(predpbButton_Click);
            predbestButton.Name = "predbestButton";
            predbestButton.Size = new Size(0xb1, 0x16);
            predbestButton.Text = "Predicted Time (Best)";
            predbestButton.Click += new EventHandler(predbestButton_Click);
            // ye
            gradientMenu.DropDownItems.AddRange(new ToolStripItem[] { horiButton, vertButton });
            gradientMenu.Name = "gradientMenu";
            gradientMenu.Size = new Size(0xad, 0x16);
            gradientMenu.Text = "Gradients...";
            horiButton.Name = "horiButton";
            horiButton.Size = new Size(0xb1, 0x16);
            horiButton.Text = "Horizontal";
            horiButton.Click += new EventHandler(horiButton_Click);
            vertButton.Name = "vertButton";
            vertButton.Size = new Size(0xb1, 0x16);
            vertButton.Text = "Vertical";
            vertButton.Click += new EventHandler(vertButton_Click);
            // more
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(170, 6);
            aboutButton.Name = "aboutButton";
            aboutButton.Size = new Size(0xad, 0x16);
            aboutButton.Text = "About";
            aboutButton.Click += new EventHandler(aboutButton_Click);
            exitButton.Name = "exitButton";
            exitButton.Size = new Size(0xad, 0x16);
            exitButton.Text = "Exit";
            exitButton.Click += new EventHandler(exitButton_Click);
            stopwatch.Interval = 15;
            stopwatch.Tick += new EventHandler(stopwatch_Tick);
            AllowDrop = true;
            base.AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Black;
            base.ClientSize = new Size(0x7c, 0x1a);
            ContextMenuStrip = timerMenu;
            base.ControlBox = false;
            ForeColor = Color.White;
            base.FormBorderStyle = FormBorderStyle.None;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            MinimumSize = new Size(0x7c, 0x1a);
            base.Name = "WSplit";
            Text = "WSplit";
            timerMenu.ResumeLayout(false);
            base.ResumeLayout(false);
            base.Icon = Resources.AppIcon;
        }

        private void vertButton_Click(object sender, EventArgs e)
        {
            if (!vertButton.Checked)
            {
                vertButton.Checked = true;
                Settings.Profile.HGrad = false;
                horiButton.Checked = false;
                updateDetailed();
            };
        }

        private void horiButton_Click(object sender, EventArgs e)
        {
            if (!horiButton.Checked)
            {
                horiButton.Checked = true;
                Settings.Profile.HGrad = true;
                vertButton.Checked = false;
                updateDetailed();
            }
        }

        private void Initialize()
        {
            if (Settings.Profile.FirstRun)
            {
                Settings.Profile.Upgrade();
                ColorSettings.Profile.Upgrade();
                Settings.Profile.FirstRun = false;
            }

            InitializeSettings();
            InitializeBackground();
            InitializeFonts();

            clockRect.Location = new Point(0, 0);
            clockRect.Size = base.Size;

            string[] commandLineArgs = Environment.GetCommandLineArgs();
            for (int i = 1; (i < commandLineArgs.Length) && !split.LiveRun; i++)
            {
                split.RunFile = commandLineArgs[i];
                LoadFile();
            }

            if ((split.RunFile == null) && Settings.Profile.LoadMostRecent)
            {
                split.RunFile = Settings.Profile.LastFile;
                LoadFile();
            }

            if (Settings.Profile.SaveWindowPos)
            {
                bool flag = false;
                Rectangle rectangle = new Rectangle(Settings.Profile.WindowPosition, base.Size);
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (rectangle.IntersectsWith(screen.WorkingArea))
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    base.StartPosition = FormStartPosition.Manual;
                    base.Location = Settings.Profile.WindowPosition;
                }
            }

            modalWindowOpened = false;
        }

        private void InitializeSettings()
        {
            // Initialize every item to the correct setting value
            stopwatch.Interval = Settings.Profile.RefreshRate;

            base.TopMost = Settings.Profile.OnTop;
            alwaysOnTop.Checked = Settings.Profile.OnTop;

            showRunTitleButton.Checked = Settings.Profile.ShowTitle;
            digitalClockButton.Checked = Settings.Profile.DigitalClock;
            showAttemptCount.Checked = Settings.Profile.ShowAttempts;
            showRunGoalMenuItem.Checked = Settings.Profile.ShowGoal;

            prevsegButton.Checked = Settings.Profile.ShowPrevSeg;
            timesaveButton.Checked = Settings.Profile.ShowTimeSave;
            sobButton.Checked = Settings.Profile.ShowSoB;
            predpbButton.Checked = Settings.Profile.PredPB;
            predbestButton.Checked = Settings.Profile.PredBest;

            horiButton.Checked = Settings.Profile.HGrad;
            vertButton.Checked = !Settings.Profile.HGrad;

            if (Settings.Profile.BestAsOverall)
                bestAsOverallButton.Checked = true;
            else
                bestAsSplitsButton.Checked = true;

            if (Settings.Profile.CompareAgainst == 0)
                compareFastestButton.Checked = true;
            else if (Settings.Profile.CompareAgainst == 1)
                compareOldButton.Checked = true;
            else if (Settings.Profile.CompareAgainst == 2)
                compareBestButton.Checked = true;
            else
                compareSumBestButton.Checked = true;

            populateRecentFiles();

            timer.useFallback = Settings.Profile.FallbackPreference == 3;
            showDecimalSeparator.Checked = Settings.Profile.ShowDecimalSeparator;
            clockAccent.Checked = Settings.Profile.ClockAccent;
            base.Opacity = Math.Min(Math.Abs(Settings.Profile.Opacity), 1.0);

            setHotkeys();
            setDisplay((DisplayMode)Settings.Profile.DisplayMode);
        }

        private void InitializeBackground()
        {
            plainBg.Checked = Settings.Profile.BackgroundPlain;
            blackBg.Checked = Settings.Profile.BackgroundBlack;
            painter.PrepareBackground();
            Invalidate();
        }

        private void InitializeFonts()
        {
            // Initialize fonts according to settings

            // Loads clockFont from file:
            if (digitLarge == null || digitMed == null)
            {
                uint num;   // Necessary, as AddFontMemResourceEx needs a uint as a out parameter

                byte[] clockFont = Resources.ClockFont;
                IntPtr destination = Marshal.AllocCoTaskMem(clockFont.Length);
                AddFontMemResourceEx(clockFont, clockFont.Length, IntPtr.Zero, out num);
                Marshal.Copy(clockFont, 0, destination, clockFont.Length);
                privateFontCollection.AddMemoryFont(destination, clockFont.Length);
                Marshal.FreeCoTaskMem(destination);

                // Once the digital font is loaded in memory, we instanciate the Font objects:
                digitLarge = new Font(privateFontCollection.Families[0], 24f, GraphicsUnit.Pixel);
                digitMed = new Font(privateFontCollection.Families[0], 17.33333f, GraphicsUnit.Pixel);
            }

            FontFamily family = FontFamily.Families.FirstOrDefault(f => f.Name == Settings.Profile.FontFamilySegments);

            if (family == null || !family.IsStyleAvailable(FontStyle.Bold) || !family.IsStyleAvailable(FontStyle.Regular))
                family = FontFamily.GenericSansSerif;

            displayFont = new Font(family, 10.66667f * Settings.Profile.FontMultiplierSegments, GraphicsUnit.Pixel);
            timeFont = new Font(family, 12f * Settings.Profile.FontMultiplierSegments, FontStyle.Bold, GraphicsUnit.Pixel);
            clockLarge = new Font(family, 22.66667f, FontStyle.Bold, GraphicsUnit.Pixel);
            clockMed = new Font(family, 18.66667f, FontStyle.Bold, GraphicsUnit.Pixel);

            dview.InitializeFonts();
        }

        public void InitializeDisplay()
        {
            if (startDelay != null)
            {
                startDelay.Dispose();
                startDelay = null;
            }
            split.ResetSegments();

            newOldButton.Enabled = false;
            menuItemStartAt.Enabled = true;
            stopButton.Enabled = false;
            resetButton.Enabled = false;

            if (split.LastIndex < 0)
            {
                dview.Hide();
                split.RunFile = null;
                closeButton.Enabled = false;
                saveButton.Enabled = false;
                saveAsButton.Enabled = false;
                reloadButton.Enabled = false;
            }
            else
            {
                if (advancedDetailButton.Checked)
                {
                    dview.Show();
                }
                closeButton.Enabled = true;
                if (split.RunFile != null)
                {
                    saveButton.Enabled = true;
                    reloadButton.Enabled = true;
                }
                else
                {
                    saveButton.Enabled = false;
                    reloadButton.Enabled = false;
                }
                saveAsButton.Enabled = true;
            }
            stopwatch.Enabled = false;
            timer.Reset();
            populateDetailed();
            updateDisplay();
            setDisplay((DisplayMode)Settings.Profile.DisplayMode);
        }

        public static bool IsTextFile(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                byte[] buffer = new byte[0x400];
                char[] chArray = new char[0x400];
                bool flag = true;
                int num = stream.Read(buffer, 0, buffer.Length);
                stream.Seek(0L, SeekOrigin.Begin);
                using (StreamReader reader = new StreamReader(stream))
                {
                    reader.Read(chArray, 0, chArray.Length);
                }
                using (MemoryStream stream2 = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(stream2))
                    {
                        writer.Write(chArray);
                        writer.Flush();
                        byte[] buffer2 = stream2.GetBuffer();
                        for (int i = 0; (i < num) && flag; i++)
                        {
                            flag = buffer[i] == buffer2[i];
                        }
                    }
                }
                return flag;
            }
        }

        private KeyModifiers keyMods(Keys key)
        {
            KeyModifiers none = KeyModifiers.None;
            if ((key & Keys.Alt) == Keys.Alt)
            {
                none |= KeyModifiers.Alt;
            }
            if ((key & Keys.Shift) == Keys.Shift)
            {
                none |= KeyModifiers.Shift;
            }
            if ((key & Keys.Control) == Keys.Control)
            {
                none |= KeyModifiers.Control;
            }
            return none;
        }

        private void LoadFile()
        {
            bool fileCanBeOpened = (File.Exists(split.RunFile) && (new FileInfo(split.RunFile).Length < (10.0 * Math.Pow(1024.0, 2.0)))) && IsTextFile(split.RunFile);

            if (!fileCanBeOpened)
            {
                closeFile();
            }

            split.Clear();
            detailPreferredSize = clockMinimumSize;
            using (StreamReader reader = new StreamReader(split.RunFile))
            {
                string splitFileLine;
                List<string> list = new List<string>();
                try
                {
                    while ((splitFileLine = reader.ReadLine()) != null)
                    {
                        if (splitFileLine.StartsWith("Title="))
                        {
                            split.RunTitle = splitFileLine.Substring(6);
                        }
                        if (splitFileLine.StartsWith("Goal="))
                        {
                            split.RunGoal = splitFileLine.Substring(5);
                        }
                        if (splitFileLine.StartsWith("Attempts="))
                        {
                            int attemptsCount = int.Parse(splitFileLine.Substring(9));
                            split.AttemptsCount = attemptsCount;
                        }
                        if (splitFileLine.StartsWith("Offset="))
                        {
                            int offsetStart = int.Parse(splitFileLine.Substring(7));
                            split.StartDelay = offsetStart;
                        }
                        if (splitFileLine.StartsWith("Width="))
                        {
                            int formWidth = int.Parse(splitFileLine.Substring(6));
                            detailPreferredSize = clockMinimumSize;
                            detailPreferredSize.Width = Math.Max(clockMinimumSize.Width, formWidth);
                        }
                        if (splitFileLine.StartsWith("Size=") && (splitFileLine.Split(new char[] { ',' }).Length == 2))
                        {
                            string[] storedWidthHeight = splitFileLine.Split(',');
                            int width = int.Parse(storedWidthHeight[0].Substring(5));
                            int height = int.Parse(storedWidthHeight[1]);

                            detailPreferredSize = new Size(
                                Math.Max(clockMinimumSize.Width, width),
                                Math.Max(clockMinimumSize.Height, height));
                        }
                        if (splitFileLine.StartsWith("Icons="))
                        {
                            foreach (string icon in Regex.Split(splitFileLine.Substring(6), "\","))
                            {
                                list.Add(icon.Replace("\"", ""));
                            }
                        }
                        if (splitFileLine.Split(new char[] { ',' }).Length == 4)
                        {
                            string[] splitData = splitFileLine.Split(new char[] { ',' });
                            string splitName = splitData[0];
                            double oldTime = double.Parse(splitData[1], NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture(""));
                            double bestTime = double.Parse(splitData[2], NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture(""));
                            double bestSegment = double.Parse(splitData[3], NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture(""));

                            split.Add(new SplitSegment(splitName, oldTime, bestTime, bestSegment));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Problem reading splits file.
                    throw;
                }

                for (int i = 0; i < split.Count; i++)
                {
                    if (i < list.Count)
                    {
                        split.segments[i].IconPath = list[i];
                        try
                        {
                            split.segments[i].Icon = Image.FromFile(list[i]);
                        }
                        catch
                        {
                            split.segments[i].Icon = Resources.MissingIcon;
                        }
                    }
                    else
                    {
                        split.segments[i].Icon = Resources.MissingIcon;
                        split.segments[i].IconPath = "";
                    }
                }
                currentDispMode = DisplayMode.Null;
                InitializeDisplay();
                if (split.RunFile != null)
                {
                    if (Settings.Profile.RecentFiles != null)
                    {
                        if (Settings.Profile.RecentFiles.Contains(split.RunFile))
                        {
                            Settings.Profile.RecentFiles.Remove(split.RunFile);
                        }
                        else if (Settings.Profile.RecentFiles.Count > 9)
                        {
                            Settings.Profile.RecentFiles.RemoveAt(Settings.Profile.RecentFiles.Count - 1);
                        }
                        Settings.Profile.RecentFiles.Insert(0, split.RunFile);
                    }
                    populateRecentFiles();
                }

                split.UnsavedSplit = false;
                return;
            }
        }

        public static int MeasureDisplayStringWidth(string text, Font font)
        {
            if (text.Length < 1)
                return 0;

            Bitmap image = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(image);
            StringFormat stringFormat = new StringFormat();
            RectangleF layoutRect = new RectangleF(0f, 0f, 1000f, 1000f);
            CharacterRange[] ranges = new CharacterRange[] { new CharacterRange(0, text.Length) };
            Region[] regionArray = new Region[1];
            stringFormat.SetMeasurableCharacterRanges(ranges);
            layoutRect = g.MeasureCharacterRanges(text, font, layoutRect, stringFormat)[0].GetBounds(g);
            g.Dispose();
            image.Dispose();
            return (int)(layoutRect.Right + 1f);
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            split.UpdateBest(Settings.Profile.BestAsOverall);
            RunEditorDialog editor = new RunEditorDialog(split)
            {
                titleBox = { Text = split.RunTitle },
                attemptsBox = { Text = split.AttemptsCount.ToString() },
                txtGoal = { Text = split.RunGoal }
            };
            base.TopMost = false;
            dview.TopMost = false;
            modalWindowOpened = true;
            if (editor.ShowDialog() == DialogResult.OK)
            {
                split.Clear();
                foreach (SplitSegment segment in editor.editList)
                {
                    split.Add(segment);
                }
                int attemptsCount = 0;
                split.RunTitle = editor.titleBox.Text;
                split.RunGoal = editor.txtGoal.Text;
                int.TryParse(editor.attemptsBox.Text, out attemptsCount);
                split.AttemptsCount = attemptsCount;
                InitializeDisplay();
                split.UnsavedSplit = true;
                split.StartDelay = editor.startDelay;
            }
            else
            {
                split.RestoreBest();
            }
            base.TopMost = Settings.Profile.OnTop;
            dview.TopMost = Settings.Profile.DViewOnTop;
            modalWindowOpened = false;
        }

        private void NewOldButton_Click(object sender, EventArgs e)
        {
            split.LiveToOld();
        }

        public void NextStage()
        {
            split.Next();
            updateDisplay();
        }

        private void MenuItemAdvancedDisplay_Click(object sender, EventArgs e)
        {
            // Used to be opacity button
            /*ChangeOpacity opacity = new ChangeOpacity(this);
            base.TopMost = false;
            this.dview.TopMost = false;
            this.modalWindowOpened = true;
            if (opacity.ShowDialog() == DialogResult.OK)
            {
                Settings.Profile.Opacity = base.Opacity;
            }
            else
            {
                base.Opacity = Math.Min(Math.Abs(Settings.Profile.Opacity), 1.0);
            }
            base.TopMost = Settings.Profile.OnTop;
            this.dview.TopMost = Settings.Profile.DViewOnTop;
            this.modalWindowOpened = false;*/

            configure(3);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            if (promptForSave())
            {
                modalWindowOpened = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    split.RunFile = openFileDialog.FileName;
                    LoadFile();
                }
                modalWindowOpened = false;
            }
        }

        public void pauseResume()
        {
            // If the refreshing stopwatch is running
            if (stopwatch.Enabled)
            {
                stopwatch.Enabled = false;
                if (startDelay != null)
                {
                    /*this.startDelay.Dispose();
                    this.startDelay = null;

                    this.menuItemStartAt.Enabled = true;
                    this.resetButton.Enabled = false;
                    this.stopButton.Enabled = false;*/

                    InitializeDisplay();
                }
                timer.Stop();
                base.Invalidate();
            }

            // If the split aren't done, so if the timer isn't running
            else if (!split.Done && (startDelay == null))
            {
                // If the timer had not been started yet
                if (timer.ElapsedTicks == 0L)
                {
                    // If it wasn't running
                    if (!timer.IsRunning)
                        InitializeDisplay();

                    startTimer();

                    menuItemStartAt.Enabled = false;
                    resetButton.Enabled = true;
                    stopButton.Enabled = true;
                }

                // If the timer was paused
                else
                    timer.Start();

                stopwatch.Enabled = true;
            }
        }

        private void plainBg_Click(object sender, EventArgs e)
        {
            blackBg.Checked = false;
            Settings.Profile.BackgroundBlack = false;
            Settings.Profile.BackgroundPlain = !Settings.Profile.BackgroundPlain;
            plainBg.Checked = Settings.Profile.BackgroundPlain;
            painter.RequestBackgroundRedraw();
            base.Invalidate();
        }

        private void prevsegButton_Click(object sender, EventArgs e)
        {
            prevsegButton.Checked = !Settings.Profile.ShowPrevSeg;
            Settings.Profile.ShowPrevSeg = !Settings.Profile.ShowPrevSeg;
            if (currentDispMode == DisplayMode.Detailed) { displayDetail(); };
        }

        private void timesaveButton_Click(object sender, EventArgs e)
        {
            timesaveButton.Checked = !Settings.Profile.ShowTimeSave;
            Settings.Profile.ShowTimeSave = !Settings.Profile.ShowTimeSave;
            if (currentDispMode == DisplayMode.Detailed) { displayDetail(); };
        }

        private void sobButton_Click(object sender, EventArgs e)
        {
            sobButton.Checked = !Settings.Profile.ShowSoB;
            Settings.Profile.ShowSoB = !Settings.Profile.ShowSoB;
            if (currentDispMode == DisplayMode.Detailed) { displayDetail(); };
        }

        private void predpbButton_Click(object sender, EventArgs e)
        {
            predpbButton.Checked = !Settings.Profile.PredPB;
            Settings.Profile.PredPB = !Settings.Profile.PredPB;
            if (currentDispMode == DisplayMode.Detailed) { displayDetail(); };
        }

        private void predbestButton_Click(object sender, EventArgs e)
        {
            predbestButton.Checked = !Settings.Profile.PredBest;
            Settings.Profile.PredBest = !Settings.Profile.PredBest;
            if (currentDispMode == DisplayMode.Detailed) { displayDetail(); };
        }

        public void populateDetailed()
        {
            dview.segs.Rows.Clear();
            dview.segs.Rows.Add(new object[] { "Segment", "Old", "Best", "SoB", "Live", "+/-" });
            dview.segs.Rows[0].Frozen = true;
            dview.finalSeg.Rows.Clear();
            dview.finalSeg.Rows.Add();

            foreach (SplitSegment segment in split.segments)
            {
                dview.segs.Rows.Add(new object[] { segment.Name });
                if (dview.finalSeg.RowCount > 1)
                    dview.finalSeg.Rows.RemoveAt(1);

                dview.finalSeg.Rows.Add(new object[] { segment.Name });
            }

            if (dview.segs.RowCount >= 2)
                dview.segs.Rows.RemoveAt(dview.segs.RowCount - 1);

            dview.finalSeg.Rows[0].Height = 0;
            dview.finalSeg.Height = (dview.finalSeg.RowCount - 1) * dview.finalSeg.RowTemplate.Height;
        }

        private void populateRecentFiles()
        {
            openRecent.DropDownItems.Clear();
            if ((Settings.Profile.RecentFiles != null) && (Settings.Profile.RecentFiles.Count > 0))
            {
                foreach (string str in Settings.Profile.RecentFiles)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem
                    {
                        Name = str,
                        Text = Path.GetFileName(str),
                        ToolTipText = str
                    };
                    item.Click += new EventHandler(recent_Click);
                    openRecent.DropDownItems.Add(item);
                }
                openRecent.DropDownItems.Add(new ToolStripSeparator());
                ToolStripMenuItem item2 = new ToolStripMenuItem
                {
                    Text = "Clear History"
                };
                item2.Click += new EventHandler(clear_Click);
                openRecent.DropDownItems.Add(item2);
            }
            else
            {
                ToolStripMenuItem item3 = new ToolStripMenuItem
                {
                    Text = "None",
                    Enabled = false
                };
                openRecent.DropDownItems.Add(item3);
            }
        }

        public void prevStage()
        {
            if (split.LiveIndex >= 1)
            {
                if (split.LiveIndex > split.LastIndex)
                {
                    stopwatch.Enabled = true;
                }
                split.Previous();
                updateDisplay();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return timerHotkey(keyData);
        }

        private void recent_Click(object sender, EventArgs e)
        {
            if (promptForSave())
            {
                split.RunFile = ((ToolStripMenuItem)sender).Name;
                LoadFile();
            }
        }

        private void reloadButton_Click(object sender, EventArgs e)
        {
            LoadFile();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            ResetSplits();
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            saveAs();
        }

        private void saveAs()
        {
            modalWindowOpened = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                split.RunFile = saveFileDialog.FileName;
                saveFile();
            }
            modalWindowOpened = false;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            saveFile();
        }

        private void saveFile()
        {
            if (split.RunFile == null)
            {
                saveButton.Enabled = false;
            }
            else
            {
                split.UpdateBest(Settings.Profile.BestAsOverall);
                new FileInfo(split.RunFile);
                StreamWriter writer = null;
                try
                {
                    writer = new StreamWriter(split.RunFile);
                    writer.WriteLine("Title=" + split.RunTitle);
                    writer.WriteLine("Goal=" + split.RunGoal);
                    writer.WriteLine("Attempts=" + split.AttemptsCount);
                    writer.WriteLine("Offset=" + split.StartDelay);
                    writer.WriteLine(string.Concat(new object[] { "Size=", detailPreferredSize.Width, ",", detailPreferredSize.Height }));
                    List<string> list = new List<string>();
                    foreach (SplitSegment segment in split.segments)
                    {
                        if (segment.Name != null)
                        {
                            list.Add("\"" + segment.IconPath + "\"");
                            string[] strArray = new string[] { segment.Name, segment.OldTime.ToString(CultureInfo.GetCultureInfo("").NumberFormat), segment.BestTime.ToString(CultureInfo.GetCultureInfo("").NumberFormat), segment.BestSegment.ToString(CultureInfo.GetCultureInfo("").NumberFormat) };
                            string str = string.Join(",", strArray);
                            writer.WriteLine(str);
                        }
                    }
                    writer.WriteLine("Icons=" + string.Join(",", list.ToArray()));
                    writer.Close();
                    split.UnsavedSplit = false;
                }
                catch (Exception exception)
                {
                    MessageBoxEx.Show(this, exception.Message, "Save error");
                }
                finally
                {
                    if (writer != null)
                    {
                        if (Settings.Profile.RecentFiles != null)
                        {
                            if (Settings.Profile.RecentFiles.Contains(split.RunFile))
                            {
                                Settings.Profile.RecentFiles.Remove(split.RunFile);
                            }
                            else if (Settings.Profile.RecentFiles.Count > 9)
                            {
                                Settings.Profile.RecentFiles.RemoveAt(Settings.Profile.RecentFiles.Count - 1);
                            }
                            Settings.Profile.RecentFiles.Insert(0, split.RunFile);
                        }
                        populateRecentFiles();
                    }
                }
            }
        }

        private void setColorsButton_Click(object sender, EventArgs e)
        {
            CustomizeColors colors = new CustomizeColors();
            base.TopMost = false;
            dview.TopMost = false;
            modalWindowOpened = true;
            if (colors.ShowDialog(this) == DialogResult.OK)
                updateDetailed();

            base.TopMost = Settings.Profile.OnTop;
            dview.TopMost = Settings.Profile.DViewOnTop;
            modalWindowOpened = false;
        }

        private void setDisplay(DisplayMode mode)
        {
            Settings.Profile.DisplayMode = (int)mode;
            clockResize = false;
            MinimumSize = new Size(0, 0);
            displayTimerOnlyButton.Checked = false;
            displayCompactButton.Checked = false;
            displayWideButton.Checked = false;
            displayDetailedButton.Checked = false;
            if (mode == DisplayMode.Compact)
            {
                displayCompactButton.Checked = true;
            }
            else if (mode == DisplayMode.Wide)
            {
                displayWideButton.Checked = true;
            }
            else if (mode == DisplayMode.Detailed)
            {
                displayDetailedButton.Checked = true;
            }
            else
            {
                displayTimerOnlyButton.Checked = true;
            }
            if (split.LiveRun)
            {
                if (mode == DisplayMode.Compact)
                {
                    displayCompact();
                }
                else if (mode == DisplayMode.Wide)
                {
                    displayWide();
                }
                else if (mode == DisplayMode.Detailed)
                {
                    displayDetail();
                }
                else
                {
                    displayTimer();
                }
            }
            else
            {
                displayTimer();
            }

            painter.RequestBackgroundRedraw();
            base.Invalidate();
        }

        private void setHotkeys()
        {
            clearHotkeys();
            if (Settings.Profile.EnabledHotkeys)
            {
                RegisterHotKey(base.Handle, 0x9d82, keyMods(Settings.Profile.SplitKey), stripkeyMods(Settings.Profile.SplitKey));
                RegisterHotKey(base.Handle, 0x9d83, keyMods(Settings.Profile.PauseKey), stripkeyMods(Settings.Profile.PauseKey));
                RegisterHotKey(base.Handle, 0x9d84, keyMods(Settings.Profile.StopKey), stripkeyMods(Settings.Profile.StopKey));
                RegisterHotKey(base.Handle, 0x9d85, keyMods(Settings.Profile.ResetKey), stripkeyMods(Settings.Profile.ResetKey));
                RegisterHotKey(base.Handle, 0x9d86, keyMods(Settings.Profile.PrevKey), stripkeyMods(Settings.Profile.PrevKey));
                RegisterHotKey(base.Handle, 0x9d87, keyMods(Settings.Profile.NextKey), stripkeyMods(Settings.Profile.NextKey));
                RegisterHotKey(base.Handle, 0x9d88, keyMods(Settings.Profile.CompTypeKey), stripkeyMods(Settings.Profile.CompTypeKey));
            }
            RegisterHotKey(base.Handle, 0x9d89, keyMods(Settings.Profile.HotkeyToggleKey), stripkeyMods(Settings.Profile.HotkeyToggleKey));
        }

        private void showAttemptCount_Click(object sender, EventArgs e)
        {
            Settings.Profile.ShowAttempts = !Settings.Profile.ShowAttempts;
            showAttemptCount.Checked = Settings.Profile.ShowAttempts;
            painter.RequestBackgroundRedraw();
            base.Invalidate();
        }

        private void showRunGoal_Click(object sender, EventArgs e)
        {
            Settings.Profile.ShowGoal = !Settings.Profile.ShowGoal;
            showRunGoalMenuItem.Checked = Settings.Profile.ShowGoal;
            setDisplay((DisplayMode)Settings.Profile.DisplayMode);
        }

        private void showDecimalSeparator_Click(object sender, EventArgs e)
        {
            Settings.Profile.ShowDecimalSeparator = !Settings.Profile.ShowDecimalSeparator;
            showDecimalSeparator.Checked = Settings.Profile.ShowDecimalSeparator;
            painter.RequestBackgroundRedraw();
            base.Invalidate();
        }

        private void showRunTitleButton_Click(object sender, EventArgs e)
        {
            Settings.Profile.ShowTitle = !Settings.Profile.ShowTitle;
            showRunTitleButton.Checked = Settings.Profile.ShowTitle;
            setDisplay((DisplayMode)Settings.Profile.DisplayMode);
        }

        private void showSegsDec()
        {
            if (Settings.Profile.DisplaySegs > 2)
            {
                Settings.Profile.DisplaySegs--;
            }
            setDisplay(currentDispMode);
            updateDetailed();
        }

        private void showSegsInc()
        {
            if (Settings.Profile.DisplaySegs < 40)
            {
                Settings.Profile.DisplaySegs++;
            }
            setDisplay(currentDispMode);
            updateDetailed();
        }

        private void startDelay_Tick(object sender, EventArgs e, long startingTicks = 0)
        {
            startDelay.Dispose();
            startDelay = null;
            split.AttemptsCount++;
            timer.StartAt(new TimeSpan(startingTicks));
        }

        private void startTimer(long startingTicks = 0, bool useDelay = true)
        {
            if (split.Count > 0)
            {
                Settings.Profile.Width = split.segments[split.LastIndex].TimeWidth;
                Settings.Profile.last = split.segments[split.LastIndex].TimeString;
            };
            if (startDelay == null)
            {
                if (useDelay && split.StartDelay > 0)
                {
                    offsetStartTime = DateTime.UtcNow;
                    startDelay = new Timer();
                    startDelay.Interval = split.StartDelay;
                    startDelay.Tick += (sender, e) => startDelay_Tick(sender, e, startingTicks);
                    startDelay.Enabled = true;
                    base.Invalidate();
                }
                else
                {
                    if (split.LiveRun)
                    {
                        split.AttemptsCount++;
                    }
                    timer.StartAt(new TimeSpan(startingTicks));
                    painter.RequestBackgroundRedraw();
                    base.Invalidate();
                }
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            StopSplits();
        }

        private void stopwatch_Tick(object sender, EventArgs e)
        {
            if (flashDelay == null)
            {
                base.Invalidate();
            }
            if (Settings.Profile.RainbowSplits)
            {
                ColorSettings.Profile.SegRainbow = Color.FromArgb(255, r, g, b);
                //colour stuff
                if (rr) { b -= 4; g += 4; };
                if (gg) { r -= 4; b += 4; };
                if (bb) { g -= 4; r += 4; };
                if ((rr) && (b == 127)) { gg = true; rr = false; };
                if ((gg) && (r == 127)) { bb = true; gg = false; };
                if ((bb) && (g == 127)) { rr = true; bb = false; };
                // I don't like this at all
                updateDetailed();
            };
        }

        private uint stripkeyMods(Keys key)
        {
            return (uint)(key & Keys.KeyCode);
        }

        private string timeFormatter(double secs, TimeFormat format)
        {
            TimeSpan span = TimeSpan.FromSeconds(Math.Abs(Math.Truncate(secs * 10) / 10));

            string str = "";
            if (((format == TimeFormat.Delta) || (format == TimeFormat.DeltaShort)) && (secs >= 0.0))
                str = str + "+";
            else if (secs < 0.0)
                str = str + "-";

            if (format == TimeFormat.Seconds)
                return (Math.Truncate(secs * 100) / 100).ToString();

            if (format == TimeFormat.Long)
            {
                if (span.TotalHours >= 1.0)
                    return (str + string.Format("{0}:{1:00}:{2:00.0}", Math.Floor(span.TotalHours), span.Minutes, span.Seconds + (span.Milliseconds / 1000.0)));
                return (str + string.Format("{0}:{1:00.0}", span.Minutes, span.Seconds + (span.Milliseconds / 1000.0)));
            }

            if ((span.TotalMinutes >= 1.0) || (format == TimeFormat.Short))
                span = TimeSpan.FromSeconds(Math.Abs(Math.Truncate(secs)));

            if ((span.TotalMinutes >= 100.0) && (format == TimeFormat.DeltaShort))
                span = new TimeSpan(0, 0x63, 0x3b);

            if (span.TotalHours >= 100.0)
                span = TimeSpan.FromMinutes(Math.Truncate(span.TotalMinutes));

            if ((span.TotalHours >= 100.0) && (format != TimeFormat.DeltaShort))
                return (str + string.Format("{0}h{1:00}", Math.Floor(span.TotalHours), span.Minutes));

            if ((span.TotalHours >= 1.0) && (format != TimeFormat.DeltaShort))
                return (str + string.Format("{0}:{1:00}:{2:00}", Math.Floor(span.TotalHours), span.Minutes, span.Seconds));

            if ((span.TotalMinutes >= 1.0) || (format == TimeFormat.Short))
                return (str + string.Format("{0}:{1:00}", Math.Floor(span.TotalMinutes), span.Seconds));

            return (str + string.Format("{0:0.0}", span.TotalSeconds));
        }

        private double timeParse(string timeString)
        {
            double num = 0.0;
            foreach (string str in timeString.Split(new char[] { ':' }))
            {
                double num2;
                if (double.TryParse(str, out num2))
                {
                    num = (num * 60.0) + num2;
                }
            }
            return num;
        }

        public void timerControl()
        {
            if (doubleTapDelay == null)
            {
                if (Settings.Profile.DoubleTapGuard > 0)
                {
                    doubleTapDelay = new Timer();
                    doubleTapDelay.Tick += new EventHandler(doubleTapDelay_Tick);
                    doubleTapDelay.Interval = Math.Min(0x1388, Settings.Profile.DoubleTapGuard);
                    doubleTapDelay.Enabled = true;
                }
                if (split.LiveRun)
                {
                    if (stopwatch.Enabled && (startDelay == null))
                    {
                        doSplit();
                    }
                    else if (split.Done)
                    {
                        StopSplits();
                    }
                    else
                    {
                        pauseResume();
                    }
                }
                else
                {
                    pauseResume();
                }
            }
        }

        // Checks for local hotkeys
        public bool timerHotkey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Space:
                case Keys.Enter:    // Split
                    timerControl();
                    return true;

                case Keys.Left:     // Previous
                    prevStage();
                    return true;

                case Keys.Up:       // Expand splits
                    showSegsInc();
                    return true;

                case Keys.Right:    // Next
                    NextStage();
                    return true;

                case Keys.Down:     // Unexpand splits
                    showSegsDec();
                    return true;

                case Keys.D0:       // Switch to timer display
                    setDisplay(DisplayMode.Timer);
                    return true;

                case Keys.D1:       // Switch to compact display
                    setDisplay(DisplayMode.Compact);
                    return true;

                case Keys.D2:       // Switch to wide display
                    setDisplay(DisplayMode.Wide);
                    return true;

                case Keys.D3:       // Switch to detailed display
                    setDisplay(DisplayMode.Detailed);
                    return true;

                // Got removed for being more annoying than useful
                /*case Keys.C:        // Configure
                    this.configure(0);
                    return true;*/

                case Keys.P:        // Pause
                    pauseResume();
                    return true;

                case Keys.R:        // Reset
                    ResetSplits();
                    return true;

                case Keys.S:        // Stop
                    StopSplits();
                    return true;

                case Keys.Tab:      // Switch comparison type
                    SwitchComparisonType();
                    return true;
            }
            return false;
        }

        private void unflashClock(object sender, EventArgs e)
        {
            flashDelay.Dispose();
            flashDelay = null;
        }

        // Detailed Update (Updates some stuff, not necessarely in the detailed window or detailed view.
        public void updateDetailed()
        {
            // TODO: Best possible time does not count down, neither does time loss / gain
            ColorSettings colors = ColorSettings.Profile;
            // For every split, checks conditions set the time string, the color and the width of the split time
            for (int i = 0; i <= split.LastIndex; i++)
            {
                if ((i < split.LiveIndex) && (timer.ElapsedTicks > 0L))
                {
                    double segDelta = split.SegDelta(split.segments[i].LiveTime, i);
                    double runDelta = split.RunDelta(split.segments[i].LiveTime, i);

                    // If there is a Delta to write...
                    if ((split.segments[i].LiveTime > 0.0) && (split.CompTime(i) > 0.0))
                    {
                        split.segments[i].TimeString = timeFormatter(split.RunDeltaAt(i), TimeFormat.Delta);
                        if (split.LiveSegment(i) != 0.0 && (split.segments[i].BestSegment == 0.0 || split.LiveSegment(i) < split.segments[i].BestSegment))
                        {
                            if (Settings.Profile.RainbowSplits)
                            {
                                split.segments[i].TimeColor = colors.SegRainbow;
                            }
                            else
                            {
                                split.segments[i].TimeColor = colors.SegBestSegment;
                            };
                        }
                        else if (runDelta < 0.0)
                        {
                            if (segDelta < 0.0)
                            {
                                split.segments[i].TimeColor = colors.SegAheadGain;
                            }
                            else
                            {
                                split.segments[i].TimeColor = colors.SegAheadLoss;
                            }
                        }
                        else if (segDelta > 0.0)
                        {
                            split.segments[i].TimeColor = colors.SegBehindLoss;
                        }
                        else
                        {
                            split.segments[i].TimeColor = colors.SegBehindGain;
                        }
                    }
                    // If the split was missed...
                    else if (split.segments[i].LiveTime == 0.0)
                    {
                        split.segments[i].TimeString = "-";
                        split.segments[i].TimeColor = colors.SegMissingTime;
                    }
                    // If there was no live time to compare splits to...
                    else if (split.CompTime(i) == 0.0)
                    {
                        split.segments[i].TimeString = timeFormatter(split.segments[i].LiveTime, TimeFormat.Short);
                        if (/*(i == 0 || this.split.segments[i - 1].LiveTime > 0.0) &&
                            (this.split.segments[i].BestSegment == 0.0) || this.split.LiveSegment(i) < this.split.segments[i].BestSegment*/
                            split.LiveSegment(i) != 0.0 && (split.segments[i].BestSegment == 0.0 || split.LiveSegment(i) < split.segments[i].BestSegment))
                        {
                            if (Settings.Profile.RainbowSplits)
                            {
                                split.segments[i].TimeColor = colors.SegRainbow;
                            }
                            else
                            {
                                split.segments[i].TimeColor = colors.SegBestSegment;
                            };
                        }
                        else
                        {
                            split.segments[i].TimeColor = colors.SegNewTime;
                        }
                    }
                }
                else if (i == split.LiveIndex)
                {
                    split.segments[i].TimeColor = colors.LiveSeg;
                    if (split.CompTime(i) > 0.0)
                    {
                        split.segments[i].TimeString = timeFormatter(split.CompTime(i), TimeFormat.Short); //changed
                    }
                    else
                    {
                        split.segments[i].TimeString = "-";
                    }
                }
                else
                {
                    split.segments[i].TimeColor = colors.FutureSegTime;
                    if (split.CompTime(i) > 0.0)
                    {
                        split.segments[i].TimeString = timeFormatter(split.CompTime(i), TimeFormat.Short);
                    }
                    else
                    {
                        split.segments[i].TimeString = "-";
                    }
                }
                split.segments[i].TimeWidth = MeasureDisplayStringWidth(split.segments[i].TimeString, timeFont);
            }

            painter.RequestBackgroundRedraw();
            base.Invalidate();

            // Updates the detailed view window columns things...
            //
            if ((dview.segs.RowCount > 0) && (dview.finalSeg.RowCount > 1))
            {
                dview.Deltas.Clear();
                for (int j = 0; j < dview.segs.RowCount; ++j)
                {
                    DataGridViewRow row;
                    if (j < (dview.segs.RowCount - 1))
                        row = dview.segs.Rows[j + 1];
                    else
                        row = dview.finalSeg.Rows[1];

                    DataGridViewCell oldTimeCell = row.Cells[1];
                    DataGridViewCell bestTimeCell = row.Cells[2];
                    DataGridViewCell sumOfBestsTimeCell = row.Cells[3];
                    DataGridViewCell liveTimeCell = row.Cells[4];
                    DataGridViewCell deltaCell = row.Cells[5];

                    double oldTime = split.segments[j].OldTime;
                    double bestTime = split.segments[j].BestTime;
                    double sumOfBestsTime = split.SumOfBests(j);
                    double liveTime = split.segments[j].LiveTime;
                    double oldDelta = split.LastDelta(j);

                    double secs = split.RunDeltaAt(j);

                    // Puts oldTime in the "Old" column
                    if (oldTime > 0.0)
                        oldTimeCell.Value = timeFormatter(oldTime, TimeFormat.Short);
                    else if (split.LastSegment.OldTime > 0.0)
                        oldTimeCell.Value = "-";
                    else
                        oldTimeCell.Value = null;

                    // Puts bestTime in "Best" column
                    if (bestTime > 0.0)
                    {
                        bestTimeCell.Value = timeFormatter(bestTime, TimeFormat.Short);
                        if ((split.segments[j].BestSegment > 0.0) && Settings.Profile.DViewShowSegs)
                        {
                            object obj2 = bestTimeCell.Value;
                            bestTimeCell.Value = string.Concat(new object[] { obj2, " [", timeFormatter(split.segments[j].BestSegment, TimeFormat.Short), "]" });
                        }
                    }
                    else if (split.LastSegment.BestTime > 0.0)
                        bestTimeCell.Value = "-";
                    else
                        bestTimeCell.Value = null;

                    // Puts sumOfBestsTime in "Sum of Bests" column
                    if (sumOfBestsTime > 0.0)
                        sumOfBestsTimeCell.Value = timeFormatter(sumOfBestsTime, TimeFormat.Short);
                    else if (split.SumOfBests(split.LastIndex) > 0.0)
                        sumOfBestsTimeCell.Value = "-";
                    else
                        sumOfBestsTimeCell.Value = null;

                    // Puts liveTime in "Live" column
                    if (liveTime > 0.0)
                    {
                        liveTimeCell.Value = timeFormatter(liveTime, TimeFormat.Short);
                        if ((split.LiveSegment(j) > 0.0) && Settings.Profile.DViewShowSegs)
                        {
                            object obj3 = liveTimeCell.Value;
                            liveTimeCell.Value = string.Concat(new object[] { obj3, " [", timeFormatter(split.LiveSegment(j), TimeFormat.Short), "]" });
                        }
                    }
                    else if ((j < split.LiveIndex) && (timer.ElapsedTicks > 0L))
                        liveTimeCell.Value = "-";
                    else
                        liveTimeCell.Value = null;

                    // If the current row corresponds to the current split
                    if (j == split.LiveIndex)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            cell.Style.BackColor = ColorSettings.Profile.UsedDViewSegHighlight;
                            cell.Style.ForeColor = ColorSettings.Profile.UsedDViewSegCurrentText;
                        }
                        deltaCell.Value = null;
                    }
                    else    // Else, apply general style...
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            cell.Style.BackColor = Color.Black;
                            cell.Style.ForeColor = row.DefaultCellStyle.ForeColor;
                        }

                        // If the current row is before the current split and the timer is running
                        if ((j < split.LiveIndex) && (timer.ElapsedTicks > 0L))
                        {
                            if ((liveTime > 0.0) && (split.CompTime(j) > 0.0))
                            {
                                deltaCell.Value = timeFormatter(secs, TimeFormat.Delta);

                                liveTimeCell.Style.ForeColor = getDViewDeltaColor(secs, secs);
                                if (split.LiveSegment(j) > 0.0 && (split.segments[j].BestSegment == 0.0 || split.LiveSegment(j) < split.segments[j].BestSegment))
                                    deltaCell.Style.ForeColor = ColorSettings.Profile.UsedDViewSegBestSegment;
                                else
                                    deltaCell.Style.ForeColor = getDViewDeltaColor(secs, oldDelta);
                            }
                            else
                            {
                                deltaCell.Value = "n/a";
                                deltaCell.Style.ForeColor = ColorSettings.Profile.UsedDViewSegMissingTime;
                            }
                        }
                        else
                            deltaCell.Value = null;
                    }

                    if (secs != 0.0)
                        dview.Deltas.Add(secs);
                    else
                        dview.Deltas.Add(0.0);
                }

                if (split.RunTitle != "")
                    dview.segs.Rows[0].Cells[0].Value = split.RunTitle + "   Goal: " + split.RunGoal;
                else
                    dview.segs.Rows[0].Cells[0].Value = "Segment";
                dview.segs.DefaultCellStyle.SelectionForeColor = ColorSettings.Profile.UsedDViewSegCurrentText;

                if (split.ComparingType == RunSplits.CompareType.Old)
                {
                    dview.segs.Columns[1].DefaultCellStyle.ForeColor = ColorSettings.Profile.UsedDViewSegCurrentText;
                    dview.segs.Columns[2].DefaultCellStyle.ForeColor = ColorSettings.Profile.UsedDViewSegDefaultText;
                    dview.segs.Columns[3].DefaultCellStyle.ForeColor = ColorSettings.Profile.UsedDViewSegDefaultText;
                }
                else if (split.ComparingType == RunSplits.CompareType.Best)
                {
                    dview.segs.Columns[1].DefaultCellStyle.ForeColor = ColorSettings.Profile.UsedDViewSegDefaultText;
                    dview.segs.Columns[2].DefaultCellStyle.ForeColor = ColorSettings.Profile.UsedDViewSegCurrentText;
                    dview.segs.Columns[3].DefaultCellStyle.ForeColor = ColorSettings.Profile.UsedDViewSegDefaultText;
                }
                else
                {
                    dview.segs.Columns[1].DefaultCellStyle.ForeColor = ColorSettings.Profile.UsedDViewSegDefaultText;
                    dview.segs.Columns[2].DefaultCellStyle.ForeColor = ColorSettings.Profile.UsedDViewSegDefaultText;
                    dview.segs.Columns[3].DefaultCellStyle.ForeColor = ColorSettings.Profile.UsedDViewSegCurrentText;
                }
                dview.segs.Columns[0].DefaultCellStyle.ForeColor = ColorSettings.Profile.UsedDViewSegDefaultText;
                dview.finalSeg.Columns[0].DefaultCellStyle.ForeColor = dview.segs.Columns[0].DefaultCellStyle.ForeColor;
                dview.finalSeg.Columns[1].DefaultCellStyle.ForeColor = dview.segs.Columns[1].DefaultCellStyle.ForeColor;
                dview.finalSeg.Columns[2].DefaultCellStyle.ForeColor = dview.segs.Columns[2].DefaultCellStyle.ForeColor;
                dview.finalSeg.Columns[3].DefaultCellStyle.ForeColor = dview.segs.Columns[3].DefaultCellStyle.ForeColor;

                dview.setDeltaPoints();
                dview.updateColumns();

                int num10 = Math.Max(Settings.Profile.DisplaySegs, 2);
                int liveIndex = split.LiveIndex;
                if (num10 < 3)
                {
                    int num13 = split.LiveIndex;
                }
                dview.segs.Height = Math.Max(2, Math.Min(num10, dview.segs.RowCount)) * dview.segs.RowTemplate.Height;
                if (dview.segs.RowCount > 1)
                {
                    int num11 = 3;
                    if (Settings.Profile.DisplaySegs < 4)
                    {
                        num11 = 2;
                    }
                    dview.segs.FirstDisplayedScrollingRowIndex = Math.Min(dview.segs.RowCount - 1, 1 + Math.Max(0, (split.LiveIndex - num10) + num11));
                }
                dview.finalSeg.FirstDisplayedScrollingRowIndex = 1;
                for (int k = 1; k < dview.finalSeg.Columns.Count; k++)
                {
                    dview.segs.AutoResizeColumn(k, DataGridViewAutoSizeColumnMode.AllCellsExceptHeader);
                    dview.finalSeg.AutoResizeColumn(k, DataGridViewAutoSizeColumnMode.AllCellsExceptHeader);
                    dview.segs.Columns[k].Width = Math.Max(dview.segs.Columns[k].Width, dview.finalSeg.Columns[k].Width);
                    dview.finalSeg.Columns[k].Width = dview.segs.Columns[k].Width;
                }
            }
            dview.resetHeight();
        }

        private void updateDisplay()
        {
            if (Settings.Profile.CompareAgainst == 0)
                split.CompType = RunSplits.CompareType.Fastest;
            else if (Settings.Profile.CompareAgainst == 1)
                split.CompType = RunSplits.CompareType.Old;
            else if (Settings.Profile.CompareAgainst == 2)
                split.CompType = RunSplits.CompareType.Best;
            else if (Settings.Profile.CompareAgainst == 3)
                split.CompType = RunSplits.CompareType.SumOfBests;
            updateDetailed();
            base.Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x312 && !modalWindowOpened)
            {
                switch (m.WParam.ToInt32())
                {
                    case 0x9d82:    // Split
                        timerControl();
                        break;

                    case 0x9d83:    // Pause
                        pauseResume();
                        break;

                    case 0x9d84:    // Stop
                        StopSplits();
                        break;

                    case 0x9d85:    // Reset
                        ResetSplits();
                        break;

                    case 0x9d86:    // Previous
                        prevStage();
                        break;

                    case 0x9d87:    // Next
                        NextStage();
                        break;

                    case 0x9d88:    // Switch Comparison Type
                        SwitchComparisonType();
                        break;

                    case 0x9d89:    // Toggle Global Hotkeys
                        Settings.Profile.EnabledHotkeys = !Settings.Profile.EnabledHotkeys;
                        setHotkeys();
                        painter.RequestBackgroundRedraw();
                        base.Invalidate();
                        break;
                }
            }
            base.WndProc(ref m);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (!modalWindowOpened)
            {
                split.RunFile = ((string[])drgevent.Data.GetData(DataFormats.FileDrop, false)).First<string>();
                LoadFile();
            }
            base.OnDragDrop(drgevent);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (!modalWindowOpened)
            {
                if (drgevent.Data.GetDataPresent(DataFormats.FileDrop))
                    drgevent.Effect = DragDropEffects.Copy;
                else
                    drgevent.Effect = DragDropEffects.None;
            }

            base.OnDragEnter(drgevent);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            clearHotkeys();
            Settings.Profile.WindowPosition = base.Location;
            Settings.Profile.LastFile = split.RunFile;
            Settings.Profile.Save();
            ColorSettings.Profile.Save();
            base.OnFormClosed(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int wParam = 0;
                int x = e.X;
                int y = e.Y;
                /*if (sender != this)
                {
                    x += ((Control)sender).Location.X;
                    y += ((Control)sender).Location.Y;
                }*/
                if ((currentDispMode == DisplayMode.Wide) && (Math.Abs(x - ((clockRect.Right + wideSegWidth) - 3)) < 4))
                {
                    wideSegResizing = true;
                    wideSegResizeWidth = wideSegWidthBase;
                    wideSegX = x;
                    Cursor.Current = Cursors.SizeWE;
                }
                else if ((currentDispMode == DisplayMode.Wide) && (Math.Abs(x - (base.Width - 0x75)) < 4))
                {
                    wideResizing = true;
                    wideResizingX = x;
                    Cursor.Current = Cursors.SizeWE;
                }
                else if ((currentDispMode == DisplayMode.Detailed) && (Math.Abs(y - (clockRect.Top - 1)) < 4))
                {
                    detailResizing = true;
                    detailResizingY = y;
                    Cursor.Current = Cursors.SizeNS;
                }
                else if ((x >= (clockRect.Right - 5)) && (x <= clockRect.Right))
                {
                    if (((y >= (clockRect.Bottom - 5)) && (y <= clockRect.Bottom)) && (currentDispMode != DisplayMode.Wide))
                    {
                        Cursor.Current = Cursors.SizeNWSE;
                        wParam = 0x11;
                    }
                    else if (((y <= (clockRect.Top + 5)) && (y >= clockRect.Top)) && (currentDispMode != DisplayMode.Wide))
                    {
                        Cursor.Current = Cursors.SizeNESW;
                        wParam = 14;
                    }
                    else
                    {
                        Cursor.Current = Cursors.SizeWE;
                        wParam = 11;
                    }
                    MinimumSize = new Size((base.Width - clockRect.Width) + clockMinimumSize.Width, (base.Height - clockRect.Height) + clockMinimumSize.Height);
                    clockResize = true;
                }
                else if ((x <= (clockRect.Left + 5)) && (x >= clockRect.Left))
                {
                    if (((y >= (clockRect.Bottom - 5)) && (y <= clockRect.Bottom)) && (currentDispMode != DisplayMode.Wide))
                    {
                        Cursor.Current = Cursors.SizeNESW;
                        wParam = 0x10;
                    }
                    else if (((y <= (clockRect.Top + 5)) && (y >= clockRect.Top)) && (currentDispMode != DisplayMode.Wide))
                    {
                        Cursor.Current = Cursors.SizeNWSE;
                        wParam = 13;
                    }
                    else
                    {
                        Cursor.Current = Cursors.SizeWE;
                        wParam = 10;
                    }
                    MinimumSize = new Size((base.Width - clockRect.Width) + clockMinimumSize.Width, (base.Height - clockRect.Height) + clockMinimumSize.Height);
                    clockResize = true;
                }
                else if (((currentDispMode != DisplayMode.Wide) && (y >= (clockRect.Bottom - 5))) && (y <= clockRect.Bottom))
                {
                    Cursor.Current = Cursors.SizeNS;
                    wParam = 15;
                    MinimumSize = new Size((base.Width - clockRect.Width) + clockMinimumSize.Width, (base.Height - clockRect.Height) + clockMinimumSize.Height);
                    clockResize = true;
                }
                else if (((currentDispMode != DisplayMode.Wide) && (y <= (clockRect.Top + 5))) && (y >= clockRect.Top))
                {
                    Cursor.Current = Cursors.SizeNS;
                    wParam = 12;
                    MinimumSize = new Size((base.Width - clockRect.Width) + clockMinimumSize.Width, (base.Height - clockRect.Height) + clockMinimumSize.Height);
                    clockResize = true;
                }
                else
                    wParam = 2;

                if (wParam > 0)
                {
                    ReleaseCapture();
                    SendMessage(base.Handle, 0xa1, wParam, 0);
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if ((Control.MouseButtons & MouseButtons.Left) != MouseButtons.None)
                clockResize = false;

            int x = e.X;
            int y = e.Y;
            /*if (sender != this)
            {
                x += ((Control)sender).Location.X;
                y += ((Control)sender).Location.Y;
            }*/

            if (wideSegResizing)
            {
                wideSegWidthBase = Math.Max(60, wideSegResizeWidth + (x - wideSegX));
                setDisplay(DisplayMode.Wide);
            }
            else if (detailResizing)
            {
                if (y < (detailResizingY + segHeight))
                {
                    if ((y <= (detailResizingY - segHeight)) && (Settings.Profile.DisplaySegs > 2))
                    {
                        if (!Settings.Profile.DisplayBlankSegs && (Settings.Profile.DisplaySegs > split.Count))
                        {
                            Settings.Profile.DisplaySegs = Math.Min(split.Count - 1, 40);
                            updateDetailed();
                        }
                        else
                            showSegsDec();

                        detailResizingY -= segHeight;
                    }
                }
                else
                {
                    int num3;
                    if (Settings.Profile.DisplayBlankSegs)
                        num3 = 40;
                    else
                        num3 = Math.Min(split.Count, 40);

                    if (Settings.Profile.DisplaySegs < num3)
                    {
                        showSegsInc();
                        detailResizingY += segHeight;
                    }
                }
            }
            else if (wideResizing)
            {
                if (x < (wideResizingX + wideSegWidth))
                {
                    if ((x <= (wideResizingX - wideSegWidth)) && (Settings.Profile.WideSegs > 1))
                    {
                        if (!Settings.Profile.WideSegBlanks && (Settings.Profile.WideSegs > split.Count))
                        {
                            Settings.Profile.WideSegs = Math.Min(split.Count - 1, 20);
                        }
                        else
                        {
                            Settings settings2 = Settings.Profile;
                            settings2.WideSegs--;
                        }
                        setDisplay(DisplayMode.Wide);
                        wideResizingX -= wideSegWidth;
                    }
                }
                else
                {
                    int num4;
                    if (Settings.Profile.WideSegBlanks)
                    {
                        num4 = 20;
                    }
                    else
                    {
                        num4 = Math.Min(split.Count, 20);
                    }
                    if (Settings.Profile.WideSegs < num4)
                    {
                        Settings settings1 = Settings.Profile;
                        settings1.WideSegs++;
                        wideResizingX += wideSegWidth;
                        setDisplay(DisplayMode.Wide);
                    }
                }
            }
            else if ((currentDispMode == DisplayMode.Wide) && (Math.Abs(x - ((clockRect.Right + wideSegWidth) - 3)) < 4))
                Cursor.Current = Cursors.SizeWE;
            else if ((currentDispMode == DisplayMode.Wide) && (Math.Abs(x - (base.Width - 0x75)) < 4))
                Cursor.Current = Cursors.SizeWE;
            else if ((currentDispMode == DisplayMode.Detailed) && (Math.Abs(y - (clockRect.Top - 1)) < 4))
                Cursor.Current = Cursors.SizeNS;
            else if ((x >= (clockRect.Right - 5)) && (x <= clockRect.Right))
            {
                if (((y >= (clockRect.Bottom - 5)) && (y <= clockRect.Bottom)) && (currentDispMode != DisplayMode.Wide))
                    Cursor.Current = Cursors.SizeNWSE;
                else if (((y <= (clockRect.Top + 5)) && (y >= clockRect.Top)) && (currentDispMode != DisplayMode.Wide))
                    Cursor.Current = Cursors.SizeNESW;
                else
                    Cursor.Current = Cursors.SizeWE;
            }
            else if ((x <= (clockRect.Left + 5)) && (x >= clockRect.Left))
            {
                if (((y >= (clockRect.Bottom - 5)) && (y <= clockRect.Bottom)) && (currentDispMode != DisplayMode.Wide))
                    Cursor.Current = Cursors.SizeNESW;
                else if (((y <= (clockRect.Top + 5)) && (y >= clockRect.Top)) && (currentDispMode != DisplayMode.Wide))
                    Cursor.Current = Cursors.SizeNWSE;
                else
                    Cursor.Current = Cursors.SizeWE;
            }
            else if (((y >= (clockRect.Bottom - 5)) && (y <= clockRect.Bottom)) && (currentDispMode != DisplayMode.Wide))
                Cursor.Current = Cursors.SizeNS;
            else if (((currentDispMode != DisplayMode.Wide) && (y <= (clockRect.Top + 5))) && (y >= clockRect.Top))
                Cursor.Current = Cursors.SizeNS;

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            clockResize = false;
            wideSegResizing = false;
            detailResizing = false;
            wideResizing = false;
            MinimumSize = new Size(0, 0);
            base.OnMouseUp(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (clockResize)
                clockRect.Size = (base.Size - MinimumSize) + clockMinimumSize;

            if (currentDispMode == DisplayMode.Detailed)
                detailPreferredSize = clockRect.Size;
            else if ((currentDispMode != DisplayMode.Wide) && (currentDispMode != DisplayMode.Null))
            {
                int width = clockRect.Width;
                if ((Settings.Profile.SegmentIcons > 1) && (currentDispMode == DisplayMode.Compact))
                    width -= clockMinimumSize.Width - clockMinimumSizeAbsolute.Width;

                Settings.Profile.ClockSize = new Size(width, clockRect.Height);
            }

            if (base.WindowState == FormWindowState.Maximized)
                base.WindowState = FormWindowState.Normal;

            base.OnResize(e);
        }

        // Functions created by Nitrofski
        // ------------------------------

        private void menuItemStartAt_Click(object sender, EventArgs e)
        {
            if (timer.IsRunning)
            {
                menuItemStartAt.Enabled = false;
                return;
            }

            StartAtDialog dialog = new StartAtDialog();
            base.TopMost = false;
            dview.TopMost = false;
            modalWindowOpened = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                StartAt(Convert.ToInt64(timeParse(dialog.StartingTime) * 10000000.0), dialog.UseDelay);
                //      Not changed... Possible problems with locale settings...
            }

            base.TopMost = Settings.Profile.OnTop;
            dview.TopMost = Settings.Profile.DViewOnTop;
            modalWindowOpened = false;
        }

        private bool promptForSave()
        {
            if (split.UnsavedSplit)
            {
                modalWindowOpened = true;
                DialogResult result = MessageBoxEx.Show(
                    "Your splits have been updated but not yet saved.\n" +
                    "Do you want to save your splits now?",
                    "Save splits?", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                modalWindowOpened = false;

                if (result == DialogResult.Cancel)
                    return false;
                if (result == DialogResult.Yes)
                {
                    if (split.RunFile == null)
                        saveAs();
                    else
                        saveFile();
                }
                split.UnsavedSplit = false;
            }
            else if (split.NeedUpdate(Settings.Profile.BestAsOverall))
            {
                modalWindowOpened = true;
                DialogResult result = MessageBoxEx.Show(
                    "You have beaten some of your best times.\n" +
                    "Do you want to update and save your splits now?",
                    "Save splits?", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                modalWindowOpened = false;

                if (result == DialogResult.Cancel)
                    return false;
                if (result == DialogResult.Yes)
                {
                    if (split.RunFile == null)
                        saveAs();
                    else
                        saveFile();
                }
            }
            return true;
        }

        private void ResetSplits()
        {
            if (split.NeedUpdate(Settings.Profile.BestAsOverall))
            {
                modalWindowOpened = true;
                DialogResult result = MessageBoxEx.Show(this,
                    "You have beaten some of your best times.\n" +
                    "Do you want to update them?",
                    "Update times?", MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                modalWindowOpened = false;

                if (result == DialogResult.Yes)
                {
                    split.UpdateBest(Settings.Profile.BestAsOverall);
                    split.UnsavedSplit = true;
                }
                if (result != DialogResult.Cancel)
                {
                    InitializeDisplay();
                }
            }
            else
                InitializeDisplay();
        }

        private void StopSplits()
        {
            if (split.NeedUpdate(Settings.Profile.BestAsOverall))
            {
                split.UpdateBest(Settings.Profile.BestAsOverall);
                split.UnsavedSplit = true;
            }
            InitializeDisplay();
        }

        private void StartAt(long startingTicks, bool useDelay)
        {
            // If the refreshing stopwatch is running
            if (stopwatch.Enabled)
            {
                throw new InvalidOperationException(
                    "Trying to start the timer with a starting time, but timer is already running.");
            }

            // If the split aren't done, so if the timer isn't running
            else if (!split.Done && (startDelay == null))
            {
                // If the timer had not been started yet
                if (timer.ElapsedTicks == 0L)
                {
                    // If it wasn't running
                    if (!timer.IsRunning)
                    {
                        InitializeDisplay();
                    }
                    startTimer(startingTicks, useDelay);

                    menuItemStartAt.Enabled = false;
                    stopButton.Enabled = true;
                    resetButton.Enabled = true;
                }

                // If the timer was paused
                else
                {
                    throw new InvalidOperationException(
                    "Trying to start the timer with a starting time, but timer is paused.");
                }
                stopwatch.Enabled = true;
            }
        }

        public enum DisplayMode
        {
            Timer,
            Compact,
            Wide,
            Detailed,
            Null
        }

        public enum KeyModifiers
        {
            Alt = 1,
            Control = 2,
            None = 0,
            Shift = 4,
            Windows = 8
        }

        public enum TimeFormat
        {
            Seconds,
            Short,
            Long,
            Delta,
            DeltaShort
        }

        private class Painter
        {
            private readonly WSplit wsplit;

            private string timeStringAbsPart;
            private string timeStringDecPart;

            private SizeF clockTimeAbsSize;
            private SizeF clockTimeDecSize;
            private SizeF clockTimeTotalSize;

            private float clockScale;

            private Bitmap background;
            private Bitmap bgImage;
            private Timer animatedBgTimer;
            private int currentAnimatedBackgroundFrame;

            private Color clockColor = Color.White;
            private Color clockGrColor = Color.White;
            private Color clockGrColor2 = Color.White;
            private Color clockPlainColor = Color.White;

            private bool bgRedrawRequested;

            private int runDelLength;
            private int runDeltaWidth;
            private bool runLosingTime;
            private int segDelLength;
            private int segDeltaWidth;
            private bool segLosingTime;
            private int segTimeLength;
            private int segTimeWidth;

            public Painter(WSplit wsplit)
            {
                this.wsplit = wsplit;
            }

            public void PrepareBackground()
            {
                bgImage = null;
                if (animatedBgTimer != null)
                {
                    animatedBgTimer.Dispose();
                    animatedBgTimer = null;
                    currentAnimatedBackgroundFrame = 0;
                }

                if (Settings.Profile.BackgroundImage)
                {
                    try
                    {
                        using (Bitmap bmp = new Bitmap(Settings.Profile.BackgroundImageFilename))
                        {
                            bgImage = bmp.Clone(Settings.Profile.BackgroundImageSelection, bmp.PixelFormat);
                        }

                        if (bgImage.FrameDimensionsList.Any(fd => fd.Equals(FrameDimension.Time.Guid))
                            && bgImage.GetFrameCount(FrameDimension.Time) > 1)
                        {
                            PropertyItem gifDelay = bgImage.GetPropertyItem(0x5100);

                            animatedBgTimer = new Timer();
                            animatedBgTimer.Interval = BitConverter.ToInt16(gifDelay.Value, 0) * 10;
                            animatedBgTimer.Tick += (o, e) =>
                                {
                                    ++currentAnimatedBackgroundFrame;
                                    if (currentAnimatedBackgroundFrame >= bgImage.GetFrameCount(FrameDimension.Time))
                                        currentAnimatedBackgroundFrame = 0;

                                    bgImage.SelectActiveFrame(FrameDimension.Time, currentAnimatedBackgroundFrame);
                                    RequestBackgroundRedraw();
                                };
                        }
                    }
                    catch (Exception e)
                    {
                        // If loading the image fails, we change de settings.
                        Settings.Profile.BackgroundImage = false;
                    }
                }
                // Prepare background
                RequestBackgroundRedraw();
            }

            public void RequestBackgroundRedraw()
            {
                bgRedrawRequested = true;
            }

            private void DrawBackground(Graphics graphics, float angle, int num5, int num8, int num13, int x)
            {
                // What is "angle?"
                // it's the angle m8...

                // If there is a need to redraw the background
                if (((background == null) || (background.Size != wsplit.Size)) || bgRedrawRequested)
                {
                    bgRedrawRequested = false;
                    GC.Collect();   // Hardcoded Garbage Collection? Mmh...

                    // Creating the bitmap
                    if ((background == null) || (background.Size != wsplit.Size))
                        background = new Bitmap(wsplit.Width, wsplit.Height);

                    Graphics bgGraphics = Graphics.FromImage(background);
                    bgGraphics.Clear(Color.Black);
                    bgGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    //
                    // If display mode is NOT Timer Only
                    //
                    if (wsplit.currentDispMode != DisplayMode.Timer)
                    {
                        Rectangle statusBarEctangle;
                        Rectangle headerTextRectangle;
                        Rectangle statusTextRectangle;
                        Rectangle timesaverectangle;
                        Rectangle sobrectangle;
                        Rectangle timesaverectangle1;
                        Rectangle sobrectangle1;
                        Rectangle timesaverectangle2;
                        Rectangle sobrectangle2;
                        Rectangle pbrectangle;
                        Rectangle pbrectangle1;
                        Rectangle pbrectangle2;
                        Rectangle bestrectangle;
                        Rectangle bestrectangle1;
                        Rectangle bestrectangle2;
                        Rectangle goalTextRectangle;

                        int ps_y, ts_y, sob_y, pb_y, best_y = 0;
                        ps_y = wsplit.clockRect.Bottom;
                        if (Settings.Profile.ShowPrevSeg) { ts_y = ps_y + 18; } else { ts_y = ps_y; };
                        if (Settings.Profile.ShowTimeSave) { sob_y = ts_y + 18; } else { sob_y = ts_y; };
                        if (Settings.Profile.ShowSoB) { pb_y = sob_y + 18; } else { pb_y = sob_y; };
                        if (Settings.Profile.PredPB) { best_y = pb_y + 18; } else { best_y = pb_y; };

                        if (wsplit.currentDispMode == DisplayMode.Wide)
                        {
                            statusBarEctangle = new Rectangle(wsplit.Width - 120, 0, 120, wsplit.Height);                       // Status bar
                            statusTextRectangle = new Rectangle(wsplit.Width - 119, wsplit.Height / 2, 118, wsplit.Height / 2); // Run status
                            timesaverectangle = new Rectangle(0, 0, 0, 0);
                            sobrectangle = new Rectangle(0, 0, 0, 0);
                            timesaverectangle1 = new Rectangle(0, 0, 0, 0);
                            sobrectangle1 = new Rectangle(0, 0, 0, 0);
                            timesaverectangle2 = new Rectangle(0, 0, 0, 0);
                            sobrectangle2 = new Rectangle(0, 0, 0, 0);
                            pbrectangle = new Rectangle(0, 0, 0, 0);
                            pbrectangle1 = new Rectangle(0, 0, 0, 0);
                            pbrectangle2 = new Rectangle(0, 0, 0, 0);
                            bestrectangle = new Rectangle(0, 0, 0, 0);
                            bestrectangle1 = new Rectangle(0, 0, 0, 0);
                            bestrectangle2 = new Rectangle(0, 0, 0, 0);

                            if ((wsplit.split.RunGoal != "") && Settings.Profile.ShowGoal)
                            {
                                headerTextRectangle = new Rectangle(wsplit.Width - 119, (wsplit.Height / 4) - 4, 59, 12);          // Run title
                                statusTextRectangle = new Rectangle(headerTextRectangle.X + headerTextRectangle.Width, (wsplit.Height / 4) - 4, 59, 12); //Run goal
                                goalTextRectangle = new Rectangle(wsplit.Width - 119, wsplit.Height / 2, 118, wsplit.Height / 2); //Run status
                            }
                            else
                            {
                                headerTextRectangle = new Rectangle(wsplit.Width - 119, (wsplit.Height / 4) - 4, 118, 12);          // Run title
                                statusTextRectangle = new Rectangle(wsplit.Width - 119, wsplit.Height / 2, 118, wsplit.Height / 2); // Run status
                                goalTextRectangle = new Rectangle(0, 0, 0, 0);  //Nothing...
                            }
                        }
                        else if (wsplit.currentDispMode == DisplayMode.Detailed)
                        {
                            headerTextRectangle = new Rectangle(0, 0, 0, 0);

                            if (Settings.Profile.ShowPrevSeg)
                            {
                                statusBarEctangle = new Rectangle(0, ps_y, wsplit.Width, 18);
                                statusTextRectangle = new Rectangle(1, ps_y + 2, wsplit.Width - 2, 16);
                            }
                            else
                            {
                                statusBarEctangle = new Rectangle(0, ps_y - 18, wsplit.Width, 18);
                                statusTextRectangle = new Rectangle(1, ps_y + 2 - 18, wsplit.Width - 2, 16);
                            };

                            timesaverectangle = new Rectangle(0, ts_y, wsplit.Width, 18);
                            sobrectangle = new Rectangle(0, sob_y, wsplit.Width, 18);
                            timesaverectangle1 = new Rectangle(0, ts_y + 2, wsplit.Width - 2, 16);
                            sobrectangle1 = new Rectangle(0, sob_y + 2, wsplit.Width - 2, 16);
                            timesaverectangle2 = new Rectangle(0, ts_y + 2, wsplit.Width - 2, 16);
                            sobrectangle2 = new Rectangle(0, sob_y + 2, wsplit.Width - 2, 16);
                            pbrectangle = new Rectangle(0, pb_y, wsplit.Width, 18);
                            bestrectangle = new Rectangle(0, best_y, wsplit.Width, 18);
                            pbrectangle1 = new Rectangle(0, pb_y + 2, wsplit.Width - 2, 16);
                            bestrectangle1 = new Rectangle(0, best_y + 2, wsplit.Width - 2, 16);
                            pbrectangle2 = new Rectangle(0, pb_y + 2, wsplit.Width - 2, 16);
                            bestrectangle2 = new Rectangle(0, best_y + 2, wsplit.Width - 2, 16);
                            statusBarEctangle = new Rectangle(0, wsplit.clockRect.Bottom, wsplit.Width, 18);            // Status bar
                            headerTextRectangle = new Rectangle(0, 0, 0, 0);                                            // Nothing? Why?
                            statusTextRectangle = new Rectangle(1, wsplit.clockRect.Bottom + 2, wsplit.Width - 2, 16);  // Run status
                            goalTextRectangle = new Rectangle(0, 0, 0, 0);                                              // Nothing...
                        }
                        else
                        {
                            statusBarEctangle = new Rectangle(0, wsplit.clockRect.Bottom, wsplit.Width, 16);            // Status bar
                            headerTextRectangle = new Rectangle(1, 2, (wsplit.Width / 2) - 2, 13);                            // Segment name
                            statusTextRectangle = new Rectangle(1, wsplit.clockRect.Bottom + 2, wsplit.Width - 2, 14);  // Run status
                            timesaverectangle = new Rectangle(0, 0, 0, 0);
                            sobrectangle = new Rectangle(0, 0, 0, 0);
                            timesaverectangle1 = new Rectangle(0, 0, 0, 0);
                            sobrectangle1 = new Rectangle(0, 0, 0, 0);
                            timesaverectangle2 = new Rectangle(0, 0, 0, 0);
                            sobrectangle2 = new Rectangle(0, 0, 0, 0);
                            pbrectangle = new Rectangle(0, 0, 0, 0);
                            pbrectangle1 = new Rectangle(0, 0, 0, 0);
                            pbrectangle2 = new Rectangle(0, 0, 0, 0);
                            bestrectangle = new Rectangle(0, 0, 0, 0);
                            bestrectangle1 = new Rectangle(0, 0, 0, 0);
                            bestrectangle2 = new Rectangle(0, 0, 0, 0);

                            if ((wsplit.split.RunGoal != "") && Settings.Profile.ShowGoal)
                            {
                                goalTextRectangle = new Rectangle(headerTextRectangle.X + headerTextRectangle.Width, 2, (wsplit.Width / 2) - 2, 13); // Run goal
                            }
                            else
                            {
                                goalTextRectangle = new Rectangle(0, 0, 0, 0); // Nothing...
                            }
                            // If the segment icon will be shown, the segment name have to be pushed right
                            if ((Settings.Profile.SegmentIcons > 0) && !wsplit.split.Done)
                            {
                                headerTextRectangle.Width -= 4 + (8 * (Settings.Profile.SegmentIcons + 1));
                                headerTextRectangle.X += 4 + (8 * (Settings.Profile.SegmentIcons + 1));
                            }
                        }

                        // If the background isn't black
                        if (!Settings.Profile.BackgroundBlack)
                        {
                            // Fill the status bar
                            if (Settings.Profile.BackgroundPlain)
                            {
                                bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.StatusBackPlain), statusBarEctangle);
                                bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.StatusBackPlain), timesaverectangle);
                                bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.StatusBackPlain), sobrectangle);
                                bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.StatusBackPlain), pbrectangle);
                                bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.StatusBackPlain), bestrectangle);
                            }
                            else
                            {
                                bgGraphics.FillRectangle(new LinearGradientBrush(statusBarEctangle, ColorSettings.Profile.StatusBack, ColorSettings.Profile.StatusBack2, angle), statusBarEctangle);
                                bgGraphics.FillRectangle(new LinearGradientBrush(statusBarEctangle, ColorSettings.Profile.StatusBack, ColorSettings.Profile.StatusBack2, angle), timesaverectangle);
                                bgGraphics.FillRectangle(new LinearGradientBrush(statusBarEctangle, ColorSettings.Profile.StatusBack, ColorSettings.Profile.StatusBack2, angle), sobrectangle);
                                bgGraphics.FillRectangle(new LinearGradientBrush(statusBarEctangle, ColorSettings.Profile.StatusBack, ColorSettings.Profile.StatusBack2, angle), pbrectangle);
                                bgGraphics.FillRectangle(new LinearGradientBrush(statusBarEctangle, ColorSettings.Profile.StatusBack, ColorSettings.Profile.StatusBack2, angle), bestrectangle);
                            };
                            // Detailed mode - Draw the title bar
                            if (wsplit.currentDispMode == DisplayMode.Detailed)
                            {
                                int titleX = 0;
                                int titleY = 0;
                                int goalX = 0;
                                int goalY = 0;

                                if (((wsplit.split.RunTitle != "") && Settings.Profile.ShowTitle) && ((wsplit.split.RunGoal != "") && Settings.Profile.ShowGoal) && !Settings.Profile.BackgroundPlain)
                                {
                                    bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.TitleBack), titleX, titleY, wsplit.Width, 32);
                                    bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.TitleBack2), titleX, titleY, wsplit.Width, 16);
                                }
                                else
                                {
                                    if ((wsplit.split.RunTitle != "") && Settings.Profile.ShowTitle)
                                    {
                                        if (Settings.Profile.BackgroundPlain)
                                            bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.TitleBackPlain), titleX, titleY, wsplit.Width, 16);
                                        else
                                        {
                                            bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.TitleBack), titleX, titleY, wsplit.Width, 16);
                                            bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.TitleBack2), titleX, titleY, wsplit.Width, 8);
                                        }
                                    }
                                    if ((wsplit.split.RunGoal != "") && Settings.Profile.ShowGoal)
                                    {
                                        if ((wsplit.split.RunTitle != "") && Settings.Profile.ShowTitle)
                                        {
                                            goalY = 16;
                                        }
                                        if (Settings.Profile.BackgroundPlain)
                                        {
                                            bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.TitleBackPlain), 0, goalY, wsplit.Width, 16);
                                        }
                                        else
                                        {
                                            bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.TitleBack), 0, goalY, wsplit.Width, 16);
                                            bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.TitleBack2), 0, goalY, wsplit.Width, 8);
                                        }
                                    }
                                }
                            }

                            // Compact mode - Draw the title bar
                            else if (wsplit.currentDispMode == DisplayMode.Compact)
                            {
                                if (Settings.Profile.BackgroundPlain)
                                    bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.TitleBackPlain), 0, 0, wsplit.Width, 18);
                                else
                                {
                                    bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.TitleBack), 0, 0, wsplit.Width, 15);
                                    bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.TitleBack2), 0, 0, wsplit.Width, 7);
                                }
                            }
                        }

                        // Refactor? Only used once, never changes
                        StringFormat format3 = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter
                        };

                        // Only used once, Alignment changes
                        StringFormat format4 = (StringFormat)format3.Clone();

                        if (wsplit.currentDispMode == DisplayMode.Compact)
                            format4.Alignment = StringAlignment.Far;

                        string s = "";
                        string statusText = "";

                        // The run is not started yet
                        if (wsplit.timer.ElapsedTicks == 0L)
                        {
                            if (wsplit.currentDispMode != DisplayMode.Wide)
                                format4.Alignment = StringAlignment.Far;

                            if (wsplit.startDelay != null)
                                statusText = "Delay";
                            else
                            {
                                statusText = "Ready";
                                if (Settings.Profile.ShowAttempts && ((wsplit.currentDispMode != DisplayMode.Detailed) || !Settings.Profile.ShowTitle))
                                    statusText += ", Attempt #" + (wsplit.split.AttemptsCount + 1);
                            }
                        }

                        // The run is done
                        else if (wsplit.split.Done)
                        {
                            if (wsplit.split.CompTime(wsplit.split.LastIndex) == 0.0)
                            {
                                if (wsplit.currentDispMode != DisplayMode.Wide)
                                    format4.Alignment = StringAlignment.Far;

                                statusText = "Done";
                            }
                            else if (wsplit.split.LastSegment.LiveTime < wsplit.split.CompTime(wsplit.split.LastIndex))
                                statusText = "New Record";
                            else
                                statusText = "Done";
                        }

                        // The run is going
                        else if ((wsplit.currentDispMode == DisplayMode.Compact) && (wsplit.split.CompTime(wsplit.split.LiveIndex) != 0.0))
                            statusText = wsplit.split.ComparingType.ToString() + ": " + wsplit.timeFormatter(wsplit.split.CompTime(wsplit.split.LiveIndex), TimeFormat.Long);
                        else if (segLosingTime)
                            statusText = "Live Segment";
                        else if (((wsplit.split.LiveIndex > 0) && (wsplit.split.segments[wsplit.split.LiveIndex - 1].LiveTime > 0.0)) && (wsplit.split.CompTime(wsplit.split.LiveIndex - 1) != 0.0))
                            statusText = "Previous Segment";

                        // Detailed mode
                        if (wsplit.currentDispMode == DisplayMode.Detailed)
                        {
                            int goalTextY = 0;
                            if ((wsplit.split.RunTitle != "") && Settings.Profile.ShowTitle)
                            {
                                bgGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                                Rectangle rectangle7 = new Rectangle(0, 1, wsplit.Width, 17);
                                goalTextY = 16;

                                // Draws the hotkey toggle indicator
                                if (Settings.Profile.HotkeyToggleKey != Keys.None)
                                {
                                    if (Settings.Profile.EnabledHotkeys)
                                    {
                                        bgGraphics.FillRectangle(Brushes.GreenYellow, wsplit.Width - 10, 4, 6, 6);
                                    }
                                    else
                                    {
                                        bgGraphics.FillRectangle(Brushes.OrangeRed, wsplit.Width - 10, 4, 6, 6);
                                    }
                                    rectangle7.Width -= 10;
                                }

                                string str8 = "";
                                if (Settings.Profile.ShowAttempts)
                                {
                                    if (wsplit.timer.IsRunning)
                                    {
                                        str8 = "#" + wsplit.split.AttemptsCount + " / ";
                                    }
                                    else
                                    {
                                        str8 = "#" + (wsplit.split.AttemptsCount + 1) + " / ";
                                    }
                                }

                                StringFormat format5 = new StringFormat
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center,
                                    Trimming = StringTrimming.EllipsisCharacter
                                };

                                bgGraphics.DrawString(str8 + wsplit.split.RunTitle, wsplit.displayFont, new SolidBrush(ColorSettings.Profile.TitleFore), rectangle7, format5);
                            }

                            if ((wsplit.split.RunGoal != "") && Settings.Profile.ShowGoal)
                            {
                                bgGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                                Rectangle goalRectangle = new Rectangle(0, 19, wsplit.Width, 17);
                                goalRectangle.Y = goalTextY;

                                if (Settings.Profile.HotkeyToggleKey != Keys.None)
                                {
                                    goalRectangle.Width -= 10;
                                }

                                StringFormat format5 = new StringFormat
                                {
                                    Alignment = StringAlignment.Center,
                                    LineAlignment = StringAlignment.Center,
                                    Trimming = StringTrimming.EllipsisCharacter
                                };
                                bgGraphics.DrawString("Goal: " + wsplit.split.RunGoal, wsplit.displayFont, new SolidBrush(ColorSettings.Profile.TitleFore), goalRectangle, format5);
                            }
                        }
                        else if (wsplit.split.Done)
                        {
                            if (wsplit.currentDispMode == DisplayMode.Wide)
                            {
                                s = statusText;
                                statusText = "Final";
                            }
                            else
                                s = "Final";
                        }
                        else if (wsplit.currentDispMode == DisplayMode.Compact)
                            s = wsplit.split.CurrentSegment.Name;
                        else if ((wsplit.split.RunTitle != "") && Settings.Profile.ShowTitle)
                            s = wsplit.split.RunTitle;
                        else
                            s = "Run";

                        if (wsplit.currentDispMode == DisplayMode.Compact)
                        {
                            headerTextRectangle.Width -= segDeltaWidth;
                            statusTextRectangle.Width -= runDeltaWidth;
                            statusTextRectangle.X += runDeltaWidth;
                        }
                        else
                        {
                            headerTextRectangle.Width -= runDeltaWidth;
                            statusTextRectangle.Width -= segDeltaWidth;
                        }

                        bgGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                        bgGraphics.DrawString(s, wsplit.displayFont, new SolidBrush(ColorSettings.Profile.StatusFore), headerTextRectangle, format3);   // To be verified, but it seems like this line writes fuck all in a negative rectangle when in Detailed mode...
                        bgGraphics.DrawString(statusText, wsplit.displayFont, new SolidBrush(ColorSettings.Profile.StatusFore), statusTextRectangle, format4);
                        // u wote m8
                        StringFormat strleft = new StringFormat
                        {
                            Alignment = StringAlignment.Near,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter
                        };
                        StringFormat strright = new StringFormat
                        {
                            Alignment = StringAlignment.Far,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter
                        };
                        // text
                        string sobtext = "-";
                        string tstext = "0:00";
                        if (Settings.Profile.ShowSoB)
                        {
                            bgGraphics.DrawString("Sum of Best", wsplit.displayFont, new SolidBrush(ColorSettings.Profile.StatusFore), sobrectangle1, strleft);
                            if (wsplit.split.SumOfBests(wsplit.split.LastIndex) > 0.0)
                            {
                                sobtext = wsplit.timeFormatter(wsplit.split.SumOfBests(wsplit.split.LastIndex), TimeFormat.Short);
                            };
                            bgGraphics.DrawString(sobtext, wsplit.displayFont, new SolidBrush(ColorSettings.Profile.StatusFore), sobrectangle2, strright);
                        };//yeah
                        //int j;
                        double segtime = 0.0;
                        //segtime = wsplit.split.segments[wsplit.split.LiveIndex].BestTime;
                        if (wsplit.split.LiveIndex > 0 && (wsplit.split.LiveIndex <= wsplit.split.LastIndex))
                        {
                            segtime = wsplit.split.segments[wsplit.split.LiveIndex].BestTime;
                            segtime -= wsplit.split.segments[wsplit.split.LiveIndex - 1].BestTime;
                        }
                        else if (wsplit.split.LiveIndex == 0)
                        {
                            segtime = wsplit.split.segments[0].BestTime;
                        };
                        if ((segtime > 0.0) && (wsplit.split.segments[wsplit.split.LiveIndex].BestSegment > 0.0) && (segtime >= wsplit.split.segments[wsplit.split.LiveIndex].BestSegment))
                        {
                            tstext = wsplit.timeFormatter(Math.Abs(segtime - wsplit.split.segments[wsplit.split.LiveIndex].BestSegment), TimeFormat.Short);
                        };
                        if ((wsplit.split.LiveIndex > 0) && (wsplit.split.LiveIndex <= wsplit.split.LastIndex) && ((wsplit.split.segments[wsplit.split.LiveIndex].BestTime == 0.0) || (wsplit.split.segments[wsplit.split.LiveIndex - 1].BestTime == 0.0)))
                        {
                            tstext = "-";
                        };
                        if (Settings.Profile.ShowTimeSave)
                        {
                            bgGraphics.DrawString("Possible Gain", wsplit.displayFont, new SolidBrush(ColorSettings.Profile.StatusFore), timesaverectangle1, strleft);
                            bgGraphics.DrawString(tstext, wsplit.displayFont, new SolidBrush(ColorSettings.Profile.StatusFore), timesaverectangle2, strright);
                        };
                        if (Settings.Profile.PredPB)
                        {
                            bgGraphics.DrawString("Predicted (PB)", wsplit.displayFont, new SolidBrush(ColorSettings.Profile.StatusFore), pbrectangle1, strleft);
                            if (wsplit.split.CompTime(wsplit.split.LastIndex) > 0.0)
                            {
                                string pbtime = "";
                                //if (wsplit.split.LiveIndex <= 0)
                                //{
                                //    pbtime = wsplit.timeFormatter(wsplit.split.CompTime(wsplit.split.LastIndex), TimeFormat.Short);
                                //}
                                //else
                                //{
                                pbtime = wsplit.timeFormatter(wsplit.split.CompTime(wsplit.split.LastIndex) + wsplit.split.LastDelta(wsplit.split.LiveIndex), TimeFormat.Short);
                                //};
                                bgGraphics.DrawString(pbtime, wsplit.displayFont, new SolidBrush(ColorSettings.Profile.StatusFore), pbrectangle2, strright);
                            }
                            else
                            {
                                bgGraphics.DrawString("-", wsplit.displayFont, new SolidBrush(ColorSettings.Profile.StatusFore), pbrectangle2, strright);
                            };
                        };
                        if (Settings.Profile.PredBest)
                        {
                            bgGraphics.DrawString("Predicted (Best)", wsplit.displayFont, new SolidBrush(ColorSettings.Profile.StatusFore), bestrectangle1, strleft);
                            if (wsplit.split.SumOfBests(wsplit.split.LastIndex) > 0.0)
                            {
                                double best = wsplit.split.SumOfBests(wsplit.split.LastIndex);
                                if ((wsplit.split.LiveIndex > 0) && (wsplit.split.LiveIndex <= wsplit.split.LastIndex))
                                {
                                    int i;
                                    for (i = wsplit.split.LiveIndex; i >= 0; i--)
                                    {
                                        if (wsplit.split.segments[i].LiveTime != 0)
                                        {
                                            best += wsplit.split.segments[i].LiveTime - wsplit.split.SumOfBests(i);
                                            break;
                                        };
                                    };
                                };
                                string besttime = wsplit.timeFormatter(best, TimeFormat.Short);
                                bgGraphics.DrawString(besttime, wsplit.displayFont, new SolidBrush(ColorSettings.Profile.StatusFore), bestrectangle2, strright);
                            }
                            else
                            {
                                bgGraphics.DrawString("-", wsplit.displayFont, new SolidBrush(ColorSettings.Profile.StatusFore), bestrectangle2, strright);
                            };
                        };
                        if (wsplit.currentDispMode != DisplayMode.Detailed && wsplit.split.RunGoal != "")
                        {
                            bgGraphics.DrawString("Goal: " + wsplit.split.RunGoal, wsplit.displayFont, new SolidBrush(ColorSettings.Profile.StatusFore), goalTextRectangle, format4);
                        }
                    }

                    //
                    // Wide or detailed modes
                    //
                    if ((wsplit.currentDispMode == DisplayMode.Wide) || (wsplit.currentDispMode == DisplayMode.Detailed))
                    {
                        Rectangle rectangle8;   // Yet another unnamed rectangle
                        int num16 = wsplit.clockRect.Right + 2;
                        int y = 0;

                        if (((wsplit.split.RunTitle != "") && Settings.Profile.ShowTitle) && ((wsplit.split.RunGoal != "") && Settings.Profile.ShowGoal))
                        {
                            y += 32;
                        }
                        else if ((wsplit.split.RunTitle != "") && Settings.Profile.ShowTitle)
                            y += 18;
                        else if ((wsplit.split.RunGoal != "") && Settings.Profile.ShowGoal)
                        {
                            y += 18;
                        }

                        if (wsplit.currentDispMode == DisplayMode.Wide)
                        {
                            rectangle8 = new Rectangle(num16, 0, (wsplit.Width - wsplit.clockRect.Width) - 122, wsplit.Height);
                            bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.StatusBack2), wsplit.clockRect.Right, 0, 2, wsplit.Height);
                        }
                        else
                            rectangle8 = new Rectangle(0, y, wsplit.Width, wsplit.clockRect.Bottom - 18);

                        if (!Settings.Profile.BackgroundBlack)
                        {
                            if (Settings.Profile.BackgroundPlain)
                                bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.SegBackPlain), rectangle8);
                            else
                                bgGraphics.FillRectangle(new LinearGradientBrush(rectangle8, ColorSettings.Profile.SegBack, ColorSettings.Profile.SegBack2, angle), rectangle8);
                        }

                        // Flag2 = Show last
                        bool flag2 = false;
                        if (((num5 > 3) && (((num13 + num5) - 1) < wsplit.split.LastIndex)) && (((wsplit.currentDispMode == DisplayMode.Detailed) && Settings.Profile.ShowLastDetailed) || ((wsplit.currentDispMode == DisplayMode.Wide) && Settings.Profile.ShowLastWide)))
                            flag2 = true;

                        if (wsplit.currentDispMode == DisplayMode.Wide)
                        {
                            if (flag2)
                                bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.StatusBack2), (wsplit.Width - 0x7a) - wsplit.wideSegWidth, 0, 2, wsplit.Height);
                            else
                                bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.StatusBack2), wsplit.Width - 0x7a, 0, 2, wsplit.Height);
                        }
                        else if (flag2)
                            bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.StatusBackPlain), 0, (wsplit.clockRect.Y - 3) - wsplit.segHeight, wsplit.Width, 3);
                        else if (Settings.Profile.BackgroundPlain || Settings.Profile.BackgroundBlack)
                            bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.StatusBackPlain), 0, wsplit.clockRect.Y - 3, wsplit.Width, 3);
                        else
                            bgGraphics.FillRectangle(new LinearGradientBrush(rectangle8, ColorSettings.Profile.StatusBack, ColorSettings.Profile.StatusBack2, 0f), 0, wsplit.clockRect.Y - 3, wsplit.Width, 3);

                        StringFormat format6 = new StringFormat
                        {
                            Trimming = StringTrimming.EllipsisCharacter,
                            LineAlignment = StringAlignment.Center
                        };

                        StringFormat format7 = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center
                        };

                        if (wsplit.currentDispMode != DisplayMode.Wide)
                        {
                            if (wsplit.segHeight > 24)
                                format6.Alignment = StringAlignment.Near;

                            format7.Alignment = StringAlignment.Far;
                        }

                        Rectangle rect = new Rectangle(0, 0, 0, 1);

                        int num18 = 0;
                        for (int i = num13; (num18 < num5) && (i <= wsplit.split.LastIndex); i++)
                        {
                            int segTimeWidth;
                            Rectangle rectangle10;
                            Rectangle rectangle11;
                            Rectangle rectangle12;
                            Rectangle rectangle13;

                            Image grayIcon;
                            ImageAttributes attributes;

                            if (((num18 + 1) >= num5) && flag2)
                            {
                                i = wsplit.split.LastIndex;
                                y += 3;
                                num16 += 2;
                            }

                            Brush brush3 = new SolidBrush(ColorSettings.Profile.FutureSegName);
                            string str9 = wsplit.split.segments[i].TimeString;
                            // thing //
                            string splittime = "";
                            if ((i < wsplit.split.LiveIndex) && (wsplit.split.segments[i].LiveTime > 0.0))
                            {
                                splittime = wsplit.timeFormatter(wsplit.split.segments[i].LiveTime, TimeFormat.Short);
                            }
                            else
                            {
                                splittime = "-";
                            };
                            // idk //
                            string name = wsplit.split.segments[i].Name;

                            if ((i == wsplit.split.LiveIndex) && runLosingTime)
                                segTimeWidth = this.segTimeWidth;
                            else
                                segTimeWidth = wsplit.split.segments[i].TimeWidth;

                            ColorMatrix newColorMatrix = new ColorMatrix
                            {
                                Matrix33 = 0.65f
                            };

                            if (wsplit.currentDispMode == DisplayMode.Wide)
                            {
                                rectangle10 = new Rectangle(num16, 0, wsplit.wideSegWidth, wsplit.Height);
                                rectangle11 = new Rectangle(num16 + 2, (wsplit.Height / 4) - 4, (wsplit.wideSegWidth - x) - 2, 12);
                                rectangle12 = new Rectangle(rectangle11.Left, wsplit.Height / 2, rectangle11.Width, wsplit.Height / 2);
                                rectangle13 = new Rectangle(rectangle11.Left + Settings.Profile.Width + 8, wsplit.Height / 2, rectangle11.Width, wsplit.Height / 2);
                            }
                            else
                            /* create rectangles here */
                            {
                                rectangle10 = new Rectangle(0, y, wsplit.Width, wsplit.segHeight);         //icon
                                rectangle11 = new Rectangle(x, y + 2, wsplit.Width - x, wsplit.segHeight); //name
                                rectangle12 = new Rectangle(x, y + 1, wsplit.Width - x, wsplit.segHeight); //delta
                                rectangle13 = new Rectangle(x - Settings.Profile.Width - 8, y + 1, wsplit.Width - x, wsplit.segHeight); //test

                                if (wsplit.segHeight <= 24)
                                {
                                    rectangle11.Width -= segTimeWidth + 2;
                                    rectangle11.Y = (y + (wsplit.segHeight / 2)) - 5;
                                    rectangle11.Height = 13;
                                }
                                else
                                {
                                    rectangle11.Y = y + 2;
                                    rectangle11.Height /= 2;
                                    rectangle12.Y = rectangle11.Bottom - 2;
                                    rectangle12.Height /= 2;
                                    rectangle13.Y = rectangle11.Bottom - 2;
                                    rectangle13.Height /= 2;
                                }
                            }
                            if ((i < wsplit.split.LiveIndex) && (wsplit.timer.ElapsedTicks > 0L))
                            {
                                brush3 = new SolidBrush(ColorSettings.Profile.PastSeg);
                                if (wsplit.split.segments[i].TimeColor == ColorSettings.Profile.SegRainbow && Settings.Profile.RainbowSplits)
                                {
                                    brush3 = new SolidBrush(ColorSettings.Profile.SegRainbow);
                                };
                            }
                            else if (i == wsplit.split.LiveIndex)
                            {
                                brush3 = new SolidBrush(ColorSettings.Profile.LiveSeg);
                                rect = rectangle10;
                                if (Settings.Profile.BackgroundPlain || Settings.Profile.BackgroundBlack)
                                {
                                    bgGraphics.FillRectangle(new SolidBrush(ColorSettings.Profile.SegHighlightPlain), rectangle10);
                                }
                                else
                                {
                                    bgGraphics.FillRectangle(new LinearGradientBrush(rectangle10, ColorSettings.Profile.SegHighlight, ColorSettings.Profile.SegHighlight2, angle), rectangle10);
                                }
                                newColorMatrix.Matrix33 = 1f;
                            }
                            if (x == 0)
                            {
                                goto Label_1F67;
                            }
                            if ((i < wsplit.split.LiveIndex) && (wsplit.timer.ElapsedTicks > 0L))
                            {
                                newColorMatrix.Matrix33 = 0.85f;
                                switch (x)
                                {
                                    case 0x10:
                                        grayIcon = wsplit.split.segments[i].GrayIcon16;
                                        goto Label_1EE9;

                                    case 0x18:
                                        grayIcon = wsplit.split.segments[i].GrayIcon24;
                                        goto Label_1EE9;

                                    case 0x20:
                                        grayIcon = wsplit.split.segments[i].GrayIcon32;
                                        goto Label_1EE9;
                                }
                                grayIcon = wsplit.split.segments[i].GrayIcon;
                            }
                            else
                            {
                                switch (x)
                                {
                                    case 0x10:
                                        grayIcon = wsplit.split.segments[i].Icon16;
                                        goto Label_1EE9;

                                    case 0x18:
                                        grayIcon = wsplit.split.segments[i].Icon24;
                                        goto Label_1EE9;

                                    case 0x20:
                                        grayIcon = wsplit.split.segments[i].Icon32;
                                        goto Label_1EE9;
                                }
                                grayIcon = wsplit.split.segments[i].Icon;
                            }
                        Label_1EE9:
                            attributes = new ImageAttributes();
                            attributes.SetColorMatrix(newColorMatrix);
                            Rectangle destRect = new Rectangle(rectangle10.X, rectangle10.Y, x, x);
                            if (wsplit.currentDispMode == DisplayMode.Wide)
                            {
                                destRect.X += wsplit.wideSegWidth - x;
                                destRect.Y += wsplit.Height - x;
                            }
                            bgGraphics.DrawImage(grayIcon, destRect, 0, 0, grayIcon.Width, grayIcon.Height, GraphicsUnit.Pixel, attributes);
                        Label_1F67:
                            bgGraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                            //test
                            if (Settings.Profile.SplitTimes && (i < wsplit.split.LiveIndex) && (x < 32) && wsplit.currentDispMode == DisplayMode.Detailed)
                            {
                                rectangle11.Width = rectangle13.Right - MeasureDisplayStringWidth(str9, wsplit.timeFont) - x - 4;
                            };
                            bgGraphics.DrawString(name, wsplit.displayFont, brush3, rectangle11, format6);
                            /* draw labels here */
                            if (i != wsplit.split.LiveIndex)
                            {
                                if (i < wsplit.split.LiveIndex && Settings.Profile.SplitTimes && (Settings.Profile.last != "-") && wsplit.timer.ElapsedTicks > 0L)
                                {
                                    bgGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                                    if (wsplit.split.segments[i].TimeColor == ColorSettings.Profile.SegRainbow && Settings.Profile.RainbowSplits)
                                    {
                                        bgGraphics.DrawString(splittime, wsplit.timeFont, new SolidBrush(ColorSettings.Profile.SegRainbow), rectangle12, format7);
                                    }
                                    else
                                    {
                                        bgGraphics.DrawString(splittime, wsplit.timeFont, new SolidBrush(ColorSettings.Profile.SegPastTime), rectangle12, format7);
                                    };
                                    bgGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                                    if (wsplit.split.CompTime(i) == 0.0) { str9 = "-"; };
                                    bgGraphics.DrawString(str9, wsplit.timeFont, new SolidBrush(wsplit.split.segments[i].TimeColor), rectangle13, format7);
                                }
                                else
                                {
                                    bgGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                                    bgGraphics.DrawString(str9, wsplit.timeFont, new SolidBrush(wsplit.split.segments[i].TimeColor), rectangle12, format7);
                                };
                            }
                            num16 += wsplit.wideSegWidth;
                            y += wsplit.segHeight;
                            num18++;
                        }

                        if (wsplit.split.LiveRun && !wsplit.split.Done)
                        {
                            Pen pen = new Pen(new SolidBrush(ColorSettings.Profile.SegHighlightBorder));
                            if (wsplit.currentDispMode == DisplayMode.Wide)
                            {
                                rect.Height--;
                                bgGraphics.DrawRectangle(pen, rect);
                            }
                            else
                            {
                                bgGraphics.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
                                bgGraphics.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
                            }
                        }
                    }

                    // Code for drawing clock back has been moved to another function. Current method's signature is temporary
                    DrawClockBack(angle, num8, bgGraphics);

                    if (((x > 0) && (wsplit.currentDispMode == DisplayMode.Compact)) && (wsplit.split.LiveRun && !wsplit.split.Done))
                    {
                        Image icon;
                        switch (x)
                        {
                            case 0x10:
                                icon = wsplit.split.CurrentSegment.Icon16;
                                break;

                            case 0x18:
                                icon = wsplit.split.CurrentSegment.Icon24;
                                break;

                            case 0x20:
                                icon = wsplit.split.CurrentSegment.Icon32;
                                break;

                            default:
                                icon = wsplit.split.CurrentSegment.Icon;
                                break;
                        }
                        Rectangle rectangle14 = new Rectangle(3, 3, x, x);
                        bgGraphics.DrawImage(icon, rectangle14, 0, 0, icon.Width, icon.Height, GraphicsUnit.Pixel);
                        bgGraphics.DrawRectangle(new Pen(new SolidBrush(wsplit.DarkenColor(clockColor, 0.7))), 3, 3, icon.Width, icon.Height);
                    }
                }

                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(background, 0, 0);
                graphics.CompositingMode = CompositingMode.SourceOver;
            }

            public void PaintAll(Graphics graphics)
            {
                TimeSpan span2;
                ColorSettings settings = ColorSettings.Profile;
                bool flag = false;
                double totalMilliseconds = wsplit.timer.Elapsed.TotalMilliseconds;

                if (((Settings.Profile.FallbackPreference == 1) || (Settings.Profile.FallbackPreference == 2)) && (totalMilliseconds > 0.0))
                {
                    double num2 = Math.Abs(wsplit.timer.driftMilliseconds);

                    if ((num2 > 500.0) && ((num2 / totalMilliseconds) > 5.5555555555555558E-05))
                    {
                        if (Settings.Profile.FallbackPreference == 2)
                            wsplit.timer.useFallback = true;
                        else
                            flag = true;
                    }
                    else if (Settings.Profile.FallbackPreference == 2)
                    {
                        wsplit.timer.useFallback = false;
                    }
                }

                TimeSpan elapsed = wsplit.timer.Elapsed;
                double num3 = wsplit.split.SegDelta(elapsed.TotalSeconds, wsplit.split.LiveIndex);
                double secs = wsplit.split.RunDelta(elapsed.TotalSeconds, wsplit.split.LiveIndex);
                int num5 = 0;
                float angle;
                if (Settings.Profile.HGrad)
                {
                    angle = 0f;
                }
                else
                {
                    angle = 270f;
                };

                if (wsplit.currentDispMode == DisplayMode.Wide)
                {
                    num5 = wsplit.displaySegsWide();
                    //angle = 90f;
                }
                else
                    num5 = wsplit.detailSegCount();

                if ((wsplit.split.StartDelay != 0) && (wsplit.timer.ElapsedTicks == 0L))
                {
                    span2 = TimeSpan.FromMilliseconds(wsplit.split.StartDelay);
                    if (wsplit.startDelay != null)
                    {
                        span2 -= DateTime.UtcNow - wsplit.offsetStartTime;
                    }
                }
                else if (wsplit.split.Done)
                    span2 = TimeSpan.FromSeconds(wsplit.split.LastSegment.LiveTime);
                else
                    span2 = elapsed;

                if (span2.TotalHours >= 100.0)
                {
                    clockTimeAbsSize = MeasureTimeStringMax("888:88:88", (Settings.Profile.DigitalClock) ? wsplit.digitLarge : wsplit.clockLarge, graphics);
                    timeStringAbsPart = string.Format("{0:000}:{1:00}:{2:00}", Math.Floor(span2.TotalHours) % 1000.0, span2.Minutes, span2.Seconds);
                }
                else
                {
                    clockTimeAbsSize = MeasureTimeStringMax("88:88:88", (Settings.Profile.DigitalClock) ? wsplit.digitLarge : wsplit.clockLarge, graphics);
                    if (span2.TotalHours >= 1.0)
                        timeStringAbsPart = $"{Math.Floor(span2.TotalHours):0}:{span2.Minutes:00}:{span2.Seconds:00}";
                    else if (span2.TotalMinutes >= 1.0)
                        timeStringAbsPart = $"{span2.Minutes}:{span2.Seconds:00}";
                    else
                        timeStringAbsPart = $"{span2.Seconds}";
                }

                if (Settings.Profile.DigitalClock)
                {
                    // If the refresh interval is greater than 42, only 1 digit is shown after the decimal
                    /*if (wsplit.stopwatch.Interval > 42)
                        this.timeStringAbsPart = this.timeStringAbsPart.PadLeft(9, ' ');*/

                    timeStringAbsPart = timeStringAbsPart.PadLeft(8, ' ');
                    if (((wsplit.split.StartDelay != 0) && (wsplit.timer.ElapsedTicks == 0L)) && (timeStringAbsPart.Substring(0, 1) == " "))
                    {
                        timeStringAbsPart = "-" + timeStringAbsPart.Substring(1, timeStringAbsPart.Length - 1);
                    }
                }
                else if (((wsplit.split.StartDelay != 0) && (wsplit.timer.ElapsedTicks == 0L)) && ((span2.TotalHours < 10.0) || ((span2.TotalHours < 100.0) && (wsplit.stopwatch.Interval > 42))))
                    timeStringAbsPart = "-" + timeStringAbsPart;

                if (Settings.Profile.ShowMilliseconds)
                {
                    clockTimeDecSize = MeasureTimeStringMax("88", (Settings.Profile.DigitalClock) ? wsplit.digitMed : wsplit.clockMed, graphics);
                    timeStringDecPart = string.Format("{0:00}", Math.Floor((double)(span2.Milliseconds / 10.0)));
                }
                else
                {
                    // TODO: Remove the extra space when no milliseconds are not displayed
                    clockTimeDecSize = new SizeF(
                        0, Settings.Profile.DigitalClock ? wsplit.digitMed.Height : wsplit.clockMed.Height);
                    timeStringDecPart = string.Empty;
                }

                bool forceTenthsDisplay = span2.TotalHours >= 100.0 || wsplit.stopwatch.Interval > 42;
                // If the number of hours is greater or equal to 100 or the refresh interval is greater than 42,
                // show only 1 digit after the decimal
                if (Settings.Profile.DecisecondsOnly || forceTenthsDisplay)
                {
                    clockTimeDecSize = MeasureTimeStringMax("8", (Settings.Profile.DigitalClock) ? wsplit.digitMed : wsplit.clockMed, graphics);
                    timeStringDecPart = string.Format("{0:0}", Math.Floor((double)(span2.Milliseconds / 100.0)));
                }

                clockTimeTotalSize = new SizeF(clockTimeAbsSize.Width + clockTimeDecSize.Width,
                    Math.Max(clockTimeAbsSize.Height, clockTimeDecSize.Height));

                int x = 0;

                if (Settings.Profile.SegmentIcons >= 1)
                    x = (Settings.Profile.SegmentIcons + 1) * 8;

                int num8 = 0;

                if (((x > 16) && (wsplit.currentDispMode == DisplayMode.Compact)) && (wsplit.split.LiveRun && !wsplit.split.Done))
                    num8 = x + 6;

                //clockScale = Math.Min((float)(((float)(wsplit.clockRect.Width - num8)) / 124f), (float)(((float)wsplit.clockRect.Height) / 26f));
                clockScale = Math.Min((wsplit.clockRect.Width - num8) / clockTimeTotalSize.Width, wsplit.clockRect.Height / clockTimeTotalSize.Height);

                Color clockColor = this.clockColor;
                Color dViewClockColor = new Color();
                if (wsplit.timer.IsRunning)
                {
                    if (wsplit.split.LiveRun)
                    {
                        if (wsplit.split.Done)
                        {
                            if ((wsplit.split.LastSegment.LiveTime < wsplit.split.CompTime(wsplit.split.LastIndex)) || (wsplit.split.CompTime(wsplit.split.LastIndex) == 0.0))
                            {
                                clockColor = ColorSettings.Profile.RecordFore;
                                clockGrColor = ColorSettings.Profile.RecordBack;
                                clockGrColor2 = ColorSettings.Profile.RecordBack2;
                                clockPlainColor = ColorSettings.Profile.RecordBackPlain;

                                dViewClockColor = ColorSettings.Profile.UsedDViewRecord;
                            }
                            else
                            {
                                clockColor = ColorSettings.Profile.FinishedFore;
                                clockGrColor = ColorSettings.Profile.FinishedBack;
                                clockGrColor2 = ColorSettings.Profile.FinishedBack2;
                                clockPlainColor = ColorSettings.Profile.FinishedBackPlain;

                                dViewClockColor = ColorSettings.Profile.UsedDViewFinished;
                            }
                        }
                        else if (wsplit.flashDelay != null)
                        {
                            clockColor = ColorSettings.Profile.Flash;
                            dViewClockColor = ColorSettings.Profile.DViewFlash;
                        }
                        else if (wsplit.split.CompTime() == 0.0)
                        {
                            clockColor = ColorSettings.Profile.AheadFore;
                            clockGrColor = ColorSettings.Profile.AheadBack;
                            clockGrColor2 = ColorSettings.Profile.AheadBack2;
                            clockPlainColor = ColorSettings.Profile.AheadBackPlain;

                            dViewClockColor = ColorSettings.Profile.UsedDViewAhead;
                        }
                        else if (elapsed.TotalSeconds < wsplit.split.CompTime())
                        {
                            if (num3 < 0.0)
                            {
                                clockColor = ColorSettings.Profile.AheadFore;
                                clockGrColor = ColorSettings.Profile.AheadBack;
                                clockGrColor2 = ColorSettings.Profile.AheadBack2;
                                clockPlainColor = ColorSettings.Profile.AheadBackPlain;

                                dViewClockColor = ColorSettings.Profile.UsedDViewAhead;
                            }
                            else
                            {
                                clockColor = ColorSettings.Profile.AheadLosingFore;
                                clockGrColor = ColorSettings.Profile.AheadLosingBack;
                                clockGrColor2 = ColorSettings.Profile.AheadLosingBack2;
                                clockPlainColor = ColorSettings.Profile.AheadLosingBackPlain;

                                dViewClockColor = ColorSettings.Profile.UsedDViewAheadLosing;
                            }
                        }
                        else if (num3 < 0.0)
                        {
                            clockColor = ColorSettings.Profile.BehindFore;
                            clockGrColor = ColorSettings.Profile.BehindBack;
                            clockGrColor2 = ColorSettings.Profile.BehindBack2;
                            clockPlainColor = ColorSettings.Profile.BehindBackPlain;

                            dViewClockColor = ColorSettings.Profile.UsedDViewBehind;
                        }
                        else
                        {
                            clockColor = ColorSettings.Profile.BehindLosingFore;
                            clockGrColor = ColorSettings.Profile.BehindLosingBack;
                            clockGrColor2 = ColorSettings.Profile.BehindLosingBack2;
                            clockPlainColor = ColorSettings.Profile.BehindLosingBackPlain;

                            dViewClockColor = ColorSettings.Profile.UsedDViewBehindLosing;
                        }
                    }
                    else
                    {
                        clockColor = ColorSettings.Profile.WatchFore;
                        clockGrColor = ColorSettings.Profile.WatchBack;
                        clockGrColor2 = ColorSettings.Profile.WatchBack2;
                        clockPlainColor = ColorSettings.Profile.WatchBackPlain;
                    }
                }
                else if (wsplit.timer.ElapsedTicks > 0L)
                {
                    clockColor = ColorSettings.Profile.Paused;
                    dViewClockColor = ColorSettings.Profile.UsedDViewPaused;
                }
                else if (wsplit.split.StartDelay != 0)
                {
                    clockColor = ColorSettings.Profile.DelayFore;
                    clockGrColor = ColorSettings.Profile.DelayBack;
                    clockGrColor2 = ColorSettings.Profile.DelayBack2;
                    clockPlainColor = ColorSettings.Profile.DelayBackPlain;

                    dViewClockColor = ColorSettings.Profile.UsedDViewDelay;
                }
                else if (wsplit.split.LiveRun)
                {
                    clockColor = ColorSettings.Profile.AheadFore;
                    clockGrColor = ColorSettings.Profile.AheadBack;
                    clockGrColor2 = ColorSettings.Profile.AheadBack2;
                    clockPlainColor = ColorSettings.Profile.AheadBackPlain;

                    dViewClockColor = ColorSettings.Profile.UsedDViewAhead;
                }
                else
                {
                    clockColor = ColorSettings.Profile.WatchFore;
                    clockGrColor = ColorSettings.Profile.WatchBack;
                    clockGrColor2 = ColorSettings.Profile.WatchBack2;
                    clockPlainColor = ColorSettings.Profile.WatchBackPlain;

                    dViewClockColor = ColorSettings.Profile.UsedDViewAhead;
                }

                if (clockColor != this.clockColor)
                {
                    RequestBackgroundRedraw();
                    this.clockColor = clockColor;
                }

                Brush brush = new SolidBrush(this.clockColor);
                wsplit.dview.clockColor = new SolidBrush(dViewClockColor);

                if (span2.TotalHours >= 100.0)
                    wsplit.dview.clockText = string.Format("{0:000}:{1:00}:{2:00.00}", Math.Floor((double)(span2.TotalHours % 1000.0)), span2.Minutes, span2.Seconds + (Math.Floor((double)(span2.Milliseconds / 10f)) / 100.0));
                else if (span2.TotalHours >= 1.0)
                    wsplit.dview.clockText = string.Format("{0:0}:{1:00}:{2:00.00}", Math.Floor((double)(span2.TotalHours % 1000.0)), span2.Minutes, span2.Seconds + (Math.Floor((double)(span2.Milliseconds / 10f)) / 100.0));
                else
                    wsplit.dview.clockText = string.Format("{0:00}:{1:00.00}", span2.Minutes, span2.Seconds + (Math.Floor((double)(span2.Milliseconds / 10f)) / 100.0));

                wsplit.dview.Invalidate();

                Rectangle layoutRectangle = new Rectangle();
                Rectangle rectangle2 = new Rectangle();
                StringFormat format = new StringFormat();
                StringFormat format2 = new StringFormat();

                string text = "";
                string str4 = "";

                double num10 = 0.0;
                Brush white = Brushes.White;

                if (wsplit.currentDispMode != DisplayMode.Timer)
                {
                    if (wsplit.currentDispMode == DisplayMode.Wide)
                    {
                        layoutRectangle = new Rectangle(wsplit.Width - 119, 2, 119, wsplit.Height / 2);
                        rectangle2 = new Rectangle(wsplit.Width - 119, layoutRectangle.Bottom - 2, 119, wsplit.Height / 2);
                    }
                    else if (wsplit.currentDispMode == DisplayMode.Detailed)
                    {
                        layoutRectangle = new Rectangle(0, 0, 0, 0);
                        rectangle2 = new Rectangle(1, wsplit.clockRect.Bottom + 2, wsplit.Width - 1, 16);
                    }
                    else
                    {
                        rectangle2 = new Rectangle(1, 2, wsplit.Width - 1, 13);
                        layoutRectangle = new Rectangle(1, wsplit.clockRect.Bottom + 2, wsplit.Width - 1, 14);
                        if ((Settings.Profile.SegmentIcons > 0) && !wsplit.split.Done)
                        {
                            rectangle2.Width -= 2 + x;
                            rectangle2.X += 2 + x;
                        }
                    }

                    format.LineAlignment = StringAlignment.Center;
                    format.Trimming = StringTrimming.EllipsisCharacter;
                    format.Alignment = StringAlignment.Far;
                    format2 = (StringFormat)format.Clone();

                    if (wsplit.currentDispMode == DisplayMode.Compact)
                        format2.Alignment = StringAlignment.Near;

                    if (num3 <= 0.0)
                        segLosingTime = false;

                    if (wsplit.timer.ElapsedTicks != 0L)
                    {
                        if (wsplit.split.Done)
                        {
                            if (wsplit.split.CompTime(wsplit.split.LastIndex) != 0.0)
                            {
                                num10 = wsplit.split.SegDelta(wsplit.split.LastSegment.LiveTime, wsplit.split.LastIndex);
                                text = wsplit.timeFormatter(num10, TimeFormat.DeltaShort);
                            }
                        }
                        // If we are losing time on the current segment...
                        else if (num3 > 0.0)
                        {
                            num10 = num3;   // The number to be written becomes the current segment delta
                            text = wsplit.timeFormatter(num10, TimeFormat.DeltaShort);

                            // If we just started losing time, we indicate we are and we ask for a redraw of the timer background since color has changed
                            if (!segLosingTime)
                            {
                                segLosingTime = true;
                                RequestBackgroundRedraw();
                            }
                        }

                        // If we aren't losing time on the current segment and if the current segment isn't the first segment...
                        else if (wsplit.split.LiveIndex > 0)
                        {
                            // The number to be written becomes the previous segment delta:
                            num10 = wsplit.split.SegDelta(wsplit.split.segments[wsplit.split.LiveIndex - 1].LiveTime, wsplit.split.LiveIndex - 1);

                            // Though, if the previous split was skipped or if the previous split had no time, we don't write anything...
                            // wsplit will probably change.
                            if ((wsplit.split.segments[wsplit.split.LiveIndex - 1].LiveTime > 0.0) && (wsplit.split.CompTime(wsplit.split.LiveIndex - 1) != 0.0))
                            {
                                text = wsplit.timeFormatter(num10, TimeFormat.DeltaShort);
                            }
                        }
                    }

                    // If we're not in the Detailed display mode
                    if (wsplit.currentDispMode != DisplayMode.Detailed)
                    {
                        // If splits are done
                        if (wsplit.split.Done)
                        {
                            if (wsplit.split.CompTime(wsplit.split.LastIndex) != 0.0)
                            {
                                double num11 = wsplit.split.RunDeltaAt(wsplit.split.LastIndex);
                                str4 = wsplit.timeFormatter(num11, TimeFormat.Delta);
                                if (num11 < 0.0)
                                {
                                    white = new SolidBrush(ColorSettings.Profile.RecordFore);
                                }
                                else
                                {
                                    white = new SolidBrush(ColorSettings.Profile.FinishedFore);
                                }
                            }
                        }

                        // If we are losing time on the current segment...
                        else if (num3 > 0.0)
                        {
                            // Format the run delta in str4
                            str4 = wsplit.timeFormatter(secs, TimeFormat.Delta);

                            if (secs < 0.0) // If ahead overall | Is wsplit part of the code even useful?
                            {
                                white = new SolidBrush(ColorSettings.Profile.SegAheadGain);
                            }
                            else            // If behind overall
                            {
                                white = new SolidBrush(ColorSettings.Profile.SegBehindLoss);
                            }
                        }

                        // If there is a previous split time and there is a time to compare it to...
                        else if (((wsplit.split.LiveIndex > 0) && (wsplit.split.segments[wsplit.split.LiveIndex - 1].LiveTime > 0.0)) && (wsplit.split.CompTime(wsplit.split.LiveIndex - 1) != 0.0))
                        {
                            // Stores the run delta at the previous split in num12
                            double num12 = wsplit.split.RunDeltaAt(wsplit.split.LiveIndex - 1);
                            str4 = wsplit.timeFormatter(num12, TimeFormat.Delta);
                            if (num12 < 0.0)
                            {
                                white = new SolidBrush(ColorSettings.Profile.SegAheadLoss);
                            }
                            else
                            {
                                white = new SolidBrush(ColorSettings.Profile.SegBehindGain);
                            }
                        }
                    }

                    if (text.Length != segDelLength)
                    {
                        segDelLength = text.Length;
                        segDeltaWidth = MeasureDisplayStringWidth(text, wsplit.timeFont);
                        RequestBackgroundRedraw();
                    }

                    if (str4.Length != runDelLength)
                    {
                        runDelLength = str4.Length;
                        runDeltaWidth = MeasureDisplayStringWidth(str4, wsplit.timeFont);
                        RequestBackgroundRedraw();
                    }
                }

                int num13 = 0;
                int num14 = (wsplit.split.LiveIndex - num5) + 2;
                int liveIndex = wsplit.split.LiveIndex;
                if (num5 >= 2)
                {
                    liveIndex--;
                }
                if (((wsplit.currentDispMode == DisplayMode.Detailed) && Settings.Profile.ShowLastDetailed) || ((wsplit.currentDispMode == DisplayMode.Wide) && Settings.Profile.ShowLastWide))
                {
                    num14++;
                }
                num13 = Math.Max(0, Math.Min(liveIndex, Math.Min((wsplit.split.LastIndex - num5) + 1, num14)));
                Rectangle rectangle3 = new Rectangle();
                Color timeColor = wsplit.split.CurrentSegment.TimeColor;
                string timeString = wsplit.split.CurrentSegment.TimeString;

                // If in wide or detailed desplay mode and if not done...
                if (((wsplit.currentDispMode == DisplayMode.Wide) || (wsplit.currentDispMode == DisplayMode.Detailed)) && !wsplit.split.Done)
                {
                    runLosingTime = false; // Not losing time... ?

                    // If there is a time to compare to and one current segment delta or run delta is greater than 0.0
                    if ((wsplit.split.CompTime() > 0.0) && ((num3 > 0.0) || (secs > 0.0)))
                    {
                        runLosingTime = true;  // Losing time...
                        // Formats run delta in timeString:
                        //timeString = wsplit.timeFormatter(secs, TimeFormat.Delta);
                    }
                    // If losing time and in detailed display mode, and if Segment height (so Icon size) is smaller or equal to 24 pixels and length has changed...
                    if ((runLosingTime && (wsplit.currentDispMode == DisplayMode.Detailed)) && ((wsplit.segHeight <= 0x18) && (timeString.Length != segTimeLength)))
                    {
                        segTimeLength = timeString.Length;
                        segTimeWidth = MeasureDisplayStringWidth(timeString, wsplit.timeFont);
                        RequestBackgroundRedraw();
                    }

                    // If in the Wide display mode...
                    if (wsplit.currentDispMode == DisplayMode.Wide)
                        rectangle3 = new Rectangle((wsplit.clockRect.Right + (wsplit.wideSegWidth * (wsplit.split.LiveIndex - num13))) + 4, wsplit.Height / 2, (wsplit.wideSegWidth - x) - 2, wsplit.Height / 2);
                    // Otherwise...
                    else
                    {
                        // Builds a rectangle for the live segment time
                        rectangle3 = new Rectangle(x, wsplit.segHeight * (wsplit.split.LiveIndex - num13), wsplit.Width - x, wsplit.segHeight);
                        // Moves the rectangle down if we have to show the run title

                        if (((wsplit.split.RunTitle != "") && Settings.Profile.ShowTitle) && ((wsplit.split.RunGoal != "") && Settings.Profile.ShowGoal))
                        {
                            rectangle3.Y += 0x20;
                        }
                        else if ((wsplit.split.RunTitle != "") && Settings.Profile.ShowTitle)
                        {
                            rectangle3.Y += 0x12;
                        }
                        else if ((wsplit.split.RunGoal != "") && Settings.Profile.ShowGoal)
                        {
                            rectangle3.Y += 0x12;
                        }
                        // If greater than 24 pixels icons
                        if (wsplit.segHeight > 0x18)
                        {
                            // Goes to the second half of the rectangle
                            rectangle3.Height /= 2;
                            rectangle3.Y += rectangle3.Height;
                        }
                        // Otherwise, moves down 1 pixel.
                        else
                        {
                            rectangle3.Y++;
                        }
                    }
                }

                // Temporary signature of the method
                DrawBackground(graphics, angle, num5, num8, num13, x);

                // Code for drawing clock fore has been moved to another function. Current method's signature is temporary
                DrawClockFore(graphics, timeStringAbsPart, ref timeStringDecPart, flag, num8, ref brush);

                if (wsplit.currentDispMode != DisplayMode.Timer)
                {
                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                    //graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    // At this point...
                    // num3 -> Current segment delta
                    // num10 -> What has to be written in the status bar

                    // If the time to write in the status bar is a new best segment...
                    /* apparently this always runs m8 */
                    if (Settings.Profile.ShowPrevSeg)
                    {
                        if (!segLosingTime && wsplit.split.LiveIndex > 0 &&
                            wsplit.split.LiveSegment(wsplit.split.LiveIndex - 1) != 0.0 && (wsplit.split.segments[wsplit.split.LiveIndex - 1].BestSegment == 0.0 ||
                            wsplit.split.LiveSegment(wsplit.split.LiveIndex - 1) < wsplit.split.segments[wsplit.split.LiveIndex - 1].BestSegment))
                        {
                            if (Settings.Profile.RainbowSplits)
                            {
                                graphics.DrawString(text, wsplit.timeFont, new SolidBrush(ColorSettings.Profile.SegRainbow), rectangle2, format);
                            }
                            else
                            {
                                graphics.DrawString(text, wsplit.timeFont, new SolidBrush(ColorSettings.Profile.SegBestSegment), rectangle2, format);
                            };
                        }
                        // Else, if The time to write in the status bar is a time loss...
                        else if (num10 > 0.0)
                        {
                            graphics.DrawString(text, wsplit.timeFont, new SolidBrush(ColorSettings.Profile.SegBehindLoss), rectangle2, format);
                        }
                        // Otherwise (the time is a time gain but not a best segment), or there is not time to write...
                        else
                        {
                            graphics.DrawString(text, wsplit.timeFont, new SolidBrush(ColorSettings.Profile.SegAheadGain), rectangle2, format);
                        }
                    };
                    if (wsplit.currentDispMode != DisplayMode.Detailed)
                    {
                        graphics.DrawString(str4, wsplit.timeFont, white, layoutRectangle, format2);
                    }
                }
                if (((wsplit.currentDispMode == DisplayMode.Wide) || (wsplit.currentDispMode == DisplayMode.Detailed)) && !wsplit.split.Done)
                {
                    StringFormat format9 = new StringFormat
                    {
                        LineAlignment = StringAlignment.Center
                    };

                    if (wsplit.currentDispMode == DisplayMode.Wide)
                        format9.Alignment = StringAlignment.Near;
                    else
                        format9.Alignment = StringAlignment.Far;

                    graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                    //graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    graphics.DrawString(timeString, wsplit.timeFont, new SolidBrush(timeColor), rectangle3, format9);
                }
            }

            private void DrawClockBack(float angle, int num8, Graphics bgGraphics)
            {
                if (!Settings.Profile.BackgroundBlack)
                {
                    if (Settings.Profile.BackgroundPlain)
                        bgGraphics.FillRectangle(new SolidBrush(clockPlainColor), wsplit.clockRect);
                    else
                    {
                        bgGraphics.FillRectangle(new LinearGradientBrush(wsplit.clockRect, clockGrColor, clockGrColor2, angle), wsplit.clockRect);
                        if ((angle == 0f) && Settings.Profile.ClockAccent)
                            bgGraphics.FillRectangle(new SolidBrush(Color.FromArgb(0x56, clockGrColor2)), wsplit.clockRect.X, wsplit.clockRect.Y, wsplit.clockRect.Width, wsplit.clockRect.Height / 2);

                        if (Settings.Profile.DigitalClock)
                        {
                            bgGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            bgGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                            bgGraphics.SmoothingMode = SmoothingMode.HighQuality;

                            // Change according to what was done in that DrawClockFore method
                            //bgGraphics.TranslateTransform(num8 + (((wsplit.clockRect.Width - (clockScale * 124f)) - num8) / 2f), wsplit.clockRect.Top + ((wsplit.clockRect.Height - (clockScale * 26f)) / 2f));
                            //bgGraphics.ScaleTransform(clockScale, clockScale);

                            bgGraphics.TranslateTransform(num8 + ((wsplit.clockRect.Width - (clockScale * clockTimeTotalSize.Width) - num8) / 2f),
                                wsplit.clockRect.Top + ((wsplit.clockRect.Height - (clockScale * clockTimeTotalSize.Height)) / 2f));
                            bgGraphics.ScaleTransform(clockScale, clockScale);

                            Brush brush4 = new SolidBrush(Color.FromArgb(86, clockGrColor2));
                            bgGraphics.DrawString("88:88:88".PadLeft(timeStringAbsPart.Length, '8'), wsplit.digitLarge, brush4, 0f, 0.15f);

                            if (Settings.Profile.ShowDecimalSeparator && Settings.Profile.ShowMilliseconds)
                            {
                                bgGraphics.DrawString(wsplit.decimalChar, wsplit.digitMed, brush4, (112 - clockTimeDecSize.Width), 4.5f);
                                bgGraphics.DrawString("".PadRight(timeStringDecPart.Length, '8'), wsplit.digitMed, brush4, (119 - clockTimeDecSize.Width), 4.5f);
                            }
                            else
                                bgGraphics.DrawString("".PadRight(timeStringDecPart.Length, '8'), wsplit.digitMed, brush4, (117.5f - clockTimeDecSize.Width), 4.5f);

                            bgGraphics.ResetTransform();
                        }
                    }
                }
            }

            // Current methods signature is temporary. Most of the parameters will temporary become class members or properties.
            private void DrawClockFore(Graphics graphics, string timeStringAbsPart, ref string timeStringDecPart, bool inaccuraciesDetected, int num8, ref Brush brush)
            {
                // Transforms the Graphics object in order to draw the clock time full-sized
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

                //graphics.TranslateTransform(num8 + (((wsplit.clockRect.Width - (clockScale * 124f)) - num8) / 2f), wsplit.clockRect.Top + ((wsplit.clockRect.Height - (clockScale * 26f)) / 2f));
                graphics.TranslateTransform(num8 + ((wsplit.clockRect.Width - (clockScale * clockTimeTotalSize.Width) - num8) / 2f),
                    wsplit.clockRect.Top + ((wsplit.clockRect.Height - (clockScale * clockTimeTotalSize.Height)) / 2f));
                graphics.ScaleTransform(clockScale, clockScale);

                // If using DigitalClock font
                if (Settings.Profile.DigitalClock)
                {
                    // The drawing of the digital clock font could have been done dynamically, but since the result wasn't
                    // exactly centered vertically, I decided to hardcode values that give the best looking result.
                    // Since this option is used by many, I had to make sure it looked nice.
                    graphics.DrawString(timeStringAbsPart, wsplit.digitLarge, brush, 0f, 0.15f);

                    if (inaccuraciesDetected)
                    {
                        timeStringDecPart = "".PadRight(timeStringDecPart.Length, '?');
                        brush = new SolidBrush(ColorSettings.Profile.Flash);
                    }

                    if (Settings.Profile.ShowDecimalSeparator && Settings.Profile.ShowMilliseconds)
                    {
                        graphics.DrawString(wsplit.decimalChar, wsplit.digitMed, brush, clockTimeAbsSize.Width - 7, 4.5f);
                        graphics.DrawString(timeStringDecPart, wsplit.digitMed, brush, clockTimeAbsSize.Width, 4.5f);
                        /*graphics.DrawString(wsplit.decimalChar, wsplit.digitMed, brush, (112 - clockTimeDecSize.Width), 4.5f);
                        graphics.DrawString(timeStringDecPart, wsplit.digitMed, brush, (119 - clockTimeDecSize.Width), 4.5f);*/
                    }
                    else
                        graphics.DrawString(timeStringDecPart, wsplit.digitMed, brush, (clockTimeAbsSize.Width), 4.5f);
                    //graphics.DrawString(timeStringDecPart, wsplit.digitMed, brush, (117.5f - clockTimeDecSize.Width), 4.5f);
                }

                // If the font used for the clock is any font but the default digital clock font, the display is done dynamically, according to font measurements.
                else
                {
                    // Calculates de relative baseline height of both fonts used in the display so that they can be aligned correctly
                    float largeBaseline = wsplit.clockLarge.Size * wsplit.clockLarge.FontFamily.GetCellAscent(wsplit.clockLarge.Style) / wsplit.clockLarge.FontFamily.GetEmHeight(wsplit.clockLarge.Style);
                    float mediumBaseline = wsplit.clockMed.Size * wsplit.clockMed.FontFamily.GetCellAscent(wsplit.clockMed.Style) / wsplit.clockMed.FontFamily.GetEmHeight(wsplit.clockMed.Style);

                    RectangleF clockTimeAbsRectF = new RectangleF(0f, 0f, clockTimeAbsSize.Width, clockTimeAbsSize.Height);
                    RectangleF clockTimeDecRectF;

                    if (Settings.Profile.ShowDecimalSeparator && Settings.Profile.ShowMilliseconds)
                    {
                        clockTimeDecRectF = new RectangleF(clockTimeAbsRectF.Right - 7, clockTimeAbsRectF.Top + (largeBaseline - mediumBaseline), clockTimeDecSize.Width, clockTimeDecSize.Height);

                        graphics.DrawString(wsplit.decimalChar, wsplit.clockMed, brush, clockTimeDecRectF);
                        clockTimeDecRectF.X = clockTimeAbsRectF.Right;
                    }
                    else
                        clockTimeDecRectF = new RectangleF(clockTimeAbsRectF.Right - 2, clockTimeAbsRectF.Top + (largeBaseline - mediumBaseline), clockTimeDecSize.Width, clockTimeDecSize.Height);

                    StringFormat format = new StringFormat { Alignment = StringAlignment.Far };
                    graphics.DrawString(timeStringAbsPart, wsplit.clockLarge, brush, clockTimeAbsRectF, format);

                    if (inaccuraciesDetected)
                    {
                        timeStringDecPart = "".PadRight(timeStringDecPart.Length, '?');
                        brush = new SolidBrush(ColorSettings.Profile.Flash);
                    }

                    format.Alignment = StringAlignment.Near;
                    graphics.DrawString(timeStringDecPart, wsplit.clockMed, brush, clockTimeDecRectF, format);
                }

                graphics.ResetTransform();
            }

            private static SizeF MeasureTimeStringMax(string timeString, Font font, Graphics graphics)
            {
                SizeF max = new SizeF(0, font.Height);
                for (int i = 0; i <= 9; ++i)
                {
                    SizeF temp = graphics.MeasureString(timeString.Replace('8', (char)(i + '0')), font);
                    if (temp.Width > max.Width)
                        max.Width = temp.Width;
                }

                return max;
            }
        }
    }
}