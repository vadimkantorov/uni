using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Insurance
{
	public partial class RussianKeyboard : UserControl, IVirtualKeyboard
	{
		readonly List<Button> alphas = new List<Button>();
		readonly List<Button> nums = new List<Button>();

		public RussianKeyboard()
		{
			InitializeComponent();
		}

		public event KeyPressEventHandler VirtualKeyPress;

		void InvokeVirtualKeyPress(KeyPressEventArgs e)
		{
			KeyPressEventHandler press = VirtualKeyPress;
			if (press != null) press(this, e);
		}

		Button ConstructButton(char c)
		{
			var btn = new Button
			{
				Dock = DockStyle.Fill,
				Text = c.ToString(),
				Tag = c,
				TabStop = false
			};
			
			btn.Click += (o, ev) => InvokeVirtualKeyPress(new KeyPressEventArgs((char)((Button)o).Tag));
			return btn;
		}

		Button ConstructButtonWithAnotherText(char c, string text)
		{
			var btn = ConstructButton(c);
			btn.Text = text;
			return btn;
		}

		private void Keyboard_Load(object sender, EventArgs e)
		{
			const string spaceText = "ПРОБЕЛ";
			
			var digs = new List<char>();
			var alphas = new List<char>();

			for (char c = '0'; c <= '9'; c++ )
				digs.Add(c);
			for (char c = 'А'; c <= 'Ю'; c++ )
				alphas.Add(c);
			alphas.Add(' ');
			alphas.Add('Я');
			
			var abc = digs.Union(alphas).ToList();

			for (int row = 2; row < tbl.RowCount; row++ )
			{
				for(int col = 0; col < tbl.ColumnCount; col++)
				{
					int ind = (row-2)*tbl.ColumnCount + col;
					if(ind < abc.Count)
					{
						var btn = abc[ind] != ' ' ? ConstructButton(abc[ind]) : ConstructButtonWithAnotherText(' ', spaceText);
						tbl.Controls.Add(btn, col, row);
					}
				}
			}
			tbl.SetColumnSpan(tbl.Controls.Cast<Button>().Where(x=>x.Text == spaceText).Single(), 8);

			var backSpaceBtn = ConstructButtonWithAnotherText('\b', "C");
			backSpaceBtn.Font = new Font(backSpaceBtn.Font.FontFamily, 65);
			tbl.Controls.Add(backSpaceBtn, tbl.ColumnCount-2, 0);
			tbl.SetColumnSpan(backSpaceBtn, 2);
			tbl.SetRowSpan(backSpaceBtn, 2);
		}
	}
}
