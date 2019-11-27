using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Services;

namespace Master
{
    public partial class Form1 : Form
    {
        private Remote.cTransfer mi_Transfer = null;
        private System.Windows.Forms.Timer mi_EraseTextTimer;

        public Form1()
        {
            InitializeComponent();
            mi_EraseTextTimer = new System.Windows.Forms.Timer();
            mi_EraseTextTimer.Tick += new EventHandler(OnTimerEraseText);
            mi_EraseTextTimer.Interval = 4000;

        }
        private void OnTimerEraseText(Object Object, EventArgs EventArgs)
        {
            mi_EraseTextTimer.Stop();
            textBoxAnswer.Text = "Waiting...";
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Remote.cTransfer.kAction k_Action = new Remote.cTransfer.kAction();
            k_Action.s_Command = textBoxMessage.Text;
            k_Action.s_Computer = Environment.MachineName;

            this.Cursor = Cursors.WaitCursor;

            string s_URL = string.Format("tcp://{0} : {1}/TestService", textBoxComputer.Text, textBoxPort.Text);

            try
            {
                mi_Transfer = (Remote.cTransfer)Activator.GetObject(typeof(Remote.cTransfer), s_URL);
                Remote.cTransfer.kResponse k_Response = mi_Transfer.CallSlave(k_Action);

                textBoxAnswer.Text = "Answer from Slave:\r\r" + k_Response.s_Result;
            }
            catch(Exception Ex)
            {
                MessageBox.Show(this, "Error sending message to Slave:\n" + Ex.Message, "Master Error");
            }
            this.Cursor = Cursors.Arrow;
            mi_EraseTextTimer.Start();
        }
    }
}
