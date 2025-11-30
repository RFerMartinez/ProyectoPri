using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using proyecto_paradigmas_2025.Core;
using proyecto_paradigmas_2025.Data;
using proyecto_paradigmas_2025.Models;

namespace proyecto_paradigmas_2025.ViewModels
{
    public class GestionInventarioViewModel : ViewModelBase
    {
        // Lista visual en pantalla
        public ObservableCollection<Componente> Inventario { get; set; }

        // El componente que se está editando en el formulario de la derecha
        private Componente _componenteSeleccionado;
        public Componente ComponenteSeleccionado
        {
            get { return _componenteSeleccionado; }
            set
            {
                _componenteSeleccionado = value;
                OnPropertyChanged();
                // Habilitar o deshabilitar botones según si hay algo seleccionado
                OnPropertyChanged(nameof(HaySeleccion));
            }
        }

        // Propiedad auxiliar para la vista (Binding de visibilidad/habilitación)
        public bool HaySeleccion => ComponenteSeleccionado != null;

        // Comandos
        public ICommand NuevoCommand { get; set; }
        public ICommand GuardarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }

        public GestionInventarioViewModel()
        {
            // Cargar datos del Singleton
            Inventario = new ObservableCollection<Componente>(AlmacenDatos.Instancia.Inventario);

            // Inicializamos comandos
            NuevoCommand = new RelayCommand(o => PrepararNuevo());
            GuardarCommand = new RelayCommand(o => Guardar());
            EliminarCommand = new RelayCommand(o => Eliminar());
        }

        private void PrepararNuevo()
        {
            // Creamos un objeto vacío para el formulario
            ComponenteSeleccionado = new Componente
            {
                Id = 0, // Id 0 marca que es NUEVO
                Nombre = "",
                Stock = 0,
                PrecioCosto = 0,
                PrecioVenta = 0
            };
        }

        private void Guardar()
        {
            if (ComponenteSeleccionado == null) return;

            // Validación simple
            if (string.IsNullOrWhiteSpace(ComponenteSeleccionado.Nombre))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return;
            }

            // Lógica: ¿Es Nuevo o Edición?
            if (ComponenteSeleccionado.Id == 0)
            {
                // --- ES NUEVO ---
                // Generar ID (simple autoincremental simulado)
                int nuevoId = Inventario.Count > 0 ? Inventario.Max(x => x.Id) + 1 : 1;
                ComponenteSeleccionado.Id = nuevoId;

                // Agregar al Singleton y a la lista visual
                AlmacenDatos.Instancia.Inventario.Add(ComponenteSeleccionado);
                Inventario.Add(ComponenteSeleccionado);

                MessageBox.Show("Producto agregado correctamente.");
            }
            else
            {
                // 1. GUARDAR REFERENCIA LOCAL:
                // Guardamos el objeto en una variable temporal antes de tocar la lista.
                // Así, aunque el DataGrid ponga 'ComponenteSeleccionado' en null, 'itemAEditar' sigue vivo.
                var itemAEditar = ComponenteSeleccionado;

                int index = Inventario.IndexOf(itemAEditar);

                // 2. Removemos el ítem para forzar a la vista a refrescarse
                // (En este momento, ComponenteSeleccionado se vuelve null automáticamente)
                Inventario.RemoveAt(index);

                // 3. Insertamos usando nuestra VARIABLE LOCAL (que es segura)
                Inventario.Insert(index, itemAEditar);

                // 4. Recuperamos la selección (opcional, para que siga pintado de azul)
                ComponenteSeleccionado = itemAEditar;

                MessageBox.Show("Producto modificado correctamente.");
            }

            // Limpiar formulario
            ComponenteSeleccionado = null;
        }

        private void Eliminar()
        {
            if (ComponenteSeleccionado == null || ComponenteSeleccionado.Id == 0) return;

            var confirm = MessageBox.Show($"¿Seguro de borrar {ComponenteSeleccionado.Nombre}?",
                                          "Confirmar", MessageBoxButton.YesNo);

            if (confirm == MessageBoxResult.Yes)
            {
                AlmacenDatos.Instancia.Inventario.Remove(ComponenteSeleccionado);
                Inventario.Remove(ComponenteSeleccionado);
                ComponenteSeleccionado = null;
            }
        }
    }
}