namespace Bll
{
    public class User
    {
        private int id;
        private string login;
        private string password;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Login
        {
            get { return login; }
            set { login = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}