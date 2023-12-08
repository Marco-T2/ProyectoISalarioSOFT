using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProyectoISW
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
            customizeDesing();

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            pictureBox2.Visible = false;
            pictureBox4.Visible = true;

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            pictureBox2.Visible = true;
            pictureBox4.Visible = false;
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd,int wMsg, int wParam, int lParam);

        private void panel5_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void customizeDesing() //Oculta subMenus al iniciar
        {
            panelPlanilla.Visible = false;
            panelRegistros.Visible = false;
        }
        private void hideSubMenu() //Metodo para ocultar el SubMenu
        {
         

            if (panelPlanilla.Visible == true)
                panelPlanilla.Visible = false;
            if (panelRegistros.Visible == true)
                panelRegistros.Visible = false;
            
        }

        private Form activeForm = null;
        private void openChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelGeneral.Controls.Add(childForm);
            panelGeneral.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();


        }

        private void showSubMenu(Panel Submenu) //Metodo para mostrar el SubMenu
        {

            if (Submenu.Visible == false)
            {
                hideSubMenu();
                Submenu.Visible = true;
            }
            else
            {
                Submenu.Visible = false;
            }
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button38_Click(object sender, EventArgs e)
        {
            panelAux.Visible = false;
            panelMenuCompleto.Visible = true;
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            panelAux.Visible = true;
            panelMenuCompleto.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            showSubMenu(panelRegistros);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            showSubMenu(panelPlanilla);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            showSubMenu(panelPlanilla);
            panelAux.Visible = false;
            panelMenuCompleto.Visible = true;
            //--
            panelRegistros.Visible = false;
            //panelPlanilla.Visible = false;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            showSubMenu(panelRegistros);
            panelAux.Visible = false;
            panelMenuCompleto.Visible = true;

            //panelRegistros.Visible = false;
            panelPlanilla.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openChildForm(new PlanillaDeSueldos());
            hideSubMenu();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            openChildForm(new Registros());
            hideSubMenu();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            openChildForm(new RegistrosDom());
            hideSubMenu();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            openChildForm(new RegistrosFaltas());
            hideSubMenu();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            openChildForm(new Personal());
            hideSubMenu();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            openChildForm(new Personal());
            hideSubMenu();
        }

        private void button13_Click(object sender, EventArgs e)
        {

            openChildForm(new Usuario());
            hideSubMenu();
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            openChildForm(new Usuario());
            hideSubMenu();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            openChildForm(new Aportes());
            hideSubMenu();
        }
    }
}
