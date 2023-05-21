using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Model
{
    public enum BookTheme
    {
        Nature,  //выбранная для выделения из останых
        Space,
        Phisics,
        Human
    }
    [Table("Books")]
    public class Book
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Author { get; set; }
        [Required]
        public BookTheme Theme { get; set; }
        [MaxLength(4)]
        public int? YearOfIssue { get; set; }
    }
}
