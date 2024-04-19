namespace CliniCare360.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Prestazioni")]
    public partial class Prestazioni
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Prestazioni()
        {
            Appuntamenti = new HashSet<Appuntamenti>();
        }

        [Key]
        public int PrestazioneId { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required]
        public string Descrizione { get; set; }

        [Display(Name = "Immagine Prestazione")]
        public byte[] ImgServizio { get; set; }


        
        public decimal Costo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Appuntamenti> Appuntamenti { get; set; }
    }
}
