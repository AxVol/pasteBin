using pasteBin.Areas.Home.Models;

namespace pasteBin.Areas.Home.ViewModels
{
    public class PasteViewModel
    {
        public PasteModel Paste { get; set; }
        public IEnumerable<CommentModel> Comments { get; set; }

        public PasteViewModel(PasteModel paste, IEnumerable<CommentModel> comments) 
        {
            Paste = paste;
            Comments = comments;
        }
    }
}
