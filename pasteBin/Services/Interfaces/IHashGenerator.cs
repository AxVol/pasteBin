namespace pasteBin.Services.Interfaces
{
    public interface IHashGenerator
    {
        public string HashForURL();
        public string PasswordHash(string password);
    }
}
