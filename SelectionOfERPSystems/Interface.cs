using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SelectionOfERPSystems
{
    //reflection
    /* способ доставания значения столбца с модулем
    ColumnToModuleName c = (ColumnToModuleName)ModulesListBox.Items[0]; как выбрать обратно объект из элемента
    var test = DB.Продукты.Where(i => i.номер==5).First(); 
    int v = Convert.ToInt32(test.GetType().GetProperty(c.ColumnName).GetValue(test, null));
    */
    public partial class Interface : Form
    {
        bool closing = false;
        public Interface()
        {
            InitializeComponent();

            var mappings = ColumnToModuleName.GetColumtNameMappings();
            foreach (var mapping in mappings)
            {
                ModulesListBox.Items.Add(mapping);
                FModulesListBox.Items.Add(mapping);
            }
            toolTip.SetToolTip(label5, "Покупка не всего продукта, а необходимых модулей, с возможностью расширения");
            toolTip.SetToolTip(label11, "Имеет значение для государственных предприятий, ОПК");
            toolTip.SetToolTip(label21, "По умолчанию будет выведено в окно, если не выбран иной способ");
            toolTip.SetToolTip(label30, "Необходимое количество рабочих мест по клиентской лицензии");

            using (var DB = new ERPDBContext())
            {
                try
                {
                    var industries = DB.Отрасли.Select(i => i.название);
                    foreach (var industry in industries)
                        IndustryComboBox.Items.Add(industry);
                    var integrations = DB.Обозначения.Where(i => i.номер > 9).Select(i => i.значение);
                    foreach (var integration in integrations)
                        IntegrationListBox.Items.Add(integration);
                    var sizes = DB.Обозначения.Where(i => i.номер < 4).Select(i => i.значение);
                    foreach (var size in sizes)
                    {
                        SizeComboBox.Items.Add(size);
                        FSizeComboBox.Items.Add(size);
                    }
                    var deployments = DB.Обозначения.Where(i => i.номер > 6 && i.номер < 10).Select(i => i.значение);
                    foreach (var deployment in deployments)
                        DeployComboBox.Items.Add(deployment);

                    SizeComboBox.SelectedIndex = 1;
                    IndustryComboBox.SelectedItem = "Универсальная";
                    DeployComboBox.SelectedIndex = 2;
                    CostMonthComboBox.SelectedIndex = 4;
                    CostForeverComboBox.SelectedIndex = 4;
                    TrialComboBox.SelectedIndex = 1;
                    ModularComboBox.SelectedIndex = 1;
                    RFComboBox.SelectedIndex = 1;
                    FSizeComboBox.SelectedIndex = 1;
                    OutputListBox.SelectedIndex = 2;
                }
                catch (Exception)
                {
                    MessageBox.Show("БД не найдена, либо повреждена!\nОбратитесь к программисту для её восстановления!", "ВНИМАНИЕ!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }                
            }            
        }
        Selection CreateSelection(int workerCount)
        {
            var integration = new List<string>();
            foreach (var item in IntegrationListBox.SelectedItems)
                integration.Add(item.ToString());
            var modules = new List<ColumnToModuleName>();
            foreach (var item in ModulesListBox.SelectedItems)
                modules.Add((ColumnToModuleName)item);
            var FModules = new List<ColumnToModuleName>();
            foreach (var item in FModulesListBox.SelectedItems)
                FModules.Add((ColumnToModuleName)item);
            
            var select = new Selection(
                industy: IndustryComboBox.Text,
                size: SizeComboBox.Text,
                deploy: DeployComboBox.SelectedIndex == 2 ? "0" : DeployComboBox.Text,
                RFLaws: RFComboBox.SelectedIndex == 1 ? 0 : 1,
                modules: modules,
                integration: integration,
                costMonth: CostMonthComboBox.SelectedIndex == CostMonthComboBox.Items.Count - 1 ? int.MaxValue : Convert.ToInt32(CostMonthComboBox.Text),
                costForever: CostForeverComboBox.SelectedIndex == CostForeverComboBox.Items.Count - 1 ? int.MaxValue : Convert.ToInt32(CostForeverComboBox.Text),
                workers: workerCount,
                modular: ModularComboBox.SelectedIndex == 1 ? 0 : 1,
                trial: TrialComboBox.SelectedIndex == 1 ? 0 : 1,
                fSize: FSizeComboBox.Text,
                fModules: FModules
            );
            return select;
        }
        void GoButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(WorkersTextBox.Text, out int workerCount))
            {
                MessageBox.Show("В поле количества пользователей (вкладка 'Приобретение') обнаружено нечисловое/нецелочисленное значение!", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Selection select = CreateSelection(workerCount); //Selection
            string output = select.FromDB(); //Output
            
            if (output == "")
                return;
            OutputType outputType = OutputType.None;
            foreach (int index in OutputListBox.SelectedIndices)
                if (index == 0)
                    outputType |= OutputType.Txt;
                else if (index == 1)
                    outputType |= OutputType.Docx;
                else if (index == 2)
                    outputType |= OutputType.Form;
            if (OutputListBox.SelectedIndices?.Count == 0)
            {
                outputType = OutputType.Form;
                OutputListBox.SelectedIndex = 2;
            }

            Output objOutput = new Output(output, NameTextBox.Text);
            objOutput.GeneralOutput(outputType);
        }
        void NameTextBox_Click(object sender, EventArgs e)
        {
            if (NameTextBox.ForeColor == Color.Gray)
                NameTextBox.Clear();
            NameTextBox.ForeColor = Color.Black;
        }
        void NameTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (NameTextBox.ForeColor == Color.Gray)
                NameTextBox.Clear();
            NameTextBox.ForeColor = Color.Black;
        }
        void NameTextBox_Leave(object sender, EventArgs e)
        {
            if (NameTextBox.Text == "")
            {
                NameTextBox.Text = "ООО 'Тестовая компания'";
                NameTextBox.ForeColor = Color.Gray;
            }
        }
        void WorkersTextBox_Click(object sender, EventArgs e)
        {
            if (WorkersTextBox.ForeColor == Color.Gray)
                WorkersTextBox.Clear();
            WorkersTextBox.ForeColor = Color.Black;
        }
        void WorkersTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (WorkersTextBox.ForeColor == Color.Gray)
                WorkersTextBox.Clear();
            WorkersTextBox.ForeColor = Color.Black;
        }
        void WorkersTextBox_Leave(object sender, EventArgs e)
        {
            if (WorkersTextBox.Text == "")
            {
                WorkersTextBox.Text = "1";
                WorkersTextBox.ForeColor = Color.Gray;
            }
        }
        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closing && MessageBox.Show("Вы уверены, что хотите закрыть приложение?", "Выход", MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
            else
            {
                closing = true;
                Application.Exit();
            }
        }
         void SizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FSizeComboBox.SelectedIndex = SizeComboBox.SelectedIndex;
        }
    }
}
