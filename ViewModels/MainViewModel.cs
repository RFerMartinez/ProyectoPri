using System.Windows.Input;
using proyecto_paradigmas_2025.Core;

namespace proyecto_paradigmas_2025.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // 1. Propiedad que guarda la vista que se está mostrando actualmente
        private object _vistaActual;
        public object VistaActual
        {
            get { return _vistaActual; }
            set
            {
                _vistaActual = value;
                OnPropertyChanged(); // ¡Avisa a la ventana para que se redibuje!
            }
        }

        // 2. Comandos para los botones del Menú Lateral
        public ICommand MostrarDashboardCommand { get; set; }
        public ICommand MostrarReparacionesCommand { get; set; }
        public ICommand MostrarIngresoCommand { get; set; }
        public ICommand MostrarClientesCommand { get; set; }
        public ICommand MostrarInventarioCommand { get; set; }

        // 3. Constructor
        public MainViewModel()
        {
            // Inicializar los comandos
            MostrarDashboardCommand = new RelayCommand(o => VistaActual = new DashboardViewModel());
            MostrarReparacionesCommand = new RelayCommand(o => VistaActual = new GestionReparacionesViewModel(this)); // Pasamos 'this'
            MostrarIngresoCommand = new RelayCommand(o => VistaActual = new NuevoIngresoViewModel(this));
            MostrarClientesCommand = new RelayCommand(o => VistaActual = new GestionClientesViewModel(this));
            MostrarInventarioCommand = new RelayCommand(o => VistaActual = new GestionInventarioViewModel());

            // Vista por defecto al arrancar
            VistaActual = new DashboardViewModel();
        }
    }
}