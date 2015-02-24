namespace UsuSite.PageModules
{
	using System;
	using System.IO;
	using System.Xml.Serialization;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	public class Entry
	{
		public string Name { get { return name; } set { name = value; } }
		public string Email { get { return email; } set { email = value; } }
		public string Message { get { return message; } set { message = value; } }
		public DateTime SubmittedAt { get { return submittedAt; } set { submittedAt = value; } }

		string name, email, message;
		DateTime submittedAt;

		public Entry() { }

		public Entry(string name, string email, string message, DateTime submittedAt)
		{
			this.name = name;
			this.email = email;
			this.message = message;
			this.submittedAt = submittedAt;
		}
	}

	public class guestbook : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.TextBox tbName;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator1;
		protected System.Web.UI.WebControls.TextBox tbEmail;
		protected System.Web.UI.WebControls.RegularExpressionValidator RegularExpressionValidator1;
		protected System.Web.UI.WebControls.Button btnSubmit;
		protected System.Web.UI.WebControls.DataList dlEntries;
		protected System.Web.UI.WebControls.TextBox tbMessage;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
				RefreshEntries();
		}

		void RefreshEntries()
		{
			dlEntries.DataSource = GetEntries();
			dlEntries.DataBind();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
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
			this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnSubmit_Click(object sender, System.EventArgs e)
		{
			if(Page.IsValid)
			{
				AddEntry(new Entry(tbName.Text, tbEmail.Text, tbMessage.Text, DateTime.Now));
				RefreshEntries();
				
				tbName.Text = tbEmail.Text = tbMessage.Text = "";
			}
		}

		void AddEntry(Entry e)
		{
			Entry[] entries = GetEntries();
			Entry[] newEntries = new Entry[entries.Length+1];
			newEntries[0] = e;
			Array.Copy(entries, 0, newEntries, 1, entries.Length);
			using(StreamWriter sw = new StreamWriter(Server.MapPath(guestBookXml)))
				ser.Serialize(sw, newEntries);
		}

		Entry[] GetEntries()
		{
			try 
			{
				using(StreamReader sr = new StreamReader(Server.MapPath(guestBookXml)))
					return (Entry[])ser.Deserialize(sr);
			}
			catch
			{
				return new Entry[0];
			}
		}

		const string guestBookXml = "~\\data\\guestbook.xml";
		static XmlSerializer ser = new XmlSerializer(typeof(Entry[]));
	}
}
