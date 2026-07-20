using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Content.Server.Database;

[Table("rw_brainrot_triggers")]
[Index(nameof(Trigger), IsUnique = true)]
public sealed class RwBrainrotTrigger
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Trigger { get; set; } = string.Empty;
}
