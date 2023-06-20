using System.ComponentModel.DataAnnotations;

namespace pasteBin.Areas.Home.Models
{
    public class PasteModel
    {
        private readonly int offsetDelete = 30;
        private string description;
        private DateTime? deleteDate;

        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        public string? Author { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? DeleteDate 
        {
            get
            {
                return deleteDate;
            }
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
        public string? Description
        {
            get => description;
            set
            {
                if (value == null)
                {
                    description = "Описание отсутствует";
                    return;
                }

                description = value;
            }
        }

        public PasteModel() 
        {
            CreateAt = DateTime.Now;
        }
    }
}
