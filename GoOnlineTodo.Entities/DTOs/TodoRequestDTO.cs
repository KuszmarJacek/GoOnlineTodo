using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoOnlineTodo.Entities.DTOs
{
    public class TodoRequestDto
    {
        public DateTime ExpiryDateTime { get; set; }
        public string Title { get; set; } = String.Empty;
        public string? Description { get; set; }
        public double PercentComplete { get; set; }
    }
}
