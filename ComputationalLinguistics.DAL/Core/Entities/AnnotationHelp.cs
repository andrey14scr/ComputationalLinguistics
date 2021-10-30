using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalLinguistics.DAL.Core.Entities
{
    [Index(nameof(TagName), Name = "ITagName")]
    public class AnnotationHelp
    {
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string TagName { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string Info { get; set; }
    }
}
