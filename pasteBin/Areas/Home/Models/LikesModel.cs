using Microsoft.AspNetCore.Identity;

namespace pasteBin.Areas.Home.Models
{
    public class LikesModel
    {
        public int Id { get; set; }
        public PasteModel Paste { get; set; }
        public IdentityUser User { get; set; }
    }
}
