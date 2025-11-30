using System;
using System.IO; // Necesario para guardar archivos
using System.Text;
using proyecto_paradigmas_2025.Models;

namespace proyecto_paradigmas_2025.Services
{
    public static class ServicioFacturacion
    {
        public static void GenerarFactura(Reparacion reparacion)
        {
            // 1. Construimos el contenido del texto usando StringBuilder
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("========================================");
            sb.AppendLine("           COMPROBANTE DE SERVICIO      ");
            sb.AppendLine("             TECH REPAIR SYSTEM         ");
            sb.AppendLine("========================================");
            sb.AppendLine($"Fecha: {DateTime.Now}");
            sb.AppendLine($"Orden Nro: {reparacion.Id}");
            sb.AppendLine("----------------------------------------");

            sb.AppendLine("DATOS DEL CLIENTE:");
            sb.AppendLine($"Nombre: {reparacion.Cliente.NombreCompleto}");
            sb.AppendLine($"DNI:    {reparacion.Cliente.DNI}");
            sb.AppendLine("----------------------------------------");

            sb.AppendLine("DETALLE DEL EQUIPO:");
            sb.AppendLine($"Equipo: {reparacion.Equipo.NombreCompleto}");
            sb.AppendLine($"Falla:  {reparacion.Equipo.DescripcionFalla}");
            sb.AppendLine($"Diagnóstico: {reparacion.DiagnosticoTecnico}");
            sb.AppendLine("----------------------------------------");

            sb.AppendLine("COSTOS:");
            sb.AppendLine($"Mano de Obra:        $ {reparacion.ManoDeObra}");

            if (reparacion.RepuestosUsados.Count > 0)
            {
                sb.AppendLine("Repuestos:");
                foreach (var repuesto in reparacion.RepuestosUsados)
                {
                    sb.AppendLine($" - {repuesto.Nombre}: $ {repuesto.PrecioVenta}");
                }
            }
            else
            {
                sb.AppendLine("Repuestos:           $ 0");
            }

            sb.AppendLine("========================================");
            sb.AppendLine($"TOTAL A PAGAR:       $ {reparacion.TotalPagar}");
            sb.AppendLine("========================================");
            sb.AppendLine("      ¡Gracias por confiar en nosotros! ");

            // 2. Definimos la ruta (En el Escritorio del usuario)
            string rutaEscritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string nombreArchivo = $"Factura_Orden_{reparacion.Id}.txt";
            string rutaCompleta = Path.Combine(rutaEscritorio, nombreArchivo);

            // 3. Escribimos el archivo
            File.WriteAllText(rutaCompleta, sb.ToString());
        }
    }
}