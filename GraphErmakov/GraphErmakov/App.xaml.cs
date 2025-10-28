using System;
using System.Windows;

namespace GraphErmakov
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Обработка глобальных исключений
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                MessageBox.Show($"Необработанное исключение: {args.ExceptionObject}");
            };

            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show($"Ошибка в UI потоке: {args.Exception.Message}\n\n{args.Exception.StackTrace}");
                args.Handled = true;
            };

            try
            {
                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске приложения: {ex.Message}");
                Shutdown();
            }
        }
    }
}