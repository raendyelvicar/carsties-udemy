using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models.SearchModels;

namespace SearchService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams param){
            var query = DB.PagedSearch<Item, Item>();

            if(!string.IsNullOrEmpty(param.SearchTerm)){
                query.Match(Search.Full, param.SearchTerm).SortByTextScore();
            }
            
            query = param.SortColumn switch
            {
                "make" => query.Sort(x => x.Ascending(a => a.Make)),
                "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
                _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
            };

            query = param.FilterBy switch
            {
                "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
                "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) 
                                                && x.AuctionEnd > DateTime.UtcNow),
                _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
            };

          
            if(!string.IsNullOrEmpty(param.Seller)){
                query.Match(x => x.Seller == param.Seller);
            }

            if(!string.IsNullOrEmpty(param.Winner)){
                query.Match(x => x.Winner == param.Winner);
            }

            query.PageNumber(param.PageNumber);
            query.PageSize(param.PageSize);

            var result = await query.ExecuteAsync();

            return Ok(new {
                results = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount
            });
        }

    }
}
