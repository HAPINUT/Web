using shanuMVCUserRoles.DTO;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HAPINUT.Models.Pages
{
    public class PageViewModel
    {
        public PageViewModel()
        {

        }
        public PageViewModel(Page page)
        {
            Id = page.Id;
            Title = page.Title;
            Body = page.Body;
            TopicId = page.TopicId;

        }
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }
        public int? TopicId { get; set; }
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 3)]
        [AllowHtml]
        public string Body { get; set; }
    }
}