using System.Globalization;

namespace TiemTapHoa;

static class Program
{
    [STAThread]
    static void Main()
    {
        // Để số hiển thị dạng 6.000 và ngày dạng dd/MM/yyyy
        var vi = new CultureInfo("vi-VN");
        CultureInfo.DefaultThreadCurrentCulture = vi;
        CultureInfo.DefaultThreadCurrentUICulture = vi;
        CultureInfo.CurrentCulture = vi;

        ApplicationConfiguration.Initialize();
        Application.Run(new FormQuanLyHangHoa());
    }
}