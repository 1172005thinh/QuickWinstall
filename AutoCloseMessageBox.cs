using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickWinstall
{
    public partial class AutoCloseMessageBox : Form
    {
        private int _timeoutMs;
        private System.Windows.Forms.Timer _timer;
        private int _secondsRemaining;

        public AutoCloseMessageBox(string message, string title, int timeoutMs)
        {
            InitializeComponent();
            this.Text = title;
            lblMessage.Text = message;
            _timeoutMs = timeoutMs;
            _secondsRemaining = timeoutMs / 1000;
            
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000; // 1 second
            _timer.Tick += Timer_Tick;
            
            UpdateButtonText();
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _secondsRemaining--;
            
            if (_secondsRemaining <= 0)
            {
                _timer.Stop();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                UpdateButtonText();
            }
        }

        private void UpdateButtonText()
        {
            btnOK.Text = $"OK ({_secondsRemaining}s)";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _timer?.Stop();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}