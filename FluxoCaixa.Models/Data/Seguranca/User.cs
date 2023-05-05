using System.ComponentModel.DataAnnotations.Schema;
using NuvTools.Security.Identity.Models;

namespace FluxoCaixa.Models.Data.Seguranca;

[Table("User", Schema = "Seguranca")]
public class User : UserBase<int>
{
    public short? SistemaId { get; set; }
}