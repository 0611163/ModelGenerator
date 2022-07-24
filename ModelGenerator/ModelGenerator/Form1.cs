using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtil;
using Utils;
using System.Text.RegularExpressions;
using ModelGenerator.DAL;
using ModelGenerator.Models;
using ModelGenerator.Enums;

namespace ModelGenerator
{
    public partial class Form1 : Form
    {
        #region 变量
        private IDal _dal = DalFactory.CreateDal(ConfigurationManager.AppSettings["DBType"]);
        private List<DBTable> _tableList;
        #endregion

        #region Form1
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region Form1_Load
        private void Form1_Load(object sender, EventArgs e)
        {
            //DataGridView 显示序号行号
            dataGridView1.RowPostPaint += new DataGridViewRowPostPaintEventHandler(delegate (object obj, DataGridViewRowPostPaintEventArgs args)
            {
                SolidBrush brush = new SolidBrush(this.dataGridView1.RowHeadersDefaultCellStyle.ForeColor);
                args.Graphics.DrawString((args.RowIndex + 1).ToString(), this.dataGridView1.DefaultCellStyle.Font, brush, args.RowBounds.Location.X + 15, args.RowBounds.Location.Y + 7);
            });

            List<NameMode> nameModeList = new List<NameMode>();
            nameModeList.Add(new NameMode("Pascal", NameModeEnum.Pascal));
            nameModeList.Add(new NameMode("全大写", NameModeEnum.AllUpper));
            nameModeList.Add(new NameMode("全小写", NameModeEnum.AllLower));

            cbx1.DataSource = nameModeList;
            cbx1.ValueMember = "Value";
            cbx1.DisplayMember = "Key";

            Task.Run(() =>
            {
                _tableList = _dal.GetAllTables();
                _tableList.Sort((a, b) => string.Compare(a.TableName, b.TableName));

                this.BeginInvoke(new Action(() =>
                {
                    dataGridView1.AutoGenerateColumns = false;
                    dataGridView1.MultiSelect = true;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.DataSource = _tableList;
                    dataGridView1.Columns.Add("TableName", "表名");
                    dataGridView1.Columns.Add("Comments", "注释");
                    dataGridView1.Columns["TableName"].DataPropertyName = "TableName";
                    dataGridView1.Columns["Comments"].DataPropertyName = "Comments";
                    dataGridView1.Columns["TableName"].ReadOnly = true;
                    dataGridView1.Columns["Comments"].ReadOnly = true;
                    dataGridView1.Columns["TableName"].Width = 200;
                    dataGridView1.Columns["Comments"].Width = 200;
                }));
            });
        }
        #endregion

