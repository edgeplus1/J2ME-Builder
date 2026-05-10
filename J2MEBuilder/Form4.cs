using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
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
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://nikita36078.github.io/J2ME_Docs/"));
        }

        private WebClient webClient;

        private void button1_Click(object sender, EventArgs e)
        {
            CheckForUpdate();
        }

        private string FormatBytesToMB(long bytes)
        {
            return (bytes / (1024.0 * 1024.0)).ToString("0.00");
        }

        private void DownloadLatestRelease()
        {
            try
            {
                webClient = new WebClient();
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                webClient.Headers.Add("User-Agent", "Anything");

                string response = webClient.DownloadString("https://api.github.com/repos/edgeplus1/J2ME-Builder/releases");
                JArray releases = JArray.Parse(response);

                if (releases.Count > 0)
                {
                    string downloadUrl = releases[0]["assets"][0]["browser_download_url"].ToString();
                    string savePath = "update.exe";

                    webClient.DownloadProgressChanged += (s, e) =>
                    {
                        progressBar1.Value = e.ProgressPercentage;

                        string receivedMB = FormatBytesToMB(e.BytesReceived);
                        string totalMB = FormatBytesToMB(e.TotalBytesToReceive);

                        labelMB.Text = Properties.Settings.Default.language == "English"
                            ? $"Downloading: {e.ProgressPercentage}% {receivedMB} MB / {totalMB} MB"
                            : $"Скачивается: {e.ProgressPercentage}% {receivedMB} МБ / {totalMB} МБ";
                    };

                    webClient.DownloadFileCompleted += (s, e) =>
                    {
                        if (e.Error == null)
                        {
                            try
                            {
                                ProcessStartInfo processStartInfo = new ProcessStartInfo(savePath)
                                {
                                    UseShellExecute = true,
                                    Verb = "runas"
                                };
                                Process.Start(processStartInfo);
                                Application.Exit();
                            }
                            catch (Win32Exception)
                            {
                                MessageBox.Show(Properties.Settings.Default.language == "English"
                                    ? "The update requires administrative privileges to install."
                                    : "Для установки обновления требуются права администратора.");
                            }
                        }
                        else
                        {
                            richTextBox1.Text = Properties.Settings.Default.language == "English"
                                ? $"Error downloading the update: {e.Error.Message}"
                                : $"Ошибка скачивания обновления: {e.Error.Message}";
                        }
                    };

                    webClient.DownloadFileAsync(new Uri(downloadUrl), savePath);
                    MessageBox.Show(Properties.Settings.Default.language == "English" ? "Update download started." : "Скачивание обновления началось.");
                }
                else
                {
                    richTextBox1.Text = Properties.Settings.Default.language == "English"
                        ? "Failed to retrieve data about the latest release."
                        : "Не удалось получить данные о последнем релизе.";
                }
            }
            catch (WebException)
            {
                richTextBox1.Text = Properties.Settings.Default.language == "English"
                    ? "Error downloading the update. Check your internet connection and try again."
                    : "Произошла ошибка при попытке скачивания обновления. Проверьте ваше Интернет подключение и повторите попытку еще раз.";
            }
            catch (Exception ex)
            {
                richTextBox1.Text = Properties.Settings.Default.language == "English"
                    ? $"An error occured: {ex.Message}"
                    : $"Возникла ошибка: {ex.Message}";
            }
        }

        private async void CheckForUpdate()
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 1;

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    webClient.Headers.Add("User-Agent", "Anything");

                    string response = await webClient.DownloadStringTaskAsync("https://api.github.com/repos/edgeplus1/J2ME-Builder/releases");

                    JArray releases = JArray.Parse(response);

                    if (releases.Count > 0)
                    {
                        progressBar1.Style = ProgressBarStyle.Blocks;
                        JObject latestRelease = (JObject)releases[0];

                        string latestVersion = latestRelease["tag_name"]?.ToString();

                        if (!string.IsNullOrEmpty(latestVersion) && Version.TryParse(latestVersion, out Version version))
                        {
                            if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(version) < 0)
                            {
                                if (Properties.Settings.Default.language == "English")
                                {
                                    label_information.Text = "New version available!";
                                    version_label.Text = $"Latest version: {latestVersion}";
                                }
                                else if (Properties.Settings.Default.language == "Русский")
                                {
                                    label_information.Text = "Доступна новая версия!";
                                    version_label.Text = $"Последняя версия: {latestVersion}";
                                }
                                richTextBox1.Text = latestRelease["body"]?.ToString();
                                buttonInstallUpdate.Enabled = true;
                                return;
                            }
                            else
                            {
                                if (Properties.Settings.Default.language == "English")
                                {
                                    MessageBox.Show("Updates not found. Your version is latest!", Properties.Settings.Default.language == "English" ? "Checking Updates" : "Проверка обновлений", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    version_label.Text = "Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                                }
                                else if (Properties.Settings.Default.language == "Русский")
                                {
                                    MessageBox.Show("Обновления не найдены!", "Проверить обновления", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    version_label.Text = "Версия: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                                }
                                buttonInstallUpdate.Enabled = false;
                                return;
                            }
                        }
                    }
                    MessageBox.Show(Properties.Settings.Default.language == "English" ? "No release information found." : "Не удалось найти информацию.", Properties.Settings.Default.language == "English" ? "Checking Updates" : "Проверка обновлений", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (WebException)
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
                MessageBox.Show(Properties.Settings.Default.language == "English" ? "Failed to connect to update server. Check your Internet connection and try again" : "Произошла ошибка при попытке подключиться к серверу. Проверьте подключение к Интернету и попробуйте еще раз", Properties.Settings.Default.language == "English" ? "Connection failed..." : "Ошибка подключения...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            catch (Exception ex)
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
                MessageBox.Show(Properties.Settings.Default.language == "English" ? $"An error occurred: {ex.Message}" : $"Возникла ошибка: {ex.Message}", Properties.Settings.Default.language == "English" ? "Error" : "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DownloadLatestRelease();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (webClient != null && webClient.IsBusy)
            {
                webClient.CancelAsync();
                MessageBox.Show(Properties.Settings.Default.language == "English" ? "Downloading was cancelled." : "Скачивание обновления отменено.");
                progressBar1.Value = 0;
                labelpercent.Text = "";
            }
            else
            {
                MessageBox.Show(Properties.Settings.Default.language == "English" ? "No download in progress to cancel." : "Операция не была отменена.", Properties.Settings.Default.language == "English" ? "Information" : "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/edgeplus1/J2ME-Builder"));
        }
    }
}
