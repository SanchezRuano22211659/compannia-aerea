﻿using Common;
using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentation.ChildForms
{
    public partial class FormUserProfile : Form
    {
        private UserModel conectedUser;//Obtiene o establece el usuario conectado actualmente.

        public FormUserProfile()
        {
            InitializeComponent();
        }

        private void FormUserProfile_Load(object sender, EventArgs e)
        {
            LoadUserData();
        }
        private void LoadUserData()
        {//Cargar datos del usuario activo.
            var userModel = new UserModel().GetUserById(ActiveUser.Id);//Obtener el usuario mediante el id del usuario activo.
            conectedUser = userModel;//Establecer el usuario conectado.

            lblFullname.Text = userModel.FullName;
            lblUsername.Text = userModel.usuario;
            lblName.Text = userModel.Nombres;
            lblLastName.Text = userModel.ApellidoP;
            lblMail.Text = userModel.Correo;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var verifyForm = new FormVerifyUser(conectedUser.contrasenia);//Control de seguridad, autenticar usuario conectado.
            DialogResult resultVR = verifyForm.ShowDialog();//Mostrar como ventana de dialogo y establecer el resultado.

            if (resultVR == System.Windows.Forms.DialogResult.OK)//Si el resultado de dialogo es OK, mostrar el formulario de actualizar usuario.
            {
                var userForm = new FormUserMaintenance(conectedUser, true);//Instanciar formulario de mantenimiento y enviar parametros necesarios (modelo de usuario y si la edicion de datos es de perfil de usuario (TRUE))
                DialogResult resultUF = userForm.ShowDialog();//Mostrar como ventana de dialogo y establecer el resultado.
                if (resultUF == System.Windows.Forms.DialogResult.OK)//Si el resultado de dialogo es OK, actualizar vista.
                {
                    LoadUserData();//Cargar datos de usuario.
                    MainForm mainForm = (MainForm)Application.OpenForms[1];//Obtener el formulario principal (El formulario login es [0], el formulario principal es [1], eso puede cambiar segun cuantos formularios se abrieron antes del formulario principal). 
                    mainForm.LoadUserData(conectedUser);//Actualizar vista del formulario principal.                    
                }
            }
           
        }

    }
}