        #region dataGridView1_KeyDown
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.KeyData.ToString();
            int index = 0;
            if (e.Control || e.Shift) return;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Selected = false;
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DBTable table = row.DataBoundItem as DBTable;
                if (table.TableName.ToUpper().IndexOf(key.ToUpper()) == 0)
                {
                    row.Selected = true;
                    dataGridView1.FirstDisplayedScrollingRowIndex = index;
                    break;
                }
                index++;
            }
        }
        #endregion

        #region txtSearchKey_KeyUp
        private void txtSearchKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtSearchKey.Text.Trim().Length > 0)
            {
                List<DBTable> tableList = new List<DBTable>();
                foreach (DBTable table in _tableList)
                {
                    if (table.TableName.ToUpper().IndexOf(txtSearchKey.Text.Trim().ToUpper()) >= 0)
                    {
                        tableList.Add(table);
                    }
                }
                dataGridView1.DataSource = tableList;
            }
            else
            {
                dataGridView1.DataSource = _tableList;
            }
        }
        #endregion

        //生成
        private void btnCreate_Click(object sender, EventArgs e)
        {
            NameModeEnum nameMode = (NameModeEnum)cbx1.SelectedValue;

            Task.Run(() =>
            {
                try
                {
                    IDal dal = DalFactory.CreateDal(ConfigurationManager.AppSettings["DBType"]);
                    List<DBTable> tableList = dal.GetAllTables();
                    string strNamespace = ConfigurationManager.AppSettings["Namespace"];
                    string strClassTemplate = string.Empty;
                    string strClassExtTemplate = string.Empty;
                    string strFieldTemplate = string.Empty;
                    Regex regField = new Regex(@"[ \t]*#field start([\s\S]*)#field end", RegexOptions.IgnoreCase);

                    #region 操作控件
                    InvokeDelegate invokeDelegate = delegate ()
                    {
                        btnCreate.Enabled = false;
                        progressBar1.Visible = true;
                        progressBar1.Minimum = 0;
                        progressBar1.Maximum = tableList.Count;
                        progressBar1.Value = 0;
                    };
                    InvokeUtil.Invoke(this, invokeDelegate);
                    #endregion

                    #region 读取模板
                    strClassTemplate = FileHelper.ReadFile(Application.StartupPath + "\\Template\\class.txt");
                    strClassExtTemplate = FileHelper.ReadFile(Application.StartupPath + "\\Template\\class_ext.txt");
                    Match matchField = regField.Match(strClassTemplate);
                    if (matchField.Success)
                    {
                        strFieldTemplate = matchField.Groups[1].Value.TrimEnd(' ');
                    }
                    #endregion

                    int i = 0;
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows) //遍历表
                    {
                        DBTable table = row.DataBoundItem as DBTable;
                        string tableName = NameUtil.GetName(table.TableName.Trim(), nameMode);
                        StringBuilder sbFields = new StringBuilder();
                        List<DBColumn> columnList = dal.GetAllColumns(table.TableName);

                        #region 原始Model
                        string tableComments = table.Comments ?? string.Empty;
                        string strClass = strClassTemplate.Replace("#table_comments", tableComments.Trim().Replace("\r\n", "\r\n    /// ").Replace("\n", "\r\n        /// "));
                        strClass = strClass.Replace("#table_name", tableName);
                        if (tableName.ToUpper() != table.TableName.Trim().ToUpper())
                        {
                            strClass = strClass.Replace("#table_atrribute", "[DBTable(\"" + table.TableName.Trim() + "\")]");
                        }
                        else
                        {
                            strClass = strClass.Replace("    #table_atrribute\r\n", string.Empty);
                        }

                        foreach (DBColumn column in columnList) //遍历字段
                        {
                            string data_type = dal.ConvertDataType(column);

                            string columnComments = column.Comments ?? string.Empty;
                            string strField = strFieldTemplate.Replace("#field_comments", columnComments.Replace("\r\n", "\r\n        /// ").Replace("\n", "\r\n        /// "));

                            if (!column.PrimaryKey)
                            {
                                strField = strField.Replace("        [DBKey]\r\n", string.Empty);
                            }

                            strField = strField.Replace("#data_type", data_type);
                            string fieldName = NameUtil.GetName(column.ColumnName, nameMode);
                            strField = strField.Replace("#field_name", fieldName);

                            if (fieldName.ToUpper() == column.ColumnName.ToUpper())
                            {
                                strField = strField.Replace("#field_atrribute_value", "[DBField]");
                            }
                            else
                            {
                                strField = strField.Replace("#field_atrribute_value", "[DBField(\"" + column.ColumnName + "\")]");
                            }

                            sbFields.Append(strField);
                        }

                        strClass = regField.Replace(strClass, sbFields.ToString());

                        FileHelper.WriteFile(Application.StartupPath + "\\Models", strClass, tableName);
                        #endregion

                        #region 扩展Model
                        string strClassExt = strClassExtTemplate.Replace("#table_comments", tableComments.Trim().Replace("\r\n", "\r\n    /// ").Replace("\n", "\r\n        /// "));
                        strClassExt = strClassExt.Replace("#table_name", tableName);

                        FileHelper.WriteFile(Application.StartupPath + "\\ExtModels", strClassExt.ToString(), tableName);
                        #endregion

                        #region 操作控件
                        invokeDelegate = delegate ()
                        {
                            progressBar1.Value = ++i;
                        };
                        InvokeUtil.Invoke(this, invokeDelegate);
                        #endregion
                    }

                    #region 操作控件
                    invokeDelegate = delegate ()
                    {
                        btnCreate.Enabled = true;
                        progressBar1.Visible = false;
                        progressBar1.Value = 0;
                    };
                    InvokeUtil.Invoke(this, invokeDelegate);
                    #endregion

                    MessageBox.Show("完成");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                }
            });
        }

    }
}
