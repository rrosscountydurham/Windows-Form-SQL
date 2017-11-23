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
    public abstract class FormGenerator
    {
        public abstract void GenForm();
    }

    public class FormMain : FormGenerator
    {
        public override void GenForm()
        {
            Button b = ControlGenerator.CreateButton(10, 10, Settings.formWidth - 40, 50, "Insert");
            ControlGenerator.BindNavButton(b,new FormInsert());
            b = ControlGenerator.CreateButton(10, 70, Settings.formWidth - 40, 50, "Update/Delete");
            ControlGenerator.BindNavButton(b, new FormUpdateDelete());
            b = ControlGenerator.CreateButton(10, 130, Settings.formWidth - 40, 50, "Search");
            ControlGenerator.BindNavButton(b, new FormSearch());
        }
    }

    public class FormInsert : FormGenerator
    {
        public override void GenForm()
        {
            ControlGenerator.CreateLabel(10, 10, "Insert");
            Button b = ControlGenerator.CreateButton(10, 40, 100, 50, "Back");
            ControlGenerator.BindNavButton(b, new FormMain());
            ControlGenerator.CreateLabel(120, 10, "Select table");
            ComboBox c = ControlGenerator.CreateComboBox(120, 40, 100);
            ControlGenerator.SetComboBoxToTableList(c);
            b = ControlGenerator.CreateButton(250, 40, 100, 50, "Insert");
            ControlGenerator.BindInsertButton(b);
            ControlGenerator.BindComboBoxLoadTableInfo(c, 10, 100, false);
        }

    }

    public class FormUpdateDelete : FormGenerator
    {
        public override void GenForm()
        {
            ControlGenerator.CreateLabel(10, 10, "Update/Delete");
            Button b = ControlGenerator.CreateButton(10, 40, 100, 50, "Back");
            ControlGenerator.BindNavButton(b, new FormMain());
            ControlGenerator.CreateLabel(120, 10, "Select table");
            ComboBox c = ControlGenerator.CreateComboBox(120, 40, 100);
            ControlGenerator.SetComboBoxToTableList(c);
            ControlGenerator.BindComboBoxLoadTableInfo(c, 10, 100, true);
            b = ControlGenerator.CreateButton(250, 40, 100, 50, "Update");
            ControlGenerator.BindUpdateButton(b);
            b = ControlGenerator.CreateButton(360, 40, 100, 50, "Delete");
            ControlGenerator.BindDeleteButton(b);
            b = ControlGenerator.CreateButton(120, 70, 50, 30, "<");
            ControlGenerator.BindChangeRecord(b, false);
            b = ControlGenerator.CreateButton(190, 70, 50, 30, ">");
            ControlGenerator.BindChangeRecord(b, true);
        }
    }

    public class FormSearch : FormGenerator
    {
        public override void GenForm()
        {
            ControlGenerator.CreateLabel(10, 10, "Search");
            Button b = ControlGenerator.CreateButton(10, 40, 100, 50, "Back");
            ControlGenerator.BindNavButton(b, new FormMain());
            ControlGenerator.CreateLabel(120, 10, "Select table");
            ComboBox c = ControlGenerator.CreateComboBox(120, 40, 100);
            ControlGenerator.SetComboBoxToTableList(c);
            ControlGenerator.CreateLabel(120, 80, "Select Field");
            ComboBox c2 = ControlGenerator.CreateComboBox(120, 110, 100);
            ControlGenerator.SetComboBoxToFillTableFieldsOfComboBox(c, c2);
            ControlGenerator.CreateLabel(120, 160, "Type search parameters");
            TextBox t = ControlGenerator.CreateTextBox(120, 190, 120, 100);
            b = ControlGenerator.CreateButton(260, 40, 100, 100, "Search");
            ControlGenerator.BindSearch(b, 10, 250, c, c2, t);
            b = ControlGenerator.CreateButton(370, 70, 50, 30, "<");
            ControlGenerator.BindChangeRecord(b, false);
            b = ControlGenerator.CreateButton(420, 70, 50, 30, ">");
            ControlGenerator.BindChangeRecord(b, true);
        }
    }
}
