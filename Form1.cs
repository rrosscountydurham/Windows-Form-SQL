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
    public partial class SQLMainForm : Form
    {
        public SQLMainForm()
        {
            InitializeComponent();
        }

        private void SQLMainForm_Load(object sender, EventArgs e)
        {
            Width = Settings.formWidth;
            Height = Settings.formHeight;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Settings.mainForm = this;
            FormMain frm = new FormMain();
            frm.GenForm();
        }
    }
}
