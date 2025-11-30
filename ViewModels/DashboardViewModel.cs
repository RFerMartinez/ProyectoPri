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

            // 1. Llenar las columnas del Kanban (Filtrando por estado)
            ListaEnEspera = new ObservableCollection<Reparacion>(
                todas.Where(r => r.Estado == EstadoReparacion.EnEspera));

            ListaEnDiagnostico = new ObservableCollection<Reparacion>(
                todas.Where(r => r.Estado == EstadoReparacion.EnDiagnostico));

            ListaEnReparacion = new ObservableCollection<Reparacion>(
                todas.Where(r => r.Estado == EstadoReparacion.EnReparacion));

            ListaReparado = new ObservableCollection<Reparacion>(
                todas.Where(r => r.Estado == EstadoReparacion.Reparado));

            // 2. Calcular KPI: Total en Proceso
            // Sumamos solo lo que está en el tablero (excluye Entregados y NoReparados)
            TotalEnProceso = ListaEnEspera.Count + ListaEnDiagnostico.Count +
                             ListaEnReparacion.Count + ListaReparado.Count;

            // 3. Calcular KPI: Ganancia Estimada
            // Tu regla de negocio: Se tiene en cuenta para ganancias si está "Reparado" o "Entregado".
            var equiposCobrables = todas.Where(r =>
                                    r.Estado == EstadoReparacion.Reparado ||
                                    r.Estado == EstadoReparacion.Entregado);

            GananciaEstimada = equiposCobrables.Sum(r => r.GananciaNeta);
        }
    }
}