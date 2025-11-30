using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // <--- 1. AGREGAR ESTO IMPORTANTE
using proyecto_paradigmas_2025.Models.Base;
using proyecto_paradigmas_2025.Models.Equipos;

namespace proyecto_paradigmas_2025.Models
{
    public enum EstadoReparacion
    {
        EnEspera,
        EnDiagnostico,
        EnReparacion,
        Reparado,
        Entregado,
        NoReparado
    }

    public class Reparacion : EntidadBase
    {
        public Cliente Cliente { get; set; }
        public Equipo Equipo { get; set; }

        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public EstadoReparacion Estado { get; set; }
        public string DiagnosticoTecnico { get; set; }

        public decimal ManoDeObra { get; set; }
        public decimal Senia { get; set; }

        // --- 2. EL CAMBIO MÁGICO ---
        // Cambiamos List por ObservableCollection.
        // Esto hace que la pantalla se entere automáticamente cuando haces .Add()
        public ObservableCollection<Componente> RepuestosUsados { get; set; } = new ObservableCollection<Componente>();

        // Nueva Propiedad Calculada: GANANCIA REAL
        // (Mano de Obra + (Precio Venta Repuesto - Precio Costo Repuesto))
        public decimal GananciaNeta
        {
            get
            {
                decimal gananciaRepuestos = 0;
                foreach (var r in RepuestosUsados)
                {
                    gananciaRepuestos += (r.PrecioVenta - r.PrecioCosto);
                }
                return ManoDeObra + gananciaRepuestos;
            }
        }

        public decimal TotalPagar
        {
            get
            {
                decimal costoRepuestos = 0;
                foreach (var r in RepuestosUsados)
                {
                    costoRepuestos += r.PrecioVenta;
                }
                return ManoDeObra + costoRepuestos;
            }
        }

        public decimal SaldoPendiente => TotalPagar - Senia;
    }
}