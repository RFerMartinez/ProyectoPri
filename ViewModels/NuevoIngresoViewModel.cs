using System;
using System.Linq; // Necesario para FirstOrDefault
using System.Windows;
using System.Windows.Input;
using proyecto_paradigmas_2025.Core;
using proyecto_paradigmas_2025.Data;
using proyecto_paradigmas_2025.Models;
using proyecto_paradigmas_2025.Models.Equipos;

namespace proyecto_paradigmas_2025.ViewModels
{
    public class NuevoIngresoViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;

        // --- 1. Datos del Cliente ---
        public string NombreCliente { get; set; }
        public string DNI { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; } // <--- NUEVO CAMPO

        // --- 2. Selección de Tipo ---
        private bool _esCelular = true;
        public bool EsCelular
        {
            get { return _esCelular; }
            set
            {
                _esCelular = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EsComputadora));
            }
        }
        public bool EsComputadora
        {
            get { return !EsCelular; }
            set
            {
                if (value) EsCelular = false;
            }
        }

        // --- 3. Datos Comunes del Equipo ---
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Falla { get; set; }

        // --- 4. Datos Específicos ---
        public string IMEI { get; set; }
        public string Patron { get; set; }
        public string SistemaOperativo { get; set; }
        public bool IncluyeCargador { get; set; }

        public ICommand GuardarCommand { get; set; }

        // Constructor recibe MainViewModel para poder navegar
        public NuevoIngresoViewModel(MainViewModel mainVM)
        {
            _mainViewModel = mainVM;
            GuardarCommand = new RelayCommand(Guardar);
        }

        // Constructor vacío para evitar errores en tiempo de diseño
        public NuevoIngresoViewModel() { }

        private void Guardar(object obj)
        {
            // 1. Validaciones
            if (string.IsNullOrEmpty(NombreCliente) || string.IsNullOrEmpty(Marca) || string.IsNullOrEmpty(DNI))
            {
                MessageBox.Show("Nombre, DNI y Marca son obligatorios.");
                return;
            }

            // 2. Buscar o Crear Cliente
            var clienteFinal = AlmacenDatos.Instancia.Clientes
                                .FirstOrDefault(c => c.DNI == this.DNI);

            if (clienteFinal == null)
            {
                // Nuevo Cliente
                clienteFinal = new Cliente
                {
                    Id = new Random().Next(100, 9999),
                    NombreCompleto = NombreCliente,
                    DNI = DNI,
                    Telefono = Telefono,
                    Email = Email // <--- Guardamos el Email
                };
                AlmacenDatos.Instancia.Clientes.Add(clienteFinal);
            }
            else
            {
                // Cliente Existente: Actualizamos datos de contacto
                clienteFinal.NombreCompleto = NombreCliente;
                clienteFinal.Telefono = Telefono;
                clienteFinal.Email = Email; // <--- Actualizamos el Email
            }

            // 3. Crear Equipo
            Equipo equipoNuevo;
            if (EsCelular)
            {
                equipoNuevo = new Celular
                {
                    Marca = Marca,
                    Modelo = Modelo,
                    DescripcionFalla = Falla,
                    IMEI = IMEI,
                    CodigoDesbloqueo = Patron,
                    TienePatron = !string.IsNullOrEmpty(Patron)
                };
            }
            else
            {
                equipoNuevo = new Computadora
                {
                    Marca = Marca,
                    Modelo = Modelo,
                    DescripcionFalla = Falla,
                    SistemaOperativo = SistemaOperativo,
                    IncluyeCargador = IncluyeCargador
                };
            }
            equipoNuevo.Id = new Random().Next(100, 9999);

            // 4. Crear Reparación
            var nuevaReparacion = new Reparacion
            {
                Id = new Random().Next(100, 9999),
                Cliente = clienteFinal,
                Equipo = equipoNuevo,
                FechaIngreso = DateTime.Now,
                Estado = EstadoReparacion.EnEspera,
                ManoDeObra = 0
            };

            AlmacenDatos.Instancia.Reparaciones.Add(nuevaReparacion);

            MessageBox.Show($"Ingreso registrado correctamente.\nCliente: {clienteFinal.NombreCompleto}");

            // 5. Limpiar y Redirigir
            LimpiarFormulario();

            // Navegamos a la lista de reparaciones pasando el MainViewModel
            _mainViewModel.VistaActual = new GestionReparacionesViewModel(_mainViewModel);
        }

        private void LimpiarFormulario()
        {
            NombreCliente = string.Empty;
            DNI = string.Empty;
            Telefono = string.Empty;
            Email = string.Empty;
            Marca = string.Empty;
            Modelo = string.Empty;
            Falla = string.Empty;
            IMEI = string.Empty;
            Patron = string.Empty;
            SistemaOperativo = string.Empty;
            IncluyeCargador = false;

            // Notificamos a la vista que todo se vació
            OnPropertyChanged(nameof(NombreCliente));
            OnPropertyChanged(nameof(DNI));
            OnPropertyChanged(nameof(Telefono));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(Marca));
            OnPropertyChanged(nameof(Modelo));
            OnPropertyChanged(nameof(Falla));
            OnPropertyChanged(nameof(IMEI));
            OnPropertyChanged(nameof(Patron));
            OnPropertyChanged(nameof(SistemaOperativo));
            OnPropertyChanged(nameof(IncluyeCargador));
        }
    }
}