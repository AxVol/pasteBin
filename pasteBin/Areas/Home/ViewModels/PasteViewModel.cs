using Microsoft.AspNetCore.Identity;
using pasteBin.Areas.Home.Models;
using System.Security.Claims;

namespace pasteBin.Areas.Home.ViewModels
{
    public class PasteViewModel
    {
        public PasteModel Paste { get; set; }
        public IEnumerable<CommentModel> Comments { get; set; }
        public IEnumerable<LikesModel> Likes { get; set; }

        public PasteViewModel(PasteModel paste, IEnumerable<CommentModel> comments, IEnumerable<LikesModel> likes)
        {
            Paste = paste;
            Comments = comments;
            Likes = likes;
        }

        public bool IsLiked(string userName)
        {
            LikesModel? isLiked = Likes.FirstOrDefault(l => l.User.UserName == userName);

            if (isLiked == null)
                return false;

            return true;
        }
    }
}
