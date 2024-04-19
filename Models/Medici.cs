namespace CliniCare360.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Medici")]
    public partial class Medici
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Medici()
        {
            Appuntamenti = new HashSet<Appuntamenti>();
        }

        [Key]
        public int MedicoId { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required]
        [StringLength(50)]
        public string Cognome { get; set; }

        [Required]
        [StringLength(100)]
        public string Specializzazione { get; set; }

        [Required]
        public string Esperienza { get; set; }

        [Required]
        [StringLength(255)]
        public string PatologieTrattate { get; set; }

  
        public byte[] ImgMedico { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Appuntamenti> Appuntamenti { get; set; }
    }
}
