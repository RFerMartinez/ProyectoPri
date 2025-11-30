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
        public Reparacion ReparacionActual { get; set; }

        // --- PROPIEDAD INTERMEDIARIA PARA EL COMBO ---
        // Esto permite que al cambiar el estado, la pantalla se entere 
        // y bloquee/desbloquee los campos inmediatamente.
        public EstadoReparacion EstadoSeleccionado
        {
            get { return ReparacionActual.Estado; }
            set
            {
                if (ReparacionActual.Estado != value)
                {
                    ReparacionActual.Estado = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EsEditable));          // Bloquea campos
                    OnPropertyChanged(nameof(EsEstadoModificable)); // <--- AGREGAR ESTO (Bloquea el combo)
                }
            }
        }

        // Lógica de bloqueo: No editable si es Reparado, Entregado o NoReparado
        public bool EsEditable
        {
            get
            {
                return ReparacionActual.Estado != EstadoReparacion.Reparado &&
                       ReparacionActual.Estado != EstadoReparacion.Entregado &&
                       ReparacionActual.Estado != EstadoReparacion.NoReparado;
            }
        }

        public bool EsEstadoModificable
        {
            get
            {
                // El combo se bloquea SOLO si ya está Entregado.
                // Si está "Reparado", todavía devuelve true para que puedas pasarlo a "Entregado".
                return ReparacionActual.Estado != EstadoReparacion.Entregado;
            }
        }
        public ObservableCollection<EstadoReparacion> EstadosDisponibles { get; set; }
        public ObservableCollection<Componente> InventarioDisponible { get; set; }
        public Componente ComponenteSeleccionado { get; set; }

        public ICommand GuardarCambiosCommand { get; set; }
        public ICommand AgregarRepuestoCommand { get; set; }
        public ICommand VolverCommand { get; set; }
        public ICommand FacturarCommand { get; set; }

        public DetalleReparacionViewModel(Reparacion reparacion, MainViewModel mainVM)
        {
            ReparacionActual = reparacion;

            EstadosDisponibles = new ObservableCollection<EstadoReparacion>(
                System.Enum.GetValues(typeof(EstadoReparacion)).Cast<EstadoReparacion>()
            );

            InventarioDisponible = new ObservableCollection<Componente>(AlmacenDatos.Instancia.Inventario);

            GuardarCambiosCommand = new RelayCommand(o =>
            {
                MessageBox.Show("Cambios guardados correctamente.");
                mainVM.VistaActual = new GestionReparacionesViewModel(mainVM);
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

            if (ComponenteSeleccionado.Stock <= 0)
            {
                MessageBox.Show("No hay stock de este componente.");
                return;
            }

            ComponenteSeleccionado.Stock--;
            ReparacionActual.RepuestosUsados.Add(ComponenteSeleccionado);

            OnPropertyChanged(nameof(ReparacionActual));
            MessageBox.Show($"Se agregó {ComponenteSeleccionado.Nombre}. Costo actualizado.");
        }

        private void GenerarFactura()
        {
            // Solo facturar si está listo
            if (ReparacionActual.Estado != EstadoReparacion.Reparado &&
                ReparacionActual.Estado != EstadoReparacion.Entregado)
            {
                MessageBox.Show("El equipo debe estar Reparado o Entregado para facturar.");
                return;
            }

            try
            {
                Services.ServicioFacturacion.GenerarFactura(ReparacionActual);
                MessageBox.Show($"¡Factura generada exitosamente!\nRevise su Escritorio.", "Facturación");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}