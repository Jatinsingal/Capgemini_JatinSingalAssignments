using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace CodeFirstEFInAsp.netcoreDemo.Models
{
    public class Course1
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        
        [Required]
        [Column("Stitle", TypeName="varchar")]
        public string Title { get; set; }
        [Required]
        [MaxLength(220)]
        public string Description { set; get; }
        public float fullprice { set; get; }
        public Author1 Author {  get; set; }

        [ForeignKey("Author")]
        public int AuthorId {  get; set; }
    }
}
