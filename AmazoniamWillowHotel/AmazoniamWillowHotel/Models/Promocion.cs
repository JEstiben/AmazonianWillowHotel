//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AmazoniamWillowHotel.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Promocion
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public Nullable<int> descuento { get; set; }
        public Nullable<System.DateTime> inicio { get; set; }
        public Nullable<System.DateTime> fin { get; set; }
        public int tipoHabitacion { get; set; }
    
        public virtual Tipo_Habitacion Tipo_Habitacion { get; set; }
    }
}
