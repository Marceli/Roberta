namespace Core.Entities
{
	public class User
	{
		private int id;
		private string login;
		private string password;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Login
		{
			get { return login; }
			set { login = value; }
		}

		public virtual string Password
		{
			get { return password; }
			set { password = value; }
		}
	}
}