using Microsoft.AspNetCore.Identity;

namespace pasteBin.Areas.Home.Models
{
    public class ViewCheatModel
    {
        public int Id { get; set; }
        public IdentityUser User { get; set; }
        public PasteModel Paste { get; set; }
    }
}
