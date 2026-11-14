using static VibeScreen.WallpaperHost.NativeMethods;

namespace VibeScreen.WallpaperHost;

/// <summary>
/// Incrusta una ventana detrás de los iconos del escritorio (técnica Progman/WorkerW).
/// Windows 11 24H2 cambió la jerarquía: WorkerW pasó a ser hijo de Progman.
/// Se contemplan ambas variantes; hay que validarlo en la máquina real (Fase 1).
/// </summary>
internal static class DesktopHost
{
    public static IntPtr FindWallpaperParent()
    {
        var progman = FindWindow("Progman", null);
        if (progman == IntPtr.Zero) return IntPtr.Zero;

        // Pide a Progman que cree el WorkerW detrás de los iconos.
        SendMessageTimeout(progman, 0x052C, IntPtr.Zero, IntPtr.Zero, 0, 1000, out _);

        // Variante nueva: WorkerW como hijo de Progman.
        var child = FindWindowEx(progman, IntPtr.Zero, "WorkerW", null);
        if (child != IntPtr.Zero) return child;

        // Variante clásica: WorkerW hermano del que contiene SHELLDLL_DefView.
        IntPtr workerW = IntPtr.Zero;
        EnumWindows((top, _) =>
        {
            if (FindWindowEx(top, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
                workerW = FindWindowEx(IntPtr.Zero, top, "WorkerW", null);
            return true;
        }, IntPtr.Zero);

        return workerW != IntPtr.Zero ? workerW : progman;
    }

    public static bool Attach(Form form, Rectangle screenBounds)
    {
        var parent = FindWallpaperParent();
        if (parent == IntPtr.Zero) return false;

        var style = GetWindowLongPtr(form.Handle, GWL_STYLE).ToInt64();
        style = (style & ~WS_POPUP) | WS_CHILD;
        SetWindowLongPtr(form.Handle, GWL_STYLE, (IntPtr)style);

        var ex = GetWindowLongPtr(form.Handle, GWL_EXSTYLE).ToInt64();
        SetWindowLongPtr(form.Handle, GWL_EXSTYLE, (IntPtr)(ex | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE));

        SetParent(form.Handle, parent);

        // Coordenadas de pantalla -> coordenadas del padre.
        var origin = new POINT { X = screenBounds.X, Y = screenBounds.Y };
        ScreenToClient(parent, ref origin);
        return SetWindowPos(form.Handle, IntPtr.Zero, origin.X, origin.Y,
            screenBounds.Width, screenBounds.Height, SWP_NOACTIVATE | SWP_SHOWWINDOW);
    }
}
