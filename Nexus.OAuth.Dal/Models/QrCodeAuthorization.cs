using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Dal.Models
{
    public class QrCodeAuthorization
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AccountId { get; set; }
        [Required]
        public int QrCodeReferenceId { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }

        [Required]
        public DateTime AuthorizeDate { get; set; }
        [Required]
        public bool Valid { get; set; }
        [StringLength(500, MinimumLength = 56)]
        public string Token { get; set; }

        [ForeignKey(nameof(QrCodeReferenceId))]
        public QrCodeReference QrCodeReference { get; set; }
    }
}
