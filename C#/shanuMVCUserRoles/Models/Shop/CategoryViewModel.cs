using shanuMVCUserRoles.DTO;

namespace shanuMVCUserRoles.Models
{
    public class CategoryViewModel
    {
        public CategoryViewModel()
        { }
        public CategoryViewModel(Category category)
        {
            Id = category.Id;
            Name = category.Name;
            Meta_Title = category.Meta_Title;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Meta_Title { get; set; }
    }
}