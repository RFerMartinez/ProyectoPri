using System.Collections.ObjectModel;
using System.Linq;
using proyecto_paradigmas_2025.Core;
using proyecto_paradigmas_2025.Data;
using proyecto_paradigmas_2025.Models;

namespace proyecto_paradigmas_2025.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        // --- KPIS (Indicadores Superiores) ---
        public decimal GananciaEstimada { get; set; }
        public int TotalEnProceso { get; set; }

        // --- COLUMNAS KANBAN (Listas Visuales) ---
        // Usamos ObservableCollection para que si algo cambia, se refleje.
        public ObservableCollection<Reparacion> ListaEnEspera { get; set; }
        public ObservableCollection<Reparacion> ListaEnDiagnostico { get; set; }
        public ObservableCollection<Reparacion> ListaEnReparacion { get; set; }
        public ObservableCollection<Reparacion> ListaReparado { get; set; }

        public DashboardViewModel()
        {
            CargarDatos();
        }

        public void CargarDatos()
        {
            var todas = AlmacenDatos.Instancia.Reparaciones;

            // 1. Llenar las columnas del Kanban
            ListaEnEspera = new ObservableCollection<Reparacion>(
                todas.Where(r => r.Estado == EstadoReparacion.EnEspera));

            ListaEnDiagnostico = new ObservableCollection<Reparacion>(
                todas.Where(r => r.Estado == EstadoReparacion.EnDiagnostico));

            ListaEnReparacion = new ObservableCollection<Reparacion>(
                todas.Where(r => r.Estado == EstadoReparacion.EnReparacion));

            ListaReparado = new ObservableCollection<Reparacion>(
                todas.Where(r => r.Estado == EstadoReparacion.Reparado));

            // 2. Calcular KPI: Total en Proceso (Todo lo activo en taller)
            TotalEnProceso = ListaEnEspera.Count + ListaEnDiagnostico.Count +
                             ListaEnReparacion.Count + ListaReparado.Count;

            // 3. Calcular KPI: Ganancia Estimada (SOLO ENTREGADOS)
            // CAMBIO APLICADO: Ahora solo sumamos si el cliente ya retiró (Pagó).
            var equiposCobrados = todas.Where(r => r.Estado == EstadoReparacion.Entregado);

            GananciaEstimada = equiposCobrados.Sum(r => r.GananciaNeta);
        }
    }
}