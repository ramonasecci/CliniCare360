namespace CliniCare360.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Appuntamenti")]
    public partial class Appuntamenti
    {
        [Key]
        public int AppId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Data { get; set; }

        public TimeSpan Ora { get; set; }

        [Required]
        [StringLength(50)]
        public string Stato { get; set; }

        public string NoteVisita { get; set; }

        public string Prescrizione { get; set; }

        public int? UserId { get; set; }

        public int? MedicoId { get; set; }

        [Display(Name = "Prestazione")]
        public int PrestazioneId { get; set; }

        public virtual Medici Medici { get; set; }

        public virtual Prestazioni Prestazioni { get; set; }

        public virtual Users Users { get; set; }
    }
}
