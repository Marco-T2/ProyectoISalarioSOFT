using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProyectoISW
{
    public partial class Usuario : Form



    {
        public Usuario()
        {
            InitializeComponent();
            actualizarTabla();
            configuracionIniciali();
            dataGridView1.Columns[0].Width = 40; // Ajustar el ancho de la columna 0 a 150 píxeles
            dataGridView1.Columns[2].Width = 120; // Ajustar el ancho de la columna 2 automáticamente
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;



        }

        NpgsqlConnection conexion = new NpgsqlConnection("server=localhost;User Id=postgres; password=1234567;Database=bdplanilla");

        private void actualizarTabla()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("SELECT * FROM usuario;", conexion);
            DataTable dt = new DataTable();
            da.Fill(dt);

            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Columns[0].HeaderText = "Nro";
            this.dataGridView1.Columns[1].HeaderText = "Nombre Apellido";
            this.dataGridView1.Columns[2].HeaderText = "Correo";
            this.dataGridView1.Columns[3].HeaderText = "Usuario";
            this.dataGridView1.Columns[4].HeaderText = "Password";

            this.dataGridView1.DataSource = dt;

        }

        public void configuracionIniciali()
        {
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            


            //Configuracion inicial de data gridview
            dataGridView1.Enabled = false;
            dataGridView1.ClearSelection();


            //Botones
            btnNuevoRegistro.Enabled = true;
            btnEditarReg.Enabled = true;
            btnGuardaNuevo.Enabled = false;
            btnGuardarMod.Enabled = false;
            btnEliminarRegistro.Enabled = false;
            btnCancelar.Enabled = false;

            //Inicializa los textos
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            label9.Text = "Nro";
        }


        private void validarNumeros(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Bloquea la entrada de caracteres no numéricos
            }
        }

        private void validarLetras(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar))
            {
                e.Handled = true; // Bloquea la entrada de caracteres que no sean letras
            }
        }


        private void btnNuevoRegistro_Click(object sender, EventArgs e)
        {
            clickNuevoRegistro();
        }

        public void clickNuevoRegistro()
        {
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            

            //habilitar botones
            btnEditarReg.Enabled = false;
            btnGuardaNuevo.Enabled = true;
            btnCancelar.Enabled = true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            configuracionIniciali();
        }

        private void btnEditarReg_Click(object sender, EventArgs e)
        {
            clickEditarRegistro();
        }

        public void clickEditarRegistro()
        {
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;

            //habilitar el Datagridview
            dataGridView1.Enabled = true;

            //habilitar botones
            btnNuevoRegistro.Enabled=false;
            btnGuardarMod.Enabled = true;
            btnEliminarRegistro.Enabled = true;
            btnCancelar.Enabled = true;
        }

        private void seleccionarCelda(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                label9.Text = row.Cells["id"].Value.ToString();
                textBox1.Text = row.Cells["nombre"].Value.ToString();
                textBox2.Text = row.Cells["correo"].Value.ToString();
                textBox3.Text = row.Cells["usuario"].Value.ToString();
                textBox4.Text = row.Cells["pasword"].Value.ToString();
            }
        }
    }
}
