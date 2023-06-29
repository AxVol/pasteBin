using Microsoft.AspNetCore.Identity;

namespace pasteBin.Areas.Home.Models
{
    public class ReportModel
    {
        public int Id { get; set; }
        public string ReportText { get; set; }
        public IdentityUser User { get; set; }
        public PasteModel Paste { get; set; }
    }
}
