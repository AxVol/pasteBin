using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace pasteBin.Areas.Home.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public IdentityUser Author { get; set; }
        public PasteModel Paste { get; set; }
        public DateTime CreateAt { get; set; }

        public CommentModel()
        {
            CreateAt = DateTime.Now;
        }
    }
}
