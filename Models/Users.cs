namespace CliniCare360.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Users()
        {
            Appuntamenti = new HashSet<Appuntamenti>();
        }

        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required]
        [StringLength(50)]
        public string Cognome { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [Display(Name = "Data di nascita")]
        public DateTime DataNascita { get; set; }

        [Required]
        [StringLength(16)]
        [Display(Name = "Codice Fiscale")]
        public string CodFiscale { get; set; }

        [StringLength(100)]
        public string Indirizzo { get; set; }

        [StringLength(150)]
        [Display(Name = "Città")]
        public string Citta { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string Ruolo { get; set; }

        [Display(Name = "Immagine Profilo")]

        public byte[] ImgUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Appuntamenti> Appuntamenti { get; set; }
    }
}
