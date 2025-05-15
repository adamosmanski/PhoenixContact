using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixContact.EF.Model
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        [MaxLength(50)]
        public string PositionLevel { get; set; }

        [MaxLength(100)]
        public string Residence { get; set; }

        [NotMapped]
        public decimal GrossSalary => Salary * 1.19m;
    }
}
