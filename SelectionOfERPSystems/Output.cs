using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace SelectionOfERPSystems
{
    [Flags]
    enum OutputType
    {
        None = 0,
        Txt = 1,
        Docx = 2,
        Form = 4,
    }
    class Output
    {
        string text;
        string companyName;        
        public Output(string text, string companyName)
        {
            this.text = text;
            this.companyName = companyName;
        }
        void WriteToForm(string correctDate)
        {
            MessageBox.Show("Для копирования текста отчёта нажмите Ctrl+C\n" + "_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _  _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _\n\n" +
                $"Подбор для {companyName} от {correctDate}\n\n" + text, "Результат подбора ERP-систем", MessageBoxButtons.OK);
        }
        void WriteToDocx(string correctDate, string fileName)
        {
            Word._Application OneWord = new Word.Application();
            Word.Document FirstDoc = OneWord.Documents.Add();

            Word.Paragraph Parag1 = FirstDoc.Content.Paragraphs.Add();
            Parag1.Range.Text = $"Подбор для {companyName} от {correctDate}" + Environment.NewLine + Environment.NewLine;
            Parag1.Format.SpaceBefore = 1;
            Parag1.Format.SpaceAfter = 1;

            Word.Paragraph Parag2 = FirstDoc.Content.Paragraphs.Add();
            Parag2.Range.Text = text;

            FirstDoc.SaveAs(Path.Combine(Directory.GetCurrentDirectory(), $"{fileName}.docx"));
            OneWord.Quit();
            Thread.Sleep(1);
            try
            {
                File.Move(Directory.GetCurrentDirectory() + $"\\{fileName}.docx", $"../../Data/Готовые отчёты/{fileName}.docx");
            }
            catch (Exception)
            {
                Thread.Sleep(5);
                File.Move(Directory.GetCurrentDirectory() + $"\\{fileName}.docx", $"../../Data/Готовые отчёты/{fileName}.docx");
            }
            if (MessageBox.Show("Хотите открыть созданный .docx файл?", "Выбор действия", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Process.Start($"..\\..\\Data\\Готовые отчёты\\{fileName}.docx");
        }
        void WriteToTxt(string correctDate, string fileName)
        {
            File.WriteAllText($"../../Data/Готовые отчёты/{fileName}.txt", $"Подбор для {companyName} от {correctDate}" + Environment.NewLine + Environment.NewLine + text);
            if (MessageBox.Show("Хотите открыть созданный .txt файл?", "Выбор действия", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Process.Start($"..\\..\\Data\\Готовые отчёты\\{fileName}.txt");
        }
        public void GeneralOutput(OutputType outputType)
        {
            string correctFullDate = (DateTime.Now.ToString(new CultureInfo("ru-RU"))).Replace(".", "-").Replace(":", "-");
            string correctDate = correctFullDate.Split(new char[] { ' ' })[0];

            StringBuilder newCompanyName = new StringBuilder(companyName); //удаление некорректных символов
            for (int i = 0; i < companyName.Length; i++)
                if (!Regex.IsMatch(companyName.Substring(i, 1), @"\w|\s|'|""|-"))
                    newCompanyName[i] = ' ';
            string fileName = $"{newCompanyName}_{correctFullDate}".Replace("\"", "'").Replace("  ", " ");

            if ((outputType & OutputType.Txt) != 0) //txt
                WriteToTxt(correctDate, fileName);
            if ((outputType & OutputType.Docx) != 0) //docx
                WriteToDocx(correctDate, fileName);
            if ((outputType & OutputType.Form) != 0) //form
                WriteToForm(correctDate);
        }
    }
}
