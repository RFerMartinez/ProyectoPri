using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using proyecto_paradigmas_2025.Core;
using proyecto_paradigmas_2025.Data;
using proyecto_paradigmas_2025.Models;

namespace proyecto_paradigmas_2025.ViewModels
{
    public class DetalleReparacionViewModel : ViewModelBase
    {
        // El objeto que estamos editando
        public Reparacion ReparacionActual { get; set; }

        public bool EsEditable
        {
            get
            {
                // Solo es editable si NO está reparado y NO está entregado y NO está 'NoReparado'
                return ReparacionActual.Estado != EstadoReparacion.Reparado &&
                       ReparacionActual.Estado != EstadoReparacion.Entregado &&
                       ReparacionActual.Estado != EstadoReparacion.NoReparado;
            }
        }

        // Listas para los ComboBox
        public ObservableCollection<EstadoReparacion> EstadosDisponibles { get; set; }
        public ObservableCollection<Componente> InventarioDisponible { get; set; }

        // Selección del ComboBox de Repuestos
        public Componente ComponenteSeleccionado { get; set; }

        // Comandos
        public ICommand GuardarCambiosCommand { get; set; }
        public ICommand AgregarRepuestoCommand { get; set; }
        public ICommand VolverCommand { get; set; }
        public ICommand FacturarCommand { get; set; }

        // Constructor que recibe la reparación seleccionada
        public DetalleReparacionViewModel(Reparacion reparacion, MainViewModel mainVM)
        {
            ReparacionActual = reparacion;

            // Cargar estados del Enum
            EstadosDisponibles = new ObservableCollection<EstadoReparacion>(
                System.Enum.GetValues(typeof(EstadoReparacion)).Cast<EstadoReparacion>()
            );

            // Cargar inventario desde el Singleton
            InventarioDisponible = new ObservableCollection<Componente>(AlmacenDatos.Instancia.Inventario);

            GuardarCambiosCommand = new RelayCommand(o =>
            {
                // Al ser referencia en memoria, los cambios ya están en el objeto.
                // Solo mostramos confirmación.
                MessageBox.Show("Cambios guardados correctamente.");
                mainVM.VistaActual = new GestionReparacionesViewModel(mainVM); // Volver a la lista
            });

            VolverCommand = new RelayCommand(o =>
            {
                mainVM.VistaActual = new GestionReparacionesViewModel(mainVM);
            });

            AgregarRepuestoCommand = new RelayCommand(AgregarRepuesto);

            FacturarCommand = new RelayCommand(o => GenerarFactura());
        }

        private void AgregarRepuesto(object obj)
        {
            if (ComponenteSeleccionado == null) return;

            // 1. Validar Stock
            if (ComponenteSeleccionado.Stock <= 0)
            {
                MessageBox.Show("No hay stock de este componente.");
                return;
            }

            // 2. Lógica de Negocio: Descontar Stock y Agregar a la Reparación
            ComponenteSeleccionado.Stock--; // Resta 1 al inventario global
            ReparacionActual.RepuestosUsados.Add(ComponenteSeleccionado);

            // 3. Avisar a la vista que actualice los totales
            OnPropertyChanged(nameof(ReparacionActual));
            MessageBox.Show($"Se agregó {ComponenteSeleccionado.Nombre}. Costo actualizado.");
        }

        private void GenerarFactura()
        {
            // Validación opcional: No facturar si no está terminado
            if (ReparacionActual.Estado != EstadoReparacion.Reparado &&
                ReparacionActual.Estado != EstadoReparacion.Entregado)
            {
                MessageBox.Show("Solo se pueden facturar equipos 'Reparados' o 'Entregados'.");
                return;
            }

            try
            {
                // LLAMADA AL SERVICIO
                Services.ServicioFacturacion.GenerarFactura(ReparacionActual);

                MessageBox.Show($"¡Factura generada exitosamente!\nRevise su Escritorio.", "Facturación");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error al generar factura: {ex.Message}");
            }
        }
    }
}