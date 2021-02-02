using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcMovie.Models
{
    public class MoviePaymentViewModel
    {
        public List<Payment> Payments { get; set; }
        public SelectList Movies { get; set; }
        public string MovieTitle { get; set; }
        public string SearchString { get; set; }
    }
}
