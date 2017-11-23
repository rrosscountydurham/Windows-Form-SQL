using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLForm
{

    public static class ControlGenerator
    {
        public static List<Button> buttons = new List<Button>();

        public static void SetFontSizeAndAdd(Control c)
        {
            c.Font = new Font(c.Font.Name, Settings.defaultFontSize);
            Settings.mainForm.Controls.Add(c);
        }
        public static Button CreateButton(int x, int y, int width, int height, string text) {
            Button button = new Button();
            button.Left = x; button.Top = y; button.Width = width; button.Height = height; button.Text = text;
            SetFontSizeAndAdd(button);
            buttons.Add(button);
            return button;
        }
        public static void BindNavButton(Button button, FormGenerator formGen)
        {
            if (buttons.Contains(button))
            {
                button.Click += (sender, e) => NavButtonWrapper(sender, e, formGen);
            }
        }

        public static void BindInsertButton(Button button)
        {
            if (buttons.Contains(button))
            {
                button.Click += (sender, e) => InsertButtonWrapper(sender, e);
            }
        }

        public static void BindUpdateButton(Button button)
        {
            if (buttons.Contains(button))
            {
                button.Click += (sender, e) => UpdateButtonWrapper(sender, e);
            }
        }

        public static void BindDeleteButton(Button button)
        {
            if (buttons.Contains(button))
            {
                button.Click += (sender, e) => DeleteButtonWrapper(sender, e);
            }
        }

        public static void BindChangeRecord(Button button, bool forward)
        {
            if (buttons.Contains(button))
            {
                button.Click += (sender, e) => ShiftRecordWrapper(sender, e, forward);
            }
        }

        public static void BindSearch(Button button, int x, int y, ComboBox from, ComboBox where, TextBox contains)
        {
            if (buttons.Contains(button))
            {
                button.Click += (sender, e) => SearchButtonWrapper(sender, e, x, y, from, where, contains);
            }
        }

        public static Label CreateLabel(int x, int y, string text)
        {
            Label label = new Label();
            label.Left = x; label.Top = y; label.Text = text; label.AutoSize = true;
            SetFontSizeAndAdd(label);
            return label;
        }

        public static TextBox CreateTextBox(int x, int y, int width, int height)
        {
            TextBox textBox = new TextBox();
            textBox.Left = x; textBox.Top = y; textBox.Width = width; textBox.Height = height;
            SetFontSizeAndAdd(textBox);
            return textBox;
        }

        public static ComboBox CreateComboBox(int x, int y, int width)
        {
            ComboBox comboBox = new ComboBox();
            comboBox.Left = x; comboBox.Top = y;
            SetFontSizeAndAdd(comboBox);
            return comboBox;
        }

        public static void SetComboBoxToTableList(ComboBox cb)
        {
            List<string> list = DatabaseConnection.GetTableList();
            foreach(string s in list)
            {
                cb.Items.Add(s);
            }
        }
        
        public static void SetComboBoxToTableFields(ComboBox cb, string table)
        {
            List<string> list = DatabaseConnection.GetTableFields(table);
            foreach(string s in list)
            {
                cb.Items.Add(s);
            }
            cb.SelectedIndex = 0;
        }

        public static void SetComboBoxToFillTableFieldsOfComboBox(ComboBox tableCb, ComboBox fieldCb)
        {
            tableCb.SelectedIndexChanged += (sender, e) => ComboChangeSetFieldsWrapper(sender, e, fieldCb);
        }

        public static void BindComboBoxLoadTableInfo(ComboBox cb, int x, int y, bool loadData)
        {
            cb.SelectedIndexChanged += (sender, e) => ComboChangeLoadTableWrapper(sender, e, x, y, loadData);
        }

        public static FlowLayoutPanel CreateFlowLayoutPanel()
        {
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel.AutoScroll = true;
            flowLayoutPanel.WrapContents = false;
            SetFontSizeAndAdd(flowLayoutPanel);
            return flowLayoutPanel;
        }
        public static void ClearControls()
        {
            List<Control> controls = Settings.mainForm.Controls.Cast<Control>().ToList();
            Settings.mainForm.Controls.Clear();
            foreach (Control c in controls)
            {
                c.Dispose();
            }
        }

        public static void RemoveControl(Control c)
        {
            if (Settings.mainForm.Controls.Contains(c))
            {
                Settings.mainForm.Controls.Remove(c);
            }
        }

        public static void NavButtonWrapper(object sender, EventArgs e, FormGenerator formGen)
        {
            ClearControls();
            formGen.GenForm();
        }

        public static void InsertButtonWrapper(object sender, EventArgs e)
        {
            DatabaseConnection.RunInsert();
        }

        public static void UpdateButtonWrapper(object sender, EventArgs e)
        {
            DatabaseConnection.RunUpdate();
        }
        
        public static void DeleteButtonWrapper(object sender, EventArgs e)
        {
            DatabaseConnection.RunDelete();
        }

        public static void SearchButtonWrapper(object sender, EventArgs e, int x, int y, ComboBox from, ComboBox where, TextBox contains)
        {
            if (from.SelectedItem == null)
            {
                MessageBox.Show("Select table"); return;
            }
            if (where.SelectedItem == null)
            {
                MessageBox.Show("Select field to search"); return;
            }
            if (contains.Text == "")
            {
                MessageBox.Show("Type in search parameter"); return;
            }
            DatabaseConnection.GetSearchResults(x, y, from.SelectedItem.ToString(), where.SelectedItem.ToString(), contains.Text);
        }

        public static void ComboChangeLoadTableWrapper(object sender, EventArgs e, int x, int y, bool loadData)
        {
            ComboBox cb = (ComboBox)sender;
            DatabaseConnection.PopulateDataSheet(x, y, cb.SelectedItem.ToString(),true,loadData);
        }

        public static void ComboChangeSetFieldsWrapper(object sender, EventArgs e, ComboBox target)
        {
            ComboBox cb = (ComboBox)sender;
            SetComboBoxToTableFields(target, cb.SelectedItem.ToString());
        }

        public static void ShiftRecordWrapper(object sender, EventArgs e, bool forward)
        {
            DatabaseConnection.ShiftRecord(forward);
        }
    }
}
