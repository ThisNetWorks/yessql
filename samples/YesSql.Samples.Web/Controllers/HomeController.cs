using Microsoft.AspNetCore.Mvc;
using YesSql.Services;
using YesSql.Samples.Web.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using YesSql.Samples.Web.Indexes;
using Microsoft.AspNetCore.Routing;
using System;
using YesSql.Samples.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using YesSql.Search;
using YesSql.Search.ModelBinding;
using YesSql.Core.FilterEngines;
using System.Linq;

namespace YesSql.Samples.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStore _store;

        public HomeController(IStore store
            )
        {
            _store = store;
        }

        [Route("/")]
        public async Task<IActionResult> Index([ModelBinder(BinderType = typeof(TermModelBinder<BlogPost>), Name = "q")] QueryFilterEngine<BlogPost> termList)
        {
            IEnumerable<BlogPost> posts;

            string currentSearchText = String.Empty;

            using (var session = _store.CreateSession())
            {
                var query = session.Query<BlogPost>();

                await termList.ExecuteAsync(query, HttpContext.RequestServices);

                currentSearchText = termList.ToString();

                posts = await query.ListAsync();

                // Map termList to model.
                // i.e. SelectedFilter needs to be filled with
                // the selected filter value from the term.
                var search = new Filter
                {
                    SearchText = currentSearchText,
                    OriginalSearchText = currentSearchText,
                    Filters = new List<SelectListItem>()
                    {
                        new SelectListItem("Select...", ""),
                        new SelectListItem("Published", ContentsStatus.Published.ToString()),
                        new SelectListItem("Draft", ContentsStatus.Draft.ToString())
                    }
                };

                // So this is essentially the mapping. Why make a func for it?
                // var statusTerm = termList.Terms.FirstOrDefault(x => x.TermName == "status");
                // if (statusTerm is TermOperationNode term && term.Operation is UnaryNode op)
                // {
                //     if (Enum.TryParse<ContentsStatus>(op.Value.ToString(), true, out var e))
                //     {
                //         search.SelectedFilter = e;
                //     }
                // }

                termList.MapTo(search);

                var vm = new BlogPostViewModel
                {
                    BlogPosts = posts,
                    Search = search
                };

                return View(vm);
            }
        }

        [HttpPost("/")]
        public IActionResult IndexPost(Filter search)
        {
            // When the user has typed something into the search input no evaluation is required.
            // But we might normalize it for them.
            if (!String.Equals(search.SearchText, search.OriginalSearchText, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index",
                      new RouteValueDictionary
                      {
                        { "q", search.SearchText }
                      }
                  );
            }

            // Not a dictionary because it may contain duplicates.
            /*

            var searchValues = search.TermList.Terms.Select(x => new { name = x.TermName, value = x.ToString() }).ToList();

            if (search.SelectedFilter != ContentsStatus.Default)
            {
                var existingStatusIndex = searchValues.FindIndex(x => x.name == "status");
                if (existingStatusIndex != -1)
                {
                    searchValues.RemoveAt(existingStatusIndex);
                }
                // Here what happens is it needs to replace the existing status value in the string.
                if (existingStatusIndex == -1)
                {
                    existingStatusIndex = searchValues.Count();
                }
                
                searchValues.Insert(existingStatusIndex, new { name = "status", value = "status:" + search.SelectedFilter.ToString().ToLowerInvariant()});                
            }


            return RedirectToAction("Index",
                       new RouteValueDictionary
                       {
                            { "q", string.Join(' ', searchValues.Select(x => x.value)) }
                       }
                   );
                */

            search.TermList.MapFrom(search);


            return RedirectToAction("Index",
                       new RouteValueDictionary
                       {
                            { "q", search.TermList.ToString() }
                       }
                   );                   

        }
    }
}
