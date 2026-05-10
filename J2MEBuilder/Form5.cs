using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace J2MEBuilder
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private string FormatBytesToMB(long bytes)
        {
            return (bytes / (1024.0 * 1024.0)).ToString("0.00");
        }

        private WebClient webClient;

        private void DownloadLatestRelease()
        {
            try
            {
                webClient = new WebClient();
                webClient.Headers.Add("User-Agent", "J2MEBuilder/1.0");
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                string downloadUrl = "https://github.com/ojdkbuild/ojdkbuild/releases/download/1.8.0.151-1/java-1.8.0-openjdk-1.8.0.151-1.b12.ojdkbuild.windows.x86_64.msi";
                string savePath = Path.Combine(Path.GetTempPath(), "jdk-8-ojdkbuild-x64.msi");

                LogMessage("Скачивание JDK 8...");

                UpdateProgress(0, 0, 0);

                webClient.DownloadProgressChanged -= WebClient_DownloadProgressChanged;
                webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;

                webClient.DownloadFileCompleted -= WebClient_DownloadFileCompleted;
                webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

                LogMessage($"Сохранение в: {savePath}");

                webClient.DownloadFileAsync(new Uri(downloadUrl), savePath);

                LogMessage("Загрузка запущена, ожидаем прогресс...");
            }
            catch (WebException ex)
            {
                LogMessage($"Ошибка сети: {ex.Message}");
                richTextBox1.Text = $"Network error: {ex.Message}";
            }
            catch (Exception ex)
            {
                LogMessage($"Ошибка: {ex.Message}");
                richTextBox1.Text = $"Error: {ex.Message}";
            }
        }

        private void UpdateProgress(int percent, long received, long total)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action(() =>
                {
                    progressBar1.Value = Math.Max(0, Math.Min(100, percent));

                    string receivedMB = FormatBytesToMB(received);
                    string totalMB = FormatBytesToMB(total);

                    labelMB.Text = Properties.Settings.Default.language == "English"
                        ? $"Downloading: {percent}% {receivedMB} MB / {totalMB} MB"
                        : $"Скачивается: {percent}% {receivedMB} МБ / {totalMB} МБ";
                }));
            }
            else
            {
                progressBar1.Value = Math.Max(0, Math.Min(100, percent));

                string receivedMB = FormatBytesToMB(received);
                string totalMB = FormatBytesToMB(total);

                labelMB.Text = Properties.Settings.Default.language == "English"
                    ? $"Downloading: {percent}% {receivedMB} MB / {totalMB} MB"
                    : $"Скачивается: {percent}% {receivedMB} МБ / {totalMB} МБ";
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            UpdateProgress(e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string savePath = Path.Combine(Path.GetTempPath(), "jdk-8-ojdkbuild-x64.msi");

            if (e.Error == null && !e.Cancelled)
            {
                try
                {
                    long fileSize = new FileInfo(savePath).Length;
                    UpdateProgress(100, fileSize, fileSize);

                    LogMessage($"Загрузка завершена! Размер: {FormatBytesToMB(fileSize)} МБ");

                    if (!File.Exists(savePath))
                    {
                        LogMessage("Ошибка: файл не найден после загрузки!");
                        MessageBox.Show(
                            Properties.Settings.Default.language == "English"
                                ? "Downloaded file not found!"
                                : "Скачанный файл не найден!",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    DialogResult result = MessageBox.Show(
                        Properties.Settings.Default.language == "English"
                            ? $"JDK 8 downloaded successfully!\n\nFile: {savePath}\n\nRun installer now?"
                            : $"JDK 8 успешно скачана!\n\nФайл: {savePath}\n\nЗапустить установщик сейчас?",

                        Properties.Settings.Default.language == "English" ? "Download Complete" : "Скачивание завершено",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            LogMessage("Запуск установщика через msiexec...");

                            ProcessStartInfo psi = new ProcessStartInfo("msiexec.exe", $"/i \"{savePath}\"");
                            psi.UseShellExecute = true;
                            psi.Verb = "runas";
                            Process.Start(psi);

                            LogMessage("Установщик запущен (запрос прав администратора)...");
                        }
                        catch (Win32Exception ex)
                        {
                            if (ex.NativeErrorCode == 1223)
                            {
                                LogMessage("Установка отменена: пользователь отказал в правах администратора.");
                            }
                            else
                            {
                                LogMessage($"Ошибка Win32: {ex.Message} (код {ex.NativeErrorCode})");
                                MessageBox.Show(
                                    Properties.Settings.Default.language == "English"
                                        ? "Failed to start installer: " + ex.Message
                                        : "Не удалось запустить установщик: " + ex.Message,
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogMessage($"Ошибка запуска: {ex.Message}");
                            MessageBox.Show(
                                Properties.Settings.Default.language == "English"
                                    ? "Error: " + ex.Message
                                    : "Ошибка: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        LogMessage($"Файл сохранён: {savePath}");
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Ошибка: {ex.Message}");
                }
            }
            else if (e.Error != null)
            {
                LogMessage($"Ошибка загрузки: {e.Error.Message}");
                UpdateProgress(0, 0, 0);
            }

            webClient?.Dispose();
            webClient = null;
        }

        private void LogMessage(string msg)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(msg + Environment.NewLine);
                    richTextBox1.ScrollToCaret();
                }));
            }
            else
            {
                richTextBox1.AppendText(msg + Environment.NewLine);
                richTextBox1.ScrollToCaret();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DownloadLatestRelease();
        }
    }
}