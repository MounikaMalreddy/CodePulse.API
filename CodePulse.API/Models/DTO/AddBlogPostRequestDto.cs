﻿namespace CodePulse.API.Models.DTO
{
    public class AddBlogPostRequestDto
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string UrlHandle { get; set; }
        public DateTime DateCreated { get; set; }
        public string Author { get; set; }
        public bool IsVisible { get; set; }
        public Guid[] Categories { get; set; }
    }
}
