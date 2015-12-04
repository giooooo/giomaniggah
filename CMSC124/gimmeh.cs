using System;

namespace CMSC124
{
	public partial class gimmeh : Gtk.Dialog
	{
		public string input;
		public gimmeh ()
		{
			this.Build ();
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{	
			input = entry1.Text;
			this.Destroy();
		}
	}
}

