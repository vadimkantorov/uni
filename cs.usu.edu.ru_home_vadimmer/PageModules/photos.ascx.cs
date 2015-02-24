namespace UsuSite.PageModules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.IO;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	public class Photo
	{
		private readonly string fullImageName;

		public string FullImageName { get { return fullImageName; } }

		public Photo(string fullImageName)
		{
			this.fullImageName = fullImageName;
		}
	}

	public class photos : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataList dlPhotos;

		IEnumerable GetPhotoList()
		{
			ArrayList list = new ArrayList();
			foreach(string fileName in Directory.GetFiles(Server.MapPath("~\\images\\full\\"),"*.jpg"))
				list.Add(new Photo(Path.GetFileName(fileName)));
			return list;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
			{
				dlPhotos.DataSource = GetPhotoList();
				dlPhotos.DataBind();
			}
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
