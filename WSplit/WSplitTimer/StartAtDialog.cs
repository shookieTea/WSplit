namespace WSplitTimer
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public partial class StartAtDialog : Form
    {
        public StartAtDialog()
        {
            InitializeComponent();
        }

        public string StartingTime
        {
            get
            {
                return textBoxOffset.Text.Trim();
            }
        }

        public bool UseDelay
        {
            get
            {
                return checkBoxDelay.Checked;
            }
        }

        private void textBoxOffset_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ':' && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
            }
        }

        private void textBoxOffset_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(textBoxOffset.Text, "[^0-9:.,]"))
            {
                textBoxOffset.Text = Regex.Replace(textBoxOffset.Text, "[^0-9:.,]", "");
            }
        }
    }
}