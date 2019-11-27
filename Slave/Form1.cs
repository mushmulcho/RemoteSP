using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Services;

namespace Slave
{
    public partial class Form1 : Form
    {
        private Remote.cTransfer mi_Transfer = null;
        private ObjRef mi_service = null;
        private TcpChannel mi_Channel = null;
        private bool mb_WaitButton = false;
        private System.Windows.Forms.Timer mi_EraseTextTimer;

        public Form1()
        {
            InitializeComponent();
            checkBoxListen.Checked = true;
            mi_EraseTextTimer = new System.Windows.Forms.Timer();
            mi_EraseTextTimer.Tick += new EventHandler(OnTimerEraseText);
            mi_EraseTextTimer.Interval = 4000;
        }
        private void OnTimerEraseText(Object Object, EventArgs EventArgs)
        {
            mi_EraseTextTimer.Stop();
            textBoxMessage.Text = "Waiting...";
        }

        private void btnRespond_Click(object sender, EventArgs e)
        {
            if (!mb_WaitButton)
                MessageBox.Show(this, "This Button has no effect until the master has send a message !", "Slave Error");
            mb_WaitButton = false;
            mi_EraseTextTimer.Start();
        }

        private void textBoxPort_TextChanged(object sender, EventArgs e)
        {
            checkBoxListen.Checked = false;
        }

        public void StopListen()
        {
            if (mi_service != null)
                RemotingServices.Unmarshal(mi_service);
            if (mi_Transfer != null)
                RemotingServices.Disconnect(mi_Transfer);
            if (mi_Channel != null)
                ChannelServices.UnregisterChannel(mi_Channel);

            mi_service = null;
            mi_Transfer = null;
            mi_Channel = null;

        }
        public void StartListen()
        {
            StopListen();
            try
            {
                int s32_port = int.Parse(textBoxPort.Text);
                mi_Channel = new TcpChannel(s32_port);
                ChannelServices.RegisterChannel(mi_Channel, false);
                mi_Transfer = new Remote.cTransfer();
                mi_service = RemotingServices.Marshal(mi_Transfer, "TestService");
                mi_Transfer.ev_SlaveCall += new Remote.cTransfer.del_SlaveCall(OnMasterEvent);
            }
            catch(Exception Ex)
            {
                MessageBox.Show(this, "Error starting listening:\n" + Ex.Message, "Slave");
                checkBoxListen.Checked = false;
            }
        }
        Remote.cTransfer.kResponse OnMasterEvent(Remote.cTransfer.kAction k_Action)
        {
            Remote.cTransfer.kResponse k_Response = new Remote.cTransfer.kResponse();
            if (mb_WaitButton)
            {
                k_Response.s_Result = "Sorry! Slave is currently busy. \r\nTry again later";
                return k_Response;
            }
            textBoxMessage.Text = string.Format("Message from [{0}] :\r\r{1}\r\n(Click button \"Send\" to answer", k_Action.s_Computer, k_Action.s_Command);

            mi_EraseTextTimer.Stop();

            mb_WaitButton = true;
            while (mb_WaitButton)
            {
                Thread.Sleep(200);
            }
            k_Response.s_Result = textBoxResponse.Text;
            return k_Response;
        }

        private void checkBoxListen_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxListen.Checked)
                StartListen();
            else
                StopListen();
        }
    }
}
