namespace UsuSite
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Web.UI;
	using System.Net;
	using Finisar.SQLite;

	/// <summary>
	///		Summary description for master.
	/// </summary>
	public class master : UserControl
	{
		protected PlaceHolder phContent;
		protected string _pagename;
		protected string _content;

		int totalHits;
		int currentClientHits;

		private void Page_Init(object sender, EventArgs e)
		{
			Control c = LoadControl(ContentPath);
			phContent.Controls.Add(c);

			UpdateStats();
		}

		void UpdateStats()
		{
			int ip = BitConverter.ToInt32(IPAddress.Parse(Request.UserHostAddress).GetAddressBytes(),0);
			using(SQLiteConnection conn = new SQLiteConnection("Data Source=" + Server.MapPath("~/data/hits.db")+";Version=3"))
			{
				conn.Open();

				using(SQLiteCommand comm = new SQLiteCommand("SELECT SUM(HitCount) FROM Hits AS TotalHits", conn))
					totalHits = Convert.ToInt32(comm.ExecuteScalar());

				using(SQLiteCommand comm = new SQLiteCommand("SELECT HitCount FROM Hits WHERE IP=@ip", conn))
				{
					comm.Parameters.Add("@ip", ip);
					currentClientHits = Convert.ToInt32(comm.ExecuteScalar());
				}

				/*using(SQLiteCommand comm = new SQLiteCommand(currentClientHits != 0 ? "update Hits set HitCount = HitCount + 1 where IP = @IP"
						  : "insert into Hits values (@IP, 1)", conn))
				{
					comm.Parameters.Add("@ip", ip);
					comm.ExecuteNonQuery();
				}*/
			}
		}

		public string PageName
		{
			get { return _pagename; }
			set { _pagename = value; }
		}

		public string ContentPath
		{
			get { return _content; }
			set { _content = value; }
		}

		public int CurrentClientHits { get { return currentClientHits; } }
		public int TotalHits { get { return totalHits; } }

		#region Web Form Designer generated code

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Init += new EventHandler(this.Page_Init);
		}

		#endregion
	}
}
