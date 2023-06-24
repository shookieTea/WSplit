using System;
using System.Windows.Forms;
using WSplitTimer.Properties;

namespace WSplitTimer
{
    public partial class DViewSetColumnsDialog : Form
    {
        public DViewSetColumnsDialog()
        {
            InitializeComponent();
            checkBoxOldTime.Checked = Settings.Profile.DViewShowOld;
            checkBoxBestTime.Checked = Settings.Profile.DViewShowBest;
            checkBoxSumOfBests.Checked = Settings.Profile.DViewShowSumOfBests;

            checkBoxAlwaysShowComp.Checked = Settings.Profile.DViewShowComp;
            checkBoxLiveTime.Checked = Settings.Profile.DViewShowLive;
            checkBoxLiveDelta.Checked = Settings.Profile.DViewShowDeltas;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Settings.Profile.DViewShowOld = checkBoxOldTime.Checked;
            Settings.Profile.DViewShowBest = checkBoxBestTime.Checked;
            Settings.Profile.DViewShowSumOfBests = checkBoxSumOfBests.Checked;

            Settings.Profile.DViewShowComp = checkBoxAlwaysShowComp.Checked;
            Settings.Profile.DViewShowLive = checkBoxLiveTime.Checked;
            Settings.Profile.DViewShowDeltas = checkBoxLiveDelta.Checked;
            base.DialogResult = DialogResult.OK;
        }
    }
}