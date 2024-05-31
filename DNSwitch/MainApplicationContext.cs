using DNSwitch.Properties;
using System.Windows.Forms;

namespace DNSwitch
{
    internal class MainApplicationContext : ApplicationContext
    {
        readonly NotifyIcon notifyIcon;

        public MainApplicationContext()
        {
            notifyIcon = new()
            {
                Text = "Dextop",
                Visible = true,
                Icon = Resources.LightIcon,
            };

            notifyIcon.DoubleClick += (_, _) => Application.Exit();
            Application.ApplicationExit += (_, _) => notifyIcon.Visible = false;
        }
    }
}
