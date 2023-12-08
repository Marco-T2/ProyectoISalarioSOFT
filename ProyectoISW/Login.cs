using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProyectoISW
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            panel1.BackColor = Color.FromArgb(20, Color.Gray);
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
        private void login()
        {
            NpgsqlConnection cn = new NpgsqlConnection();
            string consulta = "select user, contrasena from login where usuario='" + txtUser.Text + "' and contrasena='" + txtPassword.Text + "'";
            cn.ConnectionString = "Username=postgres; Password=1234;Host=localhost;Port=5434;Database=proyecto";
            cn.Open();

            NpgsqlCommand cmd = new NpgsqlCommand(consulta, cn);
            NpgsqlDataReader leer;
            leer = cmd.ExecuteReader();

            if (leer.Read())
            {
                Menu fmenu = new Menu();
                this.Hide();
                fmenu.ShowDialog();
                
            }
            else
            {
                label4.Text = "EL USUARIO O LA CONTRASEÑA INGRESADOS SON INCORRECTOS!!!";

            }
            cn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            login();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //directo
            Menu fmenu = new Menu();
            this.Hide();
            fmenu.ShowDialog();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                txtPassword.PasswordChar = '*';
            }
        }
       

    }
}
