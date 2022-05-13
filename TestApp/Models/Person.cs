

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApp.Models;

public class Person
{
    [Key]
    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    [Column(TypeName="Date")]
    public DateTime Date { get; set; }
    
    [Required]
    public bool Married { get; set; }
    
    [Required]
    public decimal Salary { get; set; }
}