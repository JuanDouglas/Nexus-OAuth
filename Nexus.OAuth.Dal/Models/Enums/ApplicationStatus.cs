using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Dal.Models.Enums;

/// <summary>
/// Application Status
/// </summary>
public enum ApplicationStatus : sbyte
{
    /// <summary>
    /// Application disabled status
    /// </summary>
    Disabled,
    /// <summary>
    /// Application Testing status
    /// </summary>
    Testing,
    /// <summary>
    /// Application active
    /// </summary>
    Active
}

