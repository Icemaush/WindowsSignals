using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsSignals
{
    public partial class Form1 : Form
    {
        private BackgroundWorker bw;

        public Form1()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(Form1_Closing);
        }

        // Cancels running task before closing the application
        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            bw.CancelAsync();
        }

        // Start button function to begin a scan
        private void btnStartScan_Click(object sender, EventArgs e)
        {
            if (bw == null)
            {
                bw = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            }
            
            if (!bw.IsBusy)
            {
                AppendMessage("Starting scan...", true);
                bw.RunWorkerAsync();
            }
        }

        // Exit button function to close the program
        private void btnExit_Click(object sender, EventArgs e)
        {
            bw.CancelAsync();
            Application.Exit();
        }

        // Runs when background worker is started
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 1; i <= 100; i++)
            {
                
                if (bw.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                } else
                {
                    Thread.Sleep(100);
                    bw.ReportProgress(i);
                }
            }
        }

        // Runs when background worker progress is changed
        public void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string[] lines = textMessages.Lines;

            if (e.ProgressPercentage == 100)
            {
                lines[lines.Length - 1] = "Scan progress: " + e.ProgressPercentage + "%" + Environment.NewLine;
            } else
            {
                lines[lines.Length - 1] = "Scan progress: " + e.ProgressPercentage + "%";
            }
            
            textMessages.Lines = lines;
        }

        // Runs when background worker task is completed
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                AppendMessage("", true);
                AppendMessage("Scan cancelled.", true);
                AppendMessage("", true);
            } else
            {
                AppendMessage("Scan complete!", true);
                AppendMessage("", true);
            }
        }

        // Appends a message to the textbox
        private void AppendMessage(string text, bool newLine)
        {
            if (newLine)
            {
                textMessages.AppendText(text);
                textMessages.AppendText(Environment.NewLine);
            } else
            {
                textMessages.AppendText(text);
            }
        }

        // Detects Ctrl+C keys pressed and cancels the running task
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                bw.CancelAsync();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
