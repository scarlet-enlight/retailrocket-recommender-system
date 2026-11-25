using System.ComponentModel.DataAnnotations;

namespace RetailRocket.Application.Services.WebAccess;

public class WebAccessSettings
{
    public const string SectionName = "Frontend";
    [Required, Url] public string BaseUrl { get; set; } = "";
}