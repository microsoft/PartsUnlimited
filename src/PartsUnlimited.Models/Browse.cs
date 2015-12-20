using System.Collections.Generic;

namespace PartsUnlimited.Models
{
    public class Browse
    {
        public IEnumerable<dynamic> Products { get; set; }
        public Category Category { get; set; } 
    }
}