using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace J2MEBuilder
{
    public partial class Form3 : Form
    {
        public string CreatedProjectPath { get; private set; }
        private readonly string toolsSourcePath;
        private string selectedWorkspacePath;

        public Form3()
        {
            InitializeComponent();
            this.Load += Form3_Load;

            toolsSourcePath = Path.Combine(Application.StartupPath, "res");
            selectedWorkspacePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "J2ME_Projects");
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            textBox2.Text = selectedWorkspacePath;
            LoadLibraries();
        }

        private void LoadLibraries()
        {
            checkedListBox1.Items.Clear();

            string libsPath = Path.Combine(Application.StartupPath, "src");
            if (!Directory.Exists(libsPath) || !Directory.GetFiles(libsPath, "*.jar").Any())
            {
                libsPath = toolsSourcePath;
            }

            if (!Directory.Exists(libsPath))
            {
                MessageBox.Show($"❌ Папка с библиотеками не найдена!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var jars = Directory.GetFiles(libsPath, "*.jar", SearchOption.TopDirectoryOnly);
            foreach (var jar in jars)
            {
                checkedListBox1.Items.Add(Path.GetFileName(jar));
            }

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                string name = checkedListBox1.Items[i].ToString().ToLower();
                if (name.Contains("midp") || name.Contains("cldc"))
                    checkedListBox1.SetItemChecked(i, true);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "Выберите папку для создания проекта";
            folderBrowserDialog1.ShowNewFolderButton = true;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
                selectedWorkspacePath = textBox2.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pName = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(pName))
            {
                MessageBox.Show("Введите имя проекта!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Выберите директорию для создания проекта!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cleanName = new string(pName.Where(char.IsLetterOrDigit).ToArray());
            if (string.IsNullOrEmpty(cleanName)) cleanName = "Project";

            string projPath = Path.Combine(textBox2.Text, pName);
            if (Directory.Exists(projPath))
            {
                MessageBox.Show($"Проект \"{pName}\" уже существует в выбранной папке!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor = Cursors.WaitCursor;
            button1.Enabled = false;

            try
            {
                CreateProject(projPath, pName, cleanName);
                CreatedProjectPath = projPath;

                MessageBox.Show($"Проект \"{pName}\" успешно создан!\n\nСейчас открою его в VS Code...", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OpenInVSCode(projPath);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания проекта:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                button1.Enabled = true;
            }
        }

        private void CreateProject(string projPath, string pName, string cleanName)
        {
            // 1. Создаём структуру папок
            Directory.CreateDirectory(Path.Combine(projPath, "src", "com", cleanName));
            Directory.CreateDirectory(Path.Combine(projPath, "lib"));
            Directory.CreateDirectory(Path.Combine(projPath, ".vscode"));

            // 2. Определяем источник библиотек
            string libsSource = Path.Combine(Application.StartupPath, "src");
            if (!Directory.Exists(libsSource) || !Directory.GetFiles(libsSource, "*.jar").Any())
                libsSource = toolsSourcePath;

            // 3. Копируем выбранные библиотеки в lib/
            foreach (var item in checkedListBox1.CheckedItems)
            {
                string libName = item.ToString();
                string srcFile = Path.Combine(libsSource, libName);
                string dstFile = Path.Combine(projPath, "lib", libName);
                if (File.Exists(srcFile))
                    File.Copy(srcFile, dstFile, true);
            }

            // 4. Копируем инструменты сборки
            foreach (var tool in new[] { "preverify.exe", "build40.bat" })
            {
                string src = Path.Combine(toolsSourcePath, tool);
                if (File.Exists(src))
                    File.Copy(src, Path.Combine(projPath, tool), true);
            }

            // 5. Копируем папки отладки (kemnmod и др.)
            if (Directory.Exists(toolsSourcePath))
            {
                foreach (var dir in Directory.GetDirectories(toolsSourcePath))
                {
                    string dirName = Path.GetFileName(dir);
                    if (!dirName.Equals("bin", StringComparison.OrdinalIgnoreCase) &&
                        !dirName.Equals("obj", StringComparison.OrdinalIgnoreCase) &&
                        !dirName.Equals("res", StringComparison.OrdinalIgnoreCase) &&
                        !dirName.Equals("src", StringComparison.OrdinalIgnoreCase))
                    {
                        CopyDirectory(dir, Path.Combine(projPath, dirName));
                    }
                }
            }

            // 6. Генерируем Java файл БЕЗ BOM (UTF-8 без BOM)
            string javaContent = GenerateJavaTemplate(pName, cleanName);
            string javaFilePath = Path.Combine(projPath, "src", "com", cleanName, $"{cleanName}.java");
            File.WriteAllText(javaFilePath, javaContent, new UTF8Encoding(false));

            // 7. Генерируем настройки VS Code тоже без BOM
            File.WriteAllText(Path.Combine(projPath, ".vscode", "settings.json"), GenerateVSCodeSettings(), new UTF8Encoding(false));
        }

        private void CopyDirectory(string sourceDir, string destDir)
        {
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            foreach (string file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), true);

            foreach (string dir in Directory.GetDirectories(sourceDir))
                CopyDirectory(dir, Path.Combine(destDir, Path.GetFileName(dir)));
        }

        private string GenerateJavaTemplate(string pName, string className)
        {
            return $@"package com.{className};

import javax.microedition.midlet.*;
import javax.microedition.lcdui.*;

public class {className} extends MIDlet implements CommandListener {{
    private Display display;
    private Form form;
    private Command exitCmd;

    public void startApp() {{
        display = Display.getDisplay(this);
        form = new Form(""{pName}"");
        form.append(""Hello from {pName}!"");
        exitCmd = new Command(""Exit"", Command.EXIT, 1);
        form.addCommand(exitCmd);
        form.setCommandListener(this);
        display.setCurrent(form);
    }}
    
    public void pauseApp() {{}}
    
    public void destroyApp(boolean unconditional) {{
        notifyDestroyed();
    }}
    
    public void commandAction(Command c, Displayable d) {{
        if (c == exitCmd)
            destroyApp(true);
    }}
}}";
        }

        private string GenerateVSCodeSettings()
        {
            return @"{
    ""java.project.referencedLibraries"": [""lib/*.jar""],
    ""java.validation.enabled"": false,
    ""java.errors.incompleteClasspath.severity"": ""ignore""
}";
        }

        private void OpenInVSCode(string projPath)
        {
            try
            {
                var psi = new ProcessStartInfo("code", $"\"{projPath}\"")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(psi);
                return;
            }
            catch { }

            string[] vsPaths = {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Microsoft VS Code", "Code.exe"),
                @"C:\Program Files\Microsoft VS Code\Code.exe"
            };

            foreach (var path in vsPaths)
            {
                if (File.Exists(path))
                {
                    Process.Start(path, $"\"{projPath}\"");
                    return;
                }
            }

            if (MessageBox.Show(
                "Visual Studio Code не найден в системе.\n\nБез него разработка J2ME будет крайне неудобной.\n\nОткрыть страницу загрузки?",
                "VS Code не установлен",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo("https://code.visualstudio.com/") { UseShellExecute = true });
            }
        }
    }
}