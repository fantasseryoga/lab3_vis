using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lab_3
{
    public partial class Form1 : Form
    {
        string Path { get; set; } = "";
        OpenFileDialog openFileDialog = new OpenFileDialog();
        private DataView dataView=new DataView();

        string path;
        public Form1()
        {
            InitializeComponent();
            dataView.Table = Workers;
            bindingSource.DataSource = dataView;
            this.Controls.SetChildIndex(statusStrip1, 1);
            openFileDialog.Filter = "dat|*.dat"; 
            openFileDialog.Multiselect = false;
            openFileDialog.FileOk += OpenFileDialog_FileOk;
        }
        //List<Worker> worker = new List<Worker>();
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }
        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            path = openFileDialog.FileName;
            FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            DataTable dataTable=  binaryFormatter.Deserialize(fileStream) as DataTable;
            foreach (DataRow item in dataTable.Rows)
            {
                Workers.ImportRow(item);
                toolStripStatusLabel1.Text= item["Birthday"].ToString();
            }
            fileStream.Close();
            Path = openFileDialog.FileName;
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileStream fileStream = new FileStream(Path, FileMode.Create, FileAccess.Write);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, Workers);
            fileStream.Close();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.ShowDialog();
        }

       

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream,Workers);
            fileStream.Close();
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(Path))
               saveToolStripMenuItem.Enabled = false;
            else
                saveToolStripMenuItem.Enabled = true;
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            DateTime dateTime = DateTime.Now;
            bool b;
            if (Workers.Rows.Count == 0)
                return;
            switch (toolStripComboBoxAge.SelectedItem)
            {
                case "remove filter":
                    bindingSource.RemoveFilter();
                    dataView.Table = Workers;
                    toolStripComboBoxSeasons.Visible = false;
                    return;
                    //break;
                case "the youngest filter":
                    dateTime = Workers.AsEnumerable().Select(row => row.Field<DateTime>("Birthday")).Max();
                    toolStripComboBoxSeasons.Visible = false;
                    break;

                case "the oldest filter":
                    dateTime = Workers.AsEnumerable().Select(row => row.Field<DateTime>("Birthday")).Min();
                    toolStripComboBoxSeasons.Visible = false;
                    break;
                case "season filter":
                    toolStripComboBoxSeasons.Visible = true;
                    bindingSource.RemoveFilter();
                    return;
               
            }
            toolStripStatusLabel1.Text = dateTime.ToShortDateString();
            bindingSource.Filter = $"Birthday=\'{dateTime.Year}.{dateTime.Month}.{dateTime.Day}\'";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //toolStripComboBoxAge.Sorted = true;
            //toolStripComboBoxAge.Items.Add("");
        }

        private void toolStripComboBoxSeasons_SelectedIndexChanged(object sender, EventArgs e)
        {
          DataView dataView1 = new DataView();
            DataTable filteredData=null;
            EnumerableRowCollection<DataRow> b = null;
            //DateTime dateTime = DateTime.Now;

           
            switch (toolStripComboBoxSeasons.SelectedItem)
            {
                case "winter":
                    b = Workers.AsEnumerable().Where(row => row.Field<DateTime>("Birthday").Month==12 || 
                                                            row.Field<DateTime>("Birthday").Month == 1 ||
                                                           row.Field<DateTime>("Birthday").Month == 2);

                    
                    
                    break;

                case "spring":
                    b = Workers.AsEnumerable().Where(row => row.Field<DateTime>("Birthday").Month == 3 ||
                                                            row.Field<DateTime>("Birthday").Month == 4 ||
                                                           row.Field<DateTime>("Birthday").Month == 5);
                    break;

                case "summer":
                    b = Workers.AsEnumerable().Where(row => row.Field<DateTime>("Birthday").Month ==6 ||
                                                            row.Field<DateTime>("Birthday").Month == 7 ||
                                                           row.Field<DateTime>("Birthday").Month == 8);
                    break;

                case "autumn":
                    b = Workers.AsEnumerable().Where(row => row.Field<DateTime>("Birthday").Month == 9 ||
                                                           row.Field<DateTime>("Birthday").Month == 10 ||
                                                          row.Field<DateTime>("Birthday").Month == 11);
                    break;
            }

            filteredData = Workers.Clone();
            filteredData.TableName = "filteredData";
            filteredData.Clear();
            foreach (DataRow row in b)
            {
                filteredData.ImportRow(row);
            }
            dataView.Table = filteredData;

        }
    }
}
