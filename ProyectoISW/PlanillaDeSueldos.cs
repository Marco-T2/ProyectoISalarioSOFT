using Npgsql;
using SpreadsheetLight.Drawing;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoISW
{
    public partial class PlanillaDeSueldos : Form
    {
        int anio1, mes1;

        public PlanillaDeSueldos()
        {
            InitializeComponent();
            actualizar_Planilla();
            dataGridView1.Columns[0].Width = 50; // Ajustar el ancho de la columna 0 a 150 píxeles
            dataGridView1.Columns[1].Width = 250; // Ajustar el ancho de la columna 2 automáticamente
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            btnexportar.Enabled = false;
        }

        NpgsqlConnection conexion = new NpgsqlConnection("server=localhost;User Id=postgres; password=1234567;Database=bdplanilla");


        private void actualizar_Planilla()
        {
            // Supongamos que deseas obtener la fecha seleccionada en el dateTimePicker1
            DateTime selectedDate = dateTimePicker1.Value;

            // Formatear la fecha al formato esperado por PostgreSQL (YYYY-MM-DD)
            string formattedDate = selectedDate.ToString("yyyy-MM-dd");


            NpgsqlDataAdapter da = new NpgsqlDataAdapter("WITH AntiguedadCalculada AS (\r\n    SELECT p.id, \r\n    EXTRACT(YEAR FROM AGE((SELECT (DATE_TRUNC('MONTH','"+ formattedDate + "'::DATE) + INTERVAL '1 MONTH - 1 day')::DATE), fecha_ingreso)) AS antiguedad\r\n    FROM personal p\r\n),\r\nPorcentajeBonoCalculado AS (\r\n    SELECT id,\r\n    CASE \r\n        WHEN antiguedad <= 2 THEN 6492*0.0\r\n        WHEN antiguedad <= 5 THEN 6492*0.05\r\n        WHEN antiguedad <= 8 THEN 6492*0.11\r\n        WHEN antiguedad <= 11 THEN 6492*0.18\r\n        WHEN antiguedad <= 15 THEN 6492*0.26\r\n        WHEN antiguedad <= 20 THEN 6492*0.34\r\n        WHEN antiguedad <= 25 THEN 6492*0.42\r\n        ELSE 6492*0.50\r\n    END AS porcentaje_bono\r\n    FROM AntiguedadCalculada\r\n),\r\nTotalHorasExtra AS (\r\n    SELECT p.id, COALESCE(SUM(he.canthoras), 0) AS totalHorasNoviembre\r\n    FROM personal p\r\n    LEFT JOIN horasExtra AS he ON he.idPersona = p.id\r\n    AND EXTRACT(YEAR FROM he.fecha) = EXTRACT(YEAR FROM '"+ formattedDate + "'::DATE) AND EXTRACT(MONTH FROM he.fecha) = EXTRACT(MONTH FROM '"+ formattedDate + "'::DATE)\r\n    GROUP BY p.id\r\n),\r\nTotalHorasDomingo AS (\r\n    SELECT p.id, COALESCE(SUM(de.cantDias), 0) AS totalHorasNoviembre\r\n    FROM personal p\r\n    LEFT JOIN DominicalesExtra AS de ON de.idPersona = p.id\r\n    AND EXTRACT(YEAR FROM de.fecha) = EXTRACT(YEAR FROM '"+ formattedDate + "'::DATE) AND EXTRACT(MONTH FROM de.fecha) = EXTRACT(MONTH FROM '"+ formattedDate + "'::DATE)\r\n    GROUP BY p.id\r\n\t\r\n),\r\nPagoDomingoCalculado AS (\r\n    SELECT p.id, COALESCE((SUM(de.cantDias)*(p.haber_basico/30))*3, 0) AS totalHorasNoviembre\r\n    FROM personal p\r\n    LEFT JOIN DominicalesExtra AS de ON de.idPersona = p.id\r\n    AND EXTRACT(YEAR FROM de.fecha) = EXTRACT(YEAR FROM '"+ formattedDate + "'::DATE) AND EXTRACT(MONTH FROM de.fecha) = EXTRACT(MONTH FROM '"+ formattedDate + "'::DATE)\r\n    GROUP BY p.id\r\n),\r\nDescuentoFaltas AS (\r\n    SELECT p.id, COALESCE((SUM(de.cantDias)*(p.haber_basico/30))) AS totalDiasFaltados\r\n    FROM personal p\r\n    LEFT JOIN diasdescuento AS de ON de.idPersona = p.id\r\n    AND EXTRACT(YEAR FROM de.fecha) = EXTRACT(YEAR FROM '"+ formattedDate + "'::DATE) AND EXTRACT(MONTH FROM de.fecha) = EXTRACT(MONTH FROM '"+ formattedDate    + "'::DATE)\r\n    GROUP BY p.id\r\n)\t\t\t\t\t \r\nSELECT p.id,p.nombre,p.nacionalidad,p.fechanacimiento,p.sexo,p.fecha_ingreso,a.antiguedad,p.cargoocupacion,p.diaspagadosmes,p.horasdiaspagados,p.haber_basico,pb.porcentaje_bono,th.totalHorasNoviembre AS totalHorasExtra,\r\n       ROUND((th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2)::numeric,2) AS pagoHorasExtra,\r\n       td.totalHorasNoviembre AS totalHorasDomingo, ROUND((pd.totalHorasNoviembre)::numeric,2) AS PagoDomingo,\r\n       ROUND(((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)::numeric,2) AS totalGanado,\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2) AS CtaAfp,\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2) AS RiesgoAFP,\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2) AS ComisionAFP,\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2) AS SolicadoioAFP,\r\n\t   ROUND((CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric,2) AS AporteNacionalSolidario,\r\n--RC IVA 13%\r\nCASE\r\n\t   WHEN(\r\n((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))-\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric)-(2362*4)))>0 THEN \r\n\r\nROUND((((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))-\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric)-(2362*4))*0.13)::numeric,2)\r\n\t\t\t\t\t\r\n\t\tELSE 0\r\n    END as RCIVA, \r\n--DESCUENTO DIAS NO TRABAJADOS\r\nROUND(COALESCE(df.totalDiasFaltados,0)::numeric,2) as DescuentoDiasFalta,\r\n--SUMADESCUENTOS\r\nROUND((\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))+\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )+(\r\nCASE\r\n\t   WHEN(\r\n((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))-\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric)-(2362*4)))>0 THEN \r\n\r\nROUND((((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))-\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric)-(2362*4))*0.13)::numeric,2)\r\n\t\t\t\t\t\r\n\t\tELSE 0\r\n    END\t\t\r\n\t\r\n\t)+COALESCE(df.totalDiasFaltados,0)::numeric)::numeric,2) As TotalDescuentos,\r\n--RESULTADO FINAL LIQUIDO PAGABLE\r\nROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n(\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))+\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )+(\r\nCASE\r\n\t   WHEN(\r\n((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))-\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric)-(2362*4)))>0 THEN \r\n\r\nROUND((((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))-\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric)-(2362*4))*0.13)::numeric,2)\r\n\t\t\t\t\t\r\n\t\tELSE 0\r\n    END\t\t\r\n\t\r\n\t)+COALESCE(df.totalDiasFaltados,0)::numeric))::numeric,2) as LiquidoPagable\r\n\t\t\t\t\t \r\nFROM personal p\r\nLEFT JOIN AntiguedadCalculada a ON p.id = a.id\r\nLEFT JOIN PorcentajeBonoCalculado pb ON p.id = pb.id\r\nLEFT JOIN TotalHorasExtra th ON p.id = th.id\t \t\t\t\t\t \r\nLEFT JOIN TotalHorasDomingo td ON p.id = td.id\r\nLEFT JOIN PagoDomingoCalculado pd ON p.id = pd.id\r\nLEFT JOIN DescuentoFaltas df ON p.id = df.id;", conexion);
            DataTable dt = new DataTable();
            da.Fill(dt);
            this.dataGridView1.DataSource = dt;
            this.dataGridView1.Columns[0].HeaderText = "Nro";
            this.dataGridView1.Columns[1].HeaderText = "Nombre y apellido";
            this.dataGridView1.Columns[2].HeaderText = "Nacionalidad";
            this.dataGridView1.Columns[3].HeaderText = "Fecha de Nacimiento";
            this.dataGridView1.Columns[4].HeaderText = "Sexo (F/M)";
            this.dataGridView1.Columns[5].HeaderText = "Fecha Ingreso";
            this.dataGridView1.Columns[6].HeaderText = "Antiguedad";
            this.dataGridView1.Columns[7].HeaderText = "Cargo/Ocupacion";
            this.dataGridView1.Columns[8].HeaderText = "Dias Pagados Mes";
            this.dataGridView1.Columns[9].HeaderText = "Hora/Dias Pagads";
            this.dataGridView1.Columns[10].HeaderText = "Haber Basico";
            this.dataGridView1.Columns[11].HeaderText = "Bono de Antiguedad";
            this.dataGridView1.Columns[12].HeaderText = "Numero de horas";
            this.dataGridView1.Columns[13].HeaderText = "Monto Pagado";
            this.dataGridView1.Columns[14].HeaderText = "Numero de dias Dominicales";
            this.dataGridView1.Columns[15].HeaderText = "Dominicales";
            this.dataGridView1.Columns[16].HeaderText = "Total Ganado";
            this.dataGridView1.Columns[17].HeaderText = "Cta. Individual AFP 10%";
            this.dataGridView1.Columns[18].HeaderText = "Riesgo Comun AFP 1.71%";
            this.dataGridView1.Columns[19].HeaderText = "Comision AFP 0.5%";
            this.dataGridView1.Columns[20].HeaderText = "Aporte Solidario 0.5%";
            this.dataGridView1.Columns[21].HeaderText = "ANS Aporte Nacional Solidadio (1% 5% 10%)";
            this.dataGridView1.Columns[22].HeaderText = "RC-IVA 13%";
            this.dataGridView1.Columns[23].HeaderText = "Descuentos dias no trabajados";
            this.dataGridView1.Columns[24].HeaderText = "Total Descuentos";
            this.dataGridView1.Columns[25].HeaderText = "Liquido Pagable";

            double sumaColumna1 = 0;
            double sumaColumna2 = 0;
            double sumaColumna3 = 0;
            double sumaColumna4 = 0;
            double sumaColumna5 = 0;
            double sumaColumna6 = 0;
            double sumaColumna7 = 0;
            double sumaColumna8 = 0;
            double sumaColumna9 = 0;
            double sumaColumna10 = 0;
            double sumaColumna11 = 0;
            double sumaColumna12 = 0;
            double sumaColumna13 = 0;
            double sumaColumna14 = 0;
            double sumaColumna15 = 0;
            double sumaColumna16 = 0;
            foreach (DataRow row in dt.Rows)
            {
                // Suponiendo que la columna que quieres sumar se llama "columna_a_sumar"
                sumaColumna1 += Convert.ToDouble(row["haber_basico"]);
                sumaColumna2 += Convert.ToDouble(row["porcentaje_bono"]);
                sumaColumna3 += Convert.ToDouble(row["totalhorasextra"]);
                sumaColumna4 += Convert.ToDouble(row["pagohorasextra"]);
                sumaColumna5 += Convert.ToDouble(row["totalhorasdomingo"]);
                sumaColumna6 += Convert.ToDouble(row["pagodomingo"]);
                sumaColumna7 += Convert.ToDouble(row["totalganado"]);
                sumaColumna8 += Convert.ToDouble(row["ctaafp"]);
                sumaColumna9 += Convert.ToDouble(row["riesgoafp"]);
                sumaColumna10 += Convert.ToDouble(row["comisionafp"]);
                sumaColumna11 += Convert.ToDouble(row["solicadoioafp"]);
                sumaColumna12 += Convert.ToDouble(row["aportenacionalsolidario"]);
                sumaColumna13 += Convert.ToDouble(row["rciva"]);
                sumaColumna14 += Convert.ToDouble(row["descuentodiasfalta"]);
                sumaColumna15 += Convert.ToDouble(row["totaldescuentos"]);
                sumaColumna16 += Convert.ToDouble(row["liquidopagable"]);

            }
            DataRow nuevaFila = dt.NewRow();
            nuevaFila["haber_basico"] = sumaColumna1;
            nuevaFila["porcentaje_bono"] = sumaColumna2;
            nuevaFila["totalhorasextra"] = sumaColumna3;
            nuevaFila["pagohorasextra"] = sumaColumna4;
            nuevaFila["totalhorasdomingo"] = sumaColumna5;
            nuevaFila["pagodomingo"] = sumaColumna6;
            nuevaFila["totalganado"] = sumaColumna7;
            nuevaFila["ctaafp"] = sumaColumna8;
            nuevaFila["riesgoafp"] = sumaColumna9;
            nuevaFila["comisionafp"] = sumaColumna10;
            nuevaFila["solicadoioafp"] = sumaColumna11;
            nuevaFila["aportenacionalsolidario"] = sumaColumna12;
            nuevaFila["rciva"] = sumaColumna13;
            nuevaFila["descuentodiasfalta"] = sumaColumna14;
            nuevaFila["totaldescuentos"] = sumaColumna15;
            nuevaFila["liquidopagable"] = sumaColumna16;


            dt.Rows.Add(nuevaFila);
            dataGridView1.DataSource = dt;
            int ultimaFilaIndex = dt.Rows.Count - 1;

            // Aplicar estilo de celda negrita para toda la fila
            dataGridView1.Rows[ultimaFilaIndex].DefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
        }

        private void AdjustDatagridViewHeight()
        {
            var height = dataGridView1.ColumnHeadersHeight;
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                height += dr.Height;
            }
            dataGridView1.Height = height;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker dtp = sender as DateTimePicker;
            btnexportar.Enabled = true;
            if (dtp != null)
            {
                // Obtener el año y el mes seleccionados
                int year = dtp.Value.Year;
                int month = dtp.Value.Month;

                // Obtener el último día del mes seleccionado
                int lastDay = DateTime.DaysInMonth(year, month);

                // Establecer el último día como valor seleccionado
                dtp.Value = new DateTime(year, month, lastDay);
            }
            if (dtp != null)
            {
                int mes = dtp.Value.Month;
                int anio = dtp.Value.Year;

                mes1 = mes;
                anio1= anio;

                label3.Text = mes.ToString(); // Mostrar el mes en el primer Label
                label5.Text = anio.ToString(); // Mostrar el año en el segundo Label
            }
            actualizar_Planilla();
        }


        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Verificar si es una celda de encabezado y si es la fila de encabezado
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

                StringFormat formato = new StringFormat();
                formato.Alignment = StringAlignment.Center;
                formato.LineAlignment = StringAlignment.Center;

                // Ajustar la altura de la fila del encabezado para permitir dos líneas
                e.Graphics.DrawString(e.FormattedValue.ToString(), e.CellStyle.Font, Brushes.Black, e.CellBounds, formato);
                e.Handled = true;
            }
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            string pathfile = AppDomain.CurrentDomain.BaseDirectory + "miExcel.xlsx";

            SLDocument sl = new SLDocument();
            // para agregar imagen en excel
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap("C:/Users/marco/Desktop/Nueva carpeta/Soft/ProyectoISalarioSOFT/ProyectoISW/Imagenes/logo1.jpeg");
            byte[] ba;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bm.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Close();
                ba = ms.ToArray();
            }
            SLPicture pic = new SLPicture(ba, DocumentFormat.OpenXml.Packaging.ImagePartType.Png);
            pic.SetPosition(0, 1);
            pic.ResizeInPixels(170, 90);
            sl.InsertPicture(pic);// fin 
                                  // agregando 
            sl.SetCellValue("I2", "PLANILLA DE SUELDOS Y SALARIOS");
            SLStyle estilo = sl.CreateStyle();
            estilo.Font.FontName = "Arial";
            estilo.Font.FontSize = 14;
            estilo.Font.Bold = true;
            estilo.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center);
            estilo.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
            sl.SetCellStyle("I2", estilo);
            sl.MergeWorksheetCells("I2", "R2");// conbina las celdas


            SLStyle estiloA = sl.CreateStyle();
            estiloA.Font.FontName = "Arial";
            estiloA.Font.FontSize = 12;
            estiloA.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center);
            estiloA.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
            // estilo.Font.Bold = true;
            sl.SetCellValue("I3", "CORRESPONDIENTE AL MES " + mes1 + " DEL " + anio1);
            sl.MergeWorksheetCells("I3", "R3");// conbina las celdas
            sl.SetCellStyle("I3", estiloA);

            sl.SetCellValue("I4", "(Expresado en Bolivianos)");
            sl.MergeWorksheetCells("I4", "R4");// conbina las celdas
            sl.SetCellStyle("I4", estiloA);


            // Supongamos que deseas obtener la fecha seleccionada en el dateTimePicker1
            DateTime selectedDate = dateTimePicker1.Value;
            // Formatear la fecha al formato esperado por PostgreSQL (YYYY-MM-DD)
            string formattedDate = selectedDate.ToString("yyyy-MM-dd");

            String query = "WITH AntiguedadCalculada AS (\r\n    SELECT p.id, \r\n    EXTRACT(YEAR FROM AGE((SELECT (DATE_TRUNC('MONTH','" + formattedDate + "'::DATE) + INTERVAL '1 MONTH - 1 day')::DATE), fecha_ingreso)) AS antiguedad\r\n    FROM personal p\r\n),\r\nPorcentajeBonoCalculado AS (\r\n    SELECT id,\r\n    CASE \r\n        WHEN antiguedad <= 2 THEN 6492*0.0\r\n        WHEN antiguedad <= 5 THEN 6492*0.05\r\n        WHEN antiguedad <= 8 THEN 6492*0.11\r\n        WHEN antiguedad <= 11 THEN 6492*0.18\r\n        WHEN antiguedad <= 15 THEN 6492*0.26\r\n        WHEN antiguedad <= 20 THEN 6492*0.34\r\n        WHEN antiguedad <= 25 THEN 6492*0.42\r\n        ELSE 6492*0.50\r\n    END AS porcentaje_bono\r\n    FROM AntiguedadCalculada\r\n),\r\nTotalHorasExtra AS (\r\n    SELECT p.id, COALESCE(SUM(he.canthoras), 0) AS totalHorasNoviembre\r\n    FROM personal p\r\n    LEFT JOIN horasExtra AS he ON he.idPersona = p.id\r\n    AND EXTRACT(YEAR FROM he.fecha) = EXTRACT(YEAR FROM '" + formattedDate + "'::DATE) AND EXTRACT(MONTH FROM he.fecha) = EXTRACT(MONTH FROM '" + formattedDate + "'::DATE)\r\n    GROUP BY p.id\r\n),\r\nTotalHorasDomingo AS (\r\n    SELECT p.id, COALESCE(SUM(de.cantDias), 0) AS totalHorasNoviembre\r\n    FROM personal p\r\n    LEFT JOIN DominicalesExtra AS de ON de.idPersona = p.id\r\n    AND EXTRACT(YEAR FROM de.fecha) = EXTRACT(YEAR FROM '" + formattedDate + "'::DATE) AND EXTRACT(MONTH FROM de.fecha) = EXTRACT(MONTH FROM '" + formattedDate + "'::DATE)\r\n    GROUP BY p.id\r\n\t\r\n),\r\nPagoDomingoCalculado AS (\r\n    SELECT p.id, COALESCE((SUM(de.cantDias)*(p.haber_basico/30))*3, 0) AS totalHorasNoviembre\r\n    FROM personal p\r\n    LEFT JOIN DominicalesExtra AS de ON de.idPersona = p.id\r\n    AND EXTRACT(YEAR FROM de.fecha) = EXTRACT(YEAR FROM '" + formattedDate + "'::DATE) AND EXTRACT(MONTH FROM de.fecha) = EXTRACT(MONTH FROM '" + formattedDate + "'::DATE)\r\n    GROUP BY p.id\r\n),\r\nDescuentoFaltas AS (\r\n    SELECT p.id, COALESCE((SUM(de.cantDias)*(p.haber_basico/30))) AS totalDiasFaltados\r\n    FROM personal p\r\n    LEFT JOIN diasdescuento AS de ON de.idPersona = p.id\r\n    AND EXTRACT(YEAR FROM de.fecha) = EXTRACT(YEAR FROM '" + formattedDate + "'::DATE) AND EXTRACT(MONTH FROM de.fecha) = EXTRACT(MONTH FROM '" + formattedDate + "'::DATE)\r\n    GROUP BY p.id\r\n)\t\t\t\t\t \r\nSELECT p.id,p.nombre,p.nacionalidad,p.fechanacimiento,p.sexo,p.fecha_ingreso,a.antiguedad,p.cargoocupacion,p.diaspagadosmes,p.horasdiaspagados,p.haber_basico,pb.porcentaje_bono,th.totalHorasNoviembre AS totalHorasExtra,\r\n       ROUND((th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2)::numeric,2) AS pagoHorasExtra,\r\n       td.totalHorasNoviembre AS totalHorasDomingo, ROUND((pd.totalHorasNoviembre)::numeric,2) AS PagoDomingo,\r\n       ROUND(((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)::numeric,2) AS totalGanado,\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2) AS CtaAfp,\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2) AS RiesgoAFP,\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2) AS ComisionAFP,\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2) AS SolicadoioAFP,\r\n\t   ROUND((CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric,2) AS AporteNacionalSolidario,\r\n--RC IVA 13%\r\nCASE\r\n\t   WHEN(\r\n((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))-\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric)-(2362*4)))>0 THEN \r\n\r\nROUND((((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))-\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric)-(2362*4))*0.13)::numeric,2)\r\n\t\t\t\t\t\r\n\t\tELSE 0\r\n    END as RCIVA, \r\n--DESCUENTO DIAS NO TRABAJADOS\r\nROUND(COALESCE(df.totalDiasFaltados,0)::numeric,2) as DescuentoDiasFalta,\r\n--SUMADESCUENTOS\r\nROUND((\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))+\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )+(\r\nCASE\r\n\t   WHEN(\r\n((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))-\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric)-(2362*4)))>0 THEN \r\n\r\nROUND((((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))-\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric)-(2362*4))*0.13)::numeric,2)\r\n\t\t\t\t\t\r\n\t\tELSE 0\r\n    END\t\t\r\n\t\r\n\t)+COALESCE(df.totalDiasFaltados,0)::numeric)::numeric,2) As TotalDescuentos,\r\n--RESULTADO FINAL LIQUIDO PAGABLE\r\nROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n(\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))+\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )+(\r\nCASE\r\n\t   WHEN(\r\n((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))-\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric)-(2362*4)))>0 THEN \r\n\r\nROUND((((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)-\r\n       (ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.afp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.riesgoComunAfp)::numeric, 2)+\r\n\t   ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.comisioAfp)::numeric, 2)+\r\n       ROUND((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre)*p.aporteSolidario)::numeric, 2))-\r\n(CASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 35000) * 0.10))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 25000) * 0.05))\r\n        ELSE 0\r\n    END +\r\n\t\tCASE\r\n        WHEN (((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) > 0 THEN\r\n            (((((pb.porcentaje_bono) + (th.totalHorasNoviembre * ((p.haber_basico/30)/8)*2) + p.haber_basico + pd.totalHorasNoviembre) - 13000) * 0.01))\r\n        ELSE 0\r\n    END )::numeric)-(2362*4))*0.13)::numeric,2)\r\n\t\t\t\t\t\r\n\t\tELSE 0\r\n    END\t\t\r\n\t\r\n\t)+COALESCE(df.totalDiasFaltados,0)::numeric))::numeric,2) as LiquidoPagable\r\n\t\t\t\t\t \r\nFROM personal p\r\nLEFT JOIN AntiguedadCalculada a ON p.id = a.id\r\nLEFT JOIN PorcentajeBonoCalculado pb ON p.id = pb.id\r\nLEFT JOIN TotalHorasExtra th ON p.id = th.id\t \t\t\t\t\t \r\nLEFT JOIN TotalHorasDomingo td ON p.id = td.id\r\nLEFT JOIN PagoDomingoCalculado pd ON p.id = pd.id\r\nLEFT JOIN DescuentoFaltas df ON p.id = df.id;";

            conexion.Open();
            NpgsqlCommand command = new NpgsqlCommand(query, conexion);
            NpgsqlDataReader reader = command.ExecuteReader();

            int celdaCabecera = 6;
            int celdaCabeceraInicio = 6;
            sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, "PlanillaSueldo");// cambia nombre de la hoja de excel

            // estilo de la cabecera inicio
            SLStyle style = sl.CreateStyle();
            style.Font.FontName = "Arial";
            style.Font.FontSize = 12;
            style.Font.Bold = true;
            style.Font.FontColor = System.Drawing.Color.White;// color de la letra
            style.Fill.SetPattern(DocumentFormat.OpenXml.Spreadsheet.PatternValues.Solid, System.Drawing.Color.FromArgb(20,52,81), System.Drawing.Color.FromArgb(20, 52, 81));
            style.SetWrapText(true);
            style.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center);
            style.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
            // fin de estilo cabecera

            sl.SetCellValue("A" + celdaCabecera, "Nro");
            sl.SetCellValue("B" + celdaCabecera, "Nombre y Apellido");
            sl.SetCellValue("C" + celdaCabecera, "Fecha de\n nacimiento");
            sl.SetCellValue("D" + celdaCabecera, "Sexo\n(M/F)");
            sl.SetCellValue("E" + celdaCabecera, "Fecha\n Ingreso");
            sl.SetCellValue("F" + celdaCabecera, "Antiguedad");
            sl.SetCellValue("G" + celdaCabecera, "Cargo");
            sl.SetCellValue("H" + celdaCabecera, "Pagodo\n(D/M)");
            sl.SetCellValue("I" + celdaCabecera, "Pagodo\n(H/D)");
            sl.SetCellValue("J" + celdaCabecera, "Haber\n basico");
            sl.SetCellValue("K" + celdaCabecera, "Porcentaje\n Bono");
            sl.SetCellValue("L" + celdaCabecera, "Total hora\n extra");
            sl.SetCellValue("M" + celdaCabecera, "Pago\n Hora extra");
            sl.SetCellValue("N" + celdaCabecera, "Total hora\n extra domingo");
            sl.SetCellValue("O" + celdaCabecera, "Pago\n Domingo");
            sl.SetCellValue("P" + celdaCabecera, "Total\n Ganado");
            sl.SetCellValue("Q" + celdaCabecera, "cta\n afp");
            sl.SetCellValue("R" + celdaCabecera, "Riesgo\n afp");
            sl.SetCellValue("S" + celdaCabecera, "Comision\n afp");
            sl.SetCellValue("T" + celdaCabecera, "Solicadoio\n afp");
            sl.SetCellValue("U" + celdaCabecera, "Aporte nacional\n solidario");
            sl.SetCellValue("V" + celdaCabecera, "RC IVA");
            sl.SetCellValue("W" + celdaCabecera, "Descuento\n dias falta");
            sl.SetCellValue("X" + celdaCabecera, "Total\n descuentos");
            sl.SetCellValue("Y" + celdaCabecera, "Liquido\n pagable");
            sl.SetCellStyle("A6", "Y6", style);

            // declarando variables para sumar totales
            double sumaColumna1 = 0;
            double sumaColumna2 = 0;
            double sumaColumna3 = 0;
            double sumaColumna4 = 0;
            double sumaColumna5 = 0;
            double sumaColumna6 = 0;
            double sumaColumna7 = 0;
            double sumaColumna8 = 0;
            double sumaColumna9 = 0;
            double sumaColumna10 = 0;
            double sumaColumna11 = 0;
            double sumaColumna12 = 0;
            double sumaColumna13 = 0;
            double sumaColumna14 = 0;
            double sumaColumna15 = 0;
            double sumaColumna16 = 0;


            while (reader.Read())
            {
                celdaCabecera++;
                sl.SetCellValue("A" + celdaCabecera, int.Parse(reader["id"].ToString()));
                sl.SetCellValue("B" + celdaCabecera, reader["nombre"].ToString());

                DateTime fecha1 = DateTime.Parse(reader["fechanacimiento"].ToString());
                string fechaFormateada = fecha1.ToString("dd/MM/yyyy");

                sl.SetCellValue("C" + celdaCabecera, fechaFormateada);
                sl.SetCellValue("D" + celdaCabecera, reader["sexo"].ToString());

                DateTime fecha_in = DateTime.Parse(reader["fecha_ingreso"].ToString());
                string fechaFormateada_in = fecha_in.ToString("dd/MM/yyyy");
                sl.SetCellValue("E" + celdaCabecera, fechaFormateada_in);
                sl.SetCellValue("F" + celdaCabecera, int.Parse(reader["antiguedad"].ToString()));
                sl.SetCellValue("G" + celdaCabecera, reader["cargoocupacion"].ToString());
                sl.SetCellValue("H" + celdaCabecera, int.Parse(reader["diaspagadosmes"].ToString()));
                sl.SetCellValue("I" + celdaCabecera, int.Parse(reader["horasdiaspagados"].ToString()));
                sl.SetCellValue("J" + celdaCabecera, double.Parse(reader["haber_basico"].ToString()));
                sumaColumna1 += double.Parse(reader["haber_basico"].ToString());
                sl.SetCellValue("K" + celdaCabecera, double.Parse(reader["porcentaje_bono"].ToString()));
                sumaColumna2 += double.Parse(reader["porcentaje_bono"].ToString());
                sl.SetCellValue("L" + celdaCabecera, int.Parse(reader["totalhorasextra"].ToString()));
                sumaColumna3 += int.Parse(reader["totalhorasextra"].ToString());
                sl.SetCellValue("M" + celdaCabecera, double.Parse(reader["pagohorasextra"].ToString()));
                sumaColumna4 += double.Parse(reader["pagohorasextra"].ToString());
                sl.SetCellValue("N" + celdaCabecera, int.Parse(reader["totalhorasdomingo"].ToString()));
                sumaColumna5 += int.Parse(reader["totalhorasdomingo"].ToString());
                sl.SetCellValue("O" + celdaCabecera, double.Parse(reader["pagodomingo"].ToString()));
                sumaColumna6 += double.Parse(reader["pagodomingo"].ToString());
                sl.SetCellValue("P" + celdaCabecera, double.Parse(reader["totalganado"].ToString()));
                sumaColumna7 += double.Parse(reader["totalganado"].ToString());
                sl.SetCellValue("Q" + celdaCabecera, double.Parse(reader["ctaafp"].ToString()));
                sumaColumna8 += double.Parse(reader["ctaafp"].ToString());
                sl.SetCellValue("R" + celdaCabecera, double.Parse(reader["riesgoafp"].ToString()));
                sumaColumna9 += double.Parse(reader["riesgoafp"].ToString());
                sl.SetCellValue("S" + celdaCabecera, double.Parse(reader["comisionafp"].ToString()));
                sumaColumna10 += double.Parse(reader["comisionafp"].ToString());
                sl.SetCellValue("T" + celdaCabecera, double.Parse(reader["solicadoioafp"].ToString()));
                sumaColumna11 += double.Parse(reader["solicadoioafp"].ToString());
                sl.SetCellValue("U" + celdaCabecera, double.Parse(reader["aportenacionalsolidario"].ToString()));
                sumaColumna12 += double.Parse(reader["aportenacionalsolidario"].ToString());
                sl.SetCellValue("V" + celdaCabecera, double.Parse(reader["rciva"].ToString()));
                sumaColumna13 += double.Parse(reader["rciva"].ToString());
                sl.SetCellValue("W" + celdaCabecera, double.Parse(reader["descuentodiasfalta"].ToString()));
                sumaColumna14 += double.Parse(reader["descuentodiasfalta"].ToString());
                sl.SetCellValue("X" + celdaCabecera, double.Parse(reader["totaldescuentos"].ToString()));
                sumaColumna15 += double.Parse(reader["totaldescuentos"].ToString());
                sl.SetCellValue("Y" + celdaCabecera, double.Parse(reader["liquidopagable"].ToString()));
                sumaColumna16 += double.Parse(reader["liquidopagable"].ToString());
                //sl.SetCellValue("U" + celdaCabecera, reader["porcentaje_bono"].ToString());
            }
            celdaCabecera++;
            sl.SetCellValue("J" + celdaCabecera, sumaColumna1);
            sl.SetCellValue("K" + celdaCabecera, sumaColumna2);
            sl.SetCellValue("L" + celdaCabecera, sumaColumna3);
            sl.SetCellValue("M" + celdaCabecera, sumaColumna4);
            sl.SetCellValue("N" + celdaCabecera, sumaColumna5);
            sl.SetCellValue("O" + celdaCabecera, sumaColumna6);
            sl.SetCellValue("P" + celdaCabecera, sumaColumna7);
            sl.SetCellValue("Q" + celdaCabecera, sumaColumna8);
            sl.SetCellValue("R" + celdaCabecera, sumaColumna9);
            sl.SetCellValue("S" + celdaCabecera, sumaColumna10);
            sl.SetCellValue("T" + celdaCabecera, sumaColumna11);
            sl.SetCellValue("U" + celdaCabecera, sumaColumna12);
            sl.SetCellValue("V" + celdaCabecera, sumaColumna13);
            sl.SetCellValue("W" + celdaCabecera, sumaColumna14);
            sl.SetCellValue("X" + celdaCabecera, sumaColumna15);
            sl.SetCellValue("Y" + celdaCabecera, sumaColumna16);

            sl.MergeWorksheetCells("A" + celdaCabecera, "i" + celdaCabecera);// conbina las celdas
            sl.SetCellValue("A" + celdaCabecera, "TOTAL");
            


            SLStyle estiloB = sl.CreateStyle();
            estiloB.Border.LeftBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            estiloB.Border.LeftBorder.Color = System.Drawing.Color.Black;
            estiloB.Border.TopBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            estiloB.Border.RightBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            estiloB.Border.BottomBorder.BorderStyle = DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin;
            sl.SetCellStyle("A" + celdaCabeceraInicio, "Y" + celdaCabecera, estiloB);

            sl.AutoFitColumn("A7", "Y7");

            SLStyle estiloTOTAL = sl.CreateStyle();
            estiloTOTAL.Font.Bold = true;
            estiloTOTAL.Font.FontSize = 12;
            estiloTOTAL.SetHorizontalAlignment(DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center);
            estiloTOTAL.SetVerticalAlignment(DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center);
            sl.SetCellStyle("A" + celdaCabecera, "i" + celdaCabecera, estiloTOTAL);// conbina las celdas

            SLStyle estiloTOTALNumeros = sl.CreateStyle();
            estiloTOTALNumeros.Font.Bold = true;
            estiloTOTALNumeros.Font.FontSize = 12;
            sl.SetCellStyle("i" + celdaCabecera, "y" + celdaCabecera, estiloTOTALNumeros);// conbina las celdas


            sl.SaveAs(pathfile);
            Process.Start(pathfile);
            // Cerrar el objeto SLDocument

            sl.Dispose();
            conexion.Close();
        }
    }
}
