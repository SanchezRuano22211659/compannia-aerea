﻿using Common;
using Domain;
using Presentation.Helpers;
using Presentation.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Presentation.ChildForms
{
    public partial class FormUserMaintenance : BaseForms.BaseFixedForm
    {
        /// <summary>
        /// Esta clase hereda de la clase BaseFixedForm.
        /// </summary>
        /// 

        #region -> Campos

        private UserModel userModel;//Obtiene o establece el modelo de dominio de usuario.
        private bool userModify; //Obtiene o establece un usuario será editado.
        private int userId;//Obtiene o establece el id del usuario a editar.


        #endregion

        #region -> Constructores

        public FormUserMaintenance()
        {//Utilice este constructor cuando desee agregar un nuevo usuario.
            InitializeComponent();
            lblCaption.Text = "Agregar nuevo usuario";
            userModel = new UserModel();//Inicializar modelo de dominio de usuario.
            userModify = false;//Establecer userModify en falso.
            //PictureBoxPhoto.Image = defaultPhoto; //Establecer la foto predeterminada para los usuarios sin foto.
            cmbPosition.DataSource = Positions.GetPositions();//Establecer lista de cargos.
            cmbPosition.SelectedIndex = -1;//Sin seleccion de indice.
        }
        public FormUserMaintenance(UserModel _userModel, bool isUserProfile)
        {//Utilice este constructor cuando desee editar un usuario o actualizar el perfil de usuario.

            InitializeComponent();
            this.TitleBarColor = Color.OrangeRed;
            btnSave.BackColor = Color.OrangeRed;
            cmbPosition.DataSource = Positions.GetPositions();

            userModel = _userModel;//Establecer modelo de dominio de usuario.
            userModify = true;  //Establecer userModify en verdadero.
            FillFields();   //LLenar los campos del formulario con el modelo de usuario (Ver metodo).                 
            lblCaption.Text = "Modificar usuario";
        }
        #endregion

        #region -> Métodos

        private void FillFields()
        {//Cargar los datos del modelo en los campos del formulario.
            userId = userModel.Id;
            txtUsername.Text = userModel.usuario;
            txtPassword.Text = userModel.contrasenia;
            txtConfirmPass.Text = userModel.contrasenia;
            txtFirstName.Text = userModel.Nombres;
            txtLastName.Text = userModel.ApellidoP;
            textBox2.Text = userModel.ApellidoM;
            txtEmail.Text = userModel.Correo;
            textBox1.Text = userModel.Telefono;
            textBox7.Text = userModel.Cp.ToString();
            textBox5.Text = userModel.Calle;
            textBox4.Text = userModel.NumCalle.ToString();
            textBox6.Text = userModel.Colonia;
            textBox3.Text = userModel.CiudadId.ToString();
        }

        private void FillUserModel()
        {//LLenar modelo con los valores de los campos del formulario.
            userModel.Id = userId;
            userModel.usuario = txtUsername.Text;
            userModel.contrasenia = txtPassword.Text;
            userModel.Nombres = txtFirstName.Text;
            userModel.ApellidoP = txtLastName.Text;
            userModel.ApellidoM = textBox2.Text;
            userModel.Correo = txtEmail.Text;
            userModel.Telefono = textBox1.Text;
            userModel.Cp = int.Parse(textBox7.Text);
            userModel.Calle = textBox5.Text;
            userModel.NumCalle = int.Parse(textBox4.Text);
            userModel.Colonia = textBox6.Text;
            userModel.CiudadId = int.Parse(textBox3.Text);
        }

        private void Save()
        {//Guardar cambios.
            int result = -1;
            try
            {
                FillUserModel();//Obtener modelo de vista.
                var validateData = new DataValidation(userModel);//Validar campos del objeto.
                var validatePassword = txtPassword.Text == txtConfirmPass.Text;//Validar contraseñas.

                if (validateData.Result == true && validatePassword == true)//Si el objeto y contraseña es válido.
                {
                    //EDITAR USUARIO
                    if (userModify == true)
                    {
                        result = userModel.ModifyUser();//Invocar metodo ModifyUser del modelo de usuario.
                        if (result > 0)
                        {
                            MessageBox.Show("Usuario actualizado con éxito", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = System.Windows.Forms.DialogResult.OK;//Establecer Ok como resultado de dialogo del formulario.
                            this.Close();//Cerrar formulario
                        }
                        else
                        {
                            MessageBox.Show("No se realizó la operación, intente nuevamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    //AGREGAR USUARIO
                    else
                    {
                        result = userModel.CreateUser(); //Invocar metodo CreateUser del modelo de usuario.

                        if (result > 0)
                        {
                            MessageBox.Show("Usuario agregado con éxito", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.DialogResult = System.Windows.Forms.DialogResult.OK; //Establecer Ok como resultado de dialogo del formulario.
                            this.Close();//Cerrar formulario
                        }
                        else
                        {
                            MessageBox.Show("No se realizó la operación, intente nuevamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
                else //Si el objeto o contraseña NO es válido, mostrar mensaje segun el caso.
                {
                    if (validateData.Result == false)
                        MessageBox.Show(validateData.ErrorMessage, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    else
                        MessageBox.Show("Las contraseñas no coinciden", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                var message = ExceptionManager.GetMessage(ex);//Obtener mensaje de excepción.
                MessageBox.Show(message, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);//Mostrar mensaje.
            }
        }
        #endregion

        #region -> Metodos de evento

        private void FormUserMaintenance_Load(object sender, EventArgs e)
        {

        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();//Guardar cambios.
        }
        private void btnAddPhoto_Click(object sender, EventArgs e)
        {
            //Agregar una imagen al cuadro de imagen para la foto del usuario.
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Images(.jpg,.png)|*.png;*.jpg";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                //PictureBoxPhoto.Image = new Bitmap(openFile.FileName);
            }
        }

        private void btnDeletePhoto_Click(object sender, EventArgs e)
        {
            //Borrar foto del usuario
            //PictureBoxPhoto.Image = defaultPhoto;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
