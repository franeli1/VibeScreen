using System.Text.Json;

namespace VibeScreen.WallpaperHost;

// Prototipo de viabilidad (Fase 1): imagen estática detrás de los iconos en un monitor.
// Uso: VibeScreen.WallpaperHost --image <ruta> [--monitor <índice>]
// Comandos por stdin: pause | resume | exit. Responde con una línea JSON por evento.
internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        string? image = ArgValue(args, "--image");
        int monitor = int.TryParse(ArgValue(args, "--monitor"), out var m) ? m : 0;

        if (image is null || !File.Exists(image))
        {
            Emit("error", "Indica --image con una ruta existente.");
            return 2;
        }

        var screens = Screen.AllScreens;
        if (monitor < 0 || monitor >= screens.Length)
        {
            Emit("error", $"Monitor {monitor} fuera de rango (hay {screens.Length}).");
            return 2;
        }

        var form = new Form
        {
            FormBorderStyle = FormBorderStyle.None,
            ShowInTaskbar = false,
            StartPosition = FormStartPosition.Manual,
            BackColor = Color.Black,
        };
        form.Controls.Add(new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = Image.FromFile(image),
        });

        bool paused = false;
        form.Shown += (_, _) =>
        {
            var ok = DesktopHost.Attach(form, screens[monitor].Bounds);
            Emit(ok ? "attached" : "error", ok ? $"monitor {monitor}" : "No se pudo incrustar en el escritorio.");
        };

        var reader = new Thread(() =>
        {
            string? line;
            while ((line = Console.In.ReadLine()) is not null)
            {
                switch (line.Trim().ToLowerInvariant())
                {
                    case "pause": paused = true; Emit("paused", ""); break;   // el vídeo usará esto
                    case "resume": paused = false; Emit("resumed", ""); break;
                    case "exit": form.BeginInvoke(form.Close); return;
                }
            }
            form.BeginInvoke(form.Close); // stdin cerrado: Electron murió, salimos.
        }) { IsBackground = true };
        reader.Start();

        Application.Run(form);
        _ = paused;
        return 0;
    }

    private static string? ArgValue(string[] args, string name)
    {
        var i = Array.IndexOf(args, name);
        return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
    }

    private static void Emit(string evt, string detail) =>
        Console.WriteLine(JsonSerializer.Serialize(new { evt, detail }));
}
