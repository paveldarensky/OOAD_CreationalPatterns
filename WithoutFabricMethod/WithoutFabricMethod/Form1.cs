﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WithoutFabricMethod
{
    public partial class Form_main : Form
    {
        private List<Document> documents;
        private ImageList imageList;
        private Document currentDocument;

        public Form_main()
        {
            InitializeComponent();

            textBox_DocumentInfo.Visible = true;
            panel_Inputs.Visible = false;

            documents = new List<Document>();

            // icons
            imageList = new ImageList();
            imageList.ImageSize = new Size(48, 48);
            imageList.Images.Add("Letter", Properties.Resources.letter);
            imageList.Images.Add("Order", Properties.Resources.order);
            imageList.Images.Add("BusinessTripOrder", Properties.Resources.business_trip_order);

            imageList.ColorDepth = ColorDepth.Depth32Bit;
            listView_Documents.LargeImageList = imageList;
            listView_Documents.View = View.Tile;
        }

        private void listView_Documents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView_Documents.SelectedItems.Count > 0)
            {
                string selectedNumber = listView_Documents.SelectedItems[0].Text;
                var doc = documents.FirstOrDefault(d => d.Number == selectedNumber);

                if (doc != null)
                {
                    textBox_DocumentInfo.Text = doc.DisplayInfo();
                }
                else
                {
                    textBox_DocumentInfo.Text = "Документ не найден";
                }
            }
        }

        private void OpenCreateDocumentForm()
        {
            if (typeOfDoc < 0 || typeOfDoc > 2)
            {
                MessageBox.Show("Ошибка: некорректный тип документа", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AddDocumentInputs(tableLayoutPanel_OwnFields);

            textBox_DocNum.Text = currentDocument.Number = $"№{documents.Count + 1}";
            currentDocument.Date = DateTime.Now;
            textBox_DateTimeCreateDocument.Text = currentDocument.Date.ToString("dd.MM.yyyy HH:mm");
            textBox_Content.Clear();

            textBox_DocumentInfo.Visible = false;
            panel_Inputs.Visible = true;
        }


        private void AddDocumentInputs(TableLayoutPanel table_panel)
        {
            // 0 - Letter, 1 - Order, 2 - BusinessTripOrder

            switch (typeOfDoc)
            {
                case 0:
                    currentDocument = new Letter();

                    table_panel.Controls.Clear();
                    Label lblType = new Label { Text = "Тип:" };
                    lblType.AutoSize = true;
                    ComboBox cmbType = new ComboBox();
                    cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbType.Items.AddRange(new string[] { "Входящее", "Исходящее" });

                    Label lblCorrespondent = new Label { Text = "Корреспондент:" };
                    lblCorrespondent.AutoSize = true;
                    TextBox txtCorrespondent = new TextBox();

                    table_panel.Controls.Add(lblType, 0, 0);
                    table_panel.Controls.Add(cmbType, 1, 0);
                    table_panel.Controls.Add(lblCorrespondent, 0, 1);
                    table_panel.Controls.Add(txtCorrespondent, 1, 1);
                break;

                case 1:
                    currentDocument = new Order();

                    table_panel.Controls.Clear();

                    Label lblDepartment = new Label { Text = "Подразделение:" };
                    lblDepartment.AutoSize = true;
                    TextBox txtDepartment = new TextBox();

                    Label lblDeadline = new Label { Text = "Срок выполнения:" };
                    lblDeadline.AutoSize = true;
                    DateTimePicker dtpDeadline = new DateTimePicker();
                    dtpDeadline.Format = DateTimePickerFormat.Short;

                    Label lblExecutor = new Label { Text = "Ответственный исполнитель:" };
                    lblExecutor.AutoSize = true;
                    TextBox txtExecutor = new TextBox();

                    table_panel.Controls.Add(lblDepartment, 0, 0);
                    table_panel.Controls.Add(txtDepartment, 1, 0);
                    table_panel.Controls.Add(lblDeadline, 0, 1);
                    table_panel.Controls.Add(dtpDeadline, 1, 1);
                    table_panel.Controls.Add(lblExecutor, 0, 2);
                    table_panel.Controls.Add(txtExecutor, 1, 2);
                break;

                case 2:
                    currentDocument = new BusinessTripOrder();

                    table_panel.Controls.Clear();

                    Label lblEmployee = new Label { Text = "Сотрудник:" };
                    lblEmployee.AutoSize = true;
                    TextBox txtEmployee = new TextBox();

                    Label lblPeriod = new Label { Text = "Период командировки:" };
                    lblPeriod.AutoSize = true;
                    ComboBox cmbPeriod = new ComboBox();
                    cmbPeriod.DropDownStyle = ComboBoxStyle.DropDownList;
                    cmbPeriod.Items.AddRange(new string[] { "1 неделя", "2 недели", "3 недели", "1 месяц" });

                    Label lblDestination = new Label { Text = "Место назначения:" };
                    lblDestination.AutoSize = true;
                    TextBox txtDestination = new TextBox();

                    table_panel.Controls.Add(lblEmployee, 0, 0);
                    table_panel.Controls.Add(txtEmployee, 1, 0);
                    table_panel.Controls.Add(lblPeriod, 0, 1);
                    table_panel.Controls.Add(cmbPeriod, 1, 1);
                    table_panel.Controls.Add(lblDestination, 0, 2);
                    table_panel.Controls.Add(txtDestination, 1, 2);
                break;

                default:
                    throw new Exception("Неизвестный тип документа");
            }
        }

        private void SaveDocumentInputs(TableLayoutPanel table_panel)
        {
            switch (typeOfDoc)
            {
                case 0:
                    var letter = currentDocument as Letter;
                    if (letter != null)
                    {
                        letter.Type = (table_panel.Controls[1] as ComboBox)?.SelectedItem as string;
                        letter.Correspondent = (table_panel.Controls[3] as TextBox)?.Text;
                    }
                    break;

                case 1:
                    var order = currentDocument as Order;
                    if (order != null)
                    {
                        order.Department = (table_panel.Controls[1] as TextBox)?.Text;
                        order.Deadline = (table_panel.Controls[3] as DateTimePicker)?.Value ?? DateTime.MinValue;
                        order.Executor = (table_panel.Controls[5] as TextBox)?.Text;
                    }
                    break;

                case 2:
                    var businessTripOrder = currentDocument as BusinessTripOrder;
                    if (businessTripOrder != null)
                    {
                        businessTripOrder.Employee = (table_panel.Controls[1] as TextBox)?.Text;
                        businessTripOrder.Period = (table_panel.Controls[3] as ComboBox)?.SelectedItem as string;
                        businessTripOrder.Destination = (table_panel.Controls[5] as TextBox)?.Text;
                    }
                    break;

                default:
                    throw new Exception("Неизвестный тип документа");
            }
        }


        int typeOfDoc; // 0 - Letter, 1 - Order, 2 - BusinessTripOrder

        private void Letter_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            typeOfDoc = 0;
            OpenCreateDocumentForm();
        }

        private void Order_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            typeOfDoc = 1;
            OpenCreateDocumentForm();
        }

        private void BusinessTripOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            typeOfDoc = 2;
            OpenCreateDocumentForm();
        }

        private void button_SaveDocument_Click(object sender, EventArgs e)
        {
            currentDocument.Content = textBox_Content.Text;
            SaveDocumentInputs(tableLayoutPanel_OwnFields);
            documents.Add(currentDocument);

            ListViewItem item = new ListViewItem(currentDocument.Number);
            item.ImageKey = currentDocument.GetImageKey();
            listView_Documents.Items.Add(item);

            textBox_DocumentInfo.Visible = true;
            panel_Inputs.Visible = false;
        }
    }
}






