using System.Collections.Generic;

namespace PartsUnlimited.Models
{
    public class Browse
    {
        public IEnumerable<IProduct> Products { get; set; }
        public Category Category { get; set; } 
    }
}