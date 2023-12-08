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
    public partial class Registros : Form
    {
        public Registros()
        {
            InitializeComponent();
            configuracionIniciali();
            listarPersonal();
            comboBox1.SelectedIndex = 0;
            dataGridView1.Columns[0].Width = 50; // Ajustar el ancho de la columna 0 a 150 píxeles
            dataGridView1.Columns[0].Width = 50; // Ajustar el ancho de la columna 0 a 150 píxeles
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


        }

        public void configuracionIniciali()
        {
            actualizarTabla();
            //dateTimePicker inicie en fecha actual
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker1.Enabled = false;


            //Limitar el valor de las horas
            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = 8;
            numericUpDown1.Enabled = false;


            //Incializacion de Label descripcion
            label5.Text = "ULTIMOS REGISTROS AGREGADOS";
            label8.Text = "Nro";

            //Inicializar parametros de personas ComboBox Datetime y label horas
            comboBox1.Enabled = true;
            numericUpDown1.Value = 0;

            //Inicializar Parametros de botones
            btnNuevoRegistro.Enabled = false; //Nuevo registro
            btnEditarReg.Enabled = false; //Editar registro
            btnGuardaNuevo.Enabled = false; //Guardar registro
            btnEliminarRegistro.Enabled = false; //Eliminar registro
            btnCancelar.Enabled = false; //Cancelar registro
            btnGuardarMod.Enabled = false; //Actualizar registro
            dataGridView1.Enabled = false;

        }
        public void clickNuevoRegistro()
        {
            actualizaCampoSeleccionado();
            btnNuevoRegistro.Enabled=false;
            btnEditarReg.Enabled=false;
            btnGuardaNuevo.Enabled = true;
            btnEliminarRegistro.Enabled=true;
            btnCancelar.Enabled=true;
            comboBox1.Enabled = false;

            //Habilita cambpos fecha y hora
            dateTimePicker1.Enabled = true;
            numericUpDown1.Enabled=true;
        }
        public void clickEditarRegistro()
        {
            actualizaCampoSeleccionado();
            btnNuevoRegistro.Enabled = false;
            btnEditarReg.Enabled = false;
            btnGuardarMod.Enabled = true;
            btnEliminarRegistro.Enabled = true;
            btnCancelar.Enabled = true;
            comboBox1.Enabled = false;

            //Habilita cambpos fecha y hora
            dateTimePicker1.Enabled = true;
            numericUpDown1.Enabled = true;

            //Habilitar la tabla
            dataGridView1.Enabled = true;
        }


        NpgsqlConnection conexion = new NpgsqlConnection("server=localhost;User Id=postgres; password=1234567;Database=bdplanilla");
        private void listarPersonal()
        {
            comboBox1.Items.Clear(); // Limpiar los elementos existentes en el ComboBox

            // Agregar un ítem predeterminado "Seleccionar"
            comboBox1.Items.Add("Seleccionar");

            NpgsqlDataAdapter da = new NpgsqlDataAdapter("SELECT nombre FROM personal;", conexion);
            DataTable dt = new DataTable();

            try
            {
                conexion.Open();
                da.Fill(dt);

                // Agregar cada nombre al ComboBox
                foreach (DataRow row in dt.Rows)
                {
                    comboBox1.Items.Add(row["nombre"].ToString());
                }
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
            // Seleccionar el ítem predeterminado "Seleccionar" si no hay selección
            if (comboBox1.SelectedIndex == -1)
            {
                comboBox1.SelectedIndex = 0;
            }

        }
        //CUANDO SELECIONA UN CAMPO
        private void campoSelecionado(object sender, EventArgs e)
        {          
            label5.Text = "DETALLE DE HORAS REGISTRADAS";
            btnNuevoRegistro.Enabled = true; //Nuevo registro
            btnEditarReg.Enabled = true; //Editar registro
            int indiceSeleccionado = comboBox1.SelectedIndex;
            
            string nombreSeleccionado = comboBox1.SelectedItem.ToString();
            
            // Consulta para obtener el 'id' basado en el 'nombre' seleccionado
            string query = "SELECT id FROM personal WHERE nombre = @Nombre";
            NpgsqlCommand cmd = new NpgsqlCommand(query, conexion);
            cmd.Parameters.AddWithValue("@Nombre", nombreSeleccionado);

            try
            {
                conexion.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    // Mostrar el 'id' en el Label
                    label7.Text = result.ToString();
                    actualizaCampoSeleccionado();
                }
                else
                {
                    label7.Text = "Nro";
                }
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
        }

        //ACTUALIZA TODOS LOS DATOS
        private void actualizarTabla()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("SELECT horasextra.id,personal.nombre,  horasextra.fecha, horasextra.canthoras\r\nFROM horasextra\r\nJOIN personal ON horasextra.idpersona = personal.id;", conexion);
            DataTable dt = new DataTable();
            da.Fill(dt);
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Columns[0].HeaderText = "Nro";
            this.dataGridView1.Columns[1].HeaderText = "Nombre y apellido";
            this.dataGridView1.Columns[2].HeaderText = "Fecha extra";
            this.dataGridView1.Columns[3].HeaderText = "Cantidad de hroas Extra";
        }

        //ACTUALIZAR SELECCIONADO
        private void actualizaCampoSeleccionado()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("SELECT horasextra.id,personal.nombre,  horasextra.fecha, horasextra.canthoras\r\nFROM horasextra\r\nJOIN personal ON horasextra.idpersona = personal.id\r\nWHERE personal.nombre = '" + comboBox1.Text + "';;", conexion);
            DataTable dt = new DataTable();
            da.Fill(dt);
            this.dataGridView1.DataSource = dt;
        }


        private void btnNuevoRegistro_Click(object sender, EventArgs e)//Nuevo registro
        {
            clickNuevoRegistro();
        }

        private void btnEditarReg_Click(object sender, EventArgs e)//Editar registro
        {
            clickEditarRegistro();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            configuracionIniciali();
        }


        private void btnGuardarMod_Click(object sender, EventArgs e)
        {
            try
            {
                conexion.Open();
                String actualizar = "update horasextra set fecha='" + dateTimePicker1.Text + "',canthoras='" + numericUpDown1.Text + "'where id='" + label8.Text + "'";
                NpgsqlCommand cmd = new NpgsqlCommand(actualizar, conexion);
                cmd.ExecuteNonQuery();
                conexion.Close();
                MessageBox.Show("El registro actualizado ");
                actualizaCampoSeleccionado();

            }
            catch (InvalidCastException ex)
            {

                throw ex;
            }

        }

        private void seleccionarCelda(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                label8.Text = row.Cells["id"].Value.ToString();
                comboBox1.Text = row.Cells["nombre"].Value.ToString();
                if (DateTime.TryParse(row.Cells["fecha"].Value.ToString(), out DateTime fecha))
                {
                    dateTimePicker1.Value = fecha;
                }
                else
                {
                    MessageBox.Show("Error al convertir la fecha");
                }
                if (int.TryParse(row.Cells["canthoras"].Value.ToString(), out int cantidadHoras))
                {
                    numericUpDown1.Value = cantidadHoras;
                }
                else
                {
                    MessageBox.Show("Error al convertir la cantidad de horas");
                }

            }
        }

        private void btnGuardaNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                conexion.Open();
                String actualizar = "INSERT INTO horasextra(idpersona, fecha, canthoras) VALUES ('" +label7.Text+"','" + dateTimePicker1.Text + "','" + numericUpDown1.Text + "');";
                NpgsqlCommand cmd = new NpgsqlCommand(actualizar, conexion);
                cmd.ExecuteNonQuery();
                conexion.Close();
                MessageBox.Show("El registro insertado");
                actualizaCampoSeleccionado();

            }
            catch (InvalidCastException ex)
            {

                throw ex;
            }
        }

        private void btnEliminarRegistro_Click(object sender, EventArgs e)
        {
            try
            {
                conexion.Open();
                String actualizar = "DELETE FROM horasextra where id='" + label8.Text + "'";
                NpgsqlCommand cmd = new NpgsqlCommand(actualizar, conexion);
                cmd.ExecuteNonQuery();
                conexion.Close();
                MessageBox.Show("El registro eliminado");
                actualizaCampoSeleccionado();

            }
            catch (InvalidCastException ex)
            {

                throw ex;
            }
        }
    }
}
