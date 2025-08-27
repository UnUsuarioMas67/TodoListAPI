using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TodoListAPI.Enums;

namespace TodoListAPI.Models;

public class PaginationParams
{
    [FromQuery(Name = "page"), DefaultValue(1), Range(1, 10000)]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "limit"), DefaultValue(10), Range(1, 10000)]
    public int PageSize { get; set; } = 10;

    [FromQuery(Name = "search_string"), DefaultValue("")]
    public string SearchString { get; set; } = "";
    
    [FromQuery(Name = "sort_by"), DefaultValue(SortOption.Id)] 
    public SortOption SortOption { get; set; } = SortOption.Id;
}