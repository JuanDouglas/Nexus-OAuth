using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Dal.Models;

public class Application
{
    public int Id { get; set; }

    /// <summary>
    /// Application Name
    /// </summary>
    [Required]
    [StringLength(150, MinimumLength = 5)]
    public string Name { get; set; }

    [Required]
    public int OwnerId { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 96)]
    public string Secret { get; set; }
    [Required]
    [StringLength(500, MinimumLength = 16)]
    public string Key { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public Account Owner { get; set; }

}

