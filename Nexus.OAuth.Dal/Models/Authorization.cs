using Nexus.OAuth.Dal.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Dal.Models;

public class Authorization
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Authorization Unique Token
    /// </summary>
    [Required]
    [StringLength(500, MinimumLength = 96)]
    public string Token { get; set; }
    [Required]
    [StringLength(500, MinimumLength = 96)]
    public string RefreshToken { get; set; }
    [Required]
    public DateTime Created { get; set; }
    public TimeSpan? ExpiresIn { get; set; }
    [Required]
    public TokenType TokenType { get; set; }
    [Required]
    public int ApplicationId { get; set; }
    [Required]
    public int AccountId { get; set; }

    [ForeignKey(nameof(AccountId))]
    public Account Account { get; set; }
    [ForeignKey(nameof(ApplicationId))]
    public Application Application { get; set; }
}
