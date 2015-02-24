using System.Windows.Forms;

namespace Insurance
{
	public interface IVirtualKeyboard
	{
		event KeyPressEventHandler VirtualKeyPress;
	}
}