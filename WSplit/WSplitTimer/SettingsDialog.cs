using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WSplitTimer.Properties;

namespace WSplitTimer
{
    public partial class SettingsDialog : Form
    {
        private WSplit wsplit;

        private readonly List<Panel> panelList = new List<Panel>();
        private int activePanel = -1;

        private readonly List<Keys> hotkeyList = new List<Keys>();
        private int selectedHotkeyIndex;
        private Keys newHotkey;

        private readonly string[] fontNames = FontFamily.Families.Select(f => f.Name).ToArray();

        private BackgroundImageDialog backgroundImageDialog;

        private BackgroundImageDialog BackgroundImageDialog
        {
            get
            {
                if (backgroundImageDialog == null)
                    backgroundImageDialog = new BackgroundImageDialog();
                return backgroundImageDialog;
            }
        }

        protected override CreateParams CreateParams
        {
            // Overriding this property as done here gets rid of graphical artifacts
            // that occur when many controls are updated at once.
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public void OpenCustomBackgroundMenuItem()
        {
            // Shows the backgroundImageDialog and, if OK is clicked, apply the settings
            if (BackgroundImageDialog.ShowDialog(this, wsplit) == DialogResult.OK)
            {
                BackgroundImageDialog.ApplyChanges();
                BackgroundSettingsChanged = true;
            }
        }

        public string StartDelay
        {
            get { return textBoxStartDelay.Text; }
            set { textBoxStartDelay.Text = value; }
        }

        public int DetailedWidth
        {
            get { return (int)numericUpDownDetailedWidth.Value; }
            set { numericUpDownDetailedWidth.Value = value; }
        }

        public bool BackgroundSettingsChanged
        {
            get;
            private set;
        }

        private int ActivePanel
        {
            get { return activePanel; }
            set
            {
                activePanel = value;
                panelList[activePanel].BringToFront();
            }
        }

        public SettingsDialog()
        {
            InitializeComponent();

            // Setting up list view and panels:
            panelList.Add(panelGeneralOptions);
            panelList.Add(panelHotkeys);
            panelList.Add(panelFontSettings);
            panelList.Add(panelDisplaySettings);

            // Setting up other controls:
            hotkeyList.AddRange(new Keys[8]);

            listViewHotkeys.BeginUpdate();
            for (int i = 0; i < hotkeyList.Count; ++i)
                listViewHotkeys.Items[i].SubItems.Add("");
            listViewHotkeys.EndUpdate();

            comboBoxPrimWndFont.BeginUpdate();
            comboBoxPrimWndFont.Items.AddRange(fontNames);
            comboBoxPrimWndFont.EndUpdate();

            comboBoxDViewFont.BeginUpdate();
            comboBoxDViewFont.Items.AddRange(fontNames);
            comboBoxDViewFont.EndUpdate();
        }

        // Custom ShowDialog method, that will populate the settings before calling the default method
        public DialogResult ShowDialog(WSplit wsplit, int startupPanel)
        {
            this.wsplit = wsplit;

            ListView_SetItemSpacing(listViewPanelSelector, (short)listViewPanelSelector.ClientSize.Width, 76);

            // Moves the wanted panel on top of the others
            listViewPanelSelector.Items[startupPanel].Selected = true;
            ActivePanel = startupPanel;

            // Tell that, so far, there were no change in the background settings:
            BackgroundSettingsChanged = false;

            // If for some reason, a value is not compatible with WSplit, the settings
            // will automatically be brought back to default.
            try
            {
                PopulateSettings();
            }
            catch (Exception)   // Any kind of exception
            {
                RestoreDefaults();
                MessageBoxEx.Show(this,
                    "An error has occurred and your settings were brought back to defaults.",
                    "Defaults Restored", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return base.ShowDialog(wsplit);
        }

        private void PopulateSettings()
        {
            //
            // Initializing controls with application global settings
            //
            // General options:
            trackBarDoubleTap.Value = Settings.Profile.DoubleTapGuard / 50;
            UpdateDoubleTapDelayDisplay();
            trackBarRefreshInterval.Value = Settings.Profile.RefreshRate;
            UpdateRefreshIntervalDisplay();
            comboBoxFallback.SelectedIndex = Settings.Profile.FallbackPreference;
            checkBoxWindowPos.Checked = Settings.Profile.SaveWindowPos;
            checkBoxReloadRun.Checked = Settings.Profile.LoadMostRecent;
            checkBox1.Checked = Settings.Profile.RainbowSplits;
            checkBox2.Checked = Settings.Profile.SplitTimes;
            numwv.Value = Settings.Profile.WideHeight;

            // Global hotkeys:
            checkBoxHotkeysEnabled.Checked = Settings.Profile.EnabledHotkeys;

            hotkeyList[0] = Settings.Profile.SplitKey;
            hotkeyList[1] = Settings.Profile.PauseKey;
            hotkeyList[2] = Settings.Profile.StopKey;
            hotkeyList[3] = Settings.Profile.ResetKey;
            hotkeyList[4] = Settings.Profile.PrevKey;
            hotkeyList[5] = Settings.Profile.NextKey;
            hotkeyList[6] = Settings.Profile.CompTypeKey;
            hotkeyList[7] = Settings.Profile.HotkeyToggleKey;

            listViewHotkeys.BeginUpdate();
            for (int i = 0; i < hotkeyList.Count; ++i)
                listViewHotkeys.Items[i].SubItems[1] = new ListViewItem.ListViewSubItem(listViewHotkeys.Items[i], FormatHotkey(hotkeyList[i]));
            listViewHotkeys.EndUpdate();

            selectedHotkeyIndex = 0;
            listViewHotkeys.Items[selectedHotkeyIndex].Selected = true;

            // Font settings:
            comboBoxPrimWndFont.SelectedItem =
                (fontNames.Any(f => f == Settings.Profile.FontFamilySegments)) ? Settings.Profile.FontFamilySegments : FontFamily.GenericSansSerif.Name;
            comboBoxDViewFont.SelectedItem =
                (fontNames.Any(f => f == Settings.Profile.FontFamilyDView)) ? Settings.Profile.FontFamilyDView : FontFamily.GenericSansSerif.Name;

            numericUpDownPrimWndMult.Value = (decimal)Settings.Profile.FontMultiplierSegments;
            checkBoxClockDigitalFont.Checked = Settings.Profile.DigitalClock;

            // If milliseconds is checked, deciseconds box is enabled
            showMilliseconds.Checked = Settings.Profile.ShowMilliseconds;
            decisecondsOnly.Checked = Settings.Profile.DecisecondsOnly;
            decisecondsOnly.Enabled = showMilliseconds.Checked;

            // Display settings:
            trackBarOpacity.Value = (int)(Settings.Profile.Opacity * 100);

            checkBoxShowTitle.Checked = Settings.Profile.ShowTitle;
            checkBoxShowAttemptCount.Checked = Settings.Profile.ShowAttempts;
            comboBoxIcons.SelectedIndex = Settings.Profile.SegmentIcons;

            switch ((WSplit.DisplayMode)Settings.Profile.DisplayMode)
            {
                case WSplit.DisplayMode.Timer: radioButtonDisplayTimer.Checked = true; break;
                case WSplit.DisplayMode.Compact: radioButtonDisplayCompact.Checked = true; break;
                case WSplit.DisplayMode.Wide: radioButtonDisplayWide.Checked = true; break;
                default: radioButtonDisplayDetailed.Checked = true; break;
            }

            checkBoxDetailedBlanks.Checked = Settings.Profile.DisplayBlankSegs;
            checkBoxDetailedShowLast.Checked = Settings.Profile.ShowLastDetailed;
            numericUpDownDetailedSegments.Value = Settings.Profile.DisplaySegs;

            checkBoxWideBlanks.Checked = Settings.Profile.WideSegBlanks;
            checkBoxWideShowLast.Checked = Settings.Profile.ShowLastWide;
            numericUpDownWideSegments.Value = Settings.Profile.WideSegs;
        }

        private void RestoreDefaults()
        {
            Settings.Profile.Reset();
            Settings.Profile.FirstRun = false;
            PopulateSettings();
        }

        public void ApplyChanges()
        {
            // Called manually after dialog is closed.
            // Saves all the control states in the Settings

            // General options:
            Settings.Profile.DoubleTapGuard = trackBarDoubleTap.Value * 50;
            Settings.Profile.RefreshRate = trackBarRefreshInterval.Value;
            Settings.Profile.FallbackPreference = comboBoxFallback.SelectedIndex;
            Settings.Profile.SaveWindowPos = checkBoxWindowPos.Checked;
            Settings.Profile.LoadMostRecent = checkBoxReloadRun.Checked;
            Settings.Profile.RainbowSplits = checkBox1.Checked;
            Settings.Profile.SplitTimes = checkBox2.Checked;
            Settings.Profile.WideHeight = (int)numwv.Value;

            // Global hotkeys:
            Settings.Profile.EnabledHotkeys = checkBoxHotkeysEnabled.Checked;

            Settings.Profile.SplitKey = hotkeyList[0];
            Settings.Profile.PauseKey = hotkeyList[1];
            Settings.Profile.StopKey = hotkeyList[2];
            Settings.Profile.ResetKey = hotkeyList[3];
            Settings.Profile.PrevKey = hotkeyList[4];
            Settings.Profile.NextKey = hotkeyList[5];
            Settings.Profile.CompTypeKey = hotkeyList[6];
            Settings.Profile.HotkeyToggleKey = hotkeyList[7];

            // Font settings:
            Settings.Profile.FontFamilySegments = (string)comboBoxPrimWndFont.SelectedItem;
            Settings.Profile.FontMultiplierSegments = (float)numericUpDownPrimWndMult.Value;
            Settings.Profile.DigitalClock = checkBoxClockDigitalFont.Checked;
            Settings.Profile.ShowMilliseconds = showMilliseconds.Checked;
            Settings.Profile.DecisecondsOnly = decisecondsOnly.Checked;

            Settings.Profile.FontFamilyDView = (string)comboBoxDViewFont.SelectedItem;

            // Display settings:
            Settings.Profile.Opacity = trackBarOpacity.Value / 100.0;

            Settings.Profile.ShowTitle = checkBoxShowTitle.Checked;
            Settings.Profile.ShowAttempts = checkBoxShowAttemptCount.Checked;
            Settings.Profile.SegmentIcons = comboBoxIcons.SelectedIndex;

            if (radioButtonDisplayTimer.Checked)
                Settings.Profile.DisplayMode = (int)WSplit.DisplayMode.Timer;
            else if (radioButtonDisplayCompact.Checked)
                Settings.Profile.DisplayMode = (int)WSplit.DisplayMode.Compact;
            else if (radioButtonDisplayWide.Checked)
                Settings.Profile.DisplayMode = (int)WSplit.DisplayMode.Wide;
            else
                Settings.Profile.DisplayMode = (int)WSplit.DisplayMode.Detailed;

            Settings.Profile.DisplayBlankSegs = checkBoxDetailedBlanks.Checked;
            Settings.Profile.ShowLastDetailed = checkBoxDetailedShowLast.Checked;
            Settings.Profile.DisplaySegs = (int)numericUpDownDetailedSegments.Value;

            Settings.Profile.WideSegBlanks = checkBoxWideBlanks.Checked;
            Settings.Profile.ShowLastWide = checkBoxWideShowLast.Checked;
            Settings.Profile.WideSegs = (int)numericUpDownWideSegments.Value;
        }

        private string FormatHotkey(Keys key)
        {
            string str = "";
            Keys keys = key & Keys.KeyCode;

            if ((keys != Keys.ControlKey) && ((key & Keys.Control) == Keys.Control))
                str += "Ctrl+";

            if ((keys != Keys.ShiftKey) && ((key & Keys.Shift) == Keys.Shift))
                str += "Shift+";

            if ((keys != Keys.Menu) && ((key & Keys.Alt) == Keys.Alt))
                str += "Alt+";

            return (str + keys);
        }

        private void UpdateDoubleTapDelayDisplay()
        {
            if (trackBarDoubleTap.Value == 0)
                labelDoubleTapDisplay.Text = "Off";
            else
                labelDoubleTapDisplay.Text = (trackBarDoubleTap.Value * 50) + " ms";
        }

        private void UpdateRefreshIntervalDisplay()
        {
            labelRefreshIntervalDisplay.Text = trackBarRefreshInterval.Value + " ms";
        }

        private void UpdateOpacityDisplay()
        {
            labelOpacityDisplay.Text = trackBarOpacity.Value + "%";
            wsplit.Opacity = trackBarOpacity.Value / 100.0;
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public void ListView_SetItemSpacing(ListView listView, short leftPadding, short topPadding)
        {
            SendMessage(listView.Handle, 0x1035, IntPtr.Zero, (IntPtr)(((ushort)leftPadding) | (uint)(topPadding << 16)));
        }

        private void listViewPanelSelector_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // If the item that sends the event is not the one that was already selected,
            // we proceed to the switch between the panels by bringing the selected one to the front
            if (ActivePanel != e.ItemIndex)
            {
                ActivePanel = e.ItemIndex;
            }
        }

        private void listViewPanelSelector_MouseUp(object sender, MouseEventArgs e)
        {
            // When the user is done clicking, the selected input is set to the last item
            // that got checked or unchecked. Therefore, if the user clicked on an item,
            // this item stays selected. If the user clicks the background, the last selected
            // item (which got unselected by clicking) is selected again.
            listViewPanelSelector.Items[activePanel].Selected = true;
        }

        private void trackBarDoubleTap_ValueChanged(object sender, EventArgs e)
        {
            UpdateDoubleTapDelayDisplay();
        }

        private void trackBarRefreshInterval_ValueChanged(object sender, EventArgs e)
        {
            UpdateRefreshIntervalDisplay();
        }

        private void listViewHotkeys_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // Same as page selector ListView
            if (e.Item.Selected)
            {
                selectedHotkeyIndex = e.ItemIndex;
                textBoxHotkey.Text = FormatHotkey(hotkeyList[selectedHotkeyIndex]);
            }
        }

        private void listViewHotkeys_MouseUp(object sender, MouseEventArgs e)
        {
            // Same as page selector ListView
            listViewHotkeys.Items[selectedHotkeyIndex].Selected = true;
            textBoxHotkey.Focus();
            textBoxHotkey.Select(0, 0);
        }

        private void textBoxHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            newHotkey = e.KeyData;
            textBoxHotkey.Text = FormatHotkey(newHotkey);
        }

        private void buttonSetHotkey_Click(object sender, EventArgs e)
        {
            hotkeyList[listViewHotkeys.SelectedIndices[0]] = newHotkey;
            listViewHotkeys.Items[listViewHotkeys.SelectedIndices[0]].SubItems[1].Text = FormatHotkey(newHotkey);
        }

        private void buttonClearHotkey_Click(object sender, EventArgs e)
        {
            newHotkey = Keys.None;
            hotkeyList[listViewHotkeys.SelectedIndices[0]] = newHotkey;
            listViewHotkeys.Items[listViewHotkeys.SelectedIndices[0]].SubItems[1].Text = FormatHotkey(newHotkey);
            textBoxHotkey.Text = FormatHotkey(newHotkey);
        }

        private void trackBarOpacity_ValueChanged(object sender, EventArgs e)
        {
            UpdateOpacityDisplay();
        }

        private void buttonDefaults_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx.Show(this, "Are you sure?", "Restore Defaults", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                RestoreDefaults();
            }
        }

        private void textBoxStartDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ':' && e.KeyChar != '.' && e.KeyChar != ',')
                e.Handled = true;
        }

        private void textBoxStartDelay_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(textBoxStartDelay.Text, "[^0-9:.,]"))
                textBoxStartDelay.Text = Regex.Replace(textBoxStartDelay.Text, "[^0-9:.,]", "");
        }

        private void buttonBackgroundImage_Click(object sender, EventArgs e)
        {
            // Shows the backgroundImageDialog and, if OK is clicked, apply the settings
            if (BackgroundImageDialog.ShowDialog(this, wsplit) == DialogResult.OK)
            {
                BackgroundImageDialog.ApplyChanges();
                BackgroundSettingsChanged = true;
            }
        }

        private void showMilliseconds_CheckedChanged(object sender, EventArgs e)
        {
            decisecondsOnly.Enabled = showMilliseconds.Checked;
            if (!showMilliseconds.Checked)
            {
                decisecondsOnly.Checked = false;
            }
        }

        private void onlyDeciseconds_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}