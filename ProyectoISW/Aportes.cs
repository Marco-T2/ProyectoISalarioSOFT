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
    public partial class Aportes : Form
    {
        public Aportes()
        {
            InitializeComponent();
            configuracionIniciali();

        }

        public void configuracionIniciali()
        {
            //Establecer bloqueo de numeric upDown
            numericUpDown1.Enabled = false;
            numericUpDown2.Enabled = false;
            numericUpDown3.Enabled = false;
            numericUpDown4.Enabled = false;

            //Bloqueo de botones de opciones
            btn1.Enabled = true;
            btn2.Enabled = false;
            btn3.Enabled = false;
            btn4.Enabled = false;

            actualizarAfp();
            actualizarRiesgoAfp();
            actualizarComisionAfp();
            actualizarAporteSolidarioAfp();
        }
 

        NpgsqlConnection conexion = new NpgsqlConnection("server=localhost;User Id=postgres; password=1234567;Database=bdplanilla");

        //ACTUALIZA TODOS LOS DATOS en textbox
        private void actualizarAfp()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select afp from personal group by afp;", conexion);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // Obtener el valor de AFP desde el DataSet
                double valorAFP = Convert.ToDouble(ds.Tables[0].Rows[0]["afp"]);
                double valorAFPEnPorcentaje = valorAFP * 100;

                // Mostrar el valor en el TextBox
                numericUpDown1.Text = $"{valorAFPEnPorcentaje:F2}";
            }
            else
            {
                // Manejar si no se encontraron datos
                numericUpDown1.Text = "Valor de AFP no encontrado";
            }

        }

        private void actualizarRiesgoAfp()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select riesgocomunafp from personal group by riesgocomunafp;", conexion);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // Obtener el valor de AFP desde el DataSet
                double valorAFP = Convert.ToDouble(ds.Tables[0].Rows[0]["riesgocomunafp"]);
                double valorAFPEnPorcentaje = valorAFP * 100;

                // Mostrar el valor en el TextBox
                numericUpDown2.Text = $"{valorAFPEnPorcentaje:F2}";
            }
            else
            {
                // Manejar si no se encontraron datos
                numericUpDown2.Text = "Valor de RIESGO AFP no encontrado";
            }

        }

        private void actualizarComisionAfp()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select comisioafp from personal group by comisioafp;", conexion);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // Obtener el valor de AFP desde el DataSet
                double valorAFP = Convert.ToDouble(ds.Tables[0].Rows[0]["comisioafp"]);
                double valorAFPEnPorcentaje = valorAFP * 100;

                // Mostrar el valor en el TextBox
                numericUpDown3.Text = $"{valorAFPEnPorcentaje:F2}";
            }
            else
            {
                // Manejar si no se encontraron datos
                numericUpDown3.Text = "Valor de Comision AFP no encontrado";
            }

        }

        private void actualizarAporteSolidarioAfp()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select aportesolidario from personal group by aportesolidario;", conexion);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                // Obtener el valor de AFP desde el DataSet
                double valorAFP = Convert.ToDouble(ds.Tables[0].Rows[0]["aportesolidario"]);
                double valorAFPEnPorcentaje = valorAFP * 100;

                // Mostrar el valor en el TextBox
                numericUpDown4.Text = $"{valorAFPEnPorcentaje:F2}";
            }
            else
            {
                // Manejar si no se encontraron datos
                numericUpDown4.Text = "Valor de Aporte Solidario AFP no encontrado";
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            actualizarAfp();
            actualizarRiesgoAfp();
            actualizarComisionAfp();
            actualizarAporteSolidarioAfp();
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            btnmodificar();
        }

        public void btnmodificar()
        {
            //Habilitar botones principales
            btn1.Enabled = false;
            btn2.Enabled = true;
            btn3.Enabled = true;
            btn4.Enabled = true;

            //habilitar numericupdown
            numericUpDown1 .Enabled = true;
            numericUpDown2 .Enabled = true;
            numericUpDown3 .Enabled = true;
            numericUpDown4 .Enabled = true;
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            double valorPorcentaje1 = Convert.ToDouble(numericUpDown1.Value) / 100;
            double valorPorcentaje2 = Convert.ToDouble(numericUpDown2.Value) / 100;
            double valorPorcentaje3 = Convert.ToDouble(numericUpDown3.Value) / 100;
            double valorPorcentaje4 = Convert.ToDouble(numericUpDown4.Value) / 100;

            try
            {
                conexion.Open();
                String actualizar = "update personal set afp='" + valorPorcentaje1 + "';update personal set riesgocomunafp='" + valorPorcentaje2 + "';update personal set comisioafp='" + valorPorcentaje3 + "';update personal set aportesolidario='" + valorPorcentaje4 + "';ALTER TABLE personal ALTER COLUMN afp SET DEFAULT '"+valorPorcentaje1+"', ALTER COLUMN riesgocomunafp SET DEFAULT '"+valorPorcentaje2+"', ALTER COLUMN comisioafp SET DEFAULT '"+valorPorcentaje3+"', ALTER COLUMN aportesolidario SET DEFAULT '"+valorPorcentaje4+"';";
                NpgsqlCommand cmd = new NpgsqlCommand(actualizar, conexion);
                cmd.ExecuteNonQuery();
                conexion.Close();
                MessageBox.Show("El registro actualizado ");

            }
            catch (InvalidCastException ex)
            {

                throw ex;
            }

        }

        private void btn4_Click(object sender, EventArgs e)
        {
            configuracionIniciali();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string informacion = "El Aporte Laboral del 10% (Aporte de Vejez). – Se realiza Administradoras de Fondos de Pensiones que pasará a la Gestora Pública conforme la Ley 065, en forma de prima para el seguro a largo plazo que tiene por fin cubrir las pensiones del trabajador por jubilación por vejez cuando pase al sector pasivo";
            label16.Text = informacion;

            label16.AutoSize = false;
            label16.Width = groupBox2.Width; // Ajusta el ancho del Label al ancho del GroupBox con un pequeño margen

            // Calcula la altura requerida para mostrar todo el texto
            Size size = TextRenderer.MeasureText(label16.Text, label16.Font, new Size(label16.Width, int.MaxValue), TextFormatFlags.WordBreak);
            label16.Height = size.Height; // Establece la altura del Label para mostrar el texto envuelto
            groupBox2.Height = label16.Height + 50; // Ajusta también la altura del GroupBox

            // Centra verticalmente el Label dentro del GroupBox si es necesario
            label16.Top = (groupBox2.Height - label16.Height) / 2;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string informacion = "El Aporte Laboral de 1.71 % por Riesgo Común (Aporte-Riesgo Común). – Se realiza a las Administradoras de Fondos de Pensiones que pasará a la Gestora Pública conforme la Ley 065, en forma de prima para el seguro a largo plazo que tiene por objeto cubrir las pensiones del trabajador por accidentes ocurridos ajenos a la actividad laboral.";
            label16.Text = informacion;

            label16.AutoSize = false;
            label16.Width = groupBox2.Width; // Ajusta el ancho del Label al ancho del GroupBox con un pequeño margen

            // Calcula la altura requerida para mostrar todo el texto
            Size size = TextRenderer.MeasureText(label16.Text, label16.Font, new Size(label16.Width, int.MaxValue), TextFormatFlags.WordBreak);
            label16.Height = size.Height; // Establece la altura del Label para mostrar el texto envuelto
            groupBox2.Height = label16.Height + 50; // Ajusta también la altura del GroupBox

            // Centra verticalmente el Label dentro del GroupBox si es necesario
            label16.Top = (groupBox2.Height - label16.Height) / 2;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string informacion = "Comisión por Administración.- Las Administradoras de Fondos de Pensiones que pasará a ser la Gestora Pública conforme la Ley 065, por la administración de los aportes del trabajador a su cuenta individual, cobra una comisión del 0.5% sobre su total ganado mensual, del trabajador..";
            label16.Text = informacion;

            label16.AutoSize = false;
            label16.Width = groupBox2.Width; // Ajusta el ancho del Label al ancho del GroupBox con un pequeño margen

            // Calcula la altura requerida para mostrar todo el texto
            Size size = TextRenderer.MeasureText(label16.Text, label16.Font, new Size(label16.Width, int.MaxValue), TextFormatFlags.WordBreak);
            label16.Height = size.Height; // Establece la altura del Label para mostrar el texto envuelto
            groupBox2.Height = label16.Height + 50; // Ajusta también la altura del GroupBox

            // Centra verticalmente el Label dentro del GroupBox si es necesario
            label16.Top = (groupBox2.Height - label16.Height) / 2;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            string informacion = "El Aporte Laboral Solidario de 0.5%. – La Nueva Ley de Pensiones 065, creó el Aporte Laboral Solidario o Aporte Solidario del Asegurado, del 0.5%, sobre el total ganado mensual del trabajador";
            label16.Text = informacion;

            label16.AutoSize = false;
            label16.Width = groupBox2.Width; // Ajusta el ancho del Label al ancho del GroupBox con un pequeño margen

            // Calcula la altura requerida para mostrar todo el texto
            Size size = TextRenderer.MeasureText(label16.Text, label16.Font, new Size(label16.Width, int.MaxValue), TextFormatFlags.WordBreak);
            label16.Height = size.Height; // Establece la altura del Label para mostrar el texto envuelto
            groupBox2.Height = label16.Height + 50; // Ajusta también la altura del GroupBox

            // Centra verticalmente el Label dentro del GroupBox si es necesario
            label16.Top = (groupBox2.Height - label16.Height) / 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string informacion = "El Aporte Nacional Solidario según el art. 87-f y el Anexo de la Ley 065.- Es el aporte obligatorio destinado al fondo solidario, que realizan las personas con ingresos superiores a los límites establecidos a los cuales se –aplica el 1%, 5% y 10% sobre la diferencia positiva del Total Solidario y   el monto correspondiente a cada porcentaje; asimismo, el Total Solidario es la sumatoria de ingresos percibidos por el Aportante Nacional i Solidario, que constituye la base sobre la que se aplica los porcentajes para el pago del Aporte Nacional Solidario, que básicamente y de manera,; actual es la diferencia positiva de un salario entre Bs. 13.000, 25.000 o/y ; 35.000; lo que, puede ser modificado cada cinco años.\n\nAporte Nacional Solidario (Nuevo creado por Ley 065)- Cuando el Total Ganado es mayor n Bs. 13.000, a éste importe, según la NLP, se le denomina: «Total Solidario (TS)», existiendo 3 rangos de forma acumulativa:\n\n-TG o TS mayor a Bs.-  13.000 ( TGS- 13.000 *1%)\n-TG o TS mayor a Bs.-  25.000 ( TGS- 25.000 *5%)\n-TG o TS mayor a Bs.-  35.000 ( TGS- 35.000 *1%)\r\n\r\nAntes con la Ley 1732, el aporte laboral era  de 12,21%\r\n\r\nAhora con la ley  065 es de 12,71 + 1% +5% +10% según corresponda";
            label16.Text = informacion;

            label16.AutoSize = false;
            label16.Width = groupBox2.Width; // Ajusta el ancho del Label al ancho del GroupBox con un pequeño margen

            // Calcula la altura requerida para mostrar todo el texto
            Size size = TextRenderer.MeasureText(label16.Text, label16.Font, new Size(label16.Width, int.MaxValue), TextFormatFlags.WordBreak);
            label16.Height = size.Height; // Establece la altura del Label para mostrar el texto envuelto
            groupBox2.Height = label16.Height + 50; // Ajusta también la altura del GroupBox

            // Centra verticalmente el Label dentro del GroupBox si es necesario
            label16.Top = (groupBox2.Height - label16.Height) / 2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string informacion = "Este a diferencia de los: aportes laborales, es un impuesto laboral que se realiza de acuerdo a disposiciones vigentes por las cuales se graba casi todos los ingresos ‘. generados del trabajo, ya sean: sueldos, salarios, comisiones, horas extras, bono de producción, primas, etc., percibidos por los trabajadores dependientes, estando excepto de éste impuesto el aguinaldo y los beneficios sociales (indemnización por años de servicios y desahucio), subsidios, rentas  por jubilación y aportes de ley.\r\n\r\nPara deducir éste impuesto, el trabajador tiene que presentar en el formulario correspondiente, hasta el 20 de cada mes, facturas sobre las compras de bienes y servicios con una antigüedad de hasta 4 meses, debiendo estar a su nombre, con excepción de las facturas de educación, agua, luz o teléfono que pueden estar a nombre de un tercero.\r\n\r\nEl RC-IVA dispuesto por la Ley 843, de 20 de mayo de 1986 y su Decreto Supremo Reglamentario No. 21531 (Texto Ordenado en 1995), está a su ves regulado por el D.S. 24013 de 20 de mayo de 1995. ANEXO I, que en su Art. 19 dispone: Con el objeto de complementar el régimen del impuesto al valor agregado, créase un impuesto sobre los ingresos de las personas naturales y sucesiones indivisas, provenientes de la inversión del capital, del trabajo o de la aplicación conjunta de ambos factores";
            label16.Text = informacion;

            label16.AutoSize = false;
            label16.Width = groupBox2.Width; // Ajusta el ancho del Label al ancho del GroupBox con un pequeño margen

            // Calcula la altura requerida para mostrar todo el texto
            Size size = TextRenderer.MeasureText(label16.Text, label16.Font, new Size(label16.Width, int.MaxValue), TextFormatFlags.WordBreak);
            label16.Height = size.Height; // Establece la altura del Label para mostrar el texto envuelto
            groupBox2.Height = label16.Height + 50; // Ajusta también la altura del GroupBox

            // Centra verticalmente el Label dentro del GroupBox si es necesario
            label16.Top = (groupBox2.Height - label16.Height) / 2;
        }
    }
}
