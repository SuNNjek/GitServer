namespace GitServer.Settings
{
	public class EmailSettings
    {
		public string ServerUri { get; set; }
		public int ServerPort { get; set; }
		public bool UseSSL { get; set; }
		public bool RequiresAuthentication { get; set; }
		public string User { get; set; }
		public string Password { get; set; }
    }
}
