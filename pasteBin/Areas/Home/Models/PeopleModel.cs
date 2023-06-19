namespace pasteBin.Areas.Home.Models
{
    public record class PeopleModel
    {
        public string name;
        public int age;

        public PeopleModel(string name, int age)
        {
            this.name = name;
            this.age = age;
        }
    }
}
