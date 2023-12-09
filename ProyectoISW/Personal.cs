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
    public partial class Personal : Form



    {
        public Personal()
        {
            InitializeComponent();
            listarCargos();
            CargarPaises();
            CargarGenero();
            actualizarTabla();
            configuracionIniciali();
            dataGridView1.Columns[0].Width = 40; // Ajustar el ancho de la columna 0 a 150 píxeles
            dataGridView1.Columns[1].Width = 120; // Ajustar el ancho de la columna 2 automáticamente
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;



        }

        NpgsqlConnection conexion = new NpgsqlConnection("server=localhost;User Id=postgres; password=1234567;Database=bdplanilla");

        private void actualizarTabla()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("SELECT id, nombre,nacionalidad,fechanacimiento,sexo, fecha_ingreso,cargoocupacion,haber_basico FROM personal order by id asc;", conexion);
            DataTable dt = new DataTable();
            da.Fill(dt);

            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Columns[0].HeaderText = "Nro";
            this.dataGridView1.Columns[1].HeaderText = "Nombre y apellido";
            this.dataGridView1.Columns[2].HeaderText = "Nacionalidad";
            this.dataGridView1.Columns[3].HeaderText = "Fecha de Nacimiento";
            this.dataGridView1.Columns[4].HeaderText = "Sexo (F/M)";
            this.dataGridView1.Columns[5].HeaderText = "Fecha Ingreso";
            this.dataGridView1.Columns[6].HeaderText = "Cargo/Ocupacion";
            this.dataGridView1.Columns[7].HeaderText = "Haber Basico";

            this.dataGridView1.DataSource = dt;

        }

        public void configuracionIniciali()
        {
            textBox2.Enabled = false;
            textBox1.Enabled = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            dateTimePicker1.Enabled = false;
            dateTimePicker2.Enabled = false;

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
            label9.Text = "Nro";
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
        }

        private void listarCargos()
        {
            // Agregar un ítem predeterminado "Escoger"
            comboBox1.Items.Add("Seleccionar");

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("SELECT cargoocupacion FROM personal GROUP BY cargoocupacion;", conexion);
            DataTable dt = new DataTable();

            try
            {
                conexion.Open();
                da.Fill(dt);

                // Limpiar ComboBox antes de agregar elementos
                comboBox1.Items.Clear();
                comboBox1.Items.Add("Seleccionar");

                // Agregar cada nombre al ComboBox
                foreach (DataRow row in dt.Rows)
                {
                    comboBox1.Items.Add(row["cargoocupacion"].ToString());
                }

                // Seleccionar el ítem predeterminado "Seleccionar"
                comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                conexion.Close();
            }

            // Seleccionar el ítem predeterminado "Escoger"
            comboBox1.SelectedIndex = 0;


        }


        private void CargarPaises()
        {
            string[] nombresPaises = { "Argentina", "Brasil", "Chile", "Colombia", "México" };

            comboBox2.Items.Add("Seleccionar"); // Ítem predeterminado

            foreach (var nombre in nombresPaises)
            {
                comboBox2.Items.Add(nombre);
            }

            comboBox2.SelectedIndex = 0; // Selección inicial
        }

        private void CargarGenero()
        {
            string[] nombresPaises = { "M", "F"};

            comboBox3.Items.Add("Seleccionar"); // Ítem predeterminado

            foreach (var nombre in nombresPaises)
            {
                comboBox3.Items.Add(nombre);
            }

            comboBox3.SelectedIndex = 0; // Selección inicial
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
            textBox2.Enabled = true;
            textBox1.Enabled = true;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            dateTimePicker1.Enabled = true;
            dateTimePicker2.Enabled = true;

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
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            comboBox3.Enabled = true;
            dateTimePicker1.Enabled = true;
            dateTimePicker2.Enabled = true;

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
                textBox2.Text = row.Cells["haber_basico"].Value.ToString();
                comboBox1.Text = row.Cells["cargoocupacion"].Value.ToString();
                comboBox2.Text = row.Cells["nacionalidad"].Value.ToString();
                comboBox3.Text = row.Cells["sexo"].Value.ToString();
                if (DateTime.TryParse(row.Cells["fechanacimiento"].Value.ToString(), out DateTime fecha))
                {
                    dateTimePicker1.Value = fecha;
                }
                else
                {
                    MessageBox.Show("Error al convertir la fecha");
                }
                if (DateTime.TryParse(row.Cells["fecha_ingreso"].Value.ToString(), out DateTime fecha2))
                {
                    dateTimePicker2.Value = fecha2;
                }
                else
                {
                    MessageBox.Show("Error al convertir la fecha");
                }


            }
        }

        private void btnGuardarMod_Click(object sender, EventArgs e)
        {
            try
            {
                conexion.Open();
                String actualizar = "UPDATE personal SET nombre='" + textBox1.Text+ "',nacionalidad='" + comboBox2.Text+ "',fechanacimiento='" + dateTimePicker1.Text+ "',sexo='" + comboBox3.Text+ "',fecha_ingreso='" + dateTimePicker2.Text+ "', cargoocupacion='" + comboBox1.Text+ "',haber_basico='" + textBox2.Text+"' WHERE id = " + label9.Text+"";
                NpgsqlCommand cmd = new NpgsqlCommand(actualizar, conexion);
                cmd.ExecuteNonQuery();
                conexion.Close();
                MessageBox.Show("El registro actualizado ");
                actualizarTabla();

            }
            catch (InvalidCastException ex)
            {

                throw ex;
            }
        }

        private void btnGuardaNuevo_Click(object sender, EventArgs e)
        {
            string actualizar = "INSERT INTO personal(nombre, nacionalidad, fechanacimiento, sexo, fecha_ingreso, cargoocupacion, haber_basico) VALUES (@nombre, @nacionalidad, @fechanacimiento, @sexo, @fechaingreso, @cargoocupacion, @haberbasico)";
            NpgsqlCommand cmd = new NpgsqlCommand(actualizar, conexion);

            // Asegúrate de configurar los parámetros con los tipos de datos correctos
            cmd.Parameters.AddWithValue("@nombre", textBox1.Text);
            cmd.Parameters.AddWithValue("@nacionalidad", comboBox1.Text);
            cmd.Parameters.AddWithValue("@fechanacimiento", dateTimePicker1.Value);
            cmd.Parameters.AddWithValue("@sexo", comboBox3.Text);
            cmd.Parameters.AddWithValue("@fechaingreso", dateTimePicker2.Value);
            cmd.Parameters.AddWithValue("@cargoocupacion", comboBox2.Text);
            cmd.Parameters.AddWithValue("@haberbasico", Convert.ToDecimal(textBox2.Text)); // Ajusta el tipo de dato según corresponda

            try
            {
                conexion.Open();
                cmd.ExecuteNonQuery();
                conexion.Close();
                MessageBox.Show("El registro insertado");
                actualizarTabla();
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (conexion.State == ConnectionState.Open)
                {
                    conexion.Close();
                }
            }
        }

        private void btnEliminarRegistro_Click(object sender, EventArgs e)
        {
            try
            {
                conexion.Open();
                String actualizar = "DELETE FROM personal where id='" + label9.Text + "'";
                NpgsqlCommand cmd = new NpgsqlCommand(actualizar, conexion);
                cmd.ExecuteNonQuery();
                conexion.Close();
                MessageBox.Show("El registro eliminado");
                actualizarTabla();

            }
            catch (InvalidCastException ex)
            {

                throw ex;
            }
        }
    }
}
