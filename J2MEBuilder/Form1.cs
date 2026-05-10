using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace J2MEBuilder
{
    public partial class Form1 : Form
    {
        private readonly string recentProjectsFile;
        private List<string> recentProjects;
        private const int MaxRecentProjects = 10;

        public Form1()
        {
            InitializeComponent();

            recentProjectsFile = Path.Combine(Application.StartupPath, "recent_projects.txt");
            recentProjects = new List<string>();

            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadRecentProjects();
            UpdateListBox();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 builder = new Form2();
            builder.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 createProject = new Form3();
            if (createProject.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(createProject.CreatedProjectPath))
                {
                    AddToRecentProjects(createProject.CreatedProjectPath);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Выберите папку проекта J2ME";
                fbd.ShowNewFolderButton = false;

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string projectPath = fbd.SelectedPath;

                    if (!IsJ2MEProject(projectPath))
                    {
                        MessageBox.Show(
                            "Выбранная папка не похожа на J2ME проект.\n\n" +
                            "Проект должен содержать:\n" +
                            "- папку src/\n" +
                            "- папку lib/\n" +
                            "- .java файлы",
                            "Неверный проект",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    AddToRecentProjects(projectPath);
                    OpenProjectInVSCode(projectPath);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите проект из списка!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Извлекаем путь из строки "ProjectName - C:\path\to\project"
            string selectedText = listBox1.SelectedItem.ToString();
            string projectPath = ExtractPathFromListBoxItem(selectedText);

            if (!Directory.Exists(projectPath))
            {
                var result = MessageBox.Show(
                    $"Проект не найден:\n{projectPath}\n\nУдалить из списка?",
                    "Проект не существует",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    RemoveFromRecentProjects(projectPath);
                    UpdateListBox();
                }
                return;
            }

            if (!IsJ2MEProject(projectPath))
            {
                MessageBox.Show("Выбранная папка больше не является J2ME проектом.",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RemoveFromRecentProjects(projectPath);
                UpdateListBox();
                return;
            }

            AddToRecentProjects(projectPath);
            OpenProjectInVSCode(projectPath);
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
                button6_Click(sender, e);
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && listBox1.SelectedItem != null)
            {
                var contextMenu = new ContextMenuStrip();

                contextMenu.Items.Add("Удалить из списка", null, (s, args) =>
                {
                    string selectedText = listBox1.SelectedItem.ToString();
                    string path = ExtractPathFromListBoxItem(selectedText);
                    RemoveFromRecentProjects(path);
                });

                contextMenu.Items.Add("Очистить весь список", null, (s, args) =>
                {
                    if (MessageBox.Show("Очистить весь список недавних проектов?",
                        "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        recentProjects.Clear();
                        SaveRecentProjects();
                        UpdateListBox();
                    }
                });

                contextMenu.Show(listBox1, e.Location);
            }
        }

        /// <summary>
        /// Извлекает путь из строки listBox1 (формат: "ProjectName - C:\path")
        /// </summary>
        private string ExtractPathFromListBoxItem(string itemText)
        {
            int dashIndex = itemText.IndexOf(" - ");
            if (dashIndex > 0)
            {
                return itemText.Substring(dashIndex + 3);
            }
            return itemText;
        }

        private bool IsJ2MEProject(string path)
        {
            bool hasSrc = Directory.Exists(Path.Combine(path, "src"));
            bool hasJavaFiles = Directory.GetFiles(path, "*.java", SearchOption.AllDirectories).Length > 0;
            bool hasLib = Directory.Exists(Path.Combine(path, "lib"));
            bool hasBuildScript = File.Exists(Path.Combine(path, "build40.bat"));

            if (hasSrc && hasJavaFiles) return true;
            if (hasBuildScript) return true;
            if (hasLib && hasSrc) return true;

            return false;
        }

        private void OpenProjectInVSCode(string projectPath)
        {
            try
            {
                var psi = new ProcessStartInfo("code", $"\"{projectPath}\"")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(psi);
                return;
            }
            catch { }

            string[] vsPaths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Programs", "Microsoft VS Code", "Code.exe"),
                @"C:\Program Files\Microsoft VS Code\Code.exe"
            };

            foreach (var path in vsPaths)
            {
                if (File.Exists(path))
                {
                    Process.Start(path, $"\"{projectPath}\"");
                    return;
                }
            }

            var result = MessageBox.Show(
                "Visual Studio Code не найден в системе.\n\n" +
                "Открыть страницу загрузки?",
                "VS Code не установлен",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo("https://code.visualstudio.com/")
                {
                    UseShellExecute = true
                });
            }
        }

        #region Recent Projects Management

        private void LoadRecentProjects()
        {
            if (File.Exists(recentProjectsFile))
            {
                try
                {
                    var lines = File.ReadAllLines(recentProjectsFile);
                    recentProjects = lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
                }
                catch
                {
                    recentProjects = new List<string>();
                }
            }
        }

        private void SaveRecentProjects()
        {
            try
            {
                File.WriteAllLines(recentProjectsFile, recentProjects);
            }
            catch { }
        }

        private void AddToRecentProjects(string projectPath)
        {
            recentProjects.Remove(projectPath);
            recentProjects.Insert(0, projectPath);

            while (recentProjects.Count > MaxRecentProjects)
            {
                recentProjects.RemoveAt(recentProjects.Count - 1);
            }

            SaveRecentProjects();
            UpdateListBox();
        }

        private void RemoveFromRecentProjects(string projectPath)
        {
            recentProjects.Remove(projectPath);
            SaveRecentProjects();
            UpdateListBox();
        }

        private void UpdateListBox()
        {
            listBox1.Items.Clear();

            foreach (var path in recentProjects)
            {
                if (Directory.Exists(path))
                {
                    string projectName = Path.GetFileName(path);
                    listBox1.Items.Add($"{projectName} - {path}");
                }
            }

            if (listBox1.Items.Count > 0)
                listBox1.Text = $"Недавние проекты ({listBox1.Items.Count})";
            else
                listBox1.Text = "Нет недавних проектов";
        }

        #endregion

        private void menuItem2_Click(object sender, EventArgs e)
        {
            Form4 aboutForm = new Form4();
            aboutForm.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form5 downloading = new Form5();
            downloading.ShowDialog();
        }
    }
}