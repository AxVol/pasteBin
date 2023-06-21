namespace pasteBin.Services
{
    public interface IHashGenerator
    {
        public string HashForURL();
        public string PasswordHash(string password);
    }
}
