using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.Models
{
    [Index(nameof(Path), IsUnique = true)]
    public class Folder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Path were the file is located with no file name
        /// </summary>
        [MaxLength(256), Required]
        public string Path { get; set; } = "";
        
        public List<File> Files { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; } = DateTime.MinValue;

        public List<Tag>? Tags { get; set; }

    }
}
