using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace Traffico_Residuo_3_C_Sharp
{
    public partial class Form1 : Form
    {
        private Timer timer = new Timer();
        private WebBrowser webBrowser = new WebBrowser();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true;
                this.Hide();
                this.ShowInTaskbar = false;
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                webBrowser.Navigate("http://internet.tre.it/");
                webBrowser.ScriptErrorsSuppressed = true;
            }

            timer.Interval = 10 * 1000;
            timer.Enabled = true;
            timer.Tick += new EventHandler(timer_Tick);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            string s = TrafficoResiduo();

            try
            {
                if (s != "Ti rimangono 500 MB di 1 GB")
                {
                    Notifica(ToolTipIcon.Info, "Traffico Residuo Giornaliero", s + " (" + Fuffa(s) + " GB previsti)");
                    timer.Enabled = false;
                }
                else
                {
                    Notifica(ToolTipIcon.Info, "Traffico Residuo Giornaliero", "Sito offline");
                    timer.Enabled = false;
                }
            }
            catch
            {
                Notifica(ToolTipIcon.Info, "Traffico Residuo Giornaliero", "Impossibile visualizzare il traffico residuo");
            }
            

        }

        private void esciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private string TrafficoResiduo()
        {
            string s = "";

            try
            {
                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(webBrowser.Document.Body.OuterHtml);
                HtmlNode htmlNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='divPercentageText']");

                s = htmlNode.InnerText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return s;
        }

        private void Notifica(ToolTipIcon icona, string titolo, string messaggio)
        {
            notifyIcon1.BalloonTipIcon = icona;
            notifyIcon1.BalloonTipTitle = titolo;
            notifyIcon1.BalloonTipText = messaggio;
            notifyIcon1.ShowBalloonTip(5000);
        }

        private double Fuffa(string traffico)
        {
            var trafficoRimanente = traffico;
            string[] parti = trafficoRimanente.Split();
            decimal trafficoMassimoGiornaliero = Convert.ToDecimal(parti[5]) / DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            decimal trafficoPrevisto = Convert.ToDecimal(parti[5]) - (trafficoMassimoGiornaliero * DateTime.Today.Day);
            return Convert.ToDouble(Math.Round(trafficoPrevisto, 2));
        }
    }
}
