using Microsoft.AspNetCore.Mvc;
using YesSql.Services;
using YesSql.Samples.Web.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using YesSql.Samples.Web.Indexes;
using Microsoft.AspNetCore.Routing;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using YesSql.Core.QueryParser;
using YesSql.Search.ModelBinding;

namespace YesSql.Samples.Web.ViewModels
{
    public class BlogPostViewModel
    {
        public IEnumerable<BlogPost> BlogPosts { get; set; }
        public Filter Search { get; set; }
    }

    public class Filter
    {
        public string Author { get; set; }
        public string SearchText { get; set; }
        public string OriginalSearchText { get; set; }
        public ContentsStatus SelectedFilter { get; set; }

        [ModelBinder(BinderType = typeof(TermModelBinder<BlogPost>), Name = "SearchText")]
        public TermList<BlogPost> TermList { get; set; }

        [BindNever]
        public List<SelectListItem> Filters { get; set; } = new();


    }
    public enum ContentsStatus
    {
        Default,
        Draft,
        Published
    }
}
