using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDemo.Dtos
{
    public class ProductDto
    {
        public int Id { set; get; }

        public string Name { set; get; }

        public float Price { set; get; }

        public ICollection<MaterialDto> Materials { get; set; }
    }

    public class MaterialDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
