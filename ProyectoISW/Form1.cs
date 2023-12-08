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

namespace ProyectoISW
{
    public partial class Form1 : Form
    {
        public int progress;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            circularProgressBar1.Value = 0;
            circularProgressBar1.Minimum = 0;
            circularProgressBar1.Maximum = 100;
            this.timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {   
            this.circularProgressBar1.Increment(3);
            this.circularProgressBar1.Text = circularProgressBar1.Value.ToString()+"%";


            if (this.circularProgressBar1.Value == 3) { this.label1.Text = "Cargando"; }
            if (this.circularProgressBar1.Value == 15) { this.label1.Text = "Cargando."; }
            if (this.circularProgressBar1.Value == 27) { this.label1.Text = "Cargando.."; }
            if (this.circularProgressBar1.Value == 39) { this.label1.Text = "Cargando..."; }
            if (this.circularProgressBar1.Value == 44) { this.label1.Text = "Cargando"; }
            if (this.circularProgressBar1.Value == 53) { this.label1.Text = "Cargando."; }
            if (this.circularProgressBar1.Value == 65) { this.label1.Text = "Iniciando.."; }
            if (this.circularProgressBar1.Value == 75) { this.label1.Text = "Iniciando..."; }

            if (circularProgressBar1.Value == circularProgressBar1.Maximum)
            {
                this.timer1.Stop();
                Login Flogin = new Login();
                this.Hide();
                Flogin.ShowDialog();

            }
            
        }
    }
}
