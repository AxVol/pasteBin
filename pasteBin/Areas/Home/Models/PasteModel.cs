using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace pasteBin.Areas.Home.Models
{
    public class PasteModel
    {
        private readonly int offsetDelete = 30;
        private string topic;
        private DateTime? deleteDate;

        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        public int Id { get; set; }
        public IdentityUser? Author { get; set; }
        public int View { get; set; } = 0;
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteDate 
        {
            get => deleteDate;
            set
            {
                if (value == null || value < DateTime.Now)
                {
                    deleteDate = DateTime.Now.AddDays(offsetDelete);

                    return;
                }
                
                deleteDate = value;
            }
        }
        public string? Topic
        {
            get => topic;
            set
            {
                if (value == null)
                {
                    topic = "Описание отсутствует";
                    return;
                }

                topic = value;
            }
        }

        public PasteModel() 
        {
            CreateAt = DateTime.Now;
        }
    }
}
