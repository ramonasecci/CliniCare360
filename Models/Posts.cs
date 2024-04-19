namespace CliniCare360.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Posts
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        [StringLength(50)]
        public string Titolo { get; set; }

        [Required]
        public string Contenuto { get; set; }

        [Display(Name = "Immagine Post")]
        public byte[] ImgPost { get; set; }

        [Required]
        [StringLength(20)]
        public string Tipo { get; set; }

        public DateTime DataOraPublic { get; set; }
    }
}
