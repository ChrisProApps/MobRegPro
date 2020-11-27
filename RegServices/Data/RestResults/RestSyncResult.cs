using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RegServices.Data;


namespace RegServices.Data
{
    public class RestSyncResult : StatusResult
    {
        public List<rsLabel> Labels { get; set; }
        public List<rsLanguage> Languages { get; set; }
        public List<rsArticle> Articles { get; set; }
        public List<rsArticleGroup> ArticleGroups { get; set; }
        public List<rsUser> Users { get; set; }
        public List<rsCar> Cars { get; set; }

        public RestSyncResult() { }
    }
}