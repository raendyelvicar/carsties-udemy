using System;

namespace SearchService.Models.SearchModels;

public class SearchParams
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortColumn { get; set; }
    public string? SortType { get; set; }
    public string? FilterBy { get; set; }
    public string? Seller { get; set; }
    public string? Winner { get; set; }
}
