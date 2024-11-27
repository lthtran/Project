using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteThuCungBento.Models;

namespace WebsiteThuCungBento.ViewModels
{
    public class CombinedViewModel
    {
        public List<ProductViewModels> Products { get; set; }
        public ListDVViewModel ListDV { get; set; }
    }
}
