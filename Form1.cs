using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace StudentManagementSystem
{
    public partial class Form1 : Form
    {
        // Declare all global controls
        TextBox txtId, txtFirstName, txtLastName, txtPhone, txtAddress, txtSearchId;
        DateTimePicker dateBirth;
        RadioButton radioMale, radioFemale;
        PictureBox pictureBoxStudent;

        public Form1()
        {
            // REMOVE InitializeComponent(); because we are not using the designer
            CreateFormUI();
        }

        private void CreateFormUI()
        {
            this.Text = "Student Record Management";
            this.Size = new Size(800, 600);

            // --- ID ---
            Label lblId = new Label();
            lblId.Text = "ID:";
            lblId.Location = new Point(20, 20);
            this.Controls.Add(lblId);

            txtId = new TextBox();
            txtId.Name = "txtId";
            txtId.Location = new Point(100, 20);
            txtId.Width = 200;
            txtId.ReadOnly = true;
            this.Controls.Add(txtId);

            // --- First Name ---
            Label lblFirstName = new Label();
            lblFirstName.Text = "First Name:";
            lblFirstName.Location = new Point(20, 60);
            this.Controls.Add(lblFirstName);

            txtFirstName = new TextBox();
            txtFirstName.Location = new Point(100, 60);
            txtFirstName.Width = 200;
            this.Controls.Add(txtFirstName);

            // --- Last Name ---
            Label lblLastName = new Label();
            lblLastName.Text = "Last Name:";
            lblLastName.Location = new Point(20, 100);
            this.Controls.Add(lblLastName);

            txtLastName = new TextBox();
            txtLastName.Location = new Point(100, 100);
            txtLastName.Width = 200;
            this.Controls.Add(txtLastName);

            // --- Birth Date ---
            Label lblBirthDate = new Label();
            lblBirthDate.Text = "Birth Date:";
            lblBirthDate.Location = new Point(20, 140);
            this.Controls.Add(lblBirthDate);

            dateBirth = new DateTimePicker();
            dateBirth.Location = new Point(100, 140);
            this.Controls.Add(dateBirth);

            // --- Gender ---
            Label lblGender = new Label();
            lblGender.Text = "Gender:";
            lblGender.Location = new Point(20, 180);
            this.Controls.Add(lblGender);

            radioMale = new RadioButton();
            radioMale.Text = "Male";
            radioMale.Location = new Point(100, 180);
            this.Controls.Add(radioMale);

            radioFemale = new RadioButton();
            radioFemale.Text = "Female";
            radioFemale.Location = new Point(170, 180);
            this.Controls.Add(radioFemale);

            // --- Phone ---
            Label lblPhone = new Label();
            lblPhone.Text = "Phone:";
            lblPhone.Location = new Point(20, 220);
            this.Controls.Add(lblPhone);

            txtPhone = new TextBox();
            txtPhone.Location = new Point(100, 220);
            txtPhone.Width = 200;
            this.Controls.Add(txtPhone);

            // --- Address ---
            Label lblAddress = new Label();
            lblAddress.Text = "Address:";
            lblAddress.Location = new Point(20, 260);
            this.Controls.Add(lblAddress);

            txtAddress = new TextBox();
            txtAddress.Location = new Point(100, 260);
            txtAddress.Width = 200;
            txtAddress.Height = 60;
            txtAddress.Multiline = true;
            this.Controls.Add(txtAddress);

            // --- Picture Box ---
            Label lblPicture = new Label();
            lblPicture.Text = "Picture:";
            lblPicture.Location = new Point(20, 340);
            this.Controls.Add(lblPicture);

            pictureBoxStudent = new PictureBox();
            pictureBoxStudent.Location = new Point(100, 340); pictureBoxStudent.Size = new Size(100, 100);
            pictureBoxStudent.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pictureBoxStudent);

            // --- Buttons ---
            Button btnAdd = new Button();
            btnAdd.Text = "Add Student";
            btnAdd.Location = new Point(100, 460);
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);

            // --- Search Section ---
            Label lblSearch = new Label();
            lblSearch.Text = "Search by ID:";
            lblSearch.Location = new Point(400, 20);
            this.Controls.Add(lblSearch);

            txtSearchId = new TextBox();
            txtSearchId.Name = "txtSearchId";
            txtSearchId.Location = new Point(500, 20);
            this.Controls.Add(txtSearchId);

            Button btnSearch = new Button();
            btnSearch.Text = "Search";
            btnSearch.Location = new Point(500, 60);
            btnSearch.Click += BtnSearch_Click;
            this.Controls.Add(btnSearch);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text;
            string lastName = txtLastName.Text;
            string gender = radioMale.Checked ? "Male" : "Female";
            string birthDate = dateBirth.Value.ToString("yyyy-MM-dd");
            string phone = txtPhone.Text;
            string address = txtAddress.Text;

            byte[] pictureBytes = null;
            if (pictureBoxStudent.Image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    pictureBoxStudent.Image.Save(ms, pictureBoxStudent.Image.RawFormat);
                    pictureBytes = ms.ToArray();
                }
            }

            using (SQLiteConnection con = new SQLiteConnection("Data Source=student.db;Version=3;"))
            {
                con.Open();
                string query = "INSERT INTO Students (FirstName, LastName, BirthDate, Gender, Phone, Address, Picture) " +
                               "VALUES (@FirstName, @LastName, @BirthDate, @Gender, @Phone, @Address, @Picture)";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@BirthDate", birthDate);
                    cmd.Parameters.AddWithValue("@Gender", gender);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Picture", pictureBytes);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Student added successfully!");
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchId.Text))
            {
                MessageBox.Show("Please enter a student ID.");
                return;
            }

            using (SQLiteConnection con = new SQLiteConnection("Data Source=student.db;Version=3;"))
            {
                con.Open();
                string query = "SELECT * FROM Students WHERE Id = @Id";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Id", txtSearchId.Text);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtId.Text = reader["Id"].ToString();
                            txtFirstName.Text = reader["FirstName"].ToString();
                            txtLastName.Text = reader["LastName"].ToString();
                            dateBirth.Value = DateTime.Parse(reader["BirthDate"].ToString()); if (reader["Gender"].ToString() == "Male") radioMale.Checked = true;
                            else radioFemale.Checked = true;
                            txtPhone.Text = reader["Phone"].ToString();
                            txtAddress.Text = reader["Address"].ToString();

                            if (reader["Picture"] != DBNull.Value)
                            {
                                byte[] imgBytes = (byte[])reader["Picture"];
                                using (MemoryStream ms = new MemoryStream(imgBytes))
                                {
                                    pictureBoxStudent.Image = Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                pictureBoxStudent.Image = null;
                            }

                            MessageBox.Show("Student found.");
                        }
                        else
                        {
                            MessageBox.Show("No student found with that ID.");
                        }
                    }
                }
            }
        }
    }
}