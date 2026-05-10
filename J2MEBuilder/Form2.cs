using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace J2MEBuilder
{
    public partial class Form2 : Form
    {
        private readonly string resPath = Path.Combine(Application.StartupPath, "res");
        private string projectPath = "";
        private string iconPath = "";

        public Form2()
        {
            InitializeComponent();
            InitializeReleaseTypeComboBox();
        }

        private void InitializeReleaseTypeComboBox()
        {
            if (comboBox1.Items.Count == 0)
            {
                comboBox1.Items.Add("Beta");
                comboBox1.Items.Add("Release");
                comboBox1.Items.Add("Alpha");
                comboBox1.Items.Add("Pre-Release");
                comboBox1.Items.Add("Release Candidate");
                comboBox1.Items.Add("Debug");
                comboBox1.Items.Add("Stable");
                comboBox1.SelectedIndex = 0;
            }
        }

        #region UI Events
        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Выберите корневую папку проекта (должна содержать src)";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    projectPath = fbd.SelectedPath;
                    textBox1.Text = projectPath;
                    TryDetectAppInfo();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "PNG Images|*.png|All Files|*.*";
                ofd.Title = "Выберите иконку приложения";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    iconPath = ofd.FileName;
                    textBox3.Text = iconPath;
                    try
                    {
                        pictureBox1.Image?.Dispose();
                        pictureBox1.Image = Image.FromFile(iconPath);
                    }
                    catch
                    {
                        Log("Ошибка загрузки превью иконки.");
                    }
                }
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(projectPath))
            {
                MessageBox.Show("Сначала выберите папку проекта!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text)) textBox2.Text = "1.0.0";
            if (string.IsNullOrWhiteSpace(textBox4.Text)) textBox4.Text = "MyStudio";
            if (comboBox1.SelectedItem == null) comboBox1.SelectedIndex = 0;

            // Читаем значения из UI элементов ДО запуска фонового потока
            string version = textBox2.Text.Trim();
            string vendor = textBox4.Text.Trim();
            string releaseType = comboBox1.SelectedItem.ToString();

            button3.Enabled = false;
            SetProgress(0);

            if (richTextBox1.InvokeRequired)
                richTextBox1.Invoke(new Action(() => richTextBox1.Clear()));
            else
                richTextBox1.Clear();

            Log("Запуск сборщика J2ME...");

            // Передаём значения в фоновый поток
            await Task.Run(() => BuildProcess(version, vendor, releaseType));

            button3.Enabled = true;
        }
        #endregion

        #region Build Logic
        private void BuildProcess(string version, string vendor, string releaseType)
        {
            try
            {
                SetProgress(5);
                Log("[1/6] Поиск JDK 8...");
                string jdkPath = FindJDK8();
                if (string.IsNullOrEmpty(jdkPath))
                    throw new Exception("JDK 8 не найден! Проверьте установку.");

                string javacPath = Path.Combine(jdkPath, "bin", "javac.exe");
                string jarPath = Path.Combine(jdkPath, "bin", "jar.exe");
                SetProgress(15);
                Log($"    Найден: {jdkPath}");

                SetProgress(25);
                Log("[2/6] Подготовка директорий...");
                string srcPath = Path.Combine(projectPath, "src");
                if (!Directory.Exists(srcPath)) throw new Exception("Папка 'src' не найдена внутри проекта.");

                if (string.IsNullOrEmpty(version)) version = "1.0.0";

                string buildPath = Path.Combine(projectPath, "build", releaseType, $"v{version}".Replace(" ", "_"));
                string classesPath = Path.Combine(buildPath, "classes");
                string verifiedPath = Path.Combine(buildPath, "verified");

                if (Directory.Exists(buildPath)) Directory.Delete(buildPath, true);
                Directory.CreateDirectory(classesPath);
                Directory.CreateDirectory(verifiedPath);
                SetProgress(35);
                Log($"    Создано: build\\{releaseType}\\v{version}\\");

                // Анализ библиотек из папки lib проекта
                SetProgress(45);
                Log("[2.5/6] Анализ библиотек в папке lib...");
                List<string> userLibs = GetLibraries();
                string userClasspathArg = "";

                if (userLibs.Count > 0)
                {
                    userClasspathArg = $"-classpath {string.Join(";", userLibs.Select(l => $"\"{l}\""))}";
                    Log($"    Найдено библиотек: {userLibs.Count}");
                    foreach (var lib in userLibs)
                    {
                        Log($"       - {Path.GetFileName(lib)}");
                    }
                }
                else
                {
                    Log("    Библиотеки не найдены в папке lib/");
                }

                SetProgress(55);
                Log("[3/6] Компиляция Java файлов...");

                string midpPath = Path.Combine(resPath, "midpapi20.jar");
                string cldcPath = Path.Combine(resPath, "cldcapi11.jar");
                string bootClassPath = $"\"{midpPath}\";\"{cldcPath}\"";

                string[] javaFiles = Directory.GetFiles(srcPath, "*.java", SearchOption.AllDirectories);
                if (javaFiles.Length == 0) throw new Exception("Файлы .java не найдены в папке src/");

                foreach (var f in javaFiles)
                {
                    Log($"    {Path.GetFileName(f)}");
                    string error;
                    string args = $"-bootclasspath {bootClassPath} {userClasspathArg} -d \"{classesPath}\" -encoding UTF-8 -source 1.3 -target 1.1 -g:none \"{f}\"";

                    int code = RunProcessSync(javacPath, args, out error);
                    if (code != 0) throw new Exception($"Ошибка компиляции:\n{error}");
                }
                SetProgress(70);
                Log("    Компиляция завершена.");

                SetProgress(80);
                Log("[4/6] Преверификация (preverify.exe)...");
                string preverifyPath = Path.Combine(resPath, "preverify.exe");
                if (!File.Exists(preverifyPath)) throw new Exception("preverify.exe не найден в папке res/");

                List<string> fullClasspath = new List<string> { midpPath, cldcPath };
                fullClasspath.AddRange(userLibs);
                string preverifyClasspath = string.Join(";", fullClasspath.Select(p => $"\"{p}\""));

                string prevError;
                int prevCode = RunProcessSync(preverifyPath,
                    $"-classpath {preverifyClasspath} -d \"{verifiedPath}\" \"{classesPath}\"",
                    out prevError);

                if (prevCode != 0) throw new Exception($"Преверификация не удалась:\n{prevError}");
                SetProgress(85);
                Log("    Преверификация завершена.");

                SetProgress(90);
                Log("[5/6] Копирование ресурсов...");
                int resCount = 0;
                foreach (var f in Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories))
                {
                    string ext = Path.GetExtension(f).ToLower();
                    if (ext != ".java" && ext != ".class")
                    {
                        string rel = f.Substring(srcPath.Length).TrimStart('\\', '/');
                        string dest = Path.Combine(verifiedPath, rel);

                        Directory.CreateDirectory(Path.GetDirectoryName(dest));
                        File.Copy(f, dest, true);
                        resCount++;
                    }
                }

                string iconFileName = "";
                if (!string.IsNullOrEmpty(iconPath) && File.Exists(iconPath))
                {
                    iconFileName = Path.GetFileName(iconPath);
                    File.Copy(iconPath, Path.Combine(verifiedPath, iconFileName), true);
                    Log($"     Иконка: {iconFileName}");
                }
                SetProgress(95);
                Log($"    Скопировано файлов: {resCount + (string.IsNullOrEmpty(iconFileName) ? 0 : 1)}");

                Log("[6/6] Создание JAR и JAD...");
                string appName = Path.GetFileName(projectPath.TrimEnd('\\', '/'));

                string firstJava = javaFiles[0];
                string mainClass = Path.GetFileNameWithoutExtension(firstJava);
                string relDir = Path.GetDirectoryName(firstJava).Substring(srcPath.Length).Trim('\\', '/');
                string pkg = relDir.Replace("\\", ".").Replace("/", ".");

                string midletDesc = $"{appName},{iconFileName},{pkg}.{mainClass}";
                string jarFileName = $"{appName}_v{version.Replace(" ", "_")}.jar";
                string jadFileName = $"{appName}_v{version.Replace(" ", "_")}.jad";

                string manifestPath = Path.Combine(buildPath, "MANIFEST.MF");
                using (var sw = new StreamWriter(manifestPath, false, new UTF8Encoding(false)))
                {
                    sw.WriteLine("Manifest-Version: 1.0");
                    sw.WriteLine($"MIDlet-Name: {appName}");
                    sw.WriteLine($"MIDlet-Version: {version}");
                    sw.WriteLine($"MIDlet-Vendor: {vendor}");
                    if (!string.IsNullOrEmpty(iconFileName)) sw.WriteLine($"MIDlet-Icon: {iconFileName}");
                    sw.WriteLine($"MIDlet-1: {midletDesc}");
                    sw.WriteLine("MicroEdition-Configuration: CLDC-1.1");
                    sw.WriteLine("MicroEdition-Profile: MIDP-2.0");
                }

                string jarOut = Path.Combine(buildPath, jarFileName);
                string jarError;
                int jarCode = RunProcessSync(jarPath,
                    $"cfm \"{jarOut}\" \"{manifestPath}\" -C \"{verifiedPath}\" .",
                    out jarError);

                if (jarCode != 0) throw new Exception($"Создание JAR не удалось:\n{jarError}");

                long jarSize = new FileInfo(jarOut).Length;
                string jadOut = Path.Combine(buildPath, jadFileName);
                using (var sw = new StreamWriter(jadOut, false, new UTF8Encoding(false)))
                {
                    sw.WriteLine($"MIDlet-Name: {appName}");
                    sw.WriteLine($"MIDlet-Version: {version}");
                    sw.WriteLine($"MIDlet-Vendor: {vendor}");
                    if (!string.IsNullOrEmpty(iconFileName)) sw.WriteLine($"MIDlet-Icon: {iconFileName}");
                    sw.WriteLine($"MIDlet-1: {midletDesc}");
                    sw.WriteLine($"MIDlet-Jar-URL: {jarFileName}");
                    sw.WriteLine($"MIDlet-Jar-Size: {jarSize}");
                    sw.WriteLine("MicroEdition-Configuration: CLDC-1.1");
                    sw.WriteLine("MicroEdition-Profile: MIDP-2.0");
                }

                SetProgress(100);
                Log($"    JAR: build\\{releaseType}\\v{version}\\{jarFileName} ({jarSize} байт)");
                Log($"    JAD: build\\{releaseType}\\v{version}\\{jadFileName}");
                Log("СБОРКА УСПЕШНА!");

                Process.Start(buildPath);
            }
            catch (Exception ex)
            {
                Log($"ОШИБКА: {ex.Message}");
                SetProgress(0);
            }
        }

        private List<string> GetLibraries()
        {
            List<string> libs = new List<string>();

            string projectLib = Path.Combine(projectPath, "lib");

            if (Directory.Exists(projectLib))
            {
                string[] jarFiles = Directory.GetFiles(projectLib, "*.jar", SearchOption.AllDirectories);

                foreach (string jar in jarFiles)
                {
                    if (File.Exists(jar) && new FileInfo(jar).Length > 0)
                    {
                        libs.Add(jar);
                    }
                }
            }

            string[] knownLibs = { "jsr82.jar", "btapi.jar", "jsr82-api.jar", "mmapi.jar", "wma20.jar" };
            foreach (string libName in knownLibs)
            {
                string resLib = Path.Combine(resPath, libName);
                if (File.Exists(resLib) && !libs.Any(l => Path.GetFileName(l).Equals(libName, StringComparison.OrdinalIgnoreCase)))
                {
                    libs.Add(resLib);
                }
            }

            return libs;
        }
        #endregion

        #region Helpers
        private void SetProgress(int value)
        {
            if (progressBar1.InvokeRequired)
                progressBar1.Invoke(new Action(() => progressBar1.Value = value));
            else
                progressBar1.Value = value;
        }

        private void Log(string msg)
        {
            Action append = () => {
                richTextBox1.AppendText(msg + Environment.NewLine);
                richTextBox1.ScrollToCaret();
            };

            if (richTextBox1.InvokeRequired)
                richTextBox1.Invoke(append);
            else
                append();
        }

        private int RunProcessSync(string file, string args, out string error)
        {
            var psi = new ProcessStartInfo(file, args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using (var p = Process.Start(psi))
            {
                error = p.StandardError.ReadToEnd();
                p.WaitForExit();
                return p.ExitCode;
            }
        }

        private string FindJDK8()
        {
            string[] bases = { @"C:\Program Files\Java", @"C:\Program Files (x86)\Java" };
            foreach (var b in bases)
            {
                if (Directory.Exists(b))
                {
                    var dirs = Directory.GetDirectories(b, "jdk1.8*")
                              .Concat(Directory.GetDirectories(b, "jdk-8*"));
                    foreach (var d in dirs)
                        if (File.Exists(Path.Combine(d, "bin", "javac.exe"))) return d;
                }
            }
            return null;
        }

        private void TryDetectAppInfo()
        {
            string srcPath = Path.Combine(projectPath, "src");
            if (!Directory.Exists(srcPath)) return;
        }
        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            string buildsPath = Path.Combine(projectPath, "build");
            if (Directory.Exists(buildsPath))
                Process.Start(buildsPath);
            else
                MessageBox.Show("Папка build ещё не создана.", "Инфо", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}