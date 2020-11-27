using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegServices.Data
{
    public class rsArticleReg
    {
        public string ArticleID { get; set; }
        public string Article { get; set; } //currently not used can be used as Alias
        public Guid OrderID { get; set; }
        public Guid PlanningID { get; set; }
        public decimal Qty {get;set;}
        public decimal? PriceIn {get;set;}
        public decimal? PriceOut {get;set;}

         // Datamanagment
        public bool IsChanged { get; set; }
        public bool IsDeleted { get; set; }
        

        public rsArticleReg()
        {
            PriceIn = PriceOut = null;
            IsChanged = false;
            IsDeleted = false;
        }
    }
}