// ----------------------------------------------------- //
abstract class Document
{
    public string Number { get; set; } // номер документа
    public DateTime Date { get; set; } // дата (пусть будет создания)
    public string Content { get; set; } // краткая информация о содержании

    public abstract string DisplayInfo(); // показать инфо о доке
    public abstract string GetImageKey(); // для иконки
}

class Letter : Document
{
    public string Type { get; set; }          // входящее/исходящее
    public string Correspondent { get; set; } // корреспондент

    public override string DisplayInfo()
    {
        string DocumentContent = $"Письмо {Number}\r\nТип: {Type}\r\nКорреспондент: {Correspondent}\r\nСодержание: {Content}\r\nДата оповещения: {Date}";
        return DocumentContent;
    }

    public override string GetImageKey() => "Letter";
}

class Order : Document
{
    public string Department { get; set; }  // подразделение
    public DateTime Deadline { get; set; }  // срок выполнения
    public string Executor { get; set; }    // ответственный исполнитель

    public override string DisplayInfo()
    {
        string DocumentContent = $"Приказ: {Number}\r\nПодразделение: {Department}\r\nОтветственный: {Executor}\r\nСодержание: {Content}\r\nДата выполнения: {Deadline}\r\nДата оповещения: {Date}";
        return DocumentContent;
    }

    public override string GetImageKey() => "Order";
}

class BusinessTripOrder : Document
{
    public string Employee { get; set; }   // сотрудник
    public string Period { get; set; }     // период командировки
    public string Destination { get; set; }  // место назначения

    public override string DisplayInfo()
    {
        string DocumentContent = $"Распоряжение о командировке: {Number}\r\nСотрудник: {Employee}\r\nПериод командировки: {Period}\r\nСодержание: {Content}\r\nМесто назначения: {Destination}\r\nДата оповещения: {Date}";
        return DocumentContent;
    }

    public override string GetImageKey() => "BusinessTripOrder";
}